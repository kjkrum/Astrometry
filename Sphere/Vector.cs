using System.Runtime.CompilerServices;

namespace CodeConCarne.Astrometry.Sphere
{
	internal readonly struct Vector
	{
		public readonly double X;
		public readonly double Y;
		public readonly double Z;

		internal Vector(double x, double y, double z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal double Dot(Vector other)
		{
			return X * other.X + Y * other.Y + Z * other.Z;
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal Vector Cross(Vector other)
		{
			var x = Y * other.Z - Z * other.Y;
			var y = Z * other.X - X * other.Z;
			var z = X * other.Y - Y * other.X;
			return new Vector(x, y, z);
		}

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		internal UnitVector Normalize()
		{
			return UnitVector.Direction(X, Y, Z);
		}
	}
}
