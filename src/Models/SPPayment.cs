using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spw.Models
{
    public class SPPayment
    {
        public SPPayment(SPItem item, string redirectUrl, string webhookUrl, string data) {
            this.item = item;
            this.redirectUrl = redirectUrl;
            this.webhookUrl = webhookUrl;
            this.data = data;
        }
        public SPItem item { get; set; }
        public string redirectUrl { get; set; }
        public string webhookUrl { get; set; }
        public string data {  get; set; }
    }
}
