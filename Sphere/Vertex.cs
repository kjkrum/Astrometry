using System;

namespace CodeConCarne.Astrometry
{
	public struct Vertex
	{
		readonly public double X;
		readonly public double Y;
		readonly public double Z;

		internal Vertex(double x, double y, double z)
		{
			X = x;
			Y = y;
			Z = z;
		}

		public double Distance(Vertex other)
		{
			var x = X - other.X;
			var y = Y - other.Y;
			var z = Z - other.Z;
			return Math.Sqrt(x * x + y * y + z * z);
		}
	}
}
