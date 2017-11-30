using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace CD4_Server.Communication
{
    class Server
    {
        Socket serverSocket;
        List<ClientHandler> clients = new List<ClientHandler>();
        //Action<string> GuiUpdater;

        public Server(string ip, int port )//, Action<string> guiUpdater
        {
            //GuiUpdater = guiUpdater;
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(IPAddress.Parse(ip), port));
            serverSocket.Listen(10);
        }



    }
}
