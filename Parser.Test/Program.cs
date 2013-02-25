using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Parser.PEG;

namespace Parser.Test
{
	class Program
	{
		static void Main(string[] args)
		{
			string input = "conf = { key = \"value\"}";
			Test t = new Test(input);
			
			Console.WriteLine("Cursor: {0}", t.Pos);
			Console.WriteLine("Parse result: {0}", t.Parse());
			
			Console.ReadKey();
		}
	}
	
	enum LuaTable
	{
		Ident = 1,
		Key = 2,
		String = 3,
		Number = 4,
		Bool = 5,
		Array = 6
	}
	
	class Test: PegCharParser
	{
		public Test(string source)
			:base(source)
		{
		
		}
		
		public bool Parse()
		{
			return TreeNT(0, () => 
				Ident()
			);
		}
		
		public bool S()
		{
			return Star(() => OneOf(" \t\r\n"));
		}
		
		public bool Ident()
		{
			return Seq(() =>
				   S() 
				&& Plus(() =>
						CharRange('a', 'z') || CharRange('A', 'Z')
					) 
				&& S()
				&& Char('=')
				&& Object()
			);
		}
		
		public bool Object()
		{
			return Seq(() =>
				   S()
				&& Char('{')
				&& S() 
				&& (
						Peek(() =>
							Char('}') || Pair()
						)
					)
				&& S()
			);
		}
		
		public bool Pair()
		{
			return Seq(() =>
				   S()
				&& Plus(() =>
						CharRange('a', 'z') || CharRange('A', 'Z')
				   )
				&& S()
				&& Char('=')
				&& S()
				&& Char('"')
				&& Star(() => Char('"'))
				&& S()
			);
		}
		
		public bool Key()
		{
			
			
			return false;
		}
		
		public bool Value()
		{
			return false;
		}
		
		public bool Array()
		{
			return false;
		}
	}
}
