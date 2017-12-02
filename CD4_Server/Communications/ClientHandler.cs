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
        /*
         * Empfängt Nachrichten (in einem eigenen Thread)
         * Beendet Clientverbindungen und verwirft obigen Thread dabei
         * Sendet Nachrichten an Clients (vom Server selbst bzw. über den Server von anderen Clients)         * 
         */

        private Action<string, Socket> action; //um GUI über Nachrichten (string) + den Sender der Nachricht (Socket) zu informieren
        private byte[] buffer = new byte[500]; //Byte-Puffer, um Byte-Nachrichten zu empfangen
        private Thread clientReceiveThread; //eigener Thread für den Empfang v. Nachrichten
        const string endMessage = "@quit"; //Befehl, um Chat zu beenden

        public string Name { get; private set; } //Name des Clients/Chatteilnehmers
        public Socket Socket { get; private set; }

        /* Eine neue ClientHanlder-Instanz benötigt einen Socket, und einen Delegate, um den
        *    Server sowohl über Nachrichten als auch über dessen Empfänger zu informieren
        */
        public ClientHandler(Socket socket, Action<string, Socket> action)
        {
            this.Socket = socket;
            this.action = action;      
            
            //Thread für den Empfang von Nachrichten initiieren und starten
            clientReceiveThread = new Thread(Receive);
            clientReceiveThread.Start();
        }

        //empfängt Nachrichten
        public void Receive() 
        {
            string message = "";

            //solange vom Server ausgehend nicht beendet wird
            while (!message.Equals(endMessage)) 
            {
                //empfangene Nachricht von Byte in String konventieren
                int length = Socket.Receive(buffer);
                message = Encoding.UTF8.GetString(buffer, 0, length);

                //bei der ersten Nachrichtenübermittlung wird der Name vergeben,
                if (Name == null && message.Contains(":"))
                {
                    Name = message.Split(':')[0]; //message = "Kati: Hallo";
                }

                //informiert die GUI über die Nachricht und Sender
                action(message, Socket); 
            }

            //Trennt den Client, falls der Server die endMessage @quit schickt
            Close(); 
        }

        //beendet Client und dessen Receive-Thread
        public void Close() 
        {
            Send(endMessage);
            Socket.Close(1); //1 = timeout
            clientReceiveThread.Abort(); //beendet Thread
        }

        //sendet Nachrichten an Clients
        public void Send(string message) 
        {
            //Nachricht von String in Byte konvertieren
            Socket.Send(Encoding.UTF8.GetBytes(message));
        }

    }
}
