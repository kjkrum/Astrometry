using Dawn;
using System;

namespace CodeConCarne.Astrometry.Sphere.V2
{
	public readonly struct Halfspace
	{
		public readonly Vector Normal;
		public readonly double Angle;
		public readonly double Distance;

		private Halfspace(Vector normal, double angle, double distance)
		{
			Normal = normal;
			Angle = angle;
			Distance = distance;
		}

		internal Halfspace Complement()
		{
			return new Halfspace(Normal.Invert(), Math.PI - Angle, -Distance);
		}

		public static Halfspace FromAngle(Vector normal, double angle)
		{
			Guard.Argument(angle, nameof(angle)).InRange(0, Math.PI);
			var distance = angle <= Math.PI / 2 ?
				Math.Cos(angle) :
				-Math.Sin(angle - Math.PI / 2);
			return new Halfspace(normal, angle, distance);
		}

		public static Halfspace FromDistance(Vector normal, double distance)
		{
			Guard.Argument(distance, nameof(distance)).InRange(-1, 1);
			var angle = distance >= 0 ?
				Math.Acos(distance) :
				Math.Asin(distance) + Math.PI / 2;
			return new Halfspace(normal, angle, distance);
		}
	}
}
