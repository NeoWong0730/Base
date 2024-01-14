using System.Collections.Generic;
using DG.Tweening;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public partial class UI_HundredPeopleArea_Layout {
        #region 界面组件
        /// <summary> 目录节点 </summary>
        public Transform transform;
        /// <summary> 挑战按钮 </summary>
        public Button button_Fight;
        /// <summary> 当前挑战关卡数 </summary>
        public Text text_BestStage;
        /// <summary> 关卡菜单 </summary>
        public List<Toggle> list_StageSubs = new List<Toggle>();
        /// <summary> 选中关卡 </summary>
        public Text text_SelectStage;
        /// <summary> 选中关卡描述 </summary>
        public Text text_Describe;
        /// <summary> 首通奖励 </summary>
        public List<Transform> list_FirstReward = new List<Transform>();
        /// <summary> 未结束奖励提示 </summary>
        public Text go_LockTips;
        /// <summary> 关卡开关组 </summary>
        public ToggleGroup toggleGroup_Stage;
        /// <summary> 关卡按钮 </summary>
        public List<Toggle> list_Stage = new List<Toggle>();
        /// <summary> 关卡菜单 </summary>
        public List<Transform> list_StageView = new List<Transform>();
        /// <summary> 掉落奖励模板 </summary>
        public Transform tr_DropRewardItem;
        /// <summary> 掉落奖励列表 </summary>
        public List<PropItem> list_DropRewardItem = new List<PropItem>();
        public Text txt_awkenLimit;
        public GameObject go_awkenLimit;
        /// <summary> 每日领奖界面 </summary>
        public GameObject go_DayRewardView;
        /// <summary> 中心节点 </summary>
        public RectTransform rt_CenterNode;
        /// <summary> 右边节点 </summary>
        public RectTransform rt_RightNode;
        public Button btnDailyAward;
        public Button btnGotReward;
        public ScrollbarMoveTo scrollbar;
        public GameObject fxDailyReward;
        public Button btnTipAwken;
        public Button btnTipAwkenBg;
        public GameObject tipAwkenNode;
        public Button btnAwken;

        public Image buffIcon;
        public Text buffText;
        #endregion

        public UI_HundredPeopleArea_Data m_Data;
        public UI_RewardList rewardList;
        public Text recommendScore;

        #region 初始化
        public void Load(Transform root) {
            this.transform = root;

            this.buffIcon = this.transform.Find("Animator/View_Front/View_Awaken/Image/Image_Awaken/Image").GetComponent<Image>();
            this.buffText = this.transform.Find("Animator/View_Front/View_Awaken/Image/Image_Awaken/Text").GetComponent<Text>();

            this.btnAwken = this.transform.Find("Animator/View_Front/View_Awaken/Image/Image_Awaken/Btn_01_Small").GetComponent<Button>();
            this.button_Fight = this.transform.Find("Animator/View_Front/View_Right/View/Btn_01").GetComponent<Button>();
            this.btnTipAwken = this.transform.Find("Animator/View_Front/Button_Awaken").GetComponent<Button>();
            this.btnTipAwkenBg = this.transform.Find("Animator/View_Front/View_Awaken/Image_Close").GetComponent<Button>();
            this.tipAwkenNode = this.transform.Find("Animator/View_Front/View_Awaken").gameObject;

            this.text_BestStage = this.transform.Find("Animator/View_Front/Image_Num/Text_Title1/Text_Number1").GetComponent<Text>();
            Transform tr_MenuNode = this.transform.Find("Animator/View_Front/View_Right/Level_Number").transform;
            for (int i = 0; i < tr_MenuNode.childCount; i++) {
                this.list_StageSubs.Add(tr_MenuNode.GetChild(i).GetComponent<Toggle>());
            }

            this.text_SelectStage = this.transform.Find("Animator/View_Front/View_Right/View/Text_Level/Text_Level2").GetComponent<Text>();
            this.text_Describe = this.transform.Find("Animator/View_Front/View_Right/View/View_Introduce/Text").GetComponent<Text>();
            this.go_LockTips = this.transform.Find("Animator/View_Front/View_Right/View/Text_Tips").GetComponent<Text>();

            this.toggleGroup_Stage = this.transform.Find("Animator/View_Front/Scroll_View/Viewport/Content/View_Dungeons/Toggle_Pic").GetComponent<ToggleGroup>();
            Transform tr_StageNode = this.transform.Find("Animator/View_Front/Scroll_View/Viewport/Content/View_Dungeons/Toggle_Pic").transform;
            for (int i = 0; i < tr_StageNode.childCount; i++) {
                int lastIndex = tr_StageNode.childCount - 1 - i;
                this.list_Stage.Add(tr_StageNode.GetChild(lastIndex).GetComponent<Toggle>());
            }
            Transform tr_StageStateNode = this.transform.Find("Animator/View_Front/Scroll_View/Viewport/Content/View_Dungeons/Level_State").transform;
            for (int i = 0; i < tr_StageStateNode.childCount; i++) {
                int lastIndex = tr_StageStateNode.childCount - 1 - i;
                this.list_StageView.Add(tr_StageStateNode.GetChild(lastIndex).transform);
            }

            this.tr_DropRewardItem = this.transform.Find("Animator/View_Front/View_Right/View/Text_Award/Scroll_View/Viewport/Item").transform;
            this.go_DayRewardView = this.transform.Find("Animator/View_Front/View_DayReward").gameObject;
            this.txt_awkenLimit = this.transform.Find("Animator/View_Front/View_Right/View/Text_Awaken/Text").GetComponent<Text>();
            this.go_awkenLimit = this.transform.Find("Animator/View_Front/View_Right/View/Text_Awaken").gameObject;

            this.rt_CenterNode = this.transform.Find("Animator/View_Front/Scroll_View").GetComponent<RectTransform>();
            this.rt_RightNode = this.transform.Find("Animator/View_Front/View_Right").GetComponent<RectTransform>();
            this.scrollbar = this.transform.Find("Animator/View_Front/Scroll_View/Scrollbar").GetComponent<ScrollbarMoveTo>();
            this.fxDailyReward = this.transform.Find("Animator/View_Front/Button_Reward/FxDailyReward").gameObject;

            this.rewardList = new UI_RewardList(this.transform.Find("Animator/View_Front/View_DayReward/Image/Scroll View/Viewport/Content"), EUIID.UI_HundredPeopleArea);
            recommendScore = this.transform.Find("Animator/View_Front/View_Right/View/Text_Score/Text_Num").GetComponent<Text>();
        }
        public void SetListener(IListener listener) {
            this.transform.Find("Animator/View_Front/View_Title/Btn_Close").GetComponent<Button>().onClick.AddListener(listener.OnClick_Close);
            this.transform.Find("Animator/View_Front/Button_Rank").GetComponent<Button>().onClick.AddListener(listener.OnClick_Rank);
            this.btnDailyAward = this.transform.Find("Animator/View_Front/Button_Reward").GetComponent<Button>();
            this.btnDailyAward.onClick.AddListener(listener.OnClick_OpenDailyRewardView);
            this.transform.Find("Animator/View_Front/View_DayReward/Image_Close").GetComponent<Button>().onClick.AddListener(listener.OnClick_CloseDailyRewardView);
            this.btnGotReward = this.transform.Find("Animator/View_Front/View_DayReward/Image/Btn_01").GetComponent<Button>();
            this.btnGotReward.onClick.AddListener(listener.OnClick_DailyReward);
            this.button_Fight.onClick.AddListener(listener.OnClick_Fight);
            this.btnAwken.onClick.AddListener(listener.OnBtnAwkenClicked);
            this.btnTipAwken.onClick.AddListener(listener.OnBtnTipAwkenClicked);
            this.btnTipAwkenBg.onClick.AddListener(listener.OnBtnTipAwkenBgClicked);

            this.transform.Find("Animator/View_Front/Scroll_View").GetComponent<Button>().onClick.AddListener(listener.OnClick_BG);

            for (int i = 0; i < this.list_Stage.Count; i++) {
                Toggle toggle = this.list_Stage[i];
                toggle.onValueChanged.AddListener((bool value) => listener.OnClick_Stage(toggle, value));
            }
            for (int i = 0; i < this.list_StageSubs.Count; i++) {
                Toggle toggle = this.list_StageSubs[i];
                toggle.onValueChanged.AddListener((bool value) => listener.OnClick_StageSub(toggle, value));
            }

        }
        #endregion

        #region 响应事件
        public interface IListener {
            void OnClick_Close();
            void OnBtnAwkenClicked();
            void OnBtnTipAwkenClicked();
            void OnBtnTipAwkenBgClicked();

            void OnClick_Rank();

            void OnClick_OpenDailyRewardView();

            void OnClick_CloseDailyRewardView();

            void OnClick_Fight();

            void OnClick_Stage(Toggle toggle, bool value);

            void OnClick_StageSub(Toggle toggle, bool value);

            void OnClick_DailyReward();
            void OnClick_BG();
        }
        #endregion

        #region 功能接口
        public void SetStageItem(int index) {
            if (index < 0 || index >= this.list_StageView.Count)
                return;

            Transform tr = this.list_StageView[index].transform;
            Text text_State = tr.Find("Image_State/Text_State").GetComponent<Text>();
            Transform tr_Reward = tr.Find("Image_State/Reward").transform;
            Image image_Icon = tr.Find("Image_State/Reward/Icon").GetComponent<Image>();

            int stageLevel = this.m_Data.GetStageID(index);
            uint passedInstanceId = Sys_HundredPeopleArea.Instance.passedInstanceId;
            int instanceId = this.m_Data.UnlockLayerLevelId(stageLevel, passedInstanceId, out UI_HundredPeopleArea_Data.EStageLockReason reason);
            if (UI_HundredPeopleArea_Data.EStageLockReason.AllOver == reason) {
                // 全部通关
                TextHelper.SetText(text_State, 1006157);

                var dict = this.m_Data.map[stageLevel];
                uint lastInstanceId = dict[dict.Count].id;
                var csv = CSVHundredChapter.Instance.GetConfData(lastInstanceId);
                tr_Reward.gameObject.SetActive(false);
                CSVItem.Data csvItem = CSVItem.Instance.GetConfData(csv.item_id);
                if (csvItem != null) {
                    ImageHelper.SetIcon(image_Icon, csvItem.small_icon_id);
                }

                this.list_Stage[index].enabled = false;
                this.list_Stage[index].GetComponent<GraphicGrayer>()?.SetGray(true);
            }
            else if (UI_HundredPeopleArea_Data.EStageLockReason.PreNotOver == reason ||
                UI_HundredPeopleArea_Data.EStageLockReason.ConditionNotValid == reason) {
                // 未解锁
                var dict = this.m_Data.map[stageLevel];
                if (reason == UI_HundredPeopleArea_Data.EStageLockReason.PreNotOver) {
                    TextHelper.SetText(text_State, 1006168);
                }
                else if (reason == UI_HundredPeopleArea_Data.EStageLockReason.ConditionNotValid) {
                    TextHelper.SetText(text_State, 1006155, dict[1].LevelLimited.ToString());
                }

                tr_Reward.gameObject.SetActive(false);
                this.list_Stage[index].enabled = false;
                this.list_Stage[index].GetComponent<GraphicGrayer>()?.SetGray(true);
            }
            else {
                // 解锁一部分小关卡
                var csv = CSVInstanceDaily.Instance.GetConfData((uint)instanceId);
                TextHelper.SetText(text_State, 1006156, ((csv.LayerStage - 1) * 10 + csv.Layerlevel).ToString());
                tr_Reward.gameObject.SetActive(false);
                this.list_Stage[index].enabled = true;
                this.list_Stage[index].GetComponent<GraphicGrayer>()?.SetGray(false);
            }
        }

        public void SubItem(int index, string name) {
            if (index < 0 || index >= this.list_StageSubs.Count)
                return;

            Transform tr = this.list_StageSubs[index].transform;
            Text text1 = tr.Find("Text").GetComponent<Text>();
            Text text2 = tr.Find("Text_Light").GetComponent<Text>();
            text2.text = text1.text = name;
        }

        public void SetDayRewardView(string title, uint id) {
            Transform tr = this.go_DayRewardView.transform;
            Text text_title = tr.Find("Image/Text").GetComponent<Text>();
            text_title.text = title;
        }

        public void RefreshTipAwken() {
            uint curLevel = Sys_TravellerAwakening.Instance.awakeLevel;
            CSVTravellerAwakening.Data csvAwken = CSVTravellerAwakening.Instance.GetConfData(curLevel);
            if (csvAwken != null) {
                CSVBuff.Data csvBuff = CSVBuff.Instance.GetConfData(csvAwken.BuffID);
                if (csvBuff != null) {
                    ImageHelper.SetIcon(this.buffIcon, csvBuff.icon);
                    TextHelper.SetText(this.buffText, csvBuff.desc);
                }
            }
        }

        public int GetStageIndex(Toggle toggle) {
            return this.list_Stage.IndexOf(toggle);
        }

        public int GetStageMenuIndex(Toggle toggle) {
            return this.list_StageSubs.IndexOf(toggle);
        }

        private void CreateRewardItemList(int number) {
            while (this.tr_DropRewardItem.parent.childCount < number) {
                GameObject.Instantiate(this.tr_DropRewardItem, this.tr_DropRewardItem.parent);
            }
        }

        public void CreatePropItemList(int number) {
            this.CreateRewardItemList(number);

            this.list_DropRewardItem.Clear();

            for (int i = 0, count = this.tr_DropRewardItem.parent.childCount; i < count; i++) {
                PropItem propItem = new PropItem();
                propItem.BindGameObject(this.tr_DropRewardItem.parent.GetChild(i).gameObject);
                this.list_DropRewardItem.Add(propItem);
            }
        }

        public bool isPlayed { get; private set; } = false;
        public void PlayAnimation_View(bool isShow, bool isImmediately = false) {
            if (isShow) {
                if (isImmediately) {
                    this.rt_CenterNode.DOAnchorPosX(-206, 0f);
                    this.rt_RightNode.DOAnchorPosX(100f, 0f);
                }
                else if (this.isPlayed) {
                    this.rt_RightNode.DOAnchorPosX(300f, 0.3f).SetEase(Ease.InSine)
                    .OnComplete(() => {
                        this.rt_RightNode.DOAnchorPosX(100f, 0.3f).SetEase(Ease.OutSine);
                    });
                }
                else {
                    this.rt_CenterNode.DOAnchorPosX(-206, 0.3f);
                    this.rt_RightNode.DOAnchorPosX(100f, 0.3f);
                }
                this.isPlayed = true;
            }
            else {
                this.rt_CenterNode.DOAnchorPosX(0f, isImmediately ? 0f : 0.3f);
                this.rt_RightNode.DOAnchorPosX(2000f, isImmediately ? 0f : 0.3f);
                this.isPlayed = false;
            }
        }
        #endregion
    }
}