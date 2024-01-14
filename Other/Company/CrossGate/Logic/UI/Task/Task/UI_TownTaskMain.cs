using System.Collections.Generic;
using Lib.Core;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public class UI_TownTaskMain : UIBase, UI_TownTaskMain.Layout.IListener, UI_TownTaskMain.Tab.IListener {
        public class Tab : UIComponent {
            public Text townName;
            public RawImage townIcon;
            public Text taskNum;
            public Text attrbuteLevel;
            public Button btnGoto;
            public Transform grayMask;

            public Transform redDot;

            public IListener listener;

            public uint townId;

            protected override void Loaded() {
                this.townName = this.transform.Find("Text1").GetComponent<Text>();
                this.townIcon = this.transform.Find("Image2").GetComponent<RawImage>();
                this.taskNum = this.transform.Find("Text2/Text").GetComponent<Text>();
                this.attrbuteLevel = this.transform.Find("Text3/Text").GetComponent<Text>();
                this.btnGoto = this.transform.Find("Button_Go").GetComponent<Button>();

                this.redDot = this.transform.Find("RedDot");

                this.grayMask = this.transform.Find("Dark");

                this.btnGoto.onClick.AddListener(this.OnBtnGoto);
            }

            public void Refresh(uint townId, int index) {
                this.townId = townId;
                if (Sys_TownTask.Instance.towns.TryGetValue(townId, out Sys_TownTask.Town town)) {
                    TextHelper.SetText(this.townName, town.csvFavority.PlaceName);
                    ImageHelper.SetTexture(townIcon, town.csvFavority.TownIcon);
                    TextHelper.SetText(this.attrbuteLevel, town.contributeLevel.ToString());
                    int canDoCount = town.canDoCount;
                    this.taskNum.text = $"{canDoCount.ToString()}/{town.max.ToString()}";

                    bool allCantDo = canDoCount <= 0;
                    this.grayMask.gameObject.SetActive(allCantDo);
                    // 红点
                    this.redDot.gameObject.SetActive(town.CanAnyGot);
                }
                else {
                    // error
                }
            }

            private void OnBtnGoto() {
                this.listener?.OnBtnGoto(this.townId);
            }
            
            public interface IListener {
                void OnBtnGoto(uint townId);
            }
        }

        public class Layout : UILayoutBase {
            public Text favorityValue;
            public Button btnFavorityHelp;

            public CP_PageSwitcher pageSwitcher;
            public Text pageIndexer;

            public Transform tabProto;
            public Transform tabProtoParent;
            public COWVd<Tab> vds = new COWVd<Tab>();

            public void Parse(GameObject root) {
                this.Init(root);

                this.favorityValue = this.transform.Find("Animator/Top/Text").GetComponent<Text>();
                this.btnFavorityHelp = this.transform.Find("Animator/Top/Button_Tips").GetComponent<Button>();
                this.pageSwitcher = this.transform.Find("Animator/View_Content").GetComponent<CP_PageSwitcher>();
                this.pageIndexer = this.transform.Find("Animator/View_Content/Image_Number/Text").GetComponent<Text>();
                this.tabProto = this.transform.Find("Animator/View_Content/Scroll_View_Gem/Viewport/Content/Item");
                this.tabProtoParent = this.transform.Find("Animator/View_Content/Scroll_View_Gem/Viewport/Content");
            }

            public void RegisterEvents(IListener listener) {
                this.btnFavorityHelp.onClick.AddListener(listener.OnBtnHelpClicked);
                this.pageSwitcher.onExec += listener.OnPageSwicth;
            }

            public interface IListener {
                void OnBtnHelpClicked();
                void OnPageSwicth(int pageIndex, int startIndex, int range);
            }
        }

        public Layout layout = new Layout();
        public List<uint> townIds = new List<uint>();

        protected override void OnLoaded() {
            this.layout.Parse(this.gameObject);
            this.layout.RegisterEvents(this);
        }

        #region 事件监听

        protected override void ProcessEvents(bool toRegister) {
            // 好感度变化
            // 城镇npc激活变化
            // 可做条件变化
            // 任务完成变化
            // 好感度等级变化
            Sys_NPCFavorability.Instance.eventEmitter.Handle(Sys_NPCFavorability.EEvents.OnPlayerFavorabilityChanged, this.OnPlayerFavorabilityChanged, toRegister);
            // 为了在UI打开的时候不会刷新列表，所以只监听好感度变化即可。
        }

        private void OnPlayerFavorabilityChanged() {
            if (this.gameObject != null && this.gameObject.activeInHierarchy) {
                this.RefreshFavority();
            }
        }

        #endregion

        protected override void OnShow() {
            this.RefreshAll();
            this.RefreshFavority();
        }

        protected override void OnDestroy() {
            layout.vds.Clear();
        }

        public void RefreshAll() {
            this.townIds = Sys_TownTask.Instance.GetTowns(false, true);
            
            // 每页4个item
            this.layout.pageSwitcher.SetCount(this.townIds.Count);

            if (this.townIds.Count > 0) {
                this.layout.pageSwitcher.SetCurrentIndex(0);
                this.layout.pageSwitcher.Exec();
            }
        }

        public void RefreshFavority() {
            TextHelper.SetText(this.layout.favorityValue, 12240, Sys_NPCFavorability.Instance.Favorability.ToString());
        }

        public void OnBtnHelpClicked() {
            UIManager.OpenUI(EUIID.UI_FavorabilityTip);
        }
        
        private int pageStartIndex;
        public void OnPageSwicth(int pageIndex, int startIndex, int range) {
            pageStartIndex = startIndex;
            layout.vds.TryBuildOrRefresh(layout.tabProto.gameObject, layout.tabProtoParent, range, OnRefresh, OnInit);
            TextHelper.SetText(layout.pageIndexer, $"{(pageIndex + 1).ToString()}/{layout.pageSwitcher.PageCount.ToString()}");
        }

        public void OnInit(Tab vd, int index) {
            vd.listener = this;
        }
        
        public void OnRefresh(Tab vd, int index) {
            vd.Refresh(townIds[index + pageStartIndex], index);
        }

        public void OnBtnGoto(uint townId) {
            if (Sys_TownTask.Instance.towns.TryGetValue(townId, out Sys_TownTask.Town town)) {
                this.CloseSelf();
                ActionCtrl.Instance.MoveToTargetNPCAndInteractive(town.csvFavority.BulletinId);
            }
        }
    }
}