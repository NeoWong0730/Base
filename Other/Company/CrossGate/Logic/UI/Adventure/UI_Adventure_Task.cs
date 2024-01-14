using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;

namespace Logic
{
    public class UI_Adventure_Task : UIComponent
    {
        public EAdventurePageType PageType { get; } = EAdventurePageType.Task;

        #region 界面组件
        private UI_ClueTaskMain_ClueList viewClueList;
        #endregion

        #region 系统函数
        protected override void Loaded()
        {
            base.Loaded();
            Parse();
            Sys_ClueTask.Instance.PreCheck();
        }
        protected override void Update()
        {
            base.Update();
            viewClueList.ExecUpdate();
        }
        public override void Show()
        {
            base.Show();
            UpdateView();
        }

        public override void OnDestroy()
        {
            viewClueList.OnDestroy();
            base.OnDestroy();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            //Sys_ClueTask.Instance.eventEmitter.Handle(Sys_ClueTask.EEvents.OnDetectiveExpChanged, UpdateView, toRegister);
            //Sys_ClueTask.Instance.eventEmitter.Handle(Sys_ClueTask.EEvents.OnAdventureExpChanged, UpdateView, toRegister);
        }
        #endregion

        #region function
        private void Parse()
        {
            viewClueList = AddComponent<UI_ClueTaskMain_ClueList>(transform.Find("View_ClueList"));
        }
        public void UpdateView()
        {
            viewClueList.Refresh(EClueTaskType.Detective);
        }
        #endregion

        #region 响应事件

        #endregion
    }
}
