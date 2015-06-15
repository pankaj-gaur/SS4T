/*
 * Filename:      IPropertyConfiguration.cs
 * Author:        Pankaj Gaur
 * Creation date: December 30, 2005
 */

using System;
using System.Collections;

namespace GenericIndexing.Common.Configuration.Interface
{
	/// <summary>
	/// <p>
	/// Defines operations for reading properties from configuration files.  It supports single-valued
	/// properties, such as, integers, booleans, strings, etc. and also multi-valued string properties.
	/// Single-valued properties are configured as a single uniquely-named tag in the XML configuration
	/// file.  Multi-valued properties are configured in the XML configuration file using repeating tags
	/// with the same name.
	/// </p>
	/// <p>
	/// An example of a single-valued property is shown below:
	/// <br /><br />
	/// <strong>
	/// &lt;Configuration&gt;<br />
	///	&lt;PropertyName&gt;PropertyValue&lt;/PropertyName&gt;<br />
	/// &lt;/Configuration&gt;
	/// </strong>
	/// <br /><br />
	/// In this case, the single-valued property is called <em>PropertyName</em> and its value is
	/// <em>PropertyValue</em>.
	/// </p>
	/// <p>
	/// An example of a multi-valued property is shown below:
	/// <br /><br />
	/// <strong>
	/// &lt;Configuration&gt;<br />
	///	&lt;PropertyName&gt;PropertyValue1&lt;/PropertyName&gt;<br />
	///	&lt;PropertyName&gt;PropertyValue2&lt;/PropertyName&gt;<br />
	///	&lt;PropertyName&gt;PropertyValue3&lt;/PropertyName&gt;<br />
	///	&lt;PropertyName&gt;PropertyValue4&lt;/PropertyName&gt;<br />
	/// &lt;/Configuration&gt;
	/// </strong>
	/// <br /><br />
	/// As in the previous example, the property is named <em>PropertyName</em>.  However, this property has
	/// multiple values, configured using repeating tags for the property name.
	/// </p>
	/// <p>
	/// Multi-valued properties should not be confused with single-valued properties which have multiple
	/// tokens separated using delimiters.  For example, consider the following configuration file:
	/// <br /><br />
	/// <strong>
	/// &lt;Configuration&gt;<br />
	///	&lt;PropertyName&gt;PropertyValue1,PropertyValue2,PropertyValue3&lt;/PropertyName&gt;<br />
	/// &lt;/Configuration&gt;
	/// </strong>
	/// <br /><br />
	/// In this example, <em>PropertyName</em> has a value which has multiple tokens separated using
	/// commas.  However, this does not make this property a multi-valued property.  This is because
	/// the delimiters have no special significance for the Configuration Framework.
	/// </p>
	/// </summary>
	public interface IPropertyConfiguration : IConfiguration
	{
		/// <summary>
		/// Returns a <em>System.Collections.IDictionary</em> containing all
		/// the properties for the current configuration.
		/// </summary>
		/// <returns>An object implementing the
		/// <em>System.Collections.IDictionary</em> interface.</returns>
		IDictionary GetAllProperties();

		/// <summary>
		/// Returns a <em>System.Boolean</em> value read from configuration
		/// file.
		/// </summary>
		/// <param name="propertyName">Property name.</param>
		/// <returns>
		/// A <em>System.Boolean</em> value if the property is found in the
		/// configuration, an exception otherwise.
		/// </returns>
		Boolean GetBooleanProperty(String propertyName);

		/// <summary>
		/// Sets a <em>System.Boolean</em> property in the configuration.
		/// </summary>
		/// <param name="propertyName">Name of the property to set.</param>
		/// <param name="propertyValue">Property value.</param>
		void SetBooleanProperty(String propertyName, Boolean propertyValue);

		/// <summary>
		/// Returns a <em>System.Char</em> value read from configuration
		/// file.
		/// </summary>
		/// <param name="propertyName">Property name.</param>
		/// <returns>
		/// A <em>System.Char</em> value if the property is found in the
		/// configuration, an exception otherwise.
		/// </returns>
		Char GetCharProperty(String propertyName);

		/// <summary>
		/// Sets a <em>System.Char</em> property in the configuration.
		/// </summary>
		/// <param name="propertyName">Name of the property to set.</param>
		/// <param name="propertyValue">Property value.</param>
		void SetCharProperty(String propertyName, Char propertyValue);

		/// <summary>
		/// Gets an array of string properties delimited by commas.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		/// <returns>
		/// An array of <em>System.String</em> objects if the property is found
		/// in the configuration, null otherwise.
		/// </returns>
		String[] GetCommaDelimitedProperties(String propertyName);

		/// <summary>
		/// Sets an array of string properties delimited by commas.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		/// <param name="propertyValues">
		/// An array of <em>System.String</em> objects.
		/// </param>
		void SetCommaDelimitedProperties(String propertyName, String[] propertyValues);

		/// <summary>
		/// Returns a <em>System.DateTime</em> value read from configuration
		/// file.
		/// </summary>
		/// <param name="propertyName">Property name.</param>
		/// <returns>
		/// A <em>System.DateTime</em> value if the property is found in the
		/// configuration, an exception otherwise.
		/// </returns>
		DateTime GetDateTimeProperty(String propertyName);

		/// <summary>
		/// Sets a <em>System.DateTime</em> property in the configuration.
		/// </summary>
		/// <param name="propertyName">Name of the property to set.</param>
		/// <param name="propertyValue">Property value.</param>
		void SetDateTimeProperty(String propertyName, DateTime propertyValue);

		/// <summary>
		/// Returns a <em>System.Decimal</em> value read from configuration
		/// file.
		/// </summary>
		/// <param name="propertyName">Property name.</param>
		/// <returns>
		/// A <em>System.Decimal</em> value if the property is found in the
		/// configuration, an exception otherwise.
		/// </returns>
		Decimal GetDecimalProperty(String propertyName);

		/// <summary>
		/// Sets a <em>System.Decimal</em> property in the configuration.
		/// </summary>
		/// <param name="propertyName">Name of the property to set.</param>
		/// <param name="propertyValue">Property value.</param>
		void SetDecimalProperty(String propertyName, Decimal propertyValue);

		/// <summary>
		/// Returns a <em>System.Double</em> value read from configuration
		/// file.
		/// </summary>
		/// <param name="propertyName">Property name.</param>
		/// <returns>
		/// A <em>System.Double</em> value if the property is found in the
		/// configuration, an exception otherwise.
		/// </returns>
		Double GetDoubleProperty(String propertyName);

		/// <summary>
		/// Sets a <em>System.Double</em> property in the configuration.
		/// </summary>
		/// <param name="propertyName">Name of the property to set.</param>
		/// <param name="propertyValue">Property value.</param>
		void SetDoubleProperty(String propertyName, Double propertyValue);

		/// <summary>
		/// Returns a <em>float</em> value read from configuration
		/// file.
		/// </summary>
		/// <param name="propertyName">Property name.</param>
		/// <returns>
		/// A <em>float</em> value if the property is found in the
		/// configuration, an exception otherwise.
		/// </returns>
		float GetFloatProperty(String propertyName);

		/// <summary>
		/// Sets a <em>float</em> property in the configuration.
		/// </summary>
		/// <param name="propertyName">Name of the property to set.</param>
		/// <param name="propertyValue">Property value.</param>
		void SetFloatProperty(String propertyName, float propertyValue);

		/// <summary>
		/// Returns an <em>int</em> value read from configuration file.
		/// </summary>
		/// <param name="propertyName">Property name.</param>
		/// <returns>
		/// An <em>int</em> value if the property is found in the
		/// configuration, an exception otherwise.
		/// </returns>
		int GetIntegerProperty(String propertyName);

		/// <summary>
		/// Sets an <em>int</em> property in the configuration.
		/// </summary>
		/// <param name="propertyName">Name of the property to set.</param>
		/// <param name="propertyValue">Property value.</param>
		void SetIntegerProperty(String propertyName, int propertyValue);

		/// <summary>
		/// Returns a <em>long</em> value read from configuration
		/// file.
		/// </summary>
		/// <param name="propertyName">Property name.</param>
		/// <returns>
		/// A <em>long</em> value if the property is found in the
		/// configuration, an exception otherwise.
		/// </returns>
		long GetLongProperty(String propertyName);

		/// <summary>
		/// Sets a <em>long</em> property in the configuration.
		/// </summary>
		/// <param name="propertyName">Name of the property to set.</param>
		/// <param name="propertyValue">Property value.</param>
		void SetLongProperty(String propertyName, long propertyValue);

		/// <summary>
		/// Gets the value of a configuration property, provided its name.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		/// <returns>
		/// Value of the property if the property is found in the
		/// configuration, null otherwise.
		/// </returns>
		String GetProperty(String propertyName);

		/// <summary>
		/// Gets the value of a configuration property, provided its name.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		/// <param name="defaultValue">
		/// Default value to be returned if the property is not found in the
		/// configuration.
		/// </param>
		/// <returns>
		/// Value of the property if found in the configuration, otherwise
		/// the default value is returned.
		/// </returns>
		String GetProperty(String propertyName, String defaultValue);

		/// <summary>
		/// Returns an array of strings for a multi-valued property.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		/// <returns>An array of <em>System.String</em> objects.</returns>
		String[] GetPropertyArray(String propertyName);

		/// <summary>
		/// Sets the value of a property.
		/// </summary>
		/// <param name="propertyName">Name of the property to be set.</param>
		/// <param name="propertyValue">Value of the property.</param>
		void SetProperty(String propertyName, String propertyValue);

		/// <summary>
		/// Sets the values for a multi-valued property.
		/// </summary>
		/// <param name="propertyName">Name of the property.</param>
		/// <param name="propertyValues">An array of <em>System.String</em> objects containing property values.</param>
		void SetPropertyArray(String propertyName, String[] propertyValues);

		/// <summary>
		/// Returns a <em>short</em> value read from configuration
		/// file.
		/// </summary>
		/// <param name="propertyName">Property name.</param>
		/// <returns>
		/// A <em>short</em> value if the property is found in the
		/// configuration, an exception otherwise.
		/// </returns>
		short GetShortProperty(String propertyName);

		/// <summary>
		/// Sets a <em>short</em> property in the configuration.
		/// </summary>
		/// <param name="propertyName">Name of the property to set.</param>
		/// <param name="propertyValue">Property value.</param>
		void SetShortProperty(String propertyName, short propertyValue);

		/// <summary>
		/// Gets a <em>String</em> property from the configuration file.
		/// </summary>
		/// <param name="propertyName">Property name.</param>
		/// <returns>
		/// A <em>String</em> value if the property is found in the
		/// configuration, an exception otherwise.
		/// </returns>
		String GetString(String propertyName);
	}
}
