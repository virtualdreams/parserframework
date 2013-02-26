using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Parser.Base;

namespace Parser.ConfigTable
{
	public class Config
	{
		private string _source = "";
		private PegTree _tree = null;
	
		public Config()
		{
			
		}
		
		public void Load(string path)
		{
			_source = LoadFile(path);
			ConfigTableParser ctp = new ConfigTableParser(_source);
			
			bool parsed = ctp.Parse();
			
			if(!parsed)
			{
				throw new Exception("Parsing config file failed. Please review your config.");
			}
			
			_tree = ctp.Tree;
		}
		
		public object GetObject(string path)
		{
			return SearchTree(path);
		}
		
		private object SearchTree(string path)
		{
			string[] pathSegments = path.Split(new char[] {'.'}, StringSplitOptions.RemoveEmptyEntries);
			
			PegNode node = _tree.Root.Child; //<- Pair
			foreach(string key in pathSegments)
			{
				node = FindKey(node, key); //returns Flat, Object, Array or null

				if(node != null && (ConfigTableEnum)node.Id == ConfigTableEnum.Flat)
				{
					return "<<value>>";
				}

				if(node != null && (ConfigTableEnum)node.Id == ConfigTableEnum.Object)
				{
					node = node.Child;
				}

				//if (node != null && (ConfigTableEnum)node.Id == ConfigTableEnum.Array)
				//{
				//    node = node.Child.Child;
				//}
			}
			
			return null;
		}
		
		private PegNode FindKey(PegNode node, string key)
		{
			PegNode k = node.Child;
			PegNode v = node.Child.Next;
			
			if(k.Match.GetString(_source).Equals(key))
			{
				return v;
			}

			if (node.Next != null)
				FindKey(node.Next, key);	
		
			return null;
		}
		
		//private string FindKey(PegNode node, string keyName)
		//{
		//    if((ConfigTableEnum)node.Id == ConfigTableEnum.Key)
		//    {
		//        if(keyName.Equals(node.Match.GetString(_source)))
		//        {
		//            if(node.Next != null)
		//            {
		//                PegNode value = node.Next;
						
		//                if((ConfigTableEnum)value.Id == ConfigTableEnum.Flat)
		//                {
		//                    if(value.Child != null)
		//                    {
		//                        switch((ConfigTableEnum)value.Child.Id)
		//                        {
		//                            case ConfigTableEnum.String:
		//                                return value.Child.Match.GetString(_source);
										
		//                            case ConfigTableEnum.Decimal:
		//                                return value.Child.Match.GetString(_source);
										
		//                            case ConfigTableEnum.Number:
		//                                return value.Child.Match.GetString(_source);
										
		//                            case ConfigTableEnum.Bool:
		//                                return value.Child.Match.GetString(_source);
										
		//                            default:
		//                                return null;
		//                        }
		//                    }
		//                }
		//            }
		//        }
		//    }
			
		//    if (node.Child != null)
		//        FindKey(node.Child, keyName);

		//    if (node.Next != null)
		//        FindKey(node.Next, keyName);
				
		//    return null;
		//}
		
		private string LoadFile(string path)
		{
			using (StreamReader sr = new StreamReader(path))
			{
				return sr.ReadToEnd();
			}
		}
	}
}
