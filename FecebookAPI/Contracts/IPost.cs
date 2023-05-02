using FecebookAPI.Models;
using Microsoft.AspNetCore.Mvc;

namespace FecebookAPI.Contracts
{
    public interface IPost
    {
        Responce<PostModel> GetAll();
        Responce<PostModel> AddPost(PostModel model);
        Responce<PostModel> DeletePost(Guid id);
    }
}
