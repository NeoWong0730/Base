using System;
using System.Collections.Generic;
using Logic.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public class UI_ClueTaskMain : UIBase, UI_ClueTaskMain_Layout.IListener {
        private UI_CurrencyTitle UI_CurrencyTitle;

        public class UI_CluetaskSubComponent : UIComponent {
            public virtual void Refresh(EClueTaskType type) { }
        }
        public class UI_TaskTabItem : UISelectableElement {
            public Tab tabInfo;

            public CP_Toggle toggle;
            public Image icon1;
            public Image icon2;

            public Text text1;
            public Text text2;

            protected override void Loaded() {
                this.toggle = this.gameObject.GetComponent<CP_Toggle>();
                this.text1 = this.transform.Find("Btn_Menu_Dark/Text_Menu_Dark").GetComponent<Text>();
                this.text2 = this.transform.Find("Image_Menu_Light/Text_Menu_Light").GetComponent<Text>();
                this.icon1 = this.transform.Find("Btn_Menu_Dark/Image_Icon").GetComponent<Image>();
                this.icon2 = this.transform.Find("Image_Menu_Light/Image_Icon").GetComponent<Image>();

                this.toggle.onValueChanged.AddListener(this.Switch);
            }
            public void Refresh(int tabType) {
                this.tabInfo = UI_ClueTaskMain.tabs[(uint)tabType];

                uint iconId = 0;
                if (tabType == (int)EClueTaskType.Detective) { iconId = 992401; }
                else if (tabType == (int)EClueTaskType.Adventure) { iconId = 992402; }
                else if (tabType == (int)EClueTaskType.Experience) { iconId = 992403; }
                ImageHelper.SetIcon(this.icon1, iconId);

                TextHelper.SetText(this.text1, this.tabInfo.name);
                TextHelper.SetText(this.text2, this.tabInfo.name);
            }
            public void Switch(bool arg) {
                if (arg) {
                    this.onSelected?.Invoke((int)this.tabInfo.type, true);
                }
            }
            public override void SetSelected(bool toSelected, bool force) {
                this.toggle.SetSelected(toSelected, true);
            }
        }

        public class Tab {
            public uint type;
            public string name;
            public string contentPath;
            public Type vdType;
            public uint funcOpen;

            public bool hasLoaded = false;

            public Tab() { }
            public Tab(uint type, string name, string contentPath, Type vdType, uint funcOpen) {
                this.type = type;
                this.name = name;
                this.contentPath = contentPath;
                this.vdType = vdType;
                this.funcOpen = funcOpen;
            }
        }
        public static Dictionary<uint, Tab> tabs = new Dictionary<uint, Tab>()
        {
            {(int)EClueTaskType.Detective, new Tab((int)EClueTaskType.Detective, "侦探", "Animator/Contents/ContentProto/View_ClueList", typeof(UI_ClueTaskMain_ClueList), EFuncOpen.FO_DetectiveClue) },
            //{(int)EClueTaskType.Adventure, new Tab((int)EClueTaskType.Adventure, "传闻", "Animator/Contents/ContentProto/View_ClueList", typeof(UI_ClueTaskMain_ClueList), EFuncOpen.FO_AdventureClue) },
            //{(int)EClueTaskType.Experience, new Tab((int)EClueTaskType.Experience, "历程", "Animator/Contents/ContentProto/View_ClueExp", typeof(UI_ClueTaskMain_Experience), EFuncOpen.FO_ExperienceClue) },
        };
        public static Dictionary<int, UI_CluetaskSubComponent> contentVds = new Dictionary<int, UI_CluetaskSubComponent>();

        private readonly UI_ClueTaskMain_Layout Layout = new UI_ClueTaskMain_Layout();

        public UIElementCollector<UI_TaskTabItem> tabVds = new UIElementCollector<UI_TaskTabItem>();

        public int tabType = -1;

        protected override void OnLoaded() {
            this.Layout.Parse(this.gameObject);
            this.Layout.RegisterEvents(this);

            this.UI_CurrencyTitle = new UI_CurrencyTitle(this.transform.Find("Animator/UI_Property").gameObject);
        }
        protected override void OnUpdate() {
            foreach (var kvp in contentVds) {
                kvp.Value.ExecUpdate();
            }
        }
        protected override void OnOpen(object arg) {
            this.tabType = -1;
            Sys_ClueTask.Instance.PreCheck();

            foreach (var kvp in contentVds) {
                kvp.Value.Reset();
            }
        }
        protected override void OnShow() {
            // 构建tabs
            List<Tab> ls = new List<Tab>(tabs.Values);
            this.tabVds.BuildOrRefresh<Tab>(this.Layout.tabProto, this.Layout.tabParent, ls, (vd, data, indexOfVdList) => {
                int selectedTabType = (int)data.type;
                uint fo = data.funcOpen;
                vd.SetUniqueId(selectedTabType);
                vd.SetSelectedAction((innerId, force) => {
                    /*if (this.tabType != selectedTabType)*/ {
                        // 设置tabtype
                        this.tabType = selectedTabType;
                        this.Refresh();
                    }
                });

                if (Sys_FunctionOpen.Instance.IsOpen(fo, false)) {
                    vd.Show();
                    vd.Refresh(selectedTabType);
                }
                else {
                    vd.Hide();
                }
            });

            this.GetCurrentTabType(ls);
            this.SetSelectedTab(ls);
            this.Refresh();
            this.UI_CurrencyTitle.InitUi();
        }
        protected override void OnDestroy() {
            this.UI_CurrencyTitle?.Dispose();

            foreach (var kvp in tabs) {
                kvp.Value.hasLoaded = false;
            }

            for (int i = 0; i < this.tabVds.Count; ++i) {
                if (this.tabVds.TryGetVdByIndex(i, out var vd)) {
                    vd.OnDestroy();
                }
            }
            this.tabVds.Clear();

            foreach (var kvp in contentVds) {
                kvp.Value.OnDestroy();
            }
            contentVds.Clear();
        }

        private int GetCurrentTabType(List<Tab> tablist) {
            if (this.tabType != -1) {
                if (!this.tabVds.TryGetVdById(this.tabType, out var vd)) {
                    this.tabType = -1;
                }
            }
            if (this.tabType == -1) {
                if (tablist.Count > 0) {
                    this.tabType = (int)tablist[0].type;
                }
            }
            return this.tabType;
        }
        private void SetSelectedTab(List<Tab> tablist) {
            if (this.tabVds.TryGetVdById(this.tabType, out var vd)) {
                vd?.SetSelected(true, true);
            }
        }
        // 刷新中间内容
        public void Refresh() {
            // 传递mapid/tabType
            if (this.tabType != -1) {
                Tab info = tabs[(uint)this.tabType];
                if (!info.hasLoaded) {
                    GameObject proto = this.gameObject.transform.Find(info.contentPath).gameObject;
                    proto.SetActive(false);
                    proto = GameObject.Instantiate<GameObject>(proto, this.Layout.contengParent);
                    proto.SetActive(true);

                    UI_CluetaskSubComponent component = Activator.CreateInstance(info.vdType) as UI_CluetaskSubComponent;
                    component.Init(proto.transform);
                    contentVds.Add(this.tabType, component);

                    info.hasLoaded = true;
                }

                foreach (var kvp in contentVds) {
                    kvp.Value.Hide();
                }

                UI_CluetaskSubComponent vd = contentVds[this.tabType];
                vd.Show();
                vd.Refresh((EClueTaskType)this.tabType);
            }
        }

        #region UI事件
        public void OnBtnReturnClicked() {
            this.CloseSelf();
        }
        #endregion

        #region 逻辑事件
        protected override void ProcessEvents(bool toRegister) {
            if (gameObject != null && gameObject.activeInHierarchy) {
                Sys_ClueTask.Instance.eventEmitter.Handle<uint, uint>(Sys_ClueTask.EEvents.OnDetectiveExpChanged, OnDetectiveExpChanged, toRegister);
                Sys_ClueTask.Instance.eventEmitter.Handle<uint, uint>(Sys_ClueTask.EEvents.OnAdventureExpChanged, OnAdventureExpChanged, toRegister);
            }
        }
        private void OnAdventureExpChanged(uint oldValue, uint newValue) {
            Refresh();
        }
        private void OnDetectiveExpChanged(uint oldValue, uint newValue) {
            Refresh();
        }
        #endregion
    }
}