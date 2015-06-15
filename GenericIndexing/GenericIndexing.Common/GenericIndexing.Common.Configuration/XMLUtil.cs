/*
 * Filename:      XMLUtil.xs
 * Author:        Pankaj Gaur
 * Creation date: January 1, 2006
 */

using System;
using System.IO;
using System.Xml;

namespace GenericIndexing.Common.Configuration
{
	/// <summary>
	/// Helper class for common functionality required for working with XML
	/// documents.
	/// </summary>
	public sealed class XMLUtil
	{
		/// <summary>
		/// Default class constructor.  Made private so that class instances
		/// cannot be created.
		/// </summary>
		private XMLUtil()
		{
		}

		/// <summary>
		/// Returns a non-validating XML document for a given
		/// <em>System.IO.Stream</em>.
		/// </summary>
		/// <param name="stream">
		/// <em>System.IO.Stream</em> from which XML data needs to be read.
		/// </param>
		/// <returns>
		/// A <em>System.Xml.XmlDocument</em> if the stream contains valid XML
		/// data, an exception otherwise.</returns>
		public static XmlDocument GetXMLDocument(Stream stream)
		{
			if(stream == null)
			{
				throw new System.ArgumentNullException("stream",
														"The parameter [stream] is null.  Cannot create an XML document.");
			}

			XmlTextReader reader = new XmlTextReader(stream);
			XmlDocument document = new XmlDocument();
			document.Load(reader);

			return document;
		}

		/// <summary>
		/// Returns a non-validating XML document for a given file.
		/// </summary>
		/// <param name="filePath">
		/// Fully qualified path of the file from which data needs to be read.
		/// </param>
		/// <returns>
		/// A <em>System.Xml.XmlDocument</em> if the file contains valid XML
		/// data, an exception otherwise.</returns>
		public static XmlDocument GetXMLDocument(String filePath)
		{
			if(filePath == null)
			{
				throw new System.ArgumentNullException("filePath",
													"The parameter [filePath] is null.  Cannot create an XML document.");
			}

			Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
			XmlDocument document = null;

			try
			{
				document = GetXMLDocument(stream);
			}
			finally
			{
				// When the operations are over, close the file stream, so that the file handle can be
				// released and any associated resources can also be freed up.  Placed inside a finally
				// block so that the stream is closed even if there is an exception while attempting to
				// open the file, such as, due to the XML being malformed.
				stream.Close();
			}

			return document;
		}

		/// <summary>
		/// Returns a validating XML document for a given <em>System.IO.Stream</em>.
		/// </summary>
		/// <param name="stream">
		/// <em>System.IO.Stream</em> from which XML data needs to be read.
		/// </param>
		/// <returns>
		/// A <em>System.Xml.XmlDocument</em> if the stream contains valid XML
		/// data, an exception otherwise.</returns>
		public static XmlDocument GetValidatedXMLDocument(Stream stream)
		{
			if(stream == null)
			{
				throw new System.ArgumentNullException("stream",
														"The parameter [stream] is null.  Cannot create an XML document.");
			}

			XmlValidatingReader reader = new XmlValidatingReader(new XmlTextReader(stream));
			reader.EntityHandling = EntityHandling.ExpandEntities;
			reader.ValidationType = ValidationType.Auto;

			XmlDocument document = new XmlDocument();
			document.Load(reader);

			return document;
		}

		/// <summary>
		/// Returns a validating XML document for a given file.
		/// </summary>
		/// <param name="filePath">
		/// Fully qualified path of the file from which data needs to be read.
		/// </param>
		/// <returns>
		/// A <em>System.Xml.XmlDocument</em> if the file contains valid XML
		/// data, an exception otherwise.</returns>
		public static XmlDocument GetValidatedXMLDocument(String filePath)
		{
			if(filePath == null)
			{
				throw new System.ArgumentNullException("filePath",
													"The parameter [filePath] is null.  Cannot create an XML document.");
			}

			Stream stream = new FileStream(filePath, FileMode.Open, FileAccess.Read);
			XmlDocument document = null;

			try
			{
				// Open the configuration file as a validated XML document.
				document = GetValidatedXMLDocument(stream);
			}
			finally
			{
				// When the operations are over, close the file stream, so that the file handle can be
				// released and any associated resources can also be freed up.  Placed inside a finally
				// block so that the stream is closed even if there is an exception while attempting to
				// open the file, such as, due to the XML being malformed.
				stream.Close();
			}

			return document;
		}
	}
}
