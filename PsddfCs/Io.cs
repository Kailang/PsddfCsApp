using System;
using System.IO;
using System.Text;

using PsddfCs.Exception;

namespace PsddfCs {
	public interface IIo {
		#region Read

		void OpenRead (int i, string fileName);

		void CloseRead (int i);

		bool EndOfFile (int i);

		bool StreamReadBool (int i);

		int StreamReadInt (int i);

		double StreamReadDouble (int i);

		int ReadInt (int i);

		double ReadDouble (int i);

		string ReadString (int i);

		#endregion

		#region Write

		void OpenWrite (int i, string fileName);

		void CloseWrite (int i);

		void StreamWrite (int i, bool val);

		void StreamWrite (int i, int val);

		void StreamWrite (int i, double val);

		void StreamWrite (int i, params object[] objs);

		void Write (int i, string str);

		void Write (int i, string format, params object[] args);

		void WriteLine (int i, string format, params object[] args);

		void Print (int i, params object[] objs);

		void PrintLine (int i, params object[] objs);

		#endregion
	}

	public class Io : IIo {
		#region Read

		readonly StreamReader[] readers = new StreamReader[100];

		public void OpenRead (int i, string fileName) {
//			Cmd.WriteLine("Io Open Read     {0, 2} == {1}", i, fileName);
			readers[i] = File.OpenText(fileName);
		}

		public void CloseRead (int i) {
			readers[i].Close();
			readers[i] = null;
		}

		public bool EndOfFile (int i) {
			EatWhitespace(i);

			return readers[i].Peek() < 0;
		}

		public bool StreamReadBool (int i) {
			return readers[i].BaseStream.ReadByte() == 1;
		}

		public int StreamReadInt (int i) {
			var bytes = new [] {
				StreamRead(i),
				StreamRead(i),
				StreamRead(i),
				StreamRead(i)
			};

			return BitConverter.ToInt32(bytes, 0);
		}

		public double StreamReadDouble (int i) {
			var bytes = new [] {
				StreamRead(i),
				StreamRead(i),
				StreamRead(i),
				StreamRead(i),
				StreamRead(i),
				StreamRead(i),
				StreamRead(i),
				StreamRead(i)
			};

			return BitConverter.ToDouble(bytes, 0);
		}

		byte StreamRead (int i) {
			var b = readers[i].BaseStream.ReadByte();

			if (b < 0)
				throw new FileException("Trying to read after the end of file.");
			
			return (byte)b;
		}

		public int ReadInt (int i) {
			EatWhitespace(i);

			var sb = new StringBuilder(); 
			while ("0123456789+-".IndexOf(Peek(i)) != -1)
				sb.Append(Read(i));

//			Cmd.WriteLine("Io Read Int      {0, 2} -> {1}", i, sb);
			return int.Parse(sb.ToString());
		}

		public double ReadDouble (int i) {
			EatWhitespace(i);

			var sb = new StringBuilder();
			while ("0123456789+-.eE".IndexOf(Peek(i)) != -1)
				sb.Append(Read(i));

//			Cmd.WriteLine("Io Read Double   {0, 2} -> {1} ({2})", i, sb, double.Parse(sb.ToString()));
			return double.Parse(sb.ToString());
		}

		public string ReadString (int i) {
			EatWhitespace(i);

			if ("'\"".IndexOf(Peek(i)) == -1)
				return "";

			var c = Read(i);

			var sb = new StringBuilder();
			while (Peek(i) != c)
				sb.Append(Read(i));
			
			Read(i);

//			Cmd.WriteLine("Io Read String   {0, 2} -> {1}", i, sb);
			return sb.ToString();
		}

		char Peek (int i) {
			return (char)readers[i].Peek();
		}

		char Read (int i) {
			return (char)readers[i].Read();
		}

		void EatWhitespace (int i) {
			while (" \t\n\r".IndexOf(Peek(i)) != -1)
				Read(i);
		}

		#endregion

		#region Write

		readonly StreamWriter[] writers = new StreamWriter[100];

		public void OpenWrite (int i, string fileName) {
//			Cmd.WriteLine("Io Open Write    {0, 2} == {1}", i, fileName);
			writers[i] = new StreamWriter(fileName);
			writers[i].AutoFlush = true;
		}

		public void CloseWrite (int i) {
			writers[i].Close();
			writers[i] = null;
		}

		public void StreamWrite (int i, bool val) {
			writers[i].BaseStream.Write(new [] { (byte)(val ? 1 : 0) }, 0, 1);
		}

		public void StreamWrite (int i, int val) {
			writers[i].BaseStream.Write(BitConverter.GetBytes(val), 0, 4);
		}

		public void StreamWrite (int i, double val) {
			writers[i].BaseStream.Write(BitConverter.GetBytes(val), 0, 8);
		}

		public void StreamWrite (int i, params object[] objs) {
			foreach (var obj in objs) {
				if (obj is int)
					StreamWrite(i, (int)obj);
				else if (obj is double)
					StreamWrite(i, (double)obj);
				else if (obj is bool)
					StreamWrite(i, (bool)obj);
			}
		}

		public void Write (int i, string str) {
//			Cmd.Write("Io Write String  {0, 2} <- \n{1}", i, str);
			foreach (var s in str)
				writers[i].Write(s);
		}

		public void Write (int i, string format, params object[] args) {
			Write(i, string.Format(format, args));
		}

		public void WriteLine (int i, string format, params object[] args) {
			Write(i, string.Format(format + "\n", args));
		}

		public void Print (int i, params object[] objs) {
			foreach (var obj in objs)
				if (obj.GetType().IsArray)
					foreach (var item in (System.Collections.ICollection)obj)
						Write(i, item + "  \t");
				else
					Write(i, obj + "  ");
		}

		public void PrintLine (int i, params object[] objs) {
			Print(i, objs);
			
			Write(i, "\n");
		}

		#endregion
	}
}

