//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: proto/SysMsg.proto
[global::System.Serializable, global::ProtoBuf.ProtoContract(Name = @"MsgPing")]
public partial class MsgPing : MsgBase, global::ProtoBuf.IExtensible
{
    public MsgPing() { protoName = "MsgPing"; }

    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
    { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
}

[global::System.Serializable, global::ProtoBuf.ProtoContract(Name = @"MsgPong")]
public partial class MsgPong : MsgBase, global::ProtoBuf.IExtensible
{
    public MsgPong() { protoName = "MsgPong"; }

    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
    { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
}