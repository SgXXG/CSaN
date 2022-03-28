using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Net.Security;
using System.IO;

namespace Pop3Client
{
    public partial class frmMain : Form
    {
        private const int READ_TIMEOUT = 1000;

        private TcpClient Pop3Client;
        private SslStream Stream;
        private bool ChangedAddr = false;

        public frmMain()
        {
            InitializeComponent();
            textField.Text = "";
        }


        private void btnConnect_Click(object sender, EventArgs e)
        {
            try
            {
                if (ChangedAddr)
                {
                    ExitConnection();
                    ChangedAddr = false;
                }

                string POP3ServerAddr = textFieldServerAddress.Text;
                if (POP3ServerAddr.Length == 0)
                {
                    MessageBox.Show("POP3-server address is empty!");
                    return;
                }

                int port;
                if (!Int32.TryParse(textFieldPort.Text, out port))
                {
                    MessageBox.Show("Incorrect port number!");
                    return;
                }

                Pop3Client = new TcpClient();
                textField.Text += "C: Trying connect to " + POP3ServerAddr + " on port " + port + "\r\n";
                Pop3Client.Connect(POP3ServerAddr, port);
                Stream = new SslStream(Pop3Client.GetStream());
                Stream.ReadTimeout = READ_TIMEOUT;
                Stream.AuthenticateAsClient(POP3ServerAddr);

                btnConnect.Enabled = false;
            }
            catch 
            {
                textField.Text += "Failed connection to " + textFieldServerAddress.Text + " on port " + textFieldPort.Text + "!\r\n";
                return;
            }

            StringBuilder response = new StringBuilder();
            byte[] buffer = new byte[4096];
            int bytes = -1;

            try
            {
                bytes = Stream.Read(buffer, 0, buffer.Length);
                response.Append("S: ");
                response.Append(Encoding.ASCII.GetString(buffer, 0, bytes));
                textField.Text += response.ToString();
            }
            catch (Exception)
            {
                textField.Text += "Failed receive response from server!\r\n";
            }
        }

        private void btnSendCommand_Click(object sender, EventArgs e)
        {
            string command = textFieldCommand.Text + "\r\n";

            byte[] data = new byte[256];
            StringBuilder response = new StringBuilder();

            byte[] buffer = new byte[4096];
            int bytes = -1;

            if (!Pop3Client.Connected)
            {
                btnConnect.Enabled = true;
                MessageBox.Show("You are not connected to server!");
                return;
            }

            try
            {
                Stream.Flush();
                textField.Text += "C: " + command;
                Stream.Write(Encoding.ASCII.GetBytes(command));
                textFieldCommand.Clear();

                bytes = Stream.Read(buffer, 0, buffer.Length);
                response.Clear();
                response.Append("S: ");
                response.Append(Encoding.ASCII.GetString(buffer, 0, bytes));
                textField.Text += response.ToString();
                Stream.Flush();

                while ((bytes = Stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    response.Clear();
                    response.Append(Encoding.ASCII.GetString(buffer, 0, bytes));
                    textField.Text += response.ToString();
                    Stream.Flush();
                }
            }
            catch {  }
        }

        private void ExitConnection()
        {
            byte[] data = new byte[256];
            StringBuilder response = new StringBuilder();

            byte[] buffer = new byte[4096];
            int bytes = -1;

            try
            {
                Stream.Flush();
                textField.Text += "C: " + "QUIT";
                Stream.Write(Encoding.ASCII.GetBytes("QUIT\r\n"));

                bytes = Stream.Read(buffer, 0, buffer.Length);
                response.Clear();
                response.Append("S: ");
                response.Append(Encoding.ASCII.GetString(buffer, 0, bytes));
                textField.Text += response.ToString();
                Stream.Flush();

                while ((bytes = Stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    response.Clear();
                    response.Append(Encoding.ASCII.GetString(buffer, 0, bytes));
                    textField.Text += response.ToString();
                    Stream.Flush();
                }
            }
            catch { }

            Pop3Client = null;
            Stream = null;
            btnConnect.Enabled = true;
            textFieldCommand.Clear();
        }

        private void textFieldServerAddress_TextChanged(object sender, EventArgs e)
        {
            ChangedAddr = true;
            btnConnect.Enabled = true;
        }

        private void textFieldPort_TextChanged(object sender, EventArgs e)
        {
            ChangedAddr = true;
            btnConnect.Enabled = true;
        }

        private void btnClear_Click(object sender, EventArgs e)
        {
            textField.Clear();
        }
    }
}
