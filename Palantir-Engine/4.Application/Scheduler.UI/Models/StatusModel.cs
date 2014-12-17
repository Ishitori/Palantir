namespace Ix.Palantir.Scheduler.UI.Models
{
    public class StatusModel
    {
        public string ServiceStatusMessage
        {
            get
            {
                return this.IsSchedulerRunning
                    ? "���������� ������������� ��������� �������"
                    : "���������� ������������� ��������� ����������";
            }
        }
        public bool IsSchedulerRunning
        {
            get
            {
                return ScheduleRunner.Instance.IsSchedulerRunning;
            }
        }
        public bool IsSchedulerStopped
        {
            get
            {
                return !ScheduleRunner.Instance.IsSchedulerRunning;
            }
        }

        public void StartScheduler()
        {
            ScheduleRunner.StartNewScheduler();
        }
        public void StopScheduler()
        {
            ScheduleRunner.Instance.Stop();
        }
    }
}