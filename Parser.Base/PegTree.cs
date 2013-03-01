using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parser.Base
{
	/// <summary>
	/// This class hold the information of the parse tree
	/// </summary>
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
