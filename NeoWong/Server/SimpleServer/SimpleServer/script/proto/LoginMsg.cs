//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

// Generated from: proto/LoginMsg.proto
[global::System.Serializable, global::ProtoBuf.ProtoContract(Name = @"MsgRegisterReq")]
public partial class MsgRegisterReq : MsgBase, global::ProtoBuf.IExtensible
{
    public MsgRegisterReq() { protoName = "MsgRegisterReq"; }

    private string _id = "";
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name = @"id", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string id
    {
        get { return _id; }
        set { _id = value; }
    }
    private string _pw = "";
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name = @"pw", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string pw
    {
        get { return _pw; }
        set { _pw = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
    { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
}

[global::System.Serializable, global::ProtoBuf.ProtoContract(Name = @"MsgRegisterAck")]
public partial class MsgRegisterAck : MsgBase, global::ProtoBuf.IExtensible
{
    public MsgRegisterAck() { protoName = "MsgRegisterAck"; }

    private int _result = default(int);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name = @"result", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int result
    {
        get { return _result; }
        set { _result = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
    { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
}

[global::System.Serializable, global::ProtoBuf.ProtoContract(Name = @"MsgLoginReq")]
public partial class MsgLoginReq : MsgBase, global::ProtoBuf.IExtensible
{
    public MsgLoginReq() { protoName = "MsgLoginReq"; }

    private string _id = "";
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name = @"id", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string id
    {
        get { return _id; }
        set { _id = value; }
    }
    private string _pw = "";
    [global::ProtoBuf.ProtoMember(2, IsRequired = false, Name = @"pw", DataFormat = global::ProtoBuf.DataFormat.Default)]
    [global::System.ComponentModel.DefaultValue("")]
    public string pw
    {
        get { return _pw; }
        set { _pw = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
    { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
}

[global::System.Serializable, global::ProtoBuf.ProtoContract(Name = @"MsgLoginAck")]
public partial class MsgLoginAck : MsgBase, global::ProtoBuf.IExtensible
{
    public MsgLoginAck() { protoName = "MsgLoginAck"; }

    private int _result = default(int);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name = @"result", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int result
    {
        get { return _result; }
        set { _result = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
    { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
}

[global::System.Serializable, global::ProtoBuf.ProtoContract(Name = @"MsgKickNtf")]
public partial class MsgKickNtf : MsgBase, global::ProtoBuf.IExtensible
{
    public MsgKickNtf() { protoName = "MsgKickNtf"; }

    private int _reson = default(int);
    [global::ProtoBuf.ProtoMember(1, IsRequired = false, Name = @"reson", DataFormat = global::ProtoBuf.DataFormat.TwosComplement)]
    [global::System.ComponentModel.DefaultValue(default(int))]
    public int reson
    {
        get { return _reson; }
        set { _reson = value; }
    }
    private global::ProtoBuf.IExtension extensionObject;
    global::ProtoBuf.IExtension global::ProtoBuf.IExtensible.GetExtensionObject(bool createIfMissing)
    { return global::ProtoBuf.Extensible.GetExtensionObject(ref extensionObject, createIfMissing); }
}