using Unity.Collections;
using Unity.Netcode;

namespace Gerallt
{
    public struct NetworkableString : INetworkSerializable
    {
        private FixedString32Bytes @string;
        
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref @string);
        }

        public override string ToString()
        {
            return @string.ToString();
        }

        public static implicit operator string(NetworkableString s) => s.ToString();
        public static implicit operator NetworkableString(string s) => new NetworkableString() { @string = new FixedString32Bytes(s) };
    }
}