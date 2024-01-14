using System;
using System.Collections.Generic;

using Lib.Core;

using Logic.Core;

using Table;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using static Logic.TaskHelper;

namespace Logic {
    /// <summary> 地图画像(显示地图内容) </summary>
    public class UI_MapImage : UIComponent
    {
        public class MapNpcNode : UIComponent {
            public MapNpc npc;

            protected override void Loaded() {
                
            }

            public void Refresh(MapNpc npc, uint mapId, UI_Map uiMap, UI_MapImage mapImage) {
                this.npc = npc;

                if (npc.TryGetPos(mapId, out Vector3 pos, out bool inThisMap)) {
                    if (!inThisMap) {
                        pos = UI_MapImage.ToVector3(pos);
                        pos = UI_MapImage.ScreenPointToLocalPoint(pos, mapImage.rawImage_BgMap.rectTransform.sizeDelta, mapImage.cSVMapInfoData?.ui_pos, mapImage.mapSize);
                        transform.localPosition = new Vector3(pos.x, pos.y, 0f);
                    }
                    else {
                        transform.localPosition = UI_MapImage.ScreenPointToLocalPoint(pos, mapImage.rawImage_BgMap.rectTransform.sizeDelta, mapImage.cSVMapInfoData?.ui_pos, mapImage.mapSize);
                    }

                    var curTrans = transform;
                    FrameworkTool.CreateChild(in curTrans, in uiMap.tags, in npc.taskCatagaries);
                }
            }
        }

        public COWVd<MapNpcNode> nodes = new COWVd<MapNpcNode>();

        #region 界面组件
        /// <summary> 背景地图 </summary>
        private RawImage rawImage_BgMap;
        /// <summary> 玩家标记 </summary>
        private RectTransform rt_PlayerPoint;
        /// <summary> 资源标记 </summary>
        private RectTransform rt_ResMarkPoint;
        /// <summary> 宠物区域 </summary>
        private Image image_PetArea;
        private RectTransform classBossArea;
        private Image image_classBossArea;
        /// <summary> 资源选项组 </summary>
        private ToggleGroup toggleGroup_ResourcePoint;
        /// <summary> 资源标记菜单 </summary>
        private Toggle toggle_ResourcePoint;
        /// <summary> 标记列表按钮 </summary>
        private Button button_MarkList;
        /// <summary> 标记列表按钮节点 </summary>
        private Transform tr_MarkListNode;
        /// <summary> 资源标记列表 </summary>
        private List<Toggle> list_ResourcePoint = new List<Toggle>();
        /// <summary> 目标标记 </summary>
        private RectTransform rt_Goal;
        /// <summary> 路径点 </summary>
        private RectTransform rt_PathPoint;
        /// <summary> 路径点父节点 </summary>
        private Transform tr_PathPointNode;

        public UI_Map uiMap;
        #endregion

        #region 数据定义
        /// <summary> 地图类型 </summary>
        public enum MapImageType
        {
            MiniMap, //小地图
            BigMap,  //大地图
        }
        /// <summary> 地图纹理类型 </summary>
        private MapImageType mapImageType
        {
            get;
            set;
        }
        /// <summary> 显示的地图数据 </summary>
        private CSVMapInfo.Data cSVMapInfoData
        {
            get;
            set;
        }
        /// <summary> 地图参数 </summary>
        private Sys_Map.MapParameter mapParameter
        {
            get;
            set;
        }
        /// <summary> 地图大小 </summary>
        private Vector2 mapSize
        {
            get;
            set;
        }
        /// <summary> 界面定位 </summary>
        private Vector3 fixedPosition;
        /// <summary> 玩家坐标 </summary>
        private Vector3 playerPosition;
        /// <summary> 平滑缓存速度 </summary>
        private Vector3 currentVelocity = Vector3.zero;
        /// <summary> 事件触发器 </summary>
        private Lib.Core.EventTrigger eventListener;
        /// <summary> 有默认选中菜单 </summary>
        private bool isOnToggle = false;
        #endregion

        #region 系统函数
        protected override void Loaded()
        {
            OnParseComponent();
        }
        public override void OnDestroy()
        {
            nodes.Clear();
            base.OnDestroy();
            ClearItemList();
        }
        public override void SetData(params object[] arg)
        {
            cSVMapInfoData = arg?.Length >= 1 ? CSVMapInfo.Instance.GetConfData((uint)arg[0]) : null;
            mapImageType = arg.Length >= 2 ? (MapImageType)arg[1] : MapImageType.BigMap;
            mapParameter = arg.Length >= 3 ? (Sys_Map.MapParameter)arg[2] : null;
            mapSize = cSVMapInfoData == null ? Vector2.zero : new Vector2(cSVMapInfoData.ui_length, cSVMapInfoData.ui_width);
        }
        public override void Show()
        {
            base.Show();
            Refresh();
        }
        public override void Hide()
        {
            UIManager.CloseUI(EUIID.UI_MapExploreDetail, false);
        }
        protected override void Update()
        {
            UpdatePlayerPoint(true);
            UpdatePath();
        }
        protected override void Refresh()
        {
            UpdateMap();

            List<Sys_Npc.MapResPointData> list_Data = null;
            if (cSVMapInfoData != null)
            {
                var data = Sys_Npc.Instance.GetNpcListInMap(cSVMapInfoData.id);
                if (data != null)
                    list_Data = new List<Sys_Npc.MapResPointData>(data.Values);
            }

            UpdatePlayerPoint(true);
            UpdateResourcePoint(list_Data);
            UpdateDynamicMarks();
            UpdatePetArea();
            UpdateClassBossArea(list_Data);
            UpdateMapPoint();
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
            rawImage_BgMap = transform.GetComponent<RawImage>();
            rt_PlayerPoint = transform.Find("Image_Person").GetComponent<RectTransform>();
            toggleGroup_ResourcePoint = transform.Find("ResourceNode").GetComponent<ToggleGroup>();
            toggle_ResourcePoint = transform.Find("ResourceNode/Toggle_Mark").GetComponent<Toggle>();
            toggle_ResourcePoint.gameObject.SetActive(false);
            SetToggleEvent(toggle_ResourcePoint);
            tr_MarkListNode = transform.Find("Grid").transform;
            button_MarkList = transform.Find("Grid/Btn01").GetComponent<Button>();
            button_MarkList.gameObject.SetActive(false);
            SetButtonEvent(button_MarkList);

            rt_Goal = transform.Find("Image_Find").GetComponent<RectTransform>();
            rt_PathPoint = transform.Find("FindNode/Image_Road").GetComponent<RectTransform>();
            tr_PathPointNode = transform.Find("FindNode").transform;
            image_PetArea = transform.Find("Image_PetArea").GetComponent<Image>();
            classBossArea = transform.Find("NpcLeaderWar").GetComponent<RectTransform>();
            image_classBossArea = transform.Find("NpcLeaderWar/Image_UnlockIcon").GetComponent<Image>();

            eventListener = Lib.Core.EventTrigger.Get(rawImage_BgMap.gameObject);
            eventListener.AddEventListener(EventTriggerType.PointerClick, OnClick_Move);
        }
        /// <summary>
        /// 注册回调事件
        /// </summary>
        private void OnProcessEventsForEnable(bool toRegister)
        {
            Sys_Npc.Instance.eventEmitter.Handle(Sys_Npc.EEvents.OnUpdateResPoint, UpdateResourcePoint, toRegister);
            Sys_Map.Instance.eventEmitter.Handle(Sys_Map.EEvents.OnHeroTel, OnHeroTel, toRegister);
            Sys_Map.Instance.eventEmitter.Handle(Sys_Map.EEvents.OnCloseSelectMark, OnCloseSelectMark, toRegister);
        }
        /// <summary>
        /// 设置模版
        /// </summary>
        /// <param name="count"></param>
        private void SetToggleItemList(Transform node, Transform child, int number)
        {
            while (list_ResourcePoint.Count < number)
            {
                Toggle toggle = GameObject.Instantiate(child, node).GetComponent<Toggle>();
                list_ResourcePoint.Add(toggle);
                SetToggleEvent(toggle);
            }
        }
        /// <summary>
        /// 设置菜单事件
        /// </summary>
        /// <param name="toggle"></param>
        private void SetToggleEvent(Toggle toggle)
        {
            toggle.onValueChanged.AddListener((bool value) => { OnClick_MapMark(toggle, value); });
        }
        /// <summary>
        /// 设置选项列表
        /// </summary>
        /// <param name="node"></param>
        /// <param name="child"></param>
        /// <param name="number"></param>
        private void SetSelectToggleList(Transform node, Transform child, int number)
        {
            while (node.childCount < number)
            {
                Button button = GameObject.Instantiate(child, node).GetComponent<Button>();
                SetButtonEvent(button);
            }
        }
        /// <summary>
        /// 设置按钮事件
        /// </summary>
        /// <param name="button"></param>
        private void SetButtonEvent(Button button)
        {
            button.onClick.AddListener(() => { OnClick_SelectMark(button.gameObject); });
        }
        /// <summary>
        /// 清理模版列表
        /// </summary>
        private void ClearItemList()
        {
            for (int i = 0, count = list_ResourcePoint.Count; i < count; i++)
            {
                var x = list_ResourcePoint[i];
                if (x != null) GameObject.Destroy(x);
            }
            list_ResourcePoint.Clear();
        }
        /// <summary>
        /// 设置路径列表
        /// </summary>
        /// <param name="maxCount"></param>
        private void SetWayPointList(int maxCount)
        {
            int count = maxCount - tr_PathPointNode.childCount;
            for (int i = 0; i < count; i++)
            {
                GameObject.Instantiate(rt_PathPoint, tr_PathPointNode);
            }
        }
        #endregion

        #region 界面显示
        /// <summary>
        /// 更新地图
        /// </summary>
        public void UpdateMap()
        {
            if (null == cSVMapInfoData) return;

            switch (mapImageType)
            {
                case MapImageType.MiniMap:
                    {
                        if (null != cSVMapInfoData.minimap_size && cSVMapInfoData.minimap_size.Count >= 2)
                            rawImage_BgMap.rectTransform.sizeDelta = new Vector2(cSVMapInfoData.minimap_size[0], cSVMapInfoData.minimap_size[1]);
                        else
                            rawImage_BgMap.rectTransform.anchoredPosition = Vector2.zero;
                    }
                    break;
                case MapImageType.BigMap:
                    {
                        if (null != cSVMapInfoData.ui_size && cSVMapInfoData.ui_size.Count >= 4)
                        {
                            rawImage_BgMap.rectTransform.anchoredPosition = new Vector2(cSVMapInfoData.ui_size[0], cSVMapInfoData.ui_size[1]);
                            rawImage_BgMap.rectTransform.sizeDelta = new Vector2(cSVMapInfoData.ui_size[2], cSVMapInfoData.ui_size[3]);
                        }
                        else
                        {
                            rawImage_BgMap.rectTransform.anchoredPosition = Vector2.zero;
                        }
                    }
                    break;
            }

            //更新背景地图
            switch (cSVMapInfoData.minimap_showtype)
            {
                case 1: //图片投影
                    {
                        ImageHelper.SetTexture(rawImage_BgMap, cSVMapInfoData.minimap_res);
                    }
                    break;
                default: //无地图
                    {
                        ImageHelper.SetTexture(rawImage_BgMap, null);
                        rawImage_BgMap.texture = null;
                    }
                    break;
            }
            //设置角色标记
            rt_PlayerPoint.gameObject.SetActive(cSVMapInfoData.id == Sys_Map.Instance.CurMapId);
        }
        /// <summary>
        /// 更新角色坐标
        /// </summary>
        public void UpdatePlayerPoint(bool Immediately = false)
        {
            if (cSVMapInfoData == null) return;
            if (cSVMapInfoData.id != Sys_Map.Instance.CurMapId) return;
            Vector3 position = Vector3.zero;
            if (null != GameCenter.mainHero && null != GameCenter.mainHero.transform)
                position = GameCenter.mainHero.transform.position;

            playerPosition = ScreenPointToLocalPoint(position, rawImage_BgMap.rectTransform.sizeDelta, cSVMapInfoData?.ui_pos, mapSize);

            if (Immediately)
                rt_PlayerPoint.localPosition = playerPosition;
            else
                rt_PlayerPoint.localPosition = Vector3.SmoothDamp(rt_PlayerPoint.localPosition, playerPosition, ref currentVelocity, 0.2f);
        }

        Vector3[] cachedPath = new Vector3[512];
        /// <summary>
        /// 更新路径
        /// </summary>
        public void UpdatePath()
        {
            var navMeshAgent = GameCenter.mainHero?.movementComponent?.mNavMeshAgent;

            if (null == navMeshAgent) return;

            if (!navMeshAgent.isActiveAndEnabled || cSVMapInfoData == null || cSVMapInfoData.id != Sys_Map.Instance.CurMapId)
            {
                if (tr_PathPointNode.gameObject.activeInHierarchy)
                    tr_PathPointNode.gameObject.SetActive(false);

                if (rt_Goal.gameObject.activeInHierarchy)
                    rt_Goal.gameObject.SetActive(false);
                return;
            }

            if ((navMeshAgent.isOnNavMesh && navMeshAgent.isStopped) || !navMeshAgent.hasPath)
            {
                if (tr_PathPointNode.gameObject.activeInHierarchy)
                    tr_PathPointNode.gameObject.SetActive(false);

                if (rt_Goal.gameObject.activeInHierarchy)
                    rt_Goal.gameObject.SetActive(false);
                return;
            }
            int pathCount = navMeshAgent.path.GetCornersNonAlloc(cachedPath);
            if (pathCount <= 0)
            {
                if (tr_PathPointNode.gameObject.activeInHierarchy)
                    tr_PathPointNode.gameObject.SetActive(false);

                if (rt_Goal.gameObject.activeInHierarchy)
                    rt_Goal.gameObject.SetActive(false);
                return;
            }

            //设置足够的路径线
            SetWayPointList(pathCount);
            RectTransform rt = null;
            Vector2 v1 = Vector2.zero, v2 = Vector2.zero;
            tr_PathPointNode.gameObject.SetActive(true);

            for (int i = 0, count = tr_PathPointNode.childCount; i < count; i++)
            {

                int j1 = pathCount - 1 - i;
                int j2 = pathCount - 2 - i;

                if (j2 >= 0)
                {
                    v1 = ScreenPointToLocalPoint(cachedPath[j1], rawImage_BgMap.rectTransform.sizeDelta, cSVMapInfoData?.ui_pos, mapSize);
                    v2 = ScreenPointToLocalPoint(cachedPath[j2], rawImage_BgMap.rectTransform.sizeDelta, cSVMapInfoData?.ui_pos, mapSize);

                    var distance = Vector2.Distance(v1, v2);
                    rt = tr_PathPointNode.GetChild(i) as RectTransform;
                    rt.gameObject.SetActive(true);
                    rt.localPosition = v1;
                    rt.localRotation = Quaternion.AngleAxis(GetAngle(v1, v2), Vector3.back);
                    rt.sizeDelta = new Vector2(Mathf.Max(distance + 4), 4);
                }
                else
                {
                    tr_PathPointNode.GetChild(i).gameObject.SetActive(false);
                }
            }

            //设置目标点
            if (cSVMapInfoData.id != Sys_Map.Instance.CurMapId)
            {
                if (rt_Goal.gameObject.activeInHierarchy)
                    rt_Goal.gameObject.SetActive(false);
            }
            else
            {
                if (!rt_Goal.gameObject.activeInHierarchy)
                    rt_Goal.gameObject.SetActive(true);
                rt_Goal.localPosition = ScreenPointToLocalPoint(navMeshAgent.pathEndPosition, rawImage_BgMap.rectTransform.sizeDelta, cSVMapInfoData?.ui_pos, mapSize);
                rt_Goal.localEulerAngles = tr_PathPointNode.GetChild(0).localEulerAngles + Vector3.forward * 180f;
            }
        }
        
        private List<uint> markTypes = new List<uint>();
        private UI_MapExplore.ETabType tabType;
        public void CtrlMarkType(UI_MapExplore.ETabType leftTabType, List<uint> markTypes, uint mapId) {
            this.markTypes = markTypes;
            this.tabType = leftTabType;
            
            // 这里不应该修改数据，但是为了方便，还是这样做了
            this.cSVMapInfoData = CSVMapInfo.Instance.GetConfData(mapId);
            this.SetToggleListNode(null, false);    
            
            this.UpdateResourcePoint();
        }

        /// <summary>
        /// 更新资源点
        /// </summary>
        private void UpdateResourcePoint() {
            List<Sys_Npc.MapResPointData> list_Data = new List<Sys_Npc.MapResPointData>(Sys_Npc.Instance.GetNpcListInMap(cSVMapInfoData.id).Values);
            UpdateResourcePoint(list_Data);
        }

        public void UpdateResourcePoint(List<Sys_Npc.MapResPointData> list_Data)
        {
            if (null == cSVMapInfoData) return;

            isOnToggle = false;
            rt_ResMarkPoint = null;
            SetToggleItemList(toggle_ResourcePoint.transform.parent, toggle_ResourcePoint.transform, list_Data.Count);
            //更新资源点
            Sys_Npc.MapResPointData mapResPointData = null;
            for (int i = 0; i < list_ResourcePoint.Count; i++)
            {
                if (list_Data.Count > i)
                {
                    mapResPointData = list_Data[i];
                    SetResourcePointItem(list_ResourcePoint[i], mapResPointData);
                }
                else
                {
                    SetResourcePointItem(list_ResourcePoint[i], null);
                }
            }
            if (!isOnToggle)
                IsTogglesOn();
        }

        private List<MapNpc> mapNpcs = new List<MapNpc>();
        private void UpdateDynamicMarks() {
            if (cSVMapInfoData != null && uiMap != null) {
                uint mapId = cSVMapInfoData.id;
                var markNode = transform.Find("DynamicMarks/Grid_Mark");
                if (markNode != null) {
                    mapNpcs = TaskHelper.GetMapNpcs(uiMap.mapNpcs, mapId);
                    nodes.TryBuildOrRefresh(markNode.gameObject, markNode.parent, mapNpcs.Count, OnRefresh, null);
                }
            }
        }
        private void OnRefresh(MapNpcNode vd, int index) {
            uint mapId = cSVMapInfoData.id;
            vd.Refresh(mapNpcs[index], mapId, uiMap, this);
        }
        
        private Dictionary<uint, uint> npcId2MarkType = new Dictionary<uint, uint>();

        public void SetResourcePointItem(Toggle toggle, Sys_Npc.MapResPointData data)
        {
            CSVMapExplorationMark.Data cSVMapExplorationMarkData = null == data ? null : CSVMapExplorationMark.Instance.GetConfData(data.subMarkType);
            if (null == cSVMapExplorationMarkData || cSVMapExplorationMarkData.resource_icon1 == 0 || cSVMapExplorationMarkData.resource_icon2 == 0)
            {
                toggle.gameObject.SetActive(false);
                return;
            }
            
            uint markId = data.mainMarkType;
            npcId2MarkType[data.npcId] = markId;

            bool isL = (cSVMapExplorationMarkData.tab_type & (int) UI_MapExplore.ETabType.Explore) == (int) UI_MapExplore.ETabType.Explore;
            bool isR = (cSVMapExplorationMarkData.tab_type & (int) UI_MapExplore.ETabType.Resource) == (int) UI_MapExplore.ETabType.Resource;
            bool isLR = isL && isR;
            if (!isLR) {
                bool notContains = (this.tabType == UI_MapExplore.ETabType.Explore && isR);
                notContains |= (this.tabType == UI_MapExplore.ETabType.Resource && isL);
                if (notContains) {
                    toggle.gameObject.SetActive(false);
                    return;
                }
            }

            toggle.gameObject.SetActive(true);
            toggle.gameObject.name = data.npcId.ToString();
            toggle.enabled = mapImageType == MapImageType.BigMap;
            toggle.graphic.gameObject.SetActive(mapImageType == MapImageType.BigMap);
            CSVNpc.Data cSVNpcData = CSVNpc.Instance.GetConfData(data.npcId);
            bool interactable = cSVMapExplorationMarkData.allow_click;
            toggle.interactable = interactable;
            Image image = toggle.transform.GetComponent<Image>();
            
            bool isOpen = Sys_Exploration.Instance.IsOpen_ResPoint(markId);
            image.gameObject.SetActive(isOpen);
            ImageHelper.SetIcon(image, data.markState ? cSVMapExplorationMarkData.resource_icon2 : cSVMapExplorationMarkData.resource_icon1, true);
            image.rectTransform.localPosition = ScreenPointToLocalPoint(ToVector3(data.position), rawImage_BgMap.rectTransform.sizeDelta, cSVMapInfoData?.ui_pos, mapSize);
            image.raycastTarget = mapImageType == MapImageType.BigMap && interactable;
            
            RectTransform rt_Name = image.transform.Find("Image_Label") as RectTransform;
            Text text_Name = image.transform.Find("Image_Label/Text").GetComponent<Text>();
            if (cSVNpcData?.mark_lan != 0)
            {
                rt_Name.gameObject.SetActive(true);
                rt_Name.anchoredPosition = cSVNpcData?.mark_move == null ? Vector2.zero : new Vector2(cSVNpcData.mark_move[0], cSVNpcData.mark_move[1]);
                text_Name.text = LanguageHelper.GetNpcTextContent(cSVNpcData.mark_lan);
            }
            else
            {
                rt_Name.gameObject.SetActive(false);
            }

            bool isMark = false;
            if (null != mapParameter)
            {
                var mapParameterType = mapParameter.GetType();

                if (mapParameterType == typeof(Sys_Map.ResMarkParameter))
                {
                    Sys_Map.ResMarkParameter resMarkParameter = (Sys_Map.ResMarkParameter)mapParameter;
                    if (resMarkParameter.resMarkType == data.mainMarkType && !data.markState)
                    {
                        isMark = true;
                    }
                }
                else if (mapParameterType == typeof(Sys_Map.TargetNpcParameter))
                {
                    Sys_Map.TargetNpcParameter targetNpcParameter = (Sys_Map.TargetNpcParameter)mapParameter;
                    if (targetNpcParameter.targetMapId == cSVMapInfoData.id && targetNpcParameter.targetNpcId == data.npcId)
                    {
                        isMark = true;
                        if (toggle.enabled && toggle.interactable)
                        {
                            toggle.isOn = true;
                            isOnToggle = true;
                        }
                    }
                }
            }
            if (isMark && rt_ResMarkPoint == null)
            {
                rt_ResMarkPoint = image.rectTransform;
            }
        }
        /// <summary>
        /// 设置地图点
        /// </summary>
        public void UpdateMapPoint()
        {
            if (!IsFixedPosition())
                return;

            RectTransform rt_MapMask = rawImage_BgMap.transform.parent as RectTransform;
            RectTransform rt_MapImage = rawImage_BgMap.rectTransform;

            Vector3 markCenter = new Vector2(rt_MapMask.rect.width / 2, -rt_MapMask.rect.height / 2);
            float mapDifferencex = rt_MapImage.rect.width - rt_MapMask.rect.width;
            float mapDifferencey = rt_MapImage.rect.height - rt_MapMask.rect.height;
            if (mapDifferencex <= 0 && mapDifferencey <= 0) return;

            Vector3 vector3 = UpdateFixedPosition();
            Vector3 offset = markCenter - rt_MapImage.InverseTransformPoint(vector3);

            if (offset.x >= 0)//左边界
            {
                offset.x = 0;
            }
            else if (offset.x <= -mapDifferencex)//右边界
            {
                offset.x = -mapDifferencex;
            }
            if (offset.y <= 0)//上边界
            {
                offset.y = 0;
            }
            else if (offset.y >= mapDifferencey)//下边界
            {
                offset.y = mapDifferencey;
            }

            rawImage_BgMap.rectTransform.localPosition = offset;
        }
        /// <summary>
        /// 更新抓宠区域
        /// </summary>
        public void UpdatePetArea()
        {
            if (null == mapParameter || mapParameter.GetType() != typeof(Sys_Map.CatchPetParameter))
            {
                image_PetArea.gameObject.SetActive(false);
                return;
            }
            Sys_Map.CatchPetParameter catchPetParameter = (Sys_Map.CatchPetParameter)mapParameter;
            if (catchPetParameter.catchPetMapId != cSVMapInfoData.id)
            {
                image_PetArea.gameObject.SetActive(false);
                return;
            }
            image_PetArea.gameObject.SetActive(true);
            image_PetArea.rectTransform.localPosition = ScreenPointToLocalPoint(ToVector3(catchPetParameter.catchPetCenter), rawImage_BgMap.rectTransform.sizeDelta, cSVMapInfoData?.ui_pos, mapSize);
            image_PetArea.rectTransform.sizeDelta = catchPetParameter.catchPetRange;
        }
        // 更新npc区域
        public void UpdateClassBossArea(List<Sys_Npc.MapResPointData> list_Data) {
            if (null == mapParameter || mapParameter.GetType() != typeof(Sys_Map.TargetClassicBossParameter)) {
                classBossArea.gameObject.SetActive(false);
                return;
            }
            Sys_Map.TargetClassicBossParameter parameter = (Sys_Map.TargetClassicBossParameter)mapParameter;
            var csvClassBoss = CSVClassicBoss.Instance.GetConfData(parameter.classicBossId);
            if (parameter == null || parameter.targetMapId != cSVMapInfoData.id || csvClassBoss == null) {
                classBossArea.gameObject.SetActive(false);
                return;
            }

            for (int i = 0, length = list_Data.Count; i < length; ++i) {
                var mapResPointData = list_Data[i];
                if (mapResPointData.mainMarkType == 14 &&
                    mapResPointData.npcId == csvClassBoss.NPCID) {
                    classBossArea.gameObject.SetActive(true);
                    classBossArea.localPosition = ScreenPointToLocalPoint(ToVector3(mapResPointData.position), rawImage_BgMap.rectTransform.sizeDelta, cSVMapInfoData?.ui_pos, mapSize);

                    ImageHelper.SetIcon(image_classBossArea, csvClassBoss.Icon);
                    break;
                }
            }
        }
        /// <summary>
        /// 更新界面固定位置
        /// </summary>
        public Vector3 UpdateFixedPosition()
        {
            var mapParameterType = null == mapParameter ? null : mapParameter.GetType();

            if (null == mapParameterType)
            {
                fixedPosition = rt_PlayerPoint.position;
            }
            else if (mapParameterType == typeof(Sys_Map.TargetMapParameter))
            {
                Sys_Map.TargetMapParameter targetMapParameter = (Sys_Map.TargetMapParameter)mapParameter;
                if (targetMapParameter.fixedPosition == Vector3.zero)
                {
                    fixedPosition = rt_PlayerPoint.position;
                }
                else
                {
                    RectTransform rt_MapMask = rawImage_BgMap.transform.parent as RectTransform;
                    Vector3 markCenter = new Vector2(rt_MapMask.rect.width / 2, -rt_MapMask.rect.height / 2);
                    RectTransform rt_MapImage = rawImage_BgMap.rectTransform;
                    fixedPosition = rt_MapImage.TransformPoint(markCenter - targetMapParameter.fixedPosition);
                }
            }
            else if (mapParameterType == typeof(Sys_Map.ResMarkParameter))
            {
                if(rt_ResMarkPoint)
                    fixedPosition = rt_ResMarkPoint.position;
            }
            else if (mapParameterType == typeof(Sys_Map.CatchPetParameter))
            {
                if (image_PetArea && image_PetArea.transform)
                    fixedPosition = image_PetArea.transform.position;
            }
            else if (mapParameterType == typeof(Sys_Map.TargetNpcParameter))
            {
                if (rt_ResMarkPoint)
                    fixedPosition = rt_ResMarkPoint.position;
            }
            return fixedPosition;
        }
        /// <summary>
        /// 显示列表
        /// </summary>
        /// <param name="toggle"></param>
        /// <param name="Idlist"></param>
        public void ShowToggleList(Toggle toggle, List<uint> Idlist)
        {
            bool isLeftOrRight = ShowLeftOrRight(toggle);
            Vector3 offfect = isLeftOrRight ? Vector3.left * 90 : Vector3.right * 90;
            tr_MarkListNode.localPosition = toggle.transform.localPosition + offfect;
            tr_MarkListNode.localEulerAngles = isLeftOrRight ? Vector3.up * 180f : Vector3.zero;

            SetSelectToggleList(tr_MarkListNode, button_MarkList.transform, Idlist.Count);
            for (int i = 0; i < tr_MarkListNode.childCount; i++)
            {
                Transform child = tr_MarkListNode.GetChild(i);
                if (Idlist.Count > i)
                    SetSelectToggleItem(child, Idlist[i], isLeftOrRight);
                else
                    SetSelectToggleItem(child, 0, isLeftOrRight);
            }
        }
        /// <summary>
        /// 设置菜单列表节点
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="value"></param>
        public void SetToggleListNode(Transform tr, bool value)
        {
            tr_MarkListNode.gameObject.SetActive(value);
        }
        /// <summary>
        /// 设置选项菜单
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="id"></param>
        public void SetSelectToggleItem(Transform tr, uint id, bool isLeftOrRight)
        {
            CSVNpc.Data cSVNpcData = CSVNpc.Instance.GetConfData(id);
            if (null == cSVNpcData)
            {
                tr.gameObject.SetActive(false);
                return;
            }
            uint taskId = Sys_Npc.Instance.GetTaskId(cSVNpcData);
            uint markId = Sys_Npc.Instance.GetMarkId(cSVNpcData);

            CSVMapExplorationMark.Data cSVMapExplorationMarkData = CSVMapExplorationMark.Instance.GetConfData(markId);
            if (null == cSVMapExplorationMarkData)
            {
                tr.gameObject.SetActive(false);
                return;
            }

            tr.gameObject.SetActive(true);
            tr.name = id.ToString();
            /// <summary> 图标 </summary>
            Image image_Icon = tr.Find("Image").GetComponent<Image>();
            /// <summary> 名字 </summary>
            Text text_Name = tr.Find("Text").GetComponent<Text>();
            if (Sys_Npc.Instance.IsTaskNpc(cSVNpcData))
            {
                CSVTask.Data taskData = CSVTask.Instance.GetConfData(taskId);
                text_Name.text = LanguageHelper.GetTaskTextContent(taskData.taskName);
                ImageHelper.SetIcon(image_Icon, cSVMapExplorationMarkData.List_Icon);
            }
            else if (Sys_Npc.Instance.IsTransmitNpc(cSVNpcData))
            {
                text_Name.text = LanguageHelper.GetNpcTextContent(cSVNpcData.name);
                ImageHelper.SetIcon(image_Icon, cSVMapExplorationMarkData.List_Icon);
            }
            else // if (Sys_Npc.Instance.IsResourcesNpc(cSVNpcData))
            {
                text_Name.text = LanguageHelper.GetNpcTextContent(cSVNpcData.name);
                ImageHelper.SetIcon(image_Icon, cSVMapExplorationMarkData.resource_icon2);
            }

            if (isLeftOrRight)
            {
                image_Icon.transform.localEulerAngles = Vector3.up * 180f;
                text_Name.transform.localEulerAngles = Vector3.up * 180f;
                text_Name.alignment = TextAnchor.MiddleRight;
            }
            else
            {
                image_Icon.transform.localEulerAngles = Vector3.zero;
                text_Name.transform.localEulerAngles = Vector3.zero;
                text_Name.alignment = TextAnchor.MiddleLeft;
            }
        }
        #endregion

        #region 响应事件
        /// <summary>
        /// 选中地图标记
        /// </summary>
        /// <param name="toggle"></param>
        /// <param name="value"></param>
        public void OnClick_MapMark(Toggle toggle, bool value)
        {
            if (value)
            {
                List<uint> list = GetOtherMarkInRangeList(toggle);
                if (list.Count <= 1)
                {
                    uint npcId = 0;
                    uint.TryParse(toggle.name, out npcId);
                    UIManager.UpdateState();
                    npcId2MarkType.TryGetValue(npcId, out uint markType);
                    UIManager.OpenUI(EUIID.UI_MapExploreDetail, false, new Tuple<uint, uint>(npcId, markType));
                    SetToggleListNode(toggle.transform, false);
                }
                else
                {
                    SetToggleListNode(toggle.transform, true);
                    ShowToggleList(toggle, list);
                }
            }
            else
            {
                UIManager.CloseUI(EUIID.UI_MapExploreDetail);
                SetToggleListNode(toggle.transform, false);
            }
        }
        /// <summary>
        /// 点击分支标记
        /// </summary>
        /// <param name="go"></param>
        public void OnClick_SelectMark(GameObject go)
        {
            uint npcId = 0;
            uint.TryParse(go.name, out npcId);
            npcId2MarkType.TryGetValue(npcId, out uint markType);
            UIManager.OpenUI(EUIID.UI_MapExploreDetail, false, new Tuple<uint, uint>(npcId, markType));
            SetToggleListNode(go.transform.parent.parent, false);
        }
        /// <summary>
        /// 点击移动
        /// </summary>
        /// <param name="eventData"></param>
        public void OnClick_Move(BaseEventData eventData)
        {
            if (eventListener.IsDraging == true) return;

            if (mapImageType == MapImageType.MiniMap) return;

            if (Sys_Team.Instance.isMainHeroFollowed()) return;

            if (Sys_Fight.Instance.IsFight())
            {
                Sys_Hint.Instance.PushForbidOprationInFight();
                return;
            }

            if (IsTogglesOn()) return;

            if (cSVMapInfoData.MinimapPathFinding) {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4591));
                return;
            }

            PointerEventData ped = eventData as PointerEventData;
            //点击界面上的坐标
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rawImage_BgMap.rectTransform, ped.position, UIManager.mUICamera, out localPoint);
            //转换成地图坐标
            Vector2 screenPoint = ToVector2(LocalPointToScreenPoint(localPoint));
            //寻路

            if (cSVMapInfoData.id == Sys_Map.Instance.CurMapId)
            {
                if (Sys_Pet.Instance.clientStateId != Sys_Role.EClientState.None) {
                    Sys_Pet.Instance.ForceStop();
                }

                Sys_PathFind.Instance.PathFindId = EUIID.UI_PathFind;
                GameCenter.mPathFindControlSystem.FindMapPoint(cSVMapInfoData.id, screenPoint, (pos) => { });
            }
            else
            {

                SkipToDiffcentMap(screenPoint);

            }
        }


        private void SkipToDiffcentMap(Vector2 screenPoint)
        {
           bool isSurvivalMap = Sys_SurvivalPvp.Instance.isSurvivalPvpMap(Sys_Map.Instance.CurMapId);

            PromptBoxParameter.Instance.Clear();
            PromptBoxParameter.Instance.content = isSurvivalMap ? LanguageHelper.GetTextContent(2022439) :CSVLanguage.Instance.GetConfData(4522).words;
            PromptBoxParameter.Instance.SetConfirm(true, () =>
            {
                if (Sys_Pet.Instance.clientStateId != Sys_Role.EClientState.None) {
                    Sys_Pet.Instance.ForceStop();
                }
                
                Sys_PathFind.Instance.PathFindId = EUIID.UI_PathFind;
                GameCenter.mPathFindControlSystem.FindMapPoint(cSVMapInfoData.id, screenPoint, (pos) => { });
                Sys_Map.Instance.eventEmitter.Trigger(Sys_Map.EEvents.OnCloseMapView);
                Sys_Adventure.Instance.eventEmitter.Trigger(Sys_Adventure.EEvents.OnCLoseAdventureView);
            });
            PromptBoxParameter.Instance.SetCancel(true, null);
            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
        }
        /// <summary>
        /// 英雄传送
        /// </summary>
        public void OnHeroTel()
        {
            if (cSVMapInfoData.id == Sys_Map.Instance.CurMapId)
            {
                UpdatePlayerPoint(true);
            }
            else
            {
                Refresh();
            }
        }
        /// <summary>
        /// 关闭选项标记
        /// </summary>
        public void OnCloseSelectMark()
        {
            CloseToggleGroup();
        }
        #endregion

        #region 提供功能
        /// <summary>
        /// 转换Vector3坐标
        /// </summary>
        /// <param name="posX"></param>
        /// <param name="posY"></param>
        /// <returns></returns>
        public static Vector3 ToVector3(Vector2 pos)
        {
            return new Vector3(pos.x, 0, pos.y);
        }
        /// <summary>
        /// 转换Vector2坐标
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public static Vector2 ToVector2(Vector3 pos)
        {
            return new Vector2(pos.x, pos.z);
        }
        /// <summary>
        /// 将场景坐标点转换成本地UI坐标点
        /// </summary>
        /// <param name="vector3"></param>
        /// <returns></returns>
        public static Vector3 ScreenPointToLocalPoint(Vector3 vector3, Vector2 mapImage, List<float> list, Vector2 mapSize)
        {
            Vector3 v3 = Vector3.zero;
            Vector3 v3_Screen = Vector3.zero;
            if (null != list && list.Count >= 2)
            {
                v3_Screen = vector3 + new Vector3(list[0], 0, list[1]);
            }
            else
            {
                v3_Screen = vector3;
            }

            v3.x = mapSize.x == 0 ? 0 : v3_Screen.x / mapSize.x * mapImage.x;
            v3.y = mapSize.y == 0 ? 0 : v3_Screen.z / mapSize.y * mapImage.y;
            v3.z = 0;
            return v3;
        }
        /// <summary>
        /// 将本地UI坐标点转换成场景坐标点
        /// </summary>
        /// <param name="vector3"></param>
        /// <returns></returns>
        public Vector3 LocalPointToScreenPoint(Vector3 vector3)
        {
            Vector3 v3 = Vector3.zero;
            Vector2 mapImage = rawImage_BgMap.rectTransform.sizeDelta;
            v3.x = vector3.x / mapImage.x * mapSize.x;
            v3.y = 0;
            v3.z = vector3.y / mapImage.y * mapSize.y;

            var list = cSVMapInfoData?.ui_pos;
            if (null != list && list.Count >= 2)
            {
                v3 -= new Vector3(list[0], 0, list[1]);
            }
            return v3;
        }
        /// <summary>
        /// 得到2个坐标的角度
        /// </summary>
        /// <param name="startPos"></param>
        /// <param name="endPos"></param>
        /// <returns></returns>
        public static float GetAngle(Vector3 startPos, Vector3 endPos)
        {
            var dir = startPos - endPos;
            var dirV2 = new Vector2(dir.x, dir.y);
            var angle = Vector2.SignedAngle(dirV2, Vector2.left);
            return angle;
        }
        /// <summary>
        /// 是否需要固定偏移坐标
        /// </summary>
        /// <returns></returns>
        public bool IsFixedPosition()
        {
            if (mapImageType != MapImageType.BigMap)
                return false; //大地图不需要偏移

            uint targetMapId = null == cSVMapInfoData ? 0 : cSVMapInfoData.id;
            bool isCurMapId = targetMapId == Sys_Map.Instance.CurMapId;
            if (isCurMapId)
                return true;//当前地图需要偏移

            var mapParameterType = null == mapParameter ? null : mapParameter.GetType();
            if (mapParameterType == typeof(Sys_Map.TargetMapParameter))
            {
                Sys_Map.TargetMapParameter targetMapParameter = (Sys_Map.TargetMapParameter)mapParameter;

                return targetMapId == targetMapParameter.targetMapId; //是目标地图需要偏移
            }
            else if (mapParameterType == typeof(Sys_Map.CatchPetParameter))
            {
                Sys_Map.CatchPetParameter catchPetParameter = (Sys_Map.CatchPetParameter)mapParameter;
                return targetMapId == catchPetParameter.catchPetMapId;//是目标地图需要偏移
            }
            else if (mapParameterType == typeof(Sys_Map.TargetNpcParameter))
            {
                Sys_Map.TargetNpcParameter targetNpcParameter = (Sys_Map.TargetNpcParameter)mapParameter;
                return targetMapId == targetNpcParameter.targetMapId; //是目标地图需要偏移
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 关闭选项
        /// </summary>
        /// <returns></returns>
        public void CloseToggleGroup()
        {
            var toggles = toggleGroup_ResourcePoint.ActiveToggles();
            foreach (var toggle in toggles)
            {
                if (toggle.isOn)
                    toggle.isOn = false;
            }
        }
        /// <summary>
        /// 是否有菜单激活
        /// </summary>
        /// <returns></returns>
        public bool IsTogglesOn()
        {
            bool isOn = toggleGroup_ResourcePoint.AnyTogglesOn();
            if (isOn)
            {
                CloseToggleGroup();
            }
            return isOn;
        }
        /// <summary>
        /// 在范围中的其他标记列表
        /// </summary>
        /// <param name="toggle"></param>
        /// <returns></returns>
        public List<uint> GetOtherMarkInRangeList(Toggle toggle)
        {
            float radius = 0;
            if (float.TryParse(CSVParam.Instance.GetConfData(936).str_value, out radius))
            {
                radius = radius / 10000f;
            }
            else
            {
                radius = 1f;
            }
            List<uint> list = new List<uint>();
            for (int i = 0, count = list_ResourcePoint.Count; i < count; i++)
            {
                Toggle t = list_ResourcePoint[i];
                if (!t.gameObject.activeInHierarchy) //未显示不进入列表
                    continue;

                uint npcId = 0;
                if (!uint.TryParse(t.name, out npcId))
                    continue;

                CSVNpc.Data cSVNpcData = CSVNpc.Instance.GetConfData(npcId);
                if (null == cSVNpcData)
                    continue;

                npcId2MarkType.TryGetValue(npcId, out uint markType);
                var csvMark = CSVMapExplorationMark.Instance.GetConfData(markType);
                if (csvMark == null || !csvMark.allow_click)
                    continue;

                if (!Sys_Npc.Instance.IsOpenNpc(cSVNpcData))
                    continue;

                if (IsInRange(radius, toggle.transform.localPosition, t.transform.localPosition))
                {
                    list.Add(npcId);
                }
            }
            return list;
        }
        /// <summary>
        /// 是否在范围内
        /// </summary>
        /// <param name="radius"></param>
        /// <param name="center"></param>
        /// <param name="point"></param>
        /// <returns></returns>
        public bool IsInRange(float radius, Vector2 center, Vector2 point)
        {
            Vector2 vector = point - center;
            return (vector.SqrMagnitude() <= (radius * radius));
        }
        /// <summary>
        /// 显示左或右
        /// </summary>
        /// <param name="toggle"></param>
        /// <returns></returns>
        public bool ShowLeftOrRight(Toggle toggle)
        {
            RectTransform rectTransform = transform.parent.GetComponent<RectTransform>();
            Vector3 vector3 = rectTransform.InverseTransformPoint(toggle.transform.position);
            Vector3 cellSize = tr_MarkListNode.GetComponent<GridLayoutGroup>().cellSize;
            float borderX = rectTransform.sizeDelta.x;
            float curX = vector3.x;
            float sizeX = cellSize.x;

            if (curX + sizeX <= borderX)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        #endregion
    }
}
