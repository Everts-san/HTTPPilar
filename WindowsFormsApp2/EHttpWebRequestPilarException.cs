using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WindowsFormsApp2
{
    class EHttpWebRequestPilarException : Exception
    {
        public string ResponseTextHTTP { get; set; }
        public EHttpWebRequestPilarException(string message, string responseTextHttp) : base(message)
        {
            this.ResponseTextHTTP = responseTextHttp;
        }
    }
}
