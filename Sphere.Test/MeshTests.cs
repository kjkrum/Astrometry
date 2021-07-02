using CodeConCarne.Astrometry.Sphere;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Sphere.Test
{
	[TestClass]
	public class MeshTests
	{

		[TestMethod]
		public void Performance()
		{
			var n = 1000000;
			var ra = new double[n];
			var dec = new double[n];
			var rand = new Random();
			for (int i = 0; i < n; ++i)
			{
				ra[i] = rand.NextDouble() * 360;
				dec[i] = rand.NextDouble() * 180 - 90;
			}
			var start = DateTime.UtcNow;
			var scratch = Mesh.Scratch();
			for (int i = 0; i < n; ++i)
			{
				Coords.Cartesian(ra[i], dec[i], out double x, out double y, out double z);
				var id = Mesh.Id(x, y, z, 20, scratch);
			}
			var end = DateTime.UtcNow;
			var elapsed = end - start;
			Console.WriteLine(elapsed);
		}

		[TestMethod]
		public void AreaRatio()
		{
			var scratch = Mesh.Scratch();
			var corner = Mesh.Trixel(0, 0, 1, 20, scratch);
			Assert.AreEqual(8796093022208L, corner.Id); // "10000000000000000000000000000000000000000000"
			var middle = Mesh.Trixel(1, 1, 1, 20, scratch);
			Assert.AreEqual(9895604649983L, middle.Id); // "10001111111111111111111111111111111111111111"
			var ratio = middle.Area() / corner.Area();
			Assert.AreEqual(2.1, Math.Round(ratio, 1));
		}
	}
}
