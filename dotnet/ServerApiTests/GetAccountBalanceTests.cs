using Microsoft.VisualStudio.TestTools.UnitTesting;
using TenmoServer.Models;
using RestSharp;

namespace ServerApiTests
{
    [TestClass]
    public class GetAccountBalanceTests
    {
        private readonly static string API_BASE_URL = "https://localhost:44315/";
        private readonly IRestClient client = new RestClient();
        private readonly string testToken = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMDAxIiwibmFtZSI6ImdyZWciLCJuYmYiOjE2MjUwNjAzNzcsImV4cCI6MTYyNTY2NTE3NywiaWF0IjoxNjI1MDYwMzc3fQ.2BATzfDYV5N20TZoNIuM3KUeaVToXYPI7AVieVaWQIg";

        [TestMethod]
        public void GetAccountBalance_for_existing_acccount_returns_account()
        {
            //Arrange
            RestRequest request = new RestRequest(API_BASE_URL + "/api/accounts", DataFormat.Json);

            int expectedUserId = 1001;
            int expectedAccountId = 2001;
            decimal expectedBalance = 1000.00m;

            //Act
            IRestResponse<Account> response = client.Get<Account>(request);

            Account returnedAccount = response.Data;

            int actualUserId = returnedAccount.UserId;
            int actualAccountId = returnedAccount.AccountId;
            decimal? actualBalance = returnedAccount.Balance;

            //Assert
            Assert.AreEqual(expectedUserId, actualUserId, "User ID returned did not match the expected value.");
            Assert.AreEqual(expectedAccountId, actualAccountId, "Account ID returned did not match the expected value.");
            Assert.AreEqual(expectedBalance, actualBalance, "Account Balance returned did not match the expected value.");
        }
    }
}
