using System;
using System.Collections.Generic;
using System.IO;
using Packet;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Logic
{
    public enum ETargetType
    {
        Int = 0,
        UInt = 1,
        Float = 2,
        Long = 3,
        Double = 4,
        Array = 5,
        Class = 6,

    }

    public class MakeTarge
    {
        public static byte Targe(uint id, ETargetType type)
        {
           uint targe =  id << 4 | (uint)type;

            return (byte)targe;
        }

        public static uint UnTargeID(byte value)
        {
            uint targe = (uint)(value >> 4); 

            return targe;
        }
    }
    public class ComputorSize
    {
        private const int LittleEndian64Size = 8;
        private const int LittleEndian32Size = 4;

        /// <summary>
        /// Computes the number of bytes that would be needed to encode a
        /// double field, including the tag.
        /// </summary>
        public static int ComputeDoubleSize(double value)
        {
            return LittleEndian64Size;
        }

        /// <summary>
        /// Computes the number of bytes that would be needed to encode a
        /// float field, including the tag.
        /// </summary>
        public static int ComputeFloatSize(float value)
        {
            return LittleEndian32Size;
        }

        /// <summary>
        /// Computes the number of bytes that would be needed to encode a
        /// uint64 field, including the tag.
        /// </summary>
        public static int ComputeUInt64Size(ulong value)
        {
            return ComputeRawVarint64Size(value);
        }

        /// <summary>
        /// Computes the number of bytes that would be needed to encode an
        /// int64 field, including the tag.
        /// </summary>
        public static int ComputeInt64Size(long value)
        {
            return ComputeRawVarint64Size((ulong)value);
        }

        /// <summary>
        /// Computes the number of bytes that would be needed to encode an
        /// int32 field, including the tag.
        /// </summary>
        public static int ComputeInt32Size(int value)
        {
            //if (value >= 0)
            //{
            //    return ComputeRawVarint32Size((uint)value);
            //}
            //else
            //{
            //    // Must sign-extend.
            //    return 10;
            //}
            return 4;
        }

        /// <summary>
        /// Computes the number of bytes that would be needed to encode a
        /// fixed64 field, including the tag.
        /// </summary>
        public static int ComputeFixed64Size(ulong value)
        {
            return LittleEndian64Size;
        }

        /// <summary>
        /// Computes the number of bytes that would be needed to encode a
        /// fixed32 field, including the tag.
        /// </summary>
        public static int ComputeFixed32Size(uint value)
        {
            return LittleEndian32Size;
        }

        /// <summary>
        /// Computes the number of bytes that would be needed to encode a
        /// bool field, including the tag.
        /// </summary>
        public static int ComputeBoolSize(bool value)
        {
            return 1;
        }

        /// <summary>
        /// Computes the number of bytes that would be needed to encode a
        /// string field, including the tag.
        /// </summary>
        public static int ComputeStringSize(String value)
        {
            int byteArraySize = Encoding.UTF8.GetByteCount(value);
            return ComputeLengthSize(byteArraySize) + byteArraySize;
        }


        /// <summary>
        /// Computes the number of bytes that would be needed to encode an
        /// embedded message field, including the tag.
        /// </summary>
        public static int ComputeMessageSize(ISerialize value)
        {
            int size = value.CalcSize();
            return ComputeLengthSize(size) + size;
        }

  

        /// <summary>
        /// Computes the number of bytes that would be needed to encode a
        /// uint32 field, including the tag.
        /// </summary>
        public static int ComputeUInt32Size(uint value)
        {
            return ComputeRawVarint32Size(value);
        }

        /// <summary>
        /// Computes the number of bytes that would be needed to encode a
        /// enum field, including the tag. The caller is responsible for
        /// converting the enum value to its numeric value.
        /// </summary>
        public static int ComputeEnumSize(int value)
        {
            // Currently just a pass-through, but it's nice to separate it logically.
            return ComputeInt32Size(value);
        }

        /// <summary>
        /// Computes the number of bytes that would be needed to encode an
        /// sfixed32 field, including the tag.
        /// </summary>
        public static int ComputeSFixed32Size(int value)
        {
            return LittleEndian32Size;
        }

        /// <summary>
        /// Computes the number of bytes that would be needed to encode an
        /// sfixed64 field, including the tag.
        /// </summary>
        public static int ComputeSFixed64Size(long value)
        {
            return LittleEndian64Size;
        }


        internal static uint EncodeZigZag32(int n)
        {
            // Note:  the right-shift must be arithmetic
            return (uint)((n << 1) ^ (n >> 31));
        }

        /// <summary>
        /// Encode a 64-bit value with ZigZag encoding.
        /// </summary>
        /// <remarks>
        /// ZigZag encodes signed integers into values that can be efficiently
        /// encoded with varint.  (Otherwise, negative values must be 
        /// sign-extended to 64 bits to be varint encoded, thus always taking
        /// 10 bytes on the wire.)
        /// </remarks>
        internal static ulong EncodeZigZag64(long n)
        {
            return (ulong)((n << 1) ^ (n >> 63));
        }

        /// <summary>
        /// Computes the number of bytes that would be needed to encode an
        /// sint32 field, including the tag.
        /// </summary>
        public static int ComputeSInt32Size(int value)
        {
            return ComputeRawVarint32Size(EncodeZigZag32(value));
        }

        /// <summary>
        /// Computes the number of bytes that would be needed to encode an
        /// sint64 field, including the tag.
        /// </summary>
        public static int ComputeSInt64Size(long value)
        {
            return ComputeRawVarint64Size(EncodeZigZag64(value));
        }

        /// <summary>
        /// Computes the number of bytes that would be needed to encode a length,
        /// as written by <see cref="WriteLength"/>.
        /// </summary>
        public static int ComputeLengthSize(int length)
        {
            return ComputeRawVarint32Size((uint)length);
        }


        public static int ComputiListSize<T>(List<T> value, Func<T,int > sizeFunc)
        {
            int size = 0;

            size += ComputeLengthSize(value.Count);

            for (int i = 0; i < value.Count; i++)
            {
                size += sizeFunc(value[i]);
            }

            return size;
        }
        /// <summary>
        /// Computes the number of bytes that would be needed to encode a varint.
        /// </summary>
        public static int ComputeRawVarint32Size(uint value)
        {
            //if ((value & (0xffffffff << 7)) == 0)
            //{
            //    return 1;
            //}
            //if ((value & (0xffffffff << 14)) == 0)
            //{
            //    return 2;
            //}
            //if ((value & (0xffffffff << 21)) == 0)
            //{
            //    return 3;
            //}
            //if ((value & (0xffffffff << 28)) == 0)
            //{
            //    return 4;
            //}
            //return 5;

            return sizeof(uint);
        }

        /// <summary>
        /// Computes the number of bytes that would be needed to encode a varint.
        /// </summary>
        public static int ComputeRawVarint64Size(ulong value)
        {
            //if ((value & (0xffffffffffffffffL << 7)) == 0)
            //{
            //    return 1;
            //}
            //if ((value & (0xffffffffffffffffL << 14)) == 0)
            //{
            //    return 2;
            //}
            //if ((value & (0xffffffffffffffffL << 21)) == 0)
            //{
            //    return 3;
            //}
            //if ((value & (0xffffffffffffffffL << 28)) == 0)
            //{
            //    return 4;
            //}
            //if ((value & (0xffffffffffffffffL << 35)) == 0)
            //{
            //    return 5;
            //}
            //if ((value & (0xffffffffffffffffL << 42)) == 0)
            //{
            //    return 6;
            //}
            //if ((value & (0xffffffffffffffffL << 49)) == 0)
            //{
            //    return 7;
            //}
            //if ((value & (0xffffffffffffffffL << 56)) == 0)
            //{
            //    return 8;
            //}
            //if ((value & (0xffffffffffffffffL << 63)) == 0)
            //{
            //    return 9;
            //}
            //return 10;

            return sizeof(ulong);
        }

        /// <summary>
        /// Computes the number of bytes that would be needed to encode a tag.
        /// </summary>
        //public static int ComputeTagSize(int fieldNumber)
        //{
        //    //return ComputeRawVarint32Size(WireFormat.MakeTag(fieldNumber, 0));
        //}

    }

   
}
