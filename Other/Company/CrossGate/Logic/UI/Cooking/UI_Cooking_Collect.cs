using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Lib.Core;
using Table;
using UnityEngine.EventSystems;
using Net;
using Packet;


namespace Logic
{
    public partial class UI_Cooking_Collect : UIBase
    {
        private Text m_CurrentCookingState;

        private Slider m_ScoreSlider;
        private Text m_SliderNum;

        private Button m_CloseButton;
        private Button m_Cooking_KnowLedgeButton;
        private Button m_IngredientButton;
        private Button m_ShopButton;
        private GameObject m_RedPoint;

        private Button m_PageLeft;
        private Button m_PageRight;
        private CP_PageDot m_CP_PageDot;
        private int curPage;
        private Text m_Describe;
        private Text m_PageTitleName;

        private CP_ToggleRegistry m_CP_ToggleRegistry;

        private UI_CurrencyTitle UI_CurrencyTitle;

        private Button m_FindNpcButton;
        private Button m_KitchenButton;

        private uint m_DefultCookingFun;


        protected override void OnInit()
        {
            curPage = Sys_Cooking.Instance.curCookingLevel;

            for (uint i = 1911; i < 1915; i++)
            {
                m_Types.Add(i);
            }

            m_DefultCookingFun = CSVCookAttr.Instance.GetConfData(12).value;
        }

        protected override void OnLoaded()
        {
            UI_CurrencyTitle = new UI_CurrencyTitle(transform.Find("Animator/UI_Property").gameObject);

            m_CurrentCookingState = transform.Find("Animator/Content/Top/Text_State").GetComponent<Text>();

            m_CP_ToggleRegistry = transform.Find("Animator/Content/BG2/Toggles01").GetComponent<CP_ToggleRegistry>();
            m_CP_ToggleRegistry.onToggleChange = OnToggleChanged;

            m_ScoreSlider = transform.Find("Animator/Content/Top/Slider").GetComponent<Slider>();
            m_SliderNum = transform.Find("Animator/Content/Top/Text_num").GetComponent<Text>();

            m_CloseButton = transform.Find("Animator/View_TipsBgNew01/Btn_Close").GetComponent<Button>();
            m_Cooking_KnowLedgeButton = transform.Find("Animator/Content/BG3/Button2").GetComponent<Button>();
            m_IngredientButton = transform.Find("Animator/Content/BG2/GetMaterial/Toggles02/Button01").GetComponent<Button>();
            m_ShopButton = transform.Find("Animator/Content/BG2/GetMaterial/Toggles02/Button02").GetComponent<Button>();
            m_RedPoint = transform.Find("Animator/Content/BG3/Button2/Image_RedTips").gameObject;

            m_CP_PageDot = transform.Find("Animator/Content/BG1/Grid").GetComponent<CP_PageDot>();
            m_PageLeft = transform.Find("Animator/Content/BG1/Arrow_Left/Button_Left").GetComponent<Button>();
            m_PageRight = transform.Find("Animator/Content/BG1/Arrow_Right/Button_Right").GetComponent<Button>();
            m_Describe = transform.Find("Animator/Content/BG1/Text_Detail").GetComponent<Text>();
            m_PageTitleName = transform.Find("Animator/Content/BG1/Text_Title").GetComponent<Text>();

            m_FindNpcButton = transform.Find("Animator/Content/BG3/Button0").GetComponent<Button>();
            m_KitchenButton = transform.Find("Animator/Content/BG3/Button1").GetComponent<Button>();

            m_CloseButton.onClick.AddListener(OnCloseButtonClicked);
            m_Cooking_KnowLedgeButton.onClick.AddListener(CookingKnowLedgeButtonClicked);
            m_IngredientButton.onClick.AddListener(OnIngredientButtonClicked);
            m_ShopButton.onClick.AddListener(OnShopButtonClicked);

            m_PageLeft.onClick.AddListener(OnPageLeftClick);
            m_PageRight.onClick.AddListener(OnPageRightClick);

            m_FindNpcButton.onClick.AddListener(OnFindNpcButtonClick);
            m_KitchenButton.onClick.AddListener(OnKitchenButtonClick);

            ParseCookingMain();
        }

        protected override void ProcessEvents(bool toRegister)
        {
            Sys_Cooking.Instance.eventEmitter.Handle(Sys_Cooking.EEvents.OnUpdateScore, OnUpdateScore, toRegister);
        }

        protected override void OnShow()
        {
            m_RedPoint.SetActive(Sys_Cooking.Instance.HasSubmitItem(0) || Sys_Cooking.Instance.HasReward());
            UpdateInfo();
            UI_CurrencyTitle.InitUi();
        }

        private void UpdateInfo()
        {
            //currentState
            TextHelper.SetText(m_CurrentCookingState, CSVCookLv.Instance.GetConfData((uint)Sys_Cooking.Instance.curCookingLevel).name);

            //page
            m_CP_PageDot.SetMax(Sys_Cooking.Instance.maxCookingLevel);
            m_CP_PageDot.Build();

            UpdatePage();

            //slider
            RefreshScore();
        }

        private void OnUpdateScore()
        {
            //slider
            RefreshScore();
            //page
            curPage = Sys_Cooking.Instance.curCookingLevel;
            if (curPage > Sys_Cooking.Instance.maxCookingLevel)
            {
                curPage = Sys_Cooking.Instance.maxCookingLevel;
            }
            UpdatePage();
            TextHelper.SetText(m_CurrentCookingState, CSVCookLv.Instance.GetConfData((uint)Sys_Cooking.Instance.curCookingLevel).name);
        }

        private void OnToggleChanged(int current, int old)
        {
            if (current == old)
            {
                return;
            }
            if (current == 1)
            {
                RefreshCookingMain();
            }
        }

        #region Page
        private void UpdatePage()
        {
            m_CP_PageDot.SetSelected(curPage - 1);

            if (curPage == 1)
            {
                m_PageLeft.gameObject.SetActive(false);
                m_PageRight.gameObject.SetActive(true);
            }
            else if (curPage == Sys_Cooking.Instance.maxCookingLevel)
            {
                m_PageRight.gameObject.SetActive(false);
                m_PageLeft.gameObject.SetActive(true);
            }
            else
            {
                m_PageLeft.gameObject.SetActive(true);
                m_PageRight.gameObject.SetActive(true);
            }
            CSVCookLv.Data cSVCookLvData = CSVCookLv.Instance.GetConfData((uint)curPage);
            if (cSVCookLvData != null)
            {
                TextHelper.SetText(m_PageTitleName, cSVCookLvData.name);
                TextHelper.SetText(m_Describe, cSVCookLvData.desc);
            }
        }

        private void OnPageLeftClick()
        {
            curPage--;
            if (curPage < 1)
            {
                curPage = 1;
            }
            UpdatePage();
        }

        private void OnPageRightClick()
        {
            curPage++;
            if (curPage > Sys_Cooking.Instance.maxCookingLevel)
            {
                curPage = Sys_Cooking.Instance.maxCookingLevel;
            }
            UpdatePage();
        }

        #endregion

        private void RefreshScore()
        {
            if (Sys_Cooking.Instance.curCookingLevel == Sys_Cooking.Instance.maxCookingLevel)
            {
                m_ScoreSlider.value = 1;
                TextHelper.SetText(m_SliderNum, Sys_Cooking.Instance.curScore.ToString());
            }
            else
            {
                uint nextLv = (uint)Sys_Cooking.Instance.curCookingLevel + 1;
                uint needScore = CSVCookLv.Instance.GetConfData(nextLv).need_score;
                TextHelper.SetText(m_SliderNum, string.Format($"{Sys_Cooking.Instance.curScore}/{needScore}"));
                float rate = (float)Sys_Cooking.Instance.curScore / (float)needScore;
                m_ScoreSlider.value = rate;
            }
        }

        private void OnCloseButtonClicked()
        {
            UIManager.CloseUI(EUIID.UI_Cooking_Collect);
        }

        private void CookingKnowLedgeButtonClicked()
        {
            UIManager.OpenUI(EUIID.UI_Knowledge_Cooking);
        }

        private void OnIngredientButtonClicked()
        {
            UIManager.OpenUI(EUIID.UI_Knowledge_RecipeCooking);
        }

        private void OnShopButtonClicked()
        {
            MallPrama mallPrama = new MallPrama();
            mallPrama.mallId = 801;
            mallPrama.shopId = 8001;
            UIManager.OpenUI(EUIID.UI_Mall, false, mallPrama);
        }

        private void OnFindNpcButtonClick()
        {
            ActionCtrl.Instance.MoveToTargetNPCAndInteractive(1500228);
            UIManager.CloseUI(EUIID.UI_Cooking_Collect);
            if (UIManager.IsOpen(EUIID.UI_Knowledge_Main))
            {
                UIManager.CloseUI(EUIID.UI_Knowledge_Main);
            }
        }

        private void OnKitchenButtonClick()
        {
            if (Sys_OperationalActivity.Instance.CheckSpecialCardPocketKitchenIsUnlock(true))
            {
                UIManager.OpenUI(EUIID.UI_Cooking_Choose, false, m_DefultCookingFun);
            }
        }
    }
}


