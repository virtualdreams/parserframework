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
			List<string> opts;
			
			OptionSet os = new OptionSet();
			os
				.Add("h|help|?", "Show this help.", option => ShowHelp(os))
				.Add("t|tree", "Display parse tree.", option => tree = option != null)
				.Add("r|request=", "Request an object.", option => {
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
							Console.WriteLine("Print tree for '{0}':", file);
							cfg.Print();
						}
						
						foreach(string path in request)
						{
							object obj = cfg.GetObject(path);
							Config.ConfigTypes type = cfg.GetObjectType(path);
							
							if(type == Config.ConfigTypes.Object)
							{
								foreach(string key in obj as Array)
								{
									Console.WriteLine("request '{0}'(object) = '{1}'", path, key);
								}
							}
							else if(type == Config.ConfigTypes.Array)
							{
								foreach(object o in obj as Array)
								{
									Console.WriteLine("request '{0}'(array) = '{1}'", path, o);
								}
							}
							else if(type == Config.ConfigTypes.Null)
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
			
			#if(DEBUG)
			Console.ReadKey();
			#endif
		}
		
		static private void ShowHelp(OptionSet os)
		{
			Console.WriteLine("ConfigTable.Check [options] FILES");
			Console.WriteLine("");
			os.WriteOptionDescriptions(Console.Error);
			//Environment.Exit(-1);
		}
	}
}
