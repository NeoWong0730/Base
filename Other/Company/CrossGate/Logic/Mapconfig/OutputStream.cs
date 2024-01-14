using System;
using System.Collections.Generic;
using Google.Protobuf;
using System.IO;
using Packet;
using System.Runtime.Serialization.Formatters.Binary;

namespace Logic
{
    public class OutputStream
    {
        public StreamWriter mWriter;
        public Stream mStream;
        public void WriteList<T>(List<T> value,Action<T> func)
        {
            int count = value.Count;
            WriteInt(count);

            for (int i = 0; i < value.Count; i++)
            {

                func?.Invoke(value[i]);
            }
        }

        public void WriteTag(byte value)
        {
            WriteByte(value);
        }
        public void WriteUInt(uint value)
        {
            byte[] bytes = BitConverter.GetBytes(value);
            WriteBytes(bytes);
        }
        public void WriteInt(int value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            WriteBytes(bytes);
        }

        public void WriteFloat(float value)
        {
            byte[] bytes = BitConverter.GetBytes(value);

            WriteBytes(bytes);
        }

        public void WriteBool(bool value)
        {
            byte bytes = (byte)(value ? 1: 0 );

            WriteByte(bytes);
        }

        public void WriteISerialize(ISerialize value)
        {
            int length = value.CalcSize();

            WriteInt(length);

            value.Serialize(this);
        }


        public void WriteBytes(byte[] bytes)
        {
            mStream.Write(bytes, 0, bytes.Length);
        }

        public void WriteByte(byte value)
        {
            mStream.WriteByte(value);
        }
    }

   
}
