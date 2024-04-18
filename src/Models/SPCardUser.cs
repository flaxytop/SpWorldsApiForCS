using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spw.Models
{
    public class SPCardUser
    {
        public SPCardUser(int _balance, string _webhook) {
            balance = _balance;
            webhook = _webhook;
        }
        public int balance {  get; set; }
        public string webhook { get; set; }
    }
}
