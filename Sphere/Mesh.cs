using Dawn;
using System;

namespace CodeConCarne.Astrometry.Sphere
{
	// based on Indexing the Sphere with the Hierarchical Triangular Mesh by Szalay et al
	// https://arxiv.org/ftp/cs/papers/0701/0701164.pdf

	public static class Mesh
	{
		// comment in reference implementation said this epsilon value failed, but it seems to work
		internal const double EPSILON = 1E-15;

		// order of offsets affects performance
		// maybe proximity of reads and writes
		private const int V0 = 0;
		private const int M2 = 3;
		private const int V1 = 6;
		private const int M0 = 9;
		private const int V2 = 12;
		private const int M1 = 15;

		private const int E = 18;
		private const int C = 21;
		private const int P = 24;
		private const int T = 27;

		public static double[] Scratch()
		{
			return new double[30];
		}

		public static long Id(double x, double y, double z, int depth, double[] scratch)
		{
			scratch[C + 0] = x;
			scratch[C + 1] = y;
			scratch[C + 2] = z;
			return Calc(depth, scratch);
		}

		public static Trixel Trixel(double x, double y, double z, int depth, double[] scratch)
		{
			scratch[C + 0] = x;
			scratch[C + 1] = y;
			scratch[C + 2] = z;
			var id = Calc(depth, scratch);
			var v0 = new Vertex(scratch[V0 + 0], scratch[V0 + 1], scratch[V0 + 2]);
			var v1 = new Vertex(scratch[V1 + 0], scratch[V1 + 1], scratch[V1 + 2]);
			var v2 = new Vertex(scratch[V2 + 0], scratch[V2 + 1], scratch[V2 + 2]);
			return new Trixel(id, depth, v0, v1, v2);
		}

		unsafe private static long Calc(int depth, double[] scratch)
		{
			// from the paper:
			//
			// "A 64-bit integer can hold an HtmID up to depth 31. However, standard double precision
			// transcendental functions break down at depth 26 where the trixel sides reach dimensions
			// below 10E-15 (depth 26 is about 10 milli-arcseconds for astronomers or 30 centimeters
			// on the earth’s surface.)"
			//
			// this manifests as no ray-triangle intersection at depth 25 or 26.

			Guard.Argument(depth, nameof(depth)).InRange(0, 20);
			Guard.Argument(scratch, nameof(scratch)).Count(30);

			fixed (double* a = scratch)
			{
				var face = Face(a);
				var id = 0b1000L + face;
				Array.Copy(INIT, face * 9 + 0, scratch, V0, 3);
				Array.Copy(INIT, face * 9 + 3, scratch, V1, 3);
				Array.Copy(INIT, face * 9 + 6, scratch, V2, 3);

				for (int d = 0; d < depth; ++d)
				{
					id <<= 2;
					Midpoints(a);
					// "Beyond depth 7, the curvature becomes irrelevant..."
					//
					// only normalizing below depth 4 still produces a good
					// distribution, with a middle to corner area ratio of
					// 2.113 instead of 2.106.
					if (d < 4)
					{
						Normalize(a);
					}
					// middle first because it's slightly larger
					if (Intersect(a, M0, M1, M2))
					{
						id += 3;
						Copy(a, M0, V0);
						Copy(a, M1, V1);
						Copy(a, M2, V2);
						continue;
					}
					// corner 0
					if (Intersect(a, V0, M2, M1))
					{
						Copy(a, M2, V1);
						Copy(a, M1, V2);
						continue;
					}
					// corner 1
					if (Intersect(a, V1, M0, M2))
					{
						id += 1;
						Copy(a, V1, V0);
						Copy(a, M0, V1);
						Copy(a, M2, V2);
						continue;
					}
					// corner 2
#if DEBUG
					if (Intersect(a, V2, M1, M0))
					{
#endif
						id += 2;
						Copy(a, V2, V0);
						Copy(a, M1, V1);
						Copy(a, M0, V2);
#if DEBUG
						continue;
					}
					throw new Exception("no intersection found");
#endif
				}
				return id;
			}
		}

		// nice explanation of Möller-Trumbore
		// https://www.scratchapixel.com/lessons/3d-basic-rendering/ray-tracing-rendering-a-triangle/moller-trumbore-ray-triangle-intersection

		unsafe private static bool Intersect(double* a, int v0, int v1, int v2)
		{
			// switched edges when excluding backface intersection
			// TODO double check that triangles face inward in the vertex order of popular 3D libs
			Subtract(a, v1, v0, E); // was E1
			Cross(a, C, E, P); // was E1
			Subtract(a, v2, v0, E); // all E0 from here
			var det = Dot(a, E, P);
			// if determinant is near zero, ray misses triangle
			// if determinant is negative, triangle is back facing
			if (det < EPSILON) return false;
			var invDet = 1 / det;
			Negate(a, v0, T);
			var u = Dot(a, T, P) * invDet;
			if (u < 0 || u > 1) return false;
			Cross(a, T, E, P); // Q reuses P
			var v = Dot(a, C, P) * invDet;
			return v >= 0 && u + v <= 1;
		}

		unsafe private static void Negate(double* a, int value, int result)
		{
			a[result + 0] = -a[value + 0];
			a[result + 1] = -a[value + 1];
			a[result + 2] = -a[value + 2];
		}

		unsafe private static void Cross(double* a, int left, int right, int result)
		{
			a[result + 0] = a[left + 1] * a[right + 2] - a[left + 2] * a[right + 1];
			a[result + 1] = a[left + 2] * a[right + 0] - a[left + 0] * a[right + 2];
			a[result + 2] = a[left + 0] * a[right + 1] - a[left + 1] * a[right + 0];
		}

		unsafe private static double Dot(double* a, int left, int right)
		{
			return a[left + 0] * a[right + 0] + a[left + 1] * a[right + 1] + a[left + 2] * a[right + 2];
		}

		unsafe private static void Subtract(double* a, int left, int right, int result)
		{
			a[result + 0] = a[left + 0] - a[right + 0];
			a[result + 1] = a[left + 1] - a[right + 1];
			a[result + 2] = a[left + 2] - a[right + 2];
		}

		unsafe private static void Midpoints(double* a)
		{
			a[M0 + 0] = (a[V1 + 0] + a[V2 + 0]) / 2;
			a[M0 + 1] = (a[V1 + 1] + a[V2 + 1]) / 2;
			a[M0 + 2] = (a[V1 + 2] + a[V2 + 2]) / 2;

			a[M1 + 0] = (a[V2 + 0] + a[V0 + 0]) / 2;
			a[M1 + 1] = (a[V2 + 1] + a[V0 + 1]) / 2;
			a[M1 + 2] = (a[V2 + 2] + a[V0 + 2]) / 2;

			a[M2 + 0] = (a[V0 + 0] + a[V1 + 0]) / 2;
			a[M2 + 1] = (a[V0 + 1] + a[V1 + 1]) / 2;
			a[M2 + 2] = (a[V0 + 2] + a[V1 + 2]) / 2;
		}

		unsafe private static void Normalize(double* a)
		{
			var d = Distance(a, M0);
			a[M0 + 0] /= d;
			a[M0 + 1] /= d;
			a[M0 + 2] /= d;

			d = Distance(a, M1);
			a[M1 + 0] /= d;
			a[M1 + 1] /= d;
			a[M1 + 2] /= d;

			d = Distance(a, M2);
			a[M2 + 0] /= d;
			a[M2 + 1] /= d;
			a[M2 + 2] /= d;
		}

		unsafe private static double Distance(double* a, int i)
		{
			var x = a[i + 0];
			var y = a[i + 1];
			var z = a[i + 2];
			return Math.Sqrt(x * x + y * y + z * z);
		}

		unsafe private static void Copy(double* a, int src, int dst)
		{
			a[dst + 0] = a[src + 0];
			a[dst + 1] = a[src + 1];
			a[dst + 2] = a[src + 2];
		}

		unsafe private static int Face(double* a)
		{
			return (a[C + 2] < 0 ? 4 : 0) + (a[C + 1] < 0 ? 2 : 0) + (a[C + 0] < 0 ? 1 : 0);
		}

		private static readonly double[] INIT = new double[]
		{
			0, 0, 1,    1, 0, 0,    0, 1, 0,
			0, 0, 1,    0, 1, 0,    -1, 0, 0,
			0, 0, 1,    0, -1, 0,   1, 0, 0,
			0, 0, 1,    -1, 0, 0,   0, -1, 0,
			0, 0, -1,   0, 1, 0,    1, 0, 0,
			0, 0, -1,   -1, 0, 0,   0, 1, 0,
			0, 0, -1,   1, 0, 0,    0, -1, 0,
			0, 0, -1,   0, -1, 0,   -1, 0, 0
		};
	}
}
