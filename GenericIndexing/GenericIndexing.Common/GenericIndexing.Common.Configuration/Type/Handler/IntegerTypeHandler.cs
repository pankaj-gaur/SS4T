/*
 * Filename:      IntegerTypeHandler.cs
 * Author:        Pankaj Gaur
 * Creation date: January 1, 2006
 */

using System;

using GenericIndexing.Common.Configuration.Interface;

namespace GenericIndexing.Common.Configuration.Type.Handler
{
	/// <summary>
	/// Type handler for the <em>int</em> data-type.
	/// </summary>
	public class IntegerTypeHandler: ITypeHandler
	{
		/// <summary>
		/// Default class constructor.
		/// </summary>
		public IntegerTypeHandler()
		{
		}

		#region Implementation of ITypeHandler methods

		/// <summary>
		/// Converts a string to its equivalent <em>int</em> value.
		/// </summary>
		/// <param name="propertyValue">String to be converted.</param>
		/// <returns>Int equivalent of the string value.</returns>
		public Object ToObject(String propertyValue)
		{
			if(propertyValue == null)
			{
				throw new ArgumentNullException("propertyValue",
												"The parameter [propertyValue] is null.  Cannot convert to an Int32.");
			}

			return int.Parse(propertyValue);
		}

		/// <summary>
		/// Converts a <em>int</em> value to its string equivalent.
		/// </summary>
		/// <param name="propertyValue">Int to be converted.</param>
		/// <returns>String equivalent of the Boolean value.</returns>
		public String ToString(Object propertyValue)
		{
			if(propertyValue == null)
			{
				throw new ArgumentNullException("propertyValue",
												"The parameter [propertyValue] is null.  Cannot convert to an Int32.");
			}

			return propertyValue.ToString();
		}

		#endregion
	}
}
