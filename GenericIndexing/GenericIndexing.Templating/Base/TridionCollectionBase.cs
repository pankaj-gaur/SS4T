using System;
using System.Collections.Generic;
using System.Text;
using Tridion.ContentManager.Templating;
using Tridion.ContentManager.ContentManagement;
using Tridion.ContentManager;
using System.Xml;
using System.Xml.XPath;
using System.Collections;

namespace GenericIndexing.Templating
{
	///<author>Yoav Niran (yoav.niran@sdltridion.com)</author>
	///<summary>Supplies an easy way of loading xml lists of tridion items and then iterating over the items</summary>
	///<remarks>
	///	This code hasnt been thoroughly tested and its performance or stabililty isnt confirmed, use at own discretion 
	///	and only apply to a production environment after proper testing
	/// </remarks>
	public class TridionCollectionBase<T> : IEnumerable<T> where T : IdentifiableObject
	{
		#region Private Members
		private static XmlNamespaceManager m_NSM = null;
		private XmlElement m_TridionList = null;
		private XmlNodeList m_Nodes = null;
		private Engine m_Engine = null;
		private string m_ItemXmlName = "Item";
		private List<string> m_Titles = null;
		private List<string> m_IDs = null;
		#endregion

		#region Constructors

		/// <summary>
		/// Creates a new Tridion collection by calling GetListItems using the provided filter on the provided organizational item
		/// </summary>
		/// <param name="engine">An instantiated Engine object <typeparamref name="Tridion.ContentManager.Templating.Engine"/></param>
		/// <param name="root">The organizational element to get the list of items for</param>
		/// <param name="filter">An instantiated Filter object with preset conditions</param>
		public TridionCollectionBase(Engine engine, OrganizationalItem root, Filter filter)
			: this(engine, root.GetListItems(new OrganizationalItemItemsFilter(filter,engine.GetSession())), String.Empty)
		{ }

		/// <summary>
		/// Creates a new Tridion collection by calling GetListItems using the provided filter on the provided organizational item
		/// the collection is then filtered by applying the xpath query provided
		/// </summary>
		/// <param name="engine">An instantiated Engine object <typeparamref name="Tridion.ContentManager.Templating.Engine"/></param>
		/// <param name="root">The organizational element to get the list of items for</param>
		/// <param name="filter">An instantiated Filter object with preset conditions</param>
		/// <param name="xpath">An Xpath expression to further filter the retrieved results </param>
		public TridionCollectionBase(Engine engine, OrganizationalItem root, Filter filter, XPathExpression xpath) : this(engine, root.GetListItems(new OrganizationalItemItemsFilter(filter,engine.GetSession())), xpath) { }

		/// <summary>
		/// Creates a new Tridion collection by calling GetListItems using the provided filter on the provided organizational item
		/// </summary>
		/// <param name="engine">An instantiated Engine object <typeparamref name="Tridion.ContentManager.Templating.Engine"/></param>
		/// <param name="root">The organizational element to get the list of items for</param>
		/// <param name="filter">An instantiated Filter object with preset conditions</param>
		/// <param name="itemXmlName">In some GetListItems methods the xml element name is different from 'Item' this allows for a different name</param>
		public TridionCollectionBase(Engine engine, OrganizationalItem root, Filter filter, string itemXmlName)
			: this(engine, root.GetListItems(new OrganizationalItemItemsFilter(filter,engine.GetSession())), itemXmlName)
		{ }

		/// <summary>
		/// Creates a new Tridion collection by loading the list provided, the list should be a result of calling GetListItems
		/// or a variation of that on a Tridion item.
		/// </summary>
		/// <param name="engine">An instantiated Engine object <typeparamref name="Tridion.ContentManager.Templating.Engine"/></param>
		/// <param name="list">An XML fragment containing the result of a GetList method </param>
		public TridionCollectionBase(Engine engine, XmlElement list)
			: this(engine, list, String.Empty)
		{ }

		/// <summary>
		/// Creates a new Tridion collection by loading the list provided, the list should be a result of calling GetListItems
		/// or a variation of that on a Tridion item.
		/// </summary>
		/// <param name="engine">An instantiated Engine object <typeparamref name="Tridion.ContentManager.Templating.Engine"/></param>
		/// <param name="list">An XML fragment containing the result of a GetList method </param>
		/// <param name="xpath">An Xpath expression to further filter the retrieved results</param>
		public TridionCollectionBase(Engine engine, XmlElement list, XPathExpression xpath)
		{
			Init(engine, list);

			m_Nodes = list.SelectNodes(xpath.Expression, NSManager);
		}

		/// <summary>
		/// Creates a new Tridion collection by loading the list provided, the list should be a result of calling GetListItems
		/// or a variation of that on a Tridion item.
		/// </summary>
		/// <param name="engine">An instantiated Engine object <typeparamref name="Tridion.ContentManager.Templating.Engine"/></param>
		/// <param name="list">An XML fragment containing the result of a GetList method </param>
		/// <param name="itemXmlName">In some GetListItems methods the xml element name is different from 'Item' this allows for a different name</param>
		public TridionCollectionBase(Engine engine, XmlElement list, string itemXmlName)
		{
			Init(engine, list);

			if (!String.IsNullOrEmpty(itemXmlName)) m_ItemXmlName = itemXmlName;

			m_Nodes = list.SelectNodes(String.Format("//tcm:{0}", m_ItemXmlName), NSManager);
		}

		#endregion

		#region Properties

		/// <summary>
		/// A list of titles for the items in the collection
		/// </summary>
		public List<string> Titles
		{
			get
			{
				if (m_Titles == null)
				{
					m_Titles = new List<string>();
					foreach (XmlNode node in m_Nodes)
					{
						m_Titles.Add(node.Attributes["Title"].InnerText);
					}
				}

				return m_Titles;
			}

		}

		/// <summary>
		/// A list of TCMURIs for the items in the collection
		/// </summary>
		public List<string> IDs
		{
			get
			{
				if (m_IDs == null)
				{
					m_IDs = new List<string>();

					foreach (XmlNode node in m_Nodes)
					{
						m_IDs.Add(node.Attributes["ID"].InnerText);
					}
				}

				return m_IDs;
			}
		}

		public T this[int index]
		{
			get
			{
				return GetItem(m_Nodes[index]);
			}

		}

		public T this[string tcmURI]
		{
			get
			{
				T item = null;

				XmlNode node =  m_TridionList.SelectSingleNode(String.Format("//tcm:{0}[@ID = '{1}']", m_ItemXmlName, tcmURI), NSManager);

				if (node != null) item = GetItem(node);

				return item;
			}
		}

		/// <summary>
		/// A NamespaceManager initialized with a few namespaces such: tcm, xlink and xhtml.
		/// </summary>
		public static XmlNamespaceManager NSManager
		{
			get
			{
				if (m_NSM == null)
				{
					m_NSM = new XmlNamespaceManager(new NameTable());

					m_NSM.AddNamespace("tcm", "http://www.tridion.com/ContentManager/5.0");
					m_NSM.AddNamespace("xlink", "http://www.w3.org/1999/xlink");
					m_NSM.AddNamespace("xhtml", "http://www.w3.org/1999/xhtml");
				}

				return m_NSM;
			}
		}

		public int Count
		{
			get { return m_Nodes.Count; }
		}

		public bool Contains(TcmUri uri)
		{
			XmlNode node = m_TridionList.SelectSingleNode(String.Format("//tcm:{0}[@ID = '{1}']", ItemXmlName, uri.ToString()), NSManager);

			return (node != null);
		}

		public bool Contains(T item)
		{
			return Contains(item.Id);
		}

		/// <summary>
		/// The name of the elements in the xml list, default is 'Item'
		/// </summary>
		public string ItemXmlName
		{
			get { return m_ItemXmlName; }
			set { m_ItemXmlName = value; }
		}

		/// <summary>
		/// Returns the xml of the list
		/// </summary>
		public string ListXML
		{
			get { return m_TridionList.OuterXml; }
		}
		#endregion

		#region IEnumerable

		public IEnumerator<T> GetEnumerator()
		{
			foreach (XmlNode node in m_Nodes)
			{
				yield return GetItem(node);
			}

			yield break;

		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();

		}
		#endregion

		#region Methods

		private void Init(Engine engine, XmlElement list)
		{
			m_TridionList = list;
			m_Engine = engine;
		}

		/// <summary>
		/// Populates the collection with items from the original list based on the xpath query
		/// </summary>
		/// <param name="xpathQuery">
		/// An XPath expression to select items from the original list
		/// </param>
		public void Refresh(string xpathQuery)
		{
			m_Nodes = m_TridionList.SelectNodes(xpathQuery);
		}

		/// <summary>
		/// Iterates over the loaded list and returns a list of instantied objects 
		/// </summary>
		/// <returns>List of T</returns>
		public List<T> GetItems()
		{
			//List<T> items = new List<T>();

			//foreach (T item in this)
			//{
			//    items.Add(item);
			//}

			return new List<T>(this); //items;
		}

		/// <summary>
		/// Get an item from the list by its title
		/// </summary>
		public T GetItem(string title)
		{
			T item = null;
			
			int idx = Titles.IndexOf(title);

			if (idx > -1) item =  this[idx];

			return item;
		}

		private T GetItem(XmlNode node)
		{
			T item = null;

			item = m_Engine.GetObject(node.Attributes["ID"].Value) as T;

			return item;
		}
		#endregion
	}
}
