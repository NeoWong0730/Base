using System.Collections.Generic;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    /// <summary> 迷宫地图 </summary>
    public class UI_Map4 : UIComponent {
        #region 界面组件
        /// <summary> 地图名 </summary>
        private Text text_Name;
        /// <summary> 菜单模版 </summary>
        private Toggle toggle_MenuItem;
        /// <summary> 菜单模版列表 </summary>
        private readonly List<Toggle> list_MenuItem = new List<Toggle>();
        /// <summary> 地图画像 </summary>
        private UI_MapImage ui_MapImage;

        private UI_Map3 map3;
        private GameObject map4;
        #endregion

        #region 数据定义
        private CSVMapInfo.Data cSVMapInfoData {
            get;
            set;
        }
        #endregion

        #region 系统函数
        protected override void Loaded() {
            base.Loaded();
            this.OnParseComponent();
        }
        public override void OnDestroy() {
            this.ui_MapImage?.OnDestroy();
            this.map3?.OnDestroy();
            base.OnDestroy();
        }

        private Sys_Map.MapParameter mapParameter {
            get;
            set;
        }
        private EMapType mapType;
        private UI_Map uiMap;
        public override void SetData(params object[] arg) {
            this.uiMap = arg[3] as UI_Map;
            this.mapType = (EMapType)arg[0];

            if (this.mapType == EMapType.Maze) {
                if (arg.Length < 2) return;
                uint id = System.Convert.ToUInt32(arg[1]);
                mapParameter = arg.Length >= 3 ? (Sys_Map.MapParameter)arg[2] : null;
                this.SetMapData(id);
                this.TryCreateItemList();
            }
            else {
                this.map3.SetData(arg);
            }
        }
        public override void Show() {
            base.Show();

            this.uiMap.mapExplore.Hide();
            if (this.mapType == EMapType.Maze) {
                this.map3.Hide();
                this.map4.gameObject.SetActive(true);

                this.SetMenuList();
                this.ui_MapImage?.Show();
            }
            else {
                this.map3.Show();
                this.map4.gameObject.SetActive(false);
            }
        }
        public override void Hide() {
            if (this.mapType == EMapType.Maze) {
                this.ui_MapImage?.Hide();
            }
            else {
                this.map3.Hide();
            }
            base.Hide();
        }
        protected override void Update() {
            if (this.mapType == EMapType.Maze) {
                this.ui_MapImage?.ExecUpdate();
            }
            else {
                this.map3.ExecUpdate();
            }
            base.Update();
        }
        protected override void Refresh() {
            if (this.mapType == EMapType.Maze) {
                base.Refresh();
                this.RefreshView();
            }
            else {
                this.map3.RefreshView();
            }
        }
        #endregion

        #region 初始化
        /// <summary>
        /// 检测组件
        /// </summary>
        private void OnParseComponent() {
            this.map4 = this.transform.Find("4").gameObject;
            this.ui_MapImage = this.AddComponent<UI_MapImage>(this.transform.Find("4/Image_Map/View_Map01/View_Map"));
            this.text_Name = this.transform.Find("4/View_Left/View_Title/Text_Name").GetComponent<Text>();
            this.toggle_MenuItem = this.transform.Find("4/View_Left/Scroll_View_Btn/TabList/BtnItem").GetComponent<Toggle>();

            this.map3 = this.AddComponent<UI_Map3>(this.transform.Find("3"));
        }
        /// <summary>
        /// 创建模版列表
        /// </summary>
        private bool created = false;
        private void TryCreateItemList() {
            if (!this.created) {
                for (int i = 0; i < 14; i++) {
                    Toggle toggle = i == 0 ? this.toggle_MenuItem : GameObject.Instantiate(this.toggle_MenuItem.transform, this.toggle_MenuItem.transform.parent).GetComponent<Toggle>();
                    this.list_MenuItem.Add(toggle);
                    toggle.onValueChanged.AddListener((bool value) => this.OnClick_Menu(toggle, value));
                }

                this.created = true;
            }
        }
        #endregion

        #region 数据处理
        /// <summary>
        /// 设置地图数据
        /// </summary>
        /// <param name="id"></param>
        public void SetMapData(uint Id) {
            this.cSVMapInfoData = CSVMapInfo.Instance.GetConfData(Id);
        }
        #endregion

        #region 界面显示
        /// <summary>
        /// 刷新界面
        /// </summary>
        public void RefreshView() {
            if (null == this.cSVMapInfoData) return;
            this.text_Name.text = LanguageHelper.GetTextContent(this.cSVMapInfoData.name);
            if (this.ui_MapImage != null) {
                this.ui_MapImage.uiMap = uiMap;
                this.ui_MapImage.SetData(this.cSVMapInfoData.id, UI_MapImage.MapImageType.BigMap, mapParameter);
                this.ui_MapImage.OnRefresh();
            }
        }
        /// <summary>
        /// 设置菜单列表
        /// </summary>
        public void SetMenuList() {
            if (null == this.cSVMapInfoData) return;
            var list = this.cSVMapInfoData.map_node;
            uint id = 0;
            var targetParams = mapParameter as Sys_Map.TargetClassicBossParameter;
            if (targetParams == null) {
                for (int i = 0; i < this.list_MenuItem.Count; i++) {
                    if (i + 3 < list?.Count) {
                        id = (uint)list[i + 3];
                        this.SetMenuItem(this.list_MenuItem[i].transform, id);
                        if (id == Sys_Map.Instance.CurMapId)
                            this.SetToggle(this.list_MenuItem[i]);
                    }
                    else {
                        this.SetMenuItem(this.list_MenuItem[i].transform, 0);
                    }
                }
            }
            else {
                for (int i = 0; i < this.list_MenuItem.Count; i++) {
                    if (i + 3 < list?.Count) {
                        id = (uint)list[i + 3];
                        this.SetMenuItem(this.list_MenuItem[i].transform, id);
                        if (id == targetParams.targetMapId)
                            this.SetToggle(this.list_MenuItem[i]);
                    }
                    else {
                        this.SetMenuItem(this.list_MenuItem[i].transform, 0);
                    }
                }
            }
        }
        /// <summary>
        /// 设置菜单
        /// </summary>
        /// <param name="toggle"></param>
        public void SetToggle(Toggle toggle) {
            //开关开启
            if (toggle.isOn) {
                this.OnClick_Menu(toggle, true);
            }
            else {
                toggle.isOn = true;
            }
        }
        /// <summary>
        /// 设置菜单模版
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="id"></param>
        public void SetMenuItem(Transform tr, uint id) {
            CSVMapInfo.Data cSVMapInfoData = CSVMapInfo.Instance.GetConfData(id);
            if (null == cSVMapInfoData) {
                tr.gameObject.SetActive(false);
                return;
            }
            tr.gameObject.SetActive(true);
            tr.name = id.ToString();
            /// <summary> 地图名 </summary>
            Text text_Name1 = tr.Find("Text").GetComponent<Text>();
            text_Name1.text = LanguageHelper.GetTextContent(cSVMapInfoData.name);
            Text text_Name2 = tr.Find("Bright/Text").GetComponent<Text>();
            text_Name2.text = LanguageHelper.GetTextContent(cSVMapInfoData.name);
            /// <summary> 标记 </summary>
            GameObject go_Name = tr.Find("Image_Explore").gameObject;
            go_Name.SetActive(id == Sys_Map.Instance.CurMapId);
        }
        #endregion

        #region 响应事件
        /// <summary>
        /// 点击菜单(界面显示)
        /// </summary>
        /// <param name="toggle"></param>
        /// <param name="value"></param>
        public void OnClick_Menu(Toggle toggle, bool value) {
            int id = 0;
            int.TryParse(toggle.name, out id);
            if (id <= 0) return;

            if (value) {
                this.SetMapData((uint)id);
                this.Refresh();
            }
        }
        #endregion

        #region 提供功能
        #endregion
    }
}
