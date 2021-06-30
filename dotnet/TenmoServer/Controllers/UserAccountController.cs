using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TenmoServer.DAO;
using TenmoServer.Models;
using TenmoServer.Security;
using System.Collections.Generic;
namespace TenmoServer.Controllers
{
    [Route("/")]
    [ApiController]
    [Authorize]
    public class UserAccountController : ControllerBase
    {
        private readonly IUserDao userDao;
        private readonly IAccountDao accountDao;

        public UserAccountController(IUserDao _userDao, IAccountDao _accountDao)
        {
            userDao = _userDao;
            accountDao = _accountDao;
        }

        [HttpGet("account/{id}/balance")]
        public ActionResult<List<decimal>> GetBalances(int id)
        {

            var balance = accountDao.GetBalances(id);
            if (balance != null)
            {
                return Ok(balance);
            }
            return NotFound();
        }

        [HttpPut("transfer/{transfer}")]
        public ActionResult<Transfer> TransferMoney(Transfer transfer)
        {
            userDao.SendMoney(transfer.SendingUserId, transfer.TransferAmount);
            userDao.RecieveMoney(transfer.RecievingUserId, transfer.TransferAmount);

            if (transfer != null)
            {
                return Ok(transfer);
            }
            return NotFound();
            
        }

        [AllowAnonymous]
        [HttpGet("users")]
        public ActionResult<List<User>> GetUsers()
        {
            var users = userDao.GetUsers();
            if (users != null)
            {
                return Ok(users);
            }
            return NotFound();
        }

        [AllowAnonymous]
        [HttpGet("usernames")]
        public ActionResult<List<string>> GetUsernames()
        {
            var usernames = userDao.GetUsernames();
            if (usernames != null)
            {
                return Ok(usernames);
            }
            return NotFound();
        }
    }

    
}
