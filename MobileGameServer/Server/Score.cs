using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using System.IO;

namespace MobileGameServer.Server
{
    public class Score
    {
        public string Device { get; set; }
        public long Points { get; set; }
        public string DeviceID { get; set; }
        public EndPoint EndPoint { get; set; }
        public DateTime Date { get; set; }
        
    }


}
