using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public interface IAccountDao
    {
        //also require account ID? one user, multiple accounts?
        public Account GetAccount(int userId);

        public bool? VerifyAccountExists(Account accountToVerify);

        public bool? VerifySufficientFunds(int userId, decimal amtToTransfer);
    }
}
