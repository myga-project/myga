using UnityEngine;

namespace MygaServer
{
    public class MygaControl : MonoBehaviour
    {
        public void OnApplicationQuit()
        {
            Server.Stop();
        }
    }
}