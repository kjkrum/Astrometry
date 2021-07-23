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
	internal class Halfspace
	{
		internal readonly Vector Normal;
		internal readonly double Distance;

		internal Halfspace(Vector normal, double distance)
		{
			Normal = normal;
			Distance = distance;
		}
	}
}
