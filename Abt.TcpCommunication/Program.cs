using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SimpleTCP;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Abt.TcpCommunication
{
    class Program
    {
        SimpleTcpServer server;
        static void Main(string[] args)
        {
            try
            {

                Console.WriteLine("Hello World!");
                //Program prg = new Program();
                //prg.ExecuteServer();
                //prg.StartServer();
                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json");
                IConfiguration config = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", true, true)
                    .Build();
                var ip = config["ipconfig"];
                var port = config["port"];
                //ExecuteServer(ip, port);
                ExecuteServer();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
        }
        public static void ExecuteServer()
        {
            try
            {
                // Establish the local endpoint  
                // for the socket. Dns.GetHostName 
                // returns the name of the host  
                // running the application. 
                //IPHostEntry ipHost = Dns.GetHostEntry(Dns.GetHostName());

                IPAddress ipAddr = IPAddress.Parse("127.0.0.1");
                IPEndPoint localEndPoint = new IPEndPoint(IPAddress.Any, 31002);

                // Creation TCP/IP Socket using  
                // Socket Class Costructor 
                Socket listener = new Socket(ipAddr.AddressFamily,
                             SocketType.Stream, ProtocolType.Tcp);

                try
                {

                    // Using Bind() method we associate a 
                    // network address to the Server Socket 
                    // All client that will connect to this  
                    // Server Socket must know this network 
                    // Address 
                    listener.Bind(localEndPoint);

                    // Using Listen() method we create  
                    // the Client list that will want 
                    // to connect to Server 
                    listener.Listen(1000);


                    while (true)
                    {
                        Stopwatch timer = new Stopwatch();
                        timer.Start();
                        Console.WriteLine("System Date time in UTC "+System.DateTime.UtcNow);
                        long startedtime = timer.ElapsedMilliseconds;
                        Console.WriteLine("Waiting for new connection : "+startedtime);
                        // Suspend while waiting for 
                        // incoming connection Using  
                        // Accept() method the server  
                        // will accept connection of client 
                        Socket clientSocket = listener.Accept();
                        long connectedtime = timer.ElapsedMilliseconds;
                        Console.WriteLine("System Date time in UTC " + System.DateTime.UtcNow);
                        Console.WriteLine("Client Connected : " + connectedtime +": time taken : "+(connectedtime-startedtime));
                        clientSocket.ReceiveTimeout = 0;
                        clientSocket.SendTimeout = 0;

                        var clientThread = new System.Threading.Thread(processClient);
                        clientThread.Start(clientSocket);
                        //if(!clientThread.IsAlive)
                        //    clientThread.Abort();
                    }
                }

                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }
            }
            catch (Exception ex)
            {

                Console.WriteLine(ex.ToString());
            }
        }


        public static void processClient(object clientSocketObj )
        {
            try
            {
                var clientSocket = (Socket)clientSocketObj;

                // Data buffer 
                byte[] bytes = new Byte[1084];
                string data = "test";

                while (true)
                {

                    int numByte = clientSocket.Receive(bytes);

                    if (numByte > 0)
                    {
                        data += Encoding.ASCII.GetString(bytes,
                                               0, numByte);

                        if (data.IndexOf("<EOF>") > -1)
                            break;
                    }
                    else
                    {
                        //client has closed the connection
                        break;
                    }
                }
                if (data.Length > 0)
                {
                    Console.WriteLine("\nText received -> {0} ", data);
                }
                string content = File.ReadAllText(@"Sample.json");

                // read JSON directly from a file

                byte[] message = Encoding.ASCII.GetBytes(content);

                // Send a message to Client  
                // using Send() method 
                clientSocket.Send(message);
                clientSocket.ReceiveTimeout = 0;
                clientSocket.SendTimeout = 0;
                //clientSocket.ReceiveBufferSize = 0;
                // Close client Socket using the 
                // Close() method. After closing, 
                // we can use the closed Socket  
                // for a new Client Connection 
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }catch(Exception ex)
            {

                Console.WriteLine(ex.ToString());
            }
        }

        //public void ExecuteServer()
        //{
        //    server = new SimpleTcpServer();
        //    server.Delimiter = 0 * 13;
        //    server.StringEncoder = Encoding.UTF8;
        //    server.DataReceived += Server_DataReceived;
        //}

        //private void Server_DataReceived(object sender, Message e)
        //{
        //    var message = e.MessageString;
        //    e.ReplyLine(string.Format("Test said: {0}", message));
        //}
        //public void StartServer()
        //{
        //    var message = "Server Starting";
        //    System.Net.IPAddress ip = System.Net.IPAddress.Parse("127.0.0.1");           
        //    server.Start(ip,8910);
        //    server.BroadcastLine(message);
        //}
        //public void StopServer()
        //{
        //    if (server.IsStarted)
        //        server.Stop();
        //}
    }
}
