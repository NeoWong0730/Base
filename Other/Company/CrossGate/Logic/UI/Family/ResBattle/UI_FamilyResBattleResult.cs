using System;
using Logic;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

// 家族资源战 战场结算
public class UI_FamilyResBattleResult : UIBase, UI_FamilyResBattleResult.Layout.IListener {
    public class Layout : LayoutBase {
        public Button btnExit;

        public GameObject winNode;
        public GameObject failNode;
        public GameObject pingNode;

        public Transform personalProtoParent;
        public Transform familyProtoParent;

        public Text redRes;
        public Text blueRes;

        public Button btnDetail;

        public void Parse(GameObject root) {
            this.Set(root);

            this.btnExit = this.transform.Find("Image_Black").GetComponent<Button>();
            this.btnDetail = this.transform.Find("Animator/Btn_Rank").GetComponent<Button>();

            this.redRes = this.transform.Find("Animator/View_Win/Image_Resource/Text1/Text_Num").GetComponent<Text>();
            this.blueRes = this.transform.Find("Animator/View_Win/Image_Resource/Text2/Text_Num").GetComponent<Text>();

            this.winNode = this.transform.Find("Animator/Image_Successbg").gameObject;
            this.failNode = this.transform.Find("Animator/Image_Failedbg").gameObject;
            this.pingNode = this.transform.Find("Animator/Image_Pingbg").gameObject;

            this.personalProtoParent = this.transform.Find("Animator/View_Win/Reward_List1/Grid");
            this.familyProtoParent = this.transform.Find("Animator/View_Win/Reward_List2/Grid");
        }

        public void RegisterEvents(IListener listener) {
            this.btnExit.onClick.AddListener(listener.OnBtnExitClicked);
            this.btnDetail.onClick.AddListener(listener.OnBtnClicked);
        }

        public interface IListener {
            void OnBtnClicked();
            void OnBtnExitClicked();
        }
    }

    public Layout layout = new Layout();
    public UI_RewardList personalList;
    public UI_RewardList familylList;

    public uint result;

    protected override void OnLoaded() {
        this.layout.Parse(this.gameObject);
        this.layout.RegisterEvents(this);
    }

    protected override void OnOpen(object arg) {
        this.result = Convert.ToUInt32(arg);
    }

    protected override void OnOpened() {
        if (this.personalList == null) {
            this.personalList = new UI_RewardList(this.layout.personalProtoParent, EUIID.UI_FamilyResBattleResult);
        }
        if (this.familylList == null) {
            this.familylList = new UI_RewardList(this.layout.familyProtoParent, EUIID.UI_FamilyResBattleResult);
        }

        uint personalDropId = 0;
        uint familyDropId = 0;

        this.layout.winNode.SetActive(this.result == 1);
        this.layout.pingNode.SetActive(this.result == 3);
        this.layout.failNode.SetActive(this.result == 2);

        if (this.result == 1) {
            // 胜利
            personalDropId = Sys_FamilyResBattle.Instance.battlewinPersonalDropId;
            familyDropId = Sys_FamilyResBattle.Instance.battlewinFamilyDropId;
        }
        else if (this.result == 2) {
            // 失败
            personalDropId = Sys_FamilyResBattle.Instance.battlefailPersonalDropId;
            familyDropId = Sys_FamilyResBattle.Instance.battlefailFamilyDropId;
        }
        else if (this.result == 3) {
            // 平局
            personalDropId = Sys_FamilyResBattle.Instance.battlepingPersonalDropId;
            familyDropId = Sys_FamilyResBattle.Instance.battlepingFamilyDropId;
        }

        this.personalList.SetRewardList(CSVDrop.Instance.GetDropItem(personalDropId));
        this.personalList?.Build(true, false, false, false, false, true, false, true, PropItem.OnClickPropItem, false, false);

        this.familylList.SetRewardList(CSVDrop.Instance.GetDropItem(familyDropId));
        this.familylList?.Build(true, false, false, false, false, true, false, true, PropItem.OnClickPropItem, false, false);

        TextHelper.SetText(this.layout.redRes, Sys_FamilyResBattle.Instance.TotalRes(Sys_FamilyResBattle.Instance.redFamilyId).ToString());
        TextHelper.SetText(this.layout.blueRes, Sys_FamilyResBattle.Instance.TotalRes(Sys_FamilyResBattle.Instance.blueFamilyId).ToString());
    }

    public void OnBtnClicked() {
        UIManager.OpenUI(EUIID.UI_FamilyResBattleRank);
    }

    public void OnBtnExitClicked() {
        this.CloseSelf();

        if (Sys_Team.Instance.canManualOperate) {
            Sys_FamilyResBattle.Instance.ReqLeave();
        }
    }
}