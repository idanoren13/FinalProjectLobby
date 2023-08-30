using Docker.DotNet;
using Docker.DotNet.Models;
using Microsoft.VisualBasic;

namespace DeployServer;

public class DockerApi
{
    private DockerClient m_Client = new DockerClientConfiguration(
            new Uri("http://192.116.98.113:2375"))
        .CreateClient();

    public async Task<string> CreateAndStartContianer(int i_HostPort)
    {
        var containerParams = new CreateContainerParameters()
        {
            Image = Utils.DOCKER_IMAGE_NAME,
            HostConfig = new HostConfig()
            {
                PortBindings = new Dictionary<string, IList<PortBinding>>
                {
                    {$"{Utils.TARGET_PORT}/tcp", new List<PortBinding>() {new PortBinding(){HostPort = i_HostPort.ToString()} }}
                }
            }
        };


        var response = await m_Client.Containers.CreateContainerAsync(containerParams);

        await m_Client.Containers.StartContainerAsync(response.ID, new ContainerStartParameters());

        return response.ID;
    }
}