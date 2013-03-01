using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace Parser.Base
{
	/// <summary>
	/// This exception is thrown when an error in the Parser occurs.
	/// </summary>
	/// <remarks>
	/// This is the base exception for all exceptions thrown in the Parser
	/// </remarks>
	[Serializable]
	public class PegException : ApplicationException
	{
		/// <summary>
		/// Initializes a new instance of the PegException class.
		/// </summary>
		public PegException()
			: base("SqlDataMapper caused an exception.")
		{
		}

		/// <summary>
		/// Initializes a new instance og the PegException class.
		/// </summary>
		public PegException(Exception ex)
			: base("SqlDataMapper caused an exception.", ex)
		{
		}

		/// <summary>
		/// Initializes a new instance og the PegException class.
		/// </summary>
		public PegException(string message)
			: base(message)
		{
		}

		/// <summary>
		/// Initializes a new instance og the PegException class.
		/// </summary>
		public PegException(string message, Exception innerException)
			: base(message, innerException)
		{
		}

		/// <summary>
		/// Initializes a new instance og the PegException class.
		/// </summary>
		protected PegException(SerializationInfo info, StreamingContext context)
			: base(info, context)
		{
		}
	}
}
