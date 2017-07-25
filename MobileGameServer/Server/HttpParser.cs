using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Web.Http;
using System.Net;



namespace MobileGameServer.Server
{
    public enum ParserContext { GET,POST}
    public static class HttpParser
    {
        public delegate byte[] ParseMethod();

        private static Dictionary<ParserContext, Dictionary<string,ParseMethod>> parser;

        static HttpParser()
        {
            parser = new Dictionary<ParserContext, Dictionary<string, ParseMethod>>();
            parser.Add(ParserContext.GET, new Dictionary<string, ParseMethod>());
            //fill
            parser[ParserContext.GET].Add("sayhelloworld", Methods.SendHello);

            parser.Add(ParserContext.POST, new Dictionary<string, ParseMethod>());
            //fill
        }

        public static ParserContext GetContextFromString(string httpMethod)
        {
            if (httpMethod == "GET")
                return ParserContext.GET;
            if (httpMethod == "POST")
                return ParserContext.POST;

            return ParserContext.GET;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns>Return a list of existed commands </returns>
        private static Dictionary<string, ParseMethod> GetParserContext(ParserContext context)
        {
            return parser[context];
        }

        /// <summary>
        /// Return a method that resolve correctly the request, if there is not request saved return null
        /// </summary>
        /// <param name="context"></param>
        /// <param name="requesttext">must be remove the '?' (interrogative point) from request</param>
        /// <returns></returns>
        public static ParseMethod ResolveRequest(ParserContext context,string requesttext)
        {
            Dictionary<string, ParseMethod> currentContext = GetParserContext(context);
            if(currentContext.ContainsKey(requesttext))
            {
                return currentContext[requesttext];
            }
            return null;
        }


        private static class Methods
        {
            public static byte[] SendHello()
            {
                string response = "Hello World!";
                return Encoding.UTF8.GetBytes(response);
            }
        }
    }
}
