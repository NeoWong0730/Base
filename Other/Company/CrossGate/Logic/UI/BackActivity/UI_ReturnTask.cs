using System;
using Logic.Core;
using Packet;
using UnityEngine.Playables;
using UnityEngine;
using System.Collections.Generic;
using Table;
using UnityEngine.UI;

namespace Logic
{
    public class UI_ReturnTask : UI_BackActivityBase
    {
        readonly uint SliderMaxWidth = 745;
        readonly uint SliderCellWidth = 102;
        private RectTransform SliderTrans;


        private InfinityGrid m_InfinityGrid;

        private CP_ToggleRegistry m_CpToggleRegistry;

        private int m_CurLable = 0; //当前选中的天数

        private Dictionary<GameObject, TaskEntry> m_Qentrys = new Dictionary<GameObject, TaskEntry>();

        private Text m_AllPointText;

        private Transform m_PointParent;

        private List<PropItem> m_PropItems = new List<PropItem>();

        private List<PropIconLoader.ShowItemData> m_ShowItemDatas = new List<PropIconLoader.ShowItemData>();

        private Transform m_DayParent;

        private List<GameObject> m_LockGo = new List<GameObject>(); //天上的锁

        private List<GameObject> m_GetGo = new List<GameObject>(); //天上的勾 当天的任务全部完成并且已领取才显示
        private List<GameObject> m_GetGo2 = new List<GameObject>();

        private List<GameObject> m_RedPoints = new List<GameObject>(); //天上的红点
        
        private  List<Text> m_Points=new List<Text>();//积分数
        
        protected override void Loaded()
        {
            m_InfinityGrid = transform.Find("Scroll View_Content").GetComponent<InfinityGrid>();
            m_CpToggleRegistry = transform.Find("Scroll View_Menu").GetComponent<CP_ToggleRegistry>();
            m_AllPointText = transform.Find("View_Bottom/Image_Bg/Text_Goal/Text_Value").GetComponent<Text>();
            m_PointParent = transform.Find("View_Bottom/Grid");
            m_DayParent = transform.Find("Scroll View_Menu/Viewport/Content");
            SliderTrans = transform.Find("View_Bottom/Slider_BG/Slider").GetComponent<RectTransform>();

            m_CpToggleRegistry.onToggleChange += OnToggleChanged;

            m_InfinityGrid.onCreateCell += OnCreateCell;
            m_InfinityGrid.onCellChange += OnCellChange;

            int childCount = m_PointParent.childCount;

            for (int i = 0; i < childCount; i++)
            {
                PropItem propItem = new PropItem();
                propItem.BindGameObject(m_PointParent.GetChild(i).Find("Image/PropItem").gameObject);
                m_PropItems.Add(propItem);
                
                m_Points.Add(m_PointParent.GetChild(i).Find("Text_Num").GetComponent<Text>());
            }

            for (int i = 0; i < m_DayParent.childCount; i++)
            {
                GameObject lockGo = m_DayParent.GetChild(i).Find("Image_UnSelected/Image_Lock").gameObject;
                m_LockGo.Add(lockGo);

                GameObject getGo = m_DayParent.GetChild(i).Find("Image_UnSelected/Image_Get").gameObject;
                m_GetGo.Add(getGo);
                GameObject getGo2 = m_DayParent.GetChild(i).Find("Image_Selected/Image_Get").gameObject;
                m_GetGo2.Add(getGo2);

                GameObject red = m_DayParent.GetChild(i).Find("Image_Dot").gameObject;
                m_RedPoints.Add(red);

                Text t1 = m_DayParent.GetChild(i).Find("Image_UnSelected/Text").GetComponent<Text>();
                TextHelper.SetText(t1,LanguageHelper.GetTextContent(2014908,(i+1).ToString()));
                
                Text t2 = m_DayParent.GetChild(i).Find("Image_Selected/Text_Select").GetComponent<Text>();
                TextHelper.SetText(t2,LanguageHelper.GetTextContent(2014908,(i+1).ToString()));
            }

            ClearCondition();
            AddCondition();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_ReturnTask.Instance.eventEmitter.Handle<int>(Sys_ReturnTask.EEvents.OnTaskStateUpdate, OnTaskStateChange, toRegister);
            Sys_ReturnTask.Instance.eventEmitter.Handle<int>(Sys_ReturnTask.EEvents.OnTaskStateUpdate, OnUpdateRedState, toRegister);
            Sys_ReturnTask.Instance.eventEmitter.Handle<int>(Sys_ReturnTask.EEvents.OnTaskStateUpdate, UpdateDayTaskIsAllOver, toRegister);
            Sys_ReturnTask.Instance.eventEmitter.Handle(Sys_ReturnTask.EEvents.OnPassDay, UpdateDayLockState, toRegister);
            Sys_ReturnTask.Instance.eventEmitter.Handle(Sys_ReturnTask.EEvents.OnUpdatePoint, UpdatePoint, toRegister);
        }

        private void ClearCondition()
        {
            m_CpToggleRegistry.ClearCondition();
        }

        private void AddCondition()
        {
            for (int i = 0; i < Sys_ReturnTask.Instance.totalDays; i++)
            {
                int day = i;

                Func<bool> condi = () =>
                {
                    bool open = day <= Sys_ReturnTask.Instance.curDay;

                    if (!open)
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2014915));
                    }

                    return open;
                };

                m_CpToggleRegistry.AddCondition(day, condi);

                CP_Toggle ct = m_CpToggleRegistry.transform.Find("Viewport/Content").GetChild(i).GetComponent<CP_Toggle>();

                if (ct != null)
                {
                    ct.RegisterCondition(condi);
                }
            }
        }
        
        private void OnCreateCell(InfinityGridCell cell)
        {
            TaskEntry entry = new TaskEntry();
            entry.BindGameObject(cell.mRootTransform.gameObject);
            cell.BindUserData(entry);
            m_Qentrys.Add(cell.mRootTransform.gameObject, entry);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            TaskEntry entry = cell.mUserData as TaskEntry;

            Sys_ReturnTask.SingleTaskData st = Sys_ReturnTask.Instance.returnTaskDatas[m_CurLable].singleTaskDatas[index];
            entry.SetData(st);
        }


        private void OnToggleChanged(int current, int old)
        {
            if (current == old)
                return;

            m_CurLable = current;
            Sys_ReturnTask.Instance.SortTask(m_CurLable);
            UpdateInfo();
        }

        private void OnTaskStateChange(int day)
        {
            if (day != m_CurLable)
                return;

            m_InfinityGrid.CellCount = Sys_ReturnTask.Instance.returnTaskDatas[m_CurLable].singleTaskDatas.Count;

            m_InfinityGrid.ForceRefreshActiveCell();
        }

        private void OnUpdateRedState(int day)
        {
            m_RedPoints[day].SetActive(Sys_ReturnTask.Instance.HasRed(day));
        }


        private void UpdateInfo()
        {
            m_InfinityGrid.CellCount = Sys_ReturnTask.Instance.returnTaskDatas[m_CurLable].singleTaskDatas.Count;
            m_InfinityGrid.ForceRefreshActiveCell();

            UpdatePoint();
            UpdateDayLockState();


            for (int i = 0; i < Sys_ReturnTask.Instance.totalDays; i++)
            {
                UpdateDayTaskIsAllOver(i);
                OnUpdateRedState(i);
            }
        }

        private void UpdateDayLockState()
        {
            int childCount = m_DayParent.childCount;

            for (int i = 0; i < childCount; i++)
            {
                bool _lock = i > Sys_ReturnTask.Instance.curDay;
                
                m_LockGo[i].SetActive(_lock);
            }
        }

        /// <summary>
        /// 刷新某一天的任务奖励是否全部领取
        /// </summary>
        private void UpdateDayTaskIsAllOver(int day)
        {
            m_GetGo[day].SetActive(Sys_ReturnTask.Instance.AllTaskOver(day));
            m_GetGo2[day].SetActive(Sys_ReturnTask.Instance.AllTaskOver(day));
        }
        
        private void UpdatePoint()
        {
            TextHelper.SetText(m_AllPointText, Sys_ReturnTask.Instance.point.ToString());
            float width = Sys_ReturnTask.Instance.GetSevenDaysTargetScoreSliderWidth(SliderCellWidth, SliderMaxWidth);
            SliderTrans.sizeDelta = new Vector2(width, SliderTrans.sizeDelta.y);

            int index = 0;

            foreach (var item in Sys_ReturnTask.Instance.points)
            {
                PropItem propItem = m_PropItems[index];

                uint pointId = item.Key;
                CSVReturnChanllengeIntegral.Data data = CSVReturnChanllengeIntegral.Instance.GetConfData(pointId);

                List<ItemIdCount> itemIdCounts = CSVDrop.Instance.GetDropItem(data.Dropid);
                uint itemId = itemIdCounts[0].id;
                uint count = (uint) itemIdCounts[0].count;

                PropIconLoader.ShowItemData showItem;

                Action<PropItem> action = arg =>
                {
                    bool canGet = Sys_ReturnTask.Instance.point >= data.Requiredpoints;
                    bool isGet = item.Value;
                    if (canGet && !isGet)
                    {
                        Sys_ReturnTask.Instance.TaskPointRewardReq(pointId);
                    }
                    else
                    {
                        PropIconLoader.ShowItemData showItemData = new PropIconLoader.ShowItemData(itemId, count, false, false, false, false, false, false, true);
                        UIManager.OpenUI(EUIID.UI_Message_Box, false, new MessageBoxEvt(EUIID.UI_BackActivity, showItemData));
                    }
                };
                
                if (index < m_ShowItemDatas.Count)
                {
                    showItem = m_ShowItemDatas[index];
                    showItem.Refresh(_id: itemId,
                        _count: count,
                        _bUseQuailty: true,
                        _bBind: false,
                        _bNew: false,
                        _bUnLock: false,
                        _bSelected: false,
                        _bShowCount: true,
                        _bShowBagCount: false,
                        _bUseClick: true,
                        _onClick: action,
                        _bshowBtnNo: false,
                        false);
                }
                else
                {
                    showItem = new PropIconLoader.ShowItemData(
                        _id: itemId,
                        _count: count,
                        _bUseQuailty: true,
                        _bBind: false,
                        _bNew: false,
                        _bUnLock: false,
                        _bSelected: false,
                        _bShowCount: true,
                        _bShowBagCount: false,
                        _bUseClick: true,
                        _onClick: action,
                        _bshowBtnNo: false,
                        false);
                    m_ShowItemDatas.Add(showItem);
                }
                
                propItem.OnEnableLongPress(true);
                propItem.SetData(new MessageBoxEvt(EUIID.UI_LuckyDraw_Result, showItem));

                bool get = item.Value;
                propItem.SetGot(get);
                if (get)
                {
                    propItem.SetBreathing(false);
                }
                else
                {
                    propItem.SetBreathing(Sys_ReturnTask.Instance.point >= data.Requiredpoints);
                }
                Text point = m_Points[index];
                TextHelper.SetText(point,data.Requiredpoints.ToString());
                
                ++index;
            }
        }

        public override void Show()
        {
            base.Show();
            m_CurLable = 0;
            Sys_ReturnTask.Instance.SortTask(m_CurLable);
            m_CpToggleRegistry.SwitchTo(m_CurLable);
            UpdateInfo();
        }

        public override bool CheckFunctionIsOpen()
        {
            return Sys_ReturnTask.Instance.CheckReturnTaskIsOpen();
        }

        public override bool CheckTabRedPoint()
        {
            return Sys_ReturnTask.Instance.HasAllRed();
        }
        public class TaskEntry
        {
            private uint m_Id;

            public uint id
            {
                get { return m_Id; }
            }

            private Sys_ReturnTask.SingleTaskData m_SingleTaskData;

            private CSVReturnChanllengeTask.Data m_Data;

            private GameObject m_Go;
            private PropItem m_PropItem;
            private GameObject m_PropGo;
            private Slider m_Slider;
            private Text m_ProGress;
            private Button m_GoToButton;
            private Button m_GetButton;
            private GameObject m_ImageGet;

            private Text m_Name;
            private Text m_Des;

            private List<ItemIdCount> m_ItemIdCounts = new List<ItemIdCount>();

            public void BindGameObject(GameObject gameObject)
            {
                m_Go = gameObject;

                m_PropGo = m_Go.transform.Find("PropItem").gameObject;
                m_PropItem = new PropItem();
                m_PropItem.BindGameObject(m_PropGo);

                m_Slider = m_Go.transform.Find("Slider_Exp").GetComponent<Slider>();
                m_ProGress = m_Go.transform.Find("Text_Num").GetComponent<Text>();
                m_ImageGet = m_Go.transform.Find("View_State/Image_Get").gameObject;
                m_GoToButton = m_Go.transform.Find("View_State/Btn_02").GetComponent<Button>();
                m_GetButton = m_Go.transform.Find("View_State/Btn_01").GetComponent<Button>();

                m_Name = m_Go.transform.Find("Text").GetComponent<Text>();
                m_Des = m_Go.transform.Find("Text01").GetComponent<Text>();

                m_GetButton.onClick.AddListener(OnGetButtonClicked);
                m_GoToButton.onClick.AddListener(OnGoToButtonClicked);
            }

            public void SetData(Sys_ReturnTask.SingleTaskData singleTaskData)
            {
                m_Id = singleTaskData.id;
                m_SingleTaskData = singleTaskData;
                m_Data = singleTaskData.csv;

                RefreshEntry();
            }

            public void RefreshEntry()
            {
                TextHelper.SetText(m_Des, m_Data.Task_Des);
                m_Name.text = "";
                m_ItemIdCounts = CSVDrop.Instance.GetDropItem(m_Data.Dropid);

                uint itemId = m_ItemIdCounts[0].id;

                uint itemCount = (uint) m_ItemIdCounts[0].count;

                PropIconLoader.ShowItemData showItem = new PropIconLoader.ShowItemData
                (_id: itemId,
                    _count: itemCount,
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
                m_PropItem.SetData(new MessageBoxEvt(EUIID.UI_LuckyDraw_Result, showItem));

                if (m_SingleTaskData.state == 0) //可领取
                {
                    m_GetButton.gameObject.SetActive(true);
                    m_ImageGet.SetActive(false);
                    m_GoToButton.gameObject.SetActive(false);
                }
                else if (m_SingleTaskData.state == 1) //未完成
                {
                    //判断跳转是否显示
                    bool showGoTo = m_SingleTaskData.csv.Change_UI != null && m_SingleTaskData.csv.Change_UI[0] != 0;
                    m_GoToButton.gameObject.SetActive(showGoTo);
                    m_GetButton.gameObject.SetActive(false);
                    m_ImageGet.SetActive(false);
                }
                else //已领取
                {
                    m_ImageGet.SetActive(true);
                    m_GoToButton.gameObject.SetActive(false);
                    m_GetButton.gameObject.SetActive(false);
                }

                m_Slider.value = (float) m_SingleTaskData.num / (float) m_SingleTaskData.need;
                
                TextHelper.SetText(m_ProGress,$"{m_SingleTaskData.num}/{m_SingleTaskData.need}");
            }

            private void OnGetButtonClicked()
            {
                Sys_ReturnTask.Instance.TaskRewardReq(m_Data.id);
            }

            private void OnGoToButtonClicked()
            {
                TaskToGo(m_Data);
            }

            private void TaskToGo(CSVReturnChanllengeTask.Data data)
            {
                if (data != null)
                {
                    UIManager.CloseUI(EUIID.UI_Activity_Summer);
                    UIManager.CloseUI(EUIID.UI_Activity_Exchange);

                    var type = data.Change_UI[0];
                    List<uint> jumpPrama = data.Change_UI;
                    if (jumpPrama[1] == 311) //EUIID.UI_Family
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
                            if (jumpPrama[1] == 94)
                            {
                                //好友跳转特殊处理
                                UIManager.OpenUI((int) jumpPrama[1]);
                            }
                            else if (jumpPrama[1] == 730) //时装抽奖 特殊处理
                            {
                                if (Sys_Fashion.Instance.activeId > 0)
                                {
                                    UIManager.OpenUI((int) jumpPrama[1]);
                                }
                                else
                                {
                                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(105103));
                                }
                            }
                            else
                            {
                                UIManager.CloseUI(EUIID.UI_SevenDaysTarget);
                                UIManager.OpenUI((int) jumpPrama[1]);
                            }
                        }
                            break;
                        case 2:
                        {
                            UIManager.CloseUI(EUIID.UI_SevenDaysTarget);
                            if (jumpPrama[1] == 58) //EUIID.UI_SkillUpgrade
                            {
                                UIManager.OpenUI((int) jumpPrama[1], false, new List<int> {(int) jumpPrama[2]});
                            }
                            else if (jumpPrama[1] == 125) //EUIID.UI_Pet_Message
                            {
                                PetPrama petPrama = new PetPrama
                                {
                                    page = (EPetMessageViewState) jumpPrama[2]
                                };
                                UIManager.OpenUI((int) jumpPrama[1], false, petPrama);
                            }
                            else
                            {
                                UIManager.OpenUI((int) jumpPrama[1], false, jumpPrama[2]);
                            }
                        }
                            break;
                        case 3:
                        {
                            UIManager.CloseUI(EUIID.UI_SevenDaysTarget);
                            if (jumpPrama != null && jumpPrama.Count > 1)
                            {
                                UIDailyActivitesParmas uiDaily = new UIDailyActivitesParmas();
                                uiDaily.IsSkipDetail = true;
                                uiDaily.SkipToID = jumpPrama[1];
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
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(jumpPrama[1]));
                        }
                            break;
                        case 5:
                        {
                            UIManager.CloseUI(EUIID.UI_SevenDaysTarget);
                            UIManager.CloseUI(EUIID.UI_Activity_Exchange);
                            UIManager.CloseUI(EUIID.UI_Activity_Summer);
                            ActionCtrl.Instance.MoveToTargetNPCAndInteractive(jumpPrama[1]);
                        }
                            break;
                        case 6: //6 跳转多重界面(界面1|子id|界面2)
                        {
                            UIManager.CloseUI(EUIID.UI_SevenDaysTarget);
                            if (jumpPrama.Count >= 3)
                            {
                                UIManager.OpenUI((int) jumpPrama[1], false, jumpPrama[2]);
                                UIManager.OpenUI((int) jumpPrama[3]);
                            }
                        }
                            break;
                        case 7: //7 商店类型跳转( 商城界面ID | 商城表ID | 商店ID)
                        {
                            UIManager.CloseUI(EUIID.UI_SevenDaysTarget);
                            if (jumpPrama.Count >= 3)
                            {
                                MallPrama mallPrama = new MallPrama();
                                mallPrama.mallId = jumpPrama[2];
                                mallPrama.shopId = jumpPrama[3];
                                UIManager.OpenUI((int) jumpPrama[1], false, mallPrama);
                            }
                        }
                            break;
                        case 8: //8  类型|等级区间1|等级区间1的跳转活动ID|等级区间2|等级区间2的跳转ID
                        {
                            UIManager.CloseUI(EUIID.UI_SevenDaysTarget);
                            int skipidcount = data.Change_UI.Count;

                            var rolelevel = Sys_Role.Instance.Role.Level;

                            for (int i = 1; i < skipidcount; i += 2)
                            {
                                bool haveNextRange = i + 2 <= (skipidcount - 2);

                                if (haveNextRange && rolelevel >= data.Change_UI[i] && rolelevel < data.Change_UI[i + 2])
                                {
                                    SkipDaily(data.Change_UI[i + 1]);
                                }
                                else if (!haveNextRange)
                                {
                                    SkipDaily(data.Change_UI[i + 1]);
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
                    UIManager.OpenUI(EUIID.UI_DailyActivites, false, new UIDailyActivitesParmas() {SkipToID = id, IsSkipDetail = true});
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3910010310));
                }
            }
        }
    }
}