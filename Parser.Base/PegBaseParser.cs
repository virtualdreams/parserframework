using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parser.Base
{
	public enum CreatorPhase
	{
		Create,
		CreationComplete,
		CreateAndComplete
	}
	
	public enum SpecialNodes
	{
		Fatal = -10001,
		AnonymNTNode = -10002,
		AnonymASTNode = -10003,
		AnonymNode = -10004
	}
	
	/// <summary>
	/// This is the basic parser class
	/// </summary>
	public abstract class PegBaseParser
	{
		public delegate bool Matcher();
		protected int _pos = 0;
		protected int _len = 0;
		protected bool _mute = false;
		private PegTree _tree = new PegTree();
		
		public PegTree Tree
		{
			get
			{
				return _tree;
			}
		}
		
		private void RestoreTree(PegNode prevCur, PegTree.AddPolicy prevPolicy)
		{
			if(_mute)
			{
				return;
			}
			
			if(prevCur == null)
			{
				Tree.Root = null;
			}
			else if(prevPolicy == PegTree.AddPolicy.AddAsChild)
			{
				prevCur.Child = null;
			}
			else
			{
				prevCur.Next = null;
			}
				
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
			if(_mute)
			{
				return;
			}
			
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
		public bool TreeNT(int ruleId, Matcher rule)
		{
			if (_mute)
			{
				return rule();
			}

			PegNode prevCur = Tree.Cur;
			PegTree.AddPolicy prevPolicy = Tree.Policy;
			PegNode ruleNode = null;

			int posBeg = _pos;

			AddTreeNode(ruleId, PegTree.AddPolicy.AddAsChild, CreatorPhase.Create);
			ruleNode = Tree.Cur;

			bool matches = rule();

			if (!matches)
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

			return matches;
		}

		public bool TreeNT(Matcher rule)
		{
			return TreeNT((int)SpecialNodes.AnonymNTNode, rule);
		}

		public bool TreeAST(int ruleId, Matcher rule)
		{
			if (_mute)
			{
				return rule();
			}

			bool matches = TreeNT(ruleId, rule);

			if (matches)
			{
				if (Tree.Cur.Child != null && Tree.Cur.Child.Next == null && Tree.Cur.Parent != null)
				{
					if (Tree.Cur.Parent.Child == Tree.Cur)
					{
						Tree.Cur.Parent.Child = Tree.Cur.Child;
						Tree.Cur.Child.Parent = Tree.Cur.Parent;
						Tree.Cur = Tree.Cur.Child;
					}
					else
					{
						PegNode prev = null;
						for (prev = Tree.Cur.Parent.Child; prev != null && prev.Next != Tree.Cur; prev = prev.Next)
						{
							//STUB
						}
						if (prev != null)
						{
							prev.Next = Tree.Cur.Child;
							Tree.Cur.Child.Parent = Tree.Cur.Parent;
							Tree.Cur = Tree.Cur.Child;
						}
					}
				}
			}
			return matches;
		}

		public bool TreeAST(Matcher rule)
		{
			return TreeAST((int)SpecialNodes.AnonymASTNode, rule);
		}
		
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
		/// &e
		/// </summary>
		public bool Peek(Matcher rule)
		{
			int pos = _pos;
			bool mute = _mute;
			_mute = true;
			
			bool bMatches = rule();
			
			_mute = mute;
			_pos = pos;
			
			return bMatches;
		}
		
		/// <summary>
		/// !e
		/// </summary>
		public bool Not(Matcher rule)
		{
			int pos = _pos;
			bool mute = _mute;
			_mute = true;
			
			bool bMatches = rule();
			
			_mute = mute;
			_pos = pos;
			return !bMatches;
		}
		
		/// <summary>
		/// e+
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
		/// e*
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
		
		/// <summary>
		/// e?
		/// </summary>
		public bool Opt(Matcher rule)
		{
			int pos = _pos;
			if (!rule())
			{
				_pos = pos;
			}
			return true;
		}
			
		/// <summary>
		/// e{x}
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
		
		/// <summary>
		/// e{min,max}
		/// </summary>
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
		/// .*
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
		
		[Obsolete("Untested rule. Use with care.")]
		public bool Until(Matcher rule)
		{
			for(;;)
			{
				int pos = _pos;
				if(_pos < _len)
				{
					if (rule())
					{
						_pos = pos;
						return true;
					}
				}
				else
				{
					return false;
				}
			}
		}
		
		#endregion
	}
}
