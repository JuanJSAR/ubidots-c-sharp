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
    public class DataSource : ApiObject
    {
        public DataSource(ServerBridge.JsonData Raw, ApiClient Api) : base(Raw, Api) { }

        /// <summary>
        /// Get the name of the Datasource
        /// </summary>
        /// <returns>The name of the Datasource</returns>
        public string GetName()
        {
            return GetRawDictionary().name;
            //return GetAttributeString("name");
        }

        /// <summary>
        /// Deletes the Datasource and all its contents.
        /// </summary>
        public void Remove()
        {
            Bridge.Delete("datasources/" + GetId());
        }

        /// <summary>
        /// Get the Variables of a Datasource
        /// </summary>
        /// <returns>The list of Variables in a Datasource</returns>
        public IEnumerator GetVariables(System.Action<Variable[]> variable)
        {
            var outputMessage = "";
            yield return Extender.Instance.StartCoroutine(Bridge.Get("datasources/" + GetId() + "/variables", result => outputMessage = result));

            List<ServerBridge.JsonData> RawValues = JsonUtility.FromJson<List<ServerBridge.JsonData>>(outputMessage);

            Variable[] Variables = new Variable[RawValues.Count];

            for (var i = 0; i < RawValues.Count; i++)
            {
                Variables[i] = new Variable(RawValues[i], Api);
            }

            variable(Variables);
        }

        /// <summary>
        /// Creates a new Variable in the Datasource
        /// </summary>
        /// <param name="Name">The name of the new Variable</param>
        /// <returns>The newly created Variable</returns>
        public IEnumerator CreateVariable(string Name, System.Action<Variable> variable)
        {
            yield return Extender.Instance.StartCoroutine(CreateVariable(Name, null, null, null, null, variable));
        }

        /// <summary>
        /// Creates a new Variable in the Datasource
        /// </summary>
        /// <param name="Name">The name of the new Variable</param>
        /// <param name="Unit">The units of the new Variable</param>
        /// <param name="Description">The description of the new 
        /// variable</param>
        /// <param name="Properties">The properties of the new Variable</param>
        /// <param name="Tags">The tags of the new Variable</param>
        /// <returns>The newly created Variable</returns>
        public IEnumerator CreateVariable(string Name, string Unit, string Description, Dictionary<string, string> Properties, string[] Tags, System.Action<Variable> variable)
        {
            var outputMessage = "";

            if (Name == null)
            {
                throw new ArgumentNullException();
            }

            Dictionary<string, object> Data = new Dictionary<string, object>();
            Data.Add("name", Name);

            if (Unit != null)
                Data.Add("unit", Unit);

            if (Description != null)
                Data.Add("description", Description);

            if (Properties != null)
                Data.Add("properties", Properties);

            if (Tags != null)
                Data.Add("tags", Tags);

            yield return Extender.Instance.StartCoroutine(Bridge.Post("datasources/" + GetId() + "/variables", JsonUtility.ToJson(Data), result => outputMessage = result));

            Variable Var = new Variable(JsonUtility.FromJson<ServerBridge.JsonData>(outputMessage), Api);

            variable(Var);
        }
    }
}
