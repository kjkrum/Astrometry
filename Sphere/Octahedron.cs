using System.Collections.Generic;
using static CodeConCarne.Astrometry.Sphere.UnitVector;

namespace CodeConCarne.Astrometry.Sphere
{
	internal static class Octahedron
	{
		/* Clockwise on the inside in a right-handed coordinate system. */

		private static readonly Trixel[] T = new Trixel[]
		{
			new Trixel(0b1111, 0, POS_Y, POS_Z, POS_X),	// N3
			new Trixel(0b1110, 0, NEG_X, POS_Z, POS_Y),	// N2
			new Trixel(0b1100, 0, POS_X, POS_Z, NEG_Y),	// N0
			new Trixel(0b1101, 0, NEG_Y, POS_Z, NEG_X),	// N1
			new Trixel(0b1000, 0, POS_X, NEG_Z, POS_Y),	// S0
			new Trixel(0b1001, 0, POS_Y, NEG_Z, NEG_X),	// S1
			new Trixel(0b1011, 0, NEG_Y, NEG_Z, POS_X),	// S3
			new Trixel(0b1010, 0, NEG_X, NEG_Z, NEG_Y)	// S2
		};

		internal static Trixel Init(double x, double y, double z)
		{
			return T[(z < 0 ? 4 : 0) + (y < 0 ? 2 : 0) + (x < 0 ? 1 : 0)];
		}

		internal static void Init(List<Trixel> list)
		{
			list.AddRange(T);
		}

		internal static void Init(Queue<Trixel> queue)
		{
			foreach(var t in T)
			{
				queue.Enqueue(t);
			}
		}
	}
}
