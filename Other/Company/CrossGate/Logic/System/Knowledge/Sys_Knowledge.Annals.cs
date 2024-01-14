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
        private Dictionary<uint, List<uint>> dictAnnals = new Dictionary<uint, List<uint>>();

        public uint SelectYearId = 0;

        private void InitAnnals()
        {
            dictAnnals.Clear();

            foreach (var data in CSVChronology.Instance.GetAll())
            {
                if (dictAnnals.ContainsKey(data.years))
                {
                    dictAnnals[data.years].Add(data.id);
                }
                else
                {
                    dictAnnals.Add(data.years, new List<uint>() { data.id });
                }
            }
        }

        /// <summary>
        /// 魔力纪年数据
        /// </summary>
        /// <returns></returns>
        public Dictionary<uint, List<uint>> GetTotalAnnals()
        {
            return dictAnnals;
        }

        /// <summary>
        /// 判断魔力纪年是否激活
        /// </summary>
        /// <param name="yearId"></param>
        /// <returns></returns>
        public bool IsAnnalActive(uint yearId)
        {
            bool isActive = false;
            List<uint> temp;
            if (dictAnnals.TryGetValue(yearId, out temp))
            {
                for (int i = 0; i < temp.Count; ++i)
                {
                    isActive = IsKnowledgeActive(temp[i]);
                    if (isActive)
                        break;
                }
            }

            return isActive;
        }

        /// <summary>
        /// 获得固定魔力纪年的事件
        /// </summary>
        /// <param name="yearId"></param>
        /// <returns></returns>
        public List<uint> GetAnnalEvents(uint yearId)
        {
            List<uint> temp = new List<uint>();
            dictAnnals.TryGetValue(yearId, out temp);
            return temp;
        }
    }
}