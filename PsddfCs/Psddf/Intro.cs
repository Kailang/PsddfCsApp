namespace PsddfCs {
	public partial class Psddf {
		/// <summary>
		/// Print input data and results of initial calculations in tabular form.
		/// </summary>
		/// <param name="d1">D1.</param>
		public void Intro (int d1) {
			int i, j, ii, k, kk, id;
			double elev;

			#region format
			const string f900 = 
				" {0,12:F2}{1,12:F2}";
			
			const string f100 = 
				"\n\n\n\n\n" +
				"                ************************************************\n" +
				"                *                                              *\n" +
				"                *    One-Dimensional Primary Consolidation,    *\n" +
				"                *                                              *\n" +
				"                *    Secondary Compression, and Desiccation    *\n" +
				"                *                                              *\n" +
				"                *              of Dredge Fill and              *\n" +
				"                *                                              *\n" +
				"                *        Compressible Foundation Layers        *\n" +
				"                *                                              *\n" +
				"                ************************************************\n\n" +
				"    Problem Name: {0,-60}";
			const string f104 = 
				"\n\n\n\n\n" +
				"                 Compressible Foundation Materials                \n" +
				" ---------------------------------------------------------------- \n" +
				"  Material     Layer     Number of                                \n" +
				"    Type     Thickness   Sublayers    Ca/Cc     Cr/Cc      OCR    \n" +
				" ---------- ----------- ----------- --------- --------- --------- ";

			const string f205 = 
				"  {0,8}   {1,9:F2}   {2,9}   {3,7:F3}   {4,7:F3}   {5,7:F3}  ";

			const string f105 =
				"\n\n" +
				"                                Compressible Foundation Soil Data                                \n" +
				" ----------------------------------------------------------------------------------------------- \n" +
				"              Material Type: {0,-3}                           Specific Gravity: {1,-6:F2}              \n" +
				" ------------------------------------------- --------------------------------------------------- ";
			
			const string f1049 =
				"\n\n" +
				"                                Compressible Foundation Soil Data                                \n" +
				" ----------------------------------------------------------------------------------------------- \n" +
				"              Material Type: {0,-3}                           Specific Gravity: {1,-6:F2}              \n" +
				" ----------------------------------------------------------------------------------------------- \n" +
				"          Overconsolidated Material Properties from Material Type: {2,-3} with OCR: {3,-7:F2}         \n" +
				" ----------------------------------------------------------------------------------------------- ";

			const string f305 = 
				"\n\n" +
				"                                      Dredged Fill Soil Data                                     \n" +
				" ----------------------------------------------------------------------------------------------- \n" +
				"                                        Material Type: {0,-3}                                       \n" +
				" ----------------------------------------------------------------------------------------------- ";

			const string f108 =
				"          Void     Effective     Hydraulic       PK =        Beta =       Dsde =       Alpha =   \n" +
				"   I     Ratio      Stress     Conductivity    k / (1+e)   d(PK) / de    dσ' / de     PK * Dsde  \n" +
				" ----- --------- ------------ -------------- ------------ ------------ ------------ ------------ ";

			const string f110 =
				"  {0,3}   {1,7:F3}   {2,10:0.000E+00}   {3,12:0.000E+00}   {4,10:0.000E+00}   {5,10:0.000E+00}   {6,10:0.000E+00}   {7,10:0.000E+00}  ";

			const string f111 = 
				"\n\n\n\n\n" +
				"                                        Dredged Fill Materials                                       \n" +
				" --------------------------------------------------------------------------------------------------- \n" +
				"                                            Void Ratio    Void Ratio       Maximum      Saturation   \n" +
				"  Material   Specific                            at            at        Crust Depth        at       \n" +
				"    Type      Gravity    Ca/Cc     Cr/Cc     Saturation   Desiccation    at Second     Desiccation   \n" +
				"                                               Limit         Limit      Stage Drying       Limit     \n" +
				" ---------- ---------- --------- --------- ------------- ------------- -------------- -------------- ";

			const string f114 =
				"  {0,8}   {1,8:F3}   {2,7:F3}   {3,7:F3}   {4,11:F3}   {5,11:F3}   {6,12:F3}   {7,12:F3}  ";

			const string f202 =
				"\n\n\n" +
				"                              Dredged Fill Layer Lifts and Print Details                              \n" +
				" ---------------------------------------------------------------------------------------------------- \n" +
				"           Dredged Fill   Dredged Fill   Number of    Void    Day to Start   Month to Start    Print  \n" +
				"   Day    Material Type   Layer Height   Sublayers    Ratio    Desiccation     Desiccation    Detail  \n" +
				" ------- --------------- -------------- ----------- -------- -------------- ---------------- -------- ";

			// 311 format(1x, f5.0, 3x, i2, 6x, f4.1, 4x, i3, 6x, f6.2, 1x, f6.0, 5x, i3, 8x, i3)
			const string f311 = 
				"  {0,5:F0}   {1,13}   {2,12:F1}   {3,9}   {4,6:F2}   {5,12:F0}   {6,14}   {7,6}  ";

			// 312 format(1x, f5.0, 35x, f6.0, 5x, i3, 8x, i3)
			const string f312 = 
				"  {0,5:F0}                                                       {1,12:F0}   {2,14}   {3,6}  ";

			const string f2001 = 
				"    NOTE: e-log p' relationship for material {0,2} was adjusted so the\n" +
				"          initial void ratio equals the void ratio at zero effective\n" +
				"          stress";

			const string f119 = 
				"\n\n\n" +
				"   Rainfall and Evaporation Data  \n" +
				" -------------------------------- \n" +
				"  Month   Rainfall   Evaporation  \n" +
				" ------- ---------- ------------- ";

			// 121 format(/21x, i2, 14x, f6.3, 15x, f6.3)
			const string f121 = 
				"  {0,5}   {1,8:F3}   {2,11:F3}  ";

			// 118 format(/7x, g8.3, 8x, f7.3, 10x, g11.5, 7x, 'z =', f7.2)
			const string f115 =
				"\n\n\n\n\n" +
				"                                         Simulation Data                                         \n" +
				" ----------------------------------------------------------------------------------------------- \n" +
				"  Time Step   Incompressible Foundation   Incompressible Foundation   Incompressible Foundation  \n" +
				"   τ (Days)           Void Ratio            Hydraulic Conductivity       Drainage Path Length    \n" +
				" ----------- --------------------------- --------------------------- --------------------------- \n" +
				"  {0,9:F3}   {1,25:F3}   {2,25:F5}   {3,25:F2}  ";

			const string f200 =
				"\n\n\n\n\n" +
				"                       Desiccation Parameters                        \n" +
				" ------------------------------------------------------------------- \n" +
				"                         Parameter                           Value   \n" +
				" --------------------------------------------------------- --------- \n" +
				"  Surface Drainage Efficiency                               {0,7:F2}  \n" +
				"  Maximum Evaporation Efficiency                            {1,7:F2}  \n" +
				"  Day to Start Desiccation After Initial Fill Placement     {2,7:F0}  \n" +
				"  Month to Start Desiccation After Initial Fill Placement   {3,7:F0}  \n" +
				"  Elevation of Fixed Phreatic Surface                       {4,7:F2}  \n" +
				"  Elevation of Top of Incompressible Foundation             {5,7:F2}  ";
			#endregion

			for (k = 1; k <= DredgedFillMaterialTypes + CompressibleFoundationMaterialTypes; k++) {
				i = nmat[k];
				gs[i] = SpecificGravities[i] * WaterUnitWeight;
				// Calculate unit weight of dredge fill solids;
				gc[i] = gs[i] - WaterUnitWeight;
				// Calculate effective unit weight of fill;
			}

			if (ndff <= 1) {
				// ndff = 1 if not a restart loop;

				// Print problem number and heading;
				// write(iout, 100);
				// write(iout, 101);
				// write(iout, 102);
				// write(iout, 103) problemname;
				Io.WriteLine(iout, f100, ProblemName);

				Calctau(d1);
				Setup(d1);

				if (ngraph != 1) {
					elev = IncompressibleFoudationElevation + CompressibleFoundationTotalInitialThickness;
					// write(iplot, 211) time, elev;
					Io.WriteLine(iplot, f900, CurrentTime, elev);
					elev = elev + DredgedFillInitialThicknesses[1];
					// write(iplot, 211) time, elev;
					Io.WriteLine(iplot, f900, CurrentTime, elev);
				}

				if (SimulationPrintOption == 2) {
					return;
				} else if (IsFoundationCompressible != 2) {
					// Check for Compressible Foundation;
					// write(iout, 104);
					// write(iout, 204);
					Io.WriteLine(iout, f104);
					for (i = 1; i <= CompressibleFoundationLayers; i++) {
						ii = CompressibleFoundationMaterialIDs[i];
						// write(iout, 205) ii, hbl[i], nsub1[i], cacc[ii], crcc[ii], OCR[i];
						Io.WriteLine(iout, f205, ii, CompressibleFoundationInitialThicknesses[i], CompressibleFoundationSublayers[i], CaCcs[ii], CrCcs[ii], CompressibleFoundationOCR[i]);
					}

					for (ii = 1; ii <= CompressibleFoundationMaterialTypes; ii++) {
						k = nmat[ii];
						j = RelationDefinitionLines[k];
						// write(iout, 105) k, gsdf[k];
						// write(iout, 108);
						// write(iout, 109);
						Io.WriteLine(iout, f105, k, SpecificGravities[k]);
						Io.WriteLine(iout, f108);
						for (i = 1; i <= j; i++) {
							// write(iout, 110) i, voidratio[i, k], effectivestress[i, k], perm[i, k], pk[i, k], beta[i, k], dsde[i, k], alpha[i, k];
							Io.WriteLine(iout, f110, i, VoidRatios[i, k], EffectiveStresses[i, k], Permeabilities[i, k], pk[i, k], beta[i, k], dsde[i, k], alpha[i, k]);
						}
					}

					for (i = 1; i <= CompressibleFoundationLayers; i++) {
						id = CompressibleFoundationMaterialIDs[i];
						if (CompressibleFoundationOCR[i] > 1.0) {
							// write (iout, 1049) id, gsdf[id];
							// write (iout, 1050) indi_id[id], OCR[i];
							// write(iout, 108);
							// write(iout, 109);
							Io.WriteLine(iout, f1049, id, SpecificGravities[id], indi_id[id], CompressibleFoundationOCR[i]);
							Io.WriteLine(iout, f108);
							for (j = 1; j <= RelationDefinitionLines[id]; j++) {
								// write(iout, 110) j, voidratio[j, id], effectivestress[j, id], perm[j, id], pk[j, id], beta[j, id], dsde[j, id], alpha[j, id];
								Io.WriteLine(iout, f110, j, VoidRatios[j, id], EffectiveStresses[j, id], Permeabilities[j, id], pk[j, id], beta[j, id], dsde[j, id], alpha[j, id]);
							}
						}
					}
				}
			} else {
				Calctau(d1);
				Setup(d1);
			}

			matindex = CompressibleFoundationMaterialTypes + 1;

			// Print soil data for dredged fill;
			// write(iout, 111);
			// write(iout, 304);
			Io.WriteLine(iout, f111);
			j = intx(matindex);
			for (i = 1; i <= DredgedFillMaterialTypes; i++) {
				k = nmat[j];
				// write(iout, 114) k, gsdf[k], cacc[k], crcc[k], sl[k], dl[k], h2[k], sat[k];
				Io.WriteLine(iout, f114, k, SpecificGravities[k], CaCcs[k], CrCcs[k], DredgedFillSaturationLimits[k], DredgedFillDesiccationLimits[k], DredgedFillDryingMaxDepth[k], DredgedFillAverageSaturation[k]);
				j = j + 1;
			}

			for (ii = 1; ii <= DredgedFillMaterialTypes; ii++) {
				k = nmat[intx(matindex)];
				// write(iout, 305) k;
				Io.WriteLine(iout, f305, k);
				j = RelationDefinitionLines[k];
				// write(iout, 108);
				// write(iout, 109);
				Io.WriteLine(iout, f108);
				for (i = 1; i <= j; i++) {
					// write(iout, 110) i, voidratio[i, k], effectivestress[i, k], perm[i, k], pk[i, k], beta[i, k], dsde[i, k], alpha[i, k];
					Io.WriteLine(iout, f110, i, VoidRatios[i, k], EffectiveStresses[i, k], Permeabilities[i, k], pk[i, k], beta[i, k], dsde[i, k], alpha[i, k]);
				}
				matindex = matindex + 1;
			}

			// write(iout, 202);
			// write(iout, 310);
			// Print Summary Table Headings;
			Io.WriteLine(iout, f202);
			elev = 0.0;
			// write(iout, 311) elev, iddf[1], hdf[1], nsub[1], e00[1], tds, ms, nsc;
			Io.WriteLine(iout, f311, elev, DredgedFillMaterialIDs[1], DredgedFillInitialThicknesses[1], DredgedFillSublayers[1], DredgedFillInitialVoidRatios[1], DredgedFillDesiccationDelayDays, DredgedFillDesiccationDelayMonths, DredgedFillPrintOption);
			kk = 1;
			for (i = 1; i <= PrintTimes; i++) {
				if (NewDredgedFillInitialThicknesses[i] != 0.0) {
					kk = kk + 1;
					// moves to next value in arrays;
					// write(iout, 311) printt[i], iddf[kk], ahdf[i], nsub[kk], e00[kk], atds[i], nms[i], nnsc[i];
					Io.WriteLine(iout, f311, PrintTimeDates[i], DredgedFillMaterialIDs[kk], NewDredgedFillInitialThicknesses[i], DredgedFillSublayers[kk], DredgedFillInitialVoidRatios[kk], NewDredgedFillDesiccationDelayDays[i], NewDredgedFillDesiccationDelayMonths[i], NewDredgedFillPrintOptions[i]);
				} else {
					// write(iout, 312) printt[i], atds[i], nms[i], nnsc[i];
					Io.WriteLine(iout, f312, PrintTimeDates[i], NewDredgedFillDesiccationDelayDays[i], NewDredgedFillDesiccationDelayMonths[i], NewDredgedFillPrintOptions[i]);
				}
			}

			// Print to output indicating e-log p' curve has been adjusted;
			for (k = 1; k <= (DredgedFillMaterialTypes + CompressibleFoundationMaterialTypes); k++) {
				if (!IsCurveAdjusteds[k]) {
					// Flag changed to False if it was adjusted;
					// write(iout, 2001);
					Io.WriteLine(iout, f2001, k);
				}
			}

			if (ndff <= 1) {
				// Print summary of rainfall and evaporation potential;
				// write(iout, 119);
				// write(iout, 120);
				Io.WriteLine(iout, f119);
				for (i = 1; i <= 12; i++) {
					// write(iout, 121) i, rf(i), pep[i];
					Io.WriteLine(iout, f121, i, AverageMonthlyRainfall[i], MaxEnvironmentalPotentialEvaporation[i]);
				}
				// Print calculation data;
				// write(iout, 115);
				// write(iout, 116);
				// write(iout, 117);
				// write(iout, 118) tau, e0, zk0, du0;
				Io.WriteLine(iout, f115, tau, IncompressibleFoudationVoidRatio, IncompressibleFoudationPermeability, IncompressibleFoudationDrainagePathLength);
				// Print desication parametrs;
				// write(iout, 200);
				// write(iout, 201)dreff, ce, tds, ms, wtelev, xel;
				Io.WriteLine(iout, f200, SurfaceDrainageEfficiencyFactor, MaxDredgedFillEvaporationEfficiency, DredgedFillDesiccationDelayDays, DredgedFillDesiccationDelayMonths, ExternalWaterSurfaceElevation, IncompressibleFoudationElevation);
				// Print tables of initial conditions;
				nflag = 1;
				Dataout();
				nflag = 0;
			} else {
				return;
			}
		}
	}
}

