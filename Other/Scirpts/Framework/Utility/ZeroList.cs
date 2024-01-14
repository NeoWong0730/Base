using System;
using System.Collections.Generic;

namespace Framework
{
    public static class EmptyList<T>
    {
        // ��Ҫ�ⲿ����add�Ȳ���
        public static readonly List<T> Value = new List<T>(0);
    }

    public static class EmptyArray<T>
    {
        public static readonly T[] Value = new T[0];
    }
}
