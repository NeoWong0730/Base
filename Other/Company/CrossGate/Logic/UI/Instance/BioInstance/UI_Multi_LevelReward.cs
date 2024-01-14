using Logic.Core;
using Packet;
using System.Collections.Generic;
using UnityEngine;
using Table;
namespace Logic
{
    public class UI_Multi_LevelReward_Parmas
    {
        public uint InstanceID { get; set; } = 0;

    }
    public class UI_Multi_LevelReward : UIBase, UI_Multi_LevelReward_Layout.IListener
    {
        private UI_Multi_LevelReward_Layout m_Layout = new UI_Multi_LevelReward_Layout();

        private UI_Multi_LevelReward_Parmas m_Parmas = new UI_Multi_LevelReward_Parmas();

        private List<CSVInstanceDaily.Data>  m_LevelList = new List<CSVInstanceDaily.Data>();
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
            UI_Multi_LevelReward_Parmas parmas = arg as UI_Multi_LevelReward_Parmas;

            if (parmas != null)
            {
                m_Parmas = parmas;

                m_LevelList = InstanceHelper.getDailyByInstanceID(m_Parmas.InstanceID);
            }
                
        }
        protected override void OnShow()
        {
            int count = m_LevelList.Count;

            m_Layout.SetLevelCount(count);

        }

        protected override void OnHide()
        {
          
           
        }

 

        public void OnClickClose()
        {
            CloseSelf();
        }

        public void OnRewardInfinityGridChange(InfinityGridCell infinityGridCell, int index)
        {
            var data =  m_LevelList[index];

            var reward = CSVDrop.Instance.GetDropItem(data.Award);

            var cellUI = infinityGridCell.mUserData as UI_Multi_LevelReward_Layout.Member;

            cellUI.SetReward(reward);
            cellUI.SetName(LanguageHelper.GetTextContent((uint)(2022931+ index)));
        }
    }
}
