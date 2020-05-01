using System;
using System.Net.Sockets;
using System.Net;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using System.Text;

namespace Server
{
    class Program
    {
        static void Main(string[] args)
        {

            Server s = new Server(5);
            s.startrServer();
            Thread serverListener = new Thread(new ThreadStart(s.StartListen));
            serverListener.Start();

            Console.WriteLine("Hello World!");
            Console.ReadKey();
        }
    }
    class Server
    {
        List<Socket> clients = new List<Socket>();
        private int maxConnections = 0;
        //private byte[] buffer = new byte[200];
        public int maxCon { get; set; }
        static Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);


        public Server(int conn)
        {
            maxConnections = conn;
        }
        public void startrServer()
        {
            Console.WriteLine("Вкажiть порт");
            int port = Int32.Parse(Console.ReadLine());
            socket.Bind(new IPEndPoint(IPAddress.Any, port));
            //socket.Listen(maxCon);


        }
        public void StartListen()
        {
            while (true)
            {
                Console.WriteLine("Жду подключения");
                socket.Listen(maxCon);
                Socket client = socket.Accept();
                if (client != null)
                {
                    Console.WriteLine($"User connected {clients.Count + 1}");
                    clients.Add(client);
                    Thread GMThread = new Thread(new ParameterizedThreadStart(GetMessage));
                    GMThread.Start(client);
                    client = null;
                }
                //Console.WriteLine("Цыкл завершон");
            }
            Console.WriteLine("поток окончен");
        }
        public void GetMessage(Object s)
        {
            byte[] buffer = new byte[100];
            Random rnd = new Random();
            int uId = rnd.Next(0, 1000);
            string OutMessage;
            while (true)
            {
                // Console.WriteLine("Etap1");
                Socket _client = (Socket)s;
                if (_client.Connected)
                {
                    //  Console.WriteLine("Etap2");
                    try 
                    {
                        _client.Receive(buffer);
                        OutMessage = $"UserId:{uId} :-: {Encoding.ASCII.GetString(buffer)}";
                        if (buffer.Length > 0)
                        {
                            //    Console.WriteLine("Etap3");
                            Console.WriteLine($"UserId:{uId} :-: {Encoding.ASCII.GetString(buffer)}");
                            foreach (Socket g in clients)
                            {
                                if (g != _client)
                                {
                                    g.Send(buffer);
                                }
                            }

                            buffer = new byte[100];

                        }
                    }
                    catch(Exception)
                    {
                        Console.WriteLine($"User {uId} disconected!");
                        clients.Remove(_client);
                        Thread.CurrentThread.Abort();
                        break;
                    }
                    //Console.WriteLine("Etap4");
                }
                else
                {
                    Thread.CurrentThread.Abort();
                    break;
                }
            }
        }


    }
}
