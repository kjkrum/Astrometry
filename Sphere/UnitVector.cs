using System;
using System.Runtime.CompilerServices;

namespace CodeConCarne.Astrometry.Sphere
{
	public readonly struct UnitVector
	{
		public static readonly UnitVector POS_X = new UnitVector(1, 0, 0);
		public static readonly UnitVector POS_Y = new UnitVector(0, 1, 0);
		public static readonly UnitVector POS_Z = new UnitVector(0, 0, 1);
		public static readonly UnitVector NEG_X = new UnitVector(-1, 0, 0);
		public static readonly UnitVector NEG_Y = new UnitVector(0, -1, 0);
		public static readonly UnitVector NEG_Z = new UnitVector(0, 0, -1);

		public readonly double X;
		public readonly double Y;
		public readonly double Z;

		private UnitVector(double x, double y, double z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public static UnitVector Direction(double x, double y, double z)
		{
			var d = Math.Sqrt(x * x + y * y + z * z);
			return new UnitVector(x / d, y / d, z / d);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal UnitVector Invert()
		{
			return new UnitVector(-X, -Y, -Z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal UnitVector Midpoint(UnitVector other)
		{
			var x = (X + other.X) / 2;
			var y = (Y + other.Y) / 2;
			var z = (Z + other.Z) / 2;
			return Direction(x, y, z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal double Dot(UnitVector other)
		{
			return X * other.X + Y * other.Y + Z * other.Z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal double Dot(Vector other)
		{
			return X * other.X + Y * other.Y + Z * other.Z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public double Angle(UnitVector other)
		{
			// when vectors are equal, dot can be like 1+2E16
			// when vectors are opposite, dot can be like -(1+2E16)
			var d = Dot(other);
			if (d > 1) return 0;
			if (d < -1) return Math.PI;
			return Math.Acos(d);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal Vector Subtract(UnitVector other)
		{
			return new Vector(X - other.X, Y - other.Y, Z - other.Z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal Vector Cross(UnitVector other)
		{
			var x = Y * other.Z - Z * other.Y;
			var y = Z * other.X - X * other.Z;
			var z = X * other.Y - Y * other.X;
			return new Vector(x, y, z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal Vector Cross(Vector other)
		{
			var x = Y * other.Z - Z * other.Y;
			var y = Z * other.X - X * other.Z;
			var z = X * other.Y - Y * other.X;
			return new Vector(x, y, z);
		}

		internal double Distance(UnitVector other)
		{
			var x = X - other.X;
			var y = Y - other.Y;
			var z = Z - other.Z;
			return Math.Sqrt(x * x + y * y + z * z);
		}
	}
}
