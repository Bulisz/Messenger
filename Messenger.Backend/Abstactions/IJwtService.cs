using Messenger.Backend.Models;
using Messenger.Backend.Models.AuthDTOs;

namespace Messenger.Backend.Abstactions
{
    public interface IJwtService
    {
        void ClearRefreshToken(string refreshToken);
        Task<TokensDTO> CreateTokensAsync(ApplicationUser user);
        Task<TokensDTO> RenewTokensAsync(string refreshToken);
    }
}