using ProtoBuf;

public class MsgProtobufHelper : IMsgHelper
{
    public MsgBase Decode(string protoName, byte[] bytes, int offset, int count)
    {
        using (var memory = new System.IO.MemoryStream(bytes, offset, count))
        {
            System.Type t = System.Type.GetType(protoName);
            MsgBase msgBase = Serializer.NonGeneric.Deserialize(t, memory) as MsgBase;
            return msgBase;
        }
    }

    public byte[] Encode(MsgBase msgBase)
    {
        using (var memory = new System.IO.MemoryStream())
        {
            Serializer.Serialize(memory, msgBase);
            return memory.ToArray();
        }
    }
}
