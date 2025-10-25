namespace ILAccess.Tests.InvalidAssemblyToProcess;

public abstract class InvalidAssemblyToProcessReference
{
#if DEBUG
    public static bool IsDebug { get; } = true;
#else
        public static bool IsDebug { get; } = false;
#endif
}