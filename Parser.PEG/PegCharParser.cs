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

		#region Built-In
		public bool LettersLowerCase()
		{
			return CharRange('a', 'z');
		}
		
		public bool LettersUpperCase()
		{
			return CharRange('A', 'Z');
		}
		
		public bool Letters()
		{
			return Seq(() =>
				LettersLowerCase() || LettersUpperCase()
			);
		}
		
		public bool Digits()
		{
			return CharRange('0', '9');
		}
		
		public bool Dec()
		{
			return Digits();
		}
		
		public bool Oct()
		{
			return CharRange('0', '7');
		}
		
		public bool Hex()
		{
			return Seq(() =>
				   Dec()
				|| CharRange('a', 'f')
				|| CharRange('A', 'F')
			);
		}
		
		public bool S()
		{
			return Seq(() =>
				Char(' ') || Char('\t') || Char('\n') || Char('\r')
			);
		}
		
		public bool WS()
		{
			return Seq(() =>
				Char(' ') || Char('\t')
			);
		}
		
		public bool LF()
		{
			return Char('\n');
		}
		
		public bool CR()
		{
			return Char('\r');
		}
		
		public bool CRLF()
		{
			return Seq(() =>
				CR() && LF()
			);
		}
		#endregion
	}
}
