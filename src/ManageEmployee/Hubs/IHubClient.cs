namespace ManageEmployee.Hubs;

public interface IHubClient
{
    Task BroadcastMessage();
}
