using System;

namespace PsddfCs {
	public static class Cmd {
		public static void Write (object o = null) {
			Console.Write(o);
		}

		public static void Write (string format, params object[] args) {
			Console.Write(format, args);
		}

		public static void WriteLine (object o = null) {
			Console.WriteLine(o);
		}

		public static void WriteLine (string format, params object[] args) {
			Console.WriteLine(format, args);
		}

		public static string ReadLine () {
			return Console.ReadLine();
		}

		public static void Print (params object[] objs) {
			foreach (var obj in objs)
				if (obj.GetType().IsArray)
					foreach (var item in (System.Collections.ICollection)obj)
						Console.Write(item + "\t");
				else
					Console.Write(obj + "\t");
			Console.WriteLine();
		}
	}
}

