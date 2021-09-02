using Dawn;
using System.Runtime.CompilerServices;

namespace CodeConCarne.Astrometry.Sphere.V2
{
	public static class Mesh
	{
		internal const double EPSILON = 1E-15;
		public const int MIN_DEPTH = 0;
		public const int MAX_DEPTH = 20;

		public static Trixel Trixel(double x, double y, double z, int depth)
		{
			Guard.Argument(depth, nameof(depth)).InRange(MIN_DEPTH, MAX_DEPTH);
			var t = Octahedron.Init(x, y, z);
			var ray = new Vector(x, y, z);
			for (int d = 0; d < depth; ++d)
			{
				t.Midpoints(out Vector m0, out Vector m1, out Vector m2);
				// middle first because it's slightly larger
				if(Intersect(ray, m0, m1, m2))
				{
					t = new Trixel((t.Id << 2) + 3, t.Depth + 1, m0, m1, m2);
					continue;
				}
				// corner 0
				if (Intersect(ray, t.V0, m2, m1))
				{
					t = new Trixel(t.Id << 2, t.Depth + 1, t.V0, m2, m1);
					continue;
				}
				// corner 1
				if (Intersect(ray, t.V1, m0, m2))
				{
					t = new Trixel((t.Id << 2) + 1, t.Depth + 1, t.V1, m0, m2);
					continue;
				}
				// corner 2
				// assume match in release build
#if DEBUG
				if (Intersect(ray, t.V2, m1, m0))
				{
#endif
					t = new Trixel((t.Id << 2) + 2, t.Depth + 1, t.V2, m1, m0);
#if DEBUG
					continue;
				}
				throw new System.Exception("no intersection found");
#endif
			}
			return t;
		}

		// nice explanation of Möller-Trumbore
		// https://www.scratchapixel.com/lessons/3d-basic-rendering/ray-tracing-rendering-a-triangle/moller-trumbore-ray-triangle-intersection

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private static bool Intersect(Vector ray, Vector v0, Vector v1, Vector v2)
		{
			var e = v1.Subtract(v0);
			var p = ray.Cross(e);
			e = v2.Subtract(v0);
			var det = e.Dot(p);
			//if (det < EPSILON) return false;
			if (det < 0) return false;
			det = 1 / det;
			var t = v0.Invert();
			var u = t.Dot(p) * det;
			// TODO use epsilon here, or don't use it above?
			if (u < 0 || u > 1) return false;
			p = t.Cross(e);
			var v = ray.Dot(p) * det;
			return v >= 0 && u + v <= 1;
		}
	}
}
