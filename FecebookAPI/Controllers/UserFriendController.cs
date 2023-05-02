using FecebookAPI.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FecebookAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class UserFriendController : ControllerBase
    {
        private readonly IUserFriend _service;

        public UserFriendController(IUserFriend service)
        {
            _service = service;
        }

        [HttpPost("addFriend")]
        public ActionResult AddFriend([FromBody] string friendUserName)
        {
            //if (!ModelState.IsValid)
            //    return BadRequest(ModelState);

            _service.AddFriend(friendUserName);

            return Ok(new { message = "your friend  has been added" });
        }

    }
}
