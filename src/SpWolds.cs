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

using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using RestSharp;

namespace spw;

public class SpWorlds
{
    private string baseUrl = "https://spworlds.ru/api/public/";

    private string BearerToken;

    private RestClient client;

    private string id { get; }

    private string token { get; }

    public SpWorlds(string _id, string _token)
    {
        id = _id;
        token = _token;
        BearerToken = "Bearer " + Convert.ToBase64String(Encoding.UTF8.GetBytes(id + ":" + token));
        client = new RestClient();
    }

    private RestRequest defaultRequest()
    {
        RestRequest restRequest = new RestRequest();
        restRequest.AddHeader("Content-Type", "application/json");
        restRequest.AddHeader("Authorization", BearerToken);
        return restRequest;
    }

    private async Task<string> GetHttpAsync(string last)
    {
        RestRequest request = defaultRequest();
        request.Resource = baseUrl + last;
        return (await client.GetAsync(request)).Content;
    }

    private async Task<string> PostHttpAsync(RestRequest request)
    {
        try
        {
            return (await client.PostAsync(request)).Content;
        }
        catch (Exception ex2)
        {
            Exception ex = ex2;
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

    private string GetHttp(string last)
    {
        RestRequest restRequest = defaultRequest();
        restRequest.Resource = baseUrl + last;
        RestResponse restResponse = client.Get(restRequest);
        return restResponse.Content;
    }

    private string PostHttp(RestRequest request)
    {
        try
        {
            RestResponse restResponse = client.Post(request);
            return restResponse.Content;
        }
        catch (Exception ex)
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

    public async Task<int> GetBalanceAsync()
    {
        try
        {
            JsonNode res = JsonNode.Parse(await GetHttpAsync("card"));
            return (int)res["balance"];
        }
        catch (Exception)
        {
            return -1;
        }
    }

    public int GetBalance()
    {
        try
        {
            JsonNode jsonNode = JsonNode.Parse(GetHttp("card"));
            return (int)jsonNode["balance"];
        }
        catch (Exception)
        {
            return -1;
        }
    }

    public async Task<string> GetUserAsync(string discordId)
    {
        JsonNode res = JsonNode.Parse(await GetHttpAsync("users/" + discordId));
        if (res["username"] == null)
        {
            return null;
        }

        return res["username"].ToString();
    }

    public string GetUser(string discordId)
    {
        JsonNode jsonNode = JsonNode.Parse(GetHttp("users/" + discordId));
        if (jsonNode["username"] == null)
        {
            return null;
        }

        return jsonNode["username"].ToString();
    }

    public async Task<bool> SendPaymentAsync(int amount, string receiver, string message)
    {
        RestRequest request = defaultRequest();
        request.Resource = baseUrl + "transactions";
        Dictionary<string, object> transitionInfo = new Dictionary<string, object>
        {
            { "receiver", receiver },
            { "amount", amount },
            { "comment", message }
        };
        request.AddBody(transitionInfo);
        try
        {
            await PostHttpAsync(request);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public bool SendPayment(int amount, string receiver, string message)
    {
        RestRequest restRequest = defaultRequest();
        restRequest.Resource = baseUrl + "transactions";
        Dictionary<string, object> obj = new Dictionary<string, object>
        {
            { "receiver", receiver },
            { "amount", amount },
            { "comment", message }
        };
        restRequest.AddBody(obj);
        try
        {
            string text = PostHttp(restRequest);
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async Task<string> CreatePaymentAsync(int amount, string redirectUrl, string webhookUrl, string message)
    {
        RestRequest request = defaultRequest();
        request.Resource = baseUrl + "payment";
        Dictionary<string, object> js = new Dictionary<string, object>
        {
            { "amount", amount },
            { "redirectUrl", redirectUrl },
            { "webhookUrl", webhookUrl },
            { "data", message }
        };
        request.AddBody(js);
        try
        {
            JsonNode res = JsonNode.Parse(await PostHttpAsync(request));
            return (string?)res["url"];
        }
        catch (Exception ex2)
        {
            Exception ex = ex2;
            return ex.Message;
        }
    }

    public string CreatePayment(int amount, string redirectUrl, string webhookUrl, string message)
    {
        RestRequest restRequest = defaultRequest();
        restRequest.Resource = baseUrl + "payment";
        Dictionary<string, object> obj = new Dictionary<string, object>
        {
            { "amount", amount },
            { "redirectUrl", redirectUrl },
            { "webhookUrl", webhookUrl },
            { "data", message }
        };
        restRequest.AddBody(obj);
        try
        {
            JsonNode jsonNode = JsonNode.Parse(PostHttp(restRequest));
            return (string?)jsonNode["url"];
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

    public bool ValidateWebhook(string body, string hashHeader)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(body);
        byte[] array = Convert.FromBase64String(hashHeader);
        HMACSHA256 hMACSHA = new HMACSHA256(Encoding.UTF8.GetBytes(token));
        byte[] array2 = hMACSHA.ComputeHash(bytes);
        if (array2.Length != array.Length)
        {
            return false;
        }

        for (int i = 0; i < array2.Length; i++)
        {
            if (array2[i] != array[i])
            {
                return false;
            }
        }

        return true;
    }

    public async Task<bool> IsSpWalletAsync()
    {
        if (await GetBalanceAsync() != -1)
        {
            return true;
        }

        return false;
    }

    public bool IsSpWallet()
    {
        int balance = GetBalance();
        if (balance != -1)
        {
            return true;
        }

        return false;
    }
}