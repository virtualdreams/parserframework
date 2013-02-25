using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parser.PEG
{
	public class PegBegEnd
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
