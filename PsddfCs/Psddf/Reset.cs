namespace PsddfCs {
	public partial class Psddf {
		/// <summary>
		/// Update previous calculations to handle additional depositions of dredged fill.
		/// Only called in Psddf.
		/// </summary>
		public void Reset (int d1) {
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
				ntemp = ndfpoint + 1;
				ndflayer = ndflayer + 1;
				id = DredgedFillMaterialIDs[ndflayer];
				el = hdf1 / (1.0 + DredgedFillInitialVoidRatios[ndflayer]);
				acumel = acumel + el;

				// Updating u for compresible foundation and dreged fill;
				for (i = 1; i <= nblpoint; i++) {
					CompressibleFoundationExcessPoreWaterPressure[i] = CompressibleFoundationExcessPoreWaterPressure[i] + el * gc[id];
				}

				for (i = 1; i <= ndfpoint; i++) {
					DredgedFillExcessPoreWaterPressure[i] = DredgedFillExcessPoreWaterPressure[i] + gc[id] * el;
				}

				ndfpoint = ndfpoint + DredgedFillSublayers[ndflayer] + 1;
				hsolids = hsolids + el * DredgedFillInitialVoidRatios[ndflayer];
				ell1 = dz[ndflayer] * DredgedFillSublayers[ndflayer] * DredgedFillInitialVoidRatios[ndflayer];
				vlop = vrint + DredgedFillDesiccationSettlement + setc + ell1;
				vrint = vlop - DredgedFillDesiccationSettlement - setc;
				da = hdf1 / DredgedFillSublayers[ndflayer];

				// Calculate additional coordinates and set void ratios;
				i = ntemp - 1;
				DredgedFillCoordZ[ntemp] = DredgedFillCoordZ[i];
				DredgedFillCoordA[ntemp] = DredgedFillCoordA[i];
				DredgedFillCoordXI[ntemp] = DredgedFillCoordXI[i];
				DredgedFillInitialVoidRatio[ntemp] = DredgedFillInitialVoidRatios[ndflayer];
				f[ntemp] = DredgedFillInitialVoidRatios[ndflayer];
				DredgedFillCurrentVoidRatio[ntemp] = DredgedFillInitialVoidRatios[ndflayer];
				DredgedFillExcessPoreWaterPressure[ntemp] = DredgedFillExcessPoreWaterPressure[i];

				for (i = ntemp + 1; i <= ndfpoint; i++) {
					ii = i - 1;
					DredgedFillCoordZ[i] = DredgedFillCoordZ[ii] + dz[ndflayer];
					DredgedFillCoordA[i] = DredgedFillCoordA[ii] + da;
					DredgedFillCoordXI[i] = DredgedFillCoordXI[ii] + da;
					DredgedFillInitialVoidRatio[i] = DredgedFillInitialVoidRatios[ndflayer];
					f[i] = DredgedFillInitialVoidRatios[ndflayer];
					DredgedFillCurrentVoidRatio[i] = DredgedFillInitialVoidRatios[ndflayer];
					DredgedFillExcessPoreWaterPressure[i] = gc[id] * (acumel - DredgedFillCoordZ[i]);
				}

				nt = ntemp - 1;
				DredgedFillCurrentVoidRatio[nt] = (DredgedFillCurrentVoidRatio[nt] + DredgedFillInitialVoidRatios[ndflayer]) * 0.5;
				f[nt] = DredgedFillCurrentVoidRatio[nt];

				// Calculate final void ratios for dredged fill;
				topstress = 0.0;
				FinalVr(d1, VoidRatios, EffectiveStresses, DredgedFillFinalVoidRatio, dz, DredgedFillSublayers, ndflayer, DredgedFillMaterialIDs, ndfpoint, ref topstress, 0, ref jin, auxdf);

				// Calculate final void ratios for foundation;
				if (IsFoundationCompressible != 2) {
					// Check for Compressible Foundation;
					FinalVr(d1, VoidRatios, EffectiveStresses, CompressibleFoundationFinalVoidRatio, dz1, CompressibleFoundationSublayers, CompressibleFoundationLayers, CompressibleFoundationMaterialIDs, nblpoint, ref topstress, 0, ref jin, auxbl);

					// Calculate ultimate settlement for commpresive foundation;
					Integral(CompressibleFoundationFinalVoidRatio, dz1, CompressibleFoundationSublayers, fint1, CompressibleFoundationLayers, 0);
					CompressibleFoundationFinalSettlement = vri1 - fint1[nblpoint];

					// Reset bottom boundary dudz;
					dudz10 = CompressibleFoundationExcessPoreWaterPressure[1] / IncompressibleFoudationDrainagePathLength;
				}

				if (IsFoundationCompressible == 2) {
					// Check for Compressible Foundation;
					dudz10 = DredgedFillExcessPoreWaterPressure[1] / IncompressibleFoudationDrainagePathLength;
				}

				// Ultimate setlement for total dredged fill;
				Integral(DredgedFillFinalVoidRatio, dz, DredgedFillSublayers, fint, ndflayer, 0);
				DredgedFillFinalSettlement = hsolids - fint[ndfpoint];


				// Set void ratio functions for reset values;
				jin = 2;
				for (i = ntemp; i <= ndfpoint; i++) {
					Intpgg(d1, RelationDefinitionLines, DredgedFillCurrentVoidRatio[i], VoidRatios, alpha, beta, ref af[i], ref bf[i], ref jin, id, auxdf, i, 0, 0);
				}

				for (i = 1; i <= ndfpoint; i++) {
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

				SecondReset(d1, VoidRatios, EffectiveStresses, pk, dvds, dsde, alpha, beta, ndflayer - 1, ntemp - 1);
				ndfcons = ndfpoint;
			}
		}
	}
}

