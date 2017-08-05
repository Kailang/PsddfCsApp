namespace PsddfCs {
	public partial class Psddf {
		static void FirstPoint (
			int d1, int[] ldf, ref double vr, double[,] voidratio, double[,] dsde, double dudz, double gcid, 
			double f1, double f2, double[] af, int l, double dzz, double cff, double bf1, double efin1, double e11, ref int jin, int id, double[,] aux) {
		
			double dsed = 0, voidx = 0, f10, af1, af2, dz2;

			af1 = af[l];
			af2 = af[l + 1];
			dz2 = 2 * dzz;

			Intpgg(d1, ldf, vr, voidratio, dsde, dsde, ref dsed, ref voidx, ref jin, id, aux, l, 15, 15);

			f10 = f2 + 2 * dzz * (gcid + dudz) / dsed;
			vr = VoidRatio(f1, f2, f10, af1, af1, af2, dzz, dzz, cff, gcid, bf1);

			if (vr > f1) {
				vr = f1;
			}

			if (vr > e11) {
				vr = e11;
			} else if (vr < efin1) {
				vr = efin1;
			}
		}
	}
}

