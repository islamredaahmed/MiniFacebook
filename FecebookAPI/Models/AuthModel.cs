using System.Text.Json.Serialization;

namespace FecebookAPI.Models
{
    public class AuthModel
    {
        public string? Message { get; set; }
        
        public string? Token { get; set; }

        [JsonIgnore]
        public bool IsAuthenticated { get; set; }
        [JsonIgnore]
        public string? RefreshToken { get; set; }
        [JsonIgnore]
        public DateTime RefreshTokenExpiration { get; set; }
    }

}
