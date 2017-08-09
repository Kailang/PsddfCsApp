namespace PsddfCs {
	public partial class Psddf {
		/// <summary>
		/// Write .pci binary file to record current status.
		/// </summary>
		public void SaveContinuation (string path) {
			Io.OpenWrite(OUTS, path);

			// write(iouts) ntypescompress, ntypedredge, numbl, numdf, hhbl, hhdf, add
			Io.StreamWrite(OUTS, CompressibleFoundationMaterialTypes, DredgedFillMaterialTypes, CompressibleFoundationLayers, DredgedFillLayers, CompressibleFoundationTotalInitialThickness, DredgedFillTotleInitialThickness, add);

			// write(iouts) in, ins, iout, iouts, gw, hsolids
			Io.StreamWrite(OUTS, IN, INS, OUT, OUTS, WaterUnitWeight, hsolids);

			// write(iouts) acumel
			Io.StreamWrite(OUTS, acumel);

			// write(iouts) nbl, ndfcons, ndflayer, ndfpoint, nblpoint
			Io.StreamWrite(OUTS, IsFoundationCompressible, ndfcons, ndflayer, ndfpoint, nblpoint);

			// write(iouts) nflag, nm, nnd, ntime
			Io.StreamWrite(OUTS, nflag, StartPrintTime, nnd, PrintTimes);

			// write(iouts) (printt(i), i = 1, ntime)
			for (int i = 1; i <= PrintTimes; i++)
				Io.StreamWrite(OUTS, PrintTimeDates[i]);

			// write(iouts) da, dudz11, dudz21
			Io.StreamWrite(OUTS, da, dudz11, dudz21);

			// write(iouts) (e00(i), i = 1, numdf)
			for (int i = 1; i <= DredgedFillLayers; i++)
				Io.StreamWrite(OUTS, DredgedFillInitialVoidRatios[i]);

			// write(iouts) (hbl(i), i = 1, numbl), (hdf(i), i = 1, numdf), hdf1, sett, sett1
			for (int i = 1; i <= CompressibleFoundationLayers; i++)
				Io.StreamWrite(OUTS, CompressibleFoundationInitialThicknesses[i]);
			for (int i = 1; i <= DredgedFillLayers; i++)
				Io.StreamWrite(OUTS, DredgedFillInitialThicknesses[i]);

			Io.StreamWrite(OUTS, hdf1, DredgedFillTotalSettlement, CompressibleFoundationTotalSettlement);

			// write(iouts) sfin, sfin1, tau, time, tprint
			Io.StreamWrite(OUTS, DredgedFillFinalSettlement, CompressibleFoundationFinalSettlement, TimeStep, CurrentTime, NextPrintDate);

			// write(iouts) ucon, ucon1, vri1
			Io.StreamWrite(OUTS, DredgedFillAverageConsolidationDegree, CompressibleFoundationAverageConsolidationDegree, vri1);

			// write(iouts) du0, dudz10, e0
			Io.StreamWrite(OUTS, IncompressibleFoudationDrainagePathLength, dudz10, IncompressibleFoudationVoidRatio);

			// write(iouts) zk0, pk0, xel, wtelev, tadd
			Io.StreamWrite(OUTS, IncompressibleFoudationPermeability, pk0, IncompressibleFoudationElevation, ExternalWaterSurfaceElevation, tadd);

			// write(iouts) m, mm, ms, nsc
			Io.StreamWrite(OUTS, m, mm, DredgedFillDesiccationDelayMonths, DredgedFillPrintOption);

			// write(iouts) aev, cset, dreff
			Io.StreamWrite(OUTS, aev, cset, SurfaceDrainageEfficiencyFactor);

			// write(iouts) dsc, dset, dtim, ce
			Io.StreamWrite(OUTS, dsc, dset, dtim, MaxDredgedFillEvaporationEfficiency);

			// write(iouts) qdf, setc, setd, setsdf, setsbl
			Io.StreamWrite(OUTS, qdf, setc, DredgedFillDesiccationSettlement, DredgedFillSecondaryCompressionSettlement, CompressibleFoundationSecondaryCompressionSettlement);

			// write(iouts) tds, tpm, vrint
			Io.StreamWrite(OUTS, DredgedFillDesiccationDelayDays, DaysInMonth, vrint);

			/*
    do i = 1, 12
        write(iouts) ep(i), pep(i), rf(i)
    enddo
			*/
			for (int i = 1; i <= 12; i++)
				Io.StreamWrite(OUTS, ep[i], MaxEnvironmentalPotentialEvaporation[i], AverageMonthlyRainfall[i]);

			/*
    do i = 1, ntypescompress + ntypedredge
        write(iouts)  nmat(i)
    enddo
			*/
			for (int i = 1; i <= CompressibleFoundationMaterialTypes + DredgedFillMaterialTypes; i++)
				Io.StreamWrite(OUTS, MaterialIDs[i]);

			/*
    do i = 1, ndfpoint
        write(iouts) a(i), af(i), bf(i), e(i), e1(i)
        write(iouts) efin(i), effstr(i), f(i), fint(i), totstr(i)
        write(iouts) u(i), u0(i), uw(i), xi(i), z(i)
        write(iouts) et(i)
    enddo
			*/
			for (int i = 1; i <= ndfpoint; i++) {
				Io.StreamWrite(OUTS, DredgedFillCoordA[i], af[i], bf[i], DredgedFillCurrentVoidRatio[i], DredgedFillInitialVoidRatio[i]);
				Io.StreamWrite(OUTS, DredgedFillFinalVoidRatio[i], DredgedFillEffectiveStress[i], f[i], fint[i], DredgedFillTotalStress[i]);
				Io.StreamWrite(OUTS, DredgedFillExcessPoreWaterPressure[i], DredgedFillHydrostaticPoreWaterPressure[i], DredgedFillTotalPoreWaterPressure[i], DredgedFillCoordXI[i], DredgedFillCoordZ[i]);
				Io.StreamWrite(OUTS, et[i]);
			}

			/*
    if (nbl /= 2) then !Check for Compressible Foundation
        do i = 1, nblpoint
            write(iouts) a1(i), af1(i), bf1(i), er(i), e11(i)
            write(iouts) efin1(i), efstr1(i), f1(i), fint1(i), tostr1(i)
            write(iouts) u1(i), u01(i), uw1(i), xi1(i), z1(i)
        enddo
    endif
			*/
			if (IsFoundationCompressible != 2) {
				for (int i = 1; i <= nblpoint; i++) {
					Io.StreamWrite(OUTS, CompressibleFoundationCoordA[i], af1[i], bf1[i], CompressibleFoundationCurrentVoidRatio[i], CompressibleFoundationInitialVoidRatio[i]);
					Io.StreamWrite(OUTS, CompressibleFoundationFinalVoidRatio[i], CompressibleFoundationEffectiveStree[i], f1[i], fint1[i], CompressibleFoundationTotalStree[i]);
					Io.StreamWrite(OUTS, CompressibleFoundationExcessPoreWaterPressure[i], CompressibleFoundationHydrostaticPoreWaterPressure[i], CompressibleFoundationTotalPoreWaterPressure[i], CompressibleFoundationCoordXI[i], CompressibleFoundationCoordZ[i]);
				}
			}

			/*
    do i = 1, numdf
        write(iouts) iddf(i), nsub(i), dz(i), primdf(i)
        id = iddf(i)
        write(iouts) ldf(id), cacc(id), crcc(id), gsdf(id), gs(id), gc(id)
        write(iouts) sl(id), dl(id), h2(id), sat(id)
        do k = 1, ldf(id)
            write(iouts) voidratio(k, id), perm(k, id), effectivestress(k, id)
        enddo
    enddo
			*/
			for (int i = 1; i <= DredgedFillLayers; i++) {
				Io.StreamWrite(OUTS, DredgedFillMaterialIDs[i], DredgedFillSublayers[i], dz[i], IsDredgedFillInPrimaryConsolidations[i]);
				var id = DredgedFillMaterialIDs[i];
				Io.StreamWrite(OUTS, RelationDefinitionLines[id], CaCcs[id], CrCcs[id], SpecificGravities[id], SoilUnitWeight[id], SoilBuoyantUnitWeight[id]);
				Io.StreamWrite(OUTS, DredgedFillSaturationLimits[id], DredgedFillDesiccationLimits[id], DredgedFillDryingMaxDepth[id], DredgedFillAverageSaturation[id]);
				for (int j = 1; j <= RelationDefinitionLines[id]; j++) {
					Io.StreamWrite(OUTS, VoidRatios[j, id], Permeabilities[j, id], EffectiveStresses[j, id]);
				}
			}

			/*
    if (nbl /= 2) then
        do i = 1, numbl
            write(iouts) idbl(i), nsub1(i), dz1(i), primbl(i)
            id = idbl(i)
            write(iouts) ldf(id), cacc(id), crcc(id), gsdf(id), gs(id), gc(id)
            do k = 1, ldf(id)
                write(iouts) voidratio(k, id), perm(k, id), effectivestress(k, id)
            enddo
        enddo
    endif
			*/
			if (IsFoundationCompressible != 2) {
				for (int i = 1; i <= CompressibleFoundationLayers; i++) {
					Io.StreamWrite(OUTS, CompressibleFoundationMaterialIDs[i], CompressibleFoundationSublayers[i], dz1[i], IsCompressibleFoundationInPrimaryConsolidations[i]);
					var id = CompressibleFoundationMaterialIDs[i];
					Io.StreamWrite(OUTS, RelationDefinitionLines[id], CaCcs[id], CrCcs[id], SpecificGravities[id], SoilUnitWeight[id], SoilBuoyantUnitWeight[id]);
					for (int j = 1; j <= RelationDefinitionLines[id]; j++) {
						Io.StreamWrite(OUTS, VoidRatios[j, id], Permeabilities[j, id], EffectiveStresses[j, id]);
					}
				}
			}

			/*
    do k = 1, 15
        write(iouts) (auxdf(k, i), i = 1, ndfpoint)
    enddo
			*/
			for (int i = 1; i <= 15; i++) {
				// write(iouts) (auxdf(k, i), i = 1, ndfpoint)
				for (int j = 1; j <= ndfpoint; j++)
					Io.StreamWrite(OUTS, auxdf[i, j]);
			}

			/*
	if (nbl /= 2) then !Check for Compressible Foundation
		do k = 1, 15
			write(iouts) (auxbl(k, i), i = 1, nblpoint)
		enddo
	endif
			*/
			if (IsFoundationCompressible != 2) {
				for (int i = 1; i <= 15; i++) {
					for (int j = 1; j <= nblpoint; j++)
						Io.StreamWrite(OUTS, auxbl[i, j]);
				}
			}

			/*
    write(iouts) matindex
    write(iouts) (ahdf(i), atds(i), nms(i), nnsc(i), i = 1, ntime)
    write(iouts) tol
    write(iouts) tpdf, tpbl
    write(iouts) difsecdf, difsecbl, qdfold
			*/
			//  write(iouts) matindex
			Io.StreamWrite(OUTS, matindex);

			// write(iouts) (ahdf(i), atds(i), nms(i), nnsc(i), i = 1, ntime)
			for (int i = 1; i <= PrintTimes; i++)
				Io.StreamWrite(OUTS, NewDredgedFillInitialThicknesses[i], NewDredgedFillDesiccationDelayDays[i], NewDredgedFillDesiccationDelayMonths[i], NewDredgedFillPrintOptions[i]);

			// write(iouts) tol
			Io.StreamWrite(OUTS, SecondaryCompressionExcessPoreWaterPressureLimit);

			// write(iouts) tpdf, tpbl
			for (int i = 1; i < tpdf.Length; i++)
				Io.StreamWrite(OUTS, tpdf[i]);
			for (int i = 1; i < tpbl.Length; i++)
				Io.StreamWrite(OUTS, tpbl[i]);

			// write(iouts) difsecdf, difsecbl, qdfold
			for (int i = 1; i < difsecdf.Length; i++)
				Io.StreamWrite(OUTS, difsecdf[i]);
			for (int i = 1; i < difsecbl.Length; i++)
				Io.StreamWrite(OUTS, difsecbl[i]);
			Io.StreamWrite(OUTS, qdfold);

			Io.CloseWrite(OUTS);
		}
	}
}
