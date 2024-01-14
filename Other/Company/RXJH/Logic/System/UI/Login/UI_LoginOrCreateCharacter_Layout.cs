// Generated by UIComponentBinder.cs, you can't modifity it manualy!
using System;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using TMPro;

namespace Logic.UI {
    [Serializable]
    public partial class UI_LoginOrCreateCharacter_Layout : UILayoutBase {
        public UnityEngine.UI.Button btnEnter = null; // [0] Path: "Animator/Btn_Start"
        public UnityEngine.UI.Button btnReturn = null; // [1] Path: "Animator/Btn_Return"
        public UnityEngine.RectTransform proto = null; // [2] Path: "Animator/Scroll/Viewport/Content/Item"
        public UnityEngine.RectTransform protoParent = null; // [3] Path: "Animator/Scroll/Viewport/Content"
        public CDText cdText = null; // [4] Path: "Animator/Remain/Remain"
        public UnityEngine.RectTransform cdNode = null; // [5] Path: "Animator/Remain"
        public UICenterOnChild centerOn = null; // [6] Path: "Animator/Scroll"
        public ToggleRegistry tgRegistry = null; // [7] Path: "Animator/Scroll/Viewport/Content"

        protected override void Loaded() {
            this.btnEnter = binder.Find<UnityEngine.UI.Button>(0);
            this.btnReturn = binder.Find<UnityEngine.UI.Button>(1);
            this.proto = binder.Find<UnityEngine.RectTransform>(2);
            this.protoParent = binder.Find<UnityEngine.RectTransform>(3);
            this.cdText = binder.Find<CDText>(4);
            this.cdNode = binder.Find<UnityEngine.RectTransform>(5);
            this.centerOn = binder.Find<UICenterOnChild>(6);
            this.tgRegistry = binder.Find<ToggleRegistry>(7);
        }

#if UNITY_EDITOR
        protected override void FindByPath(UnityEngine.Transform transform, bool check = false) {
            if (!check) {
                this.btnEnter = transform.Find("Animator/Btn_Start").GetComponent<UnityEngine.UI.Button>();
                this.btnReturn = transform.Find("Animator/Btn_Return").GetComponent<UnityEngine.UI.Button>();
                this.proto = transform.Find("Animator/Scroll/Viewport/Content/Item").GetComponent<UnityEngine.RectTransform>();
                this.protoParent = transform.Find("Animator/Scroll/Viewport/Content").GetComponent<UnityEngine.RectTransform>();
                this.cdText = transform.Find("Animator/Remain/Remain").GetComponent<CDText>();
                this.cdNode = transform.Find("Animator/Remain").GetComponent<UnityEngine.RectTransform>();
                this.centerOn = transform.Find("Animator/Scroll").GetComponent<UICenterOnChild>();
                this.tgRegistry = transform.Find("Animator/Scroll/Viewport/Content").GetComponent<ToggleRegistry>();
            }
            else {
                UnityEngine.Transform _t_ = null;
                _t_ = transform.Find("Animator/Btn_Start");
                this.btnEnter = _t_ != null ? _t_.GetComponent<UnityEngine.UI.Button>() : null;
                _t_ = transform.Find("Animator/Btn_Return");
                this.btnReturn = _t_ != null ? _t_.GetComponent<UnityEngine.UI.Button>() : null;
                _t_ = transform.Find("Animator/Scroll/Viewport/Content/Item");
                this.proto = _t_ != null ? _t_.GetComponent<UnityEngine.RectTransform>() : null;
                _t_ = transform.Find("Animator/Scroll/Viewport/Content");
                this.protoParent = _t_ != null ? _t_.GetComponent<UnityEngine.RectTransform>() : null;
                _t_ = transform.Find("Animator/Remain/Remain");
                this.cdText = _t_ != null ? _t_.GetComponent<CDText>() : null;
                _t_ = transform.Find("Animator/Remain");
                this.cdNode = _t_ != null ? _t_.GetComponent<UnityEngine.RectTransform>() : null;
                _t_ = transform.Find("Animator/Scroll");
                this.centerOn = _t_ != null ? _t_.GetComponent<UICenterOnChild>() : null;
                _t_ = transform.Find("Animator/Scroll/Viewport/Content");
                this.tgRegistry = _t_ != null ? _t_.GetComponent<ToggleRegistry>() : null;
            }
        }
#endif

        public interface IListener {
            void OnBtnClicked_btnEnter();
            void OnBtnClicked_btnReturn();
            void OnTimeRefresh_cdText(TMPro.TextMeshProUGUI text, float time, bool isEnd);
            void OnCenter_centerOn(int index, UnityEngine.Transform t);
            void OnTransform_centerOn(bool hOrV, int index, UnityEngine.Transform t, float toMiddle, UnityEngine.Vector3 srCenterOnCentent);
            void OnToggleChange_tgRegistry(int newId, int oldId, bool interaction);
        }

        public void BindEvents(IListener listener, bool toListen = true) {
            this.btnEnter.onClick.AddListener(listener.OnBtnClicked_btnEnter);
            this.btnReturn.onClick.AddListener(listener.OnBtnClicked_btnReturn);
            this.cdText.onTimeRefresh += listener.OnTimeRefresh_cdText;
            this.centerOn.onCenter += listener.OnCenter_centerOn;
            this.centerOn.onTransform += listener.OnTransform_centerOn;
            this.tgRegistry.onToggleChange += listener.OnToggleChange_tgRegistry;
        }
    }
}
