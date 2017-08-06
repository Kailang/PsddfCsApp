using PsddfCs.Exception;

namespace PsddfCs {
	public partial class Psddf {
		void Adjust (int d1, int d2, int ii, int id, int total_point, double[,] voidratio, ref bool flag) {
			/*
			! Local Variables
			integer:: i
			real*8:: new_void(d1, d2), slope
			! cont = determine if the user wishes to continue or stop program
			character*1:: cont
			*/
			int i;
			double slope;
			var new_void = new double[d1, d2];

			const string f2001 =
				"NOTE: The initial void ratio specified for material number {0,2} is less\n" +
				"      than the void ratio at zero effective stress in the specified e-log p'\n" +
				"      relationship.  Therefore the e-log p' relationship has been adjusted\n" +
				"      downward such that the initial void ratio equals the void ratio\n" +
				"      at zero effective stress (See User's Guide Section 3)";
			const string f2003 =
				"ERROR: The e-log p' relationship for material {0,2} has been adjusted once;\n" +
				"       therefore, the initial void ratio equals the void ratio at zero\n" +
				"       effective stress.  As a result, this e-log p' relationship cannot\n" +
				"       be adjusted again because one material type is already using the\n" +
				"       relationship.  As a result, a new material type needs to be identified\n" +
				"       with a new e-log p' relationship so the intial void ratio at zero\n" +
				"       effective stress.  (See User's Guide Section 3)";

			if (flag) {
				flag = false;
				// This prevents altering the curve a second time;
				// write (*, 2001) id;
				Cmd.WriteLine(f2001, id);
				// Calcuration of slope of void ratio change: Choi's method;
				slope = (DredgedFillInitialVoidRatios[ii] - voidratio[total_point, id]) / (voidratio[1, id] - voidratio[total_point, id]);
				for (i = 1; i <= total_point; i++) {
					new_void[i, id] = DredgedFillInitialVoidRatios[ii] - slope * (voidratio[1, id] - voidratio[i, id]);
				}
			} else {
				// write(*, 2003) id;
				Cmd.WriteLine(f2003, id);
				// Notify User the curve has already been altered once;
			}

			for (i = 1; i <= total_point; i++) {
				voidratio[i, id] = new_void[i, id];
			}
		}
	}
}

