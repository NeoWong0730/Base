using System;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Table;
namespace Logic
{
    public partial class UI_Goddness_Rank : UIBase, UI_Goddness_Rank_Layout.IListener
    {
        UI_Goddness_Rank_Layout m_Layout = new UI_Goddness_Rank_Layout();

        private uint m_SelectTopicID = 0;

        List<Sys_GoddnessTrial.Topic> m_TopicList = new List<Sys_GoddnessTrial.Topic>();//符合当前人物等级的主题类型ID
        List<Sys_GoddnessTrial.TopicLevel> m_TopicLevelList = new List<Sys_GoddnessTrial.TopicLevel>();//符合当前等级的主题
        protected override void OnLoaded()
        {
            m_Layout.Load(gameObject.transform);

            m_Layout.SetListener(this);
        }

        protected override void OnOpened()
        {
            RefreshData();

        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_GoddnessTrial.Instance.eventEmitter.Handle(Sys_GoddnessTrial.EEvents.TopicRank, OnRankNoifty, toRegister);
        }

        protected override void OnShow()
        {
            RefreshLeftContent();

            InitSend();

            FocusCurSelectDiffict();
        }

        private void InitSend()
        {
            //int count = Sys_GoddnessTrial.Instance.m_DifficlyList.Count;
            //for (int i = 0; i < count; i++)
            //{
            //    var result = m_TopicLevelList[0].Find(Sys_GoddnessTrial.Instance.m_DifficlyList[i]);

            //    if (result != 0)
            //    {
            //        OnClickDiffic(result);
            //        break;
            //    }
            //}
               
        }

        private void FocusCurSelectDiffict()
        {
            int indextopic = m_TopicList.FindIndex(o => o.id == Sys_GoddnessTrial.Instance.TopicTypeID);

            if (indextopic >= 0)
            {
                m_Layout.FocusContent(indextopic, false);

                //uint topicid = 0;

                //if (m_TopicLevelList[indextopic].m_DicActity.TryGetValue(Sys_GoddnessTrial.Instance.SelectDifficlyID, out topicid))
                //{
                //    CSVGoddessTopic.in
                //}

                m_Layout.FocusContentChild(indextopic, (int)Sys_GoddnessTrial.Instance.SelectDifficlyID - 1,true);
            }

        }
        protected override void OnClose()
        {
            m_Layout.SetTopicCount(0);
        }

        private void RefreshData()
        {
            m_TopicList.Clear();
            m_TopicLevelList.Clear();

            foreach (var kvp in Sys_GoddnessTrial.Instance.TopicTypeDic)
            {
                var item = kvp.Value.Find(Sys_Role.Instance.Role.Level);

                if (item != null)
                {
                    m_TopicList.Add(kvp.Value);
                   // m_TopicList.Add(kvp.Value);
                    m_TopicLevelList.Add(item);
                   // m_TopicLevelList.Add(item);
                }
            }
        }
        void RefreshLeftContent()
        {
            int index = 0;
            int topiccount = m_TopicList.Count;
            m_Layout.SetTopicCount(topiccount);

            for (int i = 0; i < topiccount; i++)
            {
                m_Layout.SetTopicID(i, m_TopicList[i].id);

                int childCount = m_TopicLevelList[i].m_DicActity.Count;

                m_Layout.SetTopicChildCount(i, childCount);

                index = 0;

                foreach (var childkvp in m_TopicLevelList[i].m_DicActity)
                {
                    int dind = (int)childkvp.Key - 1;

                    var launid = Sys_GoddnessTrial.Instance.m_DifficlyLangue[dind];

                    m_Layout.SetTopicChildName(i, index, LanguageHelper.GetTextContent(launid));
                    m_Layout.SetTopicChildID(i, index, childkvp.Value);

                    if (index == 0)
                    {
                       var data = CSVGoddessTopic.Instance.GetConfData(childkvp.Value);

                        if (data != null)
                        {
                            m_Layout.SetTopicName(i, data.topicLan);
                        }
                    }

                    index += 1;
                }
            }
        }

        private void RefreshRankInfo()
        {
            int count = Sys_GoddnessTrial.Instance.Rank.RankList.Count;

            m_Layout.SetRankSize(count);

            for (int i = 0; i < count; i++)
            {
                var value = Sys_GoddnessTrial.Instance.Rank.RankList[i];
                m_Layout.SetRankItem(i, (uint)(i + 1), value.RoleName.ToStringUtf8(), value.Score, value.RankTime,value.RoleId != Sys_Role.Instance.RoleId);
            }
        }
    }
    public partial class UI_Goddness_Rank : UIBase, UI_Goddness_Rank_Layout.IListener
    {
        public void OnClickClose()
        {
            CloseSelf();
        }

        public void OnClickDiffic(uint id)
        {
            //m_Layout.RebuildContent();

            Sys_GoddnessTrial.Instance.SendFristRank(id);
        }

        public void OnClickInstance(uint id)
        {
            m_SelectTopicID = id;
            m_Layout.RebuildContent();
        }

        public void OnClickRankDetail(int index)
        {
            var value = Sys_GoddnessTrial.Instance.Rank.RankList[index];


            Sys_GoddnessTrial.Instance.SendGetRankRoleInfo(Sys_GoddnessTrial.Instance.Rank.Id, value.RoleId);

        }
    }

    public partial class UI_Goddness_Rank : UIBase, UI_Goddness_Rank_Layout.IListener
    {
        private void OnRankNoifty()
        {
            RefreshRankInfo();
        }
    }
}
