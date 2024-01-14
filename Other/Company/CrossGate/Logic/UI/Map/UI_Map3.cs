using System.Collections.Generic;
using Lib.Core;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Logic {
    public class UI_Map3 : UIComponent {
        #region 界面组件
        /// <summary> 地图界面 </summary>
        private Transform tr_MapView;
        /// <summary> 地图画像 </summary>
        private UI_MapImage ui_MapImage;
        #endregion

        #region 数据定义
        /// <summary> 地图类型 </summary>
        private EMapType eMapType { get; set; }
        /// <summary> 编号 </summary>
        private uint Id { get; set; }
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
        public override void OnDestroy() {
            this.ui_MapImage?.OnDestroy();
            base.OnDestroy();
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
            if (this.eMapType == EMapType.Map) {
                this.ui_MapImage?.Show();
            }
            this.Refresh();
        }
        public override void Hide() {
            this.ui_MapImage?.Hide();
            base.Hide();
            this.RecoveryAsset();
        }
        protected override void Update() {
            this.ui_MapImage?.ExecUpdate();
        }
        protected override void Refresh() {
            this.RefreshView();
        }
        protected override void ProcessEventsForEnable(bool toRegister) {
            this.OnProcessEventsForEnable(toRegister);
        }
        #endregion

        #region 初始化
        /// <summary>
        /// 检测组件
        /// </summary>
        private void OnParseComponent() {
            this.ui_MapImage = this.AddComponent<UI_MapImage>(this.transform.Find("Image_Map/View_Map01/Map01/View_Map"));
            this.tr_MapView = this.transform.Find("Image_Map/View_Map01/Map01/View_Map");
        }
        /// <summary>
        /// 注册回调事件
        /// </summary>
        private void OnProcessEventsForEnable(bool toRegister) {
            Sys_Npc.Instance.eventEmitter.Handle(Sys_Npc.EEvents.OnUpdateResPoint, this.OnUpdateResPoint, toRegister);
            Sys_Exploration.Instance.eventEmitter.Handle(Sys_Exploration.EEvents.ExplorationRewardNotice, this.OnUpdateResPoint, toRegister);
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
        }
        #endregion

        #region 界面显示
        /// <summary>
        /// 刷新界面
        /// </summary>
        public void RefreshView() {
            this.SetGameObjectState();
            this.SetInfoView();
            this.SetMapView();
        }
        /// <summary>
        /// 设置物件状态
        /// </summary>
        public void SetGameObjectState() {
            this.tr_MapView.gameObject.SetActive(this.eMapType == EMapType.Map);
        }
        /// <summary>
        /// 设置信息界面
        /// </summary>
        public void SetInfoView() {
            //设置其他地图信息
            this.uiMap.mapExplore.Show();
            this.uiMap.mapExplore.Refresh(this.eMapType, this.Id, this.uiMap, Sys_Map.Instance.LastExploreTab, this.ui_MapImage);
        }
        /// <summary>
        /// 设置地图界面
        /// </summary>
        public void SetMapView() {
            switch (this.eMapType) {
                case EMapType.Map: //地图
                    {
                        CSVMapInfo.Data cSVMapInfoData = CSVMapInfo.Instance.GetConfData(this.Id);
                        if (null == cSVMapInfoData) {
                            this.tr_MapView.GetComponent<RawImage>().texture = null;
                        }
                        else {
                            if (this.ui_MapImage != null) {
                                this.ui_MapImage.uiMap = uiMap;
                                this.ui_MapImage.SetData(this.Id, UI_MapImage.MapImageType.BigMap, this.mapParameter);
                                this.ui_MapImage.OnRefresh();
                            }
                        }
                    }
                    break;
            }
        }
        #endregion

        #region 响应事件
        /// <summary>
        /// 资源点更新事件
        /// </summary>
        public void OnUpdateResPoint() {
            // this.SetInfoView();
        }
        #endregion
    }
}

