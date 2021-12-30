using MygaCross;
using System.Collections.Generic;

namespace MygaServer
{
    public delegate void ServerEventFunction(int id);
    public delegate void PackageRecieved(byte[] data, Client client);

    public class ServerEventData
    {
        public ServerEventFunction serverEvent;
        public bool once = false;

        public ServerEventData(ServerEventFunction serverEvent, bool once = false)
        {
            this.serverEvent = serverEvent;
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

    public enum ServerEvent
    {
        ServerStarted,
        ServerClose,
        ClientConnected,
        ClientDisconnected,
    }

    public static class ServerEventSystem
    {
        public static Dictionary<ServerEvent, List<ServerEventData>> serverEvents = new Dictionary<ServerEvent, List<ServerEventData>>()
        {
            { ServerEvent.ClientConnected, emptyEventList },
            { ServerEvent.ClientDisconnected, emptyEventList },
            { ServerEvent.ServerStarted, emptyEventList }
        };

        private static List<ServerEventData> emptyEventList => new List<ServerEventData>() { new ServerEventData(new ServerEventFunction((target) => { })) };


        public static void On(ServerEvent eventType, ServerEventFunction action)
        {
            serverEvents[eventType].Add(new ServerEventData(action));
        }

        public static void Once(ServerEvent eventType, ServerEventFunction action)
        {
            serverEvents[eventType].Add(new ServerEventData(action, true));
        }

        public static void DisOn(ServerEvent eventType, ServerEventFunction action)
        {
            serverEvents[eventType].Remove(new ServerEventData(action));
        }

        public static void DisOn(ServerEvent eventType, int id)
        {
            serverEvents[eventType].RemoveAt(id);
        }

        public static void StartEvent(ServerEvent eventType)
        {
            List<ServerEventData> handlers = serverEvents[eventType];

            for (int i = 0; i < handlers.Count; i++)
            {
                handlers[i].serverEvent(i);
                if (handlers[i].once)
                    DisOn(eventType, i);
            }
        }

        public static HashSet<PackageRecievedData> packageEvents = new HashSet<PackageRecievedData>();

        public static void OnPackageRecieved(PackageRecieved packageRecieved, int packageType = 0)
        {
            packageEvents.Add(new PackageRecievedData(packageRecieved, packageType));
        }

        public static void PackageRecieved(byte[] data, Client client)
        {
            foreach (PackageRecievedData recievedData in packageEvents)
            {
                Package package = new Package(data);
                if (package.packageID == recievedData.type || recievedData.type == 0)
                    recievedData.packageRecieved(data, client);
            }
        }
    }
}
