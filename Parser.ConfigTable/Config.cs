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
		
		public string GetString(string path)
		{
			return (string)GetObject(path);
		}
		
		public long GetLong(string path)
		{
			return (long)GetObject(path);
		}
		
		public double GetDouble(string path)
		{
			return (double)GetObject(path);
		}
		
		public bool GetBool(string path)
		{
			return (bool)GetObject(path);
		}
		
		private object SearchTree(string path)
		{
			string[] seg = path.Split(new char[] {'.'}, StringSplitOptions.RemoveEmptyEntries);
			
			PegNode node = _tree.Root.Child; //<- Pair
			
			for(int i = 0; i < seg.Count(); ++i)
			{
				node = FindKey(node, seg[i]); //returns Flat, Object, Array or null
				
				if(i < seg.Count() - 1) //if there are more segments available search for more objects or break
				{
					if(node != null && (ConfigTableEnum)node.Id == ConfigTableEnum.Object)
					{
						node = node.Child;
					}
					else
					{
						break;
					}
				}
				else
				{
					if(node != null && (ConfigTableEnum)node.Id == ConfigTableEnum.Object)
					{
						return GetObjectValue(node.Child);
					}
					
					if (node != null && (ConfigTableEnum)node.Id == ConfigTableEnum.Array)
					{
						return GetArrayValue(node.Child);
					}

					if (node != null && (ConfigTableEnum)node.Id == ConfigTableEnum.Flat)
					{
						return GetFlatValue(node.Child);
					}
				}
			}
			
			return null;
		}
		
		private PegNode FindKey(PegNode node, string key)
		{
			if(node == null)
				return null;
			
			PegNode k = node.Child;
			PegNode v = node.Child.Next;
			
			if(k.Match.GetString(_source).Equals(key))
			{
				return v;
			}

			if (node.Next != null)
				return FindKey(node.Next, key);	
		
			return null;
		}
		
		private object GetObjectValue(PegNode node)
		{
			PegNode n = node;
			List<string> keys = new List<string>();
			
			while(n != null && n.Child != null)
			{
				keys.Add(n.Child.Match.GetString(_source));
				n = n.Next;
			}
			return keys;
		}
		
		private object GetArrayValue(PegNode node)
		{
			PegNode n = node;
			List<object> objs = new List<object>();
			
			while(n != null)
			{
				objs.Add(GetFlatValue(n.Child));
				
				n = n.Next;
			}
			
			return objs.ToArray();
		}
		
		private object GetFlatValue(PegNode node)
		{
			if((ConfigTableEnum)node.Id == ConfigTableEnum.String) //string
			{
				return node.Match.GetString(_source);
			}

			if ((ConfigTableEnum)node.Id == ConfigTableEnum.Bool)
			{
				string tmp = node.Match.GetString(_source);
				if(tmp.Equals("true"))
					return true;
				return false;
			}

			if ((ConfigTableEnum)node.Id == ConfigTableEnum.Decimal) //long
			{
				string tmp = node.Match.GetString(_source);
				long value = 0;

				if (Int64.TryParse(tmp, out value))
				{
					return value;
				}
				else
				{
					throw new Exception("Failed to parse long value.");
				}
			}

			if ((ConfigTableEnum)node.Id == ConfigTableEnum.Number) //double
			{
				string tmp = node.Match.GetString(_source);
				double value = 0;
				
				if (Double.TryParse(tmp, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out value))
				{
					return value;
				}
				else
				{
					throw new Exception("Failed to parse double value.");
				}
			}
			
			throw new Exception("No valid value type found.");
		}
		
		private string LoadFile(string path)
		{
			using (StreamReader sr = new StreamReader(path))
			{
				return sr.ReadToEnd();
			}
		}
	}
}
