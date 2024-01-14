using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class WriteHelper
{
    public delegate void WriteFunc<T>(BinaryWriter bw, T v);

    public static void Write(BinaryWriter bw, int v)
    {
        bw.Write(v);
    }
    public static void Write(BinaryWriter bw, uint v)
    {
        bw.Write(v);
    }
    public static void Write(BinaryWriter bw, long v)
    {
        bw.Write(v);
    }
    public static void Write(BinaryWriter bw, ulong v)
    {
        bw.Write(v);
    }
    public static void Write(BinaryWriter bw, float v)
    {
        bw.Write(v);
    }

    public static void WriteString(BinaryWriter bw, string v)
    {
        int count = 0;
        byte[] bits = null;
        if (!string.IsNullOrWhiteSpace(v))
        {
            bits = System.Text.Encoding.UTF8.GetBytes(v);
            count = bits.Length;
        }

        if (count > ushort.MaxValue)
        {
            Debug.LogErrorFormat("最大支持字符数量 {0}", ushort.MaxValue.ToString());
            count = ushort.MaxValue;
        }

        ushort len = (ushort)count;
        bw.Write(len);
        if (len <= 0)
            return;

        bw.Write(bits);
    }

    public static void WriteArray<T>(BinaryWriter bw, WriteFunc<T> WriteFunc, List<T> vs)
    {
        int count = vs == null ? 0 : vs.Count;
        if (count > ushort.MaxValue)
        {
            Debug.LogErrorFormat("最大支持链表元素数量 {0}", ushort.MaxValue.ToString());
            count = ushort.MaxValue;
        }

        ushort len = (ushort)count;

        bw.Write(len);

        if (len <= 0)
            return;

        for (int i = 0; i < len; ++i)
        {
            WriteFunc(bw, vs[i]);
        }
    }

    public static void WriteArray2<T>(BinaryWriter bw, WriteFunc<T> WriteFunc, List<List<T>> vs)
    {
        int count = vs == null ? 0 : vs.Count;
        if (count > ushort.MaxValue)
        {
            Debug.LogErrorFormat("最大支持链表元素数量 {0}", ushort.MaxValue.ToString());
            count = ushort.MaxValue;
        }

        ushort len = (ushort)count;

        bw.Write(len);

        if (len <= 0)
            return;

        for (int i = 0; i < len; ++i)
        {
            WriteArray<T>(bw, WriteFunc, vs[i]);
        }
    }
}
