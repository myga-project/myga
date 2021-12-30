namespace MygaCross
{
    public class ErrorPackage : Package
    {
        public string message = string.Empty;
        public bool disconnect = false;

        public ErrorPackage(string _message, bool _disconnect = false) : base((int)PackageID.Error)
        {
            message = _message;
            disconnect = _disconnect;
            Write(_message);
            Write(_disconnect);
        }

        public ErrorPackage(byte[] _data) : base(_data)
        {
            message = ReadString();
            disconnect = ReadBool();
        }

        public override string ToString()
        {
            return message;
        }
    }
}
