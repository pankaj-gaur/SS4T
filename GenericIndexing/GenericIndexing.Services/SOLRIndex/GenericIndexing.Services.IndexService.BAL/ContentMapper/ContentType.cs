using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using System.Linq;
using GenericIndexing.Common.Logging;

namespace GenericIndexing.IndexService.BAL
{
    /// <summary>
    /// Description of ContentType.
    /// </summary>
    public class ContentType
    {
        string typeName;
        
        public string contentTypeName 
        {
            get {
                return this.typeName;
            }
        }
        
        List<ContentTypeField> cmFields;
        
        public List<ContentTypeField> fields
        {
            get {
                return this.cmFields;
            }
        }

        public List<string> CDATAfyNodeXpathList { get; set; }
        
        ContentType childContentType;
        
        public ContentType child_type
        {
            get {
                return this.childContentType;
            }
        }
        
        string XPATH_CONTENT_TYPE_FIELDS = ConfigurationManager.AppSettings.Get("XPATH_CONTENT_TYPE_FIELDS");
        string XPATH_CONTENT_TYPE_RTF_NODES = ConfigurationManager.AppSettings.Get("XPATH_CONTENT_TYPE_RTF_NODES");
        string XPATH_CHILD_TYPE = ConfigurationManager.AppSettings.Get("XPATH_CHILD_TYPE");
        string content_definition_dir = ConfigurationManager.AppSettings.Get("CONTENT_TYPES_DIR");
        
        bool isMultivalued;
        
        public bool is_multi_valued
        {
            get {
                return this.isMultivalued;
            }
        }
        
        string xPath;
        
        public string xpath {
            get {
                return this.xPath;
            }
        }

        public ContentType(string contentType)
        {
            this.typeName = contentType;
            
            XmlDocument xmlDoc = GetContentTypeDefinitionXML(contentType);
            this.CDATAfyNodeXpathList = GetCDatafyAbleNodesXpath(xmlDoc);
            this.cmFields = GetFieldsFromContentTypeXML(xmlDoc.DocumentElement);
            
            XmlNode childContentTypeNode = xmlDoc.SelectSingleNode(XPATH_CHILD_TYPE);
            if(childContentTypeNode != null){
                this.childContentType = new ContentType(childContentTypeNode);
            }
        }
        
        private ContentType(XmlNode xmlnode)
        {
            this.typeName = xmlnode.Attributes["name"].Value;
            this.cmFields = GetFieldsFromContentTypeXML(xmlnode);
            this.CDATAfyNodeXpathList = GetCDatafyAbleNodesXpath(xmlnode);
            this.isMultivalued = (xmlnode.Attributes["multivalued"].Value == "true"? true: false);
            this.xPath = xmlnode.Attributes["xpath"].Value;
        }
        
        private List<ContentTypeField> GetFieldsFromContentTypeXML(XmlNode xnod)
        {
            XmlNodeList fieldlist = xnod.SelectNodes(this.XPATH_CONTENT_TYPE_FIELDS);
            
            List<ContentTypeField> ctFields = new List<ContentTypeField>();
            
            foreach (XmlNode fielddef in fieldlist){
                ctFields.Add(GetContentTypeField(fielddef));
            }
            
            return ctFields;
        }

        private List<string> GetCDatafyAbleNodesXpath(XmlNode xnod)
        {
            XmlNodeList nodelist = xnod.SelectNodes(this.XPATH_CONTENT_TYPE_RTF_NODES);

            List<string> cdatafynodes = new List<string>();

            if (nodelist != null && nodelist.Count > 0)
            {
                foreach (XmlNode node in nodelist)
                {
                    cdatafynodes.Add(node.InnerXml);
                    SS4TLogger.WriteLog(ELogLevel.INFO, node.InnerXml);
                }
            }
            else
            {
                SS4TLogger.WriteLog(ELogLevel.INFO, "No CDATAFyable Nodes");
            }
            return cdatafynodes;
        }

        private XmlDocument GetContentTypeDefinitionXML(string contentType){
            
            XmlDocument xmlDoc = new XmlDocument();

            xmlDoc.Load(content_definition_dir + "//" + contentType.Replace(":", "_") + ".xml");
            
            return xmlDoc;
        }
        
        private static ContentTypeField GetContentTypeField(XmlNode xn){
            return new ContentTypeField(
                        xn.Attributes["name"].Value,
                        xn.Attributes["type"].Value,
                        (Utility.GetXMLAttributeValue(xn, "multivalued") == "true" ? true: false),
                        Utility.GetXMLAttributeValue(xn, "defaultvalue", String.Empty),
                        (Utility.GetXMLAttributeValue(xn, "isconstant") == "true" ? true: false),
                        Utility.GetXMLAttributeValue(xn, "xpath", String.Empty),
                        Utility.GetXMLAttributeValue(xn, "fielddelimiter", ConfigurationManager.AppSettings.Get("LINK_OBJECT_DELIMITER")),
                        Utility.GetXMLAttributeValue(xn, "transform", String.Empty),
                        (Utility.GetXMLAttributeValue(xn, "iskey") == "true" ? true: false),
                        xn.Attributes["targetfield"].Value);
        }
    }
}
