using Google.Protobuf;
using System;
using System.IO;
using UnityEngine;

namespace Net
{
    public static class NetMsgUtil
    {        
        public static byte CryptKey = 0;
        public static bool IsPrint = false;
        public static ByteString ToByteString(IMessage message)
        {
            return message.ToByteString();
        }

        public static byte[] Serialzie(IMessage message)
        {
            byte[] data;
            using (MemoryStream stream = new MemoryStream())
            {
                message.WriteTo(stream);
                data = stream.ToArray();
            }
            return data;
        }

        public static int Serialzie(IMessage message, byte[] buffer)
        {
            int length = 0;
            using (MemoryStream stream = new MemoryStream())
            {
                message.WriteTo(stream);
                stream.Position = 0;
                stream.Read(buffer, 0, (int)stream.Length);
                length = (int)stream.Length;
            }
            return length;            
        }

        public static void Serialzie(IMessage message, MemoryStream stream)
        {
            message.WriteTo(stream);
        }

        public static T Deserialize<T>(MessageParser _type, NetMsg msg) where T : class, IMessage
        {
            T rlt = msg.data as T;
            return rlt;
        }

        public static bool TryDeserialize(MessageParser _type, MemoryStream data, out IMessage rlt)
        {
            bool isSuccess = true;
            try
            {
                rlt = _type.ParseFrom(data);
            }
            catch (Exception e)
            {
                rlt = null;
                isSuccess = false;
                Lib.Core.DebugUtil.LogException(e);
            }            
            return isSuccess;
        }

        public static bool TryDeserialize<T>(MessageParser _type, byte[] data, out T rlt) where T : class, IMessage
        {
            bool isSuccess = true;
            try
            {
                rlt = _type.ParseFrom(data) as T;
            }
            catch (Exception e)
            {
                rlt = null;
                isSuccess = false;
                Lib.Core.DebugUtil.LogException(e);
            }
            return isSuccess;
        }

        public static bool TryDeserialize<T>(MessageParser _type, ByteString data, out T rlt) where T : class, IMessage 
        {
            bool isSuccess = true;
            try
            {
                rlt = _type.ParseFrom(data) as T;
            }
            catch (Exception e)
            {
                rlt = null;
                isSuccess = false;
                Lib.Core.DebugUtil.LogException(e);
            }
            return isSuccess;
        }

        public static bool TryDeserialize<T>(MessageParser _type, byte[] data, int count, out T rlt) where T : class, IMessage
        {
            bool isSuccess = true;
            using (MemoryStream stream = new MemoryStream(data, 0, count))
            {
                try
                {
                    rlt = _type.ParseFrom(stream) as T;
                }
                catch (Exception e)
                {
                    rlt = null;
                    isSuccess = false;
                    Lib.Core.DebugUtil.LogException(e);
                }
            }

            return isSuccess;
        }
        
        private static byte RandKey()
        {
            System.Random rd = new System.Random();
            return (byte)((rd.Next(1, 65535) % 252) + 3); //初始值
        }
        
        public static UInt16 ReadUInt16(byte[] data, int pos, bool revert = false)
        {
            if (!revert) {
                // 大端读取
                return (UInt16)(data[pos] << 8 | data[pos + 1]);
            }
            // 小端读取
            return (UInt16)(data[pos + 1] << 8 | data[pos]);
        }
        public static Int16 ReadInt16(byte[] data, int pos, bool revert = false)
        {
            if (!revert) {
                return (Int16)(data[pos] << 8 | data[pos + 1]);
            }
            return (Int16)(data[pos + 1] << 8 | data[pos]);
        }

        #region 协议号组装, 可以设计成客户端/服务器公用的底层c形式的dll
        /*union UU {
            uint16_t u16;
            struct ST
            {
                uint16_t first : 10;
                uint16_t second : 6;
            }st;
        };

        uint16_t GetU16(uint16_t first, uint16_t second) {
            UU u;
            u.st.first = first;
            u.st.second = second;
            return u.u16;
        }

        void GetFirstSecond(uint16_t u16, uint16_t& first, uint16_t& second) {
            UU u;
            u.u16 = u16;
            first = u.st.first;
            second = u.st.second;
        }*/

        public static void CalFirstSecond(ushort input, out ushort first, out ushort second) {
            first = (ushort)(input & 0x3FF);
            second = (ushort)(input >> 10);
        }
        
        public static ushort CalMsgCode(ushort first, ushort second) {
            var ret = 0;
            if (!BitConverter.IsLittleEndian) {
                ret = first << 6;
                ret |= second;
            }
            else {
                ret = second << 10;
                ret |= first;
            }

            return (ushort)ret;
        }
        #endregion
    }
}
