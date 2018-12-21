using gol.collation.core.RequestModels;
using System.Threading.Tasks;

namespace gol.collation.domain.Interfaces
{
    public interface IAuthService
    {
        Task<bool> LoginWithPasswordAsync(PasswordLoginRequest loginRequest);
    }
}
