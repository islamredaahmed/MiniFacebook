namespace FecebookAPI.Entities
{
    public class UserFriend
    {
        public Guid Id { get; set; }
        public Guid FriendId { get; set; }
        public string UserId { get; set; }
        public virtual ApplicationUser User { get; set; }
    }
}