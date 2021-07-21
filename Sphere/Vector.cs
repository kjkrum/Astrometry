using System;

namespace CodeConCarne.Astrometry
{
	public readonly struct Vector
	{
		readonly public double X;
		readonly public double Y;
		readonly public double Z;

		internal Vector(double x, double y, double z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public double Distance(Vector other)
		{
			var x = X - other.X;
			var y = Y - other.Y;
			var z = Z - other.Z;
			return Math.Sqrt(x * x + y * y + z * z);
		}
	}
}
