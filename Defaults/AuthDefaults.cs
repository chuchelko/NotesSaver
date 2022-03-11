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
    private string Key => _configuration.GetSection("Auth").GetSection("Key").Value;
    public SymmetricSecurityKey SymmetricSecurityKey => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Key));
}