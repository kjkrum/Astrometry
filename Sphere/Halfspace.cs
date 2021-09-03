using Dawn;
using System;

namespace CodeConCarne.Astrometry.Sphere
{
	/// <summary>
	/// "The halfspace is our basic geometry primitive. Each halfspace defines
	/// a cap on the unit sphere that is inside the halfspace and so is sliced
	/// off by the plane that bounds the halfspace. Any halfspace can be
	/// characterized by the plane that bounds it and a direction from that
	/// plane. For our purposes, it is convenient to define the halfspace by
	/// the vector from the origin pointing into the halfspace and normal to
	/// the halfspace’s bounding plane, and the distance to the plane from the
	/// origin along the vector."
	/// </summary>
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
