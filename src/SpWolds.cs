using RestSharp;
using spw.Models;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Net;
using spw.Exceptions;


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
        Console.WriteLine(BearerToken);
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
        RestResponse restResponse = await client.GetAsync(request);
        ExceptionSearch(restResponse);
        return restResponse.Content;
    }

    private async Task<string> PostHttpAsync(RestRequest request)
    {
        RestResponse restResponse = await client.PostAsync(request);
        ExceptionSearch(restResponse);
        return restResponse.Content;
    }

    private string GetHttp(string last)
    {
        RestRequest request = defaultRequest(last);
        RestResponse restResponse = client.Get(request);
        ExceptionSearch(restResponse);
        return restResponse.Content;
    }

    private string PostHttp(RestRequest request)
    {
        RestResponse restResponse = client.Post(request);
        ExceptionSearch(restResponse);
        return restResponse.Content;
    }
    private string PutHttp(RestRequest request)
    {
        RestResponse restResponse = client.Put(request);
        ExceptionSearch(restResponse);
        return restResponse.Content;
    }
    private async Task<string> PutHttpAsync(RestRequest request)
    {
        RestResponse restResponse = await client.PutAsync(request);
        ExceptionSearch(restResponse);
        return restResponse.Content;
    }

    public async Task<SPCardUser> GetCardInfoAsync()
    {
        return JsonSerializer.Deserialize<SPCardUser>(await GetHttpAsync("card"));
    }

    public SPCardUser GetCardInfo()
    {
        return JsonSerializer.Deserialize<SPCardUser>(GetHttp("card"));
    }

    public async Task<SPUser> GetUserAsync(string discordId)
    {
        return JsonSerializer.Deserialize<SPUser>(await GetHttpAsync("users/" + discordId));
    }

    public SPUser GetUser(string discordId)
    {
        return JsonSerializer.Deserialize<SPUser>(GetHttp("users/" + discordId));
    }

    public void ExceptionSearch(RestResponse resp)
    {
        if (!resp.IsSuccessStatusCode)
        {
            if (resp.StatusCode == HttpStatusCode.Unauthorized)
            {
                throw new UnathorizedException("SPException: Incorrect token or id of card (401)" + resp.ErrorMessage, resp.ErrorException);
            }

            if (resp.StatusCode == HttpStatusCode.BadRequest)
            {
                throw new BadRequestException("SPException: Incorrect data (400)" + resp.ErrorMessage, resp.ErrorException);
            }

            if (resp.StatusCode == HttpStatusCode.BadGateway)
            {
                throw new BadGatewayException("SPException: SpWorlds API off (502)." + resp.ErrorMessage, resp.ErrorException);
            }

            throw new Exception(resp.ErrorMessage, resp.ErrorException);
        }
    }

    public async Task<int> SendPaymentAsync(int amount, string receiver, string message)
    {
        RestRequest request = defaultRequest("transactions");
        Dictionary<string, object> obj = new Dictionary<string, object>
        {
            { "receiver", receiver },
            { "amount", amount },
            { "comment", message }
        };
        request.AddBody(obj);
        return (int)JsonNode.Parse(await PostHttpAsync(request))["balance"];
    }

    public int SendPayment(int amount, string receiver, string message)
    {
        RestRequest request = defaultRequest("transactions");
        Dictionary<string, object> obj = new Dictionary<string, object>
        {
            { "receiver", receiver },
            { "amount", amount },
            { "comment", message }
        };
        request.AddBody(obj);
         
        return (int)JsonNode.Parse(PostHttp(request))["balance"];
    }

    public async Task<string> CreatePaymentAsync(SPPayment payment)
    {
        RestRequest request = defaultRequest("payment");
        request.AddBody(payment);
        return (string?)JsonNode.Parse(await PostHttpAsync(request))["url"];
    }

    public string CreatePayment(SPPayment payment)
    {
        RestRequest request = defaultRequest("payment");
        request.AddBody(payment);
        return (string?)JsonNode.Parse(PostHttp(request))["url"];
    }

    public bool ValidateWebhook(string body, string hashHeader)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(body);
        byte[] array = Convert.FromBase64String(hashHeader);
        byte[] array2 = new HMACSHA256(Encoding.UTF8.GetBytes(token)).ComputeHash(bytes);
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
        try
        {
            await GetCardInfoAsync();
            return true;
        }
        catch (UnathorizedException)
        {
            return false;
        }
    }

    public bool IsSpWallet()
    {
        try
        {
            GetCardInfo();
            return true;
        }
        catch (UnathorizedException)
        {
            return false;
        }
    }

    public async Task<SPAccount> GetAccountAsync()
    {
        return JsonSerializer.Deserialize<SPAccount>(await GetHttpAsync("accounts/me"));
    }

    public SPAccount GetAccount()
    {
        return JsonSerializer.Deserialize<SPAccount>(GetHttp("accounts/me"));
    }

    public async Task<SPCard[]> GetCardsAsync(string username)
    {
        return JsonSerializer.Deserialize<SPCard[]>(await GetHttpAsync("accounts/" + username + "/cards"));
    }

    public SPCard[] GetCards(string username)
    {
        return JsonSerializer.Deserialize<SPCard[]>(GetHttp("accounts/" + username + "/cards"));
    }

    public async Task<bool> SetWebhookAsync(string webhook)
    {
        RestRequest request = defaultRequest("card/webhook");
        Dictionary<string, string> obj = new Dictionary<string, string> { { "url", webhook } };
        request.AddBody(obj);
        await PutHttpAsync(request);
        return true;
    }

    public bool SetWebhook(string webhook)
    {
        RestRequest request = defaultRequest("card/webhook");
        Dictionary<string, string> obj = new Dictionary<string, string> { { "url", webhook } };
        request.AddBody(obj);
        PutHttp(request);
        return true;
    }
}
