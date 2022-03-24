using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace TcpCLient
{
    class Program
    {
        static int port = 43;
        static void Main(string[] args)
        {
            Console.Write("Input the name of the server: ");
            string serverName;
            IPHostEntry hostEntry;
            try {
                serverName = Console.ReadLine();
            }
            catch (Exception e)
            {
                serverName = null;
                throw new Exception("Mistake");
            } 

            if (serverName == null)
            {
                throw new Exception("Mistake");
            }
            else
            {
                hostEntry = Dns.GetHostEntry(serverName);
            }
                
            IPAddress ipAddress = hostEntry.AddressList[0];
            Console.WriteLine("Address: " + ipAddress.ToString());

            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(new IPEndPoint(ipAddress, port));

            CommunicateWithServer(socket);

            socket.Shutdown(SocketShutdown.Both);
            socket.Close();

            Console.Read();
        }

        static void CommunicateWithServer(Socket socket)
        {
            Console.Write("Enter query: ");
            string request = Console.ReadLine() + "\r\n";
            byte[] outputData = Encoding.UTF8.GetBytes(request);
            socket.Send(outputData);

            StringBuilder response = new StringBuilder();
            int bytesRead = 0;
            byte[] inputData = new byte[256];

            do
            {
                bytesRead = socket.Receive(inputData);
                string text = Encoding.UTF8.GetString(inputData, 0, bytesRead);
                response.Append(text);
            }
            while (bytesRead > 0);

            Console.WriteLine("\n" + response.ToString());
        }
    }
}