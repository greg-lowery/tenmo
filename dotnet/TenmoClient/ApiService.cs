using System;
using RestSharp;
using RestSharp.Authenticators;
using System.Collections.Generic;
using System.Text;
using TenmoClient.Models;


namespace TenmoClient
{
    public class ApiService
    {
        public const string API_URL = "https://localhost:44315/api/";
        public IRestClient client = new RestClient();

        public Account GetAccountInfo()
        {
            var request = new RestRequest($"{API_URL}account", DataFormat.Json);
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            IRestResponse<Account> response = client.Get<Account>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new Exception("Unable to reach server. Please try again later.", response.ErrorException);
            }
            else if (!response.IsSuccessful && (int) response.StatusCode ==  500)
            {
                throw new Exception("Internal Server Error - Status Code : 500");
            }
            else if (!response.IsSuccessful && (int)response.StatusCode == 404)
            {
                throw new Exception("Bad Request - Status Code : 404");
            }
            else
            { 
                return response.Data; 
            }
            
        }

        public bool VerifyTransferAccountExists(Account accountToVerify)
        {
            var request = new RestRequest($"{API_URL}account/verify", DataFormat.Json);
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            request.AddJsonBody(accountToVerify);
            IRestResponse<Account> response = client.Post<Account>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new Exception("Unable to reach server. Please try again later.", response.ErrorException);
            }
            else if (!response.IsSuccessful && (int)response.StatusCode == 500)
            {
                throw new Exception("Internal Server Error - Status Code : 500");
            }
            else if ((int)response.StatusCode == 404)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Transfer SubmitTransferForProcessing(Transfer transfer)
        {
            RestRequest request = new RestRequest(API_URL, DataFormat.Json);
            request.AddJsonBody(transfer);

            IRestResponse<Transfer> response = client.Post<Transfer>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new Exception("Unable to reach server. Please try again later.", response.ErrorException);
            }
            else if (!response.IsSuccessful && (int)response.StatusCode == 500)
            {
                throw new Exception("Internal Server Error - Status Code : 500");
            }
            else if (!response.IsSuccessful && (int)response.StatusCode == 404)
            {
                throw new Exception("Transfer Account Not Found - Status Code : 404");
            }
            else if (1 == 2)
            {
                //ERROR FOR INSUFFICIENT FUNDS - 400 error
            }
            else if (1 == 2)
            {
                
            }

            Transfer completedTransfer = response.Data;

            return completedTransfer;
        }

    }
}
