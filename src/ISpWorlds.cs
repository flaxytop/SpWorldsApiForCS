using RestSharp;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spw
{
    public abstract class ISpWorlds
    {
        

        public abstract Task<int> GetBalance();
        public abstract Task<string> GetUser(string discordId);

        public abstract Task<bool> SendPayment(int amount, string reciver, string message);
        public abstract Task<string> CreatePayment(int amount, string redirectUrl, string webhookUrl, string message);

        public abstract Task<bool> Validator(string webhook, string body_hash);

        public abstract Task<bool> IsSpWallet();




    }
   
}
