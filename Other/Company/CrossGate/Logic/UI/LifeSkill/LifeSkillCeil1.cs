using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;
using Table;
using Logic.Core;

namespace Logic
{
    public class LifeSkillCeil1
    {
        private Transform transform;
        private Image icon;
        private Image quailty;
        private Text name;
        private GameObject select;
        private GameObject normalSelectGo;
        public uint id;
        public uint category;   //1:制造 2:采集
        public int dataIndex;
        private Action<LifeSkillCeil1> onClick;
        private Action<LifeSkillCeil1> onlongPressed;
        private Image eventBg;
        private bool unlock;
        private Image iconRoot;

        public void BindGameObject(GameObject go)
        {
            transform = go.transform;
            ParseComponent();
        }

        private void ParseComponent()
        {
            normalSelectGo = transform.Find("Tag_bg").gameObject;
            icon = transform.Find("PropItem/Btn_Item/Image_Icon").GetComponent<Image>();
            eventBg = transform.Find("PropItem/Btn_Item/Image_BG").GetComponent<Image>();
            eventBg = transform.Find("PropItem/Btn_Item/Image_BG").GetComponent<Image>();
            quailty = transform.Find("PropItem/Btn_Item/Image_Quality").GetComponent<Image>();
            iconRoot = transform.Find("IconRoot").GetComponent<Image>();
            name = transform.Find("Text_Name").GetComponent<Text>();
            select = transform.Find("IconSelect").gameObject;
            quailty.gameObject.SetActive(true);
            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(eventBg);
            eventListener.AddEventListener(EventTriggerType.PointerClick, OnGridClicked);
        }

        private void OnIconClicked()
        {
            if (category == 1)
            {
                CSVFormula.Data cSVFormulaData = CSVFormula.Instance.GetConfData(id);
                UIManager.OpenUI(EUIID.UI_Message_Box, false, new MessageBoxEvt(EUIID.UI_LifeSkill_Message,
                   new PropIconLoader.ShowItemData(cSVFormulaData.view_item, 0, true, false, false, false, false)));
                //弹出配方tips
                //UIManager.OpenUI(EUIID.UI_Message_Formula, false, id);
            }
            else if (category == 2)
            {
                //弹出道具tips
                long itemCount = Sys_Bag.Instance.GetItemCount(id);
                UIManager.OpenUI(EUIID.UI_Message_Box, false, new MessageBoxEvt(EUIID.UI_LifeSkill_Message,
                    new PropIconLoader.ShowItemData(id, itemCount, true, false, false, false, false)));
            }
        }

        public void AddClickListener(Action<LifeSkillCeil1> _onClick, Action<LifeSkillCeil1> onlongPressed = null)
        {
            onClick = _onClick;
            UI_LongPressButton uI_LongPressButton = eventBg.gameObject.AddComponent<UI_LongPressButton>();
            uI_LongPressButton.onStartPress.AddListener(OnLongPressed);
        }

        private void OnGridClicked(BaseEventData baseEventData)
        {
            onClick.Invoke(this);
        }

        private void OnLongPressed()
        {
            OnIconClicked();
        }

        public void SetData(uint _id, uint _category, int _dataIndex, bool _unlock = true)
        {
            this.id = _id;
            this.category = _category;
            this.dataIndex = _dataIndex;
            this.unlock = _unlock;
            Refresh();
        }

        private void Refresh()
        {
            if (category == 1)
            {
                CSVFormula.Data cSVFormulaData = CSVFormula.Instance.GetConfData(id);
                if (cSVFormulaData != null)
                {
                    quailty.gameObject.SetActive(false);
                    ImageHelper.SetIcon(icon, CSVItem.Instance.GetConfData(cSVFormulaData.view_item).icon_id);
                    TextHelper.SetText(name, CSVItem.Instance.GetConfData(cSVFormulaData.view_item).name_id);
                    //ImageHelper.GetQualityColor_Frame(quailty, (int)CSVItem.Instance.GetConfData(cSVFormulaData.view_item).quality);
                    if (cSVFormulaData.career_condition == null)
                    {
                        normalSelectGo.SetActive(false);
                    }
                    else
                    {
                        normalSelectGo.SetActive(cSVFormulaData.career_condition.Contains(Sys_Role.Instance.Role.Career));
                    }
                }
            }
            else if (category == 2)
            {
                quailty.gameObject.SetActive(true);
                ImageHelper.SetIcon(icon, CSVItem.Instance.GetConfData(id).icon_id);
                TextHelper.SetText(name, CSVItem.Instance.GetConfData(id).name_id);
                ImageHelper.GetQualityColor_Frame(quailty, (int)CSVItem.Instance.GetConfData(id).quality);
            }
            SetGray(unlock);
        }

        public void Release()
        {
            select.SetActive(false);
        }

        public void Select()
        {
            select.SetActive(true);
        }

        public void SetGray(bool active)
        {
            ImageHelper.SetImageGray(icon, !active);
            ImageHelper.SetImageGray(iconRoot, !active);
        }
    }
}


