using System.Collections.Generic;
using Lib.Core;
using Logic.Core;
using Table;
using Net;
using Packet;

namespace Logic
{
    public partial class Sys_Knowledge : SystemModuleBase<Sys_Knowledge>
    {
        private Dictionary<uint, List<uint>> dictFragments = new Dictionary<uint, List<uint>>();

        private void InitFragment()
        {
            dictFragments.Clear();

            foreach (var data in CSVMemoryPieces.Instance.GetAll())
            {
                if (dictFragments.ContainsKey(data.title_name))
                {
                    dictFragments[data.title_name].Add(data.id);
                }
                else
                {
                    dictFragments.Add(data.title_name, new List<uint>() { data.id });
                }
            }
        }

        public Dictionary<uint, List<uint>> GetTotalFragments()
        {
            return dictFragments;
        }

        public List<uint> GetFragStorys(uint fragId)
        {
            List<uint> temp = new List<uint>();
            dictFragments.TryGetValue(fragId, out temp);
            return temp;
        }

        /// <summary>
        /// 判断记忆碎片是否激活
        /// </summary>
        /// <param name="fragId"></param>
        /// <returns></returns>
        public bool IsFragActive(uint fragId)
        {
            bool isActive = false;

            List<uint> temp;
            if (dictFragments.TryGetValue(fragId, out temp))
            {
                for (int i = 0; i < temp.Count; ++i)
                {
                    bool active = IsKnowledgeActive(temp[i]);
                    if (active)
                    {
                        isActive = true;
                        break;
                    }
                }
            }

            return isActive;
        }

        /// <summary>
        /// 获得记忆碎片的icon
        /// </summary>
        /// <param name="fragId"></param>
        /// <returns></returns>
        public string GetFragIcon(uint fragId)
        {
            List<uint> temp;
            if (dictFragments.TryGetValue(fragId, out temp))
            {
                for (int i = 0; i < temp.Count; ++i)
                {
                    CSVMemoryPieces.Data data = CSVMemoryPieces.Instance.GetConfData(temp[i]);
                    if (data != null)
                        return data.icon;
                }
            }

            return string.Empty;
        }
    }
}