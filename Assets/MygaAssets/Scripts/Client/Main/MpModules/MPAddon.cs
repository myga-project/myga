using UnityEngine;

namespace MygaClient
{
    public class MPAddon : MonoBehaviour
    {
        protected MygaObject mygaObject = null;

        private void Awake()
        {
            if(TryGetComponent(out MygaObject mygaObject))
            {
                mygaObject.AddAddon(this);
                this.mygaObject = mygaObject;
            }
        }
    }
}
