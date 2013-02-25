using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parser.PEG
{
	public class PegTree
	{
		public enum AddPolicy
		{
			AddAsChild,
			AddAsSibling
		}
		
		public PegNode Root
		{
			get; set;
		}
		
		public PegNode Cur
		{
			get; set;
		}
		
		public AddPolicy Policy
		{
			get; set;
		}
	}
}
