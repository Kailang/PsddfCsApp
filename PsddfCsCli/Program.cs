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

			p.ReadInput(InputFilePath, ContinuationFilePath);

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

			p.SaveInput(InputFilePath);

			p.ResetSimulation();

			p.ReadInput(InputFilePath, ContinuationFilePath);

			p.InitSimulation();

			p.PrintIntro();

			if (p.SimulationPrintOption != 3) {
				p.RunSimulation();
			}

			if (p.IsNotSaveContinuation == 2) {
				p.SaveContinuation(ContinuationFilePath);
			}

			p.EndSimulation();
		}
	}
}
