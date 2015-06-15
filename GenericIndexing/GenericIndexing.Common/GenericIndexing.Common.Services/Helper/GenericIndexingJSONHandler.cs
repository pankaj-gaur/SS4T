///<remarks>
///====================================================================
/// Name:GenericIndexingJSONHandler.cs
/// Description: 
/// 
/// Construction Date: 22/10/2009
/// Author: Rajat Bhatia
/// Last Revision Date:		
/// Last Revision By:		
/// Last Revision Change:	
/// ====================================================================
/// Copyright (C) 2009 - 2010
/// ====================================================================
/// </remarks>
/// 

using System;
using System.Data;
using System.Configuration;
using System.Linq;
using System.Xml.Linq;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Runtime.Serialization.Json;

namespace GenericIndexing.Common.Services.Helper
{
    /// <summary>
    /// GenericIndexingJSONHandler is a static class that handles both serialization of objects into a JSON string
    /// and deserialization of a JSON string back to an object.
    /// </summary>
    public static class GenericIndexingJSONHandler
    {
        /// <summary>
        /// This method serializes a given object into its equivalent JSON string
        /// </summary>
        /// <param name="obj">Object to be converted to JSON string</param>
        /// <returns>JSON string equivalent to the passed object </returns>
        public static string ToJSONText(this object obj)
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(obj.GetType());
            MemoryStream ms = new MemoryStream();
            serializer.WriteObject(ms, obj);
            string retVal = Encoding.UTF8.GetString(ms.ToArray());
            ms.Dispose();
            return retVal;
        }        


        /// <summary>
        /// This method deserializes back a JSON string into a given equivalent object
        /// </summary>
        /// <typeparam name="T">Type of object to which the JSON string will be serialized into</typeparam>
        /// <param name="JSONString">The string that needs to be deserialized back into the passed object type</param>
        /// <returns></returns>
        public static T ToObject<T>(string JSONString)
        {
            T obj = Activator.CreateInstance<T>();
            DataContractJsonSerializer serializer;
            MemoryStream streamData;

            serializer = new DataContractJsonSerializer(typeof(T));

            using (streamData = new MemoryStream(Encoding.UTF8.GetBytes(JSONString)))
            {
                obj = (T)serializer.ReadObject(streamData);
            }
            
            return obj;

        }
    }
}
