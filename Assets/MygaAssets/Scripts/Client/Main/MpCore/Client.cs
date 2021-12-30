using UnityEngine;
using System.Net;
using System.Net.Sockets;
using System;
using MygaCross;
using System.Threading;

namespace MygaClient 
{
    public class State
    {
        public byte[] buffer = new byte[Client.bufferSize];
    }

    public static class Client
    {
        public static readonly int bufferSize = 4096;
        private static Socket _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
        private static State state = new State();

        public static string serverIp { get; private set; } = "127.0.0.1";
        public static int serverPort { get; private set; } = 7777;
        public static int myId { get; private set; } = 0;
        public static bool connected { get; private set; } = false;

        public static void Connect(string _ip, int _port)
        {
            serverIp = _ip;
            serverPort = _port;

            _socket.Connect(IPAddress.Parse(serverIp), serverPort);
            EndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
            _socket.BeginReceiveFrom(state.buffer, 0, bufferSize, SocketFlags.None, ref endPoint, RecieveCallback, state);

            Handler.ConnectEvents();
            using(ConnectPackage package = new ConnectPackage(ConnectStatus.connected))
                Send(package, true);
        }

        public static void SetConnectStatus(bool _connected) { connected = _connected; }

        public static void Send(Package _package, bool ignoreConnectStatus = false)
        {
            try
            {
                if (!connected && !ignoreConnectStatus)
                    return;

                byte[] _data = _package.ToArray();
                _socket.BeginSend(_data, 0, _data.Length, SocketFlags.None, (ar) => { _socket.EndSend(ar); }, state);
            }
            catch
            {
                Disconnect();
            }
        }

        private static void RecieveCallback(IAsyncResult result)
        {
            try
            {
                EndPoint endPoint = new IPEndPoint(IPAddress.Any, 0);
                State so = (State)result.AsyncState;
                _socket.EndReceiveFrom(result, ref endPoint);
                _socket.BeginReceiveFrom(so.buffer, 0, bufferSize, SocketFlags.None, ref endPoint, RecieveCallback, so);

                MygaNetwork.mygaConnection.AddPackage(so.buffer);
            }
            catch(Exception _ex)
            {
                Debug.Log(_ex.Message);
                Disconnect();
            }
        }

        public static void Disconnect()
        {
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            state = new State();

            Debug.Log($"Disconnected from server: {serverIp}:{serverPort}");
            ClientEventSystem.StartEvent(ClientEvent.ClientDisconnected);

            serverIp = "127.0.0.1";
            serverPort = 7777;
            myId = 0;
            connected = false;
        }
    }
}