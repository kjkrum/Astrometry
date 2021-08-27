﻿using CodeConCarne.Astrometry;
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
			var v = Vector.Normalize(1, 1, 1);
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
			// cap near the edge of face 0,
			// large enough to overlap face 4
			var v = Vector.Normalize(1, 1, 0.1);
			var a = Math.PI / 10;
			var scratch = new Cover.Scratch();
			var result = new List<Trixel>();
			Cover.Circle(v, a, 0, scratch, result);
			var r = result.ToArray();
			Assert.AreEqual(2, r.Length);
			Assert.AreEqual(0b1000, r[0].Id);
			Assert.AreEqual(0b1100, r[1].Id);
		}

		[TestMethod]
		public void TestNegativeCap()
		{
			var v = Vector.POS_X;
			var a = Math.PI * 0.6; // slightly more than half the sphere
			var scratch = new Cover.Scratch();
			var result = new List<Trixel>();
			// at depth 0, expect the 8 depth 0 faces
			Cover.Circle(v, a, 0, scratch, result);
			Assert.AreEqual(8, result.Count());
			Assert.AreEqual(8, result.Where(o => o.Depth == 0).Count());
			// at depth 1, expect 4 depth 0 faces and 12 depth 1 faces
			Cover.Circle(v, a, 1, scratch, result);
			Assert.AreEqual(16, result.Count());
			Assert.AreEqual(4, result.Where(o => o.Depth == 0).Count());
			Assert.AreEqual(12, result.Where(o => o.Depth == 1).Count());
		}

		[TestMethod]
		public void TestDeepCover()
		{
			// just checking that nothing blows up...
			var v = Vector.Normalize(1, 1, 1);
			var a = Math.PI / 10;
			var scratch = new Cover.Scratch();
			var result = new List<Trixel>();
			Cover.Circle(v, a, 20, scratch, result);
		}
	}
}