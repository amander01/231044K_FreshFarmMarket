namespace _231044K_FreshFarmMarket.Services
{
    public interface IReCaptchaService
    {
        // Preferred
        Task<bool> VerifyTokenAsync(string token, string action);

        // Backward compatible (if your Register.cshtml.cs calls VerifyAsync)
        Task<bool> VerifyAsync(string token, string action);
    }
}
