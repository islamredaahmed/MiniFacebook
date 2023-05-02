using Microsoft.EntityFrameworkCore;

namespace FecebookAPI.Entities
{
    [Owned]
    public class OTPUser
    {
        public string OTP { get; set; }
        public DateTime ExpiresOn { get; set; }
        public bool IsExpired => DateTime.UtcNow >= ExpiresOn;
        public DateTime CreatedOn { get; set; }
        public DateTime? RevokedOn { get; set; }
        public bool IsActive => RevokedOn == null && !IsExpired;
    }
}
