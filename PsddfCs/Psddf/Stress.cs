namespace PsddfCs {
	public partial class Psddf {
		/// <summary>
		/// Only called in Psddf.
		/// </summary>
		void Stress (int d1) {
			int i, jin, id, kk = 0, k, j;
			double wl1, g1, w1, elev, overpress, voidx = 0, h_solid, hsa;

			jin = 2;

			// Calculate void ratio integral and xi coordinates;
			Integral(DredgedFillCurrentVoidRatio, dz, DredgedFillSublayers, fint, ndflayer, 0);
			for (i = 1; i <= ndfpoint; i++) {
				DredgedFillCoordXI[i] = DredgedFillCoordZ[i] + fint[i];
			}

			if (IsFoundationCompressible != 2) {
				// Check for Compressible Foundation;
				Integral(CompressibleFoundationCurrentVoidRatio, dz1, CompressibleFoundationSublayers, fint1, CompressibleFoundationLayers, 0);
				for (i = 1; i <= nblpoint; i++) {
					CompressibleFoundationCoordXI[i] = CompressibleFoundationCoordZ[i] + fint1[i];
				}

				// Calculate settlement and degree of consolidation;
				CompressibleFoundationTotalSettlement = CompressibleFoundationCoordA[nblpoint] - CompressibleFoundationCoordXI[nblpoint];
				CompressibleFoundationAverageConsolidationDegree = (CompressibleFoundationTotalSettlement - CompressibleFoundationSecondaryCompressionSettlement) / CompressibleFoundationFinalSettlement;

				// Calculate surface elevation;
				elev = IncompressibleFoudationElevation - CompressibleFoundationTotalSettlement + DredgedFillCoordXI[ndfpoint] + CompressibleFoundationTotalInitialThickness;

				// Is the water table above surface elevation;
				if (ExternalWaterSurfaceElevation > elev) {
					overpress = (ExternalWaterSurfaceElevation - elev) * WaterUnitWeight;
				} else {
					overpress = 0.0;
				}

				// For compresible foundation;
				// Calculate stresses;
				hsa = 0.0;
				wl1 = DredgedFillCoordXI[ndfcons] + CompressibleFoundationCoordXI[nblpoint];
				k = 1;
				g1 = 0.0;
				for (i = 1; i <= ndflayer; i++) {
					id = DredgedFillMaterialIDs[i];
					j = k + DredgedFillSublayers[i];
					g1 = g1 + (DredgedFillCoordZ[j] - DredgedFillCoordZ[k]) * SoilBuoyantUnitWeight[id];
					k = j + 1;
				}
				g1 = qdf + g1;
				w1 = fint1[nblpoint] + DredgedFillCoordXI[ndfcons];
				k = nblpoint + 1;
				for (i = CompressibleFoundationLayers; i >= 1; i -= 1) {
					id = CompressibleFoundationMaterialIDs[i];
					for (j = 1; j <= CompressibleFoundationSublayers[i] + 1; j++) {
						kk = k - j;
						if (CompressibleFoundationCurrentVoidRatio[kk] > CompressibleFoundationFinalVoidRatio[kk]) {
							Intpgg(d1, RelationDefinitionLines, CompressibleFoundationCurrentVoidRatio[kk], VoidRatios, EffectiveStresses, EffectiveStresses, ref CompressibleFoundationEffectiveStree[kk], ref voidx, ref jin, id, auxbl, kk, 6, 6);
						} else {
							Intpgg(d1, RelationDefinitionLines, CompressibleFoundationFinalVoidRatio[kk], VoidRatios, EffectiveStresses, EffectiveStresses, ref CompressibleFoundationEffectiveStree[kk], ref voidx, ref jin, id, auxbl, kk, 6, 6);
						}
						CompressibleFoundationHydrostaticPoreWaterPressure[kk] = WaterUnitWeight * (wl1 - CompressibleFoundationCoordXI[kk]) + overpress;
						CompressibleFoundationTotalStree[kk] = WaterUnitWeight * (w1 - fint1[kk]) + hsa + g1 + overpress;
						hsa = hsa + SoilUnitWeight[id] * dz1[i];
						CompressibleFoundationTotalPoreWaterPressure[kk] = CompressibleFoundationTotalStree[kk] - CompressibleFoundationEffectiveStree[kk];
						CompressibleFoundationExcessPoreWaterPressure[kk] = CompressibleFoundationTotalPoreWaterPressure[kk] - CompressibleFoundationHydrostaticPoreWaterPressure[kk];
					}
					k = kk;
					hsa = hsa - SoilUnitWeight[id] * dz1[i];
				}
				if (CompressibleFoundationExcessPoreWaterPressure[1] < 0.0) {
					CompressibleFoundationExcessPoreWaterPressure[1] = 0.0;
				}
			} else {
				elev = IncompressibleFoudationElevation + DredgedFillCoordXI[ndfpoint];
			}


			// For dreged fill;
			// Calculate stresses;
			// Is the water table above surface elevation?;
			if (ExternalWaterSurfaceElevation > elev) {
				overpress = (ExternalWaterSurfaceElevation - elev) * WaterUnitWeight;
			} else {
				overpress = 0.0;
			}

			h_solid = 0.0;
			k = ndfcons + 1;
			DredgedFillSublayers[ndflayer] = DredgedFillSublayers[ndflayer] - (ndfpoint - ndfcons);

			for (i = ndflayer; i >= 1; i -= 1) {
				id = DredgedFillMaterialIDs[i];
				hsa = h_solid;
				for (j = 1; j <= DredgedFillSublayers[i] + 1; j++) {
					kk = k - j;
					if (DredgedFillCurrentVoidRatio[kk] > DredgedFillFinalVoidRatio[kk]) {
						// Still Consolidating;
						Intpgg(d1, RelationDefinitionLines, DredgedFillCurrentVoidRatio[kk], VoidRatios, EffectiveStresses, EffectiveStresses, ref DredgedFillEffectiveStress[kk], ref voidx, ref jin, id, auxdf, kk, 6, 6);
					} else {
						// Finished Consolidating;
						Intpgg(d1, RelationDefinitionLines, DredgedFillFinalVoidRatio[kk], VoidRatios, EffectiveStresses, EffectiveStresses, ref DredgedFillEffectiveStress[kk], ref voidx, ref jin, id, auxdf, kk, 6, 6);
					}
					DredgedFillHydrostaticPoreWaterPressure[kk] = WaterUnitWeight * (DredgedFillCoordXI[ndfcons] - DredgedFillCoordXI[kk]) + overpress;
					DredgedFillTotalStress[kk] = WaterUnitWeight * (fint[ndfcons] - fint[kk]) + hsa + qdf + overpress;
					hsa = hsa + SoilUnitWeight[id] * dz[i];
					DredgedFillTotalPoreWaterPressure[kk] = DredgedFillTotalStress[kk] - DredgedFillEffectiveStress[kk];
					if (DredgedFillTotalPoreWaterPressure[kk] < DredgedFillHydrostaticPoreWaterPressure[kk])
						DredgedFillTotalPoreWaterPressure[kk] = DredgedFillHydrostaticPoreWaterPressure[kk];
					DredgedFillExcessPoreWaterPressure[kk] = DredgedFillTotalPoreWaterPressure[kk] - DredgedFillHydrostaticPoreWaterPressure[kk];
				}
				h_solid = h_solid + SoilUnitWeight[id] * DredgedFillSublayers[i] * dz[i];
				k = kk;
			}

			DredgedFillSublayers[ndflayer] = DredgedFillSublayers[ndflayer] + (ndfpoint - ndfcons);
			DredgedFillTotalSettlement = DredgedFillCoordA[ndfpoint] - DredgedFillCoordXI[ndfpoint];

			// change 2ndary;
			setc = DredgedFillTotalSettlement - DredgedFillDesiccationSettlement - DredgedFillSecondaryCompressionSettlement;
			DredgedFillAverageConsolidationDegree = setc / DredgedFillFinalSettlement;
			if (DredgedFillExcessPoreWaterPressure[1] < 0.0) {
				DredgedFillExcessPoreWaterPressure[1] = 0.0;
			}
		}
	}
}

