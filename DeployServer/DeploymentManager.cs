
using System.Diagnostics;

namespace DeployServer
{
    public class DeploymentManager
    {
        //singleton components
        private static DeploymentManager s_Instance;
        private static readonly object sr_Lock = new object();


        private readonly Dictionary<int, ContainerData> m_Containers = new();
        private readonly Random m_Random = new Random();

        private DeploymentManager()
        {
        }

        public static DeploymentManager Instance
        {
            get
            {
                if (s_Instance == null)
                {
                    lock (sr_Lock)
                    {
                        s_Instance ??= new DeploymentManager();
                    }
                }

                return s_Instance;
            }
        }

        public int AddContainer(string i_ContainerName)
        {
            int port = m_Random.Next(Utils.MIN_PORT, Utils.MAX_PORT);
            while (m_Containers.ContainsKey(port))
            {
                port = m_Random.Next(Utils.MIN_PORT, Utils.MAX_PORT);
            }

            m_Containers.Add(port, new ContainerData() { Name = i_ContainerName, Port = port });
            activateContainer(m_Containers[port]);

            return m_Containers[port].IsRunning ? port : Utils.CONTAINER_DIDNOT_ACTIVATED;
        }

        private void activateContainer(ContainerData i_Container)
        {
            string runDocker = $"docker run -d -p {i_Container.Port}:{Utils.TARGE_TPORT} {Utils.DOCKER_IMAGE_NAME}";
            ProcessStartInfo processStartInfo = new ProcessStartInfo()
            {
                FileName = "docker",
                Arguments = runDocker,
                UseShellExecute = false,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                CreateNoWindow = true
            };


            // Create and start the process
            using (Process process = new Process())
            {
                process.StartInfo = processStartInfo;

                process.OutputDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            Console.WriteLine($"[DOCKER OUTPUT] {e.Data}");
                        }
                    };

                process.ErrorDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            Console.WriteLine($"[DOCKER ERROR] {e.Data}");
                        }
                    };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                i_Container.IsRunning = true;
            }

            i_Container.ContainerID = getContainerID();
        }

        private int getContainerID()
        {
            int containerID = -1;
            using (Process process = new Process())
            {
                process.StartInfo = new ProcessStartInfo()
                {
                    FileName = "docker",
                    Arguments = "ps -lq",
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = false
                };

                process.OutputDataReceived += (sender, e) =>
                    {
                        if (!string.IsNullOrEmpty(e.Data))
                        {
                            containerID = int.Parse(e.Data);
                        }
                    };
                process.Start();
                process.BeginOutputReadLine();

                process.WaitForExit();
            }

            return containerID;
        }

        public bool StopContainer(ContainerData i_Container)
        {
            bool stopContainer = false;
            string dockerCommand = $"docker stop {i_Container.ContainerID}";

            using (Process process = new Process())
            {
                process.StartInfo = new ProcessStartInfo()
                {
                    FileName = "docker",
                    Arguments = dockerCommand,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    CreateNoWindow = false
                };

                process.OutputDataReceived += (sender, e) =>
                    {
                        stopContainer = true;
                    };
                process.ErrorDataReceived += (sender, e) =>
                    {
                        Console.WriteLine(e.Data);
                        stopContainer = false;
                    };

                process.Start();
                process.BeginOutputReadLine();
                process.BeginErrorReadLine();
                
                process.WaitForExit();
            }

            return stopContainer;
        }
    }
}