using Lib.Core;
using Table;
using TMPro;
using UnityEngine;

namespace Logic
{
    public static class TextHelper
    {
        #region 文字风格

        public static void SetStyle(TextMeshProUGUI text, uint style)
        {
            if (style == 0)
                return;

            CSVWordStyle.Data csv = CSVWordStyle.Instance.GetConfData(style);
            SetStyle(text, csv);
        }

        public static void SetStyle(TextMeshProUGUI text, CSVWordStyle.Data style = null)
        {
            if (style == null)
                return;

            // todo style
        }

        #endregion

        #region 文字内容

        public static void SetText(TextMeshProUGUI text, string content)
        {
            if (text == null)
            {
                return;
            }

            text.text = content;
        }

        public static void SetText<T>(TextMeshProUGUI text, uint languageId, string param = null, string param1 = null, string param2 = null, string param3 = null)
        {
            if (text == null)
            {
                return;
            }

            var tuple = LanguageHelper.TryGetTuple<T>(languageId, out bool exist);
            if (exist)
            {
                // worldStyle
                // SetStyle(text, tuple.wordStyle);
                string content = LanguageHelper.MergeContent(tuple.words, param, param1, param2, param3);
                // content
                SetText(text, content);
            }
            else
            {
                DebugUtil.LogErrorFormat("{0}表 不存在id : {1}", typeof(T), languageId.ToString());
            }
        }

        public static void SetText2<T>(TextMeshProUGUI text, T table, uint languageId, uint styleId, string param = null, string param1 = null, string param2 = null, string param3 = null) where T : ILanguage
        {
            if (text == null)
            {
                return;
            }

            if (table.TryGetLanguage(languageId, out string words))
            {
                SetStyle(text, styleId);

                string content = LanguageHelper.MergeContent(words, param, param1, param2, param3);
                SetText(text, content);
            }
            else
            {
                DebugUtil.LogErrorFormat($"{typeof(T).ToString()} id = {languageId.ToString()} is invalid");
            }
        }

        #endregion
    }
}