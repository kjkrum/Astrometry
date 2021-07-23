using CodeConCarne.Astrometry;
using CodeConCarne.Astrometry.Sphere;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sphere.Test
{
	[TestClass]
	public class CoverTests
	{
		[TestMethod]
		public void TestCircle()
		{
			var s = new Cover.Scratch();
			var v = new Vector(0, 0, 1);
			var a = 90.0;
			Cover.Circle(v, a, 10, s);
		}
	}
}
