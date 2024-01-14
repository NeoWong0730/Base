using System.Collections;

using UnityEngine;

namespace Logic
{
    public class TipPetEquipRightCompare : TipPetEquipCompare
    {
        public ItemData itemInfo;

        protected override void Parse()
        {
            base.Parse();
            backgroundFirst.BtnMsg.gameObject.SetActive(true);
            backgroundFirst.BtnMsg.onClick.AddListener(OnBtnMessage);
        }

        public void UpdateItemInfo(ItemData itemEquip, uint petUid, bool isShowBtn = true, bool isShowLock = true)
        {
            itemInfo = itemEquip;

            UpdateEquipInfo(itemEquip, petUid, isShowLock);
        }

        private void OnBtnMessage()
        {
            Sys_Equip.Instance.eventEmitter.Trigger<ItemData>(Sys_Equip.EEvents.OnShowMsg, itemInfo);
        }
    }

}
