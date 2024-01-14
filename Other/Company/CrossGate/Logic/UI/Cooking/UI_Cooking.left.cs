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
        private uint m_CookId;
        private uint m_CookType;
        private bool b_FreeCook;        //是否是食谱烹饪(true：不是食谱烹饪)
        private Button m_CloseButton;
        private Transform m_StageParent;
        private List<Stage> m_Stages = new List<Stage>();
        private int m_Round;
        private bool b_SelfRound = false;

        protected override void OnOpen(object arg)
        {
            Tuple<uint, bool, uint> tuple = arg as Tuple<uint, bool, uint>;
            m_CookId = tuple.Item1;
            b_FreeCook = tuple.Item2;
            m_CookType = tuple.Item3;
        }

        protected override void OnInit()
        {
            m_Round = 0;
            m_BgAngle = 46;       //美术那边给图是46度角
            m_MaxScore = 10000;
            m_CookBonus = CSVCookAttr.Instance.GetConfData(5).value;
            m_AutoCookingStartTime = (int)CSVCookAttr.Instance.GetConfData(8).value;
            m_BackTime = (int)CSVCookAttr.Instance.GetConfData(11).value;
        }

        protected override void ProcessEvents(bool toRegister)
        {
            Sys_Cooking.Instance.eventEmitter.Handle<uint, uint>(Sys_Cooking.EEvents.OnFireOffNtf, OnFireOffNtf, toRegister);
            Sys_Cooking.Instance.eventEmitter.Handle<uint, uint>(Sys_Cooking.EEvents.OnCookEndPlay, CookEndPlay, toRegister);
            Sys_Net.Instance.eventEmitter.Handle<bool>(Sys_Net.EEvents.OnReconnectResult, OnReconnectResult, toRegister);
        }

        private void OnReconnectResult(bool res)
        {
            if (res)
            {
               Sys_Cooking.Instance.CancelCooking();
            }
        }

        protected override void OnLoaded()
        {
            RegisterLeft();
            RegisterRight();
        }

        private void RegisterLeft()
        {
            m_CloseButton = transform.Find("Animator/View_Title/Btn_Close").GetComponent<Button>();
            m_StageParent = transform.Find("Animator/Left/Mask/Scroll View/Viewport/Content");
            m_CloseButton.onClick.AddListener(OnCloseButtonClicked);
            for (int i = 0; i < m_StageParent.childCount; i++)
            {
                GameObject gameObject = m_StageParent.GetChild(i).gameObject;
                Stage stage = new Stage();
                stage.BindGameObject(gameObject);
                stage.SetIndex(i + 1);
                m_Stages.Add(stage);
            }
        }

        protected override void OnShow()
        {
            UpdateLeftInfo();
            UpdateRound();
            m_CookIdText.text = m_CookId.ToString();
        }

        private void UpdateLeftInfo()
        {
            int showIndex = (int)m_CookType - 1;
            for (int i = 0; i < m_StageParent.childCount; i++)
            {
                m_Stages[i].SetModel(m_CookType != 3);
                if (i <= showIndex)
                {
                    m_StageParent.GetChild(i).gameObject.SetActive(true);
                    m_Stages[i].SetStage();
                }
                else
                {
                    m_StageParent.GetChild(i).gameObject.SetActive(false);
                }
            }
            SetCookingStagePerform();
        }


        private void OnFireOffNtf(uint cookIndex, uint score)
        {
            for (int i = 0; i < m_Stages.Count; i++)
            {
                if (i == cookIndex)
                {
                    m_Stages[i].SetScore(score);
                }
            }
            if (cookIndex == m_CookType - 1)
            {
                FinishCook();
                return;
            }
            m_Round = (int)cookIndex + 1;
            SetCookingStagePerform();
            UpdateRound();
        }

        private void SetCookingStagePerform()
        {
            for (int i = 0; i < m_Stages.Count; i++)
            {
                m_Stages[i].SetStagePerforming(i == m_Round);
            }
        }

        private void OnCloseButtonClicked()
        {
            Sys_Cooking.Instance.CookCancelReq(1);
            UIManager.CloseUI(EUIID.UI_Cooking);
        }

        public class Stage
        {
            public GameObject gameObject;
            private GameObject m_StagePerforming;
            private Text m_Score;
            private Text m_Stage;
            private Text m_PlayerName;
            private Text m_Tool;
            private Transform m_ItemParent;
            private GameObject m_Finish;
            public uint stageIndex;
            private ulong m_RoleId;
            private bool b_SingleModle;


            public void BindGameObject(GameObject @object)
            {
                gameObject = @object;

                m_StagePerforming = gameObject.transform.Find("Image_Light").gameObject;
                m_Finish = gameObject.transform.Find("Image_finish").gameObject;
                m_Stage = gameObject.transform.Find("Text_Stage").GetComponent<Text>();
                m_Score = gameObject.transform.Find("Text_Score").GetComponent<Text>();
                m_PlayerName = gameObject.transform.Find("Text_name").GetComponent<Text>();
                m_Tool = gameObject.transform.Find("Text_fire").GetComponent<Text>();
                m_ItemParent = gameObject.transform.Find("Grid");
            }

            public void SetIndex(int index)
            {
                this.stageIndex = (uint)index;
            }

            public void SetModel(bool single)
            {
                this.b_SingleModle = single;
            }

            public void SetStagePerforming(bool perform)
            {
                m_StagePerforming.SetActive(perform);
            }

            public void SetScore(uint score)
            {
                if (!b_SingleModle)
                {
                    TextHelper.SetText(m_Score, score.ToString());
                }
                m_Finish.SetActive(true);
            }

            private void SetFixStage()
            {
                if (stageIndex == 1)
                {
                    TextHelper.SetText(m_Stage, 1003023);
                }
                else if (stageIndex == 2)
                {
                    TextHelper.SetText(m_Stage, 1003024);
                }
                else if (stageIndex == 3)
                {
                    TextHelper.SetText(m_Stage, 1003025);
                }
                m_Finish.SetActive(false);
            }

            public void SetStage()
            {
                SetFixStage();
                TextHelper.SetText(m_Score, string.Empty);
                CookStage cookStage = Sys_Cooking.Instance.cookStages[(int)stageIndex - 1];
                TextHelper.SetText(m_Tool, Sys_Cooking.Instance.GetToolName(cookStage.ToolId));
                if (!b_SingleModle)
                {
                    this.m_RoleId = cookStage.RoleId;
                    TeamMem teamMem = Sys_Team.Instance.getTeamMem(m_RoleId);
                    TextHelper.SetText(m_PlayerName, teamMem.Name.ToStringUtf8());
                }
                else
                {
                    TextHelper.SetText(m_PlayerName, string.Empty);
                }
                for (int i = 0; i < m_ItemParent.childCount; i++)
                {
                    GameObject gameObject = m_ItemParent.GetChild(i).gameObject;
                    if (i < cookStage.Foods.Count)
                    {
                        gameObject.SetActive(true);
                        uint itemId = cookStage.Foods[i].FoodId;
                        long otherCount = cookStage.Foods[i].Count;
                        CSVItem.Data item = CSVItem.Instance.GetConfData(itemId);

                        PropItem propItem = new PropItem();
                        propItem.BindGameObject(gameObject);
                        PropIconLoader.ShowItemData showItem = new PropIconLoader.ShowItemData
                            (_id: itemId,
                            _count: otherCount,
                            _bUseQuailty: true,
                            _bBind: false,
                            _bNew: false,
                            _bUnLock: false,
                            _bSelected: false,
                            _bShowCount: true,
                            _bShowBagCount: false,
                            _bUseClick: false);
                        showItem.SetQuality(item.quality);
                        propItem.SetData(new MessageBoxEvt(EUIID.UI_Cooking_Multiple, showItem));
                    }
                    else
                    {
                        gameObject.SetActive(false);
                    }
                }
            }
        }
    }
}


