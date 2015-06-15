/*
 * Filename:      ConfigurationConstants.cs
 * Author:        Pankaj Gaur
 * Creation date: December 30, 2005
 */

using System;

namespace GenericIndexing.Common.Configuration
{
	/// <summary>
	/// Contains constants for the Configuration framework.
	/// </summary>
	public sealed class ConfigurationConstants
	{
		/// <summary>
		/// Default class constructor.  Made private so that class instances
		/// cannot be created.
		/// </summary>
		private ConfigurationConstants()
		{
		}

		/// <summary>
		/// Contains names of XML attributes that can be present in configuration
		/// files.
		/// </summary>
		internal class AttributeNames
		{
			/// <summary>
			/// Default class constructor.
			/// </summary>
			private AttributeNames()
			{
			}

			/// <summary>
			/// Name of the class implementing the <em>IConfiguration</em>
			/// interface that should be used for reading a configuration file.
			/// </summary>
			public const String CLASS_NAME     = "class";

			/// <summary>
			/// Name of the specific <em>IConfiguration</em> interface to be used
			/// for reading a configuration file.
			/// 
			/// </summary>
			public const String INTERFACE_NAME = "interface";

			/// <summary>
			/// Mode of opening the configuration file, i.e., 'read-only' or
			/// 'read-write'.
			/// </summary>
			public const String OPEN_MODE      = "mode";

			/// <summary>
			/// Mode of watching the configuration file for changes.
			/// </summary>
			public const String WATCH_MODE     = "watch";
		}

		/// <summary>
		/// Contains types of Configuration objects.
		/// </summary>
		internal class ConfigurationTypes
		{
			/// <summary>
			/// Default class constructor.
			/// </summary>
			private ConfigurationTypes()
			{
			}

			/// <summary>
			/// Read-only configuration.
			/// </summary>
			public const String READ_ONLY  = "read-only";

			/// <summary>
			/// Read-write configuration.
			/// </summary>
			public const String READ_WRITE = "read-write";
		}

		/// <summary>
		/// Stores delimiters required by the Configuration framework classes.
		/// </summary>
		internal class Delimiters
		{
			/// <summary>
			/// Default class constructor.
			/// </summary>
			private Delimiters()
			{
			}

			/// <summary>
			/// Comma delimiters for string parsing.
			/// </summary>
			public const String COMMAS      = ",;";

			/// <summary>
			/// Delimiter for Configuration file paths.
			/// </summary>
			public const String PATH        = "/";

			/// <summary>
			/// Delimiter for Configuration properties.
			/// </summary>
			public const String PROPERTY    = ".";

			/// <summary>
			/// Whitespace delimiters for string parsing.
			/// </summary>
			public const String WHITESPACES = " \t\n";
		}

		/// <summary>
		/// Contains extensions of various files utilized by the framework.
		/// </summary>
		internal class FileExtensions
		{
			/// <summary>
			/// Default class constructor.
			/// </summary>
			private FileExtensions()
			{
			}

			/// <summary>
			/// Default extension of XML Configuration files.  This can be
			/// overridden with any other value by the applications.
			/// </summary>
			public const String DEFAULT = "config";
		}

		/// <summary>
		/// Contains names of various properties used by the framework.
		/// </summary>
		internal class PropertyNames
		{
			/// <summary>
			/// Default class constructor.
			/// </summary>
			private PropertyNames()
			{
			}

			/// <summary>
			/// Name of the XML node which will store the file extension to use.
			/// </summary>
			public const String FILE_EXTENSION = "fileExtension";

			/// <summary>
			/// Name of the XML node which will store the root path of all
			/// configuration files.
			/// </summary>
			public const String ROOT_PATH      = "rootPath";
		}

		/// <summary>
		/// Contains names of configuration sections in various XML files.
		/// </summary>
		internal class SectionNames
		{
			/// <summary>
			/// Default class constructor.
			/// </summary>
			private SectionNames()
			{
			}

			/// <summary>
			/// Name of the configuration section to look for in Application
			/// Configuration files.
			/// </summary>
			public const String ROOT_CONFIGURATION = "GenericIndexingConfiguration";

			/// <summary>
			/// Root element of all Configuration documents.
			/// </summary>
            public const String CONFIGURATION = "Configuration";
		}

		/// <summary>
		/// Contains modes for watching configuration files.
		/// </summary>
		internal class WatchModes
		{
			/// <summary>
			/// Default class constructor.
			/// </summary>
			private WatchModes()
			{
			}

			/// <summary>
			/// Denotes that the configuration should not be watched for changes.
			/// </summary>
			public const String NONE = "None";

			/// <summary>
			/// Denotes that the configuration should be watched for all changes,
			/// such as, file updates, file name change, file deletion, etc.
			/// </summary>
			public const String FULL = "true";
		}
	}
}
