using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Parser.Base;

using System.IO;

namespace Parser.ConfigTable
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length > 0)
			{
				Config cfg = new Config();
				cfg.Load(args[0]);

				Console.WriteLine(cfg.GetObject("config.username"));
			}
			
			using(StreamReader sr = new StreamReader(args[0]))
			{	
			    string input = sr.ReadToEnd();
				
			    ConfigTableParser t = new ConfigTableParser(input);
				
				try
				{
					Console.WriteLine("Parse result: {0}", t.Parse());
					Print(t.Tree, input);
				}
				catch(Exception ex)
				{
					Console.WriteLine(ex.Message);
				}

			    
			}
			
			Console.ReadKey();
		}

		static void Print(PegTree tree, string source)
		{
			if (tree.Root != null)
			{
				int level = 0;
				PrintNode(tree.Root, level, source);
			}
		}

		static void PrintNode(PegNode node, int level, string source)
		{
			string tab = "";
			for (int i = 0; i < level; ++i)
				tab += '\t';

			if (node.Id >= 7 && node.Id <= 10 || node.Id == 2)
				Console.WriteLine("{0}{1} -> {2}", tab, (ConfigTable)node.Id, node.Match.GetString(source));
			else
				Console.WriteLine("{0}{1}", tab, (ConfigTable)node.Id);

			if (node.Child != null)
				PrintNode(node.Child, level + 1, source);

			if (node.Next != null)
				PrintNode(node.Next, level, source);
		}
	}
}
