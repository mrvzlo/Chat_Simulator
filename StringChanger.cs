using System;
using System.Collections.Generic;

namespace Chat_Simulator
{
	class StringChanger
	{
		static public string LatinToKirill(string src)
		{
			string res = "";
			for (int i = 0; i < src.Length; i++)
			{
				switch (src[i])
				{
					case 'q': res += 'й'; break;
					case 'w': res += 'ц'; break;
					case 'e': res += 'у'; break;
					case 'r': res += 'к'; break;
					case 't': res += 'е'; break;
					case 'y': res += 'н'; break;
					case 'u': res += 'г'; break;
					case 'i': res += 'ш'; break;
					case 'o': res += 'щ'; break;
					case 'p': res += 'з'; break;
					case '[': res += 'х'; break;
					case ']': res += 'ъ'; break;
					case 'a': res += 'ф'; break;
					case 's': res += 'ы'; break;
					case 'd': res += 'в'; break;
					case 'f': res += 'а'; break;
					case 'g': res += 'п'; break;
					case 'h': res += 'р'; break;
					case 'j': res += 'о'; break;
					case 'k': res += 'л'; break;
					case 'l': res += 'д'; break;
					case ';': res += 'ж'; break;
					case '\'': res += 'э'; break;
					case 'z': res += 'я'; break;
					case 'x': res += 'ч'; break;
					case 'c': res += 'с'; break;
					case 'v': res += 'м'; break;
					case 'b': res += 'и'; break;
					case 'n': res += 'т'; break;
					case 'm': res += 'ь'; break;
					case ',': res += 'б'; break;
					case '.': res += 'ю'; break;
					case '`': res += 'ё'; break;

					case 'Q': res += 'Й'; break;
					case 'W': res += 'Ц'; break;
					case 'E': res += 'У'; break;
					case 'R': res += 'К'; break;
					case 'T': res += 'Е'; break;
					case 'Y': res += 'Н'; break;
					case 'U': res += 'Г'; break;
					case 'I': res += 'Ш'; break;
					case 'O': res += 'Щ'; break;
					case 'P': res += 'З'; break;
					case '{': res += 'Ч'; break;
					case '}': res += 'Ъ'; break;
					case 'A': res += 'Ф'; break;
					case 'S': res += 'Ы'; break;
					case 'D': res += 'В'; break;
					case 'F': res += 'А'; break;
					case 'G': res += 'П'; break;
					case 'H': res += 'Р'; break;
					case 'J': res += 'О'; break;
					case 'K': res += 'Л'; break;
					case 'L': res += 'Д'; break;
					case ':': res += 'Ж'; break;
					case '\"': res += 'Э'; break;
					case 'Z': res += 'Я'; break;
					case 'X': res += 'Ч'; break;
					case 'C': res += 'С'; break;
					case 'V': res += 'М'; break;
					case 'B': res += 'И'; break;
					case 'N': res += 'Т'; break;
					case 'M': res += 'Ь'; break;
					case '<': res += 'Б'; break;
					case '>': res += 'Ю'; break;
					case '~': res += 'Ё'; break;
					default: res += src[i]; break;
				}
			}
			return res;
		}
		static public bool NonLatin(char c)
		{
			if (c >= Convert.ToInt32('A') && c <= Convert.ToInt32('Z')) return false;
			if (c >= Convert.ToInt32('a') && c <= Convert.ToInt32('z')) return false;
			return true;
		}
		static public bool HasKirill(string s)
		{
			for (int i = 0; i < s.Length; i++)
				if ((s[i] >= Convert.ToInt32('а') && s[i] <= Convert.ToInt32('я')) || (s[i] >= Convert.ToInt32('А') && s[i] <= Convert.ToInt32('Я'))
					|| s[i] == 'ё' || s[i] == 'Ё') return true;
			return false;
		}
		static public bool Caps(string s)
		{
			string check = "., -!?%:;";
			for (int i = 0; i < s.Length; i++)
				if ((s[i] > 'Я' || s[i] < 'А') && s[i] != 'Ё')
				{
					bool temp = true;
					for (int j = 0; j < check.Length; j++)
						if (check[j] == s[i]) temp = false;
					if (temp) return false;
				}
			return true;
		}
		static public bool IsDigit(char c)
		{
			return c >= '0' && c <= '9';
		}
		static public string ToLower(string s)
		{
			if (s == null) return null;
			for (int i = 0; i < s.Length; i++)
			{
				char a = s[i];
				if (a >= 'A' && a <= 'Z') a = Convert.ToChar(Convert.ToInt32(a) + Convert.ToInt32('a') - Convert.ToInt32('A'));
				if (a >= 'А' && a <= 'Я') a = Convert.ToChar(Convert.ToInt32(a) + Convert.ToInt32('а') - Convert.ToInt32('А'));
				if (a == 'Ё') a = 'ё';
				if (a != s[i])
				{
					s = s.Remove(i, 1);
					if (a != ' ') s = s.Insert(i, a.ToString());
				}
			}
			return s;
		}
		static public string ToUpper(string s)
		{
			for (int i = 0; i < s.Length; i++)
			{
				char a = s[i];
				if (a >= 'a' && a <= 'z') a = Convert.ToChar(Convert.ToInt32(a) + Convert.ToInt32('A') - Convert.ToInt32('a'));
				if (a >= 'а' && a <= 'я') a = Convert.ToChar(Convert.ToInt32(a) + Convert.ToInt32('А') - Convert.ToInt32('а'));
				if (a == 'ё') a = 'Ё';
				if (a != s[i])
				{
					s = s.Remove(i, 1);
					if (a != ' ') s = s.Insert(i, a.ToString());
				}
			}
			return s;
		}

		static public string RemoveSymb(string s, string filter)
		{
			string symb = "";
			if (filter == "part") symb = ",;:&/\"^";
			else if (filter == "none") symb = "(){}[].,;:&!/?\\\"<>^-+";
			else if (filter == "brac") symb = "(){}[]";
			for (int i = 0; i < s.Length; i++)
			{
				char a = s[i];
				foreach (char c in symb) if (a == c) a = ' ';
				if (a != s[i])
				{
					s = s.Remove(i, 1);
					if (a != ' ') s = s.Insert(i, a.ToString());
				}
			}
			return s;
		}
		static public string[] HTMLtoText(string src)
		{
			List<string> mylist = new List<string>();
			bool open=false, script=false;
			char tag ='\0';
			string s = "";
			for (int i=0; i<src.Length; i++)
			{
				if (src[i] == '<')
				{
					open = true;
					if (src.Length > i + 1) tag = src[i + 1];
					else tag = '\0';
				}
				else if (src[i] == '>')
				{
					if (s.Contains("<script")) script = true;
					else if (s.Contains("</script")) script = false;
					if (s != "" && !script && HasKirill(s) && tag!='a')
					{
						s = RemoveSpaces(s);
						s = RemoveSymb(s, "brac");
						mylist.Add(ToLower(s));
						s = "";
					}
					open = false;
				}
				else if (!open) s += src[i];
			}
			for (int i = 0; i < mylist.Count; i++)
			{
				string str = mylist[i];
				bool remove = false;
				if (str.Length > 60 || str.Contains("]") || str.Length == 1) remove = true;
				else if (i > 1 && str == mylist[i-1]) remove = true;
				if (remove)
				{
					mylist.Remove(str);
					i--;
				}
			}
			return mylist.ToArray();
		}

		static public string RemoveSpaces(string s)
		{
			while (s.Length > 0 && !HasKirill(s[0].ToString()) && !IsDigit(s[0])) s = s.Substring(1);
			s = s.Replace("\n", " ");
			s = s.Replace(" ", " ");
			s = s.Replace("	", " ");
			while (s != s.Replace("  ", " ")) s=s.Replace("  ", " ");
			return s;
		}

		static public string[] HTMLtoURLS(string s)
		{
			List<string> mylist = new List<string>();
			int x = 0, y = 0;
			while(true)
			{
				x = s.IndexOf("http",y);
				if (x < 0) break;
				y = s.IndexOf("&amp;",x);
				if (y < 0) break;
				string url = s.Substring(x, y - x);
				if (url.Contains("google") || url.Contains("youtube")) continue;
				mylist.Add(url);
			}
			return mylist.ToArray();
		}
		
		static public string[] HTMLtoPhoto(string s)
		{
			List<string> mylist = new List<string>();
			int ndx = s.IndexOf("/head>", StringComparison.Ordinal);
			ndx = s.IndexOf("<img", ndx, StringComparison.Ordinal);

			while (ndx >= 0)
			{
				ndx = s.IndexOf("src=\"", ndx, StringComparison.Ordinal);
				ndx = ndx + 5;
				int ndx2 = s.IndexOf("\"", ndx, StringComparison.Ordinal);
				string url = s.Substring(ndx, ndx2 - ndx);
				if (!url.Contains("/static/img/blank.gif")) mylist.Add(url);
				ndx = s.IndexOf("<img", ndx, StringComparison.Ordinal);
			}
			return mylist.ToArray();
		}

		static private int ComputeLevenshteinDistance(string source, string target)
		{
			if ((source == null) || (target == null)) return 0;
			if ((source.Length == 0) || (target.Length == 0)) return 0;
			if (source == target) return source.Length;

			int sourceWordCount = source.Length;
			int targetWordCount = target.Length;

			// Step 1
			if (sourceWordCount == 0)
				return targetWordCount;

			if (targetWordCount == 0)
				return sourceWordCount;

			int[,] distance = new int[sourceWordCount + 1, targetWordCount + 1];

			// Step 2
			for (int i = 0; i <= sourceWordCount; distance[i, 0] = i++) ;
			for (int j = 0; j <= targetWordCount; distance[0, j] = j++) ;

			for (int i = 1; i <= sourceWordCount; i++)
			{
				for (int j = 1; j <= targetWordCount; j++)
				{
					// Step 3
					int cost = (target[j - 1] == source[i - 1]) ? 0 : 1;

					// Step 4
					distance[i, j] = Math.Min(Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1), distance[i - 1, j - 1] + cost);
				}
			}

			return distance[sourceWordCount, targetWordCount];
		}

		static public double CalculateSimilarity(string source, string target)
		{
			if ((source == null) || (target == null)) return 0.0;
			if ((source.Length == 0) || (target.Length == 0)) return 0.0;
			if (source == target) return 1.0;

			int stepsToSame = ComputeLevenshteinDistance(source, target);
			return (1.0 - ((double)stepsToSame / (double)Math.Max(source.Length, target.Length)));
		}
	}
}
