using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using Tridion.ContentManager;
using Tridion.ContentManager.CommunicationManagement;
using Tridion.ContentManager.ContentManagement;
using Tridion.ContentManager.ContentManagement.Fields;
using Tridion.ContentManager.Templating;
using Tridion.ContentManager.Templating.Assembly;
using System.Text;
using System.Linq;
using System.Xml.Linq;
using System.Xml.XPath;
using Tridion;

namespace GenericIndexing.Templating
{
    public class Utilities
    {
         private const string LAST_UPADTE_BY_XPATH = @"tcm:Component/tcm:Info/tcm:VersionInfo/tcm:Revisor/@xlink:title";
        private const string PUBLISH_STATUS_XPATH = @"tcm:Component/tcm:Info/tcm:PublishInfo/tcm:IsPublished";
        
        protected static Regex GetRegexForParameter(string parameter)
        {
            Regex result = null;
            if (!String.IsNullOrEmpty(parameter))
            {
                string pattern = parameter.Substring(parameter.IndexOf("("));
                pattern = pattern.Substring(1, pattern.Length - 2);
                //Logger.debug("GetRegexForParameter: " + pattern);
                result = new Regex(pattern);
            }
            return result;
        }

        protected static string GetOpeningTag(string tag, string cssClass)
        {
            string menu = "<" + tag;
            if (!String.IsNullOrEmpty(cssClass))
            {
                menu += " class='" + cssClass + "'";
            }
            menu += ">" + Environment.NewLine;
            return menu;
        }

        /// <summary>
        /// Return a list of objects of the requested type from the XML.
        /// </summary>
        /// <remarks>
        /// This method goes back to the database to retrieve more information. So it is NOT just
        /// a fast and convenient way to get a type safe list from the XML.
        /// </remarks>
        /// <typeparam name="T">The type of object to return, like Publication, User, Group, OrganizationalItem</typeparam>
        /// <param name="listElement">The XML from which to construct the list of objects</param>
        /// <returns>a list of objects of the requested type from the XML</returns>
        protected IList<T> GetObjectsFromXmlList<T>(Engine engine, XmlElement listElement) where T : IdentifiableObject
        {
            XmlNodeList itemElements = listElement.SelectNodes("*");
            List<T> result = new List<T>(itemElements.Count);
            foreach (XmlElement itemElement in itemElements)
            {
                result.Add(GetObjectFromXmlElement<T>(engine, itemElement));
            }
            result.Sort(delegate(T item1, T item2)
            {
                return item1.Title.CompareTo(item2.Title);
            });
            return result;
        }

        protected T GetObjectFromXmlElement<T>(Engine engine, XmlElement itemElement) where T : IdentifiableObject
        {
            return (T)engine.GetObject(itemElement.GetAttribute("ID"));
        }

        protected static string Encode(string value)
        {
            return System.Web.HttpUtility.HtmlEncode(value);
        }

        /// <summary>
        /// Returns the root structure group from the list of structure groups specified.
        /// </summary>
        /// <exception cref="InvalidDataException">when there is no root structure group in the list</exception>
        /// <param name="items">The list of structure groups to search.</param>
        /// <returns>the root structure group from the list of structure groups specified</returns>
        protected ListItem GetRootSG(IList<ListItem> items)
        {
            foreach (ListItem item in items)
            {
                if (item.ParentId.PublicationId == -1)
                {
                    return item;
                }
            }
            throw new InvalidDataException("Could not find root structure group");
        }

        /// <summary>
        /// Returns the root structure group for the specified item
        /// </summary>
        /// <param name="item">Any item which resides in a publication</param>
        /// <returns>The Root Structure Group in the publication</returns>
        protected StructureGroup GetRootSG(RepositoryLocalObject item)
        {
            Repository pub = item.OwningRepository;

            return GetRootSG(pub);
        }

        /// <summary>
        /// Returns the root structure group for the specified publication
        /// </summary>		
        /// <returns>The Root Structure Group in the publication</returns>
        /// <remarks>copied and modified code from Repository.RootFolder :)</remarks>
        protected StructureGroup GetRootSG(Repository publication)
        {
            Filter filter = new Filter();
            filter.Conditions["ItemType"] = ItemType.StructureGroup;
            RepositoryItemsFilter repoItemsFilter = new RepositoryItemsFilter(filter, publication.Session);
            IList<RepositoryLocalObject> items = publication.GetItems(repoItemsFilter).ToList<RepositoryLocalObject>();

            if (items.Count == 0)
                return null;
            else
                return (StructureGroup)items[0];
        }

        protected Component GetComponentValue(String fieldNAme, ItemFields fields)
        {
            if (fields.Contains(fieldNAme))
            {
                ComponentLinkField field = fields[fieldNAme] as ComponentLinkField;
                return field.Value;
            }

            return null;
        }

        protected IList<Component> GetComponentValues(string fieldName, ItemFields fields)
        {
            if (fields.Contains(fieldName))
            {
                ComponentLinkField field = (ComponentLinkField)fields[fieldName];
                return (field.Values.Count > 0) ? field.Values : null;
            }
            else
            {
                return null;
            }
        }

        protected IList<DateTime> GetDateValues(string fieldName, ItemFields fields)
        {
            if (fields.Contains(fieldName))
            {
                DateField field = (DateField)fields[fieldName];
                return (field.Values.Count > 0) ? field.Values : null;
            }
            else
            {
                return null;
            }
        }

        protected IList<Keyword> GetKeywordValues(string fieldName, ItemFields fields)
        {
            if (fields.Contains(fieldName))
            {
                KeywordField field = (KeywordField)fields[fieldName];
                return (field.Values.Count > 0) ? field.Values : null;
            }
            else
            {
                return null;
            }
        }

        protected IList<double> GetNumberValues(string fieldName, ItemFields fields)
        {
            if (fields.Contains(fieldName))
            {
                NumberField field = (NumberField)fields[fieldName];
                return (field.Values.Count > 0) ? field.Values : null;
            }
            else
            {
                return null;
            }
        }

        protected IList<string> GetStringValues(string fieldName, ItemFields fields)
        {
            if (fields.Contains(fieldName))
            {
                TextField field = (TextField)fields[fieldName];
                return (field.Values.Count > 0) ? field.Values : null;
            }
            else
            {
                return null;
            }
        }

        protected String GetSingleStringValue(string fieldName, ItemFields fields)
        {
            if (fields.Contains(fieldName))
            {
                TextField field = fields[fieldName] as TextField;

                if (field != null) return field.Value;
            }

            return null;
        }

        /// <summary>
        /// Gets the type of the specified field in an enumeration
        /// </summary>		
        public static FieldType GetFieldType(ItemField field)
        {
            return GetFieldType(field, false);
        }

        /// <summary>
        /// Gets the type of the specified field in an enumeration
        /// </summary>		
        /// <param name="limitToBase">confines the result to the base type of the field such as returning a component link field type
        /// for a multimedia field</param>
        public static FieldType GetFieldType(ItemField field, bool limitToBase)
        {
            FieldType res;

            if (limitToBase)
            {
                if (field is EmbeddedSchemaField)
                    res = FieldType.EmbeddedSchemaField;
                else if (field is NumberField)
                    res = FieldType.NumberField;
                else if (field is ComponentLinkField)
                    res = FieldType.ComponentLinkField;
                else if (field is KeywordField)
                    res = FieldType.KeywordField;
                else if (field is DateField)
                    res = FieldType.DateField;
                else
                    res = FieldType.TextField;
            }
            else
            {
                if (field is ExternalLinkField)
                    res = FieldType.ExternalLinkField;
                else if (field is EmbeddedSchemaField)
                    res = FieldType.EmbeddedSchemaField;
                else if (field is MultimediaLinkField)
                    res = FieldType.MultimediaLinkField;
                else if (field is KeywordField)
                    res = FieldType.KeywordField;
                else if (field is ComponentLinkField)
                    res = FieldType.ComponentLinkField;
                else if (field is XhtmlField)
                    res = FieldType.XHtmlField;
                else if (field is DateField)
                    res = FieldType.DateField;
                else if (field is MultiLineTextField)
                    res = FieldType.MultiLineTextField;
                else if (field is NumberField)
                    res = FieldType.NumberField;
                else
                    res = FieldType.SingleLineTextField;
            }

            return res;
        }


        /// <summary>
        /// Converts a string to a memory stream
        /// </summary>
        public static Stream ConvertStringToStream(string str)
        {
            return new MemoryStream(Encoding.UTF8.GetBytes(str));
        }


        public static XmlDocument RemoveAllNamespaces(XmlDocument doc)
        {

            XElement xDoc = doc.ToXElement();
           
            XElement docElement = RemoveAllNamespaces(xDoc);

            return docElement.ToXmlDocument();
           
        }

        private static XElement RemoveAllNamespaces(XElement xmlDocument)
        {
            if (!xmlDocument.HasElements)
            {

                XElement xElement = new XElement(xmlDocument.Name.LocalName,
                    xmlDocument.Attributes().Select(att => RemoveAttributeNamespaces(att)));
                    xElement.Value = xmlDocument.Value;
                    return xElement;
            }


            return new XElement(xmlDocument.Name.LocalName, xmlDocument.Elements().Select(el => RemoveAllNamespaces(el)), xmlDocument.Attributes().Select(att => RemoveAttributeNamespaces(att)));
           
        }

        private static XAttribute RemoveAttributeNamespaces(XAttribute att)
        {
            if (att.IsNamespaceDeclaration) return null;
           
            XAttribute attribute = new XAttribute(att.Name, att.Value);

            return attribute;
        }

        /// <summary>
        /// Removes Xlink related attributes from node
        /// </summary>
        /// <param name="node"></param>
        public static void RemoveXLinkAttributes(XmlNode node)
        {
            if (node.Attributes.Count < 1) return;
            for (int i = node.Attributes.Count - 1; i >= 0; i--)
            {
                if ((node.Attributes[i].NamespaceURI == Constants.XlinkNamespace)
                    || (node.Attributes[i].Value == Constants.XlinkNamespace))
                {
                    node.Attributes.Remove(node.Attributes[i]);
                }
            }

        }

        public static void  AddAttribute(XmlNode node, string attributeName, string attributeValue)
        {
            AddAttribute(node, attributeName, null, attributeValue);
        }
        public static void AddAttribute(XmlNode node, string attributeName, string attNamespace, string attributeValue)
        {
            XmlAttribute newAttribute = null;
            if (string.IsNullOrEmpty(attNamespace))
            {
                newAttribute = node.OwnerDocument.CreateAttribute(attributeName);
            }
            else
            {
                newAttribute = node.OwnerDocument.CreateAttribute(attributeName, attNamespace);
            }
            newAttribute.Value = attributeValue;
            node.Attributes.Append(newAttribute);
        }


         /// <summary>
        /// Get internal component informations like Last Updated by, Publish Status of component etc.
        /// </summary>
        /// <param name="infoType">information type</param>
        /// <returns>value of the information type requested</returns>
        public static string GetPublishStatus(string  xml)
        {
            string output = string.Empty;
            XmlDocument xDoc = new XmlDocument();
            xDoc.LoadXml(xml);

            if (xDoc != null)
            {
                XmlNode xmlNode = xDoc.SelectSingleNode(PUBLISH_STATUS_XPATH, TridionCollectionBase<IdentifiableObject>.NSManager);
                if (xmlNode != null)
                {
                    output = xmlNode.InnerText;
                }
            }
            return output;
        }

        public static string GetFilename(string fullpath)
        {
            if (fullpath.Contains(@"\"))
            {
                int pos = fullpath.LastIndexOf(@"\");
                return fullpath.Substring(pos + 1);
            }
            return fullpath;
        }

        public static Category GetCategoryByID(Publication objPublication, string categoryTCMID)
        {
            Filter categoryFilter = new Filter();
            categoryFilter.Conditions["ID"] = categoryTCMID;
            // TODO: Refactor deprecated method GetCategories
            foreach (Category category in objPublication.GetCategories(categoryFilter))
            {
                if (categoryTCMID.ToUpper().Equals(category.Id.ToString().ToUpper()))
                {
                    return category;
                }
            }
            return null;
        }

        public static Category GetCategoryByTitle(Publication objPublication, string categoryTitle)
        {
            Filter categoryFilter = new Filter();
            categoryFilter.Conditions["Title"] = categoryTitle;
            // TODO: Refactor deprecated method GetCategories
            foreach (Category category in objPublication.GetCategories(categoryFilter))
            {
                if (categoryTitle.ToUpper().Equals(category.Title.ToString().ToUpper()))
                {
                    return category;
                }
            }
            return null;
        }

        /// <summary>
        ///  Method  for Converting special charectors in WebDav url
        /// </summary>
        /// <param name="componentWebDavURL"></param>
        /// <returns>Converted string</returns>
        public string ConvertWebDavSpecialChars(string inputString)
        {
            StringBuilder returnString = new StringBuilder(inputString);
            returnString = returnString.Replace("%", "%25");
            returnString = returnString.Replace(" ", "%20");
            returnString = returnString.Replace("í", "%C3%AD");
            returnString = returnString.Replace("Ì", "%C3%8C");
            returnString = returnString.Replace("Ç", "%C3%87");
            returnString = returnString.Replace("ó", "%C3%B3");
            returnString = returnString.Replace("¯", "%C2%AF");
            returnString = returnString.Replace("Ü", "%C3%BC");
            returnString = returnString.Replace("ú", "%C3%BA");
            returnString = returnString.Replace("Ó", "%C3%93");
            returnString = returnString.Replace("é", "%C3%A9");
            returnString = returnString.Replace("ñ", "%C3%B1");
            returnString = returnString.Replace("ß", "%C3%9F");
            returnString = returnString.Replace("â", "%C3%A2");
            returnString = returnString.Replace("Ñ", "%C3%91");
            returnString = returnString.Replace("Ô", "%C3%94");
            returnString = returnString.Replace("ä", "%C3%A4");
            returnString = returnString.Replace("ª", "%C2%AA");
            returnString = returnString.Replace("Ò", "%C3%92");
            returnString = returnString.Replace("à", "%C3%A0");
            returnString = returnString.Replace("º", "%C2%BA");
            returnString = returnString.Replace("Õ", "%C3%B5");
            returnString = returnString.Replace("å", "%C3%A5");
            returnString = returnString.Replace("¿", "%C2%BF");
            returnString = returnString.Replace("Õ", "%C3%95");
            returnString = returnString.Replace("ç", "%C3%A7");
            returnString = returnString.Replace("µ", "%C2%B5");
            returnString = returnString.Replace("ê", "%C3%AA");
            returnString = returnString.Replace("½", "%C2%BD");
            returnString = returnString.Replace("Þ", "%C3%BE");
            returnString = returnString.Replace("ë", "%C3%AB");
            returnString = returnString.Replace("¼", "%C2%BC");
            returnString = returnString.Replace("Þ", "%C3%9E");
            returnString = returnString.Replace("è", "%C3%A8");
            returnString = returnString.Replace("¡", "%C2%A1");
            returnString = returnString.Replace("Ú", "%C3%9A");
            returnString = returnString.Replace("ï", "%C3%AF");
            returnString = returnString.Replace("«", "%C2%AB");
            returnString = returnString.Replace("Û", "%C3%9B");
            returnString = returnString.Replace("î", "%C3%AE");
            returnString = returnString.Replace("»", "%C2%BB");
            returnString = returnString.Replace("Ù", "%C3%99");
            return returnString.ToString();
        }
    }

    public enum FieldType
    {
        DateField,
        ExternalLinkField,
        EmbeddedSchemaField,
        MultimediaLinkField,
        ComponentLinkField,
        NumberField,
        MultiLineTextField,
        SingleLineTextField,
        XHtmlField,
        KeywordField,
        TextField
    }

}
