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

		// TODO need to ensure that look is normalized, if not by this method, then by Halfspace factory
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
			// this use of Dot is why all Vectors are normalized
			// TODO normalize consistently in Cover and Mesh
			// need to understand how much normalizing at depths > 7 changes vector magnitudes
			// and whether we need to use that difference as an epsilon value here
			var i0 = h.Normal.Dot(t.V0) > h.Distance;
			var i1 = h.Normal.Dot(t.V1) > h.Distance;
			var i2 = h.Normal.Dot(t.V2) > h.Distance;
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
			// if halfspace cap overlaps trixel bounding circle
			if(h.Normal.Angle(b.Normal) < h.Angle + b.Angle)
			{
				// if cap intersects trixel edge return true - section 4.2 / figure 9(d)
				// taking a different approach here...
				// given that the edge of a trixel is a segment of a great
				// circle, and a great circle is the intersection of a sphere
				// with a plane intersecting the center of the sphere, then a
				// cap overlaps the great circle if the smaller angle between
				// the halfspace's vector and the plane is less than the
				// halfspace's angle. (if the angles are equal, the cap
				// touches the trixel at one point but does not overlap.)
				// we don't care what the points of intersection are. we know
				// they must lie on the edge of the trixel because the cap
				// intersects the bounding circle of the trixel.

				// normals to the planes of the great circles. these point
				// into the three halfspaces whose intersection is equivalent to the trixel.
				// in other words, if the halfspace constructor were exposed, we could create these
				// halfspaces as new Halfspace(n0, Math.PI / 2, 0).
				var n0 = t.V0.Cross(t.V1);
				var n1 = t.V1.Cross(t.V2);
				var n2 = t.V2.Cross(t.V0);

				// TODO short circuit this condition
				var e0 = (Math.PI / 2) - (n0.Angle(h.Normal) % (Math.PI / 2)) < h.Angle;
				var e1 = (Math.PI / 2) - (n1.Angle(h.Normal) % (Math.PI / 2)) < h.Angle;
				var e2 = (Math.PI / 2) - (n2.Angle(h.Normal) % (Math.PI / 2)) < h.Angle;
				// FIXME TestCapOverlappingEdge is getting the right result for the wrong reason!
				// this should be true for face 0 in TestCapOverlappingEdge
				if (e0 || e1 || e2) return true;

				// if cap is fully inside trixel return true - formula 4.3 / figure 9(e)
				var d0 = n0.Dot(h.Normal);
				var d1 = n1.Dot(h.Normal);
				var d2 = n2.Dot(h.Normal);
				// this should NOT be true for face 0 in TestCapOverlappingEdge
				if (d0 >=0 && d1 >= 0 && d2 >= 0) return true;				
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
