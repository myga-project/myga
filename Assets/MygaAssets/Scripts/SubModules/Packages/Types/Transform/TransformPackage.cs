using UnityEngine;

namespace MygaCross
{
    public class TransformPackage : Package
    {
        public int id = 0;
        public Vector3 position = Vector3.zero;
        public Quaternion rotation = Quaternion.identity;
        public Vector3 scale = Vector3.zero;

        public TransformPackage(int _id, Vector3 _position, Quaternion _rotation, Vector3 _scale) : base((int)PackageID.Transform)
        {
            Write(_id);
            Write(_position);
            Write(_rotation);
            Write(_scale);
        }

        public TransformPackage(byte[] _data) : base(_data)
        {
            id = ReadInt();
            position = ReadVector3();
            rotation = ReadQuaternion();
            scale = ReadVector3();
        }
    }
}
