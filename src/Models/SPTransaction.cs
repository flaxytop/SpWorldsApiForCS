using ConsoleApp1.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace spw.Models
{
    public class SPTransaction
    {
        public string id { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public SPWebhookUser sender { get; set; }
        public SPWebhookUser receiver { get; set; }
        public string comment { get; set; }
        public string createdAt { get; set; }
    }
}
