using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using TenmoClient.Models;

namespace TenmoClient
{
    class ApiService
    {
        private readonly static string API_BASE_URL = "https://localhost:44384/";
        private readonly IRestClient client = new RestClient();

        
        public List<decimal> GetBalances(int id)
        {
            RestRequest request = new RestRequest($"{API_BASE_URL}account/{id}/balance");
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            IRestResponse<List<decimal>> response = client.Get<List<decimal>>(request);

            if (response.ResponseStatus != ResponseStatus.Completed || !response.IsSuccessful)
            {
                ProcessErrorResponse(response);
            }
            else
            {
                return response.Data;
            }
            return null;
        }

        public List<string> GetUsernames()
        {
            RestRequest request = new RestRequest(API_BASE_URL + "usernames");
            IRestResponse<List<string>> response = client.Get<List<string>>(request);

            if (response.ResponseStatus != ResponseStatus.Completed || !response.IsSuccessful)
            {
                ProcessErrorResponse(response);
            }
            else
            {
                return response.Data;
            }
            return null;
        }

        public List<ApiUser> GetUsers()
        {
            RestRequest request = new RestRequest(API_BASE_URL + "users");
            IRestResponse<List<ApiUser>> response = client.Get<List<ApiUser>>(request);

            if (response.ResponseStatus != ResponseStatus.Completed || !response.IsSuccessful)
            {
                ProcessErrorResponse(response);
            }
            else
            {
                return response.Data;
            }
            return null;
        }

        public bool TransferMoney(int sendingId, decimal dollarAmount, int recievingId, string type, string status)
        {
            if (GetBalances(sendingId)[0] >= dollarAmount && dollarAmount > 0)
            {
                Transfer transfer = new Transfer(sendingId, dollarAmount, recievingId, type, status);
                client.Authenticator = new JwtAuthenticator(UserService.GetToken());
                RestRequest request = new RestRequest(API_BASE_URL + "transfer/" + transfer);
                request.AddJsonBody(transfer);
                IRestResponse<Transfer> response = client.Put<Transfer>(request);


                if (response.ResponseStatus != ResponseStatus.Completed || !response.IsSuccessful)
                {
                    ProcessErrorResponse(response);
                }
                else
                {
                    Console.WriteLine("Transfer initiated...");
                    return true;
                }
                return false;
            }
            else
            {
                return false;
            }
        }

        public void ProcessErrorResponse(IRestResponse response)
        {
            
            if (response.ResponseStatus != ResponseStatus.Completed)
            {
                throw new Exception($"Accounts not found, Error Code: {(int)response.StatusCode}");
            }
            else if (!response.IsSuccessful)
            {
                if ((int)response.StatusCode == 401)
                {
                    throw new Exception($"User hasn't been authenticated, Error Code: {(int)response.StatusCode}");
                }
                else if ((int)response.StatusCode == 403)
                {
                    throw new Exception($"User isn't authorized for access, Error Code: {(int)response.StatusCode}");
                }
                else
                {
                    throw new Exception($"{(int)response.StatusCode}");
                }
            }
        }


    }
}
