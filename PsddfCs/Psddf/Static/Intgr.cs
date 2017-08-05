namespace PsddfCs {
	public partial class Psddf {
		/// <summary>
		/// Evaluates the void ratio integral to each mesh point in the material.
		/// Only f is modified.
		/// </summary>
		public static void Intgr (double[] e, double dz, int n, double[] f, double acum, int index) {
			int i, nn, nnn;

			// ... by Simpson rule for all odd numbered mesh points;
			nnn = n + 1;
			f[index] = acum;

			for (nn = 3; nn <= nnn; nn += 2) {
				i = nn + index - 1;
				f[i] = f[i - 2] + dz * (e[i - 2] + 4 * e[i - 1] + e[i]) / 3;
			}

			// ... by Simpson 3/8 rule for even numbered mesh points;
			for (nn = 4; nn <= nnn; nn += 2) {
				i = nn + index - 1;
				f[i] = f[i - 3] + dz * (e[i - 3] + 3 * (e[i - 2] + e[i - 1]) + e[i]) * 3 / 8;
			}

			f[index + 1] = f[index + 3] - dz * (e[index + 1] + 4 * e[index + 2] + e[index + 3]) / 3;
		}
	}
}

