using System;
using System.Collections.Generic;
using Logic;
using Logic.Core;
using Packet;
using Table;

namespace Logic
{
    public class UI_BattlePass_LevelUp_Parmas
    {
        public uint LastLevel;

        public uint Level;
    }
    public partial class UI_BattlePass_LevelUp : UIBase
    {
        UI_BattlePass_LevelUp_Layout m_Layout = new UI_BattlePass_LevelUp_Layout();

        UI_BattlePass_LevelUp_Parmas m_parmas = null;

        private List<ItemIdCount> m_RewardList;
        protected override void OnLoaded()
        {
            m_Layout.OnLoaded(gameObject.transform);

            m_Layout.SetListener(this);
        }

        protected override void OnOpen(object arg)
        {
            m_parmas = arg as UI_BattlePass_LevelUp_Parmas;

            m_RewardList = UI_BattlePass_Common.GetBattlePassLevelReward(m_parmas.LastLevel + 1, m_parmas.Level,Sys_BattlePass.Instance.isVip);
        }

        protected override void OnShow()
        {
            Refresh();
        }

        protected override void OnClose()
        {
            
        }


        void Refresh()
        {
            m_Layout.SetSuperBtnActive(Sys_BattlePass.Instance.isVip == false);

            m_Layout.SetLevel(Sys_BattlePass.Instance.Info.Level);

            m_Layout.SetRewardCount(m_RewardList == null ? 0 : m_RewardList.Count);
        }
    }
    public partial class UI_BattlePass_LevelUp : UI_BattlePass_LevelUp_Layout.IListener
    {


        public void OnClickSure()
        {
            CloseSelf();
        }

        public void OnClickSureSuper()
        {
            UIManager.OpenUI(EUIID.UI_BattlePass_Pay);
        }

        public void OnRewadInfinityChange(InfinityGridCell cell, int index)
        {
            m_Layout.SetReward(cell, m_RewardList[index].id,(uint)m_RewardList[index].count);
        }
    }
}
