# Astrometry

Just messing around with some astrometry algorithms.

## Sphere

Based on [Indexing the Sphere with the Hierarchical Triangular Mesh](https://arxiv.org/ftp/cs/papers/0701/0701164.pdf) by Alexander Szalay et al, Microsoft Research Advanced Technology Division, 2005, and the reference implementation found at [SkyServer](http://www.skyserver.org/htm/). The reference implementation is not entirely open source. There are some DLLs in the `required` folder of the distribution archive that I haven't been able to find the source for.

The goal of this project is a clean, modern, open source, and somewhat simplified implementation of the HTM. The trixel IDs produced by this version differ from those produced by the reference implementation. Key differences include the algorithm for identifying the initial face of the octahedron, the order (and therefore names) of the faces, the order in which the children of a trixel are tested, and the depth to which midpoints are normalized. Indexing is significantly faster than the reference implementation. Computing covers is in development and has not been benchmarked or aggressively optimized.

The simplification of convexes described in sections 3.2 through 3.8 of the paper is unnecessary for the applications I have in mind. Instead of accepting arbitrarily complex convexes, I'm only planning to support two simple types: a single halfspace describing a circle, and four halfspaces describing the projection of a rectangle onto the sphere.

The `compat` branch is an experimental attempt to make this version produce the same trixel IDs as the reference implementation. Differences in the calculation of trixel IDs were isolated from differences in the conversion of RA/Dec to cartesian coordinates. At depth 20 there are approximately 160 different trixel IDs per million random RA/Dec coordinates. Approximately 120 of these differences occur at depth 20 (that is, in the least significant 2 bits of the tixel ID) with approximately 1/4 as many differences occurring at each shallower depth. The remaining differences may be due to things like the choice of ray-triangle intersection algorithm, epsilon values, and the exact order of operations. To run these tests yourself, you'll need to download the reference implementation and update the paths of the DLL references in the `Compat.Test` project.