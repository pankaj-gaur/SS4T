/*
 * Filename:      IDataTypeHandler.cs
 * Author:        Pankaj Gaur
 * Creation date: December 29, 2005
 */

using System;

namespace GenericIndexing.Common.Configuration.Interface
{
	/// <summary>
	/// Defines the interface that all data-type handlers must implement
	/// for reading configuration properties.
	/// </summary>
	public interface ITypeHandler
	{
		/// <summary>
		/// Converts a string to its equivalent value in a particular
		/// data-type.
		/// </summary>
		/// <param name="propertyValue">String value to be converted.</param>
		/// <returns>
		/// A value of the data-type that the type-handler is responsible
		/// for.
		/// </returns>
		Object ToObject(String propertyValue);

		/// <summary>
		/// Converts an object of a particular type to it string value.
		/// </summary>
		/// <param name="propertyValue">Value to be converted.</param>
		/// <returns>String representation of the object.</returns>
		String ToString(Object propertyValue);
	}
}
