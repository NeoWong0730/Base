using Logic.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Table;
using UnityEngine.UI;
using UnityEngine;
using System;
using Lib.Core;
using Framework;
using Packet;
using System.Text;

namespace Logic
{
    public partial class UI_Activity_Exchange : UIBase
    {
        private GameObject m_QtNo;

        private InfinityGrid m_InfinityGrid_QtTask;

        private Dictionary<GameObject, QuestEntry> m_Qentrys = new Dictionary<GameObject, QuestEntry>();

        private void OnCreateCell_Qt(InfinityGridCell cell)
        {
            QuestEntry entry = new QuestEntry();
            entry.BindGameObject(cell.mRootTransform.gameObject);
            cell.BindUserData(entry);
            m_Qentrys.Add(cell.mRootTransform.gameObject, entry);
        }

        private void OnCellChange_Qt(InfinityGridCell cell, int index)
        {
            QuestEntry entry = cell.mUserData as QuestEntry;
            entry.SetData(Sys_ActivityQuest.Instance.questDatas[index]);
        }

        private void RefreshQuestNode()
        {
            if (!Sys_ActivityQuest.Instance.isActivity)
            {
                m_QtNo.SetActive(true);
                m_InfinityGrid_QtTask.CellCount = 0;
                m_InfinityGrid_QtTask.ForceRefreshActiveCell();
            }
            else
            {
                m_QtNo.SetActive(false);
                UpdateInfo();
            }
        }

        private void OnRefreshTaskEntry(uint taskId)
        {
            foreach (var item in m_Qentrys)
            {
                GameObject go = item.Key;

                if (!go.activeSelf)
                    continue;

                QuestEntry questEntry = item.Value;

                if (questEntry.id == taskId)
                {
                    item.Value.RefreshEntry();
                }
            }
            RefreshRedPoint();
        }

        private void OnRefreshTaskEntryAll()
        {
            m_InfinityGrid_QtTask.CellCount = Sys_ActivityQuest.Instance.questDatas.Count;
            m_InfinityGrid_QtTask.ForceRefreshActiveCell();
            RefreshRedPoint();
        }

        public class QuestEntry
        {
            private uint m_Id;

            public uint id
            {
                get { return m_Id; }
            }

            private Sys_ActivityQuest.QuestData m_QuestData;

            private GameObject m_Go;

            private CSVActivityQuestGroup.Data m_Data;

            private Button m_GetButton;

            private Button m_GoToButton;

            private GameObject m_RedPoint;

            private Text m_GetText;

            private Slider m_Slider;

            private Text m_Text_Percent;

            private Text m_Des;

            private Button m_GiftButton;

            private Transform m_PropTemplete;

            private Transform m_ItemParent;

            private Dictionary<GameObject, PropItem> m_PropItems = new Dictionary<GameObject, PropItem>();

            private int m_ShowCount = 3;

            private List<ItemIdCount> m_ItemIdCounts = new List<ItemIdCount>();


            public void BindGameObject(GameObject gameObject)
            {
                m_Go = gameObject;

                m_GetButton = m_Go.transform.Find("Btn_01").GetComponent<Button>();
                m_GetText = m_Go.transform.Find("Btn_01/Text_01").GetComponent<Text>();
                m_GoToButton = m_Go.transform.Find("Btn_02").GetComponent<Button>();
                m_GiftButton = m_Go.transform.Find("Reward/Viewport/Button").GetComponent<Button>();
                m_PropTemplete = m_Go.transform.Find("Reward/Viewport/PropItem");
                m_ItemParent = m_Go.transform.Find("Reward/Viewport");
                m_RedPoint = m_Go.transform.Find("Btn_01/Dot").gameObject;
                m_Slider = m_Go.transform.Find("Slider").GetComponent<Slider>();
                m_Des = m_Go.transform.Find("Text_Task").GetComponent<Text>();
                m_Text_Percent = m_Go.transform.Find("Slider/Text_Percent").GetComponent<Text>();

                m_GetButton.onClick.AddListener(OnGetButtonClicked);
                m_GoToButton.onClick.AddListener(OnGoToButtonClicked);
                m_GiftButton.onClick.AddListener(OnGiftButtonClicked);
            }

            public void SetData(Sys_ActivityQuest.QuestData questData)
            {
                m_Id = questData.id;
                m_Data = CSVActivityQuestGroup.Instance.GetConfData(m_Id);
                m_QuestData = Sys_ActivityQuest.Instance.TryGetQuestByMissionId(m_Id);
                RefreshEntry();
            }


            public void RefreshEntry()
            {
                m_ItemIdCounts = CSVDrop.Instance.GetDropItem(m_Data.Drop_Id);

                int count = m_ItemIdCounts.Count;

                if (count < 4)
                {
                    FrameworkTool.CreateChildListAndFixedLast(m_ItemParent, m_GiftButton.transform, count);

                    m_GiftButton.gameObject.SetActive(false);

                    for (int i = 0; i < count; i++)
                    {
                        GameObject go = m_ItemParent.GetChild(i).gameObject;

                        if (!m_PropItems.TryGetValue(go, out PropItem propItem))
                        {
                            propItem = new PropItem();

                            m_PropItems.Add(go, propItem);
                        }

                        propItem.BindGameObject(go);

                        SetPropItem(propItem, m_ItemIdCounts[i]);
                    }
                }
                else
                {
                    FrameworkTool.CreateChildListAndFixedLast(m_ItemParent, m_GiftButton.transform, m_ShowCount);

                    m_GiftButton.gameObject.SetActive(true);

                    for (int i = 0; i < m_ShowCount; i++)
                    {
                        GameObject go = m_ItemParent.GetChild(i).gameObject;

                        if (!m_PropItems.TryGetValue(go, out PropItem propItem))
                        {
                            propItem = new PropItem();

                            m_PropItems.Add(go, propItem);
                        }

                        propItem.BindGameObject(go);

                        SetPropItem(propItem, m_ItemIdCounts[i]);
                    }

                    m_ItemIdCounts.RemoveRange(0, m_ShowCount);
                }

                string content = string.Empty;

                if (m_Data.QuestType == 1)
                {
                    content = LanguageHelper.GetTextContent(m_Data.Quest_Info);
                }
                else if (m_Data.QuestType == 2) //重复
                {
                    content = LanguageHelper.GetTextContent(2025725, LanguageHelper.GetTextContent(m_Data.Quest_Info));
                }
                else
                {
                    content = LanguageHelper.GetTextContent(2025724, LanguageHelper.GetTextContent(m_Data.Quest_Info));
                }

                TextHelper.SetText(m_Des, content);

                if (m_QuestData.state == 0)
                {
                    m_GetButton.gameObject.SetActive(false);

                    m_GoToButton.gameObject.SetActive(true);
                }
                else if (m_QuestData.state == 1) //已经完成还没领取
                {
                    m_GetButton.gameObject.SetActive(true);
                    ButtonHelper.Enable(m_GetButton, true);

                    m_GoToButton.gameObject.SetActive(false);

                    TextHelper.SetText(m_GetText, 2025718); //领取
                }
                else if (m_QuestData.state == 2)
                {
                    m_GetButton.gameObject.SetActive(true);

                    m_GoToButton.gameObject.SetActive(false);

                    TextHelper.SetText(m_GetText, 2025719); //已领取

                    ButtonHelper.Enable(m_GetButton, false);
                }
                else
                {
                    uint remain = m_QuestData.state - 3;

                    if (remain == 0)
                    {
                        m_GetButton.gameObject.SetActive(false);

                        m_GoToButton.gameObject.SetActive(true);
                    }
                    else
                    {
                        m_GoToButton.gameObject.SetActive(false);

                        m_GetButton.gameObject.SetActive(true);
                        ButtonHelper.Enable(m_GetButton, true);

                        TextHelper.SetText(m_GetText, 2025721, remain.ToString()); //领取 {0}
                    }
                }


                m_RedPoint.SetActive(m_QuestData.canGet);

                m_Slider.value = (float)m_QuestData.completeNum / (float)m_Data.ReachTypeAchievement[m_Data.ReachTypeAchievement.Count - 1];
                TextHelper.SetText(m_Text_Percent, string.Format("{0}/{1}", m_QuestData.completeNum, m_Data.ReachTypeAchievement[m_Data.ReachTypeAchievement.Count - 1]));
            }

            public void CreateChildListAndFixedLast(Transform rParentGo, Transform rLastChild, int needchildCount)
            {
                int curchildCount = rParentGo.childCount - 1;
                if (curchildCount < 1)
                {
                    DebugUtil.LogErrorFormat($"{rParentGo.name} 没有模板子物体，无法创建子物体列表");
                    return;
                }

                GameObject rTemplateGo = rParentGo.GetChild(0).gameObject;
                int needInstantiateCount = needchildCount - curchildCount;
                if (needInstantiateCount <= 0)
                {
                    for (int i = 0; i < curchildCount; i++)
                    {
                        if (i < needchildCount)
                        {
                            rParentGo.GetChild(i).gameObject.SetActive(true);
                        }
                        else
                        {
                            rParentGo.GetChild(i).gameObject.SetActive(false);
                        }
                    }
                }
                else
                {
                    for (int i = 0; i < curchildCount; i++)
                    {
                        rParentGo.GetChild(i).gameObject.SetActive(true);
                    }

                    for (int i = 0; i < needInstantiateCount; i++)
                    {
                        CreateGameObject(rTemplateGo, rParentGo.gameObject);
                    }
                }

                rLastChild.SetAsLastSibling();
                FrameworkTool.ForceRebuildLayout(rParentGo.gameObject);
            }

            public GameObject CreateGameObject(GameObject rTemplateGo, GameObject rParentGo)
            {
                GameObject rGo = GameObject.Instantiate(rTemplateGo);
                rGo.transform.SetParent(rParentGo.transform);
                //rGo.transform.parent = rParentGo.transform;

                rGo.name = rTemplateGo.name;
                rGo.transform.localPosition = Vector3.zero;
                rGo.transform.localRotation = Quaternion.identity;
                rGo.transform.localScale = rTemplateGo.transform.localScale;

                return rGo;
            }

            private void SetPropItem(PropItem propItem, ItemIdCount itemIdCount)
            {
                PropIconLoader.ShowItemData showItem = new PropIconLoader.ShowItemData
                (_id: itemIdCount.id,
                    _count: itemIdCount.count,
                    _bUseQuailty: true,
                    _bBind: false,
                    _bNew: false,
                    _bUnLock: false,
                    _bSelected: false,
                    _bShowCount: true,
                    _bShowBagCount: false,
                    _bUseClick: true,
                    _onClick: null,
                    _bshowBtnNo: false);
                propItem.SetData(new MessageBoxEvt(EUIID.UI_LuckyDraw_Result, showItem));
            }

            private void OnGiftButtonClicked()
            {
                RewardPanelParam _param = new RewardPanelParam();
                Vector3 _vec = m_GiftButton.gameObject.GetComponent<RectTransform>().position;
                Vector3 _screenVec = CameraManager.mUICamera.WorldToScreenPoint(_vec);
                _param.propList = m_ItemIdCounts;
                _param.Pos = _screenVec;
                UIManager.OpenUI(EUIID.UI_RewardPanel, false, _param);
            }

            private void OnGetButtonClicked()
            {
                if (!Sys_OperationalActivity.Instance.CheckActivitySwitchIsOpen(212))
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2028102));
                    return;
                }

                Sys_ActivityQuest.Instance.ActivityMissonAwardTakeReq(m_Data.id);
            }

            private void OnGoToButtonClicked()
            {
                TaskToGo(m_Data);
            }

            public void TaskToGo(CSVActivityQuestGroup.Data data)
            {
                if (data != null)
                {
                    UIManager.CloseUI(EUIID.UI_Activity_Summer);
                    UIManager.CloseUI(EUIID.UI_Activity_Exchange);

                    if (data.Function_Id != 0)
                    {
                        if (!Sys_FunctionOpen.Instance.IsOpen(data.Function_Id, true))
                        {
                            return;
                        }
                    }

                    if (data.FamilyIn == 1)
                    {
                        if (!Sys_Family.Instance.familyData.isInFamily)
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2025734));
                            return;
                        }
                    }

                    var type = data.Tel_type;
                    List<uint> jumpPrama = data.Skip_Id;
                    if (jumpPrama[0] == 311) //EUIID.UI_Family
                    {
                        //家族跳转特殊处理
                        bool isInFamily = Sys_Family.Instance.familyData.isInFamily;
                        if (!isInFamily)
                        {
                            UIManager.CloseUI(EUIID.UI_SevenDaysTarget);
                            UIManager.OpenUI(EUIID.UI_ApplyFamily);
                            return;
                        }
                    }

                    //先关界面再跳转(有些界面返回主界面会被hide)
                    switch (type)
                    {
                        //1: 跳转界面 Id;  2:跳转界面Iid和子界面Id;  3: 日常活动 界面 4:只有提示，没有前往。5：前往npc
                        case 1:
                            {
                                //EUIID.UI_Society
                                if (jumpPrama[0] == 94)
                                {
                                    //好友跳转特殊处理
                                    UIManager.OpenUI((int)jumpPrama[0]);
                                }
                                else if (jumpPrama[0] == 730) //时装抽奖 特殊处理
                                {
                                    if (Sys_Fashion.Instance.activeId > 0)
                                    {
                                        UIManager.OpenUI((int)jumpPrama[0]);
                                    }
                                    else
                                    {
                                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(105103));
                                    }
                                }
                                else
                                {
                                    UIManager.CloseUI(EUIID.UI_SevenDaysTarget);
                                    UIManager.OpenUI((int)jumpPrama[0]);
                                }
                            }
                            break;
                        case 2:
                            {
                                UIManager.CloseUI(EUIID.UI_SevenDaysTarget);
                                if (jumpPrama[0] == 58) //EUIID.UI_SkillUpgrade
                                {
                                    UIManager.OpenUI((int)jumpPrama[0], false, new List<int> { (int)jumpPrama[1] });
                                }
                                else if (jumpPrama[0] == 125) //EUIID.UI_Pet_Message
                                {
                                    PetPrama petPrama = new PetPrama
                                    {
                                        page = (EPetMessageViewState)jumpPrama[1]
                                    };
                                    UIManager.OpenUI((int)jumpPrama[0], false, petPrama);
                                }
                                else
                                {
                                    UIManager.OpenUI((int)jumpPrama[0], false, jumpPrama[1]);
                                }
                            }
                            break;
                        case 3:
                            {
                                UIManager.CloseUI(EUIID.UI_SevenDaysTarget);
                                if (jumpPrama != null && jumpPrama.Count > 0)
                                {
                                    UIDailyActivitesParmas uiDaily = new UIDailyActivitesParmas();
                                    uiDaily.IsSkipDetail = true;
                                    uiDaily.SkipToID = jumpPrama[0];
                                    UIManager.OpenUI(EUIID.UI_DailyActivites, false, uiDaily);
                                }
                                else
                                {
                                    UIManager.OpenUI(EUIID.UI_DailyActivites);
                                }
                            }
                            break;
                        case 4:
                            {
                                //弹个提示，参数是语言表id
                                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(jumpPrama[0]));
                            }
                            break;
                        case 5:
                            {
                                UIManager.CloseUI(EUIID.UI_SevenDaysTarget);
                                UIManager.CloseUI(EUIID.UI_Activity_Exchange);
                                UIManager.CloseUI(EUIID.UI_Activity_Summer);
                                ActionCtrl.Instance.MoveToTargetNPCAndInteractive(jumpPrama[0]);
                            }
                            break;
                        case 6: //6 跳转多重界面(界面1|子id|界面2)
                            {
                                UIManager.CloseUI(EUIID.UI_SevenDaysTarget);
                                if (jumpPrama.Count >= 2)
                                {
                                    UIManager.OpenUI((int)jumpPrama[0], false, jumpPrama[1]);
                                    UIManager.OpenUI((int)jumpPrama[2]);
                                }
                            }
                            break;
                        case 7: //7 商店类型跳转( 商城界面ID | 商城表ID | 商店ID)
                            {
                                UIManager.CloseUI(EUIID.UI_SevenDaysTarget);
                                if (jumpPrama.Count >= 2)
                                {
                                    MallPrama mallPrama = new MallPrama();
                                    mallPrama.mallId = jumpPrama[1];
                                    mallPrama.shopId = jumpPrama[2];
                                    UIManager.OpenUI((int)jumpPrama[0], false, mallPrama);
                                }
                            }
                            break;
                        case 8: //8  类型|等级区间1|等级区间1的跳转活动ID|等级区间2|等级区间2的跳转ID
                            {
                                UIManager.CloseUI(EUIID.UI_SevenDaysTarget);
                                int skipidcount = data.Skip_Id.Count;

                                var rolelevel = Sys_Role.Instance.Role.Level;

                                for (int i = 1; i < skipidcount; i += 2)
                                {
                                    bool haveNextRange = i + 2 <= (skipidcount - 2);

                                    if (haveNextRange && rolelevel >= data.Skip_Id[i] && rolelevel < data.Skip_Id[i + 2])
                                    {
                                        SkipDaily(data.Skip_Id[i + 1]);
                                    }
                                    else if (!haveNextRange)
                                    {
                                        SkipDaily(data.Skip_Id[i + 1]);
                                    }
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            private void SkipDaily(uint id)
            {
                if (Sys_FunctionOpen.Instance.IsOpen(20201))
                {
                    UIManager.OpenUI(EUIID.UI_DailyActivites, false, new UIDailyActivitesParmas() { SkipToID = id, IsSkipDetail = true });
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3910010310));
                }
            }
        }
    }
}