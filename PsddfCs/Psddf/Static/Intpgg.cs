namespace PsddfCs {
	public partial class Psddf {
		public static void Intpgg (
			int d1, int[] min, double comin, double[,] comp, double[,] eval, double[,] evall, 
			ref double comout, ref double cmoutt, ref int jin, int id, double[,] aux, int ix, int ii, int jj) {

			bool test;
			int lin;
			double com, comm;

			if (comin <= aux[3, ix] || (ii == 0 && jj == 0) || (aux[3, ix] == 0.0)) {
				test = true;
				lin = min[id];
				if (jin >= lin) {
					jin = lin;
				}
				if (jin <= 2) {
					jin = 2;
				}

				while (test) {
					com = comin - comp[jin, id];
					if (com >= 0.0) {
						comm = comin - comp[jin - 1, id];
						if (comm >= 0.0 && jin != 2) {
							jin = jin - 1;
						} else {
							comout = eval[jin, id] + com * (eval[jin, id] - eval[jin - 1, id]) / (comp[jin, id] - comp[jin - 1, id]);
							cmoutt = evall[jin, id] + com * (evall[jin, id] - evall[jin - 1, id]) / (comp[jin, id] - comp[jin - 1, id]);
							test = false;
						}
					} else if (jin != lin) {
						jin = jin + 1;
					} else {
						comout = eval[jin, id];
						cmoutt = evall[jin, id];
						test = false;
					}
				}

			} else if (comin >= aux[2, ix]) {
				com = comin - aux[2, ix];
				comout = aux[ii - 1, ix] + com * (aux[ii - 1, ix] - aux[ii - 2, ix]) / (aux[2, ix] - aux[1, ix]);
				cmoutt = aux[jj - 1, ix] + com * (aux[jj - 1, ix] - aux[jj - 2, ix]) / (aux[2, ix] - aux[1, ix]);
			} else {
				com = comin - aux[3, ix];
				comout = aux[ii, ix] + com * (aux[ii, ix] - aux[ii - 1, ix]) / (aux[3, ix] - aux[2, ix]);
				cmoutt = aux[jj, ix] + com * (aux[jj, ix] - aux[jj - 1, ix]) / (aux[3, ix] - aux[2, ix]);
			}
		}
	}
}

