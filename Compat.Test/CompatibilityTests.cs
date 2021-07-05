using CodeConCarne.Astrometry.Sphere;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Spherical;
using Spherical.Htm;
using System;

namespace Compat.Test
{
	[TestClass]
	public class CompatibilityTests
	{
		[TestMethod]
		public void CompareTrixelIds()
		{
			var n = 10_000_000;
			var depth = 20;
			var bits = 4 + depth * 2;
			var scratch = Mesh.Scratch();
			var rand = new Random();
			var totalDiffs = 0;
			var diffsByDepth = new int[depth + 1]; // index is depth of difference
			for (int i = 0; i < n; ++i)
			{
				var ra = rand.NextDouble() * 360;
				var dec = rand.NextDouble() * 180 - 90;
				Cartesian.Radec2Xyz(ra, dec, out double x, out double y, out double z);
				var id0 = Trixel.CartesianToHid(x, y, z, depth);
				var id1 = Mesh.Id(x, y, z, depth, scratch);
				if(id0 != id1)
				{
					var diff = id0 ^ id1;
					// number of bits of difference
					var bitsDiff = (int)Math.Floor(Math.Log(diff) / Math.Log(2)) + 1;
					// number of depths of difference
					var depthDiff = (bitsDiff + 1) / 2;
					// nominal depth of first difference
					var diffDepth = (depth - depthDiff) + 1;
					totalDiffs++;
					diffsByDepth[diffDepth]++;
					//var s0 = Convert.ToString(id0, 2);
					//var s1 = Convert.ToString(id1, 2);
				}
			}
			if(totalDiffs > 0)
			{
				; // breakpoint
			}			
		}
	}
}
