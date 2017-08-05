using System;

using PsddfCs.Exception;

namespace PsddfCs {
	public partial class Psddf {
		static double dlog10 (double x) {
			return Math.Log10(x);
		}

		static int intx (double x) {
			return (int)x;
		}

		static double floatx (double x) {
			return x;
		}

		static double abs (double x) {
			return Math.Abs(x);
		}

		static double pow (double x, double y) {
			return Math.Pow(x, y);
		}

		static double[] sub (double[] a, double[] b) {
			int length = a.Length;
			if (length != b.Length)
				throw new SimulationException("Substracting arraies of different size.");

			var c = new double[length];
			Array.Copy(a, c, length);

			for (int i = 0; i < length; i++)
				c[i] -= b[i];
			
			return c;
		}
	}
}

