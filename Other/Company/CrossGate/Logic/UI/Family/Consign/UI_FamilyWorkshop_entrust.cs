using Lib.Core;
using Logic.Core;
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Framework;
using Table;
using UnityEngine.EventSystems;
using Packet;

namespace Logic
{
    public class UI_FamilyWorkshop_entrust : UIBase
    {
        private LivingSkill livingSkill;
        private uint curSelectedFormula;
        private CSVFormula.Data cSVFormulaData;
        private List<uint> m_DropDownSkillformulas;

        private Text m_Cost;
        private Toggle m_ShareToggle;
        private Button m_PreViewButton;
        private Button m_LSButton;
        private Button m_LSLvButton;
        private Button m_FormulaButton;

        private Transform m_LSDdParent;
        private Transform m_LSLvDdParent;
        private Transform m_FormulaDdParent;

        private GameObject m_LSDd_ArrowDown;
        private GameObject m_LSDd_ArrowUp;
        private GameObject m_LSLv_ArrowDown;
        private GameObject m_LSLv_ArrowUp;
        private GameObject m_Formula_ArrowDown;
        private GameObject m_Formula_ArrowUp;

        private Text m_LSParentName;
        private Text m_LSLvParentName;
        private Text m_FormulaParentName;

        private CP_ToggleRegistry m_LS_CP_ToggleRegistry;
        private CP_ToggleRegistry m_LSLv_CP_ToggleRegistry;
        private CP_ToggleRegistry m_Formula_CP_ToggleRegistry;

        private int m_LS_CurrentSelectIndex;            //从1开始
        private int m_LSLvCurrentSelectIndex;           //从1开始
        private int m_FormulaCurrentSelectIndex;        //从0开始

        private bool m_bLSDropDown;
        private bool bLsDropDown
        {
            get { return m_bLSDropDown; }
            set
            {
                if (m_bLSDropDown != value)
                {
                    m_bLSDropDown = value;
                    SetLSDrop(m_bLSDropDown);
                }
            }
        }


        private bool m_bLSLvDropDown;
        private bool bLSLvDropDown
        {
            get { return m_bLSLvDropDown; }
            set
            {
                if (m_bLSLvDropDown != value)
                {
                    m_bLSLvDropDown = value;
                    SetLSLvDrop(m_bLSLvDropDown);
                }
            }
        }


        private bool m_bFormulaDropDown;
        private bool bFormulaDropDown
        {
            get { return m_bFormulaDropDown; }
            set
            {
                if (m_bFormulaDropDown != value)
                {
                    m_bFormulaDropDown = value;
                    SetFormulaDrop(m_bFormulaDropDown);
                }
            }
        }


        private GameObject costItem1;
        private GameObject costItem2;
        private GameObject costItem3;
        private GameObject costItem4;
        private GameObject costBG1;
        private GameObject costBG2;
        private GameObject costBG3;
        private GameObject costBG4;
        private GameObject unFiexFormulaRoot;

        private GameObject costIntensify;
        private GameObject costIntensifyBG;
        private GameObject costIntensifyFlag;
        private Button costIntensifyBtn;
        private Button intensifyItem;
        private Image costIntensifyImage;
        private bool IsIntensifyEnough;

        private Text makeName;
        private Image furmulaIcon;

        private Button m_PublishButton;
        private Button m_CloseButton;

        private List<Button> itemBtns = new List<Button>();

        protected override void OnInit()
        {
            m_LS_CurrentSelectIndex = 1;
            m_LSLvCurrentSelectIndex = 1;
            m_FormulaCurrentSelectIndex = 0;
            livingSkill = Sys_LivingSkill.Instance.livingSkills[(uint)m_LS_CurrentSelectIndex];
            m_DropDownSkillformulas = Sys_LivingSkill.Instance.GetUnlockSkillFormulas_NotEquip(livingSkill.SkillId, (uint)m_LS_CurrentSelectIndex);
            RefreshData();
        }

        protected override void ProcessEvents(bool toRegister)
        {
            Sys_LivingSkill.Instance.eventEmitter.Handle(Sys_LivingSkill.EEvents.OnRefreshUnfixFormulaSelectItems, RefreshUnFixedFormulaGrids, toRegister);
        }


        protected override void OnLoaded()
        {
            m_Cost = transform.Find("Animator/View_Content/Text_Cost").GetComponent<Text>();
            m_ShareToggle = transform.Find("Animator/View_Content/Toggle").GetComponent<Toggle>();
            m_PreViewButton = transform.Find("Animator/View_Content/Btn_Preview").GetComponent<Button>();
            m_LSButton = transform.Find("Animator/View_Content/Type/Btn_Menu_Dark").GetComponent<Button>();
            m_LSLvButton = transform.Find("Animator/View_Content/Degree/Btn_Menu_Dark").GetComponent<Button>();
            m_FormulaButton = transform.Find("Animator/View_Content/Equip/Btn_Menu_Dark").GetComponent<Button>();

            m_LSDdParent = transform.Find("Animator/View_Content/Type/GridList");
            m_LSLvDdParent = transform.Find("Animator/View_Content/Degree/GridList");
            m_FormulaDdParent = transform.Find("Animator/View_Content/Equip/GridList");

            m_LSParentName = transform.Find("Animator/View_Content/Type/Btn_Menu_Dark/Text_Menu_Dark").GetComponent<Text>();
            m_LSLvParentName = transform.Find("Animator/View_Content/Degree/Btn_Menu_Dark/Text_Menu_Dark").GetComponent<Text>();
            m_FormulaParentName = transform.Find("Animator/View_Content/Equip/Btn_Menu_Dark/Text_Menu_Dark").GetComponent<Text>();

            m_LSDd_ArrowDown = transform.Find("Animator/View_Content/Type/Btn_Menu_Dark/Image_fold").gameObject;
            m_LSDd_ArrowUp = transform.Find("Animator/View_Content/Type/Btn_Menu_Dark/Image_fold01").gameObject;
            m_LSLv_ArrowDown = transform.Find("Animator/View_Content/Degree/Btn_Menu_Dark/Image_fold").gameObject;
            m_LSLv_ArrowUp = transform.Find("Animator/View_Content/Degree/Btn_Menu_Dark/Image_fold01").gameObject;
            m_Formula_ArrowDown = transform.Find("Animator/View_Content/Equip/Btn_Menu_Dark/Image_fold").gameObject;
            m_Formula_ArrowUp = transform.Find("Animator/View_Content/Equip/Btn_Menu_Dark/Image_fold01").gameObject;

            costItem1 = transform.Find("Animator/View_Content/Middle/Cost01/GameObject").gameObject;
            costItem2 = transform.Find("Animator/View_Content/Middle/Cost02/GameObject").gameObject;
            costItem3 = transform.Find("Animator/View_Content/Middle/Cost03/GameObject").gameObject;
            costItem4 = transform.Find("Animator/View_Content/Middle/Cost04/GameObject").gameObject;
            costBG1 = transform.Find("Animator/View_Content/Middle/Cost01/BG").gameObject;
            costBG2 = transform.Find("Animator/View_Content/Middle/Cost02/BG").gameObject;
            costBG3 = transform.Find("Animator/View_Content/Middle/Cost03/BG").gameObject;
            costBG4 = transform.Find("Animator/View_Content/Middle/Cost04/BG").gameObject;

            costIntensify = transform.Find("Animator/View_Content/Middle/Cost05/GameObject/PropItem").gameObject;
            costIntensifyBG = transform.Find("Animator/View_Content/Middle/Cost05/GameObject/Image_Left").gameObject;
            costIntensifyFlag = costIntensify.transform.Find("toggle/button/Checkmark").gameObject;
            intensifyItem = costIntensify.transform.Find("Btn_Item").GetComponent<Button>();
            costIntensifyBtn = costIntensify.transform.Find("toggle/button").GetComponent<Button>();
            costIntensifyImage = costIntensify.transform.Find("Btn_Item/Image_Icon").GetComponent<Image>();

            furmulaIcon = transform.Find("Animator/View_Content/Image_ICON").GetComponent<Image>();
            makeName = transform.Find("Animator/View_Content/Middle/Percent/Text").GetComponent<Text>();

            m_Cost = transform.Find("Animator/View_Content/Text_Cost").GetComponent<Text>();
            m_PublishButton = transform.Find("Animator/View_Content/Btn_01").GetComponent<Button>();
            m_CloseButton = transform.Find("Animator/View_TipsBgNew07/Btn_Close").GetComponent<Button>();

            m_LS_CP_ToggleRegistry = m_LSDdParent.GetComponent<CP_ToggleRegistry>();
            m_LSLv_CP_ToggleRegistry = m_LSLvDdParent.GetComponent<CP_ToggleRegistry>();
            m_Formula_CP_ToggleRegistry = m_FormulaDdParent.GetComponent<CP_ToggleRegistry>();

            m_LS_CP_ToggleRegistry.onToggleChange = OnLS_Changed;
            m_LSLv_CP_ToggleRegistry.onToggleChange = OnLSLv_Changed;
            m_Formula_CP_ToggleRegistry.onToggleChange = OnFormula_Changed;

            m_LSButton.onClick.AddListener(OnLSButtonClicked);
            m_LSLvButton.onClick.AddListener(OnLSLvButtonClicked);
            m_FormulaButton.onClick.AddListener(OnFormulaButtonClicked);
            m_PreViewButton.onClick.AddListener(OnPreViewButtonClicked);

            costIntensifyBtn.onClick.AddListener(OnCostIntensifyClicked);
            m_PublishButton.onClick.AddListener(OnPublishButtonClicked);
            m_CloseButton.onClick.AddListener(OnCloseButtonClicked);

            ConstructLS();
            ConstructLSLv();
            ConstructFormula();
        }

        protected override void OnClose()
        {
            Sys_LivingSkill.Instance.UnfixedFomulaItems.Clear();
        }

        private void OnPreViewButtonClicked()
        {
            if (!CSVFormula.Instance.GetConfData(curSelectedFormula).isequipment)
            {
                UIManager.OpenUI(EUIID.UI_LifeSkill_MedicineTips, false, curSelectedFormula);
            }
            else
            {
                OpenLifeSkill_MakePreviewParm openLifeSkill_MakePreviewParm = new OpenLifeSkill_MakePreviewParm();
                openLifeSkill_MakePreviewParm.formulaId = curSelectedFormula;
                UIManager.OpenUI(EUIID.UI_LifeSkill_MakeTips, false, openLifeSkill_MakePreviewParm);
            }
        }

        private void OnCostIntensifyClicked()
        {
            if (Sys_LivingSkill.Instance.bIntensify)
            {
                Sys_LivingSkill.Instance.bIntensify = !Sys_LivingSkill.Instance.bIntensify;
                costIntensifyFlag.SetActive(Sys_LivingSkill.Instance.bIntensify);
            }
            else
            {
                if (!IsIntensifyEnough)
                {
                    string content = CSVLanguage.Instance.GetConfData(2010127).words;
                    Sys_Hint.Instance.PushContent_Normal(content);
                }
                else
                {
                    Sys_LivingSkill.Instance.bIntensify = !Sys_LivingSkill.Instance.bIntensify;
                    costIntensifyFlag.SetActive(Sys_LivingSkill.Instance.bIntensify);
                }
            }
        }

        private void OnPublishButtonClicked()
        {
            Sys_Family.Instance.PublishConsignReq(curSelectedFormula, Sys_LivingSkill.Instance.bIntensify, Sys_LivingSkill.Instance.UnfixedFomulaItems, m_ShareToggle.isOn);

            UIManager.CloseUI(EUIID.UI_FamilyWorkshop_entrust);
        }

        private void OnCloseButtonClicked()
        {
            UIManager.CloseUI(EUIID.UI_FamilyWorkshop_entrust);
        }

        protected override void OnShow()
        {
            RefreshView();
        }

        private void ConstructLS()
        {
            int count = 5;
            FrameworkTool.CreateChildList(m_LSDdParent.transform, count, 1);
            for (int i = 1; i <= count; i++)
            {
                GameObject go = m_LSDdParent.GetChild(i).gameObject;
                CP_Toggle cP_Toggle = go.GetComponent<CP_Toggle>();
                cP_Toggle.id = i;
                Text text = go.transform.Find("Text_Menu_Dark").GetComponent<Text>();
                uint nameId = Sys_LivingSkill.Instance.livingSkills[(uint)i].cSVLifeSkillData.name_id;
                string content = LanguageHelper.GetTextContent(nameId);
                TextHelper.SetText(text, content);
            }
            UpdateLSParentName();
        }

        private void ConstructLSLv()
        {
            int count = (int)livingSkill.cSVLifeSkillData.max_level;
            FrameworkTool.CreateChildList(m_LSLvDdParent.transform, count, 1);
            for (int i = 1; i <= count; i++)
            {
                GameObject go = m_LSLvDdParent.GetChild(i).gameObject;
                CP_Toggle cP_Toggle = go.GetComponent<CP_Toggle>();
                cP_Toggle.id = i;
                Text text = go.transform.Find("Text_Menu_Dark").GetComponent<Text>();
                string content = string.Format(CSVLanguage.Instance.GetConfData(2010045).words, i);
                TextHelper.SetText(text, content);
            }
            UpdateLSLvParentName();
        }

        private void ConstructFormula()
        {
            int count = m_DropDownSkillformulas.Count;
            FrameworkTool.CreateChildList(m_FormulaDdParent.transform, count, 1);
            for (int i = 1; i <= count; i++)
            {
                GameObject go = m_FormulaDdParent.GetChild(i).gameObject;
                CP_Toggle cP_Toggle = go.GetComponent<CP_Toggle>();
                cP_Toggle.id = i - 1;
                Text text = go.transform.Find("Text_Menu_Dark").GetComponent<Text>();
                uint formulaId = m_DropDownSkillformulas[i - 1];
                CSVFormula.Data cSVFormulaData = CSVFormula.Instance.GetConfData(formulaId);
                TextHelper.SetText(text, CSVItem.Instance.GetConfData(cSVFormulaData.view_item).name_id);
            }
            UpdateFormulaParentName();
        }

        private void UpdateLSParentName()
        {
            uint nameId = Sys_LivingSkill.Instance.livingSkills[(uint)m_LS_CurrentSelectIndex].cSVLifeSkillData.name_id;
            string content = LanguageHelper.GetTextContent(nameId);
            TextHelper.SetText(m_LSParentName, content);
        }

        private void UpdateLSLvParentName()
        {
            TextHelper.SetText(m_LSLvParentName, string.Format(CSVLanguage.Instance.GetConfData(2010045).words, m_LSLvCurrentSelectIndex));
        }

        private void UpdateFormulaParentName()
        {
            uint formulaId = m_DropDownSkillformulas[m_FormulaCurrentSelectIndex];
            CSVFormula.Data cSVFormulaData = CSVFormula.Instance.GetConfData(formulaId);
            TextHelper.SetText(m_FormulaParentName, CSVItem.Instance.GetConfData(cSVFormulaData.view_item).name_id);
        }

        private void OnLS_Changed(int current, int old)
        {
            bLsDropDown = false;
            if (current == old)
            {
                return;
            }
            m_LS_CurrentSelectIndex = current;
            livingSkill = Sys_LivingSkill.Instance.livingSkills[(uint)m_LS_CurrentSelectIndex];
            UpdateLSParentName();

            m_LSLvCurrentSelectIndex = 1;
            ConstructLSLv();
            m_LSLv_CP_ToggleRegistry.SwitchTo(m_LSLvCurrentSelectIndex);

            m_DropDownSkillformulas = Sys_LivingSkill.Instance.GetUnlockSkillFormulas_NotEquip(livingSkill.SkillId, (uint)m_LSLvCurrentSelectIndex);
            m_FormulaCurrentSelectIndex = 0;
            ConstructFormula();
            m_Formula_CP_ToggleRegistry.SwitchTo(m_FormulaCurrentSelectIndex);

            RefreshData();
            RefreshView();
        }

        private void OnLSLv_Changed(int current, int old)
        {
            bLSLvDropDown = false;
           
            m_LSLvCurrentSelectIndex = current;
            UpdateLSLvParentName();

            m_FormulaCurrentSelectIndex = 0;
            m_DropDownSkillformulas = Sys_LivingSkill.Instance.GetUnlockSkillFormulas_NotEquip(livingSkill.SkillId, (uint)m_LSLvCurrentSelectIndex);
            ConstructFormula();
            m_Formula_CP_ToggleRegistry.SwitchTo(m_FormulaCurrentSelectIndex);

            RefreshData();
            RefreshView();
        }

        private void OnFormula_Changed(int current, int old)
        {
            bFormulaDropDown = false;
          
            m_FormulaCurrentSelectIndex = current;
            UpdateFormulaParentName();
            RefreshData();
            RefreshView();
        }

        private void RefreshData()
        {
            curSelectedFormula = m_DropDownSkillformulas[m_FormulaCurrentSelectIndex];
            cSVFormulaData = CSVFormula.Instance.GetConfData(curSelectedFormula);
            Sys_LivingSkill.Instance.bIntensify = false;
        }

        private void OnLSButtonClicked()
        {
            bLsDropDown = !bLsDropDown;
        }

        private void OnLSLvButtonClicked()
        {
            bLSLvDropDown = !bLSLvDropDown;
        }

        private void OnFormulaButtonClicked()
        {
            bFormulaDropDown = !bFormulaDropDown;
        }

        private void SetLSDrop(bool drop)
        {
            m_LSDdParent.gameObject.SetActive(drop);
            m_LSDd_ArrowDown.SetActive(drop);
            m_LSDd_ArrowUp.SetActive(!drop);
            if (drop)
            {
                m_LS_CP_ToggleRegistry.SetHighLight(m_LS_CurrentSelectIndex);
            }
        }

        private void SetLSLvDrop(bool drop)
        {
            m_LSLvDdParent.gameObject.SetActive(drop);
            m_LSLv_ArrowDown.SetActive(drop);
            m_LSLv_ArrowUp.SetActive(!drop);
            if (drop)
            {
                m_LSLv_CP_ToggleRegistry.SetHighLight(m_LSLvCurrentSelectIndex);
            }
        }

        private void SetFormulaDrop(bool drop)
        {
            m_FormulaDdParent.gameObject.SetActive(drop);
            m_Formula_ArrowDown.SetActive(drop);
            m_Formula_ArrowUp.SetActive(!drop);
            if (drop)
            {
                m_Formula_CP_ToggleRegistry.SetHighLight(m_FormulaCurrentSelectIndex);
            }
        }

        private void RefreshPublishButtonState()
        {
            bool can = true;
            if (cSVFormulaData.forge_type == 1)
            {
                int count = cSVFormulaData.normal_forge.Count;
                for (int i = 0; i < count; i++)
                {
                    uint itemId = cSVFormulaData.normal_forge[i][0];
                    uint needCount = cSVFormulaData.normal_forge[i][1];
                    uint itemCount = (uint)Sys_Bag.Instance.GetItemCount(itemId);
                    if (needCount > itemCount)
                    {
                        can = false;
                        break;
                    }
                }
            }
            else if (cSVFormulaData.forge_type == 2)
            {
                bool hasUnfixedFormula = false;
                for (int i = 0; i < Sys_LivingSkill.Instance.UnfixedFomulaItems.Count; i++)
                {
                    if (Sys_LivingSkill.Instance.UnfixedFomulaItems[i] != 0)
                    {
                        hasUnfixedFormula = true;
                    }
                }
                if (!hasUnfixedFormula)
                {
                    can = false;
                }

                int count = 0;
                for (int i = 0; i < Sys_LivingSkill.Instance.UnfixedFomulaItems.Count; i++)
                {
                    if (Sys_LivingSkill.Instance.UnfixedFomulaItems[i] > 0)
                    {
                        count++;
                    }
                }
                if (count < cSVFormulaData.forge_num_min)
                {
                    can = false;
                }
            }

            uint currencyId = cSVFormulaData.entrust_currency;
            uint needCurrencyCount = cSVFormulaData.entrust_pay;
            if (needCurrencyCount > Sys_Bag.Instance.GetItemCount(currencyId))
            {
                can = false;
            }

            ButtonHelper.Enable(m_PublishButton, can);

            uint lanId = 0;
            if (needCurrencyCount > Sys_Bag.Instance.GetItemCount(currencyId))
            {
                lanId = 590002030;
            }
            else
            {
                lanId = 590002027;
            }
            string content = LanguageHelper.GetTextContent(lanId, CSVLanguage.Instance.GetConfData(CSVItem.Instance.GetConfData(currencyId).name_id).words, needCurrencyCount.ToString(),
                Sys_Bag.Instance.GetItemCount(currencyId).ToString());
            TextHelper.SetText(m_Cost, content);
        }

        private void RefreshView()
        {
            RemoveAllButtonListeners();
            //固定配方
            if (cSVFormulaData.forge_type == 1)
            {
                int needForgeNum = (int)cSVFormulaData.normal_forge.Count;
                SetCostRoot(needForgeNum, true, cSVFormulaData.can_intensify, cSVFormulaData.can_harden, true, Sys_LivingSkill.Instance.bIntensify);
            }
            //不固定配方
            else if (cSVFormulaData.forge_type == 2)
            {
                int needForgeNum = (int)cSVFormulaData.forge_num;
                SetCostRoot(needForgeNum, false, false, false);
            }
            ImageHelper.SetIcon(furmulaIcon, CSVItem.Instance.GetConfData(cSVFormulaData.view_item).icon_id);
            TextHelper.SetText(makeName, livingSkill.cSVLifeSkillData.name_id);

            RefreshPublishButtonState();
        }

        private void SetCostRoot(int count, bool FixedFormula, bool Intensify, bool Harden, bool clearUnFixFormulaData = true, bool saveLastIntensifyFlag = false)
        {
            GameObject go = null;
            unFiexFormulaRoot = null;
            if (count == 1)
            {
                costItem1.SetActive(true);
                costItem2.SetActive(false);
                costItem3.SetActive(false);
                costItem4.SetActive(false);

                costBG1.SetActive(true);
                costBG2.SetActive(false);
                costBG3.SetActive(false);
                costBG4.SetActive(false);
                go = costItem1;
            }
            else if (count == 2)
            {
                costItem1.SetActive(false);
                costItem2.SetActive(true);
                costItem3.SetActive(false);
                costItem4.SetActive(false);

                costBG1.SetActive(false);
                costBG2.SetActive(true);
                costBG3.SetActive(false);
                costBG4.SetActive(false);
                go = costItem2;
            }
            else if (count == 3)
            {
                costItem1.SetActive(false);
                costItem2.SetActive(false);
                costItem3.SetActive(true);
                costItem4.SetActive(false);

                costBG1.SetActive(false);
                costBG2.SetActive(false);
                costBG3.SetActive(true);
                costBG4.SetActive(false);
                go = costItem3;
            }
            else if (count == 4)
            {
                costItem1.SetActive(false);
                costItem2.SetActive(false);
                costItem3.SetActive(false);
                costItem4.SetActive(true);

                costBG1.SetActive(false);
                costBG2.SetActive(false);
                costBG3.SetActive(false);
                costBG4.SetActive(true);
                go = costItem4;
            }
            if (FixedFormula)
            {
                ProcessFixedFormula(go);
            }
            else
            {
                unFiexFormulaRoot = go;
                ProcessUnFixedFormula(clearUnFixFormulaData);
            }
            ProcessIntensifyGo(Intensify, saveLastIntensifyFlag);
        }


        //固定配方
        private void ProcessFixedFormula(GameObject go)
        {
            if (go == null)
                return;
            int count = (int)cSVFormulaData.normal_forge.Count;
            if (count != go.transform.childCount)
            {
                DebugUtil.LogErrorFormat($"配置的材料个数 {count} 与{go.name}子物体个数{go.transform.childCount}不一致");
                return;
            }
            for (int i = 0; i < count; i++)
            {
                Transform child = go.transform.GetChild(i);
                GameObject btn = child.Find("Btn_None").gameObject;
                Image icon = child.Find("Btn_Item/Image_Icon").GetComponent<Image>();
                Button button = child.Find("Btn_Item").GetComponent<Button>();
                Text itemName = child.Find("Text_Name").GetComponent<Text>();
                Text itemNumber = child.Find("Text_Number").GetComponent<Text>();
                Image quality = child.Find("Btn_Item/Image_BG").GetComponent<Image>();

                uint itemId = cSVFormulaData.normal_forge[i][0];
                uint needCount = cSVFormulaData.normal_forge[i][1];
                long itemCount = Sys_Bag.Instance.GetItemCount(itemId);

                PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(itemId, needCount, false, false, false, false, false, false, true);
                itemData.bShowBtnNo = false;
                button.onClick.AddListener(() =>
                {
                    UIManager.OpenUI(EUIID.UI_Message_Box, false, new MessageBoxEvt(EUIID.UI_FamilyWorkshop_entrust, itemData));
                });

                itemNumber.gameObject.SetActive(true);
                uint contentId = 0;
                if (itemCount < needCount)
                {
                    contentId = 1601000004;
                    btn.SetActive(true);
                }
                else
                {
                    contentId = 1601000005;
                    btn.SetActive(false);
                }
                itemNumber.text = string.Format(LanguageHelper.GetTextContent(contentId), itemCount.ToString(), needCount.ToString());

                ImageHelper.SetIcon(icon, CSVItem.Instance.GetConfData(itemId).icon_id);
                ImageHelper.GetQualityColor_Frame(quality, (int)CSVItem.Instance.GetConfData(itemId).quality);
                TextHelper.SetText(itemName, CSVItem.Instance.GetConfData(itemId).name_id);
                itemBtns.Add(button);
            }
        }

        //不固定配方
        private void ProcessUnFixedFormula(bool clearData = true)
        {
            if (unFiexFormulaRoot == null)
                return;
            int count = (int)cSVFormulaData.forge_num;
            if (count != unFiexFormulaRoot.transform.childCount)
            {
                DebugUtil.LogErrorFormat($"配置的材料个数 {count} 与{unFiexFormulaRoot.name}子物体个数{unFiexFormulaRoot.transform.childCount}不一致");
                return;
            }
            if (clearData)
            {
                Sys_LivingSkill.Instance.UnfixedFomulaItems.Clear();

                for (int i = 0; i < count; i++)
                {

                    Sys_LivingSkill.Instance.UnfixedFomulaItems.Add(0);

                }
            }
            else
            {
                Dictionary<uint, int> record = new Dictionary<uint, int>();

                for (int i = 0, _count = Sys_LivingSkill.Instance.UnfixedFomulaItems.Count; i < _count; i++)
                {
                    uint itemId = Sys_LivingSkill.Instance.UnfixedFomulaItems[i];
                    if (!record.ContainsKey(itemId))
                    {
                        record.Add(itemId, (int)Sys_Bag.Instance.GetItemCount(itemId));
                    }
                }
                for (int i = 0, _count = Sys_LivingSkill.Instance.UnfixedFomulaItems.Count; i < _count; i++)
                {
                    uint itemId = Sys_LivingSkill.Instance.UnfixedFomulaItems[i];

                    if (record[itemId] == 0)
                    {
                        Sys_LivingSkill.Instance.UnfixedFomulaItems[i] = 0;
                    }
                    else
                    {
                        record[itemId]--;
                    }
                }
            }
            RefreshUnFixedFormulaGrids();
        }

        private void RefreshUnFixedFormulaGrids()
        {
            if (unFiexFormulaRoot == null)
            {
                DebugUtil.LogErrorFormat($"未选中不固定配方");
            }
            for (int i = 0, count = Sys_LivingSkill.Instance.UnfixedFomulaItems.Count; i < count; i++)
            {
                Transform child = unFiexFormulaRoot.transform.GetChild(i);
                GameObject btn = child.Find("Btn_None").gameObject;
                Image icon = child.Find("Btn_Item/Image_Icon").GetComponent<Image>();
                Button button = child.Find("Btn_Item").GetComponent<Button>();
                Text itemName = child.Find("Text_Name").GetComponent<Text>();

                uint itemId = Sys_LivingSkill.Instance.UnfixedFomulaItems[i];
                if (itemId == 0)
                {
                    icon.enabled = false;
                    btn.SetActive(true);
                    itemName.text = string.Empty;
                }
                else
                {
                    icon.enabled = true;
                    btn.SetActive(false);
                    ImageHelper.SetIcon(icon, CSVItem.Instance.GetConfData(itemId).icon_id);
                    TextHelper.SetText(itemName, CSVItem.Instance.GetConfData(itemId).name_id);
                }
                button.onClick.AddListener(() =>
                {
                    if (m_LSLvCurrentSelectIndex > livingSkill.Level)
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2010113,
                            CSVLanguage.Instance.GetConfData(livingSkill.cSVLifeSkillData.name_id).words, m_LSLvCurrentSelectIndex.ToString()));
                        return;
                    }
                    UIManager.OpenUI(EUIID.UI_LifeSkill_MedicineSelect, false, curSelectedFormula);
                });
                itemBtns.Add(button);
            }
        }

        //强化打造格子
        private void ProcessIntensifyGo(bool valid, bool saveLastIntensifyFlag = false)
        {
            if (!valid)
            {
                costIntensify.SetActive(false);
                costIntensifyBG.SetActive(false);
                return;
            }
            costIntensify.SetActive(true);

            if (!saveLastIntensifyFlag)
            {
                Sys_LivingSkill.Instance.bIntensify = false;
                costIntensifyFlag.SetActive(Sys_LivingSkill.Instance.bIntensify);
            }

            Image _icon = costIntensify.transform.Find("Btn_Item/Image_Icon").GetComponent<Image>();
            Image quality = costIntensify.transform.Find("Btn_Item/Image_BG").GetComponent<Image>();
            Text itemNumber = costIntensify.transform.Find("Text_Number").GetComponent<Text>();
            Text itemName = costIntensify.transform.Find("Text_Name").GetComponent<Text>();
            itemNumber.gameObject.SetActive(true);
            itemName.gameObject.SetActive(true);
            _icon.enabled = true;

            uint itemId = CSVFormula.Instance.GetConfData(curSelectedFormula).intensify_forge[0][0];
            long needCount = cSVFormulaData.intensify_forge[0][1];
            long itemCount = Sys_Bag.Instance.GetItemCount(itemId);

            PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(itemId, needCount, false, false, false, false, false, false, true);
            itemData.bShowBtnNo = false;
            intensifyItem.onClick.AddListener(() =>
            {
                UIManager.OpenUI(EUIID.UI_Message_Box, false, new MessageBoxEvt(EUIID.UI_FamilyWorkshop_entrust, itemData));
            });

            itemNumber.gameObject.SetActive(true);
            uint contentId = 0;
            if (itemCount < needCount)
            {
                contentId = 1601000004;
                IsIntensifyEnough = false;
            }
            else
            {
                contentId = 1601000005;
                IsIntensifyEnough = true;
            }
            itemNumber.text = string.Format(LanguageHelper.GetTextContent(contentId), itemCount.ToString(), needCount.ToString());
            TextHelper.SetText(itemName, CSVItem.Instance.GetConfData(itemId).name_id);
            ImageHelper.SetIcon(_icon, CSVItem.Instance.GetConfData(itemId).icon_id);
            ImageHelper.GetQualityColor_Frame(quality, (int)CSVItem.Instance.GetConfData(itemId).quality);
            itemBtns.Add(intensifyItem);
        }

        private void RemoveAllButtonListeners()
        {
            foreach (var item in itemBtns)
            {
                item.onClick.RemoveAllListeners();
            }
        }
    }
}


