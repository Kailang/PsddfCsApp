using PsddfCs.Exception;

namespace PsddfCs {
	public partial class Psddf {
		/// <summary>
		/// Read .pci binary file to restore status.
		/// </summary>
		public void Datain () {
			int i, k, id;

			// read(ins) ntypescompress, ntypedredge, numbl, numdf, hhbl, hhdf, add
			CompressibleFoundationMaterialTypes = Io.StreamReadInt(ins);
			DredgedFillMaterialTypes = Io.StreamReadInt(ins);
			CompressibleFoundationLayers = Io.StreamReadInt(ins);
			DredgedFillLayers = Io.StreamReadInt(ins);
			CompressibleFoundationTotalInitialThickness = Io.StreamReadDouble(ins);
			DredgedFillTotleInitialThickness = Io.StreamReadDouble(ins);
			add = Io.StreamReadDouble(ins);

			// read(ins) in, ins, iout, iouts, gw, hsolids
			Io.StreamReadInt(ins); // inx = Io.StreamReadInt(ins);
			Io.StreamReadInt(ins); // ins = Io.StreamReadInt(ins);
			Io.StreamReadInt(ins); // iout = Io.StreamReadInt(ins);
			Io.StreamReadInt(ins); // iouts = Io.StreamReadInt(ins);
			WaterUnitWeight = Io.StreamReadDouble(ins);
			hsolids = Io.StreamReadDouble(ins);

			// read(ins) acumel
			acumel = Io.StreamReadDouble(ins);

			// read(ins) nbl, ndfcons, ndflayer, ndfpoint, nblpoint
			IsFoundationCompressible = Io.StreamReadInt(ins);
			ndfcons = Io.StreamReadInt(ins);
			ndflayer = Io.StreamReadInt(ins);
			ndfpoint = Io.StreamReadInt(ins);
			nblpoint = Io.StreamReadInt(ins);
			 
			// read(ins) nflag, nm, nnd, ntime
			nflag = Io.StreamReadInt(ins);
			nm = Io.StreamReadInt(ins);
			nnd = Io.StreamReadInt(ins);
			PrintTimes = Io.StreamReadInt(ins);

			// read(ins) (printt(i), i = 1, ntime)
			for (i = 1; i <= PrintTimes; i++)
				PrintTimeDates[i] = Io.StreamReadDouble(ins);
			
			// read(ins) da, dudz11, dudz21
			da = Io.StreamReadDouble(ins);
			dudz11 = Io.StreamReadDouble(ins);
			dudz21 = Io.StreamReadDouble(ins);

			// read(ins) (e00(i), i = 1, numdf)
			for (i = 1; i <= DredgedFillLayers; i++)
				DredgedFillInitialVoidRatios[i] = Io.StreamReadDouble(ins);
			
			// read(ins) (hbl(i), i = 1, numbl), (hdf(i), i = 1, numdf), hdf1, sett, sett1
			for (i = 1; i <= CompressibleFoundationLayers; i++)
				CompressibleFoundationInitialThicknesses[i] = Io.StreamReadDouble(ins);
			for (i = 1; i <= DredgedFillLayers; i++)
				DredgedFillInitialThicknesses[i] = Io.StreamReadDouble(ins);
			
			hdf1 = Io.StreamReadDouble(ins);
			sett = Io.StreamReadDouble(ins);
			sett1 = Io.StreamReadDouble(ins);
			
			// read(ins) sfin, sfin1, tau, time, tprint
			sfin = Io.StreamReadDouble(ins);
			sfin1 = Io.StreamReadDouble(ins);
			tau = Io.StreamReadDouble(ins);
			time = Io.StreamReadDouble(ins);
			tprint = Io.StreamReadDouble(ins);

			// read(ins) ucon, ucon1, vri1
			ucon = Io.StreamReadDouble(ins);
			ucon1 = Io.StreamReadDouble(ins);
			vri1 = Io.StreamReadDouble(ins);

			// read(ins) du0, dudz10, e0
			IncompressibleFoudationDrainagePathLength = Io.StreamReadDouble(ins);
			dudz10 = Io.StreamReadDouble(ins);
			IncompressibleFoudationVoidRatio = Io.StreamReadDouble(ins);

			// read(ins) zk0, pk0, xel, wtelev, tadd
			IncompressibleFoudationPermeability = Io.StreamReadDouble(ins);
			pk0 = Io.StreamReadDouble(ins);
			IncompressibleFoudationElevation = Io.StreamReadDouble(ins);
			ExternalWaterSurfaceElevation = Io.StreamReadDouble(ins);
			tadd = Io.StreamReadDouble(ins);

			// read(ins) m, mm, ms, nsc
			m = Io.StreamReadInt(ins);
			mm = Io.StreamReadInt(ins);
			DredgedFillDesiccationDelayMonths = Io.StreamReadInt(ins);
			DredgedFillPrintOption = Io.StreamReadInt(ins);

			// read(ins) aev, cset, dreff
			aev = Io.StreamReadDouble(ins);
			cset = Io.StreamReadDouble(ins);
			SurfaceDrainageEfficiencyFactor = Io.StreamReadDouble(ins);

			// read(ins) dsc, dset, dtim, ce
			dsc = Io.StreamReadDouble(ins);
			dset = Io.StreamReadDouble(ins);
			dtim = Io.StreamReadDouble(ins);
			MaxDredgedFillEvaporationEfficiency = Io.StreamReadDouble(ins);

			// read(ins) qdf, setc, setd, setsdf, setsbl
			qdf = Io.StreamReadDouble(ins);
			setc = Io.StreamReadDouble(ins);
			setd = Io.StreamReadDouble(ins);
			setsdf = Io.StreamReadDouble(ins);
			setsbl = Io.StreamReadDouble(ins);

			// read(ins) tds, tpm, vrint
			DredgedFillDesiccationDelayDays = Io.StreamReadDouble(ins);
			DaysInMonth = Io.StreamReadDouble(ins);
			vrint = Io.StreamReadDouble(ins);

			/*
    do i = 1, 12
        read(ins)  ep(i), pep(i), rf(i)
    enddo
			*/
			for (i = 1; i <= 12; i++) {
				ep[i] = Io.StreamReadDouble(ins);
				MaxEnvironmentalPotentialEvaporation[i] = Io.StreamReadDouble(ins);
				AverageMonthlyRainfall[i] = Io.StreamReadDouble(ins);
			}

			/*
    do i = 1, ntypescompress + ntypedredge
        read(ins)  nmat(i)
    enddo
			*/
			for (i = 1; i <= CompressibleFoundationMaterialTypes + DredgedFillMaterialTypes; i++)
				nmat[i] = Io.StreamReadInt(ins);

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
				a[i] = Io.StreamReadDouble(ins);
				af[i] = Io.StreamReadDouble(ins);
				bf[i] = Io.StreamReadDouble(ins);
				e[i] = Io.StreamReadDouble(ins);
				e1[i] = Io.StreamReadDouble(ins);

				// read(ins)  efin(i), effstr(i), f(i), fint(i), totstr(i)
				efin[i] = Io.StreamReadDouble(ins);
				effstr[i] = Io.StreamReadDouble(ins);
				f[i] = Io.StreamReadDouble(ins);
				fint[i] = Io.StreamReadDouble(ins);
				totstr[i] = Io.StreamReadDouble(ins);

				// read(ins)  u(i), u0(i), uw(i), xi(i), z(i)
				u[i] = Io.StreamReadDouble(ins);
				u0[i] = Io.StreamReadDouble(ins);
				uw[i] = Io.StreamReadDouble(ins);
				xi[i] = Io.StreamReadDouble(ins);
				z[i] = Io.StreamReadDouble(ins);

				// read(ins)  et(i)
				et[i] = Io.StreamReadDouble(ins);
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
					a1[i] = Io.StreamReadDouble(ins);
					af1[i] = Io.StreamReadDouble(ins);
					bf1[i] = Io.StreamReadDouble(ins);
					er[i] = Io.StreamReadDouble(ins);
					e11[i] = Io.StreamReadDouble(ins);

					// read(ins)  efin1(i), efstr1(i), f1(i), fint1(i), tostr1(i)
					efin1[i] = Io.StreamReadDouble(ins);
					efstr1[i] = Io.StreamReadDouble(ins);
					f1[i] = Io.StreamReadDouble(ins);
					fint1[i] = Io.StreamReadDouble(ins);
					tostr1[i] = Io.StreamReadDouble(ins);

					// read(ins)  u1(i), u01(i), uw1(i), xi1(i), z1(i)
					u1[i] = Io.StreamReadDouble(ins);
					u01[i] = Io.StreamReadDouble(ins);
					uw1[i] = Io.StreamReadDouble(ins);
					xi1[i] = Io.StreamReadDouble(ins);
					z1[i] = Io.StreamReadDouble(ins);
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
				DredgedFillMaterialIDs[i] = Io.StreamReadInt(ins);
				DredgedFillSublayers[i] = Io.StreamReadInt(ins);
				dz[i] = Io.StreamReadDouble(ins);
				primdf[i] = Io.StreamReadBool(ins);

				// id = iddf(i)
				id = DredgedFillMaterialIDs[i];

				// read(ins) ldf(id), cacc(id), crcc(id), gsdf(id), gs(id), gc(id)
				RelationDefinitionLines[id] = Io.StreamReadInt(ins);
				CaCcs[id] = Io.StreamReadDouble(ins);
				CrCcs[id] = Io.StreamReadDouble(ins);
				SpecificGravities[id] = Io.StreamReadDouble(ins);
				gs[id] = Io.StreamReadDouble(ins);
				gc[id] = Io.StreamReadDouble(ins);

				// read(ins) sl(id), dl(id), h2(id), sat(id)
				DredgedFillSaturationLimits[id] = Io.StreamReadDouble(ins);
				DredgedFillDesiccationLimits[id] = Io.StreamReadDouble(ins);
				DredgedFillDryingMaxDepth[id] = Io.StreamReadDouble(ins);
				DredgedFillAverageSaturation[id] = Io.StreamReadDouble(ins);

				for (k = 1; k <= RelationDefinitionLines[id]; k++) {
					// read(ins) voidratio(k, id), perm(k, id), effectivestress(k, id)
					VoidRatios[k, id] = Io.StreamReadDouble(ins);
					Permeabilities[k, id] = Io.StreamReadDouble(ins);
					EffectiveStresses[k, id] = Io.StreamReadDouble(ins);
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
					CompressibleFoundationMaterialIDs[i] = Io.StreamReadInt(ins);
					CompressibleFoundationSublayers[i] = Io.StreamReadInt(ins);
					dz1[i] = Io.StreamReadDouble(ins);
					primbl[i] = Io.StreamReadBool(ins);

					// id = iddf(i)
					id = CompressibleFoundationMaterialIDs[i];

					// read(ins) ldf(id), cacc(id), crcc(id), gsdf(id), gs(id), gc(id)
					RelationDefinitionLines[id] = Io.StreamReadInt(ins);
					CaCcs[id] = Io.StreamReadDouble(ins);
					CrCcs[id] = Io.StreamReadDouble(ins);
					SpecificGravities[id] = Io.StreamReadDouble(ins);
					gs[id] = Io.StreamReadDouble(ins);
					gc[id] = Io.StreamReadDouble(ins);

					for (k = 1; k <= RelationDefinitionLines[id]; k++) {
						// read(ins) voidratio(k, id), perm(k, id), effectivestress(k, id)
						VoidRatios[k, id] = Io.StreamReadDouble(ins);
						Permeabilities[k, id] = Io.StreamReadDouble(ins);
						EffectiveStresses[k, id] = Io.StreamReadDouble(ins);
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
					auxdf[k, i] = Io.StreamReadDouble(ins);
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
						auxbl[k, i] = Io.StreamReadDouble(ins);
				}
			}

			// read(ins) matindex
			matindex = Io.StreamReadDouble(ins);
			// read(ins) (ahdf(i), atds(i), nms(i), nnsc(i), i = 1, ntime)
			for (i = 1; i <= PrintTimes; i++) {
				NewDredgedFillInitialThicknesses[i] = Io.StreamReadDouble(ins);
				NewDredgedFillDesiccationDelayDays[i] = Io.StreamReadDouble(ins);
				NewDredgedFillDesiccationDelayMonths[i] = Io.StreamReadInt(ins);
				NewDredgedFillPrintOptions[i] = Io.StreamReadInt(ins);
			}
			// read(ins) tol
			SecondaryCompressionExcessPoreWaterPressureLimit = Io.StreamReadDouble(ins);

			// read(ins) tpdf, tpbl
			for (i = 1; i < tpdf.Length; i++)
				tpdf[i] = Io.StreamReadDouble(ins);
			for (i = 1; i < tpbl.Length; i++)
				tpbl[i] = Io.StreamReadDouble(ins);
			
			// read(ins) difsecdf, difsecbl, qdfold
			for (i = 1; i < difsecdf.Length; i++)
				difsecdf[i] = Io.StreamReadDouble(ins);
			for (i = 1; i < difsecbl.Length; i++)
				difsecbl[i] = Io.StreamReadDouble(ins);
			qdfold = Io.StreamReadDouble(ins);

			if (!Io.EndOfFile(ins))
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
				iout,
				"\n         Continuation of Problem       {0,-60}",
				ProblemName);
		}
	}
}

