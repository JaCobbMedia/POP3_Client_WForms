using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Sockets;
using System.IO;

namespace POP3_Client_WForms
{
    public class Client
    {
        private string server;
        private int port;
        private string username;
        private string password;

        TcpClient tcpClient;
        Stream stream;
        StreamReader streamReader;

        public string Username { set => username = value; }
        public string Password { set => password = value; }

        public int NumberOfMessages { get; set; }
        public int SizeOfMessages { get; set; }

        private List<Message> Messages { get; set; }

        public Client (string server, int port)
        {
            this.server = server;
            this.port = port;
        }

        public void Connect()
        {
            try
            {
                tcpClient = new TcpClient(server, port);
            }
            catch (Exception)
            {
                throw new Exception("ERROR. Failed to connect to server.");
            }

            try
            {
                stream = tcpClient.GetStream();
            }
            catch (Exception)
            {
                throw new Exception("ERROR. Failed to reveice data stream.");
            }

            try
            {
                streamReader = new StreamReader(stream, Encoding.UTF8);
            }
            catch (Exception)
            {
                throw new Exception("ERROR. Failed to read data stream.");
            }

            string response = streamReader.ReadLine();

            if (response[0] != '+')
                throw new Exception("ERROR. No answer from server.");
        }

        public bool ExecuteCommand(string command)
        {
            Console.WriteLine(command);
            streamReader.DiscardBufferedData();

            byte[] commandBytes = System.Text.Encoding.UTF8.GetBytes((command + "\r\n").ToCharArray());
            stream.Write(commandBytes, 0, commandBytes.Length);
            stream.Flush();

            return (streamReader.Peek() == '+');
        }

        public bool LogIn(string username, string password)
        {
            Username = username;
            Password = password;

            if (!ExecuteCommand("USER " + this.username))
                return false;
            if (!ExecuteCommand("PASS " + this.password))
                return false;
            return true;
        }

        public void GetMailStats()
        {
            if (!ExecuteCommand("STAT"))
                throw new Exception("ERROR. Failed to receive inbox information.");

            string response = streamReader.ReadLine();
            Console.WriteLine("RESPONSE: " + response);
            string[] responseParts = response.Split(' ');

            NumberOfMessages = int.Parse(responseParts[1]);
            SizeOfMessages = int.Parse(responseParts[2]);

        }

        public List<Message> GetMessages(int upTo)
        {
            List<Message> list = new List<Message>();

            while (upTo > 0)
            {
                list.Add(GetMessage(upTo));
                upTo--;
            }

            return list;
        }

        public Message GetMessage(int number)
        {
            if (!ExecuteCommand("RETR " + number.ToString()))
                return new Message { Nr = number, Deleted = true };

            string response = streamReader.ReadLine();

            string[] responseParts = response.Split(' ');

            Message message = new Message()
            {
                Nr = number,
                Size = int.Parse(responseParts[1]),
                Text = new List<string>(),
                AllText = new List<string>(),
                Deleted = false
            };
            do
            {
                response = streamReader.ReadLine();
                if (response != ".")
                    message.AllText.Add(response);
            } while (response != ".");

            message.ExtractMessage();

            return message;
        }

        public List<int> GetMessagesSizes()
        {
            if (!ExecuteCommand("LIST"))
                throw new Exception("ERROR. Failed to receive information about letters.");

            List<int> list = new List<int>();

            string response = streamReader.ReadLine();

            string[] responseParts = response.Split(' ');

            for (int i = int.Parse(responseParts[1]); i >= 1; i--)
            {
                response = streamReader.ReadLine();
                responseParts = response.Split(' ');
                list.Add(int.Parse(responseParts[1]));
            }

            return list;
        }

        public int GetMessageSize(int number)
        {
            if (!ExecuteCommand("LIST " + number.ToString()))
                throw new Exception("ERROR. Failed to receive letter information.");

            string response = streamReader.ReadLine();

            string[] responseParts = response.Split(' ');

            return int.Parse(responseParts[2]);
        }

        public bool DeleteMessage(int number)
        {
            if (!ExecuteCommand("DELE " + number.ToString()))
                return false;
            return true;
        }

        public bool CheckConnection()
        {
            if (!ExecuteCommand("NOOP"))
                return false;
            return true;
        }

        public bool ResetDeletedMessages()
        {
            if (!ExecuteCommand("RSET"))
                return false;
            return true;
        }

        public bool EndConnection()
        {
            if (!ExecuteCommand("QUIT"))
                return false;
            return true;
        }
    }
}
