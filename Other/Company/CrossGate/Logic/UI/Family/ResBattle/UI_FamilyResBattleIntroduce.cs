using System;
using System.Collections.Generic;
using Logic;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

// 家族资源战简介 规则说明
public class UI_FamilyResBattleIntroduce : UIBase, UI_FamilyResBattleIntroduce.Layout.IListener {
    public class Tab : UISelectableElement {
        public CP_Toggle toggle;

        public Text ruleText;
        public Text ruleText1;

        protected override void Loaded() {
            this.toggle = this.transform.GetComponent<CP_Toggle>();

            this.ruleText = this.transform.Find("Object/Text").GetComponent<Text>();
            this.ruleText1 = this.transform.Find("Object_Selected/Text").GetComponent<Text>();

            this.toggle.onValueChanged.AddListener(this.Switch);
        }

        public void Refresh(uint id) {
            var csv = CSVBattleRules.Instance.GetConfData(id);
            if (csv != null) {
                TextHelper.SetText(this.ruleText, csv.lanId);
                TextHelper.SetText(this.ruleText1, csv.lanId);
            }
        }

        public void Switch(bool arg) {
            if (arg) {
                this.onSelected?.Invoke(this.id, true);
            }
        }
        public override void SetSelected(bool toSelected, bool force) {
            this.toggle.SetSelected(toSelected, true);
        }
    }

    public class Rule : UIComponent {
        public Text title;
        public Text desc;
        public RawImageLoader image;

        public ForceRebuildLayout builder;

        protected override void Loaded() {
            this.title = this.transform.Find("Title").GetComponent<Text>();
            this.desc = this.transform.Find("Content/Text").GetComponent<Text>();
            this.image = this.transform.Find("Content/Image").GetComponent<RawImageLoader>();
            this.builder = this.transform.GetComponent<ForceRebuildLayout>();
        }

        public void Refresh(uint id) {
            var csv = CSVBattleRuleDesc.Instance.GetConfData(id);
            if (csv != null) {
                TextHelper.SetText(this.title, csv.title);

                if (!csv.type) {
                    TextHelper.SetText(this.desc, csv.lanId);
                }
                else {
                    this.image.Set(csv.assetPath, true);
                }

                this.builder.Set();
            }
        }
    }

    public class Layout : LayoutBase {
        public GameObject tabProto;
        public Transform tabProtoParent;

        public GameObject ruleProto;
        public Transform ruleProtoParent;

        public void Parse(GameObject root) {
            this.Set(root);

            this.tabProto = this.transform.Find("Animator/ScrollView_Menu/List/RuleProto").gameObject;
            this.tabProtoParent = this.tabProto.transform.parent;

            this.ruleProto = this.transform.Find("Animator/List_Menu/TabList/Proto").gameObject;
            this.ruleProtoParent = this.ruleProto.transform.parent;
        }

        public void RegisterEvents(IListener listener) {
        }

        public interface IListener {
        }
    }

    public Layout layout = new Layout();

    public uint currentTabId = 1;
    public List<uint> tabIds = new List<uint>();
    public UIElementCollector<Tab> tabVds = new UIElementCollector<Tab>();

    public List<uint> ruleIds = new List<uint>();
    public COWVd<Rule> ruleVds = new COWVd<Rule>();

    protected override void OnLoaded() {
        this.layout.Parse(this.gameObject);
        this.layout.RegisterEvents(this);
    }
    protected override void OnDestroy() {
        this.tabVds.Clear();
        this.ruleVds.Clear();
    }

    protected override void OnOpen(object arg) {
        var tp = arg as Tuple<List<uint>, uint>;
        this.tabIds = tp.Item1;
        this.currentTabId = tp.Item2;
    }
    protected override void OnOpened() {
    }

    protected override void OnShow() {
        this.tabVds.BuildOrRefresh<uint>(this.layout.tabProto, this.layout.tabProtoParent, this.tabIds, this._OnRefreshTab);

        // 默认选中Tab
        if (this.tabVds.CorrectId(ref this.currentTabId, this.tabIds)) {
            if (this.tabVds.TryGetVdById((int)this.currentTabId, out var vd)) {
                vd.SetSelected(true, true);
            }
        }
        else {
            Debug.LogError("Can't run here!");
        }
    }

    private void _OnRefreshTab(Tab vd, uint id, int indexOfVdList) {
        vd.SetUniqueId((int)id);
        vd.SetSelectedAction((innerId, force) => {
            this.currentTabId = (uint)innerId;

            this.ruleIds = CSVBattleRules.Instance.GetConfData(this.currentTabId).rules;
            this.ruleVds.TryBuildOrRefresh(this.layout.ruleProto, this.layout.ruleProtoParent, this.ruleIds.Count, this._OnRuleRefresh);
        });
        vd.Refresh(this.tabIds[indexOfVdList]);
    }

    private void _OnRuleRefresh(Rule rule, int index) {
        rule.Refresh(this.ruleIds[index]);
    }
}