using PsddfCs.Exception;

namespace PsddfCs {
	public partial class Psddf {
		double[] ps = new double[501];

		/// <summary>
		/// Calculate the new void ratios due to desiccation in the upper parts of the dreged fill.
		/// Only called in Fdifeq.
		/// </summary>
		public void Desic (int d1) {
//			Cmd.Print("Desic()");
			int i, j, ji, jin, id, ij, ntemp;
			bool lset, lset1, lset2, lset3, check;
//			double[] ps = new double[501];
			double h2t, pc, ct, eveff, elev, cd, deav, rv, v, rl, refx, rat, aev1, eav, sav, topstress;

			const string f100 = "\n\n\n\n\n     All points at DL or efinal--REFORMULATE";
			const string f101 = "\n\n\n\n\n     Less than 4 points not at DL--REFORMULATE";

			jin = 2;
			lset = true;
			lset3 = false;
			check = false;


			// Recover actual void ratios;
			for (i = 2; i <= ndfcons; i++) {
				if (DredgedFillCurrentVoidRatio[i] > et[i]) {
					DredgedFillCurrentVoidRatio[i] = et[i];
				}
			}

			// Calculate net desication for month;
			dtim = dtim + DaysInMonth;
			Integral(DredgedFillCurrentVoidRatio, dz, DredgedFillSublayers, fint, ndflayer, 0);
			ct = DredgedFillCoordZ[ndfpoint] + fint[ndfpoint] - DredgedFillCoordZ[ndfcons] - fint[ndfcons];
			cset = vrint - fint[ndfcons];
			setc = setc + cset;
			m = m + 1;
			mm = 2;

			if (m == 13) {
				m = 1;
			}

			ep[m] = MaxEnvironmentalPotentialEvaporation[m] - ((1.0 - SurfaceDrainageEfficiencyFactor) * AverageMonthlyRainfall[m]);
			eveff = MaxDredgedFillEvaporationEfficiency * (1.0 - (ct / DredgedFillDryingMaxDepth[DredgedFillMaterialIDs[ndflayer]]));
			ep[m] = ep[m] * eveff;
			dset = ep[m] - cset - dsc;
			dsc = 0.0;

//			Cmd.WriteLine("(\t" + dset + "\t" + ct + "\t" + ndflayer + "\t" + iddf[ndflayer] + "\t" + h2[iddf[ndflayer]]);
			if (dset > 0.0 && ct < DredgedFillDryingMaxDepth[DredgedFillMaterialIDs[ndflayer]]) {
				// Addittion to prevent desiccation if surface below a fixed water table;
				Integral(DredgedFillCurrentVoidRatio, dz, DredgedFillSublayers, fint, ndflayer, 0);

				for (i = 1; i <= ndfpoint; i++) {
					DredgedFillCoordXI[i] = DredgedFillCoordZ[i] + fint[i];
				}

				if (IsFoundationCompressible != 2) {
					// Check for Compressible Foundation;
					Integral(CompressibleFoundationCurrentVoidRatio, dz1, CompressibleFoundationSublayers, fint1, CompressibleFoundationLayers, 0);
					for (i = 1; i <= nblpoint; i++) {
						CompressibleFoundationCoordXI[i] = DredgedFillCoordZ[i] + fint1[i];
					}
					CompressibleFoundationTotalSettlement = CompressibleFoundationCoordA[nblpoint] - CompressibleFoundationCoordXI[nblpoint];
				} else {
					CompressibleFoundationTotalSettlement = 0.0;
				}

				elev = IncompressibleFoudationElevation - CompressibleFoundationTotalSettlement + DredgedFillCoordXI[ndfpoint] + CompressibleFoundationTotalInitialThickness;
//				Cmd.WriteLine("_\t" + elev + "\t" + wtelev);
				if (elev > ExternalWaterSurfaceElevation) {
					DredgedFillDesiccationSettlement = DredgedFillDesiccationSettlement + dset;
					if (DredgedFillCurrentVoidRatio[ndfpoint] >= DredgedFillSaturationLimits[DredgedFillMaterialIDs[ndflayer]]) {
						// Determine which points are adjustable to sl;
						while (lset) {
							i = ndfpoint;
							lset = false;
							lset1 = true;
							lset2 = true;
							while (i > 4 && lset1 && lset2) {
								if (DredgedFillCurrentVoidRatio[i] > DredgedFillSaturationLimits[DredgedFillMaterialIDs[ndflayer]] && DredgedFillFinalVoidRatio[i] >= DredgedFillSaturationLimits[DredgedFillMaterialIDs[ndflayer]]) {
									lset1 = false;
								} else if (DredgedFillFinalVoidRatio[i] < DredgedFillSaturationLimits[DredgedFillMaterialIDs[ndflayer]]) {
									lset2 = false;
									lset3 = true;
								}
								i = i - 1;
							}
							if (i == 4 && lset1 && lset2) {
								lset3 = true;
							}
							if (!lset1) {
								// Check crust depth;
								i = i + 1;
								cd = DredgedFillCoordZ[ndfpoint] + fint[ndfpoint] - DredgedFillCoordZ[i] - fint[i];
								if (DredgedFillDryingMaxDepth[DredgedFillMaterialIDs[ndflayer]] > DredgedFillInitialThicknesses[ndflayer]) {
									DredgedFillDryingMaxDepth[DredgedFillMaterialIDs[ndflayer]] = DredgedFillInitialThicknesses[ndflayer];
								}
								/**
								 * Jiarui: Fixed PSDDF Error 1
								 * Original: h2t = h2[iddf[ndflayer]];
								 * Descriptions: According to Cargill (1985) Page B19, H2T=H2*(SL/DL).
								 **/
								h2t = DredgedFillDryingMaxDepth[DredgedFillMaterialIDs[ndflayer]] * DredgedFillSaturationLimits[DredgedFillMaterialIDs[ndflayer]] / DredgedFillDesiccationLimits[DredgedFillMaterialIDs[ndflayer]];

								if (cd <= h2t) {
									// Adjust void ratios which are above sl;
									deav = dset / dz[ndflayer];
									if (i == ndfpoint) {
										deav = 2.0 * deav;
									}
									v = DredgedFillCurrentVoidRatio[i] - deav;
									if (v > DredgedFillSaturationLimits[DredgedFillMaterialIDs[ndflayer]]) {
										DredgedFillCurrentVoidRatio[i] = v;
									} else {
										rv = deav - DredgedFillCurrentVoidRatio[i] + DredgedFillSaturationLimits[DredgedFillMaterialIDs[ndflayer]];
										DredgedFillCurrentVoidRatio[i] = DredgedFillSaturationLimits[DredgedFillMaterialIDs[ndflayer]];
										if (i == ndfpoint) {
											rv = rv * 0.5;
										}
										dset = rv * dz[ndflayer];
										if (dset > 0.0001) {
											lset = true;
										}
									}
								} else {
									lset3 = true;
								}
							}
						}
					} else {
						lset3 = true;
					}

					lset = true;
					while (lset && lset3) {
						// Determine which points are adjustable to dl;
						lset1 = true;
						lset2 = true;
						i = ndfpoint;
						while (i > 4 && lset1 && lset2) {
							if (DredgedFillCurrentVoidRatio[i] > DredgedFillDesiccationLimits[DredgedFillMaterialIDs[ndflayer]] && DredgedFillFinalVoidRatio[i] >= DredgedFillDesiccationLimits[DredgedFillMaterialIDs[ndflayer]]) {
								lset1 = false;
							} else if (DredgedFillFinalVoidRatio[i] < DredgedFillDesiccationLimits[DredgedFillMaterialIDs[ndflayer]]) {
								lset2 = false;
								// Print message when all points are at dl or efinal;
//								write(iout, 100);
//								stop;
								Io.WriteLine(iout, f100);
								throw new SimulationException(f100);
							}
							i = i - 1;
						}
						if (!lset1) {
							// Adjust void ratios which are above dl;
							i = i + 1;
							ndfcons = i;
							deav = dset / dz[ndflayer];
							if (i == ndfpoint) {
								deav = deav * 2;
							}
							v = DredgedFillCurrentVoidRatio[i] - deav;
							if (v > DredgedFillDesiccationLimits[DredgedFillMaterialIDs[ndflayer]]) {
								DredgedFillCurrentVoidRatio[i] = v;
								if (DredgedFillFinalVoidRatio[i] > DredgedFillSaturationLimits[DredgedFillMaterialIDs[ndflayer]]) {
									rl = DredgedFillSaturationLimits[DredgedFillMaterialIDs[ndflayer]];
								} else {
									rl = DredgedFillFinalVoidRatio[i];
								}
								if (DredgedFillCurrentVoidRatio[i] >= rl) {
									pc = 1.0;
								} else if (DredgedFillCurrentVoidRatio[i] < rl && rl > DredgedFillDesiccationLimits[DredgedFillMaterialIDs[ndflayer]]) {
									pc = (DredgedFillCurrentVoidRatio[i] - DredgedFillDesiccationLimits[DredgedFillMaterialIDs[ndflayer]]) / (rl - DredgedFillDesiccationLimits[DredgedFillMaterialIDs[ndflayer]]);
								} else {
									pc = 0.0;
								}
								ps[i] = DredgedFillAverageSaturation[DredgedFillMaterialIDs[ndflayer]] + ((1.0 - DredgedFillAverageSaturation[DredgedFillMaterialIDs[ndflayer]]) * pc);
//								Cmd.Print("ps", i, ps[i]);
								lset = false;
							} else {
								rv = deav - DredgedFillCurrentVoidRatio[i] + DredgedFillDesiccationLimits[DredgedFillMaterialIDs[ndflayer]];
								ndfcons = i - 1;
								ps[ndfcons] = 1.0;
//								Cmd.Print("ps ndfcons", ndfcons, ps[ndfcons]);

								DredgedFillCurrentVoidRatio[i] = DredgedFillDesiccationLimits[DredgedFillMaterialIDs[ndflayer]];
								DredgedFillFinalVoidRatio[i] = DredgedFillDesiccationLimits[DredgedFillMaterialIDs[ndflayer]];
								ps[i] = DredgedFillAverageSaturation[DredgedFillMaterialIDs[ndflayer]];
//								Cmd.Print("ps", i, ps[i]);
								if (i == ndfpoint) {
									rv = rv * 0.5;
								}
								dset = rv * dz[ndflayer];
								DredgedFillDesiccationSettlement = DredgedFillDesiccationSettlement - dset;
								// Check new crust thickness;
								ct = DredgedFillCoordZ[ndfpoint] + fint[ndfpoint] - DredgedFillCoordZ[ndfcons] - fint[ndfcons];
								if (ct < DredgedFillDryingMaxDepth[DredgedFillMaterialIDs[ndflayer]]) {
									refx = MaxDredgedFillEvaporationEfficiency * (1.0 - (ct / DredgedFillDryingMaxDepth[DredgedFillMaterialIDs[ndflayer]]));
									rat = refx / eveff;
									dset = rat * dset;
									DredgedFillDesiccationSettlement = DredgedFillDesiccationSettlement + dset;
									if (dset <= 0.0001) {
										lset = false;
									}
								} else {
									lset = false;
								}
							}
						} else {
							// Print a message when less than 4 points not at dl;
//							write(iout, 101);
//							stop;
							Io.WriteLine(iout, f101);
							throw new SimulationException(f101);
						}
					}

					// Determine surcharge due to partially saturated crust and;
					// Carry over desication due to loss of saturation and reset;
					// Stresses in crust;
//					Cmd.WriteLine("!!!\t" + ndfcons + "\t" + ndfpoint + "\t" + lset3);
					if (ndfcons != ndfpoint && lset3) {
						j = ndfpoint - 1;
						qdf = 0.0;
						aev1 = 0.0;
						for (ji = ndfcons; ji <= j; ji++) {
//							Cmd.WriteLine("-\t" + ji);
							i = j + ndfcons - ji;
							ij = i + 1;
//							Cmd.WriteLine("-@\t" + i + "\t" + ij);
							DredgedFillEffectiveStress[ij] = qdf;
							DredgedFillTotalStress[ij] = qdf;
							DredgedFillExcessPoreWaterPressure[ij] = 0.0;
							DredgedFillHydrostaticPoreWaterPressure[ij] = 0.0;
							DredgedFillTotalPoreWaterPressure[ij] = 0.0;

							eav = (DredgedFillCurrentVoidRatio[i] + DredgedFillCurrentVoidRatio[ij]) * 0.5;
//							Cmd.Print("-*", ps[i], ps[ij]);
							sav = (ps[i] + ps[ij]) * 0.5;
							id = DredgedFillMaterialIDs[ndflayer];
							aev1 = (dz[ndflayer] * eav * (1.0 - sav)) + aev1;
//							Cmd.Print("-!", qdf, dz[ndflayer], id, gs[id], eav, gw, sav);
							qdf = qdf + (dz[ndflayer] * (gs[id] + (eav * WaterUnitWeight * sav)));
//							Cmd.WriteLine("-!\t" + qdf);
						}
//						return;

						dsc = aev1 - aev;
						aev = aev1;
						check = true;
						for (i = 1; i <= ndfcons; i++) {
							DredgedFillExcessPoreWaterPressure[i] = DredgedFillExcessPoreWaterPressure[i] + (qdf - qdfold);
						}

						// Calculate new final void ratios due to lower water table;
						// For dredged fill;
						//for (i=1;i<=ndfpoint;i++) {Cmd.Print("before",time,i,efin[i]);}
						topstress = qdf;
						FinalVr(d1, VoidRatios, EffectiveStresses, DredgedFillFinalVoidRatio, dz, DredgedFillSublayers, ndflayer, DredgedFillMaterialIDs, ndfpoint, ref topstress, ndfpoint - ndfcons, ref jin, auxdf);
						//for (i=1;i<=ndfpoint;i++) {Cmd.Print("after",time,i,efin[i]);}
						// Ultimate setlement for total dredged fill;
						Integral(DredgedFillFinalVoidRatio, dz, DredgedFillSublayers, fint, ndflayer, 0);
						DredgedFillFinalSettlement = hsolids - fint[ndfpoint];
						// Reset upper boundary condition for dredged fill;
						v = DredgedFillCurrentVoidRatio[ndfcons];
						if (v > DredgedFillFinalVoidRatio[ndfcons]) {
							DredgedFillCurrentVoidRatio[ndfcons] = DredgedFillFinalVoidRatio[ndfcons];
						}
						f[ndfcons] = DredgedFillCurrentVoidRatio[ndfcons];
						dsc = (v - DredgedFillCurrentVoidRatio[ndfcons]) * dz[ndflayer] + dsc;
						// Calculate new final void ratios due to lower water table;
						// For foundation;
						if (IsFoundationCompressible != 2) {
							// Check for Compressible Foundation;
							FinalVr(d1, VoidRatios, EffectiveStresses, CompressibleFoundationFinalVoidRatio, dz1, CompressibleFoundationSublayers, CompressibleFoundationLayers, CompressibleFoundationMaterialIDs, nblpoint, ref topstress, 0, ref jin, auxbl);
							// Calculate ultimate settlement for commpresive foundation;
							Integral(CompressibleFoundationFinalVoidRatio, dz1, CompressibleFoundationSublayers, fint1, CompressibleFoundationLayers, 0);
							CompressibleFoundationFinalSettlement = vri1 - fint1[nblpoint];
							for (i = 1; i <= nblpoint; i++) {
								CompressibleFoundationExcessPoreWaterPressure[i] = CompressibleFoundationExcessPoreWaterPressure[i] + (qdf - qdfold);
							}
						}
					}
				}
			}

			// Recalculate void ratio integral for next cycle;
			Integral(DredgedFillCurrentVoidRatio, dz, DredgedFillSublayers, fint, ndflayer, 0);

			/**
			* Jiarui: Fixed PSDDF Error 2
			* Original: None.
			* Descriptions: According to Cargill (1985) Page 33 Figure 8. At free water surface, the final void ratio should be the
			* same as the current void ratio. So, I set efin equals e.
			**/
			for (i = 1; i <= ndfpoint; i++) {
				if (DredgedFillCurrentVoidRatio[i] < DredgedFillFinalVoidRatio[i]) {
					DredgedFillFinalVoidRatio[i] = DredgedFillCurrentVoidRatio[i];
				}
			}

			vrint = fint[ndfcons];
			// Reset calculation void ratios;
			for (i = 2; i <= ndfcons; i++) {
				et[i] = DredgedFillCurrentVoidRatio[i];
				if (DredgedFillCurrentVoidRatio[i] < DredgedFillFinalVoidRatio[i]) {
					DredgedFillCurrentVoidRatio[i] = DredgedFillFinalVoidRatio[i];
				}

				f[i] = DredgedFillCurrentVoidRatio[i];
			}

			if (check) {
				ntemp = ndfpoint;
				SecondReset(d1, VoidRatios, EffectiveStresses, pk, dvds, dsde, alpha, beta, ndflayer, ntemp);
			}

			qdfold = qdf;
		}
	}
}

