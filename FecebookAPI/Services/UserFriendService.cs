using FecebookAPI.Contracts;
using FecebookAPI.Data;
using FecebookAPI.Entities;
using FecebookAPI.ExceptionHandler;
using FecebookAPI.Log;
using FecebookAPI.Models;
using Microsoft.Extensions.Localization;

namespace FecebookAPI.Services
{
    public class UserFriendService : IUserFriend
    {
        private readonly CoreContext _context;
        private readonly ILoggerManager _logger;
        private readonly IAuth _auth;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public UserFriendService(CoreContext context, ILoggerManager logger, IAuth auth, IStringLocalizer<SharedResource> localizer)
        {
            _context = context;
            _logger = logger;
            _auth = auth;
            _localizer = localizer;
        }

        public Responce<bool> AddFriend(string friendUserName)
        {
            var userId = _auth.GetCurrentUser().UserId;

            if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(friendUserName))
            {
                throw new CustomValidationException(string.Format(_localizer["UserId and FrindId are Required"]));
            }

            var user = _context.Users.Any(x => x.Id == userId || x.UserName == friendUserName);
            if (!user)
                throw new CustomValidationException(string.Format(_localizer["User not found"]));

            var friendId = _context.Users.FirstOrDefault(x => x.UserName == friendUserName)?.Id;
            if (friendId is null)
                throw new CustomValidationException(string.Format(_localizer["Friend Name not found"]));

            var checkUserFriends = _context.UserFriends.Any(x => x.UserId == userId && x.FriendId == Guid.Parse(friendId));
            if (checkUserFriends)
                throw new CustomValidationException(string.Format(_localizer["You are already friends"]));

            UserFriend entity = new UserFriend();
            entity.UserId = userId;
            entity.FriendId = Guid.Parse(friendId);

            _context.UserFriends.Add(entity);
            _context.SaveChanges();
            _logger.LogInfo($"You {userId} are added new friend with id {friendId}");

            return new Responce<bool>(System.Net.HttpStatusCode.OK, string.Format(_localizer["your friend  has been added"]));
        }
    }
}
