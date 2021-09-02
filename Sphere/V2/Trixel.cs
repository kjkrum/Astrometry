using System;
using System.Runtime.CompilerServices;

namespace CodeConCarne.Astrometry.Sphere.V2
{
	public readonly struct Trixel
	{
		readonly public long Id;
		readonly public int Depth;
		readonly public Vector V0;
		readonly public Vector V1;
		readonly public Vector V2;

		internal Trixel(long id, int depth, Vector v0, Vector v1, Vector v2)
		{
			Id = id;
			Depth = depth;
			V0 = v0;
			V1 = v1;
			V2 = v2;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal void Midpoints(out Vector m0, out Vector m1, out Vector m2)
		{
			m0 = V1.Midpoint(V2);
			m1 = V2.Midpoint(V0);
			m2 = V0.Midpoint(V1);
		}

		public double Area()
		{
			var a = V0.Distance(V1);
			var b = V1.Distance(V2);
			var c = V2.Distance(V0);
			var s = (a + b + c) / 2;
			return Math.Sqrt(s * (s - a) * (s - b) * (s - c));
		}
	}
}
