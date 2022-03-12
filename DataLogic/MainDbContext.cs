using System.Data;
using MVCNotesSaver.DataLogic.Common.Exceptions;
using MVCNotesSaver.DataLogic.Entities;
using Npgsql;
using NpgsqlTypes;

namespace MVCNotesSaver.DataLogic;

public class MainDbContext
{

    private string ConnectionString => Environment.GetEnvironmentVariable("DB_CONNECTION_STRING") ??
                                          throw new EnvironmentException("Db connection string is empty");
    
    public async Task<UserEntity?> GetUser(string email, string password)
    {
        const string query = @"
            select UserId, UserName, UserEmail, UserPasswordHash, UserPasswordSalt from Users
            where Users.UserEmail = ($1);
        ";
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(query, connection)
        {
            Parameters =
            {
                new NpgsqlParameter {Value = email}
            }
        };
        await using var reader = await command.ExecuteReaderAsync();
        
        if (await reader.ReadAsync())
        {
            var userEntity = new UserEntity
            {
                Id = reader.GetInt32("UserId"),
                Name = reader.GetString("UserName"),
                Email = reader.GetString("UserEmail"),
                Password = password
            };
            var hash = reader.GetString("UserPasswordHash");
            var salt = reader.GetString("UserPasswordSalt");
            if (PasswordManager.VerifyPassword(password, hash, salt))
                return userEntity;
        }

        return null;
    }

    /// <returns>Id of the created User</returns>
    /// <exception cref="ArgumentException">Throws when one or more arguments are incorrect.</exception>
    /// <exception cref="EmailIsAlreadyTakenException">Throws when there are users with this email.</exception>
    public async Task<int> CreateUser(string email, string name, string password)
    {
        const string findEmailsQuery = @"
            select count(UserId) from Users where UserEmail = ($1);
        ";   
        const string createUserQuery = @"
            insert into Users (UserName, UserEmail, UserPasswordHash, UserPasswordSalt) 
            values (($1), ($2), ($3), ($4)) returning UserId;
        ";
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();
        
        await using var findEmailsCommand = new NpgsqlCommand(findEmailsQuery, connection);
        findEmailsCommand.Parameters.AddWithValue(email);
        var emailsCount = (Int64) (await findEmailsCommand.ExecuteScalarAsync())!;
        if (emailsCount > 0)
            throw new EmailIsAlreadyTakenException("Email is already taken.");

        PasswordManager.HashPassword(password, out byte[] hash, out byte[] salt);
        await using var createUserCommand = new NpgsqlCommand(createUserQuery, connection)
        {
            Parameters =
            {
                new NpgsqlParameter {Value = name},
                new NpgsqlParameter {Value = email},
                new NpgsqlParameter {Value = Convert.ToBase64String(hash)},
                new NpgsqlParameter {Value = Convert.ToBase64String(salt)}
            }
        };
        var id = (int) (await createUserCommand.ExecuteScalarAsync() ?? throw new ArgumentException("User doesn't exist."));
        return id;
    }

    public async Task<List<NoteEntity>> GetNotes(int userId)
    {
        const string query = @"
            select NoteId, NoteText, NoteCreatedTime from Notes where Notes.UserId=(@userId);
        ";
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@userId", userId);
        var reader = await command.ExecuteReaderAsync();

        if (!reader.HasRows) return new List<NoteEntity>();
        
        var list = new List<NoteEntity>();
        while(reader.Read())
            list.Add(new NoteEntity
            {
                Id = reader.GetInt32(0),
                Created = reader.GetDateTime(2),
                Text = reader.GetString(1)
            });
        return list;
    }
    
    /// <returns>Id of the created note.</returns>
    /// <exception cref="InvalidOperationException">Throws when one or more arguments are incorrect.</exception>
    public async Task<int> CreateNote(int userId, string text)
    {
        const string query = @"
            insert into Notes (UserId, NoteText, NoteCreatedTime) values ((@userId), (@text), (@createdAt)) returning NoteId;
        ";
        await using var connection = new NpgsqlConnection(ConnectionString);
        await connection.OpenAsync();
        await using var command = new NpgsqlCommand(query, connection);
        command.Parameters.AddWithValue("@userId", userId);
        command.Parameters.AddWithValue("@text", text);
        command.Parameters.AddWithValue("@createdAt", NpgsqlDbType.Timestamp, DateTime.Now);
        var id = (int)(await command.ExecuteScalarAsync() ?? throw new InvalidOperationException());
        return id;
    }
}