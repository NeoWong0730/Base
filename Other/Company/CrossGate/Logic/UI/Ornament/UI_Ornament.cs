using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using System;

namespace Logic
{
    public enum EOrnamentPageType
    {
        /// <summary>升级</summary>
        Upgrade = 1,
        /// <summary>重铸</summary>
        Recast = 2,
    }
    public class OrnamentPrama
    {
        public uint pageType = 1;
        public ulong itemUuid;
    }
    public class UI_Ornament : UIBase, UI_Ornament_ViewRight.IListener
    {
        private Button btnClose;

        private UI_CurrencyTitle currency;
        private UI_Ornament_ViewRight viewRight;
        private UI_Ornament_Upgrade viewUpgrade;
        private UI_Ornament_Recast viewRecast;

        private List<uint> pageList = new List<uint>();
        private Dictionary<uint, UIComponent> pageDict = new Dictionary<uint, UIComponent>();
        private uint openType = (uint)EOrnamentPageType.Upgrade;
        private ulong OpenItemUuid = 0;
        private bool isInit = false;//是否初始化过

        //private uint openType = 1u;
        #region 系统函数
        protected override void OnOpen(object arg)
        {
            if (arg != null && arg.GetType() == typeof(OrnamentPrama))
            {
                OrnamentPrama prama = arg as OrnamentPrama;
                openType = prama.pageType;
                OpenItemUuid = prama.itemUuid;
            }
        }
        protected override void OnLoaded()
        {
            Parse();
        }
        protected override void OnShow()
        {
            if (OpenItemUuid <= 0 && !isInit)
            {
                viewUpgrade.ResetDefaultState();
                isInit = true;
            }
            currency?.InitUi();
            //OnPageSelect(openType);
            viewRight.OnPageBtnInit(openType);
            if (openType == 2u)
            {
                viewRecast.OnRecastOpen(OpenItemUuid);
            }
        }
        protected override void OnHide()
        {
            viewUpgrade?.Hide();
            viewRecast?.Hide();            
        }
        protected override void OnDestroy()
        {
            currency?.Dispose();
            viewUpgrade.OnDestroy();
            viewRecast.OnDestroy();
            Sys_Ornament.Instance.UpgradeTargetUuid = 0;
        }
        //protected override void ProcessEventsForEnable(bool toRegister)
        //{            
        //    //Sys_Ornament.Instance.eventEmitter.Handle(Sys_Ornament.EEvents.OnCLoseAdventureView, OnBtnCloseClick, true);
        //}
        #endregion

        #region func
        private void Parse()
        {
            btnClose = transform.Find("Animator/View_Title01/Btn_Close").GetComponent<Button>();
            btnClose.onClick.AddListener(OnBtnCloseClick);
            currency = new UI_CurrencyTitle(transform.Find("Animator/UI_Property").gameObject);

            viewUpgrade = AddComponent<UI_Ornament_Upgrade>(transform.Find("Animator/View_Upgrade"));
            pageList.Add(viewUpgrade.PageType);
            pageDict.Add(viewUpgrade.PageType, viewUpgrade);
            viewRecast = AddComponent<UI_Ornament_Recast>(transform.Find("Animator/View_Recast"));
            pageList.Add(viewRecast.PageType);
            pageDict.Add(viewRecast.PageType, viewRecast);

            viewRight = AddComponent<UI_Ornament_ViewRight>(transform.Find("Animator/View_Left_Tabs"));
            viewRight.Register(this);
        }
        #endregion

        #region event
        private void OnBtnCloseClick()
        {
            this.CloseSelf();
        }
        public void OnPageSelect(uint type)
        {
            for (int i = 0; i < pageList.Count; i++)
            {
                uint key = pageList[i];
                if (key == type)
                {
                    pageDict[key].Show();
                }
                else
                {
                    pageDict[key].Hide();
                }
            }
        }
        #endregion

    }
}
