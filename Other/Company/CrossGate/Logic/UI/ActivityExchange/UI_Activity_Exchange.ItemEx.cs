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
        #region GUILable

        private CP_ToggleRegistry m_CP_ToggleRegistry_Lb;

        private Button m_CloseButton;

        private InfinityGrid m_InfinityGrid_ExItem;

        private Dictionary<GameObject, ExItemEntry> m_ExEntryss = new Dictionary<GameObject, ExItemEntry>();

        private Text m_ActivityValidTime;

        private Text m_RemainTime;

        private GameObject m_DaliyTips1;

        private GameObject m_DaliyTips2;

        private GameObject m_Menu;

        private GameObject m_ExNo;

        private Image m_AcBG;

        private Text m_AcTitle;

        private RawImage m_AcIcon;

        private int m_CurLable;

        private GameObject m_ItemExRed;

        private GameObject m_QuestRed;

        #endregion

        protected override void OnInit()
        {
            if (Sys_ItemExChange.Instance.isActivity)
            {
                m_CurLable = 1;
            }
            else
            {
                m_CurLable = 2;
            }
        }

        protected override void ProcessEvents(bool toRegister)
        {
            Sys_ItemExChange.Instance.eventEmitter.Handle<uint>(Sys_ItemExChange.EEvents.e_RefreshEx, OnRefreshExEntry, toRegister);
            Sys_ItemExChange.Instance.eventEmitter.Handle(Sys_ItemExChange.EEvents.e_RefreshEx, OnRefreshExEntryAll, toRegister);
            Sys_ItemExChange.Instance.eventEmitter.Handle(Sys_ItemExChange.EEvents.e_AcrossDay_Ex, RefreshExchangeNode, toRegister);
            Sys_ActivityQuest.Instance.eventEmitter.Handle<uint>(Sys_ActivityQuest.EEvents.e_RefreshTaskEntry, OnRefreshTaskEntry, toRegister);
            Sys_ActivityQuest.Instance.eventEmitter.Handle(Sys_ActivityQuest.EEvents.e_RefreshTaskEntry, OnRefreshTaskEntryAll, toRegister);
            Sys_ActivityQuest.Instance.eventEmitter.Handle(Sys_ActivityQuest.EEvents.e_AcrossDay_Qt, RefreshQuestNode, toRegister);
        }


        protected override void OnLoaded()
        {
            m_AcBG = transform.Find("Animator/View_TipsBgNew01/Image_bg01").GetComponent<Image>();
            m_AcIcon = transform.Find("Animator/Image02").GetComponent<RawImage>();
            m_AcTitle = transform.Find("Animator/Title/Text1").GetComponent<Text>();
            m_Menu = transform.Find("Animator/Menu").gameObject;
            m_ExNo = transform.Find("Animator/Scroll View01/Image_No").gameObject;
            m_QtNo = transform.Find("Animator/Scroll View02/Image_No").gameObject;
            m_ItemExRed = transform.Find("Animator/Menu/ListItem/Dot").gameObject;
            m_QuestRed = transform.Find("Animator/Menu/ListItem (1)/Dot").gameObject;
            m_ActivityValidTime = transform.Find("Animator/Image_Time/Text").GetComponent<Text>();
            m_RemainTime = transform.Find("Animator/Text_Time").GetComponent<Text>();
            m_DaliyTips1 = transform.Find("Animator/Scroll View01/Text").gameObject;
            m_DaliyTips2 = transform.Find("Animator/Scroll View02/Text").gameObject;

            m_InfinityGrid_ExItem = transform.Find("Animator/Scroll View01").GetComponent<InfinityGrid>();
            m_InfinityGrid_ExItem.onCreateCell += OnCreateCell_Ex;
            m_InfinityGrid_ExItem.onCellChange += OnCellChange_Ex;

            m_InfinityGrid_QtTask = transform.Find("Animator/Scroll View02").GetComponent<InfinityGrid>();
            m_InfinityGrid_QtTask.onCreateCell += OnCreateCell_Qt;
            m_InfinityGrid_QtTask.onCellChange += OnCellChange_Qt;

            m_CloseButton = transform.Find("Animator/View_TipsBgNew01/Btn_Close").GetComponent<Button>();
            m_CloseButton.onClick.AddListener(OnCloseButtonClicked);

            m_CP_ToggleRegistry_Lb = transform.Find("Animator/Menu").GetComponent<CP_ToggleRegistry>();
            m_CP_ToggleRegistry_Lb.onToggleChange = OnToggleChanged;
        }

        protected override void OnShow()
        {
            UpdateInfo();
        }

        private void UpdateInfo()
        {
            SetTitleAndBG();
            SetActivityRoot();
            RefreshItemEntry();
            SetActivityValidTime();
            RefreshRedPoint();
        }

        private void SetTitleAndBG()
        {
            if (Sys_ItemExChange.Instance.isActivity)
            {
                TextHelper.SetText(m_AcTitle, Sys_ItemExChange.Instance.curCSVActivityRulerData.Activity_Name);
                ImageHelper.SetIcon(m_AcBG, null, Sys_ItemExChange.Instance.curCSVActivityRulerData.Back_Image, false);
                ImageHelper.SetTexture(m_AcIcon, Sys_ItemExChange.Instance.curCSVActivityRulerData.Foreg_Image, true);
            }
            else if (Sys_ActivityQuest.Instance.isActivity)
            {
                TextHelper.SetText(m_AcTitle, Sys_ActivityQuest.Instance.curCSVActivityRulerData.Activity_Name);
                ImageHelper.SetIcon(m_AcBG, null, Sys_ActivityQuest.Instance.curCSVActivityRulerData.Back_Image, false);
                ImageHelper.SetTexture(m_AcIcon, Sys_ActivityQuest.Instance.curCSVActivityRulerData.Foreg_Image, true);
            }
        }

        private void RefreshRedPoint()
        {
            m_ItemExRed.SetActive(Sys_ItemExChange.Instance.hasRed());
            m_QuestRed.SetActive(Sys_ActivityQuest.Instance.hasRed()); 
        }

        private void RefreshExchangeNode()
        {
            if (!Sys_ItemExChange.Instance.isActivity)
            {
                m_ExNo.SetActive(true);
                m_InfinityGrid_ExItem.CellCount = 0;
                m_InfinityGrid_ExItem.ForceRefreshActiveCell();
            }
            else
            {
                m_ExNo.SetActive(false);
                UpdateInfo();
            }
        }

        private void SetActivityRoot()
        {
            if (!Sys_ItemExChange.Instance.isActivity || !Sys_ActivityQuest.Instance.isActivity)//任意一个活动没开启 就把两个页签隐藏掉
            {
                m_Menu.SetActive(false);
            }
            else
            {
                m_Menu.SetActive(true);
            }
            if (m_Menu.activeSelf)
            {
                m_CP_ToggleRegistry_Lb.SwitchTo(m_CurLable);
            }
            else
            {
                if (Sys_ItemExChange.Instance.isActivity)
                {
                    m_InfinityGrid_ExItem.gameObject.SetActive(true);
                    m_InfinityGrid_QtTask.gameObject.SetActive(false);
                }
                else if (Sys_ActivityQuest.Instance.isActivity)
                {
                    m_InfinityGrid_ExItem.gameObject.SetActive(false);
                    m_InfinityGrid_QtTask.gameObject.SetActive(true);
                }
            }
        }

        private void RefreshItemEntry()
        {
            if (m_CurLable == 1)
            {
                if (Sys_ItemExChange.Instance.isActivity)
                {
                    Sys_ItemExChange.Instance.UpdateExItemState();
                    m_InfinityGrid_ExItem.CellCount = Sys_ItemExChange.Instance.exDatas.Count;
                    m_InfinityGrid_ExItem.ForceRefreshActiveCell();
                }

                m_DaliyTips1.SetActive(false);
            }
            else if (m_CurLable == 2)
            {
                if (Sys_ActivityQuest.Instance.isActivity)
                {
                    m_InfinityGrid_QtTask.CellCount = Sys_ActivityQuest.Instance.questDatas.Count;
                    m_InfinityGrid_QtTask.ForceRefreshActiveCell();
                }

                m_DaliyTips2.SetActive(Sys_ActivityQuest.Instance.isActivity);
            }
        }

        protected override void OnUpdate()
        {
            SetActivityValidTime();
        }

        private void SetActivityValidTime()
        {
            if (m_CurLable == 1)
            {
                DateTime startTime = TimeManager.GetDateTime(Sys_ItemExChange.Instance.startTime);
                DateTime endTime = TimeManager.GetDateTime(Sys_ItemExChange.Instance.endTime);
                TextHelper.SetText(m_ActivityValidTime, LanguageHelper.GetTextContent(591000705, startTime.Year.ToString(), startTime.Month.ToString(), startTime.Day.ToString()
                    , GetTimeFormat(startTime.Hour), GetTimeFormat(startTime.Minute), endTime.Year.ToString(), endTime.Month.ToString(), endTime.Day.ToString()
                    , GetTimeFormat(endTime.Hour), GetTimeFormat(endTime.Minute)));

                if (Sys_ItemExChange.Instance.endTime < Sys_Time.Instance.GetServerTime())
                {
                    TextHelper.SetText(m_RemainTime, 2025733);
                }
                else
                {
                    uint remainTime = Sys_ItemExChange.Instance.endTime - Sys_Time.Instance.GetServerTime();
                    TextHelper.SetText(m_RemainTime, LanguageHelper.TimeToString(remainTime, LanguageHelper.TimeFormat.Type_12));
                }
            }
            else if (m_CurLable == 2)
            {
                DateTime startTime = TimeManager.GetDateTime(Sys_ActivityQuest.Instance.startTime);
                DateTime endTime = TimeManager.GetDateTime(Sys_ActivityQuest.Instance.endTime);
                TextHelper.SetText(m_ActivityValidTime, LanguageHelper.GetTextContent(591000705, startTime.Year.ToString(), startTime.Month.ToString(), startTime.Day.ToString()
                   , GetTimeFormat(startTime.Hour), GetTimeFormat(startTime.Minute), endTime.Year.ToString(), endTime.Month.ToString(), endTime.Day.ToString()
                   , GetTimeFormat(endTime.Hour), GetTimeFormat(endTime.Minute)));

                if (Sys_ActivityQuest.Instance.endTime < Sys_Time.Instance.GetServerTime())
                {
                    TextHelper.SetText(m_RemainTime, 2025733);
                }
                else
                {
                    uint remainTime = Sys_ActivityQuest.Instance.endTime - Sys_Time.Instance.GetServerTime();
                    TextHelper.SetText(m_RemainTime, LanguageHelper.TimeToString(remainTime, LanguageHelper.TimeFormat.Type_12));
                }
            }
        }

        private string GetTimeFormat(int value)
        {
            string zero = "0";
            string tp = string.Empty;
            StringBuilder sb = StringBuilderPool.GetTemporary();
            if (value < 10)
            {
                sb.Append(zero);
                sb.Append(value.ToString());
                tp = StringBuilderPool.ReleaseTemporaryAndToString(sb);
            }
            else
            {
                sb.Append(value.ToString());
                tp = StringBuilderPool.ReleaseTemporaryAndToString(sb);
            }

            return tp;
        }

        private void OnToggleChanged(int current, int old)
        {
            if (current == m_CurLable)
            {
                return;
            }
            m_CurLable = current;
            RefreshItemEntry();
        }

        private void OnCreateCell_Ex(InfinityGridCell cell)
        {
            ExItemEntry grid = new ExItemEntry();
            grid.BindGameObject(cell.mRootTransform.gameObject);
            cell.BindUserData(grid);
            m_ExEntryss.Add(cell.mRootTransform.gameObject, grid);
        }

        private void OnCellChange_Ex(InfinityGridCell cell, int index)
        {
            ExItemEntry grid = cell.mUserData as ExItemEntry;
            grid.SetData(Sys_ItemExChange.Instance.exDatas[index].id);
        }

        private void OnRefreshExEntry(uint exId)
        {
            foreach (var item in m_ExEntryss)
            {
                GameObject go = item.Key;

                if (!go.activeSelf)
                    continue;

                ExItemEntry exEntry = item.Value;

                if (exEntry.id == exId)
                {
                    item.Value.RefreshEntry();
                }
            }
            RefreshRedPoint();
        }

        private void OnRefreshExEntryAll()
        {
            m_InfinityGrid_ExItem.CellCount = Sys_ItemExChange.Instance.exDatas.Count;
            m_InfinityGrid_ExItem.ForceRefreshActiveCell();
            RefreshRedPoint();
        }


        private void OnCloseButtonClicked()
        {
            UIManager.CloseUI(EUIID.UI_Activity_Exchange);
        }

        public class ExItemEntry
        {
            private uint m_Id;

            public uint id
            {
                get
                {
                    return m_Id;
                }
            }

            public Sys_ItemExChange.ExData exData
            {
                get;
                private set;
            }

            private GameObject m_Go;

            private CSVExchangeItem.Data m_Data;

            private Transform m_RewardParent1;

            private Button m_GiftButton;

            private PropItem m_ExPropItem;

            private Button m_ExButton;

            private Text m_ExButtonText;

            private Toggle m_Toggle;

            private Text m_LimitNum;

            private GameObject m_RedPoint;

            private bool b_Extra;

            public void BindGameObject(GameObject gameObject)
            {
                m_Go = gameObject;

                m_RewardParent1 = m_Go.transform.Find("Reward/Viewport");
                m_GiftButton = m_Go.transform.Find("Reward01/Viewport/Item/Button").GetComponent<Button>();
                m_ExButton = m_Go.transform.Find("Btn_01").GetComponent<Button>();
                m_ExButtonText = m_Go.transform.Find("Btn_01/Text_01").GetComponent<Text>();
                m_Toggle = m_Go.transform.Find("Toggle").GetComponent<Toggle>();
                m_LimitNum = m_Go.transform.Find("Text_Num").GetComponent<Text>();
                m_RedPoint = m_Go.transform.Find("Btn_01/RedTips").gameObject;
                m_ExPropItem = new PropItem();
                m_ExPropItem.BindGameObject(m_Go.transform.Find("Reward01/Viewport/Item/PropItem").gameObject);

                m_ExButton.onClick.AddListener(OnExButtonClicked);
                m_Toggle.onValueChanged.AddListener(OnValueChanged);
                m_GiftButton.onClick.AddListener(OnGiftButtonClicked);
            }

            public void SetData(uint id)
            {
                m_Id = id;

                m_Data = CSVExchangeItem.Instance.GetConfData(id);

                exData = Sys_ItemExChange.Instance.TryGetExData(id);

                m_Toggle.isOn = exData.openRed;

                RefreshEntry();
            }

            public void RefreshEntry()
            {
                int needCount = m_Data.Activity_Item.Count;
                FrameworkTool.CreateChildList(m_RewardParent1, needCount);

                for (int i = 0; i < needCount; i++)
                {
                    GameObject child = m_RewardParent1.GetChild(i).gameObject;

                    uint itemId = m_Data.Activity_Item[i][0];
                    uint needItemCount = m_Data.Activity_Item[i][1];

                    PropItem propItem = new PropItem();
                    propItem.BindGameObject(child.gameObject);
                    PropIconLoader.ShowItemData showItem = new PropIconLoader.ShowItemData
                        (_id: itemId,
                        _count: needItemCount,
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
                    propItem.SetData(new MessageBoxEvt(EUIID.UI_LuckyDraw_Result, showItem));
                }

                b_Extra = exData.exNum < m_Data.Limit_Max;

                if (m_Data.Limit_Type == 0)//没有限制
                {
                    TextHelper.SetText(m_LimitNum, string.Empty);

                    if (exData.isCountEnough)
                    {
                        TextHelper.SetText(m_ExButtonText, 2025714);//兑换
                    }
                    else
                    {
                        TextHelper.SetText(m_ExButtonText, 2025715);//道具不足
                    }
                    ImageHelper.SetImageGray(m_ExButton.image, false);
                }
                else
                {
                    if (m_Data.Limit_Type == 1)//每日限制
                    {
                        TextHelper.SetText(m_LimitNum, 2025712u, (m_Data.Limit_Max - exData.exNum).ToString());
                    }
                    else if (m_Data.Limit_Type == 2)//普通限制
                    {
                        TextHelper.SetText(m_LimitNum, 2025728u, (m_Data.Limit_Max - exData.exNum).ToString());
                    }
                    if (b_Extra && exData.isCountEnough)//没有限制 并且道具足够
                    {
                        TextHelper.SetText(m_ExButtonText, 2025714);//兑换
                        ImageHelper.SetImageGray(m_ExButton.image, false);
                    }
                    else if (!exData.isCountEnough && b_Extra)//道具不足 并且拥有限制次数
                    {
                        TextHelper.SetText(m_ExButtonText, 2025715); //道具不足
                        ImageHelper.SetImageGray(m_ExButton.image, false);
                    }
                    else if (!exData.isCountEnough && !b_Extra)
                    {
                        TextHelper.SetText(m_ExButtonText, 2025730);
                        ImageHelper.SetImageGray(m_ExButton.image, true);
                    }
                    else if (!b_Extra && exData.isCountEnough)//次数用完
                    {
                        TextHelper.SetText(m_ExButtonText, 2025730);//兑换
                        ImageHelper.SetImageGray(m_ExButton.image, true);
                    }
                }


                m_RedPoint.SetActive(exData.canEx);

                uint dropId = m_Data.Exchange_Item;
                List<ItemIdCount> itemIdCounts = CSVDrop.Instance.GetDropItem(dropId);

                if (itemIdCounts.Count == 1)
                {
                    m_GiftButton.gameObject.SetActive(false);
                    m_ExPropItem.SetActive(true);
                    PropIconLoader.ShowItemData showItem = new PropIconLoader.ShowItemData
                        (_id: itemIdCounts[0].id,
                        _count: itemIdCounts[0].count,
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
                    m_ExPropItem.SetData(new MessageBoxEvt(EUIID.UI_LuckyDraw_Result, showItem));
                }
                else
                {
                    m_ExPropItem.SetActive(false);
                    m_GiftButton.gameObject.SetActive(true);
                }
            }

            private void OnExButtonClicked()
            {
                if (!Sys_OperationalActivity.Instance.CheckActivitySwitchIsOpen(211))
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2028104));
                    return;
                }
                if (m_Data.Limit_Type == 0)//没有限制
                {
                    if (exData.isCountEnough)
                    {
                        //兑换
                        Sys_ItemExChange.Instance.ActivityExchangeTakeReq(m_Id);
                    }
                    else
                    {
                        string content = LanguageHelper.GetTextContent(2025729); //兑换道具数量不足

                        Sys_Hint.Instance.PushContent_Normal(content);
                    }
                }
                else
                {
                    if (b_Extra && exData.isCountEnough)//没有限制 并且道具足够
                    {
                        //兑换
                        Sys_ItemExChange.Instance.ActivityExchangeTakeReq(m_Id);
                    }
                    else if (!exData.isCountEnough && b_Extra)//道具不足 并且拥有限制次数
                    {
                        string content = LanguageHelper.GetTextContent(2025729); //兑换道具数量不足

                        Sys_Hint.Instance.PushContent_Normal(content);
                    }
                    else if (!exData.isCountEnough && !b_Extra)//次数用完 道具不足
                    {
                        string content = LanguageHelper.GetTextContent(2025723); //活动道具兑换完毕

                        Sys_Hint.Instance.PushContent_Normal(content);
                    }
                    else if (!b_Extra && exData.isCountEnough)//次数用完
                    {
                        string content = LanguageHelper.GetTextContent(2025723); //活动道具兑换完毕

                        Sys_Hint.Instance.PushContent_Normal(content);
                    }
                }
            }

            private void OnGiftButtonClicked()
            {
                RewardPanelParam _param = new RewardPanelParam();
                Vector3 _vec = m_GiftButton.gameObject.GetComponent<RectTransform>().position;
                Vector3 _screenVec = CameraManager.mUICamera.WorldToScreenPoint(_vec);
                _param.propList = CSVDrop.Instance.GetDropItem(m_Data.Exchange_Item);
                _param.Pos = _screenVec;
                UIManager.OpenUI(EUIID.UI_RewardPanel, false, _param);
            }

            private void OnValueChanged(bool arg)
            {
                exData.openRed = arg;
                Sys_ItemExChange.Instance.SaveMemory();
            }
        }

    }
}


