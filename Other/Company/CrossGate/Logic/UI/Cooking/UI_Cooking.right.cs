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
    public partial class UI_Cooking : UIBase
    {
        private GameObject m_WaitOther;
        private Text m_WaitOtherName;
        private Button m_StartButton;
        private Button m_FireOffButton;
        private Text m_AutoCookingStartTimeText;
        private Text m_CookIdText;
        private GameObject m_Fx_ui_Cooking_fire;
        private Image m_ToolIcon;
        private Image m_ToolIcon2;
        private Transform m_MaterialParent;
        private Transform m_EndState;
        private GameObject m_CookFinish;
        private Button m_BackButton;
        private Text m_BackText;
        private Animator m_CookAnim;
        private GameObject m_Water;
        private Timer m_BackTimer;
        private int m_BackTime;
        private Timer m_CookEndShowTimer;
        private float m_CookEndShowTime;

        private GameObject m_Cuisine;
        public RectTransform pointer;
        private GameObject m_ScoreRange;
        public Image fillImg1;
        public Image fillImg2;
        private float m_MaxScore;
        public float m_BgAngle;
        public float m_CookTime;
        private float m_Min;                        //最佳火力下限
        private float m_Max;                        //最佳火力上限
        private float fillrates;
        private float m_AdjustAngle;
        private int m_AutoCookingStartTime;       //烹饪自动开始时间
        private Timer m_AutoCookingStartTimer;
        private Timer m_CookTimer;
        private float m_CurFireValue;
        private float m_CookBonus;      //食谱加成

        private void RegisterRight()
        {
            pointer = transform.Find("Animator/Right/Cuisine/fillImage1/GameObject/pointerRoot").transform as RectTransform;
            m_StartButton = transform.Find("Animator/Right/Btn_01").GetComponent<Button>();
            m_FireOffButton = transform.Find("Animator/Right/Button").GetComponent<Button>();
            m_WaitOther = transform.Find("Animator/Right/Text_Waiting").gameObject;
            m_WaitOtherName = m_WaitOther.transform.GetComponent<Text>();
            m_Cuisine = transform.Find("Animator/Right/Cuisine").gameObject;
            m_ScoreRange = transform.Find("Animator/Right/Cuisine/fillImage1/fillImage2").gameObject;
            m_EndState = transform.Find("Animator/Right/Cook/State");
            fillImg1 = transform.Find("Animator/Right/Cuisine/fillImage1").GetComponent<Image>();
            fillImg2 = transform.Find("Animator/Right/Cuisine/fillImage1/fillImage2").GetComponent<Image>();
            m_AutoCookingStartTimeText = transform.Find("Animator/Right/Btn_01/Text_01").GetComponent<Text>();
            m_CookIdText = transform.Find("Animator/cookId/cookId").GetComponent<Text>();
            m_ToolIcon = transform.Find("Animator/Right/Cook/Image").GetComponent<Image>();
            m_ToolIcon2 = transform.Find("Animator/Right/Cook/Image (2)").GetComponent<Image>();
            m_Fx_ui_Cooking_fire = transform.Find("Animator/Right/Cook/Fx_ui_Cooking_fire").gameObject;
            m_CookAnim = transform.Find("Animator/Right/Cook").GetComponent<Animator>();
            m_Water = transform.Find("Animator/Right/Cook/Image_Water").gameObject;
            m_MaterialParent = transform.Find("Animator/Right/Cook/Materials");
            m_CookFinish = transform.Find("Animator/Right/Text_End").gameObject;
            m_BackButton = transform.Find("Animator/Right/Btn_Back").GetComponent<Button>();
            m_BackText = transform.Find("Animator/Right/Btn_Back/Text_01").GetComponent<Text>();
            m_BackButton.onClick.AddListener(OnBackButtonClicked);
            m_StartButton.onClick.AddListener(OnStartButtonClicked);
            m_FireOffButton.onClick.AddListener(OnFireOffButtonClicked);

            m_CookEndShowTime = CSVCookAttr.Instance.GetConfData(10).value / 1000f;
        }

        protected override void OnClose()
        {
            m_CookTimer?.Cancel();
            m_AutoCookingStartTimer?.Cancel();
            m_CookEndShowTimer?.Cancel();
            m_BackTimer?.Cancel();
            CloseEndShow();
        }

        private void OnBackButtonClicked()
        {
            BackToParpare();
        }

        private void OnStartButtonClicked()
        {
            StartCook();
        }

        private void OnFireOffButtonClicked()
        {
            m_CookTimer.Cancel();
            EndCook();
        }

        private void UpdateRound()
        {
            b_SelfRound = Sys_Cooking.Instance.cookStages[m_Round].RoleId == Sys_Role.Instance.RoleId;
            if (b_SelfRound)
            {
                EnablePlay();
            }
            else
            {
                DisablePlay();
            }
            SetToolIcon(Sys_Cooking.Instance.cookStages[m_Round].ToolId);
            SetRecipeIcon();
            m_Water.SetActive(Sys_Cooking.Instance.cookStages[m_Round].ToolId != 4);
        }

        private void EnablePlay()
        {
            m_CookFinish.SetActive(false);
            m_BackButton.gameObject.SetActive(false);
            m_Fx_ui_Cooking_fire.SetActive(false);
            m_StartButton.gameObject.SetActive(true);
            m_FireOffButton.gameObject.SetActive(false);
            m_WaitOther.SetActive(false);
            m_CookTime = Sys_Cooking.Instance.GetCookTime(Sys_Cooking.Instance.cookStages[m_Round].ToolId);

            TextHelper.SetText(m_AutoCookingStartTimeText, LanguageHelper.GetTextContent(1003034, m_AutoCookingStartTime.ToString()));
            m_AutoCookingStartTimer?.Cancel();
            m_AutoCookingStartTimer = Timer.Register(m_AutoCookingStartTime, StartCook, OnAutoCookingStartUpdateCallback);

            m_Cuisine.SetActive(true);
            SetCuisine();
        }

        private void DisablePlay()
        {
            m_CookFinish.SetActive(false);
            m_BackButton.gameObject.SetActive(false);
            m_Fx_ui_Cooking_fire.SetActive(false);
            m_Cuisine.SetActive(false);
            m_StartButton.gameObject.SetActive(false);
            m_FireOffButton.gameObject.SetActive(false);
            m_WaitOther.SetActive(true);
            TeamMem teamMem = Sys_Team.Instance.getTeamMem(Sys_Cooking.Instance.cookStages[m_Round].RoleId);
            TextHelper.SetText(m_WaitOtherName, LanguageHelper.GetTextContent(1003072, teamMem.Name.ToStringUtf8()));
        }

        private void OnAutoCookingStartUpdateCallback(float dt)
        {
            int remainTime = 0;
            remainTime = m_AutoCookingStartTime - (int)dt;
            TextHelper.SetText(m_AutoCookingStartTimeText, LanguageHelper.GetTextContent(1003034, remainTime.ToString()));
        }

        private void StartCook()
        {
            m_CookAnim.enabled = true;
            m_Fx_ui_Cooking_fire.SetActive(true);
            m_CurFireValue = 0;
            m_AutoCookingStartTimer?.Cancel();
            m_StartButton.gameObject.SetActive(false);
            m_FireOffButton.gameObject.SetActive(true);
            m_CookTimer?.Cancel();
            m_CookTimer = Timer.Register(m_CookTime, EndCook, OnCookingUpdate);
            pointer.eulerAngles = new Vector3(0, 0, GetAngle(90 - m_AdjustAngle));
            Sys_Cooking.Instance.CookFireOnReq();
        }

        private void EndCook()
        {
            m_CookAnim.enabled = false;
            m_Fx_ui_Cooking_fire.SetActive(false);
            Sys_Cooking.Instance.CookFireOffReq((uint)Mathf.CeilToInt(m_CurFireValue));
        }

        private void FinishCook()
        {
            m_Fx_ui_Cooking_fire.SetActive(false);
            m_Cuisine.SetActive(false);
            m_StartButton.gameObject.SetActive(false);
            m_FireOffButton.gameObject.SetActive(false);
            m_WaitOther.SetActive(false);
            m_CookFinish.SetActive(true);
            if (Sys_Cooking.Instance.bIsCookingCaptain)
            {
                m_BackButton.gameObject.SetActive(true);
                m_BackTimer?.Cancel();
                m_BackTimer = Timer.Register(m_BackTime, BackToParpare, OnBackTimeUpdate);
            }
        }

        private void OnBackTimeUpdate(float dt)
        {
            int remainTime = 0;
            remainTime = m_BackTime - (int)dt;
            TextHelper.SetText(m_BackText, LanguageHelper.GetTextContent(1003074, remainTime.ToString()));
        }

        private void BackToParpare()
        {
            Sys_Cooking.Instance.CookReturnPrepareReq();
        }

        private void OnCookingUpdate(float dt)
        {
            pointer.eulerAngles = new Vector3(0, 0, GetAngle(90 - m_AdjustAngle - (dt / m_CookTime) * m_BgAngle));
            m_CurFireValue = (dt / m_CookTime) * m_MaxScore;
        }


        private void SetCuisine()
        {
            m_AdjustAngle = (180 - m_BgAngle) / 2;
            fillImg1.transform.eulerAngles = new Vector3(0, 0, (GetAngle(-m_AdjustAngle)));
            pointer.eulerAngles = new Vector3(0, 0, GetAngle(90 - m_AdjustAngle));
            fillrates = m_BgAngle / 360f;
            fillImg1.fillAmount = fillrates;
            if (b_FreeCook)
            {
                fillImg2.gameObject.SetActive(false);
            }
            else
            {
                CSVCook.Data cSVCookData = CSVCook.Instance.GetConfData(m_CookId);
                if (m_Round == 0)
                {
                    m_Min = cSVCookData.fire_value1[0];
                    m_Max = cSVCookData.fire_value1[1];
                }
                else if (m_Round == 1)
                {
                    m_Min = cSVCookData.fire_value2[0];
                    m_Max = cSVCookData.fire_value2[1];
                }
                else
                {
                    m_Min = cSVCookData.fire_value3[0];
                    m_Max = cSVCookData.fire_value3[1];
                }
                m_Min -= m_CookBonus;
                m_Max += m_CookBonus;
                fillImg2.gameObject.SetActive(true);
                float rate = m_Min / m_MaxScore;
                float angle = rate * m_BgAngle;
                fillImg2.rectTransform.eulerAngles = new Vector3(0, 0, GetAngle(-m_AdjustAngle - angle));
                float diff = m_Max - m_Min;
                fillImg2.fillAmount = (diff / m_MaxScore) * fillrates;
            }
        }

        private float GetAngle(float angle)
        {
            if (angle < 0)
                angle += 360;
            return angle;
        }

        private void SetToolIcon(uint toolId)
        {
            ImageHelper.SetIcon(m_ToolIcon, Sys_Cooking.Instance.GetToolIcon(toolId), true);
            ImageHelper.SetIcon(m_ToolIcon2, Sys_Cooking.Instance.GetToolIcon2(toolId), true);
        }

        private void SetRecipeIcon()
        {
            List<uint> itemIds = new List<uint>();
            for (int i = 0; i < Sys_Cooking.Instance.cookStages[m_Round].Foods.Count; i++)
            {
                for (int j = 0; j < Sys_Cooking.Instance.cookStages[m_Round].Foods[i].Count; j++)
                {
                    itemIds.Add(Sys_Cooking.Instance.cookStages[m_Round].Foods[i].FoodId);
                }
            }
            for (int i = itemIds.Count - 1; i >= 0; --i)
            {
                CSVFoodIsShow.Data cSVFoodIsShowData = CSVFoodIsShow.Instance.GetConfData(itemIds[i]);
                if (null == cSVFoodIsShowData)
                {
                    itemIds.RemoveAt(i);
                    continue;
                }
                if (!cSVFoodIsShowData.is_show)
                {
                    itemIds.RemoveAt(i);
                }
            }
            for (int i = 0; i < m_MaterialParent.childCount; i++)
            {
                GameObject gameObject = m_MaterialParent.GetChild(i).gameObject;
                if (i < itemIds.Count)
                {
                    Image icon = gameObject.GetComponent<Image>();
                    CSVItem.Data cSVItemData = CSVItem.Instance.GetConfData(itemIds[i]);
                    if (null != cSVItemData)
                    {
                        gameObject.SetActive(true);
                        ImageHelper.SetIcon(icon, cSVItemData.icon_id);
                    }
                    else
                    {
                        gameObject.SetActive(false);
                    }
                }
                else
                {
                    gameObject.SetActive(false);
                }
            }
        }

        private void CookEndPlay(uint cookId, uint score)
        {
            if (cookId == 0)
            {
                EndShow(1);
            }
            else
            {
                CSVCook.Data cSVCookData = CSVCook.Instance.GetConfData(cookId);
                if (cSVCookData != null)
                {
                    if (score < cSVCookData.need_score[0])
                    {
                        EndShow(0);
                    }
                    else if (score >= cSVCookData.need_score[0] && score < cSVCookData.need_score[1])
                    {
                        EndShow(2);
                    }
                    else
                    {
                        EndShow(3);
                    }
                }
            }
            m_CookEndShowTimer?.Cancel();
            m_CookEndShowTimer = Timer.Register(m_CookEndShowTime, CloseEndShow);
        }

        private void EndShow(int index)
        {
            for (int i = 0; i < m_EndState.childCount; i++)
            {
                m_EndState.GetChild(i).gameObject.SetActive(index == i);
            }
        }

        private void CloseEndShow()
        {
            for (int i = 0; i < m_EndState.childCount; i++)
            {
                m_EndState.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
}


