using PsddfCs.Exception;

namespace PsddfCs {
	public partial class Psddf {
		/// <summary>
		/// Read .pci binary file to restore status.
		/// </summary>
		void Datain () {
			int i, k, id;

			// read(ins) ntypescompress, ntypedredge, numbl, numdf, hhbl, hhdf, add
			CompressibleFoundationMaterialTypes = Io.StreamReadInt(INS);
			DredgedFillMaterialTypes = Io.StreamReadInt(INS);
			CompressibleFoundationLayers = Io.StreamReadInt(INS);
			DredgedFillLayers = Io.StreamReadInt(INS);
			CompressibleFoundationTotalInitialThickness = Io.StreamReadDouble(INS);
			DredgedFillTotleInitialThickness = Io.StreamReadDouble(INS);
			add = Io.StreamReadDouble(INS);

			// read(ins) in, ins, iout, iouts, gw, hsolids
			Io.StreamReadInt(INS); // inx = Io.StreamReadInt(ins);
			Io.StreamReadInt(INS); // ins = Io.StreamReadInt(ins);
			Io.StreamReadInt(INS); // iout = Io.StreamReadInt(ins);
			Io.StreamReadInt(INS); // iouts = Io.StreamReadInt(ins);
			WaterUnitWeight = Io.StreamReadDouble(INS);
			hsolids = Io.StreamReadDouble(INS);

			// read(ins) acumel
			acumel = Io.StreamReadDouble(INS);

			// read(ins) nbl, ndfcons, ndflayer, ndfpoint, nblpoint
			IsFoundationCompressible = Io.StreamReadInt(INS);
			ndfcons = Io.StreamReadInt(INS);
			ndflayer = Io.StreamReadInt(INS);
			ndfpoint = Io.StreamReadInt(INS);
			nblpoint = Io.StreamReadInt(INS);
			 
			// read(ins) nflag, nm, nnd, ntime
			nflag = Io.StreamReadInt(INS);
			nm = Io.StreamReadInt(INS);
			nnd = Io.StreamReadInt(INS);
			PrintTimes = Io.StreamReadInt(INS);

			// read(ins) (printt(i), i = 1, ntime)
			for (i = 1; i <= PrintTimes; i++)
				PrintTimeDates[i] = Io.StreamReadDouble(INS);
			
			// read(ins) da, dudz11, dudz21
			da = Io.StreamReadDouble(INS);
			dudz11 = Io.StreamReadDouble(INS);
			dudz21 = Io.StreamReadDouble(INS);

			// read(ins) (e00(i), i = 1, numdf)
			for (i = 1; i <= DredgedFillLayers; i++)
				DredgedFillInitialVoidRatios[i] = Io.StreamReadDouble(INS);
			
			// read(ins) (hbl(i), i = 1, numbl), (hdf(i), i = 1, numdf), hdf1, sett, sett1
			for (i = 1; i <= CompressibleFoundationLayers; i++)
				CompressibleFoundationInitialThicknesses[i] = Io.StreamReadDouble(INS);
			for (i = 1; i <= DredgedFillLayers; i++)
				DredgedFillInitialThicknesses[i] = Io.StreamReadDouble(INS);
			
			hdf1 = Io.StreamReadDouble(INS);
			DredgedFillTotalSettlement = Io.StreamReadDouble(INS);
			CompressibleFoundationTotalSettlement = Io.StreamReadDouble(INS);
			
			// read(ins) sfin, sfin1, tau, time, tprint
			DredgedFillFinalSettlement = Io.StreamReadDouble(INS);
			CompressibleFoundationFinalSettlement = Io.StreamReadDouble(INS);
			TimeStep = Io.StreamReadDouble(INS);
			CurrentTime = Io.StreamReadDouble(INS);
			tprint = Io.StreamReadDouble(INS);

			// read(ins) ucon, ucon1, vri1
			DredgedFillAverageConsolidationDegree = Io.StreamReadDouble(INS);
			CompressibleFoundationAverageConsolidationDegree = Io.StreamReadDouble(INS);
			vri1 = Io.StreamReadDouble(INS);

			// read(ins) du0, dudz10, e0
			IncompressibleFoudationDrainagePathLength = Io.StreamReadDouble(INS);
			dudz10 = Io.StreamReadDouble(INS);
			IncompressibleFoudationVoidRatio = Io.StreamReadDouble(INS);

			// read(ins) zk0, pk0, xel, wtelev, tadd
			IncompressibleFoudationPermeability = Io.StreamReadDouble(INS);
			pk0 = Io.StreamReadDouble(INS);
			IncompressibleFoudationElevation = Io.StreamReadDouble(INS);
			ExternalWaterSurfaceElevation = Io.StreamReadDouble(INS);
			tadd = Io.StreamReadDouble(INS);

			// read(ins) m, mm, ms, nsc
			m = Io.StreamReadInt(INS);
			mm = Io.StreamReadInt(INS);
			DredgedFillDesiccationDelayMonths = Io.StreamReadInt(INS);
			DredgedFillPrintOption = Io.StreamReadInt(INS);

			// read(ins) aev, cset, dreff
			aev = Io.StreamReadDouble(INS);
			cset = Io.StreamReadDouble(INS);
			SurfaceDrainageEfficiencyFactor = Io.StreamReadDouble(INS);

			// read(ins) dsc, dset, dtim, ce
			dsc = Io.StreamReadDouble(INS);
			dset = Io.StreamReadDouble(INS);
			dtim = Io.StreamReadDouble(INS);
			MaxDredgedFillEvaporationEfficiency = Io.StreamReadDouble(INS);

			// read(ins) qdf, setc, setd, setsdf, setsbl
			qdf = Io.StreamReadDouble(INS);
			setc = Io.StreamReadDouble(INS);
			DredgedFillDesiccationSettlement = Io.StreamReadDouble(INS);
			DredgedFillSecondaryCompressionSettlement = Io.StreamReadDouble(INS);
			CompressibleFoundationSecondaryCompressionSettlement = Io.StreamReadDouble(INS);

			// read(ins) tds, tpm, vrint
			DredgedFillDesiccationDelayDays = Io.StreamReadDouble(INS);
			DaysInMonth = Io.StreamReadDouble(INS);
			vrint = Io.StreamReadDouble(INS);

			/*
    do i = 1, 12
        read(ins)  ep(i), pep(i), rf(i)
    enddo
			*/
			for (i = 1; i <= 12; i++) {
				ep[i] = Io.StreamReadDouble(INS);
				MaxEnvironmentalPotentialEvaporation[i] = Io.StreamReadDouble(INS);
				AverageMonthlyRainfall[i] = Io.StreamReadDouble(INS);
			}

			/*
    do i = 1, ntypescompress + ntypedredge
        read(ins)  nmat(i)
    enddo
			*/
			for (i = 1; i <= CompressibleFoundationMaterialTypes + DredgedFillMaterialTypes; i++)
				MaterialIDs[i] = Io.StreamReadInt(INS);

			/*
    do i = 1, ndfpoint
        read(ins)  a(i), af(i), bf(i), e(i), e1(i)
        read(ins)  efin(i), effstr(i), f(i), fint(i), totstr(i)
        read(ins)  u(i), u0(i), uw(i), xi(i), z(i)
        read(ins)  et(i)
    enddo
			*/
			for (i = 1; i <= ndfpoint; i++) {
				// read(ins)  a(i), af(i), bf(i), e(i), e1(i)
				DredgedFillCoordA[i] = Io.StreamReadDouble(INS);
				af[i] = Io.StreamReadDouble(INS);
				bf[i] = Io.StreamReadDouble(INS);
				DredgedFillCurrentVoidRatio[i] = Io.StreamReadDouble(INS);
				DredgedFillInitialVoidRatio[i] = Io.StreamReadDouble(INS);

				// read(ins)  efin(i), effstr(i), f(i), fint(i), totstr(i)
				DredgedFillFinalVoidRatio[i] = Io.StreamReadDouble(INS);
				DredgedFillEffectiveStress[i] = Io.StreamReadDouble(INS);
				f[i] = Io.StreamReadDouble(INS);
				fint[i] = Io.StreamReadDouble(INS);
				DredgedFillTotalStress[i] = Io.StreamReadDouble(INS);

				// read(ins)  u(i), u0(i), uw(i), xi(i), z(i)
				DredgedFillExcessPoreWaterPressure[i] = Io.StreamReadDouble(INS);
				DredgedFillHydrostaticPoreWaterPressure[i] = Io.StreamReadDouble(INS);
				DredgedFillTotalPoreWaterPressure[i] = Io.StreamReadDouble(INS);
				DredgedFillCoordXI[i] = Io.StreamReadDouble(INS);
				DredgedFillCoordZ[i] = Io.StreamReadDouble(INS);

				// read(ins)  et(i)
				et[i] = Io.StreamReadDouble(INS);
			}

			/*
    if (nbl /= 2) then !Check for Compressible Foundation
        do i = 1, nblpoint
            read(ins)  a1(i), af1(i), bf1(i), er(i), e11(i)
            read(ins)  efin1(i), efstr1(i), f1(i), fint1(i), tostr1(i)
            read(ins)  u1(i), u01(i), uw1(i), xi1(i), z1(i)
        enddo
    endif
			*/
			if (IsFoundationCompressible != 2) {
				// Check for Compressible Foundation
				for (i = 1; i <= ndfpoint; i++) {
					// read(ins)  a1(i), af1(i), bf1(i), er(i), e11(i)
					CompressibleFoundationCoordA[i] = Io.StreamReadDouble(INS);
					af1[i] = Io.StreamReadDouble(INS);
					bf1[i] = Io.StreamReadDouble(INS);
					CompressibleFoundationCurrentVoidRatio[i] = Io.StreamReadDouble(INS);
					CompressibleFoundationInitialVoidRatio[i] = Io.StreamReadDouble(INS);

					// read(ins)  efin1(i), efstr1(i), f1(i), fint1(i), tostr1(i)
					CompressibleFoundationFinalVoidRatio[i] = Io.StreamReadDouble(INS);
					CompressibleFoundationEffectiveStree[i] = Io.StreamReadDouble(INS);
					f1[i] = Io.StreamReadDouble(INS);
					fint1[i] = Io.StreamReadDouble(INS);
					CompressibleFoundationTotalStree[i] = Io.StreamReadDouble(INS);

					// read(ins)  u1(i), u01(i), uw1(i), xi1(i), z1(i)
					CompressibleFoundationExcessPoreWaterPressure[i] = Io.StreamReadDouble(INS);
					CompressibleFoundationHydrostaticPoreWaterPressure[i] = Io.StreamReadDouble(INS);
					CompressibleFoundationTotalPoreWaterPressure[i] = Io.StreamReadDouble(INS);
					CompressibleFoundationCoordXI[i] = Io.StreamReadDouble(INS);
					CompressibleFoundationCoordZ[i] = Io.StreamReadDouble(INS);
				}
			}

			/*
    do i = 1, numdf
        read(ins) iddf(i), nsub(i), dz(i), primdf(i)
        id = iddf(i)
        read(ins) ldf(id), cacc(id), crcc(id), gsdf(id), gs(id), gc(id)
        read(ins) sl(id), dl(id), h2(id), sat(id)
        do k = 1, ldf(id)
            read(ins) voidratio(k, id), perm(k, id), effectivestress(k, id)
        enddo
    enddo
			*/
			for (i = 1; i <= DredgedFillLayers; i++) {
				// read(ins) iddf(i), nsub(i), dz(i), primdf(i)
				DredgedFillMaterialIDs[i] = Io.StreamReadInt(INS);
				DredgedFillSublayers[i] = Io.StreamReadInt(INS);
				dz[i] = Io.StreamReadDouble(INS);
				IsDredgedFillInPrimaryConsolidations[i] = Io.StreamReadBool(INS);

				// id = iddf(i)
				id = DredgedFillMaterialIDs[i];

				// read(ins) ldf(id), cacc(id), crcc(id), gsdf(id), gs(id), gc(id)
				RelationDefinitionLines[id] = Io.StreamReadInt(INS);
				CaCcs[id] = Io.StreamReadDouble(INS);
				CrCcs[id] = Io.StreamReadDouble(INS);
				SpecificGravities[id] = Io.StreamReadDouble(INS);
				gs[id] = Io.StreamReadDouble(INS);
				gc[id] = Io.StreamReadDouble(INS);

				// read(ins) sl(id), dl(id), h2(id), sat(id)
				DredgedFillSaturationLimits[id] = Io.StreamReadDouble(INS);
				DredgedFillDesiccationLimits[id] = Io.StreamReadDouble(INS);
				DredgedFillDryingMaxDepth[id] = Io.StreamReadDouble(INS);
				DredgedFillAverageSaturation[id] = Io.StreamReadDouble(INS);

				for (k = 1; k <= RelationDefinitionLines[id]; k++) {
					// read(ins) voidratio(k, id), perm(k, id), effectivestress(k, id)
					VoidRatios[k, id] = Io.StreamReadDouble(INS);
					Permeabilities[k, id] = Io.StreamReadDouble(INS);
					EffectiveStresses[k, id] = Io.StreamReadDouble(INS);
				}
			}

			/*
    if (nbl /= 2) then
        do i = 1, numbl
            read(ins) idbl(i), nsub1(i), dz1(i), primbl(i)
            id = idbl(i)
            read(ins) ldf(id), cacc(id), crcc(id), gsdf(id), gs(id), gc(id)
            do k = 1, ldf(id)
                read(ins) voidratio(k, id), perm(k, id), effectivestress(k, id)
            enddo
        enddo
    endif
			*/
			if (IsFoundationCompressible != 2) {
				for (i = 1; i <= CompressibleFoundationLayers; i++) {
					// read(ins) idbl(i), nsub1(i), dz1(i), primbl(i)
					CompressibleFoundationMaterialIDs[i] = Io.StreamReadInt(INS);
					CompressibleFoundationSublayers[i] = Io.StreamReadInt(INS);
					dz1[i] = Io.StreamReadDouble(INS);
					IsCompressibleFoundationInPrimaryConsolidations[i] = Io.StreamReadBool(INS);

					// id = iddf(i)
					id = CompressibleFoundationMaterialIDs[i];

					// read(ins) ldf(id), cacc(id), crcc(id), gsdf(id), gs(id), gc(id)
					RelationDefinitionLines[id] = Io.StreamReadInt(INS);
					CaCcs[id] = Io.StreamReadDouble(INS);
					CrCcs[id] = Io.StreamReadDouble(INS);
					SpecificGravities[id] = Io.StreamReadDouble(INS);
					gs[id] = Io.StreamReadDouble(INS);
					gc[id] = Io.StreamReadDouble(INS);

					for (k = 1; k <= RelationDefinitionLines[id]; k++) {
						// read(ins) voidratio(k, id), perm(k, id), effectivestress(k, id)
						VoidRatios[k, id] = Io.StreamReadDouble(INS);
						Permeabilities[k, id] = Io.StreamReadDouble(INS);
						EffectiveStresses[k, id] = Io.StreamReadDouble(INS);
					}
				}
			}

			/*
    do k = 1, 15
        read(ins) (auxdf(k, i), i = 1, ndfpoint)
    enddo
			*/
			for (k = 1; k <= 15; k++) {
				// read(ins) (auxdf(k, i), i = 1, ndfpoint)
				for (i = 1; i <= ndfpoint; i++)
					auxdf[k, i] = Io.StreamReadDouble(INS);
			}

			/*
    if (nbl /= 2) then ! Check for Compressible Foundation
        do k = 1, 15
            read(ins) (auxbl(k, i), i = 1, nblpoint)
        enddo
    endif
			*/
			if (IsFoundationCompressible != 2) {
				for (k = 1; k <= 15; k++) {
					// read(ins) (auxbl(k, i), i = 1, nblpoint)
					for (i = 1; i <= nblpoint; i++)
						auxbl[k, i] = Io.StreamReadDouble(INS);
				}
			}

			// read(ins) matindex
			matindex = Io.StreamReadDouble(INS);
			// read(ins) (ahdf(i), atds(i), nms(i), nnsc(i), i = 1, ntime)
			for (i = 1; i <= PrintTimes; i++) {
				NewDredgedFillInitialThicknesses[i] = Io.StreamReadDouble(INS);
				NewDredgedFillDesiccationDelayDays[i] = Io.StreamReadDouble(INS);
				NewDredgedFillDesiccationDelayMonths[i] = Io.StreamReadInt(INS);
				NewDredgedFillPrintOptions[i] = Io.StreamReadInt(INS);
			}
			// read(ins) tol
			SecondaryCompressionExcessPoreWaterPressureLimit = Io.StreamReadDouble(INS);

			// read(ins) tpdf, tpbl
			for (i = 1; i < tpdf.Length; i++)
				tpdf[i] = Io.StreamReadDouble(INS);
			for (i = 1; i < tpbl.Length; i++)
				tpbl[i] = Io.StreamReadDouble(INS);
			
			// read(ins) difsecdf, difsecbl, qdfold
			for (i = 1; i < difsecdf.Length; i++)
				difsecdf[i] = Io.StreamReadDouble(INS);
			for (i = 1; i < difsecbl.Length; i++)
				difsecbl[i] = Io.StreamReadDouble(INS);
			qdfold = Io.StreamReadDouble(INS);

			if (!Io.EndOfFile(INS))
				throw new FileException("End of continuation file is not reached at end of data input, the continuation file is corrupted.");

			/*
    ! Reset time control
    nm = ntime + 1
    ntime = ntime + mtime
    write(iout, 929) problemname
			*/
			nm = PrintTimes + 1;
			PrintTimes = PrintTimes + ContinuationPrintTimes;
			Io.WriteLine(
				OUT,
				"\n         Continuation of Problem       {0,-60}",
				ProblemName);
		}
	}
}

