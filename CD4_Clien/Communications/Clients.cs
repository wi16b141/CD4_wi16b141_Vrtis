using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CD4_Clien.Communications
{
    class Clients
    {
        byte[] buffer = new byte[500];
        Socket clientSocket;
        Action<string> MessageInformer;
        Action AbortInformer;

        public Clients(string ip, int port, Action<string> messageInformer, Action abortInformer)
        {
            try
            {
                MessageInformer = messageInformer;
                AbortInformer = abortInformer;
                TcpClient client = new TcpClient();
                client.Connect(IPAddress.Parse(ip), port);
                clientSocket = client.Client;
                StartReceiving();
            }
            catch(Exception)
            {
                messageInformer("Server not ready");
                AbortInformer(); //reset
            }           
        }

        public void StartReceiving()
        {
            Task.Factory.StartNew(Receive); //Erstellt und startet einen eigenen Task für Receive
        }

        private void Receive()
        {
            string message = "";
            while (!message.Equals("@quit")) //solange nicht beendet wird
            {
                int length = clientSocket.Receive(buffer);
                message = Encoding.UTF8.GetString(buffer, 0, length);
                
                MessageInformer(message); //GUI informieren
            }
            Close();
        }

        public void Close()
        {
            clientSocket.Close();
            AbortInformer();
        }

        public void Send(string message)
        {
            if (clientSocket != null) //Überprüft aufrechte Verbindung
            {
                clientSocket.Send(Encoding.UTF8.GetBytes(message));
            }
        }
    }
}
