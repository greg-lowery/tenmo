using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp;
using RestSharp.Authenticators;

namespace ServerApiTests
{
    [TestClass]
    public class GetAccountBalanceTests
    {
        private readonly static string API_BASE_URL = "https://localhost:44315/api/account/";
        private readonly IRestClient client = new RestClient();
        private readonly string testTokenForExistingAccount = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMDAxIiwibmFtZSI6ImdyZWciLCJuYmYiOjE2MjUwNjIzMzYsImV4cCI6MTYyNTY2NzEyOCwiaWF0IjoxNjI1MDYyMzI4fQ.tWBGAPU18JOEncO_zmHe70YopH_I94MDBNQOt8GWh9w";
        private readonly string testTokenForNonExistentAccount = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMDAyIiwibmFtZSI6ImFhcm9uIiwibmJmIjoxNjI1MDYzNDEwLCJleHAiOjE2MjU2NjgyMDAsImlhdCI6MTYyNTA2MzQwMH0.7Rs-k0W6TWNhbNS3MEzPlm4RQSJK6ZQYYnWntqUek78";

        [TestMethod]
        public void GetAccountBalance_for_existing_acccount_returns_account()
        {
            //Arrange
            client.Authenticator = new JwtAuthenticator(testTokenForExistingAccount);
            
            RestRequest request = new RestRequest(API_BASE_URL, DataFormat.Json);

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

        [TestMethod]
        public void GetAccountBalance_for_nonexistent_account_returns_404_error()
        {
            //Arrange
            client.Authenticator = new JwtAuthenticator(testTokenForNonExistentAccount);

            RestRequest request = new RestRequest(API_BASE_URL, DataFormat.Json);

            int expectedStatusCode = 404;

            //Act
            IRestResponse<Account> response = client.Get<Account>(request);

            int actualStatusCode = (int)response.StatusCode;

            //Assert
            Assert.AreEqual(expectedStatusCode, actualStatusCode, "Expected a 404 error, but did not get one");
        }

        /*
        [TestMethod]
        public void GetAccountBalance_returns_500_when_database_is_down()
        {
            //Arrange
            client.Authenticator = new JwtAuthenticator(testTokenForNonExistentAccount);

            RestRequest request = new RestRequest(API_BASE_URL, DataFormat.Json);

            int expectedStatusCode = 500;

            //Act
            IRestResponse<Account> response = client.Get<Account>(request);

            int actualStatusCode = (int)response.StatusCode;

            //Assert
            Assert.AreEqual(expectedStatusCode, actualStatusCode, "Expected a 500 error, but did not get one");
        }
        */
    }
}
