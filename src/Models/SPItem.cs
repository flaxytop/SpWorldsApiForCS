using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spw.Models
{
    public class SPItem
    {

        public SPItem(string _name, int _count, int _amount, string _comment) {
            name = _name;
            count = _count;
            amount = _amount;
            comment = _comment;
        }
        public SPItem(string _name, int _count, int _amount)
        {
            name = _name;
            count = _count;
            amount = _amount;
        }

        public string name { get; set; }
        public int count { get; set; }
        public int amount { get; set; }
        public string comment { get; set; } // can be null
    }
}
