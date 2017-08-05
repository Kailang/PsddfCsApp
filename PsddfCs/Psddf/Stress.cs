namespace PsddfCs {
	public partial class Psddf {
		/// <summary>
		/// Only called in Psddf.
		/// </summary>
		public void Stress (int d1) {
			int i, jin, id, kk = 0, k, j;
			double wl1, g1, w1, elev, overpress, voidx = 0, h_solid, hsa;

			jin = 2;

			// Calculate void ratio integral and xi coordinates;
			Integral(e, dz, DredgedFillSublayers, fint, ndflayer, 0);
			for (i = 1; i <= ndfpoint; i++) {
				xi[i] = z[i] + fint[i];
			}

			if (IsFoundationCompressible != 2) {
				// Check for Compressible Foundation;
				Integral(er, dz1, CompressibleFoundationSublayers, fint1, CompressibleFoundationLayers, 0);
				for (i = 1; i <= nblpoint; i++) {
					xi1[i] = z1[i] + fint1[i];
				}

				// Calculate settlement and degree of consolidation;
				sett1 = a1[nblpoint] - xi1[nblpoint];
				ucon1 = (sett1 - setsbl) / sfin1;

				// Calculate surface elevation;
				elev = IncompressibleFoudationElevation - sett1 + xi[ndfpoint] + CompressibleFoundationTotalInitialThickness;

				// Is the water table above surface elevation;
				if (ExternalWaterSurfaceElevation > elev) {
					overpress = (ExternalWaterSurfaceElevation - elev) * WaterUnitWeight;
				} else {
					overpress = 0.0;
				}

				// For compresible foundation;
				// Calculate stresses;
				hsa = 0.0;
				wl1 = xi[ndfcons] + xi1[nblpoint];
				k = 1;
				g1 = 0.0;
				for (i = 1; i <= ndflayer; i++) {
					id = DredgedFillMaterialIDs[i];
					j = k + DredgedFillSublayers[i];
					g1 = g1 + (z[j] - z[k]) * gc[id];
					k = j + 1;
				}
				g1 = qdf + g1;
				w1 = fint1[nblpoint] + xi[ndfcons];
				k = nblpoint + 1;
				for (i = CompressibleFoundationLayers; i >= 1; i -= 1) {
					id = CompressibleFoundationMaterialIDs[i];
					for (j = 1; j <= CompressibleFoundationSublayers[i] + 1; j++) {
						kk = k - j;
						if (er[kk] > efin1[kk]) {
							Intpgg(d1, RelationDefinitionLines, er[kk], VoidRatios, EffectiveStresses, EffectiveStresses, ref efstr1[kk], ref voidx, ref jin, id, auxbl, kk, 6, 6);
						} else {
							Intpgg(d1, RelationDefinitionLines, efin1[kk], VoidRatios, EffectiveStresses, EffectiveStresses, ref efstr1[kk], ref voidx, ref jin, id, auxbl, kk, 6, 6);
						}
						u01[kk] = WaterUnitWeight * (wl1 - xi1[kk]) + overpress;
						tostr1[kk] = WaterUnitWeight * (w1 - fint1[kk]) + hsa + g1 + overpress;
						hsa = hsa + gs[id] * dz1[i];
						uw1[kk] = tostr1[kk] - efstr1[kk];
						u1[kk] = uw1[kk] - u01[kk];
					}
					k = kk;
					hsa = hsa - gs[id] * dz1[i];
				}
				if (u1[1] < 0.0) {
					u1[1] = 0.0;
				}
			} else {
				elev = IncompressibleFoudationElevation + xi[ndfpoint];
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
					if (e[kk] > efin[kk]) {
						// Still Consolidating;
						Intpgg(d1, RelationDefinitionLines, e[kk], VoidRatios, EffectiveStresses, EffectiveStresses, ref effstr[kk], ref voidx, ref jin, id, auxdf, kk, 6, 6);
					} else {
						// Finished Consolidating;
						Intpgg(d1, RelationDefinitionLines, efin[kk], VoidRatios, EffectiveStresses, EffectiveStresses, ref effstr[kk], ref voidx, ref jin, id, auxdf, kk, 6, 6);
					}
					u0[kk] = WaterUnitWeight * (xi[ndfcons] - xi[kk]) + overpress;
					totstr[kk] = WaterUnitWeight * (fint[ndfcons] - fint[kk]) + hsa + qdf + overpress;
					hsa = hsa + gs[id] * dz[i];
					uw[kk] = totstr[kk] - effstr[kk];
					if (uw[kk] < u0[kk])
						uw[kk] = u0[kk];
					u[kk] = uw[kk] - u0[kk];
				}
				h_solid = h_solid + gs[id] * DredgedFillSublayers[i] * dz[i];
				k = kk;
			}

			DredgedFillSublayers[ndflayer] = DredgedFillSublayers[ndflayer] + (ndfpoint - ndfcons);
			sett = a[ndfpoint] - xi[ndfpoint];

			// change 2ndary;
			setc = sett - setd - setsdf;
			ucon = setc / sfin;
			if (u[1] < 0.0) {
				u[1] = 0.0;
			}
		}
	}
}

