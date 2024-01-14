using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;

namespace Logic
{
    public class UI_Adventure_ViewRight : UIComponent
    {
        
        #region 界面组件
        private ViewRightCell cellInfo;
        private CP_Toggle cellInfoToggle;
        private ViewRightCell cellMap;
        private ViewRightCell cellReward;
        private ViewRightCell cellTask;
        private ViewRightCell cellTreasure;
        #endregion
        private IListener listener;
        private Dictionary<uint, ViewRightCell> dicCells = new Dictionary<uint, ViewRightCell>();

        #region 系统函数
        protected override void Loaded()
        {
            base.Loaded();
            Init();
        }

        public override void Show()
        {
            base.Show();
        }

        public override void OnDestroy()
        {

        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Adventure.Instance.eventEmitter.Handle(Sys_Adventure.EEvents.OnTipsNumUpdate, UpdatePageTipNum, toRegister);
        }
        #endregion

        #region function
        private void Init()
        {
            cellInfo = AddComponent<ViewRightCell>(transform.Find("TabList/TabItem0"));
            cellInfo.PageType = EAdventurePageType.Info;
            cellInfo.Register(OnTabSelect);
            dicCells.Add((uint)cellInfo.PageType, cellInfo);

            cellMap = AddComponent<ViewRightCell>(transform.Find("TabList/TabItem1"));
            cellMap.PageType = EAdventurePageType.Map;
            cellMap.Register(OnTabSelect);
            dicCells.Add((uint)cellMap.PageType, cellMap);

            cellReward = AddComponent<ViewRightCell>(transform.Find("TabList/TabItem2"));
            cellReward.PageType = EAdventurePageType.Reward;
            cellReward.Register(OnTabSelect);
            dicCells.Add((uint)cellReward.PageType, cellReward);

            cellTask = AddComponent<ViewRightCell>(transform.Find("TabList/TabItem3"));
            cellTask.PageType = EAdventurePageType.Task;
            cellTask.Register(OnTabSelect);
            dicCells.Add((uint)cellTask.PageType, cellTask);

            cellTreasure = AddComponent<ViewRightCell>(transform.Find("TabList/TabItem4"));
            cellTreasure.PageType = EAdventurePageType.Treasure;
            cellTreasure.Register(OnTabSelect);
            dicCells.Add((uint)cellTreasure.PageType, cellTreasure);
        }
        public void OnPageBtnInit(uint openType)
        {
            dicCells[openType].SetSelected(true);
            UpdatePageTipNum();
        }
        private void UpdatePageTipNum()
        {
            cellInfo.UpdateTipNum();
            cellMap.UpdateTipNum();
            cellReward.UpdateTipNum();
            cellTask.UpdateTipNum();
            cellTreasure.UpdateTipNum();
        }
        public void Register(IListener _listener)
        {
            listener = _listener;
        }
        private void OnTabSelect(uint type)
        {
            listener?.OnPageSelect(type);
            switch(type){
                case 1:
                    Sys_Adventure.Instance.ReportClickEventHitPoint("Info_Page");
                    break;
                case 2:
                    Sys_Adventure.Instance.ReportClickEventHitPoint("Map_Page");
                    break;
                case 3:
                    Sys_Adventure.Instance.ReportClickEventHitPoint("Reward_Page");
                    break;
                case 4:
                    Sys_Adventure.Instance.ReportClickEventHitPoint("Task_Page");
                    break;
                case 5:
                    Sys_Adventure.Instance.ReportClickEventHitPoint("Treasure_Page");
                    break;
            }
        }
        #endregion

        #region 响应事件

        #endregion
        public interface IListener
        {
            void OnPageSelect(uint type);
        }

        //cell类
        public class ViewRightCell : UIComponent
        {
            public EAdventurePageType PageType;
            #region 界面组件
            private GameObject tips;
            private Text tipsNum;
            private CP_Toggle toggle;
            #endregion
            private System.Action<uint> _action;

            #region 系统函数
            protected override void Loaded()
            {
                base.Loaded();
                Init();
            }

            public override void Show()
            {

            }

            public override void OnDestroy()
            {
                base.OnDestroy();
            }

            protected override void ProcessEventsForEnable(bool toRegister)
            {

            }
            #endregion

            #region function
            private void Init()
            {
                tips = transform.Find("Tips").gameObject;
                tipsNum = transform.Find("Tips/Text").GetComponent<Text>();
                toggle = transform.GetComponent<CP_Toggle>();
                toggle.onValueChanged.AddListener(OnClickToggle);

            }
            public void SetSelected(bool isOn)
            {
                toggle.SetSelected(isOn, true);
            }
            public void Register(System.Action<uint> action)
            {
                _action = action;
            }
            public void UpdateTipNum()
            {
                uint num = Sys_Adventure.Instance.GetCheckNumByType(PageType);
                if (num > 0)
                {
                    tips.SetActive(true);
                    tipsNum.text = num.ToString();
                }
                else
                {
                    tips.SetActive(false);
                }
            }
            #endregion

            #region 响应事件
            private void OnClickToggle(bool isOn)
            {
                if (isOn)
                {
                    _action?.Invoke((uint)PageType);
                }
            }
            #endregion
        }

    }
}
