using CodeConCarne.Astrometry.Sphere;
using System;
using System.Collections.Generic;

namespace CodeConCarne.Astrometry
{
	public readonly struct Trixel
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
			V0 = new Vector(scratch.Array, Mesh.V0);
			V1 = new Vector(scratch.Array, Mesh.V1);
			V2 = new Vector(scratch.Array, Mesh.V2);
		}

		public double Area()
		{
			var a = V0.Distance(V1);
			var b = V1.Distance(V2);
			var c = V2.Distance(V0);
			var s = (a + b + c) / 2;
			return Math.Sqrt(s * (s - a) * (s - b) * (s - c));
		}

		internal void EnqueueChildren(Queue<Trixel> queue)
		{
			var id = Id << 2;
			var depth = Depth + 1;
			var m0 = Midpoint(V1, V2);
			var m1 = Midpoint(V2, V0);
			var m2 = Midpoint(V0, V1);
			queue.Enqueue(new Trixel(id + 0, depth, V0, m2, m1));
			queue.Enqueue(new Trixel(id + 1, depth, V1, m0, m2));
			queue.Enqueue(new Trixel(id + 2, depth, V2, m1, m0));
			queue.Enqueue(new Trixel(id + 3, depth, m0, m1, m2));
		}

		private Vector Midpoint(Vector v0, Vector v1)
		{
			var m = Vector.Normalize((v0.X + v1.X) / 2, (v0.Y + v1.Y) / 2, (v0.Z + v1.Z) / 2);
			return m;
		}

		public override bool Equals(object obj)
		{
			return obj is Trixel other && Id == other.Id;
		}

		public override int GetHashCode()
		{
			return 2108858624 + Id.GetHashCode();
		}
	}
}
