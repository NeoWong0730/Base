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
    public class UI_Filed_Upgrade_Left
    {
        public class SkillItem
        {
            private Transform transform;
            private CP_Toggle toggle;
            private Image imgQuality;
            private Image icon;
            private Action<int> _action;
            private uint _slotId;
            private int intIndex;

            public void Init(Transform trans)
            {
                transform = trans;
                toggle = transform.GetComponent<CP_Toggle>();
                toggle.onValueChanged.AddListener(OnClickToggle);
                imgQuality = transform.Find("Image_Quality").GetComponent<Image>();
                icon = transform.Find("Icon").GetComponent<Image>();
            }

            private void OnClickToggle(bool isOn)
            {
                if (isOn)
                {
                    _action?.Invoke(intIndex);
                }
            }

            public void Register(Action<int> action)
            {
                _action = action;
            }

            public void OnSelect(bool isSelect)
            {
                toggle.SetSelected(isSelect, true);
            }

            public void SetIndex(int index)
            {
                intIndex = index;
            }

            public void UpdateInfo(uint slotId)
            {
                _slotId = slotId;
                CSVUpgradeEffect.Data temp = CSVUpgradeEffect.Instance.GetConfData(_slotId);
                uint colorId = 0;
                switch (temp.quality)
                {
                    case 3: { colorId = 838; } break;
                    case 4: { colorId = 837; } break;
                    case 5: {colorId = 839; } break;
                }
                string[] _strs1 = CSVParam.Instance.GetConfData(colorId).str_value.Split('|');
                imgQuality.color = new Color(float.Parse(_strs1[0]) / 255f, float.Parse(_strs1[1]) / 255f, float.Parse(_strs1[2]) / 255f, float.Parse(_strs1[3]) / 255f);
                ImageHelper.SetIcon(imgQuality, 992901, true);
                
                uint skillId = Sys_Equip.Instance.GetSlotEffect(intIndex);
                if (skillId != 0)
                {
                    icon.gameObject.SetActive(true);
                    CSVPassiveSkillInfo.Data data = CSVPassiveSkillInfo.Instance.GetConfData(skillId);
                    if (data != null)
                    {
                        ImageHelper.SetIcon(icon, data.icon);
                    }
                }
                else
                {
                    icon.gameObject.SetActive(false);
                }
            }
        }

        private Transform transform;
        
        //skill 
        private Transform transSkill;
        private Button btnSkillPreview;
        private InfinityGrid _infinityGrid;
        
        //des
        private Transform transDesSkill;
        private Image imgDesIcon;
        private Text txtDesTitle;
        private Text txtDesName;
        private Text txtDesInfo;
        private Button btnDesClear;
        
        //lock
        private Transform transLock;
        private Text txtLockTitle;
        private Text txtLockInfo;
        
        private List<uint> listSkillSlots = new List<uint>();
        private int selectIndex;

        public void Init(Transform trans)
        {
            transform = trans;

            transSkill = transform.Find("Skill");
            btnSkillPreview = transform.Find("Skill/Button").GetComponent<Button>();
            btnSkillPreview.onClick.AddListener(OnClickSkillPreview);
            _infinityGrid = transform.Find("Skill/Scroll View").GetComponent<InfinityGrid>();
            _infinityGrid.onCreateCell += OnCreateCell;
            _infinityGrid.onCellChange += OnCellChange;

            transDesSkill = transform.Find("Des/Skillbg");
            imgDesIcon = transform.Find("Des/Skillbg/Icon").GetComponent<Image>();
            txtDesTitle = transform.Find("Des/Text_Title").GetComponent<Text>();
            txtDesName = transform.Find("Des/Text_Name").GetComponent<Text>();
            txtDesInfo = transform.Find("Des/Text_Description").GetComponent<Text>();
            btnDesClear = transform.Find("Des/Button_Clear").GetComponent<Button>();
            btnDesClear.onClick.AddListener(OnClickSkillReset);

            transLock = transform.Find("Lock");
            txtLockTitle = transform.Find("Lock/Text1").GetComponent<Text>();
            txtLockInfo = transform.Find("Lock/Text1/Text_Tips02").GetComponent<Text>();
        }

        private void OnClickSkillPreview()
        {
            UIManager.OpenUI(EUIID.UI_EquipSlot_Upgrade_SkillPreview);
        }
        
        private void OnCreateCell(InfinityGridCell cell)
        {
            SkillItem entry = new SkillItem();
            entry.Init(cell.mRootTransform);
            entry.Register(OnSelectSkill);
            entry.OnSelect(false);

            cell.BindUserData(entry);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            SkillItem entry = cell.mUserData as SkillItem;
            entry.SetIndex(index);
            entry.UpdateInfo(listSkillSlots[index]);
            entry.OnSelect(index == selectIndex);
        }

        private void OnSelectSkill(int index)
        {
            if (selectIndex != index)
            {
                selectIndex = index;
            }
            
            CSVUpgradeEffect.Data temp = CSVUpgradeEffect.Instance.GetConfData(listSkillSlots[selectIndex]);
            uint skillId = Sys_Equip.Instance.GetSlotEffect(selectIndex);
            if (skillId != 0)
            {
                transDesSkill.gameObject.SetActive(true);
                //txtDesTitle.text = "";

                CSVPassiveSkillInfo.Data data = CSVPassiveSkillInfo.Instance.GetConfData(skillId);
                if (data != null)
                {
                    ImageHelper.SetIcon(imgDesIcon, data.icon);
                    txtDesName.text = LanguageHelper.GetTextContent(data.name);
                    txtDesInfo.text = LanguageHelper.GetTextContent(data.desc);
                }
                
                btnDesClear.gameObject.SetActive(true);
            }
            else
            {
                transDesSkill.gameObject.SetActive(false);
                txtDesTitle.text = "";
                txtDesName.text = "";
                txtDesInfo.text = LanguageHelper.GetTextContent(4235,  LanguageHelper.GetTextContent(590001807 + temp.quality - 3));
                btnDesClear.gameObject.SetActive(false);
            }
        }

        private void OnClickSkillReset()
        {
            uint id = listSkillSlots[selectIndex];
            CSVUpgradeEffect.Data data = CSVUpgradeEffect.Instance.GetConfData(id);
            uint itemId = data.effect_beg_max[0];
            uint num = data.effect_beg_max[1];
            CSVItem.Data itemInfo = CSVItem.Instance.GetConfData(itemId);
            PromptBoxParameter.Instance.Clear();
            PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(4259, LanguageHelper.GetTextContent(itemInfo.name_id), num.ToString());
            PromptBoxParameter.Instance.SetConfirm(true, ()=>
            {
                Sys_Equip.Instance.OnBodyEnhanceEffectRefreshReq(listSkillSlots[selectIndex]);
            });
            PromptBoxParameter.Instance.SetCancel(true, null);
            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
        }

        public void UpdateInfo()
        {
            selectIndex = 0;
            RefreshSelect();
        }

        public void RefreshSelect()
        {
            //技能列表
            listSkillSlots.Clear();
            int count = CSVUpgradeEffect.Instance.Count;
            for (int i = 0; i < count; ++i)
            {
                CSVUpgradeEffect.Data temp = CSVUpgradeEffect.Instance.GetByIndex(i);
                listSkillSlots.Add(temp.id);
            }
            _infinityGrid.CellCount = listSkillSlots.Count;
            _infinityGrid.ForceRefreshActiveCell();
            _infinityGrid.MoveToIndex(selectIndex);
            
            //下一级解锁条件
            uint curLv = Sys_Equip.Instance.GetSlotsMinLevel();
            CSVUpgradeEffect.Data nextData = null;
            for (int i = 0; i < count; ++i)
            {
                CSVUpgradeEffect.Data temp = CSVUpgradeEffect.Instance.GetByIndex(i);
                if (curLv < temp.lev)
                {
                    nextData = temp;
                    break;
                }
            }

            if (nextData != null)
            {
                transLock.gameObject.SetActive(true);
                txtLockInfo.text =
                    LanguageHelper.GetTextContent(4234, nextData.lev.ToString(), LanguageHelper.GetTextContent(590001807 + nextData.quality - 3));
            }
            else
            {
                transLock.gameObject.SetActive(false);
            }
        }
    }
}


