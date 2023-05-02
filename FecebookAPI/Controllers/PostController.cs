using FecebookAPI.Contracts;
using FecebookAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FecebookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PostController : ControllerBase
    {
        private readonly IPost _service;

        public PostController(IPost service)
        {
            _service = service;
        }

        [HttpPost("addPost")]
        public ActionResult AddPost([FromBody] PostModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = _service.AddPost(model);

            return Ok(result);
        }

        [HttpPost("deletePost")]
        public ActionResult DeletePost(Guid id)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = _service.DeletePost(id);

            return Ok(result);
        }

        [HttpGet]
        public ActionResult GetAllPosts()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var result = _service.GetAll();

            return Ok(result);
        }



    }
}
