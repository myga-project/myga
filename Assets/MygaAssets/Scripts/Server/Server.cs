using System.Collections.Generic;
using System.Net;
using MygaCross;
using UnityEngine;

namespace MygaServer
{

    public static class Server
    {
        public static int MaxPlayers { get; private set; } = 1;
        public static int CurrentPlayers = 0;
        public static string Ip { get; private set; } = "";
        public static int Port { get; private set; } = 0;
        public static bool Running { get; private set; } = false;

        public readonly static List<Client> clients = new List<Client>();
        public static MygaControl mygaControl;

        public static void Start(string ip, int port, int maxPlayers)
        {
            if (Running)
                Debug.LogError("Server is already running!");

            Ip = ip;
            Port = port;
            MaxPlayers = maxPlayers;
            Running = true;
          
            ServerSocket.Run(ip, port);

            GameObject connection = GameObject.Instantiate(new GameObject("MygaServer"));
            mygaControl = connection.AddComponent<MygaControl>();
        }

        public static void Stop()
        {
            clients.Clear();
            ServerSocket.Close();
        }

        public static void SendAll(Package _package)
        {
            foreach (Client client in clients)
            {
                client.Send(_package);
            }
        }

        public static void SendAll(byte[] _data)
        {
            foreach (Client client in clients)
            {
                client.Send(_data);
            }
        }

        public static Client GetClient(EndPoint _clientEndPoint)
        {
            foreach (Client client in clients)
            {
                if (client.endPoint.ToString() == _clientEndPoint.ToString())
                    return client;
            }

            return null;
        }

        public static bool ClientExist(EndPoint _endPoint)
        {
            foreach (Client client in clients)
            {
                if (client.endPoint.ToString() == _endPoint.ToString())
                    return true;
            }

            return false;
        }

        private static bool ServerFull => CurrentPlayers == MaxPlayers;

        public static ConnectStatus TryAddClient(EndPoint _clientEndPoint)
        {
            if (ClientExist(_clientEndPoint))
                return ConnectStatus.already;

            if (ServerFull)
                return ConnectStatus.full;

            Client client = new Client(clients.Count, _clientEndPoint);
            clients.Add(client);

            ServerEventSystem.StartEvent(ServerEvent.ClientConnected);
            return ConnectStatus.connected;
        }
    }
}
