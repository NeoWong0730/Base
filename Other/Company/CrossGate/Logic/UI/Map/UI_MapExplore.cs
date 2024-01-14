using System;
using Lib.Core;
using Logic.Core;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public class UI_MapExplore : UIComponent {
        private Text text_Name;
        private Text text_CompleteTip;
        private Slider slider_Reward;
        private Text text_Reward;
        private Image image_ReceiveReward;
        private GameObject go_ReceiveFx;
        private GameObject go_ExploreTipsView;
        private Transform tr_exploreItem;
        private Transform tr_exploreNode;
        private Button btnDetail;

        // viewSource
        public Transform tr_ResourcesItem;
        public Transform tr_ResourcesNode;

        // tabs
        // 和prefab的toggle的id对应
        public enum ETabType {
            Explore = 1,
            Resource = 2,
        }

        public CP_ToggleRegistry resigtry;
        public GameObject tabResource;
        public GameObject tabResourceRedDot;

        // data
        private EMapType eMapType { get; set; }
        public UI_Map uiMap;
        public UI_MapImage rightMap;
        public uint mapId;
        private int tabId = (int) ETabType.Explore;

        public CSVMapInfo.Data csvMap {
            get { return CSVMapInfo.Instance.GetConfData(this.mapId); }
        }

        protected override void Loaded() {
            this.text_Name = this.transform.Find("Left/View_Title/Text_Name").GetComponent<Text>();

            this.btnDetail = this.transform.Find("Left/Btn_Detail").GetComponent<Button>();
            this.btnDetail.onClick.AddListener(this.OnClick_OpenAreaDetailTipsView);

            // Left/View_Explore
            this.text_CompleteTip = this.transform.Find("Left/View_Explore/View_Complete/Text").GetComponent<Text>();

            this.slider_Reward = this.transform.Find("Left/View_Explore/Btn_Reward/Image_ProcessBG").GetComponent<Slider>();
            this.text_Reward = this.transform.Find("Left/View_Explore/Btn_Reward/Text_Num").GetComponent<Text>();
            this.image_ReceiveReward = this.transform.Find("Left/View_Explore/Btn_Reward/Image_Reward").GetComponent<Image>();
            this.go_ReceiveFx = this.transform.Find("Left/View_Explore/Btn_Reward/Fx_ui_Btn_Reward").gameObject;
            this.transform.Find("Left/View_Explore/Btn_Reward").GetComponent<Button>().onClick.AddListener(this.OnClick_CheckExplorationReward);

            this.tr_exploreItem = this.transform.Find("Left/View_Explore/Scroll_View_Find/FindItem");
            this.tr_exploreNode = this.transform.Find("Left/View_Explore/Scroll_View_Find/TabList");
            this.tr_exploreItem.gameObject.SetActive(false);

            // Left/View_Resource
            Button btn = this.transform.Find("Left/View_Resource/Btn").GetComponent<Button>();
            btn.onClick.AddListener(this.OnBtnViewNpcClicked);

            this.tr_ResourcesItem = this.transform.Find("Left/View_Resource/Scroll_View_Find/FindItem");
            this.tr_ResourcesNode = this.transform.Find("Left/View_Resource/Scroll_View_Find/TabList");
            this.tr_ResourcesItem.gameObject.SetActive(false);

            // tab
            this.resigtry = this.transform.Find("Left/Menu").GetComponent<CP_ToggleRegistry>();
            this.resigtry.onToggleChange = OnTabChanged;
            this.tabResourceRedDot = this.transform.Find("Left/Menu/TabResouorce/RedDot").gameObject;
            this.tabResource = this.transform.Find("Left/Menu/TabResouorce").gameObject;

            // View_ExploreTips
            this.go_ExploreTipsView = this.transform.Find("View_ExploreTips").gameObject;
            this.transform.Find("View_ExploreTips/Blank").GetComponent<Button>().onClick.AddListener(this.OnClick_CloseExploreTipsView);
        }

        private void OnTabChanged(int curTabId, int lastTabId) {
            this.tabId = curTabId;
            Sys_Map.Instance.LastExploreTab = curTabId;
            this.RefreshContent();
        }

        public void Refresh(EMapType eMapType, uint mapId, UI_Map uiMap, int tabId, UI_MapImage uiMapImage = null) {
            this.eMapType = eMapType;
            this.mapId = mapId;
            this.uiMap = uiMap;
            this.rightMap = uiMapImage;

            this.tabId = tabId; // Sys_Map.Instance.LastExploreTab;
            // 默认是explore切页，和prefab中的toggle的id对应
            this.resigtry.SwitchTo(this.tabId, true);

            this.RefreshTabs();
            this.RefreshContent();
        }

        private void RefreshTabs() {
            if (this.eMapType == EMapType.Island) {
                this.tabResource.SetActive(false);
                this.btnDetail.gameObject.SetActive(false);
            }
            else if (this.eMapType == EMapType.Map) {
                bool show = this.csvMap.MapResource != null;
                this.tabResource.SetActive(show);
                this.btnDetail.gameObject.SetActive(show);
            }

            // resource红点
            bool unread = Sys_Map.Instance.unReadedMapResources.Contains(this.mapId);
            this.tabResourceRedDot.SetActive(unread);
        }

        private void RefreshContent() {
            var curTabType = (ETabType) this.resigtry.currentToggleID;
            if (curTabType == ETabType.Explore) {
                this.RefreshExplore();
            }
            else if (curTabType == ETabType.Resource) {
                this.RefreshResource();

                bool unread = Sys_Map.Instance.unReadedMapResources.Contains(this.mapId);
                if (unread) {
                    Sys_Map.Instance.ReqReadMapResource(this.mapId);
                }
            }

            this.rightMap?.CtrlMarkType(curTabType, markTypes, this.mapId);
        }

        public List<uint> markTypes = new List<uint>();

        private void RefreshExplore() {
            if (this.eMapType == EMapType.Island) {
                var vd = this.uiMap.list_ChildView[1] as UI_Map2;
                uint IslandId = vd.list_Island.Count - 1 < vd.IslandIndex ? 0 : vd.list_Island[vd.IslandIndex];
                CSVIsland.Data cSVIslandData = CSVIsland.Instance.GetConfData(IslandId);
                if (null == cSVIslandData) {
                    this.text_Name.text = string.Empty;
                }
                else {
                    this.text_Name.text = LanguageHelper.GetTextContent(cSVIslandData.name);
                }

                this.SetResourcesView(IslandId);
            }
            else if (eMapType == EMapType.Map) {
                CSVMapInfo.Data cSVMapInfoData = CSVMapInfo.Instance.GetConfData(this.mapId);
                if (null == cSVMapInfoData) {
                    this.text_Name.text = string.Empty;
                }
                else {
                    this.text_Name.text = LanguageHelper.GetTextContent(cSVMapInfoData.name);
                }

                this.SetResourcesView(this.mapId);
            }
        }

        void SetExploreItem(Transform tr, uint id, uint Num, uint MaxNum, bool useProgress) {
            CSVMapExplorationMark.Data cSVMapExplorationMarkData = CSVMapExplorationMark.Instance.GetConfData(id);
            if (null == cSVMapExplorationMarkData || cSVMapExplorationMarkData.resource_icon1 == 0 || cSVMapExplorationMarkData.List_Icon == 0) {
                tr.gameObject.SetActive(false);
                return;
            }

            tr.gameObject.SetActive(true);
            tr.name = id.ToString();   

            bool isOpen = Sys_Exploration.Instance.IsOpen_ResPoint(id);
            //图标
            Image image_Icon = tr.Find("Image_Explore").GetComponent<Image>();
            ImageHelper.SetIcon(image_Icon, isOpen ? cSVMapExplorationMarkData.List_Icon : 992801);
            //名字
            Text text_Name = tr.Find("Text_Tips").GetComponent<Text>();
            text_Name.text = isOpen ? LanguageHelper.GetTextContent(cSVMapExplorationMarkData.List_lan) : LanguageHelper.GetTextContent(4527);

            Slider slider_progress = tr.Find("Image_ProcessBG").GetComponent<Slider>();
            if (useProgress) {
                slider_progress.gameObject.SetActive(true);

                //进度
                Text text_progress = tr.Find("Image_ProcessBG/Text_Num").GetComponent<Text>();
                text_progress.text = string.Format("{0}/{1}", isOpen ? Num.ToString() : "?", isOpen ? MaxNum.ToString() : "?");
                //进度条
                slider_progress.value = !isOpen || MaxNum == 0 ? 0 : Num / (float) MaxNum;
            }
            else {
                slider_progress.gameObject.SetActive(false);
            }
        }

        void CreateMarkList(Transform node, Transform child, int number) {
            while (node.childCount < number) {
                Transform tr = GameObject.Instantiate(child, node);
                tr.Find("Image_BG").GetComponent<Button>().onClick.AddListener(() => { OnClick_OpenExploreTipsView(tr.gameObject); });
            }
        }

        void OnClick_OpenExploreTipsView(GameObject go) {
            uint id = 0;
            if (!uint.TryParse(go.name, out id))
                return;
            bool isOpen = Sys_Exploration.Instance.IsOpen_ResPoint(id);
            if (!isOpen)
                return;
            this.go_ExploreTipsView.SetActive(true);
            this.SetExploreTipsView(this.go_ExploreTipsView.transform, id);
        }

        void SetExploreTipsView(Transform tr, uint id) {
            CSVMapExplorationMark.Data cSVMapExplorationMarkData = CSVMapExplorationMark.Instance.GetConfData(id);
            if (null == cSVMapExplorationMarkData) {
                tr.gameObject.SetActive(false);
                return;
            }

            tr.gameObject.SetActive(true);

            Text text_Title = tr.Find("Image_Title/Text").GetComponent<Text>();
            text_Title.text = LanguageHelper.GetTextContent(cSVMapExplorationMarkData.List_lan);
            Image image_Icon = tr.Find("Image_Title/Image_Mark").GetComponent<Image>();
            ImageHelper.SetIcon(image_Icon, cSVMapExplorationMarkData.List_Icon);
            Text text_Explain = tr.Find("Image_BG/Text").GetComponent<Text>();
            text_Explain.text = LanguageHelper.GetTextContent(cSVMapExplorationMarkData.Des_lan);
            RectTransform rectTransform = tr.Find("Image_BG").GetComponent<RectTransform>();
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
        }

        private void SetResourcesView(uint ExplorationId) {
            uint x = 0, y = 0, z = 0;
            Sys_Exploration.ExplorationData explorationData = Sys_Exploration.Instance.GetExplorationData(ExplorationId);
            switch (this.eMapType) {
                case EMapType.Island: {
                    this.text_CompleteTip.text = LanguageHelper.GetTextContent(4533);
                }
                    break;
                case EMapType.Map: {
                    this.text_CompleteTip.text = LanguageHelper.GetTextContent(4532);
                }
                    break;
                default: {
                    this.text_CompleteTip.text = LanguageHelper.GetTextContent(4532);
                }
                    break;
            }

            if (null == explorationData) {
                this.slider_Reward.value = 0;
                this.text_Reward.text = "0%";
                ImageHelper.SetIcon(this.image_ReceiveReward, 993101);
                this.go_ReceiveFx.SetActive(false);
                for (int i = 0, count = this.tr_exploreNode.childCount; i < count; i++) {
                    this.SetExploreItem(this.tr_exploreNode.GetChild(i), x, y, z, true);
                }
            }
            else {
                //总进度
                uint CurNum = explorationData.totalProcess.CurNum;
                uint MaxNum = explorationData.totalProcess.TargetNum;
                float value = MaxNum == 0 ? 0 : CurNum / (float) MaxNum;
                this.slider_Reward.value = value;
                this.text_Reward.text = string.Format("{0}%", MaxNum == 0 ? "0" : System.Math.Round(CurNum / (float) MaxNum * 100, 1).ToString());
                //是否领奖全部领取
                bool IsComplete = explorationData.IsReceivedAllReward();
                ImageHelper.SetIcon(this.image_ReceiveReward, !IsComplete ? 993101 : (uint) 993102);
                //是否有可领取奖励
                List<uint> list = explorationData.GetReceiveRewardList();
                this.go_ReceiveFx.SetActive(list.Count > 0);
                //设置资源点
                List<ENPCMarkType> Keys = new List<ENPCMarkType>(explorationData.dict_Process.Keys);
                markTypes = Keys.ConvertAll((markType) => {
                    return (uint) markType;
                });
                
                this.CreateMarkList(this.tr_exploreNode, this.tr_exploreItem, Keys.Count);
                for (int i = 0, count = this.tr_exploreNode.childCount; i < count; i++) {
                    Transform child = this.tr_exploreNode.GetChild(i);
                    if (i < Keys.Count) {
                        Sys_Exploration.ExplorationProcess explorationProcess = explorationData.GetExplorationProcess(Keys[i]);
                        x = (uint) Keys[i];
                        y = explorationProcess.CurNum;
                        z = explorationProcess.TargetNum;
                    }
                    else {
                        x = 0;
                        y = 0;
                        z = 0;
                    }

                    this.SetExploreItem(child, x, y, z, true);
                }
            }
        }

        private void SetResPoint() {
            uint x = 0;
            var keys = this.csvMap.MapResource;
            this.markTypes = keys;
            
            this.CreateMarkList(this.tr_ResourcesNode, this.tr_ResourcesItem, keys.Count);
            for (int i = 0, count = this.tr_ResourcesNode.childCount; i < count; i++) {
                Transform child = this.tr_ResourcesNode.GetChild(i);
                if (i < keys.Count) {
                    x = keys[i];
                }
                else {
                    x = 0;
                }

                this.SetExploreItem(child, x, 0, 0, false);
            }
        }

        private void RefreshResource() {
            if (this.eMapType == EMapType.Map) {
                this.SetResPoint();
            }
        }

        private void OnBtnViewNpcClicked() {
            UIManager.OpenUI(EUIID.UI_MapFilterNpc, false, new Tuple<uint, uint>(this.mapId, 1));
        }

        private void OnClick_CheckExplorationReward() {
            Sys_Exploration.ExplorationData explorationData = null;
            switch (this.eMapType) {
                case EMapType.Island: {
                    var vd = this.uiMap.list_ChildView[1] as UI_Map2;
                    uint islandId = vd.list_Island.Count - 1 < vd.IslandIndex ? 0 : vd.list_Island[vd.IslandIndex];
                    explorationData = Sys_Exploration.Instance.GetExplorationData(islandId);
                }
                    break;
                case EMapType.Map: {
                    explorationData = Sys_Exploration.Instance.GetExplorationData(this.mapId);
                }
                    break;
            }

            if (null == explorationData)
                return;

            List<uint> list = explorationData.GetReceiveRewardList();
            if (list.Count <= 0) {
                UIManager.OpenUI(EUIID.UI_MapAward, false, explorationData.mapId);
            }
            else {
                Sys_Npc.Instance.ReqNpcGetActiveReward(list[0]);
            }
        }

        private void OnClick_CloseExploreTipsView() {
            this.go_ExploreTipsView.SetActive(false);
        }

        private void OnClick_OpenAreaDetailTipsView() {
            UIManager.OpenUI(EUIID.UI_MapAreaDetail, false, new Tuple<uint, uint>((uint) this.eMapType, this.mapId));
        }

        protected override void ProcessEventsForEnable(bool toRegister) {
            Sys_Npc.Instance.eventEmitter.Handle(Sys_Npc.EEvents.OnUpdateResPoint, this.OnUpdateResPoint, toRegister);
            Sys_Exploration.Instance.eventEmitter.Handle(Sys_Exploration.EEvents.ExplorationRewardNotice, this.OnUpdateResPoint, toRegister);
            Sys_Map.Instance.eventEmitter.Handle<uint>(Sys_Map.EEvents.OnReadMapResource, OnReadMapResource, toRegister);
        }

        /// <summary>
        /// 资源点更新事件
        /// </summary>
        public void OnUpdateResPoint() {
            this.RefreshContent();
        }

        private void OnReadMapResource(uint mapId) {
            if (mapId == this.mapId) {
                this.RefreshTabs();
            }
        }
    }
}