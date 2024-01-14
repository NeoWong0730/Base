using Google.Protobuf;
using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;

namespace Logic
{
    /// <summary> 家族系统-奖金 </summary>
    public partial class Sys_Family : SystemModuleBase<Sys_Family>
    {
        private uint fundsItemId;
        public uint FundsItemId
        {
            get
            {
                if (fundsItemId == 0)
                {
                    fundsItemId = Convert.ToUInt32(CSVParam.Instance.GetConfData(1538).str_value);
                }
                return fundsItemId;
            }
        }

        /// <summary>
        /// 获取每周获得家族资金数量
        /// </summary>
        /// <returns>每周总家族资金</returns>
        public ulong GetFundsCoinGainWeekly()
        {
            var guildDetailInfo = Sys_Family.Instance.familyData.familyDetailInfo.cmdGuildGetGuildInfoAck.Info;
            if (null == guildDetailInfo) return 0;
            return guildDetailInfo.CoinGainWeekly;
        }

        public ulong GetFundsByPositionAndWeeklyCoin(uint position)
        {
            CSVFamilyBonus.Data cSVFamilyBonus = CSVFamilyBonus.Instance.GetConfData(position);

            int index = GetFundsDataIndex(cSVFamilyBonus);

            if (index != -1)
            {
                return cSVFamilyBonus.Reward[index];
            }
            
            return 0;
        }

        public int GetFundsDataIndex(CSVFamilyBonus.Data cSVFamilyBonus)
        {
            if (null != cSVFamilyBonus && null != cSVFamilyBonus.RewardTrigger)
            {
                var weeklyCoin = GetFundsCoinGainWeekly();
                var count = cSVFamilyBonus.RewardTrigger.Count;
                int index = -1;
                for (int i = count - 1; i >= 0; i--)
                {
                    if (weeklyCoin >= cSVFamilyBonus.RewardTrigger[i])
                    {
                        index = i;
                        break;
                    }
                }
                return index;
            }

            return 0;
        }
    }
}
