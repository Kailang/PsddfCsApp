namespace PsddfCs {
	public partial class Psddf {
		static void Boundary (
			int d1, int[] ldf, int l, int k, 
			double[] e, double[] f, double[] efin, double[] dz, double[] u, 
			double[,] pk, double[,] voidratio, double[,] effectivestress, double[] eff, 
			int[] iddf, ref int jin, double[,] aux) {

			int id;
			double pk_upper = 0, pk_lower = 0, voidx = 0, const_k, u_inter, estress_int;

			// Compute K / (1 + e) for upper;
			id = iddf[k];
			Intpgg(d1, ldf, f[l], voidratio, pk, pk, ref pk_upper, ref voidx, ref jin, id, aux, l, 6, 6);
			// Compute K / (1 + e) for lower;
			id = iddf[k - 1];
			Intpgg(d1, ldf, f[l - 1], voidratio, pk, pk, ref pk_lower, ref voidx, ref jin, id, aux, l - 1, 6, 6);
			// Compute constant K;
			const_k = dz[k] * pk_lower / (pk_upper * dz[k - 1]);
			// Compute exess pore pressure at interface;
			u_inter = (u[l + 1] + const_k * u[l - 2]) / (1 + const_k);
			// Compute eff. stress @ interface;
			estress_int = eff[l] + u[l] - u_inter;
			if (estress_int < 0.0) {
				estress_int = eff[l];
			}
			u[l] = u_inter;
			u[l - 1] = u[l];
			eff[l] = estress_int;

			// Compute void ratios @ interface;
			Intpl(d1, ldf, estress_int, effectivestress, voidratio, ref e[l - 1], ref jin, id, aux, l - 1);
			id = iddf[k];
			Intpl(d1, ldf, estress_int, effectivestress, voidratio, ref e[l], ref jin, id, aux, l);

			if (e[l] > f[l]) {
				e[l] = f[l];
			}
			if (e[l - 1] > f[l - 1]) {
				e[l - 1] = f[l - 1];
			}

			if (e[l] < efin[l]) {
				e[l] = efin[l];
			}

			if (e[l - 1] < efin[l - 1]) {
				e[l - 1] = efin[l - 1];
			}
		}
	}
}