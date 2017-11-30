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
        Socket serverSocket;        
        Action<string> GuiUpdater;

        //handles clients
        List<ClientHandler> clients = new List<ClientHandler>();
        Thread acceptingThread;
        

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
            acceptingThread.IsBackground = true;
            acceptingThread.Start();
        }

        private void Accept()
        {
            while (acceptingThread.IsAlive)
            {
                try
                {
                    clients.Add(new ClientHandler(serverSocket.Accept(), new Action<string, Socket>(NewMessageReceived)));
                }
                catch (Exception e)
                {
                    //executed if serversocket.close is called
                }
            }
        }
        private void NewMessageReceived(string message, Socket senderSocket)
        {
            GuiUpdater(message); //informiert die GUI
            
            foreach (var item in clients) //informiert alle andern Clients
            {
                if (item.ClientSocket != senderSocket)
                {
                    item.Send(message);
                }
            }
        }

        public void StopAccepting() //alle Client Sockets und Threads schließen
        {
            serverSocket.Close(); //schließt den Socket für den Server
            acceptingThread.Abort(); //beendet den Thread, der neue Clients akzeptiert

            foreach (var item in clients) //alle Clients schließen
            {
                item.Close();
            }
            clients.Clear(); //Liste leeren
        }

        public void DisconnectSpecificClient(string name)
        {
            foreach (var item in clients) 
            {
                if (item.Name.Equals(name))
                {
                    item.Close();
                    clients.Remove(item);
                    //break;
                }
            }
        }
    }
}
