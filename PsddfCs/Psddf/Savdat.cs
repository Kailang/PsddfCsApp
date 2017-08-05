namespace PsddfCs {
	public partial class Psddf {
		/// <summary>
		/// Write .pci binary file to record current status.
		/// </summary>
		public void Savdat () {
			int i, k, id;

			// write(iouts) ntypescompress, ntypedredge, numbl, numdf, hhbl, hhdf, add
			Io.StreamWrite(iouts, CompressibleFoundationMaterialTypes, DredgedFillMaterialTypes, CompressibleFoundationLayers, DredgedFillLayers, CompressibleFoundationTotalInitialThickness, DredgedFillTotleInitialThickness, add);

			// write(iouts) in, ins, iout, iouts, gw, hsolids
			Io.StreamWrite(iouts, inx, ins, iout, iouts, WaterUnitWeight, hsolids);

			// write(iouts) acumel
			Io.StreamWrite(iouts, acumel);

			// write(iouts) nbl, ndfcons, ndflayer, ndfpoint, nblpoint
			Io.StreamWrite(iouts, IsFoundationCompressible, ndfcons, ndflayer, ndfpoint, nblpoint);

			// write(iouts) nflag, nm, nnd, ntime
			Io.StreamWrite(iouts, nflag, nm, nnd, PrintTimes);

			// write(iouts) (printt(i), i = 1, ntime)
			for (i = 1; i <= PrintTimes; i++)
				Io.StreamWrite(iouts, PrintTimeDates[i]);
			
			// write(iouts) da, dudz11, dudz21
			Io.StreamWrite(iouts, da, dudz11, dudz21);

			// write(iouts) (e00(i), i = 1, numdf)
			for (i = 1; i <= DredgedFillLayers; i++)
				Io.StreamWrite(iouts, DredgedFillInitialVoidRatios[i]);

			// write(iouts) (hbl(i), i = 1, numbl), (hdf(i), i = 1, numdf), hdf1, sett, sett1
			for (i = 1; i <= CompressibleFoundationLayers; i++)
				Io.StreamWrite(iouts, CompressibleFoundationInitialThicknesses[i]);
			for (i = 1; i <= DredgedFillLayers; i++)
				Io.StreamWrite(iouts, DredgedFillInitialThicknesses[i]);

			Io.StreamWrite(iouts, hdf1, DredgedFillTotalSettlement, CompressibleFoundationTotalSettlement);

			// write(iouts) sfin, sfin1, tau, time, tprint
			Io.StreamWrite(iouts, DredgedFillFinalSettlement, CompressibleFoundationFinalSettlement, tau, CurrentTime, tprint);

			// write(iouts) ucon, ucon1, vri1
			Io.StreamWrite(iouts, DredgedFillAverageConsolidationDegree, CompressibleFoundationAverageConsolidationDegree, vri1);

			// write(iouts) du0, dudz10, e0
			Io.StreamWrite(iouts, IncompressibleFoudationDrainagePathLength, dudz10, IncompressibleFoudationVoidRatio);

			// write(iouts) zk0, pk0, xel, wtelev, tadd
			Io.StreamWrite(iouts, IncompressibleFoudationPermeability, pk0, IncompressibleFoudationElevation, ExternalWaterSurfaceElevation, tadd);

			// write(iouts) m, mm, ms, nsc
			Io.StreamWrite(iouts, m, mm, DredgedFillDesiccationDelayMonths, DredgedFillPrintOption);

			// write(iouts) aev, cset, dreff
			Io.StreamWrite(iouts, aev, cset, SurfaceDrainageEfficiencyFactor);

			// write(iouts) dsc, dset, dtim, ce
			Io.StreamWrite(iouts, dsc, dset, dtim, MaxDredgedFillEvaporationEfficiency);

			// write(iouts) qdf, setc, setd, setsdf, setsbl
			Io.StreamWrite(iouts, qdf, setc, DredgedFillDesiccationSettlement, DredgedFillSecondaryCompressionSettlement, CompressibleFoundationSecondaryCompressionSettlement);

			// write(iouts) tds, tpm, vrint
			Io.StreamWrite(iouts, DredgedFillDesiccationDelayDays, DaysInMonth, vrint);

			/*
    do i = 1, 12
        write(iouts) ep(i), pep(i), rf(i)
    enddo
			*/
			for (i = 1; i <= 12; i++)
				Io.StreamWrite(iouts, ep[i], MaxEnvironmentalPotentialEvaporation[i], AverageMonthlyRainfall[i]);

			/*
    do i = 1, ntypescompress + ntypedredge
        write(iouts)  nmat(i)
    enddo
			*/
			for (i = 1; i <= CompressibleFoundationMaterialTypes + DredgedFillMaterialTypes; i++)
				Io.StreamWrite(iouts, nmat[i]);

			/*
    do i = 1, ndfpoint
        write(iouts) a(i), af(i), bf(i), e(i), e1(i)
        write(iouts) efin(i), effstr(i), f(i), fint(i), totstr(i)
        write(iouts) u(i), u0(i), uw(i), xi(i), z(i)
        write(iouts) et(i)
    enddo
			*/
			for (i = 1; i <= ndfpoint; i++) {
				Io.StreamWrite(iouts, DredgedFillCoordA[i], af[i], bf[i], DredgedFillCurrentVoidRatio[i], DredgedFillInitialVoidRatio[i]);
				Io.StreamWrite(iouts, DredgedFillFinalVoidRatio[i], DredgedFillEffectiveStress[i], f[i], fint[i], DredgedFillTotalStress[i]);
				Io.StreamWrite(iouts, DredgedFillExcessPoreWaterPressure[i], DredgedFillHydrostaticPoreWaterPressure[i], DredgedFillTotalPoreWaterPressure[i], DredgedFillCoordXI[i], DredgedFillCoordZ[i]);
				Io.StreamWrite(iouts, et[i]);
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
				for (i = 1; i <= nblpoint; i++) {
					Io.StreamWrite(iouts, CompressibleFoundationCoordA[i], af1[i], bf1[i], CompressibleFoundationCurrentVoidRatio[i], CompressibleFoundationInitialVoidRatio[i]);
					Io.StreamWrite(iouts, CompressibleFoundationFinalVoidRatio[i], CompressibleFoundationEffectiveStree[i], f1[i], fint1[i], CompressibleFoundationTotalStree[i]);
					Io.StreamWrite(iouts, CompressibleFoundationExcessPoreWaterPressure[i], CompressibleFoundationHydrostaticPoreWaterPressure[i], CompressibleFoundationTotalPoreWaterPressure[i], CompressibleFoundationCoordXI[i], CompressibleFoundationCoordZ[i]);
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
			for (i = 1; i <= DredgedFillLayers; i++) {
				Io.StreamWrite(iouts, DredgedFillMaterialIDs[i], DredgedFillSublayers[i], dz[i], IsDredgedFillInPrimaryConsolidations[i]);
				id = DredgedFillMaterialIDs[i];
				Io.StreamWrite(iouts, RelationDefinitionLines[id], CaCcs[id], CrCcs[id], SpecificGravities[id], gs[id], gc[id]);
				Io.StreamWrite(iouts, DredgedFillSaturationLimits[id], DredgedFillDesiccationLimits[id], DredgedFillDryingMaxDepth[id], DredgedFillAverageSaturation[id]);
				for (k = 1; k <= RelationDefinitionLines[id]; k++) {
					Io.StreamWrite(iouts, VoidRatios[k, id], Permeabilities[k, id], EffectiveStresses[k, id]);
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
				for (i = 1; i <= CompressibleFoundationLayers; i++) {
					Io.StreamWrite(iouts, CompressibleFoundationMaterialIDs[i], CompressibleFoundationSublayers[i], dz1[i], IsCompressibleFoundationInPrimaryConsolidations[i]);
					id = CompressibleFoundationMaterialIDs[i];
					Io.StreamWrite(iouts, RelationDefinitionLines[id], CaCcs[id], CrCcs[id], SpecificGravities[id], gs[id], gc[id]);
					for (k = 1; k <= RelationDefinitionLines[id]; k++) {
						Io.StreamWrite(iouts, VoidRatios[k, id], Permeabilities[k, id], EffectiveStresses[k, id]);
					}
				}
			}

			/*
    do k = 1, 15
        write(iouts) (auxdf(k, i), i = 1, ndfpoint)
    enddo
			*/
			for (k = 1; k <= 15; k++) {
				// write(iouts) (auxdf(k, i), i = 1, ndfpoint)
				for (i = 1; i <= ndfpoint; i++)
					Io.StreamWrite(iouts, auxdf[k, i]);
			}

			/*
	if (nbl /= 2) then !Check for Compressible Foundation
		do k = 1, 15
			write(iouts) (auxbl(k, i), i = 1, nblpoint)
		enddo
	endif
			*/
			if (IsFoundationCompressible != 2) {
				for (k = 1; k <= 15; k++) {
					for (i = 1; i <= nblpoint; i++)
						Io.StreamWrite(iouts, auxbl[k, i]);
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
			Io.StreamWrite(iouts, matindex);

			// write(iouts) (ahdf(i), atds(i), nms(i), nnsc(i), i = 1, ntime)
			for (i = 1; i <= PrintTimes; i++)
				Io.StreamWrite(iouts, NewDredgedFillInitialThicknesses[i], NewDredgedFillDesiccationDelayDays[i], NewDredgedFillDesiccationDelayMonths[i], NewDredgedFillPrintOptions[i]);

			// write(iouts) tol
			Io.StreamWrite(iouts, SecondaryCompressionExcessPoreWaterPressureLimit);

			// write(iouts) tpdf, tpbl
			for (i = 1; i < tpdf.Length; i++)
				Io.StreamWrite(iouts, tpdf[i]);
			for (i = 1; i < tpbl.Length; i++)
				Io.StreamWrite(iouts, tpbl[i]);
			
			// write(iouts) difsecdf, difsecbl, qdfold
			for (i = 1; i < difsecdf.Length; i++)
				Io.StreamWrite(iouts, difsecdf[i]);
			for (i = 1; i < difsecbl.Length; i++)
				Io.StreamWrite(iouts, difsecbl[i]);
			Io.StreamWrite(iouts, qdfold);
		}
	}
}

