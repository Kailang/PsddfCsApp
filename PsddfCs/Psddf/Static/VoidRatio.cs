namespace PsddfCs {
	public partial class Psddf {
		/// <summary>
		/// Calculate the void ratio.
		/// </summary>
		public static double VoidRatio (
			double f1, double f2, double f10, double af, double af1, double af2, double dz, double dz2, double cff, double gcid, double bf1) {
		
			double df, df2dz, ac;

			df = (f2 - f10) * 0.5;
			df2dz = (f2 - 2.0 * f1 + f10) / dz;
			ac = (af2 - af1) / dz2;

			return f1 - cff * (df * (gcid * bf1 + ac) + df2dz * af);
		}
	}
}

