using System.ComponentModel.DataAnnotations.Schema;

namespace FecebookAPI.Entities
{
    public class Post
    {
        public Guid Id { get; set; }
        public string text { get; set; }
        public byte[]? image { get; set; }


        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}
