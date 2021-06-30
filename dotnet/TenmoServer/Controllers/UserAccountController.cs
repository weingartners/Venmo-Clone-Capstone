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

        //[HttpGet("account/{id}")]
        //public ActionResult<List<Account>> GetAccounts(int id)
        //{
        //    var accounts = accountDao.GetAccounts(id);

        //    return Ok(accounts);
        //}

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
    }

    
}
