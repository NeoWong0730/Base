
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using Luban;


namespace cfg
{
public sealed partial class UISound : Luban.BeanBase
{
    public UISound(ByteBuf _buf) 
    {
        Id = _buf.ReadInt();
        AssetName = _buf.ReadString();
        Priority = _buf.ReadInt();
        Volume = _buf.ReadFloat();
    }

    public static UISound DeserializeUISound(ByteBuf _buf)
    {
        return new UISound(_buf);
    }

    /// <summary>
    /// 声音编号
    /// </summary>
    public readonly int Id;
    /// <summary>
    /// 资源名称
    /// </summary>
    public readonly string AssetName;
    /// <summary>
    /// 优先级（默认0，128最高，-128最低）
    /// </summary>
    public readonly int Priority;
    /// <summary>
    /// 音量（0~1）
    /// </summary>
    public readonly float Volume;
   
    public const int __ID__ = 298404571;
    public override int GetTypeId() => __ID__;

    public  void ResolveRef(Tables tables)
    {
        
        
        
        
    }

    public override string ToString()
    {
        return "{ "
        + "Id:" + Id + ","
        + "AssetName:" + AssetName + ","
        + "Priority:" + Priority + ","
        + "Volume:" + Volume + ","
        + "}";
    }
}

}
