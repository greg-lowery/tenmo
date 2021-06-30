using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp;

namespace ServerApiTests
{
    [TestClass]
    public class GetAccountBalanceTests
    {
        private readonly static string API_BASE_URL = "https://localhost:44315/";
        private readonly IRestClient client = new RestClient();
        private readonly string testToken = "";

        [TestMethod]
        public void TestMethod1()
        {
            Assert.AreEqual(1, 1);
        }
    }
}
