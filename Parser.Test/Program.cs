using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Parser.PEG;
using System.IO;

namespace Parser.Test
{
	class Program
	{
		static void Main(string[] args)
		{
			string input = "";
			if(args.Length > 0)
			{
				using(StreamReader sr = new StreamReader(args[0]))
				{
					input = sr.ReadToEnd();
				}
			}
			else
			{
				input = "config = {key = 123, name = \"thomas\", options = {server = \"server\", _port099 = 9000, timeout = .098, username = \"thomas\"}, keyfile=\"thomas.pub\", enabled = true, privateKeyFile = \"\"   }  \n    \r   ";
			}
			
			Table t = new Table(input);
			
			Console.WriteLine("Parse result: {0}", t.Parse());
			
			Print(t.Tree, input);
			
			Console.ReadKey();
		}
		
		static void Print(PegTree tree, string source)
		{
			if(tree.Root != null)
			{
				int level = 0;
				PrintNode(tree.Root, level, source);
			}
		}
		
		static void PrintNode(PegNode node, int level, string source)
		{
			string tab = "";
			for(int i = 0; i < level; ++i)
				tab += '\t';
			
			if(node.Id >= 7 && node.Id <= 10 || node.Id == 2)
				Console.WriteLine("{0}{1} -> {2}", tab, node.Id, node.Match.GetString(source));
			else
				Console.WriteLine("{0}{1}", tab, node.Id);
			
			if(node.Child != null)
				PrintNode(node.Child, level + 1, source);
				
			if(node.Next != null)
				PrintNode(node.Next, level, source);
		}
	}
}
