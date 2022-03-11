using MVCNotesSaver.DataLogic.Common;
using MVCNotesSaver.Models.User;

namespace MVCNotesSaver.DataLogic;

public class NpgsqlRepository : IRepository
{
    private readonly MainDbContext _context;
    public NpgsqlRepository(MainDbContext context)
    {
        _context = context;
    }
    public async Task<UserViewModel?> GetUserAsync(string email, string password)
    {
        var user = await _context.GetUser(email, password);
        return user?.ToView();
    }

    public async Task<int> CreateUserAsync(string name, string email, string password)
    {
        return await _context.CreateUser(email, name, password);
    }

    public async Task<List<NoteViewModel>> GetNotesAsync(int userId)
    {
        List<NoteViewModel> list = new List<NoteViewModel>();
        (await _context.GetNotes(userId)).ForEach(entity => list.Add(entity.ToView()));
        return list;
    }

    public async Task<int> CreateNoteAsync(int userId, string text)
    {
        return await _context.CreateNote(userId, text);
    }
}