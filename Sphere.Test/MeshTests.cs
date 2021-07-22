using CodeConCarne.Astrometry.Sphere;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Sphere.Test
{
	[TestClass]
	public class MeshTests
	{
		/// <summary>
		/// Ad hoc performance test.
		/// </summary>
		[TestMethod]
		public void Performance()
		{
			var n = 1_000_000;
			var ax = new double[n];
			var ay = new double[n];
			var az = new double[n];
			var rand = new Random();
			for(int i = 0; i < n; ++i)
			{
				var ra = rand.NextDouble() * 360;
				var dec = rand.NextDouble() * 180 - 90;
				Coords.Cartesian(ra, dec, out var x, out var y, out var z);
				ax[i] = x;
				ay[i] = y;
				az[i] = z;
			}
			var scratch = new Mesh.Scratch();
			var start = DateTime.UtcNow;
			for (int i = 0; i < n; ++i)
			{
				_ = Mesh.Id(ax[i], ay[i], az[i], 20, scratch);
			}
			var end = DateTime.UtcNow;
			var elapsed = end - start;
			Console.WriteLine(elapsed);
		}

		/// <summary>
		/// At depth, the ratio of center to corner trixel areas is
		/// approximately as documented in the paper.
		/// </summary>
		[TestMethod]
		public void AreaRatio()
		{
			var scratch = new Mesh.Scratch();
			var corner = Mesh.Trixel(0, 0, 1, 20, scratch);
			Assert.AreEqual(0b10000000000000000000000000000000000000000000L, corner.Id);
			var middle = Mesh.Trixel(1, 1, 1, 20, scratch);
			Assert.AreEqual(0b10001111111111111111111111111111111111111111L, middle.Id);
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
			var d = 20;
			var rand = new Random();
			var scratch = new Mesh.Scratch();
			var count = new int[4];
			for(int i = 0; i < n; ++i)
			{
				Coords.Random(rand, out var x, out var y, out var z);
				var id = Mesh.Id(x, y, z, d, scratch);
				for(int j = 0; j < d; ++j)
				{
					var v = id & 0b11;
					count[v]++;
					id >>= 2;
				}
			}
			var sum = (double) count[0] + count[1] + count[2];
			var mean = sum / 3;
			// unsure why corner 0 tends to be slightly less frequent
			for(int i = 0; i < 3; ++i)
			{
				var corner = count[i] / mean;
				Assert.IsTrue(corner > 0.997);
				Assert.IsTrue(corner < 1.003);
			}
			var center = count[3] / sum;
			Assert.IsTrue(center > 0.344);
			Assert.IsTrue(center < 0.346); // larger at shallower depths
		}
	}
}
