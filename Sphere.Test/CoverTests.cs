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
		public void TestCapFullyInside()
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

		[TestMethod]
		public void TestCapOverlappingEdge()
		{
			var v = new Vector(0, 1, 1);
			var a = Math.PI / 10;
			var scratch = new Cover.Scratch();
			var result = new List<Trixel>();
			Cover.Circle(v, a, 0, scratch, result);
			var r = result.ToArray();
			Assert.AreEqual(2, r.Length);
			Assert.AreEqual(0b1000, r[0].Id);
			Assert.AreEqual(0b1001, r[1].Id);
		}
	}
}
