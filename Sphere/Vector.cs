using System;

namespace CodeConCarne.Astrometry.Sphere
{
	public readonly struct Vector
	{
		readonly public static Vector POS_X = new Vector(1, 0, 0);
		readonly public static Vector POS_Y = new Vector(0, 1, 0);
		readonly public static Vector POS_Z = new Vector(0, 0, 1);
		readonly public static Vector NEG_X = new Vector(-1, 0, 0);
		readonly public static Vector NEG_Y = new Vector(0, -1, 0);
		readonly public static Vector NEG_Z = new Vector(0, 0, -1);

		readonly public double X;
		readonly public double Y;
		readonly public double Z;

		private Vector(double x, double y, double z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public static Vector Normalize(double x, double y, double z)
		{
			var d = Math.Sqrt(x * x + y * y + z * z);
			return new Vector(x / d, y / d, z / d);
		}

		internal Vector(double[] a, int i)
		{
			X = a[i + 0];
			Y = a[i + 1];
			Z = a[i + 2];
		}

		internal void CopyTo(double[] a, int i)
		{
			a[i + 0] = X;
			a[i + 1] = Y;
			a[i + 2] = Z;
		}

		internal Vector Invert()
		{
			return new Vector(-X, -Y, -Z);
		}

		internal double Distance(Vector other)
		{
			var x = X - other.X;
			var y = Y - other.Y;
			var z = Z - other.Z;
			return Math.Sqrt(x * x + y * y + z * z);
		}

		internal double Dot(Vector other)
		{
			return X * other.X + Y * other.Y + Z * other.Z;
		}


		/// <summary>
		/// Does not normalize. Do not expose.
		/// </summary>
		internal Vector Cross(Vector other)
		{
			var x = Y * other.Z - Z * other.Y;
			var y = Z * other.X - X * other.Z;
			var z = X * other.Y - Y * other.X;
			return new Vector(x, y, z);
		}

		/// <summary>
		/// Does not normalize. Do not expose.
		/// </summary>
		internal Vector Subtract(Vector other)
		{
			return new Vector(X - other.X, Y - other.Y, Z - other.Z);
		}

		/// <summary>
		/// Normalize vectors returned by <see cref="Cross(Vector)">Cross</see>
		/// and <see cref="Subtract(Vector)">Subtract</see>.
		/// </summary>
		internal Vector Normalize()
		{
			return Vector.Normalize(X, Y, Z);
		}

		public double Angle(Vector other)
		{
			// not needing to compute magnitudes here
			// is one reason all vectors are normalized.
			// when vectors are equal, dot can be like 1+2E16, making acos undefined
			// likewise, when vectors are opposite, dot can be like -(1+2E16)
			// TODO there are probably better ways to test for this
			var d = Dot(other);
			if (Math.Abs(d - 1) < Mesh.EPSILON) return 0;
			if (Math.Abs(d + 1) < Mesh.EPSILON) return Math.PI;
			return Math.Acos(d);
		}
	}
}
