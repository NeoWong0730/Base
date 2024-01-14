using Logic.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Table;
using UnityEngine.UI;
using UnityEngine;
using Lib.Core;
using UnityEngine.ResourceManagement.AsyncOperations;
using Framework;
using System;

namespace Logic
{
    /// <summary> 经典boss战 </summary>
    public class UI_ClassicBossWar : UIBase
    {
        #region 界面组件
        /// <summary> 悬赏令节点 </summary>
        private GameObject go_RewardOrderNode;
        /// <summary> 任务节点 </summary>
        private GameObject go_TaskNode;
        /// <summary> 解锁节点 </summary>
        private GameObject go_UnlockNode;
        /// <summary> 地图位置导航 </summary>
        private GameObject go_NavigationTips;
        /// <summary> 今日次数 </summary>
        private Text text_TodayCount;
        /// <summary> 前往挑战图片 </summary>
        private Image image_GotoFight;
        /// <summary> 标记模版 </summary>
        private Transform tr_MarkNode;
        /// <summary> 事件模版 </summary>
        private Transform tr_EventItem;
        /// <summary> 翻页 </summary>
        private CP_PageDot cP_PageDot;
        /// <summary> 岛屿节点模版 </summary>
        private GameObject go_IslandNodeItem;
        /// <summary> 无限滚动 </summary>
        private InfinityGridLayoutGroup gridGroup;
        /// <summary> 居中脚本 </summary>
        private UICenterOnChild uiCenterOnChild;
        /// <summary> 事件标记字典 </summary>
        private Dictionary<uint, Toggle> dict_EventMark = new Dictionary<uint, Toggle>();
        /// <summary> 事件标记组字段 </summary>
        private Dictionary<uint, ToggleGroup> dict_EventGroup = new Dictionary<uint, ToggleGroup>();
        /// <summary> 模型显示统一脚本 </summary>
        public AssetDependencies assetDependencies;
        private ShowSceneControl showSceneControl;
        private DisplayControl<EPetModelParts> petDisplay;
        private RawImage rawImage1;
        private RawImage rawImage2;
        #endregion
        #region 数据定义
        /// <summary> 当前岛屿编号 </summary>
        private uint curIsland { get; set; } = 0;
        /// <summary> 当前事件编号 </summary>
        private uint curEventId { get; set; } = 0;
        /// <summary> 岛屿列表 </summary>
        private List<uint> list_Island = new List<uint>();
        /// <summary> 岛屿对应区域事件 </summary>
        private Dictionary<uint, List<uint>> dict_Event = new Dictionary<uint, List<uint>>();
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
        protected override void OnDestroy()
        {

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

            curEventId = Sys_ClassicBossWar.Instance.ChallengeBossID;
        }
        protected override void OnOpened()
        {

        }
        protected override void OnShow()
        {
            _LoadShowScene();
            RefreshView();
        }
        protected override void OnHide()
        {
            RecoveryAsset();
            _UnloadModel();
            _UnloadShowScene();
        }
        protected override void OnUpdate()
        {

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
            go_RewardOrderNode = transform.Find("Animator/LeaderWar/View_Right/Reward").gameObject;
            go_TaskNode = transform.Find("Animator/LeaderWar/View_Right/Task").gameObject;
            go_UnlockNode = transform.Find("Animator/LeaderWar/View_Right/Open").gameObject;
            go_NavigationTips = transform.Find("Animator/View_TriedTips").gameObject;
            text_TodayCount = transform.Find("Animator/LeaderWar/View_Bottom/Object_Number/Text_Number").GetComponent<Text>();
            image_GotoFight = transform.Find("Animator/LeaderWar/View_Bottom/Button_Challenge").GetComponent<Image>();
            rawImage1 = transform.Find("Animator/LeaderWar/View_Right/Reward/Texture").GetComponent<RawImage>();
            rawImage2 = transform.Find("Animator/LeaderWar/View_Right/Task/Texture").GetComponent<RawImage>();

            tr_MarkNode = transform.Find("Animator/LeaderWar/View_Map/View_Mark/MarkList");
            tr_MarkNode.gameObject.SetActive(false);
            tr_EventItem = transform.Find("Animator/LeaderWar/View_Map/View_Mark/MarkList/Toggle");
            tr_EventItem.gameObject.SetActive(false);
            cP_PageDot = transform.Find("Animator/LeaderWar/View_Map/View_Bottom").GetComponent<CP_PageDot>();
            assetDependencies = transform.GetComponent<AssetDependencies>();

            go_IslandNodeItem = transform.Find("Animator/LeaderWar/View_Map/Object_Map/View_Island/Grid/Item").gameObject;
            gridGroup = transform.Find("Animator/LeaderWar/View_Map/Object_Map/View_Island/Grid").gameObject.GetNeedComponent<InfinityGridLayoutGroup>();
            gridGroup.updateChildrenCallback = OnUpdateChildrenCallback;
            uiCenterOnChild = transform.Find("Animator/LeaderWar/View_Map/Object_Map/View_Island").gameObject.GetNeedComponent<UICenterOnChild>();
            uiCenterOnChild.onCenter = OnCenter;

            transform.Find("Animator/View_Title09/Btn_Close").GetComponent<Button>().onClick.AddListener(OnClick_Close);
            transform.Find("Animator/LeaderWar/View_Bottom/Button_Challenge").GetComponent<Button>().onClick.AddListener(OnClick_GotoFight);
            transform.Find("Animator/LeaderWar/View_Bottom/Button_Team").GetComponent<Button>().onClick.AddListener(OnClick_Team);
            transform.Find("Animator/LeaderWar/View_Right/Reward/Btn_Check").GetComponent<Button>().onClick.AddListener(OnClick_CheckRewardOrder);
            transform.Find("Animator/LeaderWar/View_Right/Open/Button_Tips").GetComponent<Button>().onClick.AddListener(OnClick_CheckBossPlace);
            transform.Find("Animator/View_TriedTips/Background").GetComponent<Button>().onClick.AddListener(OnClick_CloseBossPlace);
        }
        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="toRegister"></param>
        private void OnProcessEventsForEnable(bool toRegister)
        {

        }
        /// <summary>
        /// 创建足够的列表
        /// </summary>
        /// <param name="node"></param>
        /// <param name="child"></param>
        /// <param name="number"></param>
        private void CreateItemList(Transform node, Transform child, int number)
        {
            while (node.childCount < number)
            {
                GameObject.Instantiate(child, node);
            }
        }
        #endregion
        #region 资源加载
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
        /// <summary>
        /// 加载显示场景
        /// </summary>
        /// <param name="rawImage"></param>
        private void _LoadShowScene()
        {
            if (showSceneControl == null)
            {
                showSceneControl = new ShowSceneControl();
            }

            GameObject sceneModel = GameObject.Instantiate<GameObject>(assetDependencies.mCustomDependencies[0] as GameObject);
            sceneModel.transform.SetParent(GameCenter.sceneShowRoot.transform);
            sceneModel.transform.localPosition = new Vector3(1000, 0, 0);
            showSceneControl.Parse(sceneModel);
            //设置RenderTexture纹理到RawImage
            rawImage1.texture = showSceneControl.GetTemporary(0, 0, 6, RenderTextureFormat.ARGB32, 1f);
            rawImage2.texture = rawImage1.texture;
        }
        /// <summary>
        /// 家族模型
        /// </summary>
        /// <param name="cSVClassicBossData"></param>
        private void _LoadShowModel(CSVClassicBoss.Data cSVClassicBossData)
        {
            CSVNpc.Data cSVNpcData = CSVNpc.Instance.GetConfData(cSVClassicBossData.NPCID);
            if (null == cSVNpcData) return;
            if (petDisplay == null)
            {
                petDisplay = DisplayControl<EPetModelParts>.Create((int)EPetModelParts.Count);
            }
            petDisplay.onLoaded = (int obj) =>
            {
                if (obj == 0)
                {
                    petDisplay.GetPart(EPetModelParts.Main).gameObject.SetActive(false);
                    uint weaponId = Constants.UMARMEDID;
                    petDisplay.mAnimation.UpdateHoldingAnimations(cSVNpcData.action_show_id, weaponId, null, EStateType.Idle, petDisplay.GetPart(EPetModelParts.Main).gameObject);
                }
            };

            string _modelPath = cSVNpcData.model_show;
            petDisplay.eLayerMask = ELayerMask.ModelShow;
            petDisplay.LoadMainModel(EPetModelParts.Main, _modelPath, EPetModelParts.None, null);
            petDisplay.GetPart(EPetModelParts.Main).SetParent(showSceneControl.mModelPos, null);
            Vector3 localRotation = new Vector3(cSVClassicBossData.rotationx / 10000.0f, cSVClassicBossData.rotationy / 10000.0f, cSVClassicBossData.rotationz / 10000.0f);
            Vector3 localScale = new Vector3(cSVClassicBossData.scale / 10000.0f, cSVClassicBossData.scale / 10000.0f, cSVClassicBossData.scale / 10000.0f);
            Vector3 localPosition = new Vector3(cSVClassicBossData.positionx / 10000.0f, cSVClassicBossData.positiony / 10000.0f, cSVClassicBossData.positionz / 10000.0f);
            showSceneControl.mModelPos.transform.localEulerAngles = localRotation;
            showSceneControl.mModelPos.transform.localScale = localScale;
            showSceneControl.mModelPos.transform.localPosition = localPosition;
        }
        /// <summary>
        /// 卸载模型
        /// </summary>
        public void _UnloadModel()
        {
            //petDisplay?.Dispose();
            DisplayControl<EPetModelParts>.Destory(ref petDisplay);
        }
        /// <summary>
        /// 卸载场景
        /// </summary>
        private void _UnloadShowScene()
        {
            showSceneControl?.Dispose();
        }
        #endregion
        #region 数据处理
        /// <summary>
        /// 设置岛屿数据
        /// </summary>
        private void SetIslandsData()
        {
            list_Island.Clear();
            //Dictionary<uint, CSVIsland.Data> dict_IslandData = CSVIsland.Instance.GetDictData();
            //List<uint> list_IslandDataKeys = new List<uint>(dict_IslandData.Keys);

            var list_IslandDatas = CSVIsland.Instance.GetAll();
            for (int i = 0, count = list_IslandDatas.Count; i < count; i++)
            {
                //uint id = list_IslandDataKeys[i];
                //var item = dict_IslandData[id];

                var item = list_IslandDatas[i];
                if (item.island_type == 2 && Sys_Hangup.Instance.IsUnlockIsland(item.id))
                {
                    list_Island.Add(item.id);
                }
            }

            dict_Event.Clear();
            //Dictionary<uint, CSVClassicBoss.Data> dict_ClassicBossData = CSVClassicBoss.Instance.GetDictData();
            //List<uint> list_ClassicBossDataKeys = new List<uint>(dict_ClassicBossData.Keys);

            var list_ClassicBossDatas = CSVClassicBoss.Instance.GetAll();
            for (int i = 0, count = list_ClassicBossDatas.Count; i < count; i++)
            {
                //uint id = list_ClassicBossDataKeys[i];
                //var item = dict_ClassicBossData[id];

                var item = list_ClassicBossDatas[i];
                if (dict_Event.ContainsKey(item.IslandID))
                {
                    dict_Event[item.IslandID].Add(item.id);
                }
                else
                {
                    dict_Event.Add(item.IslandID, new List<uint>() { item.id });
                }
            }
        }
        /// <summary>
        /// 得到默认选择事件
        /// </summary>
        /// <param name="events"></param>
        /// <returns></returns>
        private uint GetDefaultIslandId()
        {
            uint defaultId = 0;
                //Sys_ClassicBossWar.Instance.ChallengeBossID;

            //var exploredBossIds = Sys_ClassicBossWar.Instance.cmdClassicBossDataNtf.Data.ExploredBossIds;
            List<uint> keys = new List<uint>(dict_Event.Keys);

            for (int i = 0; i < keys.Count; i++)
            {
                uint key = keys[i];
                var list = dict_Event[key];
                if (list.IndexOf(Sys_ClassicBossWar.Instance.ChallengeBossID) >= 0)
                {
                    defaultId = key;
                    break;
                }

                //for (int j = 0; j < list.Count; j++)
                //{
                //    uint id = list[j];
                //    if (exploredBossIds.Contains(id))
                //    {
                //        defaultId = key;
                //        break;
                //    }
                //}
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
            SetView();
            SetPage();
            SetIslandView();
        }
        /// <summary>
        /// 设置界面
        /// </summary>
        private void SetView()
        {
            uint UsedTimes = (uint)Sys_ClassicBossWar.Instance.GetDailyTimes();
            uint MaxTimes = (uint)Sys_ClassicBossWar.Instance.GetDailyTotalTimes();
            text_TodayCount.text = (MaxTimes - UsedTimes).ToString();
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
        /// 设置岛屿界面
        /// </summary>
        private void SetIslandView()
        {
            dict_EventGroup.Clear();
            dict_EventMark.Clear();
            CreateItemList(gridGroup.transform, go_IslandNodeItem.transform, list_Island.Count);
            uiCenterOnChild.InitPageArray();
            gridGroup.minAmount = list_Island.Count;
            gridGroup.SetAmount(list_Island.Count);
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
            //uint selectEvent = Sys_ClassicBossWar.Instance.ChallengeBossID;// GetDefaultIslandId(list);
            /// <summary> 地图各个标记 </summary>
            for (int i = 0, count = list.Count; i < count; i++)
            {
                var eventId = list[i];
                GameObject go = GameObject.Instantiate(copyChild.gameObject, copyChild.transform.parent);
                Toggle toggle = go.GetComponent<Toggle>();
                toggle.onValueChanged.AddListener(((bool value) => OnClick_MarkTips(toggle, value)));
                dict_EventMark.Add(eventId, toggle);
                SetMarkItem(go.transform, eventId);
                if (curIsland == id && toggle.enabled)
                {
                    toggle.isOn = curEventId == eventId;
                    //bool isOn = selectEvent == eventId ? true : false;
                    //if (toggle.isOn != isOn)
                    //{
                    //    toggle.isOn = isOn;
                    //}
                    //else
                    //{
                    //    toggle.onValueChanged.Invoke(isOn);
                    //}
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
            CSVClassicBoss.Data cSVClassicBossData = CSVClassicBoss.Instance.GetConfData(eventId);
            if (null == cSVClassicBossData)
            {
                tr.gameObject.SetActive(false);
                return;
            }
            tr.gameObject.SetActive(true);
            tr.name = eventId.ToString();
            /// <summary> 是否解锁 </summary>
            bool isUnlock = IsUnlock(cSVClassicBossData);
            //Sys_Task.Instance.IsSubmited(cSVClassicBossData.TaskID);
            /// <summary> 坐标 </summary>
            RectTransform rt = tr.GetComponent<RectTransform>();
            Vector3 vector3 = cSVClassicBossData.UIPosition?.Count >= 2 ?
                new Vector3(cSVClassicBossData.UIPosition[0], cSVClassicBossData.UIPosition[1], 0) : Vector3.zero;
            rt.anchoredPosition3D = vector3;
            /// <summary> 未解锁 </summary>
            GameObject go_Lock = tr.Find("Image_Forbid").gameObject;
            go_Lock.SetActive(!isUnlock);
            if (!isUnlock)
            {
                Text textLock = go_Lock.transform.Find("Text_Lock").GetComponent<Text>();
                TextHelper.SetText(textLock, LanguageHelper.GetTextContent(2105001, cSVClassicBossData.TaskLv.ToString()));
            }

            GameObject go_Level_1 = tr.Find("Image_Approve").gameObject;
            Text text_Level_1 = tr.Find("Image_Approve/Text_Level").GetComponent<Text>();
            GameObject go_Level_2 = tr.Find("Image_Approve (1)").gameObject;
            Text text_Level_2 = tr.Find("Image_Approve (1)/Text_Level").GetComponent<Text>();
            TextHelper.SetText(text_Level_1, LanguageHelper.GetTextContent(2105001, cSVClassicBossData.Lv.ToString()));
            TextHelper.SetText(text_Level_2, LanguageHelper.GetTextContent(2105001, cSVClassicBossData.Lv.ToString()));
            if (isUnlock)
            {
                if (Sys_Role.Instance.Role.Level >= cSVClassicBossData.Lv)
                {
                    go_Level_1.gameObject.SetActive(true);
                    go_Level_2.gameObject.SetActive(false);
                }
                else
                {
                    go_Level_1.gameObject.SetActive(false);
                    go_Level_2.gameObject.SetActive(true);
                }
            }
            else
            {
                go_Level_1.gameObject.SetActive(false);
                go_Level_2.gameObject.SetActive(false);
            }
            /// <summary> 默认图标 </summary>
            GameObject go_Icon1 = tr.Find("Image_LockIcon").gameObject;
            Image image_Icon2 = tr.Find("Image_UnlockIcon").GetComponent<Image>();
            go_Icon1.SetActive(!isUnlock);
            image_Icon2.gameObject.SetActive(isUnlock);
            ImageHelper.SetIcon(image_Icon2, cSVClassicBossData.Icon);
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

            CSVClassicBoss.Data cSVClassicBossData = CSVClassicBoss.Instance.GetConfData(eventId);
            if (null == cSVClassicBossData || cSVClassicBossData.IslandID != curIsland) return;

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

            if (curEventId != eventId)
            {
                curEventId = eventId;
            }

            SetSelectView();
        }
        /// <summary>
        /// 设置选中界面
        /// </summary>
        private void SetSelectView()
        {
            CSVClassicBoss.Data cSVClassicBossData = CSVClassicBoss.Instance.GetConfData(curEventId);
            if (null == cSVClassicBossData)
            {
                go_RewardOrderNode.SetActive(false);
                go_TaskNode.SetActive(false);
                go_UnlockNode.SetActive(false);
                return;
            }
            /// <summary> 是否解锁 </summary>
            bool isUnlock = IsUnlock(cSVClassicBossData);
                //Sys_Task.Instance.IsSubmited(cSVClassicBossData.TaskID);
            /// <summary> 界面分支 </summary>
            switch (cSVClassicBossData.UnlockType)
            {
                case 1: //悬赏令
                    {
                        if (isUnlock)
                        {
                            go_RewardOrderNode.SetActive(false);
                            go_TaskNode.SetActive(false);
                            go_UnlockNode.SetActive(true);
                            SetUnlockView(go_UnlockNode.transform, cSVClassicBossData);
                        }
                        else
                        {
                            go_RewardOrderNode.SetActive(true);
                            go_TaskNode.SetActive(false);
                            go_UnlockNode.SetActive(false);
                            SetRewardOrderView(go_RewardOrderNode.transform, cSVClassicBossData);
                        }
                    }
                    break;
                case 2: //完成任务
                    {
                        if (isUnlock)
                        {
                            go_RewardOrderNode.SetActive(false);
                            go_TaskNode.SetActive(false);
                            go_UnlockNode.SetActive(true);
                            SetUnlockView(go_UnlockNode.transform, cSVClassicBossData);
                        }
                        else
                        {
                            go_RewardOrderNode.SetActive(false);
                            go_TaskNode.SetActive(true);
                            go_UnlockNode.SetActive(false);
                            SetTaskView(go_TaskNode.transform, cSVClassicBossData);
                        }
                    }
                    break;
            }
            /// <summary> 按钮灰态 </summary>
            var cmdClassicBossDataNtf = Sys_ClassicBossWar.Instance.cmdClassicBossDataNtf;
            bool isUnlockButton = cmdClassicBossDataNtf.Data.ExploredBossIds.Contains(cSVClassicBossData.id);
            ImageHelper.SetImageGray(image_GotoFight, !isUnlock, true);
        }
        /// <summary>
        /// 设置悬赏令界面
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="cSVClassicBossData"></param>
        private void SetRewardOrderView(Transform tr, CSVClassicBoss.Data cSVClassicBossData)
        {
            /// <summary> 标题 </summary>
            Text text_Title = tr.Find("Title/Text_Title").GetComponent<Text>();
            CSVNpc.Data cSVNpcData = CSVNpc.Instance.GetConfData(cSVClassicBossData.NPCID);
            text_Title.text = null == cSVNpcData ? string.Empty : LanguageHelper.GetNpcTextContent(cSVNpcData.name);

            /// <summary> Tips </summary>
            Text text_Tip = tr.Find("Text_Tips").GetComponent<Text>();
            SetRewardTip(text_Tip, cSVClassicBossData);

            _UnloadModel();
            _LoadShowModel(cSVClassicBossData);
        }

        /// <summary>
        /// 设置悬赏令tip
        /// </summary>
        /// <param name="text"></param>
        /// <param name="cSVClassicBossData"></param>
        private void SetRewardTip(Text text, CSVClassicBoss.Data cSVClassicBossData)
        {
            System.Text.StringBuilder sb = StringBuilderPool.GetTemporary();
            if (cSVClassicBossData.TaskLv != 0u)
            {
                if (cSVClassicBossData.CriminalID != 0)
                {
                    uint levelId = Sys_Role.Instance.Role.Level >= cSVClassicBossData.TaskLv ? 6041u : 6040u;
                    sb.Append(LanguageHelper.GetTextContent(levelId, LanguageHelper.GetTextContent(6037u, cSVClassicBossData.TaskLv.ToString())));
                    //sb.Append(LanguageHelper.GetTextContent(6037u, cSVClassicBossData.TaskLv));
                    sb.Append(LanguageHelper.GetTextContent(6039u));

                    levelId = Sys_Adventure.Instance.CheckRewardIsFinish(cSVClassicBossData.CriminalID) ? 6041u : 6040u;
                    sb.Append(LanguageHelper.GetTextContent(levelId, LanguageHelper.GetTextContent(6014u)));
                }
                else
                {
                    uint levelId = Sys_Role.Instance.Role.Level >= cSVClassicBossData.TaskLv ? 6041u : 6040u;
                    sb.Append(LanguageHelper.GetTextContent(levelId, LanguageHelper.GetTextContent(6038u, cSVClassicBossData.TaskLv.ToString())));
                }
            }
            else
            {
                if (cSVClassicBossData.CriminalID != 0)
                {
                    uint levelId = Sys_Adventure.Instance.CheckRewardIsFinish(cSVClassicBossData.CriminalID) ? 6041u : 6040u;
                    sb.Append(LanguageHelper.GetTextContent(levelId, LanguageHelper.GetTextContent(6014u)));
                }
            }
            text.text = StringBuilderPool.ReleaseTemporaryAndToString(sb);
        }

        /// <summary>
        /// 设置任务界面
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="cSVClassicBossData"></param>
        private void SetTaskView(Transform tr, CSVClassicBoss.Data cSVClassicBossData)
        {
            /// <summary> 标题 </summary>
            Text text_Title = tr.Find("Title/Text_Title").GetComponent<Text>();
            CSVNpc.Data cSVNpcData = CSVNpc.Instance.GetConfData(cSVClassicBossData.NPCID);
            text_Title.text = null == cSVNpcData ? string.Empty : LanguageHelper.GetNpcTextContent(cSVNpcData.name);

            Transform trans = tr.Find("Tips_bg");
            trans.gameObject.SetActive(false);
            if (!IsLevelCoditionAlone(cSVClassicBossData))
            {
                trans.gameObject.SetActive(true);
                /// <summary> 任务名 </summary>
                Text text_taskName = tr.Find("Tips_bg/Text").GetComponent<Text>();
                CSVTask.Data cSVTaskData = CSVTask.Instance.GetConfData(cSVClassicBossData.TaskID);
                text_taskName.text = null == cSVTaskData ? string.Empty : LanguageHelper.GetTaskTextContent(cSVTaskData.taskName);
                /// <summary> 任务等级 </summary>
                Text text_taskLevel = tr.Find("Tips_bg/Level").GetComponent<Text>();
                text_taskLevel.text = LanguageHelper.GetTextContent(2105001, cSVClassicBossData.TaskLv.ToString());
            }

            /// <summary> Tips </summary>
            Text text_Tip = tr.Find("Text_Tips").GetComponent<Text>();
            SetTaskTip(text_Tip, cSVClassicBossData);

            _UnloadModel();
            _LoadShowModel(cSVClassicBossData);
        }

        /// <summary>
        /// 任务tips
        /// </summary>
        /// <param name="text"></param>
        /// <param name="cSVClassicBossData"></param>
        private void SetTaskTip(Text text, CSVClassicBoss.Data cSVClassicBossData)
        {
            System.Text.StringBuilder sb = StringBuilderPool.GetTemporary();
            if (cSVClassicBossData.TaskLv != 0u)
            {
                if (cSVClassicBossData.TaskID != 0)
                {
                    uint levelId = Sys_Role.Instance.Role.Level >= cSVClassicBossData.TaskLv ? 6041u : 6040u;
                    sb.Append(LanguageHelper.GetTextContent(levelId, LanguageHelper.GetTextContent(6037u, cSVClassicBossData.TaskLv.ToString())));
                    sb.Append(LanguageHelper.GetTextContent(6039u));

                    levelId = Sys_Task.Instance.IsFinish(cSVClassicBossData.TaskID) ? 6041u : 6040u;
                    sb.Append(LanguageHelper.GetTextContent(levelId, LanguageHelper.GetTextContent(6012u)));
                }
                else
                {
                    uint levelId = Sys_Role.Instance.Role.Level >= cSVClassicBossData.TaskLv ? 6041u : 6040u;
                    sb.Append(LanguageHelper.GetTextContent(levelId, LanguageHelper.GetTextContent(6038u, cSVClassicBossData.TaskLv.ToString())));
                }
            }
            else
            {
                if (cSVClassicBossData.TaskID != 0)
                {
                    uint levelId = Sys_Task.Instance.IsFinish(cSVClassicBossData.TaskID) ? 6041u : 6040u;
                    sb.Append(LanguageHelper.GetTextContent(6012u));
                }
            }
            text.text = StringBuilderPool.ReleaseTemporaryAndToString(sb);
        }

        /// <summary>
        /// 判断是否只有等级一个条件
        /// </summary>
        /// <param name="cSVClassicBossData"></param>
        /// <returns></returns>
        private bool IsLevelCoditionAlone(CSVClassicBoss.Data cSVClassicBossData)
        {
            return cSVClassicBossData.TaskLv != 0u
                && cSVClassicBossData.TaskID == 0u
                && cSVClassicBossData.CriminalID == 0u;
        }

        /// <summary>
        /// 设置解锁界面
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="cSVClassicBossData"></param>
        private void SetUnlockView(Transform tr, CSVClassicBoss.Data cSVClassicBossData)
        {
            /// <summary> 标题 </summary>
            Text text_Title = tr.Find("Title/Text_Title").GetComponent<Text>();
            CSVNpc.Data cSVNpcData = CSVNpc.Instance.GetConfData(cSVClassicBossData.NPCID);
            text_Title.text = null == cSVNpcData ? string.Empty : LanguageHelper.GetNpcTextContent(cSVNpcData.name);

            /// <summary> 需求等级 </summary>
            Text text_need = tr.Find("Level_Unlock/Value").GetComponent<Text>();
            TextHelper.SetText(text_need,
                LanguageHelper.GetTextContent(2105001, cSVClassicBossData.TaskLv.ToString()),
                LanguageHelper.GetTextStyle(Sys_Role.Instance.Role.Level >= cSVClassicBossData.TaskLv ? (uint)85 : 86));

            /// <summary> 等级 </summary>
            Text text_Level = tr.Find("Level/Value").GetComponent<Text>();
            TextHelper.SetText(text_Level,
                 LanguageHelper.GetTextContent(2105001, cSVClassicBossData.Lv.ToString()),
                 LanguageHelper.GetTextStyle(Sys_Role.Instance.Role.Level >= cSVClassicBossData.Lv ? (uint)85 : 86));
            /// <summary> 推荐积分 </summary>
            Text text_Score = tr.Find("Score/Value").GetComponent<Text>();
            TextHelper.SetText(text_Score,
                cSVClassicBossData.Score.ToString(),
                LanguageHelper.GetTextStyle(Sys_Attr.Instance.rolePower >= cSVClassicBossData.Score ? (uint)85 : 86));

            /// <summary> 描述 </summary>
            Text text_Describe = tr.Find("Describe").GetComponent<Text>();
            text_Describe.text = LanguageHelper.GetTextContent(cSVClassicBossData.PreviousInformation);
            /// <summary> 地图位置导航 </summary>
            Text text_Navigation = tr.Find("Site/Value").GetComponent<Text>();
            CSVMapInfo.Data cSVMapInfoData = CSVMapInfo.Instance.GetConfData(cSVClassicBossData.MapID);
            text_Navigation.text = null == cSVMapInfoData ? string.Empty : LanguageHelper.GetTextContent(cSVMapInfoData.name);
            /// <summary> 锁定寻路 </summary>
            Text text_LockNavigation = tr.Find("Tips_bg/Text1").GetComponent<Text>();
            /// <summary> 解锁寻路 </summary>
            Text text_UnLockNavigation = tr.Find("Tips_bg/Text2").GetComponent<Text>();
            var cmdClassicBossDataNtf = Sys_ClassicBossWar.Instance.cmdClassicBossDataNtf;
            bool isUnLock = cmdClassicBossDataNtf.Data.ExploredBossIds.Contains(cSVClassicBossData.id);
            text_LockNavigation.gameObject.SetActive(!isUnLock);
            text_UnLockNavigation.gameObject.SetActive(isUnLock);
            /// <summary> 掉落设置 </summary>
            Transform tr_ItemNode = tr.Find("Scroll_View/Viewport").transform;
            GameObject go_Item = tr.Find("Scroll_View/Viewport/Item").gameObject;
            List<ItemIdCount> list_drop = CSVDrop.Instance.GetDropItem(cSVClassicBossData.DropID);
            CreateItemList(go_Item.transform.parent, go_Item.transform, list_drop.Count);

            List<PropItem> list_RewardItem = new List<PropItem>();
            uint x = 0;
            long y = 0;
            for (int i = 0; i < tr_ItemNode.childCount; i++)
            {
                PropItem propItem = new PropItem();
                propItem.BindGameObject(tr_ItemNode.GetChild(i).gameObject);
                list_RewardItem.Add(propItem);
            }
            for (int i = 0; i < list_RewardItem.Count; i++)
            {
                uint equipPara = 0;
                if (i < list_drop.Count)
                {
                    x = list_drop[i].id;
                    y = list_drop[i].count;
                    equipPara = list_drop[i].equipPara;
                }
                else
                {
                    x = 0; y = 0;
                }
                SetRewardItem(list_RewardItem[i], x, y, equipPara);
            }
            LayoutRebuilder.ForceRebuildLayoutImmediate(tr as RectTransform);
        }
        /// <summary>
        /// 设置奖励模版
        /// </summary>
        /// <param name="propItem"></param>
        /// <param name="id"></param>
        /// <param name="Num"></param>
        public void SetRewardItem(PropItem propItem, uint id, long Num, uint equipPara)
        {
            CSVItem.Data cSVItemData = CSVItem.Instance.GetConfData(id);
            if (null == cSVItemData)
            {
                propItem.SetActive(false);
                return;
            }
            propItem.SetActive(true);
            PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(id, Num, true, false, false, false, false, true, false, true, OnClickPropItem, false, false);
            itemData.SetQuality(Sys_Equip.Instance.CalPreviewQuality(equipPara));
            itemData.EquipPara = equipPara;
            propItem.SetData(new MessageBoxEvt(EUIID.UI_Map, itemData));
        }

        /// <summary>
        /// 预览点击信息
        /// </summary>
        /// <param name="item"></param>
        private void OnClickPropItem(PropItem item)
        {
            ItemData mItemData = new ItemData(0, 0, item.ItemData.id, (uint)item.ItemData.count, 0, false, false, null, null, 0);
            mItemData.EquipParam = item.ItemData.EquipPara;
            uint typeId = mItemData.cSVItemData.type_id;
            if (typeId == (uint)EItemType.Equipment)
            {
                UIManager.OpenUI(EUIID.UI_Equipment_Preview, false, mItemData);
            }
            else
            {
                PropMessageParam propParam = new PropMessageParam();
                propParam.itemData = mItemData;
                propParam.showBtnCheck = false;
                propParam.sourceUiId = EUIID.UI_ClassicBossWar;
                UIManager.OpenUI(EUIID.UI_Prop_Message, false, propParam);
            }
        }

        /// <summary>
        /// 设置地图位置导航
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="eventId"></param>
        private void SetNavigationTips(Transform tr, uint eventId)
        {
            Text text_Navigation = tr.Find("Tip/Text").GetComponent<Text>();
            CSVClassicBoss.Data cSVClassicBossData = CSVClassicBoss.Instance.GetConfData(eventId);
            text_Navigation.text = null == cSVClassicBossData ? string.Empty : LanguageHelper.GetTextContent(cSVClassicBossData.Navigation);
        }

        private bool IsUnlock(CSVClassicBoss.Data cSVClassicBossData)
        {
            bool unlock = Sys_Role.Instance.Role.Level >= cSVClassicBossData.TaskLv;
            if (cSVClassicBossData.TaskID != 0u)
                unlock &= Sys_Task.Instance.IsSubmited(cSVClassicBossData.TaskID);
            if (cSVClassicBossData.CriminalID != 0)
                unlock &= Sys_Adventure.Instance.CheckRewardIsFinish(cSVClassicBossData.CriminalID);
            return unlock;
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
            CSVClassicBoss.Data cSVClassicBossData = CSVClassicBoss.Instance.GetConfData(curEventId);
            if (null == cSVClassicBossData) return;

            if (Sys_Team.Instance.IsFastOpen(true))
                Sys_Team.Instance.OpenFastUI(cSVClassicBossData.TeamID);
        }
        /// <summary>
        /// 前往挑战
        /// </summary>
        private void OnClick_GotoFight()
        {
            CSVClassicBoss.Data cSVClassicBossData = CSVClassicBoss.Instance.GetConfData(curEventId);
            if (null == cSVClassicBossData) return;

            bool isUnlock = IsUnlock(cSVClassicBossData);
            if (!isUnlock)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2105007));
                return;
            }

            //var cmdClassicBossDataNtf = Sys_ClassicBossWar.Instance.cmdClassicBossDataNtf;
            //if (!cmdClassicBossDataNtf.Data.ExploredBossIds.Contains(cSVClassicBossData.id))
            //{
            //    PromptBoxParameter.Instance.OpenPromptBox(2105008, 0,
            //    () => {
            //        var args = new Sys_Map.TargetClassicBossParameter(cSVClassicBossData.MapID, curEventId);
            //        UIManager.OpenUI((int)EUIID.UI_Map, false, args);
            //    }, null);
            //    //Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(6035));
            //    return;
            //}
            ActionCtrl.Instance.MoveToTargetNPCAndInteractive(cSVClassicBossData.NPCID);
            OnClick_Close();

            Sys_Team.Instance.DoTeamTarget(Sys_Team.DoTeamTargetType.GotoBoss, curEventId);
        }
        /// <summary>
        /// 查看悬赏令
        /// </summary>
        private void OnClick_CheckRewardOrder()
        {
            CSVClassicBoss.Data cSVClassicBossData = CSVClassicBoss.Instance.GetConfData(curEventId);
            if (null == cSVClassicBossData) return;
            UIManager.OpenUI(EUIID.UI_Adventure, false, new AdventurePrama { page = (uint)EAdventurePageType.Reward, cellValue = cSVClassicBossData.CriminalID });
        }
        /// <summary>
        /// 查看boss地点
        /// </summary>
        private void OnClick_CheckBossPlace()
        {
            CSVClassicBoss.Data cSVClassicBossData = CSVClassicBoss.Instance.GetConfData(curEventId);
            if (null == cSVClassicBossData) return;
            var args = new Sys_Map.TargetClassicBossParameter(cSVClassicBossData.MapID, curEventId);
            UIManager.OpenUI((int)EUIID.UI_Map, false, args);
            //go_NavigationTips.SetActive(true);
            //SetNavigationTips(go_NavigationTips.transform, curEventId);
        }
        /// <summary>
        /// 关闭boss地点
        /// </summary>
        private void OnClick_CloseBossPlace()
        {
            go_NavigationTips.SetActive(false);
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
            if (index < 0 || index >= list_Island.Count) return;
            trans.name = index.ToString();
            uint id = list_Island[index];
            CSVIsland.Data cSVIslandData = CSVIsland.Instance.GetConfData(id);
            if (null == cSVIslandData) return;

            LoadAssetAsyn(cSVIslandData.map_ui, trans, cSVIslandData.id);

            if ((index == 0 && curIsland == 0) || curIsland == id)
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
            if (!int.TryParse(go.name, out index))
                return;
            if (index < 0 || index >= list_Island.Count)
                return;
            uint id = list_Island[index];
            curIsland = id;
            SwitchSelectView(id);
            cP_PageDot.SetSelected(index);
        }
        #endregion
        #region 提供功能
        #endregion
    }
}