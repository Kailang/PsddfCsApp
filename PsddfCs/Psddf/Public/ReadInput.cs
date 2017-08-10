namespace PsddfCs {
	public partial class Psddf {
		void EatLineNumber () {
			Cmd.Write("Line: {0} \t", Io.ReadInt(IN));
		}

		/// <summary>
		/// Save the .psi input file of the simulation to the filename.
		/// </summary>
		/// <param name="path">Filename.</param>
		public void ReadInput (string path, string continuationPath) {
			Cmd.WriteLine("Read input file {0}...", path);
			Io.OpenRead(IN, path);

			EatLineNumber();
			ProblemName = Io.ReadString(IN);
			IsNewSimulation = Io.ReadInt(IN);
			IsNotSaveContinuation = Io.ReadInt(IN);
			Cmd.WriteLine(
				"ProblemName: {0}, IsNewSimulation: {1}, IsNotSaveContinuation: {2}",
				ProblemName, IsNewSimulation, IsNotSaveContinuation);

			if (IsNewSimulation == 1) {
				// new simulation
				Cmd.WriteLine("\nNew simulation");

				EatLineNumber();
				SimulationPrintOption = Io.ReadInt(IN);
				IsSaveRecovery = Io.ReadInt(IN);
				IsEnglishUnit = Io.ReadInt(IN);
				Cmd.WriteLine(
					"SimulationPrintOption: {0}, IsSaveRecovery: {1}, IsEnglishUnit: {2}",
					SimulationPrintOption, IsSaveRecovery, IsEnglishUnit);

				EatLineNumber();
				IncompressibleFoudationVoidRatio = Io.ReadDouble(IN);
				IncompressibleFoudationPermeability = Io.ReadDouble(IN);
				IncompressibleFoudationDrainagePathLength = Io.ReadDouble(IN);
				IncompressibleFoudationElevation = Io.ReadDouble(IN);
				ExternalWaterSurfaceElevation = Io.ReadDouble(IN);
				WaterUnitWeight = Io.ReadDouble(IN);
				SecondaryCompressionExcessPoreWaterPressureLimit = Io.ReadDouble(IN);
				Cmd.WriteLine(
					"IncompressibleFoudationVoidRatio: {0}, IncompressibleFoudationPermeability: {1}, IncompressibleFoudationDrainagePathLength: {2}, " +
					"\n\t IncompressibleFoudationElevation: {3}, ExternalWaterSurfaceElevation: {4}, WaterUnitWeight: {5}, " +
					"\n\t SecondaryCompressionExcessPoreWaterPressureLimit: {6}",
					IncompressibleFoudationVoidRatio, IncompressibleFoudationPermeability, IncompressibleFoudationDrainagePathLength,
					IncompressibleFoudationElevation, ExternalWaterSurfaceElevation, WaterUnitWeight,
					SecondaryCompressionExcessPoreWaterPressureLimit);

				EatLineNumber();
				CompressibleFoundationLayers = Io.ReadInt(IN);
				CompressibleFoundationMaterialTypes = Io.ReadInt(IN);
				DredgedFillMaterialTypes = Io.ReadInt(IN);
				Cmd.WriteLine(
					"CompressibleFoundationLayers: {0}, CompressibleFoundationMaterialTypes: {1}, DredgedFillMaterialTypes: {2}",
					CompressibleFoundationLayers, CompressibleFoundationMaterialTypes, DredgedFillMaterialTypes);

				Cmd.WriteLine("\nRead compressible foundation layer information...");
				for (int i = 1; i <= CompressibleFoundationLayers; i++) {
					EatLineNumber();
					CompressibleFoundationInitialThicknesses[i] = Io.ReadDouble(IN);
					CompressibleFoundationMaterialIDs[i] = Io.ReadInt(IN);
					CompressibleFoundationSublayers[i] = Io.ReadInt(IN);
					CompressibleFoundationOCR[i] = Io.ReadDouble(IN);
					Cmd.WriteLine(
						"CompressibleFoundationInitialThicknesses: {0}, CompressibleFoundationMaterialIDs: {1}, CompressibleFoundationSublayers: {2}, CompressibleFoundationOCR: {3}",
						CompressibleFoundationInitialThicknesses[i], CompressibleFoundationMaterialIDs[i], CompressibleFoundationSublayers[i], CompressibleFoundationOCR[i]);
				}

				Cmd.WriteLine("\nRead compressible foundation material information...");
				for (int i = 1; i <= CompressibleFoundationMaterialTypes; i++) {
					EatLineNumber();
					var id = Io.ReadInt(IN);
					SpecificGravities[id] = Io.ReadDouble(IN);
					CaCcs[id] = Io.ReadDouble(IN);
					CrCcs[id] = Io.ReadDouble(IN);
					RelationDefinitionLines[id] = Io.ReadInt(IN);

					MaterialIDs[i] = id;
					Cmd.WriteLine(
						"MaterialID: {0}, SpecificGravities: {1}, CaCcs: {2}, CrCcs: {3}, RelationDefinitionLines: {4}",
						id, SpecificGravities[id], CaCcs[id], CrCcs[id], RelationDefinitionLines[id]);

					for (int j = 1; j <= RelationDefinitionLines[id]; j++) {
						EatLineNumber();
						VoidRatios[j, id] = Io.ReadDouble(IN);
						EffectiveStresses[j, id] = Io.ReadDouble(IN);
						Permeabilities[j, id] = Io.ReadDouble(IN);
						Cmd.WriteLine(
							"VoidRatios: {0}, EffectiveStresses: {1}, Permeabilities: {2}",
							VoidRatios[j, id], EffectiveStresses[j, id], Permeabilities[j, id]);
					}
				}

				Cmd.WriteLine("\nRead dredged fill material information...");
				for (int i = 1; i <= DredgedFillMaterialTypes; i++) {
					EatLineNumber();
					var id = Io.ReadInt(IN);
					SpecificGravities[id] = Io.ReadDouble(IN);
					CaCcs[id] = Io.ReadDouble(IN);
					CrCcs[id] = Io.ReadDouble(IN);
					DredgedFillDesiccationLimits[id] = Io.ReadDouble(IN);
					DredgedFillSaturationLimits[id] = Io.ReadDouble(IN);
					DredgedFillDryingMaxDepth[id] = Io.ReadDouble(IN);
					DredgedFillAverageSaturation[id] = Io.ReadDouble(IN);
					RelationDefinitionLines[id] = Io.ReadInt(IN);

					MaterialIDs[CompressibleFoundationMaterialTypes + i] = id;
					Cmd.WriteLine(
						"MaterialID: {0}, SpecificGravities: {1}, CaCcs: {2}, CrCcs: {3}, DredgedFillDesiccationLimits: {4}" +
						"\n\t DredgedFillSaturationLimits: {5}, DredgedFillDryingMaxDepth: {6}, DredgedFillAverageSaturation: {7}" +
						"\n\t RelationDefinitionLines: {8}",
						id, SpecificGravities[id], CaCcs[id], CrCcs[id], DredgedFillDesiccationLimits[id],
						DredgedFillSaturationLimits[id], DredgedFillDryingMaxDepth[id], DredgedFillAverageSaturation[id],
						RelationDefinitionLines[id]);

					for (int j = 1; j <= RelationDefinitionLines[id]; j++) {
						EatLineNumber();
						VoidRatios[j, id] = Io.ReadDouble(IN);
						EffectiveStresses[j, id] = Io.ReadDouble(IN);
						Permeabilities[j, id] = Io.ReadDouble(IN);
						Cmd.WriteLine(
							"VoidRatios: {0}, EffectiveStresses: {1}, Permeabilities: {2}",
							VoidRatios[j, id], EffectiveStresses[j, id], Permeabilities[j, id]);
					}
				}

				Cmd.WriteLine("\nRead print times...");
				EatLineNumber();
				PrintTimes = Io.ReadInt(IN) - 1;
				Cmd.WriteLine("PrintTimes: " + PrintTimes);

				Cmd.WriteLine("\nRead initial dredged fill layer information...");
				EatLineNumber();
				DredgedFillInitialThicknesses[1] = Io.ReadDouble(IN);
				DredgedFillInitialDesiccationDelayDays = DredgedFillDesiccationDelayDays = Io.ReadDouble(IN);
				DredgedFillInitialDesiccationDelayMonths = DredgedFillDesiccationDelayMonths = Io.ReadInt(IN);
				DredgedFillPrintOption = Io.ReadInt(IN);
				DredgedFillInitialVoidRatios[1] = Io.ReadDouble(IN);
				DredgedFillMaterialIDs[1] = Io.ReadInt(IN);
				DredgedFillSublayers[1] = Io.ReadInt(IN);
				Cmd.WriteLine(
					"DredgedFillInitialThicknesses: {0}, DredgedFillDesiccationDelayDays: {1}, DredgedFillDesiccationDelayMonths: {2}, " +
					"\n\t DredgedFillPrintOption: {3}, DredgedFillInitialVoidRatios: {4}, DredgedFillMaterialIDs: {5}, DredgedFillSublayers: {6}",
					DredgedFillInitialThicknesses[1], DredgedFillDesiccationDelayDays, DredgedFillDesiccationDelayMonths,
					DredgedFillPrintOption, DredgedFillInitialVoidRatios[1], DredgedFillMaterialIDs[1], DredgedFillSublayers[1]);

				Cmd.WriteLine("\nRead print options...");
				DredgedFillLayers = 1;
				for (int i = 1; i <= PrintTimes; i++) {
					EatLineNumber();
					PrintTimeDates[i] = Io.ReadDouble(IN);
					NewDredgedFillInitialThicknesses[i] = Io.ReadDouble(IN);
					NewDredgedFillDesiccationDelayDays[i] = Io.ReadDouble(IN);
					NewDredgedFillDesiccationDelayMonths[i] = Io.ReadInt(IN);
					NewDredgedFillPrintOptions[i] = Io.ReadInt(IN);
					Cmd.WriteLine(
						"PrintTimeDates: {0}, NewDredgedFillInitialThicknesses: {1}, NewDredgedFillDesiccationDelayDays: {2}" +
						"\n\t NewDredgedFillDesiccationDelayMonths: {3}, NewDredgedFillPrintOptions: {4}",
						PrintTimeDates[i], NewDredgedFillInitialThicknesses[i], NewDredgedFillDesiccationDelayDays[i],
						NewDredgedFillDesiccationDelayMonths[i], NewDredgedFillPrintOptions[i]);


					if (NewDredgedFillInitialThicknesses[i] != 0) {
						// New layer has been added;
						Cmd.WriteLine("\t Read new layer information...");
						DredgedFillLayers++;
						DredgedFillInitialVoidRatios[DredgedFillLayers] = Io.ReadDouble(IN);
						DredgedFillMaterialIDs[DredgedFillLayers] = Io.ReadInt(IN);
						DredgedFillSublayers[DredgedFillLayers] = Io.ReadInt(IN);

						DredgedFillInitialThicknesses[DredgedFillLayers] = NewDredgedFillInitialThicknesses[i];
						Cmd.WriteLine(
							"\t DredgedFillInitialVoidRatios: {0}, DredgedFillMaterialIDs: {1}, DredgedFillSublayers: {2}",
							DredgedFillInitialVoidRatios[DredgedFillLayers], DredgedFillMaterialIDs[DredgedFillLayers], DredgedFillSublayers[DredgedFillLayers]);
					}
				}

				Cmd.WriteLine("\nRead precipitation and evaportation data...");
				EatLineNumber();
				DaysInMonth = Io.ReadDouble(IN);
				SurfaceDrainageEfficiencyFactor = Io.ReadDouble(IN);
				MaxDredgedFillEvaporationEfficiency = Io.ReadDouble(IN);
				Cmd.WriteLine(
					"DaysInMonth: {0}, SurfaceDrainageEfficiencyFactor: {1}, MaxDredgedFillEvaporationEfficiency: {2}",
					DaysInMonth, SurfaceDrainageEfficiencyFactor, MaxDredgedFillEvaporationEfficiency);

				for (int i = 1; i <= 12; i++) {
					EatLineNumber();
					MaxEnvironmentalPotentialEvaporation[i] = Io.ReadDouble(IN);
					AverageMonthlyRainfall[i] = Io.ReadDouble(IN);
					Cmd.WriteLine(
						"Month: {0}, MaxEnvironmentalPotentialEvaporation: {1}, AverageMonthlyRainfall: {2}",
						i, MaxEnvironmentalPotentialEvaporation[i], AverageMonthlyRainfall[i]);
				}

				ResetSimulation();
			} else {
				// continuation file
				Cmd.WriteLine("\nContinuation file");

				EatLineNumber();
				ContinuationPrintTimes = Io.ReadInt(IN);
				ContinuationDredgedFillMaterialTypes = Io.ReadInt(IN);
				Cmd.WriteLine(
					"ContinuationPrintTimes: {0}, ContinuationDredgedFillMaterialTypes: {1}",
					ContinuationPrintTimes, ContinuationDredgedFillMaterialTypes);

				Cmd.WriteLine("\nRestore continuation status...");
				RestoreContinuation(continuationPath);
				Cmd.WriteLine("Restore continuation status complete");

				Cmd.WriteLine("\nRead dredged fill material information...");
				for (int i = DredgedFillMaterialTypes + 1; i <= DredgedFillMaterialTypes + ContinuationDredgedFillMaterialTypes; i++) {
					EatLineNumber();
					var id = Io.ReadInt(IN);
					SpecificGravities[id] = Io.ReadDouble(IN);
					CaCcs[id] = Io.ReadDouble(IN);
					CrCcs[id] = Io.ReadDouble(IN);
					DredgedFillDesiccationLimits[id] = Io.ReadDouble(IN);
					DredgedFillSaturationLimits[id] = Io.ReadDouble(IN);
					DredgedFillDryingMaxDepth[id] = Io.ReadDouble(IN);
					DredgedFillAverageSaturation[id] = Io.ReadDouble(IN);
					RelationDefinitionLines[id] = Io.ReadInt(IN);

					MaterialIDs[CompressibleFoundationMaterialTypes + i] = id;
					Cmd.WriteLine(
						"MaterialID: {0}, SpecificGravities: {1}, CaCcs: {2}, CrCcs: {3}, DredgedFillDesiccationLimits: {4}" +
						"\n\t DredgedFillSaturationLimits: {5}, DredgedFillDryingMaxDepth: {6}, DredgedFillAverageSaturation: {7}" +
						"\n\t RelationDefinitionLines: {8}",
						id, SpecificGravities[id], CaCcs[id], CrCcs[id], DredgedFillDesiccationLimits[id],
						DredgedFillSaturationLimits[id], DredgedFillDryingMaxDepth[id], DredgedFillAverageSaturation[id],
						RelationDefinitionLines[id]);

					for (int j = 1; j <= RelationDefinitionLines[id]; j++) {
						EatLineNumber();
						VoidRatios[j, id] = Io.ReadDouble(IN);
						EffectiveStresses[j, id] = Io.ReadDouble(IN);
						Permeabilities[j, id] = Io.ReadDouble(IN);
						Cmd.WriteLine(
							"VoidRatios: {0}, EffectiveStresses: {1}, Permeabilities: {2}",
							VoidRatios[j, id], EffectiveStresses[j, id], Permeabilities[j, id]);
					}
				}
				DredgedFillMaterialTypes += ContinuationDredgedFillMaterialTypes;

				Cmd.WriteLine("\nRead print options...");
				StartPrintTime = PrintTimes + 1;
				PrintTimes += ContinuationPrintTimes;
				DredgedFillLayers = 1;
				for (int i = StartPrintTime; i <= PrintTimes; i++) {
					EatLineNumber();
					PrintTimeDates[i] = Io.ReadDouble(IN);
					NewDredgedFillInitialThicknesses[i] = Io.ReadDouble(IN);
					NewDredgedFillDesiccationDelayDays[i] = Io.ReadDouble(IN);
					NewDredgedFillDesiccationDelayMonths[i] = Io.ReadInt(IN);
					NewDredgedFillPrintOptions[i] = Io.ReadInt(IN);
					Cmd.WriteLine(
						"PrintTimeDates: {0}, NewDredgedFillInitialThicknesses: {1}, NewDredgedFillDesiccationDelayDays: {2}" +
						"\n\t NewDredgedFillDesiccationDelayMonths: {3}, NewDredgedFillPrintOptions: {4}",
						PrintTimeDates[i], NewDredgedFillInitialThicknesses[i], NewDredgedFillDesiccationDelayDays[i],
						NewDredgedFillDesiccationDelayMonths[i], NewDredgedFillPrintOptions[i]);


					if (NewDredgedFillInitialThicknesses[i] != 0) {
						// New layer has been added;
						Cmd.WriteLine("\t Read new layer information...");
						DredgedFillLayers++;
						DredgedFillInitialVoidRatios[DredgedFillLayers] = Io.ReadDouble(IN);
						DredgedFillMaterialIDs[DredgedFillLayers] = Io.ReadInt(IN);
						DredgedFillSublayers[DredgedFillLayers] = Io.ReadInt(IN);

						DredgedFillInitialThicknesses[DredgedFillLayers] = NewDredgedFillInitialThicknesses[i];
						Cmd.WriteLine(
							"\t DredgedFillInitialVoidRatios: {0}, DredgedFillMaterialIDs: {1}, DredgedFillSublayers: {2}",
							DredgedFillInitialVoidRatios[DredgedFillLayers], DredgedFillMaterialIDs[DredgedFillLayers], DredgedFillSublayers[DredgedFillLayers]);
					}
				}
			}

			if (!Io.EndOfFile(IN))
				throw new System.Exception("Input file corrupted");

			Io.CloseRead(IN);
			Cmd.WriteLine("\nRead input file complete");
		}
	}
}

