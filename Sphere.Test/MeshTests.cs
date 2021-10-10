using CodeConCarne.Astrometry.Sphere;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;

namespace Sphere.Test
{
	[TestClass]
	public class MeshTests
	{
		private void RandomPoints(int n, out double[] ax, out double[] ay, out double[] az)
		{
			ax = new double[n];
			ay = new double[n];
			az = new double[n];
			var rand = new Random();
			for (int i = 0; i < n; ++i)
			{
				Coords.Random(rand, out double azimuthal, out double polar);
				Coords.SphericalToCartesian(azimuthal, polar, out double x, out double y, out double z);
				ax[i] = x;
				ay[i] = y;
				az[i] = z;
			}
		}

		/// <summary>
		/// Ad hoc performance test.
		/// </summary>
		[TestMethod]
		public void Performance()
		{
			var n = 1_000_000;
			RandomPoints(n, out double[] ax, out double[] ay, out double[] az);

			// performance on i7-3770
			// 2.2 seconds on Core 3.1
			// 3.3 seconds on Framework 4.7.2
			var start = DateTime.UtcNow;
			for (int i = 0; i < n; ++i)
			{
				_ = Mesh.Trixel(ax[i], ay[i], az[i], Mesh.MAX_DEPTH);
			}
			var end = DateTime.UtcNow;
			var elapsed = end - start;
			Console.WriteLine($"this lib: {elapsed}");

			// 2.4 seconds on Framework 4.7.2
			start = DateTime.UtcNow;
			for (int i = 0; i < n; ++i)
			{
				_ = Spherical.Htm.Trixel.CartesianToHid(ax[i], ay[i], az[i], Mesh.MAX_DEPTH);
			}
			end = DateTime.UtcNow;
			elapsed = end - start;
			Console.WriteLine($"reference lib: {elapsed}");
		}

		/// <summary>
		/// At depth, the ratio of center to corner trixel areas is
		/// approximately as documented in the paper.
		/// </summary>
		[TestMethod]
		public void AreaRatio()
		{
			var corner = Mesh.Trixel(0, 0, 1, 20);
			Assert.AreEqual(0b11110000000000000000000000000000000000000000L, corner.Id);
			var middle = Mesh.Trixel(1, 1, 1, 20);
			Assert.AreEqual(0b11111111111111111111111111111111111111111111L, middle.Id);
			var ratio = middle.Area() / corner.Area();
			Assert.AreEqual(2.1, Math.Round(ratio, 1));
		}

		/// <summary>
		/// At depth, trixel IDs are composed of approximately equal numbers
		/// of all three corners, and slightly more centers.
		/// </summary>
		[TestMethod]
		public void ChildDistribution()
		{
			var n = 1_000_000;
			RandomPoints(n, out double[] ax, out double[] ay, out double[] az);
			var depth = Mesh.MAX_DEPTH;
			var count = new int[4];
			for (int i = 0; i < n; ++i)
			{
				var t = Mesh.Trixel(ax[i], ay[i], az[i], depth);
				var id = t.Id;
				for (int d = 0; d < depth; ++d)
				{
					var j = id & 0b11;
					count[j]++;
					id >>= 2;
				}
			}
			// TODO determine why corner 0 is slightly less frequent
			for (var i = 0; i < 3; ++i)
			{
				Assert.IsTrue(count[i] > 4_900_000 && count[i] < 5_000_000);
			}
			Assert.IsTrue(count[3] > 5_100_000 && count[3] < 5_200_000);
		}

		/// <summary>
		/// Around 160 different trixel IDs per million random coordinates.
		/// </summary>
		[TestMethod]
		public void Compatibility()
		{
			var depth = Mesh.MAX_DEPTH;
			var n = 1_000_000;
			RandomPoints(n, out double[] ax, out double[] ay, out double[] az);
			var freq = new int[depth];
			for (int i = 0; i < n; ++i)
			{
				var actual = Mesh.Trixel(ax[i], ay[i], az[i], depth).Id;
				var expected = Spherical.Htm.Trixel.CartesianToHid(ax[i], ay[i], az[i], depth);
				if(actual != expected)
				{
					var d = Math.Abs(actual - expected);
					var j = depth - ((int) Math.Log(d, 2.0) / 2) - 1;
					++freq[j];
				}
			}
			var total = freq.Sum();
			Assert.IsTrue(total < n / 1_000_000 * 180, "Compatibility got worse.");
			Assert.IsTrue(total > n / 1_000_000 * 140, "Compatibility got better. Update the test.");
		}
	}
}
