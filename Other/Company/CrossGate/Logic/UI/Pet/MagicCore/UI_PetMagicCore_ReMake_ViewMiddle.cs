using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;
using Lib.Core;
using System;
using Packet;

namespace Logic
{
    public class UI_PetMagicCore_ReMake_ViewMiddle : UIComponent
    {
        private ItemData itemData;
        private Image iconImage;
        private Button iconBtn;
        private Text skilInfoText;
        private Text petFashionText;
        private Text nameText;
        private Button remakeBtn;
        private PropItem remakeSelectItem;
        private uint selectItemId;

        private GameObject effectGo;
        #region 系统函数
        protected override void Loaded()
        {
            Parse();
        }

        public override void Show()
        {
            base.Show();
        }
        public override void Hide()
        {
            selectItemId = 0;
            effectGo?.SetActive(false);
            base.Hide();
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Pet.Instance.eventEmitter.Handle<uint>(Sys_Pet.EEvents.OnSelectItem, OnUpgradeTargetSeleted, toRegister);
        }
        public override void OnDestroy()
        {
            base.OnDestroy();
        }
        #endregion

        #region func
        private void Parse()
        {
            iconImage = transform.Find("Item_Icon").GetComponent<Image>();
            iconBtn = iconImage.GetComponent<Button>();
            iconBtn.onClick.AddListener(OnEquipBtnCliced);
            nameText = transform.Find("Text_Name").GetComponent<Text>();
            skilInfoText = transform.Find("View_Left/grid/Item/Text_Value").GetComponent<Text>();
            petFashionText = transform.Find("View_Left/grid/Item (1)/Text_Value").GetComponent<Text>();

            remakeSelectItem = new PropItem();
            remakeSelectItem.BindGameObject(transform.Find("Btn_Item/PropItem").gameObject);
            PropIconLoader.ShowItemData itemN = new PropIconLoader.ShowItemData(0, 0, false, false, false, false, false, false, false, true, ItemGridBeClicked, _bUseTips: false);
            remakeSelectItem.SetData(itemN, EUIID.UI_PetMagicCore);
            remakeSelectItem.btnNone.gameObject.SetActive(true);
            remakeSelectItem.Layout.imgQuality.gameObject.SetActive(false);
            effectGo = transform.Find("bg/FX_ui_PetMagicCore_View_Artifice").gameObject;

            remakeBtn = transform.Find("Btn_Artifice").GetComponent<Button>();
            remakeBtn.onClick.AddListener(OnBtnRecastClick);
        }

        public void UpdateView()
        {
            SetSelectItemState();
            iconImage.gameObject.SetActive(true);
            ImageHelper.SetIcon(iconImage, itemData.cSVItemData.icon_id);
            nameText.text = LanguageHelper.GetTextContent(itemData.cSVItemData.name_id);
            UpdateEquipInfoView();
        }

        public void UpdateView(ItemData item)
        {
            SetSelectItemState();
            itemData = item;
            iconImage.gameObject.SetActive(true);
            ImageHelper.SetIcon(iconImage, itemData.cSVItemData.icon_id);
            nameText.text = LanguageHelper.GetTextContent(itemData.cSVItemData.name_id);
            UpdateEquipInfoView();
        }

        private void UpdateEquipInfoView()
        {
            CSVPetEquipSuitSkill.Data suitSkillData = CSVPetEquipSuitSkill.Instance.GetConfData(itemData.petEquip.SuitSkill);
            if (null != suitSkillData)
            {
                bool isActiveSkill = Sys_Skill.Instance.IsActiveSkill(suitSkillData.upgrade_skill);
                if (isActiveSkill) //主动技能
                {
                    CSVActiveSkillInfo.Data skillInfo = CSVActiveSkillInfo.Instance.GetConfData(suitSkillData.upgrade_skill);
                    if (skillInfo != null)
                    {
                        TextHelper.SetText(skilInfoText, LanguageHelper.GetTextContent(680000911, LanguageHelper.GetTextContent(skillInfo.name)));
                    }
                    else
                    {
                        Debug.LogErrorFormat("not found skillId={0}", suitSkillData.upgrade_skill);
                    }
                }
                else
                {
                    CSVPassiveSkillInfo.Data skillInfo = CSVPassiveSkillInfo.Instance.GetConfData(suitSkillData.upgrade_skill);
                    if (skillInfo != null)
                    {
                        TextHelper.SetText(skilInfoText, LanguageHelper.GetTextContent(680000911, LanguageHelper.GetTextContent(skillInfo.name)));
                    }
                    else
                    {
                        Debug.LogErrorFormat("not found skillId={0}", suitSkillData.upgrade_skill);
                    }
                }
            }
            

            uint suitAppearanceId = itemData.petEquip.SuitAppearance;
            bool hasSuitAppearanceId = suitAppearanceId > 0;
            if (hasSuitAppearanceId)
            {
                CSVPetEquipSuitAppearance.Data suitAppearanceData = CSVPetEquipSuitAppearance.Instance.GetConfData(suitAppearanceId);
                if (null != suitAppearanceData)
                {
                    TextHelper.SetText(petFashionText, LanguageHelper.GetTextContent(680000913, LanguageHelper.GetTextContent(suitAppearanceData.name)));
                }
            }
            else
            {
                TextHelper.SetText(petFashionText, 10915);
            }
        }

        #endregion

        #region event
        private void OnUpgradeTargetSeleted(uint selectItem)
        {
            selectItemId = selectItem;
            SetSelectItemState();
        }

        private void ItemGridBeClicked(PropItem selectItem)
        {
            UIManager.OpenUI(EUIID.UI_PetSelectMagicMakeItem, false, new Tuple<uint, uint, uint>(0, 1018204, (uint)EItemType.PetEquipSmeltItem));
        }

        private void SetSelectItemState()
        {
            bool hasItem = selectItemId > 0;
            uint needitemCount = 1;
            if(selectItemId == 0)
            {
                needitemCount = 0;
            }
            else
            {
                CSVPetEquip.Data petEquip = CSVPetEquip.Instance.GetConfData(itemData.Id);
                if (null != petEquip && null != petEquip.smelt && petEquip.smelt.Count >= 2)
                {
                    if (petEquip.smelt[0] == selectItemId)
                    {
                        needitemCount = petEquip.smelt[1];
                    }
                }
            }
            

            PropIconLoader.ShowItemData itemN = new PropIconLoader.ShowItemData(selectItemId, needitemCount, true, false, false, false, false, true, true, true, ItemGridBeClicked, _bUseTips: false);
            remakeSelectItem.SetData(itemN, EUIID.UI_PetMagicCore);
            remakeSelectItem.txtNumber.gameObject.SetActive(hasItem);
            remakeSelectItem.btnNone.gameObject.SetActive(!hasItem);
            remakeSelectItem.Layout.imgIcon.enabled = hasItem;
            remakeSelectItem.Layout.imgQuality.gameObject.SetActive(hasItem);
        }

        private void OnBtnRecastClick()
        {
            if (selectItemId == 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000906));
                return;
            }
            //安全锁
            if (Sys_Pet.Instance.IsPetEquipBeEffectWithSecureLock(itemData.petEquip))
                return;
            
            if (itemData.IsLocked)
            {
                PromptBoxParameter.Instance.Clear();
                PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(15248, LanguageHelper.GetTextContent(itemData.cSVItemData.name_id), LanguageHelper.GetTextContent(itemData.cSVItemData.name_id));
                PromptBoxParameter.Instance.SetConfirm(true, () =>
                {
                    Sys_Bag.Instance.OnItemLockReq(itemData.Uuid, false);
                });
                PromptBoxParameter.Instance.SetCancel(true, null);
                UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
                return;
            }

            CSVPetEquip.Data petEquip = CSVPetEquip.Instance.GetConfData(itemData.Id);
            if (null != petEquip && null != petEquip.smelt && petEquip.smelt.Count >= 2)
            {
                if(selectItemId == petEquip.smelt[0])
                {
                    ItemIdCount itemIdCount = new ItemIdCount(petEquip.smelt[0], petEquip.smelt[1]);
                    if (!itemIdCount.Enough)
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000907));
                        return;
                    }
                }
                else
                {
                    ItemIdCount itemIdCount = new ItemIdCount(selectItemId, 1);
                    if (!itemIdCount.Enough)
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(680000907));
                        return;
                    }
                }
            }
            
            CheckSuitFashionTips();
        }

        private void OnEquipBtnCliced()
        {
            if(null != itemData)
            {
                PetEquipTipsData petEquipTipsData = new PetEquipTipsData();
                petEquipTipsData.openUI = EUIID.UI_PetMagicCore;
                petEquipTipsData.petEquip = itemData;
                petEquipTipsData.isCompare = false;
                petEquipTipsData.isShowOpBtn = false;
                UIManager.OpenUI(EUIID.UI_Tips_PetMagicCore, false, petEquipTipsData);
            }
        }

        public void CheckSuitSkillTips()
        {
            if (itemData.petEquip.SuitSkill > 0)
            {
                if (!Sys_Pet.Instance.isSmeltSkillTips)
                {
                    UIManager.OpenUI(EUIID.UI_Pet_RemakeTips, false, 5u);
                }
                else
                {
                    effectGo?.SetActive(false);
                    effectGo?.SetActive(true);
                    Sys_Pet.Instance.ItemSmeltPetEquipReq(itemData.Uuid, Sys_Pet.Instance.GetPetEquipFitPetUid(itemData.Uuid), selectItemId);
                }
            }
            else
            {
                effectGo?.SetActive(false);
                effectGo?.SetActive(true);
                Sys_Pet.Instance.ItemSmeltPetEquipReq(itemData.Uuid, Sys_Pet.Instance.GetPetEquipFitPetUid(itemData.Uuid), selectItemId);
            }
        }

        public void OnSure()
        {
            effectGo?.SetActive(false);
            effectGo?.SetActive(true);
            Sys_Pet.Instance.ItemSmeltPetEquipReq(itemData.Uuid, Sys_Pet.Instance.GetPetEquipFitPetUid(itemData.Uuid), selectItemId);
        }

        public void CheckSuitFashionTips()
        {
            if (itemData.petEquip.SuitAppearance > 0)
            {
                if (!Sys_Pet.Instance.isSmeltFashionTips)
                {
                    UIManager.OpenUI(EUIID.UI_Pet_RemakeTips, false, 4u);
                }
                else
                {
                    CheckSuitSkillTips();
                }
            }
            else
            {
                CheckSuitSkillTips();
            }
        }
        
        #endregion

        
    }
}
