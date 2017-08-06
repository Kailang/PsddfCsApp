using System;

using PsddfCs;

namespace PsddfCsCli {
	class Program {
		const int IN = Psddf.IN;

		static string InputFilePath = "test2c.psi";
		static string ContinuationFilePath = "temp_contfile.psc";

		static ICmd Cmd = new Cmd ();
		static IIo Io = new Io();

		static void EatLineNumber () {
			Cmd.Write("Line: {0} \t", Io.ReadInt(IN));
		}

		static void Main (string[] args) {
			var p = new Psddf(Cmd, Io);

			//p.Main(new[] { InputFilePath }); return;

			// set input file
			if (args.Length > 0)
				InputFilePath = args[0];
		
			if (!System.IO.File.Exists(InputFilePath))
				throw new Exception("Input file does not exist: " + InputFilePath);

			// set output file
			if (args.Length > 1) {
				if (args[1].ToLower() == "-s")
					p.IsPrintProgress = true;
				else
					p.OutputFilePath = args[1];
			}

			Cmd.WriteLine(
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
	"                ************************************************");

			Cmd.WriteLine("Read input file {0}...", InputFilePath);
			Io.OpenRead(IN, InputFilePath);

			EatLineNumber();
			p.ProblemName = Io.ReadString(IN);
			p.IsNewSimulation = Io.ReadInt(IN);
			p.IsNotSaveContinuation = Io.ReadInt(IN);
			Cmd.WriteLine(
				"ProblemName: {0}, IsNewSimulation: {1}, IsNotSaveContinuation: {2}",
				p.ProblemName, p.IsNewSimulation, p.IsNotSaveContinuation);

			if (p.IsNewSimulation == 1) {
				// new simulation
				Cmd.WriteLine("\nNew simulation");

				EatLineNumber();
				p.SimulationPrintOption = Io.ReadInt(IN);
				p.IsSaveRecovery = Io.ReadInt(IN);
				p.IsEnglishUnit = Io.ReadInt(IN);
				Cmd.WriteLine(
					"SimulationPrintOption: {0}, IsSaveRecovery: {1}, IsEnglishUnit: {2}",
					p.SimulationPrintOption, p.IsSaveRecovery, p.IsEnglishUnit);

				EatLineNumber();
				p.IncompressibleFoudationVoidRatio = Io.ReadDouble(IN);
				p.IncompressibleFoudationPermeability = Io.ReadDouble(IN);
				p.IncompressibleFoudationDrainagePathLength = Io.ReadDouble(IN);
				p.IncompressibleFoudationElevation = Io.ReadDouble(IN);
				p.ExternalWaterSurfaceElevation = Io.ReadDouble(IN);
				p.WaterUnitWeight = Io.ReadDouble(IN);
				p.SecondaryCompressionExcessPoreWaterPressureLimit = Io.ReadDouble(IN);
				Cmd.WriteLine(
					"IncompressibleFoudationVoidRatio: {0}, IncompressibleFoudationPermeability: {1}, IncompressibleFoudationDrainagePathLength: {2}, " +
					"\n\t IncompressibleFoudationElevation: {3}, ExternalWaterSurfaceElevation: {4}, WaterUnitWeight: {5}, " +
					"\n\t SecondaryCompressionExcessPoreWaterPressureLimit: {6}",
					p.IncompressibleFoudationVoidRatio, p.IncompressibleFoudationPermeability, p.IncompressibleFoudationDrainagePathLength,
					p.IncompressibleFoudationElevation, p.ExternalWaterSurfaceElevation, p.WaterUnitWeight,
					p.SecondaryCompressionExcessPoreWaterPressureLimit);

				EatLineNumber();
				p.CompressibleFoundationLayers = Io.ReadInt(IN);
				p.CompressibleFoundationMaterialTypes = Io.ReadInt(IN);
				p.DredgedFillMaterialTypes = Io.ReadInt(IN);
				Cmd.WriteLine(
					"CompressibleFoundationLayers: {0}, CompressibleFoundationMaterialTypes: {1}, DredgedFillMaterialTypes: {2}",
					p.CompressibleFoundationLayers, p.CompressibleFoundationMaterialTypes, p.DredgedFillMaterialTypes);

				Cmd.WriteLine("\nRead compressible foundation layer information...");
				for (int i = 1; i <= p.CompressibleFoundationLayers; i++) {
					EatLineNumber();
					p.CompressibleFoundationInitialThicknesses[i] = Io.ReadDouble(IN);
					p.CompressibleFoundationMaterialIDs[i] = Io.ReadInt(IN);
					p.CompressibleFoundationSublayers[i] = Io.ReadInt(IN);
					p.CompressibleFoundationOCR[i] = Io.ReadDouble(IN);
					Cmd.WriteLine(
						"CompressibleFoundationInitialThicknesses: {0}, CompressibleFoundationMaterialIDs: {1}, CompressibleFoundationSublayers: {2}, CompressibleFoundationOCR: {3}",
						p.CompressibleFoundationInitialThicknesses[i], p.CompressibleFoundationMaterialIDs[i], p.CompressibleFoundationSublayers[i], p.CompressibleFoundationOCR[i]);
				}

				Cmd.WriteLine("\nRead compressible foundation material information...");
				for (int i = 1; i <= p.CompressibleFoundationMaterialTypes; i++) {
					EatLineNumber();
					var id = Io.ReadInt(IN);
					p.SpecificGravities[id] = Io.ReadDouble(IN);
					p.CaCcs[id] = Io.ReadDouble(IN);
					p.CrCcs[id] = Io.ReadDouble(IN);
					p.RelationDefinitionLines[id] = Io.ReadInt(IN);

					p.MaterialIDs[i] = id;
					Cmd.WriteLine(
						"MaterialID: {0}, SpecificGravities: {1}, CaCcs: {2}, CrCcs: {3}, RelationDefinitionLines: {4}",
						id, p.SpecificGravities[id], p.CaCcs[id], p.CrCcs[id], p.RelationDefinitionLines[id]);

					for (int j = 1; j <= p.RelationDefinitionLines[id]; j++) {
						EatLineNumber();
						p.VoidRatios[j, id] = Io.ReadDouble(IN);
						p.EffectiveStresses[j, id] = Io.ReadDouble(IN);
						p.Permeabilities[j, id] = Io.ReadDouble(IN);
						Cmd.WriteLine(
							"VoidRatios: {0}, EffectiveStresses: {1}, Permeabilities: {2}",
							p.VoidRatios[j, id], p.EffectiveStresses[j, id], p.Permeabilities[j, id]);
					}
				}

				Cmd.WriteLine("\nRead dredged fill material information...");
				for (int i = 1; i <= p.DredgedFillMaterialTypes; i++) {
					EatLineNumber();
					var id = Io.ReadInt(IN);
					p.SpecificGravities[id] = Io.ReadDouble(IN);
					p.CaCcs[id] = Io.ReadDouble(IN);
					p.CrCcs[id] = Io.ReadDouble(IN);
					p.DredgedFillDesiccationLimits[id] = Io.ReadDouble(IN);
					p.DredgedFillSaturationLimits[id] = Io.ReadDouble(IN);
					p.DredgedFillDryingMaxDepth[id] = Io.ReadDouble(IN);
					p.DredgedFillAverageSaturation[id] = Io.ReadDouble(IN);
					p.RelationDefinitionLines[id] = Io.ReadInt(IN);

					p.MaterialIDs[p.CompressibleFoundationMaterialTypes + i] = id;
					Cmd.WriteLine(
						"MaterialID: {0}, SpecificGravities: {1}, CaCcs: {2}, CrCcs: {3}, DredgedFillDesiccationLimits: {4}" +
						"\n\t DredgedFillSaturationLimits: {5}, DredgedFillDryingMaxDepth: {6}, DredgedFillAverageSaturation: {7}" +
						"\n\t RelationDefinitionLines: {8}",
						id, p.SpecificGravities[id], p.CaCcs[id], p.CrCcs[id], p.DredgedFillDesiccationLimits[id],
						p.DredgedFillSaturationLimits[id], p.DredgedFillDryingMaxDepth[id], p.DredgedFillAverageSaturation[id],
						p.RelationDefinitionLines[id]);

					for (int j = 1; j <= p.RelationDefinitionLines[id]; j++) {
						EatLineNumber();
						p.VoidRatios[j, id] = Io.ReadDouble(IN);
						p.EffectiveStresses[j, id] = Io.ReadDouble(IN);
						p.Permeabilities[j, id] = Io.ReadDouble(IN);
						Cmd.WriteLine(
							"VoidRatios: {0}, EffectiveStresses: {1}, Permeabilities: {2}",
							p.VoidRatios[j, id], p.EffectiveStresses[j, id], p.Permeabilities[j, id]);
					}
				}

				Cmd.WriteLine("\nRead print times...");
				EatLineNumber();
				p.PrintTimes = Io.ReadInt(IN) - 1;
				Cmd.WriteLine("PrintTimes: " + p.PrintTimes);

				Cmd.WriteLine("\nRead initial dredged fill layer information...");
				EatLineNumber();
				p.DredgedFillInitialThicknesses[1] = Io.ReadDouble(IN);
				p.DredgedFillDesiccationDelayDays = Io.ReadDouble(IN);
				p.DredgedFillDesiccationDelayMonths = Io.ReadInt(IN);
				p.DredgedFillPrintOption = Io.ReadInt(IN);
				p.DredgedFillInitialVoidRatios[1] = Io.ReadDouble(IN);
				p.DredgedFillMaterialIDs[1] = Io.ReadInt(IN);
				p.DredgedFillSublayers[1] = Io.ReadInt(IN);
				Cmd.WriteLine(
					"DredgedFillInitialThicknesses: {0}, DredgedFillDesiccationDelayDays: {1}, DredgedFillDesiccationDelayMonths: {2}, " +
					"\n\t DredgedFillPrintOption: {3}, DredgedFillInitialVoidRatios: {4}, DredgedFillMaterialIDs: {5}, DredgedFillSublayers: {6}",
					p.DredgedFillInitialThicknesses[1], p.DredgedFillDesiccationDelayDays, p.DredgedFillDesiccationDelayMonths,
					p.DredgedFillPrintOption, p.DredgedFillInitialVoidRatios[1], p.DredgedFillMaterialIDs[1], p.DredgedFillSublayers[1]);

				Cmd.WriteLine("\nRead print options...");
				p.DredgedFillLayers = 1;
				for (int i = 1; i <= p.PrintTimes; i++) {
					EatLineNumber();
					p.PrintTimeDates[i] = Io.ReadDouble(IN);
					p.NewDredgedFillInitialThicknesses[i] = Io.ReadDouble(IN);
					p.NewDredgedFillDesiccationDelayDays[i] = Io.ReadDouble(IN);
					p.NewDredgedFillDesiccationDelayMonths[i] = Io.ReadInt(IN);
					p.NewDredgedFillPrintOptions[i] = Io.ReadInt(IN);
					Cmd.WriteLine(
						"PrintTimeDates: {0}, NewDredgedFillInitialThicknesses: {1}, NewDredgedFillDesiccationDelayDays: {2}" + 
						"\n\t NewDredgedFillDesiccationDelayMonths: {3}, NewDredgedFillPrintOptions: {4}",
						p.PrintTimeDates[i], p.NewDredgedFillInitialThicknesses[i], p.NewDredgedFillDesiccationDelayDays[i],
						p.NewDredgedFillDesiccationDelayMonths[i], p.NewDredgedFillPrintOptions[i]);


					if (p.NewDredgedFillInitialThicknesses[i] != 0) {
						// New layer has been added;
						Cmd.WriteLine("\t Read new layer information...");
						p.DredgedFillLayers++;
						p.DredgedFillInitialVoidRatios[p.DredgedFillLayers] = Io.ReadDouble(IN);
						p.DredgedFillMaterialIDs[p.DredgedFillLayers] = Io.ReadInt(IN);
						p.DredgedFillSublayers[p.DredgedFillLayers] = Io.ReadInt(IN);

						p.DredgedFillInitialThicknesses[p.DredgedFillLayers] = p.NewDredgedFillInitialThicknesses[i];
						Cmd.WriteLine(
							"\t DredgedFillInitialVoidRatios: {0}, DredgedFillMaterialIDs: {1}, DredgedFillSublayers: {2}",
							p.DredgedFillInitialVoidRatios[p.DredgedFillLayers], p.DredgedFillMaterialIDs[p.DredgedFillLayers], p.DredgedFillSublayers[p.DredgedFillLayers]);
					}
				}

				Cmd.WriteLine("\nRead precipitation and evaportation data...");
				EatLineNumber();
				p.DaysInMonth = Io.ReadDouble(IN);
				p.SurfaceDrainageEfficiencyFactor = Io.ReadDouble(IN);
				p.MaxDredgedFillEvaporationEfficiency = Io.ReadDouble(IN);
				Cmd.WriteLine(
					"DaysInMonth: {0}, SurfaceDrainageEfficiencyFactor: {1}, MaxDredgedFillEvaporationEfficiency: {2}",
					p.DaysInMonth, p.SurfaceDrainageEfficiencyFactor, p.MaxDredgedFillEvaporationEfficiency);

				for (int i = 1; i <= 12; i++) {
					EatLineNumber();
					p.MaxEnvironmentalPotentialEvaporation[i] = Io.ReadDouble(IN);
					p.AverageMonthlyRainfall[i] = Io.ReadDouble(IN);
					Cmd.WriteLine(
						"Month: {0}, MaxEnvironmentalPotentialEvaporation: {1}, AverageMonthlyRainfall: {2}",
						i, p.MaxEnvironmentalPotentialEvaporation[i], p.AverageMonthlyRainfall[i]);
				}

				p.ResetSimulation();
			} else {
				// continuation file
				Cmd.WriteLine("\nContinuation file");

				EatLineNumber();
				p.ContinuationPrintTimes = Io.ReadInt(IN);
				p.ContinuationDredgedFillMaterialTypes = Io.ReadInt(IN);
				Cmd.WriteLine(
					"ContinuationPrintTimes: {0}, ContinuationDredgedFillMaterialTypes: {1}",
					p.ContinuationPrintTimes, p.ContinuationDredgedFillMaterialTypes);

				Cmd.WriteLine("\nRestore continuation status...");
				p.RestoreContinuation(ContinuationFilePath);
				Cmd.WriteLine("Restore continuation status complete");

				Cmd.WriteLine("\nRead dredged fill material information...");
				for (int i = p.DredgedFillMaterialTypes + 1; i <= p.DredgedFillMaterialTypes + p.ContinuationDredgedFillMaterialTypes; i++) {
					EatLineNumber();
					var id = Io.ReadInt(IN);
					p.SpecificGravities[id] = Io.ReadDouble(IN);
					p.CaCcs[id] = Io.ReadDouble(IN);
					p.CrCcs[id] = Io.ReadDouble(IN);
					p.DredgedFillDesiccationLimits[id] = Io.ReadDouble(IN);
					p.DredgedFillSaturationLimits[id] = Io.ReadDouble(IN);
					p.DredgedFillDryingMaxDepth[id] = Io.ReadDouble(IN);
					p.DredgedFillAverageSaturation[id] = Io.ReadDouble(IN);
					p.RelationDefinitionLines[id] = Io.ReadInt(IN);

					p.MaterialIDs[p.CompressibleFoundationMaterialTypes + i] = id;
					Cmd.WriteLine(
						"MaterialID: {0}, SpecificGravities: {1}, CaCcs: {2}, CrCcs: {3}, DredgedFillDesiccationLimits: {4}" +
						"\n\t DredgedFillSaturationLimits: {5}, DredgedFillDryingMaxDepth: {6}, DredgedFillAverageSaturation: {7}" +
						"\n\t RelationDefinitionLines: {8}",
						id, p.SpecificGravities[id], p.CaCcs[id], p.CrCcs[id], p.DredgedFillDesiccationLimits[id],
						p.DredgedFillSaturationLimits[id], p.DredgedFillDryingMaxDepth[id], p.DredgedFillAverageSaturation[id],
						p.RelationDefinitionLines[id]);

					for (int j = 1; j <= p.RelationDefinitionLines[id]; j++) {
						EatLineNumber();
						p.VoidRatios[j, id] = Io.ReadDouble(IN);
						p.EffectiveStresses[j, id] = Io.ReadDouble(IN);
						p.Permeabilities[j, id] = Io.ReadDouble(IN);
						Cmd.WriteLine(
							"VoidRatios: {0}, EffectiveStresses: {1}, Permeabilities: {2}",
							p.VoidRatios[j, id], p.EffectiveStresses[j, id], p.Permeabilities[j, id]);
					}
				}
				p.DredgedFillMaterialTypes += p.ContinuationDredgedFillMaterialTypes;

				Cmd.WriteLine("\nRead print options...");
				p.StartPrintTime = p.PrintTimes + 1;
				p.PrintTimes += p.ContinuationPrintTimes;
				p.DredgedFillLayers = 1;
				for (int i = p.StartPrintTime; i <= p.PrintTimes; i++) {
					EatLineNumber();
					p.PrintTimeDates[i] = Io.ReadDouble(IN);
					p.NewDredgedFillInitialThicknesses[i] = Io.ReadDouble(IN);
					p.NewDredgedFillDesiccationDelayDays[i] = Io.ReadDouble(IN);
					p.NewDredgedFillDesiccationDelayMonths[i] = Io.ReadInt(IN);
					p.NewDredgedFillPrintOptions[i] = Io.ReadInt(IN);
					Cmd.WriteLine(
						"PrintTimeDates: {0}, NewDredgedFillInitialThicknesses: {1}, NewDredgedFillDesiccationDelayDays: {2}" +
						"\n\t NewDredgedFillDesiccationDelayMonths: {3}, NewDredgedFillPrintOptions: {4}",
						p.PrintTimeDates[i], p.NewDredgedFillInitialThicknesses[i], p.NewDredgedFillDesiccationDelayDays[i],
						p.NewDredgedFillDesiccationDelayMonths[i], p.NewDredgedFillPrintOptions[i]);


					if (p.NewDredgedFillInitialThicknesses[i] != 0) {
						// New layer has been added;
						Cmd.WriteLine("\t Read new layer information...");
						p.DredgedFillLayers++;
						p.DredgedFillInitialVoidRatios[p.DredgedFillLayers] = Io.ReadDouble(IN);
						p.DredgedFillMaterialIDs[p.DredgedFillLayers] = Io.ReadInt(IN);
						p.DredgedFillSublayers[p.DredgedFillLayers] = Io.ReadInt(IN);

						p.DredgedFillInitialThicknesses[p.DredgedFillLayers] = p.NewDredgedFillInitialThicknesses[i];
						Cmd.WriteLine(
							"\t DredgedFillInitialVoidRatios: {0}, DredgedFillMaterialIDs: {1}, DredgedFillSublayers: {2}",
							p.DredgedFillInitialVoidRatios[p.DredgedFillLayers], p.DredgedFillMaterialIDs[p.DredgedFillLayers], p.DredgedFillSublayers[p.DredgedFillLayers]);
					}
				}
			}

			if (!Io.EndOfFile(IN))
				throw new Exception("Input file corrupted");

			Io.CloseRead(IN);
			Cmd.WriteLine("\nRead input file complete");

			p.InitSimulation();

			p.PrintIntro();
			if (p.SimulationPrintOption != 3) {
				p.RunSimulation();
			}

			if (p.IsNotSaveContinuation == 2) {
				p.SaveContinuation(ContinuationFilePath);
			}

			p.EndSimulation();

			Cmd.WriteLine("\n============================ Simulation Completed ==============================");
			Cmd.Write("\nPress enter to exit...");
			Console.ReadLine();
		}
	}
}
