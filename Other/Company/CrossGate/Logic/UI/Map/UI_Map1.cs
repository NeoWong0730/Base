using Lib.Core;
using Logic.Core;
using System.Collections.Generic;
using Framework;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    /// <summary> 世界地图 </summary>
    public class UI_Map1 : UIComponent, UI_Map1.Country.IInterface
    {
        public class Country : UIComponent {
            public List<Transform> list_island = new List<Transform>();
            
            protected override void Loaded() {
                list_island.Clear();
                for (int i = 0; i < transform.childCount; i++)
                {
                    list_island.Add(transform.GetChild(i));
                }
                for (int i = 0, count = list_island.Count; i < count; i++)
                {
                    var item = list_island[i];
                    item.GetComponent<Button>().onClick.AddListener(() => {
                        listener?.OnClickIsland(item.gameObject);
                    });
                }
            }

            public IInterface listener;
            public interface IInterface {
                void OnClickIsland(GameObject gameObject);
            }
        }
        
        #region 界面组件
        public UIScrollPage<UIPageDot> scrollPage = new UIScrollPage<UIPageDot>();
        private Dictionary<uint, Country> dict_country = new Dictionary<uint, Country>();

        public Text exploreRate;
        public Image countryIcon;
        #endregion

        #region 数据
        #endregion

        #region 系统函数
        protected override void Loaded()
        {
            base.Loaded();
            OnParseComponent();
        }
        public override void Show()
        {
            base.Show();
            Refresh();
        }
        protected override void Refresh()
        {
            base.Refresh();
            RefreshAll((int)mapId - 1);
        }
        #endregion

        #region 初始化
        /// <summary>
        /// 检测组件
        /// </summary>
        private void OnParseComponent()
        {
            var t = this.transform.Find("View_Bottom/Scroll_Switch");
            scrollPage.Init(t);
            scrollPage.onPageSwicth = OnPageSwicth;
            scrollPage.onVdClicked = OnVdClicked;

            countryIcon = transform.Find("Image_Title").GetComponent<Image>();
            
            AssetDependencies dep = transform.Find("MapsScale/Countrys").GetComponent<AssetDependencies>();
            dict_country.Clear();
            var csvIsland = CSVIsland.Instance.GetAll();
            for (int i = 0, length = csvIsland.Count; i < length; ++i) {
                var one = csvIsland[i];
                if (one.island_type == 1) {
                    Country c = new Country();
                    var cNode = GameObject.Instantiate(dep.mCustomDependencies[i] as GameObject, dep.transform).transform;
                    c.Init(cNode);
                    c.listener = this;
                    dict_country.Add(one.id, c);
                }
            }
            
            this.exploreRate = transform.Find("MapsScale/Btn_Gift/Num").GetComponent<Text>();

            Button btn = transform.Find("MapsScale/Btn_Gift").GetComponent<Button>();
            btn.onClick.AddListener(OnBtnRewardClicked);
        }
        
        public void OnClickIsland(GameObject gameObject) {
            OnClick_SwitchMap(gameObject);
        }

        private void OnBtnRewardClicked() {
            if (explorationData != null) {
                List<uint> list = explorationData.GetReceiveRewardList();
                if (list.Count <= 0) {
                    UIManager.OpenUI(EUIID.UI_MapAward, false, explorationData.mapId);
                }
                else {
                    Sys_Npc.Instance.ReqNpcGetActiveReward(list[0]);
                }
            }
        }

        private void OnPageSwicth(int pageIndex, int startIndex, int range) {
            // 移动scroll
            scrollPage.centerOn.SetCurrentPageIndex(pageIndex, true);
            // 模拟点击vd
            scrollPage.vds[pageIndex].SetSelected(true, true);
            
            // 切换地图之后，需要通知给UI_Map
            var countryId = pageIndex + 1;
            uiMap.curMap[0] = countryId;
            
            // 刷新当前页面逻辑
            RefreshView(pageIndex);
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

        private void OnVdClicked(int pageIndex) {
            // 移动scroll
            scrollPage.centerOn.SetCurrentPageIndex(pageIndex, true);
            
            scrollPage.pageSwitcher.SetCurrentIndex(pageIndex);
            _TmpHandle();

            // 切换地图之后，需要通知给UI_Map
            var countryId = pageIndex + 1;
            uiMap.curMap[0] = countryId;
            
            // 刷新当前页面逻辑
            RefreshView(pageIndex);
        }

        #endregion

        #region 界面显示
        private UI_Map uiMap;
        private uint mapId;
        private Sys_Exploration.ExplorationData explorationData;
        
        public override void SetData(params object[] arg) {
            this.mapId = System.Convert.ToUInt32(arg[1]);
            this.uiMap = arg[3] as UI_Map;
        }

        /// <summary>
        /// 刷新界面
        /// </summary>
        public void RefreshAll(int index) {
            RefreshView(index);
            
            scrollPage.Refresh(this.dict_country.Count, true, true);
            scrollPage.SetIndex(index);
        }

        public void RefreshView(int index) {
            int countryId = index + 1;
            foreach (var kvp in dict_country) {
                if (countryId == kvp.Key) {
                    kvp.Value.Show();
                    
                    for (int i = 0, count = kvp.Value.list_island.Count; i < count; i++)
                    {
                        var item = kvp.Value.list_island[i];
                        SetMapItem(item);
                    }
                }
                else {
                    kvp.Value.Hide();
                }
            }
            
            this.uiMap.mapExplore.Hide();
            this.explorationData = Sys_Exploration.Instance.GetExplorationData((uint)countryId);
            if (this.explorationData == null) {
                this.exploreRate.text = "0%";
            }
            else {
                uint curNum = this.explorationData.totalProcess.CurNum;
                uint maxNum = this.explorationData.totalProcess.TargetNum;
                float value = maxNum == 0 ? 0 : curNum / (float) maxNum;
                this.exploreRate.text = string.Format("{0}%", maxNum == 0 ? "0" : System.Math.Round(curNum / (float) maxNum * 100, 1).ToString());
            }
            
            // 更新tab的名字
            uiMap.RefreshTabNames((uint)countryId);
            
            ImageHelper.SetIcon(countryIcon, CSVIsland.Instance.GetConfData((uint)countryId).icon, true);
        }
        /// <summary>
        /// 设置地图模版
        /// </summary>
        /// <param name="tr"></param>
        public void SetMapItem(Transform tr)
        {
            uint IslandId = 0;
            uint.TryParse(tr.name, out IslandId);

            CSVIsland.Data cSVIslandData = CSVIsland.Instance.GetConfData(IslandId);
            if (null == cSVIslandData)
            {
                tr.gameObject.SetActive(false);
                return;
            }
            tr.gameObject.SetActive(true);

            Text text_Name = tr.Find("Text_Name").GetComponent<Text>();
            string Name = LanguageHelper.GetTextContent(cSVIslandData.name);
            Text text_Limit1 = tr.Find("Text_Limit").GetComponent<Text>();
            Text text_Limit2 = tr.Find("Text_Limit02").GetComponent<Text>();
            string Limit = cSVIslandData.map_lv.Count >= 2 ? LanguageHelper.GetTextContent(4501, cSVIslandData.map_lv[0].ToString(), cSVIslandData.map_lv[1].ToString()) : string.Empty;
            text_Limit1.gameObject.SetActive(true);
            text_Limit2.gameObject.SetActive(false);
            GameObject go_Mark = tr.Find("Image_Self").gameObject;
            CSVMapInfo.Data cSVMapInfoData = CSVMapInfo.Instance.GetConfData(Sys_Map.Instance.CurMapId);
            go_Mark.gameObject.SetActive(cSVMapInfoData?.map_node?[1] == IslandId);

            Slider slider_Progress = tr.Find("Image_ProcessBG").GetComponent<Slider>();
            Text text_Progress = tr.Find("Image_ProcessBG/Text_Num").GetComponent<Text>();

            uint CurNum = 0, MaxNum = 0;
            Sys_Exploration.ExplorationData explorationData = Sys_Exploration.Instance.GetExplorationData(IslandId);
            if (null != explorationData)
            {
                CurNum = explorationData.totalProcess.CurNum;
                MaxNum = explorationData.totalProcess.TargetNum;
            }
            slider_Progress.value = MaxNum == 0 ? 0 : (float)CurNum / (float)MaxNum;
            text_Progress.text = string.Format("{0}%", MaxNum == 0 ? "0" : System.Math.Round((float)CurNum / (float)MaxNum * 100, 1).ToString());

            Image image_Bg = tr.GetComponent<Image>();
            //存在内存备份图片，导致内存多一份，影响效率问题
            image_Bg.alphaHitTestMinimumThreshold = 0.1f;
            Image image_TitleBg = tr.Find("Image_BG01").GetComponent<Image>();

            if (cSVIslandData?.map_lv?.Count >= 2 && Sys_Role.Instance.Role.Level >= cSVIslandData?.map_lv[0]) //已达到等级
            {
                TextHelper.SetText(text_Name, 4523, Name);
                TextHelper.SetText(text_Limit1, 4525, Limit);
                slider_Progress.gameObject.SetActive(true);
                ImageHelper.SetImageGray(image_Bg, false, false);
                ImageHelper.SetImageGray(image_TitleBg, false, false);
            }
            else //未达到等级
            {
                TextHelper.SetText(text_Name, 4524, Name);
                TextHelper.SetText(text_Limit1, 4526, Limit);
                slider_Progress.gameObject.SetActive(false);
                ImageHelper.SetImageGray(image_Bg, true, false);
                ImageHelper.SetImageGray(image_TitleBg, true, false);
            }

            var markNode = tr.Find("Grid_Mark");
            TaskHelper.IsNpcInThisMap(uiMap.npcs, EMapType.Island, IslandId, ref taskCatagaries);
            FrameworkTool.CreateChild(in markNode, in uiMap.tags, in taskCatagaries);
        }

        private List<int> taskCatagaries = new List<int>();
        #endregion

        #region 响应事件
        /// <summary>
        /// 切换地图
        /// </summary>
        /// <param name="go"></param>
        public void OnClick_SwitchMap(GameObject go)
        {
            int id = 0;
            int.TryParse(go.name, out id);
            if (id <= 0) return;

            CSVIsland.Data cSVIslandData = CSVIsland.Instance.GetConfData((uint)id);

            if (cSVIslandData?.map_lv?.Count >= 2 && Sys_Role.Instance.Role.Level >= cSVIslandData?.map_lv[0]) //已达到等级
            {
                UI_Map.onSwitchMap(EMapType.Island, (uint)id);
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4515));
            }
        }
        #endregion
    }
}

