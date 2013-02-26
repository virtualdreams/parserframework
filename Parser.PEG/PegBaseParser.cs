using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parser.PEG
{
	public enum CreatorPhase
	{
		Create,
		CreationComplete,
		CreateAndComplete
	}
	
	public abstract class PegBaseParser
	{
		public delegate bool Matcher();
		protected int _pos;
		protected int _len;
		private PegTree _tree = new PegTree();
		
		public PegTree Tree
		{
			get
			{
				return _tree;
			}
		}
		
		public bool Fatal(string message)
		{
			return false;
		}
		
		public bool Warning(string message)
		{
			return false;
		}
		
		public bool TreeNT(int ruleId, Matcher toMatch)
		{
			PegNode prevCur = Tree.Cur;
			PegTree.AddPolicy prevPolicy = Tree.Policy;
			PegNode ruleNode = null;
			
			int posBeg = _pos;
			
			AddTreeNode(ruleId, PegTree.AddPolicy.AddAsChild, CreatorPhase.Create);
			ruleNode = Tree.Cur;
			
			bool bMatches = toMatch();
			
			if(!bMatches)
			{
				RestoreTree(prevCur, prevPolicy);	
			}
			else
			{
				ruleNode.Match.Beg = posBeg;
				ruleNode.Match.End = _pos;
				Tree.Cur = ruleNode;
				Tree.Policy = PegTree.AddPolicy.AddAsSibling;
				CreateNode(CreatorPhase.CreationComplete, ruleNode, ruleId);
			}
			
			return bMatches;
		}
		
		private void RestoreTree(PegNode prevCur, PegTree.AddPolicy prevPolicy)
		{
			if(prevCur == null)
				Tree.Root = null;
			else if(prevPolicy == PegTree.AddPolicy.AddAsChild)
				prevCur.Child = null;
			else
				prevCur.Next = null;
				
			Tree.Cur = prevCur;
			Tree.Policy = prevPolicy;
		}
		
		private void ResetTree()
		{
			Tree.Root = null;
			Tree.Cur = null;
			Tree.Policy = PegTree.AddPolicy.AddAsChild;
		}

		private PegNode CreateNode(CreatorPhase phase, PegNode node, int id)
		{
			if(phase == CreatorPhase.Create || phase == CreatorPhase.CreateAndComplete)
			{
				return new PegNode(node, id);
			}
			else
			{ 
				return null;
			}
		}
		
		private void AddTreeNode(int id, PegTree.AddPolicy newAddPolicy, CreatorPhase phase)
		{
			if(Tree.Root == null)
			{
				Tree.Root = Tree.Cur = CreateNode(phase, Tree.Cur, id);
			}
			else if(Tree.Policy == PegTree.AddPolicy.AddAsChild)
			{
				Tree.Cur = Tree.Cur.Child = CreateNode(phase, Tree.Cur, id);
			}
			else
			{
				Tree.Cur = Tree.Cur.Next = CreateNode(phase, Tree.Cur.Parent, id);
			}
			Tree.Policy = newAddPolicy;
		}
			
		#region Rules
		/// <summary>
		/// Match a sequence of rules, i.e. && or || or ()
		/// </summary>
		public bool Seq(Matcher rule)
		{
			PegNode prevCur = Tree.Cur;
			PegTree.AddPolicy prevPolicy = Tree.Policy;
			int pos = _pos;
			
			bool bMatches = rule();
			
			if(!bMatches)
			{
				_pos = pos;
				RestoreTree(prevCur, prevPolicy);
			}
			
			return bMatches;
		}
		
		/// <summary>
		/// Match a rule and reset cursor
		/// </summary>
		public bool Peek(Matcher rule)
		{
			int pos = _pos;
			bool bMatches = rule();
			_pos = pos;
			return bMatches;
		}
		
		public bool Not(Matcher rule)
		{
			int pos = _pos;
			bool bMatches = rule();
			_pos = pos;
			return !bMatches;
		}
		
		/// <summary>
		/// Match a rule minimum one or more
		/// </summary>
		public bool Plus(Matcher rule)
		{
			int i = 0;
			for(i = 0;; ++i)
			{
				int pos = _pos;
				if (!rule())
				{
					_pos = pos;
					break;
				}
			}
			return i > 0;
		}
		
		/// <summary>
		/// Match a rule until it doesn't match
		/// </summary>
		public bool Star(Matcher rule)
		{
			for(;;)
			{
				int pos = _pos;
				if (!rule())
				{
					_pos = pos;
					return true;
				}
			}
		}

		public bool Option(Matcher rule)
		{
			int pos = _pos;
			if (!rule())
			{
				_pos = pos;
			}
			return true;
		}
			
		/// <summary>
		/// match a rule n times
		/// </summary>
		public bool For(int count, Matcher rule)
		{
			PegNode prevCur = Tree.Cur;
			PegTree.AddPolicy prevPolicy = Tree.Policy;
			int pos = _pos;
			
			int i = 0;
			for(i = 0; i < count; ++i)
			{
				if (!rule())
				{
					_pos = pos;
					RestoreTree(prevCur, prevPolicy);
					return false;
				}
			}
			return true;
		}
		
		public bool For(int min, int max, Matcher rule)
		{
			PegNode prevCur = Tree.Cur;
			PegTree.AddPolicy prevPolicy = Tree.Policy;
			int pos = _pos;
			
			int i = 0;
			for(i = 0; i < max; ++i)
			{
				if(!rule())
					break;
			}
			if(i < min)
			{
				_pos = pos;
				RestoreTree(prevCur, prevPolicy);
				return false;
			}
			return true;
		}
		
		/// <summary>
		/// Match any. Moves the cursor one forward
		/// </summary>
		/// <returns></returns>
		public bool Any()
		{
			if(_pos < _len)
			{
				++_pos;
				return true;
			}
			return false;
		}
		
		#endregion
	}
}
