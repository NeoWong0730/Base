using System.IO;
using System.Text.RegularExpressions;
using Lib.AssetLoader;
using Lib.Core;
using Logic.Core;
using UnityEngine;

namespace Logic {
    // 屏蔽字
    public class Sys_ShieldedWord : SystemModuleBase<Sys_ShieldedWord> {
        public override void Init() {
            using (Stream memory = AssetMananger.Instance.LoadStream("Config/IllegalWords.txt")) {
                using (StreamReader sr = new StreamReader(memory)) {
                    string str = sr.ReadToEnd();
                    string[] illegalWordArray = str.Split("|"[0]);
                    IllegalWordDetection.Init(illegalWordArray);
                }
            }
        }

        private string Filter(string text, bool needFilter = true) {
            if (needFilter) {
                return IllegalWordDetection.Filter(text);
            }

            return text;
        }

        public bool HasIllegalWord(string text) {
            return IllegalWordDetection.IllegalWordsExistJudgement(text);
        }

        public string LimitLengthAndFilter(string text, int maxLength = 100) {
            int wordLength = 0;
            int index;
            int textLen = text.Length;

            for (index = 0; index < textLen; index++) {
                if (wordLength >= maxLength) {
                    return Filter(text.Substring(0, index));
                }

                if (text[index] <= 256) {
                    wordLength++;
                }
                else {
                    wordLength += 2;
                }
            }

            return Filter(text);
        }

        public string LimitLengthAndFilterForName(string text, int maxLength = 8) {
            int wordLength = 0;
            int index;
            int textLen = text.Length;

            for (index = 0; index < textLen; index++) {
                if (wordLength >= maxLength) {
                    return Filter(text.Substring(0, index));
                }

                wordLength++;
            }

            return Filter(text);
        }
    }

// 账户起名
    public class Sys_AccountName : SystemModuleBase<Sys_AccountName> {
        private string[] _AccountFirstNames; // 姓氏
        private string[] _AccountLastNames; // 名字

        public const string REGIX = @"[^\u4e00-\u9fa5A-Za-z0-9]";

        public override void Init() {
            using (Stream memory = AssetMananger.Instance.LoadStream("Config/AttachMent_a.txt")) {
                using (StreamReader sr = new StreamReader(memory)) {
                    string str = sr.ReadToEnd();
                    _AccountFirstNames = str.Split("|"[0]);
                }
            }

            using (Stream memory = AssetMananger.Instance.LoadStream("Config/AttachMent_b.txt")) {
                using (StreamReader sr = new StreamReader(memory)) {
                    string str = sr.ReadToEnd();
                    _AccountLastNames = str.Split("|"[0]);
                }
            }
        }

        // text是否合法
        public bool IsIllegalWord(string text) {
            return Sys_ShieldedWord.Instance.HasIllegalWord(text);
        }
        
        public bool IsIllegalName(string text) {
            return IsIllegalWord(text) || Regex.IsMatch(text, REGIX);
        }

        // 随机名字
        public string RandomName() {
            while (true) {
                string Name = TryRandomName();
                if (!IsIllegalWord(Name)) {
                    return Name;
                }
            }
        }

        // 随机名字
        private string TryRandomName() {
            int la = _AccountFirstNames.Length;
            int lb = _AccountLastNames.Length;

            string firstName = _AccountFirstNames[Random.Range(0, la - 1)];
            string lastName = _AccountLastNames[Random.Range(0, lb - 1)];
            return firstName + lastName;
        }
    }
}