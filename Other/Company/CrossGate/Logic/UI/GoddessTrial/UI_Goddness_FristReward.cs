using System;
using System.Collections.Generic;

using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_Goddness_FristReward_Parma
    {
        public uint ID { get; set; }
    }
    public class UI_Goddness_FristReward:UIBase,UI_Goddness_FristReward_Layout.IListener
    {
        UI_Goddness_FristReward_Layout m_Layout = new UI_Goddness_FristReward_Layout();

        uint ID = 0;

        public void OnClickClose()
        {
            CloseSelf();
        }

        protected override void OnLoaded()
        {
            m_Layout.Load(gameObject.transform);

            m_Layout.SetListener(this);
        }

        protected override void OnOpen(object arg)
        {
            if (arg is UI_Goddness_FristReward_Parma)
            {
                var parma = arg as UI_Goddness_FristReward_Parma;
                ID = parma != null ? parma.ID : 0u;
            }
        }
        protected override void OnShow()
        {
            Refresh();
        }

        private void Refresh()
        {
            var data = CSVGoddessTopic.Instance.GetConfData(ID);

            int rewardcount = data == null ? 0 : 1;

            m_Layout.m_GroupReward.SetChildSize(rewardcount);

            for (int i = 0; i < rewardcount; i++)
            {
               var rewareitem = m_Layout.GetRewardGroup(i);

                int count = data.rankReward.Count;

                rewareitem.m_GroupReward.SetChildSize(count);

                for (int j = 0; j < count; j++)
                {
                    var item = rewareitem.GetRewardItem(j);
                    item.SetReward(data.rankReward[j][0], data.rankReward[j][1]);
                }
                
            }

            LayoutRebuilder.MarkLayoutForRebuild(m_Layout.m_TransLayout as RectTransform);
        }
    }
}
