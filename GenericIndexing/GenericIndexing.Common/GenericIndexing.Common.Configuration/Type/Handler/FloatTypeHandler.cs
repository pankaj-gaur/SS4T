/*
 * Filename:      FloatTypeHandler.cs
 * Author:        Pankaj Gaur
 * Creation date: January 1, 2006
 */

using System;

using GenericIndexing.Common.Configuration.Interface;

namespace GenericIndexing.Common.Configuration.Type.Handler
{
	/// <summary>
	/// Type handler for the <em>float</em> data-type.
	/// </summary>
	public class FloatTypeHandler: ITypeHandler
	{
		/// <summary>
		/// Default class constructor.
		/// </summary>
		public FloatTypeHandler()
		{
		}

		#region Implementation of ITypeHandler methods

		/// <summary>
		/// Converts a string to its equivalent <em>float</em> value.
		/// </summary>
		/// <param name="propertyValue">String to be converted.</param>
		/// <returns>Float equivalent of the string value.</returns>
		public Object ToObject(String propertyValue)
		{
			if(propertyValue == null)
			{
				throw new ArgumentNullException("propertyValue",
												"The parameter [propertyValue] is null.  Cannot convert to a Float.");
			}

			return float.Parse(propertyValue);
		}

		/// <summary>
		/// Converts a <em>float</em> value to its string equivalent.
		/// </summary>
		/// <param name="propertyValue">Float to be converted.</param>
		/// <returns>String equivalent of the Boolean value.</returns>
		public String ToString(Object propertyValue)
		{
			if(propertyValue == null)
			{
				throw new ArgumentNullException("propertyValue",
												"The parameter [propertyValue] is null.  Cannot convert to a Float.");
			}

			return propertyValue.ToString();
		}

		#endregion
	}
}
