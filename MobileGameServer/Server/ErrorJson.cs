using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobileGameServer.Server
{
    public class ErrorJson
    {
        public string Message;
        public ErrorJson(string message)
        {
            Message = message;
        }
    }
}
