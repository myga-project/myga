using System.Collections.Generic;
using UnityEngine;


namespace MygaClient
{
    public static class MygaNetwork
    {
        public static MygaThreading mygaConnection;
        public static MygaSettings mygaSettings;

        public static List<MygaObject> mpObjects = new List<MygaObject>();

        public static MygaObject FindObjectByID(int _id)
        {
            foreach(MygaObject _object in mpObjects)
            {
                if (_object.ID == _id)
                    return _object;
            }

            return null;
        }

        public static MygaObject FindObjectByCID(int _clientID)
        {
            foreach (MygaObject _object in mpObjects)
            {
                if (_object.ClientID != null && _object.ClientID == _clientID)
                    return _object;
            }

            return null;
        }

        public static void Connect(string ip, int port)
        {
            Client.Connect(ip, port);
            AddNetworkObject();
        }

        public static void AddNetworkObject()
        {
            if (mygaConnection != null)
                return;

            GameObject connection = GameObject.Instantiate(new GameObject("MygaConnection"));
            mygaConnection = connection.AddComponent<MygaThreading>();
            mygaSettings = connection.AddComponent<MygaSettings>();
        }
    }
}
