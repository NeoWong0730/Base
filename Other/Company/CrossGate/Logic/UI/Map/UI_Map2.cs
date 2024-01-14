using System.Collections.Generic;
using Lib.Core;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Logic {
    public class UI_Map2 : UIComponent {
        #region 界面组件
        public UIScrollPage<UIPageDot> scrollPage = new UIScrollPage<UIPageDot>();
        
        /// <summary> 地图名 </summary>
        private Text text_Name;
        /// <summary> 翻页 </summary>
        private CP_PageDot cP_PageDot;
        /// <summary> 无限滚动 </summary>
        private InfinityGridLayoutGroup gridGroup;
        /// <summary> 居中脚本 </summary>
        private UICenterOnChild uiCenterOnChild;
        #endregion

        #region 数据定义
        /// <summary> 地图类型 </summary>
        private EMapType eMapType { get; set; }
        /// <summary> 编号 </summary>
        private uint Id { get; set; }
        /// <summary> 岛屿列表 </summary>
        public readonly List<uint> list_Island = new List<uint>();
        /// <summary> 岛屿下标 </summary>
        public int IslandIndex = 0;
        /// <summary> 地图参数 </summary>
        private Sys_Map.MapParameter mapParameter {
            get;
            set;
        }
        /// <summary> 加载资源请求 </summary>
        private readonly Dictionary<string, AsyncOperationHandle<GameObject>> mAssetRequest = new Dictionary<string, AsyncOperationHandle<GameObject>>();
        #endregion

        #region 系统函数
        protected override void Loaded() {
            this.OnParseComponent();
        }

        private UI_Map uiMap;
        public override void SetData(params object[] arg) {
            if (null == arg && arg.Length < 4)
                return;

            EMapType eMapType = (EMapType)arg[0];
            uint id = System.Convert.ToUInt32(arg[1]);
            Sys_Map.MapParameter parameter = (Sys_Map.MapParameter)arg[2];
            this.uiMap = arg[3] as UI_Map;
            this.SetMapData(eMapType, id, parameter);
        }
        public override void Show() {
            base.Show();
            this.Refresh();
        }
        public override void Hide() {
            base.Hide();
            this.RecoveryAsset();
        }

        protected override void Refresh() {
            this.RefreshView();
        }
        protected override void ProcessEventsForEnable(bool toRegister) {
            this.OnProcessEventsForEnable(toRegister);
        }
        #endregion

        #region 初始化
        private void OnParseComponent() {
            var t = this.transform.Find("View_Bottom/Scroll_Switch");
            scrollPage.Init(t);
            scrollPage.onPageSwicth = OnPageSwicth;
            scrollPage.onVdClicked = OnVdClicked;
            
            this.cP_PageDot = this.transform.Find("View_Bottom").GetComponent<CP_PageDot>();
            
            this.gridGroup = this.transform.Find("View_Map01/View_Island/Grid").gameObject.GetNeedComponent<InfinityGridLayoutGroup>();
            this.gridGroup.updateChildrenCallback = this.UpdateChildrenCallback;

            this.uiCenterOnChild = this.transform.Find("View_Map01/View_Island").gameObject.GetNeedComponent<UICenterOnChild>();
            this.uiCenterOnChild.onCenter = this.OnCenter;
        }
        /// <summary>
        /// 清理岛屿界面
        /// </summary>
        public void ClearIslandView(Transform tr) {
            if (tr) {
                for (int i = tr.childCount - 1; i >= 0; i--) {
                    var go = tr.GetChild(i).gameObject;
                    go.SetActive(false);
                }
            }
        }
        /// <summary>
        /// 注册回调事件
        /// </summary>
        private void OnProcessEventsForEnable(bool toRegister) {
            Sys_Npc.Instance.eventEmitter.Handle(Sys_Npc.EEvents.OnUpdateResPoint, this.OnUpdateResPoint, toRegister);
            Sys_Exploration.Instance.eventEmitter.Handle(Sys_Exploration.EEvents.ExplorationRewardNotice, this.OnUpdateResPoint, toRegister);
        }
        /// <summary>
        /// 加载资源
        /// </summary>
        /// <param name="path"></param>
        /// <param name="trans"></param>
        private void TryLoadAssetAsyn(string path, Transform trans) {
            AsyncOperationHandle<GameObject> gameObjectHandle;
            if (!this.mAssetRequest.TryGetValue(path, out gameObjectHandle)) {
                this.ClearIslandView(trans);
                AddressablesUtil.InstantiateAsync(ref gameObjectHandle, path, (AsyncOperationHandle<GameObject> handle) => {
                    GameObject uiGameObject = handle.Result;
                    this.SetIsland(uiGameObject.transform);
                }, true, trans);
                this.mAssetRequest[path] = gameObjectHandle;
            }
            else if (gameObjectHandle.IsValid() && gameObjectHandle.IsDone) {
                this.ClearIslandView(trans);
                this.SetIsland(gameObjectHandle.Result.transform);
            }
        }
        /// <summary>
        /// 释放资源
        /// </summary>
        private void RecoveryAsset() {
            var assetsTor = this.mAssetRequest.GetEnumerator();
            while (assetsTor.MoveNext()) {
                AsyncOperationHandle<GameObject> request = assetsTor.Current.Value;
                AddressablesUtil.Release(ref request, null);
            }
            this.mAssetRequest.Clear();
        }
        #endregion

        #region 数据处理
        /// <summary>
        /// 设置地图数据
        /// </summary>
        /// <param name="eMapType"></param>
        /// <param name="Id"></param>
        /// <param name="mapParameter"></param>
        public void SetMapData(EMapType eMapType, uint Id, Sys_Map.MapParameter mapParameter) {
            this.eMapType = eMapType;
            this.Id = Id;
            this.mapParameter = mapParameter;
            this.SetIslandsData();
        }
        /// <summary>
        /// 设置岛屿数据
        /// </summary>
        public void SetIslandsData()
        {
            this.IslandIndex = 0;
            this.list_Island.Clear();
            //Dictionary<uint, CSVIsland.Data> dict_IslandData = CSVIsland.Instance.GetDictData();
            //List<uint> list_IslandDataKeys = new List<uint>(dict_IslandData.Keys);

            var list_IslandDatas = CSVIsland.Instance.GetAll();
            for (int i = 0, len = list_IslandDatas.Count; i < len; i++)
            {
                //uint id = list_IslandDataKeys[i];
                //var item = dict_IslandData[id];
                var item = list_IslandDatas[i];
                if (item.island_type == 1 && item.islandid != null && item.islandid.Contains(this.Id))
                {
                    this.list_Island.AddRange(item.islandid);
                    break;
                }
            }

            this.cP_PageDot.SetMax(this.list_Island.Count);
            this.cP_PageDot.Build();
        }
        #endregion

        #region 界面显示
        /// <summary>
        /// 刷新界面
        /// </summary>
        public void RefreshView() {
            this.SetGameObjectState();
            
            this.SetMapView();
            this.SetPageDot();
            this.SetInfoView();
        }
        /// <summary>
        /// 设置物件状态
        /// </summary>
        public void SetGameObjectState() {
        }
        /// <summary>
        /// 设置翻页
        /// </summary>
        public void SetPageDot() {
            if (this.eMapType != EMapType.Island) {
                this.cP_PageDot.gameObject.SetActive(false);
                return;
            }

            this.cP_PageDot.gameObject.SetActive(true);
            int index = this.list_Island.IndexOf(this.Id);

            // 在uiCenterOnChild.InitPageArray之前先重新激活所有子节点，是因为如果先打开苏国的二级tab，此时centeron只有一个岛屿的childPos
            // 然后切换到大于一个岛屿的二级tab,此时就会因为某些直接点被disactive，不会被InitPageArray收纳进去，导致数组越界
            for (int i = 0, length = gridGroup.transform.childCount; i < length; ++i) {
                var ch = gridGroup.transform.GetChild(i);
                ch.gameObject.SetActive(true);
            }
            uiCenterOnChild.InitPageArray();
            
            scrollPage.Refresh(this.list_Island.Count, true, true);
            scrollPage.SetIndex(index);
            this.cP_PageDot.SetSelected(index);
        }
        
        private void OnPageSwicth(int pageIndex, int startIndex, int range) {
            // 移动scroll
            scrollPage.centerOn.SetCurrentPageIndex(pageIndex, true);
            // 模拟点击vd
            scrollPage.vds[pageIndex].SetSelected(true, true);
            
            // 刷新当前页面逻辑
            uiCenterOnChild.SetCurrentPageIndex(pageIndex, true);
        }

        private void _TmpHandle() {
            if (scrollPage.pageSwitcher.mode == CP_PageSwitcher.ETravelMode.NoCircle) {
                scrollPage.pageSwitcher.leftArrow.gameObject.SetActive(scrollPage.pageSwitcher.currentPageIndex != 0);
                scrollPage.pageSwitcher.rightArrow.gameObject.SetActive(scrollPage.pageSwitcher.currentPageIndex != scrollPage.pageSwitcher.PageCount - 1);
            }
            else if (scrollPage.pageSwitcher.mode == CP_PageSwitcher.ETravelMode.CircleRightOnly) {
                scrollPage.pageSwitcher.leftArrow.gameObject.SetActive(scrollPage.pageSwitcher.currentPageIndex != 0);
            }
            else if (scrollPage.pageSwitcher.mode == CP_PageSwitcher.ETravelMode.CircleLeftOnly) {
                scrollPage.pageSwitcher.rightArrow.gameObject.SetActive(scrollPage.pageSwitcher.currentPageIndex != scrollPage.pageSwitcher.PageCount - 1);
            }
        }
        
        private void OnVdClicked(int index) {
            // 移动scroll
            scrollPage.centerOn.SetCurrentPageIndex(index, true);
            
            scrollPage.pageSwitcher.SetCurrentIndex(index);
            _TmpHandle();
            
            // 刷新当前页面逻辑
            uiCenterOnChild.SetCurrentPageIndex(index, true);
        }

        /// <summary>
        /// 设置信息界面
        /// </summary>
        public void SetInfoView() {
            this.uiMap.mapExplore.Show();
            this.uiMap.mapExplore.Refresh(this.eMapType, this.Id, this.uiMap, (int)UI_MapExplore.ETabType.Explore);
        }
        /// <summary>
        /// 设置地图界面
        /// </summary>
        public void SetMapView() {
            this.gridGroup.SetAmount(this.list_Island.Count);
            
        }
        /// <summary>
        /// 设置岛屿
        /// </summary>
        /// <param name="tr"></param>
        public void SetIsland(Transform tr) {
            tr.gameObject.SetActive(true);
            
            Transform tr_Title = tr.Find("Area_Label");
            Transform tr_Button = tr.Find("Area_Image");

            for (int i = 0; i < tr_Button.childCount; i++) {
                this.SetMapBgItem(tr_Button.GetChild(i));
            }

            for (int i = 0; i < tr_Title.childCount; i++) {
                this.SetMapItem(tr_Title.GetChild(i));
            }
            for (int i = 0; i < tr_Button.childCount; i++) {
                Button button = tr_Button.GetChild(i).GetComponent<Button>();
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => { this.OnClick_SwitchMap(button.gameObject); });
            }
        }
        /// <summary>
        /// 设置背景图标
        /// </summary>
        /// <param name="tr"></param>
        public void SetMapBgItem(Transform tr) {
            uint mapId = 0;
            uint.TryParse(tr.name, out mapId);

            CSVMapInfo.Data cSVMapInfoData = CSVMapInfo.Instance.GetConfData(mapId);
            if (null == cSVMapInfoData) {
                tr.gameObject.SetActive(false);
                return;
            }
            tr.gameObject.SetActive(true);

            Image image_Bg = tr.GetComponent<Image>();
            //存在内存备份图片，导致内存多一份，影响效率问题
            image_Bg.alphaHitTestMinimumThreshold = 0.1f;
            if (cSVMapInfoData?.map_lv?.Count >= 2 && Sys_Role.Instance.Role.Level >= cSVMapInfoData?.map_lv[0]) //已达到等级
            {
                ImageHelper.SetImageGray(image_Bg, false, false);
            }
            else {
                ImageHelper.SetImageGray(image_Bg, true, false);
            }
        }
        /// <summary>
        /// 设置地图模版
        /// </summary>
        /// <param name="tr"></param>
        public void SetMapItem(Transform tr) {
            uint mapId = 0;
            uint.TryParse(tr.name, out mapId);

            CSVMapInfo.Data cSVMapInfoData = CSVMapInfo.Instance.GetConfData(mapId);
            if (null == cSVMapInfoData) {
                tr.gameObject.SetActive(false);
                return;
            }
            tr.gameObject.SetActive(true);
            //地图名
            Text text_Name = tr.Find("Text_Name").GetComponent<Text>();
            string Name = LanguageHelper.GetTextContent(cSVMapInfoData.name);
            //推荐等级
            Text text_Limit1 = tr.Find("Text_Limit").GetComponent<Text>();
            Text text_Limit2 = tr.Find("Text_Limit02").GetComponent<Text>();
            string Limit = cSVMapInfoData.map_lv?.Count >= 2 ? LanguageHelper.GetTextContent(4501, cSVMapInfoData.map_lv[0].ToString(), cSVMapInfoData.map_lv[1].ToString()) : string.Empty;
            text_Limit1.gameObject.SetActive(true);
            text_Limit2.gameObject.SetActive(false);
            //标记
            GameObject go_Mark = tr.Find("Image_Self").gameObject;
            CSVMapInfo.Data curCSVMapInfoData = CSVMapInfo.Instance.GetConfData(Sys_Map.Instance.CurMapId);
            go_Mark.SetActive(curCSVMapInfoData?.map_node?[2] == mapId);
            //标题背景
            Image image_TitleBg = tr.Find("Image_BG01").GetComponent<Image>();
            //进度条
            Slider slider_Progress = tr.Find("Image_ProcessBG").GetComponent<Slider>();
            Text text_Progress = tr.Find("Image_ProcessBG/Text_Num").GetComponent<Text>();
            uint CurNum = 0, MaxNum = 0;
            Sys_Exploration.ExplorationData explorationData = Sys_Exploration.Instance.GetExplorationData(mapId);
            if (null != explorationData) {
                CurNum = explorationData.totalProcess.CurNum;
                MaxNum = explorationData.totalProcess.TargetNum;
            }
            slider_Progress.value = MaxNum == 0 ? 0 : CurNum / (float)MaxNum;
            text_Progress.text = string.Format("{0}%", MaxNum == 0 ? 0 : System.Math.Round(CurNum / (float)MaxNum * 100, 1));

            if (cSVMapInfoData?.map_lv?.Count >= 2 && Sys_Role.Instance.Role.Level >= cSVMapInfoData?.map_lv[0]) //已达到等级
            {
                TextHelper.SetText(text_Name, 4523, Name);
                TextHelper.SetText(text_Limit1, 4525, Limit);
                slider_Progress.gameObject.SetActive(true);
                ImageHelper.SetImageGray(image_TitleBg, false, false);
            }
            else {
                TextHelper.SetText(text_Name, 4524, Name);
                TextHelper.SetText(text_Limit1, 4526, Limit);
                slider_Progress.gameObject.SetActive(false);
                ImageHelper.SetImageGray(image_TitleBg, true, false);
            }

            var markNode = tr.Find("Grid_Mark");
            TaskHelper.IsNpcInThisMap(uiMap.npcs, EMapType.Map, mapId, ref taskCatagaries);
            FrameworkTool.CreateChild(in markNode, in uiMap.tags, in taskCatagaries);
        }
        private List<int> taskCatagaries = new List<int>();
        
        /// <summary>
        /// 设置标记说明
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="id"></param>
        private void SetExploreTipsView(Transform tr, uint id) {
            CSVMapExplorationMark.Data cSVMapExplorationMarkData = CSVMapExplorationMark.Instance.GetConfData(id);
            if (null == cSVMapExplorationMarkData) {
                tr.gameObject.SetActive(false);
                return;
            }
            tr.gameObject.SetActive(true);

            /// <summary> 标题 </summary>
            Text text_Title = tr.Find("Image_Title/Text").GetComponent<Text>();
            text_Title.text = LanguageHelper.GetTextContent(cSVMapExplorationMarkData.List_lan);
            /// <summary> 图标 </summary>
            Image image_Icon = tr.Find("Image_Title/Image_Mark").GetComponent<Image>();
            ImageHelper.SetIcon(image_Icon, cSVMapExplorationMarkData.List_Icon);
            /// <summary> 说明 </summary>
            Text text_Explain = tr.Find("Image_BG/Text").GetComponent<Text>();
            text_Explain.text = LanguageHelper.GetTextContent(cSVMapExplorationMarkData.Des_lan);
            /// <summary> 背景 </summary>
            RectTransform rectTransform = tr.Find("Image_BG").GetComponent<RectTransform>();
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }
        #endregion

        #region 响应事件
        /// <summary>
        /// 切换地图
        /// </summary>
        /// <param name="go"></param>
        public void OnClick_SwitchMap(GameObject go) {
            int id = 0;
            int.TryParse(go.name, out id);
            if (id <= 0) return;

            CSVMapInfo.Data cSVMapInfoData = CSVMapInfo.Instance.GetConfData((uint)id);

            if (cSVMapInfoData?.map_lv?.Count >= 2 && Sys_Role.Instance.Role.Level >= cSVMapInfoData?.map_lv[0]) //已达到等级
            {
                UI_Map.onSwitchMap(EMapType.Map, (uint)id);
            }
            else {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4515));
            }
        }
        /// <summary>
        /// 更新滚动子节点
        /// </summary>
        /// <param name="index"></param>
        /// <param name="trans"></param>
        private void UpdateChildrenCallback(int index, Transform trans) {
            if (this.eMapType != EMapType.Island) return;

            if (index < 0 || index >= this.list_Island.Count) return;

            trans.name = index.ToString();

            uint id = this.list_Island[index];

            CSVIsland.Data cSVIslandData = CSVIsland.Instance.GetConfData(id);
            if (null == cSVIslandData) {
                this.ClearIslandView(trans);
            }
            else {
                this.TryLoadAssetAsyn(cSVIslandData.map_ui, trans);

                if (this.Id == id) {
                    this.uiCenterOnChild.SetCurrentPageIndex(index, false);
                }
            }
        }
        /// <summary>
        /// 居中事件
        /// </summary>
        /// <param name="go"></param>
        public void OnCenter(GameObject go) {
            int index = 0;
            if (!int.TryParse(go.name, out index))
                return;
            this.cP_PageDot.SetSelected(index);
            this.IslandIndex = index;
            this.SetInfoView();
        }
        /// <summary>
        /// 资源点更新事件
        /// </summary>
        public void OnUpdateResPoint() {
            // this.SetInfoView();
        }
        #endregion

        #region 提供功能
        #endregion
    }
}

