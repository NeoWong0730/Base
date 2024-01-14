using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using Table;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using System.Text;

namespace Logic
{
    
    public class UI_PetMagicCore_MakePreview : UIBase
    {
        public class PetMagicCoreEffectCeil
        {
            private Text name;
            private Text des;
            public void Init(Transform transform)
            {
                name = transform.Find("Name").GetComponent<Text>();
                des = transform.Find("Value").GetComponent<Text>();
            }

            public void SetView(uint effectId)
            {
                var petEquipEffect = CSVPetEquipEffect.Instance.GetConfData(effectId);
                if (null != petEquipEffect)
                {
                    var passiveSkillInfo = CSVPassiveSkillInfo.Instance.GetConfData(petEquipEffect.effect);
                    if (null != passiveSkillInfo)
                    {
                        TextHelper.SetText(name, passiveSkillInfo.name);
                        TextHelper.SetText(des, passiveSkillInfo.desc);
                    }
                    else
                    {
                        DebugUtil.LogError($"Not Find Id = {petEquipEffect.effect} In Table CSVPassiveSkillInfo");
                    }
                }
                else
                {
                    DebugUtil.LogError($"Not Find Id = {effectId} In Table CSVPetEquipEffect");
                }
            }
        }
       
        private Image m_PetEquipIcon;
        private Text m_PetEquipName;
        private Text m_PetEquipTeyp;
        private Text m_EquipLevel;
        private Text skillFashionText;
        private Text m_EmptyContent;
        private Text m_BaseAttrCountText;
        private Button m_CloseButton;

        private Transform m_AttrTitleParent;
        private Transform m_QualityParent;
        private Transform m_AttrRightGreenParent;
        private Transform m_AttrRightEffectParent;
        private Transform m_AttrEmpty;
        private GameObject m_SkllRoot;
        private GameObject m_EffectRoot;
        

        private InfinityGrid effectInfinityGrid;
        private InfinityGrid skillInfinityGrid;

        private uint m_PetEquipId;
        List<uint> skillIdList = new List<uint>(8);
        List<uint> effectId = new List<uint>(8);
        //private CSVFormula.Data cSVFormulaData;
        private CSVPetEquip.Data cSvPetEquip;

        private List<Toggle> m_Toggles = new List<Toggle>();

        protected override void OnLoaded()
        {
            m_PetEquipIcon = transform.Find("Animator/Content/Image_Equip/PropItem/Btn_Item/Image_Icon").GetComponent<Image>();
            m_PetEquipName = transform.Find("Animator/Content/Image_Equip/Name").GetComponent<Text>();
            m_PetEquipTeyp = transform.Find("Animator/Content/Image_Equip/Type/Value").GetComponent<Text>();
            m_EquipLevel = transform.Find("Animator/Content/Image_Equip/Level/Value").GetComponent<Text>();
            m_CloseButton = transform.Find("Animator/Image_BG/Btn_Close").GetComponent<Button>();

            m_AttrTitleParent = transform.Find("Animator/Content/List/Layout_Title");
            m_QualityParent = transform.Find("Animator/Content/List/Scroll View/Viewport/Content");
            m_AttrRightGreenParent = transform.Find("Animator/Content/Attribute/Content/Scroll View2/Viewport/Content");
            m_AttrRightEffectParent = transform.Find("Animator/Content/Attribute/Content/Scroll View/Viewport/Content");
            m_AttrEmpty = transform.Find("Animator/Content/Attribute/Empty");
            effectInfinityGrid = transform.Find("Animator/Content/Attribute/View_Effect/Scroll View").GetComponent<InfinityGrid>();
            effectInfinityGrid.onCellChange += OnEffectCellChange;
            effectInfinityGrid.onCreateCell += OnEffectCreateCell;;
            skillInfinityGrid = transform.Find("Animator/Content/Attribute/View_Set/Scroll View").GetComponent<InfinityGrid>();
            skillInfinityGrid.onCellChange += OnCellChange;
            skillInfinityGrid.onCreateCell += OnCreateCell; 
            m_SkllRoot = transform.Find("Animator/Content/Attribute/View_Set").gameObject;
            m_EffectRoot = transform.Find("Animator/Content/Attribute/View_Effect").gameObject;
            m_EmptyContent = transform.Find("Animator/Content/Attribute/Empty/View_Tips/Image_Bg03/Text_Tips").GetComponent<Text>();
            skillFashionText = transform.Find("Animator/Content/Attribute/View_Set/Text_Des").GetComponent<Text>();
            m_BaseAttrCountText = transform.Find("Animator/Content/List/Tip1/Text").GetComponent<Text>();

            for (int i = 0; i < 5; i++)
            {
                Toggle toggle = m_QualityParent.GetChild(i).GetChild(4).Find("Tg").GetComponent<Toggle>();
                m_Toggles.Add(toggle);

                Toggle toggle1 = m_QualityParent.GetChild(i).GetChild(5).Find("Tg").GetComponent<Toggle>();
                m_Toggles.Add(toggle1);
            }

            for (int i = 0; i < m_Toggles.Count; i++)
            {
                int index = i;
                m_Toggles[i].onValueChanged.AddListener((arg) => OnValueChanged(arg, index));
            }

            m_CloseButton.onClick.AddListener(OnCloseUI);
        }

        private void OnCloseUI()
        {
            UIManager.CloseUI(EUIID.UI_PetMagicCore_MakePreview);
        }

        protected override void OnOpen(object arg)
        {
            m_PetEquipId = Convert.ToUInt32(arg);
            cSvPetEquip = CSVPetEquip.Instance.GetConfData(m_PetEquipId);
        }

        protected override void OnShow()
        {
            RefreshTitle();
            //SetProbabilty();
            RefreshAttr();
            if (null != cSvPetEquip.special_range)
            {
                m_Toggles[8].isOn = true;
                m_Toggles[8].transform.Find("Mask").gameObject.SetActive(true);
                ShowSpecial(5);
            }
        }

        private void OnValueChanged(bool arg, int i)
        {
            if (arg)
            {
                m_Toggles[i].transform.Find("Mask").gameObject.SetActive(true);
                int index = i % 2;
                if (index == 0)//偶数 特效
                {
                    index = i / 2;
                    ShowSpecial(index + 1);
                    
                }
                else
                {
                    index = i / 2;
                    ShowSkillInfo((uint)index + 1);
                }
            }
            else
            {
                m_Toggles[i].transform.Find("Mask").gameObject.SetActive(false);
            }
        }

        private void RefreshTitle()
        {
            CSVItem.Data petEquipItemData = CSVItem.Instance.GetConfData(m_PetEquipId);
            if (null != petEquipItemData)
            {
                ImageHelper.SetIcon(m_PetEquipIcon, petEquipItemData.icon_id);
                TextHelper.SetText(m_PetEquipName, petEquipItemData.name_id);
            }

            CSVPetEquip.Data CSVPetEquipData = CSVPetEquip.Instance.GetConfData(m_PetEquipId);
            if(null != CSVPetEquipData)
            {
                TextHelper.SetText(m_PetEquipTeyp, 680010000u + CSVPetEquipData.equipment_category);
                TextHelper.SetText(m_EquipLevel, CSVPetEquipData.equipment_level.ToString());
                TextHelper.SetText(m_BaseAttrCountText, 680000910, CSVPetEquipData.attr_num.ToString());
            }
        }

        private void RefreshAttr()
        {
            List<List<uint>> attrNameId = Sys_Pet.Instance.GetPetEquipPreviewAttrIdById(m_PetEquipId);

            int attrCount = attrNameId.Count;
            if (attrCount == 0)
            {
                for (int i = 0; i < 4; i++)
                {
                    m_AttrTitleParent.GetChild(i).gameObject.SetActive(false);
                }
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        m_QualityParent.GetChild(j).gameObject.SetActive(false);
                    }
                }
            }
            else if (attrCount == 1)
            {
                for (int i = 0; i < 4; i++)
                {
                    Transform attrTrans = m_AttrTitleParent.GetChild(i);
                    Text name = attrTrans.Find("Text").GetComponent<Text>();
                    if (i == 0)
                    {
                        attrTrans.gameObject.SetActive(true);
                        TextHelper.SetText(name, CSVAttr.Instance.GetConfData(attrNameId[i][0]).name);
                    }
                    else
                    {
                        attrTrans.gameObject.SetActive(false);
                    }
                }
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if (j == 0)
                        {
                            m_QualityParent.GetChild(i).GetChild(j).gameObject.SetActive(true);
                        }
                        else
                        {
                            m_QualityParent.GetChild(i).GetChild(j).gameObject.SetActive(false);
                        }
                    }
                }
            }
            else if (attrCount == 2)
            {
                for (int i = 0; i < 4; i++)
                {
                    Transform attrTrans = m_AttrTitleParent.GetChild(i);
                    Text name = attrTrans.Find("Text").GetComponent<Text>();
                    if (i >= 2)
                    {
                        attrTrans.gameObject.SetActive(false);
                    }
                    else
                    {
                        attrTrans.gameObject.SetActive(true);
                        TextHelper.SetText(name, CSVAttr.Instance.GetConfData(attrNameId[i][0]).name);
                    }
                }
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if (j >= 2)
                        {
                            m_QualityParent.GetChild(i).GetChild(j).gameObject.SetActive(false);
                        }
                        else
                        {
                            m_QualityParent.GetChild(i).GetChild(j).gameObject.SetActive(true);
                        }
                    }
                }
            }
            else if (attrCount == 3)
            {
                for (int i = 0; i < 4; i++)
                {
                    Transform attrTrans = m_AttrTitleParent.GetChild(i);
                    Text name = attrTrans.Find("Text").GetComponent<Text>();
                    if (i >= 3)
                    {
                        attrTrans.gameObject.SetActive(false);
                    }
                    else
                    {
                        attrTrans.gameObject.SetActive(true);
                        TextHelper.SetText(name, CSVAttr.Instance.GetConfData(attrNameId[i][0]).name);
                    }
                }
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        if (j >= 3)
                        {
                            m_QualityParent.GetChild(i).GetChild(j).gameObject.SetActive(false);
                        }
                        else
                        {
                            m_QualityParent.GetChild(i).GetChild(j).gameObject.SetActive(true);
                        }
                    }
                }
            }
            else if (attrCount == 4)
            {
                for (int i = 0; i < 4; i++)
                {
                    Transform attrTrans = m_AttrTitleParent.GetChild(i);
                    Text name = attrTrans.Find("Text").GetComponent<Text>();
                    attrTrans.gameObject.SetActive(true);
                    TextHelper.SetText(name, CSVAttr.Instance.GetConfData(attrNameId[i][0]).name);
                }
                for (int i = 0; i < 5; i++)
                {
                    for (int j = 0; j < 4; j++)
                    {
                        m_QualityParent.GetChild(i).GetChild(j).gameObject.SetActive(true);
                    }
                }
            }


            for (int i = 0; i < 5; i++)
            {
                CSVPetEquipQualityParameter.Data cSVQualityParameterData = CSVPetEquipQualityParameter.Instance.GetConfData((uint)i + 1);
                uint lower_Limit = cSVQualityParameterData.base_cor[0];//下限系数
                uint upper_Limit = cSVQualityParameterData.base_cor[1];//上限系数
                for (int j = 0; j < attrCount; j++)
                {
                    List<uint> attr = attrNameId[j];
                    string lowStr = GetAttrStr(attr[0], attr[1] * lower_Limit / 10000);
                    string highStr = GetAttrStr(attr[0], attr[2] * upper_Limit / 10000);
                    Text attrText = m_QualityParent.GetChild(i).GetChild(j).Find("Text").GetComponent<Text>();
                    attrText.text = string.Format(CSVLanguage.Instance.GetConfData(2010044).words, lowStr, highStr);
                }
            }

            if (null == cSvPetEquip.special_range)
            {
                m_AttrTitleParent.GetChild(4).gameObject.SetActive(false);
                for (int i = 0; i < 5; i++)
                {
                    m_QualityParent.GetChild(i).GetChild(4).gameObject.SetActive(false);
                }
            }
            else
            {
                m_AttrTitleParent.GetChild(4).gameObject.SetActive(true);
                for (int i = 0; i < 5; i++)
                {
                    m_QualityParent.GetChild(i).GetChild(4).gameObject.SetActive(true);
                    Text green = m_QualityParent.GetChild(i).GetChild(4).Find("Text").GetComponent<Text>();
                    CSVPetEquipQualityParameter.Data cSVQualityParameterData = CSVPetEquipQualityParameter.Instance.GetConfData((uint)i + 1);
                    TextHelper.SetText(green, LanguageHelper.GetTextContent(2010039, cSVQualityParameterData.special_weight[0][0].ToString(),
                        cSVQualityParameterData.special_weight[cSVQualityParameterData.special_weight.Count - 1][0].ToString()));
                }
            }

            if (cSvPetEquip.skill == 0)
            {
                m_AttrTitleParent.GetChild(5).gameObject.SetActive(false);
                for (int i = 0; i < 5; i++)
                {
                    m_QualityParent.GetChild(i).GetChild(5).gameObject.SetActive(false);
                }
            }
            else
            {
                m_AttrTitleParent.GetChild(5).gameObject.SetActive(true);
                for (int i = 0; i < 5; i++)
                {
                    m_QualityParent.GetChild(i).GetChild(5).gameObject.SetActive(true);
                    Text effect = m_QualityParent.GetChild(i).GetChild(5).Find("Text").GetComponent<Text>();
                    CSVPetEquipQualityParameter.Data cSVQualityParameterData = CSVPetEquipQualityParameter.Instance.GetConfData((uint)i + 1);
                    var low = cSVQualityParameterData.skill_weight / 10000.0f > 1 ? 1 : 0;
                    var high = cSVQualityParameterData.skill_weight / 10000.0f > 0 ? 1 : 0;
                    TextHelper.SetText(effect, LanguageHelper.GetTextContent(2010039, low.ToString(),
                       high.ToString()));
                }
            }

            /*if (cSvPetEquip.green_id == 0 && cSvPetEquip.special_id == 0)
            {
                m_AttrNotEmpty.gameObject.SetActive(false);
                m_AttrEmpty.gameObject.SetActive(true);
                TextHelper.SetText(m_EmptyContent, 2010167);
            }
            else
            {
                m_AttrNotEmpty.gameObject.SetActive(true);
                m_AttrEmpty.gameObject.SetActive(false);
            }*/
            FrameworkTool.ForceRebuildLayout(m_AttrTitleParent.gameObject);
            FrameworkTool.ForceRebuildLayout(m_QualityParent.gameObject);
        }

        private string GetAttrStr(uint attr, uint value)
        {
            CSVAttr.Data cSVAttrData = CSVAttr.Instance.GetConfData(attr);
            if (cSVAttrData.show_type == 1)
            {
                return string.Format("{0}", value);
            }
            else
            {
                return string.Format("{0}%", value / 100f);
            }
        }

        

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            PetSkillCeil entry = cell.mUserData as PetSkillCeil;
            uint skillId = skillIdList[index];
            entry.SetData(skillId, false, false, showLevel: true);
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            PetSkillCeil entry = new PetSkillCeil();
            GameObject go = cell.mRootTransform.gameObject;
            entry.BingGameObject(go);
            entry.AddClickListener(OnSkillSelect);
            cell.BindUserData(entry);
        }

        private void OnSkillSelect(PetSkillCeil petSkillCeil)
        {
            if (petSkillCeil.petSkillBase.skillId != 0)
            {
                UIManager.OpenUI(EUIID.UI_Skill_Tips, false, new Tuple<uint, uint>(petSkillCeil.petSkillBase.skillId, 0));
            }

        }

        /// <summary>
        /// 滚动列表创建回调
        /// </summary>
        /// <param name="cell"></param>
        public void OnEffectCreateCell(InfinityGridCell cell)
        {
            PetMagicCoreEffectCeil entry = new PetMagicCoreEffectCeil();
            GameObject go = cell.mRootTransform.gameObject;
            entry.Init(go.transform);
            cell.BindUserData(entry);
        }

        /// <summary>
        /// 滚动列表滚动回调
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="index"></param>
        public void OnEffectCellChange(InfinityGridCell cell, int index)
        {
            if (index < 0 || index >= effectId.Count)
                return;
            PetMagicCoreEffectCeil entry = cell.mUserData as PetMagicCoreEffectCeil;
            entry.SetView(effectId[index]);
        }

        private void ShowSkillInfo(uint quality)
        {
            m_EffectRoot.SetActive(false);
            m_SkllRoot.SetActive(true);

            CSVPetEquip.Data CSVPetEquipData = CSVPetEquip.Instance.GetConfData(m_PetEquipId);
            CSVPetEquipQualityParameter.Data cSVQualityParameterData = CSVPetEquipQualityParameter.Instance.GetConfData(quality);
            skillIdList.Clear();
            var petIds = new List<uint>();
            foreach (var item in CSVPetEquipSuitSkill.Instance.GetAll())
            {
                if (item.group_id == CSVPetEquipData.skill && cSVQualityParameterData.skill_weight > 0)
                {
                    var fashionData = CSVPetEquipSuitAppearance.Instance.GetConfData(item.appearance_id);
                    if(null != fashionData)
                    {
                        petIds.AddRange(fashionData.pet_id);
                    }
                    skillIdList.Add(item.upgrade_skill);
                }
            }
            
            TextHelper.SetText(m_EmptyContent, 1018422);
            int needCount = skillIdList.Count;
            skillInfinityGrid.CellCount = needCount;
            skillInfinityGrid.ForceRefreshActiveCell();
            if (needCount == 0)
            {
                m_SkllRoot.SetActive(false);
                m_AttrEmpty.gameObject.SetActive(true);
                TextHelper.SetText(m_EmptyContent, 1018422);
                return;
            }
            StringBuilder stringBuilder = StringBuilderPool.GetTemporary();
            for (int i = 0; i < petIds.Count; i++)
            {
                CSVPetNew.Data pet = CSVPetNew.Instance.GetConfData(petIds[i]);
                stringBuilder.Append(LanguageHelper.GetTextContent(pet.name));
                if (i != petIds.Count - 1)
                {
                    stringBuilder.Append("、");
                }
            }
            TextHelper.SetText(skillFashionText, 680000915, StringBuilderPool.ReleaseTemporaryAndToString(stringBuilder));
            m_AttrEmpty.gameObject.SetActive(false);
        }

        private void ShowSpecial(int quality)
        {
            m_EffectRoot.SetActive(true);
            m_SkllRoot.SetActive(false);
            CSVPetEquip.Data CSVPetEquipData = CSVPetEquip.Instance.GetConfData(m_PetEquipId);
            uint specialId = CSVPetEquipData.special_range[quality - 1];
            effectId.Clear();
            foreach (var item in CSVPetEquipEffect.Instance.GetAll())
            {
                if (item.group_id == specialId)
                {
                    effectId.Add(item.id);
                }
            }
            int needCount = effectId.Count;
            effectInfinityGrid.CellCount = needCount;
            effectInfinityGrid.ForceRefreshActiveCell();
            if (needCount == 0)
            {
                m_EffectRoot.gameObject.SetActive(false);
                m_AttrEmpty.gameObject.SetActive(true);
                TextHelper.SetText(m_EmptyContent, 1018419);
                return;
            }
            m_AttrEmpty.gameObject.SetActive(false);
        }
    }
}


