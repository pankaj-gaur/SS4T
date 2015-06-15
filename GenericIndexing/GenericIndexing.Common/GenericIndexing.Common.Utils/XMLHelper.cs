///Name: XMLHelper.cs
///<remarks>
///History
///Date:			Author		Action
///-------------------------------------------------------------------
///06-27-2011		lrusta		Created
///</remarks>
///
///This is the helper util class for fetching XML Node from XML specified @ the path.

using System.Xml.XPath;
using System.Xml;
namespace GenericIndexing.Common.Utils
{
    public static class XMLHelper
    {
        /// <summary>
        /// Method for fetching the XML node
        /// </summary>
        /// <param name="xmlFilePath"></param>
        /// <param name="path"></param>
        /// <param name="parameter"></param>
        /// <returns>Object of XPathNodeIterator</returns>
        public static XPathNodeIterator GetXMLNode(string xPath, string parameter, string xmlFilePath)
        {
            XPathNodeIterator nodes;           
            XPathDocument document;
            document = LoadXPathDocument(xmlFilePath);
            nodes = GetNodesInXPathDcoument(xPath, parameter, document);     
            return nodes;
        }



        /// <summary>
        /// Method for fetching the XPathDocument      
        /// </summary>
        /// <param name="xmlFilePath"></param>
        /// <returns>Object of XPathNodeIterator</returns>
        public static XPathDocument LoadXPathDocument(string xmlFilePath)
        {
            XPathDocument document;
            document = new XPathDocument(xmlFilePath);
            return document;
        }


        /// <summary>
        /// Method for fetching the Nodes    
        /// </summary>
        /// <param name="path">Xpath to apply</param>
        /// <param name="xPathDcoument">Xpath Document</param>
        /// <returns>Object of XPathNodeIterator</returns>
        public static XPathNodeIterator GetNodesInXPathDcoument(string xPath, XPathDocument xPathDcoument)
        {
            XPathNodeIterator nodes;
            XPathNavigator navigator;
            navigator = xPathDcoument.CreateNavigator();
            navigator.MoveToRoot();
            nodes = navigator.Select(xPath);
            return nodes;
        }


        /// <summary>
        /// Method for fetching the Nodes     
        /// </summary>
        /// <param name="path">Xpath to apply</param>
        /// <param name="parameter"> parameter to fit in Xpath</param>
        /// <param name="xPathDcoument">Xpath Document</param>
        /// <returns>Object of XPathNodeIterator</returns>     
        public static XPathNodeIterator GetNodesInXPathDcoument(string xPath, string parameter, XPathDocument xPathDcoument)
        {
            XPathNodeIterator nodes;
            XPathNavigator navigator;
            navigator = xPathDcoument.CreateNavigator();
            navigator.MoveToRoot();
            nodes = navigator.Select(xPath + "'" + parameter + "']");
            return nodes;
        }


        /// <summary>
        /// Return Node list for given Xpath and XML file
        /// </summary>
        /// <param name="xPath">xpath</param>
        /// <param name="xmlFilePath">xmlfilepath</param>
        /// <returns>Node list</returns>
        public static XPathNodeIterator GetXMLNode(string xPath, string xmlFilePath)
        {
            XPathNodeIterator nodes;   
            XPathDocument document;
            document = LoadXPathDocument(xmlFilePath);
            nodes = GetNodesInXPathDcoument(xPath, document);            
            return nodes;
        }

        /// <summary>
        /// Method for fetching display name from XML
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="xmlFilePath"> XML file path</param> 
        /// <returns>display name as string</returns>
        public static string GetDisplaynamefromXML(string parameter, string xmlFilePath)
        {
           XPathNavigator nav;
           XPathDocument document = LoadXPathDocument(xmlFilePath);

           nav = document.CreateNavigator();
           nav.MoveToRoot();

           nav = document.CreateNavigator();
           nav.MoveToRoot();

           //Move to the first child node (comment field).
           nav.MoveToFirstChild();

           do
           {
               nav.MoveToFirstChild();
               do
               {
                   string id = nav.GetAttribute("location", "");
                   if (id == parameter)
                   {
                       XmlReader reader = nav.ReadSubtree();
                       //DisplayDetails(reader);
                   }
               }
               while (nav.MoveToNext());
           } while (nav.MoveToNext());

            return "";
        }
    }
}
            

            
    

