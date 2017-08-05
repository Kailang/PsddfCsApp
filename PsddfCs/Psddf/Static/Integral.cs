namespace PsddfCs {
	public partial class Psddf {
		/// <summary>
		/// Integral the specified e, dz, nsubcap, f, ncap and ndesicl.
		/// Only f is modified.
		/// </summary>
		static void Integral (double[] e, double[] dz, int[] nsubcap, double[] f, int ncap, int ndesicl) {
			double acum;
			int i, index;

			acum = 0.0;
			index = 1;
			for (i = 1; i <= ncap; i++) {
				Intgr(e, dz[i], nsubcap[i], f, acum, index);
				index = index + nsubcap[i] + 1;
				acum = f[index - 1];
			}

			for (i = 1; i <= ndesicl; i++) {
				f[index - i] = 0.0;
			}
		}
	}
}

