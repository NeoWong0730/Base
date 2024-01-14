using Logic.Core;
using System.Collections.Generic;
using Table;

namespace Logic
{
    public class UI_Multi_Reward : UIBase, UI_Multi_Reward_Layout.IListener
    {
        private UI_Multi_Reward_Layout m_Layout = new UI_Multi_Reward_Layout();


        private uint CopyID = 0;//副本ID

        

        private List<CSVInstance.Data>  m_ChaptersList = new List<CSVInstance.Data>();
        protected override void OnLoaded()
        {
            m_Layout.Load(gameObject.transform);

            m_Layout.setListener(this);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Instance.Instance.eventEmitter.Handle<uint>(Sys_Instance.EEvents.RewardRefresh, RewardNotify,toRegister);
        }
        protected override void OnOpen(object arg)
        {            
            CopyID = (uint)arg;
        }
        protected override void OnShow()
        {            
            RefreshRewardInfo();
        }

        //刷新章节列表
        private void RefreshRewardInfo()
        {
            m_ChaptersList.Clear();

           var commonData = Sys_Instance.Instance.getMultiInstanceCommon();

            foreach (var item in commonData.Entries)
            {
                if (item.Unlock)
                {
                   var data = CSVInstance.Instance.GetConfData(item.InstanceId);

                    if(data != null)
                       m_ChaptersList.Add(data);
                }
            }

            int infoCount = m_ChaptersList.Count;

            m_Layout.SetInfoCount(infoCount);

            for (int i = 0; i < infoCount; i++)
            {
                m_Layout.SetInfoIndex(i, i);

                m_Layout.SetInfoName(i, m_ChaptersList[i].Name);

               var drop = CSVDrop.Instance.GetDropItem(m_ChaptersList[i].FirstReward);

                m_Layout.SetReward(i, drop);

                bool isSelected = m_ChaptersList[i].id == commonData.SelectedInstanceId;
                if (isSelected)
                {
                    m_Layout.FocusInfo(i);
                }

                // uint langueID = isSelected ? 
                m_Layout.SetStateTex(i,isSelected ? "已选择" : "可选择");

                m_Layout.SetUsed(i, isSelected && commonData.LockedSelectedInstanceID);
                
                    
                 
            }
        }

        private void RewardNotify(uint id)
        {
          int index = m_ChaptersList.FindIndex(o => id == o.id);

            if (index < 0)
                return;

            var commonData = Sys_Instance.Instance.getMultiInstanceCommon();

            int infoCount = m_ChaptersList.Count;

            for (int i = 0; i < infoCount; i++)
            {
                bool isSelected = m_ChaptersList[i].id == commonData.SelectedInstanceId;

                if (isSelected)
                {
                    m_Layout.FocusInfo(i);
                }
                m_Layout.SetStateTex(i, isSelected ? "已选择" : "可选择");

                m_Layout.SetUsed(i, isSelected && commonData.LockedSelectedInstanceID);



            }
        }

        public void OnClickInfo(int index)
        {
            var commonData = Sys_Instance.Instance.getMultiInstanceCommon();

            if (commonData.LockedSelectedInstanceID)
            {
                Sys_Hint.Instance.PushContent_Normal("Reward had locked today");
                return;
            }

            if (m_ChaptersList.Count <= index)
                return;

            var item = m_ChaptersList[index];


            Sys_Instance.Instance.OnSelectInstanceIDReq(item.id);
        }

        public void OnUse()
        {

        }
        public void Close()
        {
            CloseSelf();
        }

    }
}
