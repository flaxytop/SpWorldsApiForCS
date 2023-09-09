using RestSharp;
using System;
using System.Buffers.Text;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace spw
{
    public class SpWorlds : ISpWorlds
    {
        private string baseUrl = "https://spworlds.ru/api/public/";
        private string id { get; }
        private string token { get; }
        private string BearerToken;
        private RestClient client;

        public SpWorlds(string id, string token) {
            this.id = id;
            this.token = token;
            BearerToken = "Bearer " + Convert.ToBase64String(Encoding.UTF8.GetBytes($"{id}:{token}"));
            client = new RestClient();
        }

        private RestRequest defaultRequest()
        {
            var res = new RestRequest();
            res.AddHeader("Content-Type", "application/json");
            res.AddHeader("Authorization", BearerToken);
            return res;
        }


        private async Task<string> GetHttp(string last)
        {
            RestRequest request = defaultRequest();
            request.Resource = baseUrl + last;
            var res = await client.GetAsync(request);
            return res.Content;
        }

        private async Task<string> PostHttp(RestRequest request)
        {
            try
            {
                var res = client.PostAsync(request).Result;
                
                return res.Content;
            }
            catch(Exception ex)
            {

                if (ex.InnerException.Message.Contains("Unauthorized"))
                {
                    throw new Exception("EncorredTokenOrId");
                }
                if (ex.InnerException.Message.Contains("BadRequest"))
                {
                    throw new Exception("EncorredForm");
                }
                throw new Exception("UnknownError");
            }
        }

        public override async Task<int> GetBalance()
        {
            try
            {
                var res = JsonNode.Parse(await GetHttp("card"));
                return (int)res["balance"];
            }
            catch (Exception ex)
            {
                return -1;
            }
        }

        public override async Task<string> GetUser(string discordId)
        {
            var res = JsonNode.Parse(await GetHttp($"users/{discordId}"));
            if (res["username"] == null) {
                return null;
            } else {
                return res["username"].ToString();
            }
        }

        public override async Task<bool> SendPayment(int amount, string receiver, string message)
        {
            RestRequest request = defaultRequest();
            request.Resource = baseUrl + "transactions";
            var transitionInfo = new Dictionary<string, object>
            {
                { "receiver", receiver },
                { "amount", amount },
                { "comment", message }
            };
            request.AddBody(transitionInfo);
            try
            {
                var str = await PostHttp(request);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async override Task<string> CreatePayment(int amount, string redirectUrl, string webhookUrl, string message)
        {
            RestRequest request = defaultRequest();
            request.Resource = baseUrl + "payment";
            Dictionary<string, object> js = new Dictionary<string, object>()
            {
                { "amount", amount },
                { "redirectUrl", redirectUrl },
                { "webhookUrl", webhookUrl },
                { "data", message }
            };
            request.AddBody(js);
            try
            {
                var res = JsonNode.Parse(await PostHttp(request));
                return (string)res["url"]; 
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public override async Task<bool> Validator(string webhook, string body_hash)
        {
            byte[] body = Encoding.UTF8.GetBytes(body_hash);
            byte[] webhooks = Encoding.UTF8.GetBytes(webhook);
            var key = new HMACSHA256(Encoding.UTF8.GetBytes(token));
            string webhook_64 = Convert.ToBase64String(key.ComputeHash(webhooks));
            return webhook_64.Equals(body);
        }

        public override async Task<bool> IsSpWallet()
        {
            var res = await GetBalance();
            if(res != -1)
            {
                return true;
            }
            return false;
        }
    }

}