namespace PsddfCs {
	public partial class Psddf {
		const int DredgedFillMaxSublayers = 1001, CompressibleFoundationMaxSublayers = 201;
		/// <summary>
		/// Max number of material types.
		/// </summary>
		const int MaxMaterialTypes = 200;
		const int Dimension1 = 100, Dimension2 = 200;

		#region IO Channels
		public readonly ICmd Cmd;
		public readonly IIo Io;

		/// <summary>
		/// Input Channel: Main input.
		/// </summary>
		public const int IN = 10;
		/// <summary>
		/// Output Channel: Main Output / Calculations.
		/// </summary>
		public const int OUT = 11;
		/// <summary>
		/// Input Channel: Stores the output from a previous simulation so that it can be restarted.
		/// Available when ndata1 == 2, read from continuation.
		/// </summary>
		public const int INS = 14;
		/// <summary>
		/// Output Channel: Stores the output from a previous simulation so that it can be restarted;
		/// Available when ndata2 == 2, save for continuation.
		/// </summary>
		public const int OUTS = 13;
		/// <summary>
		/// Output Channel: Surface Elevation data.
		/// </summary>
		public const int PLOT = 12;
		/// <summary>
		/// Output Channel: Compressible Foundation Results.
		/// </summary>
		public const int GCF = 15;
		/// <summary>
		/// Output Channel: Dredged Material Results.
		/// </summary>
		public const int GDF = 16;
		/// <summary>
		/// Output Channel: For use with the CAP model.
		/// </summary>
		public const int RCY = 20;
		/// <summary>
		/// Output Channel: For outputing the PSI file.
		/// </summary>
		public const int INOUT = 30;

		#endregion

		#region Group A - Description
		public event System.Action<double> OnProgressUpdated;
		public event System.Action<Psddf> OnPrintTimeReached;

		/// <summary>
		/// Path or name of the input data file.
		/// </summary>
		public string InputFilePath;
		/// <summary>
		/// Path or name of the output data file.
		/// Need to be set before calling Ifn().
		/// </summary>
		public string OutputFilePath = "p_temp_opt", ContinuationFilePath = "temp_contfile";
		/// <summary>
		/// Logical denotion the following options:
		/// true = print simulation process.
		/// false = do not print simulation process.
		/// </summary>
		public bool IsPrintProgress;
		public double Progress;

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

		#region Group B - Execution

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

		#region Group C - Incompressible Foundation

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
		/// γw - unit weight of soil solids
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
		public double SecondaryCompressionExcessPoreWaterPressureLimit;
		/// <summary>
		/// Kailang: Now secondary compression will be turned on automatically if the degree of consolidation is larger than maxUcon.
		/// </summary>
		const double MaxConsolidationDegree = 0.96;

		#endregion

		#region Group D - Number of Materials

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
		#endregion
		
		#region Group D1 - Compressible Foundation Layers

		/// <summary>
		/// HBL
		/// Initial thickness of compressible foundation layer. 
		/// Repeat NUMBL times.
		/// </summary>
		public readonly double[] CompressibleFoundationInitialThicknesses = new double[MaxMaterialTypes + 1];
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
		public readonly int[] CompressibleFoundationSublayers = new int[MaxMaterialTypes + 1];
		/// <summary>
		/// OCR
		/// Average overconsolidation ratio (OCR) at the middle of each compressible foundation layer.
		/// </summary>
		public readonly double[] CompressibleFoundationOCR = new double[MaxMaterialTypes + 1];

		#endregion

		#region Group D2 - Common Mateiral Properties

		/// <summary>
		/// Material IDs
		/// </summary>
		public readonly int[] MaterialIDs = new int[MaxMaterialTypes + 1];
		/// <summary>
		/// GSDF
		/// Specific gravity of soil solids of the compressible foundation material type KOM.
		/// </summary>
		public readonly double[] SpecificGravities = new double[2 * MaxMaterialTypes];
		/// <summary>
		/// CACC
		/// Ratio between secondary compression index, Ca, and Compression index, Cc from standard oedometer tests. 
		/// </summary>
		public readonly double[] CaCcs = new double[2 * MaxMaterialTypes];
		/// <summary>
		/// CRCC
		/// Ratio between recompression index, Cr, and Compression index, Cc from standard oedometer tests. 
		/// </summary>
		public readonly double[] CrCcs = new double[2 * MaxMaterialTypes];
		/// <summary>
		/// LDF
		/// Number of subsequent lines of data points that will be entered to define the void ratio-effective stress and void ratio-permeability relationships for each material type (KOM). 
		/// </summary>
		public readonly int[] RelationDefinitionLines = new int[2 * MaxMaterialTypes];
		#endregion

		#region Group D3 - Common Material Relation Definitions
		/// <summary>
		/// e - void ratio
		/// VOIDRATIO (I,KOM)
		/// Measured void ratios of compressible foundation layer under effective stresses applied in laboratory consolidation tests.
		/// </summary>
		public readonly double[,] VoidRatios = new double[Dimension1 + 1, Dimension2 + 1];
		/// <summary>
		/// σ' - effective stress
		/// EFFECTIVESTRESS (I,KOM) 
		/// Effective stresses applied in laboratory consolidation tests corresponding to the void ratios entered above.
		/// </summary>
		public readonly double[,] EffectiveStresses = new double[Dimension1 + 1, Dimension2 + 1];
		/// <summary>
		/// k(e) - coefficient of soil permeability as a function of void ratio 
		/// PERM (I,KOM) 
		/// Permeabilities from the laboratory consolidation tests on the compressible foundation layer corresponding to the void ratios entered above.
		/// </summary>
		public readonly double[,] Permeabilities = new double[Dimension1 + 1, Dimension2 + 1];
		#endregion

		#region Group D4 - Dredged Fill Material Specific Properties
		/// <summary>
		/// SL
		/// Saturation limit of the dredged fill defined as the void ratio which separates first-stage desiccation from second-stage desiccation. 
		/// </summary>
		public readonly double[] DredgedFillSaturationLimits = new double[MaxMaterialTypes + 1];
		/// <summary>
		/// DL
		/// Desiccation limit of the dredged fill defined as the lowest void ratio the material will attain during second-stage drying. 
		/// </summary>
		public readonly double[] DredgedFillDesiccationLimits = new double[MaxMaterialTypes + 1];
		/// <summary>
		/// H2
		/// The maximum depth to which second-stage drying will occur (max. crust thickness) in the dredged fill.
		/// </summary>
		public readonly double[] DredgedFillDryingMaxDepth = new double[MaxMaterialTypes + 1];
		/// <summary>
		/// SAT
		/// The average degree of saturation, expressed as a fraction of 1.0, of the dredged fill when dried to the desiccation limit, DL.
		/// </summary>
		public readonly double[] DredgedFillAverageSaturation = new double[MaxMaterialTypes + 1];

		#endregion

		#region Group E - Print Times
		/// <summary>
		/// Number of output or print times during simulation. 
		/// The value of NTIME must be less than or equal to 1000. 
		/// </summary>
		public int PrintTimes, StartPrintTime;
		#endregion

		#region Group F - First Dredged Fill Layer
		/// <summary>
		/// HDF
		/// Initial thickness of dredged fill layers. 
		/// </summary>
		public readonly double[] DredgedFillInitialThicknesses = new double[MaxMaterialTypes + 1];
		/// <summary>
		/// TDS
		/// Number of time periods (usually days) from the addition of the first layer of dredged fill to the start of desiccation in the first layer of dredged fill.
		/// </summary>
		public double DredgedFillDesiccationDelayDays, DredgedFillInitialDesiccationDelayDays;
		/// <summary>
		/// MS
		/// The number of the month at which desiccation starts in the first layer of dredged fill. (January = 1)
		/// </summary>
		public int DredgedFillDesiccationDelayMonths, DredgedFillInitialDesiccationDelayMonths;
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
		public readonly double[] DredgedFillInitialVoidRatios = new double[DredgedFillMaxSublayers + 1];
		/// <summary>
		/// IDDF
		/// Material identification number for the dredged fill layer added at PRINTT(I). 
		/// </summary>
		public readonly int[] DredgedFillMaterialIDs = new int[MaxMaterialTypes + 1];
		/// <summary>
		/// NSUB
		/// Number of sublayers that the dredged fill layer is divided into for computational purposes. 
		/// </summary>
		public readonly int[] DredgedFillSublayers = new int[MaxMaterialTypes + 1];
		#endregion

		#region Group G - New Dredged Fill Layers
		/// <summary>
		/// PRINTT
		/// Time at which the properties of the consolidating layers will be printed and/or a new layer of dredged fill will be applied. 
		/// </summary> 		
		public readonly double[] PrintTimeDates = new double[1000 + 1];
		/// <summary>
		/// Number of dredge fill layer added.
		/// </summary>
		public int DredgedFillLayers;
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

		#region Group H - Desiccation
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

		#region Group I - Percipitation
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

		#region Group J - Continuation Data

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


		#region Group K - Simulation Output

		/// <summary>
		/// The simulation time.
		/// </summary>
		public double CurrentTime;
		/// <summary>
		/// τ - time interval of the explicit finite difference analysis 
		/// TAU
		/// Time interval of the explicit finite difference analysis.
		/// </summary>
		public double TimeStep;
		/// <summary>
		/// Coordinates of nodal points.
		/// </summary>
		public readonly double[]
			DredgedFillCoordA = new double[DredgedFillMaxSublayers + 1], CompressibleFoundationCoordA = new double[CompressibleFoundationMaxSublayers + 1],
			DredgedFillCoordXI = new double[DredgedFillMaxSublayers + 1], CompressibleFoundationCoordXI = new double[CompressibleFoundationMaxSublayers + 1],
			DredgedFillCoordZ = new double[DredgedFillMaxSublayers + 1], CompressibleFoundationCoordZ = new double[CompressibleFoundationMaxSublayers + 1];
		/// <summary>
		/// Initial void ratio at each coordinate point.
		/// </summary>
		public readonly double[] 
			DredgedFillInitialVoidRatio = new double[DredgedFillMaxSublayers + 1], 
			CompressibleFoundationInitialVoidRatio = new double[CompressibleFoundationMaxSublayers + 1];
		/// <summary>
		/// Current void ratio at each coordinate point.
		/// </summary>
		public readonly double[] 
			DredgedFillCurrentVoidRatio = new double[DredgedFillMaxSublayers + 1], 
			CompressibleFoundationCurrentVoidRatio = new double[CompressibleFoundationMaxSublayers + 1];
		/// <summary>
		/// Final void ratio at each coordinate point.
		/// </summary>
		public readonly double[] 
			DredgedFillFinalVoidRatio = new double[DredgedFillMaxSublayers + 1], 
			CompressibleFoundationFinalVoidRatio = new double[CompressibleFoundationMaxSublayers + 1];
		/// <summary>
		/// Total stress at each coordinate point.
		/// </summary>
		public readonly double[] 
			DredgedFillTotalStress = new double[DredgedFillMaxSublayers + 1], 
			CompressibleFoundationTotalStress = new double[CompressibleFoundationMaxSublayers + 1];
		/// <summary>
		/// Effective stress at each coordinate point.
		/// </summary>
		public readonly double[] 
			DredgedFillEffectiveStress = new double[DredgedFillMaxSublayers + 1], 
			CompressibleFoundationEffectiveStress = new double[CompressibleFoundationMaxSublayers + 1], 
			f = new double[DredgedFillMaxSublayers + 1], f1 = new double[CompressibleFoundationMaxSublayers + 1];
		/// <summary>
		/// uw - total pore-water pressure
		/// Total pore pressure at each coordinate point.
		/// </summary>
		public readonly double[] 
			DredgedFillTotalPoreWaterPressure = new double[DredgedFillMaxSublayers + 1], 
			CompressibleFoundationTotalPoreWaterPressure = new double[CompressibleFoundationMaxSublayers + 1];
		/// <summary>
		/// u0 - hydrostatic pore-water pressure
		/// Static pore pressure at each coordinate point.
		/// </summary>
		public readonly double[] 
			DredgedFillHydrostaticPoreWaterPressure = new double[DredgedFillMaxSublayers + 1], 
			CompressibleFoundationHydrostaticPoreWaterPressure = new double[CompressibleFoundationMaxSublayers + 1];
		/// <summary>
		/// u(z,t) = uw(z,t) - u0(z,t) - excess pore-water pressure
		/// Excess pore-water pressure at each coordinate point.
		/// </summary>
		public readonly double[] 
			DredgedFillExcessPoreWaterPressure = new double[DredgedFillMaxSublayers + 1], CompressibleFoundationExcessPoreWaterPressure = new double[CompressibleFoundationMaxSublayers + 1];
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

		#region Group L - Initialized before Simulation
		
		/// <summary>
		/// HHBL
		/// Total initial thickness of compressible foundation layer.
		/// The sum of hbl.
		/// </summary>
		double CompressibleFoundationTotalInitialThickness;
		/// <summary>
		/// HHDF
		/// Totle initial thickness of dredged fill layers.
		/// The sum of hdf.
		/// </summary>
		double DredgedFillTotleInitialThickness;
		/// <summary>
		/// Total number of material types
		/// </summary>
		int TotalMaterialTypes;
		/// <summary>
		/// Total print time.
		/// </summary>
		int TotalTime;
		/// <summary>
		/// Integer denoting the following options:
		/// 1 = compressible foundation.
		/// 2 = noncompressible foundation.
		/// </summary>
		int IsFoundationCompressible;
		/// <summary>
		/// Vector holding flag for adjusting e-p curves.
		/// True: Not adjusted; False: Adjusted.
		/// </summary>
		bool[] IsCurveNotAdjusteds = new bool[DredgedFillMaxSublayers + 1];

		/// <summary>
		/// γs - unit weight of soil solids 
		/// GS
		/// </summary>
		double[] SoilUnitWeight = new double[MaxMaterialTypes + 1];
		/// <summary>
		/// γc - buoyant unit weight of solids (γc = γs - γw) 
		/// GC
		/// </summary>
		double[] SoilBuoyantUnitWeight = new double[MaxMaterialTypes + 1];
		#endregion

		/// <summary>
		/// pk = k(e) / (1 + e)
		/// pk(i, k) = perm(i, k) / (1.0 + voidratio(i, k))
		/// </summary>
		double[,] PK = new double[Dimension1 + 1, Dimension2 + 1];
		/// <summary>
		/// dσ' / de
		/// Slope of stress-void ratio curve.
		/// </summary>
		double[,] Dsde = new double[Dimension1 + 1, Dimension2 + 1];
		/// <summary>
		/// β(e) = (d / de)(k(e) / (1 + e)) - a function of the void ratio and permeability
		/// Slope of permeability function.
		/// </summary>
		double[,] Beta = new double[Dimension1 + 1, Dimension2 + 1];
		/// <summary>
		/// α(e) = (k(e) / (1 + e))(dσ' / de) - a function of void ratio, permeability, and compressibility
		/// Permeability function times dsde.
		/// </summary>
		double[,] Alpha = new double[Dimension1 + 1, Dimension2 + 1];


		/// <summary>
		/// The next time at which the properties of the consolidating layers will be printed and/or a new layer of dredged fill will be applied. 
		/// </summary>
		double NextPrintDate;
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


		/*
    ! These values control the number of layers that the program can to handle
    ! Used to be 501, 51, 50 (old removed values)
    integer, parameter:: npdf = 1001, npbl = 201, nleymax = 200
		*/
		double[]
			af = new double[DredgedFillMaxSublayers + 1], af1 = new double[CompressibleFoundationMaxSublayers + 1],
			bf = new double[DredgedFillMaxSublayers + 1], bf1 = new double[CompressibleFoundationMaxSublayers + 1];
		double[]
			fint = new double[DredgedFillMaxSublayers + 1], fint1 = new double[CompressibleFoundationMaxSublayers + 1],
			ffint = new double[DredgedFillMaxSublayers + 1], ffint1 = new double[CompressibleFoundationMaxSublayers + 1];

		double[] ep = new double[12 + 1], et = new double[DredgedFillMaxSublayers + 1];


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
		double[,] dvds = new double[Dimension1 + 1, Dimension2 + 1];

		double da, dudz10, dudz11, dudz21, pk0;

		int DredgedFillCurrentLayer;

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
		double[] pre_e = new double[DredgedFillMaxSublayers + 1], pre_er = new double[CompressibleFoundationMaxSublayers + 1];
		int CompressibleFoundationTotalSublayers, DredgedFillTotalSublayers;
		/// <summary>
		/// Previous total number of nodes for recovery.
		/// </summary>
		int pre_ndfpoint;
		/// <summary>
		/// Previous time for recovery.
		/// </summary>
		double pre_time;
		
		double 
			aev, cset, dsc, dset, dtim, qdf, setc, 
			vrint, vri1;

		int nflag, nnd, m, mm, ndfcons,nloop;
		double pretime, preelev;

		double efsbot, hsolids, acumel;

		int ngraph = 0;
		double qdfold = 0;

		int temp_id;

		double[,] auxdf = new double[15 + 1, DredgedFillMaxSublayers + 1];
		double[,] auxbl = new double[15 + 1, CompressibleFoundationMaxSublayers + 1];

		int[] indi_id = new int[1000 + 1];

		int[] ibdl = new int[MaxMaterialTypes + 1];
		double[] layer_stress = new double[MaxMaterialTypes + 1];

		double[] dz = new double[MaxMaterialTypes + 1], dz1 = new double[MaxMaterialTypes + 1];

		double[] tpdf = new double[MaxMaterialTypes + 1], difsecdf = new double[MaxMaterialTypes + 1];
		double[] tpbl = new double[MaxMaterialTypes + 1], difsecbl = new double[MaxMaterialTypes + 1];

		bool[] IsDredgedFillInPrimaryConsolidations = new bool[MaxMaterialTypes + 1];
		bool[] IsCompressibleFoundationInPrimaryConsolidations = new bool[MaxMaterialTypes + 1];
	}
}

