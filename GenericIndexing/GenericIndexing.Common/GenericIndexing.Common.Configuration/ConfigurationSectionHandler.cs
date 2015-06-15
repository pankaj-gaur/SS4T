/*
 * Filename:      ConfigurationSectionHandler.cs
 * Author:        Pankaj Gaur
 * Creation date: January 3, 2006
 */

using System;
using System.Configuration;

namespace GenericIndexing.Common.Configuration
{
	/// <summary>
	/// Section handler for <em>baseconfig4net</em> section in .NET application
	/// configuration files (i.e. <strong>Web.config</strong> or
	/// <strong>App.config</strong>.
	/// </summary>
	public class ConfigurationSectionHandler: IConfigurationSectionHandler
	{
		#region Public instance constructors

		/// <summary>
		/// Default class constructor.
		/// </summary>
		public ConfigurationSectionHandler()
		{
		}

		#endregion

		#region Implementation of IConfigurationSectionHandler methods

		/// <summary>
		/// Returns the XML node 
		/// </summary>
		/// <param name="parent"></param>
		/// <param name="configContext"></param>
		/// <param name="section"></param>
		/// <returns></returns>
		public object Create(object parent, object configContext, System.Xml.XmlNode section)
		{
			return section;
		}

		#endregion
	}
}
