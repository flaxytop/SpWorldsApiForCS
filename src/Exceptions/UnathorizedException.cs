using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace spw.Exceptions
{
    [Serializable]
    public class UnathorizedException : Exception
    {
        public UnathorizedException() { }
        public UnathorizedException(string message) : base(message){}
        public UnathorizedException(string message, Exception exception) : base(message, exception) { }
    }
}
