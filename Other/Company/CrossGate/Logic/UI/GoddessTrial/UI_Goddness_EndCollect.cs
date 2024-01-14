using System;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Table;
namespace Logic
{
    public partial class UI_Goddness_EndCollect:UIBase,UI_Goddness_EndCollect_Layout.IListener
    {
        UI_Goddness_EndCollect_Layout m_Layout = new UI_Goddness_EndCollect_Layout();

        List<CSVGoddessEnd.Data>  m_DataList = new List<CSVGoddessEnd.Data>();

        private uint FinishEndTopicCount = 0;
        protected override void OnLoaded()
        {
            m_Layout.Load(gameObject.transform);

            m_Layout.SetListener(this);
        }

        protected override void OnOpen(object arg)
        {            
            RefreshData();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_GoddnessTrial.Instance.eventEmitter.Handle(Sys_GoddnessTrial.EEvents.GetTopicEndAward, Refresh, toRegister);
        }
        protected override void OnShow()
        {            
            Refresh();
        }


        private void Refresh()
        {
            int count = m_DataList.Count;

            m_Layout.SetCollectCount(count);

            var record = Sys_GoddnessTrial.Instance.GetEndRecord();

           

            for (int i = 0; i < count; i++)
            {
                var isGet = Sys_GoddnessTrial.Instance.EndCollectIsEnd(m_DataList[i].id);

                m_Layout.SetCollectGet(i, isGet);
                m_Layout.SetCollectID(i, m_DataList[i].id);
                m_Layout.SetCollectTitle(i, m_DataList[i].endingName);
                m_Layout.SetCollectInfo(i, m_DataList[i].endingLan);
            }

            int curcount = record == null ? 0 : record.EndId.Count;


            FinishEndTopicCount = (uint)curcount;

            m_Layout.SetProcess(curcount, count);

            m_Layout.SetFxActive(record != null && record.GivedMaxRec < record.EndId.Count);


            uint iconid = curcount == count ? 993102u : 993101u;
            m_Layout.SetRewardImageIcon(iconid);
        }

        private void RefreshData()
        {
            m_DataList.Clear();

            var topicType = Sys_GoddnessTrial.Instance.TopicTypeID;

            var data = CSVGoddessEnd.Instance.GetAll();

            foreach (var kvp in data)
            {
                if (kvp.topicId == topicType)
                {
                    m_DataList.Add(kvp);
                }
            }
        }
    }

    public partial class UI_Goddness_EndCollect : UIBase, UI_Goddness_EndCollect_Layout.IListener
    {
        public void OnClickAllGet()
        {
            //当已领取的奖励小于已经解锁的结局数量，则向服务器申请奖励。否则打开展示所有结局的奖励
            var record = Sys_GoddnessTrial.Instance.GetEndRecord();

            if (record == null || record.GivedMaxRec >= record.EndId.Count)
            {
                UIManager.OpenUI(EUIID.UI_Goddness_EndAward);

                return;
            }

            Sys_GoddnessTrial.Instance.SendGetEndCollectAll(Sys_GoddnessTrial.Instance.TopicTypeID, FinishEndTopicCount);


        }

        public void OnClickClose()
        {
            CloseSelf();
        }

        public void OnClickGet(uint id)
        {

            UI_Goddness_Ending_Parma parama = new UI_Goddness_Ending_Parma();

            parama.isNextShowAward = false;

            parama.EndId = id;

            UIManager.OpenUI(EUIID.UI_Goddess_Ending, false, parama);
        }

    }
}
