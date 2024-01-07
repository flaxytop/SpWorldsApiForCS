using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spw.Models
{
    public class SPPayment
    {
        public SPPayment(SPItem _item, string _redirectUrl, string _webhookUrl, string _data) {
            item = _item;
            redirectUrl = _redirectUrl; 
            webhookUrl = _webhookUrl;
            data = _data;
        }
        public SPItem item { get; set; }
        public string redirectUrl { get; set; }
        public string webhookUrl { get; set; }
        public string data {  get; set; }
    }
}
