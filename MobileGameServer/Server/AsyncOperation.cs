using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Net;
using Newtonsoft.Json;
using System.Net.Http;
using System.Web;

namespace MobileGameServer.Server
{
    public sealed partial class AsyncOperation : IAsyncOperation
    {
        private HttpListener listener;
        private Task<HttpListenerContext> task;

        public AsyncOperation()
        {
            listener = new HttpListener();
            listener.Prefixes.Add(Server.Prefix);
            listener.Start();
            task = listener.GetContextAsync();
            Server.AddOperation(this);
        }

        private bool Done
        {
            get
            {
                return task.IsCompleted;
            }
        }

        public void End()
        {
            task.Dispose();
            listener.Close();
            Server.RemoveOperation(this);
            new AsyncOperation();
        }

        public void Update()
        {
            if (task != null && Done)
            {
                HttpListenerContext context = task.Result;
                HttpListenerRequest request = context.Request; //user request
                //parse request
                string query = request.Url.Query.Replace("?", "").Split('=')[0]; //get the first key
                try
                {
                    if (AsyncOperation.GETCalls[query](this, context, request))
                    {
                        return;
                    }
                }
                catch
                {
                    Console.WriteLine("errore");
                }
                //string[] values = request.QueryString.GetValues("DeviceID");
                
                //if(values != null && values.Length > 0)
                //{
                //    //response with json with best score of player
                //    HttpListenerResponse response = context.Response;
                //    int result = 0;
                //    bool succes = int.TryParse(values[0], out result);
                //    if(succes)
                //    {
                //        string outputJson = JsonConvert.SerializeObject(Server.GetScore(result));
                //        if (outputJson != null)
                //        {
                //            byte[] data = Encoding.UTF8.GetBytes(outputJson);
                //            response.OutputStream.Write(data, 0, data.Length);
                //            response.OutputStream.Close();
                //            End();
                //            return; //request satisfied
                //        }
                //    }
                //}

                //not an object request
                string requestUrl = request.Url.Query.Replace("?", "").ToLower();

                byte[] responseBuffer = null;
                try
                {
                    responseBuffer = HttpParser.ResolveRequest(HttpParser.GetContextFromString(request.HttpMethod), requestUrl)();
                    HttpListenerResponse response = context.Response;
                    response.OutputStream.Write(responseBuffer, 0, responseBuffer.Length);
                    response.OutputStream.Close();
                    
                }
                catch (NullReferenceException e)
                {
                    Console.WriteLine("async op aborted, request does not supported "+requestUrl);
                    HttpListenerResponse response = context.Response;
                    responseBuffer = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(new ErrorJson("Request not supported")));
                    response.OutputStream.Write(responseBuffer, 0, responseBuffer.Length);
                    response.OutputStream.Close();
                }
                finally
                {
                    End();
                }
            }
        }
    }
}
