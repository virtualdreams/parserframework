using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parser.Base
{
	/// <summary>
	/// This class hold the begin and end position of a successfull parsed node
	/// </summary>
	public class PegMatch
	{
		public int Beg
		{
			get; set;
		}
		
		public int End
		{
			get; set;
		}
		
		public int Length
		{
			get
			{
				return End - Beg;
			}
		}
		
		public string GetString(string source)
		{
			return source.Substring(Beg, Length);
		}
	}
}
