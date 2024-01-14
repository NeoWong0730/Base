using Google.Protobuf;
using System;
using System.IO;

namespace Net
{
    public struct NetMsg
    {
        public UInt16 evtId;
        public UInt16 bodyLength;        
        public IMessage data;
        internal MemoryStream stream;
    }

    public class NetMessageReceiveBuffer
    {
        public byte[] mTemp = new byte[2];

        private bool haveHead;
        public UInt16 evtId;
        public UInt16 bodyLength;
        public MemoryStream data = new MemoryStream();

        public int WriteByte(byte[] buffer, int offset, int count)
        {
            int readCount = 0;

            if (haveHead == false)
            {
                int read = (int)NetClient.HEADER_LENGTH - (int)data.Length;
                read = read <= count ? read : count;

                data.Write(buffer, offset, read);
                readCount += read;

                if (data.Length >= NetClient.HEADER_LENGTH)
                {
                    data.Position = 0;

                    data.Read(mTemp, 0, 2);
                    // protoId + msgdata
                    bodyLength = NetMsgUtil.ReadUInt16(mTemp, 0, true);
                    bodyLength -= 2;

                    data.Read(mTemp, 0, 2);
                    
                    data.Read(mTemp, 0, 2);
                    evtId = NetMsgUtil.ReadUInt16(mTemp, 0, true);

                    data.Position = 0;
                    data.SetLength(0);
                    haveHead = true;
                }
            }

            if (haveHead)
            {
                int read = (int)bodyLength - (int)data.Length;
                read = read <= count - readCount ? read : count - readCount;

                if(read > 0)
                {
                    data.Write(buffer, offset + readCount, read);
                }                
                readCount += read;
            }

            return readCount;
        }

        public bool CheckComplete()
        {
            return haveHead && (int)bodyLength <= (int)data.Length;
        }

        public void Reset()
        {
            data.Position = 0;
            data.SetLength(0);
            haveHead = false;
        }
    }
}
