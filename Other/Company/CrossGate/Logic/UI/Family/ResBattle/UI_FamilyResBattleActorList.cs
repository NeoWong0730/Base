using System.Collections.Generic;
using Logic;
using Logic.Core;
using Packet;
using Table;
using UnityEngine;
using UnityEngine.UI;

// 家族资源战
// 所有队长 和 独立作战 的人员
public class UI_FamilyResBattleActorList : UIBase, UI_FamilyResBattleActorList.Layout.IListener {
    public class Tab : UIComponent {
        public GameObject redMark;
        public GameObject blueMark;

        public Image resIcon;

        public Image charIcon;
        public Image charIconFrame;

        public Text charName;
        public Text lv;
        public Button btnTab;

        public Hero hero;

        protected override void Loaded() {
            this.redMark = this.transform.Find("Image_Mark01").gameObject;
            this.blueMark = this.transform.Find("Image_Mark02").gameObject;
            this.resIcon = this.transform.Find("Image_Resource").GetComponent<Image>();

            this.charIcon = this.transform.Find("Head").GetComponent<Image>();
            this.charIconFrame = this.transform.Find("Head/Image_Before_Frame").GetComponent<Image>();

            this.charName = this.transform.Find("Text").GetComponent<Text>();
            this.lv = this.transform.Find("Image_Icon/Text_Number").GetComponent<Text>();

            this.btnTab = this.transform.GetComponent<Button>();
            this.btnTab.onClick.AddListener(this.OnBtnClicked);
        }

        private void OnBtnClicked() {
            bool isRed = this.hero.familyResBattleComponent.isRed;
            if (isRed) {
                Sys_Role_Info.Instance.OpenRoleInfo(this.hero.UID, Sys_Role_Info.EType.FromAvatar);
            }
            else { // 攻击敌方
                Sys_FamilyResBattle.Instance.ReqAttack(this.hero, this.hero.uID, Sys_Map.Instance.CurMapId);
            }
        }

        public void Refresh(Hero hero) {
            this.hero = hero;

            TextHelper.SetText(this.charName, hero.heroBaseComponent.Name);
            TextHelper.SetText(this.lv, hero.heroBaseComponent.Level.ToString());

            uint iconId = CharacterHelper.getHeadID(hero.heroBaseComponent.HeroID, hero.heroBaseComponent.HeadId);
            ImageHelper.SetIcon(this.charIcon, iconId);
            iconId = CharacterHelper.getHeadFrameID(hero.heroBaseComponent.HeadFrameId);
            if (iconId != 0) {
                this.charIconFrame.gameObject.SetActive(true);
                ImageHelper.SetIcon(this.charIconFrame, iconId);
            }
            else {
                this.charIconFrame.gameObject.SetActive(false);
            }

            bool isRed = hero.familyResBattleComponent.isRed;
            this.redMark.SetActive(isRed);
            this.blueMark.SetActive(!isRed);

            // 携带资源
            var csv = CSVFamilyResBattleResParameter.Instance.GetConfData(hero.familyResBattleComponent.resource);
            if (csv != null) {
                this.resIcon.gameObject.SetActive(true);
                ImageHelper.SetIcon(this.resIcon, csv.IconID);
            }
            else {
                this.resIcon.gameObject.SetActive(false);
            }
        }
    }
    public class Layout : LayoutBase {
        public GameObject proto;
        public Transform protoParent;

        public void Parse(GameObject root) {
            this.Set(root);

            this.proto = this.transform.Find("Team_Teammate/Scroll View/Viewport/Grid/Proto").gameObject;
            this.protoParent = this.proto.transform.parent;
        }

        public void RegisterEvents(IListener listener) {
        }

        public interface IListener {
        }
    }

    public Layout layout = new Layout();
    public COWVd<Tab> cowTabs = new COWVd<Tab>();

    private readonly List<Hero> m_Heroes = new List<Hero>(0);
    private readonly Sys_Role_Info.InfoParmas mCurInfoParmas = new Sys_Role_Info.InfoParmas();

    protected override void OnLoaded() {
        this.layout.Parse(this.gameObject);
        this.layout.RegisterEvents(this);
    }

    protected override void OnOpen(object arg) {
        this.mCurInfoParmas.Copy(arg as Sys_Role_Info.InfoParmas);
    }

    protected override void OnShow() {
        this.m_Heroes.Clear();
        if (this.mCurInfoParmas.eType != Sys_Role_Info.EType.FromAvatar) {
            this.m_Heroes.Clear();
        }
        if (this.mCurInfoParmas.eType == Sys_Role_Info.EType.Avatar) {
            this.m_Heroes.AddRange(this.mCurInfoParmas.mHeroes);
        }

        this.cowTabs.TryBuildOrRefresh(this.layout.proto.gameObject, this.layout.protoParent, this.m_Heroes.Count, this._OnTabRefresh);
    }

    private void _OnTabRefresh(Tab vd, int index) {
        // 传入角色信息
        vd.Refresh(this.m_Heroes[index]);
    }

    #region 事件通知
    protected override void ProcessEvents(bool toRegister) {
        Sys_Role_Info.Instance.eventEmitter.Handle(Sys_Role_Info.EEvents.NetMsg_RecvRoleAck, this.OnNetMsg_RecvRoleAck, toRegister);
    }

    private void OnNetMsg_RecvRoleAck() {
        this.CloseSelf();
    }
    #endregion
}