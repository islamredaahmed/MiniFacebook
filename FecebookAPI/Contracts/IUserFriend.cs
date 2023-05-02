using FecebookAPI.Models;

namespace FecebookAPI.Contracts
{
    public interface IUserFriend
    {
        Responce<bool> AddFriend(string friendUserName);
    }
}
