using System.Collections.Generic;
using Logic;
using Logic.Core;
using Packet;
using Table;
using UnityEngine;
using UnityEngine.UI;
using static Logic.Sys_FamilyResBattle;

// 家族资源战 报名
public class UI_FamilyResBattleSignup : UIBase, UI_FamilyResBattleSignup.Layout.IListener {
    public class Layout : LayoutBase {
        public Button btnSure;
        public Transform notLeader;

        public GameObject proto;
        public Transform protoParent;

        public void Parse(GameObject root) {
            this.Set(root);

            this.btnSure = this.transform.Find("Animator/View_State/Btn_01").GetComponent<Button>();
            this.notLeader = this.transform.Find("Animator/View_State/Text1 (1)");

            this.proto = this.transform.Find("Animator/Grid/Text").gameObject;
            this.protoParent = this.transform.Find("Animator/Grid");
        }

        public void RegisterEvents(IListener listener) {
            this.btnSure.onClick.AddListener(listener.OnBtnSureClicked);
        }

        public interface IListener {
            void OnBtnSureClicked();
        }
    }

    public class Tab : UIComponent {
        public Text desc;

        protected override void Loaded() {
            this.desc = this.gameObject.GetComponent<Text>();
        }

        public void Refresh(ESignupReason index, bool valid, uint lanId, bool hasReceived) {
            // 时间是否合适
            // 族长
            // 等级
            // 人数
            // 活跃要求
            if (!hasReceived) {
                string s = LanguageHelper.GetTextContent(lanId);
                TextHelper.SetText(this.desc, 2024101, s);
            }
            else {
                if (valid) {
                    string s = LanguageHelper.GetTextContent(lanId);
                    TextHelper.SetText(this.desc, 2024101, s);
                }
                else {
                    string s = LanguageHelper.GetTextContent(lanId);
                    TextHelper.SetText(this.desc, 2024102, s);
                }
            }
        }
    }

    public Layout layout = new Layout();
    public COWVd<Tab> cows = new COWVd<Tab>();
    public List<bool> conditions = new List<bool>((int) ESignupReason.Valid) {false, false, false, false, false};
    public List<uint> conditionDescs = new List<uint>((int) ESignupReason.Valid) {3230000004, 3230000005, 3230000006, 3230000007, 3230000008};

    private bool hasReceived = false;

    protected override void OnLoaded() {
        this.layout.Parse(this.gameObject);
        this.layout.RegisterEvents(this);
    }

    protected override void OnDestroy() {
        this.cows.Clear();
        this.cows = null;
    }

    protected override void OnOpened() {
        uint pos = 100u;
        if (Sys_Family.Instance.familyData != null) {
            var cmd = Sys_Family.Instance.familyData.CheckMember(Sys_Role.Instance.RoleId);
            if (cmd != null) {
                pos = cmd.Position % 10000;
            }
        }

        Sys_Family.FamilyData.EFamilyStatus familyStatus = (Sys_Family.FamilyData.EFamilyStatus) (pos);
        var csv = CSVFamilyPostAuthority.Instance.GetConfData((uint) familyStatus);
        if (csv != null) {
            this.conditions[1] = (csv.BattleEnroll != 0);
        }
        else {
            this.conditions[1] = false;
        }

        this.Refresh();

        this.ReqSignupCondition();
    }

    protected override void ProcessEvents(bool toRegister) {
        // 条件变化监听
        Sys_FamilyResBattle.Instance.eventEmitter.Handle<CmdGuildBattleCheckApplyCondRes>(Sys_FamilyResBattle.EEvents.OnSignupConditionChanged, this.OnReceivedSignupCondition, toRegister);
        Sys_FamilyResBattle.Instance.eventEmitter.Handle<bool, bool>(Sys_FamilyResBattle.EEvents.OnSignupChanged, this.OnSignupChanged, toRegister);
    }

    public void Refresh() {
        // 5个限制条件
        this.cows.TryBuildOrRefresh(this.layout.proto, this.layout.protoParent, this.conditions.Count, this._OnRefresh);

        bool isLeader = this.conditions[1];
        if (this.hasReceived) {
            this.layout.btnSure.gameObject.SetActive(isLeader);
            this.layout.notLeader.gameObject.SetActive(!isLeader);
            ButtonHelper.Enable(this.layout.btnSure, canSignUp);
        }
        else {
            this.layout.btnSure.gameObject.SetActive(isLeader);
            this.layout.notLeader.gameObject.SetActive(!isLeader);
            ButtonHelper.Enable(this.layout.btnSure, false);
        }
    }

    private bool canSignUp {
        get {
            bool rlt = true;
            for (int i = 0, length = this.conditions.Count; i < length; ++i) {
                rlt &= (this.conditions[i]);
            }

            return rlt && this.hasReceived && !Sys_FamilyResBattle.Instance.hasSigned;
        }
    }

    private void _OnRefresh(Tab tab, int index) {
        tab.Refresh((ESignupReason) index, this.conditions[index], this.conditionDescs[index], this.hasReceived);
    }

    public void OnBtnSureClicked() {
        if (this.canSignUp) {
            Sys_FamilyResBattle.Instance.ReqApply();
        }
        else {
        }
    }

    #region 网络交互

    public void ReqSignupCondition() {
        Sys_FamilyResBattle.Instance.ReqCanSignup();
    }

    private void OnSignupChanged(bool oldStatus, bool newStatus) {
        if (oldStatus != newStatus) {
            this.CloseSelf();
        }
    }

    private void OnReceivedSignupCondition(CmdGuildBattleCheckApplyCondRes res) {
        this.conditions.Clear();
        for (int i = 0, length = res.Result.Count; i < length; i++) {
            this.conditions.Add(res.Result[i]);
        }

        this.hasReceived = true;
        this.Refresh();
    }

    #endregion
}