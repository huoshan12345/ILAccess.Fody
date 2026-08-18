using Xunit.Sdk;
using Xunit.v3;

// need to disable parallelization for the entire assembly because some tests are not thread-safe like getting/setting data for static fields and properties
[assembly: Parallelization(Mode = ParallelMode.None)]