using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Parser.Base
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

		private void GetLineNumber(int pos, out int ln, out int col)
		{
			string[] lines = _src.Split('\n');
			int lpos = 0;
			int lline = 0;
			
			foreach(string line in lines)
			{
				lpos += line.Length + 1;
				lline++;
				
				if(lpos > pos)
				{
					ln = lline;
					col = lpos - pos;
					return;
				}
			}
			ln = 0;
			col = 0;
		}

		public bool Fatal(string message)
		{
			int pos = _pos;
			
			int ln = 0;
			int col = 0;
			
			GetLineNumber(pos, out ln, out col);

			throw new Exception(System.String.Format("{0},{1} - {2}", ln, col, message));
		}
		
		#region Rules
		/// <summary>
		/// c
		/// </summary>
		public bool Char(char c0)
		{
			if(_pos < _len && c0 == _src[_pos])
			{
				++_pos;
				return true;
			}
			return false;
		}

		public bool Char(char c0, char c1)
		{
			if (_pos + 1 < _len 
				&& c0 == _src[_pos]
				&& c1 == _src[_pos + 1])
			{
				_pos += 2;
				return true;
			}
			return false;
		}

		public bool Char(char c0, char c1, char c2)
		{
			if (_pos + 2 < _len
				&& c0 == _src[_pos]
				&& c1 == _src[_pos + 1]
				&& c2 == _src[_pos + 2])
			{
				_pos += 3;
				return true;
			}
			return false;
		}

		public bool Char(char c0, char c1, char c2, char c3)
		{
			if (_pos + 3 < _len
				&& c0 == _src[_pos]
				&& c1 == _src[_pos + 1]
				&& c2 == _src[_pos + 2]
				&& c3 == _src[_pos + 3])
			{
				_pos += 4;
				return true;
			}
			return false;
		}

		public bool Char(char c0, char c1, char c2, char c3, char c4)
		{
			if (_pos + 4 < _len
				&& c0 == _src[_pos]
				&& c1 == _src[_pos + 1]
				&& c2 == _src[_pos + 2]
				&& c3 == _src[_pos + 3]
				&& c4 == _src[_pos + 4])
			{
				_pos += 5;
				return true;
			}
			return false;
		}

		public bool Char(char c0, char c1, char c2, char c3, char c4, char c5)
		{
			if (_pos + 5 < _len
				&& c0 == _src[_pos]
				&& c1 == _src[_pos + 1]
				&& c2 == _src[_pos + 2]
				&& c3 == _src[_pos + 3]
				&& c4 == _src[_pos + 4]
				&& c5 == _src[_pos + 5])
			{
				_pos += 6;
				return true;
			}
			return false;
		}

		public bool Char(char c0, char c1, char c2, char c3, char c4, char c5, char c6)
		{
			if (_pos + 6 < _len
				&& c0 == _src[_pos]
				&& c1 == _src[_pos + 1]
				&& c2 == _src[_pos + 2]
				&& c3 == _src[_pos + 3]
				&& c4 == _src[_pos + 4]
				&& c5 == _src[_pos + 5]
				&& c6 == _src[_pos + 6])
			{
				_pos += 7;
				return true;
			}
			return false;
		}

		public bool Char(char c0, char c1, char c2, char c3, char c4, char c5, char c6, char c7)
		{
			if (_pos + 7 < _len
				&& c0 == _src[_pos]
				&& c1 == _src[_pos + 1]
				&& c2 == _src[_pos + 2]
				&& c3 == _src[_pos + 3]
				&& c4 == _src[_pos + 4]
				&& c5 == _src[_pos + 5]
				&& c6 == _src[_pos + 6]
				&& c7 == _src[_pos + 7])
			{
				_pos += 8;
				return true;
			}
			return false;
		}

		public bool Char(char c0, char c1, char c2, char c3, char c4, char c5, char c6, char c7, char c8)
		{
			if (_pos + 8 < _len
				&& c0 == _src[_pos]
				&& c1 == _src[_pos + 1]
				&& c2 == _src[_pos + 2]
				&& c3 == _src[_pos + 3]
				&& c4 == _src[_pos + 4]
				&& c5 == _src[_pos + 5]
				&& c6 == _src[_pos + 6]
				&& c7 == _src[_pos + 7]
				&& c8 == _src[_pos + 8])
			{
				_pos += 9;
				return true;
			}
			return false;
		}

		public bool Char(char c0, char c1, char c2, char c3, char c4, char c5, char c6, char c7, char c8, char c9)
		{
			if (_pos + 9 < _len
				&& c0 == _src[_pos]
				&& c1 == _src[_pos + 1]
				&& c2 == _src[_pos + 2]
				&& c3 == _src[_pos + 3]
				&& c4 == _src[_pos + 4]
				&& c5 == _src[_pos + 5]
				&& c6 == _src[_pos + 6]
				&& c7 == _src[_pos + 7]
				&& c8 == _src[_pos + 8]
				&& c9 == _src[_pos + 9])
			{
				_pos += 10;
				return true;
			}
			return false;
		}
		
		/// <summary>
		/// [a-z]
		/// </summary>
		/// <param name="c0"></param>
		/// <param name="c1"></param>
		/// <returns></returns>
		public bool CharRange(char c0, char c1)
		{
			if(_pos < _len && _src[_pos] >= c0 && _src[_pos] <= c1)
			{
				++_pos;
				return true;
			}
			return false;
		}
		
		/// <summary>
		/// [xyz]
		/// </summary>
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
		
		/// <summary>
		/// Consume the source until the char is found. Doesn't consume the char
		/// </summary>
		public bool Until(char c0)
		{
			for(;;)
			{
				int pos = _pos;
				if(_pos < _len)
				{
					if(_src[_pos] == c0)
					{
						_pos = pos;
						return true;
					}
					++_pos;
				}
				else
				{
					return false;
				}
			}
		}
		
		public bool Into(Matcher rule, out string into)
		{
			int pos = _pos;
			if(rule())
			{
				into = _src.Substring(pos, _pos - pos);
				return true;
			}
			else
			{
				into = "";
				return false;
			}
		}
		
		[Obsolete("This feature is untestet. Use with care!")]
		public bool String(string s)
		{
			int strlen = s.Length;
			
			if(_pos < _len && _len - _pos > strlen)
			{
				if(_src.Substring(_pos, strlen).Equals(s))
				{
					_pos += strlen;
					return true;
				}
			}
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
		
		//public bool S()
		//{
		//    return Seq(() =>
		//        Char(' ') || Char('\t') || Char('\n') || Char('\r')
		//    );
		//}
		
		//public bool WS()
		//{
		//    return Option(() =>
		//        Seq(() =>
		//            Char(' ') || Char('\t')
		//        )
		//    );
		//}
		
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
