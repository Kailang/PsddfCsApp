namespace PsddfCs {
	public partial class Psddf {
		/// <summary>
		/// Init status
		/// </summary>
		public void RunSimulation () {
			Cmd.WriteLine("\nRun simulation...");

			for (int i = StartPrintTime; i <= PrintTimes; i++) {
				tprint = PrintTimeDates[i];
				DredgedFillPrintOption = NewDredgedFillPrintOptions[i];
				add = NewDredgedFillInitialThicknesses[i];

				// hdf1, tadd, tds, and ms is set for the first layer of dredged fill;
				if (i != 1) {
					hdf1 = NewDredgedFillInitialThicknesses[i - 1];
					tadd = tadd + hdf1;
					DredgedFillDesiccationDelayDays = NewDredgedFillDesiccationDelayDays[i - 1];
					DredgedFillDesiccationDelayMonths = NewDredgedFillDesiccationDelayMonths[i - 1];
					Reset(dim1);
				}

				Fdifq(dim1);
				Stress(dim1);

				Dataout();
				// Create Recovery input file from PSDDF output;
				// The file will take the output matricies and create the recovery input;
				if (IsSaveRecovery == 1) {
					// Only generate recovery output if needed;
					Recovery();
				}

				OnPrintTimeReached?.Invoke(this);
			}
		}
	}
}
