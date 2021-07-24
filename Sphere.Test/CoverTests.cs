using CodeConCarne.Astrometry;
using CodeConCarne.Astrometry.Sphere;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Sphere.Test
{
	[TestClass]
	public class CoverTests
	{
		[TestMethod]
		public void TestCircle()
		{
			var v = new Vector(0, 0, 1);
			var a = Math.PI / 4;
			var scratch = new Cover.Scratch();
			var result = new List<Trixel>();
			Cover.Circle(v, a, 10, scratch, result);
		}
	}
}
