using System;
using System.Collections.Generic;
using Logic;
using Logic.Core;
using Packet;
using Table;

namespace Logic
{
    public class UI_BattlePass_GotReward_Parma
    {
        public List<ItemIdCount> GotReward = new List<ItemIdCount>();

        public List<ItemIdCount> VipWillGetReward = new List<ItemIdCount>();
    }
    public partial class UI_BattlePass_GotReward : UIBase
    {
        UI_BattlePass_GotReward_Layout m_Layout = new UI_BattlePass_GotReward_Layout();


        UI_BattlePass_GotReward_Parma m_Parma = null;

      
        protected override void OnLoaded()
        {
            m_Layout.OnLoaded(gameObject.transform);

            m_Layout.SetListener(this);
        }

        protected override void OnOpen(object arg)
        {
            m_Parma = arg as UI_BattlePass_GotReward_Parma;

        }

        protected override void OnShow()
        {
            Refresh();
        }


        void Refresh()
        {
            m_Layout.SetRewardCount(m_Parma == null ? 0 :  m_Parma.GotReward.Count);


            int viprewardcount = m_Parma.VipWillGetReward.Count;

            m_Layout.SetVipRewardActive(viprewardcount > 0);

            m_Layout.SetVipRewardCount(viprewardcount);
        }
    }
    public partial class UI_BattlePass_GotReward : UI_BattlePass_GotReward_Layout.IListener
    {


        public void OnClickSure()
        {
            CloseSelf();
        }


        public void OnRewadInfinityChange(InfinityGridCell cell, int index)
        {
            m_Layout.SetReward(cell, m_Parma.GotReward[index].id,(uint)m_Parma.GotReward[index].count);
        }


        public void OnVipRewadInfinityChange(InfinityGridCell cell, int index)
        {

            m_Layout.SetReward(cell, m_Parma.VipWillGetReward[index].id, (uint)m_Parma.VipWillGetReward[index].count);
            
        }
    }
}
