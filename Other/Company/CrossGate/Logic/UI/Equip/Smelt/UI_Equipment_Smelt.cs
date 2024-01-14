using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;

namespace Logic
{
    public class UI_Equipment_Smelt : UIParseCommon, UI_Equipment_Smelt_Right.IListener
    {
        private UI_Equipment_Smelt_Right rightPanel;
        private Animator animator;
        private GameObject fxGo;

        private ItemData curOpEquip;
        private ulong _uuId;

        protected override void Parse()
        {
            base.Parse();

            rightPanel = new UI_Equipment_Smelt_Right();
            rightPanel.Init(transform.Find("View_Right"));
            rightPanel.Registerlistener(this);

            animator = rightPanel.gameObject.GetComponent<Animator>();
            animator.speed = 0f;

            fxGo = animator.transform.Find("Fx_ui_Smelt01").gameObject;
            fxGo.gameObject.SetActive(false);

            Sys_Equip.Instance.eventEmitter.Handle(Sys_Equip.EEvents.OnNotifySmelt, OnNotifySmelt, true);
            Sys_Equip.Instance.eventEmitter.Handle(Sys_Equip.EEvents.OnNotifyRevertSmelt, OnNotifyRevertSmelt, true);
        }

        public override void Show()
        {
            base.Show();
        }

        public override void Hide()
        {
            fxGo.SetActive(false);

            animator.speed = 0f;
            animator.enabled = false;

            base.Hide();
        }

        public override void OnDestroy()
        {
            Sys_Equip.Instance.eventEmitter.Handle(Sys_Equip.EEvents.OnNotifySmelt, OnNotifySmelt, false);
            Sys_Equip.Instance.eventEmitter.Handle(Sys_Equip.EEvents.OnNotifyRevertSmelt, OnNotifyRevertSmelt, false);
        }

        public override void UpdateInfo(ItemData item)
        {
            curOpEquip = item;
            _uuId = curOpEquip != null ? curOpEquip.Uuid : 0L;

            rightPanel?.UpdateInfo(curOpEquip);
        }

        public void OnClickRevert()
        {
            if (curOpEquip != null)
            {
                Sys_Equip.Instance.OnEquipmentRevertSmeltReq(curOpEquip.Uuid);
            }
        }

        public void OnClickSmelt()
        {
            if (curOpEquip != null)
            {
                if (curOpEquip.bBind)
                {
                    Sys_Hint.Instance.PushContent_Normal(Table.CSVLanguage.Instance.GetConfData(101456).words);
                    return;
                }

                if (!Sys_Equip.Instance.IsCloseSmeltBoxTip)
                {
                    MsgBoxParam param = new MsgBoxParam();
                    param.strContent = LanguageHelper.GetTextContent(4222, LanguageHelper.GetTextContent(curOpEquip.cSVItemData.name_id));
                    param.isToggle = true;
                    param.strToggleTip = LanguageHelper.GetTextContent(4223);
                    param.actionToggle = (ison) =>
                    {
                        Sys_Equip.Instance.IsCloseSmeltBoxTip = ison;
                    };
                    param.actionBtn = (isok) =>
                    {
                        if (isok)
                            Sys_Equip.Instance.OnEquipmentSmeltReq(curOpEquip.Uuid);
                    };

                    UIManager.OpenUI(EUIID.UI_MessageBox_Tip, false, param);
                }
                else
                {
                    Sys_Equip.Instance.OnEquipmentSmeltReq(curOpEquip.Uuid);
                }
            }
        }

        #region notify
        private void OnNotifySmelt()
        {
            fxGo.SetActive(false);
            fxGo.SetActive(true);

            animator.enabled = true;
            animator.speed = 1f;
            animator.Play("Smelt", 0, 0f);

            OnRefreshRightInfo();
        }

        private void OnNotifyRevertSmelt()
        {
            OnRefreshRightInfo();
        }

        private void OnRefreshRightInfo()
        {
            if (_uuId != 0L)
                curOpEquip = Sys_Equip.Instance.GetItemData(_uuId);
            rightPanel?.UpdateInfo(curOpEquip);
        }
        #endregion
    }
}


