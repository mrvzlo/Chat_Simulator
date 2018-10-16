using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Telegram.Bot;
using System.Diagnostics;
using System.Net;
using System.IO;

namespace Chat_Simulator
{
	class Program
	{
		public const long ADMIN = 361946418, BOT = 660272115;
		static Database DB;
		public const int ModeCount=4;
		const string Version = "3.1.0", name = "mypuska_bot";
		private static TelegramBotClient LoveBot;
		static DateTime TurnOn = DateTime.MinValue;
		public static string[] Tokens;
		//static Stopwatch SW = new Stopwatch(); 

		static bool CheckForInternetConnection()
		{
			try
			{
				var client = new WebClient();
				string s = client.DownloadString("https://telegram.org");
				return s.Contains("Telegram");
			}
			catch
			{
				return false;
			}
		}

		static void Main(string[] args)
		{
			while (!CheckForInternetConnection());
			Tokens = File.ReadAllLines("D:\\Sharp\\Chat_Simulator\\bin\\Debug\\Private.txt");
			LoveBot = new TelegramBotClient(Tokens[0]);
			DB = new Database();
			LoveBot.OnMessage += GetMessage;
			var me = LoveBot.GetMeAsync().Result;
			Console.Title = me.Username;
			LoveBot.StartReceiving();
			Say(Answering("приветствие"), DB.IDtoChat(ADMIN));
			while (Console.ReadLine() != "123");
			LoveBot.StopReceiving();
			DB.Closing();
		}

		private static void GetMessage(object sender, Telegram.Bot.Args.MessageEventArgs e)
		{
			var msg = e.Message;
			if (msg == null) return;
			msg.Date = msg.Date.ToLocalTime();
			if (TurnOn == DateTime.MinValue || DateTime.Compare(msg.Date, TurnOn) < 0) return; ///not first

			Chat CurChat =DB.IDtoChat(msg.Chat.Id);
			if (CurChat.LastMsgId == msg.MessageId) return;
			if (msg.Type == Telegram.Bot.Types.Enums.MessageType.Voice && CurChat.IsAdmin()) SpecCommands("block", CurChat);
			if (msg.Type == Telegram.Bot.Types.Enums.MessageType.Text)
			{
				if (msg.Text[0] == '/') CommandAnalyse(msg.Text, CurChat, msg.Date);
				else
				{
					bool FromBot = msg.ReplyToMessage != null && msg.ReplyToMessage.From.Id == BOT;
					if (!CurChat.IsPrivate() && !StringChanger.ToLower(StringChanger.RemoveSymb(msg.Text, "none")).Contains("сапфир") && !FromBot) return;
					if (CurChat.mode == 0) SimpleTextAnalyse(msg.Text, CurChat);
					else if (!TextToResp(msg.Text, CurChat))
					{
						if (!FromBot && !CurChat.IsPrivate()) return;
						if (CurChat.mode == 1)
						{
							FindPhoto(msg.Text, CurChat);
						}
						else if (CurChat.mode == 2)
						{
							msg.Text = StringChanger.RemoveSymb(msg.Text, "none");
							msg.Text = StringChanger.ToLower(msg.Text);
							FindSong(msg.Text, CurChat);
						}
						else if (CurChat.mode == 3)
						{
							msg.Text = StringChanger.RemoveSpaces(msg.Text);
							FindRhyme(msg.Text, CurChat);
						}
					}
				}
			}
			CurChat.LastMsgId = msg.MessageId;
			DB.SaveChat(CurChat, false);
		}

		static void CommandAnalyse(string src, Chat CurChat, DateTime DT)
		{
			if (src.Length < 1 || src[0] != '/') return;
			src = StringChanger.ToLower(src);
			int x = src.IndexOf("@");
			if (x > 0 && src.IndexOf(name) < 0) return;
			else if (x > 0) src = src.Substring(0, x);
			if (src == "/start") Say(Answering("приветствие"), CurChat);
			else if (src == "/reset") SpecCommands("reset", CurChat);
			else if (src == "/show") SpecCommands("show", CurChat);
			else if (src == "/exit") SpecCommands("exit", CurChat);
			else if (src == "/block") SpecCommands("block", CurChat);
			else if (src == "/full") SpecCommands("full", CurChat);
			else if (src == "/help") Say(DB.Help(), CurChat);
			else if (src == "/talk") CurChat.ChangeMode(0);
			else if (src == "/photo") CurChat.ChangeMode(1);
			else if (src == "/song") CurChat.ChangeMode(2);
			else if (src == "/poet") CurChat.ChangeMode(3);
			else if (src == "/ping") Say(Pong(DT), CurChat);
			else if (src == "/next" && CurChat.mode == 1) SendPhoto(CurChat);
			else if (src == "/next" && CurChat.mode == 3) SayRhymes(CurChat);
			else if (src == "/stat") Say(Answering(CurChat.mode + "mode"), CurChat);
			DB.SaveChat(CurChat,false);
		}

		static string Pong(DateTime sent)
		{
			DateTime now = DateTime.Now;
			now = now.AddSeconds(1);
			TimeSpan TS = now.Subtract(sent);
			double a = TS.TotalSeconds;
			a = Math.Round(a, 4);
			if (TS.TotalSeconds <= 1) return Answering("мгновенно");
			return a + " секунд";
		}

		static void SimpleTextAnalyse(string src, Chat CurChat)
		{
			if (CurChat.AddNew == "")
			{
				if (!StringChanger.HasKirill(src))
				{
					src = StringChanger.LatinToKirill(src);
					Say(Answering("mistake") + "\n" + src, CurChat);
				}
				else
				{
					src = StringChanger.ToLower(src);
					src = StringChanger.RemoveSymb(src,"part");
					if (src[src.Length - 1] != '.' && src[src.Length - 1] != '?' && src[src.Length - 1] != '!') src += '.';
					if (src.Length > 2 && src[0] == '?')
					{
						Say(Answering("чтосказать"), CurChat);
						CurChat.AddNew = src.Substring(1);
					}
					else if (StringChanger.HasKirill(src)) TextToResp(src, CurChat);
				}
			}
			else // Addnew !=""
			{
				if (!DB.CancelSignal(src))
				{
					src = StringChanger.RemoveSymb(src,"none");
					DB.UpdateOrSaveReq(src,CurChat.AddNew);
					Say(Answering("запомню"), CurChat);
					Console.WriteLine(CurChat.ID + " added " + CurChat.AddNew + " -> " + src);
				}
				else if (CurChat.AddNew != "")
				{
					DB.DeleteReq(CurChat.AddNew);
					Say(Answering("cancel"), CurChat);
				}
				CurChat.AddNew = "";
			}
			DB.SaveChat(CurChat,false);
		}

		static bool TextToResp(string src, Chat CurChat)
		{
			string key = DB.SearchKey(src);
			string[] COMMANDS = { "exit", "block", "reset", "shutdown" };
			bool command = false;
			foreach (string s in COMMANDS) if (!command) command = s == key;
			if (!command || CurChat.mode != 0)
			{
				if (key.Contains("mode") && key.Contains("to"))
				{
					CurChat.ChangeMode(key[2] - '0');
					return true;
				}
				else if (CurChat.mode == 0)
				{
					if (key == "чтосказать" && !CurChat.IsPrivate()) key = "idk";
					Say(Answering(key,CurChat), CurChat); //!!!!!!!!!!!!
					if (key== "чтосказать") CurChat.AddNew = src;
					DB.SaveChat(CurChat,false);
					return true;
				}
			}
			else
			{
				SpecCommands(key, CurChat);
				return true;
			}
			return false;
		}

		static void SpecCommands(string a, Chat CurChat)
		{
			if (CurChat.IsAdmin())
			{
				Say(Answering(a), CurChat);
				if (a == "exit" || a == "shutdown")
				{
					LoveBot.StopReceiving();
					Thread.Sleep(1000);
					if (a == "shutdown") Process.Start("shutdown", "/s /t 0");
					Environment.Exit(0);
				}
				else if (a == "block")
				{
					Process.Start(@"C:\WINDOWS\system32\rundll32.exe", "user32.dll,LockWorkStation");
				}
				else if (a == "show")
				{
					//Process.Start(@"msg.txt");
				}/*
				else if (a == "full")
				{
					string r="";
					foreach (string s in CurChat.Song)
					{
						r += s + "\n";
					}
					Say(r, CurChat);
				}*/
			}
			else 
			{
				Say(Answering("cannot"), CurChat);
			}
		}

		static public string Answering(string key, Chat CurChat=null)
		{
			string BotPrev;
			if (CurChat != null) BotPrev = CurChat.BotPrev;
			else BotPrev = "";
			string responce = "";
			if (key == null || key == "ignore") return null;
			if (key == "привет" && BotPrev.Contains("привет")) key = "а ты как";
			if (key == "как дела" && !BotPrev.Contains("как дела")) key += "+";
			if ((key == "любима" || key == "любишь") && !CurChat.IsAdmin()) key += "-";
			if (DB.ContainsKey(key))
			{
				responce = DB.RandResp(key) + " ";
				if (key == "приветствие") responce = DB.RandResp("привет") + ", " + responce + DB.RandResp("smile");
				else if (key== "настроение") responce = DB.RandResp("настроение") + DB.RandResp("smile");
				else if (key== "как дела+") responce = DB.RandResp("как дела") + ", " + responce;
				else if ((key== "любима" || key== "любишь" || key== "пока") && CurChat.IsAdmin()) responce += DB.RandResp("kiss");
				else if (key.Contains("привет")) responce += DB.RandResp("smile");
				else if (key== "none") responce += DB.RandResp("sad");
			}
			else responce = key;
			return responce;
		}

		static void FindPhoto(string tag, Chat CurChat)
		{
			string url = "https://www.google.com/search?q=" + tag + "&tbm=isch";
			url.Replace(" ", "+");
			string data = "";

			var request = (HttpWebRequest)WebRequest.Create(url);
			var response = (HttpWebResponse)request.GetResponse();

			using (Stream dataStream = response.GetResponseStream())
			{
				using (var sr = new StreamReader(dataStream))
				{
					data = sr.ReadToEnd();
				}
			}
			//File.WriteAllText("log.html", data);
			CurChat.Arr = StringChanger.HTMLtoPhoto(data);
			CurChat.index = 0;
			SendPhoto(CurChat);
		}

		static public void SendPhoto(Chat CurChat)
		{
			if (CurChat.Arr == null) return;
			int count = CurChat.Arr.GetLength(0);
			if (count < 1) return;
			CurChat.index++;
			if (CurChat.index < CurChat.Arr.Length)
				LoveBot.SendPhotoAsync(CurChat.ID, CurChat.Arr[CurChat.index]);
			else Say(Answering("кончилось"), CurChat);
			CurChat.BotPrev = "";
			DB.SaveChat(CurChat,true);
		}

		static public void Say(string s, Chat CurChat)
		{
			if (TurnOn==DateTime.MinValue) TurnOn = DateTime.Now;
			if (s == null || s == " ") return;
			LoveBot.SendTextMessageAsync(CurChat.ID, s);
			CurChat.BotPrev = s;
			DB.SaveChat(CurChat, false);
		}

		static void Check(string s = "0")
		{
			Console.WriteLine(s);
		}

		static void FindSong(string src, Chat CurChat)
		{
			string next = FindRow(src, CurChat.Arr);
			if (next != null)
			{
				Say(next, CurChat);
				return;
			}
			else
			{
				WebClient wc = new WebClient();
				string goog = "http://www.google.com/search?q=" + src + " текст песни";
				goog.Replace(' ', '+');
				string s = wc.DownloadString(goog),temp;
				s = s.Substring(s.IndexOf("id=\"search\""));
				string[] urls = StringChanger.HTMLtoURLS(s);
				foreach (string url in urls)
				{
					Console.WriteLine(url);
					try
					{
						wc.Encoding = Encoding.UTF8;
						temp = wc.DownloadString(url);
						if (!StringChanger.HasKirill(temp))
						{
							wc.Encoding = Encoding.Default;
							temp = wc.DownloadString(url);
						}
					}
					catch (WebException e) { continue; }

					string[] Song = StringChanger.HTMLtoText(temp);
					CurChat.Arr = Song;
					next = FindRow(src, Song);
					if (next != null)
					{
						Say(next, CurChat);
						DB.SaveChat(CurChat,true);
						return;
					}
				}
			}

			Say(Answering("idk"),CurChat);
		}

		static string FindRow(string src, string[] Song)
		{
			bool found = false;
			if (Song != null)
			{
				foreach (string s in Song)
				{
					if (!found && StringChanger.CalculateSimilarity(s, src) > 0.9) found = true;
					else if (found) return StringChanger.ToUpper(s);
				}
				if (found) return Answering("finish");
			}
			return null;
		}

		static void FindRhyme(string src, Chat CurChat)
		{
			src = StringChanger.ToLower(src);
			if (src.IndexOf(' ')>0) src = src.Substring(src.LastIndexOf(' '));
			if (src.Length < 0) return;
			CurChat.index = 0;
			CurChat.Arr = null;
			string goog = "https://rifmus.net/rifma/" + src;
			WebClient wc = new WebClient();
			wc.Encoding = Encoding.UTF8;
			int x = -1;
			string s="";
			try
			{
				s = wc.DownloadString(goog);
				x = s.IndexOf("multicolumn");
			}
			catch{}
			if (x < 0)
			{
				Say(Answering("none"), CurChat);
				return;
			}
			s = s.Substring(x);
			x = s.IndexOf("</div>");
			s = s.Substring(0, x);
			CurChat.Arr = StringChanger.HTMLtoText(s);
			SayRhymes(CurChat);
		}

		static void SayRhymes(Chat CurChat)
		{
			string res="";
			if (CurChat.Arr == null)
			{
				Say(Answering("none"), CurChat);
			}
			else
			{
				int n = CurChat.RhymeCount + CurChat.index;
				for (; CurChat.index < n && CurChat.index < CurChat.Arr.Length; CurChat.index++)
					res += CurChat.Arr[CurChat.index] + "\n";
				Say(res, CurChat);
			}
			DB.SaveChat(CurChat, true);
		}
	}
}
