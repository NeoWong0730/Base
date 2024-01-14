using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Lib.Core;
using Table;
using UnityEngine.EventSystems;
using Net;


namespace Logic
{
    public partial class UI_Cooking_Multiple : UIBase
    {
        private ScrollGridVertical m_ScrollGridVertical_Left;
        private Dictionary<GameObject, RecipeGrid> m_Grids = new Dictionary<GameObject, RecipeGrid>();
        private int m_CurSelectRecipeIndex;
        private GameObject m_MemberGo;
        private GameObject m_CaptainGo;
        private GameObject m_CookItem;
        private Text m_Text_Stage;
        private Text m_CookName;
        private Image m_CookIcon;
        private Dropdown m_Dropdown;
        private int m_CurSelectRecipeTab;
        private int curSelectRecipeTab
        {
            get { return m_CurSelectRecipeTab; }
            set
            {
                if (m_CurSelectRecipeTab != value)
                {
                    m_CurSelectRecipeTab = value;
                    OnCurSelectTabChanged();
                }
            }
        }
        private List<Cooking> m_RecipeCookings = new List<Cooking>();   //已激活多人食谱
        private Cooking m_CurSelectCooking;

        protected override void OnLoaded()
        {
            RegisterLeft();
            RegisterRight();
        }

        protected override void ProcessEvents(bool toRegister)
        {
            Sys_Cooking.Instance.eventEmitter.Handle<uint>(Sys_Cooking.EEvents.OnCookSelectRes, SetIngredient, toRegister);
            Sys_Cooking.Instance.eventEmitter.Handle(Sys_Cooking.EEvents.OnExchangePosition, OnExchangePosition, toRegister);
        }

        private void RegisterLeft()
        {
            m_ScrollGridVertical_Left = transform.Find("Animator/Left/Captain/Scroll View").GetComponent<ScrollGridVertical>();
            m_MemberGo = transform.Find("Animator/Left/Member").gameObject;
            m_CookItem = transform.Find("Animator/Left/Member/Item").gameObject;
            m_Text_Stage= transform.Find("Animator/Left/Member/Text_Stage").GetComponent<Text>();
            m_CookName = transform.Find("Animator/Left/Member/Item/Text").GetComponent<Text>();
            m_CookIcon = transform.Find("Animator/Left/Member/Item/Image_icon_bg/Image_icon").GetComponent<Image>();
            m_CaptainGo = transform.Find("Animator/Left/Captain").gameObject;
            m_CloseButton = transform.Find("Animator/View_Title/Btn_Close").GetComponent<Button>();
            m_CookButton = transform.Find("Animator/Right/Btn_01").GetComponent<Button>();
            m_Dropdown = transform.Find("Animator/Left/Captain/Dropdown").GetComponent<Dropdown>();
            m_CloseButton.onClick.AddListener(OnCloseButtonClicked);
            m_CookButton.onClick.AddListener(OnCookButtonClicked);
            m_Dropdown.onValueChanged.AddListener(OnClickDrop);
            m_ScrollGridVertical_Left.AddCellListener(OnCellUpdateCallback_Left);
            m_ScrollGridVertical_Left.AddCreateCellListener(OnCreateCellCallback_Left);
            AddDropdowmItems();
        }

        protected override void OnInit()
        {
            m_CurSelectRecipeTab = 0;
            m_CurSelectRecipeIndex = -1;
            Sys_Cooking.Instance.SortCooking();
        }

        protected override void OnShow()
        {
            SetCookingData();
            UpdateInfo();
            RefreshRecipe();
            SetMember();
            SetIngredient(m_CurSelectCooking == null ? 0 : m_CurSelectCooking.id);
            if (null != m_CurSelectCooking)
            {
                Sys_Cooking.Instance.CookSelectReq(m_CurSelectCooking.id);
            }
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
            Cooking cooking = Sys_Cooking.Instance.GetCooking(Sys_Cooking.Instance.lastMultiCookId);
            List<Cooking> temp = new List<Cooking>();
            if (m_CurSelectRecipeTab == 0)
            {
                temp = Sys_Cooking.Instance.cookings;
                if (cooking != null)
                {
                    m_RecipeCookings.Add(cooking);
                }
            }
            else if (m_CurSelectRecipeTab == 1)
            {
                temp = Sys_Cooking.Instance.type_1;
                if (cooking != null && cooking.cSVCookData.food_type == 1)
                {
                    m_RecipeCookings.Add(cooking);
                }
            }
            else if (m_CurSelectRecipeTab == 2)
            {
                temp = Sys_Cooking.Instance.type_2;
                if (cooking != null && cooking.cSVCookData.food_type == 2)
                {
                    m_RecipeCookings.Add(cooking);
                }
            }
            else if (m_CurSelectRecipeTab == 3)
            {
                temp = Sys_Cooking.Instance.type_3;
                if (cooking != null && cooking.cSVCookData.food_type == 3)
                {
                    m_RecipeCookings.Add(cooking);
                }
            }
            else if (m_CurSelectRecipeTab == 4)
            {
                temp = Sys_Cooking.Instance.type_4;
                if (cooking != null && cooking.cSVCookData.food_type == 4)
                {
                    m_RecipeCookings.Add(cooking);
                }
            }
            for (int i = 0; i < temp.Count; i++)
            {
                if (temp[i].cookType != 3)      //单人
                    continue;
                if (temp[i].active == 1)
                {
                    m_RecipeCookings.AddOnce<Cooking>(temp[i]);
                }
            }
            //m_CurSelectCooking = m_RecipeCookings[m_CurSelectRecipeIndex];
        }

        private void UpdateInfo()
        {
            if (Sys_Cooking.Instance.bIsCookingCaptain)
            {
                m_MemberGo.SetActive(false);
                m_CaptainGo.SetActive(true);
                m_CookButton.gameObject.SetActive(true);
            }
            else
            {
                m_MemberGo.SetActive(true);
                m_CaptainGo.SetActive(false);
                m_CookButton.gameObject.SetActive(false);
                m_CookItem.SetActive(false);//初始化默认未选择
                TextHelper.SetText(m_Text_Stage, 1003080);
            }
        }

        private void OnClickDrop(int value)
        {
            curSelectRecipeTab = value;
        }

        private void OnCurSelectTabChanged()
        {
            m_CurSelectRecipeIndex = -1;
            m_CurSelectCooking = null;
            SetIngredient(0);
            SetCookingData();
            RefreshRecipe();
        }

        private void RefreshRecipe()
        {
            if (Sys_Cooking.Instance.bIsCookingCaptain)
            {
                m_ScrollGridVertical_Left.SetCellCount(m_RecipeCookings.Count);
                LeftRecipeSelect();
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

        private void OnCellUpdateCallback_Left(ScrollGridCell cell)
        {
            RecipeGrid gridWarp = m_Grids[cell.gameObject];
            gridWarp.SetData(m_RecipeCookings[cell.index], cell.index);
            if (m_CurSelectRecipeIndex == cell.index)
            {
                gridWarp.Select();
            }
            else
            {
                gridWarp.Release();
            }
        }

        private void OnCreateCellCallback_Left(ScrollGridCell cell)
        {
            RecipeGrid gridWarp = new RecipeGrid();
            gridWarp.BindGameObject(cell.gameObject);
            gridWarp.AddEvent(OnRecipeItemSelected);
            m_Grids[cell.gameObject] = gridWarp;
        }

        private void OnRecipeItemSelected(RecipeGrid gridWarp)
        {
            if (m_CurSelectRecipeIndex != gridWarp.dataIndex)
            {
                m_CurSelectRecipeIndex = gridWarp.dataIndex;
                LeftRecipeSelect();
                m_CurSelectCooking = gridWarp.cooking;
                Sys_Cooking.Instance.CookSelectReq(m_CurSelectCooking.id);
            }
        }

        private void OnCloseButtonClicked()
        {
            Sys_Cooking.Instance.CookCancelReq(0);
            UIManager.CloseUI(EUIID.UI_Cooking_Multiple);
        }

        public class RecipeGrid
        {
            public int dataIndex;
            public Cooking cooking;
            public GameObject gameObject;
            private GameObject m_Select;
            private GameObject m_Recently;
            private Image m_Icon;
            private Text m_Name;
            private Image m_EventBg;
            private Action<RecipeGrid> m_OnClick;

            public void BindGameObject(GameObject @object)
            {
                gameObject = @object;

                m_EventBg = gameObject.transform.Find("Image_BG").GetComponent<Image>();
                m_Icon = gameObject.transform.Find("Image_icon_bg/Image_icon").GetComponent<Image>();
                m_Name = gameObject.transform.Find("Text").GetComponent<Text>();
                m_Select = gameObject.transform.Find("Image_Selected").gameObject;
                m_Recently = gameObject.transform.Find("Image_Recently").gameObject;
                Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(m_EventBg.gameObject);
                eventListener.AddEventListener(EventTriggerType.PointerClick, OnGridClicked);
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
                m_Recently.SetActive(cooking.id == Sys_Cooking.Instance.lastMultiCookId);
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
        }
    }
}


