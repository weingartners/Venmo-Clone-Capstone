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
        private readonly ITransferDao transferDao;
        public UserAccountController(IUserDao _userDao, IAccountDao _accountDao, ITransferDao _transferDao)
        {
            userDao = _userDao;
            accountDao = _accountDao;
            transferDao = _transferDao;
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
            userDao.RecieveMoney(transfer.ReceivingUserId, transfer.TransferAmount);

            if (transfer != null)
            {
                return Ok(transfer);
            }
            return NotFound();
            
        }
        [HttpPost("savetransfer")]
        public ActionResult<Transfer> SaveTransfer(Transfer transfer)
        {
            transferDao.SaveTransfer(transfer.SendingUserId, transfer.TransferAmount, transfer.ReceivingUserId, transfer.TypeId, transfer.StatusId);
            if (transfer != null)
            {
                return Ok(transfer);
            }
            return NotFound();
        }
        [HttpGet ("transfer/{id}")]
        public ActionResult<List<Transfer>> GetTransfersById(int id)
        {
            var transfers = transferDao.GetTransfersById(id);

            if (transfers != null)
            {
                return Ok(transfers);
            }
            return NotFound();
        }

        [HttpGet ("requests/{id}")]
        public ActionResult<List<Transfer>> GetRequests(int id)
        {
            var transfers = transferDao.GetRequests(id);

            if (transfers != null)
            {
                return Ok(transfers);
            }
            return NotFound();
        }

        [HttpPut ("updatestatus/{transferId}")]
        public ActionResult<Transfer> UpdateStatus(Transfer transfer)
        {
            transferDao.UpdateStatus(transfer.TransferId, transfer.StatusId);

            return Ok();
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
