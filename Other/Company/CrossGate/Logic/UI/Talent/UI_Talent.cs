using System;
using System.Collections.Generic;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public class UITalentTab : UIComponent {
        public GameObject highlight;
        public Text normalDesc;
        public Text highlightDesc;
        public Button btn;
        public Animator animator;

        public static readonly int RightId = Animator.StringToHash("Right");
        public static readonly int LeftId = Animator.StringToHash("Left");
        public static readonly int OpenId = Animator.StringToHash("Open");
        public static readonly int CloseId = Animator.StringToHash("Close");

        private UI_Talent uiTalent;
        private bool isHighlight = false;

        public TalentBranch branch;
        public uint branchId;
        public int index;

        protected override void Loaded() {
            this.highlight = this.transform.Find("Highlight").gameObject;
            this.animator = this.transform.GetComponent<Animator>();

            this.normalDesc = this.transform.Find("Normal/Text").GetComponent<Text>();
            this.highlightDesc = this.transform.Find("Highlight/Text_Light").GetComponent<Text>();

            this.btn = this.transform.GetComponent<Button>();
            this.btn.onClick.AddListener(this.OnBtnClicked);
        }

        public void Refresh(UI_Talent uiTalent, uint branchId, int index, int schemeIndex) {
            this.uiTalent = uiTalent;
            this.branchId = branchId;
            this.index = index;

            this.branch = Sys_Talent.Instance.schemes[schemeIndex].branchs[branchId];
            uint totalUsed = this.branch.usedPoint;
            TextHelper.SetText(this.normalDesc, this.branch.name, totalUsed.ToString());
            TextHelper.SetText(this.highlightDesc, this.branch.name, totalUsed.ToString());
        }

        private void OnBtnClicked() {
            this.uiTalent.SetTabSelected(this, this.branchId, this.index);
        }

        public void SetSelected(bool toSelected) {
            if (!this.isHighlight && toSelected) {
                this.animator.Play(RightId, -1, 0);
            }
            else if (this.isHighlight && !toSelected) {
                this.animator.Play(LeftId, -1, 0);
            }

            this.btn.enabled = !toSelected;

            this.isHighlight = toSelected;
            this.highlight.SetActive(toSelected);
        }
    }

    public class UITalentNode : UIComponent {
        public Image bg;
        public Image icon;
        public Text level;
        public new Text name;
        public GameObject selected;
        public Button btn;

        public int schemeIndex;
        public uint talentId;
        public TalentEntry entry;
        public UI_Talent uiTalent;

        public enum EStatus {
            AllReached,
            AnyReached,
            NoReached,
        }

        public EStatus status = EStatus.NoReached;

        protected override void Loaded() {
            this.bg = this.transform.Find("Image").GetComponent<Image>();
            this.icon = this.transform.Find("Image_Icon").GetComponent<Image>();
            this.level = this.transform.Find("Text_Level").GetComponent<Text>();
            //name = transform.Find("Text_Name").GetComponent<Text>();
            this.selected = this.transform.Find("Image_Selected").gameObject;
            this.selected.SetActive(false);

            this.btn = this.transform.Find("Image").GetComponent<Button>();
            this.btn.onClick.AddListener(this.OnBtnClicked);
        }

        public void Refresh(UI_Talent uiTalent, uint id, int schemeIndex) {
            this.uiTalent = uiTalent;
            this.talentId = id;
            this.entry = Sys_Talent.Instance.schemes[schemeIndex].GetTalent(id);
            if (this.entry != null) {
                // 主动节能
                ImageHelper.SetIcon(this.icon, this.entry.csv.icon_id);
                this.level.text = string.Format("{0}/{1}", this.entry.level.ToString(), this.entry.csv.lev.ToString());

                // 天赋加满
                if (this.entry.level >= this.entry.csv.lev) {
                    this.status = EStatus.AllReached;

                    this.bg.color = new Color(182 / 255f, 241 / 255f, 158 / 255f, 255 / 255f);
                    this.icon.color = new Color(182 / 255f, 241 / 255f, 158 / 255f, 255 / 255f);
                }
                // 满足加点条件，但是未加满
                else if (this.entry.level >= 0 && this.entry.IsBranchPointReached && this.entry.IsPreReached) {
                    this.status = EStatus.AnyReached;

                    this.bg.color = new Color(170 / 255f, 204 / 255f, 1f, 255 / 255f);
                    this.icon.color = new Color(170 / 255f, 204 / 255f, 1f, 255 / 255f);
                }
                else {
                    this.status = EStatus.NoReached;

                    this.bg.color = Color.white;
                    this.icon.color = Color.white;

                    this.level.text = null;
                }

#if DEBUG_MODE
                this.level.text = this.level.text + " Id:" + this.entry.id;
#endif
            }
        }

        public void SetSelected(bool toSelected) {
            this.SetButtonEnable(!toSelected);
            this.selected.SetActive(toSelected);
        }

        public void SetButtonEnable(bool toEnable) {
            this.btn.enabled = toEnable;
        }

        public void OnBtnClicked() {
            //if (status != EStatus.NoReached)
            {
                this.uiTalent.SetNodeSelected(this.talentId, this.btn.enabled);
            }
        }
    }

    public class UITalentNodeShow : UIComponent {
        public Image image;
        public Image bg;
        public new Text name;

        public GameObject current;
        public Text currentDesc;

        public GameObject next;
        public Text nextDesc;

        public Button btn;

        public GameObject skillDesc;
        public GameObject fx;

        public GameObject textProto;
        public GameObject viewport;
        public COWComponent<Text> texts = new COWComponent<Text>();

        public UI_Talent uiTalent;
        public uint talentId;
        public int schemeIndex;
        public TalentEntry entry;

        protected override void Loaded() {
            this.fx = this.transform.Find("SkillDesc/Image/Fx_ui_Skillup").gameObject;

            this.btn = this.transform.Find("Button_Close").GetComponent<Button>();
            this.btn.onClick.AddListener(this.OnBtnCloseClicked);

            this.btn = this.transform.Find("Button_Grade").GetComponent<Button>();
            this.btn.onClick.AddListener(this.OnBtnUpgradeClicked);

            this.skillDesc = this.transform.Find("SkillDesc").gameObject;

            this.name = this.transform.Find("SkillDesc/Text_Title").GetComponent<Text>();
            this.image = this.transform.Find("SkillDesc/Image_Icon").GetComponent<Image>();
            this.bg = this.transform.Find("SkillDesc/Image").GetComponent<Image>();

            this.viewport = this.transform.Find("SkillDesc/Scroll_View/ViewPoint").gameObject;
            this.textProto = this.transform.Find("SkillDesc/Scroll_View/ViewPoint/Text").gameObject;
            this.textProto.SetActive(false);

            this.current = this.transform.Find("Title_Tips1").gameObject;
            this.currentDesc = this.current.transform.Find("Text/Content").GetComponent<Text>();

            this.next = this.transform.Find("Title_Tips2").gameObject;
            this.nextDesc = this.next.transform.Find("Text/Content").GetComponent<Text>();
        }

        public void Refresh(UI_Talent uiTalent, uint talentId, int schemeIndex) {
            this.uiTalent = uiTalent;
            this.talentId = talentId;
            this.schemeIndex = schemeIndex;
            this.entry = Sys_Talent.Instance.schemes[schemeIndex].GetTalent(talentId);

            // Debug.LogError("talentId: " + talentId);

            ImageHelper.SetIcon(this.image, this.entry.csv.icon_id);
            TextHelper.SetText(this.name, this.entry.csv.name_lan);

            if (this.entry.level >= this.entry.csv.lev) {
                this.bg.color = new Color(182 / 255f, 241 / 255f, 158 / 255f, 255 / 255f);
                this.image.color = new Color(182 / 255f, 241 / 255f, 158 / 255f, 255 / 255f);
            }
            else if (this.entry.level >= 0 && this.entry.IsBranchPointReached && this.entry.IsPreReached) {
                this.bg.color = new Color(170 / 255f, 204 / 255f, 1f, 255 / 255f);
                this.image.color = new Color(170 / 255f, 204 / 255f, 1f, 255 / 255f);
            }
            else {
                this.bg.color = Color.white;
                this.image.color = Color.white;
            }

            this.viewport.SetActive(false);
            Color color = this.entry.IsPreReached && this.entry.IsBranchPointReached ? new Color(112f / 255, 59f / 255, 16f / 255, 1f) : Color.red;
            // 主动技能
            if (Sys_Skill.Instance.IsActiveSkill(this.entry.csv.skill_id[0])) {
                var currentSkill = this.entry.CurrentActiveSkill;
                if (currentSkill != null) {
                    TextHelper.SetText(this.currentDesc, currentSkill.desc);
                }
                else {
                    TextHelper.SetText(this.currentDesc, 2029500);
                }

                var nextSkill = this.entry.NextActiveSkill;
                if (nextSkill != null) {
                    this.next.SetActive(true);
                    TextHelper.SetText(this.nextDesc, nextSkill.desc);

                    this.viewport.SetActive(true);
                    this.textProto.SetActive(false);
                    this.texts.TryBuildOrRefresh(this.textProto, this.textProto.transform.parent, 1 + this.entry.PreLimits.Count, (goText, index) => {
                        goText.color = color;
                        if (index == 0) {
                            if (this.entry.csv.pre_unm == 0) {
                                goText.gameObject.SetActive(false);
                            }
                            else {
                                goText.gameObject.SetActive(true);
                                TextHelper.SetText(goText, 5522, this.entry.csv.pre_unm.ToString());
                            }
                        }
                        else {
                            TextHelper.SetText(goText, 5523, LanguageHelper.GetTextContent(this.entry.PreLimits[index - 1].Item1.csv.name_lan), this.entry.PreLimits[index - 1].Item2.ToString());
                        }
                    });
                }
                else {
                    this.next.SetActive(false);
                }
            }
            else {
                var currentSkill = this.entry.CurrentPassiveSkill;
                if (currentSkill != null) {
                    TextHelper.SetText(this.currentDesc, currentSkill.desc);
                }
                else {
                    TextHelper.SetText(this.currentDesc, 2029500);
                }

                var nextSkill = this.entry.NextPassiveSkill;
                if (nextSkill != null) {
                    this.next.SetActive(true);
                    TextHelper.SetText(this.nextDesc, nextSkill.desc);

                    this.viewport.SetActive(true);
                    this.textProto.SetActive(false);
                    this.texts.TryBuildOrRefresh(this.textProto, this.textProto.transform.parent, 1 + this.entry.PreLimits.Count, (goText, index) => {
                        goText.color = color;
                        if (index == 0) {
                            if (this.entry.csv.pre_unm == 0) {
                                goText.gameObject.SetActive(false);
                            }
                            else {
                                goText.gameObject.SetActive(true);
                                TextHelper.SetText(goText, 5522, this.entry.csv.pre_unm.ToString());
                            }
                        }
                        else {
                            TextHelper.SetText(goText, 5523, LanguageHelper.GetTextContent(this.entry.PreLimits[index - 1].Item1.csv.name_lan), this.entry.PreLimits[index - 1].Item2.ToString());
                        }
                    });
                }
                else {
                    this.next.SetActive(false);
                }
            }

            ButtonHelper.Enable(this.btn, this.entry.CanUpgrade);
        }

        private void OnBtnUpgradeClicked() {
            if (this.entry.CanUpgrade) {
                Sys_Talent.Instance.ReqUpdateTalentLevel((uint) schemeIndex, this.talentId);
            }
        }

        private void OnBtnCloseClicked() {
            this.uiTalent.CloseExpand();
        }

        public void SetFx(bool toEnable) {
            this.fx.SetActive(toEnable);
        }
    }

    public class TalentSchemes : UISchemes<TalentSchemes.TalentScheme> {
        public class TalentScheme : Scheme {
            public override void Refresh(int index, bool isLastIndex, bool isUsing) {
                this.index = index;

                btnAdd.gameObject.SetActive(isLastIndex);
                normalGo.gameObject.SetActive(!isLastIndex);
                if (!isLastIndex) {
                    TextHelper.SetText(tabNameDark, Sys_Talent.Instance.schemes[index].name);
                    tabNameLight.text = tabNameDark.text;

                    usingGo.gameObject.SetActive(isUsing);
                }
            }

            public override void OnBtnRenameClicked() {
                void OnRename(int schIndex, int __, string newName) {
                    Sys_Talent.Instance.ModifyNameReq((uint) schIndex, newName);
                }

                var arg = new UI_ChangeSchemeName.ChangeNameArgs() {
                    arg1 = index,
                    arg2 = 0,
                    oldName = Sys_Talent.Instance.schemes[index].name,
                    onYes = OnRename
                };
                UIManager.OpenUI(EUIID.UI_ChangeSchemeName, false, arg);
            }

            public override void OnBtnAddClicked() {
                bool valid = (Sys_Ini.Instance.Get<IniElement_IntArray>(1431, out var rlt) && rlt.value.Length >= 3);
                int limit = rlt.value[1];
                if (Sys_Talent.Instance.schemes.Count >= limit) {
                    // 上限满了
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10013701));
                    return;
                }

                void OnConform() {
                    if (Sys_Bag.Instance.GetItemCount(2) < rlt.value[2]) {
                        // 道具不足
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(5602));
                    }
                    else {
                        // 请求server新增方案，同时给新方案设置空的数据
                        Sys_Talent.Instance.CreateTalentSchemeReq();
                    }
                }

                bool isValid = CSVCheckseq.Instance.GetConfData(12104).IsValid();
                if (!isValid) {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(10013901));
                }
                else {
                    PromptBoxParameter.Instance.Clear();
                    PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(10013806, rlt.value[2].ToString());
                    PromptBoxParameter.Instance.SetConfirm(true, OnConform);
                    PromptBoxParameter.Instance.SetCancel(true, null);
                    UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
                }
            }
        }

        public override int usingIndex {
            get { return Sys_Talent.Instance.curSchemeIndex; }
        }
    }

    public class UI_Talent : UIComponent {
        public Text usedPoint;
        public Text remainPoint;

        public List<UITalentNode> talentNodes = new List<UITalentNode>();
        public Animator animator;
        public UITalentNodeShow talentNodeShow = new UITalentNodeShow();

        public Canvas nodeCanvas;
        public Canvas titleCanvas;

        public Transform redPoint;

        public LineRenderer lineRendererProto;
        public COWComponent<LineRenderer> linRenderes = new COWComponent<LineRenderer>();
        public GameObject arrowProto;
        public Button btnUsing;
        public Text btnUsingText;

        public MonoLife monoLife;
        public COWComponent<Transform> arrows = new COWComponent<Transform>();

        public UI_SkillNew ui;
        public TalentSchemes schemes;

        public List<UITalentTab> tabs = new List<UITalentTab>(1);
        public int selectedTabIndex = 0;
        public uint selectedBranchId;
        public uint selectedTalentId;

        private bool isExpand = false;

        protected override void Loaded() {
            this.redPoint = this.transform.Find("View_Title/Text_Remain/RedPoint");
            this.btnUsing = this.transform.Find("View_Middle/Button_use").GetComponent<Button>();
            btnUsing.onClick.AddListener(OnBtnUsingClicked);
            this.btnUsingText = this.transform.Find("View_Middle/Button_use/Text_01").GetComponent<Text>();

            this.nodeCanvas = this.transform.Find("View_Middle/View_Position").GetComponent<Canvas>();
            this.titleCanvas = this.transform.Find("View_Title").GetComponent<Canvas>();
            this.lineRendererProto = this.transform.Find("View_Middle/LineRenderNode/LineRender").GetComponent<LineRenderer>();

            this.arrowProto = this.transform.Find("View_Middle/Arrows/Arrow").gameObject;

            this.talentNodeShow.Init(this.transform.Find("View_Right"));
            this.animator = this.gameObject.GetComponent<Animator>();

            this.usedPoint = this.transform.Find("View_Title/Text_Used/Text_Point").GetComponent<Text>();
            this.remainPoint = this.transform.Find("View_Title/Text_Remain/Text_Point").GetComponent<Text>();
            Button btn = this.transform.Find("View_Title/Text_Remain/Button_Add").GetComponent<Button>();
            btn.onClick.AddListener(this.OnBtnAddClicked);
            btn = this.transform.Find("View_Title/Button_Reset").GetComponent<Button>();
            btn.onClick.AddListener(this.OnBtnResetClicked);

            this.talentNodes.Clear();
            Transform t = this.transform.Find("View_Middle/View_Position");
            int childCount = t.childCount;
            for (int i = 0, length = childCount; i < length; ++i) {
                UITalentNode node = new UITalentNode();
                node.Init(t.GetChild(i));
                this.talentNodes.Add(node);
            }

            this.tabs.Clear();
            t = this.transform.Find("View_Middle/Image_Bottom/TabList/TabLeft");
            UITalentTab tab = new UITalentTab();
            tab.Init(t);
            tab.Show();
            this.tabs.Add(tab);
            // t = this.transform.Find("View_Middle/Image_Bottom/TabList/TabRight");
            // tab = new UITalentTab();
            // tab.Init(t);
            // tab.Show();
            // this.tabs.Add(tab);

            this.monoLife = this.gameObject.AddComponent<MonoLife>();

            schemes = new TalentSchemes();
            schemes.Init(this.transform.Find("View_Left/Label_Scroll01"));
            schemes.selectedIndex = schemes.usingIndex;

            this.ProcessEventsForAwake(true);
        }

        public void RefreshLayer(UI_SkillNew ui) {
            this.lineRendererProto.sortingOrder = ui.nSortingOrder + 2;
            this.nodeCanvas.sortingOrder = ui.nSortingOrder + 2;
            this.titleCanvas.sortingOrder = ui.nSortingOrder + 3;
        }

        private void OnSelectedScheme(int index) {
            // 根据index获取talent数据。然后刷新

            this.RefreshLayer(ui);
            this.RefreshTitle();
            this.RefreshTabs();

            bool usingIndex = schemes.usingIndex == index;
            if (usingIndex) {
                ButtonHelper.Enable(btnUsing, false);
                TextHelper.SetText(btnUsingText, 8311);
            }
            else {
                ButtonHelper.Enable(btnUsing, true);
                TextHelper.SetText(btnUsingText, 5172);
            }

            this.GetMaxBranchId(out this.selectedTabIndex);
            this.SetTabSelected(this.tabs[this.selectedTabIndex], this.tabs[this.selectedTabIndex].branchId, this.selectedTabIndex, true);

            // 默认选中
            if (Sys_Talent.Instance.SelectedTalentId != 0) {
                this.SetNodeSelected(Sys_Talent.Instance.SelectedTalentId, true);
            }

            if (this.monoLife != null) {
                this.monoLife.onUpdate = this.Update;
            }
        }

        public void Refresh(UI_SkillNew ui) {
            //UIManager.HitPointShow(EUIID.UI_SkillUpgrade, UI_SkillNew.ETab.Talent.ToString());

            this.ui = ui;

            schemes.Refresh(Sys_Talent.Instance.schemes.Count, schemes.selectedIndex, OnSelectedScheme);
        }

        public uint GetMaxBranchId(out int index) {
            uint id = 0;
            long max = long.MinValue;
            index = 0;
            int i = 0;
            foreach (var kvp in Sys_Talent.Instance.schemes[schemes.selectedIndex].branchs) {
                if (max < kvp.Value.usedPoint) {
                    max = kvp.Value.usedPoint;
                    id = kvp.Key;
                    index = i;
                }

                ++i;
            }

            return id;
        }

        private new void Update() {
            this.RefreshLines(false);
        }

        public void SetTabSelected(UITalentTab talentTab, uint branchId, int index, bool force = true) {
            this.selectedTabIndex = index;
            this.selectedBranchId = branchId;

            this.RefreshNodes(force);
            this.RefreshLines();

            if (force) {
                this.CloseExpand();
                this.selectedTalentId = 0;
            }
        }

        private void RefreshLines(bool includeArrow = true) {
            if (this.monoLife == null) {
                return;
            }

            if (!Sys_Talent.Instance.schemes[schemes.selectedIndex].branchs.TryGetValue(this.selectedBranchId, out TalentBranch branch)) {
                return;
            }

            List<TalentEntry> entries = branch.talents;

            int totalCount = 0;
            // 含有前置的entry下标
            int realIndex = 0;
            for (int i = 0, lengthI = entries.Count; i < lengthI; ++i) {
                TalentEntry current = entries[i];
                List<Tuple<TalentEntry, uint>> pres = current.PreLimits;

                if (pres.Count > 0) {
                    for (int j = 0, lengthJ = pres.Count; j < lengthJ; ++j) {
                        totalCount += 1;
                        this.linRenderes.TryBuildOrRefresh(this.lineRendererProto.gameObject, this.lineRendererProto.transform.parent, totalCount, null);
                        this.DrawLine(this.talentNodes[pres[j].Item1.PositionIndex], this.talentNodes[current.PositionIndex], this.linRenderes[totalCount - 1]);
                    }

                    if (false) {
                        this.arrows.TryBuildOrRefresh(this.arrowProto, this.arrowProto.transform.parent, ++realIndex, null);
                        RectTransform rect = this.arrows[realIndex - 1].transform as RectTransform;
                        Vector3 pos = this.talentNodes[current.PositionIndex].transform.position;
                        rect.position = new Vector3(pos.x, pos.y, 0f);
                        rect.anchoredPosition3D = new Vector3(rect.anchoredPosition3D.x + 38f, rect.anchoredPosition3D.y, 0f);

                        this.arrows[realIndex - 1].gameObject.SetActive(true);
                    }
                }
            }
        }

        private void DrawLine(UITalentNode left, UITalentNode right, LineRenderer line) {
            if (left != right) {
                line.positionCount = 1;
                int vertexIndex = 0;
                line.SetPosition(vertexIndex++, left.transform.position);

                if (left.entry.csv.position[1] == right.entry.csv.position[1]) {
                    line.positionCount += 1;
                    line.SetPosition(vertexIndex++, right.transform.position);
                }
                else {
                    int diffRow = (int) right.entry.csv.position[0] - (int) left.entry.csv.position[0];
                    int totalDiff = diffRow * 4;
                    UITalentNode up = this.talentNodes[left.entry.PositionIndex + totalDiff - 4];
                    UITalentNode down = this.talentNodes[left.entry.PositionIndex + totalDiff];

                    Vector3 middle = Vector3.Lerp(up.transform.position, down.transform.position, 0.5f);
                    line.positionCount += 1;
                    line.SetPosition(vertexIndex++, middle);

                    up = this.talentNodes[right.entry.PositionIndex - 4];
                    middle = Vector3.Lerp(up.transform.position, right.transform.position, 0.5f);
                    line.positionCount += 1;
                    line.SetPosition(vertexIndex++, middle);

                    line.positionCount += 1;
                    line.SetPosition(vertexIndex++, right.transform.position);
                }
            }
        }

        public void SetNodeSelected(uint talentId, bool isSelected) {
            this.selectedTalentId = talentId;
            Sys_Talent.Instance.SelectedTalentId = talentId;

            for (int i = 0, length = this.talentNodes.Count; i < length; ++i) {
                this.talentNodes[i].SetSelected(this.talentNodes[i].talentId == talentId);
            }

            if (!this.isExpand) {
                this.animator.Play(UITalentTab.OpenId, -1, 0f);
                this.isExpand = true;
            }

            this.talentNodeShow.Refresh(this, talentId, schemes.selectedIndex);
        }

        public void CloseExpand() {
            if (this.isExpand) {
                for (int i = 0, length = this.talentNodes.Count; i < length; ++i) {
                    this.talentNodes[i].SetButtonEnable(true);
                }

                this.animator.Play(UITalentTab.CloseId, -1, 0f);
                this.isExpand = false;
            }
        }

        public void RefreshTabs() {
            int index = 0;
            foreach (var kvp in Sys_Talent.Instance.schemes[schemes.selectedIndex].branchs) {
                this.tabs[index].Refresh(this, kvp.Key, index++, schemes.selectedIndex);
            }
        }

        public void RefreshNodes(bool clearHighlight = false) {
            for (int i = 0, length = this.talentNodes.Count; i < length; ++i) {
                this.talentNodes[i].Hide();
            }

            TalentBranch branch = Sys_Talent.Instance.schemes[schemes.selectedIndex].branchs[this.selectedBranchId];
            var branchs = branch.talents;
            for (int i = 0, length = branchs.Count; i < length; ++i) {
                int index = branch.talents[i].PositionIndex;
                this.talentNodes[index].Refresh(this, branchs[i].id, schemes.selectedIndex);
                this.talentNodes[index].Show();
                if (clearHighlight) {
                    this.talentNodes[i].SetSelected(false);
                }
            }
        }

        public void RefreshTitle() {
            this.usedPoint.text = Sys_Talent.Instance.schemes[schemes.selectedIndex].usedTalentPoint.ToString();
            this.remainPoint.text = Sys_Talent.Instance.schemes[schemes.selectedIndex].canUseTalentPoint.ToString();

            bool canShowPoint = Sys_Talent.Instance.CanLianhua();
            this.redPoint.gameObject.SetActive(canShowPoint);
        }

        private void OnBtnResetClicked() {
            UIManager.OpenUI(EUIID.UI_TalentReset, false, schemes.selectedIndex);
        }

        private void OnBtnUsingClicked() {
            Sys_Talent.Instance.SwitchSchemeReq((uint)schemes.selectedIndex);
        }

        private void OnBtnAddClicked() {
            UIManager.OpenUI(EUIID.UI_TalentExchange, false, schemes.selectedIndex);
        }

        #region 事件处理

        protected override void ProcessEventsForAwake(bool toRegister) {
            Sys_Talent.Instance.eventEmitter.Handle<uint, uint>(Sys_Talent.EEvents.OnUpdateTalentLevel, this.OnUpdateTalentLevel, toRegister);
            Sys_Talent.Instance.eventEmitter.Handle<uint>(Sys_Talent.EEvents.OnResetTalentPoint, this.OnResetTalentPoint, toRegister);
            Sys_Talent.Instance.eventEmitter.Handle(Sys_Talent.EEvents.OnExchangeTalentPoint, this.OnExchangeTalentPoint, toRegister);
            Sys_Talent.Instance.eventEmitter.Handle(Sys_Talent.EEvents.OnTalentLimitChanged, this.OnTalentLimitChanged, toRegister);
            Sys_Talent.Instance.eventEmitter.Handle(Sys_Talent.EEvents.OnCanRemainTalentChanged, this.OnCanRemainTalentChanged, toRegister);
            Sys_Plan.Instance.eventEmitter.Handle<uint, uint, string>(Sys_Plan.EEvents.ChangePlanName, OnChangeName, toRegister);
            Sys_Plan.Instance.eventEmitter.Handle<uint, uint>(Sys_Plan.EEvents.AddNewPlan, OnAddNewPlan, toRegister);
            Sys_Bag.Instance.eventEmitter.Handle<int, int>(Sys_Bag.EEvents.OnRefreshChangeData, this.OnItemCountChanged, toRegister);
            Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnAddExp, this.OnPlayerExpChanged, toRegister);
            Sys_Talent.Instance.eventEmitter.Handle<uint>(Sys_Talent.EEvents.ChangePlan, OnChangePlan, toRegister);
        }

        private void OnAddNewPlan(uint planType, uint index) {
            if (planType == (uint) Sys_Plan.EPlanType.Talent) {
                Refresh(ui);
            }
        }

        private void OnChangePlan(uint index) {
            if (index == schemes.selectedIndex) {
                Refresh(ui);
            }
        }

        private new void OnRefresh() {
            this.RefreshTitle();
            this.RefreshTabs();

            // keep before of SetTabSelected
            if (this.selectedTalentId != 0) {
                this.talentNodeShow.Refresh(this, this.selectedTalentId, schemes.selectedIndex);
            }

            this.SetTabSelected(this.tabs[this.selectedTabIndex], this.tabs[this.selectedTabIndex].branchId, this.selectedTabIndex, false);
        }

        private void OnUpdateTalentLevel(uint talentId, uint index) {
            if (index != schemes.selectedIndex) {
                return;
            }

            Sys_Hint.Instance.PushEffectInNextFight();

            var talent = Sys_Talent.Instance.schemes[schemes.selectedIndex].GetTalent(talentId);
            if (talent != null && talent.csv.branch_id == this.selectedBranchId) {
                this.OnRefresh();
            }

            this.talentNodeShow?.SetFx(true);
        }

        private void OnResetTalentPoint(uint index) {
            if (index != schemes.selectedIndex) {
                return;
            }

            this.OnRefresh();
        }

        private void OnExchangeTalentPoint() {
            this.OnRefresh();
        }

        private void OnTalentLimitChanged() {
            this.OnRefresh();
        }

        private void OnCanRemainTalentChanged() {
            this.OnRefresh();
        }

        private void OnChangeName(uint planType, uint index, string name) {
            if (planType == (uint) Sys_Plan.EPlanType.Talent) {
                schemes.Refresh(Sys_Talent.Instance.schemes.Count, schemes.selectedIndex, OnSelectedScheme, false);
            }
        }

        private void OnItemCountChanged(int changeType, int curBoxId) {
            this.OnRefresh();
        }

        private void OnPlayerExpChanged() {
            this.OnRefresh();
        }

        #endregion
    }
}