using System.Security.Cryptography;

namespace MVCNotesSaver.DataLogic;

public static class PasswordManager
{
    public static void HashPassword(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
        using var hash = new HMACSHA256();
        passwordSalt = hash.Key;
        passwordHash = hash.ComputeHash(Convert.FromBase64String(password));
    }

    public static bool VerifyPassword(string password, string passwordHash, string passwordSalt)
    {
        using var hash = new HMACSHA256(Convert.FromBase64String(passwordSalt));
        byte[] hashedPassword = hash.ComputeHash(Convert.FromBase64String(password));
        return hashedPassword.SequenceEqual(Convert.FromBase64String(passwordHash));
    }
}