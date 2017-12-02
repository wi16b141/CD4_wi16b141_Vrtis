using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CD4_Server.Communications
{
    class Server
    {
        /*
         * Akzeptiert neue Clients (in eigenem Thread),
         * Trennt die Verbindung zu allen Clients und stoppt gleichzeitig den Thread für neue Clients
         * Trennt spezifische Clients
         * Informiert alle Clients über Nachrichten von den anderen Clients (das Senden der
         *   eigentlichen Nachricht wird in der ClientHandler Klasse definiert!)
         */ 

        Socket serverSocket;       
        Action<string> GuiUpdater; //delegate, um GUI über Nachrichten von Clients zu informieren
        List<ClientHandler> clients = new List<ClientHandler>(); //verwaltet alle ClientHandler
        Thread acceptingThread; //eigener Thread für das Akzeptieren v. Clients

        //eine Server-Instanz benötigt eine IP, einen Port und einen Delegate, um die GUI upzudaten
        public Server(string ip, int port, Action<string> guiupdater)
        {
            GuiUpdater = guiupdater;
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
            serverSocket.Listen(10);
        }

        public void StartAccepting()
        {
            acceptingThread = new Thread(new ThreadStart(Accept));
            acceptingThread.IsBackground = true; //wird beendet sobald Foreground-Prozess beendet wird
            acceptingThread.Start();
        }

        private void Accept()
        {
            //solange der Thread nicht beendet oder abgebrochen wurde
            while (acceptingThread.IsAlive)
            {
                try
                {
                    //neuen ClientHandler pro Client hinzufügen
                    clients.Add(new ClientHandler(serverSocket.Accept(),
                        new Action<string, Socket>(NewMessageReceived)));
                }
                catch (Exception)
                {
                    //sobald serversocket.close aufgerufen wird
                }
            }
        }

        //empfängt Nachrichten von Clients und leitet dies an die eigene GUI und die Clients weiter
        private void NewMessageReceived(string message, Socket senderSocket)
        {
            GuiUpdater(message); //informiert die GUI, wenn ein Client eine Nachricht gesendet hat
            
            foreach (var item in clients) //sendet die Nachricht an alle anderen Clients weiter
            {
                if (item.Socket != senderSocket)
                {
                    item.Send(message);
                }
            }
        }

        /* schließt den Socket des Servers, trennt alle Clients und löscht sie aus der 
         *    Liste und schließt den Thread für d. Akzeptieren von neuen Clients
         */
        public void StopAccepting() 
        {
            serverSocket.Close(); //schließt den Socket für den Server
            acceptingThread.Abort(); //beendet den Thread, der neue Clients akzeptiert

            foreach (var item in clients) //alle Clients schließen
            {
                item.Close();
            }
            clients.Clear(); //Liste leeren
        }

        //einen spezifischen Client schließen und aus der Liste löschen
        public void DisconnectSpecificClient(string name)
        {
            try 
            {
                foreach (var item in clients)
                {
                    if (item.Name.Equals(name)) 
                    {
                        item.Close();//trennt spezifischen Client
                        clients.Remove(item); //und entfernt ihn aus der Clients Liste
                        //break;
                    }
                }
            }
            catch (Exception)
            {
                //throw new Exception("No clients left");
            }
            
        }
    }
}
