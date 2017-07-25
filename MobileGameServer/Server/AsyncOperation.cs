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
using System.IO;

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
                string query = request.Url.Query.Replace("?", "").Split('=')[0]; //get the first key



                //is post request
                if (request.HttpMethod == "POST")
                {
                    AsyncOperation.POSTCalls["DeviceID"](this, context, request);
                }


                //parse request
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
                    Console.WriteLine("async op aborted, request does not supported " + requestUrl);
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
