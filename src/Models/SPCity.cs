using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spw.Models
{
    public class SPCity
    {
        public string id {  get; set; }
        public string name { get; set; }
        public int x { get; set; }
        public int y { get; set; }
        public bool isMayor { get; set; }
    }
}
