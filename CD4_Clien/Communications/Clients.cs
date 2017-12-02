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
        /*
         * Empfängt Nachrichten (in einem eigenen Task)
         * Trennt den Client
         * Sendet Nachrichten an den Server
         */

        byte[] buffer = new byte[500];
        Socket clientSocket;
        Action<string> MessageInformer; //Nachrichten an die GUI des Clients
        Action AbortInformer; //trennt die Verbindung des Clients

        /* eine Client-Instanz benötigt eine IP, einen Port und zwei Delegates:
         *   messageInformer schickt Nachrichten an die eigene GUI (des Clients)
         *   abortInformer informiert die GUI über die Trennung
         */
        public Clients(string ip, int port, Action<string> messageInformer, Action abortInformer)
        {
            try
            {
                MessageInformer = messageInformer;
                AbortInformer = abortInformer;
                TcpClient client = new TcpClient();
                client.Connect(IPAddress.Parse(ip), port);
                clientSocket = client.Client;

                //Empfang von Nachrichten initialisieren
                StartReceiving();
            }
            catch(Exception)
            {
                messageInformer("Server not ready"); 
                AbortInformer();
            }           
        }

        //Empfang von Nachrichten (im eigenen Task) starten
        public void StartReceiving()
        {
            //Erstellt und startet einen eigenen Task für Receive
            Task.Factory.StartNew(Receive); 
        }

        //Nachrichten vom Server bzw. über den Server von anderen Clients empfangen
        private void Receive()
        {
            string message = "";

            //solange der Server nicht die endMessage @quit schickt
            while (message != "@quit") 
            {
                //Nachricht von Byte in String konvertieren
                int length = clientSocket.Receive(buffer);
                message = Encoding.UTF8.GetString(buffer, 0, length);

                //GUI über Nachrichten informieren
                MessageInformer(message); 
            }

            //Client beenden, wenn Server endMessage @quit schickt
            Close(); 
        }

        //schließt den Socket des Clients und informiert die GUI
        public void Close()
        {
            //Socket des Clients schließen
            clientSocket.Close();

            //GUI über die Trennung informieren (-> Button Connect wird aktivierbar)
            AbortInformer(); 
        }

        //sendet Nachrichten an den Server
        public void Send(string message) 
        {
            if (clientSocket != null) //Überprüft aufrechte Verbindung
            {
                //Nachricht von String in Byte konvertieren
                clientSocket.Send(Encoding.UTF8.GetBytes(message));
            }
        }
    }
}
