using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace MVCNotesSaver.Defaults;

public class AuthDefaults
{
    private readonly IConfiguration _configuration;
    public AuthDefaults(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    public string Issuer => _configuration.GetSection("Auth").GetSection("Issuer").Value;
    public string Audience => _configuration.GetSection("Auth").GetSection("Audience").Value;
    private string Key => Environment.GetEnvironmentVariable("KEY") ?? throw new InvalidOperationException();
    public SymmetricSecurityKey SymmetricSecurityKey => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
}