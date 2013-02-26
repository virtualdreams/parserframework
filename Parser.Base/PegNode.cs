using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parser.Base
{
	public class PegNode
	{
		//public int _id;
		//public PegNode _parent, _child, _next;
		//public PegBegEnd _match;
		
		public int Id
		{
			get; set;
		}
		
		public PegNode Parent
		{
			get; set;
		}
		
		public PegNode Child
		{
			get; set;
		}
		
		public PegNode Next
		{
			get; set;
		}
		
		public PegBegEnd Match
		{
			get; set;
		}
		
		public PegNode(PegNode parent, int id, PegBegEnd match, PegNode child, PegNode next)
		{
			Parent = parent; 
			Id = id;
			Child = child;
			Next = next;
			Match = match;
		}
		
		public PegNode(PegNode parent, int id, PegBegEnd match, PegNode child)
			:this(parent, id, match, child, null)
		{
		}
		
		public PegNode(PegNode parent, int id, PegBegEnd match)
			:this(parent, id, match, null, null)
		{
		}
		
		public PegNode(PegNode parent, int id)
			:this(parent, id, new PegBegEnd(), null, null)
		{
		}
	}
}
