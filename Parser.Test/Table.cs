using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parser.Base;

namespace Parser.Test
{
	enum ConfigTable : int
	{
		Table = 1,
		Key = 2,
		Flat = 3,
		Object = 4,
		Array = 5,
		Pair = 6,
		String = 7,
		Number = 8,
		Decimal = 9,
		Bool = 10
	}
	
	public class Table: PegCharParser
	{
		public Table(string source)
			:base(source)
		{
		
		}
		
		public bool Parse()
		{
			return TreeNT((int)ConfigTable.Table, () =>
				RuleTable()
			);
		}
		
		private bool RuleTable()
		{
			return Seq(() =>
				   S()
				&& RuleKey()
				&& S()
				&& Char('=')
				&& S()
				&& RuleObject()
			);
		}
		
		private bool RuleObject()
		{
			return TreeNT((int)ConfigTable.Object, () =>
				Seq(() =>
					   S()
					&& Char('{')
					&& S()
					&& RuleElement()
					&& Star(() =>
						Seq(() =>
							   S()
							&& Char(',')
							&& S()
							&& RuleElement()
						)
					)
					&& S()
					&& Char('}')
				)
			);
		}
		
		private bool RuleArray()
		{
			return TreeNT((int)ConfigTable.Array, () =>
				Seq(() =>
					   S()
					&& Char('[')
					&& S()
					&& Star(() =>
						RuleFlat()
						&& Star(() =>
							Seq(() =>
								   S()
								&& Char(',')
								&& S()
								&& RuleFlat()
							)
						)
					)
					&& S()
					&& Char(']')
				)
			);
		}
		
		private bool RuleKey()
		{
			return TreeNT((int)ConfigTable.Key, () =>
				Seq(() =>
					   S()
					&& ( Letters() || Char('_') )
					&& Star(() =>
						Letters() || Char('_') || Dec()
					)
				)
			);
		}
		
		private bool RuleElement()
		{
			return TreeNT((int)ConfigTable.Pair, () =>
				Seq(() =>
					   S()
					&& RuleKey()
					&& S()
					&& Char('=')
					&& S()
					&& Star(() =>
						RuleFlat() || RuleObject() || RuleArray()
					)
				)
			);
		}
		
		private bool RuleFlat()
		{
			return TreeNT((int)ConfigTable.Flat, () =>
				Seq(() =>
					S()
					&& Seq(() =>
						   RuleString()
						|| RuleNumber()
						|| RuleDecimal()
						|| RuleBool()
					)
				)
			);
		}
		
		private bool RuleString()
		{
			return Seq(() =>
				   S()
				&& Char('"')
				&& RuleStringContent()
				&& Char('"')
			);
		}
		
		private bool RuleStringContent()
		{
			return TreeNT((int)ConfigTable.String, () =>
				Until('"')
			);
		}
		
		private bool RuleNumber()
		{
			return TreeNT((int)ConfigTable.Number, () =>
				Seq(() =>
					   S()
					&& For(0, 1, () =>
						OneOf("+-")
					)
					&& Star(() =>
						Dec()
					)
					&& Char('.')
					&& Plus(() =>
						Dec()
					)
				)
			);
		}
		
		private bool RuleDecimal()
		{
			return TreeNT((int)ConfigTable.Decimal, () =>
				Seq(() =>
					   S()
					&& For(0, 1, () =>
						OneOf("+-")
					)
					&& Plus(() =>
						Dec()
					)
				)
			);
		}
		
		private bool RuleBool()
		{
			return TreeNT((int)ConfigTable.Bool, () =>
				Seq(() =>
					S()
					&& 
					(
						Seq(() =>
							   Char('t')
							&& Char('r')
							&& Char('u')
							&& Char('e')
						)
						||
						Seq(() =>
							   Char('f')
							&& Char('a')
							&& Char('l')
							&& Char('s')
							&& Char('e')
						)
					)
				)
			);
		}
		
		private bool S()
		{
			return Star(() =>
				OneOf(" \t\n\r")
			);
		}
	}
}
