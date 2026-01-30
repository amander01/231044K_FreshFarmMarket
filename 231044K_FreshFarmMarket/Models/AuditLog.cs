using System.ComponentModel.DataAnnotations;

namespace _231044K_FreshFarmMarket.Models
{
    public class AuditLog
    {
        public int Id { get; set; }

        [StringLength(450)]
        public string? UserId { get; set; }

        [Required, StringLength(40)]
        public string Action { get; set; } = ""; // LOGIN_SUCCESS, LOGIN_FAIL, LOGOUT

        [StringLength(300)]
        public string? Detail { get; set; }

        [StringLength(60)]
        public string? IpAddress { get; set; }

        public DateTime CreatedUtc { get; set; } = DateTime.UtcNow;
    }
}
