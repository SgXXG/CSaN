using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ftp_server
{
    class ClientConnection : IDisposable
    {
        private TcpClient _client;
        private TcpClient _dataClient;

        private NetworkStream _networkStream;
        private StreamReader _reader;
        private StreamWriter _writer;     
        private StreamWriter _dataWriter;

        private IPEndPoint _dataEndpoint;

        private string _currentDirectory;
        private bool _disposed;
        private bool _correctPass;
        private bool _correctUsername;


        public ClientConnection(TcpClient client)
        {
            _client = client;

            _networkStream = _client.GetStream();
            _reader = new StreamReader(_networkStream);
            _writer = new StreamWriter(_networkStream);
            _correctPass = false;
            _correctUsername = false;
        }

        private string Response(string cmd, string argument)
        {
            string response = "";
            switch (cmd)
            {
                case "USER":
                    response = "503 Already logged in!";
                    break;
                case "PASS":
                    response = "503 Already logged in!";
                    break;
                case "CWD":
                    response = ChangeWorkingDirectory(argument);
                    break;
                case "QUIT":
                    response = "221 Service closing connection";
                    break;
                case "XPWD":
                    response = PrintWorkingDirectory();
                    break;
                case "PORT":
                    response = Port(argument);
                    break;
                case "NLST":
                    response = List(argument);
                    break;
                case "RETR":
                    response = Retrieve(argument);
                    break;
                case "STOR":
                    response = Store(argument);
                    break;
                case "DELE":
                    response = Delete(argument);
                    break;
                case "XMKD":
                    response = MakeDirectory(argument);
                    break;
                case "XRMD":
                    response = RemoveDirectoyy(argument);
                    break;
                default:
                    response = "502 Command not implemented";
                    break;
            }
            return response;
        }

        private string LoginResponse(string cmd, string argument)
        {
            string response = "";

            switch (cmd)
            {
                case "USER":
                    response = CheckUsername(argument);
                    break;
                case "PASS":
                    response = CheckPassword(argument);
                    break;
                default:
                    response = "530 Not logged in";
                    break;
            }
            return response;
        }

        public void HandleClient()
        {
            try
            {
                _writer.WriteLine("220 Ready");
                _writer.Flush();
            }
            catch(IOException ex)
            {
                Console.WriteLine(ex.Message);
                return;
            }

            string line = null;

            // Make socket for transmit and receive data
            _dataClient = new TcpClient();
            
            while (!string.IsNullOrEmpty(line = _reader.ReadLine()))
            {
                Console.WriteLine(line);
                string response = null;

                string[] command = line.Split(' ');

                string cmd = command[0].ToUpper();

                string argument = command.Length > 1 ? line.Substring(command[0].Length + 1) : null;

                if (string.IsNullOrWhiteSpace(argument))
                    argument = null;

                if(_correctPass && _correctUsername)
                {
                    response = Response(cmd, argument);
                }
                else
                {
                    response = LoginResponse(cmd, argument);
                }

                if (_client == null || !_client.Connected)
                {
                    break;
                }
                else
                {
                    try
                    {
                        _writer.WriteLine(response);
                        _writer.Flush();
                    }
                    catch(IOException)
                    {
                        Console.WriteLine("Connection lost.");
                        break;
                    }
                    
                    if (response.StartsWith("221"))
                    {
                        break;
                    }
                }
            }
            Dispose();
        }

        private string Port(string hostPort)
        {

            string[] ipAndPort = hostPort.Split(',');

            byte[] ipAddress = new byte[4];
            byte[] port = new byte[2];

            for (int i = 0; i < 4; i++)
            {
                ipAddress[i] = Convert.ToByte(ipAndPort[i]);
            }

            for (int i = 4; i < 6; i++)
            {
                port[i - 4] = Convert.ToByte(ipAndPort[i]);
            }

            if (BitConverter.IsLittleEndian)
                Array.Reverse(port);

            int prt = port[1] * 256 + port[0];

            _dataEndpoint = new IPEndPoint(new IPAddress(ipAddress), prt);

            return "200 Data Connection Established";
        }

        // Help
        private string CheckUsername(string username)
        {
            if (username == null)
            {
                return "530 Not logged in. Missing <username>";
            }

            if (username == "sgxxg")
            {
                _correctUsername = true;
                return "331 Username ok, need password";
            }

            return "530 Not logged in";            
        }

        private string CheckPassword(string password)
        {
            if (password == null)
            {
                return "530 Not logged in. Missing <password>";
            }

            if (password == "123")
            {
                _currentDirectory = "D:\\";
                _correctPass = true;
                return "230 User logged in";
            }
            else
            {
                return "530 Not logged in";
            }
        }

        private string ChangeWorkingDirectory(string pathname)
        {
            if (pathname == "/")
            {
                _currentDirectory = "D:\\";
            }
            else
            {
                string newDir;
                try
                {
                    if (pathname.StartsWith("/"))
                    {
                        pathname = pathname.Substring(1).Replace('/', '\\');
                        newDir = Path.Combine("D:\\", pathname);
                    }
                    else
                    {
                        pathname = pathname.Replace('/', '\\');
                        newDir = Path.Combine(_currentDirectory, pathname);
                    }
                }
                catch(ArgumentException)
                {
                    return "550 Directory not found";
                }
                

                if (Directory.Exists(newDir))
                {
                    _currentDirectory = new DirectoryInfo(newDir).FullName;

                    if (!IsPathValid(_currentDirectory))
                    {
                        _currentDirectory = "D:\\";
                        return "550 Can't access directory";
                    }
                }
                else
                {
                    _currentDirectory = "D:\\";
                    return "550 Directory not found";
                }
            }

            return "250 Changed to new directory";
        }

        #region FTPServiceCommands

        private string List(string pathname)
        {
            pathname = NormalizeFilename(pathname);

            if(pathname != null)
            {   
                try
                { 
                    _writer.WriteLine("150 Opening Passive mode data transfer for LIST");
                    _writer.Flush();

                    return HandleList(pathname);
                }
                catch (IOException ex)
                {
                    Console.WriteLine(ex.Message);
                }

            }

            return "450 Requested file action not taken";
        }

        private string Delete(string pathname)
        {
            pathname = NormalizeFilename(pathname);

            if(pathname != null)
            {
                if(File.Exists(pathname))
                {
                    File.Delete(pathname);
                }
                else
                {
                    return "550 File not found";
                }

                return "250 Requested file action okay, completed";
            }

            return "550 File not found";
        }

        private string RemoveDirectoyy(string pathname)
        {
            pathname = NormalizeFilename(pathname);

            if(pathname != null)
            {
                if(Directory.Exists(pathname))
                {
                    Directory.Delete(pathname);
                }
                else
                {
                    return "550 Directory not found";
                }

                return "250 Requested directory action okay, comlpeted";
            }

            return "550 Directory not found";
        }

        private string MakeDirectory(string pathname)
        {
            pathname = NormalizeFilename(pathname);
            
            if(pathname != null)
            {
                if(!Directory.Exists(pathname))
                {
                    Directory.CreateDirectory(pathname);
                }
                else
                {
                    return "550 Directory already exists";
                }

                return "250 Requested directory action okay, completed";
            }

            return "550 Directory not created";
        }

        private string HandleList(string pathname)
        {
            _dataClient = new TcpClient(_dataEndpoint.AddressFamily);
            _dataClient.Connect(_dataEndpoint.Address, _dataEndpoint.Port);

            using (NetworkStream stream = _dataClient.GetStream())
            {
                _dataWriter = new StreamWriter(stream, Encoding.ASCII);

                IEnumerable<string> directories = Directory.EnumerateDirectories(pathname);

                foreach (string dir in directories)
                {
                    DirectoryInfo d = new DirectoryInfo(dir);
                    
                    string line = " ";
                    line = string.Concat(line, d.Name);

                    try
                    {
                        _dataWriter.WriteLine(line);
                        _dataWriter.Flush();
                    }
                    catch(IOException ex)
                    {
                        Console.WriteLine(ex.Message);
                        return "550 Requested action not taken";
                    }

                }


                IEnumerable<string> files = Directory.EnumerateFiles(pathname);

                foreach (string file in files)
                {
                    FileInfo f = new FileInfo(file);
                    
                    string line = string.Format(" ", f.Length, f.Name);

                    try
                    {
                        _dataWriter.WriteLine(line);
                        _dataWriter.Flush();
                    }
                    catch (IOException ex)
                    {
                        Console.WriteLine(ex.Message);
                        return "550 Requested action not taken";
                    }
                }
                
            }
            
            _dataClient.Close();
            _dataClient = null;

            return "226 Transfer complete";
        }

        private void HandleRetrieve(IAsyncResult res)
        {
            string pathname = res.AsyncState as string;

            _dataClient.EndConnect(res);

            using (NetworkStream dataStream = _dataClient.GetStream())
            {
                using (FileStream fs = new FileStream(pathname, FileMode.Open, FileAccess.Read))
                {
                    if(CopyStream(fs, dataStream, 4096) > 0)
                    {
                        try
                        {
                            _writer.WriteLine("226 Closing data connection, file transfer succesful");
                            _writer.Flush();
                        }
                        catch(IOException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
            }
            _dataClient.Close();
            _dataClient = null;
        }

        private string Retrieve(string pathname)
        {
            pathname = NormalizeFilename(pathname);

            if(IsPathValid(pathname))
            {
                if(File.Exists(pathname))
                {
                    _dataClient = new TcpClient(_dataEndpoint.AddressFamily);
                    _dataClient.BeginConnect(_dataEndpoint.Address, _dataEndpoint.Port, HandleRetrieve, pathname);

                    return "150 Opening Passive mode data transfer for RETR";
                }
            }
            return "550 File Not Found";
        }

        private string Store(string pathname)
        {
            pathname = NormalizeFilename(pathname);

            if (pathname != null)
            {           
                _dataClient = new TcpClient(_dataEndpoint.AddressFamily);
                _dataClient.BeginConnect(_dataEndpoint.Address, _dataEndpoint.Port, HandleStore, pathname);

                return "150 Opening Passive mode data transfer for STOR";
            }

            return "450 Requested file action not taken";
        }

        private void HandleStore(IAsyncResult res)
        {
            string pathname = res.AsyncState as string;
            
            _dataClient.EndConnect(res);

            using (NetworkStream dataStream = _dataClient.GetStream())
            {
                using (FileStream fs = new FileStream(pathname, FileMode.OpenOrCreate, FileAccess.Write, FileShare.None, 4096, FileOptions.SequentialScan))
                {
                    if(CopyStream(dataStream, fs, 4096) > 0)
                    {
                        try
                        {
                            _writer.WriteLine("226 Closing data connection, file transfer succesful");
                            _writer.Flush();
                        }
                        catch(IOException ex)
                        {
                            Console.WriteLine(ex.Message);
                        }
                    }
                }
            }
            _dataClient.Close();
            _dataClient = null;
        }

        private string PrintWorkingDirectory()
        {
            string current = _currentDirectory.Replace("D:\\", string.Empty).Replace('\\', '/');

            if (current.Length == 0)
            {
                current = "/";
            }

            return string.Format("257 \"{0}\" is current directory.", current); ;
        }

        #endregion
        private long CopyStream(Stream input, Stream output, int bufferSize)
        {
            byte[] buffer = new byte[bufferSize];
            int count = 0;
            long total = 0;

            while ((count = input.Read(buffer, 0, buffer.Length)) > 0)
            {
                try
                {
                    output.Write(buffer, 0, count);
                    total += count;
                }
                catch (IOException ioe)
                {
                    Console.WriteLine(ioe.Message);
                    return -1;
                }
            }

            return total;
        }

        private string NormalizeFilename(string path)
        {
            if (path == null)
            {
                path = string.Empty;
            }

            if (path == "/")
            {
                return "D:\\";
            }
            else if (path.StartsWith("/"))
            {
                path = new FileInfo(Path.Combine("D:\\", path.Substring(1))).FullName;
            }
            else
            {
                try
                {
                    path = new FileInfo(Path.Combine(_currentDirectory, path)).FullName;
                }
                catch (ArgumentException)
                {
                    return null;
                }
            }

            return IsPathValid(path) ? path : null;
        }

        private bool IsPathValid(string path)
        {
            return path.StartsWith("D:\\");
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                if (_client != null)
                {
                    _client.Close();
                }

                if (_dataClient != null)
                {
                    _dataClient.Close();
                }

                if (_networkStream != null)
                {
                    _networkStream.Close();
                }

                if (_reader != null)
                {
                    _reader.Close();
                }

                if (_writer != null)
                {
                    _writer.Close();
                }
            }

            _disposed = true;
        }
    }
}
