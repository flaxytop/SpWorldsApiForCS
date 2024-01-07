using RestSharp;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json.Nodes;
using System.Net;
using spw.Exceptions;

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

    private RestRequest defaultRequest(string last)
    {
        RestRequest restRequest = new RestRequest(baseUrl + last);
        restRequest.AddHeader("Content-Type", "application/json");
        restRequest.AddHeader("Authorization", BearerToken);
        return restRequest;
    }

    private async Task<string> GetHttpAsync(string last)
    {
        RestRequest request = defaultRequest(last);
        var resp = await client.GetAsync(request);
        if (resp.IsSuccessStatusCode)
        {
            return resp.Content;
        }
        if (resp.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new UnathorizedException("SPException: Incorrect token or id of card (401)" + resp.ErrorMessage, resp.ErrorException);
        }
        else if (resp.StatusCode == HttpStatusCode.BadRequest)
        {
            throw new BadRequestException("SPException: Incorrect data (400)" + resp.ErrorMessage, resp.ErrorException);
        }
        else if (resp.StatusCode == HttpStatusCode.BadGateway)
        {
            throw new BadGatewayException("SPException: SpWorlds API off (502)." + resp.ErrorMessage, resp.ErrorException);
        }
        else
        {
            throw new Exception(resp.ErrorMessage, resp.ErrorException);
        }
    }

    private async Task<string> PostHttpAsync(RestRequest request)
    {
        var resp = await client.PostAsync(request);

        if (resp.IsSuccessStatusCode)
        {
            return resp.Content;
        }
        if (resp.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new UnathorizedException("SPException: Incorrect token or id of card (401)" + resp.ErrorMessage, resp.ErrorException);
        }
        else if (resp.StatusCode == HttpStatusCode.BadRequest)
        {
            throw new BadRequestException("SPException: Incorrect data (400)" + resp.ErrorMessage, resp.ErrorException);
        }
        else if (resp.StatusCode == HttpStatusCode.BadGateway)
        {
            throw new BadGatewayException("SPException: SpWorlds API off (502)." + resp.ErrorMessage, resp.ErrorException);
        }
        else
        {
            throw new Exception(resp.ErrorMessage, resp.ErrorException);
        }
    
    }

    private string GetHttp(string last)
    {
        RestRequest request = defaultRequest(last);
        var resp = client.Get(request);
        if (resp.IsSuccessStatusCode)
        {
            return resp.Content;
        }
        if (resp.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new UnathorizedException("SPException: Incorrect token or id of card (401)" + resp.ErrorMessage, resp.ErrorException);
        }
        else if (resp.StatusCode == HttpStatusCode.BadRequest)
        {
            throw new BadRequestException("SPException: Incorrect data (400)" + resp.ErrorMessage, resp.ErrorException);
        }
        else if (resp.StatusCode == HttpStatusCode.BadGateway)
        {
            throw new BadGatewayException("SPException: SpWorlds API off (502)." + resp.ErrorMessage, resp.ErrorException);
        }
        else
        {
            throw new Exception(resp.ErrorMessage, resp.ErrorException);
        }
    }

    private string PostHttp(RestRequest request)
    {
        var resp = client.Post(request);

        if (resp.IsSuccessStatusCode)
        {
            return resp.Content;
        }
        if (resp.StatusCode == HttpStatusCode.Unauthorized)
        {
            throw new UnathorizedException("SPException: Incorrect token or id of card (401)" + resp.ErrorMessage, resp.ErrorException);
        }
        else if (resp.StatusCode == HttpStatusCode.BadRequest)
        {
            throw new BadRequestException("SPException: Incorrect data (400)" + resp.ErrorMessage, resp.ErrorException);
        }
        else if (resp.StatusCode == HttpStatusCode.BadGateway)
        {
            throw new BadGatewayException("SPException: SpWorlds API off (502)." + resp.ErrorMessage, resp.ErrorException);
        }
        else
        {
            throw new Exception(resp.ErrorMessage, resp.ErrorException);
        }
    }
    public async Task<int> GetBalanceAsync()
    {
        JsonNode res = JsonNode.Parse(await GetHttpAsync("card"));
        return (int)res["balance"];
    }

    public int GetBalance()
    {
        JsonNode jsonNode = JsonNode.Parse(GetHttp("card"));
        return (int)jsonNode["balance"];
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
        RestRequest request = defaultRequest("transactions");
        Dictionary<string, object> transitionInfo = new Dictionary<string, object>
        {
            { "receiver", receiver },
            { "amount", amount },
            { "comment", message }
        };
        request.AddBody(transitionInfo);
        await PostHttpAsync(request);
        return true;
    }

    public bool SendPayment(int amount, string receiver, string message)
    {
        RestRequest restRequest = defaultRequest("transactions");
        Dictionary<string, object> obj = new Dictionary<string, object>
        {
            { "receiver", receiver },
            { "amount", amount },
            { "comment", message }
        };
        restRequest.AddBody(obj);
        string text = PostHttp(restRequest);
        return true;
    }

    public async Task<string> CreatePaymentAsync(int amount, string redirectUrl, string webhookUrl, string message)
    {
        RestRequest request = defaultRequest("payment");
        Dictionary<string, object> js = new Dictionary<string, object>
        {
            { "amount", amount },
            { "redirectUrl", redirectUrl },
            { "webhookUrl", webhookUrl },
            { "data", message }
        };
        request.AddBody(js);
        JsonNode res = JsonNode.Parse(await PostHttpAsync(request));
        return (string?)res["url"];
    }

    public string CreatePayment(int amount, string redirectUrl, string webhookUrl, string message)
    {
        RestRequest restRequest = defaultRequest("payment");
        Dictionary<string, object> obj = new Dictionary<string, object>
        {
            { "amount", amount },
            { "redirectUrl", redirectUrl },
            { "webhookUrl", webhookUrl },
            { "data", message }
        };
        restRequest.AddBody(obj);
        JsonNode jsonNode = JsonNode.Parse(PostHttp(restRequest));
        return (string?)jsonNode["url"];
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