using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubidots
{
    public class ApiObject
    {
        private ServerBridge.JsonData Raw;
        protected ApiClient Api;
        protected ServerBridge Bridge;

        public ApiObject(ServerBridge.JsonData Raw, ApiClient Api)
        {
            this.Raw = Raw;
            this.Api = Api;
            Bridge = Api.GetServerBridge();
        }

        protected ServerBridge.JsonData GetRawDictionary()
        {
            return Raw;
        }

        //protected string GetAttributeString(string Attribute)
        //{
        //    return (string)Raw[Attribute];
        //}

        //protected double GetAttributeDouble(string Attribute)
        //{
        //    return (double)Raw[Attribute];
        //}

        //protected object GetAttribute(string Attribute)
        //{
        //    return Raw[Attribute];
        //}

        protected string GetId()
        {
            //return (string)Raw["id"];
            return Raw.id;
        }
    }
}
