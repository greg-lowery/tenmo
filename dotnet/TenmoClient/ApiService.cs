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
            else if (!response.IsSuccessful && (int)response.StatusCode == 500)
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
            var request = new RestRequest($"{API_URL}account/verify/account", DataFormat.Json);
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            request.AddJsonBody(accountToVerify);
            IRestResponse response = client.Post(request);
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

        public bool VerifySufficientFunds(decimal amtToTransfer)
        {
            string amtToTransferString = amtToTransfer.ToString();

            var request = new RestRequest($"{API_URL}account/verify/funds", DataFormat.Json);
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            request.AddJsonBody(amtToTransferString);
            IRestResponse response = client.Post(request);

            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new Exception("Unable to reach server. Please try again later.", response.ErrorException);
            }
            else if (!response.IsSuccessful && (int)response.StatusCode == 500)
            {
                throw new Exception("Internal Server Error - Status Code : 500");
            }
            else if ((int)response.StatusCode == 400)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        public Transfer SubmitTransferForProcessing(Transfer newtransfer, ReturnUser transferDestinationUser)
        {
            TransferUser transferUser = new TransferUser();
            transferUser.Transfer = newtransfer;
            transferUser.UserId = transferDestinationUser.UserId;
            RestRequest request = new RestRequest($"{API_URL}account/transfer", DataFormat.Json);
            request.AddJsonBody(transferUser);
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());

            //api request to the controller
            IRestResponse<Transfer> response = client.Post<Transfer>(request);

            //I talked to the server and got no response/nothing was transported back:
            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new Exception("Unable to reach server. Please try again later.", response.ErrorException);
            }

            //I got a response, but the server told me that my response was not successful:
            else if (!response.IsSuccessful && (int)response.StatusCode == 500)
            {
                throw new Exception($"{response.ErrorMessage} - Status Code : 500");
            }

            else
            {
                Transfer successfulTransfer = response.Data;

                return successfulTransfer;
            }


        }
        public List<ReturnUser> GetAllUsers()
        {
            var request = new RestRequest($"{API_URL}account/userlist", DataFormat.Json); 
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            IRestResponse<List<ReturnUser>> response = client.Get<List<ReturnUser>>(request);

            if (response.ResponseStatus != ResponseStatus.Completed)    //check error handling
            {
                throw new Exception("Unable to reach server. Please try again later.", response.ErrorException);
            }
            else if (!response.IsSuccessful && (int)response.StatusCode == 500)
            {
                throw new Exception("Internal Server Error - Error Code : 500");
            }
            else if (!response.IsSuccessful && (int)response.StatusCode == 403)
            {
                throw new Exception($"Bad Request - You are not logged in. Please log in to continue. Error Code: 403");
            }
            else if ((int)response.StatusCode == 204)
            {
                throw new Exception($"Bad Request - There are no other users in the system! Error Code: 204");
            }
            else
            {
                return response.Data;
            }
        }
    }
}
