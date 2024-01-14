using System.Collections.Generic;
using Lib.Core;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Logic {
    public class UIHelpRule {
        public uint uiid;
        public Transform parent;
        public MonoLife mono;

        private AsyncOperationHandle<GameObject> assetRequest;
        private readonly CSVUIRule.Data csvRule;

        public UIHelpRule(int uiid, Transform parent) {
            this.uiid = (uint)uiid;
            this.parent = parent;
            this.csvRule = CSVUIRule.Instance.GetConfData(this.uiid);
        }

        public void TryLoad() {
            bool need = this.csvRule != null && this.csvRule.active;
            if (!need) {
                return;
            }

            if (this.assetRequest.IsValid()) {
                AddressablesUtil.Release<GameObject>(ref this.assetRequest, this.OnCompleted);
            }
            AddressablesUtil.LoadAssetAsync<GameObject>(ref this.assetRequest, "UI/Cell/UI_HelpButton.prefab", this.OnCompleted);
        }

        private void OnCompleted(AsyncOperationHandle<GameObject> handle) {
            GameObject go = GameObject.Instantiate<GameObject>(handle.Result);
            go.transform.SetParent(this.parent);

            go.transform.localScale = Vector3.one;
            if (this.csvRule.pos != null && this.csvRule.pos.Count >= 2) {
                go.transform.localPosition = new Vector3(this.csvRule.pos[0], this.csvRule.pos[1]);
            }
            else {
                go.transform.localPosition = Vector3.zero;
            }

            if (!go.TryGetComponent<MonoLife>(out this.mono)) {
                this.mono = go.AddComponent<MonoLife>();
                this.mono.onDestroy = this.OnDestroy;
            }
            Button btn = go.GetComponent<Button>();
            btn.onClick.AddListener(this.OnBtnClicked);
        }

        private void OnBtnClicked() {
            UIManager.OpenUI(EUIID.UI_HelpRule, false, this.csvRule.ruleIds);
        }

        private void OnDestroy() {
            if (this.assetRequest.IsValid()) {
                AddressablesUtil.Release<GameObject>(ref this.assetRequest, this.OnCompleted);
            }
        }
    }
    public class UI_HelpRule : UIBase {
        public class Tab : UIComponent {
            public Text title;
            public GameObject proto;

            public COWComponent<Text> texts = new COWComponent<Text>();

            public uint id;
            private CSVRule.Data csvRule;

            protected override void Loaded() {
                this.title = this.transform.Find("Text_Title").GetComponent<Text>();
                this.proto = this.transform.Find("Grid/Text").gameObject;
            }

            public override void OnDestroy() {
                this.texts.Clear();
            }

            public void Refresh(uint id) {
                this.id = id;

                this.csvRule = CSVRule.Instance.GetConfData(id);
                if (this.csvRule != null) {
                    TextHelper.SetText(this.title, this.csvRule.title);

                    int count = this.csvRule.contents != null ? this.csvRule.contents.Count : 0;
                    this.texts.TryBuildOrRefresh(this.proto, this.proto.transform.parent, count, this.OnRefresh);

                    LayoutRebuilder.ForceRebuildLayoutImmediate(this.rectTransform);
                }
            }

            private void OnRefresh(Text text, int index) {
                if (this.csvRule != null) {
                    TextHelper.SetText(text, this.csvRule.contents[index]);
                }
            }
        }

        private List<uint> contents = new List<uint>();
        private GameObject proto;
        public COWVd<Tab> titles = new COWVd<Tab>();

        protected override void OnLoaded() {
            Button btn = this.transform.Find("Animator/View_TipsBgNew02/Btn_Close").GetComponent<Button>();
            btn.onClick.AddListener(this.OnBtnClicked);

            this.proto = this.transform.Find("Animator/List_Menu/TabList/ListItem").gameObject;
        }

        private void OnBtnClicked() {
            this.CloseSelf();
        }

        protected override void OnOpen(object arg = null) {
            this.contents = arg == null ? new List<uint>() : arg as List<uint>;
        }

        protected override void OnOpened() {
            this.titles.TryBuildOrRefresh(this.proto, this.proto.transform.parent, this.contents.Count, this.OnRefresh);
        }

        protected override void OnShow() {
            FrameworkTool.ForceRebuildLayout(this.gameObject);
        }
        private void OnRefresh(Tab tab, int index) {
            tab.Refresh(this.contents[index]);
        }

        protected override void OnDestroy() {
            for (int i = 0, length = this.titles.Count; i < length; ++i) {
                this.titles[i].OnDestroy();
            }
            this.titles.Clear();
        }
    }
}
