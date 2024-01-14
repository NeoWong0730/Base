using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Logic
{
    public class UI_Equipment_Quenching : UIParseCommon,  UI_Equipment_Quenching_Right.IListener
    {
        private UI_Equipment_Quenching_Right rightPanel;

        private ItemData curOpEquip;

        protected override void Parse()
        {
            base.Parse();

            rightPanel = new UI_Equipment_Quenching_Right();
            rightPanel.Init(transform.Find("View_Right"));
            rightPanel.Registerlistener(this);
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

        }

        public override void UpdateInfo(ItemData item)
        {
            curOpEquip = item;
            rightPanel?.UpdateInfo(item);
        }

        public void OnClickQuenching()
        {
            if (curOpEquip != null)
            {
                //绑定装备不能粹炼
                if (curOpEquip.bBind)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(101457));
                    return;
                }

                //镶嵌了宝石不能粹炼
                if (Sys_Equip.Instance.IsInlayJewel(curOpEquip))
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4181));
                    return;
                }

                Sys_Equip.Instance.OnEquipmentQuenchingReq(curOpEquip.Uuid);
            }
        }
    }
}


