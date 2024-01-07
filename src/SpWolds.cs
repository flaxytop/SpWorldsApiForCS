using RestSharp;
using spw.Models;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Transactions;

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
        RestRequest restRequest = defaultRequest(last);
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

    public async Task<SPCardUser> GetCardInfoAsync()
    {
        try
        {
            return JsonSerializer.Deserialize<SPCardUser>(await GetHttpAsync("card"));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }

    public SPCardUser GetCardInfo()
    {
        try
        {
            return JsonSerializer.Deserialize<SPCardUser>(GetHttp("card"));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }

    public async Task<SPUser> GetUserAsync(string discordId)
    {
        try
        {
            return JsonSerializer.Deserialize<SPUser>(await GetHttpAsync("users/" + discordId));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }

    public SPUser GetUser(string discordId)
    {
        try
        {
            return JsonSerializer.Deserialize<SPUser>(GetHttp("users/" + discordId));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
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
        RestRequest restRequest = defaultRequest("transactions");
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

    public async Task<string> CreatePaymentAsync(SPPayment payment)
    {
        RestRequest request = defaultRequest("payment");
        request.AddBody(payment);
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

    public string CreatePayment(SPPayment payment)
    {
        RestRequest request = defaultRequest("payment");
        request.AddBody(payment);
        try
        {
            JsonNode jsonNode = JsonNode.Parse(PostHttp(request));
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
        if ((await GetCardInfoAsync()).balance != -1)
        {
            return true;
        }

        return false;
    }

    public bool IsSpWallet()
    {
        int balance = GetCardInfo().balance;
        if (balance != -1)
        {
            return true;
        }

        return false;
    }

    public async Task<SPAccount> GetAccountAsync()
    {
        try
        {
            return JsonSerializer.Deserialize<SPAccount>(await GetHttpAsync("account"));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }

    public SPAccount GetAccount()
    {
        try
        {
            return JsonSerializer.Deserialize<SPAccount>(GetHttp("account"));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }

    public async Task<SPCard[]> GetCardsAsync(string username)
    {
        try
        {
            return JsonSerializer.Deserialize<SPCard[]>(await GetHttpAsync($"account/{username}/cards"));
        }
        catch(Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }
    public SPCard[] GetCards(string username)
    {
        try
        {
            return JsonSerializer.Deserialize<SPCard[]>(GetHttp($"account/{username}/cards"));
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
            return null;
        }
    }
    public async Task<bool> SetWebhookAsync(string webhook)
    {
        try
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
        catch (Exception)
        {
            return false;
        }
    }

    public bool SetWebhook(string webhook)
    {
        try
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
        catch (Exception)
        {
            return false;
        }
    }

}