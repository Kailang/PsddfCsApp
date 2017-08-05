using PsddfCs;

namespace PsddfCsCli {
	class Program {
		static void Main (string[] args) {
			var psddf = new Psddf(new Cmd(), new Io());
			psddf.Main(args.Length != 0 ? args : new[] { @"D:\Misc\psddf\test.psi" });

			System.Console.WriteLine("\nPress enter to exit...");
			System.Console.ReadLine();
		}
	}
}
