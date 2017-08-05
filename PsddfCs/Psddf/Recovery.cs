namespace PsddfCs {
	public partial class Psddf {
		/// <summary>
		/// Recovery Calculations;
		/// These have been translated from the Corp's Recovery program;
		/// The array that are read from are generated in dataout.f90;
		/// There have been some modifications due to interface between dredge lifts not considered before;
		/// </summary>
		void Recovery () {
			/*
			integer, dimension(1000):: id_element1, id_element
			integer:: total_element1, total_element, id_max_pore
			integer:: k, i, g, kk
			real*8:: del_time, time_yr, max_pore
			real*8, dimension(1000):: del_er, del_e, ave_del_er, ave_del_e
			real*8, dimension(1000):: void_volume1, void_volume, ele_flux1, ele_flux
			real*8, dimension(1000):: ele_pore1, ele_pore
			real*8, dimension(1000):: ele_flux_temp, ele_pore_temp, sum_flux
			real*8, dimension(1000):: sol_thick1, sol_thick
			real*8, dimension(1000):: thick1, void_element1, density_element1
			real*8, dimension(1000):: porosity_element1, flux1
			real*8, dimension(1000):: thick, void_element, density_element
			real*8, dimension(1000):: porosity_element, flux
			*/

			int[] id_element1 = new int[1000 + 1], id_element = new int[1000 + 1];
			int total_element1, total_element, id_max_pore;
			int k, i, g, kk;
			double del_time, time_yr, max_pore;

			double[] del_er = new double[1000 + 1], del_e = new double[1000 + 1], ave_del_er = new double[1000 + 1], ave_del_e = new double[1000 + 1];
			double[] void_volume1 = new double[1000 + 1], void_volume = new double[1000 + 1], ele_flux1 = new double[1000 + 1], ele_flux = new double[1000 + 1];
			double[] ele_pore1 = new double[1000 + 1], ele_pore = new double[1000 + 1];
			double[] ele_flux_temp = new double[1000 + 1], ele_pore_temp = new double[1000 + 1], sum_flux = new double[1000 + 1];
			double[] sol_thick1 = new double[1000 + 1], sol_thick = new double[1000 + 1];
			double[] thick1 = new double[1000 + 1], void_element1 = new double[1000 + 1], density_element1 = new double[1000 + 1];
			double[] porosity_element1 = new double[1000 + 1], flux1 = new double[1000 + 1];
			double[] thick = new double[1000 + 1], void_element = new double[1000 + 1], density_element = new double[1000 + 1];
			double[] porosity_element = new double[1000 + 1], flux = new double[1000 + 1];

			const string f922 = "{0,10:F3}{1,10}";
			const string f923 = "{0,10}{1,10:F4}{2,10:F5}{3,10:F0}{4,10:F4}";

			// Beginning of Recovery Calculations;
			// These have been translated from the Corp's Recovery program;
			// The array that are read from are generated in dataout.f90;
			// There have been some modifications due to interface between dredge lifts not considered before;

			// Find time interval between t and t + 1;
			del_time = CurrentTime - pre_time;
			pre_time = CurrentTime;
			// Find change of void ratio in compressible foundation between t and t + 1;
			if (IsFoundationCompressible != 2) {
				// Nbl is not 2 means there are compressible layers;
				for (i = 1; i <= nblpoint; i++) {
					del_er[i] = pre_er[i] - CompressibleFoundationCurrentVoidRatio[i];
					pre_er[i] = CompressibleFoundationCurrentVoidRatio[i];
					// For the next time step;
				}
			}
			// Find change of void ratio in dredged fill between t and t + 1;
			if (pre_ndfpoint < ndfpoint) {
				// In case new dredge layer is installed;
				for (i = 1; i <= pre_ndfpoint; i++) {
					del_e[i] = pre_e[i] - DredgedFillCurrentVoidRatio[i];
					pre_e[i] = DredgedFillCurrentVoidRatio[i];
				}
				for (i = pre_ndfpoint + 1; i <= ndfpoint; i += 1) {
					del_e[i] = DredgedFillCurrentVoidRatio[ndfpoint] - DredgedFillCurrentVoidRatio[i];
					pre_e[i] = DredgedFillCurrentVoidRatio[i];
				}
				pre_ndfpoint = ndfpoint;
			} else {
				for (i = 1; i <= ndfpoint; i++) {
					del_e[i] = pre_e[i] - DredgedFillCurrentVoidRatio[i];
					pre_e[i] = DredgedFillCurrentVoidRatio[i];
				}
			}
			// Initialization of total elements in both foundation and dredged fill;
			total_element1 = 0;
			total_element = 0;
			// Find element variables in compressible foundation;
			i = 1;
			g = 1;

			if (IsFoundationCompressible != 2) {
				// Nbl is not 2 means there are compressible layers;
				for (k = 1; k <= CompressibleFoundationLayers; k++) {
					for (kk = 1; kk <= CompressibleFoundationSublayers[k]; kk++) {
						id_element1[i] = i;
						// Id of element;

						thick1[i] = CompressibleFoundationCoordXI[g + 1] - CompressibleFoundationCoordXI[g];
						// Thickness of element (solid + void);
						sol_thick1[i] = CompressibleFoundationCoordZ[g + 1] - CompressibleFoundationCoordZ[g];
						// Thickness of solid element;
						void_element1[i] = (CompressibleFoundationCurrentVoidRatio[g + 1] + CompressibleFoundationCurrentVoidRatio[g]) / 2;
						// Ave. void ratio in element at time t + 1;
						porosity_element1[i] = void_element1[i] / (1 + void_element1[i]);
						// Porosity in element;
						density_element1[i] = SpecificGravities[CompressibleFoundationMaterialIDs[k]] * (1.0 - porosity_element1[i]) * 1000000;
						// Dry density (g / m^3);
						ave_del_er[i] = (del_er[g + 1] + del_er[g]) / 2;
						// Ave. difference of void ratio in element;
						void_volume1[i] = ave_del_er[i] * sol_thick1[i];
						// Void volume change = void change * Vs (ft);
						ele_flux1[i] = void_volume1[i] / del_time;
						// Generated flux in element due to consolidation;
						// Ft / dat;
						ele_pore1[i] = (CompressibleFoundationExcessPoreWaterPressure[g + 1] + CompressibleFoundationExcessPoreWaterPressure[g]) / 2;
						// Ave. excessive pore pressure in element;
						total_element1 = i;
						// Number of total element in compressible foundation;
						// Unit conversion;
						if (IsEnglishUnit == 1) {
							thick1[i] = thick1[i] / 3.2808;
							// Ft --> m;
							ele_flux1[i] = ele_flux1[i] * (365.25 / 3.2808);
							// Ft / day --> m / yr;
						}

						g = g + 1;
						i = i + 1;
					}
					g = g + 1;
				}
			}
			// Find element variables in dredged layers;
			i = 1;
			g = 1;

			for (k = 1; k <= ndflayer; k++) {
				for (kk = 1; kk <= DredgedFillSublayers[k]; kk++) {
					id_element[i] = i;
					// Id of element;
					thick[i] = DredgedFillCoordXI[g + 1] - DredgedFillCoordXI[g];
					// Thickness of element;
					sol_thick[i] = DredgedFillCoordZ[g + 1] - DredgedFillCoordZ[g];
					// Thickness of solid element;
					void_element[i] = (DredgedFillCurrentVoidRatio[g + 1] + DredgedFillCurrentVoidRatio[g]) / 2;
					// Ave. void ratio in element;
					porosity_element[i] = void_element[i] / (1 + void_element[i]);
					// Porosity in element;
					density_element[i] = SpecificGravities[DredgedFillMaterialIDs[k]] * (1.0 - porosity_element[i]) * 1000000;
					ave_del_e[i] = (del_e[g + 1] + del_e[g]) / 2;
					// Ave. difference of void ratio in element;
					void_volume[i] = ave_del_e[i] * sol_thick[i];
					// Void volume change = void change * Vs (ft);
					ele_flux[i] = void_volume[i] / del_time;
					// Generated flux in element due to consolidation;
					// Ft / dat;
					ele_pore[i] = (DredgedFillExcessPoreWaterPressure[g + 1] + DredgedFillExcessPoreWaterPressure[g]) / 2;
					// Ave. excessive pore pressure in element;
					// Dry density (g / m^3);

					total_element = i;
					// Number of total element in compressible foundation;
					// Unit conversion;
					if (IsEnglishUnit == 1) {
						thick[i] = thick[i] / 3.2808;
						// Ft --> m;
						ele_flux[i] = ele_flux[i] * (365.25 / 3.2808);
						// Ft / day --> m / yr;
					}

					g = g + 1;
					i = i + 1;
				}
				g = g + 1;
			}
			// Combine flux and excess porepressure of foundation and dredged layer;
			// To find max. excess porepressure and summation of flux;
			max_pore = 0.0;
			id_max_pore = 1;
			for (i = 1; i <= total_element1 + total_element; i++) {
				if (i <= total_element1) {
					ele_flux_temp[i] = ele_flux1[i];
					ele_pore_temp[i] = ele_pore1[i];
				} else {
					ele_flux_temp[i] = ele_flux[i - total_element1];
					ele_pore_temp[i] = ele_pore[i - total_element1];
				}

				if (ele_pore_temp[i] > max_pore) {
					max_pore = ele_pore_temp[i];
					id_max_pore = i;
				}
			}
			// Summation of element flux: positive flux --> upward flux;
			// Negative flux --> downward flux;
			sum_flux[id_max_pore] = ele_flux_temp[id_max_pore];
			for (i = id_max_pore; i <= total_element1 + total_element - 1; i += 1) {
				sum_flux[i + 1] = sum_flux[i] + ele_flux_temp[i + 1];
			}

			if (id_max_pore != 1) {
				sum_flux[id_max_pore - 1] = -ele_flux_temp[id_max_pore - 1];
				for (i = id_max_pore - 1; i >= 2; i -= 1) {
					sum_flux[i - 1] = sum_flux[i] - ele_flux_temp[i - 1];
				}
			}

			for (i = 1; i <= total_element1 + total_element; i++) {
				if (i <= total_element1) {
					flux1[i] = sum_flux[i];
				} else {
					flux[i - total_element1] = sum_flux[i];
				}
			}

			time_yr = CurrentTime / 365.25;
			// Time conversion from day to year;
			// This prints the output data;
			// write(recoveryout, 922) time_yr, total_element1 + total_element;
			Io.WriteLine(RCY, f922, time_yr, total_element1 + total_element);
			// Time and number of consolidation elements;
			// Print out dredged layer first (upper part);
			for (i = total_element; i >= 1; i -= 1) {
				// write(recoveryout, 923) id_element[i], thick[i], porosity_element[i], density_element[i], flux[i];
				Io.WriteLine(RCY, f923, id_element[i], thick[i], porosity_element[i], density_element[i], flux[i]);
			}
			// Print out compressible foundation if considered;
			if (IsFoundationCompressible != 2) {
				for (i = total_element1; i >= 1; i -= 1) {
					// write(recoveryout, 923) id_element1[i], thick1[i], porosity_element1[i], density_element1[i], flux1[i];
					Io.WriteLine(RCY, f923, id_element1[i], thick1[i], porosity_element1[i], density_element1[i], flux1[i]);
				}
			}

		}
	}
}