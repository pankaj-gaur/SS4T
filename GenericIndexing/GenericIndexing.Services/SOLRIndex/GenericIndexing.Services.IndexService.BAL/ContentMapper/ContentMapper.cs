using System;
using System.Collections.Generic;
using System.Xml;
using System.Linq;
using GenericIndexing.Common.Logging;
using GenericIndexing.Common.Services.Helper;
using System.Xml.Linq;
using System.Text.RegularExpressions;

namespace GenericIndexing.IndexService.BAL
{
    /// <summary>
    /// Description of ContentMapper.
    /// </summary>
    public static class ContentMapper
    {
        public static MappedContent GetMappedContent(XmlDocument componentPresentation,
                                                                 XmlNode xroot = null,
                                                                 Boolean processChild = true,
                                                                 ContentType contentType = null)
        {
            SS4TLogger.WriteLog(ELogLevel.INFO, "Entering Method: ContentMapper.GetMappedContent");
            object target_field_value;
            ContentType childType;
            List<Dictionary<string, object>> docList = new List<Dictionary<string, object>>();
            List<KeyValuePair<string, object>> deleteCriteriaList = new List<KeyValuePair<string, object>>();
            Dictionary<string, object> mappedContent = new Dictionary<string, object>();
            XmlNamespaceManager xmlnsManager = new XmlNamespaceManager(componentPresentation.NameTable);

            AddNamespace(xmlnsManager);

            if (xroot == null)
            {
                xroot = componentPresentation.DocumentElement;
            }

            if (contentType == null)
            {
                contentType = new ContentType(Utility.GetContentType(componentPresentation));
            }


            foreach (string cdatafyNodeXpath in contentType.CDATAfyNodeXpathList)
            {
                XmlNodeList nodes = xroot.SelectNodes(cdatafyNodeXpath, xmlnsManager);
                if (nodes != null && nodes.Count > 0)
                {
                    foreach (XmlNode node in nodes)
                    {
                        CDatafyXMLNode(componentPresentation, node);
                        mappedContent.Add(node.Name, node.InnerXml);
                        SS4TLogger.WriteLog(ELogLevel.DEBUG, "CDATAFYED: " + node.InnerText);
                    }
                }
            }
            mappedContent.Add(SolrServicesConstants.ContentType, contentType.contentTypeName);

            SS4TLogger.WriteLog(ELogLevel.INFO, "Content Type is: " + contentType.contentTypeName);

            childType = contentType.child_type;

            foreach (ContentTypeField field in contentType.fields)
            {
                string xpath = field.cpxpath;

                if (field.is_constant)
                {
                    target_field_value = field.default_value;
                }
                else if (field.IsAutoDateField())
                {
                    target_field_value = DateTime.Now.ToString();
                }
                else if (field.IsCurrentPositionNumber())
                {
                    target_field_value = GetNodePosition(xroot);
                }
                else if (field.IsParentPositionNumber())
                {
                    target_field_value = GetNodePosition(xroot.ParentNode);
                }
                else if (field.is_multi_valued)
                {
                    XmlNodeList nodes = xroot.SelectNodes(xpath, xmlnsManager);
                    if (nodes == null)
                    {
                        // Continue mapping other fields
                        SS4TLogger.WriteLog(ELogLevel.DEBUG,
                                               "Field " + field.field_name +
                                               " not found in XML - Skipping");
                        continue;
                    }
                    List<object> values = new List<object>();

                    foreach (XmlNode node in nodes)
                    {
                        values.Add(GetProcessedFieldValue(field, node, componentPresentation));
                    }
                    target_field_value = values.ToArray();
                }
                else if (field.IsMultimediaField())
                {
                    XmlNode node = xroot.SelectSingleNode(xpath, xmlnsManager);
                    if (node == null)
                    {
                        // Continue mapping other fields
                        SS4TLogger.WriteLog(ELogLevel.DEBUG,
                                               "Field " + field.field_name +
                                               " not found in XML - Skipping");
                        continue;
                    }
                    else
                    {
                        if (node.Attributes != null && node.Attributes.Count > 0 && node.Attributes["Path"] != null)
                        {
                            target_field_value = node.Attributes["Path"].Value;
                        }
                        else
                        {
                            target_field_value = string.Empty;
                        }
                    }
                }
                else
                {
                    XmlNode node = xroot.SelectSingleNode(xpath, xmlnsManager);
                    if (node == null)
                    {
                        // Continue mapping other fields
                        SS4TLogger.WriteLog(ELogLevel.DEBUG,
                                               "Field " + field.field_name +
                                               " not found in XML - Skipping");
                        continue;
                    }
                    else
                    {
                        target_field_value = node.InnerText;
                    }
                }

                SS4TLogger.WriteLog(ELogLevel.DEBUG,
                                       "    " + field.target_field_name +
                                       ":    " + target_field_value);
                mappedContent.Add(field.target_field_name, target_field_value);

                if (field.is_key)
                {
                    KeyValuePair<string, object> deleteCriteria = new KeyValuePair<string, object>(field.target_field_name,
                                                                                   target_field_value);
                    deleteCriteriaList.Add(deleteCriteria);
                }
            }

            docList.Add(mappedContent);

            if ((childType != null) && processChild)
            {
                SS4TLogger.WriteLog(ELogLevel.DEBUG, "Mapping Child Content Type: " + childType.contentTypeName);
                MappedContent mappedChildContent;

                if (childType.is_multi_valued)
                {
                    foreach (XmlNode xn in xroot.SelectNodes(childType.xpath, xmlnsManager))
                    {
                        mappedChildContent = GetMappedContent(componentPresentation, xn, false, childType);
                        docList.AddRange(mappedChildContent.MappedDocuments);
                        deleteCriteriaList.AddRange(mappedChildContent.DeleteCriteria);
                    }
                }
                else
                {
                    mappedChildContent = GetMappedContent(componentPresentation,
                                                          xroot.SelectSingleNode(childType.xpath, xmlnsManager),
                                                          false,
                                                          childType);
                    docList.AddRange(mappedChildContent.MappedDocuments);
                    deleteCriteriaList.AddRange(mappedChildContent.DeleteCriteria);
                }
            }

            SS4TLogger.WriteLog(ELogLevel.INFO,
                                   "Exiting Method: ContentMapper.GetMappedContent");
            return new MappedContent(docList, deleteCriteriaList);
        }

        private static void AddNamespace(XmlNamespaceManager xmlnsManager)
        {
            //Add the namespace used in Document List to the XmlNamespaceManager.

            xmlnsManager.AddNamespace("a", "http://schemas.datacontract.org/2004/07/GenericIndexingRestService.Models");
            xmlnsManager.AddNamespace("i", "http://www.w3.org/2001/XMLSchema-instance");
            xmlnsManager.AddNamespace("b", "http://schemas.microsoft.com/2003/10/Serialization/Arrays");
        }

        static object GetProcessedFieldValue(ContentTypeField field,
                                                 XmlNode value_node,
                                                 XmlDocument componentPresentation)
        {
            object field_value;

            if (field.IsLink())
            {
                field_value = Utility.ResolvedLink(value_node.Value);
            }
            else if (field.IsComplexLink())
            {
                field_value = Utility.ResolveComplexLink(value_node,
                                                         field.complex_field_delimiter);
            }
            else if (field.IsDateField())
            {
                field_value = DateTime.Parse(value_node.InnerXml);
            }
            else if (field.IsRTF())
            {
                CDatafyXMLNode(componentPresentation, value_node);
                field_value = value_node.InnerXml;
            }
            else if (field.IsBinarySerializedField())
            {
                field_value = string.Empty;
                if (!string.IsNullOrEmpty(value_node.InnerXml))
                {
                    value_node.InnerXml = value_node.InnerXml.Replace("xmlns:tridion=\"http://www.tridion.com/ContentManager/5.0\"", "");
                    field_value = HelperMethods.GetBinarySerializedStream(value_node.OuterXml.ToString());
                }
            }

            else
            {
                field_value = value_node.InnerText;
            }
            if (field.IsTransformationRequired())
            {

                field_value = Utility.Transform(value_node.InnerXml,
                                                field.transform_field_name);
            }

            return field_value;
        }

        private static void CDatafyXMLNode(XmlDocument doc, XmlNode thenode)
        {
            XmlNode cdata_node = doc.CreateCDataSection(thenode.InnerXml);
            thenode.RemoveAll();
            thenode.AppendChild(cdata_node);
        }

        private static int GetNodePosition(XmlNode node)
        {
            int position = 0;
            string nodeName = node.LocalName;
            XmlNodeList nodeList = node.ParentNode.SelectNodes(nodeName);
            foreach (XmlNode xNode in nodeList)
            {
                if (xNode == node)
                    return position;
                position++;
            }
            return 0;
        }

    }
}