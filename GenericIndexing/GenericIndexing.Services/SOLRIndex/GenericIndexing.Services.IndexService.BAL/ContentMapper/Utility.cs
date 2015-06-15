using System;
using System.Xml;
using System.Collections.Generic;
using System.Configuration;
using GenericIndexing.IndexService.DataHelper;
using System.Text.RegularExpressions;
using System.Xml.XPath;

namespace GenericIndexing.IndexService.BAL
{
    /// <summary>
    /// Utility Functions for Content Mapper
    /// </summary>
    public static class Utility
    {
        private const string TCM_URI_PATTERN = @"tcm:[0-9][0-9]*-[0-9][0-9]*";

        public static string GetConfigurationValue(string parameter)
        {
            return ConfigurationManager.AppSettings.Get(parameter);
        }

        public static string GetXMLAttributeValue(XmlNode xNode,
                                      string attributeName,
                                      string defaultValue = null)
        {
            if (xNode != null)
            {
                if (attributeName != null)
                {
                    XmlAttribute xmlAttribute = xNode.Attributes[attributeName];
                    if (xmlAttribute != null)
                    {
                        return xmlAttribute.Value;
                    }
                }
            }
            return defaultValue;
        }

        public static List<string> GetStringListFromXMLNodeList(List<XmlNode> xnodes)
        {
            List<string> stringList = new List<string>();
            if (xnodes != null)
            {
                foreach (XmlNode xn in xnodes)
                {
                    stringList.Add(xn.InnerXml);
                }
            }
            return stringList;
        }

        public static bool IsTCMURI(string linkstring)
        {
            if (!string.IsNullOrEmpty(linkstring))
            {
                return (linkstring.IndexOf("tcm:") > -1);
            }
            return false;
        }

        public static bool IsTridionLinkResolutionEnabled()
        {
            return string.Equals(GetConfigurationValue("DISABLE_TRIDION_LINK_RESOLUTION"), "false");
        }

        public static string ResolvedLink(string uri)
        {
            if (IsTCMURI(uri))
            {
                if (IsTridionLinkResolutionEnabled())
                {
                    return TridionDataHelper.ComponentLinkMethod(uri);
                }
                else
                {
                    return "TridionLinkResolvedDisabled(" + uri + ")";
                }
            }
            return uri;
        }

        static string GetWebPageKeywordTitle(string itemUri)
        {
            int publicationId = TridionDataHelper.GetContentRepositoryId(itemUri);
            int CONSTANT_WEBPAGE_KEYWORD_URI = int.Parse(GetConfigurationValue("KEYWORD_WEBPAGE_ITEM_ID"));
            int CONSTANT_WEBPAGE_KEYWORD_TYPE = int.Parse(GetConfigurationValue("KEYWORD_WEBPAGE_ITEM_TYPE"));

            var keyword = TridionDataHelper.GetKeywords(publicationId,
                                                        CONSTANT_WEBPAGE_KEYWORD_URI,
                                                        CONSTANT_WEBPAGE_KEYWORD_TYPE);

            if (keyword != null)
                return keyword.KeywordName;
            else
                return "Web Page";
        }

        public static string ResolveComplexLink(XmlNode linknode, string linkFragmentDelimiter)
        {
            string urltype = string.Empty;
            string url = string.Empty;
            string linktitle = string.Empty;

            if (linknode.SelectSingleNode("title") != null)
            {
                linktitle = linknode.SelectSingleNode("title").InnerXml;
            }
            if (linknode.SelectSingleNode("urltype") != null)
            {
                urltype = linknode.SelectSingleNode("urltype").InnerXml;

                switch (urltype ?? String.Empty)
                {
                    case "Internal":
                        url = ResolvedLink(linknode.SelectSingleNode("internallink").Attributes["Id"].Value);
                        break;
                    case "External":
                        if (linknode.SelectSingleNode("externallink") != null)
                        {
                            url = linknode.SelectSingleNode("externallink").InnerXml;
                        }
                        if (linknode.SelectSingleNode("calltoaction") != null)
                        {
                            url = linknode.SelectSingleNode("calltoaction").InnerXml;
                        }
                        break;
                }
            }

            string windowoption = linknode.SelectSingleNode("openinwhichwindow").InnerXml;

            return linkFragmentDelimiter + linktitle +
                   linkFragmentDelimiter + urltype +
                   linkFragmentDelimiter + url +
                   linkFragmentDelimiter + windowoption;
        }

        public static string Transform(string valueToTransform, string tranformField)
        {
            switch (tranformField)
            {
                case "year_in_date":
                    valueToTransform = DateTime.Parse(valueToTransform).Year.ToString();
                    break;
                case "binary_type_webpage":
                    valueToTransform = GetWebPageKeywordTitle(valueToTransform);
                    break;
                case "resolve_rtf_component_links":
                    valueToTransform = ResolveRTFComponentLinks(valueToTransform);
                    break;
            }
            return valueToTransform;
        }

        private static string ResolveRTFComponentLinks(string desc_XmlString)
        {
            MatchCollection tcmURIs = Regex.Matches(desc_XmlString, TCM_URI_PATTERN);

            foreach (Match tcmURI in tcmURIs)
            {
                desc_XmlString = desc_XmlString.Replace(tcmURI.Value, Utility.ResolvedLink(tcmURI.Value));
            }
            return desc_XmlString;
        }

        public static string GetPublicationName(XmlDocument component_presentation)
        {
            string publication_node_xpath = ConfigurationManager.AppSettings.Get("XPATH_PUBLICATION_NAME");
            return component_presentation.DocumentElement.SelectSingleNode(publication_node_xpath).Value;
        }

        public static string GetContentType(XmlDocument component_presentation_xml)
        {
            return component_presentation_xml.DocumentElement.Name.ToString();
        }

        public static string GetContentType(string component_presentation_xml)
        {
            XmlDocument xdoc = new XmlDocument();
            xdoc.LoadXml(component_presentation_xml);
            return xdoc.DocumentElement.Name.ToString();
        }
    }
}