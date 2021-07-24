using System;

namespace CodeConCarne.Astrometry
{
	public readonly struct Vector
	{
		readonly public double X;
		readonly public double Y;
		readonly public double Z;

		public Vector(double x, double y, double z)
		{
			X = x;
			Y = y;
			Z = z;
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

		internal double Distance(Vector other)
		{
			var x = X - other.X;
			var y = Y - other.Y;
			var z = Z - other.Z;
			return Math.Sqrt(x * x + y * y + z * z);
		}

		internal double Magnitude()
		{
			return Math.Sqrt(X * X + Y * Y + Z * Z);
		}

		internal double Dot(Vector other)
		{
			return X * other.X + Y * other.Y + Z * other.Z;
		}

		internal Vector Cross(Vector other)
		{
			var x = Y * other.Z - Z * other.Y;
			var y = Z * other.X - X * other.Z;
			var z = X * other.Y - Y * other.X;
			return new Vector(x, y, z);
		}

		internal Vector Subtract(Vector other)
		{
			return new Vector(X - other.X, Y - other.Y, Z - other.Z);
		}

		internal Vector DivideBy(double value)
		{
			return new Vector(X / value, Y / value, Z / value);
		}

		public double Angle(Vector other)
		{
			return Math.Acos(Dot(other) / (Magnitude() * other.Magnitude()));
		}
	}
}
