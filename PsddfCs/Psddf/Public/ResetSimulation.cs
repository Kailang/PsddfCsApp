namespace PsddfCs {
	public partial class Psddf {
		/// <summary>
		/// Reset status
		/// </summary>
		public void ResetSimulation () {
			StartPrintTime = 1;
			DredgedFillCurrentLayer = 1;

			CurrentTime = 0;
			TimeStep = 0;

			// sublayers
			for (int i = 1; i <= DredgedFillMaxSublayers; i++) {
				DredgedFillCoordA[i] = 0;
				DredgedFillCoordXI[i] = 0;
				DredgedFillCoordZ[i] = 0;
				DredgedFillInitialVoidRatio[i] = 0;
				DredgedFillCurrentVoidRatio[i] = 0;
				DredgedFillFinalVoidRatio[i] = 0;
				DredgedFillTotalStress[i] = 0;
				DredgedFillEffectiveStress[i] = 0;
				DredgedFillTotalPoreWaterPressure[i] = 0;
				DredgedFillHydrostaticPoreWaterPressure[i] = 0;
				DredgedFillExcessPoreWaterPressure[i] = 0;
				f[i] = 0;
				af[i] = 0;
				bf[i] = 0;
				pre_e[i] = 0;
				fint[i] = 0;
				ffint[i] = 0;
				et[i] = 0;
			}

			for (int i = 1; i <= CompressibleFoundationMaxSublayers; i++) {
				CompressibleFoundationCoordA[i] = 0;
				CompressibleFoundationCoordXI[i] = 0;
				CompressibleFoundationCoordZ[i] = 0;
				CompressibleFoundationInitialVoidRatio[i] = 0;
				CompressibleFoundationCurrentVoidRatio[i] = 0;
				CompressibleFoundationFinalVoidRatio[i] = 0;
				CompressibleFoundationTotalStress[i] = 0;
				CompressibleFoundationEffectiveStress[i] = 0;
				CompressibleFoundationTotalPoreWaterPressure[i] = 0;
				CompressibleFoundationHydrostaticPoreWaterPressure[i] = 0;
				CompressibleFoundationExcessPoreWaterPressure[i] = 0;
				f1[i] = 0;
				af1[i] = 0;
				bf1[i] = 0;
				pre_er[i] = 0;
				fint1[i] = 0;
				ffint1[i] = 0;
			}

			DredgedFillAverageConsolidationDegree = 0;
			DredgedFillTotalSettlement = 0;
			DredgedFillFinalSettlement = 0;
			DredgedFillSecondaryCompressionSettlement = 0;
			DredgedFillDesiccationSettlement = 0;

			CompressibleFoundationAverageConsolidationDegree = 0;
			CompressibleFoundationTotalSettlement = 0;
			CompressibleFoundationFinalSettlement = 0;
			CompressibleFoundationSecondaryCompressionSettlement = 0;

			// aux
			for (int i = 1; i <= DredgedFillMaxSublayers; i++) {
				for (int j = 1; j <= 15; j++) {
					auxdf[j, i] = 0;
				}
			}

			for (int i = 1; i <= CompressibleFoundationMaxSublayers; i++) {
				for (int j = 1; j <= 15; j++) {
					auxbl[j, i] = 0;
				}
			}
			
			// materials
			for (int i = 1; i <= MaxMaterialTypes; i++) {
				ibdl[i] = 0;
				layer_stress[i] = 0;

				dz[i] = 0;
				tpbl[i] = 0;
				difsecbl[i] = 0;
				IsCompressibleFoundationInPrimaryConsolidations[i] = true;

				dz1[i] = 0;
				tpdf[i] = 0;
				difsecdf[i] = 0;
				IsDredgedFillInPrimaryConsolidations[i] = true;
			}

			// dim
			for (int i = 1; i <= Dimension1; i++) {
				for (int j = 1; j <= Dimension2; j++) {
					PK[i, j] = 0;
					Dsde[i, j] = 0;
					Beta[i, j] = 0;
					Alpha[i, j] = 0;

					dvds[i, j] = 0;
				}
			}

			// misc
			NextPrintDate = 0;
			add = 0;
			hdf1 = 0;
			tadd = 0;

			da = 0;
			dudz10 = 0;
			dudz11 = 0;
			dudz21 = 0;
			pk0 = 0;

			CompressibleFoundationTotalSublayers = 0;
			DredgedFillTotalSublayers = 0;

			pre_ndfpoint = 0;
			pre_time = 0;

			aev = 0;
			cset = 0;
			dsc = 0;
			dset = 0;
			dtim = 0;
			qdf = 0;
			setc = 0;
			vrint = 0;
			vri1 = 0;

			nflag = 0;
			nnd = 0;
			m = 0;
			mm = 0;
			ndfcons = 0;
			nloop = 0;

			pretime = 0;
			preelev = 0;

			efsbot = 0;
			hsolids = 0;
			acumel = 0;

			ngraph = 0;
			qdfold = 0;

			temp_id = 0;
		}
	}
}
