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
		public enum ConfigTypes
		{
			Object,
			Array,
			String,
			Double,
			Long,
			Bool,
			Null
		}
		
		private string _source = "";
		private PegTree _tree = null;
	
		public Config()
		{
			
		}
		
		public void Load(string path)
		{
			_source = LoadFile(path);
			ConfigTableParser ctp = new ConfigTableParser(_source);
			
			try
			{
				bool parsed = ctp.Parse();
			
				if(parsed)
				{
					_tree = ctp.Tree;
				}
				else
				{
					throw new Exception("Parsing config file failed. Pleas review your config.");
				}
			}
			catch(Exception ex)
			{
				throw ex;
			}
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
		
		public object[] GetArray(string path)
		{
			return (object[])GetObject(path);
		}
		
		public ConfigTypes GetObjectType(string path)
		{
			object obj = GetObject(path);
			if(obj != null)
			{
				Type type = obj.GetType();
				if(type == typeof(string))
					return ConfigTypes.String;
					
				if(type == typeof(double))
					return ConfigTypes.Double;
					
				if(type == typeof(long))
					return ConfigTypes.Long;
					
				if(type.IsArray && type == typeof(string[]))
					return ConfigTypes.Object;
				
				if(type.IsArray && type == typeof(object[]))
					return ConfigTypes.Array;
			}
			
			return ConfigTypes.Null;
		}
		
		private object SearchTree(string path)
		{
			string[] seg = path.Split(new char[] {'.', '/', '|'}, StringSplitOptions.RemoveEmptyEntries);
			
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
				else //the last segment must be one of...
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
			PegNode n = node;
			while(n != null)
			{
				PegNode child = GetChild(n, key);
				if(child != null)
				{
					return child;
				}
				n = n.Next;
			}
			return null;
		}
		
		private PegNode GetChild(PegNode node, string key)
		{
			if (node == null)
				return null;

			PegNode k = node.Child;
			PegNode v = node.Child.Next;

			if (k.Match.GetString(_source).Equals(key))
			{
				return v;
			}
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
			return keys.ToArray();
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
