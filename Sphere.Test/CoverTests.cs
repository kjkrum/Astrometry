using CodeConCarne.Astrometry;
using CodeConCarne.Astrometry.Sphere;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Sphere.Test
{
	[TestClass]
	public class CoverTests
	{
		[TestMethod]
		public void TestCapOnOriginalFace()
		{
			var v = new Vector(1, 1, 1);
			var a = Math.PI / 10;
			var scratch = new Cover.Scratch();
			var result = new List<Trixel>();
			Cover.Circle(v, a, 0, scratch, result);
			Assert.AreEqual(1, result.Count);
			var t = result.Single();
			Assert.AreEqual(0b1000, t.Id);
		}
	}
}
