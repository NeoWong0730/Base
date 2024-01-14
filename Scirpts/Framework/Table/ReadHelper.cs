using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Framework
{
    public static class ReadHelper
    {
        #region Binary Parse
        public delegate T ReadFunc<T>(BinaryReader br);

        public static sbyte ReadSByte(BinaryReader br)
        {
            return br.ReadSByte();
        }
        public static List<sbyte> ReadArray_ReadSByte(BinaryReader br)
        {
            return ReadHelper.ReadArray<sbyte>(br, ReadHelper.ReadSByte);
        }
        public static List<List<sbyte>> ReadArray2_ReadSByte(BinaryReader br)
        {
            return ReadHelper.ReadArray2<sbyte>(br, ReadHelper.ReadSByte);
        }

        public static byte ReadByte(BinaryReader br)
        {
            return br.ReadByte();
        }
        public static List<byte> ReadArray_ReadByte(BinaryReader br)
        {
            return ReadHelper.ReadArray<byte>(br, ReadHelper.ReadByte);
        }
        public static List<List<byte>> ReadArray2_ReadByte(BinaryReader br)
        {
            return ReadHelper.ReadArray2<byte>(br, ReadHelper.ReadByte);
        }

        public static short ReadShort(BinaryReader br)
        {
            return br.ReadInt16();
        }
        public static List<short> ReadArray_ReadShort(BinaryReader br)
        {
            return ReadHelper.ReadArray<short>(br, ReadHelper.ReadShort);
        }
        public static List<List<short>> ReadArray2_ReadShort(BinaryReader br)
        {
            return ReadHelper.ReadArray2<short>(br, ReadHelper.ReadShort);
        }

        public static ushort ReadUShort(BinaryReader br)
        {
            return br.ReadUInt16();
        }
        public static List<ushort> ReadArray_ReadUShort(BinaryReader br)
        {
            return ReadHelper.ReadArray<ushort>(br, ReadHelper.ReadUShort);
        }
        public static List<List<ushort>> ReadArray2_ReadUShort(BinaryReader br)
        {
            return ReadHelper.ReadArray2<ushort>(br, ReadHelper.ReadUShort);
        }

        public static int ReadInt(BinaryReader br)
        {
            return br.ReadInt32();
        }
        public static List<int> ReadArray_ReadInt(BinaryReader br)
        {
            return ReadHelper.ReadArray<int>(br, ReadHelper.ReadInt);
        }
        public static List<List<int>> ReadArray2_ReadInt(BinaryReader br)
        {
            return ReadHelper.ReadArray2<int>(br, ReadHelper.ReadInt);
        }

        public static uint ReadUInt(BinaryReader br)
        {
            return br.ReadUInt32();
        }
        public static List<uint> ReadArray_ReadUInt(BinaryReader br)
        {
            return ReadHelper.ReadArray<uint>(br, ReadHelper.ReadUInt);
        }
        public static List<List<uint>> ReadArray2_ReadUInt(BinaryReader br)
        {
            return ReadHelper.ReadArray2<uint>(br, ReadHelper.ReadUInt);
        }

        public static long ReadInt64(BinaryReader br)
        {
            return br.ReadInt64();
        }
        public static List<long> ReadArray_ReadInt64(BinaryReader br)
        {
            return ReadHelper.ReadArray<long>(br, ReadHelper.ReadInt64);
        }
        public static List<List<long>> ReadArray2_ReadInt64(BinaryReader br)
        {
            return ReadHelper.ReadArray2<long>(br, ReadHelper.ReadInt64);
        }

        public static ulong ReadUInt64(BinaryReader br)
        {
            return br.ReadUInt64();
        }
        public static List<ulong> ReadArray_ReadUInt64(BinaryReader br)
        {
            return ReadHelper.ReadArray<ulong>(br, ReadHelper.ReadUInt64);
        }
        public static List<List<ulong>> ReadArray2_ReadUInt64(BinaryReader br)
        {
            return ReadHelper.ReadArray2<ulong>(br, ReadHelper.ReadUInt64);
        }

        public static float ReadFloat(BinaryReader br)
        {
            int v = br.ReadInt32();
            return v * 0.001f;
            //return br.ReadSingle();
        }
        public static List<float> ReadArray_ReadFloat(BinaryReader br)
        {
            return ReadHelper.ReadArray<float>(br, ReadHelper.ReadFloat);
        }
        public static List<List<float>> ReadArray2_ReadFloat(BinaryReader br)
        {
            return ReadHelper.ReadArray2<float>(br, ReadHelper.ReadFloat);
        }

        public static string ReadString(BinaryReader br)
        {
            ushort len = br.ReadUInt16();
            string s = System.Text.Encoding.UTF8.GetString(br.ReadBytes(len));
            return s;
        }
        public static List<string> ReadArray_ReadString(BinaryReader br)
        {
            return ReadHelper.ReadArray<string>(br, ReadHelper.ReadString);
        }
        public static List<List<string>> ReadArray2_ReadString(BinaryReader br)
        {
            return ReadHelper.ReadArray2<string>(br, ReadHelper.ReadString);
        }
        public static string ReadString(BinaryReader br, string[] strings)
        {
            int index = br.ReadInt32();
            return strings[index];
        }
        public static List<string> ReadArray_ReadString(BinaryReader br, string[] strings)
        {
            ushort len = br.ReadUInt16();
            if (len <= 0)
                return null;

            List<string> rlt = new List<string>(len);
            for (int i = 0; i < len; ++i)
            {
                int index = br.ReadInt32();
                rlt.Add(strings[index]);
            }

            return rlt;
        }
        public static List<List<string>> ReadArray2_ReadString(BinaryReader br, string[] strings)
        {
            ushort len = br.ReadUInt16();
            if (len <= 0)
                return null;

            List<List<string>> rlt = new List<List<string>>(len);
            for (int i = 0; i < len; ++i)
            {
                List<string> rlt2 = ReadArray_ReadString(br, strings);
                if (rlt2 != null)
                {
                    rlt.Add(rlt2);
                }
            }

            return rlt;
        }

        public static Color32 ReadColor32(BinaryReader br)
        {
            int rgba = br.ReadInt32();
            Color32 color = new Color32((byte)(rgba >> 24), (byte)((rgba >> 16) & 0xff), (byte)((rgba >> 8) & 0xff), (byte)(rgba & 0xff));
            return color;
        }
        public static List<Color32> ReadArray_ReadColor32(BinaryReader br)
        {
            return ReadHelper.ReadArray<Color32>(br, ReadHelper.ReadColor32);
        }

        internal static List<T> ReadArray<T>(BinaryReader br, ReadFunc<T> readFunc)
        {
            if (br == null || readFunc == null)
                return null;

            ushort len = br.ReadUInt16();
            if (len <= 0)
                return null;

            List<T> rlt = new List<T>(len);
            for (int i = 0; i < len; ++i)
            {
                T t = readFunc(br);
                rlt.Add(t);
            }

            return rlt;
        }
        private static List<List<T>> ReadArray2<T>(BinaryReader br, ReadFunc<T> readFunc)
        {
            if (br == null || readFunc == null)
                return null;

            ushort len = br.ReadUInt16();
            if (len <= 0)
                return null;

            List<List<T>> rlt = new List<List<T>>(len);
            for (int i = 0; i < len; ++i)
            {
                List<T> rlt2 = ReadArray<T>(br, readFunc);
                if (rlt2 != null)
                {
                    rlt.Add(rlt2);
                }
            }

            return rlt;
        }
        #endregion

        #region String Parse

        public delegate T ReadFuncString<T>(string s);

        public static int ReadInt(string s)
        {
            int v = 0;
            int.TryParse(s, out v);
            return v;
        }
        public static List<int> ReadArray_ReadInt(string s, char separator)
        {
            return ReadArray<int>(s, separator, ReadInt);
        }
        public static List<List<int>> ReadArray2_ReadInt(string s, char separator, char separator2)
        {
            return ReadArray<int>(s, separator, separator2, ReadInt);
        }

        public static uint ReadUInt(string s)
        {
            uint v = 0;
            uint.TryParse(s, out v);
            return v;
        }
        public static List<uint> ReadArray_ReadUInt(string s, char separator)
        {
            return ReadArray<uint>(s, separator, ReadUInt);
        }
        public static List<List<uint>> ReadArray2_ReadUInt(string s, char separator, char separator2)
        {
            return ReadArray<uint>(s, separator, separator2, ReadUInt);
        }

        public static long ReadInt64(string s)
        {
            long v = 0;
            long.TryParse(s, out v);
            return v;
        }
        public static List<long> ReadArray_ReadInt64(string s, char separator)
        {
            return ReadArray<long>(s, separator, ReadInt64);
        }
        public static List<List<long>> ReadArray2_ReadInt64(string s, char separator, char separator2)
        {
            return ReadArray<long>(s, separator, separator2, ReadInt64);
        }

        public static ulong ReadUInt64(string s)
        {
            ulong v = 0;
            ulong.TryParse(s, out v);
            return v;
        }
        public static List<ulong> ReadArray_ReadUInt64(string s, char separator)
        {
            return ReadArray<ulong>(s, separator, ReadUInt64);
        }
        public static List<List<ulong>> ReadArray2_ReadUInt64(string s, char separator, char separator2)
        {
            return ReadArray<ulong>(s, separator, separator2, ReadUInt64);
        }

        public static float ReadFloat(string s)
        {
            float v = 0;
            float.TryParse(s, out v);
            return v;
        }
        public static List<float> ReadArray_ReadFloat(string s, char separator)
        {
            return ReadArray<float>(s, separator, ReadFloat);
        }
        public static List<List<float>> ReadArray2_ReadFloat(string s, char separator, char separator2)
        {
            return ReadArray<float>(s, separator, separator2, ReadFloat);
        }

        public static string ReadString(string s)
        {
            return s.Replace("\\n", "\n");
        }
        public static List<string> ReadArray_ReadString(string s, char separator)
        {
            return ReadArray<string>(s, separator, ReadString);
        }
        public static List<List<string>> ReadArray2_ReadString(string s, char separator, char separator2)
        {
            return ReadArray<string>(s, separator, separator2, ReadString);
        }

        public static List<T> ReadArray<T>(string s, char separator, ReadFuncString<T> readFunc)
        {
            if (string.IsNullOrWhiteSpace(s))
                return null;

            string[] vs = s.Split(separator);
            int len = vs.Length;
            if (len == 0)
                return null;

            List<T> rlt = new List<T>(len);
            for (int i = 0; i < len; ++i)
            {
                T t = readFunc(vs[i]);
                rlt.Add(t);
            }

            return rlt;
        }
        public static List<List<T>> ReadArray<T>(string s, char separator, char separator2, ReadFuncString<T> readFunc)
        {
            if (string.IsNullOrWhiteSpace(s))
                return null;

            string[] vs = s.Split(separator);
            int len = vs.Length;
            if (len == 0)
                return null;

            List<List<T>> rlt = new List<List<T>>(len);
            for (int i = 0; i < len; ++i)
            {
                List<T> rlt2 = ReadArray<T>(vs[i], separator2, readFunc);
                if (rlt2 != null)
                {
                    rlt.Add(rlt2);
                }
            }

            return rlt;
        }
        #endregion

        public static bool GetBoolByIndex(byte boolArray, int index)
        {
            return ((1 << index) & boolArray) != 0;
        }
    }
}