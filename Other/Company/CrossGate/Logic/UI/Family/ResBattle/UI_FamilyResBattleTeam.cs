using System.Collections.Generic;
using Logic;
using Logic.Core;
using Net;
using Packet;
using Table;
using UnityEngine;
using UnityEngine.UI;

// 家族资源战 组队
public class UI_FamilyResBattleTeam : UIBase, UI_FamilyResBattleTeam.Layout.IListener {
    public class Tab : UISelectableElement {
        public CP_Toggle toggle;
        public Text text;
        public Text text1;

        protected override void Loaded() {
            this.toggle = this.transform.GetComponent<CP_Toggle>();
            this.text = this.transform.Find("ItemBig/Btn_Menu_Dark/Text_Menu_Dark").GetComponent<Text>();
            this.text1 = this.transform.Find("ItemBig/Btn_Menu_Light/Text_Menu_Dark").GetComponent<Text>();

            this.toggle.onValueChanged.AddListener(this.Switch);
        }

        public new void Refresh() {
            var csv = CSVFamilyResBattleTeam.Instance.GetConfData((uint)this.id);
            if (csv != null) {
                this.Show();
                TextHelper.SetText(this.text, csv.play_name);
                this.text1.text = this.text.text;
            }
            else {
                this.Hide();
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
    public class Line : UIComponent {
        public Image headIcon;
        public Image headFrame;
        public Text name;
        public Text teamDesc;
        public Text memberCount;
        public Image career;
        public Text careerText;
        public Text lv;

        public Button btnApply;
        public Button btnInvite;

        private int index;

        protected override void Loaded() {
            this.headIcon = this.transform.Find("Image_BG/Head").GetComponent<Image>();
            this.headFrame = this.transform.Find("Image_BG/Head/Image_Before_Frame").GetComponent<Image>();
            this.name = this.transform.Find("Text_Name").GetComponent<Text>();
            this.teamDesc = this.transform.Find("Text_Task").GetComponent<Text>();
            this.memberCount = this.transform.Find("Text_Task/Text_Number").GetComponent<Text>();

            this.career = this.transform.Find("Image_Prop").GetComponent<Image>();
            this.careerText = this.transform.Find("Text_Profession").GetComponent<Text>();

            this.lv = this.transform.Find("Text_Number").GetComponent<Text>();

            this.btnApply = this.transform.Find("Button_Apply").GetComponent<Button>();
            this.btnApply.onClick.AddListener(this.OnBtnApplyClicked);
            this.btnInvite = this.transform.Find("Button_Invite").GetComponent<Button>();
            this.btnInvite.onClick.AddListener(this.OnBtnInviteClicked);
        }

        public BattleMatchTeamData teamInfo;
        public BattleMatchRoleData roleInfo;

        private void RefreshRole(BattleMatchRoleData info) {
            uint iconId = CharacterHelper.getHeadID(info.HeroId, info.RoleHead);
            ImageHelper.SetIcon(this.headIcon, iconId);
            iconId = CharacterHelper.getHeadFrameID(info.RoleHeadFrame);
            ImageHelper.SetIcon(this.headFrame, iconId);

            if (Sys_FamilyResBattle.Instance.redRoles.TryGetValue(info.RoleId, out var role) || Sys_FamilyResBattle.Instance.blueRoles.TryGetValue(info.RoleId, out role)) {
                TextHelper.SetText(this.name, role.RoleName.ToStringUtf8());
            }
            else {
                TextHelper.SetText(this.name, "NotExist");
            }
            TextHelper.SetText(this.lv, info.Level.ToString());

            TextHelper.SetText(this.teamDesc, 3230000001);

            // 职业
            TextHelper.SetText(this.careerText, OccupationHelper.GetTextID(info.Career, 0));
            uint careerIconId = OccupationHelper.GetTeamIconID(info.Career);
            ImageHelper.SetIcon(this.career, careerIconId);

        }
        public void Refresh(object arg, int index) {
            this.index = index;

            if (index == 0) {
                this.btnApply.gameObject.SetActive(true);
                this.btnInvite.gameObject.SetActive(false);

                this.teamInfo = arg as BattleMatchTeamData;
                this.RefreshRole(this.teamInfo.LeaderData);
                TextHelper.SetText(this.memberCount, string.Format("({0}/{1})", this.teamInfo.MemberCount.ToString(), "5"));
            }
            else if (index == 1) {
                this.btnInvite.gameObject.SetActive(true);
                this.btnApply.gameObject.SetActive(false);

                this.roleInfo = arg as BattleMatchRoleData;
                this.RefreshRole(this.roleInfo);
                TextHelper.SetText(this.memberCount, "(1/1)");
            }
        }

        private void OnBtnApplyClicked() {
            Sys_Team.Instance.ApplyJoinTeam(this.teamInfo.TeamId, Sys_Role.Instance.RoleId);
        }
        private void OnBtnInviteClicked() {
            Sys_Team.Instance.InvitedOther(this.roleInfo.RoleId);
        }
    }

    public class Layout : LayoutBase {
        public GameObject tabProto;
        public Transform tabProtoParent;

        public GameObject noList;
        public InfinityGrid hasList;

        public Button btnCreateTeam;
        public Button btnRefresh;

        public void Parse(GameObject root) {
            this.Set(root);

            this.tabProto = this.transform.Find("Animator/View_Apply/Middle_Menu/Scroll_View/TabList/Proto").gameObject;
            this.tabProtoParent = this.tabProto.transform.parent;

            this.hasList = this.transform.Find("Animator/View_Apply/Right_Content/Scroll_View").GetComponent<InfinityGrid>();
            this.noList = this.transform.Find("Animator/View_Apply/Middle_Menu/Empty").gameObject;

            this.btnCreateTeam = this.transform.Find("Animator/View_Apply/Right_Content/Button_Create").GetComponent<Button>();
            this.btnRefresh = this.transform.Find("Animator/View_Apply/Right_Content/Button_Refersh").GetComponent<Button>();
        }

        public void RegisterEvents(IListener listener) {
            this.btnCreateTeam.onClick.AddListener(listener.OnBtnCreateClicked);
            this.btnRefresh.onClick.AddListener(listener.OnBtnRefreshClicked);

            this.hasList.onCreateCell += listener.OnCreateCell;
            this.hasList.onCellChange += listener.OnCellChange;
        }

        public interface IListener {
            void OnBtnCreateClicked();
            void OnBtnRefreshClicked();
            void OnCreateCell(InfinityGridCell cell);
            void OnCellChange(InfinityGridCell cell, int index);
        }
    }

    public Layout layout = new Layout();
    public int currentIndex = 0;
    public List<uint> tabIds = new List<uint>(2);
    public UIElementCollector<Tab> tabVds = new UIElementCollector<Tab>();

    public List<BattleMatchTeamData> teams = new List<BattleMatchTeamData>();
    public List<BattleMatchRoleData> roles = new List<BattleMatchRoleData>();

    protected override void OnLoaded() {
        this.layout.Parse(this.gameObject);
        this.layout.RegisterEvents(this);
    }

    protected override void OnOpen(object arg) {
        this.tabIds.Clear();

        //for (int i = 0, length = CSVFamilyResBattleTeam.Instance.Count; i < length; ++i) {
        //    var t = CSVFamilyResBattleTeam.Instance[i];
        //    this.tabIds.Add(t.id);
        //}
        this.tabIds.AddRange(CSVFamilyResBattleTeam.Instance.GetKeys());

        this.teams.Clear();
        this.roles.Clear();
    }
    protected override void OnDestroy() {
        this.tabVds.Clear();
    }

    protected override void OnShow()
    {
        this.tabVds.BuildOrRefresh<uint>(this.layout.tabProto, this.layout.tabProtoParent, this.tabIds, (vd, id, indexOfVdList) =>
        {
            vd.SetUniqueId((int)id);
            vd.SetSelectedAction((innerId, force) =>
            {
                this.currentIndex = indexOfVdList;
                this.OnBtnRefreshClicked();
                this.RefreshRight(this.currentIndex);
            });
            vd.Refresh();
        });

        // 默认选中Tab
        if (this.tabVds.CorrectId(ref this.currentIndex, this.tabIds))
        {
            if (this.tabVds.TryGetVdById(this.currentIndex, out var vd))
            {
                vd.SetSelected(true, true);
            }
        }
        else
        {
            Debug.LogError("Can't run here!");
        }
    }

    private void RefreshRight(int index) {
        if (index != this.currentIndex) {
            return;
        }
        bool has = false;
        if (this.currentIndex == 0) {
            this.layout.hasList.CellCount = this.teams.Count;

            has = this.teams.Count > 0;
        }
        else if (this.currentIndex == 1) {
            this.layout.hasList.CellCount = this.roles.Count;

            has = this.roles.Count > 0;
        }

        if (has) {
            this.layout.hasList.gameObject.SetActive(true);
            this.layout.noList.SetActive(false);
            this.layout.hasList.ForceRefreshActiveCell();
        }
        else {
            this.layout.hasList.gameObject.SetActive(false);
            this.layout.noList.SetActive(true);
        }
    }

    #region 事件处理
    public void OnBtnCreateClicked() {
        ulong roleid = Sys_Role.Instance.Role.RoleId;
        Sys_Team.Instance.ApplyCreateTeam(roleid);

        UIManager.HitButton(EUIID.UI_Team_Member, "CreateTeam");
    }

    public void OnBtnRefreshClicked() {
        if (this.currentIndex == 0) {
            this.MatchTeamDataReq();
        }
        else if (this.currentIndex == 1) {
            this.MatchRoleDataReq();
        }
    }

    public void OnCreateCell(InfinityGridCell cell) {
        Line entry = new Line();
        entry.Init(cell.mRootTransform);
        cell.BindUserData(entry);
    }

    public void OnCellChange(InfinityGridCell cell, int index) {
        var entry = cell.mUserData as Line;
        if (this.currentIndex == 0) {
            entry.Refresh(this.teams[index], this.currentIndex);
        }
        else if (this.currentIndex == 1) {
            entry.Refresh(this.roles[index], this.currentIndex);
        }
    }

    protected override void ProcessEvents(bool toRegister) {
        Sys_Team.Instance.eventEmitter.Handle(Sys_Team.EEvents.TargetList, this.OnRefreshList, toRegister);
        Sys_Team.Instance.eventEmitter.Handle<uint, uint, bool>(Sys_Team.EEvents.TargetMatching, this.OnMatching, toRegister);
        Sys_Team.Instance.eventEmitter.Handle(Sys_Team.EEvents.HaveTeam, this.OnHaveTeam, toRegister);

        if (toRegister) {
            EventDispatcher.Instance.AddEventListener((ushort)CmdGuildBattle.MatchTeamDataReq, (ushort)CmdGuildBattle.MatchTeamDataRes, this.OnRecvMatchTeamDataRes, CmdGuildBattleMatchTeamDataRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdGuildBattle.MatchRoleDataReq, (ushort)CmdGuildBattle.MatchRoleDataRes, this.OnRecvMatchRoleDataRes, CmdGuildBattleMatchRoleDataRes.Parser);
        }
        else {
            EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuildBattle.MatchTeamDataRes, this.OnRecvMatchTeamDataRes);
            EventDispatcher.Instance.RemoveEventListener((ushort)CmdGuildBattle.MatchRoleDataRes, this.OnRecvMatchRoleDataRes);
        }
    }
    private void OnRefreshList() {
        this.RefreshRight(this.currentIndex);
    }

    private void OnMatching(uint lastid, uint id, bool isMatching) {
        this.RefreshRight(this.currentIndex);
    }

    private void OnHaveTeam() {
        this.RefreshRight(this.currentIndex);
    }

    public void MatchTeamDataReq() {
        CmdGuildBattleMatchTeamDataReq req = new CmdGuildBattleMatchTeamDataReq();
        NetClient.Instance.SendMessage((ushort)CmdGuildBattle.MatchTeamDataReq, req);

    }
    private void OnRecvMatchTeamDataRes(NetMsg msg) {
        CmdGuildBattleMatchTeamDataRes res = NetMsgUtil.Deserialize<CmdGuildBattleMatchTeamDataRes>(CmdGuildBattleMatchTeamDataRes.Parser, msg);

        this.teams.Clear();
        for (int i = 0, length = res.Teams.Count; i < length; ++i) {
            var r = res.Teams[i];
            if (r.LeaderData != null && r.LeaderData.RoleId != Sys_Role.Instance.RoleId) {
                this.teams.Add(r);
            }
        }

        this.RefreshRight(0);
    }

    public void MatchRoleDataReq() {
        CmdGuildBattleMatchRoleDataReq req = new CmdGuildBattleMatchRoleDataReq();
        NetClient.Instance.SendMessage((ushort)CmdGuildBattle.MatchRoleDataReq, req);
    }

    private void OnRecvMatchRoleDataRes(NetMsg msg) {
        CmdGuildBattleMatchRoleDataRes res = NetMsgUtil.Deserialize<CmdGuildBattleMatchRoleDataRes>(CmdGuildBattleMatchRoleDataRes.Parser, msg);

        this.roles.Clear();
        for (int i = 0, length = res.Roles.Count; i < length; ++i) {
            var r = res.Roles[i];
            if (r.RoleId != Sys_Role.Instance.RoleId) {
                this.roles.Add(r);
            }
        }

        this.RefreshRight(1);
    }
    #endregion
}