using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parser.PEG
{
	public class PegCharParser: PegBaseParser
	{
		protected string _src;
		
		public PegCharParser(string source)
		{
			_src = source;
			_pos = 0;
			_len = source.Length;
		}

		public int Pos
		{
			get
			{
				return _pos;
			}
		}
		
		#region Rules
		public bool Char(char c0)
		{
			if(c0 == _src[_pos])
			{
				++_pos;
				return true;
			}
			return false;
		}
		
		public bool CharRange(char c0, char c1)
		{
			if(_src[_pos] >= c0 && _src[_pos] <= c1)
			{
				++_pos;
				return true;
			}
			return false;
		}
		
		public bool OneOf(string s)
		{
			if(_pos < _len)
			{
				if(s.IndexOf(_src[_pos]) != -1)
				{
					++_pos;
					return true;
				}
			}
			return false;
		}
		
		public bool String(string s)
		{
			return false;
		}
		#endregion
	}
}
