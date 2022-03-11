using MVCNotesSaver.Models.User;

namespace MVCNotesSaver.DataLogic;

public interface IRepository
{
    public Task<UserViewModel?> GetUserAsync(string email, string password);
    public Task<int> CreateUserAsync(string name, string email, string password);
    public Task<List<NoteViewModel>> GetNotesAsync(int userId);
    public Task<int> CreateNoteAsync(int userId, string text);
}