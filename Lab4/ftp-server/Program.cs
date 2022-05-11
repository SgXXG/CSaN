using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ftp_server
{
    class Program
    {
        static void Main(string[] args)
        {
            Ftpserv ftpServer = new Ftpserv(21);

            ftpServer.Start();
            Console.WriteLine("Press any key to stop...");
            Console.ReadKey(true);

        }
    }
}
