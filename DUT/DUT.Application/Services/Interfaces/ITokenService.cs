namespace DUT.Application.Services.Interfaces
{
    public interface ITokenService
    {
        Task<string> GetUserTokenAsync(int userId);
    }
}
