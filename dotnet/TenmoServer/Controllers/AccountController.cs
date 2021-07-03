using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.DAO;
using TenmoServer.Models;


// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

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


        [HttpGet]
        public ActionResult<Account> VerifyTransferAccountExists()  //build out to verify account exists
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









        // GET api/<AccountController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }

        // POST api/<AccountController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<AccountController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<AccountController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
