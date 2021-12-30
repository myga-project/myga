using MygaCross;
using System.Collections.Generic;
using UnityEngine;

namespace MygaClient 
{
    public class MygaThreading : MonoBehaviour
    {
        public List<byte[]> uncompletedPackages { get; private set; } = new List<byte[]>();
        public List<byte[]> completedPackages { get; private set; } = new List<byte[]>();

        public void AddPackage(byte[] _bytedPackage)
        {
            uncompletedPackages.Add(_bytedPackage);
        }

        void Update()
        {
            List<byte[]> copiedUncompletedPackages = new List<byte[]>();
            lock (uncompletedPackages)
            {
                copiedUncompletedPackages.AddRange(uncompletedPackages);
                uncompletedPackages.Clear();
            }

            foreach (byte[] _bytedPackage in copiedUncompletedPackages)
            {
                ClientEventSystem.PackageRecieved(_bytedPackage);
                completedPackages.Add(_bytedPackage);
                uncompletedPackages.Remove(_bytedPackage);
            }
        }

        private void OnApplicationQuit()
        {
            using(DisconnectPackage _disconnectPackage = new DisconnectPackage())
                Client.Send(_disconnectPackage);
        }
    }
}


