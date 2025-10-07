namespace ManageEmployee.Queues;

public interface IChartOfAccountCaculatorQueue
{
    void Perform(int year, string dbName);
}
