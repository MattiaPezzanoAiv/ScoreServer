using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace MobileGameServer.Server
{
    public sealed partial class AsyncOperation : IAsyncOperation
    {
        public delegate bool GET(AsyncOperation op, HttpListenerContext context, HttpListenerRequest request);

        public static Dictionary<string, GET> GETCalls;

        static AsyncOperation()
        {
            GETCalls = new Dictionary<string, GET>();
            GETCalls.Add("DeviceID", Get_DeviceID);
            GETCalls.Add("Best", Get_Best);
        }

        private static bool Get_DeviceID(AsyncOperation op, HttpListenerContext context, HttpListenerRequest request)
        {
            string[] values = request.QueryString.GetValues("DeviceID");

            if (values != null && values.Length > 0)
            {
                //response with json with best score of player
                HttpListenerResponse response = context.Response;
                int result = 0;
                bool succes = int.TryParse(values[0], out result);
                if (succes)
                {
                    string outputJson = JsonConvert.SerializeObject(Server.GetScore(result));
                    if (outputJson != null)
                    {
                        byte[] data = Encoding.UTF8.GetBytes(outputJson);
                        response.OutputStream.Write(data, 0, data.Length);
                        response.OutputStream.Close();
                        op.End();
                        return true; //request satisfied
                    }
                }
            }
            return false;
        }

        private static bool Get_Best(AsyncOperation op, HttpListenerContext context, HttpListenerRequest request)
        {
            string[] values = request.QueryString.GetValues("Best");

            if (values != null && values.Length > 0)
            {
                //response with json with best score of player
                HttpListenerResponse response = context.Response;
                int result = 0;
                bool succes = int.TryParse(values[0], out result);
                if (succes)
                {
                    Dictionary<int, Score> bests = new Dictionary<int, Score>();
                    foreach (var s in Server.Scores)
                    {
                        bests.Add(s.Key, s.Value);
                        if (--result <= 0) break;
                    }
                    string outputJson = JsonConvert.SerializeObject(bests);
                    byte[] data = Encoding.UTF8.GetBytes(outputJson);
                    response.OutputStream.Write(data, 0, data.Length);
                    response.OutputStream.Close();
                    op.End();
                    return true; //request satisfied
                }
            }
            return false;
        }
    }
}
