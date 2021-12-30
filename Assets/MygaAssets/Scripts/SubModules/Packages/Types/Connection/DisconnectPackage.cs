namespace MygaCross
{
    public class DisconnectPackage : Package
    {
        public DisconnectPackage() : base((int)PackageID.Disconnect) { }
        public DisconnectPackage(byte[] _data) : base(_data) { }
    }
}
