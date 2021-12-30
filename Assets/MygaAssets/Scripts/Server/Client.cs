using MygaCross;
using System.Net;

namespace MygaServer
{
    public class Client
    {
        public readonly int id;
        public EndPoint endPoint { get; private set; }

        public Client(int _clientId, EndPoint _endPoint)
        {
            id = _clientId;
            endPoint = _endPoint;
        }

        public void Send(Package _package)
        {
            ServerSocket.Send(this, _package);
        }

        public void Send(byte[] _data)
        {
            ServerSocket.Send(this, _data);
        }

        public void Disconnect()
        {
            Server.clients.Remove(this);
            endPoint = null;
        }
    }
}
