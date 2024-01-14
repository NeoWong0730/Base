using System;
using System.Collections.Generic;
using Logic;
using Logic.Core;
using Table;
using UnityEngine;

namespace Logic
{
    public partial class UI_BattlePass_BuyLevel
    {
        class RewardDrop
        {
            public uint ID;
            // public List<ItemIdCount> itemIdCounts;

            public int StartIndex;

            public uint count;
        }
    }
    public partial class UI_BattlePass_BuyLevel : UIBase
    {
        UI_BattlePass_BuyLevel_Layout m_Layout = new UI_BattlePass_BuyLevel_Layout();

        private uint m_AddLevelCount = 0;
        private float m_Process = 0;

        private List<RewardDrop> m_DropDic = new List<RewardDrop>(16);

        private List<ItemIdCount> m_ItemList = new List<ItemIdCount>(16);

        private uint m_MinLevel = 0;
        private uint m_MaxLevel = 0;

        private uint eachPrice = 0;
        protected override void OnLoaded()
        {
            m_Layout.OnLoaded(gameObject.transform);

            m_Layout.SetListener(this);
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_BattlePass.Instance.eventEmitter.Handle(Sys_BattlePass.EEvents.UnLockNewLevelReward, OnBuyLevel, toRegister);
        }

        protected override void OnOpen(object arg)
        {
           var paramdata =  CSVParam.Instance.GetConfData(1116);

            if (paramdata != null)
                eachPrice = uint.Parse(paramdata.str_value);
        }

        protected override void OnShow()
        {
            Refresh();

           var itemdata = CSVItem.Instance.GetConfData(1);
            m_Layout.SetCostIcon(itemdata.icon_id);
        }

        protected override void OnClose()
        {
            m_DropDic.Clear();
        }


        void Refresh()
        {
            m_MinLevel = Sys_BattlePass.Instance.Info.Level;


            var maxdata = Sys_BattlePass.Instance.GetUpgradeTableDataMaxLevel();
            uint maxlevel = maxdata.id % 1000;

            m_MaxLevel = (uint)Mathf.Min(m_MinLevel + 10, maxlevel);//Sys_BattlePass.Instance.GetSpalceLevel(Sys_BattlePass.Instance.Info.Level );

            if (m_MaxLevel == Sys_BattlePass.Instance.Info.Level)
            {
                m_MaxLevel = Sys_BattlePass.Instance.GetSpalceLevel(Sys_BattlePass.Instance.Info.Level + 1);
            }


            if (m_MaxLevel == 0)
            {
                m_MaxLevel = maxlevel;
            }

            uint levelcount = m_MaxLevel - m_MinLevel;

            for (uint i = 1; i <= levelcount; i++)
            {
                uint leveleid = m_MinLevel + i;

               var lastCount =  m_ItemList.Count;
          
                RewardDrop rewardDrop = new RewardDrop();

                UI_BattlePass_Common.GetBattlePassLevelReward(m_ItemList, leveleid, Sys_BattlePass.Instance.isVip);

                var nowCount = m_ItemList.Count;

                rewardDrop.ID = leveleid;
                rewardDrop.StartIndex = lastCount;
                rewardDrop.count = (uint)(nowCount - lastCount);

                m_DropDic.Add(rewardDrop);

            }

            m_Layout.SetTips(m_MaxLevel, (uint)m_ItemList.Count);

            SetBuyCount(1);


        }


        private void SetBuyCount(uint levelcount)
        {
            if (m_MaxLevel - m_MinLevel < levelcount)
                return;

            m_AddLevelCount = levelcount;

            m_Layout.SetBuyCount(m_AddLevelCount, m_MinLevel + m_AddLevelCount);

            m_Process = m_AddLevelCount * 1f / (m_MaxLevel - m_MinLevel);

            m_Layout.SetBuyProcess(m_Process);

            m_Layout.SetBuyCost(eachPrice * levelcount);

            SetReward(levelcount);

        }

        private void SetReward(uint levelcount)
        {
            uint count = 0;

            for (uint i = 0; i < levelcount; i++)
            {
                var ritem = m_DropDic[(int)i];

                count += ritem.count;
            }

            m_Layout.SetTips( m_MinLevel + levelcount, count);

            m_Layout.SetRewardCount((int)count);
        }


        private void OnBuyLevel()
        {
            CloseSelf();

            UI_BattlePass_LevelUp_Parmas parmas = new UI_BattlePass_LevelUp_Parmas();
            parmas.LastLevel = m_MinLevel;

            parmas.Level = Sys_BattlePass.Instance.Info.Level;

            UIManager.OpenUI(EUIID.UI_BattlePass_SpecialLv, false, parmas);

         }
    }
    public partial class UI_BattlePass_BuyLevel : UI_BattlePass_BuyLevel_Layout.IListener
    {
        public void OnClickBuy()
        {
            if (m_AddLevelCount == 0)
            {
                return;
            }

            var moneyCount =  Sys_Bag.Instance.GetCurrencyCount(1);

            if (moneyCount < (long)(eachPrice * m_AddLevelCount))
            {
                MallPrama param = new MallPrama();
                param.mallId = 101u;
                param.isCharge = true;
                UIManager.OpenUI(EUIID.UI_Mall, false, param);
                return;
            }

            Sys_BattlePass.Instance.SendBuyBattlePassLevel(m_AddLevelCount);
        }

        public void OnClickClose()
        {
            CloseSelf();
        }

        public void OnCellChange(InfinityGridCell cell, int index)
        {
            m_Layout.SetRewardItem(cell, m_ItemList[index].id, (uint)m_ItemList[index].count);
        }

        public void OnClickCountAdd()
        {
            SetBuyCount(m_AddLevelCount + 1);
        }
        public void OnClickCountSub()
        {
            if (m_AddLevelCount == 0)
                return;

            SetBuyCount(m_AddLevelCount - 1);
        }

        public void OnCountChange(float value)
        {
            if (m_Process == value)
                return;

           int level = (int)((m_MaxLevel - m_MinLevel) * value);

            if (level == m_AddLevelCount)
                return;

            SetBuyCount((uint)level);
        }
    }
}
