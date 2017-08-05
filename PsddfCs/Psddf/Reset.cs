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
					u1[i] = u1[i] + el * gc[id];
				}

				for (i = 1; i <= ndfpoint; i++) {
					u[i] = u[i] + gc[id] * el;
				}

				ndfpoint = ndfpoint + DredgedFillSublayers[ndflayer] + 1;
				hsolids = hsolids + el * DredgedFillInitialVoidRatios[ndflayer];
				ell1 = dz[ndflayer] * DredgedFillSublayers[ndflayer] * DredgedFillInitialVoidRatios[ndflayer];
				vlop = vrint + setd + setc + ell1;
				vrint = vlop - setd - setc;
				da = hdf1 / DredgedFillSublayers[ndflayer];

				// Calculate additional coordinates and set void ratios;
				i = ntemp - 1;
				z[ntemp] = z[i];
				a[ntemp] = a[i];
				xi[ntemp] = xi[i];
				e1[ntemp] = DredgedFillInitialVoidRatios[ndflayer];
				f[ntemp] = DredgedFillInitialVoidRatios[ndflayer];
				e[ntemp] = DredgedFillInitialVoidRatios[ndflayer];
				u[ntemp] = u[i];

				for (i = ntemp + 1; i <= ndfpoint; i++) {
					ii = i - 1;
					z[i] = z[ii] + dz[ndflayer];
					a[i] = a[ii] + da;
					xi[i] = xi[ii] + da;
					e1[i] = DredgedFillInitialVoidRatios[ndflayer];
					f[i] = DredgedFillInitialVoidRatios[ndflayer];
					e[i] = DredgedFillInitialVoidRatios[ndflayer];
					u[i] = gc[id] * (acumel - z[i]);
				}

				nt = ntemp - 1;
				e[nt] = (e[nt] + DredgedFillInitialVoidRatios[ndflayer]) * 0.5;
				f[nt] = e[nt];

				// Calculate final void ratios for dredged fill;
				topstress = 0.0;
				FinalVr(d1, VoidRatios, EffectiveStresses, efin, dz, DredgedFillSublayers, ndflayer, DredgedFillMaterialIDs, ndfpoint, ref topstress, 0, ref jin, auxdf);

				// Calculate final void ratios for foundation;
				if (IsFoundationCompressible != 2) {
					// Check for Compressible Foundation;
					FinalVr(d1, VoidRatios, EffectiveStresses, efin1, dz1, CompressibleFoundationSublayers, CompressibleFoundationLayers, CompressibleFoundationMaterialIDs, nblpoint, ref topstress, 0, ref jin, auxbl);

					// Calculate ultimate settlement for commpresive foundation;
					Integral(efin1, dz1, CompressibleFoundationSublayers, fint1, CompressibleFoundationLayers, 0);
					sfin1 = vri1 - fint1[nblpoint];

					// Reset bottom boundary dudz;
					dudz10 = u1[1] / IncompressibleFoudationDrainagePathLength;
				}

				if (IsFoundationCompressible == 2) {
					// Check for Compressible Foundation;
					dudz10 = u[1] / IncompressibleFoudationDrainagePathLength;
				}

				// Ultimate setlement for total dredged fill;
				Integral(efin, dz, DredgedFillSublayers, fint, ndflayer, 0);
				sfin = hsolids - fint[ndfpoint];


				// Set void ratio functions for reset values;
				jin = 2;
				for (i = ntemp; i <= ndfpoint; i++) {
					Intpgg(d1, RelationDefinitionLines, e[i], VoidRatios, alpha, beta, ref af[i], ref bf[i], ref jin, id, auxdf, i, 0, 0);
				}

				for (i = 1; i <= ndfpoint; i++) {
					et[i] = e[i];
				}

				n = ntemp - ndfcons - 1;
				if (n > 0) {
					de = (e[ntemp - 1] - e[ndfcons]) / floatx(n);
					for (i = ndfcons + 1; i <= ntemp - 2; i++) {
						ii = i - 1;
						e[i] = e[ii] + de;
						f[i] = e[i];
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

