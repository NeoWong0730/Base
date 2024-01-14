using Logic.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    /// <summary> 地图区域详情 </summary>
    public class UI_MapAreaDetail : UIBase {
        #region 界面组件

        /// <summary> 名称 </summary>
        private Text text_Name;

        /// <summary> 地图 </summary>
        private RawImage image_Map;

        /// <summary> 详情 </summary>
        private Text text_Detail;

        #endregion

        #region 数据定义

        private EMapType mapType;
        private uint mapId;

        #endregion

        #region 系统函数

        protected override void OnLoaded() {
            OnParseComponent();
        }

        protected override void OnOpen(object arg) {
            var tp = arg as Tuple<uint, uint>;
            if (tp != null) {
                mapType = (EMapType) tp.Item1;
                mapId = tp.Item2;
            }
        }

        protected override void OnShow() {
            RefreshView();
        }

        #endregion

        #region 初始化

        /// <summary>
        /// 检测组件 
        /// </summary>
        private void OnParseComponent() {
            text_Name = transform.Find("Animator/View_AreaDetail/Image_Title/Text").GetComponent<Text>();
            image_Map = transform.Find("Animator/View_AreaDetail/Image_Map").GetComponent<RawImage>();
            text_Detail = transform.Find("Animator/View_AreaDetail/Text_Detail").GetComponent<Text>();

            transform.Find("ClickClose").GetComponent<Button>().onClick.AddListener(OnClick_Close);
            transform.Find("Animator/View_AreaDetail/Btn_Close").GetComponent<Button>().onClick.AddListener(OnClick_Close);
        }

        #endregion

        #region 界面显示

        /// <summary>
        /// 刷新界面
        /// </summary>
        private void RefreshView() {
            if (mapType == EMapType.Map) {
                CSVMapInfo.Data csv = CSVMapInfo.Instance.GetConfData(mapId);
                if (null == csv) return;

                text_Name.text = LanguageHelper.GetTextContent(csv.name);
                
                text_Detail.gameObject.SetActive(true);
                image_Map.gameObject.SetActive(true);

                text_Detail.text = LanguageHelper.GetTextContent(csv.des);
                ImageHelper.SetTexture(image_Map, csv.mapIcon);
            }
            else if (mapType == EMapType.Island) {
                var csv = CSVIsland.Instance.GetConfData(mapId);
                if (null == csv) return;

                text_Name.text = LanguageHelper.GetTextContent(csv.name);
                
                text_Detail.gameObject.SetActive(false);
                image_Map.gameObject.SetActive(false);
                // text_Detail.text = LanguageHelper.GetTextContent(cSVMapInfoData.des);
                // ImageHelper.SetTexture(image_Map, cSVMapInfoData.mapIcon);
            }
        }

        #endregion

        #region 响应事件

        /// <summary>
        /// 关闭按钮
        /// </summary>
        public void OnClick_Close() {
            CloseSelf(true);
        }

        #endregion

        #region 提供功能

        #endregion
    }
}