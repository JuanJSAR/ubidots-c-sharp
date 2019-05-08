using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Newtonsoft.Json;

namespace Ubidots
{
    public class ApiClient
    {
        private ServerBridge Bridge;

        private DataSource dataSource;

        public ApiClient(string ApiKey)
        {
            Bridge = new ServerBridge(ApiKey);
        }

        public ApiClient(string ApiKey, System.Action onClomplete)
        {
            Bridge = new ServerBridge(ApiKey);
        }

        public ApiClient(string ApiKey, string BaseUrl)
        {
            Bridge = new ServerBridge(ApiKey, BaseUrl);
        }

        public IEnumerator StartService(System.Action onClomplete)
        {
            yield return Extender.Instance.StartCoroutine(Bridge.ServerBridgeStart(onClomplete));
        }

        /// <summary>
        /// Gets the ServerBridge used in this ApiClient.
        /// </summary>
        /// <returns>The ServerBridge used in this ApiClient</returns>
        internal ServerBridge GetServerBridge()
        {
            return Bridge;
        }

        /// <summary>
        /// Gets all the DataSources in the user account.
        /// </summary>
        /// <returns>A list with all the DataSources</returns>
        public IEnumerator GetDataSources(System.Action<DataSource[]> dataSources)
        {
            var outputMessage = "";
            yield return Extender.Instance.StartCoroutine(Bridge.Get("datasources", result => outputMessage = result));

            List<ServerBridge.JsonData> RawValues = JsonUtility.FromJson<List<ServerBridge.JsonData>>(outputMessage);

            DataSource[] DataSources = new DataSource[RawValues.Count];

            for (var i = 0; i < RawValues.Count; i++)
            {
                DataSources[i] = new DataSource(RawValues[i], this);
            }

            dataSources(DataSources);
        }


        /// <summary>
        /// Gets a single DataSource in the user account
        /// </summary>
        /// <param name="Id">The DataSource Id</param>
        /// <returns>The DataSource wanted by the user</returns>
        public IEnumerator GetDataSource(string Id, System.Action<DataSource> dataSources)
        {
            var outputMessage = "";
            yield return Extender.Instance.StartCoroutine(Bridge.Get("datasources/" + Id, result => outputMessage = result));

            ServerBridge.JsonData RawValues = JsonUtility.FromJson<ServerBridge.JsonData>(outputMessage);

            if (!string.IsNullOrEmpty(RawValues.detail) || !string.IsNullOrEmpty(RawValues.details))
            {
                dataSources(null);
            }
            else
            {
                dataSources(new DataSource(RawValues, this));
            }
        }
        /// <summary>
        /// Creates a DataSource in the user account
        /// </summary>
        /// <param name="Name">The name of the DataSource</param>
        /// <returns>The newly created DataSource</returns>
        public IEnumerator CreateDataSource(string Name, System.Action<DataSource> values)
        {
            yield return Extender.Instance.StartCoroutine(CreateDataSource(Name, null, null, values));
        }

        /// <summary>
        /// Creates a DataSource in the user account
        /// </summary>
        /// <param name="Name">The name of the DataSource</param>
        /// <param name="Context">The context of the DataSource</param>
        /// <param name="Tags">The tags of the DataSource</param>
        /// <returns>The newly created DataSource</returns>
        public IEnumerator CreateDataSource(string Name, Dictionary<string, string> Context, string[] Tags, System.Action<DataSource> values)
        {

            var outputMessage = "";

            if (Name == null)
            {
                throw new ArgumentNullException();
            }

            Dictionary<string, object> Data = new Dictionary<string, object>();
            Data.Add("name", Name);

            if (Context != null)
                Data.Add("context", Context);

            if (Tags != null)
                Data.Add("tags", Tags);

            yield return Extender.Instance.StartCoroutine(Bridge.Post("datasources/", JsonUtility.ToJson(Data), result => outputMessage = result));

            DataSource Var = new DataSource(JsonUtility.FromJson<ServerBridge.JsonData>(outputMessage), this);

            values(Var);
        }

        public string GetToken()
        {
            return Bridge.GetToken();
        }

        public void SetDefaultToken(string token)
        {
            Bridge.SetDefaultToken(token);
        }

        /// <summary>
        /// Get all the variables of the user account
        /// </summary>
        /// <returns>A list with all the variables</returns>
        public IEnumerator GetVariables(System.Action<Variable[]> variable, float size = 20)
        {
            var outputMessage = "";
            yield return Extender.Instance.StartCoroutine(Bridge.Get("variables/" + "?page=1&page_size=" + (size <= 0 ? 20 : size), result => outputMessage = result));


            List<ServerBridge.JsonData> RawValues = JsonUtility.FromJson<List<ServerBridge.JsonData>>(outputMessage);

            Variable[] Variables = new Variable[RawValues.Count];

            for (var i = 0; i < RawValues.Count; i++)
            {
                Variables[i] = new Variable(RawValues[i], this);
            }

            variable(Variables);
        }

        ///// <summary>
        ///// Get a specific Variable in the user account
        ///// </summary>
        ///// <param name="Id">The ID of the Variable</param>
        ///// <returns>The Variable with that ID</returns>
        //public IEnumerator SetVariable(string Id, System.Action<Variable> variable)
        //{
        //    var outputMessage = "";
        //    yield return Extender.Instance.StartCoroutine(Bridge.Get("variables/" + Id + "/values/", result => outputMessage = result));

        //    ServerBridge.JsonData RawValues = JsonUtility.FromJson<ServerBridge.JsonData>(outputMessage);

        //    if (!string.IsNullOrEmpty(RawValues.detail) || !string.IsNullOrEmpty(RawValues.details))
        //    {
        //        variable(null);
        //    }
        //    else
        //    {
        //        variable(new Variable(RawValues, this));
        //    }
        //}

        /// <summary>
        /// Get a specific Variable in the user account
        /// </summary>
        /// <param name="Id">The ID of the Variable</param>
        /// <returns>The Variable with that ID</returns>
        public IEnumerator GetVariable(string Id, System.Action<Variable> variable)
        {
            var outputMessage = "";
            yield return Extender.Instance.StartCoroutine(Bridge.Get("variables/" + Id, result => outputMessage = result));

            ServerBridge.JsonData RawValues = JsonUtility.FromJson<ServerBridge.JsonData>(outputMessage);

            if (!string.IsNullOrEmpty(RawValues.detail) || !string.IsNullOrEmpty(RawValues.details))
            {
                variable(null);
            }
            else
            {
                variable(new Variable(RawValues, this));
            }
        }

        // Testing purposes
        public void SetServerBridge(ServerBridge Bridge)
        {
            this.Bridge = Bridge;
        }
    }
}
