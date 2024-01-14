using System;
using System.Collections.Generic;
using Lib.Core;
using Table;

namespace Logic
{
    public static partial class LanguageHelper
    {
        private static System.Object[] _args;

        public struct Word
        {
            public uint wordStyle;
            public string words;
            public bool existInTable;

            public Word(uint wordStyle, string words, bool existInTable)
            {
                this.wordStyle = wordStyle;
                this.words = words;
                this.existInTable = existInTable;
            }
        }

        // 逐参数假设
        public static string MergeContent(string content, string param = null, string param1 = null, string param2 = null, string param3 = null)
        {
            if (content == null)
            {
                return null;
            }
            else if (param == null)
            {
                return content;
            }
            else if (param1 == null)
            {
                return string.Format(content, param);
            }
            else if (param2 == null)
            {
                return string.Format(content, param, param1);
            }
            else if (param3 == null)
            {
                return string.Format(content, param, param1, param2);
            }
            else
            {
                if (_args == null)
                {
                    _args = new System.Object[4];
                }

                _args[0] = param;
                _args[1] = param1;
                _args[2] = param2;
                _args[3] = param3;
                return string.Format(content, _args);
            }
        }

        public static Word TryGetTuple<T>(uint languageID, out bool exist)
        {
            Word tuple = new Word(0, null, false);

            if (!_typeTuple.TryGetValue(typeof(T), out var func))
            {
                // DebugUtil.LogErrorFormat("cant find func for {0}, id = {1}", typeof(T), languageID.ToString());
                exist = false;
            }
            else
            {
                tuple = func.Invoke(languageID);
                exist = tuple.existInTable;
            }

            return tuple;
        }

        // 如果有新增的语言表，需要修改配置_typeTuple
        public static string GetContent<T>(uint languageId, string param = null, string param1 = null, string param2 = null, string param3 = null)
        {
            var tuple = TryGetTuple<T>(languageId, out bool exist);
            if (exist)
            {
                string content = MergeContent(tuple.words, param, param1, param2, param3);
                return content;
            }
            else
            {
                DebugUtil.LogErrorFormat("{0}表 不存在id : {1}", typeof(T), languageId.ToString());
            }

            return tuple.words;
        }

        // 如果有新增的语言表，需要修改配置_typeTuple
        public static string GetContent2<T>(T table, uint languageId, string param = null, string param1 = null, string param2 = null, string param3 = null) where T : ILanguage
        {
            if (table.TryGetLanguage(languageId, out string words))
            {
                return LanguageHelper.MergeContent(words, param, param1, param2, param3);
            }
            else
            {
                DebugUtil.LogErrorFormat($"{typeof(T).ToString()} id = {languageId.ToString()} is invalid");
            }
            return string.Empty;
        }
    }

    public static partial class LanguageHelper
    {
        private static readonly Dictionary<Type, Func<uint, Word>> _typeTuple = new Dictionary<Type, Func<uint, Word>>() {
            {typeof(CSVLanguage), _Do},
            {typeof(CSVLanguageTask), _Do_Task},
            {typeof(CSVLanguageMainTask), _Do_MainTask},
        };

        private static Word _Do(uint languageId)
        {
            Word tp = new Word(0, null, false);
            CSVLanguage.Data data = CSVLanguage.Instance.GetConfData(languageId);
            if (data == null)
            {
#if DEBUG_MODE
                DebugUtil.LogErrorFormat("CSVLanguage not find id = {0}", languageId.ToString());
                tp.words = languageId.ToString();
#else
                tp.words = null;
#endif
            }
            else
            {
                // tp.wordStyle = data.wordStyle;
                tp.words = data.words;
                tp.existInTable = true;
            }

            return tp;
        }

        private static Word _Do_Task(uint languageId)
        {
            Word tp = new Word(0, null, false);
            CSVLanguageTask.Data data = CSVLanguageTask.Instance.GetConfData(languageId);
            if (data == null)
            {
#if DEBUG_MODE
                DebugUtil.LogErrorFormat("CSVLanguageTask not find id = {0}", languageId.ToString());
                tp.words = languageId.ToString();
#else
                tp.words = null;
#endif
            }
            else
            {
                // tp.wordStyle = data.wordStyle;
                tp.words = data.words;
                tp.existInTable = true;
            }

            return tp;
        }
        private static Word _Do_MainTask(uint languageId)
        {
            Word tp = new Word(0, null, false);
            CSVLanguageMainTask.Data data = CSVLanguageMainTask.Instance.GetConfData(languageId);
            if (data == null)
            {
#if DEBUG_MODE
                DebugUtil.LogErrorFormat("CSVLanguageMainTask not find id = {0}", languageId.ToString());
                tp.words = languageId.ToString();
#else
                tp.words = null;
#endif
            }
            else
            {
                // tp.wordStyle = data.wordStyle;
                tp.words = data.words;
                tp.existInTable = true;
            }

            return tp;
        }
    }
}