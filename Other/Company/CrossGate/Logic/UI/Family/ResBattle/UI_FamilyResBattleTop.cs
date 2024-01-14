using System;
using System.Collections.Generic;
using Lib.Core;
using Logic;
using Logic.Core;
using Packet;
using Table;
using UnityEngine;
using UnityEngine.UI;
using pbc = global::Google.Protobuf.Collections;

// 家族资源战top
public class UI_FamilyResBattleTop : UIBase, UI_FamilyResBattleTop.Layout.IListener {
    public class Layout : LayoutBase {
        public RectTransform redSlider;
        public Text redRes;
        public RectTransform leftSliderBg;

        public RectTransform blueSlider;
        public Text blueRes;

        public Text myRes;
        public Text myRank;
        public CDText remainTime;

        public Button btnDetail;

        public Text resName;
        public CDText resRemainTime;
        public Button btnResDetail;

        public GameObject upNode;
        public Button btnShow;
        public Transform arrow;
        public Transform teamList;

        public void Parse(GameObject root) {
            this.Set(root);

            this.upNode = this.transform.Find("Animator/Up").gameObject;
            this.redSlider = this.transform.Find("Animator/Up/Image_BG/Left/Slider01").GetComponent<RectTransform>();
            this.redRes = this.transform.Find("Animator/Up/Image_BG/Left").GetComponent<Text>();
            this.leftSliderBg = this.transform.Find("Animator/Up/Image_BG/Left/BG").GetComponent<RectTransform>();

            this.blueSlider = this.transform.Find("Animator/Up/Image_BG/Right/Slider01").GetComponent<RectTransform>();
            this.blueRes = this.transform.Find("Animator/Up/Image_BG/Right").GetComponent<Text>();

            this.myRes = this.transform.Find("Animator/Up/Down/MyRes/XXX").GetComponent<Text>();
            this.myRank = this.transform.Find("Animator/Up/Down/MyRank/XXX").GetComponent<Text>();
            this.remainTime = this.transform.Find("Animator/Up/Down/RemainTime/XXX").GetComponent<CDText>();

            this.btnDetail = this.transform.Find("Animator/Up/Down/Btn01").GetComponent<Button>();
            this.btnResDetail = this.transform.Find("Animator/View_FamilyResBattle/Button").GetComponent<Button>();

            this.resRemainTime = this.transform.Find("Animator/View_FamilyResBattle/Text_Time").GetComponent<CDText>();
            this.resName = this.transform.Find("Animator/View_FamilyResBattle/Text_Active").GetComponent<Text>();

            this.btnShow = this.transform.Find("Animator/Up/Btn_hexin").GetComponent<Button>();
            this.arrow = this.transform.Find("Animator/Up/Btn_hexin/Image_Arrow");
            this.teamList = this.transform.Find("Animator/Up/TeamList");
        }

        public void RegisterEvents(IListener listener) {
            this.btnDetail.onClick.AddListener(listener.OnBtnDetailClicked);
            this.btnResDetail.onClick.AddListener(listener.OnBtnResDetailClicked);
            this.btnShow.onClick.AddListener(listener.OnBtnShowClicked);

            this.remainTime.onTimeRefresh = listener.OnTimeRefresh;
            this.resRemainTime.onTimeRefresh = listener.OnTimeRefresh;
        }

        public interface IListener {
            void OnBtnDetailClicked();
            void OnBtnResDetailClicked();
            void OnBtnShowClicked();
            void OnTimeRefresh(Text text, float time, bool isEnd);
        }
    }

    public class TeamMember : UIComponent {
        public Image resIcon;
        public Transform fightingFlag;

        public Image heroIcon;

        // public Image heroFrameIcon;
        public Text heroName;

        public Button btn;

        protected override void Loaded() {
            resIcon = transform.Find("Btn_Head/Res").GetComponent<Image>();
            fightingFlag = transform.Find("Btn_Head/Fighting");
            heroIcon = transform.Find("Btn_Head/Icon").GetComponent<Image>();
            // heroFrameIcon = transform.Find("Btn_Head/HeadFrame").GetComponent<Image>();
            heroName = transform.Find("Text_Name").GetComponent<Text>();

            btn = transform.Find("Btn_Head").GetComponent<Button>();
            btn.onClick.AddListener(OnBtnClicked);
        }

        private void OnBtnClicked() {
            UIManager.OpenUI(EUIID.UI_FamilyResBattleTeamMember, false, new ArgTransfer(guildId, team));
        }

        private BattleCoreTeamMapData team;
        private ulong guildId;

        public void Refresh(BattleCoreTeamMapData team, ulong guildId, bool isRed) {
            this.team = team;
            this.guildId = guildId;

            var captain = team.Roles[0];
            var roleId = captain.RoleId;
            BattleRoleMapData mapData = null;
            if (isRed) {
                Sys_FamilyResBattle.Instance.redRoles.TryGetValue(roleId, out mapData);
            }
            else {
                Sys_FamilyResBattle.Instance.blueRoles.TryGetValue(roleId, out mapData);
            }

            // 名字
            if (mapData != null) {
                TextHelper.SetText(this.heroName, mapData.RoleName.ToStringUtf8());
            }
            else {
                TextHelper.SetText(this.heroName, "");
            }

            // 头像
            uint iconId = CharacterHelper.getHeadID(captain.HeroId, 0);
            ImageHelper.SetIcon(this.heroIcon, iconId);

            // 携带资源
            var csv = CSVFamilyResBattleResParameter.Instance.GetConfData(team.Resource);
            if (csv != null) {
                this.resIcon.gameObject.SetActive(true);
                ImageHelper.SetIcon(this.resIcon, csv.IconID);
            }
            else {
                this.resIcon.gameObject.SetActive(false);
            }

            // 战斗标记
            fightingFlag.gameObject.SetActive(team.Fighting);
        }
    }

    public class TeamList : UIComponent {
        public COWVd<TeamMember> left = new COWVd<TeamMember>();
        public COWVd<TeamMember> right = new COWVd<TeamMember>();

        public List<BattleCoreTeamMapData> leftTeam;
        public List<BattleCoreTeamMapData> rightTeam;

        public GameObject lg;
        public GameObject rg;
        public Transform leftParent;
        public Transform rightParent;

        protected override void Loaded() {
            lg = transform.Find("List_Left/Item").gameObject;
            rg = transform.Find("List_Right/Item").gameObject;
            leftParent = transform.Find("List_Left");
            rightParent = transform.Find("List_Right");
        }

        public void Refresh() {
            if (Sys_FamilyResBattle.Instance.guildTeams.TryGetValue(Sys_FamilyResBattle.Instance.redFamilyId, out leftTeam) &&
                Sys_FamilyResBattle.Instance.guildTeams.TryGetValue(Sys_FamilyResBattle.Instance.blueFamilyId, out rightTeam)) {

                // todo 按照rank排序
                left.TryBuildOrRefresh(lg, leftParent, leftTeam.Count, _OnLeftRefresh);
                right.TryBuildOrRefresh(rg, rightParent, rightTeam.Count, _OnRightRefresh);
            }
            else {
                left.TryBuildOrRefresh(lg, leftParent, 0, _OnLeftRefresh);
                right.TryBuildOrRefresh(rg, rightParent, 0, _OnRightRefresh);
            }
        }

        private void _OnLeftRefresh(TeamMember member, int index) {
            member.Refresh(leftTeam[index], Sys_FamilyResBattle.Instance.redFamilyId, true);
        }

        private void _OnRightRefresh(TeamMember member, int index) {
            member.Refresh(rightTeam[index], Sys_FamilyResBattle.Instance.blueFamilyId, false);
        }

        public override void OnDestroy() {
            left.Clear();
            right.Clear();
        }
    }

    public class ArgTransfer {
        public ulong roleid;
        public BattleCoreTeamMapData team;

        public ArgTransfer(ulong roleid, BattleCoreTeamMapData team) {
            this.roleid = roleid;
            this.team = team;
        }
    }

    public Layout layout = new Layout();
    private Timer timerRefresh;

    public TeamList teamList = new TeamList();

    protected override void OnLoaded() {
        this.layout.Parse(this.gameObject);
        this.layout.RegisterEvents(this);

        teamList.Init(layout.teamList);
    }

    protected override void OnShow() {
        this.RefreshAll();
    }

    protected override void OnOpened() {
        this.timerRefresh?.Cancel();
        // 强制10s刷新一次，因为cdtext内部有timescale的误差
        this.timerRefresh = Timer.RegisterOrReuse(ref this.timerRefresh, 10f, this.RefreshAll, null, true, true);
        this.RefreshAll();
    }

    protected override void OnDestroy() {
        this.timerRefresh?.Cancel();
        this.teamList.OnDestroy();
    }

    public void OnTimeRefresh(Text text, float time, bool isEnd) {
        if (isEnd) {
            TextHelper.SetText(text, "00:00:00");
        }
        else {
            var t = Mathf.Round(time);
            var s = LanguageHelper.TimeToString((uint) t, LanguageHelper.TimeFormat.Type_1);
            TextHelper.SetText(text, s);
        }
    }

    public void OnBtnDetailClicked() {
        UIManager.OpenUI(EUIID.UI_FamilyResBattleRank, false);
    }

    public void OnBtnResDetailClicked() {
        if (Sys_FamilyResBattle.Instance.stage == Sys_FamilyResBattle.EStage.ReadyBattle) {
        }
        else {
            UIManager.OpenUI(EUIID.UI_FamilyResBattleCDRes, false);
        }
    }

    public void OnBtnShowClicked() {
        Sys_FamilyResBattle.Instance.isFocusing = !Sys_FamilyResBattle.Instance.isFocusing;
        Sys_FamilyResBattle.Instance.ReqFocusTeamList();
        layout.teamList.gameObject.SetActive(Sys_FamilyResBattle.Instance.isFocusing);

        // 点击展开立即刷新的话，可能底层数据已经被clear导致UI展示空白，所以先用老的数据，然后等待server数据驱动UI刷新
        // if (Sys_FamilyResBattle.Instance.isFocusing) {
        //     teamList.Refresh();
        // }
    }

    // 资源进度条的长度
    public float LENGTH {
        get { return this.layout.leftSliderBg.sizeDelta.x; }
    }

    public void RefreshAll() {
        if (!Sys_FamilyResBattle.Instance.InFamilyBattle) {
            return;
        }

        Sys_FamilyResBattle.EStage stage = Sys_FamilyResBattle.Instance.stage;
        if (stage == Sys_FamilyResBattle.EStage.ReadyBattle) {
            this.layout.upNode.SetActive(false);
            TextHelper.SetText(this.layout.resName, CSVFamilyResBattleState.Instance.GetConfData((uint) stage).SrateText);

            long diff = Sys_FamilyResBattle.Instance.endTimeOfStage - Sys_Time.Instance.GetServerTime();
            this.layout.resRemainTime.Begin(diff);
        }
        else {
            this.layout.upNode.SetActive(true);

            uint total = Sys_FamilyResBattle.Instance.TotalRes(Sys_FamilyResBattle.Instance.redFamilyId);
            TextHelper.SetText(this.layout.redRes, 3230000060, total.ToString());
            var sizeDelta = this.layout.redSlider.sizeDelta;
            float width = Mathf.Lerp(0f, 1f, 1f * total / Sys_FamilyResBattle.Instance.battlewinMaxRes) * this.LENGTH;
            this.layout.redSlider.sizeDelta = new Vector2(width, sizeDelta.y);

            total = Sys_FamilyResBattle.Instance.TotalRes(Sys_FamilyResBattle.Instance.blueFamilyId);
            TextHelper.SetText(this.layout.blueRes, 3230000061, total.ToString());
            sizeDelta = this.layout.blueSlider.sizeDelta;
            width = Mathf.Lerp(0f, 1f, 1f * total / Sys_FamilyResBattle.Instance.battlewinMaxRes) * this.LENGTH;
            this.layout.blueSlider.sizeDelta = new Vector2(width, sizeDelta.y);

            Sys_FamilyResBattle.Instance.allRols.Sort((l, r) => {
                var rlt = (int) (r.Score - (long) l.Score);
                if (rlt == 0) {
                    rlt = (int) (l.RoleId - r.RoleId);
                }

                return rlt;
            });

            int index = Sys_FamilyResBattle.Instance.allRols.FindIndex(0, this._Find);
            if (index != -1) {
                BattleRoleMapData myInfo = Sys_FamilyResBattle.Instance.allRols[index];
                TextHelper.SetText(this.layout.myRes, myInfo.Score.ToString());
                TextHelper.SetText(this.layout.myRank, (index + 1).ToString());
            }
            else {
                TextHelper.SetText(this.layout.myRes, "0");
                TextHelper.SetText(this.layout.myRank, 3230000030u);
            }

            long diff = Sys_FamilyResBattle.Instance.endTimeOfStage - Sys_Time.Instance.GetServerTime();
            this.layout.remainTime.Begin(diff);

            uint resId = 1;
            var csvRes = CSVFamilyResBattleResParameter.Instance.GetConfData(resId);
            TextHelper.SetText(this.layout.resName, csvRes.NameID);

            long refreshTime = Sys_FamilyResBattle.Instance.allRes[resId].nextRefreshTime;
            diff = refreshTime - Sys_Time.Instance.GetServerTime();
            this.layout.resRemainTime.Begin(diff);
        }

        if (Sys_FamilyResBattle.Instance.isFocusing) {
            layout.teamList.gameObject.SetActive(true);
            layout.arrow.localEulerAngles = Vector3.zero;
            teamList.Refresh();
        }
        else {
            layout.teamList.gameObject.SetActive(false);
            layout.arrow.localEulerAngles = new Vector3(0f, 0f, 180f);
        }
    }

    private bool _Find(BattleRoleMapData info) {
        return info.RoleId == Sys_Role.Instance.RoleId;
    }

    #region 事件监听

    protected override void ProcessEvents(bool toRegister) {
        // 家族资源变化
        // 自己资源变化
        // 自己排名变化
        Sys_FamilyResBattle.Instance.eventEmitter.Handle(Sys_FamilyResBattle.EEvents.OnMyResChanged, this.OnMyResChanged, toRegister);
        Sys_FamilyResBattle.Instance.eventEmitter.Handle(Sys_FamilyResBattle.EEvents.OnResChanged, this.OnResChanged, toRegister);
        Sys_FamilyResBattle.Instance.eventEmitter.Handle<uint, uint, long>(Sys_FamilyResBattle.EEvents.OnStageChanged, this.OnStageChanged, toRegister);
        Sys_Time.Instance.eventEmitter.Handle<uint, uint>(Sys_Time.EEvents.OnTimeNtf, this.OnTimeNtf, toRegister);
        Sys_FamilyResBattle.Instance.eventEmitter.Handle<bool>(Sys_FamilyResBattle.EEvents.OnReceiveTeamOrRankChanged, OnReceiveTeamOrRankChanged, toRegister);
    }

    private void OnMyResChanged() {
        this.RefreshAll();
    }

    private void OnResChanged() {
        this.RefreshAll();
    }

    private void OnStageChanged(uint _, uint __, long ___) {
        this.RefreshAll();
    }

    private void OnTimeNtf(uint oldTime, uint newTime) {
        this.RefreshAll();
    }

    private void OnReceiveTeamOrRankChanged(bool _) {
        this.RefreshAll();
    }

    #endregion
}