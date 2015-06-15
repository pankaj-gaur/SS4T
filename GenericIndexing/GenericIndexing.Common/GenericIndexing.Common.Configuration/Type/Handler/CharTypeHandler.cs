/*
 * Filename:      CharTypeHandler.cs
 * Author:        Pankaj Gaur
 * Creation date: January 1, 2006
 */

using System;

using GenericIndexing.Common.Configuration.Interface;

namespace GenericIndexing.Common.Configuration.Type.Handler
{
	/// <summary>
	/// Type handler for the <em>System.Char</em> data-type.
	/// </summary>
	public class CharTypeHandler: ITypeHandler
	{
		/// <summary>
		/// Default class constructor.
		/// </summary>
		public CharTypeHandler()
		{
		}

		#region Implementation of ITypeHandler methods

		/// <summary>
		/// Converts a string to its equivalent <em>System.Char</em> value.
		/// </summary>
		/// <param name="propertyValue">String to be converted.</param>
		/// <returns>Char equivalent of the string value.</returns>
		public Object ToObject(String propertyValue)
		{
			if(propertyValue == null)
			{
				throw new ArgumentNullException("propertyValue",
												"The parameter [propertyValue] is null.  Cannot convert to a Char.");
			}

			return Char.Parse(propertyValue);
		}

		/// <summary>
		/// Converts a <em>System.Char</em> value to its string equivalent.
		/// </summary>
		/// <param name="propertyValue">Char to be converted.</param>
		/// <returns>String equivalent of the Boolean value.</returns>
		public String ToString(Object propertyValue)
		{
			if(propertyValue == null)
			{
				throw new ArgumentNullException("propertyValue",
												"The parameter [propertyValue] is null.  Cannot convert to a Char.");
			}

			return propertyValue.ToString();
		}

		#endregion
	}
}
