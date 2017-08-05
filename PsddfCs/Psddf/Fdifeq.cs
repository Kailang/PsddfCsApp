namespace PsddfCs {
	public partial class Psddf {
		/// <summary>
		/// Calculate new void ratios as consolidation proceeds.
		/// Only called in Psddf.
		/// </summary>
		public void Fdifq (int d1) {
			int jin, id, i = 0, ii, ij, l, k, j;

			double cf, dz2, dz12, cf1, est = 0, cc = 0, ca;
			double ut, est1 = 0, ut1, rpker = 0, pke = 0, raf, rbf, stab, cons;
			double maxu, voidx = 0, ratio;

			bool logic1;

			#region format

			const string f100 = 
				"\n\n\n\n\n  Stability Error --FOUNDATION -- LAYER/{0,5}";
			const string f101 = 
				"\n\n\n\n\n  Consistency Error --FOUNDATION --LAYER/{0,5}\n" +
				"  An element size in this layer is too large.\n" +
				"  -->Increase the number of sublayers (NSUB1) in this layer.";
			const string f102 = 
				"\n\n\n\n\n  Stability Error --DREDGED FILL --LAYER/{0,5}";
			const string f103 = 
				"\n\n\n\n\n  Consistency Error --DREDGED FILL --LAYER/{0,5}\n" +
				"  An element size in this layer is too large.\n" +
				"  -->Increase the number of sublayers (NSUB1) in this layer.";

			#endregion

			jin = 2;
			nnd = ndfcons - 1;
			logic1 = true;

			// set constrainns;
			while (logic1) {
				if (IsFoundationCompressible != 2) {
					// Check for Compressible Foundation;
					l = 1;
					for (k = 1; k <= CompressibleFoundationLayers; k++) {
						id = CompressibleFoundationMaterialIDs[k];
						cf1 = tau / (WaterUnitWeight * dz1[k]);
						dz12 = dz1[k] * 2.0;
						if (IsCompressibleFoundationInPrimaryConsolidations[k]) {
							if (l == 1) {
								FirstPoint(d1, RelationDefinitionLines, ref CompressibleFoundationCurrentVoidRatio[1], VoidRatios, dsde, dudz11, gc[id], f1[1], f1[2], af1, 1, dz1[k], cf1, bf1[1], CompressibleFoundationFinalVoidRatio[1], CompressibleFoundationInitialVoidRatio[1], ref jin, id, auxbl);
							} else {
								Boundary(d1, RelationDefinitionLines, l, k, CompressibleFoundationCurrentVoidRatio, f1, CompressibleFoundationFinalVoidRatio, dz1, CompressibleFoundationExcessPoreWaterPressure, pk, VoidRatios, EffectiveStresses, CompressibleFoundationEffectiveStree, CompressibleFoundationMaterialIDs, ref jin, auxbl);
							}
						}
						if (IsCompressibleFoundationInPrimaryConsolidations[k]) {
							maxu = 0;
							for (j = 1; j <= CompressibleFoundationSublayers[k] - 1; j++) {
								i = l + j;
								ij = i + 1;
								ii = i - 1;
								CompressibleFoundationCurrentVoidRatio[i] = VoidRatio(f1[i], f1[ij], f1[ii], af1[i], af1[ii], af1[ij], dz1[k], dz12, cf1, gc[id], bf1[i]);
								if (CompressibleFoundationCurrentVoidRatio[i] > f1[i]) {
									CompressibleFoundationCurrentVoidRatio[i] = f1[i];
								}

								if (CompressibleFoundationCurrentVoidRatio[i] <= CompressibleFoundationFinalVoidRatio[i]) {
									CompressibleFoundationCurrentVoidRatio[i] = CompressibleFoundationFinalVoidRatio[i];
								}

								Intpgg(d1, RelationDefinitionLines, CompressibleFoundationCurrentVoidRatio[i], VoidRatios, EffectiveStresses, EffectiveStresses, ref CompressibleFoundationEffectiveStree[i], ref voidx, ref jin, id, auxbl, i, 6, 6);
								Intpgg(d1, RelationDefinitionLines, f1[i], VoidRatios, EffectiveStresses, EffectiveStresses, ref voidx, ref voidx, ref jin, id, auxbl, i, 6, 6);
								CompressibleFoundationExcessPoreWaterPressure[i] = CompressibleFoundationExcessPoreWaterPressure[i] - (CompressibleFoundationEffectiveStree[i] - voidx);

								if (CompressibleFoundationExcessPoreWaterPressure[i] > maxu) {
									maxu = CompressibleFoundationExcessPoreWaterPressure[i];
								}
							}

//							if (maxu < tol) {
							// If Compressible Foundation's degree of consolidatoin is larger than 96%,
							// start secondary compression.
							if (CompressibleFoundationAverageConsolidationDegree > uconmax) {
								IsCompressibleFoundationInPrimaryConsolidations[k] = false;
								tpbl[k] = CurrentTime;
								if (l > 1) CompressibleFoundationCurrentVoidRatio[l - 1] = CompressibleFoundationFinalVoidRatio[l - 1];
								for (j = 0; j <= CompressibleFoundationSublayers[k]; j++) {
									i = l + j;
									CompressibleFoundationCurrentVoidRatio[i] = CompressibleFoundationFinalVoidRatio[i];
									Intpgg(d1, RelationDefinitionLines, CompressibleFoundationCurrentVoidRatio[i], VoidRatios, EffectiveStresses, EffectiveStresses, ref voidx, ref voidx, ref jin, id, auxbl, i, 6, 6);
									auxbl[4, i] = voidx;
								}
								Integral(CompressibleFoundationCurrentVoidRatio, dz1, CompressibleFoundationSublayers, fint1, CompressibleFoundationLayers, 0);
								difsecbl[k] = fint1[i] - fint1[l];
							}
						} else {
							Intpgg(d1, RelationDefinitionLines, CompressibleFoundationFinalVoidRatio[l], VoidRatios, dvds, dvds, ref cc, ref voidx, ref jin, id, auxbl, l, 12, 12);
							ca = cc * CaCcs[id];

							CompressibleFoundationCurrentVoidRatio[l] = f1[l] + ca * dlog10(CurrentTime / (CurrentTime - tau));

							for (j = 1; j <= CompressibleFoundationSublayers[k]; j++) {
								jin = 2;
								i = l + j;
								Intpgg(d1, RelationDefinitionLines, CompressibleFoundationFinalVoidRatio[i], VoidRatios, dvds, dvds, ref cc, ref voidx, ref jin, id, auxbl, i, 12, 12);
								ca = cc * CaCcs[id];
								CompressibleFoundationCurrentVoidRatio[i] = f1[i] + ca * dlog10(CurrentTime / (CurrentTime - tau));
							}
							Integral(sub(f1, CompressibleFoundationCurrentVoidRatio), dz1, CompressibleFoundationSublayers, ffint1, CompressibleFoundationLayers, 0);
							CompressibleFoundationSecondaryCompressionSettlement = CompressibleFoundationSecondaryCompressionSettlement + (ffint1[i] - ffint1[l]);
						}
						l = l + CompressibleFoundationSublayers[k] + 1;
					}

					// Reset for next loop (Compresible Foundation);
					for (i = 1; i <= nblpoint; i++) {
						f1[i] = CompressibleFoundationCurrentVoidRatio[i];
					}

					// Reset Bottom Boundary Gradient for C.F.;
					id = CompressibleFoundationMaterialIDs[1];
					Intpgg(d1, RelationDefinitionLines, CompressibleFoundationCurrentVoidRatio[1], VoidRatios, pk, pk, ref rpker, ref voidx, ref jin, id, auxbl, 1, 0, 0);
					Intpgg(d1, RelationDefinitionLines, CompressibleFoundationCurrentVoidRatio[1], VoidRatios, EffectiveStresses, EffectiveStresses, ref voidx, ref est1, ref jin, id, auxbl, 1, 6, 6);

					dudz11 = dudz10 * pk0 / rpker;
					ut1 = CompressibleFoundationExcessPoreWaterPressure[1] - est1 + CompressibleFoundationEffectiveStree[1];
					dudz10 = ut1 / IncompressibleFoudationDrainagePathLength;

					// Calculate void ratio of top point in C.F.;
					Bcfdf(d1, VoidRatios, EffectiveStresses, pk, ref jin);
				} else {
					// Rest Bottom Boundary dudz for D.F.;
					id = DredgedFillMaterialIDs[1];
					Intpgg(d1, RelationDefinitionLines, DredgedFillCurrentVoidRatio[1], VoidRatios, pk, pk, ref pke, ref voidx, ref jin, id, auxdf, 1, 0, 0);
					Intpgg(d1, RelationDefinitionLines, DredgedFillCurrentVoidRatio[1], VoidRatios, EffectiveStresses, EffectiveStresses, ref voidx, ref est, ref jin, id, auxdf, 1, 10, 4);
					dudz21 = dudz10 * pk0 / pke;
					ut = DredgedFillExcessPoreWaterPressure[1] - est + DredgedFillEffectiveStress[1];
					dudz10 = ut / IncompressibleFoudationDrainagePathLength;
				}

				DredgedFillSublayers[ndflayer] = DredgedFillSublayers[ndflayer] - (ndfpoint - ndfcons);
				l = 1;
				for (k = 1; k <= ndflayer; k++) {
					id = DredgedFillMaterialIDs[k];
					cf = tau / (WaterUnitWeight * dz[k]);
					dz2 = dz[k] * 2.0;
					if (IsDredgedFillInPrimaryConsolidations[k]) {
						if (l == 1 && IsFoundationCompressible == 2) {
							FirstPoint(d1, RelationDefinitionLines, ref DredgedFillCurrentVoidRatio[l], VoidRatios, dsde, dudz21, gc[id], f[l], f[l + 1], af, l, dz[k], cf, bf[l], DredgedFillFinalVoidRatio[l], DredgedFillInitialVoidRatio[l], ref jin, id, auxdf);
						} else if (l != 1) {
							Boundary(d1, RelationDefinitionLines, l, k, DredgedFillCurrentVoidRatio, f, DredgedFillFinalVoidRatio, dz, DredgedFillExcessPoreWaterPressure, pk, VoidRatios, EffectiveStresses, DredgedFillEffectiveStress, DredgedFillMaterialIDs, ref jin, auxdf);
						}
					}
					if (IsDredgedFillInPrimaryConsolidations[k]) {
						maxu = 0;
						for (j = 1; j <= DredgedFillSublayers[k] - 1; j++) {
							i = l + j;
							if (DredgedFillCurrentVoidRatio[i] > DredgedFillFinalVoidRatio[i]) {
								ij = i + 1;
								ii = i - 1;
								DredgedFillCurrentVoidRatio[i] = VoidRatio(f[i], f[ij], f[ii], af[i], af[ii], af[ij], dz[k], dz2, cf, gc[id], bf[i]);
								if (DredgedFillCurrentVoidRatio[i] <= DredgedFillFinalVoidRatio[i]) {
									DredgedFillCurrentVoidRatio[i] = DredgedFillFinalVoidRatio[i];
								}
								if (DredgedFillCurrentVoidRatio[i] > f[i]) {
									DredgedFillCurrentVoidRatio[i] = f[i];
								}
								Intpgg(d1, RelationDefinitionLines, DredgedFillCurrentVoidRatio[i], VoidRatios, EffectiveStresses, EffectiveStresses, ref DredgedFillEffectiveStress[i], ref voidx, ref jin, id, auxdf, i, 6, 6);
								Intpgg(d1, RelationDefinitionLines, f[i], VoidRatios, EffectiveStresses, EffectiveStresses, ref voidx, ref voidx, ref jin, id, auxdf, i, 6, 6);
								DredgedFillExcessPoreWaterPressure[i] = DredgedFillExcessPoreWaterPressure[i] - (DredgedFillEffectiveStress[i] - voidx);
								if (DredgedFillExcessPoreWaterPressure[i] > maxu) {
									maxu = DredgedFillExcessPoreWaterPressure[i];
								}
							}
						}

//						if (maxu < tol) {
						// If Dredged Fill's degree of consolidatoin is larger tha 96%,
						// start secondary compression.
						if (DredgedFillAverageConsolidationDegree > uconmax) {
							IsDredgedFillInPrimaryConsolidations[k] = false;
							// Changes this because of secondary compression??;

							tpdf[k] = CurrentTime;
							if (l > 1) DredgedFillCurrentVoidRatio[l - 1] = DredgedFillFinalVoidRatio[l - 1];
							for (j = 0; j <= DredgedFillSublayers[k]; j++) {
								i = l + j;
								DredgedFillCurrentVoidRatio[i] = DredgedFillFinalVoidRatio[i];
								Intpgg(d1, RelationDefinitionLines, DredgedFillCurrentVoidRatio[i], VoidRatios, EffectiveStresses, EffectiveStresses, ref voidx, ref voidx, ref jin, id, auxdf, i, 6, 6);
								auxdf[4, i] = voidx;
							}
							Integral(DredgedFillCurrentVoidRatio, dz, DredgedFillSublayers, fint, ndflayer, 0);
							difsecdf[k] = fint[i] - fint[l];
							if (k == ndflayer) {
								auxdf[4, ndfpoint] = auxdf[4, ndfpoint - 1] * 0.01;
							}
						}
					} else {
						Intpgg(d1, RelationDefinitionLines, DredgedFillFinalVoidRatio[l], VoidRatios, dvds, dvds, ref cc, ref voidx, ref jin, id, auxdf, l, 12, 12);
						ca = cc * CaCcs[id];
						DredgedFillCurrentVoidRatio[l] = f[l] + ca * dlog10(CurrentTime / (CurrentTime - tau));

						for (j = 1; j <= DredgedFillSublayers[k]; j++) {
							jin = 2;
							i = l + j;
							Intpgg(d1, RelationDefinitionLines, DredgedFillFinalVoidRatio[i], VoidRatios, dvds, dvds, ref cc, ref voidx, ref jin, id, auxdf, i, 12, 12);
							ca = cc * CaCcs[id];
							DredgedFillCurrentVoidRatio[i] = f[i] + ca * dlog10(CurrentTime / (CurrentTime - tau));

						}

						Integral(sub(f, DredgedFillCurrentVoidRatio), dz, DredgedFillSublayers, ffint, ndflayer, 0);

						/**
						* Jiarui: Fixed PSDDF Error 3
						* Original: None.
						* Descriptions: Secondary compression is calculated by setsdf = setsdf + (ffint[i] ‐ ffint[l]);
						* ffint[i] ‐ ffint[l] should be greater than or equal to zero. In this case, ffint[l]=0 indicates that there is no
						* secondary compression. Set ffint[i] ‐ ffint[l] = 0 if there is no secondary compression.
						**/
						if (ffint[i] - ffint[l] < 0) {
							ffint[i] = ffint[l];
						}

						DredgedFillSecondaryCompressionSettlement = DredgedFillSecondaryCompressionSettlement + (ffint[i] - ffint[l]);
						//Cmd.Print (time, setsdf);
						//Cmd.Print (time, ffint[i],ffint[l]);
						//for (i = 1; i <= ndfpoint; i++) {
						//Cmd.Print (time, i,f [i], e [i], ffint [i]);
						//}
					}
					l = l + DredgedFillSublayers[k] + 1; 
				}
				DredgedFillSublayers[ndflayer] = DredgedFillSublayers[ndflayer] + (ndfpoint - ndfcons);
				for (i = 1; i <= nnd; i++) {
					f[i] = DredgedFillCurrentVoidRatio[i];
				}
				// calculate alpha and beta for current void ratios;
				Vrfunc(d1);

				// calculate current time and check against desication time and print time;
				CurrentTime = CurrentTime + tau;

				if (CurrentTime > DredgedFillDesiccationDelayDays && mm == 1) {
					mm = 2;
					Integral(DredgedFillCurrentVoidRatio, dz, DredgedFillSublayers, fint, ndflayer, ndfpoint - ndfcons);
					cset = vrint - fint[ndfcons];
					setc = setc + cset;
					vrint = fint[ndfcons];
				}
				if (CurrentTime >= dtim) {
					Desic(d1);
				}

				// tnnn = tnnn + 1;
				if (CurrentTime >= tprint) {
					logic1 = false;
					// Recover actual void ratios;
					for (i = 2; i <= ndfcons; i++) {
						if (DredgedFillCurrentVoidRatio[i] > et[i]) {
							DredgedFillCurrentVoidRatio[i] = et[i];
						}
					}
					Vrfunc(d1);

					// Check stability and consistency;
					// Compressible Foundation;
					if (IsFoundationCompressible != 2) {
						// Checks for Compressible Foundation;
						ii = 1;
						for (i = 1; i <= CompressibleFoundationLayers; i++) {
							if (IsCompressibleFoundationInPrimaryConsolidations[i]) {
								rbf = abs(bf1[ii]);
								raf = abs(af1[ii]);
								for (j = 1; j <= CompressibleFoundationSublayers[i]; j++) {
									ii = ii + 1;
									if (abs(af1[ii]) > raf) {
										raf = abs(af1[ii]);
										rbf = abs(bf1[ii]);
									}
								}
								ii = ii + 1;
								stab = abs((dz1[i] * dz1[i] * WaterUnitWeight) / (2.0 * raf));
								if (stab < tau) {
									// write[iout, 100] i;
									// write[*, 100] i;
									Io.WriteLine(iout, f100, i);
									Cmd.WriteLine(f100, i);
								}
								id = CompressibleFoundationMaterialIDs[i];
								cons = abs((2.0 * raf) / (gc[id] * rbf));
								if (cons <= dz1[i]) {
									// write[iout, 101] i;
									// write[*, 101] i;
									Io.WriteLine(iout, f101, i);
									Cmd.WriteLine(f101, i);
								}
							} else {
								ii = ii + CompressibleFoundationSublayers[i] + 1;
							}
						}
					}

					// Dredge Fill;
					ii = 1;
					for (i = 1; i <= ndflayer; i++) {
						if (IsDredgedFillInPrimaryConsolidations[i]) {
							rbf = abs(bf[ii]);
							raf = abs(af[ii]);
							for (j = 1; j <= DredgedFillSublayers[i]; j++) {
								ii = ii + 1;
								if (abs(af[ii]) > raf) {
									raf = abs(af[ii]);
									rbf = abs(bf[ii]);
								}
							}
							ii = ii + 1;
							stab = abs((dz[i] * dz[i] * WaterUnitWeight) / (2.0 * raf));
							if (stab < tau) {
								// write[iout, 102] i;
								// write[*, 102] i;
								Io.WriteLine(iout, f102, i);
								Cmd.WriteLine(f102, i);
							}
							id = DredgedFillMaterialIDs[i];
							cons = abs((2.0 * raf) / (gc[id] * rbf));
							if (cons <= dz[i]) {
								// write[iout, 103] i;
								// write[*, 103] i;
								Io.WriteLine(iout, f103, i);
								Cmd.WriteLine(f103, i);
							}
						} else {
							ii = ii + DredgedFillSublayers[i] + 1;
						}
					}
				}

				// write the status of the simulation to the screen;
				if (IsPrintProcess) {
					ratio = CurrentTime / LastPrintTimeDate * 100;
					// write (*, '(a36, f6.1)') '+ Percentage of Execution Completed:', ratio;
					Cmd.WriteLine("+ Percentage of Execution Completed:{0,6:F1}", ratio);
				}
			}
		}
	}
}

