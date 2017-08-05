﻿namespace PsddfCs {
	public partial class Psddf {
		/// <summary>
		/// Create output files and open output streams.
		/// </summary>
		public void Ifn () {
			/*
    eof = 0
    prev = 99

    name = 'p_temp_opt'
			*/
			const string name = "p_temp_opt";
			const string temp = "temp_contfile";

			/*
    fname(1) = name(1:i) // '.pso'  ! Output/Calculations
    fname(2) = name(1:i) // '.psp'  ! Surface Elevation data
    fname(3) = name(1:i) // '.psc'  !!Stores the output from a previous simulation so that it can be restarted
    fname(4) = name(1:i) // '.pgc'  ! Compressible Foundation Results
    fname(5) = name(1:i) // '.pgd'  ! Dredged Material Results
    fname(6) = name(1:i) // '.rcy'  !!The file generated by PSDDF for use with the CAP model

    open(iout, file = fname(1))
    open(iplot, file = fname(2))
    open(igracf, file = fname(4))
    open(igradf, file = fname(5))
    open(recoveryout, file = fname(6))
			*/
			// Output/Calculations
			Io.OpenWrite(iout, name + ".pso");
			// Surface Elevation data
			Io.OpenWrite(iplot, name + ".psp");
			// Compressible Foundation Results
			Io.OpenWrite(igracf, name + ".pgc");
			// Dredged Material Results
			Io.OpenWrite(igradf, name + ".pgd");
			// The file generated by PSDDF for use with the CAP model
			Io.OpenWrite(recoveryout, name + ".rcy");

			/*
    if (ndata2 == 2) then  ! Output saving for continuation
        open(iouts, file = fname(3), form = 'unformatted', access = 'stream')
    endif
			*/
			if (IsNotSaveContinuation == 2) {
				// Output saving for continuation
				Io.OpenWrite(iouts, name + ".psc");
			}
				
			/*
    if (ndata1 == 2) then  ! For continuation file
        name = 'temp_contfile'
        i = 1
        do while (name(i:i) /= ' ' .and. name(i:i) /= '.')
            i = i + 1
        enddo
        i = i - 1;

        fname(3) = name(1:i) // '.psc'
        open(ins, file = fname(3), form = 'unformatted', access = 'stream')

        name = name(1:i) // '.psp'
        open(prev, file = name)
        do while (eof /= -1)
            read(prev, *, iostat = eof) void1, void2
            write(iplot, '(2f15.5)') void1, void2
        enddo
        close (prev)
    endif
			*/
			if (IsNewSimulation == 2) {
				// For continuation file
				Io.OpenRead(ins, temp + ".psc");

				const int prev = 99;
				Io.OpenRead(prev, temp + ".psp");
				while (!Io.EndOfFile(prev)) {
					// write(iplot, '(2f15.5)') void1, void2
					Io.WriteLine(
						iplot, 
						"{0,15:F5}{1,15:F5}", 
						Io.ReadDouble(prev), 
						Io.ReadDouble(prev));
				}
				Io.CloseRead(prev);
			}
		}
	}
}

