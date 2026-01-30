namespace _231044K_FreshFarmMarket.Models
{
    public class GoogleReCaptchaOptions
    {
        public string SiteKey { get; set; } = "";
        public string SecretKey { get; set; } = "";
        public double MinimumScore { get; set; } = 0.5;
    }
}
