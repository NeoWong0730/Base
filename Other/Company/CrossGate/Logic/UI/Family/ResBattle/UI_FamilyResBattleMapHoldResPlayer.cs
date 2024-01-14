using Logic;
using Logic.Core;
using Packet;
using Table;
using UnityEngine;
using UnityEngine.UI;

// 家族资源战地图
public class UI_FamilyResBattleMapHoldResPlayer : UIBase, UI_FamilyResBattleMapHoldResPlayer.Layout.IListener {
    public class Layout : LayoutBase {
        public Image icon;
        public Image iconFrame;
        public Text name;

        public Text familyName;
        public Text occup;
        public Text level;
        public Text score;

        public Button btnWatch;

        public void Parse(GameObject root) {
            this.Set(root);

            this.icon = this.transform.Find("Animator/View_ExploreDetail/Image_Title/Head").GetComponent<Image>();
            this.iconFrame = this.transform.Find("Animator/View_ExploreDetail/Image_Title/Head/Image_Before_Frame").GetComponent<Image>();
            this.name = this.transform.Find("Animator/View_ExploreDetail/Image_Title/Text").GetComponent<Text>();

            this.familyName = this.transform.Find("Animator/View_ExploreDetail/Text1/XXX").GetComponent<Text>();
            this.occup = this.transform.Find("Animator/View_ExploreDetail/Text2/XXX").GetComponent<Text>();
            this.level = this.transform.Find("Animator/View_ExploreDetail/Text3/XXX").GetComponent<Text>();
            this.score = this.transform.Find("Animator/View_ExploreDetail/Text4/XXX").GetComponent<Text>();
            this.btnWatch = this.transform.Find("Animator/View_ExploreDetail/Btn_01").GetComponent<Button>();
        }

        public void RegisterEvents(IListener listener) {
            this.btnWatch.onClick.AddListener(listener.OnBtnWatchClicked);
        }

        public interface IListener {
            void OnBtnWatchClicked();
        }
    }

    public Layout layout = new Layout();
    public BattleRoleMiniMapData miniInfo;

    protected override void OnLoaded() {
        this.layout.Parse(this.gameObject);
        this.layout.RegisterEvents(this);
    }

    // 传入角色相关信息
    protected override void OnOpen(object arg) {
        this.miniInfo = arg as BattleRoleMiniMapData;
    }

    protected override void OnOpened() {
        uint iconId = CharacterHelper.getHeadID(this.miniInfo.Extra.HeroId, this.miniInfo.Extra.RoleHead);
        ImageHelper.SetIcon(this.layout.icon, iconId);
        iconId = CharacterHelper.getHeadFrameID(this.miniInfo.Extra.RoleHeadFrame);
        ImageHelper.SetIcon(this.layout.iconFrame, iconId);

        this.layout.name.text = "Null";
        this.layout.familyName.text = "Null";
        this.layout.score.text = "Null";

        ulong roleId = this.miniInfo.RoleId;
        if (Sys_FamilyResBattle.Instance.redRoles.TryGetValue(roleId, out BattleRoleMapData role) || Sys_FamilyResBattle.Instance.blueRoles.TryGetValue(roleId, out role)) {
            TextHelper.SetText(this.layout.name, role.RoleName.ToStringUtf8());
            BattleGuildMapData guild = Sys_FamilyResBattle.Instance.GetFamilyByRoleId(roleId);
            if (guild != null) {
                TextHelper.SetText(this.layout.familyName, guild.GuildName.ToStringUtf8());
                TextHelper.SetText(this.layout.score, role.Score.ToString());
            }
        }

        TextHelper.SetText(this.layout.level, this.miniInfo.Extra.Level.ToString());
        var csv = CSVFamilyPostAuthority.Instance.GetConfData(this.miniInfo.Extra.Position);
        if (csv != null) {
            TextHelper.SetText(this.layout.occup, csv.PostName);
        }
        else {
            this.layout.occup.text = "Null";
        }

        ButtonHelper.Enable(this.layout.btnWatch, this.miniInfo.Extra.Fighting);
    }

    public void OnBtnWatchClicked() {
        this.CloseSelf();
        UIManager.CloseUI(EUIID.UI_FamilyResBattleMap);

        Sys_Fight.Instance.SendCmdBattleWatchReq(this.miniInfo.RoleId);
    }
}