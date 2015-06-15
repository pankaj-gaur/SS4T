/*
 * Filename:      IConfigurationCache.cs
 * Author:        Pankaj Gaur
 * Creation date: December 31, 2005
 */

using System;
using System.Globalization;

namespace GenericIndexing.Common.Configuration.Interface
{
	/// <summary>
	/// Interface for a Configuration cache.  A configuration cache holds
	/// references to configuration objects.
	/// </summary>
	public interface IConfigurationCache
	{
		/// <summary>
		/// Returns a configuration object, provided its name.
		/// </summary>
		/// <param name="configName">Name of the configuration object to load.</param>
		/// <returns>
		/// A reference to an object implementing the <em>IConfiguration</em>
		/// interface if the configuration object is found in the cache, or can be
		/// loaded from the file system, an exception otherwise.
		/// </returns>
		IConfiguration GetConfiguration(String configName);

		/// <summary>
		/// Returns a culture-specific configuration object, provided its name.
		/// </summary>
		/// <param name="configName">Name of the configuration object to load.</param>
		/// <param name="culture">
		/// A <em>System.Globalization.CultureInfo</em> object containing details
		/// of the culture for which the configuration is required.
		/// </param>
		/// <returns>
		/// A reference to an object implementing the <em>IConfiguration</em>
		/// interface if the configuration object is found in the cache, or can be
		/// loaded from the file system, an exception otherwise.
		/// </returns>
		IConfiguration GetConfiguration(String configName, CultureInfo culture);

		/// <summary>
		/// Removes a configuration object from the cache.
		/// </summary>
		/// <param name="configName">Name of the configuration object to remove.</param>
		void RemoveConfiguration(String configName);

		/// <summary>
		/// Removes a culture-specific configuration object from the cache.
		/// </summary>
		/// <param name="configName">Name of the configuration object to remove.</param>
		/// <param name="culture">
		/// Culture for which the configuration needs to be removed.
		/// </param>
		void RemoveConfiguration(String configName, CultureInfo culture);
	}
}
