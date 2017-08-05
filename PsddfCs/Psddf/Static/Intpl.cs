using PsddfCs.Exception;

namespace PsddfCs {
	public partial class Psddf {
		/// <summary>
		/// Interpolate the specified d1, min, comin, comp, eval, comout, jin, id, aux and ix.
		/// </summary>
		public static void Intpl (
			int d1, int[] min, double comin, double[,] comp, double[,] eval, 
			ref double comout, ref int jin, int id, double[,] aux, int ix) {

			bool test;
			int lin;
			double com, comm;

			if (comin >= aux[6, ix]) {
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
					if (com <= 0.0) {
						comm = comin - comp[jin - 1, id];
						if (comm <= 0.0 && jin != 2) {
							jin = jin - 1;
						} else {
							comout = eval[jin, id] + com * (eval[jin, id] - eval[jin - 1, id]) / (comp[jin, id] - comp[jin - 1, id]);
							test = false;
						}
					} else if (jin != lin) {
						jin = jin + 1;
					} else {
						comout = eval[jin, id];
						test = false;
					}
				}

				if (comout > eval[1, id] || comout < eval[lin, id]) {
					throw new SimulationException("Interpolation out of range.");
				}
			} else if (comin < aux[5, ix]) {
				com = comin - aux[4, ix];
				comout = aux[2, ix] + com * (aux[2, ix] - aux[1, ix]) / (aux[5, ix] - aux[4, ix]);
			} else {
				com = comin - aux[6, ix];
				comout = aux[3, ix] + com * (aux[3, ix] - aux[2, ix]) / (aux[6, ix] - aux[5, ix]);
			}
		}
	}
}

