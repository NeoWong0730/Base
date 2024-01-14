using System;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Table;

namespace Logic
{
    public class UI_GoddnessTrial_Parma
    {
        public uint instanceID = 0;
        public int instanceIndex = -1;
        public uint TopicID = 0;
        public bool isShowDetail = false;

        public bool isEmpty()
        {
            if (instanceID == 0)
                return true;

            return false;
        }


        public void Copy(UI_GoddnessTrial_Parma value)
        {
            instanceID = value.instanceID;
            isShowDetail = value.isShowDetail;
            instanceIndex = value.instanceIndex;
            TopicID = value.TopicID;
        }
    }
    public partial class UI_GoddnessTrial:UIBase, UI_GoddnessTrial_Layout.IListener
    {

        UI_GoddnessTrial_Layout m_Layout = new UI_GoddnessTrial_Layout();

        private int m_DifficlySelectIndex = 0;

        private List<uint> m_DifficlyLangue = new List<uint>();

        UI_GoddnessTrial_Parma m_Parma = new UI_GoddnessTrial_Parma();


        bool isCumstom = false;

        int SelectInstanceIndex = -1; 
        protected override void OnLoaded()
        {
            m_Layout.Load(gameObject.transform);

            m_Layout.SetListener(this);

            var paramData = CSVParam.Instance.GetConfData(847);
             string[] paramValues = paramData.str_value.Split('|');

            for (int i = 0; i < paramValues.Length; i++)
            {
                uint uvalue = 0;
                uint.TryParse(paramValues[i], out uvalue);

                m_DifficlyLangue.Add(uvalue);
            }
        }

        protected override void OnOpen(object arg)
        {
          
            UI_GoddnessTrial_Parma valeu = arg as UI_GoddnessTrial_Parma;

            if (valeu != null)
            {
                m_Parma.Copy(valeu);

                isCumstom = true;
            }
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_GoddnessTrial.Instance.eventEmitter.Handle(Sys_GoddnessTrial.EEvents.RefrshData, OnRefreshData, toRegister);
            Sys_GoddnessTrial.Instance.eventEmitter.Handle(Sys_GoddnessTrial.EEvents.TeamMemsRecord, OnTeamRecordInfo, toRegister);
            Sys_GoddnessTrial.Instance.eventEmitter.Handle(Sys_GoddnessTrial.EEvents.TopicDifficult, OnTopicDifficult, toRegister);
        }
        protected override void OnShow()
        {

            Sys_GoddnessTrial.Instance.SendInstanceReq();


            //Refresh();


        }        

        private void Refresh()
        {
            var data = CSVGoddessTopic.Instance.GetConfData(Sys_GoddnessTrial.Instance.SelectID);

            if (data == null)
                return;

            var count = data.iconId.Count;

            for (int i = 0; i < count; i++)
            {
                m_Layout.SetChapter(i,(uint)i,data.iconId[i]);
            }

            m_Layout.SetTitle(data.topicIcon);

            m_Layout.SetLevelStr(data.copyLevel[0], data.copyLevel[1]);

            m_DifficlySelectIndex = Sys_GoddnessTrial.Instance.m_DifficlyList.FindIndex(o => o == Sys_GoddnessTrial.Instance.SelectDifficlyID);

            if (SelectInstanceIndex < 0)
                SelectInstanceIndex = Sys_GoddnessTrial.Instance.ChaperIndex;
            else
            {
                Sys_GoddnessTrial.Instance.SetSelectChaperIndex((uint)SelectInstanceIndex);
                Sys_GoddnessTrial.Instance.RefreshSelectChaperInstance();
            }
                  

            var instanceidIndex = SelectInstanceIndex;

            if (m_Parma != null && m_Parma.isEmpty() == false && m_Parma.TopicID == Sys_GoddnessTrial.Instance.SelectID)
            {
                instanceidIndex = m_Parma.instanceIndex;

                m_Parma.instanceID = 0;
                m_Parma.instanceIndex = 0;
            }

            m_Layout.SetFocusCapter(instanceidIndex,false);


            if (m_Parma.isShowDetail)
            {
                m_Layout.SetShowDetail(true);

                RefreshDetail();

                Sys_GoddnessTrial.Instance.SetSelectChaperIndex((uint)instanceidIndex);
            }

        }

        private void RefreshDetail()
        {
            int count = Sys_GoddnessTrial.Instance.m_DifficlyList.Count;

            m_Layout.SetDifficlySize(count);

            for (int i = 0; i < count; i++)
            {
                var diffic = Sys_GoddnessTrial.Instance.m_DifficlyList[i];
                var maxDiffic = Sys_GoddnessTrial.Instance.MaxDifficlyID;

                m_Layout.SetDifficlyID(i, diffic, m_DifficlyLangue[i]);

                m_Layout.SetDifficInteractable(i, diffic <= maxDiffic);
            }

        
        }

        private void RefreshDetailWithDifficly()
        {
            var InstanceID = Sys_GoddnessTrial.Instance.SelectInstance;

            var InstanceData = CSVInstance.Instance.GetConfData(InstanceID);
            var drop = CSVDrop.Instance.GetDropItem(InstanceData.FirstReward);
            m_Layout.SetDetailDrop(drop);

            m_Layout.SetDetailName(InstanceData.Name);

            var data = CSVGoddessTopic.Instance.GetConfData(Sys_GoddnessTrial.Instance.SelectID);
            if (data != null && Sys_GoddnessTrial.Instance.ChaperIndex >= 0)
                m_Layout.SetDetailInfo(data.lanID[Sys_GoddnessTrial.Instance.ChaperIndex]);

            m_Layout.SetDetailScore((uint)InstanceData.RecommondPoint, (uint)Sys_Attr.Instance.rolePower);
        }
    }
    public partial class UI_GoddnessTrial : UIBase, UI_GoddnessTrial_Layout.IListener
    {
        private void OnRefreshData()
        {
            if (isCumstom && Sys_GoddnessTrial.Instance.SelectID == m_Parma.TopicID)
            {
                var data = CSVGoddessTopic.Instance.GetConfData(Sys_GoddnessTrial.Instance.SelectID);
                m_DifficlySelectIndex = Sys_GoddnessTrial.Instance.m_DifficlyList.FindIndex(o => o == data.topicDifficulty);

                if (SelectInstanceIndex < 0)
                    OnClickChapter((uint)m_Parma.instanceIndex);
            }
            else
            {
                Refresh();
            }
        }

        private void OnTeamRecordInfo()
        {
            CloseSelf();
            UIManager.OpenUI(EUIID.UI_Goddess_Enter, false, 1);
        }

        private void OnTopicDifficult()
        {
            m_DifficlySelectIndex = Sys_GoddnessTrial.Instance.m_DifficlyList.FindIndex(o => o == Sys_GoddnessTrial.Instance.SelectDifficlyID);

            RefreshDetailWithDifficly();

            Refresh();

            m_Layout.SetDifficlyFocuse(m_DifficlySelectIndex,false);
        }
    }
    public partial class UI_GoddnessTrial : UIBase, UI_GoddnessTrial_Layout.IListener
    {
        public void OnClickClose()
        {
            CloseSelf();
        }
        public void OnClickCharacter()
        {
            UIManager.OpenUI(EUIID.UI_Goddness_Charac);
        }

        public void OnClickEnding()
        {
            UIManager.OpenUI(EUIID.UI_Goddess_EndCollect);
        }

        public void OnClickChapter(uint id)
        {
            SelectInstanceIndex = (int)id;

            Sys_GoddnessTrial.Instance.SetSelectChaperIndex(id);

            m_Layout.SetShowDetail(true);

            RefreshDetail();

            m_Layout.SetDifficlyFocuse(m_DifficlySelectIndex);

        }

        public void OnClickDifficulty(int id, int index)//困难度
        {  
            Sys_GoddnessTrial.Instance.SetDifficly((uint)id);

            m_DifficlySelectIndex = index;
   
        }

        public void OnClickRand()
        {
            UIManager.OpenUI(EUIID.UI_Goddess_Rank);
        }
        public void OnClickFastTeam()
        {
           // uint targetId = 4101u;

            var topicData = CSVGoddessTopic.Instance.GetConfData(Sys_GoddnessTrial.Instance.SelectID);
            //uint offset = topicData == null ? 0 : ((topicData.topicDifficulty - 1) * 10u);
            //targetId = targetId + offset;
 
            if (Sys_Team.Instance.IsFastOpen(true))
                Sys_Team.Instance.OpenFastUI(topicData.teamTarget);
        }
        public void OnClickGoWithTeam()
        {
            if (Sys_Team.Instance.HaveTeam == false)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2022359));
                return;
            }

            bool result = Sys_Instance.Instance.CheckTeamCondition(Sys_GoddnessTrial.Instance.SelectInstance);


            if (result == false)
                return;

            Sys_GoddnessTrial.Instance.SendGetTeamMemsRecord(Sys_GoddnessTrial.Instance.SelectID);

           // UIManager.OpenUI(EUIID.UI_Goddess_Enter, false, 1);
        }

        public void OnClickCloseDetail()
        {
            m_Layout.SetShowDetail(false);
        }

        public void OnClickDeDetail()
        {

            var datas = Sys_GoddnessTrial.Instance.CurTopicLevel;
            int count = datas.m_DicActity.Count;

            List<uint> ids = new List<uint>(count);
            foreach (var kvp in datas.m_DicActity)
            {
                ids.Add(kvp.Value);
            }

            Sys_GoddnessTrial.Instance.SendGetTopicFirstTeamReq(ids);

            UIManager.OpenUI(EUIID.UI_Goddess_Difficult);
        }
    }
}
