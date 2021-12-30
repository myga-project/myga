using MygaCross;
using System.Collections;
using UnityEngine;

namespace MygaClient
{
    public class MygaTransform : MPAddon
    {
#if UNITY_EDITOR
        [ReadOnly] [SerializeField] private Vector3 syncedPosition = Vector3.zero;
        public Vector3 SyncedPosition => syncedPosition;

        [ReadOnly] [SerializeField] private Quaternion syncedRotation = Quaternion.identity;
        public Quaternion SyncedRotation => syncedRotation;

        [ReadOnly] [SerializeField] private Vector3 syncedScale = Vector3.zero;
        public Vector3 SyncedScale => syncedScale;
#else
        [SerializeField] private Vector3 syncedPosition = Vector3.zero;
        public Vector3 SyncedPosition => syncedPosition;

        [SerializeField] private Quaternion syncedRotation = Quaternion.identity;
        public Quaternion SyncedRotation => syncedRotation;

        [SerializeField] private Vector3 syncedScale = Vector3.zero;
        public Vector3 SyncedScale => syncedScale;
#endif

        private void Start()
        {
            if (!mygaObject.Mine)
                return;

            ApplyPosition(transform.position);
            ApplyRotation(transform.rotation);
            ApplyScale(transform.localScale);

            StartCoroutine(UpdateTransform());
        }

        IEnumerator UpdateTransform()
        {
            yield return new WaitForSeconds(1 / 64);

            if(syncedPosition != transform.position || syncedRotation != transform.rotation || syncedScale != transform.localScale)
            {
                Client.Send(new TransformPackage(mygaObject.ID, transform.position, transform.rotation, transform.localScale));
            }

            StartCoroutine(UpdateTransform());
        }

        public void ApplyTransform(Vector3 _position, Quaternion _rotation, Vector3 _scale)
        {
            ApplyPosition(_position);
            ApplyRotation(_rotation);
            ApplyScale(_scale);
        }

        private void ApplyPosition(Vector3 _position)
        {
            syncedPosition = _position;
            transform.position = _position;
        }

        private void ApplyRotation(Quaternion _rotation)
        {
            syncedRotation = _rotation;
            transform.rotation = _rotation;
        }

        private void ApplyScale(Vector3 _scale)
        {
            syncedScale = _scale;
            transform.localScale = _scale;
        }
    }
}
