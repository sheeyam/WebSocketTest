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
        private static string[] hololensIP;

        static void Main(string[] args)
        {
            Console.Write("**** Welcome to S P A R T - Central Control Module ****");

            StartSharingService();
            Console.CancelKeyPress += Console_CancelKeyPress;

            Console.Write("No of Hololens to Connect -> ");
            int noOfDevices = Convert.ToInt32(Console.ReadLine());

            if (noOfDevices > 0)
            {
                hololensIP = new string[noOfDevices];

                for (int i = 0; i < noOfDevices; i++)
                {
                    Console.Write("Enter Hololens Ip -> ");
                    hololensIP[i] = Console.ReadLine();
                    Console.WriteLine("Hololens IP ::: " + hololensIP[i]);
                }

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
            else {
                // No Devices
            } 
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

            /*
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
            */
            //Connect(hololensIP, initialConnPort, value);
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

        static void Connect(String[] holoIP, int serverPort, String cmd)
        {
            int n = holoIP.Length;
            for (int i = 0; i < n; i++) {
                Console.WriteLine(holoIP[i]);
                Console.WriteLine("Connecting to Hololens - " + holoIP[i]) ;
                TcpConnect(holoIP[i], serverPort, cmd);
                Thread.Sleep(1000);
            }
        }
        
        // TCP Connection Code Snippet
        static void TcpConnect(String server, int serverPort, String cmd)
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
                Byte[] data = System.Text.Encoding.ASCII.GetBytes(cmd);

                // Get a client stream for reading and writing.
                //  Stream stream = client.GetStream();

                NetworkStream stream = client.GetStream();

                // Send the message to the connected TcpServer. 
                stream.Write(data, 0, data.Length);

                Console.WriteLine("Sent: {0}", cmd);

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
