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
			scratch.Clear();
			result.Clear();
			if (angle == 0) return;
			if (angle == Math.PI)
			{
				Octahedron.FullSphere(result);
				return;
			}
			var h = Halfspace.FromAngle(look, angle);
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
			// this use of Dot is one reason all Vectors are normalized
			// TODO normalize consistently in Cover and Mesh
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
			var v = t.V1.Subtract(t.V0).Cross(t.V2.Subtract(t.V1)).Normalize();
			var d = t.V0.Dot(v);
			var b = Halfspace.FromDistance(v, d);
			// if halfspace cap overlaps trixel bounding circle
			if(h.Normal.Angle(b.Normal) < h.Angle + b.Angle)
			{
				// if cap intersects trixel edge return true - section 4.2 / figure 9(d)
				// if cap is fully inside trixel return true - formula 4.3 / figure 9(e)

				// different approach...
				// if we're here, either the cap is positive and none of the
				// vertices are within it, or the cap is negative and all of
				// the vertices are within it.
				if (h.Distance < 0)
				{
					h = h.Complement();
				}

				// trixel edges lie in great circles, so trixels can be
				// thought of as the intersection of three zero-distance
				// halfspaces. compute the normals of those halfspaces.
				// if the angles between all three halfspace normals and the
				// cap normal are less than or equal to half pi, the cap
				// normal points inside the trixel. if all three are less than
				// half pi plus the cap angle, the cap normal either points
				// inside the trixel or close enough outside it to overlap the
				// edge. (less than or equal would be touching but not
				// overlapping.)
				var a = Math.PI / 2 + h.Angle;
				return
					t.V0.Cross(t.V1).Normalize().Angle(h.Normal) < a &&
					t.V1.Cross(t.V2).Normalize().Angle(h.Normal) < a &&
					t.V2.Cross(t.V0).Normalize().Angle(h.Normal) < a;

				// equivalent without short circuiting
				//var n0 = t.V0.Cross(t.V1).Normalize();
				//var n1 = t.V1.Cross(t.V2).Normalize();
				//var n2 = t.V2.Cross(t.V0).Normalize();
				//var a0 = n0.Angle(h.Normal);
				//var a1 = n1.Angle(h.Normal);
				//var a2 = n2.Angle(h.Normal);
				//var e0 = a0 < Math.PI / 2 + h.Angle;
				//var e1 = a1 < Math.PI / 2 + h.Angle;
				//var e2 = a2 < Math.PI / 2 + h.Angle;
				//return e0 && e1 && e2;
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
