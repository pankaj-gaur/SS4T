/*
 * Filename:      ConfigurationManager.cs
 * Author:        Pankaj Gaur
 * Creation date: December 29, 2005
 */

using System;
using System.Globalization;
using System.IO;
using System.Security;
using System.Security.Permissions;
using System.Threading;
using System.Xml;
using System.Configuration;
using GenericIndexing.Common.Configuration.Cache;
using GenericIndexing.Common.Configuration.Interface;
using GenericIndexing.Common.Configuration;

namespace GenericIndexing.Common.Configuration
{
	/// <summary>
	/// <p>
	/// Manages the overall system <em>Configuration</em>.  In this regard,
	/// <em>Configuration</em> is defined as any data that needs to be
	/// externalized from the code that processes the business rules configured
	/// for the system.  In this sense, <em>Configuration</em> can be either
	/// small pieces of information required for the working of the application,
	/// such as, Database Connection strings, external machine addresses, etc.;
	/// or, it can be externalized content required at runtime, such as, error
	/// messages, etc.
	/// </p>
	/// <p>
	/// Although the .NET framework provides Application Configuration files to
	/// allow developers to store <em>Configuration</em> information, there are a
	/// few problems with the default .NET Configuration framework.  These
	/// include:
	/// <br /><br />
	/// <li><strong>Inability to change <em>Configuration</em> information without
	/// recompilation</strong> - This is because the Application Configuration
	/// file is embedded in the executable assembly for the application.
	/// Changing anything in the Configuration file requires a complete rebuild
	/// of the assembly.</li>
	/// <li><strong>Inability to update <em>Configuration</em> information at
	/// runtime</strong> - As the default .NET Configuration file is embedded
	/// inside an assembly that is loaded when the application first starts, it
	/// is not possible to change the Configuration information while the
	/// application is running.  A new version of the compiled assembly with new
	/// (or updated) <em>Configuration</em> information can take effect only after
	/// the application has been stopped and restarted.</li>
	/// <li><strong>Inability to test components independently</strong> - 
	/// Components that are developed as independent assemblies cannot be tested
	/// in isolation until they are plugged into a .NET application.  This is
	/// because .NET Class Libraries are not allowed to have their own
	/// Configuration files.  This is not only quite expensive from an effort
	/// stand-point but also quite cumbersome.  For example, if someone is developing
	/// a Class Library for reading error messages at runtime, the classes being
	/// developed cannot be unit tested until the Class Library has been plugged into
	/// an executable assembly.  All the code written as part of an executable
	/// assembly just for the purposes of unit testing some other code is simply
	/// throw-away and hence a waste of effort.</li>
	/// </p>
	/// <p>
	/// This class is part of a generic framework that allows externalizing
	/// <em>Configuration</em> information and storing it on the file system as
	/// XML files.  The only requirement to use this framework is to provide it
	/// with a root path where all external <em>Configuration</em> files will be
	/// stored.
	/// </p>
	/// <p>
	/// It is intended that the entire <em>Configuration</em> for an application
	/// be accessible from a central location.  This is required in order to
	/// ensure that any changes to the <em>Configuration</em> can be monitored
	/// centrally and dependent objects be notified of the changes.  For this reason,
	/// this class has been implemented as a <strong>Singleton</strong>.
	/// </p>
	/// <p>
	/// There are only two pieces of information that the <em>Configuration framework
	/// </em> needs before it can start reading XML files and providing access to the
	/// information inside them.  These are:
	/// <br /><br />
	/// <ol>
	/// <li><strong>Configuration root path: </strong>This should be a path on a
	/// Windows file system where all XML configuration files will be stored.  The
	/// files can be further organized within the base folder into sub-folders and it
	/// is not required for all the files to stored directly under the base folder
	/// itself.  It is also required that the Windows account under which the .NET
	/// application is running to have at least read-access to the base folder and
	/// all folders and files under it.  The root path information is mandatory for
	/// the framework to function as failing this the framework will not be able to
	/// figure out where the configuration files are located.
	/// </li>
	/// <li><strong>Configuration file extension: </strong>All configuration files
	/// should have the same extension for the framework to pick them up.  This is an
	/// optional piece of information and if not supplied is assumed to be ".xml".
	/// </li>
	/// </ol>
	/// </p>
	/// <p>
	/// In order to overcome the shortcomings of the default .NET Configuration
	/// system, the current framework provides multiple ways of passing the starting
	/// information to it.  The base location of configuration files and the file
	/// extension can be passed to the framework in one of the following ways:
	/// <br /><br />
	/// <ol>
	/// <li><strong>Using the Application Configuration: </strong>It is possible to
	/// include a <em>baseconfig4net</em> section in the .NET application's
	/// configuration file, i.e., <strong>Web.config</strong> or
	/// <strong>App.config</strong>.  Information about the base location of
	/// <em>Configuration</em> files and file extension can be included under the
	/// <em>baseconfig4net</em> section as individual nodes.  The following
	/// example shows how to set the root configuration path to
	/// <em>C:\AppPath\Config</em> and file extension to <em>config</em> through
	/// the <strong>Web.config</strong> file:
	/// <br /><br />
	/// &lt;configuration&gt;<br />
	///	&amp;nbsp;&amp;nbsp;&lt;configSections&gt;<br />
	/// <strong>
	/// </strong>
	///	&amp;nbsp;&amp;nbsp;&amp;nbsp;&amp;nbsp;&lt;/configSections&gt;<br />
	/// <strong>
	///	&amp;nbsp;&amp;nbsp;&amp;nbsp;&amp;nbsp;&lt;baseconfig4net&gt;<br />
	///	&amp;nbsp;&amp;nbsp;&amp;nbsp;&amp;nbsp;&amp;nbsp;&amp;nbsp;&amp;nbsp;&amp;nbsp;&lt;rootPath&gt;C:\AppPath\Config&lt;/rootPath&gt;<br />
	///	&amp;nbsp;&amp;nbsp;&amp;nbsp;&amp;nbsp;&amp;nbsp;&amp;nbsp;&amp;nbsp;&amp;nbsp;&lt;fileExtension&gt;config&lt;/fileExtension&gt;<br />
	///	&amp;nbsp;&amp;nbsp;&amp;nbsp;&amp;nbsp;&lt;/baseconfig4net&gt;<br />
	/// </strong>
	///	&lt;/configuration&gt;
	/// </li>
	/// <li><strong>Using <em>System.Xml.XmlElement</em>: </strong>It is also
	/// possible to pass an <em>System.Xml.XmlElement</em> to the framework with
	/// the root node set as <em>baseconfig4net</em>.</li>
	/// <li><strong>Using <em>System.IO.Stream</em>: </strong>If a
	/// <em>System.Xml.XmlElement</em> is not available directly, a
	/// <em>System.IO.Stream</em> reference can be passed to the framework.  This
	/// reference should be for a valid XML string containing a <em>baseconfig4net</em>
	/// section.  It does not matter where the <em>baseconfig4net</em> element is in
	/// the XML string.</li>
	/// <li><strong>Using <em>System.IO.FileInfo</em>: </strong>The last possible
	/// way of configuring the framework is by passing a valid
	/// <em>System.IO.FileInfo</em> reference to it.  This reference should point to
	/// a valid XML file containing a <em>baseconfig4net</em> section.  It does not
	/// matter where the <em>baseconfig4net</em> element is in the XML file.</li>
	/// </ol>
	/// </p>
	/// </summary>
	public sealed class ConfigurationManager : IConfigurationManager
	{
		#region Private static variables

		// File extension for all configuration files.
		private static String                        FILE_EXTENSION = ConfigurationConstants.FileExtensions.DEFAULT;

		// Singleton instance.
		private static readonly ConfigurationManager INSTANCE       = new ConfigurationManager();

		// Root path for all configuration files.
		private static String                        ROOT_PATH;

		#endregion

		#region Private instance variables

		// Internal cache of IConfiguration objects.
		private IConfigurationCache _Cache;

		#endregion

		#region Private instance constructors

		/// <summary>
		/// Default class constructor.  Made private so that class instances cannot
		/// be created directly.
		/// </summary>
		private ConfigurationManager()
		{
			_Cache = new SynchronizedConfigurationCache();
		}

		#endregion

		#region Public static methods for Singleton management

		/// <summary>
		/// Returns singleton instance for the current class.
		/// </summary>
		/// <returns></returns>
		public static ConfigurationManager GetInstance()
		{
			if(ROOT_PATH == null)
			{
				// As the root configuration path is not set, initialize the
				// framework using the default mechanism of using the Application
				// Configuration file.
				Initialize();
			}

			return INSTANCE;
		}

		#endregion

		#region Public static methods for initializing the framework

		/// <summary>
		/// Initializes the framework using the Application Configuration file.  It
		/// searches for a section named <em>baseconfig4net</em> in the .NET
		/// application's configuration file (i.e. <strong>Web.config</strong> or
		/// <strong>App.config</strong> to determine the root location and file
		/// extension of all configuration files.
		/// </summary>
		public static void Initialize()
		{
			// Make sure that the system is not initialized again if already done.
			if(ROOT_PATH == null)
			{
				Initialize(System.Configuration.ConfigurationManager.GetSection(ConfigurationConstants.SectionNames.ROOT_CONFIGURATION) as XmlElement);
			}
		}

		/// <summary>
		/// Initializes the framework using a <em>System.IO.FileInfo</em> object.
		/// This object must point to an XML file containing a <em>baseconfig4net</em>
		/// node.
		/// </summary>
		/// <param name="configFileInfo">
		/// A <em>System.IO.FileInfo</em> reference.
		/// </param>
		public static void Initialize(FileInfo configFileInfo)
		{
            //FileInfo file = new FileInfo(
			// Make sure that the system is not initialized again if already done.
			if(ROOT_PATH == null)
			{
				if(configFileInfo == null)
				{
					throw new System.ArgumentNullException("configFileInfo",
														"The parameter [configFileInfo] cannot be null.  Unable to initialize the XML Configuration Framework.  This can happen when a valid FileInfo reference is not passed to this function.  Make sure to pass a valid FileInfo object as the parameter [configFileInfo] to this method.");
				}

				// Check to make sure that the file pointed to by 'configFileInfo'
				// exists.
				if(File.Exists(configFileInfo.FullName))
				{
					Initialize(configFileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.Read));
				}
			}
		}

		/// <summary>
		/// Initializes the framework using a <em>System.IO.Stream</em> object.
		/// This object must contain XML with a <em>baseconfig4net</em> node.
		/// </summary>
		/// <param name="configStream">
		/// A <em>System.IO.Stream</em> reference.
		/// </param>
		public static void Initialize(Stream configStream)
		{
			// Make sure that the system is not initialized again if already done.
			if(ROOT_PATH == null)
			{
				if(configStream == null)
				{
					throw new System.ArgumentNullException("configStream",
															"The parameter [configStream] cannot be null.  Unable to initialize the XML Configuration Framework.  This can happen when an attempt is made to pass a reference to this function which points to a file which is either non-existent or cannot be accessed due to reasons such as file permissions.  Make sure to check that the file actually exists and is accessible.");
				}

				// Load the XML content into an XML document.
				XmlDocument configDocument = XMLUtil.GetValidatedXMLDocument(configStream);

				// Read all nodes named 'baseconfig4net' from the document.
				XmlNodeList configNodes = configDocument.GetElementsByTagName(ConfigurationConstants.SectionNames.ROOT_CONFIGURATION);

				// Check to make sure that only one 'baseconfig4net' section was read.
				if(configNodes.Count == 1)
				{
					// Proceed to initialize the system using the XML node read.
					Initialize(configNodes[0] as XmlElement);
				}
				else
				{
					throw new System.ArgumentException(String.Format("The parameter [configStream] points to a stream that has either zero or more than one '{0}' elements.  Unable to initialize the XML Configuration Framework.", ConfigurationConstants.SectionNames.ROOT_CONFIGURATION), "configName");
				}
			}
		}

		/// <summary>
		/// Initializes the framework using a <em>System.Xml.XmlElement</em> set to
		/// <em>config4net</em>.
		/// </summary>
		/// <param name="configRootElement">
		/// Root element for the Configuration section.
		/// </param>
		public static void Initialize(XmlElement configRootElement)
		{
			// Make sure that the system is not initialized again if already done.
			if(ROOT_PATH == null)
			{
				if(configRootElement == null)
				{
					throw new System.ArgumentNullException("configRootElement",
															String.Format("The parameter [configRootElement] cannot be null.  Unable to initialize the XML Configuration Framework.  This can happen when the framework is called from a non-executable assembly (such as, a Windows class library), or the executable assembly does not have a configuration file (such as, App.config or Web.config), or the configuration file does not contain a '{0}' section.", ConfigurationConstants.SectionNames.ROOT_CONFIGURATION));
				}

				if(configRootElement.LocalName != ConfigurationConstants.SectionNames.ROOT_CONFIGURATION)
				{
					throw new System.ApplicationException(String.Format("XmlElement does not start with '{0}'.  Unable to initialize the XML Configuration Framework.", ConfigurationConstants.SectionNames.ROOT_CONFIGURATION));
				}

				// Go through the child elements of the XML node to determine if a
				// configuration root path and file extension has been provided.
				String rootPath = null;
				String fileExtension = null;
				foreach(XmlElement childElement in configRootElement)
				{
					if(rootPath == null && childElement.LocalName == ConfigurationConstants.PropertyNames.ROOT_PATH)
					{
						rootPath = childElement.FirstChild.Value;
					}
					else if(fileExtension == null && childElement.LocalName == ConfigurationConstants.PropertyNames.FILE_EXTENSION)
					{
						fileExtension = childElement.FirstChild.Value;
					}
				}

				if(rootPath == null)
				{
					throw new System.ApplicationException(String.Format("Unable to find the element called [rootPath] under the '{0}' section.  Unable to initialize the XML Configuration Framework.", ConfigurationConstants.SectionNames.ROOT_CONFIGURATION));
				}

				ROOT_PATH = rootPath;

				if(fileExtension != null)
				{
					FILE_EXTENSION = fileExtension;
				}
			}
		}

		#endregion

		#region Public static methods for watching configuration files

		/// <summary>
		/// Puts a watchdog on a configuration file so that any changes to the file
		/// are picked up and the configuration cache refreshed.
		/// </summary>
		/// <param name="configName">Name of the configuration to watch.</param>
		[FileIOPermission(SecurityAction.Demand, Unrestricted=false)]
		public static void PutWatch(String configName)
		{
			ConfigurationWatcher.StartWatching(configName);
		}

		#endregion

		#region Public properties

		/// <summary>
		/// Root Path property.
		/// </summary>
		public String RootPath
		{
			get { return ROOT_PATH; }
		}

		/// <summary>
		/// File Extension property.
		/// </summary>
		public String FileExtension
		{
			get { return FILE_EXTENSION; }
		}

		#endregion

		#region Implementation of IConfigurationManager methods

		/// <summary>
		/// <p>
		/// Returns an object implementing the <em>IConfiguration</em> interface,
		/// provided its name.  A <em>Configuration</em> object is loaded from
		/// an XML file with a root element called <em>Configuration</em>.  The
		/// root element can also have three optional attributes, as follows:
		/// </p>
		/// <p>
		/// <ol>
		/// <li><strong>mode</strong> - This determines whether the Configuration
		/// contained in the file is <em>read-only</em> or <em>read-write</em>.
		/// This attribute is optional and if not provided the Configuration is
		/// assumed to be <em>read-write</em>.</li>
		/// <li><strong>interface</strong> - Indicates a child interface of
		/// <em>IConfiguration</em> that represents the content of the XML file.
		/// Various child interfaces of <em>IConfiguration</em> can be present
		/// in a system to provide specialized functionality.  This attribute is
		/// optional, but should be provided if the <strong>class</strong>
		/// attribute (see below) has not been provided.</li>
		/// <li><strong>class</strong> - This represents the actual class that
		/// implements the <em>IConfiguration</em> interface.  </li>
		/// <li></li>
		/// </ol>
		/// </p>
		/// <p>
		/// <strong>Note: </strong>A lot of times references to configuration
		/// objects are cached by developers in either static variables, or in
		/// collection objects for the fear that if a configuration file will be
		/// read as many times as the named configuration that it is associated
		/// with is requested from the framework.  However, the current framework
		/// reads a configuration file only the first time it is requested.  This
		/// strategy, known as <strong>lazy loading</strong> ensures that a
		/// configuration file is read only when it is required for the first time
		/// and once loaded, the file content is cached for all subsequent requests
		/// for the same configuration.
		/// <br /><br />
		/// For this reason, the value returned by the <em>GetConfiguration</em>
		/// methods should never be stored in static variables or collection objects.
		/// If a configuration reference is cached by the code that uses configuration
		/// data, there is no guarantee that the reference will continue to work if
		/// the underlying configuration file is changed, or deleted.
		/// </p>
		/// </summary>
		/// <param name="configName">Name of the configuration object.</param>
		/// <returns>
		/// An object implementing the <em>IConfiguration</em> interface
		/// if the configuration is found, an exception otherwise.
		/// </returns>
		public IConfiguration GetConfiguration(String configName)
		{
			if(configName == null)
			{
				throw new System.ArgumentNullException("configName",
														"The parameter [configName] cannot be null.  Unable to get the configuration.");
			}

			if(!configName.StartsWith(ConfigurationConstants.Delimiters.PATH))
			{
				// Configuration name does not start with the path delimiter.
				// Even if an attempt is made to load the Configuration, an
				// incorrect Configuration may be loaded, hence abort.
				throw new System.ArgumentException("The parameter [configName] must begin with the path delimiter '/'.  Unable to get the configuration.",
													"configName");
			}

			return _Cache.GetConfiguration(configName);
		}

		/// <summary>
		/// Returns a culture-specific read-only configuration object, provided
		/// its name.
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
		public IConfiguration GetConfiguration(String configName,
												CultureInfo culture)
		{
			if(configName == null)
			{
				throw new System.ArgumentNullException("configName",
														"The parameter [configName] cannot be null.  Unable to get the configuration.");
			}

			if(culture == null)
			{
				// No culture has been specified, return the default configuration
				// with the given name.
				return GetConfiguration(configName);
			}

			if(!configName.StartsWith(ConfigurationConstants.Delimiters.PATH))
			{
				// Configuration name does not start with the path delimiter.
				// Even if an attempt is made to load the Configuration, an
				// incorrect Configuration may be loaded, hence abort.
				throw new System.ArgumentException("The parameter [configName] must begin with the path delimiter '/'.  Unable to get the configuration.",
													"configName");
			}

			return _Cache.GetConfiguration(configName, culture);
		}

		/// <summary>
		/// Removes a configuration from the cache.
		/// </summary>
		/// <param name="configName">Name of the configuration to remove.</param>
		public void RemoveConfiguration(String configName)
		{
			_Cache.RemoveConfiguration(configName);
		}

		#endregion

		#region Private inner classes

		/// <summary>
		/// Configures a watchdog listener for a configuration file.  This is done
		/// in order to ensure that when a configuration file is modified on the
		/// file system, the XML Configuration Framework invalidates the cached
		/// reference to the configuration object associated with the underlying file.
		/// </summary>
		private sealed class ConfigurationWatcher
		{
			#region Private instance variables

			// Name of the configuration file to watch.
			private String   _ConfigName;

			// Timer for introducing a small delay between the time that a file
			// change is detected and the configuration cache for the file is
			// reset.
			private Timer    _Timer;

			#endregion

			#region Private instance constructors

			/// <summary>
			/// Default class constructor.
			/// </summary>
			/// <param name="configName"></param>
			private ConfigurationWatcher(String configName)
			{
				_ConfigName = configName;

				FileInfo configFileInfo = new FileInfo(ConfigurationManager.GetInstance().RootPath + configName + "." + ConfigurationManager.GetInstance().FileExtension);

				// Create a new FileSystemWatcher on the configuration file.
				FileSystemWatcher configFileWatcher = new FileSystemWatcher();
				configFileWatcher.Path   = configFileInfo.DirectoryName;
				configFileWatcher.Filter = configFileInfo.Name;

				configFileWatcher.NotifyFilter = NotifyFilters.CreationTime | NotifyFilters.FileName | NotifyFilters.LastWrite;

				// Associate watcher events with appropriate handlers.
				configFileWatcher.Changed += new FileSystemEventHandler(OnChanged);
				configFileWatcher.Created += new FileSystemEventHandler(OnChanged);
				configFileWatcher.Changed += new FileSystemEventHandler(OnChanged);
				configFileWatcher.Renamed += new RenamedEventHandler(OnRenamed);

				configFileWatcher.EnableRaisingEvents = true;

				// Create a timer that can introduce a delay between the time that a
				// change to the file is detected and the time that the change is
				// handled by the framework.  This is necessary to ensure that ample
				// time is provided for the changes to be committed to the file system
				// before the configuration cache is refreshed.
				_Timer = new Timer(new TimerCallback(FileChangeEvent), null, Timeout.Infinite, Timeout.Infinite);
			}

			#endregion

			#region Internal static methods

			/// <summary>
			/// Starts watching a file on the filesystem.
			/// </summary>
			/// <param name="configName">Name of the configuration to watch.</param>
			internal static void StartWatching(String configName)
			{
				ConfigurationWatcher watcher = new ConfigurationWatcher(configName);
			}

			#endregion

			#region Event handlers

			/// <summary>
			/// Event that gets fired when the underlying configuration file has
			/// changed.
			/// </summary>
			/// <param name="source">Event source.</param>
			/// <param name="e">Information about the event.</param>
			private void OnChanged(Object source, FileSystemEventArgs e)
			{
				// The configuration cache needs to be refreshed so that the cached
				// version of the configuration being watched is cleared and a fresh
				// version is picked up.  This cannot be done immediately as the
				// configuration file could be large and changes made to it may not
				// have been committed to the file system.  Therefore, wait for
				// 500 milliseconds before invalidating the cache for
				_Timer.Change(500, Timeout.Infinite);
			}

			/// <summary>
			/// Event that gets fired when the underlying configuration file is
			/// renamed.
			/// </summary>
			/// <param name="source">Event source.</param>
			/// <param name="e">Information about the event.</param>
			private void OnRenamed(Object source, RenamedEventArgs e)
			{
				// The configuration cache needs to be refreshed so that the cached
				// version of the configuration being watched is cleared and a fresh
				// version is picked up.  This cannot be done immediately as the
				// configuration file could be large and changes made to it may not
				// have been committed to the file system.  Therefore, wait for
				// 500 milliseconds before invalidating the cache for
				_Timer.Change(500, Timeout.Infinite);
			}

			/// <summary>
			/// Event that is fired when the internal timer times out.  It first
			/// clears the configuration from the framework cache and then loads
			/// it again so that a fresh copy is available.
			/// </summary>
			/// <param name="state"></param>
			private void FileChangeEvent(Object state)
			{
				ConfigurationManager.GetInstance().RemoveConfiguration(_ConfigName);

				ConfigurationManager.GetInstance().GetConfiguration(_ConfigName);
			}

			#endregion
		}

		#endregion
	}
}
