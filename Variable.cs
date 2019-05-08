using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Newtonsoft.Json;

namespace Ubidots
{
    public class Variable : ApiObject
    {
        public Variable(ServerBridge.JsonData Raw, ApiClient Api) : base(Raw, Api) { }

        /// <summary>
        /// Get the name of the variable.
        /// </summary>
        /// <returns>The name of the variable</returns>
        public ServerBridge.JsonData GetDictionary()
        {
            return GetRawDictionary();
        }

        /// <summary>
        /// Get the unit of the variable.
        /// </summary>
        // <returns>The unit of the value.</returns>
        //public Dictionary<string, object> GetRawLastValue()
        //{
        //    return JsonUtility.FromJson<Dictionary<string, object>>(GetAttribute("last_value").ToString());
        //}

        /// <summary>
        /// Get the name of the variable.
        /// </summary>
        /// <returns>The name of the variable</returns>
        public string GetName()
        {
            return GetRawDictionary().name;
            //return GetAttributeString("name");
        }

        /// <summary>
        /// Get the name of the variable.
        /// </summary>
        /// <returns>The name of the variable</returns>
        public string GetDescription()
        {
            return GetRawDictionary().description;
            //return GetAttributeString("description");
        }

        /// <summary>
        /// Get the name of the variable.
        /// </summary>
        /// <returns>The name of the variable</returns>
        public string GetCreatedAt()
        {
            return GetRawDictionary().last_value.created_at.ToString();
            //return GetAttribute("created_at").ToString();
        }

        /// <summary>
        /// Get the unit of the variable.
        /// </summary>
        // <returns>The unit of the value.</returns>
        public string GetUnit()
        {
            return GetRawDictionary().unit;
            //return GetAttributeString("unit");
        }

        /// <summary>
        /// Get the unit of the variable.
        /// </summary>
        // <returns>The unit of the value.</returns>
        public string GetLastActivity()
        {
            return GetRawDictionary().last_activity.ToString();
            //return GetAttribute("last_activity").ToString();
        }

        /// <summary>
        /// Get the unit of the variable.
        /// </summary>
        // <returns>The unit of the value.</returns>
        public double GetLastValue()
        {
            return GetRawDictionary().last_value.value;
            //return (double)GetRawLastValue()["value"];
        }

        /// <summary>
        /// Get the unit of the variable.
        /// </summary>
        // <returns>The unit of the value.</returns>
        public long GetLastValueTimestamp()
        {
            return GetRawDictionary().last_value.timestamp;
            //return GetRawLastValue()["timestamp"].ToString();
        }

        /// <summary>
        /// Deletes the variable and all its contents.
        /// </summary>
        public void Remove()
        {
            Bridge.Delete("variables/" + GetId());
        }

        /// <summary>
        /// Get all the values of the variable.
        /// </summary>
        /// <returns>The list of all the values of the variable</returns>
        public IEnumerator GetValues(System.Action<Value[]> values, float size = 20)
        {
            var outputMessage = "";
            yield return Extender.Instance.StartCoroutine(Bridge.Get("variables/" + GetId() + "/values/" + "?page=1&page_size=" + (size <= 0 ? 20 : size), result => outputMessage = result));

            ServerBridge.JsonData RawValues = JsonUtility.FromJson<ServerBridge.JsonData>(outputMessage);

            Value[] Values = new Value[RawValues.results.Count];

            if (!string.IsNullOrEmpty(RawValues.detail) || !string.IsNullOrEmpty(RawValues.details))
            {
                values(null);
            }
            else
            {
                for (var i = 0; i < RawValues.results.Count; i++)
                {
                    Values[i] = new Value(new ServerBridge.JsonData() { value = RawValues.results[i].value, timestamp = RawValues.results[i].timestamp, created_at = RawValues.results[i].created_at }, Api);
                }
            }

            values(Values);

            //Value[] Values = new Value[RawValues.results.Count];

            //for (var i = 0; i < RawValues.results.Count; i++)
            //{
            //    Values[i] = new Value(RawValues.results[i], Api);
            //}
        }

        /// <summary>
        /// Send a value to Ubidots API and save it.
        /// </summary>
        /// <param name="Value">The value to be saved</param>
        public IEnumerator SaveValue(double Value, System.Action<Value> values)
        {
            yield return Extender.Instance.StartCoroutine(SaveValue((int)Value, values));
        }

        /// <summary>
        /// Send a value to Ubidots API and save it.
        /// </summary>
        /// <param name="Value">The value to be saved</param>
        public IEnumerator SaveValue(int Value, System.Action<Value> values)
        {
            var outputMessage = "";

            ServerBridge.JsonData Data = new ServerBridge.JsonData() { value = Value, timestamp = GetTimestamp() };
            //Dictionary<string, object> Data = new Dictionary<string, object>();
            //Data.Add("value", Value);
            //Data.Add("timestamp", GetTimestamp());

            string Json = JsonUtility.ToJson(Data);

            Debug.Log(Json);

            yield return Extender.Instance.StartCoroutine(Bridge.Post("variables/" + GetId() + "/values", Json, result => outputMessage = result));

            ServerBridge.JsonData RawValues = JsonUtility.FromJson<ServerBridge.JsonData>(outputMessage);

            Value Values = new Value(new ServerBridge.JsonData() { value = RawValues.value, timestamp = RawValues.timestamp, created_at = RawValues.created_at }, Api);
            values(Values);
        }

        /// <summary>
        /// Send a value and a context to Ubidots API and save it.
        /// </summary>
        /// <param name="Value">The value to be saved</param>
        /// <param name="Context">The context to be saved</param>
        public IEnumerator SaveValue(double Value, ServerBridge.contextData Context, System.Action<Value> values)
        {
            yield return Extender.Instance.StartCoroutine(SaveValue((int)Value, Context, values));
        }

        /// <summary>
        /// Send a value and a context to Ubidots API and save it.
        /// </summary>
        /// <param name="Value">The value to be saved</param>
        /// <param name="Context">The context to be saved</param>
        public IEnumerator SaveValue(int Value, ServerBridge.contextData Context, System.Action<Value> values)
        {
            var outputMessage = "";

            ServerBridge.JsonData Data = new ServerBridge.JsonData() { value = Value, timestamp = GetTimestamp(), context= Context };
            //Dictionary<string, object> Data = new Dictionary<string, object>();
            //Data.Add("value", Value);
            //Data.Add("context", Context);
            //Data.Add("timestamp", GetTimestamp());

            string Json = JsonUtility.ToJson(Data);

            yield return Extender.Instance.StartCoroutine(Bridge.Post("variables/" + GetId() + "/values", Json, result => outputMessage = result));

            ServerBridge.JsonData RawValues = JsonUtility.FromJson<ServerBridge.JsonData>(outputMessage);

            Value Values = new Value(new ServerBridge.JsonData() { value = RawValues.value, timestamp = RawValues.timestamp, created_at = RawValues.created_at }, Api);
            values(Values);
        }

        /// <summary>
        /// Send a bulk of values to  Ubidots API and save them
        /// </summary>
        /// <remarks>Values and Timestamps arrays must have the same length</remarks>
        /// <param name="Values">The values to be sent</param>
        /// <param name="Timestamps">The timestamps of the values to be sent</param>
        public IEnumerator SaveValues(int[] Values, long[] Timestamps, System.Action<Value[]> values)
        {
            double[] Data = new double[Values.Length];

            for (var i = 0; i < Values.Length; i++)
            {
                Data[i] = (double)Values[i];
            }

            yield return Extender.Instance.StartCoroutine(SaveValues(Data, Timestamps, values));
        }

        /// <summary>
        /// Send a bulk of values to  Ubidots API and save them
        /// </summary>
        /// <remarks>Values and Timestamps arrays must have the same length</remarks>
        /// <param name="Values">The values to be sent</param>
        /// <param name="Timestamps">The timestamps of the values to be sent</param>
        public IEnumerator SaveValues(double[] Values, long[] Timestamps, System.Action<Value[]> values)
        {
            var outputMessage = "";

            if (Values == null || Timestamps == null)
            {
                throw new ArgumentNullException();
            }
            else if (Values.Length != Timestamps.Length)
            {
                throw new IndexOutOfRangeException("Values and Timestamps must"
                + " have the same values");
            }

            List<Dictionary<string, object>> ValuesList = new List<Dictionary<string, object>>();
            for (var i = 0; i < Values.Length; i++)
            {
                Dictionary<string, object> Data = new Dictionary<string, object>();
                Data.Add("value", Values[i]);
                Data.Add("timestamp", Timestamps[i]);
                ValuesList.Add(Data);
            }

            string Json = JsonUtility.ToJson(ValuesList);

            yield return Extender.Instance.StartCoroutine(Bridge.Post("variables/" + GetId() + "/values", Json, result => outputMessage = result));

            ServerBridge.JsonData RawValues = JsonUtility.FromJson<ServerBridge.JsonData>(outputMessage);


            Value[] tempValues = new Value[RawValues.results.Count];

            if (!string.IsNullOrEmpty(RawValues.detail) || !string.IsNullOrEmpty(RawValues.details))
            {
                values(null);
            }
            else
            {
                for (var i = 0; i < RawValues.results.Count; i++)
                {
                    tempValues[i] = new Value(new ServerBridge.JsonData() { value = RawValues.results[i].value, timestamp = RawValues.results[i].timestamp, created_at = RawValues.results[i].created_at }, Api);
                }
            }

            values(tempValues);
        }

        /// <summary>
        /// Get the mean of the values
        /// </summary>
        /// <returns>The mean of the values</returns>
        public IEnumerator GetMean(System.Action<double> variable)
        {
            yield return Extender.Instance.StartCoroutine(GetMean(result => variable(result), 0L, GetTimestamp()));
            //return GetMean(0L, GetTimestamp());
        }

        /// <summary>
        /// Get the mean of the values in a given time
        /// </summary>
        /// <param name="StartTime">Initial time to evaluate the values</param>
        /// <param name="EndTime">End time to evaluate the values</param>
        /// <returns>The mean of the values</returns>
        public IEnumerator GetMean(System.Action<double> variable, long StartTime, long EndTime)
        {
            yield return Extender.Instance.StartCoroutine(GetStatistics(result => variable(result), Value.StatisticFigures.MEAN, StartTime, EndTime));
            //return GetStatistics(Value.StatisticFigures.MEAN, StartTime, EndTime);
        }

        /// <summary>
        /// Get the variance of the values
        /// </summary>
        /// <returns>The variance of the values</returns>
        public IEnumerator GetVariance(System.Action<double> variable)
        {
            yield return Extender.Instance.StartCoroutine(GetVariance(result => variable(result), 0L, GetTimestamp()));
            //return GetVariance(0L, GetTimestamp());
        }

        /// <summary>
        /// Get the variance of the values in a given time
        /// </summary>
        /// <param name="StartTime">Initial time to evaluate the values</param>
        /// <param name="EndTime">End time to evaluate the values</param>
        /// <returns>The variance of the values</returns>
        public IEnumerator GetVariance(System.Action<double> variable, long StartTime, long EndTime)
        {
            yield return Extender.Instance.StartCoroutine(GetStatistics(result => variable(result), Value.StatisticFigures.VARIANCE, StartTime, EndTime));
            //return GetStatistics(Value.StatisticFigures.VARIANCE, StartTime, EndTime);
        }

        /// <summary>
        /// Get the minimum value among the values
        /// </summary>
        /// <returns>The minimum value among the values</returns>
        public IEnumerator GetMin(System.Action<double> variable)
        {
            yield return Extender.Instance.StartCoroutine(GetMin(result => variable(result), 0L, GetTimestamp()));
            //return GetMin(0L, GetTimestamp());
        }

        /// <summary>
        /// Get the minimum value among the values in a given time.
        /// </summary>
        /// <param name="StartTime">Initial time to evaluate the values</param>
        /// <param name="EndTime">End time to evaluate the values</param>
        /// <returns>The minimum value among the values</returns>
        public IEnumerator GetMin(System.Action<double> variable, long StartTime, long EndTime)
        {
            yield return Extender.Instance.StartCoroutine(GetStatistics(result => variable(result), Value.StatisticFigures.MIN, StartTime, EndTime));
            //return GetStatistics(Value.StatisticFigures.MIN, StartTime, EndTime);
        }

        /// <summary>
        /// Get the maximum value among the values
        /// </summary>
        /// <returns>The maximum value among the values</returns>
        public IEnumerator GetMax(System.Action<double> variable)
        {
            yield return Extender.Instance.StartCoroutine(GetMax(result => variable(result), 0L, GetTimestamp()));
            //return GetMax(0L, GetTimestamp());
        }

        /// <summary>
        /// Get the maximum value among the values in a given time.
        /// </summary>
        /// <param name="StartTime">Initial time to evaluate the values</param>
        /// <param name="EndTime">End time to evaluate the values</param>
        /// <returns>The maximum value among the values</returns>
        public IEnumerator GetMax(System.Action<double> variable, long StartTime, long EndTime)
        {
            yield return Extender.Instance.StartCoroutine(GetStatistics(result => variable(result), Value.StatisticFigures.MAX, StartTime, EndTime));
            //return GetStatistics(Value.StatisticFigures.MAX, StartTime, EndTime);
        }

        /// <summary>
        /// Get the count of the values
        /// </summary>
        /// <returns>The count of the values</returns>
        public IEnumerator GetCount(System.Action<double> variable)
        {
            yield return Extender.Instance.StartCoroutine(GetCount(result => variable(result), 0L, GetTimestamp()));
            //return GetCount(0L, GetTimestamp());
        }

        /// <summary>
        /// Get the count of the values in a given time
        /// </summary>
        /// <param name="StartTime">Initial time to evaluate the values</param>
        /// <param name="EndTime">End time to evaluate the values</param>
        /// <returns>The count of the values</returns>
        public IEnumerator GetCount(System.Action<double> variable, long StartTime, long EndTime)
        {
            yield return Extender.Instance.StartCoroutine(GetStatistics(result => variable(result), Value.StatisticFigures.COUNT, StartTime, EndTime));
            //return GetStatistics(Value.StatisticFigures.COUNT, StartTime, EndTime);
        }

        /// <summary>
        /// Get the sum of the values
        /// </summary>
        /// <returns>The sum of the values</returns>
        public IEnumerator GetSum(System.Action<double> variable)
        {
            yield return Extender.Instance.StartCoroutine(GetSum(result => variable(result), 0L, GetTimestamp()));
            //return GetSum(0L, GetTimestamp());
        }

        /// <summary>
        /// Get the sum of the values in a given time
        /// </summary>
        /// <param name="StartTime">Initial time to evaluate the values</param>
        /// <param name="EndTime">End time to evaluate the values</param>
        /// <returns>The sum of the values</returns>
        public IEnumerator GetSum(System.Action<double> variable, long StartTime, long EndTime)
        {
            yield return Extender.Instance.StartCoroutine(GetStatistics(result => variable(result), Value.StatisticFigures.SUM, StartTime, EndTime));
            //return GetStatistics(Value.StatisticFigures.SUM, StartTime, EndTime);
        }

        /// <summary>
        /// Get the timestamp in Unix time
        /// </summary>
        /// <returns>The current time taken from January 1 1970</returns>
        private long GetTimestamp()
        {
            DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
            UnityEngine.Debug.Log(Jan1st1970 + " - " + (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds);

            return (long)(DateTime.UtcNow - Jan1st1970).TotalMilliseconds;
        }

        /// <summary>
        /// Method to get the response of the given figure
        /// </summary>
        /// <param name="Figure">The figure we want to get.</param>
        /// <param name="StartTime">Initial time to get the statistics.</param>
        /// <param name="EndTime">Final time to get the statistics.</param>
        /// <returns>The response with the result from the server</returns>
        public IEnumerator GetStatistics(System.Action<double> variable, String Figure, long StartTime, long EndTime)
        {
            var outputMessage = "";
            yield return Extender.Instance.StartCoroutine(Bridge.Get("variables/" + GetId() + "/statistics/" + Figure + "/" + StartTime + "/" + EndTime, result => outputMessage = result));

            Dictionary<string, object> RawValues = JsonUtility.FromJson<Dictionary<string, object>>(outputMessage);

            variable((double)RawValues[Figure]);
        }
    }
}