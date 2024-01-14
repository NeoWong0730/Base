using System;
using System.Collections.Generic;
using Lib.Core;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;
using static Logic.TaskHelper;

namespace Logic {
    public class UIPageDot : UISelectableElement {
        public Text number;
        public Text numberSelect;
        public CP_Toggle toggle;
        
        protected override void Loaded() {
            number = transform.Find("Text").GetComponent<Text>();
            numberSelect = transform.Find("Select/Text_Select").GetComponent<Text>();
            
            this.toggle = this.gameObject.GetComponent<CP_Toggle>();
            this.toggle.onValueChanged.AddListener(this.Switch);
        }
        
        public void Switch(bool arg) {
            if (arg) {
                this.onSelected?.Invoke(index, true);
            }
        }
        public override void SetSelected(bool toSelected, bool force) {
            this.toggle.SetSelected(toSelected, true);
        }

        public int index;
        public override void SetData(params object[] arg)  {
            this.index = Convert.ToInt32(arg[0]);
            
            number.text = numberSelect.text = (index + 1).ToString();
        }
    }

    public class UIScrollPage<T> : UIComponent where T : UISelectableElement, new() {
        public CP_PageSwitcher pageSwitcher;
        public Action<int, int, int> onPageSwicth;

        public UICenterOnChild centerOn;

        public Action<int> onVdClicked;

        public GameObject proto;
        public Transform protoParent;
        public UIElementCollector<T> vds = new UIElementCollector<T>();
        
        protected override void Loaded() {
            pageSwitcher = transform.Find("Btn").GetComponent<CP_PageSwitcher>();
            pageSwitcher.mode = CP_PageSwitcher.ETravelMode.NoCircleKeepButton;
            pageSwitcher.onExec += _OnPageSwicth;
            
            centerOn = gameObject.GetNeedComponent<UICenterOnChild>();
            
            proto = transform.Find("Viewport/Content/Toggle").gameObject;
            protoParent = proto.transform.parent;
        }

        private void _OnPageSwicth(int pageIndex, int startIndex, int range) {
            onPageSwicth?.Invoke(pageIndex, startIndex, range);
        }

        public void Refresh(int count, bool useBtns = true, bool useCenterOn = true) {
            pageSwitcher.gameObject.SetActive(useBtns);
            if (useBtns) {
                pageSwitcher.SetCount(count);
            }
            
            vds.TryBuildOrRefresh(proto, protoParent, count, _OnRefresh);
            
            centerOn.enabled = useCenterOn;
            centerOn.InitPageArray();
        }

        private void _OnRefresh(T t, int index) {
            t.SetUniqueId(index + 1);
            t.SetSelectedAction(_OnSelect);

            t.SetData(index);
        }

        // 点击vd的事件
        private void _OnSelect(int id, bool force) {
            onVdClicked?.Invoke(id);
        }

        public int curIndex = 0;
        public void SetIndex(int index) {
            this.curIndex = index;

            pageSwitcher.SetCurrentIndex(curIndex);
            pageSwitcher.Exec();
        }
    }

    /// <summary> 地图界面 </summary>
    public class UI_Map : UIBase {
        #region 委托事件
        /// <summary> 切换地图 </summary>
        public delegate void OnSwitchMap(EMapType eMapType, uint id);
        public static OnSwitchMap onSwitchMap;
        #endregion

        #region 界面组件
        public MapSlider slider;

        public Text name1, name1_1;
        public Text name2, name2_1;
        public Text name3, name3_1;
        public Text name4, name4_1;
        public Text tabName4, tabName4_1;

        /// <summary> 子界面 </summary>
        public readonly List<UIComponent> list_ChildView = new List<UIComponent>();
        /// <summary> 菜单列表 </summary>
        private readonly List<Toggle> list_Menu = new List<Toggle>();
        private readonly List<GameObject> list_MenuSelecte = new List<GameObject>();

        public Dictionary<int, Transform> tags = new Dictionary<int, Transform>();

        public UI_MapExplore mapExplore;
        #endregion

        #region 数据定义
        /// <summary> 当前所在地图信息 </summary>
        private CSVMapInfo.Data defaultCSVMap {
            get;
            set;
        }
        public List<int> curMap = new List<int>(3);

        /// <summary> 地图参数 </summary>
        private Sys_Map.MapParameter mapParameter {
            get;
            set;
        }
        public bool showCurrent = true;
        private bool InDoor {
            get {
                int count = this.defaultCSVMap?.map_node?.Count ?? 0;
                return count >= 4;
            }
        }
        #endregion

        #region 系统函数
        protected override void OnLoaded() {
            this.OnParseComponent();
        }
        protected override void OnUpdate() {
            for (int i = 0, count = this.list_ChildView.Count; i < count; i++) {
                var x = this.list_ChildView[i];
                x.ExecUpdate();
            }
        }
        protected override void OnOpen(object arg) {
            var argType = arg == null ? null : arg.GetType();

            if (argType == typeof(Sys_Map.TargetMapParameter)) {
                Sys_Map.TargetMapParameter parameter = arg as Sys_Map.TargetMapParameter;
                this.mapParameter = parameter;
                this.defaultCSVMap = CSVMapInfo.Instance.GetConfData(parameter.targetMapId);
            }
            else if (argType == typeof(Sys_Map.ResMarkParameter)) {
                Sys_Map.ResMarkParameter parameter = arg as Sys_Map.ResMarkParameter;
                this.mapParameter = parameter;
                this.defaultCSVMap = CSVMapInfo.Instance.GetConfData(Sys_Map.Instance.CurMapId);
            }
            else if (argType == typeof(Sys_Map.CatchPetParameter)) {
                Sys_Map.CatchPetParameter parameter = arg as Sys_Map.CatchPetParameter;
                this.mapParameter = parameter;
                this.defaultCSVMap = CSVMapInfo.Instance.GetConfData(parameter.catchPetMapId);
            }
            else if (argType == typeof(Sys_Map.TargetNpcParameter)) {
                Sys_Map.TargetNpcParameter parameter = arg as Sys_Map.TargetNpcParameter;
                this.mapParameter = parameter;
                this.defaultCSVMap = CSVMapInfo.Instance.GetConfData(parameter.targetMapId);
            }
            else if (argType == typeof(Sys_Map.TargetClassicBossParameter)) {
                Sys_Map.TargetClassicBossParameter parameter = arg as Sys_Map.TargetClassicBossParameter;
                this.mapParameter = parameter;
                this.defaultCSVMap = CSVMapInfo.Instance.GetConfData(parameter.targetMapId);

                this.showCurrent = false;
                this.forceIndex = 3;
            }
            else {
                this.mapParameter = null;
                this.defaultCSVMap = CSVMapInfo.Instance.GetConfData(Sys_Map.Instance.CurMapId);
            }

            this.npcs = TaskHelper.GetNpcsByTrackedTask();
            this.mapNpcs = TaskHelper.GetTaskMapNpcs(this.npcs);
            this.curMap.Clear();
            if (this.defaultCSVMap != null) {
                this.curMap.Add(this.defaultCSVMap.map_node[0]);
                this.curMap.Add(this.defaultCSVMap.map_node[1]);
                this.curMap.Add(this.defaultCSVMap.map_node[2]);
            }
        }
        protected override void OnShow() {
            this.RefreshView(this.forceIndex);

            if (this.needRefresh) {
                this.OnIndexed(this.slider.curIndex, this.slider.curIndex);
                this.needRefresh = false;
            }
        }
        protected override void OnHideStart() {
            //for (int i = 0, count = this.list_ChildView.Count; i < count; i++) {
            //    var x = this.list_ChildView[i];
            //    x.Hide();
            //}
        }
        protected override void OnDestroy() {
            for (int i = 0, count = this.list_ChildView.Count; i < count; i++) {
                var x = this.list_ChildView[i];
                x.OnDestroy();
            }
            this.list_ChildView.Clear();
            this.mapExplore.OnDestroy();
            onSwitchMap = null;
        }
        protected override void ProcessEvents(bool toRegister) {
            Sys_Map.Instance.eventEmitter.Handle(Sys_Map.EEvents.OnHeroTel, this.OnHeroTel, toRegister);
        }

        protected override void ProcessEventsForEnable(bool toRegister) {
            Sys_Map.Instance.eventEmitter.Handle(Sys_Map.EEvents.OnCloseMapView, this.OnClick_Close, toRegister);
            Sys_Task.Instance.eventEmitter.Handle<int, uint, TaskEntry>(Sys_Task.EEvents.OnTraced, this.OnTraced, toRegister);
        }
        #endregion

        #region 初始化
        /// <summary>
        /// 检测组件 
        /// </summary>
        private void OnParseComponent() {
            this.transform.Find("Animator/View_Title01_Map/Btn_Close").GetComponent<Button>().onClick.AddListener(this.OnClick_Close);

            var map1 = this.AddComponent<UI_Map1>(this.transform.Find("Animator/Pages/1"));
            this.list_ChildView.Add(map1);
            var map2 = this.AddComponent<UI_Map2>(this.transform.Find("Animator/Pages/2"));
            this.list_ChildView.Add(map2);
            var map3 = this.AddComponent<UI_Map3>(this.transform.Find("Animator/Pages/3"));
            this.list_ChildView.Add(map3);
            var map4 = this.AddComponent<UI_Map4>(this.transform.Find("Animator/Pages/4"));
            this.list_ChildView.Add(map4);
            
            this.mapExplore = new UI_MapExplore();
            this.mapExplore.Init(this.transform.Find("Animator/PageLeft"));

            for (int i = 0, count = this.list_ChildView.Count; i < count; i++) {
                var item = this.list_ChildView[i];
                item.gameObject.SetActive(false);
            }

            Transform tagGo = this.transform.Find("Animator/TaskCatagaryMarks");
            for (int i = 0, length = tagGo.childCount; i < length; i++) {
                var tr = tagGo.GetChild(i);
                if (int.TryParse(tr.name, out int taskCatagary)) {
                    this.tags[taskCatagary] = tr;
                }
            }

            for (int i = 0; i < 4; i++) {
                Toggle toggle = this.transform.Find(string.Format("Animator/Button_Switch/SwitchGroup/Button_Switch_{0}", i + 1)).GetComponent<Toggle>();
                this.list_Menu.Add(toggle);
                this.list_MenuSelecte.Add(toggle.transform.Find("Image_Select").gameObject);
            }

            this.name1 = this.transform.Find("Animator/Button_Switch/SwitchGroup/Button_Switch_1/Text (1)").GetComponent<Text>();
            this.name1_1 = this.transform.Find("Animator/Button_Switch/SwitchGroup/Button_Switch_1/Image_Select/Text (1)").GetComponent<Text>();
            
            this.name2 = this.transform.Find("Animator/Button_Switch/SwitchGroup/Button_Switch_2/Text (1)").GetComponent<Text>();
            this.name2_1 = this.transform.Find("Animator/Button_Switch/SwitchGroup/Button_Switch_2/Image_Select/Text (1)").GetComponent<Text>();

            this.name3 = this.transform.Find("Animator/Button_Switch/SwitchGroup/Button_Switch_3/Text (1)").GetComponent<Text>();
            this.name3_1 = this.transform.Find("Animator/Button_Switch/SwitchGroup/Button_Switch_3/Image_Select/Text (1)").GetComponent<Text>();

            this.name4 = this.transform.Find("Animator/Button_Switch/SwitchGroup/Button_Switch_4/Text (1)").GetComponent<Text>();
            this.name4_1 = this.transform.Find("Animator/Button_Switch/SwitchGroup/Button_Switch_4/Image_Select/Text (1)").GetComponent<Text>();

            this.tabName4 = this.transform.Find("Animator/Button_Switch/SwitchGroup/Button_Switch_4/Text").GetComponent<Text>();
            this.tabName4_1 = this.transform.Find("Animator/Button_Switch/SwitchGroup/Button_Switch_4/Image_Select/Text").GetComponent<Text>();

            this.slider = this.transform.Find("Animator/Button_Switch/Slider").GetComponent<MapSlider>();
            this.slider.onIndexed += this.OnIndexed;

            for (int i = 0, count = this.list_Menu.Count; i < count; i++) {
                var item = this.list_Menu[i];
                int toggleIndex = i;
                item.onValueChanged.AddListener((flag) => {
                    if (flag) {
                        this.slider.OnTabClicked(toggleIndex, false);
                    }
                });
                item.onValueChanged.Invoke(false);
            }

            onSwitchMap = this.ReadySwitchMap;
        }

        private void OnIndexed(int preIndex, int curIndex) {
            if (curIndex >= this.list_Menu.Count - 1) {
                // 点击当前进行重置
                this.curMap.Clear();
                this.curMap.Add(this.defaultCSVMap.map_node[0]);
                this.curMap.Add(this.defaultCSVMap.map_node[1]);
                this.curMap.Add(this.defaultCSVMap.map_node[2]);
            }

            bool isFinalIndex = curIndex == this.slider.to;

            this.RefreshTabNames((uint)this.curMap[0]);

            if (0 <= preIndex && preIndex < this.slider.ctrls.Length && preIndex != curIndex) {
                this.list_MenuSelecte[preIndex].SetActive(false);
            }

            this.list_Menu[curIndex].SetIsOnWithoutNotify(true);
            this.OnClick_ResetMenu(curIndex);
            this.OnClick_Menu(this.list_Menu[curIndex], true);
        }
        #endregion

        #region 界面显示
        public Dictionary<int, List<uint>> npcs = new Dictionary<int, List<uint>>();
        public Dictionary<uint, MapNpc> mapNpcs = new Dictionary<uint, MapNpc>();
        /// <summary>
        /// 刷新界面
        /// </summary>
        private void RefreshView(int forceIndex = -1) {
            this.SetMenuList(forceIndex);
        }
        public void RefreshTabNames(uint countryId) {
            var csvIsland = CSVIsland.Instance.GetConfData(countryId);
            this.name1.text = this.name1_1.text = LanguageHelper.GetTextContent(csvIsland.name);
            csvIsland = CSVIsland.Instance.GetConfData((uint)this.curMap[1]);
            this.name2.text = this.name2_1.text = LanguageHelper.GetTextContent(csvIsland.name);
            var csvMap = CSVMapInfo.Instance.GetConfData((uint)this.curMap[2]);
            this.name3.text = this.name3_1.text = LanguageHelper.GetTextContent(csvMap.name);
            this.name4.text = this.name4_1.text = LanguageHelper.GetTextContent(this.defaultCSVMap.name);
            this.tabName4.text = this.tabName4_1.text = this.showCurrent ? LanguageHelper.GetTextContent(4564) : LanguageHelper.GetTextContent(4565);
        }
        /// <summary>
        /// 设置菜单列表
        /// </summary>
        private void SetMenuList(int forceIndex = -1) {
            if (null == this.defaultCSVMap) return;
            //当前地图类型
            EMapType eMapType = this.GetMapType(this.defaultCSVMap.id);
            if (forceIndex != -1) {
                eMapType = (EMapType)(forceIndex + 1);
            }

            //切换地图
            this.SwitchMap(eMapType, this.defaultCSVMap.id);
        }

        //public void Refresh
        #endregion

        #region 响应事件
        /// <summary>
        /// 关闭按钮
        /// </summary>
        public void OnClick_Close() {
            this.CloseSelf();
        }
        /// <summary>
        /// 点击菜单(重置数据)
        /// </summary>
        /// <param name="go"></param>
        private void OnClick_ResetMenu(int index) {
            if (index < 0) return;

            switch (index) {
                case 0: {
                        this.list_ChildView[index].SetData(EMapType.Country, this.curMap[0], this.mapParameter, this);
                    }
                    break;
                case 1: {
                        int id = 0;
                        if (null != this.curMap && this.curMap.Count >= 2) {
                            id = this.curMap[1];
                        }
                        this.list_ChildView[index].SetData(EMapType.Island, id, this.mapParameter, this);
                    }
                    break;
                case 2: {
                        int id = 0;
                        if (null != this.curMap && this.curMap.Count >= 3) {
                            id = this.curMap[2];
                        }
                        this.list_ChildView[index].SetData(EMapType.Map, id, this.mapParameter, this);
                    }
                    break;
                case 3: {
                        if (this.InDoor) {
                            int id = 0;
                            if (null != this.defaultCSVMap) {
                                id = (int)this.defaultCSVMap.id;
                            }
                            this.list_ChildView[index].SetData(EMapType.Maze, id, this.mapParameter, this);
                        }
                        else {
                            int id = 0;
                            if (null != this.curMap && this.curMap.Count >= 3) {
                                id = this.curMap[2];
                            }
                            this.list_ChildView[index].SetData(EMapType.Map, id, this.mapParameter, this);
                        }
                    }
                    break;
            }
        }
        /// <summary>
        /// 点击菜单(界面显示)
        /// </summary>
        /// <param name="toggle"></param>
        /// <param name="value"></param>
        public void OnClick_Menu(Toggle toggle, bool value) {
            int index = this.list_Menu.IndexOf(toggle);
            if (index < 0) return;

            if (value) {
                this.list_ChildView[index].Show();
            }
            else {
                this.list_ChildView[index].Hide();
            }
            this.list_MenuSelecte[index].SetActive(value);
        }
        private void ReadySwitchMap(EMapType eMapType, uint id) {
            switch (eMapType) {
                case EMapType.Island: {
                        this.curMap[1] = (int)id;
                    }
                    break;
                case EMapType.Map: {
                        this.curMap[1] = CSVMapInfo.Instance.GetConfData(id).map_node[1];
                        this.curMap[2] = (int)id;
                    }
                    break;
            }

            this.slider.OnTabClicked((int)eMapType - 1, false);
        }
        /// <summary>
        /// 切换地图
        /// </summary>
        /// <param name="eMapType"></param>
        /// <param name="id"></param>
        public void SwitchMap(EMapType eMapType, uint id) {
            int index = this.GetMenuIndex(eMapType);
            //设置数据
            this.list_ChildView[index].SetData(eMapType, id, this.mapParameter, this);
            this.SetToggle(this.list_Menu[index]);
        }
        /// <summary>
        /// 设置菜单
        /// </summary>
        /// <param name="toggle"></param>
        public void SetToggle(Toggle toggle) {
            //开关开启
            if (toggle.isOn) {
                toggle.onValueChanged.Invoke(true);
            }
            else {
                toggle.isOn = true;
            }
        }
        private bool needRefresh = false;
        private int forceIndex = -1;
        /// <summary>
        /// 英雄传送
        /// </summary>
        public void OnHeroTel() {
            if (this.mapParameter != null) {
                return;
            }

            if (this.defaultCSVMap.id != Sys_Map.Instance.CurMapId) {
                this.defaultCSVMap = CSVMapInfo.Instance.GetConfData(Sys_Map.Instance.CurMapId);

                this.curMap.Clear();
                this.curMap.Add(this.defaultCSVMap.map_node[0]);
                this.curMap.Add(this.defaultCSVMap.map_node[1]);
                this.curMap.Add(this.defaultCSVMap.map_node[2]);

                this.RefreshView();

                EMapType eMapType = this.GetMapType(this.defaultCSVMap.id);
                int index = this.GetMenuIndex(eMapType);
                if (this.slider.curIndex == index) {
                    this.needRefresh = true;
                }
            }
        }
        private void OnTraced(int menuId, uint id, TaskEntry taskEntry) {
            this.npcs = TaskHelper.GetNpcsByTrackedTask();
            this.mapNpcs = TaskHelper.GetTaskMapNpcs(this.npcs);
            this.RefreshView();
        }
        #endregion

        #region 提供功能
        /// <summary>
        /// 获取地图类型
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public EMapType GetMapType(uint id) {
            CSVMapInfo.Data cSVMapInfoData = CSVMapInfo.Instance.GetConfData(id);

            int count = cSVMapInfoData?.map_node?.Count ?? 0;
            if (count == 0) return EMapType.None;

            switch (count) {
                case 1: return EMapType.Country;
                case 2: return EMapType.Island;
                case 3: return EMapType.Map;
                default: return EMapType.Maze;
            }
        }
        /// <summary>
        /// 得到对应菜单下标
        /// </summary>
        /// <param name="eMapType"></param>
        /// <returns></returns>
        public int GetMenuIndex(EMapType eMapType) {
            int index = 0;
            switch (eMapType) {
                case EMapType.None:
                case EMapType.Country: {
                        index = 0;
                    }
                    break;
                case EMapType.Island: {
                        index = 1;
                    }
                    break;
                case EMapType.Map: {
                        index = 2;
                    }
                    break;
                case EMapType.Maze: {
                        index = 3;
                    }
                    break;
            }
            return index;
        }
        #endregion
    }
}


