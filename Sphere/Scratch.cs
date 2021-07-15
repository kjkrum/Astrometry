namespace CodeConCarne.Astrometry.Sphere
{
	public class Scratch
	{
		// order of offsets affects performance
		// maybe proximity of reads and writes
		internal const int V0 = 0;
		internal const int M2 = 3;
		internal const int V1 = 6;
		internal const int M0 = 9;
		internal const int V2 = 12;
		internal const int M1 = 15;

		internal const int E = 18;
		internal const int C = 21;
		internal const int P = 24;
		internal const int T = 27;

		internal double[] Array = new double[30];
	}
}
