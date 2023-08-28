namespace DeployServer;

public static class Utils
{
    public static string USER_NAME = "idanoren13";
    public static string PASSWORD = "s7Na6Q3c3MJtGGw";
    private static string IMAGE_VERSION = "3.1";
    public static string DOCKER_IMAGE_NAME = "idanoren13/gameroomserver:" + IMAGE_VERSION;

    public static int MIN_PORT = 30000;
    public static int MAX_PORT = 31000;
    public static int CONTAINER_DIDNOT_ACTIVATED = -100;
    public static int TARGE_TPORT = 80;
}