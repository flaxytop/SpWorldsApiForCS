using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace spw.Models
{
    public class SPAccount
    {
        public int id { get; set; }
        public string username { get; set; }
        public string minecraftUUID { get; set; }
        public string status { get; set; }
        public string[] roles { get; set; }
        public SPCity city { get; set; }
        public SPCard[] cards { get; set; }
        public string createdAt { get; set; }

    }
}
