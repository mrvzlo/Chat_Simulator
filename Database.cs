using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace Chat_Simulator
{
	class Database
	{
		MySqlConnection MyConn;
		static Random Rand = new Random();

		public void Closing()
		{
			MyConn.Close();
		}

		public Database()
		{
			MyConn = new MySqlConnection("server=" + Program.Tokens[4] + ";user=" + Program.Tokens[1] + ";database=" + Program.Tokens[2] + ";password=" + Program.Tokens[3] + ";SslMode=none;characterset=Utf8mb4");
			MyConn.Open();
		}

		public void SaveReq(string key, string src)
		{
			string sql = "Insert into Request values ('" + key + "', '" + src + "')";
			using (MySqlCommand command = new MySqlCommand(sql, MyConn))
			{
				command.ExecuteNonQuery();
			}
		}

		public void AddChat(long id)
		{
			string sql = "Insert into Chat (id, rhymes) values (" + id + ", 5)";
			using (MySqlCommand command = new MySqlCommand(sql, MyConn)){
				command.ExecuteNonQuery();
			}
		}

		public void SaveChat(Chat CurChat, bool WithArr)
		{
			string sql = "Update Chat set ";
			sql += " message_id = " + CurChat.LastMsgId + ",";
			sql += " mode = " + CurChat.mode + ",";
			sql += " botprev = '" + CurChat.BotPrev + "',";
			sql += " addnew = '" + CurChat.AddNew+"',";
			sql += " string_id = " + CurChat.index;
			sql += " Where id = " + CurChat.ID;
			using (MySqlCommand command = new MySqlCommand(sql, MyConn))
			{
				command.ExecuteScalar();
			}
			if (WithArr) ReplaceArray(CurChat);
		}

		public void ReplaceArray(Chat CurChat)
		{
			string sql = "DELETE FROM Arrays WHERE Chat = " + CurChat.ID;
			using (MySqlCommand command = new MySqlCommand(sql, MyConn))
			{
				command.ExecuteScalar();
			}
			int i = 0;
			foreach (string s in CurChat.Arr)
			{
				if (i == 0) sql = "Insert into Arrays (Chat, string) values (" + CurChat.ID + ", '" + s + "')";
				else sql += ", (" + CurChat.ID + ", '" + s + "')";
				i++;
			}
			using (MySqlCommand command = new MySqlCommand(sql, MyConn))
			{
				command.ExecuteScalar();
			}
		}

		public string Help()
		{
			string sql = "Select * From Help";
			string res = "";
			using (MySqlCommand command = new MySqlCommand(sql, MyConn))
			{
				MySqlDataReader reader = command.ExecuteReader();
				while (reader.Read()) res += reader.GetValue(0) + "\n";
			}
			return res;
		}

		public Chat IDtoChat(long id)
		{
			Chat C = new Chat();
			C.ID = id;
			bool found = false;
			string sql = "SELECT * FROM Chat WHERE id = " + id;
			using (MySqlCommand command = new MySqlCommand(sql, MyConn))
			{
				MySqlDataReader reader = command.ExecuteReader();
				if (reader.Read())
				{
					found = true;
					C.LastMsgId = Convert.ToInt32(reader.GetValue(1));
					C.mode = Convert.ToInt32(reader.GetValue(2));
					C.index = Convert.ToInt32(reader.GetValue(3));
					C.BotPrev = Convert.ToString(reader.GetValue(4));
					C.AddNew = Convert.ToString(reader.GetValue(5));
					C.RhymeCount = Convert.ToInt32(reader.GetValue(6));
				}
			}
			if (!found)
			{
				C.RhymeCount = 5;
				C.mode = 0;
				C.index = 0;
				C.LastMsgId = 0;
				AddChat(id);
			}
			else
			{
				sql = "SELECT String FROM Arrays WHERE Chat = " + id;
				using (MySqlCommand command = new MySqlCommand(sql, MyConn))
				{
					MySqlDataReader reader = command.ExecuteReader();
					List<string> L = new List<string>();
					while (reader.Read())
					{
						L.Add(reader.GetValue(0).ToString());
					}
					C.Arr = L.ToArray();
				}
			}
			return C;
		}

		public string RandResp(string src)
		{
			string sql = "SELECT answer FROM Responce WHERE key_word = '" + src + "'";
			List<string> L = new List<string>();
			using (MySqlCommand command = new MySqlCommand(sql, MyConn))
			{
				MySqlDataReader reader = command.ExecuteReader();
				while (reader.Read()) L.Add(reader.GetValue(0).ToString()); 
			}
			int n = L.Count;
			if (n < 1) return null;
			return L[Rand.Next(n)];
		}

		public bool ContainsKey(string src)
		{
			string sql = "SELECT key_word FROM Responce WHERE key_word = '" + src +"'";
			using (MySqlCommand command = new MySqlCommand(sql, MyConn))
			{
				MySqlDataReader reader = command.ExecuteReader();
				return reader.HasRows;
			}
		}

		public bool CancelSignal(string src)
		{
			string sql = "SELECT key_word FROM Request WHERE source = '" + src+"'";
			using (MySqlCommand command = new MySqlCommand(sql, MyConn))
			{
				var a = command.ExecuteScalar();
				return (a != null && a.ToString() == "cancel");
			}
		}

		public void UpdateOrSaveReq(string key, string req)
		{
			string sql = "SELECT key_word FROM Request WHERE source = '" + req + "'";
			int x=0;
			using (MySqlCommand command = new MySqlCommand(sql, MyConn))
			{
				MySqlDataReader reader = command.ExecuteReader();
				while (reader.Read()) x++;
			}
			if (x == 0) SaveReq(key, req);
			else
			{
				sql = "Update Request set key_word = '" + key + "' where source = '" + req + "'";
				using (MySqlCommand command = new MySqlCommand(sql, MyConn))
				{
					command.ExecuteReader().Close();
				}
			}
		}

		public void DeleteReq(string req)
		{
			string sql = "Delete from Request where source = '" + req + "'";
			using (MySqlCommand command = new MySqlCommand(sql, MyConn))
			{ 
				command.ExecuteNonQuery();
			}
		}

		public string SearchKey(string src)
		{
			string KEY = null;
			string sql = "SELECT * FROM Request";
			List<string[]> L = new List<string[]>();
			Dictionary<string, int> Count = new Dictionary<string, int>();
			using (MySqlCommand command = new MySqlCommand(sql, MyConn))
			{
				MySqlDataReader reader = command.ExecuteReader();
				while (reader.Read())
				{
					string[] row = { reader.GetValue(0).ToString(), reader.GetValue(1).ToString() };
					L.Add(row);
				}
			}
			foreach (var row in L)
				if (src.Contains(row[1]))
				{
					string k = row[0];
					if (Count.ContainsKey(k)) Count[k]++;
					else Count.Add(k, row[1].Length);
				}
			if (Count.ContainsKey("пока")) KEY = "пока";
			else if (Count.ContainsKey("какдела")) KEY = "какдела";
			else if (Count.ContainsKey("привет")) KEY = "привет";
			if (!Count.Any()) KEY = "чтосказать";
			if (KEY != null) return KEY;
			foreach (var x in Count) if (KEY == null || Count[KEY] < x.Value) KEY = x.Key;
			return KEY;
		}
	}
}
