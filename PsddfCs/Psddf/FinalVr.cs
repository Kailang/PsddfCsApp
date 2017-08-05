namespace PsddfCs {
	public partial class Psddf {
		/// <summary>
		/// Called in Setup, Reset, and Desic.
		/// </summary>
		void FinalVr (
			int d1, double[,] voidratio, double[,] effectivestress, double[] ef, double[] dzz, 
			int[] nsubcap, int ncap, int[] idm, int nnodes, ref double top_stress, int ndesicl, ref int jin, double[,] aux) {

			int i, j, k, id;
			double efs;

			nsubcap[ncap] = nsubcap[ncap] - ndesicl;
			k = nnodes - ndesicl;
			for (i = ncap; i >= 1; i -= 1) {
				id = idm[i];
				efs = top_stress;
				for (j = 1; j <= nsubcap[i] + 1; j++) {
					Intpl(d1, RelationDefinitionLines, efs, effectivestress, voidratio, ref ef[k - j + 1], ref jin, id, aux, k - j + 1);
					efs = efs + gc[id] * dzz[i];
				}
				k = k - j + 1;
				top_stress = top_stress + gc[id] * dzz[i] * nsubcap[i];
			}
			nsubcap[ncap] = nsubcap[ncap] + ndesicl;
		}
	}
}

