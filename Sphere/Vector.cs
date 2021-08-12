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

		// TODO normalize all vectors?
		// eliminating calls to Magnitude in Angle might be the most compelling
		// reason to normalize all Vectors including Trixel vertices at all depths.
		// this might give up a bit of indexing performance for faster computation
		// of covers, which is probably a good deal.

		internal Vector(double x, double y, double z, bool normalize)
		{
			if(normalize)
			{
				var d = Math.Sqrt(x * x + y * y + z * z);
				X = x / d;
				Y = y / d;
				Z = z / d;
			}
			else
			{
				X = x;
				Y = y;
				Z = z;
			}
		}

		public Vector(double x, double y, double z) : this(x, y, z, true) { }

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
			return new Vector(x, y, z, false);
		}

		internal Vector Subtract(Vector other)
		{
			return new Vector(X - other.X, Y - other.Y, Z - other.Z, false);
		}

		internal Vector DivideBy(double value)
		{
			return new Vector(X / value, Y / value, Z / value, false);
		}

		public double Angle(Vector other)
		{
			// when vectors are equal, dot can be like 1+2E16, making acos undefined
			// likewise, when vectors are opposite, dot can be like -(1+2E16)
			var d = Dot(other) / (Magnitude() * other.Magnitude());
			if (Math.Abs(d - 1) < Mesh.EPSILON) return 0;
			if (Math.Abs(d + 1) < Mesh.EPSILON) return Math.PI;
			return Math.Acos(d);
		}
	}
}
