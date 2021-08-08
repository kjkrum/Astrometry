using CodeConCarne.Astrometry.Sphere;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Sphere.Test
{
	[TestClass]
	public class VectorTests
	{
		[TestMethod]
		public void Angle()
		{
			var v0 = new Vector(1, 0, 0);
			var v1 = new Vector(0, 1, 0);
			var v2 = new Vector(-1, 0, 0);
			Assert.AreEqual(Math.PI / 2, v0.Angle(v1));
			Assert.AreEqual(Math.PI, v0.Angle(v2));
		}
	}
}
