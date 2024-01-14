using Lib.Core;
using Lib.AssetLoader;
using System.IO;
using System.Text;
using Logic.Core;
using System.Text.RegularExpressions;
using Table;
namespace Logic
{
    public enum EReNameConsumeType
    {
        None,
        ECard,
        EDiamond,
    }

    /// <summary>
    /// 角色名字系统
    /// </summary>
    public class Sys_RoleName : SystemModuleBase<Sys_RoleName>
    {
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        public enum EEvents
        {
        }

        public int minNameLength { get; set; } = 2;
        public int maxNameLength { get; set; } = 8;

        private string[] Library_A;
        private string[] Library_B;

        public override void Init()
        {
            Stream memory_a = AssetMananger.Instance.LoadStream("Config/AttachMent_a.txt");
            StreamReader sr_a = new StreamReader(memory_a);
            string str_a = sr_a.ReadToEnd();
            Library_A = str_a.Split("|"[0]);
            sr_a.Close();
            sr_a.Dispose();
            memory_a.Close();
            memory_a.Dispose();

            Stream memory_b = AssetMananger.Instance.LoadStream("Config/AttachMent_b.txt");
            StreamReader sr_b = new StreamReader(memory_b);
            string str_b = sr_b.ReadToEnd();
            Library_B = str_b.Split("|"[0]);
            sr_b.Close();
            sr_b.Dispose();
            memory_b.Close();
            memory_b.Dispose();
        }

        public string LimitLengthAndFilterForName(string text)
        {
            return Sys_WordInput.Instance.LimitLengthAndFilterForName(text, maxNameLength);
        }

        public bool HasBadWords(string name)
        {
            return Sys_WordInput.Instance.HasLimitWord(name);
        }

        public bool HasBadNames(string name)
        {
            return HasBadWords(name) || HasNameLimitWord(name);
        }

        public bool HasNameLimitWord(string name)
        {
            CSVParam.Data cSVParamData = CSVParam.Instance.GetConfData(844u);
            if (null != cSVParamData)
            {
                return Regex.IsMatch(name, cSVParamData.str_value);
            }         
            else
            {
                return false;
            }
        }

        public string RandomName()
        {
            while (true)
            {
                string Name = TryRandomName();
                if (!HasBadNames(Name))
                {
                    return Name;
                }
            }
        }
        private string TryRandomName()
        {
            int Length_A = Library_A.Length;
            int Length_B = Library_B.Length;

            string prefixName = Library_A[UnityEngine.Random.Range(0, Length_A)];
            StringBuilder ab = new StringBuilder(prefixName);
            ab.Append(Library_B[UnityEngine.Random.Range(0, Length_B)]);
            return ab.ToString();
        }
    }
}
