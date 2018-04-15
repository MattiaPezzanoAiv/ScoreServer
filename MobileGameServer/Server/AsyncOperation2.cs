using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace MobileGameServer.Server
{
    public sealed partial class AsyncOperation : IAsyncOperation
    {
        public delegate bool GET(AsyncOperation op, HttpListenerContext context, HttpListenerRequest request);
        public delegate bool POST(AsyncOperation op, HttpListenerContext context, HttpListenerRequest request);


        public static Dictionary<string, GET> GETCalls;
        public static Dictionary<string, POST> POSTCalls;


        static AsyncOperation()
        {
            GETCalls = new Dictionary<string, GET>();
            GETCalls.Add("DeviceID", Get_DeviceID);
            GETCalls.Add("Best", Get_Best);

            POSTCalls = new Dictionary<string, POST>();
            POSTCalls.Add("DeviceID", Post_DeviceID);
        }

        private static bool Post_DeviceID(AsyncOperation op, HttpListenerContext context, HttpListenerRequest request)
        {
            string[] parameters = new string[2];
            using (StreamReader reader = new StreamReader(request.InputStream))
            {
                string stringRequest = reader.ReadToEnd();
                parameters = stringRequest.Split('&');
            }


            HttpListenerResponse response = context.Response;
            Score score = new Score();
            score.Points = long.Parse(parameters[1].Split('=')[1]); //must be fixed with all score values
            string outputJson = Server.SetScore(parameters[0].Split('=')[1], score).ToString(); //set score
            if (outputJson != null)
            {
                byte[] data = Encoding.UTF8.GetBytes(outputJson);
                response.OutputStream.Write(data, 0, data.Length);
                response.OutputStream.Close();
                op.End();
                Console.WriteLine("POST SUCCESFUL");
                return true; //request satisfied
            }
            Console.WriteLine("POST ABORTED");
            return false;
        }

        private static bool Get_DeviceID(AsyncOperation op, HttpListenerContext context, HttpListenerRequest request)
        {
            string[] values = request.QueryString.GetValues("DeviceID");

            if (values != null && values.Length > 0)
            {
                //response with json with best score of player
                HttpListenerResponse response = context.Response;

                string outputJson = JsonConvert.SerializeObject(Server.GetScore(values[0]));
                if (outputJson != null)
                {
                    byte[] data = Encoding.UTF8.GetBytes(outputJson);
                    response.OutputStream.Write(data, 0, data.Length);
                    response.OutputStream.Close();
                    op.End();
                    return true; //request satisfied
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
                    Dictionary<string, Score> bests = new Dictionary<string, Score>();
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
