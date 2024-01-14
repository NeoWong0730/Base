using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Logic.Core;
using Packet;
using Table;

namespace Logic
{
    public class UI_Treasure : UIBase
    {
        private UI_CurrencyTitle currency;
        private Button mBtnClose;
        private UI_Treasure_Display mDisplay;
        private UI_Treasure_List mList;

        protected override void OnLoaded()
        {            
            currency = new UI_CurrencyTitle(transform.Find("UI_Property").gameObject);

            mBtnClose = transform.Find("Animator/View_Title03/Btn_Close").GetComponent<Button>();
            mBtnClose.onClick.AddListener(() =>
            {
                UIManager.CloseUI(EUIID.UI_Treasure);
            });

            mDisplay = new UI_Treasure_Display();
            mDisplay.Init(transform.Find("Collect_Display"));

            mList = new UI_Treasure_List();
            mList.Init(transform.Find("Collect_List"));
        }
        
        protected override void ProcessEventsForEnable(bool toRegister)
        {            
            Sys_Treasure.Instance.eventEmitter.Handle(Sys_Treasure.EEvents.OnRefreshNtf, OnRefresh, toRegister);
        }

        protected override void OnShow()
        {            
            mDisplay.Show();
            mList.Show();
            currency.InitUi();
        }

        protected override void OnHide()
        {
            mDisplay.Hide();
            mList.Hide();            
        }

        private void OnRefresh()
        {
            mDisplay.UpdateInfo();
            mList.Refresh();
        }
    }
}


