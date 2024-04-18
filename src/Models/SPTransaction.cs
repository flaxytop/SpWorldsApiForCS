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
        public string sender_username { get; set; }
        public string sender_number { get; set; }
        public string receiver_username { get; set; }
        public string receiver_number { get; set; }
        public string comment {  get; set; }
        public string createdAt { get; set; }
    }
}
