using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.DAO;
using TenmoServer.Models;

namespace TenmoServer.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountDao _dao;

        public AccountController(IAccountDao _accountDao)
        {
            _dao = _accountDao;
        }

        //URL: SERVERURL/api/account
        //GET: account balance
        [HttpGet]
        public ActionResult<Account> GetAccountBalance()
        {
            int userId = Convert.ToInt32(User.FindFirst("sub")?.Value);

            Account account = _dao.GetAccount(userId);

            if (account == null)
            {
                return StatusCode(500, "Internal Server Error. Please try again later.");
            }
            else if (account.Balance == -1)
            {
                return NotFound();
            }

            return Ok(account);
        }

        [HttpPost("verify/account")]
        public ActionResult VerifyTransferAccountExists(Account accountToVerify)
        {
            int accountNumberToVerify = accountToVerify.AccountId;

            bool? accountExists = _dao.VerifyAccountExists(accountToVerify);

            if (accountExists == null)
            {
                return StatusCode(500, "Internal Server Error. Please try again later.");
            }
            else if (accountExists == false)
            {
                return NotFound();
            }
            return Ok();
        }

        [HttpPost("verify/funds")]
        public ActionResult VerifySufficientFunds(string amtToTransferString)
        {
            int userId = Convert.ToInt32(User.FindFirst("sub")?.Value);
            decimal amtToTransfer = Convert.ToDecimal(amtToTransferString);

            bool? sufficientFunds = _dao.VerifySufficientFunds(userId, amtToTransfer); 

            if (sufficientFunds == null)
            {
                return StatusCode(500, "Internal Server Error. Please try again later.");
            }
            else if (sufficientFunds == false)
            {
                return BadRequest("Insufficient Funds to Transfer");
            }
            return Ok();
        }



        
        //URL: SERVERURL/api/account/transfer
        [HttpPost("transfer")]
        public ActionResult<Transfer> ProcessAccountTransfer(Transfer newTransfer)
        {
            int requesterUserId = Convert.ToInt32(User.FindFirst("sub")?.Value);

            Transfer successfulTransfer = _dao.ProcessAccountTransfer(newTransfer, requesterUserId);

            if (successfulTransfer == null)
            {
                return StatusCode(500, "Internal Server Error. Please try again later.");
            }
            else if (successfulTransfer.TransferId == 0)
            {
                return StatusCode(500, "There was an Internal Error while processing your request. Please contact customer support.");
            }
            return Ok(successfulTransfer);
        }
    }
}
