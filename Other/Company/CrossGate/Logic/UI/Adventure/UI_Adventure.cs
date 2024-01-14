using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using System;

namespace Logic
{
    public class AdventurePrama
    {
        public uint page;
        public uint cellValue;
    }
    public enum EAdventurePageType
    {
        /// <summary>详情</summary>
        Info = 1,
        /// <summary>地图探索</summary>
        Map = 2, 
        /// <summary>悬赏令</summary>
        Reward = 3, 
        /// <summary>侦探任务</summary>
        Task = 4, 
        /// <summary>宝藏</summary>
        Treasure = 5, 
    }
    public class UI_Adventure : UIBase,UI_Adventure_ViewRight.IListener
    {
        private Button btnClose;
        private UI_CurrencyTitle currency;
        private UI_Adventure_ViewRight viewRight;

        private UI_Adventure_Info viewInfo;
        private UI_Adventure_Map viewMap;
        private UI_Adventure_Reward viewReward;
        private UI_Adventure_Task viewTask;
        private UI_Adventure_Treasure viewTreasure;

        private List<EAdventurePageType> pageList = new List<EAdventurePageType>();
        private Dictionary<EAdventurePageType,UIComponent> pageDict = new Dictionary<EAdventurePageType, UIComponent>();

        private uint openType = 1u;// EAdventurePageType.Info;
        private uint openValue = 0;
        #region 系统函数
        protected override void OnOpen(object arg)
        {            
            if (arg != null)
            {
                if (arg.GetType() == typeof(Tuple<uint, object>))
                {
                    Tuple<uint, object> tuple = arg as Tuple<uint, object>;
                    if (tuple != null)
                    {
                        openType = tuple.Item2 == null ? 0 : Convert.ToUInt32(tuple.Item2);
                    }
                }
                else if (arg.GetType() == typeof(AdventurePrama))
                {
                    AdventurePrama prama = arg as AdventurePrama;
                    openType = (prama.page > 0) ? prama.page : 1;
                    openValue = (prama.cellValue > 0) ? prama.cellValue : 0;
                }
                else
                {
                    openType = (uint)arg;
                }
            }
        }
        protected override void OnLoaded()
        {
            Init();
        }
        protected override void OnShow()
        {            
            currency?.InitUi();
            if (openType == (uint)EAdventurePageType.Reward && openValue > 0)
            {
                viewReward.openValue = openValue;
                openValue = 0;
            }
            OnPageSelect(openType);
            viewRight.OnPageBtnInit(openType);
        }
        protected override void OnUpdate()
        {            
            viewTask.ExecUpdate();
        }
        protected override void OnDestroy()
        {
            currency?.Dispose();
            viewInfo.OnDestroy();
            viewMap.OnDestroy();
            viewReward.OnDestroy();
            viewTask.OnDestroy();
            viewTreasure.OnDestroy();
        }

        protected override void ProcessEvents(bool toRegister)
        {            
            Sys_Adventure.Instance.eventEmitter.Handle(Sys_Adventure.EEvents.OnCLoseAdventureView, OnBtnCloseClick, toRegister);
        }
        #endregion

        #region 初始化
        private void Init()
        {
            currency = new UI_CurrencyTitle(transform.Find("Animator/UI_Property").gameObject);
            btnClose = transform.Find("Animator/View_Title01/Btn_Close").GetComponent<Button>();
            btnClose.onClick.AddListener(OnBtnCloseClick);

            viewRight = AddComponent<UI_Adventure_ViewRight>(transform.Find("Animator/Label_Scroll01"));
            viewRight.Register(this);
            viewInfo = AddComponent<UI_Adventure_Info>(transform.Find("Animator/View_Details"));
            pageList.Add(viewInfo.PageType);
            pageDict.Add(viewInfo.PageType, viewInfo);
            viewMap = AddComponent<UI_Adventure_Map>(transform.Find("Animator/View_Explore"));
            pageList.Add(viewMap.PageType);
            pageDict.Add(viewMap.PageType, viewMap);
            viewReward = AddComponent<UI_Adventure_Reward>(transform.Find("Animator/View_Reward"));
            pageList.Add(viewReward.PageType);
            pageDict.Add(viewReward.PageType, viewReward);
            viewTask = AddComponent<UI_Adventure_Task>(transform.Find("Animator/View_ClueTask"));
            pageList.Add(viewTask.PageType);
            pageDict.Add(viewTask.PageType, viewTask);
            viewTreasure = AddComponent<UI_Adventure_Treasure>(transform.Find("Animator/View_Treasure"));
            pageList.Add(viewTreasure.PageType);
            pageDict.Add(viewTreasure.PageType, viewTreasure);
        }
        #endregion

        #region function
        #endregion

        #region 响应事件
        uint pageShowTime = 0;
        public void OnPageSelect(uint type)
        {
            for (int i = 0; i < pageList.Count; i++)
            {
                EAdventurePageType key = pageList[i];
                if ((uint)key == type)
                {
                    pageDict[key].Show();
                    UIManager.HitPointShow(EUIID.UI_Adventure, type.ToString());
                    pageShowTime = Sys_Time.Instance.GetServerTime();
                }
                else
                {
                    pageDict[key].Hide();
                    if(openType == (uint)key)
                    {
                        UIManager.HitPointHide(EUIID.UI_Adventure, pageShowTime, type.ToString());
                    }
                }
            }
            openType = type;
        }
        private void OnBtnCloseClick()
        {
            Sys_Adventure.Instance.ReportClickEventHitPoint("Close");
            UIManager.CloseUI(EUIID.UI_Adventure);
        }
        #endregion
    }

}
