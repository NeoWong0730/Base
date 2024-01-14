public interface IMsgHelper
{
    /// <summary>
    /// 编码
    /// </summary>
    /// <param name="msgBase">消息</param>
    /// <returns>字节数组</returns>
    public byte[] Encode(MsgBase msgBase);

    /// <summary>
    /// 解码
    /// </summary>
    /// <param name="protoName">协议名</param>
    /// <param name="bytes">字节数组</param>
    /// <param name="offset">偏移量</param>
    /// <param name="count">长度</param>
    /// <returns></returns>
    public MsgBase Decode(string protoName, byte[] bytes, int offset, int count);
}
