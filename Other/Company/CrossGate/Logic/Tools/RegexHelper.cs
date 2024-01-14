using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine;
using Framework;
using Table;
using Lib.Core;

namespace Logic
{
    public static class RegexHelper
    {
        public enum RegexType
        {
            Normal,
            E_Item,//道具
        }
        private static List<string> srcSub = new List<string>();
        private static List<string> dstSub = new List<string>();


        /// <summary>
        /// 如   <item>6</item>jihdfh<item>87</item>989  转换得到   
        ///      ?? jihdfh ?? 989      （？？处根据具体需求自行添加枚举转换）
        /// </summary>
        /// <param name="regexType"></param>
        /// <param name="srcText"></param>
        /// <param name="formatStart"></param>
        /// <param name="formatEnd"></param>
        /// <returns></returns>
        public static string Parse(RegexType regexType, string srcText, string formatStart, string formatEnd)
        {
            string res;
            Fliter(regexType, srcText, formatStart, formatEnd);
            res = _Parse(formatStart, srcText);
            res = _Parse(formatEnd, res);
            for (int i = 0; i < srcSub.Count; i++)
            {
                res = res.Replace(srcSub[i], dstSub[i]);
            }
            return res;
        }

        private static void Fliter(RegexType regexType, string srcText, string formatStart, string formatEnd)
        {
            srcSub.Clear();
            dstSub.Clear();
            Regex regexStart = new Regex(formatStart);
            Regex regexEnd = new Regex(formatEnd);
            StringBuilder stringBuilder = new StringBuilder();
            MatchCollection matchsStart = regexStart.Matches(srcText);
            MatchCollection matchsEnd = regexEnd.Matches(srcText);

            int matchCount = matchsStart.Count;
            for (int i = 0; i < matchCount; i++)
            {
                Match matchStart = matchsStart[i];
                Match matchEnd = matchsEnd[i];
                int startIndex = matchStart.Index + matchStart.Length;
                int endIndex = matchEnd.Index;
                string sub = srcText.Substring(startIndex, endIndex - startIndex);
                srcSub.Add(sub);
                switch (regexType)
                {
                    case RegexType.Normal:
                        uint value = uint.Parse(sub);
                        value += 10;
                        dstSub.Add(value.ToString());
                        break;
                    case RegexType.E_Item:
                        uint itemId = uint.Parse(sub);
                        string itemName = LanguageHelper.GetTextContent(CSVItem.Instance.GetConfData(itemId).name_id);
                        dstSub.Add(itemName);
                        break;
                    default:
                        break;
                }
            }
        }

        private static string _Parse(string format, string srcText)
        {
            Regex regex = new Regex(format);
            int offestIndex = 0;
            StringBuilder stringBuilder = StringBuilderPool.GetTemporary();
            MatchCollection matchs = regex.Matches(srcText);
            int matchCount = matchs.Count;
            for (int i = 0; i < matchCount; i++)
            {
                Match matchStart = matchs[i];
                stringBuilder.Append(srcText, offestIndex, matchStart.Index - offestIndex);
                offestIndex = matchStart.Index + matchStart.Length;
            }
            if (offestIndex < srcText.Length)
            {
                stringBuilder.Append(srcText, offestIndex, srcText.Length - offestIndex);
            }
            string rlt = StringBuilderPool.ReleaseTemporaryAndToString(stringBuilder);
            return rlt;
        }
    }
}


