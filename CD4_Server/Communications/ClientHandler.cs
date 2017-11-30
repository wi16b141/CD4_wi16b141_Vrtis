using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CD4_Server.Communications
{
    class ClientHandler
    {
        private Action<string, Socket> action; //um GUI über Nachrichten (string) + den Sender der Nachricht (Socket) zu informieren
        private byte[] buffer = new byte[500];
        private Thread clientReceiveThread;
        const string endMessage = "@quit"; //Befehl, um Chat zu beenden

        public string Name { get; private set; } //Name des Clients/Chatteilnehmers
        public Socket ClientSocket { get; private set; }

        public ClientHandler(Socket socket, Action<string, Socket> action)
        {
            ClientSocket = socket;
            this.action = action;

            //eigener Thread für Receiving
            clientReceiveThread = new Thread(Receive);
            clientReceiveThread.Start();
        }

        public void Receive() //empfängt Nachrichten
        {
            string message = "";
            while (!message.Equals(endMessage)) //solange nicht beendet wird
            {
                int length = ClientSocket.Receive(buffer);
                message = Encoding.UTF8.GetString(buffer, 0, length);

                if(Name == null && message.Contains(":"))
                {
                    Name = message.Split(':')[0]; //falls der Name in der message enthalten ist (zB "Kati: Hallo")
                }

                action(message, ClientSocket); //informiert die GUI über die Nachricht und den Sender der Nachricht
            }
            Close(); //Trennt den Client, falls endMessage @quit eingegeben wurde
        }

        public void Close() //beendet Clients und deren Threads
        {
            Send(endMessage);
            ClientSocket.Close(1); //1 = timeout
            clientReceiveThread.Abort(); //beendet Thread
        }

        public void Send(string message) //sendet Nachrichten an Clients
        {
            ClientSocket.Send(Encoding.UTF8.GetBytes(message));
        }

    }
}
