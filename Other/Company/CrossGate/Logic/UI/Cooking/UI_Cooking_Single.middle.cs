using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Lib.Core;
using Table;
using UnityEngine.EventSystems;
using Net;
using static Packet.CmdCookCookReq.Types;

namespace Logic
{
    public partial class UI_Cooking_Single : UIBase
    {
        private int m_CurSelectMidGridIndex;
        private List<MidGrid> m_MidGrids = new List<MidGrid>();
        private List<MidRecipeGrid> m_MidRecipeGrids = new List<MidRecipeGrid>();
        private Dictionary<int, ItemData> m_Step1 = new Dictionary<int, ItemData>();
        private Dictionary<int, ItemData> m_Step2 = new Dictionary<int, ItemData>();
        private bool m_RecipeStep1Enough;
        private bool m_RecipeStep2Enough;
        private CP_ToggleRegistry m_CP_ToggleRegistry_Step;             //阶段切换
        private int m_CurStep;                                 //当前食材页签
        private int curStep
        {
            get { return m_CurStep; }
            set
            {
                if (m_CurStep != value)
                {
                    m_CurStep = value;
                    m_CurSelectMidGridIndex = 0;
                    OnRefreshStep();
                }
            }
        }
        private CP_ToggleRegistry m_CP_ToggleRegistry_Kitchen;
        private uint m_CurSelectKitchen_Step1;
        private uint m_CurSelectKitchen_Step2;

        private Button m_CookButton;
        private Button m_CloseButton;
        private GameObject m_StepRoot;
        private Transform m_MidGridsParent1;    //自由烹饪
        private Transform m_MidGridsParent2;    //食谱烹饪
        private GameObject m_NotSupportSuperCooking;
        private GameObject m_CooRecipeEmptyBg;
        private GameObject m_CookRoot;
        private Image m_ToolIcon;
        private Image m_ToolIcon2;

        //批量烹饪
        private GameObject m_ImageNumber;
        private UI_Common_Num m_NumUI;
        private Button m_btnSub;
        private Button m_btnAdd;
        private Button m_btnMax;
        private uint m_DefaultCookCount = 1;
        private uint m_curCookCount = 1;

        private void RegisterMid()
        {
            m_ToolIcon = transform.Find("Animator/Center/Bottom/Image").GetComponent<Image>();
            m_ToolIcon2 = transform.Find("Animator/Center/Bottom/Image (2)").GetComponent<Image>();
            m_StepRoot = transform.Find("Animator/Center/Cook/Tab").gameObject;
            m_CookRoot = transform.Find("Animator/Center/Cook").gameObject;
            m_CookButton = transform.Find("Animator/Center/Cook/Btn_start").GetComponent<Button>();
            m_CloseButton = transform.Find("Animator/View_Title07/Btn_Close").GetComponent<Button>();
            m_NotSupportSuperCooking = transform.Find("Animator/Center/Bottom/Text_Tips").gameObject;
            m_CooRecipeEmptyBg = transform.Find("Animator/Center/Bottom/Text_Tips").gameObject;
            m_MidGridsParent1 = transform.Find("Animator/Center/Cook/Material");
            m_MidGridsParent2 = transform.Find("Animator/Center/Cook/Material2");
            m_CP_ToggleRegistry_Step = transform.Find("Animator/Center/Cook/Tab").GetComponent<CP_ToggleRegistry>();
            m_CP_ToggleRegistry_Step.onToggleChange = OnStepChanged;
            for (int i = 0; i < m_MidGridsParent1.childCount; i++)
            {
                GameObject gameObject = m_MidGridsParent1.GetChild(i).gameObject;
                MidGrid midGrid = new MidGrid();
                midGrid.BindGameObject(gameObject);
                midGrid.SetIndex(i);
                midGrid.AddEvent(OnMidGridSelect, OnMidGridLongPressed, OnReplaced);
                m_MidGrids.Add(midGrid);
                m_Step1[i] = null;
                m_Step2[i] = null;
            }
            for (int i = 0; i < m_MidGridsParent2.childCount; i++)
            {
                GameObject gameObject = m_MidGridsParent2.GetChild(i).gameObject;
                MidRecipeGrid midGrid = new MidRecipeGrid();
                midGrid.BindGameObject(gameObject);
                midGrid.SetIndex(i);
                midGrid.AddEvent(null, null);
                m_MidRecipeGrids.Add(midGrid);
            }
            m_CP_ToggleRegistry_Kitchen = transform.Find("Animator/Center/Cook/Toggle").GetComponent<CP_ToggleRegistry>();
            m_CP_ToggleRegistry_Kitchen.onToggleChange = OnKitchenChanged;
            m_CookButton.onClick.AddListener(OnCookButtonClicked);
            m_CloseButton.onClick.AddListener(OnCloseButtonClicked);
            AddKitchenCondition();

            //批量烹饪
            m_ImageNumber =transform.Find("Animator/Center/Cook/Image_Number").gameObject;
            m_NumUI = new UI_Common_Num();
            m_NumUI.Init(transform.Find("Animator/Center/Cook/Image_Number/InputField_Number"), 99999);
            m_NumUI.SetOffset(Sys_NumInput.NumInputOffsetDir.Bottom);
            m_NumUI.RegEnd(OnInputEnd);

            m_btnSub = transform.Find("Animator/Center/Cook/Image_Number/Button_Sub").GetComponent<Button>();
            UI_LongPressButton LongPressSubButton = m_btnSub.gameObject.AddComponent<UI_LongPressButton>();
            LongPressSubButton.interval = 0.3f;
            LongPressSubButton.bPressAcc = true;
            LongPressSubButton.onRelease.AddListener(OnClickBtnSub);
            LongPressSubButton.OnPressAcc.AddListener(OnClickBtnSub);


            m_btnAdd = transform.Find("Animator/Center/Cook/Image_Number/Button_Add").GetComponent<Button>();
            UI_LongPressButton LongPressAddButton = m_btnAdd.gameObject.AddComponent<UI_LongPressButton>();
            LongPressAddButton.interval = 0.3f;
            LongPressAddButton.bPressAcc = true;
            LongPressAddButton.onRelease.AddListener(OnClickBtnAdd);
            LongPressAddButton.OnPressAcc.AddListener(OnClickBtnAdd);

            m_btnMax = transform.Find("Animator/Center/Cook/Image_Number/Button_Max").GetComponent<Button>();
            m_btnMax.onClick.AddListener(OnClickBtnMax);
        }

        private void AddKitchenCondition()
        {
            m_CP_ToggleRegistry_Kitchen.ClearCondition();
            m_CP_ToggleRegistry_Kitchen.AddCondition(1, ConditionKitchen1);
            m_CP_ToggleRegistry_Kitchen.AddCondition(2, ConditionKitchen2);
            m_CP_ToggleRegistry_Kitchen.AddCondition(3, ConditionKitchen3);
            m_CP_ToggleRegistry_Kitchen.AddCondition(4, ConditionKitchen4);
        }

        private bool ConditionKitchen1()
        {
            bool support = false;
            if (m_CurSelectCookingModel == 1 || m_CurSelectCookingModel == 2)
            {
                support = m_SupportKitchen.Contains(1);
                if (!support)
                {
                    //提示
                    Sys_Cooking.Instance.KitchenNotSupport();
                }
                return support;
            }
            else
            {
                support = false;
                if (m_CurStep == 1)
                {
                    support = m_CurSelectCooking.cSVCookData.tool1 == 1;
                }
                else
                {
                    support = m_CurSelectCooking.cSVCookData.tool2 == 1;
                }
                if (!support)
                {
                    Sys_Cooking.Instance.FixKitchen();
                    return false;
                }
            }
            if (!Sys_Cooking.Instance.ToolValid(1))
            {
                return false;
            }
            return true;
        }

        private bool ConditionKitchen2()
        {
            bool support = false;
            if (m_CurSelectCookingModel == 1 || m_CurSelectCookingModel == 2)
            {
                support = m_SupportKitchen.Contains(2);
                if (!support)
                {
                    //提示
                    Sys_Cooking.Instance.KitchenNotSupport();
                }
                return support;
            }
            else
            {
                support = false;
                if (m_CurStep == 1)
                {
                    support = m_CurSelectCooking.cSVCookData.tool1 == 2;
                }
                else
                {
                    support = m_CurSelectCooking.cSVCookData.tool2 == 2;
                }
                if (!support)
                {
                    Sys_Cooking.Instance.FixKitchen();
                    return false;
                }
            }
            if (!Sys_Cooking.Instance.ToolValid(2))
            {
                return false;
            }
            return true;
        }

        private bool ConditionKitchen3()
        {
            bool support = false;
            if (m_CurSelectCookingModel == 1 || m_CurSelectCookingModel == 2)
            {
                support = m_SupportKitchen.Contains(3);
                if (!support)
                {
                    Sys_Cooking.Instance.KitchenNotSupport();
                }
                return support;
            }
            else
            {
                support = false;
                if (m_CurStep == 1)
                {
                    support = m_CurSelectCooking.cSVCookData.tool1 == 3;
                }
                else
                {
                    support = m_CurSelectCooking.cSVCookData.tool2 == 3;
                }
                if (!support)
                {
                    Sys_Cooking.Instance.FixKitchen();
                    return false;
                }
            }
            if (!Sys_Cooking.Instance.ToolValid(3))
            {
                return false;
            }
            return true;
        }

        private bool ConditionKitchen4()
        {
            bool support = false;
            if (m_CurSelectCookingModel == 1 || m_CurSelectCookingModel == 2)
            {
                support = m_SupportKitchen.Contains(4);
                if (!support)
                {
                    //提示
                    Sys_Cooking.Instance.KitchenNotSupport();
                }
                return support;
            }
            else
            {
                support = false;
                if (m_CurStep == 1)
                {
                    support = m_CurSelectCooking.cSVCookData.tool1 == 4;
                }
                else
                {
                    support = m_CurSelectCooking.cSVCookData.tool2 == 4;
                }
                if (!support)
                {
                    Sys_Cooking.Instance.FixKitchen();
                    return false;
                }
            }
            if (!Sys_Cooking.Instance.ToolValid(4))
            {
                return false;
            }
            return true;
        }

        private void UpdateSelectKitchen()
        {
            if (m_CurSelectCookingModel == 1)
            {
                UpdateNomal1SelectKitchen();
            }
            else if (m_CurSelectCookingModel == 2)
            {
                UpdateNomal2SelectKitchen();
            }
            else
            {
                UpdateRecipeSelectKitchen();
            }
        }

        private void ClearMidSelection(bool refresh = true)
        {
            if (refresh)
            {
                foreach (var item in m_Step1)
                {
                    ItemData itemData = item.Value;
                    if (itemData != null)
                    {
                        OnReplaced(itemData);
                    }
                }
                foreach (var item in m_Step2)
                {
                    ItemData itemData = item.Value;
                    if (itemData != null)
                    {
                        OnReplaced(itemData);
                    }
                }
            }
            for (int i = 0; i < 4; i++)
            {
                m_Step1[i] = null;
                m_Step2[i] = null;
            }
            m_CurStep = 1;
        }

        private void OnClearMidSelection()
        {
            ClearMidSelection(false);
        }

        private void RefreshMiddleView()
        {
            m_CooRecipeEmptyBg.SetActive(false);
            m_m_LeftCooRecipeEmptyBg.SetActive(false);
            m_CookRoot.SetActive(true);
            if (m_CurSelectCookingModel == 1)
            {
                m_MidGridsParent1.gameObject.SetActive(true);
                m_MidGridsParent2.gameObject.SetActive(false);
                m_StepRoot.SetActive(false);
                m_ImageNumber.SetActive(false);
                m_CurStep = 1;
                OnRefreshStep();
            }
            else if (m_CurSelectCookingModel == 2)
            {
                m_MidGridsParent1.gameObject.SetActive(true);
                m_MidGridsParent2.gameObject.SetActive(false);
                m_StepRoot.SetActive(true);
                m_CP_ToggleRegistry_Step.SwitchTo(m_CurStep);
                OnRefreshStep();
            }
            else
            {
                m_ImageNumber.SetActive(true);
                InitCurrCount();
                if (m_RecipeCookings.Count == 0)
                {
                    m_CooRecipeEmptyBg.SetActive(true);
                    m_m_LeftCooRecipeEmptyBg.SetActive(true);
                    m_CookRoot.SetActive(false);
                }
                else
                {
                    m_CooRecipeEmptyBg.SetActive(false);
                    m_m_LeftCooRecipeEmptyBg.SetActive(false);
                    m_CookRoot.SetActive(true);
                    m_MidGridsParent1.gameObject.SetActive(false);
                    m_MidGridsParent2.gameObject.SetActive(true);
                    if (m_CurSelectCooking.cookType == 1)
                    {
                        m_StepRoot.SetActive(false);
                        m_CurStep = 1;
                        OnRefreshStep();
                        m_NotSupportSuperCooking.SetActive(false);
                        ButtonHelper.Enable(m_CookButton, true);
                    }
                    else if (m_CurSelectCooking.cookType == 2)
                    {
                        m_StepRoot.SetActive(true);
                        m_CP_ToggleRegistry_Step.SwitchTo(m_CurStep);
                        OnRefreshStep();
                        //此处不支持多段烹饪
                        m_NotSupportSuperCooking.SetActive(!m_SupportModel.Contains(2));
                        ButtonHelper.Enable(m_CookButton, m_SupportModel.Contains(2));
                    }
                }
            }
        }

        private void OnCloseButtonClicked()
        {
            UIManager.CloseUI(EUIID.UI_Cooking_Single);
        }

        private void OnCookButtonClicked()
        {
            if (m_CurSelectCookingModel == 1)
            {
                bool step1FullSelected = true;
                List<uint> items = new List<uint>();
                foreach (var item in m_Step1)
                {
                    if (item.Value == null)
                    {
                        step1FullSelected = false;
                    }
                    else
                    {
                        items.Add(item.Value.Id);
                    }
                }
                if (step1FullSelected)
                {
                    //请求烹饪
                    List<FreeStage> freeStages = new List<FreeStage>();
                    FreeStage freeStage = new FreeStage();
                    freeStage.ToolId = m_CurSelectKitchen_Step1;
                    for (int i = 0; i < items.Count; i++)
                    {
                        freeStage.FoodIds.Add(items[i]);
                    }
                    freeStages.Add(freeStage);
                    uint cookId = Sys_Cooking.Instance.CalFree1CookId(m_CurSelectKitchen_Step1, items);
                    Cooking cooking = Sys_Cooking.Instance.GetCooking(cookId);
                    if (cooking == null)
                    {
                        cookId = 0;
                    }
                    else
                    {
                        if (cooking.cookType == 2)
                        {
                            cookId = 0;
                        }
                    }
                    Sys_Cooking.Instance.CookCookReq(1, true, cookId, freeStages);
                    //ClearMidSelection(false);
                    //UIManager.CloseUI(EUIID.UI_Cooking_Single);
                }
                else
                {
                    Sys_Cooking.Instance.FoodNotEnough(3);
                }
            }
            else if (m_CurSelectCookingModel == 2)
            {
                bool step1FullSelected = true;
                List<uint> items1 = new List<uint>();
                List<uint> items2 = new List<uint>();
                foreach (var item in m_Step1)
                {
                    if (item.Value == null)
                    {
                        step1FullSelected = false;
                    }
                    else
                    {
                        items1.Add(item.Value.Id);
                    }
                }
                bool step2FullSelected = true;
                foreach (var item in m_Step2)
                {
                    if (item.Value == null)
                    {
                        step2FullSelected = false;
                    }
                    else
                    {
                        items2.Add(item.Value.Id);
                    }
                }
                if (step1FullSelected && step2FullSelected)
                {
                    //请求烹饪
                    List<FreeStage> freeStages = new List<FreeStage>();
                    FreeStage freeStage1 = new FreeStage();
                    freeStage1.ToolId = m_CurSelectKitchen_Step1;
                    for (int i = 0; i < items1.Count; i++)
                    {
                        freeStage1.FoodIds.Add(items1[i]);
                    }
                    FreeStage freeStage2 = new FreeStage();
                    freeStage2.ToolId = m_CurSelectKitchen_Step2;
                    for (int i = 0; i < items2.Count; i++)
                    {
                        freeStage2.FoodIds.Add(items2[i]);
                    }
                    freeStages.Add(freeStage1);
                    freeStages.Add(freeStage2);
                    uint cookId = Sys_Cooking.Instance.CalFree2CookId(m_CurSelectKitchen_Step1, items1, m_CurSelectKitchen_Step2, items2);
                    Sys_Cooking.Instance.CookCookReq(2, true, cookId, freeStages);
                    //ClearMidSelection(false);
                    //UIManager.CloseUI(EUIID.UI_Cooking_Single);
                }
                else if (!step1FullSelected)
                {
                    Sys_Cooking.Instance.FoodNotEnough(1);
                }
                else if (!step2FullSelected)
                {
                    Sys_Cooking.Instance.FoodNotEnough(2);
                }
            }
            else
            {
                if (m_CurSelectCooking.cookType == 1)
                {
                    if (!m_RecipeStep1Enough)
                    {
                        //缺少食材，无法开始烹饪
                        Sys_Cooking.Instance.FoodNotEnough(3);
                        return;
                    }
                    else if (!m_SupportKitchen.Contains(m_CurSelectKitchen_Step1))
                    {
                        //厨具不支持
                        Sys_Cooking.Instance.KitchenNotMatch();
                        return;
                    }
                    else if (!Sys_Cooking.Instance.ToolValid(m_CurSelectKitchen_Step1))
                    {
                        return;
                    }
                }
                else if (m_CurSelectCooking.cookType == 2)
                {
                    if (!m_RecipeStep1Enough)
                    {
                        //缺少食材，无法开始烹饪
                        Sys_Cooking.Instance.FoodNotEnough(1);
                        return;
                    }
                    else if (!m_RecipeStep2Enough)
                    {
                        //缺少食材，无法开始烹饪
                        Sys_Cooking.Instance.FoodNotEnough(2);
                        return;
                    }
                    else if (!m_SupportKitchen.Contains(m_CurSelectKitchen_Step1) || !m_SupportKitchen.Contains(m_CurSelectKitchen_Step2))
                    {
                        //厨具不支持
                        Sys_Cooking.Instance.KitchenNotMatch();
                        return;
                    }
                    else if (!Sys_Cooking.Instance.ToolValid(m_CurSelectKitchen_Step1) || !Sys_Cooking.Instance.ToolValid(m_CurSelectKitchen_Step2))
                    {
                        return;
                    }
                }
                //请求烹饪
                Sys_Cooking.Instance.CookCookReq(m_CurSelectCooking.cookType, false, m_CurSelectCooking.id, null,m_curCookCount);
                //ClearMidSelection(false);
                //UIManager.CloseUI(EUIID.UI_Cooking_Single);
            }
        }

        private void OnInputEnd(uint num)
        {
            if (!Sys_Cooking.Instance.CheckBatchCookingIsOpen())
            {
                InitCurrCount();
                return;
            }
            uint result = num;
            if (result >0)
            {
                uint maxCount = CaculateBatchCount();
                result = result > maxCount ? maxCount : result;
            }
            else
            {
                result = m_DefaultCookCount;
            }
            m_curCookCount = result;
            m_NumUI.SetData(result);
        }
        private void OnClickBtnSub()
        {
            if (!Sys_Cooking.Instance.CheckBatchCookingIsOpen())
            {
                InitCurrCount();
                return;
            }
            if (m_curCookCount > 1)
            {
                m_curCookCount--;
            }
            m_NumUI.SetData(m_curCookCount);
        }

        private void OnClickBtnAdd()
        {
            if (!Sys_Cooking.Instance.CheckBatchCookingIsOpen())
            {
                InitCurrCount();
                return;
            }
            uint maxCount = CaculateBatchCount();
            if (m_curCookCount < maxCount)
            {
                m_curCookCount++;
            }
            //else
            //{
            //    Sys_Cooking.Instance.FoodNotEnough(3);
            //}

            m_NumUI.SetData(m_curCookCount);
        }

        private void OnClickBtnMax()
        {
            if (!Sys_Cooking.Instance.CheckBatchCookingIsOpen())
            {
                InitCurrCount();
                return;
            }
            m_curCookCount = CaculateBatchCount();
            m_NumUI.SetData(m_curCookCount);
        }
        private void OnKitchenChanged(int curToggle, int old)
        {
            if (m_CurStep == 1)
            {
                m_CurSelectKitchen_Step1 = (uint)curToggle;
                ImageHelper.SetIcon(m_ToolIcon, Sys_Cooking.Instance.GetToolIcon(m_CurSelectKitchen_Step1), true);
                ImageHelper.SetIcon(m_ToolIcon2, Sys_Cooking.Instance.GetToolIcon2(m_CurSelectKitchen_Step1), true);
            }
            if (m_CurStep == 2)
            {
                m_CurSelectKitchen_Step2 = (uint)curToggle;
                ImageHelper.SetIcon(m_ToolIcon, Sys_Cooking.Instance.GetToolIcon(m_CurSelectKitchen_Step2), true);
                ImageHelper.SetIcon(m_ToolIcon2, Sys_Cooking.Instance.GetToolIcon2(m_CurSelectKitchen_Step2), true);
            }
        }

        private void OnStepChanged(int curToggle, int old)
        {
            curStep = curToggle;
        }

        private void OnRefreshStep()
        {
            if (m_CurStep == 1)
            {
                m_CP_ToggleRegistry_Kitchen.SwitchTo((int)m_CurSelectKitchen_Step1);
                ImageHelper.SetIcon(m_ToolIcon, Sys_Cooking.Instance.GetToolIcon(m_CurSelectKitchen_Step1), true);
                ImageHelper.SetIcon(m_ToolIcon2, Sys_Cooking.Instance.GetToolIcon2(m_CurSelectKitchen_Step1), true);
            }
            if (m_CurStep == 2)
            {
                m_CP_ToggleRegistry_Kitchen.SwitchTo((int)m_CurSelectKitchen_Step2);
                ImageHelper.SetIcon(m_ToolIcon, Sys_Cooking.Instance.GetToolIcon(m_CurSelectKitchen_Step2), true);
                ImageHelper.SetIcon(m_ToolIcon2, Sys_Cooking.Instance.GetToolIcon2(m_CurSelectKitchen_Step2), true);
            }
            if (m_CurSelectCookingModel == 1 || m_CurSelectCookingModel == 2)
            {
                for (int i = 0; i < m_MidGrids.Count; i++)
                {
                    if (m_CurStep == 1)
                    {
                        m_MidGrids[i].InitData(m_Step1[i]);
                    }
                    else if (m_CurStep == 2)
                    {
                        m_MidGrids[i].InitData(m_Step2[i]);
                    }
                }
                AutoSelectNextEmptyGrid();
            }
            else
            {
                SetRecipeMidData();
                //设置厨具字体颜色
            }
        }

        private void SetRecipeMidData()
        {
            m_RecipeStep1Enough = true;
            m_RecipeStep2Enough = true;

            if (m_CurSelectCooking.cSVCookData.food1 != null)
            {
                for (int i = 0; i < m_CurSelectCooking.cSVCookData.food1.Count; i++)
                {
                    uint itemId = m_CurSelectCooking.cSVCookData.food1[i][0];
                    uint count = m_CurSelectCooking.cSVCookData.food1[i][1];
                    long bagCount = Sys_Bag.Instance.GetItemCount(itemId);
                    if (bagCount < count)
                    {
                        m_RecipeStep1Enough = false;
                        break;
                    }
                }
            }
            
            if (m_CurSelectCooking.cSVCookData.food2 != null)
            {
                for (int i = 0; i < m_CurSelectCooking.cSVCookData.food2.Count; i++)
                {
                    uint itemId = m_CurSelectCooking.cSVCookData.food2[i][0];
                    uint count = m_CurSelectCooking.cSVCookData.food2[i][1];
                    for (int j = 0; j < m_CurSelectCooking.cSVCookData.food1.Count; j++)
                    {
                        if (itemId== m_CurSelectCooking.cSVCookData.food1[j][0])
                        {
                            count += m_CurSelectCooking.cSVCookData.food1[j][1];
                        }
                    }
                    
                    long bagCount = Sys_Bag.Instance.GetItemCount(itemId);
                    if (bagCount < count)
                    {
                        m_RecipeStep2Enough = false;
                        break;
                    }
                }
            }

            if (m_CurStep == 1)
            {
                for (int i = 0; i < m_MidRecipeGrids.Count; i++)
                {
                    if (i < m_CurSelectCooking.cSVCookData.food1.Count)
                    {
                        uint itemId = m_CurSelectCooking.cSVCookData.food1[i][0];
                        uint count = m_CurSelectCooking.cSVCookData.food1[i][1];
                        m_MidRecipeGrids[i].gameObject.SetActive(true);
                        m_MidRecipeGrids[i].SetData(itemId, count);
                    }
                    else
                    {
                        m_MidRecipeGrids[i].gameObject.SetActive(false);
                    }
                }
            }
            else if (m_CurStep == 2)
            {
                for (int i = 0; i < m_MidRecipeGrids.Count; i++)
                {
                    if (i < m_CurSelectCooking.cSVCookData.food2.Count)
                    {
                        uint itemId = m_CurSelectCooking.cSVCookData.food2[i][0];
                        uint count = m_CurSelectCooking.cSVCookData.food2[i][1];
                        m_MidRecipeGrids[i].gameObject.SetActive(true);
                        m_MidRecipeGrids[i].SetData(itemId, count);
                    }
                    else
                    {
                        m_MidRecipeGrids[i].gameObject.SetActive(false);
                    }
                }
            }
        }

        private void SetNormalMidData(ItemData itemData)
        {
            for (int i = 0; i < m_MidGrids.Count; i++)
            {
                if (m_CurSelectMidGridIndex == m_MidGrids[i].dataIndex)
                {
                    m_MidGrids[i].SetData(itemData);
                    if (m_CurStep == 1)
                    {
                        m_Step1[i] = itemData;
                    }
                    else if (m_CurStep == 2)
                    {
                        m_Step2[i] = itemData;
                    }
                }
            }
            AutoSelectNextEmptyGrid();
        }

        private void OnMidGridSelect(MidGrid midGrid)
        {
            m_CurSelectMidGridIndex = midGrid.dataIndex;
            MidShowSelect();
        }

        private void OnMidGridLongPressed(MidGrid midGrid)
        {
            if (midGrid.itemData == null)
            {
                return;
            }
            PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(midGrid.itemData.Id, 0, false, false, false, false, false, false, false);
            itemData.bShowBtnNo = false;
            UIManager.OpenUI(EUIID.UI_Message_Box, false, new MessageBoxEvt(EUIID.UI_Cooking_Single, itemData));
        }

        private void MidShowSelect()
        {
            for (int i = 0; i < m_MidGrids.Count; i++)
            {
                if (m_MidGrids[i].dataIndex == m_CurSelectMidGridIndex)
                {
                    m_MidGrids[i].Select();
                }
                else
                {
                    m_MidGrids[i].Release();
                }
            }
        }

        private void AutoSelectNextEmptyGrid()
        {
            for (int i = 0; i < m_MidGrids.Count; i++)
            {
                if (m_MidGrids[i].empty)
                {
                    m_CurSelectMidGridIndex = i;
                    break;
                }
            }
            MidShowSelect();
        }
        private void InitCurrCount()
        {
            m_curCookCount = m_DefaultCookCount;
            m_NumUI.SetData(m_curCookCount);
        }
        private uint CaculateBatchCount()
        {
            List<List<uint>> _list = new List<List<uint>>();
            for (int n=0;n< m_CurSelectCooking.cSVCookData.food1.Count;n++)
            {
                _list.Add(m_CurSelectCooking.cSVCookData.food1[n]);
            }
            if (m_CurSelectCooking.cSVCookData.food2!=null)
            {
                for(int k=0;k< m_CurSelectCooking.cSVCookData.food2.Count;k++)
                {
                    _list.Add(m_CurSelectCooking.cSVCookData.food2[k]);
                }
            }
            long mCount = 1000;
            for (int i = 0; i < _list.Count; i++)
            {
                uint nowId = _list[i][0];
                uint _count = 0;
                for (int j = 0; j < _list.Count; j++)
                {
                    if (nowId==_list[j][0])
                    {
                        _count += _list[j][1];
                    }
                }
                long thisCount = Sys_Bag.Instance.GetItemCount(nowId) / _count;
                if (thisCount < mCount)
                {
                    mCount = thisCount;
                }
            }
            uint maxCount = (uint)mCount;
            if (maxCount<=0)
            {
                maxCount = m_DefaultCookCount;
            }
            if (maxCount>999)
            {
                maxCount = 999;
            }
            return maxCount;
        }

        private void OnMidRecipeGridLongPressed(MidRecipeGrid midRecipeGrid)
        {
            PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(midRecipeGrid.itemId, 0, false, false, false, false, false, false, false);
            itemData.bShowBtnNo = false;
            UIManager.OpenUI(EUIID.UI_Message_Box, false, new MessageBoxEvt(EUIID.UI_Cooking_Single, itemData));
        }


        public class MidGrid
        {
            public int dataIndex;
            public ItemData itemData;
            public GameObject gameObject;
            private GameObject m_Select;
            private GameObject m_Empty;
            private Image m_Icon;
            private Button m_ClickButton;
            private Action<MidGrid> m_OnClicked;
            private Action<MidGrid> m_OnLongPressed;
            private Action<ItemData> m_OnReplaced;
            public bool empty = true;

            public void BindGameObject(GameObject @object)
            {
                gameObject = @object;
                ParseCp();
            }

            private void ParseCp()
            {
                m_Select = gameObject.transform.Find("Image_select").gameObject;
                m_Empty = gameObject.transform.Find("Image_empty").gameObject;
                m_Icon = gameObject.transform.Find("ListItem/Btn_Item/Image_Icon").GetComponent<Image>();
                m_ClickButton = gameObject.transform.Find("ListItem/Btn_Item").GetComponent<Button>();
                m_ClickButton.onClick.AddListener(OnClicked);
            }

            public void SetIndex(int dataIndex)
            {
                this.dataIndex = dataIndex;
            }

            public void InitData(ItemData itemData)
            {
                empty = itemData == null ? true : false;
                this.itemData = itemData;
                Refresh();
            }

            public void SetData(ItemData itemData)
            {
                if (empty == false)
                {
                    m_OnReplaced?.Invoke(this.itemData);
                }
                this.itemData = itemData;
                empty = false;
                Refresh();
            }

            public void Refresh()
            {
                if (itemData == null)
                {
                    m_Empty.SetActive(true);
                    m_Icon.gameObject.SetActive(false);
                }
                else
                {
                    m_Icon.gameObject.SetActive(true);
                    m_Empty.SetActive(false);
                    ImageHelper.SetIcon(m_Icon, itemData.cSVItemData.icon_id);
                }
            }

            public void AddEvent(Action<MidGrid> onClick = null, Action<MidGrid> onLongPressed = null, Action<ItemData> onReplaced = null)
            {
                m_OnClicked = onClick;
                if (onLongPressed != null)
                {
                    m_OnLongPressed = onLongPressed;
                    UI_LongPressButton uI_LongPressButton = m_ClickButton.gameObject.GetNeedComponent<UI_LongPressButton>();
                    uI_LongPressButton.onStartPress.AddListener(OnLongPressed);
                }
                m_OnReplaced = onReplaced;
            }

            private void OnClicked()
            {
                m_OnClicked?.Invoke(this);
            }
            private void OnLongPressed()
            {
                m_OnLongPressed?.Invoke(this);
            }

            public void Release()
            {
                m_Select.SetActive(false);
            }

            public void Select()
            {
                m_Select.SetActive(true);
            }
        }

        public class MidRecipeGrid
        {
            public int dataIndex;
            public uint itemId;
            public uint count;
            public GameObject gameObject;
            private GameObject m_ButtonNo;
            private GameObject m_Select;
            private Text m_Text_Number;
            private Button m_ClickButton;
            private Action<MidRecipeGrid> m_OnClicked;
            private Action<MidRecipeGrid> m_OnLongPressed;
            private PropItem m_PropItem;

            public void BindGameObject(GameObject @object)
            {
                gameObject = @object;

                m_Select = gameObject.transform.Find("Image_select").gameObject;
                m_ClickButton = gameObject.transform.Find("ListItem/Btn_Item").GetComponent<Button>();
                m_ClickButton.onClick.AddListener(OnClicked);

                m_PropItem = new PropItem();
                m_PropItem.BindGameObject(gameObject.transform.Find("ListItem").gameObject);
                //m_PropItem.OnEnableLongPress(true);
            }

            public void SetIndex(int dataIndex)
            {
                this.dataIndex = dataIndex;
            }

            public void SetData(uint itemId, uint count)
            {
                this.itemId = itemId;
                this.count = count;
                Refresh();
            }

            public void Refresh()
            {
                PropIconLoader.ShowItemData showItem = new PropIconLoader.ShowItemData
                    (_id: itemId,
                    _count: count,
                    _bUseQuailty: true,
                    _bBind: false,
                    _bNew: false,
                    _bUnLock: false,
                    _bSelected: false,
                    _bShowCount: true,
                    _bShowBagCount: true,
                    _bUseClick: true,
                    _onClick: null,
                    _bshowBtnNo: false);
                m_PropItem.SetData(new MessageBoxEvt(EUIID.UI_Cooking_Single, showItem));
            }

            public void AddEvent(Action<MidRecipeGrid> onClick = null, Action<MidRecipeGrid> onLongPressed = null, Action<ItemData> onReplaced = null,
                Action onSetComplete = null)
            {
                m_OnClicked = onClick;
                if (onLongPressed != null)
                {
                    m_OnLongPressed = onLongPressed;
                    UI_LongPressButton uI_LongPressButton = m_ClickButton.gameObject.GetNeedComponent<UI_LongPressButton>();
                    uI_LongPressButton.onStartPress.AddListener(OnLongPressed);
                }
            }

            private void OnClicked()
            {
                m_OnClicked?.Invoke(this);
            }
            private void OnLongPressed()
            {
                m_OnLongPressed?.Invoke(this);
            }
        }
    }
}


