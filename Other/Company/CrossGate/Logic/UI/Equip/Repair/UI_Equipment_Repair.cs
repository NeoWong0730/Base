using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Logic
{
    public class UI_Equipment_Repair : UIParseCommon, UI_Equipment_Repair_Right.IListener
    {
        private UI_Equipment_Repair_Right rightPanel;
        private Animator animator;

        private ItemData curOpEquip;
        private ulong _uuId;

        protected override void Parse()
        {
            base.Parse();

            rightPanel = new UI_Equipment_Repair_Right();
            rightPanel.Init(transform.Find("View_Right"));
            rightPanel.RegisterListener(this);
            animator = rightPanel.gameObject.GetComponent<Animator>();

            Sys_Equip.Instance.eventEmitter.Handle<bool>(Sys_Equip.EEvents.OnNotifyRepair, OnNotifyRepair, true);
        }

        public override void Show()
        {
            base.Show();
        }

        public override void Hide()
        {
            base.Hide();
        }

        public override void OnDestroy()
        {
            Sys_Equip.Instance.eventEmitter.Handle<bool>(Sys_Equip.EEvents.OnNotifyRepair, OnNotifyRepair, false);
        }

        public override void UpdateInfo(ItemData item)
        {
            curOpEquip = item;
            _uuId = curOpEquip != null ? curOpEquip.Uuid : 0L;

            rightPanel?.UpdateInfo(curOpEquip);
        }

        public void OnClickRepairNormal()
        {
            Sys_Equip.Instance.OnEquipmentRepairReq(curOpEquip.Uuid, 0);
        }

        public void OnClickRepairStrong()
        {
            Sys_Equip.Instance.OnEquipmentRepairReq(curOpEquip.Uuid, 1);
        }

        #region notify
        private void OnNotifyRepair(bool strongth)
        {
            if (_uuId != 0L)
            {
                curOpEquip = Sys_Equip.Instance.GetItemData(_uuId);
                rightPanel?.UpdateInfo(curOpEquip);
            }

            animator.enabled = true;
            animator.Rebind();
            string name = strongth ? "Strengthen" : "Ordinary";
            animator.Play(name, 0, 0f);
        }
        #endregion
    }
}


