using System;
using System.Collections.Generic;
using Google.Protobuf;
using System.IO;
using Packet;
using System.Runtime.Serialization.Formatters.Binary;

namespace Logic
{
    public class InputStream
    {
        Stream mStream;

        long mPosition;
        long mBufferSize;

        long mBufferSizeLimite;

        Stack<long> mLimitstack = new Stack<long>();

        long Position { get { return mStream.Position; } set { mStream.Position = value; } }
        public InputStream(Stream stream)
        {
            mStream = stream;
            mStream.Position = 0;

            mPosition = 0;
            mBufferSize = mStream.Length;

            mLimitstack.Push(mStream.Length - 1);
        }
        public byte ReadTag()
        {
            long limitePosition = LimitePosition();

            if (Position >= limitePosition)
                return 0;

            return ReadByte();
        }

        public uint ReadUInt()
        {
            byte[] bytes = ReadBytes(4);

            uint value = BitConverter.ToUInt32(bytes,0);

            return value;
        }

        public int ReadInt()
        {

            byte[] bytes = ReadBytes(4);

            int value = BitConverter.ToInt32(bytes, 0);

            return value;
        }

        public float ReadFloat()
        {

            byte[] bytes = ReadBytes(4);

            float value = BitConverter.ToSingle(bytes, 0);

            return value;
        }

        public bool ReadBool()
        {
            byte value = ReadByte();

            return value == 0 ? false : true;
        }

        public void ReadList<T>(List<T> value, Action<T> readFunc) where T : ISerialize,new()
        {
            int count =  ReadLength();

            for (int i = 0; i < count; i++)
            {
                T item = new T();

                ReadSerialize(item);

                value.Add(item);
            }
        }

        public void ReadList<T>(List<T> value, Func<T> readFunc)
        {
            int count = ReadLength();

            for (int i = 0; i < count; i++)
            {
                value.Add(readFunc());
            }
        }
        public int ReadLength()
        {
            return ReadInt();
        }

        
        public void ReadSerialize(ISerialize value)
        {
            int length = ReadLength();

            PushLimite(Position + length);

            value.DeSerialize(this);

            PopLimite();

        }

        private void PushLimite(long value)
        {
            mLimitstack.Push(value);
        }

        private void PopLimite()
        {
            mLimitstack.Pop();
        }


        private long LimitePosition()
        {
            return mLimitstack.Peek();
        }
        public byte ReadByte()
        {
            return (byte)mStream.ReadByte();
        }

        public byte[] ReadBytes(int count)
        {
            byte[] bytes = new byte[count];

            int readCount = mStream.Read(bytes, 0, count);

            return bytes;
        }
    }

   
}
