using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using MVCNotesSaver.DataLogic;
using MVCNotesSaver.Models;

namespace MVCNotesSaver.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IRepository _repository;
    public HomeController(ILogger<HomeController> logger, IRepository repository)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        if (User.Identity!.IsAuthenticated)
        {
            int id = int.Parse(User.Claims.FirstOrDefault(claim =>
                claim.Type == JwtRegisteredClaimNames.Jti)!.Value);
            ViewBag.NotesCount = await _repository.GetNotesCountAsync(id);
        }
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel {RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier});
    }
}