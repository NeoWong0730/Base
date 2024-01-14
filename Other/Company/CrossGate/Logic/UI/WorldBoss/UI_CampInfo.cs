using System;
using System.Collections.Generic;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    // 阵营信息
    public class UI_CampInfo : UIBase {
        public class UIPopdownItem : UISelectableElement {
            public uint campId;
            public string campName;

            public Text text;
            public Button button;
            public GameObject highlight;

            protected override void Loaded() {
                this.button = this.transform.GetComponent<Button>();
                this.text = this.transform.Find("Text").GetComponent<Text>();
                this.highlight = this.transform.Find("Image").gameObject;
                this.button.onClick.AddListener(this.OnBtnClicked);
            }

            public void SetHighlight(bool setHighLight = false) {
                this.highlight.SetActive(setHighLight);
            }
            public void Refresh(uint campId, int index) {
                this.campId = campId;

                if (campId != 0) {
                    CSVCampInformation.Data csv = CSVCampInformation.Instance.GetConfData(campId);
                    if (csv != null) {
                        TextHelper.SetText(this.text, csv.camp_name);
                        this.campName = this.text.text;
                    }
                }
            }
            private void OnBtnClicked() {
                this.onSelected?.Invoke((int)this.campId, true);
            }
            public override void SetSelected(bool toSelected, bool force) {
                this.OnBtnClicked();
            }
        }
        public class Head : UISelectableElement {
            public Transform redDot;

            public GameObject lockGo;  // 未开放
            public GameObject unlockGo; // 已经开放
            public CP_Toggle toggle;

            public Text nameText;
            public Text jobText;
            public Image icon;
            public Image frameIcon;

            public uint bossId = 0;
            public CSVBOSSManual.Data csvManual {
                get {
                    return CSVBOSSManual.Instance.GetConfData((uint)this.id);
                }
            }
            public CSVBOSSManual.Data preCsvManual {
                get {
                    return CSVBOSSManual.Instance.GetConfData(this.csvManual.manual_id);
                }
            }

            public bool isUnlock = false;

            protected override void Loaded() {
                this.toggle = this.transform.GetComponent<CP_Toggle>();
                this.toggle.onValueChanged.AddListener(this.Switch);

                this.redDot = this.transform.Find("Image_Red");

                this.lockGo = this.transform.Find("Unknow").gameObject;
                this.unlockGo = this.transform.Find("Know").gameObject;

                this.nameText = this.transform.Find("Text_name").GetComponent<Text>();
                this.jobText = this.transform.Find("Text_job").GetComponent<Text>();
                this.icon = this.transform.Find("Know/Image_boss").GetComponent<Image>();
                this.frameIcon = this.transform.Find("Proto01").GetComponent<Image>();
            }
            public void Switch(bool arg) {
                if (arg) {
                    if (this.isUnlock) {
                        this.onSelected?.Invoke(this.id, true);

                        UIManager.OpenUI(EUIID.UI_WorldBossManualShow, false, new Tuple<uint, uint>(this.bossId, (uint)this.id));
                    }
                }
            }
            public void Refresh(uint bossId) {
                uint manualId = (uint)this.id;
                this.bossId = bossId;

                if (this.csvManual != null) {
                    this.isUnlock = Sys_WorldBoss.Instance.unlockedBossManuales.TryGetValue((uint)this.id, out BossManualData bd);
                    if (this.isUnlock) {
                        this.lockGo.SetActive(false);
                        this.unlockGo.SetActive(true);

                        TextHelper.SetText(this.nameText, this.csvManual.BOSS_name);
                        TextHelper.SetText(this.jobText, this.csvManual.BOSS_title);
                        ImageHelper.SetIcon(this.icon, this.csvManual.head_icon);
                        this.redDot.gameObject.SetActive(bd.HasRewardUnGot);
                    }
                    else {
                        this.lockGo.SetActive(true);
                        this.unlockGo.SetActive(false);

                        TextHelper.SetText(this.nameText, "未知");
                        TextHelper.SetText(this.jobText, "未知");
                        this.redDot.gameObject.SetActive(false);
                    }

                    ImageHelper.SetIcon(this.frameIcon, this.csvManual.headFrame_id);
                    Vector2 anchorPos = new Vector3(this.csvManual.head_postion[0], this.csvManual.head_postion[1]);
                    (this.transform as RectTransform).anchoredPosition = anchorPos;

                    float scale = 1f * this.csvManual.head_scale / 10000f;
                    this.transform.localScale = new Vector3(scale, scale, scale);
#if UNITY_EDITOR
                    this.nameText.text = this.nameText.text + " id:" + this.id.ToString();
                    this.jobText.text = this.jobText.text + " " + anchorPos;
                    this.gameObject.name = this.id.ToString();
#endif
                }
                else {
                    // error
                }
            }
            public override void SetSelected(bool toSelected, bool force) {
                this.toggle.SetSelected(toSelected, force);
            }
        }
        public class Desc : UIComponent {
            public Text title;
            public Text desc;
            public Image icon;

            public Animator animator;
            public Button btn;

            protected override void Loaded() {
                this.animator = this.transform.GetComponent<Animator>();

                this.btn = this.transform.Find("Button").GetComponent<Button>();
                this.btn.onClick.AddListener(this.OnBtnClicked);

                this.title = this.transform.Find("Image_Title/Text_Ttile").GetComponent<Text>();
                this.desc = this.transform.Find("Image_Title/Text_Des").GetComponent<Text>();
                this.icon = this.transform.Find("Image_Title/Image_bottom/Image").GetComponent<Image>();
            }
            public void Refresh(uint campId) {
                CSVCampInformation.Data csv = CSVCampInformation.Instance.GetConfData(campId);
                if (csv != null) {
                    TextHelper.SetText(this.title, csv.camp_name);
                    TextHelper.SetText(this.desc, csv.camp_description);
                    ImageHelper.SetIcon(this.icon, csv.camp_icon);
                }
                else {
                    // error
                }
            }
            private void OnBtnClicked() {
                this.PlayAnimiation(false);
            }
            public void PlayAnimiation(bool open) {
                if (open) {
                    this.animator.Play("Open", -1, 0);
                }
                else {
                    this.animator.Play("Close", -1, 0);
                }
            }
        }

        public Button btnExit;
        public Button btnCampRule;
        public CP_PopdownList popdownList;
        public CP_ToggleRegistry registry;
        public ScrollRect scrollRect;
        public UIElementCollector<UIPopdownItem> popdownVds = new UIElementCollector<UIPopdownItem>();
        public UIElementCollector<Head> headVds = new UIElementCollector<Head>();
        public COWComponent<Image> lines = new COWComponent<Image>();

        public GameObject lineProto;
        public Transform lineProtoParent;

        public GameObject headProto;
        public Transform headProtoParent;

        public Button detailBtn;
        public GameObject detailGo;
        public Desc detailDesc;

        public uint currentCampId;
        public uint moveToManualId;
        public bool hasTargetManualId = false;
        public BossCamp campInfo;

        protected override void OnLoaded() {
            this.lineProto = this.transform.Find("Animator/GameObject/Scroll View/viewport/content/Lines/LineProto").gameObject;
            this.lineProtoParent = this.transform.Find("Animator/GameObject/Scroll View/viewport/content/Lines");
            this.headProto = this.transform.Find("Animator/GameObject/Scroll View/viewport/content/Proto").gameObject;
            this.headProtoParent = this.transform.Find("Animator/GameObject/Scroll View/viewport/content");

            this.btnExit = this.transform.Find("Animator/View_Title09/Btn_Close").GetComponent<Button>();
            this.btnExit.onClick.AddListener(this.OnBtnExitClicked);
            this.btnCampRule = this.transform.Find("Animator/GameObject/Top/Button").GetComponent<Button>();
            this.btnCampRule.onClick.AddListener(this.OnBtnCampRuleClicked);

            this.detailBtn = this.transform.Find("Animator/GameObject/Button").GetComponent<Button>();
            this.detailBtn.onClick.AddListener(this.OnBtnDetailClicked);

            this.scrollRect = this.transform.Find("Animator/GameObject/Scroll View").GetComponent<ScrollRect>();
            this.registry = this.transform.Find("Animator/GameObject/Scroll View/viewport/content").GetComponent<CP_ToggleRegistry>();
            this.detailGo = this.transform.Find("Animator/GameObject/Image_Right").gameObject;

            this.popdownList = this.transform.Find("Animator/GameObject/PopupList").GetComponent<CP_PopdownList>();
        }
        protected override void OnOpen(object arg) {
            Tuple<uint, uint> tp = arg as Tuple<uint, uint>;
            if (tp != null) {
                this.currentCampId = tp.Item1;
                this.moveToManualId = tp.Item2;
                this.hasTargetManualId = this.moveToManualId != 0;
            }
        }
        protected override void OnShow() {
            this.RefreshAll();
        }
        public void RefreshAll() {
            // 设置当前下拉菜单中的默认选中项
            var keys = Sys_WorldBoss.Instance.unlockedCamps.Keys;
            var camps = new List<uint>(keys);
            this.popdownVds.BuildOrRefresh(this.popdownList.optionProto, this.popdownList.optionParent, camps, (vd, id, index) => {
                vd.SetUniqueId((int)id);
                vd.SetSelectedAction((campId, force) => {
                    this.popdownVds.ForEach((e) => {
                        e.SetHighlight(false);
                    });
                    vd.SetHighlight(true);

                    this.popdownList.Expand(false);
                    this.popdownList.SetSelected(vd.campName);

                    //if(currentCampId != (uint)campId)
                    {
                        this.currentCampId = (uint)campId;
                        this.campInfo = Sys_WorldBoss.Instance.unlockedCamps[this.currentCampId];
                        if (!this.hasTargetManualId) {
                            this.moveToManualId = this.campInfo.coreBossId;
                        }
                        else {
                            this.hasTargetManualId = false;
                        }

                        // 切换下拉菜单的时候，清空highlight
                        this.registry.SetHighLight(-1);
                        this.RefreshContent();
                    }
                });
                vd.Refresh(id, index);
                vd.SetHighlight(false);
            });

            // 默认选中Tab
            if (this.popdownVds.CorrectId(ref this.currentCampId, camps)) {
                if (this.popdownVds.TryGetVdById((int)this.currentCampId, out var vd)) {
                    vd.SetSelected(true, true);
                }
            }
        }

        public void RefreshContent() {
            // 1. 刷新boss的加载
            List<uint> manuals = new List<uint>();
            if (Sys_WorldBoss.Instance.unlockedCamps.TryGetValue(this.currentCampId, out BossCamp bossCamp)) {
                manuals = bossCamp.AllManualList;
            }

            // 计算scrollRect的content的大小
            this.CalcBounds(manuals);

            this.headVds.BuildOrRefresh(this.headProto, this.headProtoParent, manuals, (vd, data, index) => {
                vd.SetUniqueId((int)data);
                uint bossId = 0;
                foreach (var kvp in CSVBOSSInformation.Instance.GetAll()) {
                    if (kvp.bossManual_id == data) {
                        bossId = kvp.id;
                        break;
                    }
                }
                vd.Refresh(bossId);
            });
            // 2. 创建lines，个数本来应该小于 headVds.Count， 这里为了方便，让其等于
            this.lines.TryBuild(this.lineProto, this.lineProtoParent, this.headVds.Count);

            // 3. 划线
            for (int i = 0, length = this.headVds.Count; i < length; ++i) {
                this.DrawLine(this.headVds[i], this.lines[i]);
            }

            if (this.detailDesc != null && this.detailDesc.gameObject.activeInHierarchy) {
                this.detailDesc.Refresh(this.currentCampId);
            }

            this.MoveToTargetBoss();
        }

        public void MoveToTargetBoss() {
            if (this.headVds.TryGetVdById((int)this.moveToManualId, out Head vd)) {
                float hor = 1f * Mathf.Abs(vd.csvManual.head_postion[0]) / this.scrollRect.content.rect.width;
                float ver = 1 - 1f * Mathf.Abs(vd.csvManual.head_postion[1]) / this.scrollRect.content.rect.height;
                this.scrollRect.normalizedPosition = new Vector2(hor, ver);

                vd.SetSelected(true, false);
            }
        }

        public void CalcBounds(List<uint> manuals) {
            float minX = float.MaxValue, minY = float.MaxValue, maxX = float.MinValue, maxY = float.MinValue;

            for (int i = 0, length = manuals.Count; i < length; ++i) {
                CSVBOSSManual.Data csvManual = CSVBOSSManual.Instance.GetConfData(manuals[i]);
                if (csvManual != null) {
                    Vector2 pos = new Vector2(csvManual.head_postion[0], csvManual.head_postion[1]);
                    if (pos.x > maxX) {
                        maxX = pos.x;
                    }
                    if (pos.x < minX) {
                        minX = pos.x;
                    }

                    if (pos.y > maxY) {
                        maxY = pos.y;
                    }
                    if (pos.y < minY) {
                        minY = pos.y;
                    }
                }
            }

#if UNITY_EDITOR
            this.scrollRect.content.name = new Vector2(minX, minY) + " -> " + new Vector2(maxX, maxY);
#endif
            // proto宽搞都是200
            this.scrollRect.content.sizeDelta = new Vector2(maxX - minX + 200, maxY - minY + 200);
        }

        public void DrawLine(Head head, Image line) {
            uint manualId = head.csvManual.id;
            var csv = head.preCsvManual;
            uint preManualId = 0;
            if (csv != null) {
                preManualId = head.preCsvManual.id;
            }

            if (line != null) {
                line.gameObject.SetActive(false);
            }
            if (preManualId != 0 && this.headVds.TryGetVdById((int)preManualId, out Head preHead)) {
                this.DrawLine(head, preHead, line);
            }
        }
        public void DrawLine(Head left, Head right, Image line) {
            if (left != null && right != null && left != right) {
                line.gameObject.SetActive(true);

                // 位置
                Vector3 pos = Vector3.Lerp(left.transform.localPosition, right.transform.localPosition, 0.5f);
                line.transform.localPosition = pos;

                // 旋转角度
                float angle = NGUIMath.GetAngleByPoints(left.transform.localPosition, right.transform.localPosition);
                line.transform.localEulerAngles = new Vector3(0f, 0f, angle);

                // 设置宽高
                float distance = Vector3.Distance(left.transform.localPosition, right.transform.localPosition);
                line.rectTransform.sizeDelta = new Vector2(distance, line.rectTransform.sizeDelta.y);

#if UNITY_EDITOR
                line.name = left.id + " " + right.id + " " + left.transform.localPosition + " " + right.transform.localPosition + " pos:" + pos + " angle:" + angle + " distance:" + distance;
#endif
            }
        }

        private void OnBtnExitClicked() {
            this.CloseSelf();
        }
        private void OnBtnCampRuleClicked() {
            UIManager.OpenUI(EUIID.UI_WorldBossCampPreview, false, this.currentCampId);
        }
        private void OnBtnDetailClicked() {
            if (this.detailDesc == null) {
                this.detailDesc = new Desc();
                this.detailDesc.Init(this.detailGo.transform);
            }
            this.detailDesc.Refresh(this.currentCampId);

            this.detailDesc.Show();
            this.detailDesc.PlayAnimiation(true);
        }
    }
}


