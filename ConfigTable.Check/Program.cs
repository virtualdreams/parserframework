using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Mono.Options;
using Parser.ConfigTable;

namespace ConfigTable.Check
{
	class Program
	{
		static void Main(string[] args)
		{
			bool tree = false;
			List<string> request = new List<string>();
			List<string> plain = new List<string>();
			List<string> opts;
			
			OptionSet os = new OptionSet();
			os
				.Add("h|help|?", "Show this help.", option => ShowHelp(os))
				.Add("tree", "Display the parse tree if can parsed.", option => tree = option != null)
				.Add("p|plain=", "Request an object from file and return the plain value.", option => {
					if(!plain.Contains(option))
					{
						plain.Add(option);
					}
				})
				.Add("r|request=", "Request an object from file.", option => {
					if(!request.Contains(option))
					{
						request.Add(option);
					}
				});
			
			try
			{
				opts = os.Parse(args);

				foreach (string file in opts)
				{
					try
					{
						Config cfg = new Config();
						cfg.Load(file);
						
						if(tree)
						{
							Console.WriteLine("Print tree for '{0}'", file);
							cfg.Print();
						}

						foreach (string path in plain)
						{
							object obj = cfg.GetObject(path);
							Console.WriteLine(obj);
						}
						
						foreach(string path in request)
						{
							object obj = cfg.GetObject(path);
							if(cfg.GetObjectType(path) == Config.ConfigTypes.Object)
							{
								Console.WriteLine("request '{0}' = '{1}'", path, "<Object>");
							}
							else if(cfg.GetObjectType(path) == Config.ConfigTypes.Array)
							{
								foreach(object o in obj as Array)
								{
									Console.WriteLine("request '{0}' = '{1}'", path, o);
								}
							}
							else if(cfg.GetObjectType(path) == Config.ConfigTypes.Null)
							{
								Console.WriteLine("request '{0}' = '{1}'", path, "<Not found>");
							}
							else
							{
								Console.WriteLine("request '{0}' = '{1}'", path, obj);
							}
						}
					}
					catch(Exception ex)
					{
						Console.WriteLine(ex.Message);
					}
				}
			}
			catch(OptionException)
			{
				ShowHelp(os);
			}

			Console.ReadKey();
		}
		
		static private void ShowHelp(OptionSet os)
		{
			Console.WriteLine("Help for ConfigTable.Check");
			os.WriteOptionDescriptions(Console.Error);
			//Environment.Exit(-1);
		}
	}
}
