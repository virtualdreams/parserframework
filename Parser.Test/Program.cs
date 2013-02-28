using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Parser.Base;

using System.IO;
using System.Diagnostics;

namespace Parser.ConfigTable
{
	class Program
	{
		static void Main(string[] args)
		{
			if (args.Length > 0)
			{
				Stopwatch ti= new Stopwatch();
				ti.Start();
				for(int i = 0; i < 100; ++i)
				{
					Config cfg = new Config();
					cfg.Load(args[0]);
				}
				ti.Stop();
				Console.WriteLine("Elapsed: " + ti.ElapsedMilliseconds);
			}
			Console.ReadKey();
			return;
			
			
			
			if (args.Length > 0)
			{
				Config cfg = new Config();
				cfg.Load(args[0]);

				string[] keys = new string[] { 
					"config",
					"cfg.less",
					"config.less",
					"config.username.less",
					"config.username", 
					"config.float1", 
					"config.float2", 
					"config.float3", 
					"config.int1", 
					"config.int2", 
					"config.client.cert.enabled", 
					"config.array2", 
					"config.server"
				};
				
				Config.ConfigTypes t1 = cfg.GetObjectType("config.array");
				Config.ConfigTypes t2 = cfg.GetObjectType("object.");

				object obj = null;
				foreach (string key in keys)
				{
					obj = cfg.GetObject(key);
					if (obj != null)
					{
						Type type = obj.GetType();
						if(type.IsArray)
						{
							if(obj is string)
							{
								Console.WriteLine("keylist");
							}
							
							if(obj is object)
							{
								foreach(object o in obj as Array)
								{
									Console.WriteLine(String.Format("{0}: {1} = {2}", o.GetType().ToString(), key, o));
								}
							}
						}
						else
						{

							Console.WriteLine(String.Format("{0}: {1} = {2}", obj.GetType().ToString(), key, obj));
						}
					}
					else
						Console.WriteLine(String.Format("Path not found '{0}'", key));
				}
			}

			using (StreamReader sr = new StreamReader(args[0]))
			{
				string input = sr.ReadToEnd();

				ConfigTableParser t = new ConfigTableParser(input);

				try
				{
					Console.WriteLine("Parse result: {0}", t.Parse());
					Print(t.Tree, input);
				}
				catch (Exception ex)
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
				tab += ' ';

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
