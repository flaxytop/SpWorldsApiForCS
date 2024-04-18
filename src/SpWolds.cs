using RestSharp;
using spw.Models;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
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
        var resp = await client.GetAsync(request);
        ExceptionSearch(resp);
        return resp.Content;
    }

    private async Task<string> PostHttpAsync(RestRequest request)
    {
        var resp = await client.PostAsync(request);
        ExceptionSearch(resp);
        return resp.Content!;
    }

    private string GetHttp(string last)
    {
        RestRequest request = defaultRequest(last);
        var resp = client.Get(request);
        ExceptionSearch(resp);
        return resp.Content!;
    }

    private string PostHttp(RestRequest request)
    {
        RestResponse resp = client.Post(request);
        ExceptionSearch(resp);
        return resp.Content!;
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
        if(resp.IsSuccessStatusCode) {}
        else if (resp.StatusCode == HttpStatusCode.Unauthorized)
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

    public async Task<string> CreatePaymentAsync(SPPayment payment)
    {
        RestRequest request = defaultRequest("payment");
        request.AddBody(payment);
        JsonNode res = JsonNode.Parse(await PostHttpAsync(request));
        return (string?)res["url"];
    }

    public string CreatePayment(SPPayment payment)
    {
        RestRequest request = defaultRequest("payment");
        request.AddBody(payment);
        JsonNode jsonNode = JsonNode.Parse(PostHttp(request));
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
        return JsonSerializer.Deserialize<SPAccount>(await GetHttpAsync("account"));
    }

    public SPAccount GetAccount()
    {
        return JsonSerializer.Deserialize<SPAccount>(GetHttp("account"));
    }

    public async Task<SPCard[]> GetCardsAsync(string username)
    {
        return JsonSerializer.Deserialize<SPCard[]>(await GetHttpAsync($"account/{username}/cards"));
    }
    public SPCard[] GetCards(string username)
    {
        return JsonSerializer.Deserialize<SPCard[]>(GetHttp($"account/{username}/cards"));
    }
    public async Task<bool> SetWebhookAsync(string webhook)
    {
        RestRequest restRequest = defaultRequest("card/webhook");
        Dictionary<string, string> dict = new Dictionary<string, string>()
            {
                {"url", webhook}
            };
        restRequest.AddBody(dict);
        var resp = await PostHttpAsync(restRequest);
        return true;
    }

    public bool SetWebhook(string webhook)
    {
        RestRequest restRequest = defaultRequest("card/webhook");
        Dictionary<string, string> dict = new Dictionary<string, string>()
            {
                {"url", webhook}
            };
        restRequest.AddBody(dict);
        var resp = PostHttp(restRequest);
        return true;
    }

}
public class main
{
    public void Main()
    {
        SpWorlds sp = new SpWorlds("f4323357-1d13-40a2-b1b6-f27cbb11425f", "iqvk7yPaYm/SwNU3F6fNEntEp79wHWVG");
        Console.ReadKey();
    }
}