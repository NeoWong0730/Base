using Logic.Core;
using Lib.AssetLoader;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Framework;

namespace Logic
{
    /// <summary> 迷你地图 </summary>
    public class UI_MiniMap : UIComponent
    {
        #region 界面组件
        /// <summary> 地图名字 </summary>
        private Text text_Name;
        /// <summary> 地图遮罩 </summary>
        private RectTransform rt_MapMask;
        /// <summary> 地图图片1节点(显示大小) </summary>
        private RectTransform rt_MapImage1;
        /// <summary> 地图图片2节点(旋转后大小) </summary>
        private RectTransform rt_MapImage2;
        /// <summary> 玩家标记 </summary>
        private RectTransform rt_PlayerPoint;
        /// <summary> 地图画像 </summary>
        private UI_MapImage ui_MapImage;
        /// <summary> 资源节点 </summary>
        private Transform tr_ResourceNode;
        #endregion
        #region 数据
        /// <summary> 地图数据 </summary>
        private CSVMapInfo.Data csvMapInfoData
        {
            get;
            set;
        }
        /// <summary> 玩家坐标 </summary>
        private Vector2 playerPosition
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
        /// <summary> 遮罩中心点 </summary>
        private Vector3 markCenter
        {
            get;
            set;
        }
        /// <summary> 地图与遮罩差 </summary>
        private Vector2 mapDifference
        {
            get;
            set;
        }
        /// <summary> 平滑缓存速度 </summary>
        private Vector3 currentVelocity = Vector3.zero;
        /// <summary> 地图四角 </summary>
        public Vector3[] corner1 = new Vector3[4];
        public Vector3[] corner2 = new Vector3[4];
        #endregion
        #region 系统函数        
        protected override void Loaded()
        {
            base.Loaded();
            OnParseComponent();
        }
        public override void OnDestroy()
        {
            ui_MapImage?.OnDestroy();
            base.OnDestroy();
        }
        public override void Show()
        {
            base.Show();
            ui_MapImage?.Show();
            RefreshView();
        }
        public override void Hide()
        {
            ui_MapImage?.Hide();
            base.Hide();
        }
        protected override void Update()
        {
            ui_MapImage?.ExecUpdate();
            UpdatePlayerPoint();
            UpdateMapPoint(true);
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            base.ProcessEventsForEnable(toRegister);
            OnProcessEventsForEnable(toRegister);
        }
        #endregion
        #region 初始化
        /// <summary>
        /// 检测组件
        /// </summary>
        private void OnParseComponent()
        {
            ui_MapImage = AddComponent<UI_MapImage>(transform.Find("MapBG/Image_Map/MapNode/View_Map"));
            text_Name = transform.Find("MapBG/Text_Name").GetComponent<Text>();

            rt_MapMask = transform.Find("MapBG/Image_Map").GetComponent<RectTransform>();

            rt_MapImage1 = transform.Find("MapBG/Image_Map/MapNode/View_Map").GetComponent<RectTransform>();
            rt_MapImage2 = transform.Find("MapBG/Image_Map/MapNode").GetComponent<RectTransform>();

            rt_PlayerPoint = transform.Find("MapBG/Image_Map/MapNode/View_Map/Image_Person").GetComponent<RectTransform>();
            tr_ResourceNode = transform.Find("MapBG/Image_Map/MapNode/View_Map/ResourceNode");

            transform.Find("MapBG/Image_Map/MapNode/View_Map").GetComponent<Button>().onClick.AddListener(OnClick_Map);
        }
        /// <summary>
        /// 注册回调事件
        /// </summary>
        private void OnProcessEventsForEnable(bool toRegister)
        {
            Sys_Map.Instance.eventEmitter.Handle(Sys_Map.EEvents.OnHeroTel, OnHeroTel, toRegister);
        }
        #endregion
        #region 界面显示
        /// <summary>
        /// 
        /// </summary>
        public void RefreshView()
        {
            UpdateMap();
            UpdateMapAngle(45f);
            UpdatePlayerPoint();
            UpdateMapPoint(true);
        }
        /// <summary>
        /// 更新地图
        /// </summary>
        public void UpdateMap()
        {
            csvMapInfoData = CSVMapInfo.Instance.GetConfData(Sys_Map.Instance.CurMapId);
            if (null == csvMapInfoData) return;

            mapSize = new Vector2(csvMapInfoData.length, csvMapInfoData.width);
            markCenter = new Vector2(rt_MapMask.rect.width / 2, -rt_MapMask.rect.height / 2);

            ui_MapImage?.SetData(csvMapInfoData.id, UI_MapImage.MapImageType.MiniMap);
            ui_MapImage?.OnRefresh();
        }
        /// <summary>
        /// 更新角色坐标
        /// </summary>
        public void UpdatePlayerPoint()
        {
            if (null != GameCenter.mainHero && null != GameCenter.mainHero.transform)
            {
                var position = PosConvertUtil.Client2Svr(GameCenter.mainHero.transform.position);
                if (position != playerPosition)
                {
                    playerPosition = position;
                    text_Name.text = LanguageHelper.GetTextContent(4502, csvMapInfoData == null ? string.Empty : LanguageHelper.GetTextContent(csvMapInfoData.name), ((uint)(playerPosition.x / 100)).ToString(), ((uint)(playerPosition.y / 100)).ToString());
                }
            }
        }
        /// <summary>
        /// 更新地图坐标
        /// </summary>
        public void UpdateMapPoint(bool Immediately = false)
        {
            float mapDifferencex = rt_MapImage2.rect.width - rt_MapMask.rect.width;
            float mapDifferencey = rt_MapImage2.rect.height - rt_MapMask.rect.height;
            //默认地图左上角开始（0，0）
            if (mapDifferencex <= 0 && mapDifferencey <= 0)
                return;

            Vector3 offset = markCenter - rt_MapImage2.InverseTransformPoint(rt_PlayerPoint.position);

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

            if (Immediately)
                rt_MapImage2.localPosition = offset;
            else
                rt_MapImage2.localPosition = Vector3.SmoothDamp(rt_MapImage2.localPosition, offset, ref currentVelocity, 0.3f);
        }
        /// <summary>
        /// 设置地图角度
        /// </summary>
        /// <param name="angle"></param>
        public void UpdateMapAngle(float angle)
        {
            rt_MapImage1.anchoredPosition = Vector2.zero;
            rt_MapImage1.localEulerAngles = new Vector3(0, 0, angle);
            rt_MapImage1.GetWorldCorners(corner1);
            GetLocalPoint(rt_MapImage2, corner1);
            Vector4 v4_Visible = GetVector4(corner1); //可见地图区域

            rt_MapImage2.anchoredPosition = Vector2.zero;
            rt_MapImage2.sizeDelta = new Vector2(v4_Visible.z - v4_Visible.x, v4_Visible.w - v4_Visible.y);
            rt_MapImage2.GetWorldCorners(corner2);
            GetLocalPoint(rt_MapImage2, corner2);
            Vector4 v4_Reality = GetVector4(corner2); //实际地图区域

            rt_MapImage1.anchoredPosition = new Vector2(v4_Reality.x, v4_Reality.y) - new Vector2(v4_Visible.x, v4_Visible.y);

            //下级资源位置归正
            for (int i = 0; i < tr_ResourceNode.childCount; i++)
            {
                tr_ResourceNode.GetChild(i).transform.localEulerAngles = new Vector3(0, 0, -angle);
            }

            rt_PlayerPoint.localEulerAngles = new Vector3(0, 0, -angle);
        }
        #endregion
        #region 响应事件
        /// <summary>
        ///点击地图事件
        /// </summary>
        public void OnClick_Map()
        {
            if (!Sys_FamilyResBattle.Instance.InFamilyBattle) {
                UIManager.OpenUI(EUIID.UI_Map);
            }
            else {
                UIManager.OpenUI(EUIID.UI_FamilyResBattleMap);
            }
        }
        /// <summary>
        /// 英雄传送(包括切换地图)
        /// </summary>
        public void OnHeroTel()
        {
            Refresh();
        }
        #endregion
        #region 提供功能
        /// <summary>
        /// 得到坐标最小最大值
        /// </summary>
        /// <param name="corners"></param>
        /// <returns></returns>
        public Vector4 GetVector4(Vector3[] corners)
        {
            float minX = 0, minY = 0;
            float maxX = 0, maxY = 0;

            for (int i = 0; i < corners.Length; i++)
            {
                if (i == 0)
                {
                    maxX = minX = corners[i].x;
                    maxY = minY = corners[i].y;
                }
                else
                {
                    if (corners[i].x < minX)
                        minX = corners[i].x;
                    if (corners[i].y < minY)
                        minY = corners[i].y;
                    if (corners[i].x > maxX)
                        maxX = corners[i].x;
                    if (corners[i].y > maxY)
                        maxY = corners[i].y;
                }
            }
            return new Vector4(minX, minY, maxX, maxY);
        }
        /// <summary>
        /// 世界坐标转换相对坐标
        /// </summary>
        /// <param name="rectTransform"></param>
        /// <param name="corners"></param>
        private void GetLocalPoint(RectTransform rectTransform, Vector3[] corners)
        {
            for (int i = 0; i < corners.Length; i++)
            {
                corners[i] = rectTransform.InverseTransformPoint(corners[i]);
            }
        }
        #endregion
    }
}


