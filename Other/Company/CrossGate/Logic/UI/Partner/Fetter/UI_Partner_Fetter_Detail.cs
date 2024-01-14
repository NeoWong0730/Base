using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Logic.Core;
using Lib.Core;
using Table;
using Framework;

namespace Logic
{
    public class UI_Partner_Fetter_Detail : UIBase
    {
        private UI_Partner_Fetter_Detail_BondInfo bondInfo;
        private UI_Partner_Fetter_Detail_List listInfo;
        private UI_Partner_Fetter_Detail_Effects effectsInfo;

        private uint bondId;

        protected override void OnLoaded()
        {
            bondInfo = new UI_Partner_Fetter_Detail_BondInfo();
            bondInfo.Init(transform.Find("Animator/List/Item_Main"));

            listInfo = new UI_Partner_Fetter_Detail_List();
            listInfo.Init(transform.Find("Animator/List/Scroll View"));

            effectsInfo = new UI_Partner_Fetter_Detail_Effects();
            effectsInfo.Init(transform.Find("Animator/View_Result"));

            Button btnClose = transform.Find("Animator/View_TipsBgNew01/Btn_Close").GetComponent<Button>();
            btnClose.onClick.AddListener(() => { this.CloseSelf(); });
        }

        protected override void OnOpen(object arg)
        {
            bondId = 0;
            if (arg != null)
            {
                bondId = (uint)arg;
            }
        }

        //protected override void ProcessEventsForEnable(bool toRegister)
        //{            
        //    //Sys_Equip.Instance.eventEmitter.Handle<EquipmentOperations>(Sys_Equip.EEvents.OnOperationType, OnEquipOpType, toRegister);
        //}

        protected override void OnShow()
        {
            UpdateInfo();
        }

        protected override void OnHide()
        {            

        }

        private void UpdateInfo()
        {
            bondInfo?.UpdateInfo(bondId);
            listInfo?.UpdateInfo(bondId);
            effectsInfo?.UpdateInfo(bondId);
        }
    }
}


