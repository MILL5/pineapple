# Pineapple
MILL5's core libraries for .NET

Pineapple is a set of core libraries that includes utilities, primitives, helper classes and more!  The inspiration for Pineapple comes from Google's Guava framework.

We plan to offer Pineapple in two flavors.

1) A NuGet library downloadable as a .NET Core library known as M5.Common [Available]
2) A Shared Class library of internal classes which can be embedded into your assemblies [Planned]

Initial Release

Cleanup       - A helper class that is meant to improve your error handling by safeguarding your Exception blocks or your Dispose and Finalizer methods.
Preconditions - A helper class for requirements that must be met when entering a method or property.
Check         - A helper class for checking IsAlive or IsRunning conditions (i.e. network, web, process, etc.)
ShortGuid     - A class that encapsulates a Guid into a shorter textual representation (i.e. 22 vs. 36 characters)

