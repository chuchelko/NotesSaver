namespace MVCNotesSaver.DataLogic.Entities;

public class NoteEntity
{
    public int Id { get; init; }
    public string Text { get; init; }
    public DateTime Created { get; init; }
}