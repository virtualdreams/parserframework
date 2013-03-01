using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parser.Base
{
	/// <summary>
	/// This class hold the informations of a node in the tree
	/// </summary>
	public class PegNode
	{
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
		
		public PegMatch Match
		{
			get; set;
		}
		
		public PegNode(PegNode parent, int id, PegMatch match, PegNode child, PegNode next)
		{
			Parent = parent; 
			Id = id;
			Child = child;
			Next = next;
			Match = match;
		}
		
		public PegNode(PegNode parent, int id, PegMatch match, PegNode child)
			:this(parent, id, match, child, null)
		{
		}
		
		public PegNode(PegNode parent, int id, PegMatch match)
			:this(parent, id, match, null, null)
		{
		}
		
		public PegNode(PegNode parent, int id)
			:this(parent, id, new PegMatch(), null, null)
		{
		}
	}
}
