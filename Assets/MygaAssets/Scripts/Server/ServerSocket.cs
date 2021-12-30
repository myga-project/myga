using MygaCross;
using System;
using System.Net;
using System.Net.Sockets;
using UnityEngine;

namespace MygaServer
{
    public class State
    {
        public byte[] buffer = new byte[ServerSocket.bufferSize];
    }

    public static class ServerSocket
    {
        public static readonly int bufferSize = 4096;
        private static readonly Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private static readonly State state = new State();

        public static void Run(string ip, int port)
        {
            Handler.ConnectEvents();

            socket.SetSocketOption(SocketOptionLevel.IP, SocketOptionName.ReuseAddress, true);
            socket.Bind(new IPEndPoint(IPAddress.Parse(ip), port));

            EndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
            socket.BeginReceiveFrom(state.buffer, 0, bufferSize, SocketFlags.None, ref clientEndPoint, RecieveCallback, state);

            ServerEventSystem.StartEvent(ServerEvent.ServerStarted);
        }

        public static void Close()
        {
            socket.Close();
        }

        public static void Send(Client _client, Package _package)
        {
            try
            {
                byte[] _data = _package.ToArray();
                socket.BeginSendTo(_data, 0, _data.Length, SocketFlags.None, _client.endPoint, (ar) => { socket.EndSend(ar); }, state);
            }
            catch (Exception _ex)
            {
                Debug.Log($"Error while sending data to client: {_ex}. Disconnecting client!");
                _client.Disconnect();
            }
        }

        public static void Send(Client _client, byte[] _data)
        {
            try
            {
                socket.BeginSendTo(_data, 0, _data.Length, SocketFlags.None, _client.endPoint, (ar) => { socket.EndSend(ar); }, state);
            }
            catch (Exception _ex)
            {
                Debug.Log($"Error while sending data to client: {_ex}. Disconnecting client!");
                _client.Disconnect();
            }
        }

        public static void Send(EndPoint _endPoint, Package _package)
        {
            try
            {
                byte[] _data = _package.ToArray();
                socket.BeginSendTo(_data, 0, _data.Length, SocketFlags.None, _endPoint, (ar) => { socket.EndSend(ar); }, state);
            }
            catch (Exception _ex)
            {
                Debug.Log($"Error while sending data: {_ex}.");
            }
        }

        public static void SendAll(Package _package, int _exceptID = -1)
        {
            foreach(Client client in Server.clients)
                if(client.id != _exceptID)
                    Send(client, _package);
        }

        public static void SendAll(Package _package, int[] _exceptIDs)
        {
            foreach (Client client in Server.clients)
                foreach(int _exceptID in _exceptIDs)
                    if (client.id != _exceptID)
                        Send(client, _package);
        }

        private static void RecieveCallback(IAsyncResult _result)
        {
            EndPoint clientEndPoint = new IPEndPoint(IPAddress.Any, 0);
            State so = (State)_result.AsyncState;

            socket.EndReceiveFrom(_result, ref clientEndPoint);
            socket.BeginReceiveFrom(so.buffer, 0, bufferSize, SocketFlags.None, ref clientEndPoint, RecieveCallback, so);
            TryConnectClient(clientEndPoint, so);
        }

        private static void TryConnectClient(EndPoint _clientEndPoint, State _so)
        {
            ConnectStatus connectStatus = Server.TryAddClient(_clientEndPoint);
            switch (connectStatus)
            {
                case ConnectStatus.connected:
                    ServerEventSystem.PackageRecieved(_so.buffer, Server.GetClient(_clientEndPoint));
                    break;
                case ConnectStatus.already:
                    DisconnectCheck(_clientEndPoint, _so);
                    ServerEventSystem.PackageRecieved(_so.buffer, Server.GetClient(_clientEndPoint));
                    break;
            }

            if (connectStatus == ConnectStatus.full || connectStatus == ConnectStatus.connected)
                using (ConnectPackage package = new ConnectPackage(connectStatus))
                    Send(_clientEndPoint, package);
        }

        private static bool DisconnectCheck(EndPoint _clientEndPoint, State _so)
        {
            Package package = new Package(_so.buffer);

            if (package.packageID == (int)PackageID.Disconnect)
            {
                Server.GetClient(_clientEndPoint).Disconnect();
                ServerEventSystem.StartEvent(ServerEvent.ClientDisconnected);
                return true;
            }

            return false;
        }
    }
}
