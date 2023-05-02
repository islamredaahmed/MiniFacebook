using FecebookAPI.Contracts;
using FecebookAPI.Data;
using FecebookAPI.Entities;
using FecebookAPI.ExceptionHandler;
using FecebookAPI.Log;
using FecebookAPI.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using System.Linq;

namespace FecebookAPI.Services
{
    public class PostService : IPost
    {
        private readonly CoreContext _context;
        private readonly ILoggerManager _logger;
        private readonly IAuth _auth;
        private readonly IStringLocalizer<SharedResource> _localizer;

        public PostService(CoreContext context, ILoggerManager logger, IAuth auth, IStringLocalizer<SharedResource> localizer)
        {
            _context = context;
            _logger = logger;
            _auth = auth;
            _localizer = localizer;
        }

        public Responce<PostModel> DeletePost(Guid id)
        {
            if (id.Equals(Guid.Empty) || id == new Guid())
                throw new CustomValidationException(string.Format(_localizer["PostId is Required"]));

            var post = _context.Posts.Find(id);
            if (post is null)
                throw new CustomValidationException(string.Format(_localizer["Post not found"]));

            _context.Posts.Remove(post);
            _context.SaveChanges();

            return new Responce<PostModel>(System.Net.HttpStatusCode.OK, string.Format(_localizer["your post  has been deleted"]));
        }

        public Responce<PostModel> GetAll()
        {
            List<PostModel> Posts = new List<PostModel>();

            var userId = _auth.GetCurrentUser().UserId;

            var frindIds = _context.UserFriends.Where(f => f.UserId == userId).Select(x => x.FriendId.ToString()).ToList();
            var data = _context.Posts.Include(i => i.User.UserFriends)
                .Where(x => frindIds.Contains(x.UserId) || x.UserId == userId).ToList();

            foreach (var post in data)
            {
                PostModel postModel = new PostModel();
                postModel.Id = post.Id;
                postModel.Text = post.text;
                postModel.Image = post.image;
                postModel.Username = post.User.UserName;
                Posts.Add(postModel);
            }

            return new Responce<PostModel>(System.Net.HttpStatusCode.OK, Posts);
        }

        public Responce<PostModel> AddPost(PostModel model)
        {
            var userId = _auth.GetCurrentUser().UserId;
            _logger.LogInfo($"Add New Post for userid = {userId}");

            if (userId.Equals(Guid.Empty) || string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(model.Text))
            {
                throw new CustomValidationException(string.Format(_localizer["userId and text are Required"]));
            }

            var post = new Post()
            {
                Id = Guid.NewGuid(),
                text = model.Text,
                image = model.Image,
                UserId = userId,
            };
            _context.Posts.Add(post);
            _context.SaveChanges();

            return new Responce<PostModel>(System.Net.HttpStatusCode.OK, string.Format(_localizer["your post has been added"]));
        }

    }
}
