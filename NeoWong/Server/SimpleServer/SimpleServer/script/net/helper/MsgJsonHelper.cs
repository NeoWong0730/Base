using System;

public class MsgJsonHelper : IMsgHelper
{
    /// <summary>
    /// 编码
    /// </summary>
    /// <param name="msgBase">消息</param>
    /// <returns>字节数组</returns>
    public byte[] Encode(MsgBase msgBase)
    {
        string s = Newtonsoft.Json.JsonConvert.SerializeObject(msgBase);
        return System.Text.Encoding.UTF8.GetBytes(s);
    }

    /// <summary>
    /// 解码
    /// </summary>
    /// <param name="protoName">协议名</param>
    /// <param name="bytes">字节数组</param>
    /// <param name="offset">偏移量</param>
    /// <param name="count">长度</param>
    /// <returns></returns>
    public MsgBase Decode(string protoName, byte[] bytes, int offset, int count)
    {
        string s = System.Text.Encoding.UTF8.GetString(bytes, offset, count);
        MsgBase msgBase = (MsgBase)Newtonsoft.Json.JsonConvert.DeserializeObject(s, Type.GetType(protoName));
        return msgBase;
    }
}
