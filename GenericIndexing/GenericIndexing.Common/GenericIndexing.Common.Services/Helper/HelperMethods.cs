
/*
 * File Name    : HelperMethods
 * Author       : pgaur
 * Date Created : 10/31/2009 12:39:16 PM
 */

using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace GenericIndexing.Common.Services.Helper
{
  public class HelperMethods
    {
      public static T Deserialize<T>(XmlDocument xmlDocument, Type type)
      {
          XmlSerializer ser = new XmlSerializer(type);
          xmlDocument.XmlResolver = null;
          StringReader reader = new StringReader(xmlDocument.InnerXml);
          XmlReader xmlReader = new XmlTextReader(reader);
          //Deserialize the object.
          return (T)ser.Deserialize(xmlReader);
      }

      public static T Deserialize<T>(XmlDocument xmlDocument)
      {
          XmlSerializer ser = new XmlSerializer(typeof(T));

          StringReader reader = new StringReader(xmlDocument.InnerXml);
          XmlReader xmlReader = new XmlTextReader(reader);
          //Deserialize the object.
          return (T)ser.Deserialize(xmlReader);
      }

      /// <summary>
      /// Check where a passed string contains all digit or not
      /// </summary>
      /// <param name="stringToCheck">string to check</param>
      /// <returns>True if passed string contain all digit, False otherwise</returns>
      public static bool IsNumber(string stringToCheck)
      {
          bool isNumber = false;
          for (int index = 0; index < stringToCheck.Length; index++)
          {
              isNumber = char.IsDigit(stringToCheck[index]);
              if (!isNumber)
              {
                  break;
              }
          }
          return isNumber;
      }

      /// <summary>
      /// Removes currently invlaid special characters from SOLR search query
      /// </summary>
      /// <param name="value">search query string</param>
      /// <returns>Formatted string after removing required special characters</returns>
      /// <remarks>Visit http://wiki.apache.org/solr/SolrQuerySyntax (Lucene Query Parser syntax) to fetch latest list of special characters</remarks>
      public static string RemoveSolrSpecialChars(string value)
      {
          string r = Regex.Replace(value, "(\\+|\\-|\\&\\&|\\|\\||\\!|\\{|\\}|\\[|\\]|\\^|\\(|\\)|\\\"|\\~|\\:|\\*|\\?|\\\\)", "\\$1");
          return r;
      }

      public static string GetBinarySerializedStream<TDataType>(TDataType objectToSerialize)
      {
          string binarySerializedData = null;
          BinaryFormatter formatter = new BinaryFormatter();
          MemoryStream memoryStream = new MemoryStream();
          try
          {
              formatter.Serialize(memoryStream, objectToSerialize);
              memoryStream.Flush();
              memoryStream.Position = 0;
              binarySerializedData = Convert.ToBase64String(memoryStream.ToArray());
          }
          catch (SerializationException e)
          {
              Console.WriteLine("Failed to serialize. Reason: " + e.Message);
              throw;
          }
          finally
          {
              formatter = null;
              memoryStream.Close();
              memoryStream = null;
          }

          return binarySerializedData;
      }

      public static TDataType GetObjectFromBinarySerializedStream<TDataType>(string binarySerializedStream)
      {
          byte[] byteArray = Convert.FromBase64String(binarySerializedStream);
          BinaryFormatter formatter = new BinaryFormatter();
          using (var memoryStream = new MemoryStream(byteArray))
          {
              memoryStream.Seek(0, SeekOrigin.Begin);
              return (TDataType)formatter.Deserialize(memoryStream);
          }
      }

      public static string GetJsonFromXmlString(string xml)
      {
          XmlDocument doc = new XmlDocument();
          doc.LoadXml(xml);
          string jsonText = JsonConvert.SerializeXmlNode(doc);

          return jsonText;
      }
    }
}
