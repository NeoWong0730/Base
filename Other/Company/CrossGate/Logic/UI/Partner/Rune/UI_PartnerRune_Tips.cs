using Logic.Core;
using Lib.Core;
using System;
using Table;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Logic
{
    public class UI_PartnerRune_Tips : UIBase
    {
        private Image closeBtn;
        private Image runeIconImage;
        private Image runeLevelImage;
        private Text runeNameText;
        private Text runeTypeText;
        private Text skillText;
        private GameObject attrTextGo;
        private Transform attrTrans;

        private CSVRuneInfo.Data runeInfoData;
        protected override void OnLoaded()
        {
            runeIconImage = transform.Find("Animator/View_Message/ListItem/Btn_Item/Image_Icon").GetComponent<Image>();
            runeLevelImage = transform.Find("Animator/View_Message/ListItem/Btn_Item/Image_RuneRank")?.GetComponent<Image>();
            runeNameText = transform.Find("Animator/View_Message/Text_Name").GetComponent<Text>();
            runeTypeText = transform.Find("Animator/View_Message/Text_Type").GetComponent<Text>();
            skillText = transform.Find("Animator/View_Message/Text_Ccontent").GetComponent<Text>();
            attrTextGo = transform.Find("Animator/View_Message/Attr").gameObject;
            attrTrans = transform.Find("Animator/View_Message/Attr_Grid");
            closeBtn = transform.Find("ClickClose").GetComponent<Image>();
            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(closeBtn);
            eventListener.AddEventListener(EventTriggerType.PointerClick, (BaseEventData eventData) =>
            {
                runeInfoData = null;
                CloseSelf();
            });
            
        }

        protected override void OnOpen(object arg)
        {
            if (null != arg)
                runeInfoData = CSVRuneInfo.Instance.GetConfData(Convert.ToUInt32(arg));
        }

        protected override void OnShow()
        {
            SetView();
        }

        private void SetView()
        {
            if(null != runeInfoData)
            {
                ImageHelper.SetIcon(runeIconImage, runeInfoData.icon);
                ImageHelper.SetIcon(runeLevelImage, Sys_Partner.Instance.GetRuneLevelImageId(runeInfoData.rune_lvl));
                runeLevelImage.gameObject.SetActive(true);

                TextHelper.SetText(runeNameText, runeInfoData.rune_name);
                TextHelper.SetText(runeTypeText, 1590010000u + runeInfoData.rune_type);
                FrameworkTool.DestroyChildren(attrTrans.gameObject);
                if (runeInfoData.rune_attribute.Count >= 2)
                {
                    //防止符文属性配置为多属性
                    int num = runeInfoData.rune_attribute.Count / 2;
                    for (int k = 0; k < num; k++)
                    {
                        uint attrId = runeInfoData.rune_attribute[k];
                        uint attrValue = runeInfoData.rune_attribute[k + 1];
                        CSVAttr.Data attrInfo = CSVAttr.Instance.GetConfData(attrId);
                        if (null != attrInfo)
                        {
                            GameObject go = GameObject.Instantiate<GameObject>(attrTextGo, attrTrans);
                            TextHelper.SetText(go.GetComponent<Text>(), attrInfo.name);
                            TextHelper.SetText(go.transform.Find("Text_Number").GetComponent<Text>(), LanguageHelper.GetTextContent(2006142u, attrInfo.show_type == 1 ? attrValue.ToString() : attrValue / 100.0f + "%"));
                            go.SetActive(true);
                        }                        
                    }
                }
                if (runeInfoData.rune_passiveskillID != 0)
                {
                    CSVPassiveSkillInfo.Data passiveSkill = CSVPassiveSkillInfo.Instance.GetConfData(runeInfoData.rune_passiveskillID);
                    if (null != passiveSkill)
                    {
                        TextHelper.SetText(skillText, passiveSkill.desc);
                    }
                    else
                    {
                        skillText.text = "";
                    }
                }
                else
                {
                    skillText.text = "";
                }
            }
        }

    }
}


