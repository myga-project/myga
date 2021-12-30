using System.Collections.Generic;
using UnityEngine;

namespace MygaClient
{
    public class MygaObject : MonoBehaviour
    {
        [SerializeField] private int id = 0;
        public int ID => id;

        [SerializeField] private int? clientID = null;
        public int? ClientID => clientID;

        [SerializeField] private bool mine = false;
        public bool Mine => mine;

        public List<MPAddon> addons { get; private set; } = new List<MPAddon>();

        private void Start()
        {
            MygaNetwork.mpObjects.Add(this);
        }

        public void Initialize(int _id, int _clientID = -1, bool _mine = false)
        {

            id = _id;
            if(_clientID > -1)
                clientID = _clientID;

            mine = _mine;
        }

        public void AddAddon(MPAddon addon)
        {
            addons.Add(addon);
        }

        public T GetAddon<T>() where T: MPAddon
        {
            foreach (MPAddon addon in addons)
                if (addon is T)
                    return (T)addon;

            return default(T);
        }

        public bool HasAddon<T>() where T : MPAddon
        {
            foreach (MPAddon addon in addons)
                if (addon is T)
                    return true;

            return false;
        }
    }
}


