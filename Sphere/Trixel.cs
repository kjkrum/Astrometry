using CodeConCarne.Astrometry.Sphere;
using System;

namespace CodeConCarne.Astrometry
{
	public sealed class Trixel
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

		internal Trixel(long id, int depth, Mesh.Scratch scratch)
		{
			Id = id;
			Depth = depth;
			V0 = new Vector(scratch.Array, Mesh.Scratch.V0);
			V1 = new Vector(scratch.Array, Mesh.Scratch.V1);
			V2 = new Vector(scratch.Array, Mesh.Scratch.V2);
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
