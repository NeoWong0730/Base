using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Lib.Core;
using Logic.Core;
using Table;
using Packet;

namespace Logic
{
    public class UI_Quenching_Success : UIBase
    {
        private PropItem propItem;
        private Text textName;

        private UI_Quenching_BasicAttr_Info infoBasic;
        //private TipEquipInfoGreen infoGreen;
        private UI_Quenching_MustAttr_Info infoMust;
        private UI_Quenching_SpecialEffect_Info infoSpecial;
        //private TipEquipInfoJewel infoJewel;
        //private TipEquipInfoSmelt infoSmelt;
        //private TipEquipInfoEnchant infoEnchant;
        private UI_Quenching_Suit_Info infoSuit;

        private ItemData curItem;

        protected override void OnLoaded()
        {            
            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(transform.Find("Black").GetComponent<Image>());
            eventListener.AddEventListener(EventTriggerType.PointerClick, (eventData) => {
                UIManager.CloseUI(EUIID.UI_Quenching_Success);
            });

            propItem = new PropItem();
            propItem.BindGameObject(transform.Find("Animator/View_Right/PropItem").gameObject);
            textName = transform.Find("Animator/View_Right/Text_Name").GetComponent<Text>();

            infoBasic = new UI_Quenching_BasicAttr_Info();
            infoBasic.Init(transform.Find("Animator/Scroll_View/GameObject/View_Basic_Prop"));

            infoMust = new UI_Quenching_MustAttr_Info();
            infoMust.Init(transform.Find("Animator/Scroll_View/GameObject/View_Must_Prop"));

            infoSpecial = new UI_Quenching_SpecialEffect_Info();
            infoSpecial.Init(transform.Find("Animator/Scroll_View/GameObject/View_Special_Prop"));

            infoSuit = new UI_Quenching_Suit_Info();
            infoSuit.Init(transform.Find("Animator/Scroll_View/GameObject/View_Suit_Prop"));
        }

        protected override void OnOpen(object arg)
        {            
            ulong uId = 0;
            if (arg != null)
                uId = (ulong)arg;
            curItem = Sys_Equip.Instance.GetItemData(uId);
        }

        protected override void OnShow()
        {         
            UpdatePanel();
        }

        private void UpdatePanel()
        {
            if (curItem == null)
            {
                Debug.LogError("jewel is  null");
                return;
            }

            PropIconLoader.ShowItemData itemInfo = new PropIconLoader.ShowItemData(curItem.Id, 1, true, false, false, false, false);
            propItem.SetData(new MessageBoxEvt( EUIID.UI_Quenching_Success, itemInfo));
            textName.text = LanguageHelper.GetTextContent(curItem.cSVItemData.name_id);

            infoBasic.UpdateQuenchingInfo(curItem, curItem.essence.BaseAttrValue);
            infoMust.UpdateMustAttrInfo(curItem, curItem.essence);
            infoSpecial.UpdateSpecialEffectInfo(curItem, curItem.essence);
        }
    }
}
