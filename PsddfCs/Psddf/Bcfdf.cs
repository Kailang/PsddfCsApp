namespace PsddfCs {
	public partial class Psddf {
		void Bcfdf (int d1, double[,] voidratio, double[,]effectivestress, double[,] pk, ref int jin) {
			int id;
			double pk_upper = 0, pk_lower = 0, voidx = 0, const_k, u_inter, estress_int;

			// Compute K / (1 + e) for upper;
			id = DredgedFillMaterialIDs[1];
			Intpgg(d1, RelationDefinitionLines, f[1], voidratio, pk, pk, ref pk_upper, ref voidx, ref jin, id, auxdf, 1, 6, 6);
			// Compute K / (1 + e) for lower;
			id = CompressibleFoundationMaterialIDs[CompressibleFoundationLayers];
			Intpgg(d1, RelationDefinitionLines, f1[CompressibleFoundationTotalSublayers], voidratio, pk, pk, ref pk_lower, ref voidx, ref jin, id, auxbl, CompressibleFoundationTotalSublayers, 6, 6);
			// Compute constant K;
			const_k = dz[1] * pk_lower / (pk_upper * dz1[CompressibleFoundationLayers]);
			// Compute exess pore pressure at interface;
			u_inter = (DredgedFillExcessPoreWaterPressure[2] + const_k * CompressibleFoundationExcessPoreWaterPressure[CompressibleFoundationTotalSublayers - 1]) / (1 + const_k);
			// Compute eff. stress @ interface;
			estress_int = DredgedFillEffectiveStress[1] + DredgedFillExcessPoreWaterPressure[1] - u_inter;
			if (estress_int < 0.0) {
				estress_int = DredgedFillEffectiveStress[1];
			}
			DredgedFillExcessPoreWaterPressure[1] = u_inter;
			CompressibleFoundationExcessPoreWaterPressure[CompressibleFoundationTotalSublayers] = DredgedFillExcessPoreWaterPressure[1];
			DredgedFillEffectiveStress[1] = estress_int;

			// Compute void ratios @ interface;
			Intpl(d1, RelationDefinitionLines, estress_int, effectivestress, voidratio, ref CompressibleFoundationCurrentVoidRatio[CompressibleFoundationTotalSublayers], ref jin, id, auxbl, CompressibleFoundationTotalSublayers);
			id = DredgedFillMaterialIDs[1];
			Intpl(d1, RelationDefinitionLines, estress_int, effectivestress, voidratio, ref DredgedFillCurrentVoidRatio[1], ref jin, id, auxdf, 1);

			if (DredgedFillCurrentVoidRatio[1] > f[1]) {
				DredgedFillCurrentVoidRatio[1] = f[1];
			}

			if (CompressibleFoundationCurrentVoidRatio[CompressibleFoundationTotalSublayers] > f1[CompressibleFoundationTotalSublayers]) {
				CompressibleFoundationCurrentVoidRatio[CompressibleFoundationTotalSublayers] = f1[CompressibleFoundationTotalSublayers];
			}

			if (DredgedFillCurrentVoidRatio[1] < DredgedFillFinalVoidRatio[1]) {
				DredgedFillCurrentVoidRatio[1] = DredgedFillFinalVoidRatio[1];
			}

			if (CompressibleFoundationCurrentVoidRatio[CompressibleFoundationTotalSublayers] < CompressibleFoundationFinalVoidRatio[CompressibleFoundationTotalSublayers]) {
				CompressibleFoundationCurrentVoidRatio[CompressibleFoundationTotalSublayers] = CompressibleFoundationFinalVoidRatio[CompressibleFoundationTotalSublayers];
			}
		}
	}
}

