using Dawn;
using System;

namespace CodeConCarne.Astrometry.Sphere
{
	public static class Coords
	{
		// https://bridge.math.oregonstate.edu/papers/spherical.pdf

		/// <summary>
		/// Converts right ascension and declination to Cartesian coordinates.
		/// </summary>
		/// <param name="ra">right ascension in degrees, 0 inclusive to 360 exclusive</param>
		/// <param name="dec">declination in degrees, -90 inclusive to 90 inclusive</param>
		/// <param name="x">the x coordinate</param>
		/// <param name="y">the y coordinate</param>
		/// <param name="z">the z coordinate</param>
		public static void Cartesian(double ra, double dec, out double x, out double y, out double z)
		{
			Guard.Argument(ra, nameof(ra)).Min(0).LessThan(360);
			Guard.Argument(dec, nameof(dec)).InRange(-90, 90);
			var phi = ra / 180 * Math.PI;
			var theta = (90 - dec) / 180 * Math.PI;
			x = Math.Sin(theta) * Math.Cos(phi);
			y = Math.Sin(theta) * Math.Sin(phi);
			z = Math.Cos(theta);
		}

		/// <summary>
		/// Produces random Cartesian coordinates that would be uniformly
		/// distributed if projected onto the surface of a sphere.
		/// </summary>
		/// <param name="rand">the PRNG to use</param>
		/// <param name="x">the x coordinate</param>
		/// <param name="y">the y coordinate</param>
		/// <param name="z">the z coordinate</param>
		public static void Random(Random rand, out double x, out double y, out double z)
		{
			// http://corysimon.github.io/articles/uniformdistn-on-sphere
			var phi = 2 * Math.PI * rand.NextDouble();
			var theta = Math.Acos(1 - 2 * rand.NextDouble());
			x = Math.Sin(theta) * Math.Cos(phi);
			y = Math.Sin(theta) * Math.Sin(phi);
			z = Math.Cos(theta);
		}
	}
}
