namespace PsddfCs {
	public partial class Psddf {
		/// <summary>
		/// Reset status
		/// </summary>
		public void ResetSimulation (string path) {
			for (int i = 1; i <= npdf; i++) af[i] = 0;
			// Set flag to normally consolidated;
			for (int i = 1; i <= nleymax; i++) CompressibleFoundationOCR[i] = 1;
			// Clear adjustflags ;
			for (int i = 1; i <= npdf; i++) IsCurveAdjusteds[i] = true;


		}
	}
}
