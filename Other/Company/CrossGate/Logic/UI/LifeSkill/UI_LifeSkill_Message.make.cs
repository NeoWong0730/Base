using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Table;
using UnityEngine.UI;
using Lib.Core;
using DG.Tweening;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using DG.Tweening.Core;
using DG.Tweening.Plugins.Options;
using System;
using Packet;
using UnityEngine.EventSystems;

namespace Logic
{
    // make
    public partial class UI_LifeSkill_Message : UIBase
    {
        private GameObject view2;
        private GameObject view2dropDownParent;
        private GameObject view2Arrow_down;
        private GameObject view2Arrow_up;
        private Text view2dropDowmLevel;
        private CP_ToggleRegistry CP_ToggleRegistry_Make;
        private Button make_view2MenuBtn;
        private InfinityGrid infinity_make;
        private Transform infinityParent_make;
        private bool _bDropDown_make;

        private bool bDropDown_make
        {
            get { return _bDropDown_make; }
            set
            {
                if (_bDropDown_make != value)
                {
                    _bDropDown_make = value;
                    SetView2Drop(_bDropDown_make);
                }
            }
        }

        private int curView2MenuParentLable = -1;

        private int CurView2MenuParentLable
        {
            get { return curView2MenuParentLable; }
            set
            {
                if (curView2MenuParentLable != value)
                {
                    curView2MenuParentLable = value;
                    curLifeSkillSelectIndex_make = 0;
                    OnView2MenuParentLableChanged();
                    UpdateView2dropDowmLevel();
                    CancelMakeAnim();
                    bDropDown_make = false;
                }
            }
        }

        private List<uint> dropDownSkillformulas = new List<uint>();
        private Dictionary<GameObject, LifeSkillCeil1> ceil1s_make = new Dictionary<GameObject, LifeSkillCeil1>();
        private int curLifeSkillSelectIndex_make;
        private uint curSelectedFormula;
        private CSVFormula.Data cSVFormulaData;
        private GameObject m_CloseBg2;

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

        private GameObject costHarden;
        private Button costHardenBtn;
        private Button costHardendeleteBtn;
        private Image costHardenImage;
        private GameObject costHardenbuttonNo;
        private Text costHardenName;

        private GameObject successRate;
        private List<Button> itemBtns = new List<Button>();

        private Button Make;
        private bool makeFun;
        private Text makeName;
        private Button preViewButton;
        private GameObject viewProcess;
        private Image sliderImage;
        private Text extra_desc;
        private Text formula_desc;
        private Image furmulaIcon;

        private bool isMaking;

        private Transform makeAnimIconParent;
        private GameObject makeAnim;

        private Image makeAnimBg;

        //private Image makeAnimProcess;
        private Text animName;

        private bool canMake = false;

        private GameObject m_Lucky;
        private Button m_LuckyButton;
        private GameObject m_LuckyDesGo;
        private Text m_LuckyTitle;
        private Text m_LuckyDes;
        private Image m_LuckClickClose;
        private Slider m_LuckSlider;
        private Text m_LuckValue;
        private GameObject m_FxLucky;

        private void ParseView2Component()
        {
            view2 = transform.Find("Animator/View02").gameObject;
            forgetSkillButton = transform.Find("Animator/View02/Center/Button_Delete").GetComponent<Button>();
            m_CloseBg2 = view2.transform.Find("Right/GridList/Image_Close").gameObject;
            successRate = transform.Find("Animator/View02/Center/Percent/Text_Percent").gameObject;
            extra_desc = view2.transform.Find("Center/Text_Notice").GetComponent<Text>();
            furmulaIcon = view2.transform.Find("Center/Image_ICON").GetComponent<Image>();
            formula_desc = view2.transform.Find("Center/Percent/Text_Tips").GetComponent<Text>();
            costItem1 = view2.transform.Find("Center/Cost01/GameObject").gameObject;
            costItem2 = view2.transform.Find("Center/Cost02/GameObject").gameObject;
            costItem3 = view2.transform.Find("Center/Cost03/GameObject").gameObject;
            costItem4 = view2.transform.Find("Center/Cost04/GameObject").gameObject;
            costBG1 = view2.transform.Find("Center/Cost01/BG").gameObject;
            costBG2 = view2.transform.Find("Center/Cost02/BG").gameObject;
            costBG3 = view2.transform.Find("Center/Cost03/BG").gameObject;
            costBG4 = view2.transform.Find("Center/Cost04/BG").gameObject;
            m_MakeListButton = view2.transform.Find("View_List/Button").GetComponent<Button>();
            m_MakeListToggle = view2.transform.Find("Right/Toggle").GetComponent<Toggle>();
            m_fenjieToggle = view2.transform.Find("Right/Toggle1").GetComponent<Toggle>();
            m_ViewList = view2.transform.Find("View_List/Image_BG").gameObject;
            m_InfinityGrid = m_ViewList.transform.Find("Scroll View").GetComponent<InfinityGrid>();
            m_ViewListClose = view2.transform.Find("View_List/Image_BG/Image_Close").GetComponent<Image>();
            costIntensify = view2.transform.Find("Center/Cost05/GameObject/PropItem").gameObject;
            costIntensifyBG = view2.transform.Find("Center/Cost05/GameObject/Image_Left").gameObject;
            costIntensifyFlag = costIntensify.transform.Find("toggle/button/Checkmark").gameObject;
            costHarden = view2.transform.Find("Center/Cost05/GameObject/PropItem (1)").gameObject;
            costHardenName = costHarden.transform.Find("Text_Name").GetComponent<Text>();
            costHardenbuttonNo = costHarden.transform.Find("Btn_None").gameObject;
            costHardenImage = costHarden.transform.Find("Btn_Item/Image_Icon").GetComponent<Image>();
            costHardenBtn = costHarden.transform.Find("Btn_Item").GetComponent<Button>();
            costHardendeleteBtn = costHarden.transform.Find("Btn_Delete").GetComponent<Button>();
            m_CostIcon = transform.Find("Animator/View_Message/Image_Property01/Image_Icon").GetComponent<Image>();
            m_Cost = transform.Find("Animator/View_Message/Image_Property01/Text_Number").GetComponent<Text>();
            m_FlyObj = transform.Find("Animator/Image_ICON_Fly").gameObject;
            m_Lucky = transform.Find("Animator/View02/Luck").gameObject;
            m_LuckyButton = transform.Find("Animator/View02/Luck/Button").GetComponent<Button>();
            m_LuckyDesGo = transform.Find("Animator/View02/Luck/View_Detail_Tips").gameObject;
            m_LuckyDes = transform.Find("Animator/View02/Luck/View_Detail_Tips/Image_BG/Text").GetComponent<Text>();
            m_LuckyTitle = transform.Find("Animator/View02/Luck/View_Detail_Tips/Image_Title/Text_Title").GetComponent<Text>();
            m_LuckClickClose = transform.Find("Animator/View02/Luck/View_Detail_Tips/Blank").GetComponent<Image>();
            m_LuckSlider = transform.Find("Animator/View02/Luck/Slider_Exp").GetComponent<Slider>();
            m_LuckValue = transform.Find("Animator/View02/Luck/Slider_Exp/Text_Percent").GetComponent<Text>();
            m_FxLucky = transform.Find("Animator/View02/Luck/Button/Fx_dianji").gameObject;

            m_FlyObjIcon = m_FlyObj.GetComponent<Image>();

            preViewButton = view2.transform.Find("Center/Btn_Preview").GetComponent<Button>();
            viewProcess = view2.transform.Find("Center/View_process").gameObject;
            makeAnimBg = view2.transform.Find("Center/View_process/NPC_Collect/Root/Image1").GetComponent<Image>();
            sliderImage = viewProcess.transform.Find("NPC_Collect/Root/Image_Blank/Image3").GetComponent<Image>();
            makeAnimIconParent = viewProcess.transform.Find("NPC_Collect/Root/Button_Icon");
            animName = viewProcess.transform.Find("NPC_Collect/Root/Text").GetComponent<Text>();
            view2dropDownParent = view2.transform.Find("Right/GridList").gameObject;
            intensifyItem = costIntensify.transform.Find("Btn_Item").GetComponent<Button>();
            costIntensifyBtn = costIntensify.transform.Find("toggle/button").GetComponent<Button>();
            costIntensifyImage = costIntensify.transform.Find("Btn_Item/Image_Icon").GetComponent<Image>();
            CP_ToggleRegistry_Make = view2dropDownParent.GetComponent<CP_ToggleRegistry>();
            make_view2MenuBtn = view2.transform.Find("Right/Btn_Menu_Dark").GetComponent<Button>();
            view2Arrow_down = make_view2MenuBtn.transform.Find("Image_fold01").gameObject;
            view2Arrow_up = make_view2MenuBtn.transform.Find("Image_fold").gameObject;
            view2dropDowmLevel = make_view2MenuBtn.transform.Find("Text_Menu_Dark").GetComponent<Text>();
            Make = view2.transform.Find("Center/Btn_01").GetComponent<Button>();
            makeName = Make.transform.Find("Text_01").GetComponent<Text>();
            infinityParent_make = view2.transform.Find("Right/TargetScroll");
            infinity_make = infinityParent_make.gameObject.GetComponent<InfinityGrid>();
            infinity_make.onCreateCell += OnCreateCell_Make;
            infinity_make.onCellChange += OnCellChange_Make;
            //for (int i = 0; i < infinityParent_make.childCount; i++)
            //{
            //    GameObject go = infinityParent_make.GetChild(i).gameObject;
            //    LifeSkillCeil1 lifeSkillCeil1 = new LifeSkillCeil1();
            //    lifeSkillCeil1.BindGameObject(go);
            //    lifeSkillCeil1.AddClickListener(OnCeilMakeSelected);
            //    ceil1s_make.Add(go, lifeSkillCeil1);
            //}
        }

        private void RegistMakeEvent()
        {
            CP_ToggleRegistry_Make.onToggleChange = OnView2MenuChildChanged;
            make_view2MenuBtn.onClick.AddListener(OnView2MenuBtnClicked);
            costIntensifyBtn.onClick.AddListener(() =>
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
            });
            forgetSkillButton.onClick.AddListener(OnForgetSkillClicked);
            costHardenBtn.onClick.AddListener(OnCostHardenButtonClicked);
            Make.onClick.AddListener(OnMakeButtonClicked);
            preViewButton.onClick.AddListener(OnPreViewButtonClicked);
            costHardendeleteBtn.onClick.AddListener(OnCostHardendeleteButtonClicked);
            m_MakeListButton.onClick.AddListener(OnMakeListButtonClicked);
            m_LuckyButton.onClick.AddListener(OnLuckyButtonClicked);

            Lib.Core.EventTrigger eventTrigger = Lib.Core.EventTrigger.Get(m_ViewListClose);
            eventTrigger.AddEventListener(EventTriggerType.PointerClick, OnViewListClose);

            Lib.Core.EventTrigger eventTrigger2 = Lib.Core.EventTrigger.Get(m_CloseBg2);
            eventTrigger2.AddEventListener(EventTriggerType.PointerClick, OnCLoseBg2);

            Lib.Core.EventTrigger eventTrigger3 = Lib.Core.EventTrigger.Get(m_LuckClickClose);
            eventTrigger3.AddEventListener(EventTriggerType.PointerClick, OnCloseLuckyDes);

            m_InfinityGrid.onCreateCell += OnCreateCell;
            m_InfinityGrid.onCellChange += OnCellChange;
        }

        private void ConstructView2DropDownRoot()
        {
            FrameworkTool.CreateChildList(view2dropDownParent.transform, (int) livingSkill.cSVLifeSkillData.max_level, 1);
            for (int i = 1, count = view2dropDownParent.transform.childCount; i < count; i++)
            {
                GameObject game = view2dropDownParent.transform.GetChild(i).gameObject;
                CP_Toggle cP_Toggle = game.GetComponent<CP_Toggle>();
                cP_Toggle.id = i;
                Text text = game.transform.Find("Text_Menu_Dark").GetComponent<Text>();
                string content = string.Format(CSVLanguage.Instance.GetConfData(2010045).words, i);
                TextHelper.SetText(text, content);
            }

            OnView2MenuParentLableChanged();
            UpdateView2dropDowmLevel();
            bDropDown_make = false;
            view2Arrow_up.SetActive(true);
            //CurView2MenuParentLable = (int)livingSkill.Level;
        }

        private void OnCLoseBg2(BaseEventData baseEventData)
        {
            m_CloseBg2.SetActive(false);
            bDropDown_make = false;
        }

        private void OnCloseLuckyDes(BaseEventData baseEventData)
        {
            m_LuckyDesGo.SetActive(false);
        }

        private void OnView2MenuBtnClicked()
        {
            bDropDown_make = !bDropDown_make;
        }

        private void UpdateView2dropDowmLevel()
        {
            TextHelper.SetText(view2dropDowmLevel, string.Format(CSVLanguage.Instance.GetConfData(2010045).words, CurView2MenuParentLable));
        }

        private void SetView2Drop(bool value)
        {
            view2dropDownParent.SetActive(value);
            view2Arrow_down.SetActive(value);
            view2Arrow_up.SetActive(!value);
            m_CloseBg2.SetActive(value);
            if (value)
            {
                CP_ToggleRegistry_Make.SetHighLight(CurView2MenuParentLable);
            }
        }

        private void OnView2MenuChildChanged(int curToggle, int old)
        {
            CurView2MenuParentLable = curToggle;
        }

        private void OnView2MenuParentLableChanged()
        {
            dropDownSkillformulas = Sys_LivingSkill.Instance.GetUnlockSkillFormulas(livingSkill.SkillId, (uint) curView2MenuParentLable);
            infinity_make.CellCount = dropDownSkillformulas.Count;
            infinity_make.ForceRefreshActiveCell();
            int curSelectedFormulaId = Sys_LivingSkill.Instance.GetSelectFormula(livingSkill.SkillId, (uint) curView2MenuParentLable);
            if (curSelectedFormulaId == -1)
            {
                curSelectedFormula = dropDownSkillformulas[curLifeSkillSelectIndex_make];
            }
            else
            {
                curLifeSkillSelectIndex_make = dropDownSkillformulas.IndexOf((uint) curSelectedFormulaId);
                curSelectedFormula = dropDownSkillformulas[curLifeSkillSelectIndex_make];
            }

            OnSelectMake();
            cSVFormulaData = CSVFormula.Instance.GetConfData(curSelectedFormula);
            RefreshRight_view2();
            //CancelMakeAnim();
        }

        private void OnSelectMake()
        {
            foreach (var item in ceil1s_make)
            {
                if (item.Value.dataIndex != curLifeSkillSelectIndex_make)
                {
                    item.Value.Release();
                }
                else
                {
                    item.Value.Select();
                }
            }
        }

        private void OnCreateCell_Make(InfinityGridCell cell)
        {
            LifeSkillCeil1 lifeSkillCeil1 = new LifeSkillCeil1();
            lifeSkillCeil1.BindGameObject(cell.mRootTransform.gameObject);
            lifeSkillCeil1.AddClickListener(OnCeilMakeSelected);
            cell.BindUserData(lifeSkillCeil1);
            ceil1s_make.Add(cell.mRootTransform.gameObject, lifeSkillCeil1);
        }

        private void OnCellChange_Make(InfinityGridCell cell, int index)
        {
            LifeSkillCeil1 lifeSkillCeil1 = cell.mUserData as LifeSkillCeil1;
            lifeSkillCeil1.SetData(dropDownSkillformulas[index], livingSkill.category, index, Sys_LivingSkill.Instance.IsSkillFormulaUnlock(dropDownSkillformulas[index]));
            if (index != curLifeSkillSelectIndex_make)
            {
                lifeSkillCeil1.Release();
            }
            else
            {
                lifeSkillCeil1.Select();
            }

            cell.mRootTransform.gameObject.name = string.Format($"formula{dropDownSkillformulas[index]}");
        }

        private void UpdateChildrenCallback_Make(int index, Transform trans)
        {
            LifeSkillCeil1 lifeSkillCeil1 = ceil1s_make[trans.gameObject];
            lifeSkillCeil1.SetData(dropDownSkillformulas[index], livingSkill.category, index, Sys_LivingSkill.Instance.IsSkillFormulaUnlock(dropDownSkillformulas[index]));
            if (index != curLifeSkillSelectIndex_make)
            {
                lifeSkillCeil1.Release();
            }
            else
            {
                lifeSkillCeil1.Select();
            }

            trans.gameObject.name = string.Format($"formula{dropDownSkillformulas[index]}");
        }

        private void OnCeilMakeSelected(LifeSkillCeil1 lifeSkillCeil1)
        {
            if (lifeSkillCeil1.dataIndex == curLifeSkillSelectIndex_make)
                return;
            curLifeSkillSelectIndex_make = lifeSkillCeil1.dataIndex;
            foreach (var item in ceil1s_make)
            {
                if (item.Value.dataIndex == curLifeSkillSelectIndex_make)
                {
                    item.Value.Select();
                }
                else
                {
                    item.Value.Release();
                }
            }

            if (lifeSkillCeil1.category == 1)
            {
            }
            else if (lifeSkillCeil1.category == 2)
            {
            }

            curSelectedFormula = dropDownSkillformulas[curLifeSkillSelectIndex_make];
            cSVFormulaData = CSVFormula.Instance.GetConfData(curSelectedFormula);
            Sys_LivingSkill.Instance.bIntensify = false;
            RefreshRight_view2();
            CancelMakeAnim();
        }

        private void RefreshRight_view2()
        {
            RemoveAllButtonListeners();
            //固定配方
            if (cSVFormulaData.forge_type == 1)
            {
                int needForgeNum = (int) cSVFormulaData.normal_forge.Count;
                SetCostRoot(needForgeNum, true, cSVFormulaData.can_intensify, cSVFormulaData.can_harden, true, Sys_LivingSkill.Instance.bIntensify);
            }
            //不固定配方
            else if (cSVFormulaData.forge_type == 2)
            {
                int needForgeNum = (int) cSVFormulaData.forge_num;
                SetCostRoot(needForgeNum, false, false, false);
            }

            m_fenjieToggle.gameObject.SetActive(cSVFormulaData.isequipment);
            if (cSVFormulaData.can_intensify)
            {
                extra_desc.gameObject.SetActive(true);

                uint itemId = cSVFormulaData.intensify_forge[0][0];
                TextHelper.SetText(extra_desc, string.Format(CSVLanguage.Instance.GetConfData(cSVFormulaData.extra_desc).words,
                    LanguageHelper.GetTextContent(CSVItem.Instance.GetConfData(itemId).name_id)));
            }
            else
            {
                extra_desc.gameObject.SetActive(false);
            }

            ImageHelper.SetIcon(furmulaIcon, CSVItem.Instance.GetConfData(cSVFormulaData.view_item).icon_id);
            TextHelper.SetText(formula_desc, CSVItem.Instance.GetConfData(cSVFormulaData.view_item).describe_id);
            //UpdateCost();
            TextHelper.SetText(makeName, livingSkill.cSVLifeSkillData.name_id);
            OnUpdateLuckyValue(livingSkill.SkillId, livingSkill.luckyValue);
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
                successRate.SetActive(false);
            }
            else
            {
                unFiexFormulaRoot = go;
                successRate.SetActive(true);
                ProcessUnFixedFormula(clearUnFixFormulaData);
            }

            ProcessIntensifyGo(Intensify, saveLastIntensifyFlag);
            ProcessHardenGo(Harden);
        }


        //固定配方
        private void ProcessFixedFormula(GameObject go)
        {
            if (go == null)
                return;
            int count = (int) cSVFormulaData.normal_forge.Count;
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
                button.onClick.AddListener(() => { UIManager.OpenUI(EUIID.UI_Message_Box, false, new MessageBoxEvt(EUIID.UI_LifeSkill_Message, itemData)); });

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
                ImageHelper.GetQualityColor_Frame(quality, (int) CSVItem.Instance.GetConfData(itemId).quality);
                TextHelper.SetText(itemName, CSVItem.Instance.GetConfData(itemId).name_id);

                itemBtns.Add(button);
            }
        }

        //不固定配方
        private void ProcessUnFixedFormula(bool clearData = true)
        {
            if (unFiexFormulaRoot == null)
                return;
            int count = (int) cSVFormulaData.forge_num;
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
                        record.Add(itemId, (int) Sys_Bag.Instance.GetItemCount(itemId));
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
                    if (isMaking)
                    {
                        return;
                    }

                    if (curView2MenuParentLable > livingSkill.Level)
                    {
                        //string content = CSVLanguage.Instance.GetConfData(2010113).words;
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2010113, CSVLanguage.Instance.GetConfData(livingSkill.cSVLifeSkillData.name_id).words, curView2MenuParentLable.ToString()));
                        return;
                    }

                    UIManager.OpenUI(EUIID.UI_LifeSkill_MedicineSelect, false, curSelectedFormula);
                });
                itemBtns.Add(button);
            }

            UpdateSuccessRate();
        }


        private void UpdateSuccessRate()
        {
            int success = 0;
            float successrate = 0;
            for (int i = 0; i < Sys_LivingSkill.Instance.UnfixedFomulaItems.Count; i++)
            {
                uint item = Sys_LivingSkill.Instance.UnfixedFomulaItems[i];
                if (item == 0)
                    continue;
                int level = (int) CSVFormula.Instance.GetConfData(curSelectedFormula).level_formula;
                int val = (int) CSVItem.Instance.GetConfData(item).fun_value[level + 1];
                success += val;
            }

            successrate = (float) success / (float) 100;
            successRate.GetComponent<Text>().text = successrate.ToString(1) + "%";
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
            intensifyItem.onClick.AddListener(() => { UIManager.OpenUI(EUIID.UI_Message_Box, false, new MessageBoxEvt(EUIID.UI_LifeSkill_Message, itemData)); });

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
            ImageHelper.GetQualityColor_Frame(quality, (int) CSVItem.Instance.GetConfData(itemId).quality);
            itemBtns.Add(intensifyItem);
        }

        //淬炼格子
        private void ProcessHardenGo(bool valid)
        {
            if (!valid)
            {
                costHarden.SetActive(false);
                return;
            }

            costHarden.SetActive(true);
            costHardenbuttonNo.SetActive(true);
            costHardendeleteBtn.gameObject.SetActive(false);
            costHardenImage.gameObject.SetActive(false);
            Sys_LivingSkill.Instance.uuid = 0;
            TextHelper.SetText(costHardenName, 2010054);
        }

        private void OnCostHardenButtonClicked()
        {
            CSVFormula.Data cSVFormulaData = CSVFormula.Instance.GetConfData(curSelectedFormula);
            if (null != cSVFormulaData)
            {
                uint itemId = cSVFormulaData.harden_item_id;
                List<ulong> uuids = Sys_Bag.Instance.GetUuidsByItemId(itemId);
                if (uuids.Count == 0)
                {
                    Sys_LivingSkill.Instance.bHasItem = false;
                    List<ulong> temp = new List<ulong>();
                    temp.Add(cSVFormulaData.harden_item_source);
                    UIManager.OpenUI(EUIID.UI_ChooseItem, false, temp);
                }
                else
                {
                    Sys_LivingSkill.Instance.bHasItem = true;
                    if (uuids.Contains(Sys_LivingSkill.Instance.uuid))
                    {
                        List<ulong> temp = new List<ulong>();
                        uuids.Remove(Sys_LivingSkill.Instance.uuid);
                        temp.Add(Sys_LivingSkill.Instance.uuid);
                        temp.AddRange(uuids);
                        UIManager.OpenUI(EUIID.UI_ChooseItem, false, temp);
                    }
                    else
                    {
                        UIManager.OpenUI(EUIID.UI_ChooseItem, false, uuids);
                    }
                }
            }
        }

        private void OnCostHardendeleteButtonClicked()
        {
            costHardendeleteBtn.gameObject.SetActive(false);
            costHardenImage.gameObject.SetActive(false);
            costHardenbuttonNo.SetActive(true);
            Sys_LivingSkill.Instance.uuid = 0;
        }

        private void OnSetHardenItem()
        {
            costHardendeleteBtn.gameObject.SetActive(true);
            costHardenImage.gameObject.SetActive(true);
            costHardenImage.enabled = true;
            costHardenbuttonNo.SetActive(false);
            ItemData itemData = Sys_Bag.Instance.GetItemDataByUuid(Sys_LivingSkill.Instance.uuid);
            ImageHelper.SetIcon(costHardenImage, itemData.cSVItemData.icon_id);
            TextHelper.SetText(costHardenName, itemData.cSVItemData.name_id);
        }

        private void OnMakeButtonClicked()
        {
            if (!makeFun)
            {
                A?.Kill();
                makeFun = !makeFun;
            }
            else
            {
                if (CanMake())
                {
                    makeFun = !makeFun;
                    TryMake();
                }
            }

            UpdateMakeButton();
        }

        private bool CanMake()
        {
            canMake = false;
            if (curView2MenuParentLable > livingSkill.Level)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2010113, CSVLanguage.Instance.GetConfData(livingSkill.cSVLifeSkillData.name_id).words, curView2MenuParentLable.ToString()));
                return canMake;
            }

            if (!Sys_LivingSkill.Instance.IsSkillFormulaUnlock(curSelectedFormula))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2010119));
                return canMake;
            }

            uint needVitality = cSVFormulaData.cost;
            if (needVitality > Sys_Bag.Instance.GetItemCount(5))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2010120));
                return canMake;
            }

            //固定
            if (cSVFormulaData.forge_type == 1)
            {
                bool enough = true;
                int count = cSVFormulaData.normal_forge.Count;
                for (int i = 0; i < count; i++)
                {
                    uint itemId = cSVFormulaData.normal_forge[i][0];
                    uint needCount = cSVFormulaData.normal_forge[i][1];
                    uint itemCount = (uint) Sys_Bag.Instance.GetItemCount(itemId);
                    if (needCount > itemCount)
                    {
                        enough = false;
                        break;
                    }
                }

                if (!enough)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2010121));
                    return canMake;
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
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2010123));
                    return canMake;
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
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2010142, cSVFormulaData.forge_num_min.ToString()));
                    return canMake;
                }
            }

            if (Sys_Bag.Instance.BoxFull(BoxIDEnum.BoxIdNormal))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2010131));
                return false;
            }

            canMake = true;

            return canMake;
        }

        private void TryMake()
        {
            isMaking = true;
            if (UIManager.IsOpen(EUIID.UI_LifeSkill_MedicineSelect))
            {
                UIManager.CloseUI(EUIID.UI_LifeSkill_MedicineSelect);
            }

            viewProcess.SetActive(true);
            TextHelper.SetText(animName, cSVFormulaData.animation_name);

            //ButtonHelper.Enable(Make, false);
            //LoadMakeIconAssetAsyn(cSVFormulaData.formula_animation);
            ImageHelper.SetIcon(makeAnimBg, cSVFormulaData.formula_animation[1]);
            ImageHelper.SetIcon(sliderImage, cSVFormulaData.formula_animation[0]);


            float duration ;
            CollectTimeScale(out duration);
            A = DOTween.To(() => sliderImage.fillAmount, x => sliderImage.fillAmount = x, 1, duration);
            A.onComplete += OnMakeAnimationPlayOver;
        }

        private void CollectTimeScale(out float duration)
        {
            duration = 3;
            float timeScale = Time.timeScale;
            duration *= timeScale;
        }

        TweenerCore<float, float, FloatOptions> A;

        private void OnMakeAnimationPlayOver()
        {
            DestroyMakeAnim();
            if (!canMake)
                return;
            if (livingSkill.bExpFull)
            {
                string ExpFullcontent = CSVLanguage.Instance.GetConfData(2010110).words;
                Sys_Hint.Instance.PushContent_Normal(ExpFullcontent);
            }
            
            Sys_LivingSkill.Instance.FormulaBuildReq(curSelectedFormula, Sys_LivingSkill.Instance.bIntensify,
                Sys_LivingSkill.Instance.uuid, Sys_LivingSkill.Instance.UnfixedFomulaItems);
            //ButtonHelper.Enable(Make, true);
        }

        private void UpdateMakeButton()
        {
            if (makeFun)
            {
                TextHelper.SetText(makeName, livingSkill.cSVLifeSkillData.name_id);
                DestroyMakeAnim();
            }
            else
            {
                TextHelper.SetText(makeName, 2010157); //取消
            }
        }

        private void CancelMakeAnim()
        {
            canMake = false;
            makeFun = true;
            UpdateMakeButton();
            A?.Kill();
            //ButtonHelper.Enable(Make, true);
            DestroyMakeAnim();
            m_MakeListWillRemove.Clear();
            m_MakeLists.Clear();
            m_MakeListButton.gameObject.SetActive(false);
            m_MakeListToggle.isOn = false;
            m_fenjieToggle.isOn = false;
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
                if (livingSkill.b_LuckyFull)
                {
                    openLifeSkill_MakePreviewParm.lucky = true;
                }
                else
                {
                    openLifeSkill_MakePreviewParm.lucky = false;
                }

                UIManager.OpenUI(EUIID.UI_LifeSkill_MakeTips, false, openLifeSkill_MakePreviewParm);
            }
        }

        private void OnForgetSkillClicked()
        {
            PromptBoxParameter.Instance.OpenPromptBox(2010152, 0, () => { Sys_LivingSkill.Instance.LifeSkill_SkillForgetReq(livingSkill.SkillId); });
        }

        private void OnUpdateLuckyValue(uint skillId, uint luckyValue)
        {
            if (livingSkill.SkillId != skillId)
            {
                return;
            }

            if (livingSkill.cSVLifeSkillData.lucky__value_max == 0)
            {
                m_Lucky.SetActive(false);
                return;
            }

            m_Lucky.SetActive(true);
            m_LuckSlider.wholeNumbers = true;
            m_LuckSlider.maxValue = livingSkill.cSVLifeSkillData.lucky__value_max;
            m_LuckSlider.value = luckyValue;
            m_LuckValue.text = string.Format("{0}/{1}", luckyValue, livingSkill.cSVLifeSkillData.lucky__value_max);
            m_FxLucky.SetActive(livingSkill.b_LuckyFull);
        }

        private void OnLuckyButtonClicked()
        {
            m_LuckyDesGo.SetActive(true);
            TextHelper.SetText(m_LuckyTitle, 2010170);
            
            uint max = livingSkill.cSVLifeSkillData.equip_orange_times;
            uint cur = livingSkill.orangeCount;
            TextHelper.SetText(m_LuckyDes, 2010171, max.ToString(), cur.ToString(), max.ToString());
        }

        private void RemoveAllButtonListeners()
        {
            foreach (var item in itemBtns)
            {
                item.onClick.RemoveAllListeners();
            }
        }

        private void LoadMakeIconAssetAsyn(string path)
        {
            AddressablesUtil.InstantiateAsync(ref requestRefMake, path, OnAssetsLoaded1);
        }

        private void OnAssetsLoaded1(AsyncOperationHandle<GameObject> handle)
        {
            makeAnim = handle.Result;
            if (null != makeAnim)
            {
                makeAnim.transform.SetParent(makeAnimIconParent);
                RectTransform rectTransform = makeAnim.transform as RectTransform;
                rectTransform.localPosition = Vector3.zero;
                rectTransform.localEulerAngles = Vector3.zero;
                rectTransform.localScale = Vector3.one;
            }
        }

        private void DestroyMakeAnim()
        {
            isMaking = false;
            if (null != makeAnim)
            {
                GameObject.Destroy(makeAnim);
                makeAnim = null;
            }

            sliderImage.fillAmount = 0;
            viewProcess.SetActive(false);
        }

        protected override void OnDestroy()
        {
            m_UI_CurrencyTitle.Dispose();
            A?.Kill();
            curView2MenuParentLable = -1;
            curView3MenuParentLable = -1;
        }

        private void OnSkipToItem(uint formulaId)
        {
            int index = dropDownSkillformulas.IndexOf(formulaId);
            if (index > -1)
            {
                infinity_make.MoveToIndex(index);
            }
        }
    }
}