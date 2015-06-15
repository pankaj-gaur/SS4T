/*
 * Filename:      ConfigurationFactory.cs
 * Author:        Pankaj Gaur
 * Creation date: December 30, 2005
 */

using System;
using System.Collections;
using System.Reflection;

using GenericIndexing.Common.Configuration.Interface;

namespace GenericIndexing.Common.Configuration
{
	/// <summary>
	/// Creates objects implementing the <em>IConfiguration</em> interface.
	/// </summary>
	public class ConfigurationFactory
	{
		#region Private static variables

		// Internal cache of IConfiguration types.
		private static readonly Hashtable            CACHE    = new Hashtable();

		// Singleton instance.
		private static readonly ConfigurationFactory INSTANCE = new ConfigurationFactory();

		#endregion

		#region Static constructor

		/// <summary>
		/// Populates the internal cache of IConfiguration types.
		/// </summary>
		static ConfigurationFactory()
		{
			// Add DefaultPropertyConfiguration type to the cache.
            System.Type configObjectType = typeof(GenericIndexing.Common.Configuration.DefaultPropertyConfiguration);
			ConstructorInfo constructorInfo = configObjectType.GetConstructor(new System.Type[0]);

            CACHE["GenericIndexing.Common.Configuration.Interface.IPropertyConfiguration"] = constructorInfo;
		}

		#endregion

		#region Private instance constructors

		/// <summary>
		/// Default class constructor.  Made private so that class instances
		/// cannot be created.
		/// </summary>
		private ConfigurationFactory()
		{
		}

		#endregion

		#region Public static methods for singleton management

		/// <summary>
		/// Returns the singleton instance of the class.
		/// </summary>
		/// <returns>A <em>ConfigurationFactory</em> object.</returns>
		public static ConfigurationFactory GetInstance()
		{
			return INSTANCE;
		}

		#endregion

		#region Public instance methods

		/// <summary>
		/// Returns an object implementing a specific child interface of
		/// <em>IConfiguration</em>.  It first performs a look up in the internal
		/// cache of <em>IConfiguration</em> types to find a class that
		/// implements the requested interface.
		/// </summary>
		/// <param name="configTypeName">
		/// Name of the interface for which an instance is required.
		/// </param>
		/// <returns>An object implementing the requested interface.
		/// </returns>
		public IConfiguration GetConfiguration(String configTypeName)
		{
			if(configTypeName == null)
			{
				throw new ArgumentNullException("configTypeName",
												"The parameter [configTypeName] is null.  Cannot create a Configuration object.");
			}

			ConstructorInfo constructorInfo = (ConstructorInfo)CACHE[configTypeName];

			IConfiguration config = null;
			if(constructorInfo != null)
			{
				config = (IConfiguration)constructorInfo.Invoke(null);
			}

			return config;
		}

		/// <summary>
		/// Returns an instance of <em>DefaultPropertyConfiguration</em> which
		/// is a default implementation of the <em>IConfiguration</em> interface.
		/// </summary>
		/// <returns>A <em>DefaultPropertyConfiguration</em> instance.</returns>
		public IConfiguration GetDefaultConfiguration()
		{
            return (IConfiguration)((ConstructorInfo)CACHE["GenericIndexing.Common.Configuration.Interface.IPropertyConfiguration"]).Invoke(null);
		}

		#endregion
	}
}
