using Logic.Core;
using System.IO;
using Lib.AssetLoader;
using Lib.Core;

namespace Logic
{
    public class Sys_WordInput : SystemModuleBase<Sys_WordInput>
    {
        public bool needFilter = true;

        public override void Init()
        {
            base.Init();

            Stream memory = AssetMananger.Instance.LoadStream("Config/IllegalWords.txt");
            StreamReader sr = new StreamReader(memory);

            string str = sr.ReadToEnd();
            string[] illegalWordArray = str.Split("|"[0]);
            IllegalWordDetection.Init(illegalWordArray);

            sr.Close();
            sr.Dispose();
            memory.Close();
            memory.Dispose();
;        }

        private string Filter(string text)
        {
            if (needFilter)
                return IllegalWordDetection.Filter(text);
            return string.Empty;
        }

        public bool HasLimitWord(string text)
        {
            return IllegalWordDetection.IllegalWordsExistJudgement(text);
        }

        public string LimitLengthAndFilter(string text, int maxLength = 100)
        {
            int wordLength = 0;
            int index;
            int textLen = text.Length;

            for (index = 0; index < textLen; index++)
            {
                if (wordLength >= maxLength)
                {
                    return Filter(text.Substring(0, index));
                }

                if (text[index] <= 256)
                {
                    wordLength++;
                }
                else
                {
                    wordLength += 2;
                }
            }
            return Filter(text);
        }

        public string LimitLengthAndFilterForName(string text, int maxLength = 8)
        {
            int wordLength = 0;
            int index;
            int textLen = text.Length;

            for (index = 0; index < textLen; index++)
            {
                if (wordLength >= maxLength)
                {
                    return Filter(text.Substring(0, index));
                }
                wordLength++;
            }
            return Filter(text);
        }
    }
}
