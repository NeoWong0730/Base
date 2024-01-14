using System.Collections.Generic;
using Lib.Core;
using Logic;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

// 家族资源战矿区刷新
public class UI_FamilyResBattleCDRes : UIBase, UI_FamilyResBattleCDRes.Layout.IListener {
    public class Res : UIComponent {
        public Image icon;
        public Text name;
        public CP_SliderLerp slider;
        public Text percent;
        public CDText remainTime;

        protected override void Loaded() {
            this.icon = this.transform.Find("Image_Icon").GetComponent<Image>();
            this.name = this.transform.Find("Text").GetComponent<Text>();
            this.slider = this.transform.Find("Image_ProcessBG").GetComponent<CP_SliderLerp>();
            this.percent = this.transform.Find("Image_ProcessBG/Text_Num").GetComponent<Text>();

            this.remainTime = this.transform.Find("Text_Time").GetComponent<CDText>();
            this.remainTime.onTimeRefresh = this.OnTimeRefresh;
        }

        public CSVFamilyResBattleResParameter.Data csvRes;
        public void Refresh(uint resId, Sys_FamilyResBattle.Res info) {
            this.csvRes = CSVFamilyResBattleResParameter.Instance.GetConfData(resId);
            var max = Sys_FamilyResBattle.Instance.allRes[resId].max;

            ImageHelper.SetIcon(this.icon, this.csvRes.IconID, true);
            TextHelper.SetText(this.name, this.csvRes.NameID);

            if (info == null) {
                this.slider.Refresh(0f);
                TextHelper.SetText(this.percent, string.Format("{0}/{1}", "0", max.ToString()));
                this.remainTime.Begin(0f);
            }
            else {
                this.slider.Refresh(1f * info.leftCount / max);
                TextHelper.SetText(this.percent, string.Format("{0}/{1}", info.leftCount.ToString(), max.ToString()));
                long diff = info.nextRefreshTime - Sys_Time.Instance.GetServerTime();
                this.remainTime.Begin(diff);
            }
        }

        private void OnTimeRefresh(Text text, float time, bool isEnd) {
            if (isEnd) {
                TextHelper.SetText(text, "00:00:00");
            }
            else {
                var t = Mathf.Round(time);
                var s = LanguageHelper.TimeToString((uint)t, LanguageHelper.TimeFormat.Type_1);
                TextHelper.SetText(text, s);
            }
        }
    }

    public class Layout : LayoutBase {
        public Button btnExit;
        public GameObject proto;

        public void Parse(GameObject root) {
            this.Set(root);

            this.proto = this.transform.Find("Animator/Scroll View/Viewport/Grid/Image_Item").gameObject;
            this.btnExit = this.transform.Find("close").GetComponent<Button>();
        }

        public void RegisterEvents(IListener listener) {
            this.btnExit.onClick.AddListener(listener.OnBtnClicked);
        }

        public interface IListener {
            void OnBtnClicked();
        }
    }

    public Layout layout = new Layout();
    public COWVd<Res> vds = new COWVd<Res>();
    public List<Sys_FamilyResBattle.Res> resInfos = new List<Sys_FamilyResBattle.Res>();
    private Timer timerRefresh;

    protected override void OnLoaded() {
        this.layout.Parse(this.gameObject);
        this.layout.RegisterEvents(this);
    }

    protected override void OnShow() {
        this.RefreshAll();
    }
    protected override void OnOpened() {
        this.resInfos.Clear();
        foreach (var kvp in Sys_FamilyResBattle.Instance.allRes) {
            this.resInfos.Add(kvp.Value);
        }

        this.timerRefresh?.Cancel();
        // 强制10s刷新一次，因为cdtext内部有误差，而且很大
        this.timerRefresh = Timer.RegisterOrReuse(ref this.timerRefresh, 10f, this.RefreshAll, null, true);

        this.RefreshAll();
    }
    protected override void OnDestroy() {
        this.timerRefresh?.Cancel();
        this.vds.Clear();
    }

    public void OnBtnClicked() {
        this.CloseSelf();
    }
    public void RefreshAll() {
        if (!Sys_FamilyResBattle.Instance.InFamilyBattle) {
            return;
        }

        this.vds.TryBuildOrRefresh(this.layout.proto, this.layout.proto.transform.parent, this.resInfos.Count, this._OnRefresh);
    }

    private void _OnRefresh(Res vd, int index) {
        var info = 0 <= index && index < this.resInfos.Count ? this.resInfos[index] : null;
        vd.Refresh(this.resInfos[index].resId, info);
    }

    #region 事件监听
    protected override void ProcessEvents(bool toRegister) {
        Sys_FamilyResBattle.Instance.eventEmitter.Handle(Sys_FamilyResBattle.EEvents.OnResChanged, this.OnResChanged, toRegister);
        Sys_Time.Instance.eventEmitter.Handle<uint, uint>(Sys_Time.EEvents.OnTimeNtf, this.OnTimeNtf, toRegister);
    }

    private void OnResChanged() {
        this.RefreshAll();
    }
    private void OnTimeNtf(uint oldTime, uint newTime) {
        this.RefreshAll();
    }
    #endregion
}