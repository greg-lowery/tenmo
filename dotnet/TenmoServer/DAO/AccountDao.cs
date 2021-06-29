using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.SqlClient;

namespace TenmoServer.DAO
{
    public class AccountDao : IAccountDao
    {
        private readonly string connectionString;

        public AccountDao (string connString)
        {
            connectionString = connString;
        }

        public decimal GetAccountBalance(int userId)
        {
            decimal accountBalance = 0;

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlCommand cmd = new SqlCommand("SELECT balance FROM accounts WHERE user_id = @user_id; ", conn);
                cmd.Parameters.AddWithValue("@user_id", userId);

                SqlDataReader reader = cmd.ExecuteReader();

                if (reader.Read())
                {
                    accountBalance = Convert.ToDecimal(reader["balance"]);
                }
            }
            return accountBalance;
        }

             
        
    }
}
