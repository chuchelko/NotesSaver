using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MVCNotesSaver.DataLogic;

namespace MVCNotesSaver.Controllers;

[Authorize]
public class NotesController : Controller
{
    private readonly IRepository _repository;

    public NotesController(IRepository repository)
    {
        _repository = repository;
    }

    private int UserId => int.Parse(User.Claims.First(c => c.Type == JwtRegisteredClaimNames.Jti).Value);

    [HttpGet]
    public async Task<IActionResult> Index()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Index(string noteText)
    {
        await _repository.CreateNoteAsync(UserId, noteText);
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetNotes()
    {
        var notes = await _repository.GetNotesAsync(UserId);
        return Json(notes);
    }
}