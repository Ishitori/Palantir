namespace Ix.Palantir.Scheduler.Runner
{
    public interface IActionListener
    {
        object FireAction(ActionContext key);
    }
}