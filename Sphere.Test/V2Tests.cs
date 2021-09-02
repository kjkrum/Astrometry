using CodeConCarne.Astrometry.Sphere.V2;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Sphere.Test
{
	[TestClass]
	public class V2Tests
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

		[TestMethod]
		public void Performance()
		{
			// with aggressive inlining in this version (same as array version)
			// and array version normalizing at all depths (same as this version)
			// performance is identical; consistently about 2.16 seconds on i7-3770
			var n = 1_000_000;
			RandomPoints(n, out double[] ax, out double[] ay, out double[] az);
			var start = DateTime.UtcNow;
			for (int i = 0; i < n; ++i)
			{
				_ = Mesh.Trixel(ax[i], ay[i], az[i], Mesh.MAX_DEPTH);
			}
			var end = DateTime.UtcNow;
			var elapsed = end - start;
			Console.WriteLine(elapsed);
		}

		[TestMethod]
		public void AreaRatio()
		{
			var corner = Mesh.Trixel(0, 0, 1, 20);
			Assert.AreEqual(0b10000000000000000000000000000000000000000000L, corner.Id);
			var middle = Mesh.Trixel(1, 1, 1, 20);
			Assert.AreEqual(0b10001111111111111111111111111111111111111111L, middle.Id);
			var ratio = middle.Area() / corner.Area();
			Assert.AreEqual(2.1, Math.Round(ratio, 1));
		}

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
			for(var i = 0; i < 3; ++i)
			{
				Assert.IsTrue(count[i] > 4_900_000 && count[i] < 5_000_000);
			}
			Assert.IsTrue(count[3] > 5_100_000 && count[3] < 5_200_000);
		}
	}
}
