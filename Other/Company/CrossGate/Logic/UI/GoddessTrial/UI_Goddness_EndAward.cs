using System;
using System.Collections.Generic;
using Logic.Core;
using Table;

namespace Logic
{
    public class UI_Goddness_EndAward : UIBase, UI_Goddness_EndAward_Layout.IListener
    {
        UI_Goddness_EndAward_Layout m_Layout = new UI_Goddness_EndAward_Layout();

        protected override void OnLoaded()
        {
            m_Layout.Load(gameObject.transform);

            m_Layout.SetListener(this);
        }

        protected override void OnShow()
        {
           var data = CSVGoddessTopic.Instance.GetConfData(Sys_GoddnessTrial.Instance.SelectID);

            int count = data.EndReward.Count;

            m_Layout.SetEndAwardCount(count);

            for (int i = 0; i < count; i++)
            {
               var items =  CSVDrop.Instance.GetDropItem(data.EndReward[i][1]);

                m_Layout.SetEndAward(i, items);

                m_Layout.SetEndAwardTextNum(i, data.EndReward[i][0]);
            }
        }
        public void OnClickClose()
        {
            CloseSelf();
        }
    }
}
