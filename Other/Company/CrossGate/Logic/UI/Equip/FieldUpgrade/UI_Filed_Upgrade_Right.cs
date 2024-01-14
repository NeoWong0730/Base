using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using Lib.Core;
using Logic.Core;
using Table;

namespace Logic
{
    public class UI_Filed_Upgrade_Right
    {
        public class EquipSlot
        {
            private Transform transform;
            private CP_Toggle toggle;

            private Action<int> _action;
            private int _index;

            public void Init(Transform trans)
            {
                transform = trans;
                toggle = transform.GetComponent<CP_Toggle>();
                toggle.onValueChanged.AddListener(OnToggle);
            }

            public void Register(Action<int> action, int index)
            {
                _action = action;
                _index = index;
            }

            private void OnToggle(bool isOn)
            {
                if (isOn)
                {
                    _action?.Invoke(_index);
                }
            }

            public void OnSelect(bool isSelect)
            {
                toggle?.SetSelected(isSelect, true);
            }
        }
        
        private Transform transform;
        private List<EquipSlot> listSlots = new List<EquipSlot>();
        
        //exp
        private Text txtExpLv;
        private Slider sliderExp;
        private Text txtExpPercent;
        private Text txtExpUp;
        
        //present
        private Transform transPresentTemplate;
        //Next
        private Transform transNextTemplate;

        private PropItem propItem;
        private Button btnPropAdd;
        private Button btnUpgrade;
        private Text txtLeftTimeTip;
        
        private int curSlotIndex = 0;
        private int curSlot = 0;
        private ulong curMatUid = 0;
        private bool lvLimit;

        public void Init(Transform trans)
        {
            transform = trans;
            listSlots.Clear();
            for (int i = 0; i < 5; ++i)
            {
                string str = string.Format("Button/Item{0}", i + 1);
                Transform child = transform.Find(str);
                EquipSlot slot = new EquipSlot();
                slot.Init(child);
                slot.Register(OnSelectSlot, i + 1);
                slot.OnSelect(false);
                listSlots.Add(slot);
            }

            txtExpLv = transform.Find("Exp/Image/Text_Exp").GetComponent<Text>();
            sliderExp = transform.Find("Exp/Slider_Exp").GetComponent<Slider>();
            txtExpPercent = transform.Find("Exp/Text_Percent").GetComponent<Text>();
            txtExpUp = transform.Find("Exp/Text_Up").GetComponent<Text>();

            transPresentTemplate = transform.Find("Present/Attr_Grid/Attr");
            transPresentTemplate.gameObject.SetActive(false);
            transNextTemplate = transform.Find("Next/Attr_Grid/Attr");
            transNextTemplate.gameObject.SetActive(false);

            propItem = new PropItem();
            propItem.BindGameObject(transform.Find("PropItem").gameObject);

            btnPropAdd = transform.Find("Btn_Add").GetComponent<Button>();
            btnPropAdd.onClick.AddListener(OnClickPropAdd);
            btnUpgrade = transform.Find("Btn_Upgrade").GetComponent<Button>();
            btnUpgrade.onClick.AddListener(OnClickUpgrade);

            txtLeftTimeTip = transform.Find("Text_Tips (3)").GetComponent<Text>();
        }

        private void OnSelectSlot(int index)
        {
            if (curSlotIndex != index)
            {
                curSlotIndex = index;
                RefreshData();
            }
        }

        private void OnClickPropAdd()
        {
            UIManager.OpenUI(EUIID.UI_EquipSlot_Upgrade_Items, false,  (uint)curSlot);
        }
        
        private void OnClickUpgrade()
        {
            if (lvLimit)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4256));
                return;
            }
            
            if (curMatUid != 0)
            {
                ItemData item = Sys_Bag.Instance.GetItemDataByUuid(curMatUid);
                if (item != null)
                {
                    bool isGem = false;
                    for (int i = 0; i < item.Equip.JewelinfoId.Count; ++i)
                    {
                        uint jewelInfoId = item.Equip.JewelinfoId[i];
                        if (jewelInfoId != 0)
                        {
                            isGem = true;
                        }
                    }

                    if (isGem)
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4257));
                        return;
                    }

                    if (item.IsLocked)
                    {
                        PromptBoxParameter.Instance.Clear();
                        PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(15248, LanguageHelper.GetTextContent(item.cSVItemData.name_id), LanguageHelper.GetTextContent(item.cSVItemData.name_id));
                        PromptBoxParameter.Instance.SetConfirm(true, () =>
                        {
                            Sys_Bag.Instance.OnItemLockReq(item.Uuid, false);
                        });
                        PromptBoxParameter.Instance.SetCancel(true, null);
                        UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
                        return;
                    }

                    if (Sys_Equip.Instance.IsSlotUpgradeTip)
                    {
                        MsgBoxParam param = new MsgBoxParam();
                        param.strContent = LanguageHelper.GetTextContent(4641);
                        param.isToggle = true;
                        param.toggleState = false;
                        param.strToggleTip = LanguageHelper.GetTextContent(4642);
                        param.actionToggle = (ison) =>
                        {
                            Sys_Equip.Instance.IsSlotUpgradeTip = !ison;
                        };
                        param.actionBtn = (isok) =>
                        {
                            if (isok)
                                Sys_Equip.Instance.OnBodyEnhanceReq((uint)curSlot, curMatUid);
                        };

                        UIManager.OpenUI(EUIID.UI_MessageBox_Tip, false, param);
                    }
                    else
                    {
                        Sys_Equip.Instance.OnBodyEnhanceReq((uint)curSlot, curMatUid);
                    }
                }
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4255));
            }
        }

        public void UpdateInfo()
        {
            curSlotIndex = 0;
            curSlot = 0;
            curMatUid = 0;
            Sys_Equip.Instance.CheckSlotNextTime();
            
            listSlots[0].OnSelect(true);
        }

        public void RefreshData()
        {
            curMatUid = 0;
            propItem.transform.gameObject.SetActive(false);
            btnPropAdd.gameObject.SetActive(true);
            
            curSlot = curSlotIndex > 1 ? curSlotIndex + 1 : curSlotIndex;
            uint curLevel = Sys_Equip.Instance.GetSlotLevel(curSlotIndex);
            uint curExp = Sys_Equip.Instance.GetSlotExp(curSlotIndex);
            uint upExp = 0;

            lvLimit = false;
            
            //exp
            txtExpUp.text = "";
            txtExpLv.text = LanguageHelper.GetTextContent(6005, curLevel.ToString());

            CSVSlotUpgrade.Data curData = Sys_Equip.Instance.GetEquipSlotData((uint)curSlot, curLevel);
            if (curData != null)
            {
                FrameworkTool.DestroyChildren(transPresentTemplate.parent.gameObject, transPresentTemplate.name);
                int count = curData.attr.Count;
                for (int i = 0; i < count; ++i)
                {
                    GameObject goAttr = GameObject.Instantiate(transPresentTemplate.gameObject);
                    goAttr.transform.SetParent(transPresentTemplate.parent);
                    goAttr.transform.localScale = Vector3.one;
                    goAttr.transform.localPosition = new Vector3(goAttr.transform.localPosition.x, goAttr.transform.localPosition.y, 0f);
                    goAttr.SetActive(true);

                    uint attrId = curData.attr[i][0];
                    uint value = curData.attr[i][1];
                    CSVAttr.Data attrData = CSVAttr.Instance.GetConfData(attrId);
                    goAttr.FindChildByName("Text_Property").GetComponent<Text>().text =
                        LanguageHelper.GetTextContent(attrData.name);
                    goAttr.FindChildByName("Text_Num").GetComponent<Text>().text =
                        Sys_Attr.Instance.GetAttrValue(attrData, value).ToString();
                }

                //FrameworkTool.ForceRebuildLayout(transPresentTemplate.parent.gameObject);
                
                upExp = curData.exp;
                sliderExp.value = curExp * 1.0f / upExp;
                txtExpPercent.text = string.Format("{0}/{1}", curExp.ToString(), upExp.ToString());
            }
            
            CSVSlotUpgrade.Data nextData = Sys_Equip.Instance.GetEquipSlotData((uint)curSlot, curLevel + 1);
            if (nextData != null)
            {
                FrameworkTool.DestroyChildren(transNextTemplate.parent.gameObject, transNextTemplate.name);
                int count = nextData.attr.Count;
                for (int i = 0; i < count; ++i)
                {
                    GameObject goAttr = GameObject.Instantiate(transNextTemplate.gameObject);
                    goAttr.transform.SetParent(transNextTemplate.parent);
                    goAttr.transform.localScale = Vector3.one;
                    goAttr.transform.localPosition = new Vector3(goAttr.transform.localPosition.x, goAttr.transform.localPosition.y, 0f);
                    goAttr.SetActive(true);

                    uint attrId = nextData.attr[i][0];
                    uint value = nextData.attr[i][1];
                    CSVAttr.Data attrData = CSVAttr.Instance.GetConfData(attrId);
                    goAttr.FindChildByName("Text_Property").GetComponent<Text>().text =
                        LanguageHelper.GetTextContent(attrData.name);
                    goAttr.FindChildByName("Text_Num").GetComponent<Text>().text =
                        Sys_Attr.Instance.GetAttrValue(attrData, value).ToString();
                }
                
                //FrameworkTool.ForceRebuildLayout(transNextTemplate.parent.gameObject);
            }

            lvLimit = curLevel >= Sys_Role.Instance.Role.Level;
            ImageHelper.SetImageGray(btnUpgrade.image, lvLimit);

            txtLeftTimeTip.text = LanguageHelper.GetTextContent(4643, Sys_Equip.Instance.slotLowMatTimes.ToString());
        }

        public void SetData(ulong matUid)
        {
            curMatUid = matUid;
            ItemData item = Sys_Bag.Instance.GetItemDataByUuid(matUid);
            PropIconLoader.ShowItemData costData = new PropIconLoader.ShowItemData(item.Id, 1, true, false, false, false, false, false, true, 
                true, OnClickPropItem, false, false);
            costData.SetQuality(item.Quality);
            costData.SetMarketEnd(item.bMarketEnd);
            propItem.SetData(new MessageBoxEvt( EUIID.UI_EquipSlot_Upgrade, costData));

            CSVEquipment.Data data = CSVEquipment.Instance.GetConfData(item.Id);
            if (data != null)
            {
                uint expendId = Sys_Equip.Instance.GetEquipRebuildExpendId(data.equipment_level, data.equipment_type, (uint)item.Equip.Score);
                CSVRebuildExpend.Data temp = CSVRebuildExpend.Instance.GetConfData(expendId);
                    
                txtExpUp.text = LanguageHelper.GetTextContent(4268, temp.exp_add.ToString());
            }
            
            propItem.transform.gameObject.SetActive(true);
            btnPropAdd.gameObject.SetActive(false);
        }

        private void OnClickPropItem(PropItem item)
        {
            curMatUid = 0;
            propItem.transform.gameObject.SetActive(false);
            btnPropAdd.gameObject.SetActive(true);
            txtExpUp.text = "";
        }
    }
}


