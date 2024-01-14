using Logic.Core;
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{ 
    public partial class UI_PvpSingle_TodayReward : UIComponent, UI_PvpSingle_TodayReward_Layout.IListener
    {
        private UI_PvpSingle_TodayReward_Layout m_Layout = new UI_PvpSingle_TodayReward_Layout();

        public Action HideEvent { get; set; }

        protected override void Loaded()
        {
            base.Loaded();

            m_Layout.Load(gameObject.transform);

            m_Layout.SetListener(this);
        }

        public override void Show()
        {
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();

            HideEvent?.Invoke();
        }

        public void SetRewardList(List<ItemIdCount> rewardlist,int state)
        {
            m_Layout.SetReward(rewardlist, state == 2);
        }

        public void SetRewardShowPosition(Vector3[] points)
        {
            m_Layout.SetRewardShowPosition(points);
        }

        public void SetWinNum(int num)
        {
            m_Layout.SetTitleTex(10169, num);
        }

        public void SetSort(int sort)
        {
            m_Layout.SetSort(sort);
        }
    }


    public partial class UI_PvpSingle_TodayReward : UIComponent, UI_PvpSingle_TodayReward_Layout.IListener
    {
        public void OnClickClose()
        {
            Hide();
        }

    }
}
