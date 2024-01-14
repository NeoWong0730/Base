using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Lib.Core;
using Table;
using UnityEngine.EventSystems;

namespace Logic
{
    public partial class UI_Cooking_Single : UIBase
    {
        private UI_CurrencyTitle m_UI_CurrencyTitle;
        private GameObject m_FreeRoot;
        private GameObject m_Free1;
        private GameObject m_Free2;
        private GameObject m_RecipeRoot;
        private GameObject m_m_LeftCooRecipeEmptyBg;
        private CP_ToggleRegistry m_CP_ToggleRegistry_CookingModel;//模式切换
        private int m_CurSelectCookingModel = 1;
        private Timer m_Timer;
        private int curSelectCookingModel
        {
            get { return m_CurSelectCookingModel; }
            set
            {
                if (m_CurSelectCookingModel != value)
                {
                    m_CurSelectCookingModel = value;
                    OnCookingModelChanged();
                }
            }
        }
        private InfinityGrid m_InfinityGrid_Left;
        private Dictionary<GameObject, RecipeGrid> m_Grids = new Dictionary<GameObject, RecipeGrid>();
        private Dropdown m_Dropdown;
        private int m_CurSelectRecipeTab;                       //食谱页签
        private int curSelectRecipeTab
        {
            get { return m_CurSelectRecipeTab; }
            set
            {
                if (m_CurSelectRecipeTab != value)
                {
                    m_CurSelectRecipeTab = value;
                    OnRecipeTabChanged();
                }
            }
        }
        private int m_CurSelectRecipeIndex;
        private List<Cooking> m_RecipeCookings = new List<Cooking>();   //已激活单人食谱
        private Cooking m_CurSelectCooking;


        private void RegisterLeft()
        {
            m_UI_CurrencyTitle = new UI_CurrencyTitle(transform.Find("Animator/UI_Property").gameObject);
            m_FreeRoot = transform.Find("Animator/Left/Unknow").gameObject;
            m_Free1 = transform.Find("Animator/Left/Unknow/Text_Explain").gameObject;
            m_Free2 = transform.Find("Animator/Left/Unknow/Text_Explain2").gameObject;
            m_RecipeRoot = transform.Find("Animator/Left/Image_Title2").gameObject;
            m_m_LeftCooRecipeEmptyBg = transform.Find("Animator/Left/Image_Title2/Empty").gameObject;
            m_CP_ToggleRegistry_CookingModel = transform.Find("Animator/Left/Toggle").GetComponent<CP_ToggleRegistry>();
            m_InfinityGrid_Left = transform.Find("Animator/Left/Image_Title2/Scroll View").GetComponent<InfinityGrid>();

            m_InfinityGrid_Left.onCreateCell += OnCreateCellCallback_Left;
            m_InfinityGrid_Left.onCellChange += OnCellUpdateCallback_Left;
            m_CP_ToggleRegistry_CookingModel.onToggleChange = OnCookingModelChanged;
            m_Dropdown = transform.Find("Animator/Left/Image_Title2/Dropdown").GetComponent<Dropdown>();
            m_Dropdown.onValueChanged.AddListener(OnClickDrop);
            AddDropdowmItems();
            AddCookingModelCondition();
        }

        private void AddCookingModelCondition()
        {
            m_CP_ToggleRegistry_CookingModel.ClearCondition();
            m_CP_ToggleRegistry_CookingModel.AddCondition(1, ConditionModel1);
            m_CP_ToggleRegistry_CookingModel.AddCondition(2, ConditionModel2);
            m_CP_ToggleRegistry_CookingModel.AddCondition(3, ConditionModel3);
        }

        private bool ConditionModel1()
        {
            bool support = Sys_Cooking.Instance.Free1StageValid();
            if (!support)
            {
                string content = LanguageHelper.GetTextContent(5932);
                Sys_Hint.Instance.PushContent_Normal(content);
            }
            return support;
        }

        private bool ConditionModel2()
        {
            bool support = m_SupportModel.Contains(2);
            if (!support)
            {
                string content = LanguageHelper.GetTextContent(5904);
                Sys_Hint.Instance.PushContent_Normal(content);
                return false;
            }
            support &= Sys_Cooking.Instance.Free2StageValid();
            if (!support)
            {
                string content = LanguageHelper.GetTextContent(5933);
                Sys_Hint.Instance.PushContent_Normal(content);
            }
            return support;
        }

        private bool ConditionModel3()
        {
            bool has = Sys_Cooking.Instance.HasRecipeActive();
            if (!has)
            {
                string content = LanguageHelper.GetTextContent(5918);
                Sys_Hint.Instance.PushContent_Normal(content);
            }
            return has;
        }

        private void AddDropdowmItems()
        {
            m_Dropdown.ClearOptions();
            List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
            Dropdown.OptionData op_0 = new Dropdown.OptionData();
            op_0.text = LanguageHelper.GetTextContent(1003052);
            options.Add(op_0);

            Dropdown.OptionData op_1 = new Dropdown.OptionData();
            op_1.text = LanguageHelper.GetTextContent(1003048);
            options.Add(op_1);

            Dropdown.OptionData op_2 = new Dropdown.OptionData();
            op_2.text = LanguageHelper.GetTextContent(1003049);
            options.Add(op_2);

            Dropdown.OptionData op_3 = new Dropdown.OptionData();
            op_3.text = LanguageHelper.GetTextContent(1003050);
            options.Add(op_3);

            Dropdown.OptionData op_4 = new Dropdown.OptionData();
            op_4.text = LanguageHelper.GetTextContent(1003051);
            options.Add(op_4);
            m_Dropdown.AddOptions(options);
        }

        private void SetCookingData()
        {
            m_RecipeCookings.Clear();
            Cooking lastCooking = Sys_Cooking.Instance.GetCooking(Sys_Cooking.Instance.lastCookId);
            List<Cooking> temp = new List<Cooking>();
            if (m_CurSelectRecipeTab == 0)
            {
                temp = Sys_Cooking.Instance.cookings;
                if (lastCooking != null)
                {
                    m_RecipeCookings.Add(lastCooking);
                }
            }
            else if (m_CurSelectRecipeTab == 1)
            {
                temp = Sys_Cooking.Instance.type_1;
                if (lastCooking != null && lastCooking.cSVCookData.food_type == 1)
                {
                    m_RecipeCookings.Add(lastCooking);
                }
            }
            else if (m_CurSelectRecipeTab == 2)
            {
                temp = Sys_Cooking.Instance.type_2;
                if (lastCooking != null && lastCooking.cSVCookData.food_type == 2)
                {
                    m_RecipeCookings.Add(lastCooking);
                }
            }
            else if (m_CurSelectRecipeTab == 3)
            {
                temp = Sys_Cooking.Instance.type_3;
                if (lastCooking != null && lastCooking.cSVCookData.food_type == 3)
                {
                    m_RecipeCookings.Add(lastCooking);
                }
            }
            else if (m_CurSelectRecipeTab == 4)
            {
                temp = Sys_Cooking.Instance.type_4;
                if (lastCooking != null && lastCooking.cSVCookData.food_type == 4)
                {
                    m_RecipeCookings.Add(lastCooking);
                }
            }
            for (int i = 0; i < temp.Count; i++)
            {
                //if (temp[i].cookType == 3)      //多人
                //    continue;
                if (temp[i].active == 1)
                {
                    m_RecipeCookings.AddOnce<Cooking>(temp[i]);
                }
            }
            if (m_CookId > 0)
            {
                for (int i = 0; i < m_RecipeCookings.Count; i++)
                {
                    if (m_CookId == m_RecipeCookings[i].id)
                    {
                        m_CurSelectRecipeIndex = i;
                    }
                }
                m_CookId = 0;
            }
            if (m_RecipeCookings.Count > 0)
            {
                m_CurSelectCooking = m_RecipeCookings[m_CurSelectRecipeIndex];
                UpdateRecipeSelectKitchen();
            }
        }

        private void RefreshLeftView()
        {
            if (m_CurSelectCookingModel == 1)
            {
                m_RecipeRoot.SetActive(false);
                m_FreeRoot.SetActive(true);
                m_Free1.SetActive(true);
                m_Free2.SetActive(false);
            }
            else if (m_CurSelectCookingModel == 2)
            {
                m_RecipeRoot.SetActive(false);
                m_FreeRoot.SetActive(true);
                m_Free1.SetActive(false);
                m_Free2.SetActive(true);
            }
            else if (m_CurSelectCookingModel == 3)
            {
                m_FreeRoot.SetActive(false);
                m_RecipeRoot.SetActive(true);
                OnRefreshRecipeModel();
            }
        }

        private void OnRefreshRecipeModel()
        {
            m_InfinityGrid_Left.CellCount = m_RecipeCookings.Count;
            m_InfinityGrid_Left.ForceRefreshActiveCell();
            if (m_JumpId > 0)
            {
                int m_index = 0;
                for (int i = 0; i < m_RecipeCookings.Count; i++)
                {
                    if (m_RecipeCookings[i].cSVCookData.id == m_JumpId)
                    {
                        m_index = i;
                        break;
                    }
                }
                m_JumpId = 0;
                m_CurSelectRecipeIndex = m_index;
                m_Timer?.Cancel();
                m_Timer = Timer.Register(0.3f, () =>
                 {
                     OnRefreshRecipe();
                 });
                return;
            }
            else
            {
                OnRefreshRecipe();
            }
        }

        private void OnRefreshRecipe()
        {
            m_InfinityGrid_Left.MoveToIndex(m_CurSelectRecipeIndex);
            LeftRecipeSelect();

        }
        private void OnCookingModelChanged(int curToggle, int old)
        {
            curSelectCookingModel = curToggle;
        }

        private void OnCookingModelChanged()
        {
            ClearMidSelection();
            m_CurSelectRecipeTab = 0;
            m_CurSelectRecipeIndex = 0;
            SetCookingData();
            UpdateSelectKitchen();
            RefreshLeftView();
            RefreshMiddleView();
            OnShowOrHideRightMask();
        }

        private void OnClickDrop(int value)
        {
            curSelectRecipeTab = value;
        }

        private void OnRecipeTabChanged()
        {
            m_CurSelectRecipeIndex = 0;
            SetCookingData();
            UpdateRecipeSelectKitchen();
            m_InfinityGrid_Left.CellCount = m_RecipeCookings.Count;
            m_InfinityGrid_Left.ForceRefreshActiveCell();
            LeftRecipeSelect();
            RefreshMiddleView();
        }

        private void UpdateNomal1SelectKitchen()
        {
            m_CurSelectKitchen_Step1 = m_SupportKitchen[0];
        }

        private void UpdateNomal2SelectKitchen()
        {
            m_CurSelectKitchen_Step1 = m_CurSelectKitchen_Step2 = m_SupportKitchen[0];
        }

        private void UpdateRecipeSelectKitchen()
        {
            if (m_CurSelectCooking == null)
            {
                return;
            }
            if (m_CurSelectCooking.cookType == 1)
            {
                m_CurSelectKitchen_Step1 = m_CurSelectCooking.cSVCookData.tool1;
            }
            else if (m_CurSelectCooking.cookType == 2)
            {
                m_CurSelectKitchen_Step1 = m_CurSelectCooking.cSVCookData.tool1;
                m_CurSelectKitchen_Step2 = m_CurSelectCooking.cSVCookData.tool2;
            }
        }

        private void LeftRecipeSelect()
        {
            foreach (var item in m_Grids)
            {
                if (item.Value.dataIndex == m_CurSelectRecipeIndex)
                {
                    item.Value.Select();
                }
                else
                {
                    item.Value.Release();
                }
            }
        }

        private void OnCellUpdateCallback_Left(InfinityGridCell cell, int index)
        {
            RecipeGrid gridWarp = cell.mUserData as RecipeGrid;
            gridWarp.SetData(m_RecipeCookings[index], index);
            if (m_CurSelectRecipeIndex == index)
            {
                gridWarp.Select();
            }
            else
            {
                gridWarp.Release();
            }
        }

        private void OnCreateCellCallback_Left(InfinityGridCell cell)
        {
            RecipeGrid gridWarp = new RecipeGrid();
            gridWarp.BindGameObject(cell.mRootTransform.gameObject);
            cell.BindUserData(gridWarp);
            gridWarp.AddEvent(OnRecipeItemSelected);
            m_Grids[cell.mRootTransform.gameObject] = gridWarp;
            LeftRecipeSelect();
        }

        private void OnRecipeItemSelected(RecipeGrid gridWarp)
        {
            if (m_CurSelectRecipeIndex != gridWarp.dataIndex)
            {
                if (gridWarp.cooking.cookType == 3)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(5935));
                    return;
                }
                else if (gridWarp.cooking.cookType == 2 && Sys_Cooking.Instance.curCookingLevel < 2)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(5936));
                    return;
                }
                m_CurSelectRecipeIndex = gridWarp.dataIndex;
                LeftRecipeSelect();
                m_CurSelectCooking = gridWarp.cooking;
                UpdateRecipeSelectKitchen();
                RefreshMiddleView();
            }
        }

        public class RecipeGrid
        {
            public int dataIndex;
            public Cooking cooking;
            public GameObject gameObject;
            private Image m_EventBg;
            private GameObject m_Select;
            private GameObject m_Multi;
            private GameObject m_Recently;
            private GameObject m_Advance;
            private GameObject m_Watch;
            private Image m_Icon;
            private Text m_Name;
            private Action<RecipeGrid> m_OnClick;
            private GameObject m_LockObj;
            private UI_LongPressButton m_longPressBtn;

            public void BindGameObject(GameObject @object)
            {
                gameObject = @object;
                ParseCp();
            }

            private void ParseCp()
            {
                m_EventBg = gameObject.transform.Find("Image_BG").GetComponent<Image>();
                m_Select = gameObject.transform.Find("Image_select").gameObject;
                m_Advance = gameObject.transform.Find("Image_Senior").gameObject;
                m_Multi = gameObject.transform.Find("Image_Multi").gameObject;
                m_Watch = gameObject.transform.Find("Image_Collect").gameObject;
                m_Recently = gameObject.transform.Find("Image_Recently").gameObject;
                m_LockObj = gameObject.transform.Find("Image_Black").gameObject;
                m_Name = gameObject.transform.Find("Text").GetComponent<Text>();
                m_Icon = gameObject.transform.Find("Image_icon_bg/Image_icon").GetComponent<Image>();
                Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(m_EventBg.gameObject);
                eventListener.AddEventListener(EventTriggerType.PointerClick, OnGridClicked);
                if (!this.gameObject.TryGetComponent(out this.m_longPressBtn))
                {
                    this.m_longPressBtn = this.gameObject.AddComponent<UI_LongPressButton>();
                }
                m_longPressBtn.onStartPress.AddListener(OnLongPressed);
            }

            public void SetData(Cooking cooking, int dataIndex)
            {
                this.cooking = cooking;
                this.dataIndex = dataIndex;
                Refresh();
            }

            public void Refresh()
            {
                ImageHelper.SetIcon(m_Icon, cooking.cSVCookData.icon);
                TextHelper.SetText(m_Name, cooking.cSVCookData.name);
                m_LockObj.SetActive(cooking.cookType == 3);
                m_Advance.SetActive(cooking.cookType == 2);
                m_Multi.SetActive(cooking.cookType == 3);
                m_Watch.SetActive(cooking.watch);
                m_Recently.SetActive(cooking.id == Sys_Cooking.Instance.lastCookId);
            }

            public void AddEvent(Action<RecipeGrid> action)
            {
                m_OnClick = action;
            }

            private void OnGridClicked(BaseEventData baseEventData)
            {
                m_OnClick.Invoke(this);
            }

            public void Release()
            {
                m_Select.SetActive(false);
            }

            public void Select()
            {
                m_Select.SetActive(true);
            }
            public void OnLongPressed()
            {
                UIManager.OpenUI(EUIID.UI_Message_Box, false, new MessageBoxEvt(EUIID.UI_Cooking_Single,
                new PropIconLoader.ShowItemData(cooking.cSVCookData.result[1], 0, true, false, false, false, false)));
            }
        }
    }
}


