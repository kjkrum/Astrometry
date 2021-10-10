# Astrometry

Various projects related to astrometry. If anyone is using this code, let me know and I'll start paying more attention to formal versioning.

## Sphere

![visualization of a round cover](example.png)

Based on [Indexing the Sphere with the Hierarchical Triangular Mesh](https://arxiv.org/ftp/cs/papers/0701/0701164.pdf) by Alexander Szalay et al, Microsoft Research Advanced Technology Division, 2005, and the reference implementation found at [SkyServer](http://www.skyserver.org/htm/). The goal of this project is a clean, easy to use, and somewhat simplified implementation of the HTM. It's entirely written in C# and targets .NET Standard 2.0 for compatibility with platforms like Xamarin and Unity3D.

### Compatibility

Trixel IDs are mostly compatible with the reference implementation. Isolating differences in indexing from differences in converting polar to cartesian coordinates, at depth 20 there are approximately 160 different trixel IDs per million random coordinates. About 120 of these differences occur at depth 20 (that is, in the least significant 2 bits of the trixel ID) with approximately 1/4 as many differences at each shallower depth. These differences may be due to things like the choice of ray-triangle intersection algorithm, epsilon values, and the exact order of operations.

Instead of simplifying arbitrarily complex convexes as described in sections 3.2 through 3.8 of the paper, I only plan to support circular and rectangular covers. I think this will support the vast majority of use cases. 

### Performance

Indexing is significantly faster on .NET Core than .NET Framework. (The reference implementation directly targets .NET Framework.) Performance was tested by indexing one million random coordinates. These results were obtained on my \[ct\]rusty i7-3770:

|Library|Platform|Time|
|---|---|---|
|Sphere (this lib)|.NET Core 3.1|2.2 sec|
|Spherical (reference)|.NET Standard 4.7.2|2.4 sec|
|Sphere (this lib)|.NET Standard 4.7.2|3.3 sec|

Computing covers is still in development and has not been aggressively optimized or benchmarked.