using System;
using System.Diagnostics;
using System.IO;
using System.Net.Sockets;
using System.Threading;
using SuperWebSocket;

namespace WebSocketTest
{
    class Program
    {
        private static WebSocketServer wsServer;
        private static int initialConnPort = 9090;
        private static int currConnPort = 9090;
        private static string hololensIP = "10.91.68.73";

        static void Main(string[] args)
        {
            Console.Write("Enter Hololens Ip -> ");
            hololensIP = Console.ReadLine();
            Console.WriteLine("Hololens IP ::: " + hololensIP);

            StartSharingService();
            Console.CancelKeyPress += Console_CancelKeyPress;

            wsServer = new WebSocketServer();
            int port = 8088;
            wsServer.Setup(port);
            wsServer.NewSessionConnected += WsServer_NewSessionConnected;
            wsServer.NewMessageReceived += WsServer_NewMessageReceived;
            wsServer.NewDataReceived += WsServer_NewDataReceived;
            wsServer.SessionClosed += WsServer_SessionClosed;
            wsServer.Start();
            Console.WriteLine("Server is running on port " + port + ". Press ENTER to exit....");
            Console.ReadKey();
        }

        private static void WsServer_SessionClosed(WebSocketSession session, SuperSocket.SocketBase.CloseReason value)
        {
            Console.WriteLine("Session Closed");
        }

        private static void WsServer_NewDataReceived(WebSocketSession session, byte[] value)
        {
            Console.WriteLine("New Data Received");
        }

        private static void WsServer_NewMessageReceived(WebSocketSession session, string value)
        {
            Console.WriteLine("New Message Received: " + value);

            string scene = null;
            string cmd = null;

            string[] str = value.Split(' ');
            if (str.Length == 2)
            {
                if (!(str[0].Equals(null)))
                {
                    scene = str[0];
                    Console.WriteLine("SCENE: " + scene);
                }

                if (!(str[1].Equals(null)))
                {
                    cmd = str[1];
                    Console.WriteLine("CMD: " + cmd);
                }
            }
            else {
                Console.WriteLine("Error: Check Input Size on Web Module" + cmd);
            }

            if (cmd.Equals("CONNN"))
            {
                Connect(hololensIP, initialConnPort, scene, cmd);

                switch (scene)
                {
                    case "SCN01":
                        currConnPort = initialConnPort + 1;
                        break;
                    case "SCN02":
                        currConnPort = initialConnPort + 2;
                        break;
                    case "SCN03":
                        currConnPort = initialConnPort + 3;
                        break;
                    case "SCN04":
                        currConnPort = initialConnPort + 4;
                        break;
                    case "SCN05":
                        currConnPort = initialConnPort + 5;
                        break;
                    case "SCN06":
                        currConnPort = initialConnPort + 6;
                        break;
                    case "SCN07":
                        currConnPort = initialConnPort + 7;
                        break;
                    case "SCN08":
                        currConnPort = initialConnPort + 8;
                        break;
                    case "SCN09":
                        currConnPort = initialConnPort + 9;
                        break;
                    case "SCN10":
                        currConnPort = initialConnPort + 10;
                        break;
                    case "RELAX":
                        currConnPort = initialConnPort + 11;
                        break;
                    default:
                        currConnPort = initialConnPort;
                        break;
                }
            }
            else if (cmd.Equals("PREVS"))
            {
                Console.WriteLine(currConnPort);
                Connect(hololensIP, currConnPort, scene, cmd);
                currConnPort = currConnPort - 1;
                Console.WriteLine(currConnPort);
            }
            else if (cmd.Equals("NEXXT"))
            {
                Console.WriteLine(currConnPort);
                Connect(hololensIP, currConnPort, scene, cmd);
                currConnPort = currConnPort + 1;
                Console.WriteLine(currConnPort);
            }
            else if (cmd.Equals("PLAYY"))
            {
                Console.WriteLine(currConnPort);
                Connect(hololensIP, currConnPort, scene, cmd);
            }
            else if (cmd.Equals("STOPP"))
            {
                Console.WriteLine(currConnPort);
                Connect(hololensIP, currConnPort, scene, cmd);
            }
            else if (cmd.Equals("PAUSE"))
            {
                Console.WriteLine(currConnPort);
                Connect(hololensIP, currConnPort, scene, cmd);
            }
            else
            {
                switch (scene)
                {
                    case "SCN01":
                        Connect(hololensIP, 9091, scene, cmd);
                        currConnPort = 9091;
                        break;
                    case "SCN02":
                        Connect(hololensIP, 9092, value, cmd);
                        currConnPort = 9092;
                        break;
                    case "SCN03":
                        Connect(hololensIP, 9093, value, cmd);
                        break;
                    case "SCN04":
                        Connect(hololensIP, 9094, value, cmd);
                        break;
                    case "SCN05":
                        Connect(hololensIP, 9095, value, cmd);
                        break;
                    case "SCN06":
                        Connect(hololensIP, 9096, value, cmd);
                        break;
                    case "SCN07":
                        Connect(hololensIP, 9097, value, cmd);
                        break;
                    case "SCN08":
                        Connect(hololensIP, 9098, value, cmd);
                        break;
                    case "SCN09":
                        Connect(hololensIP, 9099, value, cmd);
                        break;
                    case "SCN10":
                        Connect(hololensIP, 9100, value, cmd);
                        break;
                    case "RELAX":
                        Connect(hololensIP, 9101, value, cmd);
                        break;
                    default:
                        Connect(hololensIP, 9090, value, cmd);
                        break;
                }
            }
        }

        // Start Sharing Service
        private static void StartSharingService()
        {
            string workingDirectory = Environment.CurrentDirectory;
            string projectDirectory = Directory.GetParent(workingDirectory).Parent.FullName;
            Console.WriteLine("Starting Sharing Service");

            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = projectDirectory + "\\SharingService.exe";
            startInfo.Arguments = "-local";

            try
            {
                Process.Start(startInfo);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private static void WsServer_NewSessionConnected(WebSocketSession session)
        {
            Console.WriteLine("New Session Connected");
        }

        // TCP Connection Code Snippet
        static void Connect(String server, int serverPort, String message, String cmd)
        {
            try
            {
                // Create a TcpClient.
                // Note, for this client to work you need to have a TcpServer 
                // connected to the same address as specified by the server, port
                // combination.
                Int32 port = serverPort;
                TcpClient client = new TcpClient(server, port);

                // Translate the passed message into ASCII and store it as a Byte array.
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(message + " " + cmd);

                // Get a client stream for reading and writing.
                //  Stream stream = client.GetStream();

                NetworkStream stream = client.GetStream();

                // Send the message to the connected TcpServer. 
                stream.Write(data, 0, data.Length);

                Console.WriteLine("Sent: {0}", message + " " + cmd);

                // Receive the TcpServer.response.

                // Buffer to store the response bytes.
                data = new Byte[256];

                // String to store the response ASCII representation.
                String responseData = String.Empty;

                // Read the first batch of the TcpServer response bytes.
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                Console.WriteLine("Received: {0}", responseData);

                Thread.Sleep(2000);

                // Close everything.
                stream.Close();
                client.Close();
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine("ArgumentNullException: {0}", e);
            }
            catch (SocketException e)
            {
                Console.WriteLine("SocketException: {0}", e);
            }
        }

        // Cancel Key Press Event
        static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
        {
            EndTask("SharingService");
        }

        // End Process of an EXE
        static void EndTask(string taskname)
        {
            string processName = taskname.Replace(".exe", "");

            foreach (Process process in Process.GetProcessesByName(processName))
            {
                process.Kill();
            }
        }
    }
}
