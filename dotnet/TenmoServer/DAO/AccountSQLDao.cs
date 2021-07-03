using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;
using TenmoServer.Models;

namespace TenmoServer.DAO
{
    public class AccountSQLDao : IAccountDao
    {
        private readonly string connectionString;

        public AccountSQLDao (string connString)
        {
            connectionString = connString;
        }

        public Account GetAccount(int userId)
        {
            Account account = new Account();

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT balance, account_id, user_id FROM accounts WHERE user_id = @user_id; ", conn);
                    cmd.Parameters.AddWithValue("@user_id", userId);


                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        account.Balance = Convert.ToDecimal(reader["balance"]);
                        account.AccountId = Convert.ToInt32(reader["account_id"]);
                        account.UserId = Convert.ToInt32(reader["user_id"]);


                    }
                    else
                    {
                        account.Balance = -1;
                    }
                }
            }
            catch (SqlException)
            {
                return null;
            }

            return account;
        }

        public bool? VerifyAccountExists(Account accountToVerify)
        {
            bool accountExists = false;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT account_id FROM accounts WHERE account_id = @account_id;", conn);
                    cmd.Parameters.AddWithValue("@account_id", accountToVerify.AccountId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        int? accountIdFromDatabase = Convert.ToInt32(reader["account_id"]);

                        if (accountIdFromDatabase != null)
                        {
                            accountExists = true;
                        }
                    }
                }
            }
            catch (SqlException)
            {
                return null;
            }

            return accountExists;
        }

        public bool? VerifySufficientFunds(int userId, decimal amtToTransfer)
        {
            bool sufficientFunds = false;
            decimal userAccountBalance = 0;

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("SELECT balance FROM accounts WHERE user_id = @user_id;", conn);
                    cmd.Parameters.AddWithValue("@user_id", userId);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        userAccountBalance = Convert.ToDecimal(reader["balance"]);
                    }
                }
            }
            catch (SqlException)
            {
                return null;
            }

            if (userAccountBalance >= amtToTransfer)
            {
                sufficientFunds = true;
            }

            return sufficientFunds;
        }

        public Transfer ProcessAccountTransfer(Transfer newTransfer, int requesterUserId)
        {
            Transfer successfulTransfer = new Transfer();
            successfulTransfer.TransferAmount = newTransfer.TransferAmount;
            successfulTransfer.AccountTo = newTransfer.AccountTo;
            

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    //TODO use subqueries for the transfer type and transfer status in case those ints change in the database
                        //Should we be re-verifying the destination account exists/is valid and that the user has sufficient funds within the body of this SQL transaction?
                        //Those were handled separately earlier on...
                    SqlCommand cmd = new SqlCommand($"BEGIN TRANSACTION; UPDATE accounts SET balance = balance - @transferAmount " +
                                                    $"WHERE account_id = (SELECT account_id FROM accounts WHERE user_id = @requesterUserId); " +
                                                    $"UPDATE accounts SET balance = balance + @transferAmount WHERE account_id = @accoutTo; COMMIT; " +
                                                    $"INSERT INTO transfers (transfer_type_id, transfer_status_id, account_from, account_to, amount) " +
                                                    $"VALUES(2, 2, (SELECT account_id FROM accounts WHERE user_id = @requesterUserId), @accoutTo, @transferAmount); " +
                                                    $"SELECT @@IDENTITY AS [successfulTransfer]; COMMIT; ", conn);
                    cmd.Parameters.AddWithValue("@transferAmount", newTransfer.TransferAmount);
                    cmd.Parameters.AddWithValue("@requesterUserId", requesterUserId);
                    cmd.Parameters.AddWithValue("@accoutTo", newTransfer.AccountTo);

                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        int successfulTransferId = Convert.ToInt32(reader["successfulTransfer"]);
                        successfulTransfer.TransferId = successfulTransferId;
                    }
                }
            }
            catch (SqlException)
            {
                return null;
            }

            return successfulTransfer;
        }


    }
}
