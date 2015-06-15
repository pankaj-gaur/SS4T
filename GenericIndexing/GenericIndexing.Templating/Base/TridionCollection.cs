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
	public class TridionCollection : TridionCollectionBase<IdentifiableObject>
	{
		public TridionCollection(Engine engine, OrganizationalItem root, Filter filter) : base(engine, root, filter) { }

		public TridionCollection(Engine engine, OrganizationalItem root, Filter filter, string itemXmlName) : base(engine, root, filter, itemXmlName) { }

		public TridionCollection(Engine engine, OrganizationalItem root, Filter filter, XPathExpression xpath) : base(engine, root, filter, xpath) { }

		public TridionCollection(Engine engine, XmlElement list, string itemXmlName) : base(engine, list, itemXmlName) { }

		public TridionCollection(Engine engine, XmlElement list, XPathExpression xpath) : base(engine, list, xpath) { }

		public TridionCollection(Engine engine, XmlElement list) : base(engine, list) { }
	}	
}
