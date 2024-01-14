using Logic.Core;
using System.Collections.Generic;
using Table;
using UnityEngine.UI;
using UnityEngine;
using Lib.Core;
using UnityEngine.ResourceManagement.AsyncOperations;
using System;
using Packet;

namespace Logic
{
    /// <summary> 挂机打怪 </summary>
    public class UI_HangupFight : UIBase
    {
        #region 界面组件
        /// <summary> 卡时 </summary>
        private Text text_WorkingHour;
        /// <summary> 卡时状态 </summary>
        private Text text_WorkingHourState;
        /// <summary> 疲劳度 </summary>
        private Text text_Fatigue;
        /// <summary> 标题 </summary>
        private Text text_Title;
        /// <summary> 推荐等级 </summary>
        private Text text_RecommendationLevel;
        /// <summary> 说明 </summary>
        private Text text_Explain;
        /// <summary> 工作状态 </summary>
        private Text text_WorkState;
        /// <summary> 标记模版 </summary>
        private Transform tr_MarkNode;
        /// <summary> 事件模版 </summary>
        private Transform tr_EventItem;
        /// <summary> 岛屿节点模版 </summary>
        private GameObject go_IslandNodeItem;
        private Transform go_IslandNode;
        /// <summary> 事件标记字典 </summary>
        private Dictionary<uint, Toggle> dict_EventMark = new Dictionary<uint, Toggle>();
        /// <summary> 事件标记组字段 </summary>
        private Dictionary<uint, ToggleGroup> dict_EventGroup = new Dictionary<uint, ToggleGroup>();
        /// <summary> 事件挂机层 </summary>
        private List<Toggle> list_EventLayer = new List<Toggle>();
        private CP_LRArrowSwitch arrowSwitcher;
        
        public Button btnGoto;
        public Button btnStopGoto;
        public Button btnCatchPet;
        #endregion
        #region 数据定义
        /// <summary> 当前岛屿编号 </summary>
        private uint curIsland { get; set; } = 0;
        /// <summary> 当前事件编号 </summary>
        private uint curEventId { get; set; } = 0;
        /// <summary> 当前挂机层编号 </summary>
        private uint curLayerId { get; set; } = 0;
        /// <summary> 岛屿列表 </summary>
        private List<uint> list_Island = new List<uint>();
        /// <summary> 岛屿对应区域事件 </summary>
        private Dictionary<uint, List<uint>> dict_Event = new Dictionary<uint, List<uint>>();
        /// <summary> 区域事件对应层数 </summary>
        private Dictionary<uint, List<uint>> dict_Layer = new Dictionary<uint, List<uint>>();
        /// <summary> 资源列表 </summary>
        private Dictionary<string, AsyncOperationHandle<GameObject>> mAssetRequest = new Dictionary<string, AsyncOperationHandle<GameObject>>();
        #endregion
        #region 系统函数
        protected override void OnInit()
        {
            SetIslandsData();
        }
        protected override void OnLoaded()
        {
            OnParseComponent();
        }
        protected override void OnOpen(object arg)
        {
            curIsland = GetDefaultIslandId();
            if (curIsland == 0)
            {
                CSVMapInfo.Data cSVMapInfoData = CSVMapInfo.Instance.GetConfData(Sys_Map.Instance.CurMapId);
                if (null != cSVMapInfoData && null != cSVMapInfoData.map_node && cSVMapInfoData.map_node.Count >= 2)
                {
                    curIsland = (uint)cSVMapInfoData.map_node[1];
                }
            }
        }
        protected override void OnShow()
        {
            RefreshView();
        }
        protected override void OnHide()
        {
            RecoveryAsset();
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
            this.arrowSwitcher = this.transform.Find("Animator/View_Map/Object_Map/View_Island/Arrows").GetComponent<CP_LRArrowSwitch>();
            this.arrowSwitcher.onExec += OnArrowSwicth;
            
            text_WorkingHour = transform.Find("Animator/VIew_TopLeft/Image_Top/Text_Num").GetComponent<Text>();
            text_WorkingHourState = transform.Find("Animator/VIew_TopLeft/Image_Bottom/Text").GetComponent<Text>();
            text_Fatigue = transform.Find("Animator/View_Right/Tired/Text_Tired_Num").GetComponent<Text>();
            text_Title = transform.Find("Animator/View_Right/UP/Image_Title/Text_Title").GetComponent<Text>();
            text_RecommendationLevel = transform.Find("Animator/View_Right/UP/Image_Bottom/Text_Lv").GetComponent<Text>();
            text_Explain = transform.Find("Animator/View_Right/UP/Image_Bottom/Text_Des").GetComponent<Text>();
            text_WorkState = transform.Find("Animator/View_Bottom/Btn_Start/Text_01").GetComponent<Text>();

            tr_MarkNode = transform.Find("Animator/View_Map/View_Mark/MarkList");
            tr_MarkNode.gameObject.SetActive(false);
            tr_EventItem = transform.Find("Animator/View_Right/Down/Scroll_View/Grid/Event");
            tr_EventItem.gameObject.SetActive(false);

            go_IslandNodeItem = transform.Find("Animator/View_Map/Object_Map/View_Island/Grid/Item").gameObject;
            go_IslandNode = transform.Find("Animator/View_Map/Object_Map/View_Island/Grid");
            
            transform.Find("Animator/VIew_TopLeft/Image_Top/Button").GetComponent<Button>().onClick.AddListener(OnClick_CheckWorkingHourTips);
            transform.Find("Animator/View_Right/Tired/Button_Tips").GetComponent<Button>().onClick.AddListener(OnClick_CheckFatigueTips);
            transform.Find("Animator/View_Bottom/Btn_Start").GetComponent<Button>().onClick.AddListener(OnClick_Work);
            transform.Find("Animator/View_Bottom/Button_Team").GetComponent<Button>().onClick.AddListener(OnClick_Team);
            transform.Find("Animator/View_Bottom/Button_Setting").GetComponent<Button>().onClick.AddListener(OnClick_Setting);
            transform.Find("Animator/View_Title09/Btn_Close").GetComponent<Button>().onClick.AddListener(OnClick_Close);
            
            btnGoto = transform.Find("Animator/View_Bottom/Button_Receive").GetComponent<Button>();
            btnGoto.onClick.AddListener(OnClick_GotoHangup);
            
            btnStopGoto = transform.Find("Animator/View_Bottom/Button_Stop").GetComponent<Button>();
            btnStopGoto.onClick.AddListener(OnClick_StopGoto);
            
            btnCatchPet = transform.Find("Animator/View_Bottom/Button_Pet").GetComponent<Button>();
            btnCatchPet.onClick.AddListener(OnClick_CatchPet);
        }
        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="toRegister"></param>
        private void OnProcessEventsForEnable(bool toRegister)
        {
            Sys_Hangup.Instance.eventEmitter.Handle(Sys_Hangup.EEvents.OnWorkingHourSwitch, OnWorkingHourSwitch, toRegister);
            Sys_Hangup.Instance.eventEmitter.Handle(Sys_Hangup.EEvents.OnWorkingHourPoint, OnOnWorkingHourPoint, toRegister);
            Sys_Pet.Instance.eventEmitter.Handle<int, int>(Sys_Pet.EEvents.OnPatrolStateChange, OnPatrolStateChange, toRegister);
            // Sys_Hangup.Instance.eventEmitter.Handle(Sys_Hangup.EEvents.OnHangupExit, OnHangupExit, toRegister);
        }
        public void OnArrowSwicth(int index, uint id) {
            this.Refresh(id, index);
        }
        /// <summary>
        /// 设置挂机层模版
        /// </summary>
        /// <param name="count"></param>
        private void SetLayerItemList(int count)
        {
            while (count > list_EventLayer.Count)
            {
                GameObject go = GameObject.Instantiate(tr_EventItem.gameObject, tr_EventItem.transform.parent);
                Toggle toggle = go.GetComponent<Toggle>();
                Button button = go.transform.Find("Button").GetComponent<Button>();
                button.onClick.AddListener(() => { OnClick_CheckLayerTips(go); });
                toggle.onValueChanged.AddListener((bool value) => OnClick_EventLayer(toggle, value));
                list_EventLayer.Add(toggle);
            }
        }
        private void SetFatigueView()
        {
            uint level = Sys_Role.Instance.Role.Level;
            CSVCharacterAttribute.Data cSVCharacterAttributeData = CSVCharacterAttribute.Instance.GetConfData(level);
            if (null == cSVCharacterAttributeData)
            {
                text_Fatigue.text = string.Empty;
                return;
            }
            long maxExp = (long)cSVCharacterAttributeData.DailyHangupTotalExp;
            CmdHangUpDataNtf cmdHangUpDataNtf = Sys_Hangup.Instance.cmdHangUpDataNtf;
            bool isRestTime = !Sys_Time.IsServerSameDay5(Sys_Time.Instance.GetServerTime(), cmdHangUpDataNtf.RestExpTime);
            long curExp = isRestTime ? 0 : cmdHangUpDataNtf.RestExp;
            float value = maxExp == 0 ? 0 : (float)((double)curExp / (double)maxExp);
            uint uintValue = (uint)(value * 100);
            if (uintValue >= 100)
                uintValue = 100;
            text_Fatigue.text = string.Format("{0}%", uintValue);
            
            if (value >= 1f)
            {
                text_Fatigue.color = Color.red;
            }
            else if (value >= 0.5f)
            {
                text_Fatigue.color = new Color(1f, 139f/255f, 0f, 1f);
            }
            else
            {
                text_Fatigue.color = new Color(139f/255f, 90f/255f, 107f/255f, 1f);
            }
        }
        /// <summary>
        /// 创建足够的列表
        /// </summary>
        /// <param name="node"></param>
        /// <param name="child"></param>
        /// <param name="number"></param>
        public static void CreateItemList(Transform node, Transform child, int number)
        {
            while (node.childCount < number)
            {
                GameObject.Instantiate(child, node);
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
        #region 数据处理
        /// <summary>
        /// 设置岛屿数据
        /// </summary>
        private void SetIslandsData()
        {
            list_Island.Clear();
            var list_IslandDatas = CSVIsland.Instance.GetAll();
            for (int i = 0, count = list_IslandDatas.Count; i < count; i++)
            {
                var item = list_IslandDatas[i];
                if (item.island_type == 2 && Sys_Hangup.Instance.IsUnlockIsland(item.id))
                {
                    list_Island.Add(item.id);
                }
            }

            dict_Event.Clear();
            var list_HangupDatas = CSVHangup.Instance.GetAll();
            for (int i = 0, count = list_HangupDatas.Count; i < count; i++)
            {
                var item = list_HangupDatas[i];
                if (dict_Event.ContainsKey(item.IslandID))
                {
                    dict_Event[item.IslandID].Add(item.id);
                }
                else
                {
                    dict_Event.Add(item.IslandID, new List<uint>() { item.id });
                }
            }

            dict_Layer.Clear();
            var list_HangupLayerStageDatas = CSVHangupLayerStage.Instance.GetAll();
            for (int i = 0, count = list_HangupLayerStageDatas.Count; i < count; i++)
            {
                var item = list_HangupLayerStageDatas[i];
                if (dict_Layer.ContainsKey(item.Hangupid))
                {
                    dict_Layer[item.Hangupid].Add(item.id);
                }
                else
                {
                    dict_Layer.Add(item.Hangupid, new List<uint>() { item.id });
                }
            }
        }
        /// <summary>
        /// 获得默认岛屿编号
        /// </summary>
        /// <returns></returns>
        private uint GetDefaultIslandId()
        {
            uint defaultId = 0;
            uint defaultWeightValue = 0;

            List<uint> keys = new List<uint>(dict_Event.Keys);
            for (int i = 0; i < keys.Count; i++)
            {
                uint key = keys[i];
                var events = dict_Event[key];

                uint eventId = GetDefaultIslandEventId(events);
                bool isUnlockHangupPoint = Sys_Hangup.Instance.IsUnlockHangupPoint(eventId);
                bool isRecommendationLevelHangupPoint = Sys_Hangup.Instance.IsRecommendationLevelHangupPoint(eventId);

                uint weightValue = 0;
                if (isUnlockHangupPoint) //解锁
                {
                    if (isRecommendationLevelHangupPoint) //推荐等级
                    {
                        weightValue = 2;
                    }
                    else //低等级
                    {
                        weightValue = 1;
                    }
                }
                else //未解锁
                {
                    weightValue = 0;
                }

                if (defaultId == 0) //未赋值
                {
                    defaultId = key;
                    defaultWeightValue = weightValue;
                }
                else //已赋值
                {
                    if (weightValue > defaultWeightValue) //当前权重值高
                    {
                        defaultId = key;
                        defaultWeightValue = weightValue;
                    }
                    else if (weightValue == defaultWeightValue && weightValue != 0) //当前权重值一样且解锁
                    {
                        defaultId = key;
                        defaultWeightValue = weightValue;
                    }
                }
            }
            return defaultId;
        }
        /// <summary>
        /// 得到默认岛屿事件编号
        /// </summary>
        /// <param name="events"></param>
        /// <returns></returns>
        private uint GetDefaultIslandEventId(List<uint> events)
        {
            uint defaultId = 0;
            uint defaultWeightValue = 0;

            for (int i = 0; i < events.Count; i++)
            {
                uint eventId = events[i];
                bool isUnlockHangupPoint = Sys_Hangup.Instance.IsUnlockHangupPoint(eventId);
                bool isRecommendationLevelHangupPoint = Sys_Hangup.Instance.IsRecommendationLevelHangupPoint(eventId);

                uint weightValue = 0;
                if (isUnlockHangupPoint) //解锁
                {
                    if (isRecommendationLevelHangupPoint) //推荐等级
                    {
                        weightValue = 2;
                    }
                    else //低等级
                    {
                        weightValue = 1;
                    }
                }
                else //未解锁
                {
                    weightValue = 0;
                }

                if (defaultId == 0) //未赋值
                {
                    defaultId = eventId;
                    defaultWeightValue = weightValue;
                }
                else //已赋值
                {
                    if (weightValue > defaultWeightValue) //当前权重值高
                    {
                        defaultId = eventId;
                        defaultWeightValue = weightValue;
                    }
                    else if (weightValue == defaultWeightValue && weightValue != 0) //当前权重值一样且解锁
                    {
                        defaultId = eventId;
                        defaultWeightValue = weightValue;
                    }
                }
            }
            return defaultId;
        }
        /// <summary>
        /// 得到默认事件层ID
        /// </summary>
        /// <param name="layers"></param>
        /// <returns></returns>
        private uint GetDefaultEventLayerId(List<uint> layers)
        {
            uint defaultId = 0;
            uint defaultWeightValue = 0;

            for (int i = 0; i < layers.Count; i++)
            {
                uint layerId = layers[i];
                bool isUnlockHangupLayer = Sys_Hangup.Instance.IsUnlockHangupLayer(layerId);
                bool isRecommendationLevelHangupLayer = Sys_Hangup.Instance.IsRecommendationLevelHangupLayer(layerId);

                uint weightValue = 0;
                if (isUnlockHangupLayer) //解锁
                {
                    if (isRecommendationLevelHangupLayer) //推荐等级
                    {
                        weightValue = 2;
                    }
                    else //低等级
                    {
                        weightValue = 1;
                    }
                }
                else //未解锁
                {
                    weightValue = 0;
                }

                if (defaultId == 0) //未赋值
                {
                    defaultId = layerId;
                    defaultWeightValue = weightValue;
                }
                else //已赋值
                {
                    if (weightValue > defaultWeightValue) //当前权重值高
                    {
                        defaultId = layerId;
                        defaultWeightValue = weightValue;
                    }
                    else if (weightValue == defaultWeightValue && weightValue != 0) //当前权重值一样且解锁
                    {
                        defaultId = layerId;
                        defaultWeightValue = weightValue;
                    }
                }
            }
            return defaultId;
        }
        #endregion
        #region 界面显示
        /// <summary>
        /// 刷新界面
        /// </summary>
        private void RefreshView()
        {
            SetWorkingHourView();
            SetFatigueView();
            SetIslandView();

            RefreshHangupBtns();
        }

        private void RefreshHangupBtns() {
            btnGoto.gameObject.SetActive(Sys_Pet.Instance.clientStateId != Sys_Role.EClientState.Hangup);
            btnStopGoto.gameObject.SetActive(Sys_Pet.Instance.clientStateId == Sys_Role.EClientState.Hangup);
        }

        /// <summary>
        /// 设置时卡界面
        /// </summary>
        private void SetWorkingHourView()
        {
            var cmdHangUpDataNtf = Sys_Hangup.Instance.cmdHangUpDataNtf;
            text_WorkingHour.text = cmdHangUpDataNtf.WorkingHourPoint.ToString();
            text_WorkingHourState.text = LanguageHelper.GetTextContent(cmdHangUpDataNtf.WorkingHourOpened ? (uint)2104012 : 2104013);
            text_WorkState.text = LanguageHelper.GetTextContent(cmdHangUpDataNtf.WorkingHourOpened ? (uint)2104018 : 2104017);
        }
        /// <summary>
        /// 设置岛屿界面
        /// </summary>
        private void SetIslandView()
        {
            dict_EventGroup.Clear();
            dict_EventMark.Clear();
            CreateItemList(this.go_IslandNode, go_IslandNodeItem.transform, list_Island.Count);

            this.arrowSwitcher.SetData(this.list_Island);
            int index = this.list_Island.IndexOf(curIsland);
            if (index < 0 || index >= list_Island.Count) {
                return;
            }
            
            this.arrowSwitcher.SetCurrentIndex(index);
            this.arrowSwitcher.Exec();
            
            // 默认地图的一个右侧显示
            List<uint> list = dict_Event[curIsland];
            uint selectEvent = GetDefaultIslandEventId(list);
            curEventId = selectEvent;
            SetSelectView();
        }
        /// <summary>
        /// 设置岛屿模版
        /// </summary>
        /// <param name="tr"></param>
        private void SetIslandItem(Transform tr)
        {
            uint id = 0;
            if (!uint.TryParse(tr.gameObject.name, out id))
            {
                tr.gameObject.SetActive(false);
                return;
            }

            // 已经刷新过，就不进行刷新
            if (this.dict_EventGroup.ContainsKey(id)) {
                return;
            }

            tr.gameObject.SetActive(true);
            /// <summary> 地图区域内容 </summary>
            Transform tr_label = tr.Find("Area_Label");
            tr_label.gameObject.SetActive(false);
            /// <summary> 地图区域图片 </summary>
            Transform tr_Image = tr.Find("Area_Image");
            for (int i = 0, count = tr_Image.childCount; i < count; i++)
            {
                Transform item = tr_Image.GetChild(i);
                item.transform.GetComponent<Button>().enabled = false;
                uint mapId = 0;
                uint.TryParse(item.name, out mapId);
                /// <summary> 地图是否解锁 </summary>
                bool isUnlockMap = Sys_Hangup.Instance.IsUnlockMap(mapId);
                ImageHelper.SetImageGray(item, !isUnlockMap, true);
            }

            /// <summary> 地图标记节点 </summary>
            GameObject copyNode = GameObject.Instantiate(tr_MarkNode.gameObject, tr);
            copyNode.SetActive(true);
            ToggleGroup toggleGroup = copyNode.GetComponent<ToggleGroup>();
            dict_EventGroup.Add(id, toggleGroup);
            Transform copyChild = copyNode.transform.GetChild(0);
            copyChild.gameObject.SetActive(false);

            List<uint> list = null;
            if (!dict_Event.TryGetValue(id, out list))
                return;
            uint selectEvent = GetDefaultIslandEventId(list);
            /// <summary> 地图各个标记 </summary>
            for (int i = 0, count = list.Count; i < count; i++)
            {
                var eventId = list[i];
                GameObject go = GameObject.Instantiate(copyChild.gameObject, copyChild.transform.parent);
                Toggle toggle = go.GetComponent<Toggle>();
                Lib.Core.EventTrigger.Get(go).onClick = OnClick_LockTips;
                toggle.onValueChanged.AddListener(((bool value) => OnClick_MarkTips(toggle, value)));
                dict_EventMark.Add(eventId, toggle);
                SetMarkItem(go.transform, eventId);

                if (curIsland == id && toggle.enabled)
                {
                    bool isOn = selectEvent == eventId ? true : false;
                    if (toggle.isOn != isOn)
                    {
                        toggle.isOn = isOn;
                    }
                    else
                    {
                        toggle.onValueChanged.Invoke(isOn);
                    }
                }
            }
        }
        /// <summary>
        /// 设置标记模版
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="eventId"></param>
        private void SetMarkItem(Transform tr, uint eventId)
        {
            CSVHangup.Data cSVHangupData = CSVHangup.Instance.GetConfData(eventId);
            if (null == cSVHangupData)
            {
                tr.gameObject.SetActive(false);
                return;
            }
            tr.gameObject.SetActive(true);
            tr.name = eventId.ToString();
            /// <summary> 是否解锁地图 </summary>
            bool isUnlockMap = Sys_Hangup.Instance.IsUnlockMap(cSVHangupData.MapID);
            /// <summary> 是否解锁挂机点 </summary>
            bool isUnlockHangupPoint = Sys_Hangup.Instance.IsUnlockHangupPoint(cSVHangupData.id);
            /// <summary> 坐标 </summary>
            RectTransform rt = tr.GetComponent<RectTransform>();
            Vector3 vector3 = cSVHangupData.UIPosition?.Count >= 2 ?
                new Vector3(cSVHangupData.UIPosition[0], cSVHangupData.UIPosition[1], 0) : Vector3.zero;
            rt.anchoredPosition3D = vector3;
            /// <summary> 标题名 </summary>
            GameObject go_Name = tr.Find("Image_Name").gameObject;
            go_Name.SetActive(isUnlockMap);
            Text Text_Name = tr.Find("Image_Name/Text_Name").GetComponent<Text>();
            Text_Name.text = LanguageHelper.GetTextContent(cSVHangupData.HangupName);
            Text Text_Level = tr.Find("Text").GetComponent<Text>();
            Text_Level.gameObject.SetActive(isUnlockMap);
            if (isUnlockHangupPoint)
            {
                uint lowerLv = 0, upperLv = 0;
                if (null != cSVHangupData.RecommendLv && cSVHangupData.RecommendLv.Count >= 2)
                {
                    lowerLv = cSVHangupData.RecommendLv[0];
                    upperLv = cSVHangupData.RecommendLv[1];
                }
                Text_Level.text = LanguageHelper.GetTextContent(2104014, lowerLv.ToString(), upperLv.ToString());
            }
            else
            {
                Text_Level.text = LanguageHelper.GetTextContent(2104016);
            }
            /// <summary> 默认图标 </summary>
            GameObject go_Icon1 = tr.Find("Image_Icon/Image_Icon_Lock").gameObject;
            GameObject go_Icon2 = tr.Find("Image_Icon/Image_Icon").gameObject;
            go_Icon1.SetActive(!isUnlockMap);
            go_Icon2.SetActive(isUnlockMap);
            /// <summary> 选项开关 </summary>
            Toggle toggle = tr.GetComponent<Toggle>();
            toggle.enabled = isUnlockMap;
        }
        /// <summary>
        /// 设置选中界面
        /// </summary>
        private void SetSelectView()
        {
            SetSelectEvent();
            SetLayer();
        }
        /// <summary>
        /// 设置选中事件
        /// </summary>
        private void SetSelectEvent()
        {
            CSVHangup.Data cSVHangupData = CSVHangup.Instance.GetConfData(curEventId);
            if (null == cSVHangupData)
            {
                text_Title.text = string.Empty;
                text_Explain.text = string.Empty;
                text_RecommendationLevel.text = string.Empty;
                return;
            }

            text_Title.text = LanguageHelper.GetTextContent(cSVHangupData.HangupName);
            text_Explain.text = LanguageHelper.GetTextContent(cSVHangupData.HangupDes);

            uint lowerLv = 0, upperLv = 0;
            if (null != cSVHangupData.RecommendLv && cSVHangupData.RecommendLv.Count >= 2)
            {
                lowerLv = cSVHangupData.RecommendLv[0];
                upperLv = cSVHangupData.RecommendLv[1];
            }
            text_RecommendationLevel.text = LanguageHelper.GetTextContent(2104014, lowerLv.ToString(), upperLv.ToString());
        }
        /// <summary>
        /// 设置挂机层
        /// </summary>
        private void SetLayer()
        {
            List<uint> layers;
            if (dict_Layer.TryGetValue(curEventId, out layers))
            {
                SetLayerItemList(layers.Count);
            }
            else
            {
                layers = new List<uint>();
            }
            uint selectLayer = GetDefaultEventLayerId(layers);
            for (int i = 0, count = list_EventLayer.Count; i < count; i++)
            {
                uint layrId = layers.Count > i ? layers[i] : 0;
                SetLayerItem(list_EventLayer[i].transform, layrId);
                bool isOn = selectLayer == layrId ? true : false;
                Toggle toggle = list_EventLayer[i];
                if (toggle.isOn != isOn)
                {
                    toggle.isOn = isOn;
                }
                else
                {
                    toggle.onValueChanged.Invoke(isOn);
                }
            }
        }
        /// <summary>
        /// 设置关卡层模版
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="layerId"></param>
        private void SetLayerItem(Transform tr, uint layerId)
        {
            CSVHangupLayerStage.Data cSVHangupLayerStageData = CSVHangupLayerStage.Instance.GetConfData(layerId);
            if (null == cSVHangupLayerStageData)
            {
                tr.gameObject.SetActive(false);
                return;
            }
            tr.gameObject.SetActive(true);
            tr.name = layerId.ToString();

            bool isUnlockHangupLayer = Sys_Hangup.Instance.IsUnlockHangupLayer(layerId);
            bool isLowLevelHangupLayer = Sys_Hangup.Instance.IsLowLevelHangupLayer(layerId);
            bool isHighLevelHangupLayer = Sys_Hangup.Instance.IsHighLevelHangupLayer(layerId);
            bool isRecommendationLevelHangupLayer = Sys_Hangup.Instance.IsRecommendationLevelHangupLayer(layerId);

            GameObject go_Lock = tr.Find("UnSelect/Image_Lock").gameObject;
            GameObject go_UnExp = tr.Find("UnSelect/Image_UnExp").gameObject;
            GameObject go_Normal = tr.Find("UnSelect/Image_Normal").gameObject;
            GameObject go_Select = tr.Find("Select/Image_Select").gameObject;

            if (!isUnlockHangupLayer)
            {
                go_Lock.SetActive(true);
                go_UnExp.SetActive(false);
                go_Normal.SetActive(false);
            }
            else if (isLowLevelHangupLayer || isHighLevelHangupLayer)
            {
                go_Lock.SetActive(false);
                go_UnExp.SetActive(true);
                go_Normal.SetActive(false);
            }
            else
            {
                go_Lock.SetActive(false);
                go_UnExp.SetActive(false);
                go_Normal.SetActive(true);
            }

            Text text_Lock_Layer = tr.Find("UnSelect/Image_Lock/Text_Order").GetComponent<Text>();
            Text text_UnExp_Layer = tr.Find("UnSelect/Image_UnExp/Text_Order").GetComponent<Text>();
            Text text_Normal_Layer = tr.Find("UnSelect/Image_Normal/Text_Order").GetComponent<Text>();
            Text text_Select_Layer = tr.Find("Select/Image_Select/Text_Order").GetComponent<Text>();
            text_Lock_Layer.text = cSVHangupLayerStageData.LayerStage.ToString();
            text_UnExp_Layer.text = text_Lock_Layer.text;
            text_Normal_Layer.text = text_Lock_Layer.text;
            text_Select_Layer.text = text_Lock_Layer.text;

            Text text_UnExp_Level = tr.Find("UnSelect/Image_UnExp/Text_Lv").GetComponent<Text>();
            Text text_Normal_Level = tr.Find("UnSelect/Image_Normal/Text_Lv").GetComponent<Text>();
            Text text_Select_Level = tr.Find("Select/Image_Select/Text_Lv").GetComponent<Text>();

            uint lowerLv = 0, upperLv = 0;
            if (cSVHangupLayerStageData.RecommendLv.Count >= 2)
            {
                lowerLv = cSVHangupLayerStageData.RecommendLv[0];
                upperLv = cSVHangupLayerStageData.RecommendLv[1];
            }
            text_UnExp_Level.text = string.Format("{0}-{1}", lowerLv, upperLv);
            text_Normal_Level.text = text_UnExp_Level.text;
            text_Select_Level.text = text_UnExp_Level.text;

            GameObject go_LowLevel_stage = tr.Find("UnSelect/Image_UnExp/Text_Stage_Low").gameObject;
            GameObject go_HighLevel_stage = tr.Find("UnSelect/Image_UnExp/Text_Stage_High").gameObject;
            go_LowLevel_stage.SetActive(isLowLevelHangupLayer);
            go_HighLevel_stage.SetActive(isHighLevelHangupLayer);

            GameObject go_Normal_stage = tr.Find("UnSelect/Image_Normal/Text_Stage").gameObject;
            go_Normal_stage.SetActive(isUnlockHangupLayer && isRecommendationLevelHangupLayer);

            GameObject go_Select_Level = tr.Find("Select/Image_Select/Text_Lv").gameObject;
            GameObject go_Select_NotFind = tr.Find("Select/Image_Select/Text_NotFind").gameObject;
            GameObject go_Select_Stage = tr.Find("Select/Image_Select/Text_Stage").gameObject;
            GameObject go_Select_LowLevel = tr.Find("Select/Image_Select/Text_Stage_Low").gameObject;
            GameObject go_Select_HighLevel = tr.Find("Select/Image_Select/Text_Stage_High").gameObject;

            go_Select_Level.SetActive(isUnlockHangupLayer);
            go_Select_NotFind.SetActive(!isUnlockHangupLayer);
            go_Select_Stage.SetActive(isUnlockHangupLayer && isRecommendationLevelHangupLayer);
            go_Select_LowLevel.SetActive(isUnlockHangupLayer && isLowLevelHangupLayer);
            go_Select_HighLevel.SetActive(isUnlockHangupLayer && isHighLevelHangupLayer);
        }
        /// <summary>
        /// 切换选中界面
        /// </summary>
        /// <param name="id"></param>
        private void SwitchSelectView(uint id)
        {
            ToggleGroup toggleGroup;
            if (!dict_EventGroup.TryGetValue(id, out toggleGroup))
                return;
            /// <summary> 选中菜单,刷新界面 </summary>
            var ActiveToggles = toggleGroup.ActiveToggles();
            foreach (var child in ActiveToggles)
            {
                if (child.isOn)
                {
                    SetCurSelectToggle(child);
                    return;
                }
            }
            /// <summary> 无选中菜单,刷新界面 </summary>
            SetCurSelectToggle(null);
        }
        /// <summary>
        /// 设置选中菜单
        /// </summary>
        /// <param name="toggle"></param>
        private void SetSelectToggle(Toggle toggle)
        {
            uint eventId = 0;
            if (null != toggle)
                uint.TryParse(toggle.name, out eventId);

            CSVHangup.Data cSVHangupData = CSVHangup.Instance.GetConfData(eventId);
            if (null == cSVHangupData || cSVHangupData.IslandID != curIsland) return;

            SetCurSelectToggle(toggle);
        }
        /// <summary>
        /// 设置当前页签中的选中菜单
        /// </summary>
        /// <param name="toggle"></param>
        private void SetCurSelectToggle(Toggle toggle)
        {
            uint eventId = 0;
            if (null != toggle)
                uint.TryParse(toggle.name, out eventId);

            curEventId = eventId;
            SetSelectView();
        }
        #endregion
        #region 响应事件
        /// <summary>
        /// 关闭按钮
        /// </summary>
        public void OnClick_Close()
        {
            CloseSelf();
        }
        /// <summary>
        /// 卡时提示
        /// </summary>
        public void OnClick_CheckWorkingHourTips()
        {
            uint level = Sys_Role.Instance.Role.Level;
            CSVCharacterAttribute.Data cSVCharacterAttributeData = CSVCharacterAttribute.Instance.GetConfData(level);
            string param1 = null == cSVCharacterAttributeData ? "0" : cSVCharacterAttributeData.PerWorkingHourExp.ToString();
            string param2 = CSVHangupParam.Instance.GetConfData(2).str_value;
            string param3 = CSVHangupParam.Instance.GetConfData(3).str_value;

            UIManager.OpenUI(EUIID.UI_Rule, false, new UIRuleParam
            {
                StrContent = LanguageHelper.GetTextContent(2104021, param1, param2, param3)
            });
        }
        /// <summary>
        /// 开始工作
        /// </summary>
        public void OnClick_Work()
        {
            Sys_Hangup.Instance.SetWorkingHour();
        }
        /// <summary>
        /// 便捷组队
        /// </summary>
        private void OnClick_Team()
        {
            CSVHangup.Data cSVHangupData = CSVHangup.Instance.GetConfData(curEventId);
            if (null == cSVHangupData) return;

            if (Sys_Team.Instance.IsFastOpen(true))
                Sys_Team.Instance.OpenFastUI(cSVHangupData.TeamID);
        }
        /// <summary>
        /// 前往挂机
        /// </summary>
        private void OnClick_GotoHangup()
        {
            bool isInFight = Sys_Fight.Instance.IsFight();
            if (isInFight) {
                return;
            }

            bool isSuccess = Sys_Hangup.Instance.GotoHangup(curLayerId);
            if (isSuccess)
            {
                Sys_Hangup.Instance.eventEmitter.Trigger(Sys_Hangup.EEvents.OnHangupEnter);
                OnClick_Close();
            }
        }

        private void OnClick_StopGoto() {
            Sys_Hangup.Instance.TryStopHangup();
        }
        
        private void OnClick_CatchPet() {
            CloseSelf();
            UIManager.OpenUI(EUIID.UI_Pet_Message, false, new MessageEx {
                messageState = EPetMessageViewState.Book,
                subPage = 0,
            });
        }

        private void OnClick_Setting() 
        {
            UIManager.OpenUI(EUIID.UI_Setting, false, Tuple.Create<ESettingPage, ESetting>(ESettingPage.Settings, ESetting.Hangup));
        }
        /// <summary>
        /// 选择挂机层
        /// </summary>
        /// <param name="toggle"></param>
        /// <param name="value"></param>
        private void OnClick_EventLayer(Toggle toggle, bool value)
        {
            if (value)
            {
                uint layerId = 0;
                uint.TryParse(toggle.name, out layerId);
                curLayerId = layerId;
            }
        }
        /// <summary>
        /// 查看层提示界面
        /// </summary>
        private void OnClick_CheckLayerTips(GameObject go)
        {
            uint id = 0;
            if (!uint.TryParse(go.name, out id)) {
                return;
            }
            
            UIManager.OpenUI(EUIID.UI_HangupFightOption, false, id);
        }

        /// <summary>
        /// 查看疲劳提示界面
        /// </summary>
        private void OnClick_CheckFatigueTips()
        {
            UIManager.OpenUI(EUIID.UI_HangupFightTriedTips);
        }
        /// <summary>
        /// 锁定提示
        /// </summary>
        /// <param name="go"></param>
        private void OnClick_LockTips(GameObject go)
        {
            Toggle toggle = go.GetComponent<Toggle>();
            if (null == toggle || toggle.enabled)
                return;

            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2104015));
        }
        /// <summary>
        /// 标记提示
        /// </summary>
        /// <param name="toggle"></param>
        /// <param name="value"></param>
        private void OnClick_MarkTips(Toggle toggle, bool value)
        {
            //提示界面
            if (value)
            {
                SetSelectToggle(toggle);
            }
        }
        /// <summary>
        /// 更新滚动子节点
        /// </summary>
        /// <param name="index"></param>
        /// <param name="trans"></param>
        private void OnUpdateChildrenCallback(int index, Transform trans)
        {
            if (index < 0 || index >= list_Island.Count) {
                return;
            }
            trans.name = index.ToString();
            uint id = list_Island[index];
            CSVIsland.Data cSVIslandData = CSVIsland.Instance.GetConfData(id);
            if (null == cSVIslandData) {
                return;
            }

            for (int i = 0, length = this.list_Island.Count; i < length; ++i) {
                // Error: 这种写法要求：siblingindex是顺序的，否则就会出现问题了
                Transform t = go_IslandNode.transform.GetChild(i);
                t.gameObject.SetActive(i == index);
            }

            LoadAssetAsyn(cSVIslandData.map_ui, trans, cSVIslandData.id);
        }

        private void Refresh(uint id, int index) {
            Transform tr = go_IslandNode.transform.GetChild(index); // Error: 这种写法要求：siblingindex是顺序的，否则就会出现问题了
            OnUpdateChildrenCallback(index, tr);
            
            curIsland = id;
            SwitchSelectView(id);
        }
        
        /// <summary>
        /// 开关卡时
        /// </summary>
        private void OnWorkingHourSwitch()
        {
            SetWorkingHourView();
        }
        /// <summary>
        /// 卡时更新
        /// </summary>
        private void OnOnWorkingHourPoint()
        {
            SetWorkingHourView();
        }

        private void OnPatrolStateChange(int oldV, int newV) {
            RefreshHangupBtns();
        }

        private void OnHangupExit() {
            RefreshHangupBtns();
        }

        #endregion
        #region 提供功能
        #endregion
    }
}