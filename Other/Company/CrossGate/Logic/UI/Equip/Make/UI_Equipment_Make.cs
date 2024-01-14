using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Logic
{
    public class UI_Equipment_Make : UIParseCommon, UI_Equipment_Make_Right.IListener
    {
        private UI_Equipment_Make_Right rightPanel;
        private Animator animator;

        private ItemData curOpEquip;
        private ulong _uuId;

        protected override void Parse()
        {
            base.Parse();

            rightPanel = new UI_Equipment_Make_Right();
            rightPanel.Init(transform.Find("View_Right"));
            rightPanel.RegisterListener(this);
            animator = rightPanel.gameObject.GetComponent<Animator>();

            Sys_Equip.Instance.eventEmitter.Handle(Sys_Equip.EEvents.OnNotifyMake, OnNotifyMake, true);
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
            Sys_Equip.Instance.eventEmitter.Handle(Sys_Equip.EEvents.OnNotifyMake, OnNotifyMake, false);
        }

        public override void UpdateInfo(ItemData item)
        {
            curOpEquip = item;

            _uuId = 0L;
            if (item != null)
                _uuId = item.Uuid;
            rightPanel?.UpdateInfo(item);
        }

        public void OnClickMake()
        {
            if (curOpEquip != null && curOpEquip.bBind)
            {
                Sys_Hint.Instance.PushContent_Normal(Table.CSVLanguage.Instance.GetConfData(101458).words);
                return;
            }

            Sys_Equip.Instance.OnEquipmentRepairReq(curOpEquip.Uuid, 0);
        }


        #region notify
        private void OnNotifyMake()
        {
            if (_uuId != 0L)
            {
                curOpEquip = Sys_Equip.Instance.GetItemData(_uuId);
                rightPanel?.UpdateInfo(curOpEquip);
            }

            //animator.enabled = true;
            //string name = strongth ? "Strengthen" : "Ordinary";
            //animator.Play(name, 0, 0f);
        }
        #endregion
    }
}


