namespace PsddfCs {
	public partial class Psddf {
		/// <summary>
		/// Calculate functions for dredged fill and compresible foundation.
		/// Only called in Calctau.
		/// </summary>
		public void Pfunc () {
			int kk, k, i, l, ii, ij;
			double cd, den;

			for (kk = 1; kk <= CompressibleFoundationLayers + DredgedFillLayers; kk++) {
				if (kk <= CompressibleFoundationLayers) {
					k = CompressibleFoundationMaterialIDs[kk];
				} else {
					k = DredgedFillMaterialIDs[kk - CompressibleFoundationLayers];
				}

				for (i = 1; i <= RelationDefinitionLines[k]; i++) {
					pk[i, k] = Permeabilities[i, k] / (1.0 + VoidRatios[i, k]);
				}

				cd = VoidRatios[2, k] - VoidRatios[1, k];
				beta[1, k] = (pk[2, k] - pk[1, k]) / cd;
				dsde[1, k] = (EffectiveStresses[2, k] - EffectiveStresses[1, k]) / cd;
				EffectiveStresses[1, k] = EffectiveStresses[2, k] / 100;

				den = dlog10(EffectiveStresses[2, k]) - dlog10(EffectiveStresses[1, k]);

				dvds[1, k] = cd / den;
				l = RelationDefinitionLines[k] - 1;

				for (i = 2; i <= l; i++) {
					ii = i - 1;
					ij = i + 1;
					cd = VoidRatios[ij, k] - VoidRatios[ii, k];
					den = dlog10(EffectiveStresses[ij, k]) - dlog10(EffectiveStresses[ii, k]);

					if (ii == 1) {
						EffectiveStresses[1, k] = 0.0;
					}

					beta[i, k] = (pk[ij, k] - pk[ii, k]) / cd;
					dsde[i, k] = (EffectiveStresses[ij, k] - EffectiveStresses[ii, k]) / cd;
					dvds[i, k] = cd / den;
				}

				ii = RelationDefinitionLines[k];
				cd = VoidRatios[ii, k] - VoidRatios[l, k];

				den = dlog10(EffectiveStresses[ii, k]) - dlog10(EffectiveStresses[l, k]);

				beta[ii, k] = (pk[ii, k] - pk[l, k]) / cd;
				dsde[ii, k] = (EffectiveStresses[ii, k] - EffectiveStresses[l, k]) / cd;
				dvds[ii, k] = cd / den;

				for (i = 1; i <= RelationDefinitionLines[k]; i++) {
					alpha[i, k] = pk[i, k] * dsde[i, k];
				}
			}
		}
	}
}

