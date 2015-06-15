using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Xml.Linq;

namespace GenericIndexing.Templating
{
    public static class XMLExtensions
    {
        public static XmlDocument ToXmlDocument(this XElement xDocument)
        {
            var xmlDocument = new XmlDocument();
            xmlDocument.Load(xDocument.CreateReader());
            return xmlDocument;
        }

        public static XElement ToXElement(this XmlDocument xmlDocument)
        {
            using (var nodeReader = new XmlNodeReader(xmlDocument))
            {
                nodeReader.MoveToContent();
                return XElement.Load(nodeReader);
               
            }
        }
    }
}
