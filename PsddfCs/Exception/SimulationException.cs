namespace PsddfCs.Exception {
	public class SimulationException : System.Exception {
		public SimulationException (object o) : base(o.ToString()) {
		}

		public SimulationException (string format, params object[] args) : base(string.Format(format, args)) {
		}
	}
}

