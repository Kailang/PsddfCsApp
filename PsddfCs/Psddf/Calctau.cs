namespace PsddfCs {
	public partial class Psddf {
		/// <summary>
		/// Calculate the tau(the time step) variable when it is not given for the user.
		/// Only called in Intro.
		/// </summary>
		void Calctau (int d1) {
			int i, j, nbd, jin = 0, k, l, temp, id;
			double dzz, efs, dabl, v = 0, tdz, taubl, taudf, sum;
			double[] cent_eff = new double[MaxMaterialTypes], cent_eff_ocr = new double[MaxMaterialTypes], cent_e_ocr = new double[MaxMaterialTypes];

			taubl = 1.0e30;
			taudf = 1.0e30;

			// Calculate dz;
			for (i = 1; i <= DredgedFillLayers; i++) {
				dz[i] = DredgedFillInitialThicknesses[i] / ((1 + DredgedFillInitialVoidRatios[i]) * DredgedFillSublayers[i]);
			}

			if (IsFoundationCompressible != 2 && IsNewSimulation == 1) {
				// Check For Compressible Foundation;
				dzz = 0.0;
				efs = 0.0;
				k = 0;
				for (i = 1; i <= CompressibleFoundationLayers; i++) {
					k = k + CompressibleFoundationSublayers[i];
				}
				k = k + CompressibleFoundationLayers;

				l = 0;
				for (i = CompressibleFoundationLayers; i >= 1; i -= 1) {
					nbd = 10 * CompressibleFoundationSublayers[i];
					dabl = CompressibleFoundationInitialThicknesses[i] / nbd;
					sum = 0;
					for (j = 1; j <= nbd; j++) {
						l = l + 1;
						Intpl(d1, RelationDefinitionLines, efs, EffectiveStresses, VoidRatios, ref v, ref jin, CompressibleFoundationMaterialIDs[i], auxbl, k);
						tdz = dabl / (1.0 + v);

						// in case of under consolidated compressible foundation;
						if (CompressibleFoundationOCR[i] < 1.0) {
							if (j <= intx(nbd / 2)) {
								efs = efs + SoilBuoyantUnitWeight[CompressibleFoundationMaterialIDs[i]] * tdz * CompressibleFoundationOCR[i];
								sum = sum + efs;
							} else {
								efs = efs + SoilBuoyantUnitWeight[CompressibleFoundationMaterialIDs[i]] * tdz * (2 - CompressibleFoundationOCR[i]);
								sum = sum + efs;
							}
						} else {
							efs = efs + SoilBuoyantUnitWeight[CompressibleFoundationMaterialIDs[i]] * tdz;
							sum = sum + efs;
						}
						dzz = dzz + tdz;
						if (l == 10) {
							l = 0;
							k = k - 1;
						}
					}
					dz1[i] = dzz / CompressibleFoundationSublayers[i];
					dzz = 0.0;
					cent_eff[i] = sum / nbd;
					if (CompressibleFoundationOCR[i] > 1) {
						cent_eff_ocr[i] = CompressibleFoundationOCR[i] * cent_eff[i];
						Intpl(d1, RelationDefinitionLines, cent_eff_ocr[i], EffectiveStresses, VoidRatios, ref cent_e_ocr[i], ref jin, CompressibleFoundationMaterialIDs[i], auxbl, i);
					}
				}

				temp = 1;
				temp_id = 100;
				// ntypedredge + ntypescompress;
				for (i = 1; i <= CompressibleFoundationLayers; i++) {
					id = CompressibleFoundationMaterialIDs[i];
					if (CompressibleFoundationOCR[i] > 1) {
						temp_id = 100 + temp;
						// ntypedredge + ntypescompress + temp;
						for (j = 1; j <= RelationDefinitionLines[id]; j++) {
							EffectiveStresses[j, temp_id] = EffectiveStresses[j, id];
							Permeabilities[j, temp_id] = Permeabilities[j, id];
							if (cent_eff_ocr[i] > EffectiveStresses[j, id]) {
								VoidRatios[j, temp_id] = CrCcs[id] * (VoidRatios[j, id] - cent_e_ocr[i]) + cent_e_ocr[i];
							} else {
								VoidRatios[j, temp_id] = VoidRatios[j, id];
							}
						}
						SpecificGravities[temp_id] = SpecificGravities[id];
						SoilUnitWeight[temp_id] = SoilUnitWeight[id];
						SoilBuoyantUnitWeight[temp_id] = SoilBuoyantUnitWeight[id];
						CaCcs[temp_id] = CaCcs[id];
						CrCcs[temp_id] = CrCcs[id];
						RelationDefinitionLines[temp_id] = RelationDefinitionLines[id];
						CompressibleFoundationMaterialIDs[i] = temp_id;
						indi_id[temp_id] = id;
						temp = temp + 1;
					}
				}

				dzz = 0.0;
				efs = 0.0;
				k = 0;
				for (i = 1; i <= CompressibleFoundationLayers; i++) {
					k = k + CompressibleFoundationSublayers[i];
				}
				k = k + CompressibleFoundationLayers;
				l = 0;

				for (i = CompressibleFoundationLayers; i >= 1; i -= 1) {
					nbd = 10 * CompressibleFoundationSublayers[i];
					dabl = CompressibleFoundationInitialThicknesses[i] / nbd;
					for (j = 1; j <= nbd; j++) {
						l = l + 1;
						Intpl(d1, RelationDefinitionLines, efs, EffectiveStresses, VoidRatios, ref v, ref jin, CompressibleFoundationMaterialIDs[i], auxbl, k);
						tdz = dabl / (1.0 + v);

						// in case of under consolidated compressible foundation;
						if (CompressibleFoundationOCR[i] < 1.0) {
							if (j <= intx(nbd / 2)) {
								efs = efs + SoilBuoyantUnitWeight[CompressibleFoundationMaterialIDs[i]] * tdz * CompressibleFoundationOCR[i];
							} else {
								efs = efs + SoilBuoyantUnitWeight[CompressibleFoundationMaterialIDs[i]] * tdz * (2 - CompressibleFoundationOCR[i]);
							}
						} else {
							efs = efs + SoilBuoyantUnitWeight[CompressibleFoundationMaterialIDs[i]] * tdz;
						}

						dzz = dzz + tdz;
						if (l == 10) {
							l = 0;
							k = k - 1;
						}
					}
					dz1[i] = dzz / CompressibleFoundationSublayers[i];
					dzz = 0.0;
					layer_stress[i] = efs;
				}
				efsbot = efs;
			}

			Pfunc();

			// This will find the tau for the dredgefill;
			taudf = Mintau(d1, Alpha, DredgedFillLayers, DredgedFillMaterialIDs, dz);

			if (IsFoundationCompressible != 2) {
				// Check for Compressible Foundation;
				// This will find the tau for the compressible foundation;
				taubl = Mintau(d1, Alpha, CompressibleFoundationLayers, CompressibleFoundationMaterialIDs, dz1);
			}

			if (taubl < taudf) {
				TimeStep = taubl * 0.99;
			} else {
				TimeStep = taudf * 0.99;
			}
		}

		double Mintau (int d1, double[,] alpha, int nlayers, int[] idm, double[] di) {
			int i, j, id;
			double alphamax, taumax, stab;

			taumax = 1000000.0;

			for (i = 1; i <= nlayers; i++) {
				// ntypes;
				id = idm[i];
				// nmat(npunt + i);
				alphamax = 0.0;
				for (j = 1; j <= RelationDefinitionLines[id]; j++) {
					if (abs(alpha[j, id]) > abs(alphamax)) {
						alphamax = alpha[j, id];
					}
				}

				stab = -(pow(di[i], 2) * WaterUnitWeight) / (2 * alphamax);
				if (taumax > stab) {
					taumax = stab;
				}
			}

			return taumax;
		}
	}
}

