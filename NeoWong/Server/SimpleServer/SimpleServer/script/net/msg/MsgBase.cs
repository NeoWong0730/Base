using ProtoBuf;
using System;

public abstract class MsgBase : ProtoBuf.IExtensible
{
    public string protoName = "null";

    private static IMsgHelper s_msgHelper = null;

    /// <summary>
    /// 设置网络消息辅助器
    /// </summary>
    /// <param name="msgHelper">网络消息辅助器</param>
    public static void SetMsgHelper(IMsgHelper msgHelper)
    {
        s_msgHelper = msgHelper;
    }

    /// <summary>
    /// 编码
    /// </summary>
    /// <param name="msgBase">消息</param>
    /// <returns>字节数组</returns>
    public static byte[] Encode(MsgBase msgBase)
    {
        return s_msgHelper.Encode(msgBase);
    }

    /// <summary>
    /// 解码
    /// </summary>
    /// <param name="protoName">协议名</param>
    /// <param name="bytes">字节数组</param>
    /// <param name="offset">偏移量</param>
    /// <param name="count">长度</param>
    /// <returns></returns>
    public static MsgBase Decode(string protoName, byte[] bytes, int offset, int count)
    {
        return s_msgHelper.Decode(protoName, bytes, offset, count);
    }

    /// <summary>
    /// 编码协议名（2字节长度+字符串）
    /// </summary>
    public static byte[] EncodeName(MsgBase msgBase)
    {
        //名字bytes和长度
        byte[] nameBytes = System.Text.Encoding.Default.GetBytes(msgBase.protoName);
        Int16 len = (Int16)nameBytes.Length;
        //申请bytes数值
        byte[] bytes = new byte[2 + len];
        //组装2字节的长度信息
        bytes[0] = (byte)(len % 256);
        bytes[1] = (byte)(len / 256);
        //组装名字bytes
        Array.Copy(nameBytes, 0, bytes, 2, len);

        return bytes;
    }

    /// <summary>
    /// 解码协议名（2字节长度+字符串）
    /// </summary>
    public static string DecodeName(byte[] bytes, int offset, out int count)
    {
        count = 0;
        //必须大于2字节
        if (offset + 2 > bytes.Length)
        {
            return "";
        }
        //读取长度
        Int16 len = (Int16)((bytes[offset + 1] << 8) | bytes[offset]);
        //长度必须足够
        if (offset + 2 + len > bytes.Length)
        {
            return "";
        }
        //解析
        count = 2 + len;
        string name = System.Text.Encoding.Default.GetString(bytes, offset + 2, len);
        return name;
    }

    public IExtension GetExtensionObject(bool createIfMissing)
    {
        throw new NotImplementedException();
    }
}