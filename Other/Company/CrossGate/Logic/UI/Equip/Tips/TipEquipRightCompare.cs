using System.Collections;

using UnityEngine;

namespace Logic
{
    public class TipEquipRightCompare : TipEquipCompare
    {
        public ItemData itemInfo;

        protected override void Parse()
        {
            base.Parse();

            backgroundFirst.BtnSwitch.gameObject.SetActive(false);
            backgroundFirst.BtnMsg.gameObject.SetActive(true);
            backgroundFirst.BtnMsg.onClick.AddListener(OnBtnMessage);
        }

        public void UpdateItemInfo(ItemData itemEquip, bool isShowBtn = true, bool isShowLock = true)
        {
            itemInfo = itemEquip;

            UpdateEquipInfo(itemEquip, isShowLock);
        }

        private void OnBtnMessage()
        {
            Sys_Equip.Instance.eventEmitter.Trigger<ItemData>(Sys_Equip.EEvents.OnShowMsg, itemInfo);
        }
    }

}
