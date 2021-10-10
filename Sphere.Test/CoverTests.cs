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
			var v = UnitVector.Direction(1, 1, 1);
			var a = Math.PI / 10;
			var h = Halfspace.FromAngle(v, a);
			var scratch = new Cover.Scratch();
			var result = new List<Trixel>();
			Cover.Circle(h, 0, scratch, result);
			Assert.AreEqual(1, result.Count);
			var t = result.Single();
			Assert.AreEqual(0b1111, t.Id);
		}

		[TestMethod]
		public void TestCapOverlappingEdge()
		{
			// cap near the edge of face 0,
			// large enough to overlap face 4
			var v = UnitVector.Direction(1, 1, 0.1);
			var a = Math.PI / 10;
			var h = Halfspace.FromAngle(v, a);
			var scratch = new Cover.Scratch();
			var result = new List<Trixel>();
			Cover.Circle(h, 0, scratch, result);
			var r = result.ToArray();
			Assert.AreEqual(2, r.Length);
			Assert.AreEqual(0b1111, r[0].Id);
			Assert.AreEqual(0b1000, r[1].Id);
		}

		[TestMethod]
		public void TestNegativeCap()
		{
			var v = UnitVector.POS_X;
			var a = Math.PI * 0.6; // slightly more than half the sphere
			var h = Halfspace.FromAngle(v, a);
			var scratch = new Cover.Scratch();
			var result = new List<Trixel>();
			// at depth 0, expect the 8 depth 0 faces
			Cover.Circle(h, 0, scratch, result);
			Assert.AreEqual(8, result.Count());
			Assert.AreEqual(8, result.Where(o => o.Depth == 0).Count());
			// at depth 1, expect 4 depth 0 faces and 12 depth 1 faces
			Cover.Circle(h, 1, scratch, result);
			Assert.AreEqual(16, result.Count());
			Assert.AreEqual(4, result.Where(o => o.Depth == 0).Count());
			Assert.AreEqual(12, result.Where(o => o.Depth == 1).Count());
		}

		[TestMethod]
		public void TestDeepCover()
		{
			// just checking that nothing blows up...
			var v = UnitVector.Direction(1, 1, 1);
			var a = Math.PI / 10;
			var h = Halfspace.FromAngle(v, a);
			var scratch = new Cover.Scratch();
			var result = new List<Trixel>();
			Cover.Circle(h, 20, scratch, result);
		}

		[TestMethod]
		public void TestIssue1Fixed()
		{
			// https://github.com/kjkrum/Astrometry/issues/1
			var v = UnitVector.POS_Z;
			var d = 0.4135585f;
			var h = Halfspace.FromDistance(v, d);
			// sanity check that this is the halfspace that showed the problem in the visualizer
			Assert.AreEqual(0.3642857f * Math.PI, h.Angle, 0.0001);
			var scratch = new Cover.Scratch();
			var result = new List<Trixel>();
			Cover.Circle(h, 3, scratch, result);
			// this was one of the trixels that was overshooting
			Assert.IsFalse(result.Any(o => o.Id == 0b1010010101));
		}
	}
}
