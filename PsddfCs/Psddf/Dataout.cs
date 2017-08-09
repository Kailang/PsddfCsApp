namespace PsddfCs {
	public partial class Psddf {
		void Dataout () {
			int i, j, k, id;
			double elev;

			#region format

			// 100 format (/////14('*'), 'Initial Conditions in Compressible', ' Foundation', 13('*'))
			const string f100 =
				"\n\n\n\n\n" +
				"***************** Initial Conditions in Compressible Foundation ****************";
			
			// 101 format (//8x, 5('*'), ' Coordinates ', 5('*'), 13x, 5('*'), ' Void Ratios ', 5('*'))
			// 102 format (/7x, 'A', 10x, 'XI', 11x, 'Z', 7x, 'Einitial', 8x, 'E', 10x, 'Eeop', 2x, 'Material')
			const string f101 =
				"\n\n" +
				"               Coordinate                               Void Ratio                            \n" +
				" -------------------------------------- ------------------------------------------            \n" +
				"  Lagrangian   Convective    Material      Initial      Current    End of Primary   Material  \n" +
				"  Coordinate   Coordinate   Coordinate   Void Ratio   Void Ratio     Void Ratio       Type    \n" +
				"       A           XI            Z            e            e              e                   \n" +
				" ------------ ------------ ------------ ------------ ------------ ---------------- ---------- ";
			
			// 103 format (2x, 5(f8.2, 4x), f8.2, 2x, i4)
			const string f103 =
				"  {0,10:F2}   {1,10:F2}   {2,10:F2}   {3,10:F2}   {4,10:F2}   {5,14:F2}   {6,8}  ";
			
			// 104 format (//15x, 5('*'), ' Stresses ', 5('*'), 7x, 5('*'), ' Pore Pressures ', 5('*'))
			// 105 format (/6x, 'XI', 9x, 'Total', 5x, 'Effective', 5x, 'Total', 6x, 'Static', 6x, 'Excess', 1x, 'Material')
			const string f104 = 
				"\n\n" +
				"                      Stress                   Pore-Water Pressure                      \n" +
				"  Convective  ---------------------- ---------------------------------------            \n" +
				"  Coordinate                          Hydrostatic     Excess        Total     Material  \n" +
				"      XI         Total    Effective    Pore-Water   Pore-Water   Pore-Water     Type    \n" +
				"                Stress      Stress      Pressure     Pressure     Pressure              \n" +
				" ------------ ---------- ----------- ------------- ------------ ------------ ---------- ";
			const string f105 =
				"  {0,10:F2}   {1,8:F2}   {2,9:F2}   {3,11:F2}   {4,10:F2}   {5,10:F2}   {6,8}  ";
			
			// 106 format (/////19('*'), 'Initial Conditions in Dredged Fill', 19('*'))
			const string f106 = 
				"\n\n\n\n\n" +
				"********************** Initial Conditions in Dredged Fill **********************";
			
			// 108 format (/////14('*'), 'Current Conditions in Compressible', ' Foundation', 13('*'))
			const string f108 =
				"\n\n\n\n\n" +
				"***************** Current Conditions in Compressible Foundation ****************";
			
			// 109 format (/////19('*'), 'Current Conditions in Dredged Fill', 19('*'))
			const string f109 = 
				"\n\n\n\n\n" +
				"********************** Current Conditions in Dredged Fill **********************";

			// 107 format (/, 10x, 'Day = ', f8.0, 5x, 'Degree of Consolidation = ', f4.0, '%')
			// 110 format (/10x, 'Total Settlement = ', f8.3, //, 10x, 'Settlement at End of Primary Consolidation =', f8.3)
			// 112 format (/10x, 'Settlement caused by Primary Consolidation at time', f6.0 ' = ', f8.3)
			// 120 format (/10x, 'Settlement caused by Secondary Compression at time', f6.0 ' = ', f8.3)
//			const string f107 =
//				"\n" +
//				"        Day = {0:F0}\n\n" +
//				"        Average Degree of Consolidation at Day {0:F0} = {1:F0}%\n\n" +
//				"        Total Settlement at Day {0:F0} = {2:F2}\n\n" +
//				"        Settlement Caused by Primary Consolidation at Day {0:F0} = {3:F2}\n\n" +
//				"        Settlement Caused by Secondary Compression at Day {0:F0} = {4:F2}\n\n" +
//				"        Settlement at End of Primary Consolidation for Current Layer(s) = {5:F2}";

			// Has Dessiccation
//			const string f1072 =
//				"\n" +
//				"        Day = {0:F0}\n\n" +
//				"        Average Degree of Consolidation at Day {0:F0} = {1:F0}%\n\n" +
//				"        Total Settlement at Day {0:F0} = {2:F2}\n\n" +
//				"        Settlement Caused by Primary Consolidation at Day {0:F0} = {3:F2}\n\n" +
//				"        Settlement Caused by Secondary Compression at Day {0:F0} = {4:F2}\n\n" +
//				"        Settlement Caused by Desiccation at Day {0:F0} = {5:F2}\n\n" +
//				"        Settlement at End of Primary Consolidation for Current Layer(s) = {6:F2}";

			const string f107h = 
				"\n" +
				"        Day = {0:F0}\n\n" +
				"        Average Degree of Consolidation at Day {0:F0} = {1:F0}%\n\n" +
				"        Total Settlement at Day {0:F0} = {2:F2}\n\n" +
				"            Settlement Caused by Primary Consolidation at Day {0:F0} = {3:F3}\n\n";

			const string f107sc = 
				"            Settlement Caused by Secondary Compression at Day {0:F0} = {1:F3}\n\n";

			const string f107d =
				"            Settlement Caused by Desiccation at Day {0:F0} = {1:F3}\n\n";

			const string f107epc = 
				"        Settlement at End of Primary Consolidation for Current Layer(s) = {1:F2}";

			// 114 format (/10x, 'Surface Elevation = ', f8.2)
			const string f114 = 
				"\n\n" +
				"        Surface Elevation at Day {0:F0} = {1:F2}";

			// 10  format (2f15.5)
			const string f10 = 
				"{0,15:F5}{1,15:F5}";
			
			#endregion

			if (CurrentTime <= TimeStep) {
				// write(igracf, '(i10)') numbl;
				Io.WriteLine(GCF, "{0,10}", CompressibleFoundationLayers);
				j = 1;
				for (i = 1; i <= CompressibleFoundationLayers; i++) {
					j = j + CompressibleFoundationSublayers[i];
					// write(igracf, '(g20.10)') z1[j];
					Io.WriteLine(GCF, "{0,20:F10}", CompressibleFoundationCoordZ[j]);
					j = j + 1;
				}
			}

			// write(igracf, '(i10)') nblpoint;
			// write(igradf, '(i10)') ndfpoint;
			Io.WriteLine(GCF, "{0,10}", nblpoint);
			Io.WriteLine(GDF, "{0,10}", ndfpoint);

			// For Compressible Foundation;
			if (IsFoundationCompressible != 2) {
				// Check for Compressible Foundation;
				switch (nflag) {
				case 0:
					// write(iout, 108);
					Io.WriteLine(OUT, f108);
					break;
				case 1:
					// write(iout, 100);
					Io.WriteLine(OUT, f100);
					// Saving intital void ratio in compressible layer for recovery calculation;
					if (IsSaveRecovery == 1) {
						for (i = 1; i <= nblpoint; i++) {
							pre_er[i] = CompressibleFoundationCurrentVoidRatio[i];
						}
					}
					break;
				}

				if (DredgedFillPrintOption != 3) {
					// write(iout, 101);
					// write(iout, 102);
					Io.WriteLine(OUT, f101);
					i = nblpoint;
					for (j = CompressibleFoundationLayers; j >= 1; j -= 1) {
						id = CompressibleFoundationMaterialIDs[j];
						for (k = 1; k <= CompressibleFoundationSublayers[j] + 1; k++) {
							// write(iout, 103)a1[i], xi1[i], z1[i], e11[i], er[i], efin1[i], id;
							Io.WriteLine(
								OUT, f103, 
								CompressibleFoundationCoordA[i], 
								CompressibleFoundationCoordXI[i], 
								CompressibleFoundationCoordZ[i], 
								CompressibleFoundationInitialVoidRatio[i], 
								CompressibleFoundationCurrentVoidRatio[i], 
								CompressibleFoundationFinalVoidRatio[i], id);
							i = i - 1;
						}
					}

					if (DredgedFillPrintOption != 2) {
						// write(iout, 104);
						// write(iout, 105);
						Io.WriteLine(OUT, f104);
						i = nblpoint;
						for (j = CompressibleFoundationLayers; j >= 1; j -= 1) {
							id = CompressibleFoundationMaterialIDs[j];
							for (k = 1; k <= CompressibleFoundationSublayers[j] + 1; k++) {
								// write(iout, 103) xi1[i], tostr1[i], efstr1[i], uw1[i], u01[i], u1[i], id;
								Io.WriteLine(
									OUT, f105, 
									CompressibleFoundationCoordXI[i], 
									CompressibleFoundationTotalStree[i], 
									CompressibleFoundationEffectiveStree[i], 
									CompressibleFoundationHydrostaticPoreWaterPressure[i], 
									CompressibleFoundationExcessPoreWaterPressure[i], 
									CompressibleFoundationTotalPoreWaterPressure[i], 
									id);
								i = i - 1;
							}
						}
					}
				}

				// write(igracf, '(g20.10)') time;
				Io.WriteLine(GCF, "{0,20:F10}", CurrentTime);
				for (i = 1; i <= nblpoint; i++) {
					// write(igracf, '(4g20.10)') z1[i], u1[i], efstr1[i], er[i];
					Io.WriteLine(GCF, "{0,20:F10}{1,20:F10}{2,20:F10}{3,20:F10}", CompressibleFoundationCoordZ[i], CompressibleFoundationExcessPoreWaterPressure[i], CompressibleFoundationEffectiveStree[i], CompressibleFoundationCurrentVoidRatio[i]);
				}

				// write(*, '(80(''*''))');
				// write(*, *) '           COMPRESSIBLE FOUNDATION';
				Cmd.WriteLine(
					"\n" +
					"********************************************************************************\n" +
					"                COMPRESSIBLE FOUNDATION");

				Io.Write(
					OUT, f107h, 
					CurrentTime, 
					CompressibleFoundationAverageConsolidationDegree * 100, 
					CompressibleFoundationTotalSettlement, 
					CompressibleFoundationTotalSettlement - CompressibleFoundationSecondaryCompressionSettlement);
				Cmd.Write(
					f107h, 
					CurrentTime, 
					CompressibleFoundationAverageConsolidationDegree * 100, 
					CompressibleFoundationTotalSettlement, 
					CompressibleFoundationTotalSettlement - CompressibleFoundationSecondaryCompressionSettlement);

				if (CompressibleFoundationAverageConsolidationDegree > MaxConsolidationDegree) {
					Io.Write(OUT, f107sc, CurrentTime, CompressibleFoundationSecondaryCompressionSettlement);
					Cmd.Write(f107sc, CurrentTime, CompressibleFoundationSecondaryCompressionSettlement);
				} else {
					Io.Write(OUT, "            Secondary Compression Not Activited at Day {0:F0}\n\n", CurrentTime);
					Cmd.Write("            Secondary Compression Not Activited at Day {0:F0}\n\n", CurrentTime);
				}
				Io.WriteLine(OUT, f107epc, CurrentTime, CompressibleFoundationFinalSettlement);
				Cmd.WriteLine(f107epc, CurrentTime, CompressibleFoundationFinalSettlement);

				// write(iout, 107) time, ucon1 * 100;
				// write(iout, 110) sett1, sfin1; 
				// write(iout, 112) time, sett1 - setsbl;
				// write(iout, 120) time, setsbl;
//				Io.WriteLine(iout, f107, time, ucon1 * 100, sett1, sett1 - setsbl, setsbl, sfin1);

				// write(*, 107) time, ucon1 * 100;
				// write(*, 110) sett1, sfin1;
				// write(*, 112) time, sett1 - setsbl;
				// write(*, 120) time, setsbl;
//				Cmd.WriteLine(f107, time, ucon1 * 100, sett1, sett1 - setsbl, setsbl, sfin1);
			}

			// For Dredge Fill;
			switch (nflag) {
			case 0:
				// write(iout, 109);
				Io.WriteLine(OUT, f109);
				break;
			case 1:
				// Intitial conditions;
				// write(iout, 106);
				Io.WriteLine(OUT, f106);
				// Saving intital void ratio in compressible layer for recovery calculation;
				if (IsSaveRecovery == 1) {
					for (i = 1; i <= ndfpoint; i++) {
						pre_e[i] = DredgedFillCurrentVoidRatio[i];
					}
					pre_ndfpoint = ndfpoint;
					pre_time = CurrentTime;
				}
				break;
			}

			if (DredgedFillPrintOption != 3) {
				// write(iout, 101);
				// write(iout, 102);
				Io.WriteLine(OUT, f101);
				i = ndfpoint;
				for (j = ndflayer; j >= 1; j -= 1) {
					id = DredgedFillMaterialIDs[j];
					for (k = 1; k <= DredgedFillSublayers[j] + 1; k++) {
						// write(iout, 103)a[i], xi[i], z[i], e1[i], e[i], efin[i], id;
						Io.WriteLine(
							OUT, f103, 
							DredgedFillCoordA[i], 
							DredgedFillCoordXI[i], 
							DredgedFillCoordZ[i], 
							DredgedFillInitialVoidRatio[i], 
							DredgedFillCurrentVoidRatio[i], 
							DredgedFillFinalVoidRatio[i], 
							id);
						i = i - 1;
					}
				}

				if (DredgedFillPrintOption != 2) {
					// write(iout, 104);
					// write(iout, 105);
					Io.WriteLine(OUT, f104);
					i = ndfpoint;
					for (j = ndflayer; j >= 1; j -= 1) {
						id = DredgedFillMaterialIDs[j];
						for (k = 1; k <= DredgedFillSublayers[j] + 1; k++) {
							// write(iout, 103) xi[i], totstr[i], effstr[i], uw[i], u0[i], u[i], id;
							Io.WriteLine(
								OUT, f105, 
								DredgedFillCoordXI[i], 
								DredgedFillTotalStress[i], 
								DredgedFillEffectiveStress[i], 
								DredgedFillHydrostaticPoreWaterPressure[i], 
								DredgedFillExcessPoreWaterPressure[i], 
								DredgedFillTotalPoreWaterPressure[i], 
								id);
							i = i - 1;
						}
					}
				}
			}

			// write(igradf, '(g20.10)') time;
			Io.WriteLine(GDF, "{0,20:F10}", CurrentTime);

			for (i = 1; i <= ndfpoint; i++) {
				// write(igradf, '(4g20.10)')z[i], u[i], effstr[i], e[i];
				Io.WriteLine(GDF, "{0,20:F10}{1,20:F10}{2,20:F10}{3,20:F10}", DredgedFillCoordZ[i], DredgedFillExcessPoreWaterPressure[i], DredgedFillEffectiveStress[i], DredgedFillCurrentVoidRatio[i]);
			}

			// write(*, '(80(''*''))');
			// write(*, *) '                      DREDGED FILL';
			Cmd.WriteLine(
				"\n" +
				"********************************************************************************\n" +
				"                DREDGED FILL");

			Io.Write(
				OUT, f107h, 
				CurrentTime, 
				DredgedFillAverageConsolidationDegree * 100, 
				DredgedFillTotalSettlement, 
				DredgedFillTotalSettlement - DredgedFillSecondaryCompressionSettlement - DredgedFillDesiccationSettlement);
			Cmd.Write(
				f107h, 
				CurrentTime, 
				DredgedFillAverageConsolidationDegree * 100, 
				DredgedFillTotalSettlement, 
				DredgedFillTotalSettlement - DredgedFillSecondaryCompressionSettlement - DredgedFillDesiccationSettlement);

			if (DredgedFillAverageConsolidationDegree > MaxConsolidationDegree) {
				Io.Write(OUT, f107sc, CurrentTime, DredgedFillSecondaryCompressionSettlement);
				Cmd.Write(f107sc, CurrentTime, DredgedFillSecondaryCompressionSettlement);
			} else {
				Io.Write(OUT, "            Secondary Compression Not Activited at Day {0:F0}\n\n", CurrentTime);
				Cmd.Write("            Secondary Compression Not Activited at Day {0:F0}\n\n", CurrentTime);
			}
			if (CurrentTime >= DredgedFillDesiccationDelayDays) {
				Io.Write(OUT, f107d, CurrentTime, DredgedFillDesiccationSettlement);
				Cmd.Write(f107d, CurrentTime, DredgedFillDesiccationSettlement);
			} else {
				Io.Write(OUT, "            Desiccation Not Activited at Day {0:F0}\n\n", CurrentTime);
				Cmd.Write("            Desiccation Not Activited at Day {0:F0}\n\n", CurrentTime);
			}
			Io.WriteLine(OUT, f107epc, CurrentTime, DredgedFillFinalSettlement);
			Cmd.WriteLine(f107epc, CurrentTime, DredgedFillFinalSettlement);

//			if (time >= tds) {
//				Io.WriteLine(iout, f1072, time, ucon * 100, sett, sett - setsdf - setd, setsdf, setd, sfin);
//				Cmd.WriteLine(f1072, time, ucon * 100, sett, sett - setsdf - setd, setsdf, setd, sfin);
//			} else {
//				Io.WriteLine(iout, f107, time, ucon * 100, sett, sett - setsdf - setd, setsdf, sfin);
//				Cmd.WriteLine(f107, time, ucon * 100, sett, sett - setsdf - setd, setsdf, sfin);
//			}

			// write(iout, 107) time, ucon*100;
			// write(iout, 110) sett, sfin;
			// write(iout, 112) time, sett-setsdf-setd;
			// write(iout, 120) time, setsdf;
//			Io.WriteLine(iout, f107, time, ucon * 100, sett, sett - setsdf - setd, setsdf, sfin);

			// write(*, 107) time, ucon*100;
			// write(*, 110) sett, sfin;
			// write(*, 112) time, sett-setsdf-setd;
			// write(*, 120) time, setsdf;
//			Cmd.WriteLine(f107, time, ucon * 100, sett, sett - setsdf - setd, setsdf, sfin);

			if (nflag == 1) {
				// write(*, *);
				Cmd.WriteLine();
			}

//			if (time >= tds) {
			// write(iout, 113) setd;
			// write(*, 113) setd;
//				Io.WriteLine(iout, f113, time, setd);
//				Cmd.WriteLine(f113, time, setd);
//			}

			if (nflag != 1) {
				if (ngraph != 1 && nloop != 0) {
					elev = preelev + hdf1;
					// write(iplot, 10) pretime, elev;
					Io.WriteLine(PLOT, f10, pretime, elev);
				}

				elev = IncompressibleFoudationElevation - CompressibleFoundationTotalSettlement + DredgedFillCoordXI[ndfpoint] + CompressibleFoundationTotalInitialThickness;
				// write(iout, 114) elev;
				// write(*, 114) elev;
				// write(*, *);
				Io.WriteLine(OUT, f114, CurrentTime, elev);
				Cmd.WriteLine(f114, CurrentTime, elev);
				if (ngraph != 1) {
					// write(iplot, 10) time, elev;
					Io.WriteLine(PLOT, f10, CurrentTime, elev);
					pretime = CurrentTime;
					preelev = elev;
					nloop = 1;
				}
			}
		}
	}
}

