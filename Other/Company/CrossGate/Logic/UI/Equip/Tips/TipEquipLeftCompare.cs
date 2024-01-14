using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Logic
{
    public class TipEquipLeftCompare : TipEquipCompare
    {
        private ItemData itemInfo;

        protected override void Parse()
        {
            base.Parse();

            backgroundFirst.BtnMsg.gameObject.SetActive(false);
            backgroundFirst.BtnSwitch.onClick.AddListener(OnBtnSwitch);
        }

        public void UpdateInfo(ItemData itemEquip, bool isShowLock = true)
        {
            itemInfo = itemEquip;

            UpdateEquipInfo(itemInfo, isShowLock);
            //backgroundFirst.BtnSwitch.gameObject.SetActive(Sys_Equip.Instance.compareList.Count > 1);

            Sys_Equip.Instance.eventEmitter.Trigger(Sys_Equip.EEvents.OnCompareBasicAttrValue, itemInfo);

        }

        private void OnBtnSwitch()
        {
            //ItemData newItem = null;

            //foreach (var item in Sys_Equip.Instance.compareList)
            //{
            //    if (itemInfo.Uuid != item.Uuid)
            //    {
            //        newItem = item;
            //        break;
            //    }
            //}

            //Debug.LogError("id =" + newItem.Id + " time=" + Time.realtimeSinceStartup);
            //UpdateInfo(newItem);
        }
    }

}
