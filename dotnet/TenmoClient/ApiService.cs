using RestSharp;
using RestSharp.Authenticators;
using System;
using System.Collections.Generic;
using TenmoClient.Models;

namespace TenmoClient
{
    class ApiService
    {
        private readonly static string API_BASE_URL = "https://localhost:44315/";
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
