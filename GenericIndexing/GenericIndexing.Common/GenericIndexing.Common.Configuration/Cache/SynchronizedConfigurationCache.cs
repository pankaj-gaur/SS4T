/*
 * Filename:      SynchronizedConfigurationCache .cs
 * Author:        Pankaj Gaur
 * Creation date: December 31, 2005
 */

using System;
using System.Collections;
using System.Globalization;
using System.IO;
using System.Xml;

using GenericIndexing.Common.Configuration.Interface;

namespace GenericIndexing.Common.Configuration.Cache
{
	/// <summary>
	/// Provides a synchronized implementation of the <em>IConfigurationCache</em>
	/// interface.  It uses an unsynchronized <em>System.Collections.Hashtable</em>
	/// to store a configuration cache that is locked when the Hashtable needs
	/// to be changed.  This ensures that the cache is synchronized when it is
	/// changed, while at the same time also makes sure that read operations on
	/// the cache are not costly (which would be the case had a synchronized
	/// Hashtable been used).
	/// </summary>
	public class SynchronizedConfigurationCache : IConfigurationCache
	{
		#region Private instance variables

		// Cache of Configuration objects.
		private Hashtable _Cache;

		#endregion

		#region Public instance constructors
		
		/// <summary>
		/// Default class constructor.
		/// </summary>
		public SynchronizedConfigurationCache()
		{
			_Cache = new Hashtable();
		}

		#endregion

		#region Implementation of IConfigurationCache

		/// <summary>
		/// Returns a configuration object, provided its name.
		/// </summary>
		/// <param name="configName">Name of the configuration object to load.</param>
		/// <returns>
		/// A reference to an object implementing the <em>IConfiguration</em>
		/// object if the configuration object is found in the cache, or can be
		/// loaded from the file system, an exception otherwise.
		/// </returns>
		public IConfiguration GetConfiguration(String configName)
		{
			// Look for the Configuration object in the cache.
			IConfiguration config = (IConfiguration)_Cache[configName];

			if(config == null)
			{
				lock(_Cache)
				{
					// Configuration object was not found in the cache.  Try to load
					// it from the file system.
					config = LoadConfiguration(configName);

					if(config != null)
					{
						// Save the loaded object in the cache for future use.
						_Cache[configName] = config;
					}
				}
			}

			return config;
		}

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
		public IConfiguration GetConfiguration(String configName, CultureInfo culture)
		{
			// Look for a language and country specific configuration.
			String countrySpecificConfigName = configName + "." + culture.Name;
			String languageSpecificConfigName = configName + "." + culture.TwoLetterISOLanguageName;

			IConfiguration config = (IConfiguration)_Cache[countrySpecificConfigName];

			if(config == null)
			{
				// Try loading a language and country specific configuration.
				try
				{
					config = GetConfiguration(countrySpecificConfigName);
				}
				catch(System.Exception)
				{
				}
			}

			if(config == null)
			{
				// Look for a language specific configuration.
				config = (IConfiguration)_Cache[languageSpecificConfigName];
			}

			if(config == null)
			{
				// Try loading a language specific configuration.
				try
				{
					config = GetConfiguration(languageSpecificConfigName);
				}
				catch(System.Exception)
				{
				}
			}

			if(config == null)
			{
				// Try loading the default configuration with the given name.
				config = GetConfiguration(configName);
			}

			return config;
		}

		/// <summary>
		/// Removes a configuration object from the cache.
		/// </summary>
		/// <param name="configName">Name of the configuration object to remove.</param>
		public void RemoveConfiguration(String configName)
		{
			_Cache.Remove(configName);
		}

		/// <summary>
		/// Removes a culture-specific configuration object from the cache.
		/// </summary>
		/// <param name="configName">Name of the configuration object to remove.</param>
		/// <param name="culture">
		/// Culture for which the configuration should be removed.
		/// </param>
		public void RemoveConfiguration(String configName, CultureInfo culture)
		{
			// Remove all culture-specific instances of the named configuration.
			_Cache.Remove(configName + "." + culture.Name);
			_Cache.Remove(configName + "." + culture.TwoLetterISOLanguageName);
		}

		#endregion

		#region Private instance methods

		/// <summary>
		/// Creates a <em>Configuration object for a given XML document.</em>
		/// </summary>
		/// <param name="configName">Name of the <em>Configuration</em>.</param>
		/// <param name="rootNode">Root <em>System.Xml.XmlNode</em> for the Configuration.</param>
		/// <returns></returns>
		private IConfiguration CreateConfiguration(String configName, XmlNode rootNode)
		{
			// From the configuration XML, extract interface which is to be returned, specific class names,
			// configuration read mode and configuration watch mode.
			String configInterfaceName = GetAttributeValue(rootNode, ConfigurationConstants.AttributeNames.INTERFACE_NAME);
			String configClassName = GetAttributeValue(rootNode, ConfigurationConstants.AttributeNames.CLASS_NAME);
			String configReadMode = GetAttributeValue(rootNode, ConfigurationConstants.AttributeNames.OPEN_MODE);
			String configWatchMode = GetAttributeValue(rootNode, ConfigurationConstants.AttributeNames.WATCH_MODE);

			IConfiguration config = null;
			if(!string.IsNullOrEmpty(configClassName))
			{
				// Try loading a Configuration object by class name first.
				config = ConfigurationFactory.GetInstance().GetConfiguration(configClassName);
			}
			else if(!string.IsNullOrEmpty(configInterfaceName))
			{
				// If a class name has not been specified, try loading it by
				// interface name.
				config = ConfigurationFactory.GetInstance().GetConfiguration(configInterfaceName);
			}

			if(config == null)
			{
				// If loading by class and interface names fails, try loading using a
				// default Configuration class.
				config = ConfigurationFactory.GetInstance().GetDefaultConfiguration();
			}

			// Set properties on the Configuration.
			config.SetName(configName);
			config.PreLoad(rootNode);
			if(configReadMode == ConfigurationConstants.ConfigurationTypes.READ_ONLY)
			{
				config.SetReadOnly();
			}

			// Check if the configuration file has been set to be watched for changes.
			if(configWatchMode == ConfigurationConstants.WatchModes.FULL)
			{
				// Put a watchdog on the current configuration so that any changes
				// can be absorbed automatically the cache.
				ConfigurationManager.PutWatch(configName);
			}

			return config;
		}

		/// <summary>
		/// Returns the value of an XML attribute associated with a node.
		/// </summary>
		/// <param name="xmlNode">XML node to which the attribute is associated.</param>
		/// <param name="attributeName">Name of the attribute.</param>
		/// <returns>
		/// Value of the attribute if it is found with the node, null otherwise.
		/// </returns>
		private String GetAttributeValue(XmlNode xmlNode, String attributeName)
		{
			String attributeValue = null;

			XmlAttribute xmlAttribute = xmlNode.Attributes[attributeName];
			if(xmlAttribute != null)
			{
				attributeValue = xmlAttribute.Value;
			}

			return attributeValue;
		}

		/// <summary>
		/// Loads a <em>Configuration</em> object based on its name.  It looks
		/// for the XML file containing the configuration in root path
		/// </summary>
		/// <param name="configName">Name of the configuration.</param>
		/// <returns>
		/// An object implementing the <em>IConfiguration</em> interface if the
		/// Configuration can be successfully loaded, an exception otherwise.
		/// </returns>
		private IConfiguration LoadConfiguration(String configName)
		{
			// Use the root configuration path to create the fully qualified
			// path to the configuration file.
			String fullConfigPath = String.Format("{0}{1}.{2}", ConfigurationManager.GetInstance().RootPath,
									configName, ConfigurationManager.GetInstance().FileExtension);

			// It is important to check that the configuration file exists
			// before attempting to read it.
			if(!File.Exists(fullConfigPath))
			{
				throw new ArgumentException(String.Format("The file [{0}] was not found or access was denied to it.  Unable to load the configuration.", fullConfigPath));
			}

			// Create a validating XML reader for the configuration file.
			// However, disable schema validations and turn on entity
			// expansion.
			XmlDocument configDocument = XMLUtil.GetValidatedXMLDocument(fullConfigPath);

			// Read the root element of the document.
			XmlNode rootNode = configDocument.LastChild;

			if(rootNode.LocalName != ConfigurationConstants.SectionNames.CONFIGURATION)
			{
				throw new ApplicationException(String.Format("The document [{0}] does not begin with the root element 'Configuration'.  Unable to get the configuration.", configName));
			}

			return CreateConfiguration(configName, rootNode);
		}

		#endregion
	}
}
