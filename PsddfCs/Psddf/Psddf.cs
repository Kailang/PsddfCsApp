﻿namespace PsddfCs {
	public partial class Psddf {
		public Psddf (ICmd cmd, IIo io) {
			Cmd = cmd;
			Io = io;
		}

		/// <summary>
		/// ndff = 1 if not a restart loop.
		/// </summary>
		int ndff;

		public void Main (string[] args) {
			double tds1 = 0;
			int id = 0;

			for (int i = 1; i <= DredgedFillMaxLayers; i++) af[i] = 0;
			// Set flag to normally consolidated;
			for (int i = 1; i <= MaxMaterialTypes; i++) CompressibleFoundationOCR[i] = 1;
			// Clear adjustflags ;
			for (int i = 1; i <= DredgedFillMaxLayers; i++) IsCurveNotAdjusteds[i] = true;
			ndff = 1;

			// Get the name of the file holding the input data
			SetMode(args);

			/*
    ! Open the input file and read the data into the program
    open(in, file = fname1)
    ! ndata1 = 1 for new simulation; = 2 for continuation file
    ! ndata2 = 1 for output not saved; = 2 for output saved for continuation
    read(in, *) line, problemname, ndata1, ndata2
			*/
			Io.OpenRead(IN, InputFilePath);
//			Cmd.WriteLine("Open the input file and read the data into the program");
			Io.ReadInt(IN);
			ProblemName = Io.ReadString(IN);
			IsNewSimulation = Io.ReadInt(IN);
			IsNotSaveContinuation = Io.ReadInt(IN);

			/*
    ! ifn will name all the files used for output
    call ifn(ndata1, ndata2, iout, iplot, iouts, ins, igracf, igradf, recoveryout)
			*/
			Ifn();

			if (IsNewSimulation == 1) {
				// New simulation ---------------------------------------------------------

				/*
        ! Read Flag to determine if a recovery out put is needed (new in version 2.1)
        ! recoveryflag will tell the program to write a recovery output file (1 for recovery)
        ! dimensionflag will tell recovery to convert from feet to meters (1 for conversion)
        read(in, *) line, npt, recoveryflag, dimensionflag
				*/
//				Cmd.WriteLine("Read Flag to determine if a recovery out put is needed");
				Io.ReadInt(IN);
				SimulationPrintOption = Io.ReadInt(IN);
				IsSaveRecovery = Io.ReadInt(IN);
				IsEnglishUnit = Io.ReadInt(IN);

				/*
        ! Consolidation Calculation data
        read(in, *) line, e0, zk0, du0, xel, wtelev, gw, tol
				*/
//				Cmd.WriteLine("Consolidation Calculation data");
				Io.ReadInt(IN);
				IncompressibleFoudationVoidRatio = Io.ReadDouble(IN);
				IncompressibleFoudationPermeability = Io.ReadDouble(IN);
				IncompressibleFoudationDrainagePathLength = Io.ReadDouble(IN);
				IncompressibleFoudationElevation = Io.ReadDouble(IN);
				ExternalWaterSurfaceElevation = Io.ReadDouble(IN);
				WaterUnitWeight = Io.ReadDouble(IN);
				SecondaryCompressionExcessPoreWaterPressureLimit = Io.ReadDouble(IN);

				/*
        ! Soil data for foundation layer or soft layer
        ! numbl = number of layers in comprssible foundation (zero when incompressible)
        ! ntypescompress = number of different material types in compressible foundation (zero for incompressible)
        ! ntypedredge = number of different dredged fill material types
        read(in, *)line, numbl, ntypescompress, ntypedredge
				*/
//				Cmd.WriteLine("Soil data for foundation layer or soft layer");
				Io.ReadInt(IN);
				CompressibleFoundationLayers = Io.ReadInt(IN);
				CompressibleFoundationMaterialTypes = Io.ReadInt(IN);
				DredgedFillMaterialTypes = Io.ReadInt(IN);

				/*
        if (numbl == 0) then
            nbl = 2 ! Noncompressible foundation
            ntypescompress = 0
        else
            nbl = 1 ! Compressible foundation
        endif
				*/
				if (CompressibleFoundationLayers == 0) {
					// Noncompressible foundation;
					IsFoundationCompressible = 2;
					CompressibleFoundationMaterialTypes = 0;
				} else {
					// Compressible foundation;
					IsFoundationCompressible = 1;
				}

				/*
        ! Read from input file for compressible foundation
        ! numbl = number of layers in comprssible foundation (zero when incompressible)
        do kk = 1, numbl
            read(in, *) line, hbl(kk), idbl(kk), nsub1(kk), OCR(kk)
        enddo
				*/
//				Cmd.WriteLine("Read from input file for compressible foundation");
				for (int i = 1; i <= CompressibleFoundationLayers; i++) {
					Io.ReadInt(IN);
					CompressibleFoundationInitialThicknesses[i] = Io.ReadDouble(IN);
					CompressibleFoundationMaterialIDs[i] = Io.ReadInt(IN);
					CompressibleFoundationSublayers[i] = Io.ReadInt(IN);
					CompressibleFoundationOCR[i] = Io.ReadDouble(IN);
				}

				/*
        ! ntypescompress = number of different material types in compressible foundation (zero for incompressible)
        do kk = 1, ntypescompress
            read(in, *) line, kom, gsdf(kom), cacc(kom), crcc(kom), ldf(kom)
            nmat(kk) = kom

            do i = 1, ldf(kom)
                read(in, *) line, voidratio(i, kom), effectivestress(i, kom), perm(i, kom)
            enddo
        enddo
				*/
				for (int i = 1; i <= CompressibleFoundationMaterialTypes; i++) {
					Io.ReadInt(IN);
					id = Io.ReadInt(IN);
					SpecificGravities[id] = Io.ReadDouble(IN);
					CaCcs[id] = Io.ReadDouble(IN);
					CrCcs[id] = Io.ReadDouble(IN);
					RelationDefinitionLines[id] = Io.ReadInt(IN);
					MaterialIDs[i] = id;

					for (int j = 1; j <= RelationDefinitionLines[id]; j++) {
						Io.ReadInt(IN);
						VoidRatios[j, id] = Io.ReadDouble(IN);
						EffectiveStresses[j, id] = Io.ReadDouble(IN);
						Permeabilities[j, id] = Io.ReadDouble(IN);
					}
				}
					
				/*
        matindex = ntypescompress + 1 ! matindex points to a value in an array
        hhbl = 0
        do i = 1, numbl
            hhbl = hhbl + hbl(i)
        enddo
				*/
				matindex = CompressibleFoundationMaterialTypes + 1;
				for (int i = 1; i <= CompressibleFoundationLayers; i++) {
					CompressibleFoundationTotalInitialThickness = CompressibleFoundationTotalInitialThickness + CompressibleFoundationInitialThicknesses[i];
				}
			
				/*
        ! Read from input file for dredge fill
        ! ntypedredge = number of different dredged fill material types
        do kk = 1, ntypedredge
            read(in, *) line, kom, gsdf(kom), cacc(kom), crcc(kom), dl(kom), sl(kom), h2(kom), sat(kom), ldf(kom)
            nmat(matindex) = kom
            matindex = matindex + 1

            ! dl = desiccation limit
            ! sl = saturation limit in desiccation process
            ! h2 = maximum depth to which second-stage drying will occur
            ! sat = saturation (decimal number) of dredge fill material dried to the dl
            ! ldf = total number of data points of void ratio, effective stress and permeability for each material type (kom)
            do i = 1, ldf(kom)
                read(in, *) line, voidratio(i, kom), effectivestress(i, kom), perm(i, kom)
            enddo

        enddo
        hhdf = 0.0
				*/
//				Cmd.WriteLine("Read from input file for dredge fill");
				for (int i = 1; i <= DredgedFillMaterialTypes; i++) {
					Io.ReadInt(IN);
					id = Io.ReadInt(IN);
					SpecificGravities[id] = Io.ReadDouble(IN);
					CaCcs[id] = Io.ReadDouble(IN);
					CrCcs[id] = Io.ReadDouble(IN);
					DredgedFillDesiccationLimits[id] = Io.ReadDouble(IN);
					DredgedFillSaturationLimits[id] = Io.ReadDouble(IN);
					DredgedFillDryingMaxDepth[id] = Io.ReadDouble(IN);
					DredgedFillAverageSaturation[id] = Io.ReadDouble(IN);
					RelationDefinitionLines[id] = Io.ReadInt(IN);
					MaterialIDs[intx(matindex)] = id;
					matindex++;
//					const string f114 =
//						"\n" +
//						"  {0,3}   {1,7:F3}  {2,7:F3}  {3,7:F3}  {4,7:F3}    {5,7:F3}   {6,7:F3}   {7,7:F3}";
//					Cmd.WriteLine(f114, kom, gsdf[kom], cacc[kom], crcc[kom], sl[kom], dl[kom], h2[kom], sat[kom]);

					for (int j = 1; j <= RelationDefinitionLines[id]; j++) {
						Io.ReadInt(IN);
						VoidRatios[j, id] = Io.ReadDouble(IN);
						EffectiveStresses[j, id] = Io.ReadDouble(IN);
						Permeabilities[j, id] = Io.ReadDouble(IN);
					}
				}
			
				/*
        ! Read the number of print times (must be less than 1000)
        read(in, *) line, ntime
        ntime = ntime - 1
				*/
//				Cmd.WriteLine("Read the number of print times (must be less than 1000)");
				Io.ReadInt(IN);
				PrintTimes = Io.ReadInt(IN);
				PrintTimes--;

				/*
        ! Read for print times
        read(in, *) line, hdf(1), tds, ms, nsc, e00(1), iddf(1), nsub(1)
				*/
//				Cmd.WriteLine("Read for print times");
				Io.ReadInt(IN);
				DredgedFillInitialThicknesses[1] = Io.ReadDouble(IN);
				DredgedFillDesiccationDelayDays = Io.ReadDouble(IN);
				DredgedFillDesiccationDelayMonths = Io.ReadInt(IN);
				DredgedFillPrintOption = Io.ReadInt(IN);
				DredgedFillInitialVoidRatios[1] = Io.ReadDouble(IN);
				DredgedFillMaterialIDs[1] = Io.ReadInt(IN);
				DredgedFillSublayers[1] = Io.ReadInt(IN);

				/*
        kk = 1
        tds1 = tds
        do i = 1, ntime
            read(in, *) line, printt(i), ahdf(i), atds(i), nms(i), nnsc(i)

            if (ahdf(i) /= 0) then ! Check to see if new layer has been added
                kk = kk + 1
                backspace(in)
                ! Read in data for added fill
                read(in, *) line, printt(i), ahdf(i), atds(i), nms(i), nnsc(i), e00(kk), iddf(kk), nsub(kk)
                hdf(kk) = ahdf(i)
            endif
        enddo
        ! numdf = number of dredge fill deposits
        numdf = kk
				*/
				DredgedFillLayers = 1;
				tds1 = DredgedFillDesiccationDelayDays;
				for (int i = 1; i <= PrintTimes; i++) {
					Io.ReadInt(IN);
					PrintTimeDates[i] = Io.ReadDouble(IN);
					NewDredgedFillInitialThicknesses[i] = Io.ReadDouble(IN);
					NewDredgedFillDesiccationDelayDays[i] = Io.ReadDouble(IN);
					NewDredgedFillDesiccationDelayMonths[i] = Io.ReadInt(IN);
					NewDredgedFillPrintOptions[i] = Io.ReadInt(IN);

					if (NewDredgedFillInitialThicknesses[i] != 0) {
						// New layer has been added;
						DredgedFillLayers++;
						DredgedFillInitialVoidRatios[DredgedFillLayers] = Io.ReadDouble(IN);
						DredgedFillMaterialIDs[DredgedFillLayers] = Io.ReadInt(IN);
						DredgedFillSublayers[DredgedFillLayers] = Io.ReadInt(IN);
						DredgedFillInitialThicknesses[DredgedFillLayers] = NewDredgedFillInitialThicknesses[i];
					}
				}

				/*
        ! Check the initial void ratio against the void ratio at zero effective stress
        ! Must also check to see if layer is compressible
        do i = 1, kk !check all added layers
            if (e00(i) /= voidratio(1, iddf(i))) then
                call adjust(dim1, dim2, i, iddf(i), ldf(iddf(i)), voidratio, adjustflag(iddf(i)))
            endif
        enddo
				*/
//				Cmd.WriteLine("Check the initial void ratio against the void ratio at zero effective stress");
				for (int i = 1; i <= DredgedFillLayers; i++) {
					// Check all added layers;
					if (DredgedFillInitialVoidRatios[i] != VoidRatios[1, DredgedFillMaterialIDs[i]]) {
//						Cmd.WriteLine("e00[{0}] != voidratio[0, iddf[{0}]; call adjust()", i);
						Adjust(Dimension1, Dimension2, i, DredgedFillMaterialIDs[i], RelationDefinitionLines[DredgedFillMaterialIDs[i]], VoidRatios, ref IsCurveNotAdjusteds[DredgedFillMaterialIDs[i]]);
					}
				}

				/*
        ! Calculate the total height of dredge fill added
        do i = 1, numdf
            hhdf = hhdf + hdf(i)
        enddo
				*/
				for (int i = 1; i <= DredgedFillLayers; i++) {
					DredgedFillTotleInitialThickness += DredgedFillInitialThicknesses[i];
				}

				/*
        ! Desiccation calculation data
        read(in, *) line, tpm, dreff, ce
				*/
//				Cmd.WriteLine("Desiccation calculation data");
				Io.ReadInt(IN);
				DaysInMonth = Io.ReadDouble(IN);
				SurfaceDrainageEfficiencyFactor = Io.ReadDouble(IN);
				MaxDredgedFillEvaporationEfficiency = Io.ReadDouble(IN);
			
				/*
        ! Read in precipitation and evaportationdata
        do i = 1, 12
            read(in, *) line, pep(i), rf(i)
        enddo

        totaltypes = ntypescompress + ntypedredge
				*/
//				Cmd.WriteLine("Read in precipitation and evaportationdata");
				for (int i = 1; i <= 12; i++) {
					Io.ReadInt(IN);
					MaxEnvironmentalPotentialEvaporation[i] = Io.ReadDouble(IN);
					AverageMonthlyRainfall[i] = Io.ReadDouble(IN);
				}

				TotalMaterialTypes = CompressibleFoundationMaterialTypes + DredgedFillMaterialTypes;
//				Cmd.WriteLine(totaltypes);

				/*
        ! Set initial variables
        do i = 1, numbl
            primbl(i) = .true.
            tpbl(i) = 0.0
            difsecbl(i) = 0.0
        enddo

        ! Initialize array variables
        do i = 1, numdf
            primdf(i) = .true.
            tpdf(i) = 0.0
            difsecdf(i) = 0.0
        enddo

        do i = 1, npdf
            do j = 1, 15
                auxdf(j, i) = 0.0
            enddo
        enddo

        do i = 1, npbl
            do j = 1, 15
                auxbl(j, i) = 0.0
            enddo
        enddo
				*/
				for (int i = 1; i <= CompressibleFoundationLayers; i++) {
					IsCompressibleFoundationInPrimaryConsolidations[i] = true;
					tpbl[i] = 0.0;
					difsecbl[i] = 0.0;
				}

				for (int i = 1; i <= DredgedFillLayers; i++) {
					IsDredgedFillInPrimaryConsolidations[i] = true;
					tpdf[i] = 0.0;
					difsecdf[i] = 0.0;
				}

				for (int i = 1; i <= DredgedFillMaxLayers; i++) {
					for (int j = 1; j <= 15; j++) {
						auxdf[j, i] = 0.0;
					}
				}

				for (int i = 1; i <= CompressibleFoundationMaxLayers; i++) {
					for (int j = 1; j <= 15; j++) {
						auxbl[j, i] = 0.0;
					}
				}

				/*
        ! Initialize program variables
        aev = 0.0
        dsc = 0.0
        qdf = 0.0
        m = ms - 1
        dtim = tds + tpm
        setc = 0.0
        setd = 0.0
        time = 0.0
        ucon = 0.0
        sett = 0.0
        sfin = 0.0  ! Settlement at the end of primary consolidation
        ucon1 = 0.0
        sett1 = 0.0
        sfin1 = 0.0
        setsdf = 0.0
        setsbl = 0.0
        vri1 = 0.0
        nm = 1
        mm = 1
        hdf1 = 0.0
        dudz21 = 0.0
        dudz11 = 0.0
        tadd = 0.0
        ndflayer = 1
				*/
				aev = 0.0;
				dsc = 0.0;
				qdf = 0.0;
				m = DredgedFillDesiccationDelayMonths - 1;
				dtim = DredgedFillDesiccationDelayDays + DaysInMonth;
				setc = 0.0;
				DredgedFillDesiccationSettlement = 0.0;

				CurrentTime = 0.0;

				DredgedFillAverageConsolidationDegree = 0.0;
				DredgedFillTotalSettlement = 0.0;
				DredgedFillFinalSettlement = 0.0;

				CompressibleFoundationAverageConsolidationDegree = 0.0;
				CompressibleFoundationTotalSettlement = 0.0;
				CompressibleFoundationFinalSettlement = 0.0;

				DredgedFillSecondaryCompressionSettlement = 0.0;
				CompressibleFoundationSecondaryCompressionSettlement = 0.0;

				vri1 = 0.0;
				StartPrintTime = 1;
				mm = 1;
				hdf1 = 0.0;
				dudz21 = 0.0;
				dudz11 = 0.0;
				tadd = 0.0;
				ndflayer = 1;

				/*
        ! Print input data and make initial calculations
        call intro(dim1, voidratio, perm, pk, effectivestress, dsde, beta, alpha, dvds, ndff)
				*/
				Intro(Dimension1);
			} else {
				// Continuation file ----------------------------------------------------------------------
				/*
        ! Restart loop
        ! newdf = number of new dredge material types to be used
        read(in, *) line, mtime, newdf
				*/
				Io.ReadInt(IN);
				ContinuationPrintTimes = Io.ReadInt(IN);
				ContinuationDredgedFillMaterialTypes = Io.ReadInt(IN);

				/*
        call datain(dim1, voidratio, perm, effectivestress, tol)
        ntypedredge = ntypedredge + newdf
        ndff = numdf + 1
				*/
				Datain();
				DredgedFillMaterialTypes = DredgedFillMaterialTypes + ContinuationDredgedFillMaterialTypes;
				ndff = DredgedFillLayers + 1;

				/*
        ! Read in new dredge fill material types
        do kk = 1, newdf
            read(in, *) line, kom, gsdf(kom), cacc(kom), crcc(kom), dl(kom), sl(kom), h2(kom), sat(kom), ldf(kom)
            nmat(matindex) = kom
            matindex = matindex + 1
            do i = 1, ldf(kom)
                read(in, *) line, voidratio(i, kom), effectivestress(i, kom), perm(i, kom)
            enddo
        enddo
				*/
				for (int i = 1; i <= ContinuationDredgedFillMaterialTypes; i++) {
					Io.ReadInt(IN);
					id = Io.ReadInt(IN);
					SpecificGravities[id] = Io.ReadDouble(IN);
					CaCcs[id] = Io.ReadDouble(IN);
					CrCcs[id] = Io.ReadDouble(IN);
					DredgedFillDesiccationLimits[id] = Io.ReadDouble(IN);
					DredgedFillSaturationLimits[id] = Io.ReadDouble(IN);
					DredgedFillDryingMaxDepth[id] = Io.ReadDouble(IN);
					DredgedFillAverageSaturation[id] = Io.ReadDouble(IN);
					RelationDefinitionLines[id] = Io.ReadInt(IN);
					MaterialIDs[intx(matindex)] = id;
					matindex++;

					for (int j = 1; j <= RelationDefinitionLines[id]; j++) {
						Io.ReadInt(IN);
						VoidRatios[j, id] = Io.ReadDouble(IN);
						EffectiveStresses[j, id] = Io.ReadDouble(IN);
						Permeabilities[j, id] = Io.ReadDouble(IN);
					}
				}

				/*
        ! Read in new print times
        do i = nm, ntime
            read(in, *) line, printt(i), ahdf(i), atds(i), nms(i), nnsc(i)
            if (ahdf(i) /= 0) then
                numdf = numdf + 1
                primdf(numdf) = .true.
                backspace(in)
                read(in, *) line, printt(i), ahdf(i), atds(i), nms(i), nnsc(i), &
                e00(numdf), iddf(numdf), nsub(numdf)
                hdf(numdf) = ahdf(i)
                hhdf = hhdf + hdf(numdf)
            endif
        enddo
				*/
				for (int i = StartPrintTime; i <= PrintTimes; i++) {
					Io.ReadInt(IN);
					PrintTimeDates[i] = Io.ReadDouble(IN);
					NewDredgedFillInitialThicknesses[i] = Io.ReadDouble(IN);
					NewDredgedFillDesiccationDelayDays[i] = Io.ReadDouble(IN);
					NewDredgedFillDesiccationDelayMonths[i] = Io.ReadInt(IN);
					NewDredgedFillPrintOptions[i] = Io.ReadInt(IN);

					if (NewDredgedFillInitialThicknesses[i] != 0) {
						// New layer has been added;
						DredgedFillLayers++;
						IsDredgedFillInPrimaryConsolidations[DredgedFillLayers] = true;
						DredgedFillInitialVoidRatios[DredgedFillLayers] = Io.ReadDouble(IN);
						DredgedFillMaterialIDs[DredgedFillLayers] = Io.ReadInt(IN);
						DredgedFillSublayers[DredgedFillLayers] = Io.ReadInt(IN);
						DredgedFillInitialThicknesses[DredgedFillLayers] = NewDredgedFillInitialThicknesses[i];
						DredgedFillTotleInitialThickness = DredgedFillTotleInitialThickness + DredgedFillInitialThicknesses[DredgedFillLayers];
					}
				}

				/*
        do i = ndff, numdf
            dz(i) = hdf(i) / ((1+e00(i))*nsub(i))
        enddo
				*/
				for (int i = ndff; i <= DredgedFillLayers; i++) {
					dz[i] = DredgedFillInitialThicknesses[i] / ((1 + DredgedFillInitialVoidRatios[i]) * DredgedFillSublayers[i]);
				}

				Intro(Dimension1);
			}

			// Run program (npt = 3 will only print out soil conditions);
			if (SimulationPrintOption != 3) {
				TotalTime = intx(PrintTimeDates[PrintTimes]);

				// Main simulation loop.
				for (int i = StartPrintTime; i <= PrintTimes; i++) {
					NextPrintDate = PrintTimeDates[i];
					DredgedFillPrintOption = NewDredgedFillPrintOptions[i];
					add = NewDredgedFillInitialThicknesses[i];

					// hdf1, tadd, tds, and ms is set for the first layer of dredged fill;
					if (i != 1) {
						hdf1 = NewDredgedFillInitialThicknesses[i - 1];
						tadd = tadd + hdf1;
						DredgedFillDesiccationDelayDays = NewDredgedFillDesiccationDelayDays[i - 1];
						DredgedFillDesiccationDelayMonths = NewDredgedFillDesiccationDelayMonths[i - 1];
						Reset(Dimension1);
					}				

					Fdifq(Dimension1);
					Stress(Dimension1);
					Dataout();

					// Create Recovery input file from PSDDF output;
					// The file will take the output matricies and create the recovery input;
					if (IsSaveRecovery == 1) {
						// Only generate recovery output if needed;
						Recovery();
					}
				}

				DredgedFillDesiccationDelayDays = tds1;
				if (IsNotSaveContinuation == 2) {
					// Savdat(dim1, voidratio, perm, effectivestress, tol);
					Savdat();
				}
			}

			Cmd.WriteLine(
				"\n" +
				"============================ Simulation Completed ==============================");
		}
	}
}

