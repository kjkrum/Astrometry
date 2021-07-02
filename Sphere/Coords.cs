using Dawn;
using System;

namespace CodeConCarne.Astrometry.Sphere
{
	public static class Coords
	{
		public static void Cartesian(double ra, double dec, out double x, out double y, out double z)
		{
			Guard.Argument(ra, nameof(ra)).Min(0).LessThan(360);
			Guard.Argument(dec, nameof(dec)).InRange(-90, 90);
			var phi = ra * Math.PI / 180;
			var theta = (90 - dec) * Math.PI / 180;
			x = Epsilon(Math.Sin(theta) * Math.Cos(phi));
			y = Epsilon(Math.Sin(theta) * Math.Sin(phi));
			z = Epsilon(Math.Cos(theta));
		}

		private static double Epsilon(double value)
		{
			return value > Mesh.EPSILON || value < -Mesh.EPSILON ? value : 0;
		}
	}
}
