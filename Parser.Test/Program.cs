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
			string input = "key = \"value1\"\nkey = \"value2\"\n";
			Test t = new Test(input);
			
			Console.WriteLine("Cursor: {0}", t.Pos);
			Console.WriteLine("Parse result: {0}", t.Parse());
			
			Console.ReadKey();
		}
	}
	
	enum KeyValue: int
	{
		KeyValue = 1,
		Pair = 2,
		Key = 3,
		Value = 4
	}
	
	class Test: PegCharParser
	{
		public Test(string source)
			:base(source)
		{
		
		}
		
		public bool Parse()
		{
			return TreeNT((int)KeyValue.KeyValue, () => 
				Star(() =>
					Pair()
				)
			);
		}
		
		public bool Pair()
		{
			bool r = TreeNT((int)KeyValue.Pair, () =>
				Seq(() =>
					   WS()
					&& Key()
					&& WS()
					&& Char('=')
					&& WS()
					&& Value()
					&& S()
				)
			);
			return r;
		}
		
		public bool Key()
		{
			bool r = TreeNT((int)KeyValue.Key, () =>
				Plus(() =>
					Letters()
				)
			);
			return r;
		}
		
		public bool Value()
		{
			bool r = Seq(() =>
				Char('"') && StringContent() && Char('"')
			);
			return r;
		}
		
		public bool StringContent()
		{
			bool r = TreeNT((int)KeyValue.Value, () =>
				//Star(() => 
				//    Letters() || Digits()
				//)
				Until('"')
			);
			return r;
		}
	}
}
