using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Parser.PEG;

namespace Parser.Test
{
	enum ConfigTable : int
	{
		Table = 1,
		Object = 2,
		Pair = 3,
		Key = 4,
		Value = 5,
		String = 6,
		Number = 7,
		Decimal = 8,
		Bool = 9
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
					&& S()
					&& Star(() =>
						Seq(() =>
							Char(',')
							&& S()
							&& RuleElement()
						)
					)
					&& S()
					&& Char('}')
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
						RuleValue() || RuleObject()
					)
				)
			);
		}
		
		private bool RuleValue()
		{
			return TreeNT((int)ConfigTable.Value, () =>
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
				Plus(() =>
					Dec()
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
