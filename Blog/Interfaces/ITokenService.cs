using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Blog.Interfaces
{
    public interface ITokenService
    {
        string CreateToken(IdentityUser user);
    }
}