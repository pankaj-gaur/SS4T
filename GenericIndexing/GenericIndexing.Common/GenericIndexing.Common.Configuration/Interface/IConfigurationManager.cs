/*
 * Filename:      IConfigurationManager
 * Author:        Pankaj Gaur
 * Creation date: December 30, 2005
 */

using System;
using System.Globalization;

namespace GenericIndexing.Common.Configuration.Interface
{
	/// <summary>
	/// Base interface that allows application developers to fetch named
	/// configurations stored relative to the application's root configuration
	/// path.
	/// </summary>
	public interface IConfigurationManager
	{
		/// <summary>
		/// Returns a configuration object, provided its name.
		/// </summary>
		/// <param name="configName">Name of the configuration object.</param>
		/// <returns>
		/// An object implementing the <em>IConfiguration</em> interface
		/// if the configuration is found, an exception otherwise.
		/// </returns>
		IConfiguration GetConfiguration(String configName);

		/// <summary>
		/// Returns a culture-specific configuration object, provided its name.
		/// </summary>
		/// <param name="configName">Name of the configuration object.</param>
		/// <param name="culture">
		/// A <em>System.Globalization.CultureInfo</em> object containing details
		/// of the culture for which a configuration is required.
		/// </param>
		/// <returns>
		/// An object implementing the <em>IConfiguration</em> interface
		/// if the configuration is found, an exception otherwise.
		/// </returns>
		IConfiguration GetConfiguration(String configName, CultureInfo culture);

		/// <summary>
		/// Removes a configuration from the framework cache so that a fresh copy
		/// can be loaded.
		/// </summary>
		/// <param name="configName">Name of the configuration to remove.</param>
		void RemoveConfiguration(String configName);
	}
}
