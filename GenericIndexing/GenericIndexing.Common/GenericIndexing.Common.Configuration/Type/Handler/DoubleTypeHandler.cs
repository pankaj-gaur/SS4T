/*
 * Filename:      DoubleTypeHandler.cs
 * Author:        Pankaj Gaur
 * Creation date: January 1, 2006
 */

using System;

using GenericIndexing.Common.Configuration.Interface;

namespace GenericIndexing.Common.Configuration.Type.Handler
{
	/// <summary>
	/// Type handler for the <em>System.Double</em> data-type.
	/// </summary>
	public class DoubleTypeHandler: ITypeHandler
	{
		/// <summary>
		/// Default class constructor.
		/// </summary>
		public DoubleTypeHandler()
		{
		}

		#region Implementation of ITypeHandler methods

		/// <summary>
		/// Converts a string to its equivalent <em>System.Double</em> value.
		/// </summary>
		/// <param name="propertyValue">String to be converted.</param>
		/// <returns>Double equivalent of the string value.</returns>
		public Object ToObject(String propertyValue)
		{
			if(propertyValue == null)
			{
				throw new ArgumentNullException("propertyValue",
												"The parameter [propertyValue] is null.  Cannot convert to a Double.");
			}

			return Double.Parse(propertyValue);
		}

		/// <summary>
		/// Converts a <em>System.Double</em> value to its string equivalent.
		/// </summary>
		/// <param name="propertyValue">Double to be converted.</param>
		/// <returns>String equivalent of the Boolean value.</returns>
		public String ToString(Object propertyValue)
		{
			if(propertyValue == null)
			{
				throw new ArgumentNullException("propertyValue",
												"The parameter [propertyValue] is null.  Cannot convert to a Double.");
			}

			return propertyValue.ToString();
		}

		#endregion
	}
}
