using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
//using Newtonsoft.Json;
//using Newtonsoft.Json.Linq;
using UnityEngine;
using static AdminNetworking;

namespace Ubidots
{
    public class ServerBridge
    {
        /* Constant configuration variables */
        public const string DEFAULT_BASE_URL = "https://things.ubidots.com/api/";
        public const string DEFAULT_BASE_VERSION = "v1.6";

        /* Instance variables */
        private string BaseUrl;
        private string ApiKey;
        private string Token;
        private List<Headers> TokenHeader;
        private List<Headers> ApiKeyHeader;

        public ServerBridge(string ApiKey) : this(ApiKey, DEFAULT_BASE_URL, DEFAULT_BASE_VERSION) { }

        public ServerBridge(string ApiKey, string BaseUrl) : this(ApiKey, BaseUrl, DEFAULT_BASE_VERSION) { }

        public ServerBridge(string ApiKey, string BaseUrl, string BaseVersion)
        {
            this.ApiKey = ApiKey;
            this.BaseUrl = BaseUrl + BaseVersion + "/";
            //Token = null;

            //ApiKeyHeader = new List<Headers>();
            //ApiKeyHeader.Add(new Headers() { name = "X-UBIDOTS-APIKEY", value = this.ApiKey });

            //AdminWebData.Instance.StartCoroutine(ReceiveToken(onClomplete));
        }

        public IEnumerator ServerBridgeStart(System.Action onClomplete)
        {
            Token = null;

            ApiKeyHeader = new List<Headers>();
            ApiKeyHeader.Add(new Headers() { name = "X-UBIDOTS-APIKEY", value = this.ApiKey });

            yield return Extender.Instance.StartCoroutine(ReceiveToken(onClomplete));
        }

        /// <summary>
        /// Receives the token from Ubidots and save it in Token variable
        /// </summary>
        private IEnumerator ReceiveToken(System.Action onClomplete)
        {
            var outputMessage = "";
            yield return Extender.Instance.StartCoroutine(PostWithApiKey("auth/token", result => outputMessage = result));

            Token = JsonUtility.FromJson<JsonToken>(outputMessage).token;

            TokenHeader = new List<Headers>();
            TokenHeader.Add(new Headers() { name = "X-AUTH-TOKEN", value = Token });

            onClomplete.Invoke();
        }

        /// <summary>
        /// Prepare the headers for the HTTP Request
        /// </summary>
        /// <param name="CustomHeaders" /> The headers to combine in one WebHeaderCollection
        /// <returns>The combination of the headers to be sent to Ubidots</returns>
        private List<Headers> PrepareHeaders(List<Headers> CustomHeaders)
        {
            List<Headers> Headers = new List<Headers>();

            CustomHeaders.ForEach((x) => Headers.Add(x));

            return Headers;
        }

        public string GetToken()
        {
            return Token;
        }

        public void SetDefaultToken(string token)
        {
            Token = token;
        }

        /// <summary>
        /// Send a POST request to Ubidots using the API Key
        /// Generally this is used for getting the token, nothing else.
        /// </summary>
        /// <param name="Path" /> The path were we're going to query in Ubidots API
        /// <returns>A string containing the response of the server</returns>
        public IEnumerator PostWithApiKey(string Path, System.Action<string> outputMessage)
        {
            string Response = null;
            //List<Headers> Headers = PrepareHeaders(ApiKeyHeader);
            string Url = BaseUrl + Path;

            //Headers.Add(new AdminWebData.Headers() { name = "Content-Type", value = "application/json" });
            yield return Extender.Instance.StartCoroutine(SendRequest(Url, MethodsHTTP.POST, delegate (bool state, string message) { Response = message; }, headers: new List<Headers>() { new Headers() { name = "X-UBIDOTS-APIKEY", value = this.ApiKey }, new Headers() { name = "Content-Type", value = "application/json" } }));

            outputMessage(Response);
        }


        /// <summary>
        /// Send a GET request to Ubidots using the Token
        /// </summary>
        /// <param name="Path" />  The path were we're going to query in Ubidots API
        /// <returns>A string containing the response of the server</returns>
        public IEnumerator Get(string Path, System.Action<string> outputMessage)
        {
            string Response = null;
            //List<Headers> Headers =  PrepareHeaders(TokenHeader);
            string Url = BaseUrl + Path;
            //Debug.Log(Url);
            //try
            //{
            //Headers.Add(new AdminWebData.Headers() { name = "Content-Type", value = "application/json" });
            //AdminWebData.Instance.StartCoroutine(SendRequest(Url, MethodsHTTP.GET, outputMessage, headers: Headers));
            yield return Extender.Instance.StartCoroutine(SendRequest(Url, MethodsHTTP.GET, delegate (bool state, string message) { Response = message; }, headers: new List<Headers>() { new Headers() { name = "X-AUTH-TOKEN", value = Token }, new Headers() { name = "Content-Type", value = "application/json" } }));

            outputMessage(Response);
            //}
            //catch (Exception e)
            //{
            //    Console.WriteLine(e.Message);
            //}

            /*try
            {
                HttpWebRequest Request = WebRequest.Create(Url) as HttpWebRequest;
                Request.Method = "GET";
                Request.Headers = Headers;
                Request.ContentType = "application/json";

                using (HttpWebResponse WebResponse = Request.GetResponse() as HttpWebResponse)
                {
                    using (Stream ResponseStream = WebResponse.GetResponseStream())
                    {
                        StreamReader Reader = new StreamReader(ResponseStream, Encoding.UTF8);
                        Response = Reader.ReadToEnd();

                        if (JObject.Parse(Response).GetValue("results") != null)
                        {
                            Response = JObject.Parse(Response).GetValue("results").ToString();
                        }

                        return Response;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }*/
        }

        /// <summary>
        /// Send a POST request to Ubidots using the Token
        /// </summary>
        /// <param name="Path" />  The path were we're going to query in Ubidots API
        /// <param name="Json" /> The json we're going to send to the server
        /// <returns>A string containing the response of the server</returns>
        public IEnumerator Post(string Path, string Json, System.Action<string> outputMessage)
        {
            string Response = null;
            //List<Headers> Headers = PrepareHeaders(TokenHeader);

            string Url = BaseUrl + Path;

            yield return Extender.Instance.StartCoroutine(SendRequest(Url, MethodsHTTP.POST, delegate (bool state, string message) { Response = message; }, headers: new List<Headers>() { new Headers() { name = "X-AUTH-TOKEN", value = Token }, new Headers() { name = "Content-Type", value = "application/json" } }, json:Json));

            outputMessage(Response);
            /*try
            {
                HttpWebRequest Request = WebRequest.Create(Url) as HttpWebRequest;
                Request.Method = "POST";
                //Request.Headers = Headers;
                Request.ContentType = "application/json";
                Request.Accept = "application/json";

                using (StreamWriter SWriter = new StreamWriter(Request.GetRequestStream()))
                {
                    SWriter.Write(Json);
                    SWriter.Flush();
                }

                using (HttpWebResponse WebResponse = Request.GetResponse() as HttpWebResponse)
                {
                    using (Stream ResponseStream = WebResponse.GetResponseStream())
                    {
                        StreamReader Reader = new StreamReader(ResponseStream, Encoding.UTF8);
                        Response = Reader.ReadToEnd();
                        return Response;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }*/
        }

        /// <summary>
        /// Send a DELETE request to Ubidots using the Token
        /// </summary>
        /// <param name="Path" />  The path were we're going to query in Ubidots API
        /// <returns>A string containing the response of the server</returns>
        public string Delete(string Path)
        {
            string Response = null;
            List<Headers> Headers = PrepareHeaders(TokenHeader);

            string Url = BaseUrl + Path;
            try
            {
                HttpWebRequest Request = WebRequest.Create(Url) as HttpWebRequest;
                Request.Method = "DELETE";
                //Request.Headers = Headers;
                Request.ContentType = "application/json";

                using (HttpWebResponse WebResponse = Request.GetResponse() as HttpWebResponse)
                {
                    using (Stream ResponseStream = WebResponse.GetResponseStream())
                    {
                        StreamReader Reader = new StreamReader(ResponseStream, Encoding.UTF8);
                        Response = Reader.ReadToEnd();
                        return Response;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        [System.Serializable]
        public class JsonToken
        {
            public string token;
        }

        [System.Serializable]
        public class JsonData
        {
            public string id;
            public float value;
            public long timestamp;
            public string name;
            public string unit;
            public string label;
            public string description;
            public int last_activity;
            public int created_at;
            public LastValueData last_value;
            public List<LastValueData> results;
            public contextData context;

            public string details;
            public string detail;
        }

        [System.Serializable]
        public class LastValueData
        {
            public string id;
            public float value;
            public long timestamp;
            public int created_at;
        }

        [System.Serializable]
        public class contextData
        {
            public string property1;
            public string property2;
        }
    }
}
