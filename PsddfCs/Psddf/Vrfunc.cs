namespace PsddfCs {
	public partial class Psddf {
		/// <summary>
		/// Calculate alpha and beta functions for current void ratios.
		/// </summary>
		public void Vrfunc (int d1) {
			int i, j, k, kk = 0, id, jin;
		
			jin = 2;
			k = 0;
			if (IsFoundationCompressible != 2) {
				// Only for compresible fundation;
				for (i = 1; i <= CompressibleFoundationLayers; i++) {
					id = CompressibleFoundationMaterialIDs[i];
					for (j = 1; j <= CompressibleFoundationSublayers[i] + 1; j++) {
						kk = k + j;
						Intpgg(d1, RelationDefinitionLines, CompressibleFoundationCurrentVoidRatio[kk], VoidRatios, alpha, alpha, ref af1[kk], ref af1[kk], ref jin, id, auxbl, kk, 9, 9);
						Intpgg(d1, RelationDefinitionLines, CompressibleFoundationCurrentVoidRatio[kk], VoidRatios, beta, beta, ref bf1[kk], ref bf1[kk], ref jin, id, auxbl, kk, 0, 0);
					}
					k = kk;
				}
			}

			k = 0;

			// For dredged fill;
			DredgedFillSublayers[ndflayer] = DredgedFillSublayers[ndflayer] - (ndfpoint - ndfcons);
			for (i = 1; i <= ndflayer; i++) {
				id = DredgedFillMaterialIDs[i];
				for (j = 1; j <= DredgedFillSublayers[i] + 1; j++) {
					kk = k + j;
					Intpgg(d1, RelationDefinitionLines, DredgedFillCurrentVoidRatio[kk], VoidRatios, alpha, alpha, ref af[kk], ref af[kk], ref jin, id, auxdf, kk, 9, 9);
					Intpgg(d1, RelationDefinitionLines, DredgedFillCurrentVoidRatio[kk], VoidRatios, alpha, beta, ref bf[kk], ref bf[kk], ref jin, id, auxdf, kk, 0, 0);
				}
				k = kk;
			}

			DredgedFillSublayers[ndflayer] = DredgedFillSublayers[ndflayer] + (ndfpoint - ndfcons);
		}
	}
}

