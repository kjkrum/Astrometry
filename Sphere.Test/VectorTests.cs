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
			var v0 = Vector.POS_X;
			var v1 = Vector.POS_Y;
			Assert.AreEqual(Math.PI / 2, v0.Angle(v1));
			var v2 = Vector.NEG_X;
			Assert.AreEqual(Math.PI, v0.Angle(v2));
			var v3 = new Vector(1, 1, 1);
			var v4 = new Vector(-1, -1, -1);
			Assert.AreEqual(Math.PI, v3.Angle(v4));
			Assert.AreEqual(0, v3.Angle(v3));
		}
	}
}
