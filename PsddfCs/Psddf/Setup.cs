namespace PsddfCs {
	public partial class Psddf {
		/// <summary>
		/// Only called in Intro.
		/// </summary>
		void Setup (int d1) {
			int jin, i, ii, id, k, ij, j, kk = 0, para;
			double efs, h2mx, dz2, g1, w1, hsa, wl1, efsdl = 0, elev, overpress, voidx = 0, totstress;

			jin = 2;
			CompressibleFoundationTotalSublayers = 0;

			for (i = 1; i <= CompressibleFoundationLayers; i++) {
				CompressibleFoundationTotalSublayers = CompressibleFoundationTotalSublayers + CompressibleFoundationSublayers[i];
			}

			CompressibleFoundationTotalSublayers = CompressibleFoundationTotalSublayers + CompressibleFoundationLayers;

			if (IsNewSimulation == 1) {
				// ndff = 1 when it is not a restart;
				// Set constants;
				DredgedFillTotalSublayers = DredgedFillSublayers[1] + 1;
				ndfcons = DredgedFillTotalSublayers;
				pk0 = IncompressibleFoudationPermeability / (1.0 + IncompressibleFoudationVoidRatio);
				// Kailang: Record du0;
				OriginalDU0 = IncompressibleFoudationDrainagePathLength;
				IncompressibleFoudationDrainagePathLength = IncompressibleFoudationDrainagePathLength / (1.0 + IncompressibleFoudationVoidRatio);

				if (IsFoundationCompressible != 2) {
					// Checks For Compressible Foundation (1 = Comprssible) (2 = Incompressible);
					// Calculate initial coordinates and void ratios;
					// for compresible foundation layer;
					k = 0;
					totstress = 0.0;
					CompressibleFoundationSublayers[CompressibleFoundationLayers] = CompressibleFoundationSublayers[CompressibleFoundationLayers] - 1;
					for (i = 1; i <= CompressibleFoundationLayers; i++) {
						id = CompressibleFoundationMaterialIDs[i];
						efs = efsbot - totstress;
						ii = CompressibleFoundationSublayers[i] + 1;
						// nsub1[i] is odd or even number;
						para = 1;
						for (j = 1; j <= CompressibleFoundationSublayers[i]; j++) {
							if (para == 1) {
								para = 2;
							} else {
								para = 1;
							}
						}

						for (j = 1; j <= ii; j++) {
							ij = k + j;
							Intpl(d1, RelationDefinitionLines, efs, EffectiveStresses, VoidRatios, ref CompressibleFoundationInitialVoidRatio[ij], ref jin, id, auxbl, ij);
							f1[ij] = CompressibleFoundationInitialVoidRatio[ij];
							CompressibleFoundationCurrentVoidRatio[ij] = CompressibleFoundationInitialVoidRatio[ij];

							// in case of under consolidated compressible foundation;
							if (CompressibleFoundationOCR[i] < 1.0) {
								if (i != CompressibleFoundationLayers) {
									if (para == 1) {
										// nsub1[i] is even number;
										if (j <= intx(CompressibleFoundationSublayers[i] / 2)) {
											efs = efs - SoilBuoyantUnitWeight[id] * dz1[i] * (2 - CompressibleFoundationOCR[i]) - 0.5 * (layer_stress[i] + layer_stress[i + 1]) * CompressibleFoundationOCR[i] / CompressibleFoundationSublayers[i];
										} else {
											efs = efs - SoilBuoyantUnitWeight[id] * dz1[i] * CompressibleFoundationOCR[i] + 0.5 * (layer_stress[i] + layer_stress[i + 1]) * CompressibleFoundationOCR[i] / CompressibleFoundationSublayers[i];
										}
									} else if (para == 2) {
										// nsub1[i] is odd number;
										if (j <= intx(CompressibleFoundationSublayers[i] / 2)) {
											efs = efs - SoilBuoyantUnitWeight[id] * dz1[i] * (2 - CompressibleFoundationOCR[i]) - 0.5 * (layer_stress[i] + layer_stress[i + 1]) * CompressibleFoundationOCR[i] / CompressibleFoundationSublayers[i];
										} else if (j == intx(CompressibleFoundationSublayers[i] / 2) + 1) {
											efs = efs - SoilBuoyantUnitWeight[id] * dz1[i];
										} else {
											efs = efs - SoilBuoyantUnitWeight[id] * dz1[i] * CompressibleFoundationOCR[i] + 0.5 * (layer_stress[i] + layer_stress[i + 1]) * CompressibleFoundationOCR[i] / CompressibleFoundationSublayers[i];
										}
									}
								} else {
									if (para == 2) {
										// nsub1[numbl] is odd number;
										if (j <= intx(CompressibleFoundationSublayers[i] / 2) + 1) {
											efs = efs - SoilBuoyantUnitWeight[id] * dz1[i] * (2 - CompressibleFoundationOCR[i]);
										} else {
											efs = efs - SoilBuoyantUnitWeight[id] * dz1[i] * CompressibleFoundationOCR[i];
										}
									} else if (para == 1) {
										// nsub1[i] is even number;
										if (j <= intx(CompressibleFoundationSublayers[i] / 2)) {
											efs = efs - SoilBuoyantUnitWeight[id] * dz1[i] * (2 - CompressibleFoundationOCR[i]);
										} else if (j == intx(CompressibleFoundationSublayers[i] / 2) + 1) {
											efs = efs - SoilBuoyantUnitWeight[id] * dz1[i];
										} else {
											efs = efs - SoilBuoyantUnitWeight[id] * dz1[i] * CompressibleFoundationOCR[i];
										}
									}

								}

							} else {
								efs = efs - SoilBuoyantUnitWeight[id] * dz1[i];
							}
						}
						k = k + j - 1;
						// after } j = ii + 1 Fortran's feature (reasong for -1);
						totstress = totstress + SoilBuoyantUnitWeight[id] * dz1[i] * CompressibleFoundationSublayers[i];
					}

					CompressibleFoundationSublayers[CompressibleFoundationLayers] = CompressibleFoundationSublayers[CompressibleFoundationLayers] + 1;
					id = CompressibleFoundationMaterialIDs[CompressibleFoundationLayers];
					CompressibleFoundationInitialVoidRatio[CompressibleFoundationTotalSublayers] = VoidRatios[1, id];
					f1[CompressibleFoundationTotalSublayers] = CompressibleFoundationInitialVoidRatio[CompressibleFoundationTotalSublayers];
					CompressibleFoundationCurrentVoidRatio[CompressibleFoundationTotalSublayers] = CompressibleFoundationInitialVoidRatio[CompressibleFoundationTotalSublayers];

					Integral(CompressibleFoundationCurrentVoidRatio, dz1, CompressibleFoundationSublayers, fint1, CompressibleFoundationLayers, 0);

					CompressibleFoundationCoordZ[1] = 0.0;
					CompressibleFoundationCoordA[1] = 0.0;
					CompressibleFoundationCoordXI[1] = 0.0;
					k = 0;
					for (i = 1; i <= CompressibleFoundationLayers; i++) {
						ii = CompressibleFoundationSublayers[i] + 1;
						for (j = 2; j <= ii; j++) {
							ij = k + j;
							CompressibleFoundationCoordZ[ij] = CompressibleFoundationCoordZ[ij - 1] + dz1[i];
							CompressibleFoundationCoordA[ij] = CompressibleFoundationCoordZ[ij] + fint1[ij];
							CompressibleFoundationCoordXI[ij] = CompressibleFoundationCoordA[ij];
						}
						k = k + j - 1;
						// after } j = ii + 1 Fortran's feature (reasong for -1);
						CompressibleFoundationCoordZ[k + 1] = CompressibleFoundationCoordZ[k];
						CompressibleFoundationCoordA[k + 1] = CompressibleFoundationCoordA[k];
						CompressibleFoundationCoordXI[k + 1] = CompressibleFoundationCoordXI[k];
					}
				}

				// Calculate hsolids for first dredged fill layer;
				vrint = DredgedFillInitialVoidRatios[1] * DredgedFillInitialThicknesses[1] / (1.0 + DredgedFillInitialVoidRatios[1]);
				hsolids = vrint;
				acumel = DredgedFillInitialThicknesses[1] / (1.0 + DredgedFillInitialVoidRatios[1]);

				// Calculate initial coordinates and set void ratios;
				DredgedFillCoordZ[1] = 0.0;
				DredgedFillInitialVoidRatio[1] = DredgedFillInitialVoidRatios[1];
				DredgedFillCoordA[1] = 0.0;
				DredgedFillCoordXI[1] = 0.0;
				f[1] = DredgedFillInitialVoidRatios[1];
				DredgedFillCurrentVoidRatio[1] = DredgedFillInitialVoidRatios[1];
				et[1] = DredgedFillInitialVoidRatios[1];
				da = DredgedFillInitialThicknesses[1] / DredgedFillSublayers[1];
				for (i = 2; i <= DredgedFillSublayers[1] + 1; i++) {
					DredgedFillCoordZ[i] = DredgedFillCoordZ[i - 1] + dz[1];
					DredgedFillCoordA[i] = DredgedFillCoordA[i - 1] + da;
					DredgedFillCoordXI[i] = DredgedFillCoordA[i];
					DredgedFillInitialVoidRatio[i] = DredgedFillInitialVoidRatios[1];
					f[i] = DredgedFillInitialVoidRatios[1];
					DredgedFillCurrentVoidRatio[i] = DredgedFillInitialVoidRatios[1];
					et[i] = DredgedFillInitialVoidRatios[1];
				}
				elev = IncompressibleFoudationElevation + DredgedFillCoordXI[DredgedFillTotalSublayers] + CompressibleFoundationTotalInitialThickness;

				// Is the water table above surface elevation;
				if (ExternalWaterSurfaceElevation > elev) {
					overpress = (ExternalWaterSurfaceElevation - elev) * WaterUnitWeight;
				} else {
					overpress = 0.0;
				}
				totstress = 0.0;

				// Calculate final void ratios for dredged fill;
				FinalVr(d1, VoidRatios, EffectiveStresses, DredgedFillFinalVoidRatio, dz, DredgedFillSublayers, DredgedFillCurrentLayer, DredgedFillMaterialIDs, DredgedFillTotalSublayers, ref totstress, 0, ref jin, auxdf);

				// Calculate maximum second stage drying depth;
				id = DredgedFillMaterialIDs[1];
				Intpgg(d1, RelationDefinitionLines, DredgedFillDesiccationLimits[id], VoidRatios, EffectiveStresses, EffectiveStresses, ref efsdl, ref voidx, ref jin, id, auxdf, 1, 0, 0);
				dz2 = efsdl / (SoilUnitWeight[id] + (WaterUnitWeight * DredgedFillDesiccationLimits[id] * DredgedFillAverageSaturation[id]));
				h2mx = dz2 * (1.0 + DredgedFillDesiccationLimits[id]);
				if (DredgedFillDryingMaxDepth[id] > h2mx) {
					DredgedFillDryingMaxDepth[id] = h2mx;
				}

				// Calculate final void ratios for compressible foundation;
				if (IsFoundationCompressible != 2) {
					// Checks For Compressible Foundation (1 = Compressible);
					FinalVr(d1, VoidRatios, EffectiveStresses, CompressibleFoundationFinalVoidRatio, dz1, CompressibleFoundationSublayers, CompressibleFoundationLayers, CompressibleFoundationMaterialIDs, CompressibleFoundationTotalSublayers, ref totstress, 0, ref jin, auxbl);

					// Calculate initial stresses and pore pressures for foundation layer;
					wl1 = DredgedFillCoordXI[DredgedFillTotalSublayers] + CompressibleFoundationCoordXI[CompressibleFoundationTotalSublayers];
					id = DredgedFillMaterialIDs[1];
					g1 = (DredgedFillCoordZ[DredgedFillTotalSublayers] * SoilBuoyantUnitWeight[id]);
					w1 = fint1[CompressibleFoundationTotalSublayers] + DredgedFillCoordXI[DredgedFillTotalSublayers];
					k = CompressibleFoundationTotalSublayers + 1;
					hsa = 0.0;
					for (i = CompressibleFoundationLayers; i >= 1; i -= 1) {
						id = CompressibleFoundationMaterialIDs[i];
						for (j = 1; j <= CompressibleFoundationSublayers[i] + 1; j++) {
							kk = k - j;
							Intpgg(d1, RelationDefinitionLines, CompressibleFoundationCurrentVoidRatio[kk], VoidRatios, EffectiveStresses, EffectiveStresses, ref CompressibleFoundationEffectiveStress[kk], ref voidx, ref jin, id, auxbl, kk, 0, 0);
							CompressibleFoundationHydrostaticPoreWaterPressure[kk] = WaterUnitWeight * (wl1 - CompressibleFoundationCoordXI[kk]) + overpress;
							CompressibleFoundationTotalStress[kk] = WaterUnitWeight * (w1 - fint1[kk]) + hsa + g1 + overpress;
							hsa = hsa + SoilUnitWeight[id] * dz1[i];
							CompressibleFoundationTotalPoreWaterPressure[kk] = CompressibleFoundationTotalStress[kk] - CompressibleFoundationEffectiveStress[kk];
							CompressibleFoundationExcessPoreWaterPressure[kk] = CompressibleFoundationTotalPoreWaterPressure[kk] - CompressibleFoundationHydrostaticPoreWaterPressure[kk];
						}
						k = kk;
						hsa = hsa - SoilUnitWeight[id] * dz1[i];
					}

					// Ultimate settlement for compresible foundation;
					vri1 = fint1[CompressibleFoundationTotalSublayers];
					Integral(CompressibleFoundationFinalVoidRatio, dz1, CompressibleFoundationSublayers, fint1, CompressibleFoundationLayers, 0);
					CompressibleFoundationFinalSettlement = vri1 - fint1[CompressibleFoundationTotalSublayers];
				}

				// For dredged fill layer;
				id = DredgedFillMaterialIDs[1];
				for (i = 1; i <= DredgedFillTotalSublayers; i++) {
					DredgedFillHydrostaticPoreWaterPressure[i] = WaterUnitWeight * (DredgedFillCoordXI[DredgedFillTotalSublayers] - DredgedFillCoordXI[i]) + overpress;
					DredgedFillExcessPoreWaterPressure[i] = SoilBuoyantUnitWeight[id] * (DredgedFillInitialThicknesses[1] / (1 + DredgedFillInitialVoidRatios[1]) - DredgedFillCoordZ[i]);
					DredgedFillTotalPoreWaterPressure[i] = DredgedFillHydrostaticPoreWaterPressure[i] + DredgedFillExcessPoreWaterPressure[i];
					DredgedFillEffectiveStress[i] = 0.0;
					DredgedFillTotalStress[i] = DredgedFillTotalPoreWaterPressure[i];
				}

				// Ultimate settlement for dredged fill;
				Integral(DredgedFillFinalVoidRatio, dz, DredgedFillSublayers, fint, DredgedFillCurrentLayer, 0);
				DredgedFillFinalSettlement = hsolids - fint[DredgedFillTotalSublayers];

				if (IsFoundationCompressible != 2) {
					// Check for Compressible Foundation;
					// Calculate bottom boundary dudz;
					dudz10 = CompressibleFoundationExcessPoreWaterPressure[1] / IncompressibleFoudationDrainagePathLength;
				} else {
					dudz10 = DredgedFillExcessPoreWaterPressure[1] / IncompressibleFoudationDrainagePathLength;
				}

				// Compute void ratio function for initial values;
				Vrfunc(d1);
			}
		}
	}
}

