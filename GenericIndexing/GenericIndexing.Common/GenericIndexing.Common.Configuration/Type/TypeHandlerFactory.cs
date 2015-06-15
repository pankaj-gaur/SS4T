/*
 * Filename:      TypeHandlerFactory.cs
 * Author:        Pankaj Gaur
 * Creation date: January 1, 2006
 */

using System;
using System.Collections;

using GenericIndexing.Common.Configuration.Interface;
using GenericIndexing.Common.Configuration.Type.Handler;

namespace GenericIndexing.Common.Configuration.Type
{
	/// <summary>
	/// Factory for objects that implement the <em>ITypeHandler interface</em>.
	/// </summary>
	public sealed class TypeHandlerFactory
	{
		#region Private static variables

		// Singleton instance of the class.
		private static readonly TypeHandlerFactory INSTANCE = new TypeHandlerFactory();

		// Internal cache of type handlers.
		private static readonly Hashtable          CACHE    = new Hashtable();

		#endregion

		#region Static constructor

		/// <summary>
		/// Adds instances of type handlers to the internal cache.
		/// </summary>
		static TypeHandlerFactory()
		{
			CACHE[typeof(Boolean)]  = new BooleanTypeHandler();
			CACHE[typeof(Char)]     = new CharTypeHandler();
			CACHE[typeof(DateTime)] = new DateTimeTypeHandler();
			CACHE[typeof(Decimal)]  = new DecimalTypeHandler();
			CACHE[typeof(Double)]   = new DoubleTypeHandler();
			CACHE[typeof(float)]    = new FloatTypeHandler();
			CACHE[typeof(int)]      = new IntegerTypeHandler();
			CACHE[typeof(long)]     = new LongTypeHandler();
			CACHE[typeof(short)]    = new ShortTypeHandler();
		}

		#endregion

		#region Public instance constructors

		/// <summary>
		/// Default class constructor.
		/// </summary>
		private TypeHandlerFactory()
		{
		}

		#endregion

		#region Public static methods

		/// <summary>
		/// Returns the singleton instance of this class.
		/// </summary>
		/// <returns></returns>
		public static TypeHandlerFactory GetInstance()
		{
			return INSTANCE;
		}

		#endregion

		#region Public instance methods

		/// <summary>
		/// Looks up in the cache for a type handler appropriate for a
		/// particular data type.
		/// </summary>
		/// <param name="type">
		/// <em>System.Type</em> for which a handler is required.
		/// </param>
		/// <returns>
		/// An object implementing the <em>ITypeHandler</em> interface.
		/// </returns>
		public ITypeHandler GetTypeHandler(System.Type type)
		{
			return (ITypeHandler)CACHE[type];
		}

		#endregion
	}
}
