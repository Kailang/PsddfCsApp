namespace PsddfCs {
	public partial class Psddf {
		const int npdf = 1001, npbl = 201;
		/// <summary>
		/// Max number of material types.
		/// </summary>
		const int nleymax = 200;
		const int dim1 = 100, dim2 = 200;

		#region IO Channels

		/// <summary>
		/// Input Channel: Main input.
		/// </summary>
		const int inx = 10;
		/// <summary>
		/// Output Channel: Main Output / Calculations.
		/// </summary>
		const int iout = 11;
		/// <summary>
		/// Output Channel: Surface Elevation data.
		/// </summary>
		const int iplot = 12;
		/// <summary>
		/// Output Channel: Stores the output from a previous simulation so that it can be restarted;
		/// Available when ndata2 == 2, save for continuation.
		/// </summary>
		const int iouts = 13;
		/// <summary>
		/// Input Channel: Stores the output from a previous simulation so that it can be restarted.
		/// Available when ndata1 == 2, read from continuation.
		/// </summary>
		const int ins = 14;
		/// <summary>
		/// Output Channel: Compressible Foundation Results.
		/// </summary>
		const int igracf = 15;
		/// <summary>
		/// Output Channel: Dredged Material Results.
		/// </summary>
		const int igradf = 16;
		/// <summary>
		/// Output Channel: For use with the CAP model.
		/// </summary>
		const int recoveryout = 20;
		/// <summary>
		/// Output Channel: For outputing the PSI file.
		/// </summary>
		const int inout = 30;

		#endregion

		#region Group A

		/// <summary>
		/// Path or name of the input data file.
		/// </summary>
		public string InputFilePath;
		/// <summary>
		/// Logical denotion the following options:
		/// true = print simulation process.
		/// false = do not print simulation process.
		/// </summary>
		public bool IsPrintProcess;

		/// <summary>
		/// NPROB
		/// Description of simulation which can be a maximum of any 60 characters except a single quote.
		/// </summary>
		public string ProblemName;
		/// <summary>
		/// NDATA1
		/// Integer denoting the following options:
		/// 1 = new simulation.
		/// 2 = continuation/restart of previous simulation.
		/// </summary>
		public int IsNewSimulation;
		/// <summary>
		/// NDATA2
		/// Integer denoting the following options:
		/// 1 = output not saved in a continuation file.
		/// 2 = output saved in a continuation file for subsequent restart of simulation.
		/// </summary>
		public int IsNotSaveContinuation;

		#endregion

		#region Group B

		/// <summary>
		/// NPT
		/// Integer denoting the following options:
		/// 1 = complete program execution and printing soil data, initial conditions, and current conditions for all specified print times.
		/// 2 = complete program execution but do not print soil data and initial conditions.
		/// 3 = terminate program execution after printing soil data and initial conditions. 
		/// </summary>
		public int SimulationPrintOption;
		/// <summary>
		/// RECOVERYFLAG
		/// Integer denoting the following option:
		/// 1 = create output file for use with RECOVERY/CAP models.
		/// 2 = no output file for use with RECOVERY/CAP models is created.
		/// </summary>
		public int IsSaveRecovery;
		/// <summary>
		/// DIMENSIONFLAG 
		/// Integer denoting the following options:
		/// 1 = English units are used.
		/// 2 = SI units are used.
		/// </summary>
		public int IsEnglishUnit;

		#endregion

		#region Group C

		/// <summary>
		/// E0
		/// Void ratio at the top of incompressible foundation.
		/// </summary>
		public double IncompressibleFoudationVoidRatio;
		/// <summary>
		/// ZK0
		/// Permeability at the top of incompressible foundation. 
		/// </summary>
		public double IncompressibleFoudationPermeability;
		/// <summary>
		/// DU0
		/// Length of vertical drainage path through the incompressible foundation.
		/// Kailang: du0 get changed by Setup(), add odu0 to record the original du0.
		/// </summary>
		public double IncompressibleFoudationDrainagePathLength, OriginalDU0;
		/// <summary>
		/// XEL
		/// Elevation at the top of the incompressible foundation.
		/// </summary>
		public double IncompressibleFoudationElevation;
		/// <summary>
		/// WTELEV
		/// Elevation of the external water surface at the placement site.  
		/// </summary>
		public double ExternalWaterSurfaceElevation;
		/// <summary>
		/// GW
		/// Unit weight of water in consistent units, e.g., 62.4 lbs/ft3.
		/// </summary>
		public double WaterUnitWeight;
		/// <summary>
		/// TOL
		/// Excess pore-water pressure at which the secondary compression model is activated.
		/// If the user selects not to simulate secondary compression, PSDDF assigns the variable TOL a value of zero.
		/// If the user activates secondary compression in this screen, the user must set the variable TOL to a value greater than zero in Data Input Group E. 
		/// Kailang: Now secondary compression will be turned on automatically if the degree of consolidation is larger than maxUcon.
		///          This variable is now useless.
		/// </summary>
		double SecondaryCompressionExcessPoreWaterPressureLimit;
		/// <summary>
		/// Kailang: Now secondary compression will be turned on automatically if the degree of consolidation is larger than maxUcon.
		/// </summary>
		const double uconmax = 0.96;

		#endregion

		#region Group D

		/// <summary>
		/// NUMBL
		/// Number of different layers in the compressible foundation (insert zero if foundation is incompressible). 
		/// </summary>
		public int CompressibleFoundationLayers;
		/// <summary>
		/// NTYPESCOMPRESS
		/// Number of different material types in the compressible foundation (insert zero if foundation is incompressible).
		/// </summary>
		public int CompressibleFoundationMaterialTypes;
		/// <summary>
		/// NTYPEDREDGE 
		/// Number of different dredged fill material types.
		/// </summary>
		public int DredgedFillMaterialTypes;
		/// <summary>
		/// Integer denoting the following options:
		/// 1 = compressible foundation.
		/// 2 = noncompressible foundation.
		/// </summary>
		int IsFoundationCompressible;
		#endregion
		
		#region Group D1

		/// <summary>
		/// HBL
		/// Initial thickness of compressible foundation layer. 
		/// Repeat NUMBL times.
		/// </summary>
		public readonly double[] CompressibleFoundationInitialThicknesses = new double[nleymax + 1];
		/// <summary>
		/// HHBL
		/// Total initial thickness of compressible foundation layer.
		/// The sum of hbl.
		/// </summary>
		double CompressibleFoundationTotalInitialThickness;
		/// <summary>
		/// IDBL
		/// Material identification number for the compressible foundation layer. 
		/// Repeat NUMBL times. 
		/// </summary>
		public readonly int[] CompressibleFoundationMaterialIDs = new int[1000 + 1];
		/// <summary>
		/// NSUB1
		/// Number of sublayers that the compressible foundation layer is divided into for computational purposes. 
		/// Repeat NUMBL times.
		/// </summary>
		public readonly int[] CompressibleFoundationSublayers = new int[nleymax + 1];
		/// <summary>
		/// OCR
		/// Average overconsolidation ratio (OCR) at the middle of each compressible foundation layer.
		/// </summary>
		public readonly double[] CompressibleFoundationOCR = new double[nleymax + 1];

		#endregion

		#region Group D2

		/// <summary>
		/// KOM
		/// Material identification number for the compressible foundation layer. 
		/// </summary>
		int MaterialID;
		/// <summary>
		/// GSDF
		/// Specific gravity of soil solids of the compressible foundation material type KOM.
		/// </summary>
		public readonly double[] SpecificGravities = new double[2 * nleymax];
		/// <summary>
		/// CACC
		/// Ratio between secondary compression index, Ca, and Compression index, Cc from standard oedometer tests. 
		/// </summary>
		public readonly double[] CaCcs = new double[2 * nleymax];
		/// <summary>
		/// CRCC
		/// Ratio between recompression index, Cr, and Compression index, Cc from standard oedometer tests. 
		/// </summary>
		public readonly double[] CrCcs = new double[2 * nleymax];
		/// <summary>
		/// LDF
		/// Number of subsequent lines of data points that will be entered to define the void ratio-effective stress and void ratio-permeability relationships for each material type (KOM). 
		/// </summary>
		public readonly int[] RelationDefinitionLines = new int[2 * nleymax];
		#endregion

		#region D3
		/// <summary>
		/// VOIDRATIO (I,KOM)
		/// Measured void ratios of compressible foundation layer under effective stresses applied in laboratory consolidation tests.
		/// </summary>
		public readonly double[,] VoidRatios = new double[dim1 + 1, dim2 + 1];
		/// <summary>
		/// EFFECTIVESTRESS (I,KOM) 
		/// Effective stresses applied in laboratory consolidation tests corresponding to the void ratios entered above.
		/// </summary>
		public readonly double[,] EffectiveStresses = new double[dim1 + 1, dim2 + 1];
		/// <summary>
		/// PERM (I,KOM) 
		/// Permeabilities from the laboratory consolidation tests on the compressible foundation layer corresponding to the void ratios entered above.
		/// </summary>
		public readonly double[,] Permeabilities = new double[dim1 + 1, dim2 + 1];
		#endregion

		#region D4
		/// <summary>
		/// SL
		/// Saturation limit of the dredged fill defined as the void ratio which separates first-stage desiccation from second-stage desiccation. 
		/// </summary>
		public readonly double[] DredgedFillSaturationLimits = new double[nleymax + 1];
		/// <summary>
		/// DL
		/// Desiccation limit of the dredged fill defined as the lowest void ratio the material will attain during second-stage drying. 
		/// </summary>
		public readonly double[] DredgedFillDesiccationLimits = new double[nleymax + 1];
		/// <summary>
		/// H2
		/// The maximum depth to which second-stage drying will occur (max. crust thickness) in the dredged fill.
		/// </summary>
		public readonly double[] DredgedFillDryingMaxDepth = new double[nleymax + 1];
		/// <summary>
		/// SAT
		/// The average degree of saturation, expressed as a fraction of 1.0, of the dredged fill when dried to the desiccation limit, DL.
		/// </summary>
		public readonly double[] DredgedFillAverageSaturation = new double[nleymax + 1];

		#endregion

		#region Group E
		/// <summary>
		/// Number of output or print times during simulation. 
		/// The value of NTIME must be less than or equal to 1000. 
		/// </summary>
		public int PrintTimes;
		#endregion

		#region Group F
		/// <summary>
		/// HDF
		/// Initial thickness of dredged fill layers. 
		/// </summary>
		public readonly double[] DredgedFillInitialThicknesses = new double[nleymax + 1];
		/// <summary>
		/// HHDF
		/// Totle initial thickness of dredged fill layers.
		/// The sum of hdf.
		/// </summary>
		double DredgedFillTotleInitialThickness;
		/// <summary>
		/// TDS
		/// Number of time periods (usually days) from the addition of the first layer of dredged fill to the start of desiccation in the first layer of dredged fill.
		/// </summary>
		public double DredgedFillDesiccationDelayDays;
		/// <summary>
		/// MS
		/// The number of the month at which desiccation starts in the first layer of dredged fill. (January = 1)
		/// </summary>
		public int DredgedFillDesiccationDelayMonths;
		/// <summary>
		/// NSC
		/// Integer denoting the following options:
		/// 1 = Print the current void ratio, effective stress, and pore-water pressure profiles for particular print time.
		/// 2 = Print only the current void ratio profile.
		/// 3 = Do not print void ratio, effective stress, and porewater pressure profiles for print time.
		/// </summary>
		public int DredgedFillPrintOption;
		/// <summary>
		/// E00
		/// Initial void ratio of new dredged fill after sedimentation and before the start of consolidation. 
		/// </summary>
		public readonly double[] DredgedFillInitialVoidRatios = new double[npdf + 1];
		/// <summary>
		/// IDDF
		/// Material identification number for the dredged fill layer added at PRINTT(I). 
		/// </summary>
		public readonly int[] DredgedFillMaterialIDs = new int[nleymax + 1];
		/// <summary>
		/// NSUB
		/// Number of sublayers that the dredged fill layer is divided into for computational purposes. 
		/// </summary>
		public readonly int[] DredgedFillSublayers = new int[nleymax + 1];
		#endregion

		#region Group G
		/// <summary>
		/// PRINTT
		/// Time at which the properties of the consolidating layers will be printed and/or a new layer of dredged fill will be applied. 
		/// </summary> 		
		public readonly double[] PrintTimeDates = new double[1000 + 1];
		/// <summary>
		/// AHDF
		/// Initial thickness of new dredged fill layer to be added at PRINTT(I).
		/// </summary>
		public readonly double[] NewDredgedFillInitialThicknesses = new double[1000 + 1];
		/// <summary>
		/// ATDS
		/// Cumulative number of time periods (usually days) from the addition of the first layer of dredged fill to the start of desiccation in the dredged fill placed at PRINTT(I). 
		/// </summary>
		public readonly double[] NewDredgedFillDesiccationDelayDays = new double[1000 + 1];
		/// <summary>
		/// NMS
		/// The number of the month at which desiccation starts for this print time.
		/// </summary>
		public readonly int[] NewDredgedFillDesiccationDelayMonths = new int[1000 + 1];
		/// <summary>
		/// NNSC
		/// Integer denoting the following options:
		/// 1 = Print the current void ratio, effective stress, and porewater pressure profiles for print time, PRINTT(I).
		/// 2 = Print only the current void ratio profile at PRINTT(I).
		/// 3 = Do not print void ratio, effective stress, and pore-water pressure profiles for print time, PRINTT(I).
		/// </summary>
		public readonly int[] NewDredgedFillPrintOptions = new int[1000 + 1];
		#endregion

		#region Group H
		/// <summary>
		/// TPM
		/// Number of basic time periods in a month.
		/// If time is measured in days, TPM is equal to 30.
		/// </summary>
		public double DaysInMonth;
		/// <summary>
		/// DREFF
		/// Surface drainage efficiency factor of the containment area which is defined as the ratio of the overland runoff volume to the rainfall volume. 
		/// </summary>
		public double SurfaceDrainageEfficiencyFactor;
		/// <summary>
		/// CE
		/// The maximum dredged fill evaporation efficiency for desiccation drying. 
		/// </summary>
		public double MaxDredgedFillEvaporationEfficiency;
		#endregion

		#region Group I
		/// <summary>
		/// PEP
		/// Monthly Class A pan or maximum environmental potential evaporation expected at the containment area each month of the year. 
		/// January is month number 1. 
		/// </summary>
		public readonly double[] MaxEnvironmentalPotentialEvaporation = new double[12 + 1];
		/// <summary>
		/// RF
		/// Average monthly rainfall expected at the containment area for each month of the year. 
		/// January is month number 1.
		/// </summary>
		public readonly double[] AverageMonthlyRainfall = new double[12 + 1];
		#endregion


		#region Group K - Continuation Data

		/// <summary>
		/// Number of output or print times during simulation in continuation. 
		/// The value of NTIME must be less than or equal to 1000. 
		/// </summary>
		public int ContinuationPrintTimes;
		/// <summary>
		/// Number of dredge fill layer added in continuation.
		/// </summary>
		public int ContinuationDredgedFillMaterialTypes;

		#endregion

		#region Simulation Data

		/// <summary>
		/// The simulation time.
		/// </summary>
		public double CurrentTime;
		/// <summary>
		/// TAU
		/// Time interval of the explicit finite difference analysis.
		/// </summary>
		public double TimeStep;
		/// <summary>
		/// Coordinates of nodal points.
		/// </summary>
		public readonly double[]
			DredgedFillCoordA = new double[npdf + 1], CompressibleFoundationCoordA = new double[npbl + 1],
			DredgedFillCoordXI = new double[npdf + 1], CompressibleFoundationCoordXI = new double[npbl + 1],
			DredgedFillCoordZ = new double[npdf + 1], CompressibleFoundationCoordZ = new double[npbl + 1];
		/// <summary>
		/// Initial void ratio at each coordinate point.
		/// </summary>
		public readonly double[] DredgedFillInitialVoidRatio = new double[npdf + 1], CompressibleFoundationInitialVoidRatio = new double[npbl + 1];
		/// <summary>
		/// Current void ratio at each coordinate point.
		/// </summary>
		public readonly double[] DredgedFillCurrentVoidRatio = new double[npdf + 1], CompressibleFoundationCurrentVoidRatio = new double[npbl + 1];
		/// <summary>
		/// Final void ratio at each coordinate point.
		/// </summary>
		public readonly double[] DredgedFillFinalVoidRatio = new double[npdf + 1], CompressibleFoundationFinalVoidRatio = new double[npbl + 1];
		/// <summary>
		/// Total stress at each coordinate point.
		/// </summary>
		public readonly double[] DredgedFillTotalStress = new double[npdf + 1], CompressibleFoundationTotalStree = new double[npbl + 1];
		/// <summary>
		/// Effective stress at each coordinate point.
		/// </summary>
		public readonly double[] DredgedFillEffectiveStress = new double[npdf + 1], CompressibleFoundationEffectiveStree = new double[npbl + 1], f = new double[npdf + 1], f1 = new double[npbl + 1];
		/// <summary>
		/// Total pore pressure at each coordinate point.
		/// </summary>
		public readonly double[] DredgedFillTotalPoreWaterPressure = new double[npdf + 1], CompressibleFoundationTotalPoreWaterPressure = new double[npbl + 1];
		/// <summary>
		/// Static pore pressure at each coordinate point.
		/// </summary>
		public readonly double[] DredgedFillHydrostaticPoreWaterPressure = new double[npdf + 1], CompressibleFoundationHydrostaticPoreWaterPressure = new double[npbl + 1];
		/// <summary>
		/// Excess pore-water pressure at each coordinate point.
		/// </summary>
		public readonly double[] DredgedFillExcessPoreWaterPressure = new double[npdf + 1], CompressibleFoundationExcessPoreWaterPressure = new double[npbl + 1];
		/// <summary>
		/// Degree of consolidation.
		/// </summary>
		public double DredgedFillAverageConsolidationDegree, CompressibleFoundationAverageConsolidationDegree;
		/// <summary>
		/// Settlement at current time.
		/// </summary>
		public double DredgedFillTotalSettlement, CompressibleFoundationTotalSettlement;
		/// <summary>
		/// Final settlement at the end of primary consolidation.
		/// </summary>
		public double DredgedFillFinalSettlement, CompressibleFoundationFinalSettlement;
		/// <summary>
		/// Settlement caused by secondary compression.
		/// </summary>
		public double DredgedFillSecondaryCompressionSettlement, CompressibleFoundationSecondaryCompressionSettlement;
		/// <summary>
		/// Settlement caused by desiccation.
		/// </summary>
		public double DredgedFillDesiccationSettlement;

		#endregion

		/// <summary>
		/// pk(i, k) = perm(i, k) / (1.0 + voidratio(i, k))
		/// </summary>
		double[,] pk = new double[dim1 + 1, dim2 + 1];
		double[,] dvds = new double[dim1 + 1, dim2 + 1];
		/// <summary>
		/// Slope of stress-void ratio curve.
		/// </summary>
		double[,] dsde = new double[dim1 + 1, dim2 + 1];
		/// <summary>
		/// Slope of permeability function.
		/// </summary>
		double[,] beta = new double[dim1 + 1, dim2 + 1];
		/// <summary>
		/// Permeability function times dsde.
		/// </summary>
		double[,] alpha = new double[dim1 + 1, dim2 + 1];


		/// <summary>
		/// Number of dredge fill layer added.
		/// </summary>
		int DredgedFillLayers;

		/// <summary>
		/// The next time at which the properties of the consolidating layers will be printed and/or a new layer of dredged fill will be applied. 
		/// </summary>
		double tprint;
		/// <summary>
		/// Kailang: This is somehow useless.
		/// </summary>
		double add;
		/// <summary>
		/// Initial thickness of new dredged fill layer to be added.
		/// </summary>
		double hdf1;
		/// <summary>
		/// Accumulation for each add.
		/// </summary>
		double tadd;
		/// <summary>
		/// Total print time.
		/// </summary>
		int LastPrintTimeDate;



		/*
    ! These values control the number of layers that the program can to handle
    ! Used to be 501, 51, 50 (old removed values)
    integer, parameter:: npdf = 1001, npbl = 201, nleymax = 200
		*/


		/*
    ! Group One
    real*8:: dz(nleymax), dz1(nleymax)
    real*8:: da, du0, dudz10, dudz11, dudz21, &
             e0, e00(npdf), gc(nleymax), gs(nleymax), &
             gsdf(2 * nleymax), gw, hbl(nleymax), &
             hdf(nleymax), ddf1, pk0, sett, sett1, sfin, &
             sfin1, tprint, ucon, ucon1, vril, zk0, &
             a(npdf), a1(npbl), af(npdf), af1(npbl), &
             bf(npdf), bf1(npbl)
    real*8:: time, tau
    integer:: ndflayer
		*/
		double[] 
			dz = new double[nleymax + 1], dz1 = new double[nleymax + 1];

		double 
			da, dudz10, dudz11, dudz21, pk0;
		double[] 
			gc = new double[nleymax + 1], gs = new double[nleymax + 1],
			af = new double[npdf + 1], af1 = new double[npbl + 1],
			bf = new double[npdf + 1], bf1 = new double[npbl + 1];
		int ndflayer;

		/*
    ! Group Two
    integer:: matindex
    real*8:: e(npdf), e1(npdf), &
             e11(npbl), efin(npdf), efin1(npbl), er(npbl), &
             effsr(npdf), efstr1(npbl), f(npdf), f1(npbl)
    real*8:: pre_e(npdf), pre_er(npbl)  ! Previous void ratio saving for recovery
    integer:: nblpoint, ndfpoint
    integer:: pre_ndfpoint  ! Previous total number of nodes for recovery
    real*8:: pre_time  ! Previous time for recovery
    integer, dimension(nleymax):: nmat, nsub, nsub1
		*/
		double matindex;

		/// <summary>
		/// Previous void ratio saving for recovery.
		/// </summary>
		double[]
			pre_e = new double[npdf + 1], pre_er = new double[npbl + 1];
		int nblpoint, ndfpoint;
		/// <summary>
		/// Previous total number of nodes for recovery.
		/// </summary>
		int pre_ndfpoint;
		/// <summary>
		/// Previous time for recovery.
		/// </summary>
		double pre_time;
		int[] nmat = new int[nleymax + 1];

		/*
    ! Group Four
    real*8:: totstr(npdf), tostr1(npbl), u(npdf), u1(npbl), u0(npdf), &
             u01(npbl), uw(npdf), uw1(npbl), xi(npdf), xi1(npbl), z(npdf), &
             z1(npbl), fint(npdf), fint1(npbl), aev, ce, cset, dreff, &
             dsc, dset, dtim, qdf, setc, setd, tds, tpm, &
             vrint, xel, ep(12), et(npdf), pep(12), rf(12), wtelev, &
             add, tadd, hhbl, hhdf, setsbl, setsdf, ffint(npdf), ffint1(npbl)
		*/
		double 
			aev, cset, dsc, dset, dtim, qdf, setc, 
			vrint;
		double[]
			fint = new double[npdf + 1], fint1 = new double[npbl + 1],
			ep = new double[12 + 1], et = new double[npdf + 1],
			ffint = new double[npdf + 1], ffint1 = new double[npbl + 1];

		/*
    ! Group Five
    integer:: mtime, in = 10, iouts = 13, ins = 14
    integer:: nbl, nflag, nm, npt, nnd, ntime, &
              m, mm, ms, ndfcons, nsc, &
              ntypescompress, ntypedredge, &
              numbl, numdf
    integer:: nloop
    real*8:: pretime, preelev
    integer, dimension(nleymax):: ldf(2 * nleymax), ibdl, iddf
    real*8:: efsbot, hsolids, acumel, cacc(2 * nleymax), crcc(2 * nleymax)
		*/





		int nflag, nm, nnd, m, mm, ndfcons;
		int nloop;
		double pretime, preelev;

		int[] ibdl = new int[nleymax + 1];
		double efsbot, hsolids, acumel;


		/*
    ! Group Seven
    real*8:: auxdf(15, npdf), tpdf(nleymax), difsecdf(nleymax)
		*/
		double[,] auxdf = new double[15 + 1, npdf + 1];
		double[] tpdf = new double[nleymax + 1], difsecdf = new double[nleymax + 1];

		/*
    ! Group Eight
    real*8:: auxbl(15, npbl), tpbl(nleymax), difsecbl(nleymax)
    logical*1:: primdf(nleymax), primbl(nleymax)
    character(len=60):: problemname
		*/
		double[,] auxbl = new double[15 + 1, npbl + 1];
		double[] tpbl = new double[nleymax + 1], difsecbl = new double[nleymax + 1];
		bool[] IsDredgedFillInPrimaryConsolidations = new bool[nleymax + 1], IsCompressibleFoundationInPrimaryConsolidations = new bool[nleymax + 1];

		/*
    ! Input and output variables
    integer:: iout = 11, iplot = 12, igracf = 15, igradf = 16, ngraph = 0, recoveryout = 20
    real*8:: qdfold = 0.0
		*/

		int ngraph = 0;
		double qdfold = 0;

		/*
    ! MOVED THESE VARIABLE TO SOLVE USE BEFORE VALUE PROBLEM
    real*8:: vri1, hdf1
    integer:: last_print
    integer, dimension(1000):: idbl
    real*8, dimension(1000):: effstr
		*/
		double vri1;


		/*
    ! Tells if the compressible foundation layer is over-consolidated
    ! OCR < 1 under-consolidated; = 1 normally consolidated; > 1 over-consolidated
    real*8, dimension(nleymax):: OCR
		*/


		/*
    ! Holds the values for the segment length and void ratio for recovery subroutine
    logical:: adjustflag(npdf)  ! Vector holding flag for adjusting e-p curves
    integer:: dimensionflag = 0
    integer:: recoveryflag  ! Tells if the program needs to create a recovery output file
    integer:: totaltypes, temp_id
    integer, dimension(1000):: indi_id
		*/
		/// <summary>
		/// Vector holding flag for adjusting e-p curves.
		/// True: Not adjusted; False: Adjusted.
		/// </summary>
		bool[] IsCurveAdjusteds = new bool[npdf + 1];

		int MaterialTypes, temp_id;
		int[] indi_id = new int[1000 + 1];

		/*
    ! Desiccation parameter modification to array form for each material type
    real*8, dimension (200):: sl, dl, h2, sat
    real*8, dimension (200):: layer_stress
		*/

		double[] layer_stress = new double[nleymax + 1];
	}
}

