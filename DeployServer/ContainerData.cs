namespace DeployServer;

public class ContainerData
{
    public int Port { get; init; }
    public string Name { get; init; }
    public bool IsRunning { get; set; } = false;

    public int ContainerID { get; set; } = -1;
}