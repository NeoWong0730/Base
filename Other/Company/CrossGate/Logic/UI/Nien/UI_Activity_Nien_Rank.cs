using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Logic.Core;
using Packet;
using Table;
using Framework;
using System;
using Lib.Core;

namespace Logic
{
    public class UI_Activity_Nien_Rank  : UIBase
    {
        protected override void OnLoaded()
        {
            Button btnClose = transform.Find("Animator/View_TipsBg02_Big/Btn_Close").GetComponent<Button>();
            btnClose.onClick.AddListener(OnClickClose);
        }

        protected override void OnOpen(object arg)
        {

        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {            
            Sys_Rank.Instance.eventEmitter.Handle<uint>(Sys_Rank.EEvents.GetRankRes, GetRankRes, toRegister);
            // Sys_MallActivity.Instance.eventEmitter.Handle(Sys_MallActivity.EEvents.OnRefreshShopData, OnRefreshShopData, toRegister);
        }

        protected override void OnShow()
        {
            UpdateInfo();
        }

        protected override void OnHide()
        {

        }

        protected override void OnDestroy()
        {

        }

        private void OnClickClose()
        {
            this.CloseSelf();
        }

        private void UpdateInfo()
        {
            Sys_Rank.Instance.RankQueryReq((uint) RankType.MonsterNian, 0);
        }

        private void GetRankRes(uint key)
        {
            Debug.LogError("RankQueryRes");
        }
    }
}


