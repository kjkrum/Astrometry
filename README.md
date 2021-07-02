# Astrometry

Just messing around with some astrometry algorithms.

## Sphere

Based on [Indexing the Sphere with the Hierarchical Triangular Mesh](https://arxiv.org/ftp/cs/papers/0701/0701164.pdf) by Alexander Szalay et al, Microsoft Research Advanced Technology Division, 2005, and the reference implementation found at [SkyServer](http://www.skyserver.org/htm/). The reference implementation is not entirely open source. There are some DLLs in the `required` folder of the distribution archive that I haven't been able to find the source for.

The goal of this project is a clean, modern, fully open source implementation of the HTM. Performance is comparable to the reference implementation. The `master` branch is slightly faster and the `compat` branch is slightly slower.

The `compat` branch is an experimental attempt to make this version produce the same trixel IDs as the reference implementation. Key differences include the algorithm for identifying the initial face of the octahedron, the order (and therefore names) of the faces, the order in which the children of a trixel are tested, and the depth to which midpoints are normalized.