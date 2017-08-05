namespace PsddfCs.Exception {
	public class FileException : System.Exception {
		public FileException (object o) : base(o.ToString()) {
		}

		public FileException (string format, params object[] args) : base(string.Format(format, args)) {
		}
	}
}

