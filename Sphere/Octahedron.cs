using static CodeConCarne.Astrometry.Sphere.Scratch;

namespace CodeConCarne.Astrometry.Sphere
{
	internal static class Octahedron
	{
		private readonly static Trixel[] FACES = new Trixel[]
		{
			new Trixel(0b1000, 1, new Vector(0, 0, 1), new Vector(1, 0, 0), new Vector(0, 1, 0)),
			new Trixel(0b1001, 1, new Vector(0, 0, 1), new Vector(0, 1, 0), new Vector(-1, 0, 0)),
			new Trixel(0b1010, 1, new Vector(0, 0, 1), new Vector(0, -1, 0), new Vector(1, 0, 0)),
			new Trixel(0b1011, 1, new Vector(0, 0, 1), new Vector(-1, 0, 0), new Vector(0, -1, 0)),
			new Trixel(0b1100, 1, new Vector(0, 0, -1), new Vector(0, 1, 0), new Vector(1, 0, 0)),
			new Trixel(0b1101, 1, new Vector(0, 0, -1), new Vector(-1, 0, 0), new Vector(0, 1, 0)),
			new Trixel(0b1110, 1, new Vector(0, 0, -1), new Vector(1, 0, 0), new Vector(0, -1, 0)),
			new Trixel(0b1111, 1, new Vector(0, 0, -1), new Vector(0, -1, 0), new Vector(-1, 0, 0)),
		};

		internal static long Init(double x, double y, double z, Scratch scratch)
		{
			var i = (z < 0 ? 4 : 0) + (y < 0 ? 2 : 0) + (x < 0 ? 1 : 0);
			var t = FACES[i];
			var a = scratch.Array;
			a[C + 0] = x;
			a[C + 1] = y;
			a[C + 2] = z;
			t.V0.CopyTo(a, V0);
			t.V1.CopyTo(a, V1);
			t.V2.CopyTo(a, V2);
			return t.Id;
		}
	}
}
