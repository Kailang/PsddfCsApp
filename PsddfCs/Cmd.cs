using System;

namespace PsddfCs {
	public interface ICmd {
		void Write (object o = null);
		void Write (string format, params object[] args);

		void WriteLine (object o = null);
		void WriteLine (string format, params object[] args);

		void Print (params object[] objs);
	}

	public class Cmd : ICmd {
		public void Write (object o = null) {
			Console.Write(o);
		}

		public void Write (string format, params object[] args) {
			Console.Write(format, args);
		}

		public void WriteLine (object o = null) {
			Console.WriteLine(o);
		}

		public void WriteLine (string format, params object[] args) {
			Console.WriteLine(format, args);
		}

		public void Print (params object[] objs) {
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

