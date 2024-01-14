using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Logic
{
    public class TipPetEquipLeftCompare : TipPetEquipCompare
    {
        private ItemData itemInfo;

        protected override void Parse()
        {
            base.Parse();

            backgroundFirst.BtnMsg.gameObject.SetActive(false);
        }

        public void UpdateInfo(ItemData itemEquip, uint petUid, bool isShowLock = true)
        {
            itemInfo = itemEquip;

            UpdateEquipInfo(itemInfo, petUid, isShowLock);
            //backgroundFirst.BtnSwitch.gameObject.SetActive(Sys_Equip.Instance.compareList.Count > 1);

            Sys_Equip.Instance.eventEmitter.Trigger(Sys_Equip.EEvents.OnCompareBasicAttrValue, itemInfo);

        }

    }

}
