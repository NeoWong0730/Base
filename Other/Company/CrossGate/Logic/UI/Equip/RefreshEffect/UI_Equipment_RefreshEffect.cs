using System.Collections;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_Equipment_RefreshEffect : UIParseCommon
    {
        //
        private GameObject goAnim;
        //equip
        private EquipItem2 equipItem;
        private Text txtEquipName;
        private Text txtEquipLv;
        //reset
        private Transform transReset;
        private Text txtResetCost;
        private Image imgResetCost;
        private Button btnReset;
        //refresh
        private PropItem propRefreshCost;
        private Button btnRefresh;
        //effectBefore
        private Transform transBefore;
        
        //effectAfter
        private Transform transAfter;

        private ulong uId;
        private uint refreshCostId;
        private uint refreshCostNum;
        private uint resetCostId;
        private uint resetCostNum;
        
        protected override void Parse()
        {
            base.Parse();

            goAnim = transform.Find("View_Right/Fx_ui_Smelt01").gameObject;
            goAnim.SetActive(false);

            equipItem = new EquipItem2();
            equipItem.Bind(transform.Find("View_Right/View01/EquipItem2").gameObject);
            txtEquipName = transform.Find("View_Right/View01/Text_Name").GetComponent<Text>();
            txtEquipLv = transform.Find("View_Right/View01/Text_Level").GetComponent<Text>();

            transReset = transform.Find("View_Right/View01/View_Property");
            txtResetCost = transform.Find("View_Right/View01/View_Property/Text_Cost").GetComponent<Text>();
            imgResetCost = transform.Find("View_Right/View01/View_Property/Text_Cost/Image_Coin").GetComponent<Image>();
            btnReset = transform.Find("View_Right/View01/View_Property/Button_Clear").GetComponent<Button>();
            btnReset.onClick.AddListener(OnClickReset);
            
            propRefreshCost = new PropItem();
            propRefreshCost.BindGameObject(transform.Find("View_Right/View01/Grid/PropItem").gameObject);
            btnRefresh = transform.Find("View_Right/View01/Button_Clear_Up").GetComponent<Button>();
            btnRefresh.onClick.AddListener(OnClickRfresh);
            
            transBefore = transform.Find("View_Right/Suit_Before");
            transAfter = transform.Find("View_Right/Suit_After");

            List<uint> freshList = ReadHelper.ReadArray_ReadUInt(CSVParam.Instance.GetConfData(239).str_value, '|');
            refreshCostId = freshList[0];
            refreshCostNum = freshList[1];
            
            List<uint> resetList = ReadHelper.ReadArray_ReadUInt(CSVParam.Instance.GetConfData(245).str_value, '|');
            resetCostId = resetList[0];
            resetCostNum = resetList[1];
            
            Sys_Equip.Instance.eventEmitter.Handle(Sys_Equip.EEvents.OnNtfRefreshEffect, OnNtfRefreshEffect, true);
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
            Sys_Equip.Instance.eventEmitter.Handle(Sys_Equip.EEvents.OnNtfRefreshEffect, OnNtfRefreshEffect, false);
        }
        
        
        private void OnClickRfresh()
        {
            Sys_Equip.Instance.OnRefreshEffectReq(uId);
        }

        private void OnClickReset()
        {
            Sys_Equip.Instance.OnRefreshEffectReq(uId, true);
        }

        public override void UpdateInfo(ItemData item)
        {
            uId = item.Uuid;
            equipItem.SetData(item);
            CSVEquipment.Data equipInfo = CSVEquipment.Instance.GetConfData(item.Id);
            CSVItem.Data itemInfo = CSVItem.Instance.GetConfData(item.Id);
            txtEquipLv.text = LanguageHelper.GetTextContent(1000002, equipInfo.TransLevel().ToString());
            txtEquipName.text = LanguageHelper.GetTextContent(itemInfo.name_id);

            transReset.gameObject.SetActive(false);
            transAfter.gameObject.SetActive(false);
            if (item.Equip.EffectAttr2 != null && item.Equip.EffectAttr2.Count > 0)
            {
                CSVItem.Data resetCostItem = CSVItem.Instance.GetConfData(resetCostId);
                txtResetCost.text = resetCostNum.ToString();
                ImageHelper.SetIcon(imgResetCost, resetCostItem.icon_id);
                
                transReset.gameObject.SetActive(true);
                transAfter.gameObject.SetActive(true);
                
                //before
                Transform trasBeforeAttr = transBefore.Find("Attr_Grid");
                int childCount = trasBeforeAttr.childCount;
                for (int i = 0; i < childCount; ++i)
                {
                    Transform temp = trasBeforeAttr.GetChild(i);
                    if (i < item.Equip.EffectAttr2.Count)
                    {
                        temp.gameObject.SetActive(true);

                        Text des = temp.Find("Text_Num").GetComponent<Text>();
                        Text name = temp.Find("Text_Property").GetComponent<Text>();
                        des.text = "";
                        name.text = "";

                        CSVEquipmentEffect.Data effectData = CSVEquipmentEffect.Instance.GetDataByEffectId(item.Equip.EffectAttr2[i].Attr2.Id);
                        if (effectData != null)
                            name.text = LanguageHelper.GetTextContent(effectData.name);

                        //AttributeRow row = attrList[i].Attr2;
                        CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(item.Equip.EffectAttr2[i].Attr2.Id);
                        if (skillInfo != null)
                            des.text = LanguageHelper.GetTextContent(skillInfo.desc);
                        else
                            Debug.LogErrorFormat("CSVPassiveSkillInfo 找不到 id {0}", item.Equip.EffectAttr2[i].Attr2.Id);
                    }
                    else
                    {
                        temp.gameObject.SetActive(false);
                    }
                }
                
                //after
                Transform trasAfterAttr = transAfter.Find("Attr_Grid");
                childCount = trasAfterAttr.childCount;
                for (int i = 0; i < childCount; ++i)
                {
                    Transform temp = trasAfterAttr.GetChild(i);
                    if (i < item.Equip.EffectAttr.Count)
                    {
                        temp.gameObject.SetActive(true);

                        Text des = temp.Find("Text_Num").GetComponent<Text>();
                        Text name = temp.Find("Text_Property").GetComponent<Text>();
                        des.text = "";
                        name.text = "";

                        CSVEquipmentEffect.Data effectData = CSVEquipmentEffect.Instance.GetDataByEffectId(item.Equip.EffectAttr[i].Attr2.Id);
                        if (effectData != null)
                            name.text = LanguageHelper.GetTextContent(effectData.name);

                        //AttributeRow row = attrList[i].Attr2;
                        CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(item.Equip.EffectAttr[i].Attr2.Id);
                        if (skillInfo != null)
                            des.text = LanguageHelper.GetTextContent(skillInfo.desc);
                        else
                            Debug.LogErrorFormat("CSVPassiveSkillInfo 找不到 id {0}", item.Equip.EffectAttr[i].Attr2.Id);
                    }
                    else
                    {
                        temp.gameObject.SetActive(false);
                    }
                }
            }
            else
            {
                //before
                Transform trasBeforeAttr = transBefore.Find("Attr_Grid");
                int childCount = trasBeforeAttr.childCount;
                for (int i = 0; i < childCount; ++i)
                {
                    Transform temp = trasBeforeAttr.GetChild(i);
                    if (i < item.Equip.EffectAttr.Count)
                    {
                        temp.gameObject.SetActive(true);

                        Text des = temp.Find("Text_Num").GetComponent<Text>();
                        Text name = temp.Find("Text_Property").GetComponent<Text>();
                        des.text = "";
                        name.text = "";

                        CSVEquipmentEffect.Data effectData = CSVEquipmentEffect.Instance.GetDataByEffectId(item.Equip.EffectAttr[i].Attr2.Id);
                        if (effectData != null)
                            name.text = LanguageHelper.GetTextContent(effectData.name);

                        //AttributeRow row = attrList[i].Attr2;
                        CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(item.Equip.EffectAttr[i].Attr2.Id);
                        if (skillInfo != null)
                            des.text = LanguageHelper.GetTextContent(skillInfo.desc);
                        else
                            Debug.LogErrorFormat("CSVPassiveSkillInfo 找不到 id {0}", item.Equip.EffectAttr[i].Attr2.Id);
                    }
                    else
                    {
                        temp.gameObject.SetActive(false);
                    }
                }
            }
            
            PropIconLoader.ShowItemData showitem = new PropIconLoader.ShowItemData(refreshCostId, refreshCostNum, true, false, false, false, false, true, true, false);
            propRefreshCost.SetData(showitem, EUIID.UI_Equipment);
        }

        private void OnNtfRefreshEffect()
        {
            //Debug.LogError("OnNtfRefreshEffect");
            // ItemData item = Sys_Bag.Instance.GetItemDataByUuid(uId);
            // if (item != null)
            //     UpdateInfo(item);
        }
    }
}


