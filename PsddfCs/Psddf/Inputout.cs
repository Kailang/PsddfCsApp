namespace PsddfCs {
	public partial class Psddf {
		/// <summary>
		/// Save the .psi input file of the simulation to the filename.
		/// </summary>
		/// <param name="filename">Filename.</param>
		void Inputout (string filename) {
			int i, j = 100, kk;

			Io.OpenWrite(INOUT, filename);

			Io.PrintLine(INOUT, j++, "'" + ProblemName + "'", IsNewSimulation, IsNotSaveContinuation);

			if (IsNewSimulation == 1) { // New simulation
				Io.PrintLine(INOUT, j++, SimulationPrintOption, IsSaveRecovery, IsEnglishUnit);
				Io.PrintLine(INOUT, j++, IncompressibleFoudationVoidRatio, IncompressibleFoudationPermeability, OriginalDU0, IncompressibleFoudationElevation, ExternalWaterSurfaceElevation, WaterUnitWeight, SecondaryCompressionExcessPoreWaterPressureLimit);
				Io.PrintLine(INOUT, j++, CompressibleFoundationLayers, CompressibleFoundationMaterialTypes, DredgedFillMaterialTypes);
				Io.PrintLine(INOUT);

				for (kk = 1; kk <= CompressibleFoundationLayers; kk++) {
					Io.PrintLine(INOUT, j++, CompressibleFoundationInitialThicknesses[kk], CompressibleFoundationMaterialIDs[kk] % 100, CompressibleFoundationSublayers[kk], CompressibleFoundationOCR[kk]);
				}
				Io.PrintLine(INOUT);

				for (kk = 1; kk <= CompressibleFoundationMaterialTypes; kk++) {
					var kom = MaterialIDs[kk];
					Io.PrintLine(INOUT, j++, kom, SpecificGravities[kom], CaCcs[kom], CrCcs[kom], RelationDefinitionLines[kom]);

					for (i = 1; i <= RelationDefinitionLines[kom]; i++) {
						Io.PrintLine(INOUT, j++, VoidRatios[i, kom], EffectiveStresses[i, kom], Permeabilities[i, kom]);
					}
				}
				Io.PrintLine(INOUT);

				for (kk = 1; kk <= DredgedFillMaterialTypes; kk++) {
					var kom = MaterialIDs[CompressibleFoundationMaterialTypes + kk];
					Io.PrintLine(INOUT, j++, kom, SpecificGravities[kom], CaCcs[kom], CrCcs[kom], DredgedFillDesiccationLimits[kom], DredgedFillSaturationLimits[kom], DredgedFillDryingMaxDepth[kom], DredgedFillAverageSaturation[kom], RelationDefinitionLines[kom]);

					for (i = 1; i <= RelationDefinitionLines[kom]; i++) {
						Io.PrintLine(INOUT, j++, VoidRatios[i, kom], EffectiveStresses[i, kom], Permeabilities[i, kom]);
					}
				}
				Io.PrintLine(INOUT);

				kk = 1;
				Io.PrintLine(INOUT, j++, PrintTimes + 1);
				Io.PrintLine(INOUT, j++, DredgedFillInitialThicknesses[1], DredgedFillDesiccationDelayDays, DredgedFillDesiccationDelayMonths, DredgedFillPrintOption, DredgedFillInitialVoidRatios[1], DredgedFillMaterialIDs[1], DredgedFillSublayers[1]);
				for (i = 1; i <= PrintTimes; i++) {
					Io.Print(INOUT, j++, PrintTimeDates[i], NewDredgedFillInitialThicknesses[i], NewDredgedFillDesiccationDelayDays[i], NewDredgedFillDesiccationDelayMonths[i], NewDredgedFillPrintOptions[i]);

					if (NewDredgedFillInitialThicknesses[i] != 0) {
						kk++;
						Io.Print(INOUT, DredgedFillInitialVoidRatios[kk], DredgedFillMaterialIDs[kk], DredgedFillSublayers[kk]);
					}

					Io.PrintLine(INOUT);
				}
				Io.PrintLine(INOUT);

				Io.PrintLine(INOUT, j++, DaysInMonth, SurfaceDrainageEfficiencyFactor, MaxDredgedFillEvaporationEfficiency);
				for (i = 1; i <= 12; i++)
					Io.PrintLine(INOUT, j++, MaxEnvironmentalPotentialEvaporation[i], AverageMonthlyRainfall[i]);
			} else { // Continuation file
				Io.PrintLine(INOUT, j++, ContinuationPrintTimes, ContinuationDredgedFillMaterialTypes);
				Io.PrintLine(INOUT);

				for (kk = 1; kk <= ContinuationDredgedFillMaterialTypes; kk++) {
					var kom = MaterialIDs[CompressibleFoundationMaterialTypes + DredgedFillMaterialTypes + kk];
					Io.PrintLine(INOUT, j++, kom, SpecificGravities[kom], CaCcs[kom], CrCcs[kom], DredgedFillDesiccationLimits[kom], DredgedFillSaturationLimits[kom], DredgedFillDryingMaxDepth[kom], DredgedFillAverageSaturation[kom], RelationDefinitionLines[kom]);

					for (i = 1; i <= RelationDefinitionLines[kom]; i++) {
						Io.PrintLine(INOUT, j++, VoidRatios[i, kom], EffectiveStresses[i, kom], Permeabilities[i, kom]);
					}
				}
				Io.PrintLine(INOUT);

				for (i = StartPrintTime; i <= PrintTimes; i++) {
					Io.Print(INOUT, j++, PrintTimeDates[i], NewDredgedFillInitialThicknesses[i], NewDredgedFillDesiccationDelayDays[i], NewDredgedFillDesiccationDelayMonths[i], NewDredgedFillPrintOptions[i]);

					if (NewDredgedFillInitialThicknesses[i] != 0) {
						kk++;
						Io.Print(INOUT, DredgedFillInitialVoidRatios[kk], DredgedFillMaterialIDs[kk], DredgedFillSublayers[kk]);
					}
					Io.PrintLine(INOUT);
				}
			}
		}
	}
}

