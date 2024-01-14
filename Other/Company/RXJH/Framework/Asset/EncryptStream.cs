using System.IO;

namespace Lib.AssetLoader
{
    public class EncryptFileStream : FileStream
    {
        public EncryptFileStream(string path, FileMode mode) : base(path, mode)
        {

        }

        public EncryptFileStream(string path, FileMode mode, FileAccess access) : base(path, mode, access)
        {

        }

        public EncryptFileStream(string path, FileMode mode, FileAccess access, FileShare share, int bufferSize, bool useAsync)
            : base(path, mode, access, share, bufferSize, useAsync)
        {

        }

        public override int ReadByte()
        {
            int index = base.ReadByte();
            index ^= AssetMananger.key;
            return index;
        }

        public override int Read(byte[] array, int offset, int count)
        {
            int index = base.Read(array, offset, count);
            for (int i = 0; i < array.Length; ++i)
            {
                array[i] ^= AssetMananger.key;
            }
            return index;
        }

        public override void WriteByte(byte value)
        {
            value ^= AssetMananger.key;
            base.WriteByte(value);
        }

        public override void Write(byte[] array, int offset, int count)
        {
            for (int i = 0; i < array.Length; ++i)
            {
                array[i] ^= AssetMananger.key;
            }
            base.Write(array, offset, count);
        }
    }

    public class EncryptMemoryStream : MemoryStream
    {
        public EncryptMemoryStream(byte[] buffer) : base(buffer)
        {

        }

        public override int ReadByte()
        {
            int index = base.ReadByte();
            index ^= AssetMananger.key;
            return index;
        }

        public override int Read(byte[] array, int offset, int count)
        {
            int index = base.Read(array, offset, count);
            for (int i = 0; i < array.Length; ++i)
            {
                array[i] ^= AssetMananger.key;
            }
            return index;
        }

        public override void WriteByte(byte value)
        {
            value ^= AssetMananger.key;
            base.WriteByte(value);
        }

        public override void Write(byte[] array, int offset, int count)
        {
            for (int i = 0; i < array.Length; ++i)
            {
                array[i] ^= AssetMananger.key;
            }
            base.Write(array, offset, count);
        }
    }
}