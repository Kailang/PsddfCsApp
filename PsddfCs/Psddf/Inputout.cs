namespace PsddfCs {
	public partial class Psddf {
		/// <summary>
		/// Save the .psi input file of the simulation to the filename.
		/// </summary>
		/// <param name="filename">Filename.</param>
		void Inputout (string filename) {
			int i, j = 100, kk;

			Io.OpenWrite(inout, filename);

			Io.PrintLine(inout, j++, "'" + ProblemName + "'", IsNewSimulation, IsNotSaveContinuation);

			if (IsNewSimulation == 1) { // New simulation
				Io.PrintLine(inout, j++, SimulationPrintOption, IsSaveRecovery, IsEnglishUnit);
				Io.PrintLine(inout, j++, IncompressibleFoudationVoidRatio, IncompressibleFoudationPermeability, OriginalDU0, IncompressibleFoudationElevation, ExternalWaterSurfaceElevation, WaterUnitWeight, SecondaryCompressionExcessPoreWaterPressureLimit);
				Io.PrintLine(inout, j++, CompressibleFoundationLayers, CompressibleFoundationMaterialTypes, DredgedFillMaterialTypes);
				Io.PrintLine(inout);

				for (kk = 1; kk <= CompressibleFoundationLayers; kk++) {
					Io.PrintLine(inout, j++, CompressibleFoundationInitialThicknesses[kk], CompressibleFoundationMaterialIDs[kk] % 100, CompressibleFoundationSublayers[kk], CompressibleFoundationOCR[kk]);
				}
				Io.PrintLine(inout);

				for (kk = 1; kk <= CompressibleFoundationMaterialTypes; kk++) {
					var kom = nmat[kk];
					Io.PrintLine(inout, j++, kom, SpecificGravities[kom], CaCcs[kom], CrCcs[kom], RelationDefinitionLines[kom]);

					for (i = 1; i <= RelationDefinitionLines[kom]; i++) {
						Io.PrintLine(inout, j++, VoidRatios[i, kom], EffectiveStresses[i, kom], Permeabilities[i, kom]);
					}
				}
				Io.PrintLine(inout);

				for (kk = 1; kk <= DredgedFillMaterialTypes; kk++) {
					var kom = nmat[CompressibleFoundationMaterialTypes + kk];
					Io.PrintLine(inout, j++, kom, SpecificGravities[kom], CaCcs[kom], CrCcs[kom], DredgedFillDesiccationLimits[kom], DredgedFillSaturationLimits[kom], DredgedFillDryingMaxDepth[kom], DredgedFillAverageSaturation[kom], RelationDefinitionLines[kom]);

					for (i = 1; i <= RelationDefinitionLines[kom]; i++) {
						Io.PrintLine(inout, j++, VoidRatios[i, kom], EffectiveStresses[i, kom], Permeabilities[i, kom]);
					}
				}
				Io.PrintLine(inout);

				kk = 1;
				Io.PrintLine(inout, j++, PrintTimes + 1);
				Io.PrintLine(inout, j++, DredgedFillInitialThicknesses[1], DredgedFillDesiccationDelayDays, DredgedFillDesiccationDelayMonths, DredgedFillPrintOption, DredgedFillInitialVoidRatios[1], DredgedFillMaterialIDs[1], DredgedFillSublayers[1]);
				for (i = 1; i <= PrintTimes; i++) {
					Io.Print(inout, j++, PrintTimeDates[i], NewDredgedFillInitialThicknesses[i], NewDredgedFillDesiccationDelayDays[i], NewDredgedFillDesiccationDelayMonths[i], NewDredgedFillPrintOptions[i]);

					if (NewDredgedFillInitialThicknesses[i] != 0) {
						kk++;
						Io.Print(inout, DredgedFillInitialVoidRatios[kk], DredgedFillMaterialIDs[kk], DredgedFillSublayers[kk]);
					}

					Io.PrintLine(inout);
				}
				Io.PrintLine(inout);

				Io.PrintLine(inout, j++, DaysInMonth, SurfaceDrainageEfficiencyFactor, MaxDredgedFillEvaporationEfficiency);
				for (i = 1; i <= 12; i++)
					Io.PrintLine(inout, j++, MaxEnvironmentalPotentialEvaporation[i], AverageMonthlyRainfall[i]);
			} else { // Continuation file
				Io.PrintLine(inout, j++, ContinuationPrintTimes, ContinuationDredgedFillMaterialTypes);
				Io.PrintLine(inout);

				for (kk = 1; kk <= ContinuationDredgedFillMaterialTypes; kk++) {
					var kom = nmat[CompressibleFoundationMaterialTypes + DredgedFillMaterialTypes + kk];
					Io.PrintLine(inout, j++, kom, SpecificGravities[kom], CaCcs[kom], CrCcs[kom], DredgedFillDesiccationLimits[kom], DredgedFillSaturationLimits[kom], DredgedFillDryingMaxDepth[kom], DredgedFillAverageSaturation[kom], RelationDefinitionLines[kom]);

					for (i = 1; i <= RelationDefinitionLines[kom]; i++) {
						Io.PrintLine(inout, j++, VoidRatios[i, kom], EffectiveStresses[i, kom], Permeabilities[i, kom]);
					}
				}
				Io.PrintLine(inout);

				for (i = nm; i <= PrintTimes; i++) {
					Io.Print(inout, j++, PrintTimeDates[i], NewDredgedFillInitialThicknesses[i], NewDredgedFillDesiccationDelayDays[i], NewDredgedFillDesiccationDelayMonths[i], NewDredgedFillPrintOptions[i]);

					if (NewDredgedFillInitialThicknesses[i] != 0) {
						kk++;
						Io.Print(inout, DredgedFillInitialVoidRatios[kk], DredgedFillMaterialIDs[kk], DredgedFillSublayers[kk]);
					}
					Io.PrintLine(inout);
				}
			}
		}
	}
}

