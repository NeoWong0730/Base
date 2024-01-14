using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Lib.Core;
using Table;
using UnityEngine.EventSystems;
using Net;
using static Packet.CmdCookPrepareConfirmNtf.Types;
using Packet;

namespace Logic
{
    public partial class UI_Cooking_Multiple : UIBase
    {
        private Transform m_StageRoot;
        private List<Stage> m_Stages = new List<Stage>();
        private Button m_CloseButton;
        private Button m_CookButton;


        private void RegisterRight()
        {
            m_StageRoot = transform.Find("Animator/Right/Stages");
            for (int i = 0; i < m_StageRoot.childCount; i++)
            {
                GameObject gameObject = m_StageRoot.GetChild(i).gameObject;
                Stage stage = new Stage();
                stage.SetIndex(i + 1);
                stage.BindGameObject(gameObject);
                m_Stages.Add(stage);
            }
        }

        /// <summary>
        /// 设置队员
        /// </summary>
        private void SetMember()
        {
            for (int i = 0; i < m_Stages.Count; i++)
            {
                Stage stage = m_Stages[i];
                stage.SetMember(Sys_Cooking.Instance.roles[i]);
            }
        }

        /// <summary>
        /// 设置食材
        /// </summary>
        private void SetIngredient(uint cookId)
        {
            if (!Sys_Cooking.Instance.bIsCookingCaptain)
            {
                if (cookId == 0)
                {
                    m_CookItem.SetActive(false);
                    TextHelper.SetText(m_Text_Stage, 1003080);
                }
                else
                {
                    m_CookItem.SetActive(true);
                    TextHelper.SetText(m_CookName, CSVCook.Instance.GetConfData(cookId).name);
                    ImageHelper.SetIcon(m_CookIcon, CSVCook.Instance.GetConfData(cookId).icon);
                    TextHelper.SetText(m_Text_Stage, 1003035);
                }
            }
            for (int i = 0; i < m_Stages.Count; i++)
            {
                Stage stage = m_Stages[i];
                stage.SetIngredient(cookId);
            }
        }

        /// <summary>
        /// 交换位置
        /// </summary>
        private void OnExchangePosition()
        {
            SetMember();
            for (int i = 0; i < m_Stages.Count; i++)
            {
                Stage stage = m_Stages[i];
                stage.Refresh();
            }
        }

        private void OnCookButtonClicked()
        {
            //判断食材够不够
            for (int i = 0; i < m_Stages.Count; i++)
            {
                if (!m_Stages[i].stageEnough)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(5912, m_Stages[i].GetTeamName()));
                    return;
                }
            }
            //判断厨具
            if (!Sys_Cooking.Instance.supportKitchens.Contains(m_CurSelectCooking.cSVCookData.tool1))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(5913,
                    Sys_Cooking.Instance.GetToolName(m_CurSelectCooking.cSVCookData.tool1)));
                return;
            }
            if (!Sys_Cooking.Instance.supportKitchens.Contains(m_CurSelectCooking.cSVCookData.tool2))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(5913,
                    Sys_Cooking.Instance.GetToolName(m_CurSelectCooking.cSVCookData.tool2)));
                return;
            }
            if (!Sys_Cooking.Instance.supportKitchens.Contains(m_CurSelectCooking.cSVCookData.tool3))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(5913,
                    Sys_Cooking.Instance.GetToolName(m_CurSelectCooking.cSVCookData.tool3)));
                return;
            }
            Sys_Cooking.Instance.CookCookReq(3, false, m_CurSelectCooking.id, null);
        }

        public class Stage
        {
            public uint stageIndex;
            public bool stageEnough = true;
            private ulong m_RoleId;
            private bool b_SelfStage;           //是否是自己的阶段
            public GameObject gameObject;
            private Text m_Stage;
            private Text m_StageBg;
            private Text m_PlayerName;
            private Text m_Tool;
            private Button m_Exchange;
            private Transform m_ItemParent;
            private uint m_CookId;

            public void BindGameObject(GameObject @object)
            {
                gameObject = @object;

                m_Stage = gameObject.transform.Find("Text_Stage").GetComponent<Text>();
                m_StageBg = gameObject.transform.Find("Text_Stage/Text_Stage (1)").GetComponent<Text>();
                m_PlayerName = gameObject.transform.Find("Text_name").GetComponent<Text>();
                m_Tool = gameObject.transform.Find("Text_Tool").GetComponent<Text>();
                m_Exchange = gameObject.transform.Find("Button_Change").GetComponent<Button>();
                m_ItemParent = gameObject.transform.Find("Grid");
                m_Exchange.onClick.AddListener(OnExchangeButtonClicked);
                SetFixStage();
            }

            public void SetIndex(int index)
            {
                this.stageIndex = (uint)index;
            }

            private void SetFixStage()
            {
                if (stageIndex == 1)
                {
                    TextHelper.SetText(m_Stage, 1003023);
                    TextHelper.SetText(m_StageBg, 1003023);
                }
                else if (stageIndex == 2)
                {
                    TextHelper.SetText(m_Stage, 1003024);
                    TextHelper.SetText(m_StageBg, 1003024);
                }
                else if (stageIndex == 3)
                {
                    TextHelper.SetText(m_Stage, 1003025);
                    TextHelper.SetText(m_StageBg, 1003025);
                }
            }

            public void SetMember(ulong roleId)
            {
                this.m_RoleId = roleId;
                this.b_SelfStage = m_RoleId == Sys_Role.Instance.RoleId;
                TextHelper.SetText(m_PlayerName, GetTeamName());
                m_Exchange.gameObject.SetActive(!b_SelfStage);
            }

            public string GetTeamName()
            {
                TeamMem teamMem = Sys_Team.Instance.getTeamMem(m_RoleId);
                if (null != teamMem)
                {
                    return teamMem.Name.ToStringUtf8();
                }
                return string.Empty;
            }

            public void SetIngredient(uint cookId)
            {
                m_CookId = cookId;
                Refresh();
            }

            public void Refresh()
            {
                stageEnough = true;
                if (m_CookId == 0)
                {
                    stageEnough = false;
                    for (int i = 0; i < m_ItemParent.childCount; i++)
                    {
                        GameObject gameObject = m_ItemParent.GetChild(i).gameObject;
                        PropItem propItem = new PropItem();
                        propItem.BindGameObject(gameObject);
                        propItem.SetEmpty();
                    }
                    //m_Tool.gameObject.SetActive(false);
                }
                else
                {
                    CSVCook.Data cSVCookData = CSVCook.Instance.GetConfData(m_CookId);
                    List<List<uint>> foods = new List<List<uint>>();
                    m_Tool.gameObject.SetActive(true);
                    if (stageIndex == 1)
                    {
                        TextHelper.SetText(m_Tool, Sys_Cooking.Instance.GetToolName(cSVCookData.tool1));
                        foods = cSVCookData.food1;
                    }
                    else if (stageIndex == 2)
                    {
                        TextHelper.SetText(m_Tool, Sys_Cooking.Instance.GetToolName(cSVCookData.tool2));
                        foods = cSVCookData.food2;
                    }
                    else if (stageIndex == 3)
                    {
                        TextHelper.SetText(m_Tool, Sys_Cooking.Instance.GetToolName(cSVCookData.tool3));
                        foods = cSVCookData.food3;
                    }
                    CookStage cookStage = Sys_Cooking.Instance.GetCookStage(m_RoleId);
                    for (int i = 0; i < m_ItemParent.childCount; i++)
                    {
                        GameObject gameObject = m_ItemParent.GetChild(i).gameObject;
                        if (i < foods.Count)
                        {
                            gameObject.SetActive(true);
                            uint itemId = foods[i][0];
                            long needCount = foods[i][1];
                            CSVItem.Data item = CSVItem.Instance.GetConfData(itemId);
                            long otherCount = 0;
                            for (int j = 0; j < cookStage.Foods.Count; j++)
                            {
                                if (itemId == cookStage.Foods[j].FoodId)
                                {
                                    otherCount = cookStage.Foods[j].Count;
                                    break;
                                }
                            }
                            if (otherCount < needCount)
                            {
                                stageEnough = false;
                            }
                            PropItem propItem = new PropItem();
                            propItem.BindGameObject(gameObject);
                            PropIconLoader.ShowItemData showItem = new PropIconLoader.ShowItemData
                                (_id: itemId,
                                _count: needCount,
                                _bUseQuailty: true,
                                _bBind: false,
                                _bNew: false,
                                _bUnLock: false,
                                _bSelected: false,
                                _bShowCount: true,
                                _bShowBagCount: true,
                                _bUseClick: b_SelfStage,
                                _onClick: null,
                                _bshowBtnNo: false);
                            showItem.SetQuality(item.quality);
                            showItem.SetOtherCount(otherCount);
                            propItem.SetData(new MessageBoxEvt(EUIID.UI_Cooking_Multiple, showItem));
                        }
                        else
                        {
                            gameObject.SetActive(false);
                            //PropItem propItem = new PropItem();
                            //propItem.BindGameObject(gameObject);
                            //propItem.SetEmpty();
                        }
                    }
                }
            }

            private void OnExchangeButtonClicked()
            {
                Sys_Cooking.Instance.CookExchangeReq(stageIndex - 1);
            }
        }
    }
}


