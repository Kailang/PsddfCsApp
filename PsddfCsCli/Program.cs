using System;

using PsddfCs;

namespace PsddfCsCli {
	class Program {
		const int IN = Psddf.IN;

		static string InputFilePath = "test.psi";

		static ICmd Cmd = new Cmd ();
		static IIo Io = new Io();

		static void EatLineNumber () {
			Cmd.Write("Line: {0} \t", Io.ReadInt(IN));
		}

		static void Main (string[] args) {
			var p = new Psddf(Cmd, Io);

			// set input file
			if (args.Length > 0)
				InputFilePath = args[0];
		
			if (!System.IO.File.Exists(InputFilePath))
				throw new Exception("Input file does not exist: " + InputFilePath);

			// set output file
			if (args.Length > 1) {
				if (args[1].ToLower() == "-s")
					p.IsPrintProcess = true;
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
					"\n\tIncompressibleFoudationElevation: {3}, ExternalWaterSurfaceElevation: {4}, WaterUnitWeight: {5}, " +
					"\n\tSecondaryCompressionExcessPoreWaterPressureLimit: {6}",
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

				Cmd.WriteLine("\nRead Compressible Foundation layer information...");
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

				Cmd.WriteLine("\nRead Compressible Foundation material information...");
				for (int i = 1; i <= p.CompressibleFoundationMaterialTypes; i++) {
					EatLineNumber();
					var id = Io.ReadInt(IN);
					p.SpecificGravities[id] = Io.ReadDouble(IN);
					p.CaCcs[id] = Io.ReadDouble(IN);
					p.CrCcs[id] = Io.ReadDouble(IN);
					p.RelationDefinitionLines[id] = Io.ReadInt(IN);
					p.nmat[i] = id;
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


			}
























			System.Console.WriteLine("\nPress enter to exit...");
			System.Console.ReadLine();
		}
	}
}
