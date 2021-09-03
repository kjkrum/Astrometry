using System;
using System.Runtime.CompilerServices;

namespace CodeConCarne.Astrometry.Sphere
{
	public readonly struct Vector
	{
		public static readonly Vector POS_X = new Vector(1, 0, 0);
		public static readonly Vector POS_Y = new Vector(0, 1, 0);
		public static readonly Vector POS_Z = new Vector(0, 0, 1);
		public static readonly Vector NEG_X = new Vector(-1, 0, 0);
		public static readonly Vector NEG_Y = new Vector(0, -1, 0);
		public static readonly Vector NEG_Z = new Vector(0, 0, -1);

		public readonly double X;
		public readonly double Y;
		public readonly double Z;

		/// <summary>
		/// Not normalized!
		/// </summary>
		internal Vector(double x, double y, double z)
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

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal Vector Midpoint(Vector other)
		{
			var x = (X + other.X) / 2;
			var y = (Y + other.Y) / 2;
			var z = (Z + other.Z) / 2;
			return Normalize(x, y, z);
		}

		/// <summary>
		/// Not normalized!
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal Vector Subtract(Vector other)
		{
			return new Vector(X - other.X, Y - other.Y, Z - other.Z);
		}

		/// <summary>
		/// Not normalized!
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal Vector Cross(Vector other)
		{
			var x = Y * other.Z - Z * other.Y;
			var y = Z * other.X - X * other.Z;
			var z = X * other.Y - Y * other.X;
			return new Vector(x, y, z);
		}

		internal Vector Normalize()
		{
			return Normalize(X, Y, Z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal double Dot(Vector other)
		{
			return X * other.X + Y * other.Y + Z * other.Z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
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
