
using System.Collections.Generic;
using System;
using System.IO;

namespace Net
{
    public class Packet
    {
        //1字节key,加密用
        public const int HEADER_KEY = 1;

        //3字节长度
        public const int HEADER_LENGTH = 3;

        //2字节消息号
        public const int HEADER_CMD_LENGTH = 2;

        //�̶����ȵ��м�����
        private byte[] TEMP_BYTE_ARRAY = null;

        //��ǰ���鳤��
        private int CURRENT_LENGTH = 0;

        //��ǰPopָ��λ��
        private int CURRENT_POSITION = 0;

 
        public Packet(int length)
        {
            CURRENT_LENGTH = length;
            this.Initialize();        
        }

        public Packet(byte[] bytes, int length)
        {
            CURRENT_LENGTH = length;
            this.Initialize();
            Array.Copy(bytes, 0, TEMP_BYTE_ARRAY, 0, length);
        }

        public void Initialize()
        {
            TEMP_BYTE_ARRAY = new byte[CURRENT_LENGTH];
            TEMP_BYTE_ARRAY.Initialize();
            CURRENT_POSITION = 0;
        }

        public int Length
        {
            get
            {
                return CURRENT_LENGTH;
            }
        }

        public int Position
        {
            get
            {
                return CURRENT_POSITION;
            }
            set
            {
                CURRENT_POSITION = value;
            }
        }

        public byte[] ByteArray()
        {
            return TEMP_BYTE_ARRAY;
        }

        public bool CheckBytesLeft(int inLenLeft)
        {
            if (CURRENT_LENGTH - CURRENT_POSITION < inLenLeft)
            {
                return false;
            }
            return true;
        }

        public bool WatchUInt32(ref UInt32 val, int pos)
        {
            if (CURRENT_LENGTH - pos < 4)
            {
                return false;
            }
            val = (UInt32)(TEMP_BYTE_ARRAY[pos] << 24 | TEMP_BYTE_ARRAY[pos + 1] << 16 | TEMP_BYTE_ARRAY[pos + 2] << 8 | TEMP_BYTE_ARRAY[pos + 3]);
            return true;
        }

        public bool WatchUInt16(ref UInt16 val, int pos)
        {
            if (CURRENT_LENGTH - pos < 2)
            {
                return false;
            }
            val = (UInt16)(TEMP_BYTE_ARRAY[pos] << 8 | TEMP_BYTE_ARRAY[pos + 1]);
            return true;
        }

        public bool PushByte(byte by)
        {
            if (!CheckBytesLeft(1)) return false;

            TEMP_BYTE_ARRAY[CURRENT_POSITION++] = by;
            return true;
        }

        public bool PushUInt16(UInt16 Num)
        {
            if (!CheckBytesLeft(2)) return false;

            TEMP_BYTE_ARRAY[CURRENT_POSITION++] = (byte)(((Num & 0xff00) >> 8) & 0xff);
            TEMP_BYTE_ARRAY[CURRENT_POSITION++] = (byte)((Num & 0x00ff) & 0xff);
            return true;
        }

        public bool PushUInt32(UInt32 Num)
        {
            if (!CheckBytesLeft(4)) return false;

            TEMP_BYTE_ARRAY[CURRENT_POSITION++] = (byte)(((Num & 0xff000000) >> 24) & 0xff);
            TEMP_BYTE_ARRAY[CURRENT_POSITION++] = (byte)(((Num & 0x00ff0000) >> 16) & 0xff);
            TEMP_BYTE_ARRAY[CURRENT_POSITION++] = (byte)(((Num & 0x0000ff00) >> 8) & 0xff);
            TEMP_BYTE_ARRAY[CURRENT_POSITION++] = (byte)((Num & 0x000000ff) & 0xff);
            return true;
        }

        public bool PushUInt64(UInt64 Num)
        {
            if (!CheckBytesLeft(8)) return false;

            UInt32 iHighValue = (UInt32)(Num >> 32);
            UInt32 iLowValue = (UInt32)Num;
            PushUInt32(iHighValue);
            PushUInt32(iLowValue);
            return true;
        }

        public bool PushByteArray(byte[] buf, int length)
        {
            if (!CheckBytesLeft(length)) return false;

            Array.Copy(buf, 0, TEMP_BYTE_ARRAY, CURRENT_POSITION, length);
            CURRENT_POSITION += length;
            return true;
        }

        public bool PopByte(ref byte val)
        {
            if (!CheckBytesLeft(1)) return false;

            val = TEMP_BYTE_ARRAY[CURRENT_POSITION++];
            return true;
        }

        public bool PopUInt16(ref UInt16 val)
        {
            if (!CheckBytesLeft(2)) return false;

            val = (UInt16)(TEMP_BYTE_ARRAY[CURRENT_POSITION] << 8 | TEMP_BYTE_ARRAY[CURRENT_POSITION + 1]);
            CURRENT_POSITION += 2;
            return true;
        }

        public bool PopUInt32(ref UInt32 val)
        {
            if (!CheckBytesLeft(4)) return false;

            val = (UInt32)(TEMP_BYTE_ARRAY[CURRENT_POSITION] << 24 | TEMP_BYTE_ARRAY[CURRENT_POSITION + 1] << 16 | TEMP_BYTE_ARRAY[CURRENT_POSITION + 2] << 8 | TEMP_BYTE_ARRAY[CURRENT_POSITION + 3]);
            CURRENT_POSITION += 4;

            return true;
        }

        public bool PopUInt64(ref UInt64 val)
        {
            if (!CheckBytesLeft(8)) return false;

            UInt32 iHighValue = 0;
            UInt32 iLowValue = 0;
            PopUInt32(ref iHighValue);
            PopUInt32(ref iLowValue);

            UInt64 iTemp = iHighValue;
            val = (iTemp << 32) + iLowValue;
            return true;
        }

        public bool PopByteArray(ref byte[] val, int len)
        {
            if (!CheckBytesLeft(len)) return false;

            Array.Copy(TEMP_BYTE_ARRAY, CURRENT_POSITION, val, 0, len);
            CURRENT_POSITION += len;
            return true;
        }
    }
}