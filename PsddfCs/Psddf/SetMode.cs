using PsddfCs.Exception;

namespace PsddfCs {
	public partial class Psddf {
		/// <summary>
		/// Set fname1 and lstat using args from console.
		/// </summary>
		public void SetMode (string[] args) {
			if (args.Length < 1)
				throw new ConsoleException("Too few arugments.");

			InputFilePath = args[0];
			if (!System.IO.File.Exists(InputFilePath))
				throw new ConsoleException("Input file does not exist in {0}.", InputFilePath);

			// If the second args is "-s", output progress;
			IsPrintProcess = args.Length > 1 && args[1].ToLower() == "-s";

//			const string f100 =
//				" ********************************************************* \n" +
//				" *                                                       * \n" +
//				" *          PSDDF: Primary Consolidation                 * \n" +
//				" *                 Secondary Compression                 * \n" +
//				" *            and Desiccation of Dredged Fill            * \n" +
//				" *                                                       * \n" +
//				" *        by: Timothy D. Stark & Hangseok Choi           * \n" +
//				" *       University of Illinois at Urbana-Champaign      * \n" +
//				" *                                                       * \n" +
//				" *                      Version 3.1                      * \n" +
//				" *                      April, 2011                      * \n" +
//				" *********************************************************";

			const string f101 =
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
				"                ************************************************";

			Cmd.WriteLine(f101);
		}
	}
}

