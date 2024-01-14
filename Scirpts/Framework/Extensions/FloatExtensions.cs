using UnityEngine;

namespace Framework
{
    public static class FloatExtensions
    {
        //float 值保留小数点后几位
        public static string ToString(this float num, int save)
        {
            string numToString = num.ToString();

            int index = numToString.IndexOf(".");
            int length = numToString.Length;

            if (index != -1)
            {
                return string.Format("{0}.{1}", numToString.Substring(0, index), numToString.Substring(index + 1, Mathf.Min(length - index - 1, save)));
            }
            else
            {
                return num.ToString();
            }
        }

        public static string ToString(this decimal num, int save)
        {
            string numToString = num.ToString();

            int index = numToString.IndexOf(".");
            int length = numToString.Length;

            if (index != -1)
            {
                return string.Format("{0}.{1}", numToString.Substring(0, index), numToString.Substring(index + 1, Mathf.Min(length - index - 1, save)));
            }
            else
            {
                return num.ToString();
            }
        }
    }
}