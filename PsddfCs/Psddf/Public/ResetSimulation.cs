namespace PsddfCs {
	public partial class Psddf {
		/// <summary>
		/// Reset status
		/// </summary>
		public void ResetSimulation () {
			for (int i = 1; i <= npdf; i++) af[i] = 0;

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

			for (int i = 1; i <= npdf; i++) {
				for (int j = 1; j <= 15; j++) {
					auxdf[j, i] = 0.0;
				}
			}

			for (int i = 1; i <= npbl; i++) {
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
		}
	}
}
