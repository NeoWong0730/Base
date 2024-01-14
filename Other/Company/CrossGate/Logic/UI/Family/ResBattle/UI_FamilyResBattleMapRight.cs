using System.Collections.Generic;
using Logic.Core;
using Packet;
using Table;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Logic {
    // 持矿玩家
    public class UI_MapDynamicMarkerHoldPlayers : UIComponent {
        public class Player : UISelectableElement {
            public Image icon;
            public GameObject isBatting;
            public CP_Toggle toggle;

            public BattleRoleMiniMapData info;

            protected override void Loaded() {
                this.icon = this.transform.GetComponent<Image>();
                this.isBatting = this.transform.Find("Image_Batting").gameObject;

                this.toggle = this.transform.GetComponent<CP_Toggle>();
                this.toggle.onValueChanged.AddListener(this.Switch);
            }

            public void Switch(bool arg) {
                if (arg) {
                    UIManager.OpenUI(EUIID.UI_FamilyResBattleMapHoldResPlayer, false, this.info);
                }
            }
            public override void SetSelected(bool toSelected, bool force) {
                this.toggle.SetSelected(toSelected, true);
            }

            public void Refresh(BattleRoleMiniMapData info, Vector2 sizeDelta, List<float> list, Vector2 mapSize) {
                this.info = info;

                ImageHelper.SetIcon(this.icon, CSVFamilyResBattleResParameter.Instance.GetConfData(info.Extra.Resource).IconID, true);
                this.isBatting.SetActive(info.Extra.Fighting);
                Vector3 worldPos = PosConvertUtil.Svr2Client(info.PosX, info.PosY);
                var pos = UI_MapImage.ScreenPointToLocalPoint(worldPos, sizeDelta, list, mapSize);
                this.transform.localPosition = pos;
            }
        }

        public GameObject proto;
        public UIElementCollector<Player> vds = new UIElementCollector<Player>();
        public List<BattleRoleMiniMapData> roles = new List<BattleRoleMiniMapData>();

        protected override void Loaded() {
            this.proto = this.transform.Find("DynamicMarks/FamilyResBattleNode/Players/Image_Player").gameObject;
        }
        public override void OnDestroy() {
            this.vds.Clear();
            base.OnDestroy();
        }

        public Vector2 sizeDelta;
        public List<float> list;
        public Vector2 mapSize;
        public void RefreshBaseInfo(Vector2 sizeDelta, List<float> list, Vector2 mapSize) {
            this.sizeDelta = sizeDelta;
            this.list = list;
            this.mapSize = mapSize;
        }

        public void Refresh(List<BattleRoleMiniMapData> rs) {
            if (rs != null) {
                this.roles = rs;
            }

            this.vds.BuildOrRefresh<BattleRoleMiniMapData>(this.proto, this.proto.transform.parent, this.roles, (vd, id, indexOfVdList) => {
                vd.SetUniqueId(indexOfVdList);
                vd.Refresh(this.roles[indexOfVdList], this.sizeDelta, this.list, this.mapSize);
            });
        }
    }
    // 非持矿玩家
    public class UI_MapDynamicMarkerUnholdPlayers : UIComponent {
        public GameObject proto;
        public List<BattleRoleMiniMapData> roles = new List<BattleRoleMiniMapData>();
        public COWComponent<Transform> vds = new COWComponent<Transform>();

        protected override void Loaded() {
            this.proto = this.transform.Find("DynamicMarks/FamilyResBattleNode/Greens/Image_Green").gameObject;
        }
        public override void OnDestroy() {
            this.vds.Clear();
            base.OnDestroy();
        }

        public Vector2 sizeDelta;
        public List<float> list;
        public Vector2 mapSize;
        public void RefreshBaseInfo(Vector2 sizeDelta, List<float> list, Vector2 mapSize) {
            this.sizeDelta = sizeDelta;
            this.list = list;
            this.mapSize = mapSize;
        }

        public void Refresh(List<BattleRoleMiniMapData> rs) {
            if (rs != null) {
                this.roles = rs;
            }
            else {
                this.roles.Clear();
            }

            this.vds.TryBuildOrRefresh(this.proto, this.proto.transform.parent, this.roles.Count, this._OnRefresh);
        }

        private void _OnRefresh(Transform t, int index) {
            var info = this.roles[index];
            Vector3 worldPos = PosConvertUtil.Svr2Client(info.PosX, info.PosY);
            var pos = UI_MapImage.ScreenPointToLocalPoint(worldPos, this.sizeDelta, this.list, this.mapSize);
            t.localPosition = pos;
        }
    }

    public class UI_MapStaticMarker : UIComponent {
        public class StaticMarker : UIComponent {
            public Image image;
            public RectTransform rt_Name;
            public Text text_Name;
            public Toggle tg;

            public CSVMapExplorationMark.Data cSVMapExplorationMarkData;

            protected override void Loaded() {
                this.tg = this.transform.GetComponent<Toggle>();
                this.image = this.transform.GetComponent<Image>();
                this.rt_Name = this.transform.Find("Image_Label") as RectTransform;
                this.text_Name = this.image.transform.Find("Image_Label/Text").GetComponent<Text>();
            }

            public void Refresh(Sys_Npc.MapResPointData info, Vector2 sizeDelta, List<float> uipos, Vector2 mapSize) {
                this.cSVMapExplorationMarkData = CSVMapExplorationMark.Instance.GetConfData(info.subMarkType);

                if (this.cSVMapExplorationMarkData.resource_icon1 == 0 || this.cSVMapExplorationMarkData.resource_icon2 == 0) {
                    this.Hide();
                    return;
                }

                this.Show();
                CSVNpc.Data cSVNpcData = CSVNpc.Instance.GetConfData(info.npcId);
                bool isOpen = Sys_Exploration.Instance.IsOpen_ResPoint(info.mainMarkType);
                this.image.gameObject.SetActive(isOpen);
                if (isOpen) {
                    ImageHelper.SetIcon(this.image, info.markState ? this.cSVMapExplorationMarkData.resource_icon2 : this.cSVMapExplorationMarkData.resource_icon1, true);
                    this.image.rectTransform.localPosition = UI_MapImage.ScreenPointToLocalPoint(UI_MapImage.ToVector3(info.position), sizeDelta, uipos, mapSize);
                }

                if (cSVNpcData?.mark_lan != 0) {
                    this.rt_Name.gameObject.SetActive(true);
                    this.rt_Name.anchoredPosition = cSVNpcData?.mark_move == null ? Vector2.zero : new Vector2(cSVNpcData.mark_move[0], cSVNpcData.mark_move[1]);
                    this.text_Name.text = LanguageHelper.GetNpcTextContent(cSVNpcData.mark_lan);
                }
                else {
                    this.rt_Name.gameObject.SetActive(false);
                }
            }
        }

        // <summary> 资源标记 </summary>
        private RectTransform rt_ResMarkPoint;
        // <summary> 资源标记菜单 </summary>
        private Toggle toggle_ResourcePoint;
        // <summary> 资源标记列表 </summary>
        private readonly List<Toggle> list_ResourcePoint = new List<Toggle>();

        private readonly COWVd<StaticMarker> staticMarkers = new COWVd<StaticMarker>();

        public uint mapId;
        public List<Sys_Npc.MapResPointData> staticRes = new List<Sys_Npc.MapResPointData>();
        public Vector2 siseDelta;
        public Vector2 mapSize;

        private CSVMapInfo.Data cSVMapInfoData {
            get {
                return CSVMapInfo.Instance.GetConfData(this.mapId);
            }
        }

        protected override void Loaded() {
            this.toggle_ResourcePoint = this.transform.Find("ResourceNode/Toggle_Mark").GetComponent<Toggle>();
            this.toggle_ResourcePoint.gameObject.SetActive(false);
        }

        public override void OnDestroy() {
            this.staticMarkers.Clear();
            base.OnDestroy();
        }

        public void RefreshBaseInfo(uint mapId, bool showStaticMarker, Vector2 siseDelta, Vector2 mapSize, bool useToggle = true) {
            this.mapId = mapId;
            this.siseDelta = siseDelta;
            this.mapSize = mapSize;

            if (showStaticMarker) {
                if (this.cSVMapInfoData != null) {
                    var data = Sys_Npc.Instance.GetNpcListInMap(this.cSVMapInfoData.id);
                    if (data != null)
                        this.staticRes = new List<Sys_Npc.MapResPointData>(data.Values);
                }
                this.UpdateResourcePoint(useToggle);
            }
        }

        private bool useToggle = true;
        public void UpdateResourcePoint(bool useToggle = true) {
            if (null == this.cSVMapInfoData) return;

            this.useToggle = useToggle;
            this.rt_ResMarkPoint = null;
            this.staticMarkers.TryBuildOrRefresh(this.toggle_ResourcePoint.gameObject, this.toggle_ResourcePoint.transform.parent, this.staticRes.Count, this._OnRefresh);
        }

        private void _OnRefresh(StaticMarker vd, int index) {
            vd.Refresh(this.staticRes[index], this.siseDelta, this.cSVMapInfoData?.ui_pos, this.mapSize);
            vd.tg.enabled = this.useToggle;
        }
    }

    public class UI_FamilyResBattleMapController : UIComponent {
        public RawImage rawImage_BgMap;
        private Lib.Core.EventTrigger eventListener;

        // <summary> 玩家标记 </summary>
        private RectTransform rt_PlayerPoint;
        // <summary> 路径点父节点 </summary>
        private Transform tr_PathPointNode;
        // <summary> 路径点 </summary>
        private RectTransform rt_PathPoint;
        // <summary> 目标标记 </summary>
        private RectTransform rt_Goal;

        /// <summary> 平滑缓存速度 </summary>
        private Vector3 currentVelocity = Vector3.zero;
        // <summary> 界面定位 </summary>
        private Vector3 fixedPosition;

        private Vector3 playerPosition;

        public uint mapId;
        public bool trackPlayerPath;

        public CSVMapInfo.Data cSVMapInfoData {
            get {
                return CSVMapInfo.Instance.GetConfData(this.mapId);
            }
        }
        public Vector2 mapSize {
            get {
                return new Vector2(this.cSVMapInfoData.ui_length, this.cSVMapInfoData.ui_width);
            }
        }
        public UI_MapImage.MapImageType mapImageType {
            get {
                return UI_MapImage.MapImageType.BigMap;
            }
        }

        protected override void Loaded() {
            this.rawImage_BgMap = this.transform.GetComponent<RawImage>();
            this.rt_PlayerPoint = this.transform.Find("Image_Person").GetComponent<RectTransform>();
            this.rt_Goal = this.transform.Find("Image_Find").GetComponent<RectTransform>();
            this.rt_PathPoint = this.transform.Find("FindNode/Image_Road").GetComponent<RectTransform>();
            this.tr_PathPointNode = this.transform.Find("FindNode").transform;
        }

        protected override void Update() {
            this.UpdatePlayerPoint(true);
            this.UpdatePath();
        }

        public void UpdatePlayerPoint(bool Immediately = false) {
            Vector3 position = Vector3.zero;
            if (null != GameCenter.mainHero && null != GameCenter.mainHero.transform) {
                position = GameCenter.mainHero.transform.position;
            }

            this.playerPosition = UI_MapImage.ScreenPointToLocalPoint(position, this.rawImage_BgMap.rectTransform.sizeDelta, this.cSVMapInfoData?.ui_pos, this.mapSize);

            if (Immediately)
                this.rt_PlayerPoint.localPosition = this.playerPosition;
            else
                this.rt_PlayerPoint.localPosition = Vector3.SmoothDamp(this.rt_PlayerPoint.localPosition, this.playerPosition, ref this.currentVelocity, 0.2f);
        }

        private readonly Vector3[] cachedPath = new Vector3[512];
        public void UpdatePath() {
            var navMeshAgent = GameCenter.mainHero?.movementComponent?.mNavMeshAgent;

            if (null == navMeshAgent) {
                return;
            }

            if (!navMeshAgent.isActiveAndEnabled || this.cSVMapInfoData == null || this.cSVMapInfoData.id != Sys_Map.Instance.CurMapId) {
                if (this.tr_PathPointNode.gameObject.activeInHierarchy)
                    this.tr_PathPointNode.gameObject.SetActive(false);

                if (this.rt_Goal.gameObject.activeInHierarchy)
                    this.rt_Goal.gameObject.SetActive(false);
                return;
            }

            if ((navMeshAgent.isOnNavMesh && navMeshAgent.isStopped) || !navMeshAgent.hasPath) {
                if (this.tr_PathPointNode.gameObject.activeInHierarchy)
                    this.tr_PathPointNode.gameObject.SetActive(false);

                if (this.rt_Goal.gameObject.activeInHierarchy)
                    this.rt_Goal.gameObject.SetActive(false);
                return;
            }
            int pathCount = navMeshAgent.path.GetCornersNonAlloc(this.cachedPath);
            if (pathCount <= 0) {
                if (this.tr_PathPointNode.gameObject.activeInHierarchy)
                    this.tr_PathPointNode.gameObject.SetActive(false);

                if (this.rt_Goal.gameObject.activeInHierarchy)
                    this.rt_Goal.gameObject.SetActive(false);
                return;
            }

            // 设置足够的路径线
            this.SetWayPointList(pathCount);
            RectTransform rt = null;
            Vector2 v1 = Vector2.zero, v2 = Vector2.zero;
            this.tr_PathPointNode.gameObject.SetActive(true);

            for (int i = 0, count = this.tr_PathPointNode.childCount; i < count; i++) {
                int j1 = pathCount - 1 - i;
                int j2 = pathCount - 2 - i;

                if (j2 >= 0) {
                    v1 = UI_MapImage.ScreenPointToLocalPoint(this.cachedPath[j1], this.rawImage_BgMap.rectTransform.sizeDelta, this.cSVMapInfoData?.ui_pos, this.mapSize);
                    v2 = UI_MapImage.ScreenPointToLocalPoint(this.cachedPath[j2], this.rawImage_BgMap.rectTransform.sizeDelta, this.cSVMapInfoData?.ui_pos, this.mapSize);

                    var distance = Vector2.Distance(v1, v2);
                    rt = this.tr_PathPointNode.GetChild(i) as RectTransform;
                    rt.gameObject.SetActive(true);
                    rt.localPosition = v1;
                    rt.localRotation = Quaternion.AngleAxis(UI_MapImage.GetAngle(v1, v2), Vector3.back);
                    rt.sizeDelta = new Vector2(Mathf.Max(distance + 4), 4);
                }
                else {
                    this.tr_PathPointNode.GetChild(i).gameObject.SetActive(false);
                }
            }

            //设置目标点
            if (this.cSVMapInfoData.id != Sys_Map.Instance.CurMapId) {
                if (this.rt_Goal.gameObject.activeInHierarchy)
                    this.rt_Goal.gameObject.SetActive(false);
            }
            else {
                if (!this.rt_Goal.gameObject.activeInHierarchy)
                    this.rt_Goal.gameObject.SetActive(true);
                this.rt_Goal.localPosition = UI_MapImage.ScreenPointToLocalPoint(navMeshAgent.pathEndPosition, this.rawImage_BgMap.rectTransform.sizeDelta, this.cSVMapInfoData?.ui_pos, this.mapSize);
                this.rt_Goal.localEulerAngles = this.tr_PathPointNode.GetChild(0).localEulerAngles + Vector3.forward * 180f;
            }
        }

        private void SetWayPointList(int maxCount) {
            int count = maxCount - this.tr_PathPointNode.childCount;
            for (int i = 0; i < count; i++) {
                GameObject.Instantiate(this.rt_PathPoint, this.tr_PathPointNode);
            }
        }

        public Vector3 LocalPointToScreenPoint(Vector3 vector3) {
            Vector3 v3 = Vector3.zero;
            Vector2 mapImage = this.rawImage_BgMap.rectTransform.sizeDelta;
            v3.x = vector3.x / mapImage.x * this.mapSize.x;
            v3.y = 0;
            v3.z = vector3.y / mapImage.y * this.mapSize.y;

            var list = this.cSVMapInfoData?.ui_pos;
            if (null != list && list.Count >= 2) {
                v3 -= new Vector3(list[0], 0, list[1]);
            }
            return v3;
        }

        public void RefreshBaseInfo(uint id, bool trackPlayerPath) {
            this.mapId = id;
            this.trackPlayerPath = trackPlayerPath;

            this.InitMap();
            this.UpdatePlayerPoint(true);
            // UpdateMapPoint前必须先调用UpdatePlayerPoint，设置player位置
            this.UpdateMapPoint();

            //设置角色标记
            this.rt_PlayerPoint.gameObject.SetActive(trackPlayerPath && this.cSVMapInfoData.id == Sys_Map.Instance.CurMapId);
            if (trackPlayerPath) {
                this.eventListener = Lib.Core.EventTrigger.Get(this.rawImage_BgMap.gameObject);
                this.eventListener.AddEventListener(EventTriggerType.PointerClick, this.OnClick_Move);
            }
        }

        public void InitMap() {
            if (null == this.cSVMapInfoData) return;

            switch (this.mapImageType) {
                case UI_MapImage.MapImageType.MiniMap: {
                        if (null != this.cSVMapInfoData.minimap_size && this.cSVMapInfoData.minimap_size.Count >= 2)
                            this.rawImage_BgMap.rectTransform.sizeDelta = new Vector2(this.cSVMapInfoData.minimap_size[0], this.cSVMapInfoData.minimap_size[1]);
                        else
                            this.rawImage_BgMap.rectTransform.anchoredPosition = Vector2.zero;
                    }
                    break;
                case UI_MapImage.MapImageType.BigMap: {
                        if (null != this.cSVMapInfoData.ui_size && this.cSVMapInfoData.ui_size.Count >= 4) {
                            this.rawImage_BgMap.rectTransform.anchoredPosition = new Vector2(this.cSVMapInfoData.ui_size[0], this.cSVMapInfoData.ui_size[1]);
                            this.rawImage_BgMap.rectTransform.sizeDelta = new Vector2(this.cSVMapInfoData.ui_size[2], this.cSVMapInfoData.ui_size[3]);
                        }
                        else {
                            this.rawImage_BgMap.rectTransform.anchoredPosition = Vector2.zero;
                        }
                    }
                    break;
            }

            // 更新背景地图
            switch (this.cSVMapInfoData.minimap_showtype) {
                case 1: {
                        //图片投影
                        ImageHelper.SetTexture(this.rawImage_BgMap, this.cSVMapInfoData.minimap_res);
                    }
                    break;
                default: {
                        //无地图
                        ImageHelper.SetTexture(this.rawImage_BgMap, null);
                        this.rawImage_BgMap.texture = null;
                    }
                    break;
            }
        }
        public Vector3 UpdateFixedPosition() {
            this.fixedPosition = this.rt_PlayerPoint.position;
            return this.fixedPosition;
        }
        public bool IsFixedPosition() {
            if (this.mapImageType != UI_MapImage.MapImageType.BigMap)
                return false; //大地图不需要偏移

            uint targetMapId = null == this.cSVMapInfoData ? 0 : this.cSVMapInfoData.id;
            bool isCurMapId = targetMapId == Sys_Map.Instance.CurMapId;
            if (isCurMapId)
                return true;//当前地图需要偏移

            return false;
        }

        public void UpdateMapPoint() {
            if (!this.IsFixedPosition())
                return;

            if (!this.trackPlayerPath) {
                return;
            }

            RectTransform rt_MapMask = this.rawImage_BgMap.transform.parent as RectTransform;
            RectTransform rt_MapImage = this.rawImage_BgMap.rectTransform;

            Vector3 markCenter = new Vector2(rt_MapMask.rect.width / 2, -rt_MapMask.rect.height / 2);
            float mapDifferencex = rt_MapImage.rect.width - rt_MapMask.rect.width;
            float mapDifferencey = rt_MapImage.rect.height - rt_MapMask.rect.height;
            if (mapDifferencex <= 0 && mapDifferencey <= 0) return;

            Vector3 vector3 = this.UpdateFixedPosition();
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

            this.rawImage_BgMap.rectTransform.localPosition = offset;
        }

        public void OnClick_Move(BaseEventData eventData) {
            if (this.eventListener.IsDraging == true) { return; }
            if (Sys_Team.Instance.isMainHeroFollowed()) { return; }
            if (Sys_Fight.Instance.IsFight()) {
                Sys_Hint.Instance.PushForbidOprationInFight();
                return;
            }

            PointerEventData ped = eventData as PointerEventData;
            // 点击界面上的坐标
            Vector2 localPoint;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(this.rawImage_BgMap.rectTransform, ped.position, UIManager.mUICamera, out localPoint);
            // 转换成地图坐标
            Vector2 screenPoint = UI_MapImage.ToVector2(this.LocalPointToScreenPoint(localPoint));
            // 寻路
            if (this.cSVMapInfoData.id == Sys_Map.Instance.CurMapId) {
                Sys_PathFind.Instance.PathFindId = EUIID.UI_PathFind;
                GameCenter.mPathFindControlSystem.FindMapPoint(this.cSVMapInfoData.id, screenPoint, (pos) => { });
            }
            else {
            }
        }
    }
}

