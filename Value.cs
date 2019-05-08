using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ubidots
{
    public class Value : ApiObject
    {
        public class StatisticFigures
        {
            public const string MEAN = "mean";
            public const string VARIANCE = "variance";
            public const string MIN = "min";
            public const string MAX = "max";
            public const string COUNT = "count";
            public const string SUM = "sum";
        }

        public Value(ServerBridge.JsonData Raw, ApiClient Api) : base(Raw, Api) {}

        /// <summary>
        /// Get the name of the variable.
        /// </summary>
        /// <returns>The name of the variable</returns>
        public ServerBridge.JsonData GetDictionary()
        {
            return GetRawDictionary();
        }

        /// <summary>
        /// Gets the timestamp of the value.
        /// </summary>
        /// <returns>The timestamp of the value. Formatted in Unix Time</returns>
        public long GetTimestamp()
        {
            return GetRawDictionary().timestamp;
            //return (long)GetAttributeDouble("timestamp");
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        // <returns>The value</returns>
        public double GetValue() 
        {
            return GetRawDictionary().value;
            //return GetAttributeDouble("value");
        }

        /// <summary>
        /// Gets the timestamp of the value.
        /// </summary>
        /// <returns>The timestamp of the value. Formatted in Unix Time</returns>
        public long GetLastTimestamp()
        {
            return GetRawDictionary().last_value.timestamp;
            //return (long)GetAttributeDouble("timestamp");
        }

        /// <summary>
        /// Gets the value.
        /// </summary>
        // <returns>The value</returns>
        public double GetLastValue()
        {
            return GetRawDictionary().last_value.value;
            //return GetAttributeDouble("value");
        }

        ///// <summary>
        ///// Gets the value.
        ///// </summary>
        //// <returns>The value</returns>
        //public List<double> GetValues()
        //{
        //    List<double> temp = new List<double>();
        //    GetRawDictionary().results.ForEach((x) => temp.Add(x.value));
        //    return temp;
        //    //return GetAttributeDouble("value");
        //}

        ///// <summary>
        ///// Gets the timestamp of the value.
        ///// </summary>
        ///// <returns>The timestamp of the value. Formatted in Unix Time</returns>
        //public List<long> GetTimestamps()
        //{
        //    List<long> temp = new List<long>();
        //    GetRawDictionary().results.ForEach((x) => temp.Add(x.timestamp));
        //    return temp;
        //    //return GetAttributeDouble("value");
        //}
    }
}
