namespace DeployServer;

public class ContainerData
{
    public int Port { get; init; }
    public string RoomCode { get; init; }
    public bool IsRunning { get; set; } = false;

    public string ContainerID { get; set; }
}