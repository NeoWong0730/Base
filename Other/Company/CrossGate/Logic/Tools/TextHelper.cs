using Framework;
using Lib.Core;
using System.Text.RegularExpressions;
using Table;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace Logic
{
    public static class TextHelper
    {
        /// <summary>
        /// 供框架层反射调用，因为同名函数太多，所以额外写一个
        /// </summary>
        /// <param name="text"></param>
        /// <param name="languageID"></param>
        public static void SetTextByLanguageID(Text text, uint languageID)
        {
            SetText(text, languageID);
        }

        public static void SetText(Text text, string content)
        {
            if (text == null)
                return;

            text.text = content;
        }
        public static void SetText(Text text, string content, CSVWordStyle.Data style)
        {
            if (text == null)
                return;

            text.text = content;

            if (style == null)
                return;

            string fontName = style.FontName;
            //fontName
            if (string.IsNullOrWhiteSpace(fontName))
            {
                fontName = GlobalAssets.sFont_zhanghaishanruixian;
            }
            Font font = FontManager.GetFont(fontName);
            if (font != null)
            {
                text.font = font;
            }
            else
            {
                DebugUtil.LogWarning("未找到字体:" + style.FontName);
            }

            //fontStyle;      
            //将表中的0设置为无效 4设置为Normal
            switch (style.FontStyle)
            {
                case 1:
                    text.fontStyle = FontStyle.Bold;
                    break;
                case 2:
                    text.fontStyle = FontStyle.Italic;
                    break;
                case 3:
                    text.fontStyle = FontStyle.BoldAndItalic;
                    break;
                case 4:
                    text.fontStyle = FontStyle.Normal;
                    break;
                default:
                    break;
            }

            //color
            if (style.FontColor != Color.clear)
            {
                text.color = style.FontColor;
            }

            //fontSize;
            //0 为无效值
            if (style.FontSize != 0)
            {
                text.fontSize = style.FontSize;
            }

            //linespacing;
            //0 为无效值
            if (style.Linespacing != 0)
            {
                text.lineSpacing = style.Linespacing;
            }

            //gradientIsVertical;
            //gradientIsMultiplyTextColor;
            //gradientColorKeys;
            //"1&0|96&0&5&0.7|179&170&65&0.3";
            int gradientColor = style.GradientColor == null ? 0 : style.GradientColor.Count;
            if (gradientColor > 1)
            {
                UnityEngine.UI.Extensions.Gradient gradient = text.GetNeedComponent<UnityEngine.UI.Extensions.Gradient>();
                gradient.enabled = true;
                gradient.GradientMode = (UnityEngine.UI.Extensions.GradientMode)style.GradientColorMode;//UnityEngine.UI.Extensions.GradientMode.Local;
                gradient.GradientDir = (GradientDir)style.GradientColorDir;
                if (gradient.GradientMode == UnityEngine.UI.Extensions.GradientMode.RainbowColor
                    || gradient.GradientMode == UnityEngine.UI.Extensions.GradientMode.RainbowVertexColor)
                {
                    gradient.RainbowColor = style.GradientColor;
                }
                else
                {
                    gradient.Vertex1 = style.GradientColor[0];
                    gradient.Vertex2 = style.GradientColor[1];
                }
            }
            else
            {
                if (text.TryGetComponent<UnityEngine.UI.Extensions.Gradient>(out UnityEngine.UI.Extensions.Gradient gradient))
                {
                    gradient.enabled = false;
                }
            }

            //shadowColor;
            //shadowDistance;
            //"221#64#64#128#1.62#1";
            if (style.ShadowX != 0 || style.ShadowY != 0)
            {
                Shadow shadow = text.GetNeedComponent<Shadow>();
                shadow.effectColor = style.Shadow;
                shadow.effectDistance = new Vector2(style.ShadowX, style.ShadowY);
            }

            //outlineColor;
            //outlineDistance;
            //"226#122#122#128#1#-1";
            if (style.OutlineX != 0 || style.OutlineY != 0)
            {
                text.GetNeedComponent<Outline>();

                Outline[] outlines = text.GetComponents<Outline>();
                for (int i = 0; i < outlines.Length; ++i)
                {
                    Outline _outline = outlines[i];
                    _outline.enabled = true;
                    _outline.effectDistance = new Vector2(style.OutlineX, style.OutlineY);
                    if (style.Outline != Color.clear)
                    {
                        _outline.effectColor = style.Outline;
                    }
                }
            }
            else if (style.Outline != Color.clear)
            {
                Outline[] outline = text.GetComponents<Outline>();
                for (int i = 0; i < outline.Length; ++i)
                {
                    outline[i].effectColor = style.Outline;
                }
            }
            else
            {
                Outline[] outline = text.GetComponents<Outline>();
                for (int i = 0; i < outline.Length; ++i)
                {
                    outline[i].enabled = false;
                }
            }
        }

        #region 任务文本设置
        public static void SetTaskText(Text text, CSVTaskLanguage.Data languageData)
        {
            if (text == null)
                return;

            CSVWordStyle.Data wordStytleData = null;
            string textContent = null;
            if (languageData != null)
            {
                textContent = LanguageHelper.GetTaskTextContent(languageData);
                wordStytleData = CSVWordStyle.Instance.GetConfData(languageData.wordStyle);
            }

            SetText(text, textContent, wordStytleData);
        }
        public static void SetTaskText(Text text, uint languageID)
        {
            if (text == null)
                return;

            CSVTaskLanguage.Data languageData = CSVTaskLanguage.Instance.GetConfData(languageID);
            SetTaskText(text, languageData);
        }

        public static void SetTaskText(Text text, CSVTaskLanguage.Data languageData, string param)
        {
            if (text == null)
                return;

            CSVWordStyle.Data wordStytleData = null;
            string textContent = null;
            if (languageData != null)
            {
                textContent = LanguageHelper.GetTaskTextContent(languageData, param);
                wordStytleData = CSVWordStyle.Instance.GetConfData(languageData.wordStyle);
            }

            SetText(text, textContent, wordStytleData);
        }
        public static void SetTaskText(Text text, uint languageID, string param)
        {
            if (text == null)
                return;

            CSVTaskLanguage.Data languageData = CSVTaskLanguage.Instance.GetConfData(languageID);
            SetTaskText(text, languageData, param);
        }

        public static void SetTaskText(Text text, CSVTaskLanguage.Data languageData, string param, string param1)
        {
            if (text == null)
                return;

            CSVWordStyle.Data wordStytleData = null;
            string textContent = null;
            if (languageData != null)
            {
                textContent = LanguageHelper.GetTaskTextContent(languageData, param, param1);
                wordStytleData = CSVWordStyle.Instance.GetConfData(languageData.wordStyle);
            }

            SetText(text, textContent, wordStytleData);
        }
        public static void SetTaskText(Text text, uint languageID, string param, string param1)
        {
            if (text == null)
                return;

            CSVTaskLanguage.Data languageData = CSVTaskLanguage.Instance.GetConfData(languageID);
            SetTaskText(text, languageData, param, param1);
        }

        public static void SetTaskText(Text text, CSVTaskLanguage.Data languageData, string param, string param1, string param2)
        {
            if (text == null)
                return;

            CSVWordStyle.Data wordStytleData = null;
            string textContent = null;
            if (languageData != null)
            {
                textContent = LanguageHelper.GetTaskTextContent(languageData, param, param1, param2);
                wordStytleData = CSVWordStyle.Instance.GetConfData(languageData.wordStyle);
            }

            SetText(text, textContent, wordStytleData);
        }
        public static void SetTaskText(Text text, uint languageID, string param, string param1, string param2)
        {
            if (text == null)
                return;

            CSVTaskLanguage.Data languageData = CSVTaskLanguage.Instance.GetConfData(languageID);
            SetTaskText(text, languageData, param, param1, param2);
        }

        public static void SetTaskText(Text text, CSVTaskLanguage.Data languageData, params string[] param)
        {
            if (text == null)
                return;

            CSVWordStyle.Data wordStytleData = null;
            string textContent = null;
            if (languageData != null)
            {
                textContent = LanguageHelper.GetTaskTextContent(languageData, param);
                wordStytleData = CSVWordStyle.Instance.GetConfData(languageData.wordStyle);
            }

            SetText(text, textContent, wordStytleData);
        }
        public static void SetTaskText(Text text, uint languageID, params string[] param)
        {
            if (text == null)
                return;

            CSVTaskLanguage.Data languageData = CSVTaskLanguage.Instance.GetConfData(languageID);
            SetTaskText(text, languageData, param);
        }
        #endregion

        #region 任务设置
        public static void SetText(Text text, CSVLanguage.Data languageData)
        {
            if (text == null)
                return;

            CSVWordStyle.Data wordStytleData = null;
            string textContent = null;
            if (languageData != null)
            {
                textContent = LanguageHelper.GetTextContent(languageData);
                wordStytleData = CSVWordStyle.Instance.GetConfData(languageData.wordStyle);
            }

            SetText(text, textContent, wordStytleData);
        }
        public static void SetText(Text text, uint languageID)
        {
            if (text == null)
                return;

            CSVLanguage.Data languageData = CSVLanguage.Instance.GetConfData(languageID);
            CSVWordStyle.Data wordStytleData = null;

            string textContent = null;
            if (languageData != null)
            {
                textContent = LanguageHelper.GetTextContent(languageData);
                wordStytleData = CSVWordStyle.Instance.GetConfData(languageData.wordStyle);
            }
            else
            {
#if DEBUG_MODE
                DebugUtil.LogErrorFormat("CSVLanguage not find id = {0}", languageID);
                textContent = languageID.ToString();
#endif
            }

            SetText(text, textContent, wordStytleData);
        }

        public static void SetText(Text text, CSVLanguage.Data languageData, string param)
        {
            if (text == null)
                return;

            CSVWordStyle.Data wordStytleData = null;
            string textContent = null;
            if (languageData != null)
            {
                textContent = LanguageHelper.GetTextContent(languageData, param);
                wordStytleData = CSVWordStyle.Instance.GetConfData(languageData.wordStyle);
            }

            SetText(text, textContent, wordStytleData);
        }
        public static void SetText(Text text, uint languageID, string param)
        {
            if (text == null)
                return;

            CSVLanguage.Data languageData = CSVLanguage.Instance.GetConfData(languageID);
            CSVWordStyle.Data wordStytleData = null;

            string textContent = null;
            if (languageData != null)
            {
                textContent = LanguageHelper.GetTextContent(languageData, param);
                wordStytleData = CSVWordStyle.Instance.GetConfData(languageData.wordStyle);
            }
            else
            {
#if DEBUG_MODE
                DebugUtil.LogErrorFormat("CSVLanguage not find id = {0}", languageID);
                textContent = languageID.ToString();
#endif
            }

            SetText(text, textContent, wordStytleData);
        }

        public static void SetText(Text text, CSVLanguage.Data languageData, string param, string param1)
        {
            if (text == null)
                return;

            CSVWordStyle.Data wordStytleData = null;
            string textContent = null;
            if (languageData != null)
            {
                textContent = LanguageHelper.GetTextContent(languageData, param, param1);
                wordStytleData = CSVWordStyle.Instance.GetConfData(languageData.wordStyle);
            }

            SetText(text, textContent, wordStytleData);
        }
        public static void SetText(Text text, uint languageID, string param, string param1)
        {
            if (text == null)
                return;

            CSVLanguage.Data languageData = CSVLanguage.Instance.GetConfData(languageID);
            CSVWordStyle.Data wordStytleData = null;

            string textContent = null;
            if (languageData != null)
            {
                textContent = LanguageHelper.GetTextContent(languageData, param, param1);
                wordStytleData = CSVWordStyle.Instance.GetConfData(languageData.wordStyle);
            }
            else
            {
#if DEBUG_MODE
                DebugUtil.LogErrorFormat("CSVLanguage not find id = {0}", languageID);
                textContent = languageID.ToString();
#endif
            }

            SetText(text, textContent, wordStytleData);
        }

        public static void SetText(Text text, CSVLanguage.Data languageData, string param, string param1, string param2)
        {
            if (text == null)
                return;

            CSVWordStyle.Data wordStytleData = null;
            string textContent = null;
            if (languageData != null)
            {
                textContent = LanguageHelper.GetTextContent(languageData, param, param1, param2);
                wordStytleData = CSVWordStyle.Instance.GetConfData(languageData.wordStyle);
            }

            SetText(text, textContent, wordStytleData);
        }
        public static void SetText(Text text, uint languageID, string param, string param1, string param2)
        {
            if (text == null)
                return;

            CSVLanguage.Data languageData = CSVLanguage.Instance.GetConfData(languageID);
            CSVWordStyle.Data wordStytleData = null;

            string textContent = null;
            if (languageData != null)
            {
                textContent = LanguageHelper.GetTextContent(languageData, param, param1, param2);
                wordStytleData = CSVWordStyle.Instance.GetConfData(languageData.wordStyle);
            }
            else
            {
#if DEBUG_MODE
                DebugUtil.LogErrorFormat("CSVLanguage not find id = {0}", languageID);
                textContent = languageID.ToString();
#endif
            }

            SetText(text, textContent, wordStytleData);
        }

        public static void SetText(Text text, CSVLanguage.Data languageData, params string[] param)
        {
            if (text == null)
                return;

            CSVWordStyle.Data wordStytleData = null;
            string textContent = null;
            if (languageData != null)
            {
                textContent = LanguageHelper.GetTextContent(languageData, param);
                wordStytleData = CSVWordStyle.Instance.GetConfData(languageData.wordStyle);
            }

            SetText(text, textContent, wordStytleData);
        }
        public static void SetText(Text text, uint languageID, params string[] param)
        {
            if (text == null)
                return;

            CSVLanguage.Data languageData = CSVLanguage.Instance.GetConfData(languageID);
            CSVWordStyle.Data wordStytleData = null;

            string textContent = null;
            if (languageData != null)
            {
                textContent = LanguageHelper.GetTextContent(languageData, param);
                wordStytleData = CSVWordStyle.Instance.GetConfData(languageData.wordStyle);
            }
            else
            {
#if DEBUG_MODE
                DebugUtil.LogErrorFormat("CSVLanguage not find id = {0}", languageID);
                textContent = languageID.ToString();
#endif
            }

            SetText(text, textContent, wordStytleData);
        }
        #endregion

        #region 品质设置
        public static void SetQuailtyText(Text text, uint quailty)
        {
            uint lanId = GetQuailtyLangId(quailty);
            SetText(text, lanId);
        }
        public static void SetQuailtyText(Text text, uint quailty, string param)
        {
            uint lanId = GetQuailtyLangId(quailty);
            SetText(text, lanId, param);
        }
        public static void SetQuailtyText(Text text, uint quailty, string param, string param1)
        {
            uint lanId = GetQuailtyLangId(quailty);
            SetText(text, lanId, param, param1);
        }
        public static void SetQuailtyText(Text text, uint quailty, string param, string param1, string param2)
        {
            uint lanId = GetQuailtyLangId(quailty);
            SetText(text, lanId, param, param1, param2);
        }
        public static void SetQuailtyText(Text text, uint quailty, params string[] param)
        {
            uint lanId = GetQuailtyLangId(quailty);
            SetText(text, lanId, param);
        }

        public static uint GetQuailtyLangId(uint quailty)
        {
            switch ((EItemQuality)(quailty))
            {
                case EItemQuality.White:
                    return 2007221;
                case EItemQuality.Green:
                    return 2007222;
                case EItemQuality.Blue:
                    return 2007223;
                case EItemQuality.Purple:
                    return 2007224;
                case EItemQuality.Orange:
                    return 2007225;
                default:
                    break;
            }
            return 0;
        }
        #endregion

        public static void SetTextGradient(Text text, Color color1, Color color2)
        {
            UnityEngine.UI.Extensions.Gradient gradient = text.GetComponent<UnityEngine.UI.Extensions.Gradient>();
            gradient.enabled = true;
            if (gradient != null)
            {
                gradient.Vertex1 = color1;
                gradient.Vertex2 = color2;
            }
        }
        public static void SetTextOutLine(Text text, Color color)
        {
            Outline[] outlines = text.GetComponents<Outline>();
            foreach (var item in outlines)
            {
                item.enabled = true;
                item.effectColor = color;
            }
        }

        /// <summary>
        /// 得到字数，英文算1个,中文算2个
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static int GetCharNum(string text)
        {
            /*
            int totalNum = 0;
            char[] charText = text.ToCharArray();
            foreach (var item in charText)
            {
                int leng = System.Text.Encoding.UTF8.GetByteCount(item.ToString());
                if (leng >= 2) leng = 2;
                totalNum += leng;
            }
            return totalNum;
            */
            /*
            if (string.IsNullOrEmpty(text))
                return 0;

            char[] chars = new char[1];
            int totalNum = 0;
            int length = text.Length;
            for (int i = 0; i < length; ++i)
            {
                chars[0] = text[i];
                totalNum += Mathf.Min(2, System.Text.Encoding.UTF8.GetByteCount(chars));
            }
            return totalNum;
            */
            return FrameworkTool.GetCharNum(text);
        }
        /// <summary>
        /// 只包含数字英文中文
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static bool IsIntergetOrLetterOrChinese(string text)
        {
            /*
            System.Text.RegularExpressions.Regex reg1 
                = new System.Text.RegularExpressions.Regex(@"^[\u4e00-\u9fa5a-zA-Z0-9]+$");

            if (reg1.IsMatch(text))
                return true;
            else
                return false;
            */
            return FrameworkTool.IsIntergetOrLetterOrChinese(text);
        }
    }
}