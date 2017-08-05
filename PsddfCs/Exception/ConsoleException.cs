namespace PsddfCs.Exception {
	public class ConsoleException : System.Exception {
		public ConsoleException (object o) : base(o.ToString()) {
		}

		public ConsoleException (string format, params object[] args) : base(string.Format(format, args)) {
		}
	}
}

