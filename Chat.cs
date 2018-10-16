using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Chat_Simulator
{
	class Chat
	{
		public int LastMsgId, mode, index, RhymeCount;
		public long ID;
		public string[] Arr;
		public string BotPrev, AddNew;
		public bool IsPrivate()
		{
			return ID > 0;
		}
		public void Output()
		{
			Console.WriteLine(ID);
			Console.WriteLine(LastMsgId);
			Console.WriteLine(mode);
			Console.WriteLine(index);
			Console.WriteLine(RhymeCount);
			Console.WriteLine(BotPrev);
			Console.WriteLine(AddNew);
		}
		public bool IsAdmin()
		{
			return ID == Program.ADMIN;
		}

		public void ChangeMode(int newMode)
		{
			if (newMode < 0 || newMode >= Program.ModeCount) return;
			if (mode == newMode) return;
			Console.WriteLine(ID + " changing " + mode + " -> " + newMode);
			mode = newMode;
			Arr = null;
			index = 0;
			string atb = Program.Answering("to" + mode + "mode");
			if (mode != 0 && !IsPrivate()) atb += "\nЧтобы это функция работала в пубичном чате, запрос должен быть ответом на моё сообщение";
			Program.Say(atb, this);
		}
	}
}
