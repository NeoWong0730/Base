using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;

namespace Logic
{
    public class UI_Adventure_Treasure : UIComponent
    {
        public EAdventurePageType PageType { get; } = EAdventurePageType.Treasure;

        #region 界面组件
        private UI_Treasure_Display mDisplay;
        private UI_Treasure_List mList;
        #endregion

        #region 系统函数
        protected override void Loaded()
        {
            base.Loaded();
            Parse();
        }

        public override void Show()
        {
            base.Show();
            UpdateView();
        }

        public override void OnDestroy()
        {
            base.OnDestroy();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            base.ProcessEventsForEnable(toRegister);
            Sys_Treasure.Instance.eventEmitter.Handle(Sys_Treasure.EEvents.OnRefreshNtf, OnRefreshView, toRegister);
        }
        #endregion

        #region function
        private void Parse()
        {
            mDisplay = new UI_Treasure_Display();
            mDisplay.Init(transform.Find("Collect_Display"));
            mList = new UI_Treasure_List();
            mList.Init(transform.Find("Collect_List"));
        }
        private void UpdateView()
        {
            mDisplay.Show();
            mList.Show();
        }
        #endregion

        #region 响应事件
        private void OnRefreshView()
        {
            mDisplay.UpdateInfo();
            mList.Refresh();
        }
        #endregion
    }
}
