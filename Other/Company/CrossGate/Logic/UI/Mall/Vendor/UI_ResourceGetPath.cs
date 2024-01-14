using System.Collections.Generic;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Logic {
    public class UI_ResourceGetPath : UIBase, UI_ResourceGetPath.Tab.IListener {
        public class Tab : UIComponent {
            private Image icon;
            private RawImage bg;
            private Text desc;

            private GameObject notOpenGo;
            private Button btnGoto;

            private IListener listener;

            private CSVTaskLevelConfine.Data csv;

            private AsyncOperationHandle<Texture> assetRequest;

            protected override void Loaded() {
                this.icon = this.transform.Find("Icon").GetComponent<Image>();
                this.bg = this.transform.GetComponent<RawImage>();
                this.desc = this.transform.Find("Desc").GetComponent<Text>();

                this.notOpenGo = this.transform.Find("Lock").gameObject;
                this.btnGoto = this.transform.Find("Button_Goto").GetComponent<Button>();
                this.btnGoto.onClick.AddListener(this.OnBtnGotoClicked);
            }
            public override void OnDestroy() {
                if (this.assetRequest.IsValid()) {
                    AddressablesUtil.Release<Texture>(ref this.assetRequest, this.OnCompleted);
                }
                base.OnDestroy();
            }

            private void OnBtnGotoClicked() {
                this.listener?.OnBtnGotoClicked(this.csv);
            }
            public void Refresh(CSVTaskLevelConfine.Data csv) {
                this.csv = csv;

                TextHelper.SetText(this.desc, csv.Description);
                ImageHelper.SetIcon(this.icon, csv.Icon);

                if (this.assetRequest.IsValid()) {
                    AddressablesUtil.Release<Texture>(ref this.assetRequest, this.OnCompleted);
                }
                AddressablesUtil.LoadAssetAsync<Texture>(ref this.assetRequest, csv.bg, this.OnCompleted);

                bool isOpen = Sys_FunctionOpen.Instance.IsOpen(csv.Way, false);
                this.notOpenGo.gameObject.SetActive(!isOpen);
                this.btnGoto.gameObject.SetActive(isOpen);
            }

            private void OnCompleted(AsyncOperationHandle<Texture> handle) {
                this.bg.texture = handle.Result;
            }
            public void Register(IListener listener) {
                this.listener = listener;
            }

            public interface IListener {
                void OnBtnGotoClicked(CSVTaskLevelConfine.Data csv);
            }
        }

        public Button btnExit;

        public GameObject proto;
        public Transform protoParent;
        public COWVd<Tab> tabs = new COWVd<Tab>();
        public Text text;

        private List<CSVTaskLevelConfine.Data>  ls;
        private int[] wordList;

        protected override void OnLoaded() {
            this.proto = this.transform.Find("Animator/View_Right/Scroll View/Viewport/Content/Proto").gameObject;
            this.protoParent = this.transform.Find("Animator/View_Right/Scroll View/Viewport/Content");

            this.btnExit = this.transform.Find("Animator/View_TipsBgNew01/Btn_Close").GetComponent<Button>();
            this.btnExit.onClick.AddListener(this.OnBtnExitClicked);

            this.text = this.transform.Find("Animator/View_Left/Image_bottom/Text").GetComponent<Text>();
            Button btn = this.transform.Find("Animator/View_Left/Button").GetComponent<Button>();
            btn.onClick.AddListener(this.OnBtnChangeTextClicked);
        }
        protected override void OnDestroy() {
            this.tabs.ForEach((vd) => {
                vd.OnDestroy();
            });
            this.tabs.Clear();            
        }
        public void OnBtnExitClicked() {
            this.CloseSelf();
        }
        protected override void OnOpened() {
            this.ls = new List<CSVTaskLevelConfine.Data>(CSVTaskLevelConfine.Instance.GetAll());
            this.ls.RemoveAll((t) => {
                return !t.active;
            });

            Sys_Ini.Instance.Get<IniElement_IntArray>(913, out IniElement_IntArray array);
            this.wordList = array.value;

            this.OnBtnChangeTextClicked();
            this.tabs.TryBuildOrRefresh(this.proto, this.protoParent, this.ls.Count, this.OnVdRefresh, this.OnVdInit);
        }

        private void OnVdInit(Tab vd, int index) {
            vd.Register(this);
        }
        private void OnVdRefresh(Tab vd, int index) {
            vd.Refresh(this.ls[index]);
        }

        #region 实现接口
        public void OnBtnGotoClicked(CSVTaskLevelConfine.Data csv) {
            this.CloseSelf();

            CSVOpenUi.Data csvOpenUI = CSVOpenUi.Instance.GetConfData(csv.Uiid);
            if (csvOpenUI != null) {
                if (csvOpenUI.Uiid == (uint)EUIID.UI_DailyActivites) {
                    Sys_Daily.Instance.SkipToDailyForMainTask();
                }
                else {
                    UIManager.OpenUI((EUIID)csvOpenUI.Uiid, false, csvOpenUI.ui_para);
                }
            }
        }

        private int lastIndex = 0;
        private void OnBtnChangeTextClicked() {
            int index = 0;
            if (this.wordList.Length > 1) {
                index = Random.Range(0, this.wordList.Length);
                while (this.lastIndex == index) {
                    index = Random.Range(0, this.wordList.Length);
                }

                this.lastIndex = index;
            }

            TextHelper.SetText(this.text, (uint)this.wordList[index]);
        }
        #endregion
    }
}