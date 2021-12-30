using MygaCross;
using System.Collections.Generic;

namespace MygaClient
{
    public delegate void ClientEventFunction(int id);
    public delegate void PackageRecieved(byte[] data);

    public class ClientEventData
    {
        public ClientEventFunction clientEvent;
        public bool once = false;

        public ClientEventData(ClientEventFunction clientEvent, bool once = false)
        {
            this.clientEvent = clientEvent;
            this.once = once;
        }
    }


    public class PackageRecievedData
    {
        public PackageRecieved packageRecieved;
        public int type;

        public PackageRecievedData(PackageRecieved packageRecieved, int type)
        {
            this.packageRecieved = packageRecieved;
            this.type = type;
        }
    }

    public enum ClientEvent
    {
        ClientConnected,
        ClientDisconnected,
    }


    public static class ClientEventSystem
    {
        public static Dictionary<ClientEvent, List<ClientEventData>> ClientEvents = new Dictionary<ClientEvent, List<ClientEventData>>()
        {
            { ClientEvent.ClientConnected, emptyEventList },
            { ClientEvent.ClientDisconnected, emptyEventList },
        };

        private static List<ClientEventData> emptyEventList => new List<ClientEventData>() { new ClientEventData(new ClientEventFunction((target) => { })) };


        public static void On(ClientEvent eventType, ClientEventFunction action)
        {
            ClientEvents[eventType].Add(new ClientEventData(action));
        }

        public static void Once(ClientEvent eventType, ClientEventFunction action)
        {
            ClientEvents[eventType].Add(new ClientEventData(action, true));
        }

        public static void DisOn(ClientEvent eventType, ClientEventFunction action)
        {
            ClientEvents[eventType].Remove(new ClientEventData(action));
        }

        public static void DisOn(ClientEvent eventType, int id)
        {
            ClientEvents[eventType].RemoveAt(id);
        }

        public static void StartEvent(ClientEvent eventType)
        {
            List<ClientEventData> handlers = ClientEvents[eventType];

            for (int i = 0; i < handlers.Count; i++)
            {
                handlers[i].clientEvent(i);
                if (handlers[i].once)
                    DisOn(eventType, i);
            }
        }

        public static HashSet<PackageRecievedData> packageEvents = new HashSet<PackageRecievedData>();

        public static void OnPackageRecieved(PackageRecieved packageRecieved, int packageType = 0)
        {
            packageEvents.Add(new PackageRecievedData(packageRecieved, packageType));
        }

        public static void PackageRecieved(byte[] data)
        {
            foreach (PackageRecievedData recievedData in packageEvents)
            {
                Package package = new Package(data);

                if (package.packageID == recievedData.type || recievedData.type == 0)
                    recievedData.packageRecieved(data);
            }
        }
    }
}
