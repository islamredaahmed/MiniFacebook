using Microsoft.Build.Framework;

namespace FecebookAPI.Models
{
    public class PostModel
    {
        public Guid Id { get; set; }
        public string Text { get; set; }
        public byte[]? Image { get; set; }
        public string? Username { get; set; }

    }
}
