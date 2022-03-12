using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using MVCNotesSaver.DataLogic;
using MVCNotesSaver.DataLogic.Common.Exceptions;
using MVCNotesSaver.Defaults;
using MVCNotesSaver.Models.User;

namespace MVCNotesSaver.Controllers;

[Authorize]
public class AccountController : Controller
{
    private readonly IRepository _repository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AccountController> _logger;
    public AccountController(IRepository repository, IConfiguration configuration, ILogger<AccountController> logger)
    {
        _logger = logger;
        _repository = repository;
        _configuration = configuration;
    }
    
    
    [HttpGet]
    [AllowAnonymous]
    public IActionResult SignIn()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SignIn(LoginViewModel loginData)
    {
        if (!ModelState.IsValid)
            return View(loginData);
        
        var user = await _repository.GetUserAsync(loginData.Email, loginData.Password);

        if (user == null)
        {
            loginData.UserDoesntExist = true;
            return View(loginData);
        }
        
        HttpContext.Response.Cookies.Append("Authentication.Token", GenerateJwtToken(user), new CookieOptions
        {
            MaxAge = TimeSpan.FromMinutes(30)
        });
        return RedirectToAction("Index", "Home");
    }

    private string GenerateJwtToken(UserViewModel user)
    {
        var defaults = new AuthDefaults(_configuration);
        var credentials = new SigningCredentials(defaults.SymmetricSecurityKey, SecurityAlgorithms.HmacSha256);
        var claims = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Sub, user.Name),
            new Claim(JwtRegisteredClaimNames.Jti, user.Id.ToString())
        };
        var token = new JwtSecurityToken(
            issuer: defaults.Issuer,
            audience: defaults.Audience,
            claims: claims,
            signingCredentials: credentials,
            expires: DateTime.Now.AddMinutes(20)
        );
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult SignUp(string returnUrl)
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SignUp(RegisterViewModel registerData)
    {
        if(!ModelState.IsValid)
            return View(registerData);


        try
        {
            await _repository.CreateUserAsync(registerData.Name, registerData.Email, registerData.Password);
        }
        catch (EmailIsAlreadyTakenException e)
        {
            ModelState.AddModelError("Email", "Email is already taken.");
            return View(registerData);
        }

        return RedirectToAction("Index", "Home");
    }

    [Authorize]
    public IActionResult Sign_out()
    {
        HttpContext.Response.Cookies.Delete("Authentication.Token");
        return Redirect("/");
    }
}