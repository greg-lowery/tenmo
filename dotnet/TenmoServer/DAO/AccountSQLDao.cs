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

             
        
    }
}
