using System;
using System.Collections.Generic;
using System.Json;
using Framework;
using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using Table;

namespace Logic
{
    public partial class Sys_BattlePass : SystemModuleBase<Sys_BattlePass>
    {

        public class LevelReward
        {
            public List<ItemIdCount> NormalReward = new List<ItemIdCount>();

            public List<ItemIdCount> VipReward = new List<ItemIdCount>();
        }

        private Dictionary<uint, LevelReward> m_LevelRewardDic = new Dictionary<uint, LevelReward>();
        private Dictionary<uint, uint> m_MaxLevelDic = new Dictionary<uint, uint>();
        private void LoadLevelReward()
        {
            var battlePassUpgrades = CSVBattlePassUpgrade.Instance.GetAll();

            for (int i = 0, len = battlePassUpgrades.Count; i < len; i++)
            {
                var leverReward = new LevelReward();

                var csvdata = battlePassUpgrades[i];

                leverReward.NormalReward = CSVDrop.Instance.GetDropItem(csvdata.Base_Reward);

                leverReward.VipReward = CSVDrop.Instance.GetDropItem(csvdata.Advanced_reward);

                m_LevelRewardDic.Add(csvdata.id, leverReward);
            }
           
        }

        public LevelReward GetLevelReward(uint id)
        {
            LevelReward reward = null;

            uint realid = BranchID * 1000 + id;

            if (m_LevelRewardDic.TryGetValue(realid, out reward) == false)
            {
                reward = new LevelReward();

                var csvdata = CSVBattlePassUpgrade.Instance.GetConfData(realid);

                reward.NormalReward = CSVDrop.Instance.GetDropItem(csvdata.Base_Reward);

                reward.VipReward = CSVDrop.Instance.GetDropItem(csvdata.Advanced_reward);

                m_LevelRewardDic.Add(csvdata.id, reward);
            }

            return reward;
        }

        public void ReadMaxLevelConfig()
        {
            var data = CSVParam.Instance.GetConfData(1365u);
            var configs = data.str_value.Split('|');
            int length = configs.Length;
            for (int i = 0; i < length; i++)
            {
                var strvalues = configs[i].Split('&');
                uint brandid = uint.Parse(strvalues[0]);
                uint maxlevel = uint.Parse(strvalues[1]);

                m_MaxLevelDic.Add(brandid, maxlevel);
            }
        }
        /// <summary>
        /// 战令升级奖励表数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CSVBattlePassUpgrade.Data GetUpgradeTableData(uint id)
        {
           return CSVBattlePassUpgrade.Instance.GetConfData(BranchID*1000+id);
        }

        public CSVBattlePassUpgrade.Data GetUpgradeTableDataMaxLevel()
        {
            uint maxlevel = m_MaxLevelDic[BranchID];

            return CSVBattlePassUpgrade.Instance.GetConfData(BranchID * 1000 + maxlevel);
        }

        /// <summary>
        /// 奖励展示
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CSVBattlePassRewardDisplay.Data GetRewardDisplayTableData(uint id)
        {
           return CSVBattlePassRewardDisplay.Instance.GetConfData(BranchID * 1000 + id);
        }

        /// <summary>
        /// 战令购买奖励
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CSVBattlePassPurchase.Data GetRewardPurchaseTableData(uint id)
        {
            return CSVBattlePassPurchase.Instance.GetConfData(BranchID * 10 + id);
        }

        /// <summary>
        /// 宣传图
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public CSVPublicityMap.Data GetPublicityTableData(uint id)
        {
            return CSVPublicityMap.Instance.GetConfData(BranchID * 1000 + id);
        }

        public List<CSVPublicityMap.Data>  GetPublicityTableData()
        {
            List<CSVPublicityMap.Data> allDatas = new List<CSVPublicityMap.Data>();

            var datas = CSVPublicityMap.Instance.GetAll();
            int count = datas.Count;
            for (int i = 0; i < count; i++)
            {
                var branchid = datas[i].id/10;
                if (branchid == BranchID)
                {
                    allDatas.Add(datas[i]);
                }
            }
            return allDatas;
        }
    }
}
