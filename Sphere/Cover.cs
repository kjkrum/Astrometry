using Dawn;
using System;
using System.Collections.Generic;

namespace CodeConCarne.Astrometry.Sphere
{
	public static class Cover
	{
		// Trixel is an object because this class stores it in collections.
		// if we want to continue down the performance rabbit hole, consider
		// changing it back to a struct and marshalling to an array.
		// hmm... maybe create some collections of T where T is struct...

		//public static void Rectangle(Vector look, Vector up, double ax, double ay, int depth) {}

		public static void Circle(Vector look, double angle, int depth, Scratch scratch)
		{
			Guard.Argument(angle, nameof(angle)).InRange(0, 360);
			Guard.Argument(depth, nameof(depth)).InRange(Mesh.MIN_DEPTH, Mesh.MAX_DEPTH);
			var a = angle / 180 * Math.PI;
			double distance = angle <= Math.PI ? Math.Cos(a / 2) : -Math.Cos((2 * Math.PI - a) / 2);
			var h = new Halfspace(look, distance);
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
			// TODO do something with cover
			scratch.Clear();
		}

		private static Intersection Intersect(Trixel t, Halfspace h)
		{
			// TODO double check these decision trees; add references to sections in paper
			var i0 = t.V0.Dot(h.Normal) > h.Distance;
			var i1 = t.V1.Dot(h.Normal) > h.Distance;
			var i2 = t.V2.Dot(h.Normal) > h.Distance;
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
				// TODO check for center intersection - return partial
			}
			else // negative halfspace
			{
				if (i0 && i1 && i2)
				{
					// TODO check for center intersection - return partial
					return Intersection.Full;
				}
				if (i0 || i1 || i2)
				{
					return Intersection.Partial;
				}
			}
			return Intersection.None;
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
