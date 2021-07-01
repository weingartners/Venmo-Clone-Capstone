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
        public List<Transfer> GetTransfersById(int id)
        {
            RestRequest request = new RestRequest($"{API_BASE_URL}transfer/{id}");
            client.Authenticator = new JwtAuthenticator(UserService.GetToken());
            IRestResponse<List<Transfer>> response = client.Get<List<Transfer>>(request);

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
        public bool TransferMoney(int sendingId, decimal dollarAmount, int receivingId, int typeId, int statusId )
        {
            if (GetBalances(sendingId)[0] >= dollarAmount && dollarAmount > 0)
            {
                Transfer transfer = new Transfer(sendingId, dollarAmount, receivingId, typeId, statusId);
                client.Authenticator = new JwtAuthenticator(UserService.GetToken());
                RestRequest putRequest = new RestRequest(API_BASE_URL + "transfer/" + transfer);
                putRequest.AddJsonBody(transfer);
                IRestResponse<Transfer> putResponse = client.Put<Transfer>(putRequest);

                RestRequest postRequest = new RestRequest(API_BASE_URL + "savetransfer");
                client.Authenticator = new JwtAuthenticator(UserService.GetToken());
                postRequest.AddJsonBody(transfer);
                IRestResponse<Transfer> postResponse = client.Post<Transfer>(postRequest);



                if (putResponse.ResponseStatus != ResponseStatus.Completed || !putResponse.IsSuccessful)
                {
                    ProcessErrorResponse(putResponse);
                }
                else if (postResponse.ResponseStatus != ResponseStatus.Completed || !postResponse.IsSuccessful)
                {
                    ProcessErrorResponse(postResponse);
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
