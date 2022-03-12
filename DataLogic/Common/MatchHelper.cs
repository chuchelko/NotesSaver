using MVCNotesSaver.DataLogic.Entities;
using MVCNotesSaver.Models.User;

namespace MVCNotesSaver.DataLogic.Common;

public static class MatchHelper
{
    public static UserViewModel ToView(this UserEntity userEntity)
    {
        return new UserViewModel
        {
            Email = userEntity.Email,
            Id = userEntity.Id,
            Name = userEntity.Name,
            Password = userEntity.Password
        };
    }

    public static NoteViewModel ToView(this NoteEntity noteEntity)
    {
        return new NoteViewModel
        {
            Created = noteEntity.Created,
            Text = noteEntity.Text,
            Id = noteEntity.Id
        };
    }
}