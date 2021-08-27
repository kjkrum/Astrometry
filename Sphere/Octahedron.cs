﻿namespace CodeConCarne.Astrometry.Sphere
{
	internal static class Octahedron
	{
		private readonly static Vector[] V = new Vector[]
		{
			Vector.POS_Z,
			Vector.POS_X,
			Vector.POS_Y,
			Vector.NEG_X,
			Vector.NEG_Y,
			Vector.NEG_Z
		};

		/* Clockwise on the inside in a right-handed coordinate system. */

		private readonly static Trixel[] T = new Trixel[]
		{
			new Trixel(0b1000, 0, V[0], V[1], V[2]),
			new Trixel(0b1001, 0, V[0], V[2], V[3]),
			new Trixel(0b1011, 0, V[0], V[4], V[1]),
			new Trixel(0b1010, 0, V[0], V[3], V[4]),
			new Trixel(0b1100, 0, V[5], V[2], V[1]),
			new Trixel(0b1101, 0, V[5], V[3], V[2]),
			new Trixel(0b1110, 0, V[5], V[1], V[4]),
			new Trixel(0b1111, 0, V[5], V[4], V[3])
		};

		internal static long Init(double x, double y, double z, Mesh.Scratch scratch)
		{
			var i = (z < 0 ? 4 : 0) + (y < 0 ? 2 : 0) + (x < 0 ? 1 : 0);
			var t = T[i];
			var a = scratch.Array;
			a[Mesh.C + 0] = x;
			a[Mesh.C + 1] = y;
			a[Mesh.C + 2] = z;
			t.V0.CopyTo(a, Mesh.V0);
			t.V1.CopyTo(a, Mesh.V1);
			t.V2.CopyTo(a, Mesh.V2);
			return t.Id;
		}

		internal static void Init(Cover.Scratch scratch)
		{
			for (int i = 0; i < T.Length; ++i)
			{
				scratch.Queue.Enqueue(T[i]);
			}
		}
	}
}
