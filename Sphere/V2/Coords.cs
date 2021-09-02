using Dawn;
using System;

namespace CodeConCarne.Astrometry.Sphere.V2
{
	public static class Coords
	{
		/// <summary>
		/// Converts right ascension and declination in degrees to azimuthal
		/// and polar angles in radians.
		/// </summary>
		/// <param name="ra">right ascension in degrees, 0 inclusive to 360 exclusive</param>
		/// <param name="dec">declination in degrees, -90 inclusive to 90 inclusive</param>
		/// <param name="azimuthal">the azimuthal angle in radians</param>
		/// <param name="polar">the polar angle in radians</param>
		public static void AstroToSpherical(double ra, double dec, out double azimuthal, out double polar)
		{
			Guard.Argument(ra, nameof(ra)).Min(0).LessThan(360);
			Guard.Argument(dec, nameof(dec)).InRange(-90, 90);
			azimuthal = ra / 180 * Math.PI;
			polar = (90 - dec) / 180 * Math.PI;
		}

		/// <summary>
		/// Uses a right-handed coordinate system with polar angle 0 in the
		/// positive Z axis and azimuthal angle 0 in the positive X axis and
		/// increasing toward the positive Y axis. This is the most common
		/// coordinate system in mathematical illustrations of spherical
		/// coordinates. The parameter order also follows the mathematical
		/// convention of expressing azimuthal angle before polar angle.
		/// </summary>
		/// <param name="azimuthal">the azimuthal angle in radians, 0 inclusive to 2π exclusive</param>
		/// <param name="polar">the polar angle in radians, 0 inclusive to π inclusive</param>
		/// <param name="x">the x coordinate</param>
		/// <param name="y">the y coordinate</param>
		/// <param name="z">the z coordinate</param>
		public static void SphericalToCartesian(double azimuthal, double polar, out double x, out double y, out double z)
		{
			Guard.Argument(azimuthal, nameof(azimuthal)).Min(0).LessThan(2 * Math.PI);
			Guard.Argument(polar, nameof(polar)).InRange(0, Math.PI);
			x = Math.Sin(polar) * Math.Cos(azimuthal);
			y = Math.Sin(polar) * Math.Sin(azimuthal);
			z = Math.Cos(polar);
		}

		/// <summary>
		/// Produces uniformly distributed random spherical coordinates.
		/// </summary>
		/// <param name="rand">the PRNG to use</param>
		/// <param name="azimuthal">the azimuthal coordinate</param>
		/// <param name="polar">the polar coordinate</param>
		public static void Random(Random rand, out double azimuthal, out double polar)
		{
			// http://corysimon.github.io/articles/uniformdistn-on-sphere
			azimuthal = 2 * Math.PI * rand.NextDouble();
			polar = Math.Acos(1 - 2 * rand.NextDouble());
		}
	}
}
