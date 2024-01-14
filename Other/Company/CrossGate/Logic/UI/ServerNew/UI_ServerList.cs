using System;
using System.Collections.Generic;
using Logic.Core;
using Packet;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public class UI_ServerItem_Layout {
        #region UI Variable Statement

        public GameObject mRoot { get; private set; }
        public Transform mTrans { get; private set; }
        public RectTransform ServerItem_RectTransform { get; private set; }
        public GameObject Image_Role { get; private set; }
        public CP_Toggle toggle { get; private set; }
        public Image Image_State_Image { get; private set; }
        public Text ServerName_Text { get; private set; }
        public Text Text_Count { get; private set; }
        public Image Image_New_Icon_Image { get; private set; }
        public Image Image_Recommend_Image { get; private set; }
        public Image Image_Hot_Image { get; private set; }
        public GameObject SubTitleParent { get; private set; }
        public Transform unregisterNode { get; private set; }
        public Text SubTitle { get; private set; }

        #endregion

        public void Parse(GameObject root) {
            this.mRoot = root;
            this.mTrans = root.transform;
            this.toggle = this.mTrans.gameObject.GetComponent<CP_Toggle>();
            this.ServerItem_RectTransform = this.mTrans.GetComponent<RectTransform>();
            this.Image_Role = this.mTrans.Find("Image_Role").gameObject;
            this.Image_State_Image = this.mTrans.Find("Image_State").GetComponent<Image>();
            this.ServerName_Text = this.mTrans.Find("Text_ServerName").GetComponent<Text>();
            this.Text_Count = this.mTrans.Find("Count").GetComponent<Text>();
            this.Image_New_Icon_Image = this.mTrans.Find("Tags/Image_New_Icon").GetComponent<Image>();
            this.Image_Recommend_Image = this.mTrans.Find("Tags/Image_Recommend").GetComponent<Image>();
            this.Image_Hot_Image = this.mTrans.Find("Tags/Image_Hot").GetComponent<Image>();
            this.SubTitle = this.mTrans.Find("Tips/Tips01").GetComponent<Text>();
            this.SubTitleParent = this.mTrans.Find("Tips").gameObject;
            this.unregisterNode = this.mTrans.Find("Image_Close");
        }

        public void RegisterEvents(IListener listener) {
            this.toggle.onValueChanged.AddListener(listener.Switch);
        }

        public interface IListener {
            void Switch(bool flag);
        }
    }

    public class UI_ServerItem : UISelectableElement, UI_ServerItem_Layout.IListener {
        public UI_ServerItem_Layout Layout = new UI_ServerItem_Layout();
        private ServerEntry mServerEntry;

        private UI_ServerList ui;
        private UI_ServerList.EServerItemType itemType;
        private int index;

        public void Parse(UI_ServerList ui, GameObject root) {
            this.ui = ui;
            this.Layout.Parse(root);
            this.Layout.RegisterEvents(this);
        }

        public void Switch(bool arg) {
            if (arg) {
                // Sys_Login.Instance.SetSelected(this.mServerEntry);
                this.ui.OnItemClicked(this.itemType, this.mServerEntry, this.index);
            }
        }

        public void SetData(int index, ServerEntry entry, UI_ServerList.EServerItemType itemType = UI_ServerList.EServerItemType.Normal) {
            this.mServerEntry = entry;
            this.itemType = itemType;
            this.index = index;

#if UNITY_EDITOR
            this.Layout.ServerName_Text.text = string.Format("{0}:{1}", entry.mServerInfo.ServerName, entry.mServerInfo.ServerId);
#else
            Layout.ServerName_Text.text = entry.mServerInfo.ServerName;
#endif
            var tag = entry.tags;
            this.Layout.Image_New_Icon_Image.gameObject.SetActive(false);
            this.Layout.Image_Recommend_Image.gameObject.SetActive(false);
            this.Layout.Image_Hot_Image.gameObject.SetActive(false);

            if (tag.Contains("NEW")) {
                this.Layout.Image_New_Icon_Image.gameObject.SetActive(true);
            }
            else if (tag.Contains("HOT")) {
                this.Layout.Image_Hot_Image.gameObject.SetActive(true);
            }
            else if (tag.Contains("RECOMMEND")) {
                this.Layout.Image_Recommend_Image.gameObject.SetActive(true);
            }

            if (entry.mRoleInfo.Count > 0) {
                this.Layout.Text_Count.gameObject.SetActive(true);
                this.Layout.Image_Role.SetActive(true);

                int count = entry.mRoleInfo.Count;
                TextHelper.SetText(this.Layout.Text_Count, count.ToString());
            }
            else {
                this.Layout.Text_Count.gameObject.SetActive(false);
                this.Layout.Image_Role.SetActive(false);
            }
            
            this.Layout.SubTitleParent.gameObject.SetActive(false);
            if (!string.IsNullOrEmpty(entry.SubTitle)) {
                TextHelper.SetText(this.Layout.SubTitle, entry.SubTitle);
                this.Layout.SubTitleParent.gameObject.SetActive(true);
            }

            entry.GetState(out uint stateIcon, out uint stateText);
            // var color = entry.mServerInfo.Color.ToStringUtf8();
            ImageHelper.SetIcon(this.Layout.Image_State_Image, stateIcon);

            if (Layout.unregisterNode != null) {
                Layout.unregisterNode.gameObject.SetActive(entry.mServerInfo.Closereg == 1);
            }
        }
    }

    public class UI_ServerList : UIBase {
        public enum EServerItemType {
            None = 0,
            Latest = 1,
            Recommend = 2,
            Normal = 3,
        }

        public class TabItem : UISelectableElement {
            public CP_Toggle toggle;
            public Text light;
            public Text dark;

            protected override void Loaded() {
                this.light = this.transform.Find("Image_Menu_Light/_Text_Menu_Light").GetComponent<Text>();
                this.dark = this.transform.Find("Btn_Menu_Dark/_Text_Menu_Dark").GetComponent<Text>();

                this.toggle = this.gameObject.GetComponent<CP_Toggle>();
                this.toggle.onValueChanged.AddListener(this.Switch);
            }

            public override void SetSelected(bool toSelected, bool force) {
                this.toggle.SetSelected(toSelected, force);
            }

            public void Switch(bool arg) {
                if (arg) {
                    this.onSelected?.Invoke(this.id, true);
                }
            }

            public void Refresh(uint tabId) {
                var group = Sys_Login.Instance.tabs[tabId];
                string content = group.GroupName.ToStringUtf8();
                this.light.text = content;
                this.dark.text = content;
            }
        }

        public class Tipitem : UIComponent {
            public GameObject charNode;
            public Image charIcon;
            public Text charName;
            public Text careerName;
            public Image careerIcon;
            public GameObject careerGo;
            public Text charLevel;
            public Button charButton;

            public GameObject plusNode;
            public Button plusButton;

            public DirRoleInfo dirRoleInfo;
            public ServerEntry serverEntry;
            public UI_ServerList ui;

            protected override void Loaded() {
                this.charNode = this.transform.Find("View_Had").gameObject;
                this.careerGo = this.transform.Find("View_Had/Image_Prop").gameObject;
                this.careerIcon = this.transform.Find("View_Had/Image_Prop").GetComponent<Image>();
                this.careerName = this.transform.Find("View_Had/Image_Prop/Text_Profession").GetComponent<Text>();

                this.charIcon = this.transform.Find("View_Had/Image_HeadBG").GetComponent<Image>();
                this.charName = this.transform.Find("View_Had/Text_Name").GetComponent<Text>();
                this.charLevel = this.transform.Find("View_Had/Text_Level/Text_Num").GetComponent<Text>();
                this.charButton = this.transform.Find("View_Had/Image_Bg").GetComponent<Button>();
                this.charButton.onClick.AddListener(this.OnBtnLoginClicked);

                this.plusNode = this.transform.Find("View_Create").gameObject;
                this.plusButton = this.transform.Find("View_Create/Image_Create").GetComponent<Button>();
                this.plusButton.onClick.AddListener(this.OnBtnCreateRoleClicked);
            }

            private void OnBtnLoginClicked() {
                if (!NetworkHelper.IsWanOrLanOpen()) {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10638));
                    return;
                }

                Sys_Login.Instance.SetSelected(this.serverEntry);
                Sys_Login.Instance.selectedRoleId = this.dirRoleInfo.RoleId;
                Sys_Login.Instance.selectedServerId = (int) this.serverEntry.mServerInfo.ServerId;

                if (Sys_Net.Instance.Connect(this.serverEntry.mServerInfo.ServerIp, (int) this.serverEntry.mServerInfo.ServerPort)) {
                    UIManager.OpenUI(EUIID.UI_BlockClickNetwork, false, 10f);
                }
            }

            private void OnBtnCreateRoleClicked() {
                if (serverEntry.mServerInfo.Closereg == 1) {
                    void OnConform() {
                        ui.RefreshCurrentTab(EMode.Force);
                    }
                    
                    PromptBoxParameter.Instance.Clear();
                    PromptBoxParameter.Instance.content = CSVLanguage.Instance.GetConfData(15120).words;
                    PromptBoxParameter.Instance.SetConfirm(true, OnConform, 15121);
                    PromptBoxParameter.Instance.SetCancel(false, null, 15121);
                    UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
                    return;
                }
                
                if (!NetworkHelper.IsWanOrLanOpen()) {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10638));
                    return;
                }

                HitPointManager.HitPoint("game_createrole_click");
                Sys_Login.Instance.SetSelected(this.serverEntry);
                Sys_Login.Instance.selectedRoleId = 0;
                Sys_Login.Instance.selectedServerId = (int) this.serverEntry.mServerInfo.ServerId;

                if (Sys_Net.Instance.Connect(this.serverEntry.mServerInfo.ServerIp, (int) this.serverEntry.mServerInfo.ServerPort)) {
                    UIManager.OpenUI(EUIID.UI_BlockClickNetwork, false, 10f);
                }
            }

            public void RefreshData(ServerEntry serverEntry, DirRoleInfo dirRoleInfo, UI_ServerList ui) {
                this.serverEntry = serverEntry;
                this.dirRoleInfo = dirRoleInfo;
                this.ui = ui;

                if (dirRoleInfo == null) {
                    this.charNode.SetActive(false);
                    this.plusNode.SetActive(true);
                }
                else {
                    this.charNode.SetActive(true);
                    this.plusNode.SetActive(false);

                    Table.CSVCharacter.Data csv = Table.CSVCharacter.Instance.GetConfData(dirRoleInfo.HeroId);
                    if (csv != null) {
                        ImageHelper.SetIcon(this.charIcon, csv.headid);
                    }

                    uint careerId = dirRoleInfo.Career == 0 ? 100 : dirRoleInfo.Career;
                    Table.CSVCareer.Data csvCareer = Table.CSVCareer.Instance.GetConfData(careerId);
                    if (csvCareer != null) {
                        this.careerGo.SetActive(true);
                        ImageHelper.SetIcon(this.careerIcon, csvCareer.logo_icon);
                        TextHelper.SetText(this.careerName, csvCareer.name);
                    }
                    else {
                        this.careerGo.SetActive(false);
                    }

                    this.charName.text = dirRoleInfo.RoleName;
                    TextHelper.SetText(this.charLevel, 11004, dirRoleInfo.RoleLv.ToString());
                }
            }
        }

        // 每页显示的服务器数量
        public static readonly int MY_SERVER_COUNT = 8;
        public static readonly int MAX_CHAR_COUNT = 6;

        public bool IsTipShowing = false;
        public EServerItemType showTipItemType = EServerItemType.None;
        public int showTipItemIndex = -1;

        public Button btnClose;

        public Transform tabProtoGo;
        public Transform latestTabGo;
        public Transform recommendTabGo;
        public Transform tabProtoParent;

        private TabItem latestTab;
        private TabItem recommendTab;
        private readonly UIElementCollector<TabItem> tabs = new UIElementCollector<TabItem>();
        private int currentTab = -1;
        private List<uint> normalTabIds = new List<uint>(2);

        public Text title;
        public TipScrollView tipScrollView;
        public CP_ScrollWhenNotVisible scrollWhen;
        public ScrollRect scrollRect;
        public CP_ToggleRegistry registry;

        public Transform tipProtoGo;
        public GridLayoutGroup tipGroupLayout;

        private readonly List<UI_ServerItem> serverItems = new List<UI_ServerItem>();

        private readonly COWVd<Tipitem> tipVds = new COWVd<Tipitem>();

        protected override void OnLoaded() {
            this.btnClose = this.transform.Find("Image_BG/View_Title06/Btn_Close").GetComponent<Button>();
            this.btnClose.onClick.AddListener(() => { UIManager.CloseUI(EUIID.UI_Server, false, true); });
            this.tabProtoGo = this.transform.Find("Image_BG/Widget_List_Left03/Scroll View/TabList/TabProto");
            this.latestTabGo = this.transform.Find("Image_BG/Widget_List_Left03/Scroll View/TabList/Latest");
            this.recommendTabGo = this.transform.Find("Image_BG/Widget_List_Left03/Scroll View/TabList/Recommend");
            this.tabProtoParent = this.transform.Find("Image_BG/Widget_List_Left03/Scroll View/TabList");

            this.title = this.transform.Find("View_Server_List/Image/Text").GetComponent<Text>();
            this.tipScrollView = this.transform.Find("View_Server_List/Scroll View/ServerList").GetComponent<TipScrollView>();
            this.scrollRect = this.transform.Find("View_Server_List/Scroll View").GetComponent<ScrollRect>();
            this.registry = this.transform.Find("View_Server_List/Scroll View/ServerList").GetComponent<CP_ToggleRegistry>();
            this.scrollWhen = this.transform.Find("View_Server_List/Scroll View/ServerList").GetComponent<CP_ScrollWhenNotVisible>();

            this.tipProtoGo = this.transform.Find("View_Server_List/Scroll View/ServerList/TipGroupParent/TipNode/CharProto");
            this.tipGroupLayout = this.transform.Find("View_Server_List/Scroll View/ServerList/TipGroupParent/TipNode").GetComponent<GridLayoutGroup>();
        }

        protected override void OnDestroy() {
            this.latestTab?.OnDestroy();
            this.recommendTab?.OnDestroy();
            this.tabs.ForEach((ele) => { ele.OnDestroy(); });
            this.tipVds.ForEach((ele) => { ele.OnDestroy(); });
        }

        public enum EMode {
            Normal = 0,
            Force,
        }

        protected override void OnOpened() {
            this.currentTab = -1;
            this.IsTipShowing = false;
            this.showTipItemIndex = -1;

            if (this.latestTab == null) {
                this.latestTab = new TabItem();
                this.latestTab.Init(this.latestTabGo);
                this.latestTab.SetSelectedAction((innerId, force) => {
                    this.currentTab = -1;

                    // 收起tip
                    this.OnItemClicked(EServerItemType.Latest, null, -1);

                    this.RefreshLatest();
                });
            }

            if (this.recommendTab == null) {
                this.recommendTab = new TabItem();
                this.recommendTab.Init(this.recommendTabGo);
                this.recommendTab.SetSelectedAction((innerId, force) => {
                    this.currentTab = -1;

                    // 收起tip
                    this.OnItemClicked(EServerItemType.Recommend, null, -1);

                    this.RefreshRecommend();
                });
            }
        }

        public void RefreshLatest() {
            int count = Mathf.Min(MY_SERVER_COUNT, Sys_Login.Instance.mMyServerEntries.Count);
            this.title.text = LanguageHelper.GetTextContent(1000040);
            this.tipScrollView.TryBuildOrRefresh(count, (rt, index) => {
                var server = new UI_ServerItem();
                server.Parse(this, rt.gameObject);
                this.serverItems.Add(server);
            }, (rt, index) => { this.serverItems[index].SetData(index, Sys_Login.Instance.mMyServerEntries[index], EServerItemType.Latest); });
        }

        public void RefreshRecommend() {
            int count = Sys_Login.Instance.mRecommendServerEntries.Count;
            this.title.text = LanguageHelper.GetTextContent(1000041);
            this.tipScrollView.TryBuildOrRefresh(count, (rt, index) => {
                var server = new UI_ServerItem();
                server.Parse(this, rt.gameObject);
                this.serverItems.Add(server);
            }, (rt, index) => { this.serverItems[index].SetData(index, Sys_Login.Instance.mRecommendServerEntries[index], EServerItemType.Recommend); });
        }

        public void RefreshNormal(uint tabId) {
            var group = Sys_Login.Instance.tabs[tabId];
            var list = Sys_Login.Instance.FilterSvrIds(group.SvrIds);
            this.title.text = group.GroupName.ToStringUtf8();
            this.tipScrollView.TryBuildOrRefresh(list.Count, (rt, index) => {
                var server = new UI_ServerItem();
                server.Parse(this, rt.gameObject);
                this.serverItems.Add(server);
            }, (rt, index) => {
                var serverId = list[index];
                var entry = Sys_Login.Instance.allServers[serverId];
                this.serverItems[index].SetData(index, entry, EServerItemType.Normal);
            });
        }

        // 展开/收缩Tip
        public void OnItemClicked(EServerItemType itemType, ServerEntry entry, int index) {
            if (entry != null && Sys_Login.Instance.HasMainTitle(entry, true)) {
                this.IsTipShowing = false;
                this.tipScrollView.Sort(0, false);
                return;
            }
            
            if (this.showTipItemType != itemType) {
                this.registry.SetHighLight(-1);
                this.scrollRect.verticalNormalizedPosition = 1f;

                this.IsTipShowing = false;
                this.showTipItemIndex = -1;
            }
            else {
                if (index == -1) {
                    this.IsTipShowing = false;
                }
                else {
                    if (this.showTipItemIndex != index) {
                        this.IsTipShowing = true;
                    }
                    else {
                        this.IsTipShowing = !this.IsTipShowing;
                    }
                }
                
                this.showTipItemIndex = index;
            }

            this.showTipItemType = itemType;

            int itemCount = 0;
            if (itemType == EServerItemType.Latest) {
                itemCount = Mathf.Min(MY_SERVER_COUNT, Sys_Login.Instance.mMyServerEntries.Count);
            }
            else if (itemType == EServerItemType.Recommend) {
                itemCount = Sys_Login.Instance.mRecommendServerEntries.Count;
            }
            else if (itemType == EServerItemType.Normal) {
                if (Sys_Login.Instance.tabs.TryGetValue((uint) this.currentTab, out var sg)) {
                    var ls = Sys_Login.Instance.FilterSvrIds(sg.SvrIds);
                    itemCount = ls.Count;
                }
                else {
                    itemCount = 0;
                }
            }

            // 刷新tip
            int tipCount = 0;
            if (this.IsTipShowing) {
                tipCount = entry.mRoleInfo.Count;
                tipCount = Mathf.Min(MAX_CHAR_COUNT - 1, tipCount) + 1;
                this.tipVds.TryBuildOrRefresh(this.tipProtoGo.gameObject, this.tipGroupLayout.transform, tipCount, (vd, vdIndex) => {
                    if (vdIndex < entry.mRoleInfo.Count) {
                        vd.RefreshData(entry, entry.mRoleInfo[vdIndex], this);
                    }
                    else {
                        vd.RefreshData(entry, null, this);
                    }
                });

                float GetExpandHeight(int count) {
                    count = Mathf.CeilToInt(1f * count / this.tipGroupLayout.constraintCount);
                    return count * (this.tipGroupLayout.cellSize.y + this.tipGroupLayout.spacing.y) - this.tipGroupLayout.spacing.y;
                }

                float height = GetExpandHeight(tipCount);
                this.tipScrollView.Sort(itemCount, true, index, height);
                bool isLastLine = (itemCount - 2 <= index && index < itemCount);
                this.scrollWhen.TryBecomeVisible(isLastLine, height);
            }
            else {
                this.tipScrollView.Sort(itemCount, false);
            }
        }

        protected override void OnShow() {
            this.RefreshCurrentTab();
        }

        public void RefreshCurrentTab(EMode mode = EMode.Normal, int targetTab = -1) {
            ListHelper.CopyKeyTo(Sys_Login.Instance.tabs, ref this.normalTabIds);
            this.tabs.BuildOrRefresh<uint>(this.tabProtoGo.gameObject, this.tabProtoParent, this.normalTabIds, (item, data, index) => {
                item.SetUniqueId((int) data);
                item.Refresh(data);
                item.SetSelectedAction((innerId, force) => {
                    this.currentTab = innerId;

                    // 收起tip
                    this.OnItemClicked(EServerItemType.Normal, null, -1);

                    this.RefreshNormal((uint) innerId);
                });
            });

            if (mode == EMode.Force) {
                this.currentTab = targetTab;
            }

            // 如果刷新后，tab个数变少了，矫正一下
            if (this.currentTab != -1) {
                if (!this.normalTabIds.Contains((uint) this.currentTab)) {
                    this.currentTab = -1;
                }
            }

            if(mode == EMode.Normal) {
                if (this.currentTab == -1) {
                    if (Sys_Login.Instance.IsNewCharacter) {
                        this.recommendTab.SetSelected(true, true);
                    }
                    else {
                        this.latestTab.SetSelected(true, true);
                    }
                }
                else {
                    if (this.tabs.TryGetVdById(currentTab, out var vd)) {
                        vd.SetSelected(true, true);
                    }
                    else if (this.tabs.RealCount > 0) {
                        tabs[0].SetSelected(true, true);
                    }
                }
            }
            else {
                if (this.currentTab == -1) {
                    this.recommendTab.SetSelected(true, true);
                }
                else {
                    if (this.tabs.TryGetVdById(currentTab, out var vd)) {
                        vd.SetSelected(true, true);
                    }
                    else if (this.tabs.RealCount > 0) {
                        tabs[0].SetSelected(true, true);
                    }
                }
            }
        }

        protected override void ProcessEventsForEnable(bool toRegister) {
            Sys_Login.Instance.eventEmitter.Handle(Sys_Login.EEvents.OnServerListChanged, this.OnServerListChange, toRegister);
        }

        private void OnServerListChange() {
            // 收缩
            //this.latestPage?.tipScrollView.Sort(1, false);
            //this.myRolePage?.tipScrollView.Sort(4, false);
            //this.normalOrRecommendPage.tipScrollView.Sort(Sys_Login.COUNT_PER_PAGE, false);

            this.RefreshCurrentTab();
        }
    }
}