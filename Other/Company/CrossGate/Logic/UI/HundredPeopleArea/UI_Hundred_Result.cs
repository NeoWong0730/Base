using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public class UI_Hundred_ResultLayout {
        public GameObject mRoot { get; private set; }
        public Transform mTrans { get; private set; }

        public Transform curRewardNode;

        public GameObject nextRewardRoot;
        public Transform nextRewardNode;
        public Button btnExit;
        public Button btnGoto;
        public Button btnBg;

        public Text stageLevel;
        public Text tips;

        public void Parse(GameObject root) {
            this.mRoot = root;
            this.mTrans = root.transform;

            this.btnGoto = this.mTrans.Find("Animator/Btn_GoOn").GetComponent<Button>();
            this.btnExit = this.mTrans.Find("Animator/Btn_Quit").GetComponent<Button>();
            this.btnBg = this.mTrans.Find("Animator/off-bg").GetComponent<Button>();
            this.stageLevel = this.mTrans.Find("Animator/Image_Successbg/Image_Result/Text").GetComponent<Text>();
            this.tips = this.mTrans.Find("Animator/Text_Lock").GetComponent<Text>();

            this.curRewardNode = this.mTrans.Find("Animator/View/Grid");
            this.nextRewardNode = this.mTrans.Find("Animator/NextNode/View/Grid2");
            nextRewardRoot = this.mTrans.Find("Animator/NextNode").gameObject;
        }

        public void RegisterEvents(IListener listener) {
            this.btnGoto.onClick.AddListener(listener.OnBtnGotoClicked);
            this.btnExit.onClick.AddListener(listener.OnBtnExitClicked);
            //this.btnBg.onClick.AddListener(listener.OnBtnExitClicked);
        }

        public interface IListener {
            void OnBtnGotoClicked();
            void OnBtnExitClicked();
        }
    }

    public class UI_Hundred_Result : UIBase, UI_Hundred_ResultLayout.IListener {
        public UI_Hundred_ResultLayout Layout = new UI_Hundred_ResultLayout();
        private readonly UI_HundredPeopleArea_Data m_Data = new UI_HundredPeopleArea_Data();

        public UI_RewardList curRewardList;
        public UI_RewardList nextRewardList;

        private uint instanceId;

        protected override void OnLoaded() {
            this.Layout.Parse(this.gameObject);
            this.Layout.RegisterEvents(this);
        }

        protected override void OnOpen(object arg) {
            this.instanceId = System.Convert.ToUInt32(arg);
            this.m_Data.Reset();
            this.m_Data.LoadData();
            this.m_Data.UpdateStageIDList();
            this.m_Data.UpdateSubList();
        }

        protected override void OnOpened() {
            var csv = CSVInstanceDaily.Instance.GetConfData(this.instanceId);
            if (csv != null) {
                if (this.curRewardList == null) {
                    this.curRewardList = new UI_RewardList(this.Layout.curRewardNode, EUIID.UI_Hundred_Result);
                }

                var ls = CSVDrop.Instance.GetDropItem(csv.Award);
                ls.AddRange(CSVDrop.Instance.GetDropItem(csv.RandomAward));
                this.curRewardList.SetRewardList(ls);
                this.curRewardList?.Build(true, false, false, false, false, true, false, true, PropItem.OnClickPropItem, false, false);

                TextHelper.SetText(this.Layout.stageLevel, 1006167, csv.LayerStage.ToString(), (((csv.LayerStage - 1) * 10) + csv.Layerlevel).ToString());

                bool isShowTip = false;
                bool canNext = false;
                uint nextInstanceId = Sys_HundredPeopleArea.Instance.GetNextInstanceId(m_Data.map, (int)csv.LayerStage, (int)csv.Layerlevel);
                var csvNext = CSVInstanceDaily.Instance.GetConfData(nextInstanceId);
                if (csvNext != null) {
                    uint level = csvNext.LevelLimited;
                    isShowTip = Sys_Role.Instance.Role.Level < level;
                    canNext = !isShowTip;

                    this.Layout.btnGoto.gameObject.SetActive(true);
                    if (isShowTip) {
                        TextHelper.SetText(this.Layout.tips, 1006163, level.ToString());
                    }
                    else {
                        TextHelper.SetText(this.Layout.tips, 1006192);
                    }
                    isShowTip = true;

                    Layout.nextRewardRoot.SetActive(true);
                    if (this.nextRewardList == null) {
                        this.nextRewardList = new UI_RewardList(this.Layout.nextRewardNode, EUIID.UI_Hundred_Result);
                    }
                    var nextLs = CSVDrop.Instance.GetDropItem(csvNext.Award);
                    nextLs.AddRange(CSVDrop.Instance.GetDropItem(csvNext.RandomAward));
                    this.nextRewardList.SetRewardList(nextLs);
                    this.nextRewardList?.Build(true, false, false, false, false, true, false, true, PropItem.OnClickPropItem, false, false);
                }
                else {
                    isShowTip = false;
                    this.Layout.btnGoto.gameObject.SetActive(false);

                    Layout.nextRewardRoot.SetActive(false);
                }

                this.Layout.tips.gameObject.SetActive(isShowTip);
                ButtonHelper.Enable(this.Layout.btnGoto, canNext);
            }
        }
        
        public void OnBtnExitClicked() {
            Sys_Instance.Instance.InstanceExitReq();
            this.CloseSelf();
        }

        public void OnBtnGotoClicked() {
            Sys_HundredPeopleArea.Instance.ReqNextStage();
            this.CloseSelf();
        }
    }
}
