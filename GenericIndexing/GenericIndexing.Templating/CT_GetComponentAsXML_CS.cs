using System;
using System.Xml;
using System.Linq;
using System.Xml.Linq;
using Tridion;
using Tridion.ContentManager;
using Tridion.ContentManager.Templating;
using Tridion.ContentManager.Templating.Assembly;
using Tridion.ContentManager.CommunicationManagement;
using Tridion.ContentManager.ContentManagement;
using Tridion.ContentManager.ContentManagement.Fields;
using System.Collections;
using System.Text.RegularExpressions;

namespace GenericIndexing.Templating
{
    [TcmTemplateTitle("CT_GetComponentAsXML_CS")]
    class CT_GetComponentAsXML_CS : TemplateBase
    {
        #region Internal Classes

        internal static class XmlConstants
        {
            public static string COMPONENTID = "Id";
            public static string PUBLICATION_NODE = "publication";
            public static string PUBLICATIONID = "Id";
            public static string PUBLICATIONTITLE = "Title";
            public static string COMPONENTTITLE = "Title";
            public static string TRIDION_HREF_ATTRIB = "tridion:href";
            public static string TRIDION_TYPE_ATTRIB = "tridion:type";
            public static string TRIDION_TARGET_ATTRIB = "tridion:targetattribute";
            public static string TRIDION_MULTIMEDIALINK_ATTRIBVAL = "Multimedia";
            public static string MULTIMEDIA_PATH_ATTRIB = "Path";
            public static string SRC = "src";
            public static string FOOTER_NODE = "footer";
            public static string ATT_HREF = "href";
            public static string ANCHOR_NODE = "a";
            public static string CONTENT_LIST = "contentitem";
        }

        internal static class Parameters
        {
            public static string INCLUDE_PUB_METADATA = "IncludePubMetaData";
            public static string PACKAGE_FIELD_NAME = "PackageFieldName";
            public static string EXPAND_LINKED_COMPONENTS = "ExpandLinkedComponents";
            public static string COMPONENT_LINK_RECURSION_LIMIT = "ComponentLinkRecursionLimit";
        }

        internal static class ParameterValues
        {
            public const string CURRENT_VERSION = "Current";
            public const string LAST_PUBLISHED_VERSION = "Last Published";
        }

        #endregion
        #region Declarations
        private bool m_expandLinkedComponents = false;
        private bool m_includePublicationMetaData = true;
        private int m_componentLinkRecursionLimit = 10;

        private Publication m_publication = null;
        private Hashtable m_traversedComponents = null;
        private XmlDocument m_ownerDoc = null;
        private TcmUri m_componentUri = null;
        private string m_rootNode = string.Empty;

        private string m_packageFieldName = "Output";//default package output
        #endregion


        public override void Transform(Engine engine, Package package)
        {
            Logger.Info("GetComponentAsXML invoked");
            InitializeTemplate(engine, package);

            Component component = GetComponent();

            m_componentUri = component.Id;

            if (component == null)
            {
                Logger.Error("No component found in current package.");
            }
            else
            {
                //create initial XML structure
                InitializeComponentXml(component);
                m_rootNode = m_ownerDoc.FirstChild.Name;
                
                ProcessComponent(m_ownerDoc.DocumentElement, component);
                //clean up all namespaces from the XML

                string xmlPattern = "xmlns=\\\"*(?<url>[^\\\"]*)\\\"";
                
                MatchCollection matchCol = Regex.Matches(m_ownerDoc.InnerXml, xmlPattern);

                foreach (Match m in matchCol)
                {   
                    m_ownerDoc.InnerXml = m_ownerDoc.InnerXml.Replace(m.ToString(), "");
                }

                HandleRTFFields(m_ownerDoc, component);

                package.PushItem(this.PackageFieldName, package.CreateXmlDocumentItem(ContentType.Xml, m_ownerDoc));
            }
        }



        #region Initialization

        /// <summary>
        /// Initializes parameters and commonly used object references.
        /// </summary>
        /// <param name="engine"></param>
        /// <param name="package"></param>
        protected void InitializeTemplate(Engine engine, Package package)
        {
            base.Initialize(engine, package);
            m_publication = GetPublication();
            m_traversedComponents = new Hashtable();
            InitializeParameters();
        }
        /// <summary>
        /// Creates the initial XML document using the component's default XML representation
        /// and adds publication data
        /// </summary>
        /// <param name="component"></param>
        private void InitializeComponentXml(Component component)
        {

            m_ownerDoc = new XmlDocument();
            m_ownerDoc.InnerXml = component.Content.OuterXml;
            XmlElement docRoot = m_ownerDoc.DocumentElement;
            docRoot.SetAttribute(string.Format("xmlns:{0}", Constants.XlinkPrefix), Constants.XlinkNamespace);
            docRoot.SetAttribute("xmlns:" + Constants.TcmPrefix, Constants.TcmNamespace);
            AddPublicationData(docRoot);

        }


        /// <summary>
        /// Initializes parameters - set either via the parameter schema or defaults if schema
        /// not used/ values not supplied.
        /// </summary>
        private void InitializeParameters()
        {
            string paramValue = m_Package.GetValue(Parameters.INCLUDE_PUB_METADATA);
            bool.TryParse(paramValue, out m_includePublicationMetaData);
            paramValue = m_Package.GetValue(Parameters.PACKAGE_FIELD_NAME);
            if (!string.IsNullOrEmpty(paramValue))
            {
                m_packageFieldName = paramValue;
            }
            paramValue = m_Package.GetValue(Parameters.EXPAND_LINKED_COMPONENTS);
            bool.TryParse(paramValue, out m_expandLinkedComponents);
            //paramValue = m_Package.GetValue(Parameters.COMPONENT_LINK_RECURSION_LIMIT);
            //Int32.TryParse(paramValue, out m_componentLinkRecursionLimit);
            m_componentLinkRecursionLimit = 5;
        }
        #endregion

        /// <summary>
        /// Process a component to resolve it's XML representation
        /// </summary>
        /// <param name="node">XMLNode corresponding to the current component </param>
        /// <param name="component"></param>
        private void ProcessComponent(XmlNode node, Component component, int componentLinkRecursionLevel = 0)
        {
            Logger.Info("ProcessComponent invoked with componentLinkRecursionLevel: " + componentLinkRecursionLevel);
            //added check for contentlist node so that component links for product drawer tab are not expanded
            if (componentLinkRecursionLevel > m_componentLinkRecursionLimit)
            {
                Logger.Info("Exceeded componentLinkRecursionLimit: " +
                    m_componentLinkRecursionLimit + " Not processing further" + "Node Name: " + node.Name);
                return;
            }

            componentLinkRecursionLevel++;

            string componentId = component.Id.ToString();
            //If componentId has not already been resolved earlier, process it
            // else use previously resolved node
            if (!m_traversedComponents.ContainsKey(componentId))
            {
                m_traversedComponents.Add(componentId, node);
                AddCommonComponentAttributes(node, component);
                ImportMetaData(node, component.Metadata);
                if (component.ComponentType == ComponentType.Multimedia)
                {
                    ProcessMultimediaComponent(node, component);
                }
                else
                {
                    ResolveLinks(node, componentLinkRecursionLevel);
                }
            }
            else // node has been resolved before
            {
                CopyNode(node, componentId);
            }
        }

        private void HandleRTFFields(XmlDocument content, Component component)
        {
            ItemFields fields = new ItemFields(component.Content, component.Schema);
            foreach (ItemField field in fields)
            {
                if (field.Definition.GetType().Name == "XhtmlFieldDefinition")
                {
                    XmlNode node = content.SelectSingleNode(content.DocumentElement.Name + "/" + field.Name);
                    if (node != null)
                    {
                        CDatafyXMLNode(content, node);
                    }
                }
            }
        }

        private static void CDatafyXMLNode(XmlDocument doc, XmlNode thenode)
        {
            XmlNode cdata_node = doc.CreateCDataSection(thenode.InnerXml);
            thenode.RemoveAll();
            thenode.AppendChild(cdata_node);
        }

        /// <summary>
        /// Process multimedia component. Add basic information and metadata plus 
        /// add binary links in Dreamweaver format so that they can be used
        /// associated TBBs to publish/resolve binary links.
        /// </summary>
        /// <param name="node"></param>
        /// <param name="component"></param>
        private void ProcessMultimediaComponent(XmlNode node, Component component)
        {
            Item binaryItem = AddBinaryToPackage(component.Id);

            Utilities.AddAttribute(node, XmlConstants.TRIDION_HREF_ATTRIB, Constants.TcmNamespace, component.Id.ToString());

            Utilities.AddAttribute(node, XmlConstants.TRIDION_TYPE_ATTRIB, Constants.TcmNamespace, XmlConstants.TRIDION_MULTIMEDIALINK_ATTRIBVAL);

            Utilities.AddAttribute(node, XmlConstants.TRIDION_TARGET_ATTRIB, Constants.TcmNamespace, XmlConstants.MULTIMEDIA_PATH_ATTRIB);
            Utilities.AddAttribute(node, XmlConstants.MULTIMEDIA_PATH_ATTRIB, String.Empty);
        }

        /// <summary>
        /// Adds publication specific data to the XML. Basic data includes Id and Title.
        /// If meta data is to be included, it adds all fields in metadata as child elements of 
        /// <Publication> element
        /// </summary>
        /// <param name="docRoot"></param>
        private void AddPublicationData(XmlElement docRoot)
        {
            XmlNode pub = docRoot.AppendChild(m_ownerDoc.CreateElement(XmlConstants.PUBLICATION_NODE));
            Utilities.AddAttribute(pub, XmlConstants.PUBLICATIONID, m_publication.Id.ToString());
            Utilities.AddAttribute(pub, XmlConstants.PUBLICATIONTITLE, m_publication.Title);
            if (IncludePublicationMetaData)
            {
                ImportMetaData(pub, m_publication.Metadata);
            }
        }

        /// <summary>
        /// Adds meta data fields as child nodes of parent Node
        /// </summary>
        /// <param name="node">Parent node that represents a component/publication etc.</param>
        /// <param name="metadata"></param>
        private void ImportMetaData(XmlNode node, XmlElement metadata)
        {
            if (metadata != null && metadata.HasChildNodes)
            {
                XmlNodeList metaDataFields = metadata.ChildNodes;
                foreach (XmlNode metadataNode in metaDataFields)
                {
                    XmlNode newNode = node.OwnerDocument.CreateNode(metadataNode.NodeType, metadataNode.LocalName, "");
                    if (newNode.HasChildNodes)
                        newNode.InnerXml = metadataNode.InnerXml;
                    node.AppendChild(node.OwnerDocument.ImportNode(metadataNode, true));
                }
            }
        }

        /// <summary>
        /// Looks for all component links in the XML representation of a component.
        /// </summary>
        /// <param name="componentElement"></param>
        private void ResolveLinks(XmlNode componentElement, int componentLinkRecursionLevel)
        {
            Logger.Info("ResolveLinks invoked with componentLinkRecursionLevel: " + componentLinkRecursionLevel);
            string rootName = componentElement.Name;

            string linkPath = @".//*[@xlink:href]"; // find all descendant nodes with an xlink:href attribute

            XmlNodeList linkedComponentList = componentElement.SelectNodes(linkPath, NSManager);

            Logger.Info("Resolve Link:" + linkedComponentList.Count + "|" + linkPath);
            foreach (XmlNode node in linkedComponentList)
            {
                Logger.Info("Node Name" + node.Name.ToLower());
                ResolveLinkedNode(node, componentLinkRecursionLevel);
            }
        }

        private void ProcessImageComponent(XmlNode node)
        {
            TcmUri tcmUri = new TcmUri(node.Attributes["xlink:href"].Value);
            Item binaryItem = AddBinaryToPackage(tcmUri);
            Utilities.AddAttribute(node, XmlConstants.TRIDION_HREF_ATTRIB, Constants.TcmNamespace, tcmUri.ToString());
            Utilities.AddAttribute(node, XmlConstants.TRIDION_TYPE_ATTRIB, Constants.TcmNamespace, XmlConstants.TRIDION_MULTIMEDIALINK_ATTRIBVAL);
            Utilities.AddAttribute(node, XmlConstants.TRIDION_TARGET_ATTRIB, Constants.TcmNamespace, XmlConstants.SRC);
            Utilities.AddAttribute(node, XmlConstants.SRC, String.Empty);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        private void ResolveLinkedNode(XmlNode node, int componentLinkRecursionLevel)
        {
            Logger.Info("ResolveLinkedNode invoked with componentLinkRecursionLevel: " + componentLinkRecursionLevel);
            XmlAttribute hrefAttribute = (XmlAttribute)node.Attributes.GetNamedItem("xlink:href");
            XmlAttribute titleAttribute = (XmlAttribute)node.Attributes.GetNamedItem("xlink:title");
            //remove all xlink related attributes from the node
            Utilities.RemoveXLinkAttributes(node);
            string componentUri = hrefAttribute.Value;
            if (titleAttribute != null)// this is a linked component
            {
                //check if the component corresponding to this node has already been resolved previously
                if (m_traversedComponents.ContainsKey(componentUri))
                {
                    CopyNode(node, componentUri);
                }
                else
                {
                    Component component = GetLinkedComponent(componentUri);
                    if (component != null)
                    {
                        ResolveLinkedComponent(node, component, componentLinkRecursionLevel);
                    }
                }
            }
            else // external link
            {
                XmlText extLink = node.OwnerDocument.CreateTextNode(hrefAttribute.Value);
                node.AppendChild(extLink);
            }
        }

        /// <summary>
        /// Fetches component object for specified TCM Uri
        /// </summary>
        /// <param name="componentUri">TCMURI of component</param>
        /// <returns></returns>
        private Component GetLinkedComponent(string componentUri)
        {

            Component com = m_Engine.GetObject(componentUri) as Component;

            return com;
        }
        private string GetLinkedComponentXml(string componentUri)
        {

            string com = m_Engine.GetTridionXml(componentUri);

            return com;
        }

        /// <summary>
        /// Replace node with an already resolved version
        /// </summary>
        /// <param name="node">Node to be replaced</param>
        /// <param name="componentUri">Uri used as a key to fetch resolved version</param>
        private void CopyNode(XmlNode node, string componentUri)
        {
            XmlNode resolvedNode = m_traversedComponents[componentUri] as XmlNode;
            //the only thing that will differ between the 2 is possibly the element name itself
            // that represents the field name of this component in the current parent's schema
            string nodeName = node.Name;
            node.InnerXml += resolvedNode.InnerXml;
            foreach (XmlAttribute attrib in resolvedNode.Attributes)
            {
                Utilities.AddAttribute(node, attrib.Name, attrib.NamespaceURI, attrib.Value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="node"></param>
        /// <param name="component"></param>
        private void ResolveLinkedComponent(XmlNode node, Component component, int componentLinkRecursionLevel)
        {
            Logger.Info("ResolveLinkedComponent invoked with componentLinkRecursionLevel: " + componentLinkRecursionLevel);
            if (component.ComponentType == ComponentType.Multimedia) // always process multimedia content the same way
            {
                ProcessComponent(node, component, componentLinkRecursionLevel);
            }
            else if (ExpandLinkedComponents) // expand normal linked component (not for footer components)
            {
                node.InnerXml = component.Content.InnerXml + node.InnerXml; //In Case there is a Multimedia/Component links in RTF field, node.InnerXml will not be empty

                ProcessComponent(node, component, componentLinkRecursionLevel);
            }
            else // do not expand normal linked component
            {
                AddCommonComponentAttributes(node, component);
            }

        }

        private void AddCommonComponentAttributes(XmlNode node, Component component)
        {
            if ((component.ComponentType == ComponentType.Multimedia) ||
                (ExpandLinkedComponents) || (component.Id.Equals(m_componentUri)))
            {
                Utilities.AddAttribute(node, XmlConstants.COMPONENTTITLE, component.Title);// don't add title for linked components in a non expdanded view, since we're not sure of the version of the component being used
            }
            if (m_rootNode == XmlConstants.FOOTER_NODE && node.Name == XmlConstants.ANCHOR_NODE) // add href for footer node.
            {
                Utilities.AddAttribute(node, XmlConstants.ATT_HREF, component.Id.ToString());
            }
            else
            {
                Utilities.AddAttribute(node, XmlConstants.COMPONENTID, component.Id.ToString());
            }

        }

        /// <summary>
        /// Adds a binary (associated with multimedia component) to package
        /// </summary>
        /// <param name="itemUri"></param>
        /// <returns></returns>
        private Item AddBinaryToPackage(TcmUri itemUri)
        {
            // Found item to link to, publish that item and replace the path
            Item binaryItem = m_Package.CreateMultimediaItem(itemUri);


            // Make the item name the file name (can be null)
            string itemName;
            //binaryItem.Properties.TryGetValue(Item.ItemPropertyFileName, out itemName);
            binaryItem.Properties.TryGetValue(Item.ItemPropertyTcmUri, out itemName);


            Item existingItem = m_Package.GetByName(itemName);
            if (
                existingItem == null ||
                !existingItem.Properties[Item.ItemPropertyTcmUri].Equals(itemUri) ||
                !existingItem.Equals(binaryItem) // Ensure that a transformed item is not considered the same
            )
            {
                Logger.Debug(string.Format("Image {0} ({1}) unique, adding to package", itemName, itemUri));
                m_Package.PushItem(itemName, binaryItem);
                return binaryItem;
            }
            else
            {
                Logger.Debug(string.Format("Image {0} ({1}) already present in package, not adding again", itemName, itemUri));
                return existingItem;
            }

        }

        #region Properties

        public string PackageFieldName
        {
            get
            {
                return m_packageFieldName;
            }
            set
            {
                m_packageFieldName = value;
            }
        }

        public bool ExpandLinkedComponents
        {
            get
            {
                return m_expandLinkedComponents;
            }
            set { m_expandLinkedComponents = value; }
        }

        public bool IncludePublicationMetaData
        {
            get
            {
                return m_includePublicationMetaData;
            }
            set
            {
                m_includePublicationMetaData = value;
            }
        }

        public int ComponentLinkRecursionLimit
        {
            get
            {
                return m_componentLinkRecursionLimit;
            }
            set 
            {
                m_componentLinkRecursionLimit = value;
            }
        }

        #endregion
    }
}


