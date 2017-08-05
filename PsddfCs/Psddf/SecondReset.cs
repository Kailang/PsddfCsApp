namespace PsddfCs {
	public partial class Psddf {
		/// <summary>
		/// Reset the values used in the secondary compresion process.
		/// Only called in Desic and Reset.
		/// </summary>
		public void SecondReset (
			int d1, double[,] voidratio, double[,] effectivestress, double[,] pk, double[,] dvds, double[,] dsde, double[,] alpha, double[,] beta, 
			int nlayer, int n_point) {

			double voidx, cd, den, temp1 = 0, temp2 = 0, temp3 = 0, incre, alphamax, tautemp;
			int i, j, k, l, id, jin = 0;

			alphamax = 0.0;
			tautemp = 1.0e9;
			DredgedFillSublayers[nlayer] = DredgedFillSublayers[nlayer] - (n_point - ndfcons);

			if (IsFoundationCompressible != 2) {
				// Check for Compressible Foundation;
				l = 1;
				for (k = 1; k <= CompressibleFoundationLayers; k++) {
					id = CompressibleFoundationMaterialIDs[k];
					if (!IsCompressibleFoundationInPrimaryConsolidations[k]) {
						if (k == 1) {
							dudz11 = 0.0;
						}

						IsCompressibleFoundationInPrimaryConsolidations[k] = true;
						incre = pow((CurrentTime / tpbl[k]), (CaCcs[id] / (1 - CrCcs[id])));
						for (j = 0; j <= CompressibleFoundationSublayers[k]; j++) {
							i = l + j;
							Intpgg(d1, RelationDefinitionLines, CompressibleFoundationCurrentVoidRatio[i], voidratio, effectivestress, effectivestress, ref temp1, ref temp1, ref jin, id, auxbl, i, 6, 6);
							auxbl[1, i] = CompressibleFoundationCurrentVoidRatio[i];
							auxbl[5, i] = auxbl[4, i] * incre;
							if (temp1 >= auxbl[5, i]) {
								auxbl[5, i] = temp1 * 1.005;
							}

							Intpl(d1, RelationDefinitionLines, auxbl[5, i], effectivestress, voidratio, ref auxbl[2, i], ref jin, id, auxbl, i);
							if (auxbl[5, i] > auxbl[6, i]) {
								auxbl[6, i] = auxbl[5, i];
								auxbl[3, i] = auxbl[2, i];
							}
							auxbl[13, i] = (auxbl[5, i] - auxbl[4, i]) / (auxbl[2, i] - auxbl[1, i]);

							Intpgg(d1, RelationDefinitionLines, auxbl[3, i], voidratio, dsde, dvds, ref auxbl[15, i], ref auxbl[12, i], ref jin, id, auxbl, i, 0, 0);

							Intpgg(d1, RelationDefinitionLines, auxbl[1, i], voidratio, pk, pk, ref temp1, ref temp1, ref jin, id, auxbl, i, 0, 0);
							Intpgg(d1, RelationDefinitionLines, auxbl[3, i], voidratio, pk, pk, ref temp3, ref temp3, ref jin, id, auxbl, i, 0, 0);
							cd = auxbl[2, i] - auxbl[1, i];

							den = dlog10(auxbl[5, i]) - dlog10(auxbl[4, i]);

							auxbl[10, i] = cd / den;

							if (auxbl[3, i] != auxbl[2, i]) {
								voidx = (auxbl[6, i] - auxbl[5, i]) / (auxbl[3, i] - auxbl[2, i]);
								auxbl[15, i] = (voidx + auxbl[15, i]) * 0.5;
								auxbl[14, i] = 0.5 * (auxbl[13, i] + voidx);
								Intpgg(d1, RelationDefinitionLines, auxbl[2, i], voidratio, pk, pk, ref temp2, ref temp2, ref jin, id, auxbl, i, 0, 0);
								cd = auxbl[3, i] - auxbl[2, i];

								den = dlog10(auxbl[6, i]) - dlog10(auxbl[5, i]);

								voidx = cd / den;
								auxbl[12, i] = (voidx + auxbl[12, i]) * 0.5;
								auxbl[11, i] = 0.5 * (auxbl[10, i] + voidx);
							} else {
								auxbl[15, i] = (auxbl[15, i] + auxbl[13, i]) * 0.5;
								auxbl[14, i] = auxbl[15, i];
								temp2 = temp3;
								auxbl[12, i] = 0.5 * (auxbl[12, i] + auxbl[10, i]);
								auxbl[11, i] = auxbl[12, i];
							}

							auxbl[7, i] = temp1 * auxbl[13, i];
							auxbl[8, i] = temp2 * auxbl[14, i];
							auxbl[9, i] = temp3 * auxbl[15, i];
							if (abs(auxbl[7, i]) > alphamax) {
								alphamax = abs(auxbl[7, i]);
							}
							if (abs(auxbl[8, i]) > alphamax) {
								alphamax = abs(auxbl[8, i]);
							}
							if (abs(auxbl[9, i]) > alphamax) {
								alphamax = abs(auxbl[9, i]);
							}
							tautemp = (pow(dz1[k], 2) * WaterUnitWeight) / (2 * alphamax);
							if (tautemp < tau) {
								tau = 0.99 * tautemp;
							}
						}
					}
					l = l + CompressibleFoundationSublayers[k] + 1;
				}
			}

			l = 1;
			for (k = 1; k <= nlayer; k++) {
				id = DredgedFillMaterialIDs[k];
				if (!IsDredgedFillInPrimaryConsolidations[k]) {
					if (k == 1) {
						dudz21 = 0.0;
					}
					IsDredgedFillInPrimaryConsolidations[k] = true;
					incre = pow((CurrentTime / tpdf[k]), (CaCcs[id] / (1 - CrCcs[id])));
					for (j = 0; j <= DredgedFillSublayers[k]; j++) {
						i = l + j;
						Intpgg(d1, RelationDefinitionLines, DredgedFillCurrentVoidRatio[i], voidratio, effectivestress, effectivestress, ref temp1, ref temp1, ref jin, id, auxdf, i, 6, 6);
						auxdf[1, i] = DredgedFillCurrentVoidRatio[i];
						auxdf[5, i] = auxdf[4, i] * incre;

						if (temp1 >= auxdf[5, i]) {
							auxdf[5, i] = temp1 * 1.005;
						}

						Intpl(d1, RelationDefinitionLines, auxdf[5, i], effectivestress, voidratio, ref auxdf[2, i], ref jin, id, auxdf, i);
						if (auxdf[5, i] > auxdf[6, i]) {
							auxdf[6, i] = auxdf[5, i];
							auxdf[3, i] = auxdf[2, i];
						}

						auxdf[13, i] = (auxdf[5, i] - auxdf[4, i]) / (auxdf[2, i] - auxdf[1, i]);
						Intpgg(d1, RelationDefinitionLines, auxdf[3, i], voidratio, dsde, dvds, ref auxdf[15, i], ref auxdf[12, i], ref jin, id, auxdf, i, 0, 0);

						Intpgg(d1, RelationDefinitionLines, auxdf[1, i], voidratio, pk, pk, ref temp1, ref temp1, ref jin, id, auxdf, i, 0, 0);
						Intpgg(d1, RelationDefinitionLines, auxdf[3, i], voidratio, pk, pk, ref temp3, ref temp3, ref jin, id, auxdf, i, 0, 0);
						cd = auxdf[2, i] - auxdf[1, i];

						den = dlog10(auxdf[5, i]) - dlog10(auxdf[4, i]);

						auxdf[10, i] = cd / den;

						if (auxdf[3, i] != auxdf[2, i]) {
							voidx = (auxdf[6, i] - auxdf[5, i]) / (auxdf[3, i] - auxdf[2, i]);
							auxdf[15, i] = (voidx + auxdf[15, i]) * 0.5;
							auxdf[14, i] = 0.5 * (auxdf[13, i] + voidx);
							Intpgg(d1, RelationDefinitionLines, auxdf[2, i], voidratio, pk, pk, ref temp2, ref temp2, ref jin, id, auxdf, i, 0, 0);
							cd = auxdf[3, i] - auxdf[2, i];

							den = dlog10(auxdf[6, i]) - dlog10(auxdf[5, i]);

							voidx = cd / den;
							auxdf[12, i] = (voidx + auxdf[12, i]) * 0.5;
							auxdf[11, i] = 0.5 * (auxdf[10, i] + voidx);
						} else {
							auxdf[15, i] = (auxdf[15, i] + auxdf[13, i]) * 0.5;
							auxdf[14, i] = auxdf[15, i];
							temp2 = temp3;
							auxdf[12, i] = 0.5 * (auxdf[12, i] + auxdf[10, i]);
							auxdf[11, i] = auxdf[12, i];
						}

						auxdf[7, i] = temp1 * auxdf[13, i];
						auxdf[8, i] = temp2 * auxdf[14, i];
						auxdf[9, i] = temp3 * auxdf[15, i];
						if (abs(auxdf[7, i]) > alphamax) {
							alphamax = abs(auxdf[7, i]);
						}
						if (abs(auxdf[8, i]) > alphamax) {
							alphamax = abs(auxdf[8, i]);
						}
						if (abs(auxdf[9, i]) > alphamax) {
							alphamax = abs(auxdf[9, i]);
						}
						tautemp = (pow(dz[k], 2) * WaterUnitWeight) / (2 * alphamax);
						if (tautemp < tau) {
							tau = 0.99 * tautemp;
						}
					}
					Integral(DredgedFillCurrentVoidRatio, dz, DredgedFillSublayers, fint, nlayer, 0);

					difsecdf[k] = 0.0;
				}

				l = l + DredgedFillSublayers[k] + 1;
			}
			DredgedFillSublayers[nlayer] = DredgedFillSublayers[nlayer] + (n_point - ndfcons);

			Vrfunc(d1);
		}
	}
}

