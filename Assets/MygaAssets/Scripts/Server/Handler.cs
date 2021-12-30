using MygaCross;
using System;
using UnityEngine;

namespace MygaServer
{
    public static class Handler
    {
        public static void ConnectEvents()
        {

            ServerEventSystem.On(ServerEvent.ServerStarted, (eventID) => {
                Debug.Log($"Server started on: {Server.Ip}:{Server.Port} with maximum amount of players: {Server.MaxPlayers}!");
            });

            ServerEventSystem.On(ServerEvent.ClientConnected, (eventID) => {
                Server.CurrentPlayers++;
                Debug.Log("Player connected: " + Server.CurrentPlayers);

                ConnectPackage package = new ConnectPackage(ConnectStatus.connected);
                Server.clients[Server.CurrentPlayers - 1].Send(package);
            });

            ServerEventSystem.On(ServerEvent.ClientDisconnected, (eventID) =>
            {
                Server.CurrentPlayers--;
                Debug.Log("Player disconnected: " + Server.CurrentPlayers);
            });

            ConnectBasicEvents();
            ConnectModuleEvents();
        }

        public static void ConnectBasicEvents()
        {

        }

        private static void ConnectModuleEvents()
        {
            ServerEventSystem.OnPackageRecieved(OnTransformPackage, (int)PackageID.Transform);
        }

        public static void OnTransformPackage(byte[] data)
        {
            Server.SendAll(data);
        }
    }
}
