namespace PsddfCs {
	public partial class Psddf {
		/// <summary>
		/// Update previous calculations to handle additional depositions of dredged fill.
		/// Only called in Psddf.
		/// </summary>
		void Reset (int d1) {
			int i, ii, jin, id, nt, n, ntemp;
			double el, ell1, vlop, de, topstress;

			jin = 2;
			dtim = DredgedFillDesiccationDelayDays + DaysInMonth;
			m = DredgedFillDesiccationDelayMonths - 1;
			if (hdf1 > 0.0) {
				aev = 0.0;
				dsc = 0.0;
				qdf = 0.0;
				mm = 1;
				// Calculate ell for next dredged fill layer and reset constants;
				ntemp = DredgedFillTotalSublayers + 1;
				DredgedFillCurrentLayer = DredgedFillCurrentLayer + 1;
				id = DredgedFillMaterialIDs[DredgedFillCurrentLayer];
				el = hdf1 / (1.0 + DredgedFillInitialVoidRatios[DredgedFillCurrentLayer]);
				acumel = acumel + el;

				// Updating u for compresible foundation and dreged fill;
				for (i = 1; i <= CompressibleFoundationTotalSublayers; i++) {
					CompressibleFoundationExcessPoreWaterPressure[i] = CompressibleFoundationExcessPoreWaterPressure[i] + el * SoilBuoyantUnitWeight[id];
				}

				for (i = 1; i <= DredgedFillTotalSublayers; i++) {
					DredgedFillExcessPoreWaterPressure[i] = DredgedFillExcessPoreWaterPressure[i] + SoilBuoyantUnitWeight[id] * el;
				}

				DredgedFillTotalSublayers = DredgedFillTotalSublayers + DredgedFillSublayers[DredgedFillCurrentLayer] + 1;
				hsolids = hsolids + el * DredgedFillInitialVoidRatios[DredgedFillCurrentLayer];
				ell1 = dz[DredgedFillCurrentLayer] * DredgedFillSublayers[DredgedFillCurrentLayer] * DredgedFillInitialVoidRatios[DredgedFillCurrentLayer];
				vlop = vrint + DredgedFillDesiccationSettlement + setc + ell1;
				vrint = vlop - DredgedFillDesiccationSettlement - setc;
				da = hdf1 / DredgedFillSublayers[DredgedFillCurrentLayer];

				// Calculate additional coordinates and set void ratios;
				i = ntemp - 1;
				DredgedFillCoordZ[ntemp] = DredgedFillCoordZ[i];
				DredgedFillCoordA[ntemp] = DredgedFillCoordA[i];
				DredgedFillCoordXI[ntemp] = DredgedFillCoordXI[i];
				DredgedFillInitialVoidRatio[ntemp] = DredgedFillInitialVoidRatios[DredgedFillCurrentLayer];
				f[ntemp] = DredgedFillInitialVoidRatios[DredgedFillCurrentLayer];
				DredgedFillCurrentVoidRatio[ntemp] = DredgedFillInitialVoidRatios[DredgedFillCurrentLayer];
				DredgedFillExcessPoreWaterPressure[ntemp] = DredgedFillExcessPoreWaterPressure[i];

				for (i = ntemp + 1; i <= DredgedFillTotalSublayers; i++) {
					ii = i - 1;
					DredgedFillCoordZ[i] = DredgedFillCoordZ[ii] + dz[DredgedFillCurrentLayer];
					DredgedFillCoordA[i] = DredgedFillCoordA[ii] + da;
					DredgedFillCoordXI[i] = DredgedFillCoordXI[ii] + da;
					DredgedFillInitialVoidRatio[i] = DredgedFillInitialVoidRatios[DredgedFillCurrentLayer];
					f[i] = DredgedFillInitialVoidRatios[DredgedFillCurrentLayer];
					DredgedFillCurrentVoidRatio[i] = DredgedFillInitialVoidRatios[DredgedFillCurrentLayer];
					DredgedFillExcessPoreWaterPressure[i] = SoilBuoyantUnitWeight[id] * (acumel - DredgedFillCoordZ[i]);
				}

				nt = ntemp - 1;
				DredgedFillCurrentVoidRatio[nt] = (DredgedFillCurrentVoidRatio[nt] + DredgedFillInitialVoidRatios[DredgedFillCurrentLayer]) * 0.5;
				f[nt] = DredgedFillCurrentVoidRatio[nt];

				// Calculate final void ratios for dredged fill;
				topstress = 0.0;
				FinalVr(d1, VoidRatios, EffectiveStresses, DredgedFillFinalVoidRatio, dz, DredgedFillSublayers, DredgedFillCurrentLayer, DredgedFillMaterialIDs, DredgedFillTotalSublayers, ref topstress, 0, ref jin, auxdf);

				// Calculate final void ratios for foundation;
				if (IsFoundationCompressible != 2) {
					// Check for Compressible Foundation;
					FinalVr(d1, VoidRatios, EffectiveStresses, CompressibleFoundationFinalVoidRatio, dz1, CompressibleFoundationSublayers, CompressibleFoundationLayers, CompressibleFoundationMaterialIDs, CompressibleFoundationTotalSublayers, ref topstress, 0, ref jin, auxbl);

					// Calculate ultimate settlement for commpresive foundation;
					Integral(CompressibleFoundationFinalVoidRatio, dz1, CompressibleFoundationSublayers, fint1, CompressibleFoundationLayers, 0);
					CompressibleFoundationFinalSettlement = vri1 - fint1[CompressibleFoundationTotalSublayers];

					// Reset bottom boundary dudz;
					dudz10 = CompressibleFoundationExcessPoreWaterPressure[1] / IncompressibleFoudationDrainagePathLength;
				}

				if (IsFoundationCompressible == 2) {
					// Check for Compressible Foundation;
					dudz10 = DredgedFillExcessPoreWaterPressure[1] / IncompressibleFoudationDrainagePathLength;
				}

				// Ultimate setlement for total dredged fill;
				Integral(DredgedFillFinalVoidRatio, dz, DredgedFillSublayers, fint, DredgedFillCurrentLayer, 0);
				DredgedFillFinalSettlement = hsolids - fint[DredgedFillTotalSublayers];


				// Set void ratio functions for reset values;
				jin = 2;
				for (i = ntemp; i <= DredgedFillTotalSublayers; i++) {
					Intpgg(d1, RelationDefinitionLines, DredgedFillCurrentVoidRatio[i], VoidRatios, Alpha, Beta, ref af[i], ref bf[i], ref jin, id, auxdf, i, 0, 0);
				}

				for (i = 1; i <= DredgedFillTotalSublayers; i++) {
					et[i] = DredgedFillCurrentVoidRatio[i];
				}

				n = ntemp - ndfcons - 1;
				if (n > 0) {
					de = (DredgedFillCurrentVoidRatio[ntemp - 1] - DredgedFillCurrentVoidRatio[ndfcons]) / floatx(n);
					for (i = ndfcons + 1; i <= ntemp - 2; i++) {
						ii = i - 1;
						DredgedFillCurrentVoidRatio[i] = DredgedFillCurrentVoidRatio[ii] + de;
						f[i] = DredgedFillCurrentVoidRatio[i];
					}
					ndfcons = ntemp - 1;
					Vrfunc(d1);
				}

				SecondReset(d1, VoidRatios, EffectiveStresses, PK, dvds, Dsde, Alpha, Beta, DredgedFillCurrentLayer - 1, ntemp - 1);
				ndfcons = DredgedFillTotalSublayers;
			}
		}
	}
}

