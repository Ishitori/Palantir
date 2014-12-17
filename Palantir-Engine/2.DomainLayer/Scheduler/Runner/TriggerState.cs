namespace Ix.Palantir.Scheduler.Runner
{
    public enum TriggerState
    {
        Uninitialized = 0, 
        Initialized = 1, 
        Expired = 2, 
        Error = 3
    }
}