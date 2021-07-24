using Dawn;
using System;
using System.Collections;
using System.Collections.Generic;

namespace CodeConCarne.Astrometry.Sphere
{
	public static class Cover
	{
		// angle/angle or angle/aspect?
		// public static void Rectangle(Vector look, Vector up, double ax, double ay, int depth, Scratch scratch) {}

		public static void Circle(Vector look, double angle, int depth, Scratch scratch, List<Trixel> result)
		{
			Guard.Argument(angle, nameof(angle)).InRange(0, Math.PI);
			Guard.Argument(depth, nameof(depth)).InRange(Mesh.MIN_DEPTH, Mesh.MAX_DEPTH);
			var h = Halfspace.FromAngle(look, angle);
			scratch.Clear();
			Octahedron.Init(scratch);
			var q = scratch.Queue;
			var c = scratch.Cover;
			// TODO consider terminating recursion using the heuristic described in section 4.4
			while (q.Count > 0)
			{
				var t = q.Dequeue();
				var i = Intersect(t, h);
				if(i == Intersection.Full || (i == Intersection.Partial && t.Depth == depth))
				{
					c.Add(t);
				}
				else if(i == Intersection.Partial)
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
			var i0 = t.V0.Dot(h.Normal) > h.Distance;
			var i1 = t.V1.Dot(h.Normal) > h.Distance;
			var i2 = t.V2.Dot(h.Normal) > h.Distance;
			// positive halfspace - section 4.1
			if(h.Distance >= 0)
			{
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
			else // negative halfspace - simplification of section 4.3 (ii)
			{
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
			var v = t.V1.Subtract(t.V0).Cross(t.V2.Subtract(t.V1));
			v = v.DivideBy(v.Magnitude());
			// TODO does only normalizing to depth 7 significantly affect d?
			var d = t.V0.Dot(v);
			var b = Halfspace.FromDistance(v, d);
			// if bounding circle overlaps halfspace
			if(h.Normal.Angle(b.Normal) < h.Angle + b.Angle)
			{
				// TODO if edge intersects return true - section 4.2 / figure 9(d)
				// TODO if fully inside return true - formula 4.3 / figure 9(e)
			}
			return false;
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
