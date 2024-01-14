using Logic.Core;
using Packet;
using System.Collections.Generic;
using UnityEngine;
using Table;
using Lib.Core;

namespace Logic
{
    public class UI_Multi_StageReward_Parmas
    {
        public uint InstanceID { get; set; } = 0;

        public CmdInstancePassStageNtf info { get; set; }

    }
    public class UI_Multi_StageReward : UIBase, UI_Multi_StageReward_Layout.IListener
    {
        private UI_Multi_StageReward_Layout m_Layout = new UI_Multi_StageReward_Layout();

        private UI_Multi_StageReward_Parmas m_Parmas = new UI_Multi_StageReward_Parmas();

        Timer m_Timer;
        float showTime = 0;
        protected override void OnLoaded()
        {
            m_Layout.Load(gameObject.transform);

            m_Layout.setListener(this);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {

        }
        protected override void OnOpen(object arg) 
        {
            UI_Multi_StageReward_Parmas parmas = arg as UI_Multi_StageReward_Parmas;

            if (parmas != null)
            {
                m_Parmas = parmas;

                //m_LevelList = InstanceHelper.getDailyByInstanceID(m_Parmas.InstanceID);
            }

            var timeconfig = CSVParam.Instance.GetConfData(1355);

            showTime = float.Parse(timeconfig.str_value) / 1000;
        }
        protected override void OnShow()
        {

            int count = m_Parmas.info == null ? 0 : m_Parmas.info.Rewards.Count;

            m_Layout.SetRewardCount(count);

            for (int i = 0; i < count; i++)
            {
                m_Layout.SetReward(i, m_Parmas.info.Rewards[i].ItemId,(uint)m_Parmas.info.Rewards[i].Count);
            }

            m_Timer = Timer.Register(showTime, OnClickClose);
        }

        protected override void OnClose()
        {
            if (m_Timer != null)
                m_Timer.Cancel();
            m_Timer = null;
        }



        public void OnClickClose()
        {
            CloseSelf();
        }


    }
}
