using NUnit.Framework;

[assembly: Parallelizable(ParallelScope.None)] // Enables class-level parallelism
[assembly: LevelOfParallelism(4)] // Number of threads (adjust as per your system)
