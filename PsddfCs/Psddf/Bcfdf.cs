namespace PsddfCs {
	public partial class Psddf {
		public void Bcfdf (int d1, double[,] voidratio, double[,]effectivestress, double[,] pk, ref int jin) {
			int id;
			double pk_upper = 0, pk_lower = 0, voidx = 0, const_k, u_inter, estress_int;

			// Compute K / (1 + e) for upper;
			id = DredgedFillMaterialIDs[1];
			Intpgg(d1, RelationDefinitionLines, f[1], voidratio, pk, pk, ref pk_upper, ref voidx, ref jin, id, auxdf, 1, 6, 6);
			// Compute K / (1 + e) for lower;
			id = CompressibleFoundationMaterialIDs[CompressibleFoundationLayers];
			Intpgg(d1, RelationDefinitionLines, f1[nblpoint], voidratio, pk, pk, ref pk_lower, ref voidx, ref jin, id, auxbl, nblpoint, 6, 6);
			// Compute constant K;
			const_k = dz[1] * pk_lower / (pk_upper * dz1[CompressibleFoundationLayers]);
			// Compute exess pore pressure at interface;
			u_inter = (u[2] + const_k * u1[nblpoint - 1]) / (1 + const_k);
			// Compute eff. stress @ interface;
			estress_int = effstr[1] + u[1] - u_inter;
			if (estress_int < 0.0) {
				estress_int = effstr[1];
			}
			u[1] = u_inter;
			u1[nblpoint] = u[1];
			effstr[1] = estress_int;

			// Compute void ratios @ interface;
			Intpl(d1, RelationDefinitionLines, estress_int, effectivestress, voidratio, ref er[nblpoint], ref jin, id, auxbl, nblpoint);
			id = DredgedFillMaterialIDs[1];
			Intpl(d1, RelationDefinitionLines, estress_int, effectivestress, voidratio, ref e[1], ref jin, id, auxdf, 1);

			if (e[1] > f[1]) {
				e[1] = f[1];
			}

			if (er[nblpoint] > f1[nblpoint]) {
				er[nblpoint] = f1[nblpoint];
			}

			if (e[1] < efin[1]) {
				e[1] = efin[1];
			}

			if (er[nblpoint] < efin1[nblpoint]) {
				er[nblpoint] = efin1[nblpoint];
			}
		}
	}
}

