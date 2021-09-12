using Dawn;
using System;
using System.Collections.Generic;

namespace CodeConCarne.Astrometry.Sphere
{
	public static class Cover
	{
		// TODO rectanglar covers
		// public static void Rectangle(Vector look, Vector up, double ax, double ay, int depth, Scratch scratch) {}

		public static void Circle(Halfspace cap, int depth, Scratch scratch, List<Trixel> result)
		{
			Guard.Argument(depth, nameof(depth)).InRange(Mesh.MIN_DEPTH, Mesh.MAX_DEPTH);
			scratch.Clear();
			result.Clear();
			if (cap.Angle == 0 || cap.Distance == 1)
			{
				return;
			}
			if (cap.Angle == Math.PI || cap.Distance == -1)
			{
				Octahedron.Init(result);
				return;
			}
			var q = scratch.Queue;
			var c = scratch.Cover;
			Octahedron.Init(q);
			// TODO consider terminating recursion using the heuristic described in section 4.4
			while (q.Count > 0)
			{
				var t = q.Dequeue();
				var i = Intersect(t, cap);
				if (i == Intersection.Full || (i == Intersection.Partial && t.Depth == depth))
				{
					c.Add(t);
				}
				else if (i == Intersection.Partial)
				{
					t.EnqueueChildren(q);
				}
			}
			result.AddRange(c);
		}

		// TODO methods to group trixels into ranges, combine ranges, etc.

		private static Intersection Intersect(Trixel t, Halfspace h)
		{
			// formula 4.1
			// this use of Dot is one reason all Vectors are normalized
			var i0 = h.Normal.Dot(t.V0) > h.Distance;
			var i1 = h.Normal.Dot(t.V1) > h.Distance;
			var i2 = h.Normal.Dot(t.V2) > h.Distance;
			if (h.Distance >= 0)
			{
				// positive halfspace - section 4.1
				if (i0 && i1 && i2)
				{
					return Intersection.Full;
				}
				if (i0 || i1 || i2)
				{
					return Intersection.Partial;
				}
				if (Hole(t, h))
				{
					return Intersection.Partial;
				}
			}
			else
			{
				// negative halfspace - section 4.3 (ii)
				if (i0 && i1 && i2)
				{
					if (Hole(t, h))
					{
						return Intersection.Partial;
					}
					return Intersection.Full;
				}
				if (i0 || i1 || i2)
				{
					return Intersection.Partial;
				}
			}
			return Intersection.None;
		}

		private static bool Hole(Trixel t, Halfspace h)
		{
			// calculate bounding circle as a halfspace - formula 4.2
			var v = t.V1.Subtract(t.V0).Cross(t.V2.Subtract(t.V1)).Normalize();
			var d = t.V0.Dot(v);
			var b = Halfspace.FromDistance(v, d);

			// equivalent and simpler
			// TODO compute complement once up in Circle and pass it down?
			if (h.Distance < 0)
			{
				h = h.Complement();
			}

			// if halfspace cap overlaps trixel bounding circle
			if (h.Normal.Angle(b.Normal) < h.Angle + b.Angle)
			{
				// if cap intersects trixel edge return true - section 4.2 / figure 9(d)
				if (Edge(t.V0, t.V1, h) ||
					Edge(t.V1, t.V2, h) ||
					Edge(t.V2, t.V0, h)) return true;

				// if cap is fully inside trixel return true - formula 4.3 / figure 9(e)
				// normals to planes of great circles of trixel edges
				var n0 = t.V0.Cross(t.V1).Normalize();
				var n1 = t.V1.Cross(t.V2).Normalize();
				var n2 = t.V2.Cross(t.V0).Normalize();
				// halfspace vector points inside trixel
				if (n0.Dot(h.Normal) >= 0 &&
					n1.Dot(h.Normal) >= 0 &&
					n2.Dot(h.Normal) >= 0) return true;
			}
			return false;
		}

		private static bool Edge(UnitVector v0, UnitVector v1, Halfspace h)
		{
			// equation 4.8
			// -u²(γ₁+d)s² + (γ₁(u²-1)+γ₂(u²+1))s + γ₁-d = 0
			// d = h.Distance
			// θ = v0.Angle(v1)
			// u = tan(θ/2)
			// γ₁ = h.Normal.Dot(v0)
			// γ₂ = h.Normal.Dot(v1)
			var d = h.Distance;
			var a = v0.Angle(v1);
			var u = Math.Pow(Math.Tan(a / 2), 2); // always squared, so just square it here
			var g0 = h.Normal.Dot(v0);
			var g1 = h.Normal.Dot(v1);
			// equation 4.8 rewritten with our variables
			// -u(g0+d)s² + (g0(u-1) + g1(u+1))s + g0-d = 0
			var s = PositiveRoot(-u * (g0 + d), g0 * (u - 1) + g1 * (u + 1), g0 - d);
			// TODO use gte/lte?
			// TODO use epsilon?
			// TODO special case for zero distance halfspace coincident with trixel edge?
			// using lte/gte or epsilon here, with halfspace normal in the +Z axis and
			// distance 0, computing cover to depth > 0, depth 0 trixels in the +Z half
			// intersect at the edge and aren't whole.
			return s > 0 && s < 1;
		}

		private static double PositiveRoot(double a, double b, double c)
		{
			// TODO use the "better algorithm"?
			// https://en.wikipedia.org/wiki/Loss_of_significance#A_better_algorithm
			var pre = b * b - 4 * a * c;
			if(pre < 0)
			{
				return double.NaN;
			}
			return (Math.Sqrt(pre) - b) / (2 * a);
		}

		public class Scratch
		{
			internal readonly Queue<Trixel> Queue = new Queue<Trixel>();
			internal readonly List<Trixel> Cover = new List<Trixel>();

			internal void Clear()
			{
				Queue.Clear();
				Cover.Clear();
			}
		}
	}
}
