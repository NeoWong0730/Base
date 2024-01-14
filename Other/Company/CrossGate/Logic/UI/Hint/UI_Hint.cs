using Lib.Core;
using Logic.Core;
using System;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    // HintUI
    public class UI_Hint : UIBase {
        public class HintBase : UIComponent {
            protected UI_Hint uiHint;
            protected Timer timer;
            protected HintElement hint;
            protected CP_AnimationCurve curve;

            protected override void Loaded() {
                this.curve = this.gameObject.GetComponent<CP_AnimationCurve>();
                if (this.curve.useCurve) {
                    this.curve.onChange += this.UpdateCurve;
                }
            }
            public HintBase(UI_Hint uiMain) {
                this.uiHint = uiMain;
            }
            private void UpdateCurve(float posX, float posY, float posZ) {
                this.transform.localPosition = new Vector3(posX, posY, posZ);
            }
            public virtual void Refresh(HintElement hint, int index) {
                this.hint = hint;
                if (hint.hintType != HintType.Property) {
                    ++this.uiHint.normalShowingCount;
                }
                else {
                    ++this.uiHint.otherShowingCount;
                }

                this.timer?.Cancel();
                if (this.curve.useCurve) {
                    this.curve.Set(true).SetPositionIndex(index);
                }
                this.timer = Timer.RegisterOrReuse(ref this.timer, this.curve.fadeTime, this.TrySuicide);
                FrameworkTool.ForceRebuildLayout(this.gameObject);
                // 播放tween动画
            }
            protected virtual bool CanPushToPool() {
                return true;
            }
            protected virtual void TrySuicide() {
                if (this.hint.hintType != HintType.Property) {
                    --this.uiHint.normalShowingCount;
                }
                else {
                    --this.uiHint.otherShowingCount;
                }

                this.Hide();
                this.timer?.Cancel();
                this.curve.Set(false);
                Sys_Hint.Instance.PushToPool(this.hint);
                if (!this.CanPushToPool()) {
                    UnityEngine.Object.Destroy(this.gameObject);
                }
            }
        }
        public class Hint_Normal : HintBase {
            protected HintElement_Normal hintConcerete;

            private Text cpText;

            public Hint_Normal(UI_Hint uiMain) : base(uiMain) { }
            protected override void Loaded() {
                base.Loaded();
                this.cpText = this.transform.Find("BG/Mirror/Image_Notice_BG/Text_Notice").GetComponent<Text>();
                this.cpText.text = "";
            }
            public override void Refresh(HintElement hint, int index) {
                base.Refresh(hint, index);
                this.hintConcerete = hint as HintElement_Normal;
                this.cpText.text = this.hintConcerete.content; // + "  " + uiHint.showingCount + " " + index.ToString();

                if (string.IsNullOrEmpty(this.hintConcerete.content)) {
                    DebugUtil.LogFormat(ELogType.eHint, "hint error");
                }
            }
            protected override bool CanPushToPool() {
                return this.uiHint.poolNormal.Push(this);
            }
        }
        public class Hint_GetReward : HintBase {
            protected HintElement_GetReward hintConcerete;

            private GameObject itemNode;
            private Image itemIcon;
            private Text itemName;
            private Image itemSkill;

            public Hint_GetReward(UI_Hint uiMain) : base(uiMain) { }
            protected override void Loaded() {
                base.Loaded();
                this.itemNode = this.transform.Find("Text_Item").gameObject;
                this.itemIcon = this.itemNode.transform.Find("icon").GetComponent<Image>();
                this.itemName = this.itemNode.transform.Find("Text2").GetComponent<Text>();
                this.itemSkill = this.itemNode.transform.Find("icon/Image_Skill").GetComponent<Image>();
            }
            public override void Refresh(HintElement hint, int index) {
                base.Refresh(hint, index);
                this.hintConcerete = hint as HintElement_GetReward;

                this.itemName.text = this.hintConcerete.content;
                ImageHelper.SetIcon(this.itemIcon, CSVItem.Instance.GetConfData(this.hintConcerete.itemid).icon_id);
                Sys_Skill.Instance.ShowPetSkillBook(itemSkill, CSVItem.Instance.GetConfData(this.hintConcerete.itemid));
                LayoutRebuilder.ForceRebuildLayoutImmediate(this.itemNode.transform as RectTransform);
            }
            protected override bool CanPushToPool() {
                return this.uiHint.poolGetReward.Push(this);
            }
        }
        public class Hint_Property : HintBase {
            protected HintElement_Property hintConcerete;

            public new Text name;
            public Text dot;
            public Text number;

            public Hint_Property(UI_Hint uiMain) : base(uiMain) { }
            protected override void Loaded() {
                base.Loaded();
                this.name = this.transform.Find("Text_Property/Text_Property").GetComponent<Text>();
                this.dot = this.transform.Find("Text_Property/Text_Property/Text_Add").GetComponent<Text>();
                this.number = this.transform.Find("Text_Property/Text_Property/Text_Num").GetComponent<Text>();
            }
            public override void Refresh(HintElement hint, int index) {
                base.Refresh(hint, index);
                this.hintConcerete = hint as HintElement_Property;

                if (this.hintConcerete.csvAttr != null) {
                    float abs = Mathf.Abs(this.hintConcerete.diff);
                    string propertyName = LanguageHelper.GetTextContent(this.hintConcerete.csvAttr.name);
                    if (this.hintConcerete.diff >= 0) {
                        TextHelper.SetText(this.name, 2007801, propertyName);
                        TextHelper.SetText(this.dot, 2007802);
                        TextHelper.SetText(this.number, 2007803, this.hintConcerete.csvAttr.show_type == 1 ? abs.ToString() : (abs / 10000f).ToString("P2"));
                    }
                    else {
                        TextHelper.SetText(this.name, 2007804, propertyName);
                        TextHelper.SetText(this.dot, 2007805);
                        TextHelper.SetText(this.number, 2007806, this.hintConcerete.csvAttr.show_type == 1 ? abs.ToString() : (abs / 10000f).ToString("P2"));
                    }
                }
                else if (this.hintConcerete.csvActiveSkill != null) {
                    float abs = Mathf.Abs(this.hintConcerete.diff);
                    string propertyName = LanguageHelper.GetTextContent(this.hintConcerete.csvActiveSkill.name);
                    if (this.hintConcerete.diff >= 0) {
                        TextHelper.SetText(this.name, 2007807, propertyName);
                        TextHelper.SetText(this.dot, 2007808);
                        TextHelper.SetText(this.number, 2007809,  abs.ToString());
                    }
                    else {
                        TextHelper.SetText(this.name, 2007804, propertyName);
                        TextHelper.SetText(this.dot, 2007805);
                        TextHelper.SetText(this.number, 2007806, abs.ToString());
                    }
                }
            }
            protected override bool CanPushToPool() {
                return this.uiHint.poolProperty.Push(this);
            }
        }
        public class Hint_PropertyDirect : HintBase {
            protected HintElement_PropertyDirect hintConcerete;

            public Text content;
            public Text add;
            public Text num;

            public Hint_PropertyDirect(UI_Hint uiMain) : base(uiMain) { }
            protected override void Loaded() {
                base.Loaded();
                this.content = this.transform.Find("Text_Property/Text_Property").GetComponent<Text>();
                this.add = this.transform.Find("Text_Property/Text_Property/Text_Add").GetComponent<Text>();
                this.num = this.transform.Find("Text_Property/Text_Property/Text_Num").GetComponent<Text>();
            }
            public override void Refresh(HintElement hint, int index) {
                base.Refresh(hint, index);
                this.hintConcerete = hint as HintElement_PropertyDirect;

                this.add.text = "";
                this.num.text = "";
                TextHelper.SetText(this.content, this.hintConcerete.content);
            }
            protected override bool CanPushToPool() {
                return this.uiHint.poolPropertyDirect.Push(this);
            }
        }

        public class Hint_InBattle : HintBase
        {
            protected HintElement_InBattle hintConcerete;

            private Text cpText;

            public Hint_InBattle(UI_Hint uiMain) : base(uiMain) { }
            protected override void Loaded()
            {
                base.Loaded();
                this.cpText = this.transform.Find("BG/Mirror/Image_Notice_BG/Text_Notice").GetComponent<Text>();
                this.cpText.text = "";
            }
            public override void Refresh(HintElement hint, int index)
            {
                base.Refresh(hint, index);
                this.hintConcerete = hint as HintElement_InBattle;
                this.cpText.text = this.hintConcerete.content; 

                if (string.IsNullOrEmpty(this.hintConcerete.content))
                {
                    DebugUtil.LogFormat(ELogType.eHint, "hint error");
                }
            }
            protected override bool CanPushToPool()
            {
                return this.uiHint.poolInBattle.Push(this);
            }
        }
        public class Hint_Static : HintBase {
            protected HintElement_Static hintConcerete;
            private Text cpText;

            public Hint_Static(UI_Hint uiMain) : base(uiMain) { }

            protected override void Loaded() {
                this.cpText = this.transform.Find("BG/Mirror/Image_Notice_BG/Text_Notice").GetComponent<Text>();
                this.curve = this.gameObject.GetComponent<CP_AnimationCurve>();
                this.cpText.text = "";
            }
            public override void Refresh(HintElement hint, int index) {
                ++this.uiHint.staticShowingCount;

                this.hintConcerete = hint as HintElement_Static;
                this.cpText.text = this.hintConcerete.content; // + "  " + uiHint.showingCount + " " + index.ToString();

                if (string.IsNullOrEmpty(this.hintConcerete.content)) {
                    DebugUtil.LogFormat(ELogType.eHint, "hint error");
                }

                this.timer?.Cancel();
                if (this.curve.useCurve) {
                    this.curve.Set(true).SetPositionIndex(index);
                }
                this.timer = Timer.Register(this.curve.fadeTime, this.TrySuicide);

                FrameworkTool.ForceRebuildLayout(this.gameObject);
            }
            protected override void TrySuicide() {
                --this.uiHint.staticShowingCount;
                Sys_Hint.Instance.hintStatic.Clear();

                this.Hide();
                this.timer?.Cancel();
                this.curve.Set(false);
                Sys_Hint.Instance.PushToPool(this.hint);
                if (!this.CanPushToPool()) {
                    UnityEngine.Object.Destroy(this.gameObject);
                }
            }
            protected override bool CanPushToPool() {
                return this.uiHint.poolStatic.Push(this);
            }
        }

        public Transform proto_Normal;
        public Transform proto_GetReward;
        public Transform proto_Property;
        public Transform proto_PropertyDirect;
        public Transform proto_Static;
        public Transform proto_inBattle;

        // public Animator mapEnterGo;
        // public Text textMapEnter;

        public Transform protoParent;

        public const int StaticShowMax = 1;
        public const int NormalShowMax = 5;
        public const int OtherShowMax = 1;

        public int staticShowingCount = 0;
        public int normalShowingCount = 0;
        public int otherShowingCount = 0;

        private int staticIndex = 0;
        private int normalIndex = 0;
        private int otherIndex = 0;

        public Lib.Core.ObjectPool<HintBase> poolNormal = new Lib.Core.ObjectPool<HintBase>(5);
        public Lib.Core.ObjectPool<HintBase> poolGetReward = new Lib.Core.ObjectPool<HintBase>(5);
        public Lib.Core.ObjectPool<HintBase> poolProperty = new Lib.Core.ObjectPool<HintBase>(5);
        public Lib.Core.ObjectPool<HintBase> poolPropertyDirect = new Lib.Core.ObjectPool<HintBase>(1);
        public Lib.Core.ObjectPool<HintBase> poolInBattle = new Lib.Core.ObjectPool<HintBase>(5);
        public Lib.Core.ObjectPool<HintBase> poolStatic = new Lib.Core.ObjectPool<HintBase>(1);

        private UI_HintNotice _UI_HintNotice;
        private UI_HintBottomNotice _UI_HintBottomNotice;

        protected override void OnLoaded() {
            // this.mapEnterGo = this.transform.Find("Animator/Hint_MapEnter").GetComponent<Animator>();
            // this.textMapEnter = this.transform.Find("Animator/Hint_MapEnter/Animator/Text").GetComponent<Text>();
            // this.mapEnterGo.gameObject.SetActive(false);
            
            this.proto_Normal = this.transform.Find("Animator/Hint_Normal");
            this.proto_Normal.gameObject.SetActive(false);

            this.proto_GetReward = this.transform.Find("Animator/Hint_GetReward");
            this.proto_GetReward.gameObject.SetActive(false);

            this.proto_Property = this.transform.Find("Animator/Hint_Property");
            this.proto_Property.gameObject.SetActive(false);

            this.proto_PropertyDirect = this.transform.Find("Animator/Hint_PropertyDirect");
            this.proto_PropertyDirect.gameObject.SetActive(false);

            this.proto_Static = this.transform.Find("Animator/Hint_Task");
            this.proto_Static.gameObject.SetActive(false);

            this.proto_inBattle = this.transform.Find("Animator/Hint_Battle_Skill");
            this.proto_inBattle.gameObject.SetActive(false);

            this.protoParent = this.transform.Find("Animator");

            _UI_HintNotice = AddComponent<UI_HintNotice>(transform.Find("Animator/Hint_Notice"));
            _UI_HintBottomNotice = AddComponent<UI_HintBottomNotice>(transform.Find("Animator/Hint_BottomNotice"));

        }

        protected override void OnUpdate() {
            //if (Input.GetKeyDown(KeyCode.V)) {
            //    Sys_Hint.Instance.PushContent_Property("治理 + 2");
            //    Sys_Hint.Instance.PushContent_Property("物理 + 2");
            //}
            //else if (Input.GetKeyDown(KeyCode.B))
            //{
            //    Sys_Hint.Instance.PushContent_Normal("========");
            //}
            // else if (Input.GetKeyDown(KeyCode.C))
            // {
            //     Sys_Hint.Instance.PushContent_Map("法兰城");
            // }

#if USE_SPLIT_FRAME
            //if (TimeManager.updateCount % 3 == 0)
#else
            if (Time.frameCount % 3 == 0)
#endif
            {
                if (!Framework.SceneManager.bMainSceneLoading && !Sys_CutScene.Instance.isPlaying) {
                    this.HandleNormal();
                    this.HandleStatic();
                    if (GameMain.Procedure.m_ProcedureFsm != null && GameMain.Procedure.CurrentProcedure != null && GameMain.Procedure.CurrentProcedure.ProcedureType != ProcedureManager.EProcedureType.Fight) {
                        this.HandleOther();
                    }
                }
            }
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {            
            Sys_Hint.Instance.eventEmitter.Handle<HitElement_Marquee>(Sys_Hint.EEvents.RefreshMarqueeData, (data) => _UI_HintNotice.RefreshMarqueeData(data), toRegister);
            Sys_Hint.Instance.eventEmitter.Handle<HitElement_CommonInfo>(Sys_Hint.EEvents.RefreshCommonInfoData, (data) => _UI_HintBottomNotice.RefreshCommonInfoData(data), toRegister);
            Sys_Hint.Instance.eventEmitter.Handle(Sys_Hint.EEvents.HideGameObject, HideGameObject, toRegister);       
            
            Sys_Map.Instance.eventEmitter.Handle<uint>(Sys_Map.EEvents.OnFirstEnterMap, this.OnFirstEnterMap, toRegister);
        }

        private void OnFirstEnterMap(uint mapId) {
            // if (!UIManager.IsOpen(EUIID.UI_Menu)) {
            //     return;
            // }
            //
            // // 动画结束之后，关闭节点
            // var csvMap = CSVMapInfo.Instance.GetConfData(mapId);
            // if (csvMap != null && csvMap.PromptForMapUnlocking) {
            //     this.mapEnterGo.gameObject.SetActive(true);
            //     // 重新播放动画
            //     this.mapEnterGo.Play("Open", -1, 0);
            //     TextHelper.SetText(this.textMapEnter, 4568, LanguageHelper.GetTextContent(csvMap.name));
            // }
            // else {
            //     this.mapEnterGo.gameObject.SetActive(false);
            // }
        }

        private void HideGameObject()
        {
            if (_UI_HintNotice.gameObject.activeSelf)
                _UI_HintNotice.SetHide();
            if (_UI_HintBottomNotice.gameObject.activeSelf)
                _UI_HintBottomNotice.SetHide();
            
            // this.mapEnterGo.gameObject.SetActive(false);
        }

        private HintBase TryGetHint(HintElement heInfo) {
            HintBase hint = null;
            if (heInfo.hintType == HintType.Normal) {
                hint = this.poolNormal.Get();
                if (hint == null) {
                    hint = new Hint_Normal(this);
                    hint?.Init(GameObject.Instantiate<GameObject>(this.proto_Normal.gameObject, this.protoParent).transform);
                }
            }
            else if (heInfo.hintType == HintType.GetReward) {
                hint = this.poolGetReward.Get();
                if (hint == null) {
                    hint = new Hint_GetReward(this);
                    hint?.Init(GameObject.Instantiate<GameObject>(this.proto_GetReward.gameObject, this.protoParent).transform);
                }
            }
            else if (heInfo.hintType == HintType.Property) {
                hint = this.poolProperty.Get();
                if (hint == null) {
                    hint = new Hint_Property(this);
                    hint?.Init(GameObject.Instantiate<GameObject>(this.proto_Property.gameObject, this.protoParent).transform);
                }
            }
            else if (heInfo.hintType == HintType.PropertyDirect) {
                hint = this.poolPropertyDirect.Get();
                if (hint == null) {
                    hint = new Hint_PropertyDirect(this);
                    hint?.Init(GameObject.Instantiate<GameObject>(this.proto_PropertyDirect.gameObject, this.protoParent).transform);
                }
            }
            else if (heInfo.hintType == HintType.InBattle)
            {
                hint = this.poolInBattle.Get();
                if (hint == null)
                {
                    hint = new Hint_InBattle(this);
                    hint?.Init(GameObject.Instantiate<GameObject>(this.proto_inBattle.gameObject, this.protoParent).transform);
                }
            }
            else if (heInfo.hintType == HintType.Static) {
                hint = this.poolStatic.Get();
                if (hint == null) {
                    hint = new Hint_Static(this);
                    hint?.Init(GameObject.Instantiate<GameObject>(this.proto_Static.gameObject, this.protoParent).transform);
                }
            }
            return hint;
        }
        private void HandleNormal() {
            if (Sys_Hint.Instance.CountForNormal() > 0) {
                if (this.normalShowingCount >= NormalShowMax) { return; }
                if (this.normalShowingCount <= 0) { this.normalIndex = 0; }

                HintElement heInfo = Sys_Hint.Instance.hintNormal.Pop();
                if (heInfo != null) {
                    HintBase hint = this.TryGetHint(heInfo);
                    if (hint != null) {
                        this.normalIndex = this.normalIndex % NormalShowMax;
                        hint.Show();
                        hint.Refresh(heInfo, this.normalIndex);

                        ++this.normalIndex;
#if UNITY_EDITOR
                        hint.transform.SetAsLastSibling();
                        hint.transform.gameObject.name = heInfo.ToString();
#endif
                    }
                }
            }
        }
        private void HandleOther() {
            if (Sys_Hint.Instance.CountForOther() > 0) {
                if (this.otherShowingCount >= OtherShowMax) { return; }
                if (this.otherShowingCount <= 0) { this.otherIndex = 0; }

                HintElement heInfo = Sys_Hint.Instance.hintOther.Pop();
                if (heInfo != null) {
                    HintBase hint = this.TryGetHint(heInfo);
                    if (hint != null) {
                        this.otherIndex = this.otherIndex % OtherShowMax;
                        hint.Show();
                        hint.Refresh(heInfo, this.otherIndex);

                        ++this.otherIndex;
#if UNITY_EDITOR
                        hint.transform.SetAsLastSibling();
                        hint.transform.gameObject.name = heInfo.ToString();
#endif
                    }
                }
            }
        }
        private void HandleStatic() {
            if (Sys_Hint.Instance.CountForStatic() > 0) {
                if (this.staticShowingCount >= StaticShowMax) { return; }
                if (this.staticShowingCount <= 0) { this.staticIndex = 0; }

                HintElement heInfo = Sys_Hint.Instance.hintStatic.Pop();
                if (heInfo != null) {
                    HintBase hint = this.TryGetHint(heInfo);
                    if (hint != null) {
                        hint.Show();
                        hint.Refresh(heInfo, this.staticIndex);

#if UNITY_EDITOR
                        hint.transform.SetAsLastSibling();
                        hint.transform.gameObject.name = heInfo.ToString();
#endif
                    }
                }
            }
        }
    }
}
