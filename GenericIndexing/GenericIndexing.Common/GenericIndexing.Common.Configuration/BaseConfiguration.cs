/*
 * Filename:      IConfiguration.cs
 * Author:        Pankaj Gaur
 * Creation date: December 30, 2005
 */

using System;
using System.Xml;

using GenericIndexing.Common.Configuration.Interface;

namespace GenericIndexing.Common.Configuration
{
	/// <summary>
	/// Base class for encapsulating Configuration information.  It provides
	/// default implementation of the methods declared in the
	/// <em>IConfiguration</em> interface so that individual Configuration
	/// classes need not repeat the code for basic information.
	/// </summary>
	public abstract class BaseConfiguration : IConfiguration
	{
		#region Protected instance variables

		/// <summary>
		/// Name of the current configuration.
		/// </summary>
		protected String  _Name;

		/// <summary>
		/// Denotes if the current configuration is writable.
		/// </summary>
		protected bool _Writable;

		#endregion

		#region Protected instance constructors

		/// <summary>
		/// Default class constructor.  Sets 'writable' to true.
		/// </summary>
		protected BaseConfiguration()
		{
			_Writable = true;
		}

		#endregion

		#region Implementation of IConfiguration methods

		/// <summary>
		/// Determines if the current configuration is read-only.
		/// </summary>
		/// <returns>
		/// True if the configuration is read-only, false if it is read-write.
		/// </returns>
		public Boolean IsReadOnly()
		{
			return !_Writable;
		}

		/// <summary>
		/// Returns the name of the current configuration.
		/// </summary>
		/// <returns>Name of the current configuration.</returns>
		public String GetName()
		{
			return _Name;
		}

		/// <summary>
		/// Should be overridden by child classes to provide their custom
		/// implementation for pre-loading configuration data from XML files.
		/// </summary>
		/// <param name="rootNode">Root XML node for a configuration file.</param>
		public abstract void PreLoad(XmlNode rootNode);

		/// <summary>
		/// Sets the name of the current configuration.
		/// </summary>
		/// <param name="configurationName">Name of the current configuration.</param>
		public void SetName(String configurationName)
		{
			_Name = configurationName;
		}

		/// <summary>
		/// Sets the current configuration read-only.  Once made read-only, a
		/// configuration cannot be made read-write.
		/// </summary>
		public void SetReadOnly()
		{
			_Writable = false;
		}

		#endregion
	}
}
