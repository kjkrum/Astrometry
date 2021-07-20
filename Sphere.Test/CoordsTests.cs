using CodeConCarne.Astrometry.Sphere;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Sphere.Test
{
	[TestClass]
	public class CoordsTests
	{
		// use Mesh.EPSILON if it's ever exposed
		private const double EPSILON = 1E-15;

		/// <summary>
		/// RA/Dec values map to the expected verticies of the octahedron.
		/// </summary>
		[TestMethod]
		public void OctahedronVertices()
		{
			Coords.Cartesian(0, 0, out double x, out double y, out double z);
			Assert.AreEqual(1, x, EPSILON);
			Assert.AreEqual(0, y, EPSILON);
			Assert.AreEqual(0, z, EPSILON); // != 0

			Coords.Cartesian(90, 0, out x, out y, out z);
			Assert.AreEqual(0, x, EPSILON); // != 0
			Assert.AreEqual(1, y, EPSILON);
			Assert.AreEqual(0, z, EPSILON); // != 0

			Coords.Cartesian(180, 0, out x, out y, out z);
			Assert.AreEqual(-1, x, EPSILON);
			Assert.AreEqual(0, y, EPSILON); // != 0
			Assert.AreEqual(0, z, EPSILON); // != 0

			Coords.Cartesian(270, 0, out x, out y, out z);
			Assert.AreEqual(0, x, EPSILON); // != 0
			Assert.AreEqual(-1, y, EPSILON);
			Assert.AreEqual(0, z, EPSILON); // != 0

			Coords.Cartesian(0, 90, out x, out y, out z);
			Assert.AreEqual(0, x, EPSILON);
			Assert.AreEqual(0, y, EPSILON);
			Assert.AreEqual(1, z, EPSILON);

			Coords.Cartesian(0, -90, out x, out y, out z);
			Assert.AreEqual(0, x, EPSILON); // != 0
			Assert.AreEqual(0, y, EPSILON);
			Assert.AreEqual(-1, z, EPSILON);
		}
	}
}
