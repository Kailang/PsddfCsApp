namespace PsddfCs {
	public partial class Psddf {
		/// <summary>
		/// Only called in Intro.
		/// </summary>
		public void Setup (int d1) {
			int jin, i, ii, id, k, ij, j, kk = 0, para;
			double efs, h2mx, dz2, g1, w1, hsa, wl1, efsdl = 0, elev, overpress, voidx = 0, totstress;

			jin = 2;
			nblpoint = 0;

			for (i = 1; i <= CompressibleFoundationLayers; i++) {
				nblpoint = nblpoint + CompressibleFoundationSublayers[i];
			}

			nblpoint = nblpoint + CompressibleFoundationLayers;

			if (ndff <= 1) {
				// ndff = 1 when it is not a restart;
				// Set constants;
				ndfpoint = DredgedFillSublayers[1] + 1;
				ndfcons = ndfpoint;
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
							Intpl(d1, RelationDefinitionLines, efs, EffectiveStresses, VoidRatios, ref e11[ij], ref jin, id, auxbl, ij);
							f1[ij] = e11[ij];
							er[ij] = e11[ij];

							// in case of under consolidated compressible foundation;
							if (CompressibleFoundationOCR[i] < 1.0) {
								if (i != CompressibleFoundationLayers) {
									if (para == 1) {
										// nsub1[i] is even number;
										if (j <= intx(CompressibleFoundationSublayers[i] / 2)) {
											efs = efs - gc[id] * dz1[i] * (2 - CompressibleFoundationOCR[i]) - 0.5 * (layer_stress[i] + layer_stress[i + 1]) * CompressibleFoundationOCR[i] / CompressibleFoundationSublayers[i];
										} else {
											efs = efs - gc[id] * dz1[i] * CompressibleFoundationOCR[i] + 0.5 * (layer_stress[i] + layer_stress[i + 1]) * CompressibleFoundationOCR[i] / CompressibleFoundationSublayers[i];
										}
									} else if (para == 2) {
										// nsub1[i] is odd number;
										if (j <= intx(CompressibleFoundationSublayers[i] / 2)) {
											efs = efs - gc[id] * dz1[i] * (2 - CompressibleFoundationOCR[i]) - 0.5 * (layer_stress[i] + layer_stress[i + 1]) * CompressibleFoundationOCR[i] / CompressibleFoundationSublayers[i];
										} else if (j == intx(CompressibleFoundationSublayers[i] / 2) + 1) {
											efs = efs - gc[id] * dz1[i];
										} else {
											efs = efs - gc[id] * dz1[i] * CompressibleFoundationOCR[i] + 0.5 * (layer_stress[i] + layer_stress[i + 1]) * CompressibleFoundationOCR[i] / CompressibleFoundationSublayers[i];
										}
									}
								} else {
									if (para == 2) {
										// nsub1[numbl] is odd number;
										if (j <= intx(CompressibleFoundationSublayers[i] / 2) + 1) {
											efs = efs - gc[id] * dz1[i] * (2 - CompressibleFoundationOCR[i]);
										} else {
											efs = efs - gc[id] * dz1[i] * CompressibleFoundationOCR[i];
										}
									} else if (para == 1) {
										// nsub1[i] is even number;
										if (j <= intx(CompressibleFoundationSublayers[i] / 2)) {
											efs = efs - gc[id] * dz1[i] * (2 - CompressibleFoundationOCR[i]);
										} else if (j == intx(CompressibleFoundationSublayers[i] / 2) + 1) {
											efs = efs - gc[id] * dz1[i];
										} else {
											efs = efs - gc[id] * dz1[i] * CompressibleFoundationOCR[i];
										}
									}

								}

							} else {
								efs = efs - gc[id] * dz1[i];
							}
						}
						k = k + j - 1;
						// after } j = ii + 1 Fortran's feature (reasong for -1);
						totstress = totstress + gc[id] * dz1[i] * CompressibleFoundationSublayers[i];
					}

					CompressibleFoundationSublayers[CompressibleFoundationLayers] = CompressibleFoundationSublayers[CompressibleFoundationLayers] + 1;
					id = CompressibleFoundationMaterialIDs[CompressibleFoundationLayers];
					e11[nblpoint] = VoidRatios[1, id];
					f1[nblpoint] = e11[nblpoint];
					er[nblpoint] = e11[nblpoint];

					Integral(er, dz1, CompressibleFoundationSublayers, fint1, CompressibleFoundationLayers, 0);

					z1[1] = 0.0;
					a1[1] = 0.0;
					xi1[1] = 0.0;
					k = 0;
					for (i = 1; i <= CompressibleFoundationLayers; i++) {
						ii = CompressibleFoundationSublayers[i] + 1;
						for (j = 2; j <= ii; j++) {
							ij = k + j;
							z1[ij] = z1[ij - 1] + dz1[i];
							a1[ij] = z1[ij] + fint1[ij];
							xi1[ij] = a1[ij];
						}
						k = k + j - 1;
						// after } j = ii + 1 Fortran's feature (reasong for -1);
						z1[k + 1] = z1[k];
						a1[k + 1] = a1[k];
						xi1[k + 1] = xi1[k];
					}
				}

				// Calculate hsolids for first dredged fill layer;
				vrint = DredgedFillInitialVoidRatios[1] * DredgedFillInitialThicknesses[1] / (1.0 + DredgedFillInitialVoidRatios[1]);
				hsolids = vrint;
				acumel = DredgedFillInitialThicknesses[1] / (1.0 + DredgedFillInitialVoidRatios[1]);

				// Calculate initial coordinates and set void ratios;
				z[1] = 0.0;
				e1[1] = DredgedFillInitialVoidRatios[1];
				a[1] = 0.0;
				xi[1] = 0.0;
				f[1] = DredgedFillInitialVoidRatios[1];
				e[1] = DredgedFillInitialVoidRatios[1];
				et[1] = DredgedFillInitialVoidRatios[1];
				da = DredgedFillInitialThicknesses[1] / DredgedFillSublayers[1];
				for (i = 2; i <= DredgedFillSublayers[1] + 1; i++) {
					z[i] = z[i - 1] + dz[1];
					a[i] = a[i - 1] + da;
					xi[i] = a[i];
					e1[i] = DredgedFillInitialVoidRatios[1];
					f[i] = DredgedFillInitialVoidRatios[1];
					e[i] = DredgedFillInitialVoidRatios[1];
					et[i] = DredgedFillInitialVoidRatios[1];
				}
				elev = IncompressibleFoudationElevation + xi[ndfpoint] + CompressibleFoundationTotalInitialThickness;

				// Is the water table above surface elevation;
				if (ExternalWaterSurfaceElevation > elev) {
					overpress = (ExternalWaterSurfaceElevation - elev) * WaterUnitWeight;
				} else {
					overpress = 0.0;
				}
				totstress = 0.0;

				// Calculate final void ratios for dredged fill;
				FinalVr(d1, VoidRatios, EffectiveStresses, efin, dz, DredgedFillSublayers, ndflayer, DredgedFillMaterialIDs, ndfpoint, ref totstress, 0, ref jin, auxdf);

				// Calculate maximum second stage drying depth;
				id = DredgedFillMaterialIDs[1];
				Intpgg(d1, RelationDefinitionLines, DredgedFillDesiccationLimits[id], VoidRatios, EffectiveStresses, EffectiveStresses, ref efsdl, ref voidx, ref jin, id, auxdf, 1, 0, 0);
				dz2 = efsdl / (gs[id] + (WaterUnitWeight * DredgedFillDesiccationLimits[id] * DredgedFillAverageSaturation[id]));
				h2mx = dz2 * (1.0 + DredgedFillDesiccationLimits[id]);
				if (DredgedFillDryingMaxDepth[id] > h2mx) {
					DredgedFillDryingMaxDepth[id] = h2mx;
				}

				// Calculate final void ratios for compressible foundation;
				if (IsFoundationCompressible != 2) {
					// Checks For Compressible Foundation (1 = Compressible);
					FinalVr(d1, VoidRatios, EffectiveStresses, efin1, dz1, CompressibleFoundationSublayers, CompressibleFoundationLayers, CompressibleFoundationMaterialIDs, nblpoint, ref totstress, 0, ref jin, auxbl);

					// Calculate initial stresses and pore pressures for foundation layer;
					wl1 = xi[ndfpoint] + xi1[nblpoint];
					id = DredgedFillMaterialIDs[1];
					g1 = (z[ndfpoint] * gc[id]);
					w1 = fint1[nblpoint] + xi[ndfpoint];
					k = nblpoint + 1;
					hsa = 0.0;
					for (i = CompressibleFoundationLayers; i >= 1; i -= 1) {
						id = CompressibleFoundationMaterialIDs[i];
						for (j = 1; j <= CompressibleFoundationSublayers[i] + 1; j++) {
							kk = k - j;
							Intpgg(d1, RelationDefinitionLines, er[kk], VoidRatios, EffectiveStresses, EffectiveStresses, ref efstr1[kk], ref voidx, ref jin, id, auxbl, kk, 0, 0);
							u01[kk] = WaterUnitWeight * (wl1 - xi1[kk]) + overpress;
							tostr1[kk] = WaterUnitWeight * (w1 - fint1[kk]) + hsa + g1 + overpress;
							hsa = hsa + gs[id] * dz1[i];
							uw1[kk] = tostr1[kk] - efstr1[kk];
							u1[kk] = uw1[kk] - u01[kk];
						}
						k = kk;
						hsa = hsa - gs[id] * dz1[i];
					}

					// Ultimate settlement for compresible foundation;
					vri1 = fint1[nblpoint];
					Integral(efin1, dz1, CompressibleFoundationSublayers, fint1, CompressibleFoundationLayers, 0);
					sfin1 = vri1 - fint1[nblpoint];
				}

				// For dredged fill layer;
				id = DredgedFillMaterialIDs[1];
				for (i = 1; i <= ndfpoint; i++) {
					u0[i] = WaterUnitWeight * (xi[ndfpoint] - xi[i]) + overpress;
					u[i] = gc[id] * (DredgedFillInitialThicknesses[1] / (1 + DredgedFillInitialVoidRatios[1]) - z[i]);
					uw[i] = u0[i] + u[i];
					effstr[i] = 0.0;
					totstr[i] = uw[i];
				}

				// Ultimate settlement for dredged fill;
				Integral(efin, dz, DredgedFillSublayers, fint, ndflayer, 0);
				sfin = hsolids - fint[ndfpoint];

				if (IsFoundationCompressible != 2) {
					// Check for Compressible Foundation;
					// Calculate bottom boundary dudz;
					dudz10 = u1[1] / IncompressibleFoudationDrainagePathLength;
				} else {
					dudz10 = u[1] / IncompressibleFoudationDrainagePathLength;
				}

				// Compute void ratio function for initial values;
				Vrfunc(d1);
			}
		}
	}
}

