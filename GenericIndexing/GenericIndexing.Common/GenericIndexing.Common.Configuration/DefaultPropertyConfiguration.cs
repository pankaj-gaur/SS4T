/*
 * Filename:      DefaultPropertyConfiguration.cs
 * Author:        Pankaj Gaur
 * Creation date: December 29, 2005
 */

using System;
using System.Collections;
using System.Collections.Specialized;
using System.Xml;

using GenericIndexing.Common.Configuration.Interface;
using GenericIndexing.Common.Configuration.Type;
using GenericIndexing.Common.Configuration;

namespace GenericIndexing.Common.Configuration
{
	/// <summary>
	/// Default implementation of the <em>IPropertyConfiguration</em> interface.
	/// It provides a convenient way to access name-value properties stored in
	/// XML configuration files.
	/// </summary>
	internal class DefaultPropertyConfiguration : BaseConfiguration, IPropertyConfiguration
	{
		#region Private instance variables

		// Internal cache of property names and values.
		private Hashtable _PropertyCache;

		#endregion

		#region Public instance constructors

		/// <summary>
		/// Default class constructor.
		/// </summary>
		public DefaultPropertyConfiguration() : base()
		{
			_PropertyCache = new Hashtable();
		}

		#endregion

		#region Implementation of IConfiguration methods

		/// <summary>
		/// Iterates over all the XML nodes under the root node and pre-loads
		/// the information contained therein to an internal 
		/// </summary>
		/// <param name="rootNode"></param>
		override public void PreLoad(XmlNode rootNode)
		{
			// The root node of all Configuration documents is 'Configuration'.
			// Properties will be found as child nodes of the root node, so
			// iterate over the child nodes, adding them to the property cache.
			foreach(XmlNode xmlNode in rootNode.ChildNodes)
			{
				AddProperties(xmlNode.Name, xmlNode.ChildNodes);
			}
		}

		#endregion

		#region Implementation of IPropertyConfiguration methods

		/// <summary>
		/// Returns a <em>System.Collections.Hashtable</em> with all the
		/// property names and values for the current configuration.
		/// </summary>
		/// <returns>A <em>System.Collections.IDictionary</em> reference.</returns>
		public IDictionary GetAllProperties()
		{
			return new Hashtable(_PropertyCache);
		}

		/// <summary>
		/// Returns a <em>System.Boolean</em> value read from configuration file.
		/// </summary>
		/// <param name="propertyName">Property name.</param>
		/// <returns>
		/// A <em>System.Boolean</em> value if the property is found in the configuration, false otherwise.
		/// </returns>
		public Boolean GetBooleanProperty(String propertyName)
		{
			if(propertyName == null)
			{
				throw new ArgumentNullException("propertyName",
												"The parameter [propertyName] cannot be null.  Unable to get property value.");
			}

			String propertyValue = GetProperty(propertyName);

			if(propertyValue != null )
			{
				return (Boolean)TypeHandlerFactory.GetInstance().GetTypeHandler(typeof(Boolean)).ToObject(propertyValue);
			}

			return false;
		}

		/// <summary>
		/// Sets a <em>System.Boolean</em> property in the configuration.
		/// </summary>
		/// <param name="propertyName">Name of the property to set.</param>
		/// <param name="propertyValue">Property value.</param>
		public void SetBooleanProperty(String propertyName, Boolean propertyValue)
		{
			if(!_Writable)
			{
				throw new InvalidOperationException(String.Format("The configuration [{0}] is not writable.  Cannot set property value.", _Name));
			}

			if(propertyName == null)
			{
				throw new ArgumentNullException("propertyName",
												"The parameter [propertyName] cannot be null.  Unable to set property value.");
			}

			SetProperty(propertyName, TypeHandlerFactory.GetInstance().GetTypeHandler(typeof(Boolean)).ToString(propertyValue));
		}

		/// <summary>
		/// Returns a <em>System.Char</em> value read from configuration file.
		/// </summary>
		/// <param name="propertyName">Property name.</param>
		/// <returns>
		/// A <em>System.Char</em> value if the property is found in the configuration, null otherwise.
		/// </returns>
		public Char GetCharProperty(String propertyName)
		{
			if(propertyName == null)
			{
				throw new ArgumentNullException("propertyName",
												"The parameter [propertyName] cannot be null.  Unable to get property value.");
			}

			return (Char)TypeHandlerFactory.GetInstance().GetTypeHandler(typeof(Char)).ToObject(GetProperty(propertyName));
		}

		/// <summary>
		/// Sets a <em>System.Char</em> property in the configuration.
		/// </summary>
		/// <param name="propertyName">Name of the property to set.</param>
		/// <param name="propertyValue">Property value.</param>
		public void SetCharProperty(String propertyName, Char propertyValue)
		{
			if(!_Writable)
			{
				throw new InvalidOperationException(String.Format("The configuration [{0}] is not writable.  Cannot set property value.", _Name));
			}

			if(propertyName == null)
			{
				throw new ArgumentNullException("propertyName",
												"The parameter [propertyName] cannot be null.  Unable to set property value.");
			}

			SetProperty(propertyName, TypeHandlerFactory.GetInstance().GetTypeHandler(typeof(Char)).ToString(propertyValue));
		}

		/// <summary>
		/// Gets an array of string properties delimited by commas.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		/// <returns>
		/// An array of <em>System.String</em> objects if the property is found
		/// in the configuration, null otherwise.
		/// </returns>
		public String[] GetCommaDelimitedProperties(String propertyName)
		{
			String propertyString   = GetProperty(propertyName);
			String[] propertyValues = null;

			if(!string.IsNullOrEmpty(propertyString))
			{
				propertyValues = propertyString.Split(",".ToCharArray());
			}

			return propertyValues;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="propertyName"></param>
		/// <param name="propertyValues"></param>
		public void SetCommaDelimitedProperties(String propertyName, String[] propertyValues)
		{
			// TODO: Implement
		}

		/// <summary>
		/// Returns a <em>System.DateTime</em> value read from configuration
		/// file.
		/// </summary>
		/// <param name="propertyName">Property name.</param>
		/// <returns>
		/// A <em>System.DateTime</em> value if the property is found in the
		/// configuration, null otherwise.
		/// </returns>
		public DateTime GetDateTimeProperty(String propertyName)
		{
			if(propertyName == null)
			{
				throw new ArgumentNullException("propertyName",
												"The parameter [propertyName] cannot be null.  Unable to get property value.");
			}

			return (DateTime)TypeHandlerFactory.GetInstance().GetTypeHandler(typeof(DateTime)).ToObject(GetProperty(propertyName));
		}

		/// <summary>
		/// Sets a <em>System.DateTime</em> property in the configuration.
		/// </summary>
		/// <param name="propertyName">Name of the property to set.</param>
		/// <param name="propertyValue">Property value.</param>
		public void SetDateTimeProperty(String propertyName, DateTime propertyValue)
		{
			if(!_Writable)
			{
				throw new InvalidOperationException(String.Format("The configuration [{0}] is not writable.  Cannot set property value.", _Name));
			}

			if(propertyName == null)
			{
				throw new ArgumentNullException("propertyName",
												"The parameter [propertyName] cannot be null.  Unable to set property value.");
			}

			SetProperty(propertyName, TypeHandlerFactory.GetInstance().GetTypeHandler(typeof(DateTime)).ToString(propertyValue));
		}

		/// <summary>
		/// Returns a <em>System.Decimal</em> value read from configuration file.
		/// </summary>
		/// <param name="propertyName">Property name.</param>
		/// <returns>
		/// A <em>System.Decimal</em> value if the property is found in the configuration, null otherwise.
		/// </returns>
		public Decimal GetDecimalProperty(String propertyName)
		{
			if(propertyName == null)
			{
				throw new ArgumentNullException("propertyName",
												"The parameter [propertyName] cannot be null.  Unable to get property value.");
			}

			return (Decimal)TypeHandlerFactory.GetInstance().GetTypeHandler(typeof(Decimal)).ToObject(GetProperty(propertyName));
		}

		/// <summary>
		/// Sets a <em>System.Decimal</em> property in the configuration.
		/// </summary>
		/// <param name="propertyName">Name of the property to set.</param>
		/// <param name="propertyValue">Property value.</param>
		public void SetDecimalProperty(String propertyName, Decimal propertyValue)
		{
			if(!_Writable)
			{
				throw new InvalidOperationException(String.Format("The configuration [{0}] is not writable.  Cannot set property value.", _Name));
			}

			if(propertyName == null)
			{
				throw new ArgumentNullException("propertyName",
												"The parameter [propertyName] cannot be null.  Unable to set property value.");
			}

			SetProperty(propertyName, TypeHandlerFactory.GetInstance().GetTypeHandler(typeof(Decimal)).ToString(propertyValue));
		}

		/// <summary>
		/// Returns a <em>System.Double</em> value read from configuration file.
		/// </summary>
		/// <param name="propertyName">Property name.</param>
		/// <returns>
		/// A <em>System.Double</em> value if the property is found in the configuration, null otherwise.
		/// </returns>
		public Double GetDoubleProperty(String propertyName)
		{
			if(propertyName == null)
			{
				throw new ArgumentNullException("propertyName",
												"The parameter [propertyName] cannot be null.  Unable to get property value.");
			}

			return (Double)TypeHandlerFactory.GetInstance().GetTypeHandler(typeof(Double)).ToObject(GetProperty(propertyName));
		}

		/// <summary>
		/// Sets a <em>System.Double</em> property in the configuration.
		/// </summary>
		/// <param name="propertyName">Name of the property to set.</param>
		/// <param name="propertyValue">Property value.</param>
		public void SetDoubleProperty(String propertyName, Double propertyValue)
		{
			if(!_Writable)
			{
				throw new InvalidOperationException(String.Format("The configuration [{0}] is not writable.  Cannot set property value.", _Name));
			}

			if(propertyName == null)
			{
				throw new ArgumentNullException("propertyName",
												"The parameter [propertyName] cannot be null.  Unable to set property value.");
			}

			SetProperty(propertyName, TypeHandlerFactory.GetInstance().GetTypeHandler(typeof(Double)).ToString(propertyValue));
		}

		/// <summary>
		/// Returns a <em>float</em> value read from configuration file.
		/// </summary>
		/// <param name="propertyName">Property name.</param>
		/// <returns>
		/// A <em>float</em> value if the property is found in the configuration, null otherwise.
		/// </returns>
		public float GetFloatProperty(String propertyName)
		{
			if(propertyName == null)
			{
				throw new ArgumentNullException("propertyName",
												"The parameter [propertyName] cannot be null.  Unable to get property value.");
			}

			return (float)TypeHandlerFactory.GetInstance().GetTypeHandler(typeof(float)).ToObject(GetProperty(propertyName));
		}

		/// <summary>
		/// Sets a <em>float</em> property in the configuration.
		/// </summary>
		/// <param name="propertyName">Name of the property to set.</param>
		/// <param name="propertyValue">Property value.</param>
		public void SetFloatProperty(String propertyName, float propertyValue)
		{
			if(!_Writable)
			{
				throw new InvalidOperationException(String.Format("The configuration [{0}] is not writable.  Cannot set property value.", _Name));
			}

			if(propertyName == null)
			{
				throw new ArgumentNullException("propertyName",
												"The parameter [propertyName] cannot be null.  Unable to set property value.");
			}

			SetProperty(propertyName, TypeHandlerFactory.GetInstance().GetTypeHandler(typeof(float)).ToString(propertyValue));
		}

		/// <summary>
		/// Returns a <em>int</em> value read from configuration file.
		/// </summary>
		/// <param name="propertyName">Property name.</param>
		/// <returns>
		/// A <em>float</em> value if the property is found in the configuration, null otherwise.
		/// </returns>
		public int GetIntegerProperty(String propertyName)
		{
			if(propertyName == null)
			{
				throw new ArgumentNullException("propertyName",
												"The parameter [propertyName] cannot be null.  Unable to get property value.");
			}

			return (int)TypeHandlerFactory.GetInstance().GetTypeHandler(typeof(int)).ToObject(GetProperty(propertyName));
		}

		/// <summary>
		/// Sets a <em>int</em> property in the configuration.
		/// </summary>
		/// <param name="propertyName">Name of the property to set.</param>
		/// <param name="propertyValue">Property value.</param>
		public void SetIntegerProperty(String propertyName, int propertyValue)
		{
			if(!_Writable)
			{
				throw new InvalidOperationException(String.Format("The configuration [{0}] is not writable.  Cannot set property value.", _Name));
			}

			if(propertyName == null)
			{
				throw new ArgumentNullException("propertyName",
												"The parameter [propertyName] cannot be null.  Unable to set property value.");
			}

			SetProperty(propertyName, TypeHandlerFactory.GetInstance().GetTypeHandler(typeof(int)).ToString(propertyValue));
		}

		/// <summary>
		/// Returns a <em>long</em> value read from configuration file.
		/// </summary>
		/// <param name="propertyName">Property name.</param>
		/// <returns>
		/// A <em>long</em> value if the property is found in the configuration, null otherwise.
		/// </returns>
		public long GetLongProperty(String propertyName)
		{
			if(propertyName == null)
			{
				throw new ArgumentNullException("propertyName",
												"The parameter [propertyName] cannot be null.  Unable to get property value.");
			}

			return (long)TypeHandlerFactory.GetInstance().GetTypeHandler(typeof(long)).ToObject(GetProperty(propertyName));
		}

		/// <summary>
		/// Sets a <em>long</em> property in the configuration.
		/// </summary>
		/// <param name="propertyName">Name of the property to set.</param>
		/// <param name="propertyValue">Property value.</param>
		public void SetLongProperty(String propertyName, long propertyValue)
		{
			if(!_Writable)
			{
				throw new InvalidOperationException(String.Format("The configuration [{0}] is not writable.  Cannot set property value.", _Name));
			}

			if(propertyName == null)
			{
				throw new ArgumentNullException("propertyName",
												"The parameter [propertyName] cannot be null.  Unable to set property value.");
			}

			SetProperty(propertyName, TypeHandlerFactory.GetInstance().GetTypeHandler(typeof(long)).ToString(propertyValue));
		}

		/// <summary>
		/// Gets the value of a configuration property, provided its name.  If
		/// the property has not been configured in the current configured an
		/// exception is thrown.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		/// <returns>Value of the property.</returns>
		public String GetProperty(String propertyName)
		{
			if(propertyName == null)
			{
				throw new ArgumentNullException("propertyName",
												"The parameter [propertyName] cannot be null.  Unable to get property value.");
			}

			if(!IsSingleValueProperty(propertyName))
			{
				throw new ArgumentException(String.Format("The property [{0}] is a multi-valued property.  Check the configuration file to make sure that there is only one tag for this property.", propertyName), "propertyName");
			}

			return (String)_PropertyCache[propertyName];
		}

		/// <summary>
		/// Gets the value of a configuration property, provided its name.  If
		/// the property is not found in the current configuration, a default
		/// value as passed by the caller is returned.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		/// <param name="defaultValue">
		/// Default value to be returned if the property is not found in the configuration.
		/// </param>
		/// <returns>
		/// Value of the property if found in the configuration, the default value is returned.
		/// </returns>
		public String GetProperty(String propertyName, String defaultValue)
		{
			String propertyValue = GetProperty(propertyName);

			if(propertyValue == null)
			{
				return defaultValue;
			}

			return propertyValue;
		}

		/// <summary>
		/// Returns an array of strings for a multi-valued property.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		/// <returns>An array of <em>System.String</em> objects.</returns>
		public String[] GetPropertyArray(String propertyName)
		{
			if(IsSingleValueProperty(propertyName))
			{
				throw new ArgumentException(String.Format("The property [{0}] is a single-valued property.  This property should be accessed using the GetProperty method.", propertyName), "propertyName");
			}

			ArrayList propertyValues = (ArrayList)_PropertyCache[propertyName];
			String[] values = null;

			if(propertyValues != null)
			{
				values = (String[])propertyValues.ToArray(typeof(String));
			}

			return values;
		}

		/// <summary>
		/// Sets the value of a property.
		/// </summary>
		/// <param name="propertyName">Name of the property to be set.</param>
		/// <param name="propertyValue">Value of the property.</param>
		public void SetProperty(String propertyName, String propertyValue)
		{
			if(!_Writable)
			{
				throw new InvalidOperationException(String.Format("The configuration [{0}] is not writable.  Cannot set property value.", _Name));
			}

			if(propertyName == null)
			{
				throw new ArgumentNullException("propertyName",
												"The parameter [propertyName] cannot be null.  Unable to set property value.");
			}

			if(propertyValue == null)
			{
				throw new ArgumentNullException("propertyValue",
												"The parameter [propertyValue] cannot be null.  Unable to set property value.");
			}

			if(!IsSingleValueProperty(propertyName))
			{
				throw new ArgumentException(String.Format("The property [{0}] is a multi-value property.", propertyName), "propertyName");
			}

			_PropertyCache.Remove(propertyName);
			_PropertyCache[propertyName] = propertyValue;
		}

		/// <summary>
		/// Sets the values for a multi-valued property.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		/// <param name="propertyValues">An array of <em>System.String</em> objects containing property values.</param>
		public void SetPropertyArray(String propertyName, String[] propertyValues)
		{
			if(!_Writable)
			{
				throw new InvalidOperationException(String.Format("The configuration [{0}] is not writable.  Cannot set property values.", _Name));
			}

			if(IsSingleValueProperty(propertyName))
			{
				throw new ArgumentException(String.Format("The property [{0}] is a single-value property.  Multiple values cannot be set for it.", propertyName), "propertyName");
			}

			ArrayList values = (ArrayList)_PropertyCache[propertyName];

			if(values == null)
			{
				values = new ArrayList();
			}

			values.Clear();
			values.AddRange(propertyValues);
		}

		/// <summary>
		/// Returns a <em>short</em> value read from configuration file.
		/// </summary>
		/// <param name="propertyName">Property name.</param>
		/// <returns>
		/// A <em>short</em> value if the property is found in the configuration, null otherwise.
		/// </returns>
		public short GetShortProperty(String propertyName)
		{
			if(propertyName == null)
			{
				throw new ArgumentNullException("propertyName",
												"The parameter [propertyName] cannot be null.  Unable to get property value.");
			}

			return (short)TypeHandlerFactory.GetInstance().GetTypeHandler(typeof(short)).ToObject(GetProperty(propertyName));
		}

		/// <summary>
		/// Sets a <em>short</em> property in the configuration.
		/// </summary>
		/// <param name="propertyName">Name of the property to set.</param>
		/// <param name="propertyValue">Property value.</param>
		public void SetShortProperty(String propertyName, short propertyValue)
		{
			if(!_Writable)
			{
				throw new InvalidOperationException(String.Format("The configuration [{0}] is not writable.  Cannot set property value.", _Name));
			}

			if(propertyName == null)
			{
				throw new ArgumentNullException("propertyName",
												"The parameter [propertyName] cannot be null.  Unable to set property value.");
			}

			SetProperty(propertyName, TypeHandlerFactory.GetInstance().GetTypeHandler(typeof(short)).ToString(propertyValue));
		}

		/// <summary>
		/// Gets a <em>String</em> property from the configuration file.
		/// </summary>
		/// <param name="propertyName">Property name.</param>
		/// <returns>
		/// A <em>String</em> value if the property is found in the configuration, an exception otherwise.
		/// </returns>
		public String GetString(String propertyName)
		{
			return GetProperty(propertyName);
		}

		#endregion

		#region Private instance methods

		/// <summary>
		/// Adds all nodes with a non-blank value under a given sub-node of the
		/// XML document to the configuration properties.
		/// </summary>
		/// <param name="parentNodeName">Name of the parent node.</param>
		/// <param name="xmlNodes">List of nodes to be added to configuration properties.</param>
		private void AddProperties(String parentNodeName, XmlNodeList xmlNodes)
		{
			if(xmlNodes != null && xmlNodes.Count > 0)
			{
				foreach(XmlNode node in xmlNodes)
				{
					// Values provided in enclosing XML tags are considered by the Microsoft XML parser to be
					// not of the tag but a child tag called '#text'.  For this reason, the content of a child
					// node named '#text' should be considered a part of the parent node.
					if(node.NodeType==XmlNodeType.Text && node.Value!=null)
					{
						Object nodeValue = _PropertyCache[parentNodeName];

						if(nodeValue != null)
						{
							// A value already exists for the property name passed.  This must therefore be a
							// multi-valued property.  The property value must therefore be checked to make
							// sure that it can accept multiple values.
							ArrayList propertyValues = null;

							if(IsSingleValueProperty(parentNodeName))
							{
								// The current value of the property is of the type String.  It must be
								// converted to a collection so that it can store multiple values.
								propertyValues = new ArrayList();
								propertyValues.Add(nodeValue);

								_PropertyCache[parentNodeName] = propertyValues;
							}
							else
							{
								// The current value of the property can already store multiple values.
								propertyValues = (ArrayList)nodeValue;
							}

							propertyValues.Add(node.Value);
						}
						else
						{
							_PropertyCache[parentNodeName] = node.Value;
						}
					}

					if(node.HasChildNodes)
					{
						AddProperties(String.Format("{0}.{1}", parentNodeName, node.Name), node.ChildNodes);
					}
				}
			}
		}

		/// <summary>
		/// Determines if a given property is single-valued (or multi-valued), based on the data type
		/// of the value associated with that property in the property cache.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		private bool IsSingleValueProperty(String propertyName)
		{
			// Get property value.
			Object propertyValue = _PropertyCache[propertyName];

			// Check if the property value is of the type String.
			return propertyValue==null || propertyValue.GetType().Equals(typeof(String));
		}

		#endregion
	}
}
