using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RestSharp;
using RestSharp.Extensions;

namespace youAreWhatYouEat.Authorization
{
    public class MyTokenReply
    {
        public bool active { get; set; } = false;
        public string? client_id { get; set; }
        public string? username { get; set; }
        public string? token_type { get; set; }
        public int? exp { get; set; }
        public int? iat { get; set; }
        public int? nbf { get; set; }
        public string? sub { get; set; }
        public List<string>? aud { get; set; }
        public string? iss { get; set; }
    }

    public class MyTokenRequest
    {
        public string token { get; set; }
        public string? token_type_hint { get; set; } = "access_token";

        public MyTokenRequest(string t)
        {
            token = t;
        }
    }
    public class MyToken
    {
        static HttpClient client = new HttpClient();
        static public async Task<MyTokenReply> checkToken(string token)
        {
            var client = new RestClient(System.Configuration.ConfigurationManager.AppSettings["CasdoorBase"]+"");
            var request = new RestRequest("/api/login/oauth/introspect", Method.Post);
            request.AddHeader("accept", "application/json");
            request.AddHeader("Content-Type", "application/x-www-form-urlencoded");
            request.AddHeader("Authorization", System.Configuration.ConfigurationManager.AppSettings["ClientAuth"] + "");
            request.AddParameter("token", token);
            request.AddParameter("token_type_hint", "access_token");
            /*var response = client.Execute(request);*/
            var response = await client.PostAsync<MyTokenReply>(request);
            return response;
        }
    }
}
