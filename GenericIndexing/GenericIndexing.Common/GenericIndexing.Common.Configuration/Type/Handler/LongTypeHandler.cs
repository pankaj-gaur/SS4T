/*
 * Filename:      LongTypeHandler.cs
 * Author:        Pankaj Gaur
 * Creation date: January 1, 2006
 */

using System;

using GenericIndexing.Common.Configuration.Interface;

namespace GenericIndexing.Common.Configuration.Type.Handler
{
	/// <summary>
	/// Type handler for the <em>long</em> data-type.
	/// </summary>
	public class LongTypeHandler: ITypeHandler
	{
		/// <summary>
		/// Default class constructor.
		/// </summary>
		public LongTypeHandler()
		{
		}

		#region Implementation of ITypeHandler methods

		/// <summary>
		/// Converts a string to its equivalent <em>long</em> value.
		/// </summary>
		/// <param name="propertyValue">String to be converted.</param>
		/// <returns>Long equivalent of the string value.</returns>
		public Object ToObject(String propertyValue)
		{
			if(propertyValue == null)
			{
				throw new ArgumentNullException("propertyValue",
												"The parameter [propertyValue] is null.  Cannot convert to an Int64.");
			}

			return long.Parse(propertyValue);
		}

		/// <summary>
		/// Converts a <em>long</em> value to its string equivalent.
		/// </summary>
		/// <param name="propertyValue">Long to be converted.</param>
		/// <returns>String equivalent of the Boolean value.</returns>
		public String ToString(Object propertyValue)
		{
			if(propertyValue == null)
			{
				throw new ArgumentNullException("propertyValue",
												"The parameter [propertyValue] is null.  Cannot convert to an Int64.");
			}

			return propertyValue.ToString();
		}

		#endregion
	}
}
