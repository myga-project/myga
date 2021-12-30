using MygaCross;
using UnityEngine;

namespace MygaClient
{
    public static class Handler
    {

        public static void ConnectEvents()
        {
            ConnectBasicEvents();
            ConnectModuleEvents();
        }

        private static void ConnectBasicEvents()
        {
            ClientEventSystem.OnPackageRecieved(OnPackage);
            ClientEventSystem.OnPackageRecieved(OnConnectPackage, (int)PackageID.Connect);
            ClientEventSystem.OnPackageRecieved(OnErrorPackage, (int)PackageID.Error);
        }

        private static void ConnectModuleEvents()
        {
            ClientEventSystem.OnPackageRecieved(OnTransformPackage, (int)PackageID.Transform);
        }

        public static void OnPackage(byte[] data)
        {
        }

        public static void OnErrorPackage(byte[] data)
        {
            using (ErrorPackage package = new ErrorPackage(data))
            {
                Debug.LogWarning(package.message);
                if (package.disconnect)
                    Client.Disconnect();
            }
        }

        public static void OnConnectPackage(byte[] data)
        {
            if (Client.connected)
                return;

            using(ConnectPackage package = new ConnectPackage(data))
            {
                switch (package.status)
                {
                    case ConnectStatus.connected:
                        Client.SetConnectStatus(true);
                        ClientEventSystem.StartEvent(ClientEvent.ClientConnected);
                        Debug.Log($"Successfully connected to server: {Client.serverIp}:{Client.serverPort}");
                        break;
                    case ConnectStatus.full:
                        Debug.LogWarning($"Can't connect to server: {Client.serverIp}:{Client.serverPort}. Server is full!");
                        break;
                }
            }
        }

        public static void OnTransformPackage(byte[] data)
        {
            using (TransformPackage package = new TransformPackage(data))
            {
                Transform _transform = MygaNetwork.FindObjectByID(package.id).transform;
                _transform.position = package.position;
                _transform.rotation = package.rotation;
                _transform.localScale = package.scale;
            }
        }
    }
}
