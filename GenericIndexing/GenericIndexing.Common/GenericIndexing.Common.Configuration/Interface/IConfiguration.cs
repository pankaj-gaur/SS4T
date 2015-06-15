/*
 * Filename:      IConfiguration.cs
 * Author:        Pankaj Gaur
 * Creation date: December 30, 2005
 */

using System;
using System.Xml;

namespace GenericIndexing.Common.Configuration.Interface
{
	/// <summary>
	/// Base interface for all types of Configuration objects.
	/// </summary>
	public interface IConfiguration
	{
		/// <summary>
		/// Determines if the current configuration is read-only.
		/// </summary>
		/// <returns>
		/// True if the configuration is read-only, false if it is read-write.
		/// </returns>
		Boolean IsReadOnly();

		/// <summary>
		/// Returns the name of the current configuration.
		/// </summary>
		/// <returns>Name of the current configuration.</returns>
		String GetName();

		/// <summary>
		/// Pre-loads all configuration data stored in a Configuration
		/// file using the root XML node of the file.
		/// </summary>
		/// <param name="rootNode">Root XML node for the Configuration.</param>
		void PreLoad(XmlNode rootNode);

		/// <summary>
		/// Sets the name of the current configuration.
		/// </summary>
		/// <param name="configurationName">Name of the current configuration.</param>
		void SetName(String configurationName);

		/// <summary>
		/// Sets the current configuration read-only.
		/// </summary>
		void SetReadOnly();
	}
}
