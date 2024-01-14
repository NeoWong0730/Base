using Lib.Core;
using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using Table;
using UnityEngine.UI;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
using Framework;

namespace Logic
{
    /// <summary> 地域防范 </summary>
    public class UI_AreaProtection : UIBase
    {
        #region 界面组件
        /// <summary> 月亮图标 </summary>
        private GameObject go_Moon;
        /// <summary> 太阳图标 </summary>
        private GameObject go_Sun;
        /// <summary> 时间进度 </summary>
        private Slider slider_Time;
        /// <summary> 下一个事件时间 </summary>
        private Text text_NextEventTime;
        /// <summary> 预览界面 </summary>
        private RectTransform rt_Preview;
        /// <summary> 接取界面 </summary>
        private RectTransform rt_Current;
        /// <summary> 组队按钮 </summary>
        private Button button_Team;
        /// <summary> 领取委托按钮 </summary>
        private Button button_Entruest;
        /// <summary> 放弃任务按钮 </summary>
        private Button button_GiveUpTask;
        /// <summary> 任务次数 </summary>
        private Text text_TaskCount;
        /// <summary> 提示节点 </summary>
        private Transform tr_TipsNode;
        /// <summary> 提示事件名 </summary>
        private Transform tr_TipsName;
        /// <summary> 事件开关组 </summary>
        private ToggleGroup tg_EventMark;
        /// <summary> 提示内容 </summary>
        private Transform tr_TipsContent;
        /// <summary> 提示背景 </summary>
        private Transform tr_Background;
        /// <summary> 标记模版 </summary>
        private Transform tr_MarkNode;
        /// <summary> 事件模版 </summary>
        private Transform tr_EventItem;
        /// <summary> 翻页 </summary>
        private CP_PageDot cP_PageDot;
        /// <summary> 无限滚动 </summary>
        private InfinityGridLayoutGroup gridGroup;
        /// <summary> 居中脚本 </summary>
        private UICenterOnChild uiCenterOnChild;
        /// <summary> 事件标记字典 </summary>
        private Dictionary<uint, Toggle> dict_EventMark = new Dictionary<uint, Toggle>();
        /// <summary> 事件字典 </summary>
        private Dictionary<uint, Toggle> dict_EventObject = new Dictionary<uint, Toggle>();
        #endregion
        #region 数据定义
        /// <summary> 当前事件编号 </summary>
        private uint curEventId { get; set; }
        /// <summary> 岛屿列表 </summary>
        private List<uint> list_Island = new List<uint>();
        /// <summary> 区域事件 </summary>
        private Dictionary<uint, List<uint>> dict_Event = new Dictionary<uint, List<uint>>();
        /// <summary> 资源列表 </summary>
        private Dictionary<string, AsyncOperationHandle<GameObject>> mAssetRequest = new Dictionary<string, AsyncOperationHandle<GameObject>>();
        /// <summary> 每天初始时间 </summary>
        private uint onedaytime, starttime;
        #endregion

        private Toggle toggleSkip;
        #region 系统函数
        protected override void OnInit()
        {
            //按照120帧跑120/120 1000ms
            SetIntervalFrame(120);

            onedaytime = CSVWeatherTime.Instance.GetConfData(1).time;
            starttime = Framework.TimeManager.ConvertFromZeroTimeZone(CSVWeatherTime.Instance.GetConfData(4).time);
        }
        protected override void OnLoaded()
        {
            OnParseComponent();
            SetIslandsData();
            SetPage();
            SetEventItemList();
        }
        protected override void OnUpdate()
        {
            UpdateView();
        }
        protected override void OnOpen(object arg)
        {
            curEventId = Sys_Activity.Instance.ringTaskData.ringId;
        }
        protected override void OnShow()
        {
            RefreshView();
        }
        protected override void OnHide()
        {
            RecoveryAsset();
            dict_EventMark.Clear();
        }        
        protected override void OnDestroy()
        {
            dict_EventObject.Clear();
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            OnProcessEventsForEnable(toRegister);
        }
        #endregion
        #region 初始化
        /// <summary>
        /// 检测组件 
        /// </summary>
        private void OnParseComponent()
        {
            go_Moon = transform.Find("Animator/View_Right/Clock/Image_Moon").gameObject;
            go_Sun = transform.Find("Animator/View_Right/Clock/Image_Sun").gameObject;
            slider_Time = transform.Find("Animator/View_Right/Clock/Slider_Progress").GetComponent<Slider>();
            text_NextEventTime = transform.Find("Animator/View_Right/Text").GetComponent<Text>();
            rt_Preview = transform.Find("Animator/View_Right/Event_Preview") as RectTransform;
            rt_Current = transform.Find("Animator/View_Right/Event_Current") as RectTransform;
            button_Team = transform.Find("Animator/View_Bottom/Button_Team").GetComponent<Button>();
            button_Entruest = transform.Find("Animator/View_Bottom/Button_Receive").GetComponent<Button>();
            button_GiveUpTask = transform.Find("Animator/View_Bottom/Button_Giveup").GetComponent<Button>();
            text_TaskCount = transform.Find("Animator/View_Bottom/Object_Number/Text_Number").GetComponent<Text>();
            tg_EventMark = transform.Find("Animator/View_Map/Object_Map/View_Island/Grid").GetComponent<ToggleGroup>();
            tr_TipsNode = transform.Find("Animator/View_Map/View_Tips");
            tr_TipsName = transform.Find("Animator/View_Map/View_Tips/View_Tip1");
            tr_TipsContent = transform.Find("Animator/View_Map/View_Tips/View_Tip2");
            tr_Background = transform.Find("Animator/View_Map/View_Tips/Background");
            tr_Background.gameObject.SetActive(false);
            tr_MarkNode = transform.Find("Animator/View_Map/View_Mark/MarkList");
            tr_MarkNode.gameObject.SetActive(false);
            tr_EventItem = transform.Find("Animator/View_Right/Event_Preview/Scroll_Event/Grid/Event");
            tr_EventItem.gameObject.SetActive(false);
            cP_PageDot = transform.Find("Animator/View_Map/View_Bottom").GetComponent<CP_PageDot>();

            gridGroup = transform.Find("Animator/View_Map/Object_Map/View_Island/Grid").gameObject.GetNeedComponent<InfinityGridLayoutGroup>();
            gridGroup.updateChildrenCallback = OnUpdateChildrenCallback;

            uiCenterOnChild = transform.Find("Animator/View_Map/Object_Map/View_Island").gameObject.GetNeedComponent<UICenterOnChild>();
            uiCenterOnChild.onCenter = OnCenter;

            tr_Background.GetComponent<Button>().onClick.AddListener(OnClick_CanncelTips);
            transform.Find("Animator/View_Title09/Btn_Close").GetComponent<Button>().onClick.AddListener(OnClick_Close);
            transform.Find("Animator/View_Right/Button_Tips").GetComponent<Button>().onClick.AddListener(OnClick_PreviewTips);
            transform.Find("Animator/View_Bottom/Object_Number/Button_Tips").GetComponent<Button>().onClick.AddListener(OnClick_CountTips);
            button_Team.onClick.AddListener(OnClick_Team);
            button_Entruest.onClick.AddListener(OnClick_Entruest);
            button_GiveUpTask.onClick.AddListener(OnClick_GiveUpTask);

            toggleSkip = transform.Find("Animator/Toggle").GetComponent<Toggle>();
            toggleSkip.onValueChanged.AddListener(OnClickToggle);
        }
        /// <summary>
        /// 事件注册
        /// </summary>
        /// <param name="toRegister"></param>
        protected void OnProcessEventsForEnable(bool toRegister)
        {
            Sys_Weather.Instance.eventEmitter.Handle(Sys_Weather.EEvents.OnDayNightChange, OnDayNightChange, toRegister);
            Sys_Activity.Instance.eventEmitter.Handle<Sys_Activity.AreaProtectionEvent>(Sys_Activity.EEvents.OnUpdateAreaProtectionEvent, OnUpdateAreaProtectionEvent, toRegister);
        }
        /// <summary>
        /// 设置岛屿数据
        /// </summary>
        private void SetIslandsData()
        {
            list_Island.Clear();
            dict_Event.Clear();

            //Dictionary<uint, CSVIsland.Data> dict_IslandData = CSVIsland.Instance.GetDictData();
            //List<uint> list_IslandDataKeys = new List<uint>(dict_IslandData.Keys);

            var list_IslandDatas = CSVIsland.Instance.GetAll();
            for (int i = 0, count = list_IslandDatas.Count; i < count; i++)
            {
                //uint id = list_IslandDataKeys[i];
                //var item = dict_IslandData[id];

                var item = list_IslandDatas[i];
                if (item.island_type == 2)
                {
                    list_Island.Add(item.id);
                }
            }

            //Dictionary<uint, CSVAreaProtection.Data> dict_AreaProtectionData = CSVAreaProtection.Instance.GetDictData();
            //List<uint> list_AreaProtectionDataKeys = new List<uint>(dict_AreaProtectionData.Keys);

            var list_AreaProtectionDatas = CSVAreaProtection.Instance.GetAll();
            for (int i = 0, count = list_AreaProtectionDatas.Count; i < count; i++)
            {
                //uint id = list_AreaProtectionDataKeys[i];
                //var item = dict_AreaProtectionData[id];

                var item = list_AreaProtectionDatas[i];
                if (dict_Event.ContainsKey(item.map_id))
                {
                    dict_Event[item.map_id].Add(item.id);
                }
                else
                {
                    dict_Event.Add(item.map_id, new List<uint>() { item.id });
                }
            }
        }
        /// <summary>
        /// 设置页数
        /// </summary>
        private void SetPage()
        {
            cP_PageDot.SetMax(list_Island.Count);
            cP_PageDot.Build();
        }
        /// <summary>
        /// 设置事件模版
        /// </summary>
        private void SetEventItemList()
        {
            List<uint> list_EventKeys = new List<uint>(dict_Event.Keys);

            for (int i = 0, icount = list_EventKeys.Count; i < icount; i++)
            {
                var list = dict_Event[list_EventKeys[i]];

                for (int k = 0, kcount = list.Count; k < kcount; k++)
                {
                    var eventId = list[k];
                    var areaProtectionEvent = Sys_Activity.Instance.GetAreaProtectionEvent(eventId);
                    if (null == areaProtectionEvent) continue;

                    GameObject go = GameObject.Instantiate(tr_EventItem.gameObject, tr_EventItem.transform.parent);
                    Toggle toggle = go.GetComponent<Toggle>();
                    dict_EventObject.Add(eventId, toggle);
                    Lib.Core.EventTrigger.Get(go).onClick = OnClick_LockTips;
                    toggle.onValueChanged.AddListener(((bool value) => OnClick_EventTips(toggle, value)));
                    SetEventItem(go.transform, eventId);
                }
            }
        }
        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="path"></param>
        /// <param name="trans"></param>
        /// <param name="id"></param>
        private void LoadAssetAsyn(string path, Transform trans, uint id)
        {
            AsyncOperationHandle<GameObject> gameObjectHandle;
            if (!mAssetRequest.TryGetValue(path, out gameObjectHandle))
            {
                AddressablesUtil.InstantiateAsync(ref gameObjectHandle, path, (AsyncOperationHandle<GameObject> handle) =>
                {
                    GameObject uiGameObject = handle.Result;
                    uiGameObject.name = id.ToString();
                    SetIslandItem(uiGameObject.transform);
                }, true, trans);
                mAssetRequest.Add(path, gameObjectHandle);
            }
            else if (gameObjectHandle.IsValid() && gameObjectHandle.IsDone)
            {
                SetIslandItem(gameObjectHandle.Result.transform);
            }
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        private void RecoveryAsset()
        {
            var assetsTor = mAssetRequest.GetEnumerator();
            while (assetsTor.MoveNext())
            {
                AsyncOperationHandle<GameObject> request = assetsTor.Current.Value;
                AddressablesUtil.Release(ref request, null);
            }
            mAssetRequest.Clear();
        }
        #endregion
        #region 界面显示
        /// <summary>
        /// 刷新界面
        /// </summary>
        private void RefreshView()
        {
            bool isCurrentEvent = curEventId != 0;
            rt_Preview.gameObject.SetActive(!isCurrentEvent);
            rt_Current.gameObject.SetActive(isCurrentEvent);
            button_Team.gameObject.SetActive(!isCurrentEvent);
            button_Entruest.gameObject.SetActive(!isCurrentEvent);
            button_GiveUpTask.gameObject.SetActive(isCurrentEvent);
            gridGroup.SetAmount(list_Island.Count);
            OnDayNightChange();
            SetTaskCount();
            UpdateView();
            SetEventItemSort();
            SetDefaultTipsView();
            SetCurEventView(rt_Current);

            toggleSkip.isOn = Sys_Task.Instance.GetSkipState(50 * 100000u + 0);
        }
        /// <summary>
        /// 设置岛屿模版
        /// </summary>
        /// <param name="tr"></param>
        private void SetIslandItem(Transform tr)
        {
            Transform tr_label = tr.Find("Area_Label");
            tr_label.gameObject.SetActive(false);

            Transform tr_Image = tr.Find("Area_Image");
            for (int i = 0, count = tr_Image.childCount; i < count; i++)
            {
                Transform item = tr_Image.GetChild(i);
                item.transform.GetComponent<Button>().enabled = false;
            }

            GameObject copyNode = GameObject.Instantiate(tr_MarkNode.gameObject, tr);
            copyNode.SetActive(true);
            Transform copyChild = copyNode.transform.GetChild(0);
            copyChild.gameObject.SetActive(false);

            uint id = 0;
            uint.TryParse(tr.gameObject.name, out id);
            List<uint> list = null;
            dict_Event.TryGetValue(id, out list);
            if (null == list) return;

            for (int i = 0, count = list.Count; i < count; i++)
            {
                var eventId = list[i];
                var areaProtectionEvent = Sys_Activity.Instance.GetAreaProtectionEvent(eventId);
                if (null == areaProtectionEvent) continue;

                GameObject go = GameObject.Instantiate(copyChild.gameObject, copyChild.transform.parent);
                Toggle toggle = go.GetComponent<Toggle>();
                toggle.group = tg_EventMark;
                Lib.Core.EventTrigger.Get(go).onClick = OnClick_LockTips;
                toggle.onValueChanged.AddListener(((bool value) => OnClick_MarkTips(toggle, value)));
                dict_EventMark.Add(eventId, toggle);
                SetMarkItem(go.transform, eventId);
                SetMarkItemState(toggle, areaProtectionEvent.isShow, !areaProtectionEvent.isOpen);
            }
        }
        /// <summary>
        /// 设置标记模版
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="eventId"></param>
        private void SetMarkItem(Transform tr, uint eventId)
        {
            CSVAreaProtection.Data cSVAreaProtectionData = CSVAreaProtection.Instance.GetConfData(eventId);
            if (null == cSVAreaProtectionData)
            {
                tr.gameObject.SetActive(false);
                return;
            }
            tr.gameObject.SetActive(true);
            tr.name = eventId.ToString();

            Vector3 vector3 = cSVAreaProtectionData.event_position?.Count >= 3 ?
            new Vector3(cSVAreaProtectionData.event_position[0], cSVAreaProtectionData.event_position[1], cSVAreaProtectionData.event_position[2]) : Vector3.zero;

            (tr as RectTransform).anchoredPosition3D = vector3;
        }
        /// <summary>
        /// 设置事件模版
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="eventId"></param>
        private void SetEventItem(Transform tr, uint eventId)
        {
            CSVAreaProtection.Data cSVAreaProtectionData = CSVAreaProtection.Instance.GetConfData(eventId);
            if (null == cSVAreaProtectionData)
            {
                tr.gameObject.SetActive(false);
                return;
            }
            tr.gameObject.SetActive(true);
            tr.name = eventId.ToString();
            /// <summary> 事件名字 </summary>
            Text text_EventName = tr.Find("Text").GetComponent<Text>();
            text_EventName.text = LanguageHelper.GetTextContent(cSVAreaProtectionData.eventName_id);
        }
        /// <summary>
        /// 设置提示界面
        /// </summary>
        /// <param name="isShow"></param>
        /// <param name="eventId"></param>
        private void SetTipsView(bool isShow, uint eventId = 0)
        {
            if (isShow)
            {
                CSVAreaProtection.Data cSVAreaProtectionData = CSVAreaProtection.Instance.GetConfData(eventId);
                if (null == cSVAreaProtectionData) return;

                /// <summary> 事件名字 </summary>
                Text text_EventName = tr_TipsName.Find("Text").GetComponent<Text>();
                text_EventName.text = LanguageHelper.GetTextContent(cSVAreaProtectionData.eventName_id);
                /// <summary> 事件内容 </summary>
                Text text_Content = tr_TipsContent.Find("Text_Introduce").GetComponent<Text>();
                text_Content.text = LanguageHelper.GetTextContent(cSVAreaProtectionData.eventDescription_id);
                /// <summary> 事件委托 </summary>
                Text text_Entrust = tr_TipsContent.Find("Text_Introduce/Text_Content/Text").GetComponent<Text>();
                text_Entrust.text = LanguageHelper.GetTextContent(cSVAreaProtectionData.entrustDescription_id);

                Vector3 off_Name = cSVAreaProtectionData.eventName_position?.Count >= 3 ?
                    new Vector3(cSVAreaProtectionData.eventName_position[0],
                    cSVAreaProtectionData.eventName_position[1],
                    cSVAreaProtectionData.eventName_position[2]) : Vector3.zero;
                Vector3 off_Content = cSVAreaProtectionData.eventDescription_position?.Count >= 3 ?
                    new Vector3(cSVAreaProtectionData.eventDescription_position[0],
                    cSVAreaProtectionData.eventDescription_position[1],
                    cSVAreaProtectionData.eventDescription_position[2]) : Vector3.zero;
                (tr_TipsName as RectTransform).anchoredPosition3D = off_Name;
                (tr_TipsContent as RectTransform).anchoredPosition3D = off_Content;

                bool isReverse = System.Convert.ToBoolean(cSVAreaProtectionData.eventName_direction);
                Vector3 localScale = new Vector3(isReverse ? -1f : 1f, 1f, 1f);
                tr_TipsName.localScale = localScale;
                text_EventName.transform.localScale = localScale;

                Animator animator_TipsName = tr_TipsName.GetComponent<Animator>();
                animator_TipsName.enabled = true;
                animator_TipsName.Play("Open");
                Animator animator_TipsContent = tr_TipsContent.GetComponent<Animator>();
                animator_TipsContent.enabled = true;
                animator_TipsContent.Play("Open");
                tr_Background.gameObject.SetActive(true);
            }
            else
            {
                Animator animator_TipsName = tr_TipsName.GetComponent<Animator>();
                animator_TipsName.enabled = true;
                animator_TipsName.Play("Close");
                Animator animator_TipsContent = tr_TipsContent.GetComponent<Animator>();
                animator_TipsContent.enabled = true;
                animator_TipsContent.Play("Close");
                tr_Background.gameObject.SetActive(false);
            }
        }
        /// <summary>
        /// 设置默认提示界面
        /// </summary>
        private void SetDefaultTipsView()
        {
            tr_TipsName.GetComponent<Animator>().enabled = false;
            tr_TipsName.GetComponent<CanvasGroup>().alpha = 0;
            tr_TipsContent.GetComponent<Animator>().enabled = false;
            tr_TipsContent.GetComponent<CanvasGroup>().alpha = 0;
        }
        /// <summary>
        /// 设置当前事件界面
        /// </summary>
        /// <param name="tr"></param>
        private void SetCurEventView(Transform tr)
        {
            CSVAreaProtection.Data cSVAreaProtectionData = CSVAreaProtection.Instance.GetConfData(curEventId);
            if (null == cSVAreaProtectionData)
                return;

            /// <summary> 名字 </summary>
            Text text_Name = tr.Find("Text_Title").GetComponent<Text>();
            text_Name.text = LanguageHelper.GetTextContent(cSVAreaProtectionData.eventName_id);
            /// <summary> 内容 </summary>
            Text text_Content = tr.Find("Scroll_Event/Text").GetComponent<Text>();
            text_Content.text = LanguageHelper.GetTextContent(cSVAreaProtectionData.eventDescription_id);
        }
        /// <summary>
        /// 设置事件模版状态
        /// </summary>
        /// <param name="eventTime"></param>
        public void SetEventItemState(Sys_Activity.AreaProtectionEvent eventTime)
        {
            uint eventId = eventTime.eventId;
            Toggle toggle = null;
            if (dict_EventObject.TryGetValue(eventId, out toggle))
            {
                SetEventItemState(toggle, eventTime.isShow, !eventTime.isOpen);
            }
        }
        /// <summary>
        /// 设置事件模版状态
        /// </summary>
        /// <param name="toggle"></param>
        /// <param name="isShow"></param>
        /// <param name="isGray"></param>
        private void SetEventItemState(Toggle toggle, bool isShow, bool isGray)
        {
            toggle.gameObject.SetActive(isShow);
            if (isShow)
            {
                ImageHelper.SetImageGray(toggle, isGray, true);
                toggle.interactable = !isGray;
            }
        }
        /// <summary>
        /// 设置事件模版排序
        /// </summary>
        private void SetEventItemSort()
        {
            int index = 0;
            List<uint> list_EventObjectKeys = new List<uint>(dict_EventObject.Keys);
            for (int i = 0, count = list_EventObjectKeys.Count; i < count; i++)
            {
                uint id = list_EventObjectKeys[i];
                var item = dict_EventObject[id];

                var areaProtectionEvent = Sys_Activity.Instance.GetAreaProtectionEvent(id);
                if (areaProtectionEvent.isOpen)
                {
                    index++;
                    item.transform.SetSiblingIndex(index);
                    SetEventItemState(item, areaProtectionEvent.isShow, !areaProtectionEvent.isOpen);
                }
                else
                {
                    item.transform.SetAsLastSibling();
                    SetEventItemState(item, areaProtectionEvent.isShow, !areaProtectionEvent.isOpen);
                }
            }
        }
        /// <summary>
        /// 设置是否关闭界面
        /// </summary>
        /// <param name="areaProtectionEvent"></param>
        private void SetCloseTipsView(Sys_Activity.AreaProtectionEvent areaProtectionEvent)
        {
            if (!areaProtectionEvent.isShow)
            {
                Toggle toggle_mark = null;
                if (dict_EventMark.TryGetValue(areaProtectionEvent.eventId, out toggle_mark))
                {
                    if (toggle_mark.isOn)
                    {
                        OnClick_CanncelTips();
                    }
                }
            }
        }
        /// <summary>
        /// 设置标记模版状态
        /// </summary>
        /// <param name="eventTime"></param>
        public void SetMarkItemState(Sys_Activity.AreaProtectionEvent eventTime)
        {
            uint eventId = eventTime.eventId;
            Toggle toggle = null;
            if (dict_EventMark.TryGetValue(eventId, out toggle))
            {
                SetMarkItemState(toggle, eventTime.isShow, !eventTime.isOpen);
            }
        }
        /// <summary>
        /// 设置标记模版状态
        /// </summary>
        /// <param name="toggle"></param>
        /// <param name="isShow"></param>
        /// <param name="isGray"></param>
        private void SetMarkItemState(Toggle toggle, bool isShow, bool isGray)
        {
            toggle.gameObject.SetActive(isShow);
            if (isShow)
            {
                ImageHelper.SetImageGray(toggle, isGray, true);
                toggle.interactable = !isGray;
            }
        }
        /// <summary>
        /// 设置任务次数
        /// </summary>
        private void SetTaskCount()
        {
            CSVAreaProtectionParameters.Data cSVAreaProtectionParametersData = CSVAreaProtectionParameters.Instance.GetConfData(1);
            Sys_Activity.Instance.CheckAreaProtectionExpireTime();
            var rewardLimit = Sys_Activity.Instance.ringTaskData.rewardLimit;
            text_TaskCount.text = (cSVAreaProtectionParametersData.rewardTimes - rewardLimit.UsedTimes).ToString();
        }
        /// <summary>
        /// Update中持续更新的界面
        /// </summary>
        private void UpdateView()
        {
            uint time = Sys_Time.Instance.GetServerTime();
            slider_Time.value = (float)(time - starttime) % (onedaytime * 2) / (onedaytime * 2);
            int nextTime = Sys_Activity.Instance.nextEventTime;
            text_NextEventTime.text = LanguageHelper.GetTextContent(3830000000, (nextTime / 60 + 1).ToString("D2"));
        }
        #endregion
        #region 响应事件
        /// <summary>
        /// 关闭按钮
        /// </summary>
        private void OnClick_Close()
        {
            CloseSelf();
        }
        /// <summary>
        /// 便捷组队
        /// </summary>
        private void OnClick_Team()
        {
            if (Sys_Team.Instance.IsFastOpen(true))
                Sys_Team.Instance.OpenFastUI((uint)60);
        }
        /// <summary>
        /// 领取委托
        /// </summary>
        private void OnClick_Entruest()
        {
            Sys_Activity.Instance.AcceptRingTaskReq();
        }
        /// <summary>
        /// 放弃任务
        /// </summary>
        private void OnClick_GiveUpTask()
        {
            Sys_Activity.Instance.GiveUpRingTaskReq();
        }
        /// <summary>
        /// 预览提示
        /// </summary>
        private void OnClick_PreviewTips()
        {
            UIManager.OpenUI(EUIID.UI_Rule, false, new UIRuleParam { StrContent = LanguageHelper.GetTextContent(3830000001) });
        }
        /// <summary>
        /// 数量提示
        /// </summary>
        private void OnClick_CountTips()
        {
            UIManager.OpenUI(EUIID.UI_Rule, false, new UIRuleParam { StrContent = LanguageHelper.GetTextContent(3830000002) });
        }
        /// <summary>
        /// 事件提示
        /// </summary>
        private void OnClick_EventTips(Toggle toggle, bool value)
        {
            uint eventId = 0;
            uint.TryParse(toggle.name, out eventId);
            //界面标记
            Toggle toggle_mark = null;
            if (dict_EventMark.TryGetValue(eventId, out toggle_mark))
            {
                if (toggle_mark.isOn != value)
                    toggle_mark.isOn = value;
            }
        }
        /// <summary>
        /// 标记提示
        /// </summary>
        /// <param name="toggle"></param>
        /// <param name="value"></param>
        private void OnClick_MarkTips(Toggle toggle, bool value)
        {
            uint eventId = 0;
            uint.TryParse(toggle.name, out eventId);

            Toggle toggle_event = null;
            if (dict_EventObject.TryGetValue(eventId, out toggle_event))
            {
                if (toggle_event.isOn != value)
                    toggle_event.isOn = value;
            }

            //提示界面
            if (value)
            {
                CSVAreaProtection.Data cSVAreaProtectionData = CSVAreaProtection.Instance.GetConfData(eventId);
                if (null != cSVAreaProtectionData)
                {
                    int index = list_Island.IndexOf(cSVAreaProtectionData.map_id);
                    uiCenterOnChild.SetCurrentPageIndex(index, false);
                    SetTipsView(true, eventId);
                }
                else
                {
                    SetTipsView(false);
                }
            }
            else
            {
                SetTipsView(false);
            }
        }
        /// <summary>
        /// 锁定提示
        /// </summary>
        /// <param name="go"></param>
        private void OnClick_LockTips(GameObject go)
        {
            Toggle toggle = go.GetComponent<Toggle>();
            if (null == toggle || toggle.interactable)
                return;

            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3830000007));
        }
        /// <summary>
        /// 取消提示
        /// </summary>
        private void OnClick_CanncelTips()
        {
            tg_EventMark.SetAllTogglesOff();
        }
        /// <summary>
        /// 昼夜改变
        /// </summary>
        private void OnDayNightChange()
        {
            bool IsDay = Sys_Weather.Instance.isDay;
            go_Sun.SetActive(IsDay);
            go_Moon.SetActive(!IsDay);
        }
        /// <summary>
        /// 更新地域防范事件
        /// </summary>
        private void OnUpdateAreaProtectionEvent(Sys_Activity.AreaProtectionEvent areaProtectionEvent)
        {
            SetCloseTipsView(areaProtectionEvent);
            SetEventItemState(areaProtectionEvent);
            SetMarkItemState(areaProtectionEvent);
            SetEventItemSort();
        }
        /// <summary>
        /// 更新滚动子节点
        /// </summary>
        /// <param name="index"></param>
        /// <param name="trans"></param>
        private void OnUpdateChildrenCallback(int index, Transform trans)
        {
            if (index < 0 || index >= list_Island.Count) return;
            trans.name = index.ToString();
            uint id = list_Island[index];
            CSVIsland.Data cSVIslandData = CSVIsland.Instance.GetConfData(id);
            if (null == cSVIslandData) return;

            LoadAssetAsyn(cSVIslandData.map_ui, trans, cSVIslandData.id);

            uint tempId = 0;
            CSVAreaProtection.Data temp = CSVAreaProtection.Instance.GetConfData(curEventId);
            if (temp != null)
                tempId = temp.map_id;
            
            if ((index == 0 && curEventId == 0) || tempId == id)
            {
                uiCenterOnChild.SetCurrentPageIndex(index, false);
            }
        }
        /// <summary>
        /// 居中事件
        /// </summary>
        /// <param name="go"></param>
        private void OnCenter(GameObject go)
        {
            int index = 0;
            int.TryParse(go.name, out index);
            if (index < 0) return;
            cP_PageDot.SetSelected(index);
        }
        #endregion
        #region 提供功能
        #endregion

        private void OnClickToggle(bool isOn)
        {
            Sys_Task.Instance.OnChageSkipReq(50 * 100000 + 0, isOn);
        }
    }
}


