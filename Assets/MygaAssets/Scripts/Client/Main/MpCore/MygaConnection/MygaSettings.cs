using UnityEngine;

namespace MygaClient
{
    public class MygaSettings : MonoBehaviour
    {
        public int tickRate = 64;

        public int SendTickRate => 1 / tickRate;
    }
}