using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spw.Models
{
    public class SPItem
    {

        public SPItem(string name, int count, int amount, string comment) {
            this.name = name;
            this.count = count;
            this.amount = amount;
            this.comment = comment;
        }
        public SPItem(string name, int count, int amount)
        {
            this.name = name;
            this.count = count;
            this.amount = amount;
        }
        public SPItem() { }
        public string name { get; set; }
        public int count { get; set; }
        public int amount { get; set; }
        public string comment { get; set; } // can be null
    }
}
