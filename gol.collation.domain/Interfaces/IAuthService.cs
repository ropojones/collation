using gol.collation.core.Entities;
using gol.collation.core.RequestModels;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace gol.collation.domain.Interfaces
{
    public interface IAuthService
    {
        Task<bool> LoginAsync(LoginRequest loginRequest);
        Task<ApiUser> AuthenticateApiUserAsync(LoginRequest request);
        Task<IList<Claim>> GetClaimsAsync(ApiUser user);
    }
}
