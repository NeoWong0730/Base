using System;
using Lib.Core;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic {
    public class UI_FavorabilityStageRewardPreview_Layout : UILayoutBase {
        public Button btnExit;
        public Transform proto;
        public Text title;

        public void Parse(GameObject root) {
            Init(root);

            btnExit = transform.Find("Close").GetComponent<Button>();
            proto = transform.Find("ZoneAward/View_Content/Scroll_View01/Viewport/Proto");
            title = transform.Find("ZoneAward/View_Content/Text_Tips").GetComponent<Text>();
        }

        public void RegisterEvents(IListener listener) {
            btnExit.onClick.AddListener(listener.OnBtnExitClicked);
        }

        public interface IListener {
            void OnBtnExitClicked();
        }
    }

    // 阶段奖励预览
    public class UI_FavorabilityStageRewardPreview : UIBase, UI_FavorabilityStageRewardPreview_Layout.IListener {
        public class ItemGroup : UIComponent {
            public Text itemTypeName;
            public UI_RewardList rewards;
            public GameObject detailGo;
            public Text detailDesc;
            public Text value;

            public Transform rewardParent;

            public UI_FavorabilityStageRewardPreview ui;
            public FavorabilityNPC npc;

            protected override void Loaded() {
                itemTypeName = transform.Find("Image_Title/Text_Title").GetComponent<Text>();
                detailGo = transform.Find("Image_Tips").gameObject;
                detailDesc = transform.Find("Image_Tips/Text").GetComponent<Text>();
                value = transform.Find("Image_Title/Image_Amount/Text_Amount").GetComponent<Text>();

                rewardParent = transform.Find("Scroll_View/Viewport");
            }

            public void Refresh(UI_FavorabilityStageRewardPreview ui, FavorabilityNPC npc, uint stage) {
                this.ui = ui;
                this.npc = npc;
                rewards?.Show(false);
                detailGo.gameObject.SetActive(false);
                var lastCSV = CSVNPCFavorabilityStage.Instance.GetConfData(FavorabilityNPC.GetFavorabilityStageId(npc.id, stage - 1));
                var currentCSV = CSVNPCFavorabilityStage.Instance.GetConfData(FavorabilityNPC.GetFavorabilityStageId(npc.id, stage));
                if (currentCSV != null) {
                    if (rewards == null) {
                        rewards = new UI_RewardList(rewardParent.transform, EUIID.UI_FavorabilityStageRewardPreview);
                    }

                    TextHelper.SetText(this.value, 2010648, lastCSV.FavorabilityValueMax.ToString());
                    CSVFavorabilityStageName.Data csvStageName = CSVFavorabilityStageName.Instance.GetConfData(stage);
                    if (csvStageName != null) {
                        TextHelper.SetText(this.itemTypeName, csvStageName.name);
                    }
                    else {
                        TextHelper.SetText(itemTypeName, "表格不存在对应阶段");
                    }

                    if (lastCSV.RewardType == (uint) EFavorabilityLetterRewardType.Drop) {
                        uint dropId = lastCSV.Reward;
                        var rewardLs = CSVDrop.Instance.GetDropItem(dropId);
                        rewards.SetRewardList(rewardLs);
                        rewards.Show(rewardLs.Count > 0);
                        bool hasGot = npc.favorabilityStage >= stage;
                        rewards.Build(true, false, false, false, false, true, false, true, PropItem.OnClickPropItem, false, hasGot);
                        detailGo.gameObject.SetActive(false);
                    }
                    else {
                        rewards.Show(false);
                        detailGo.gameObject.SetActive(true);
                        if (lastCSV.RewardType == (uint) EFavorabilityLetterRewardType.Partner) {
                            TextHelper.SetText(detailDesc, 2010603);
                        }
                        else if (lastCSV.RewardType == (uint) EFavorabilityLetterRewardType.Mall) {
                            TextHelper.SetText(detailDesc, 2010602);
                        }
                    }
                }

                FrameworkTool.ForceRebuildLayout(gameObject);
            }
        }

        public UI_FavorabilityStageRewardPreview_Layout Layout = new UI_FavorabilityStageRewardPreview_Layout();
        public COWVd<ItemGroup> groups = new COWVd<ItemGroup>();

        public float x;
        public float y;
        public uint npcId;
        public FavorabilityNPC npc;

        public void OnBtnExitClicked() {
            CloseSelf();
        }

        protected override void OnLoaded() {
            this.Layout.Parse(this.gameObject);
            this.Layout.RegisterEvents(this);
        }

        protected override void OnOpen(object arg) {
            Tuple<float, float, uint> tp = arg as Tuple<float, float, uint>;
            if (tp != null) {
                x = tp.Item1;
                y = tp.Item2;
                npcId = tp.Item3;
                Sys_NPCFavorability.Instance.TryGetNpc(npcId, out npc, false);
            }
        }

        protected override void OnOpened() {
            RectTransform node = transform.Find("ZoneAward").GetComponent<RectTransform>();
            if (node != null) {
                node.anchoredPosition = new Vector2(x, y);
            }
        }

        protected override void OnShow() {
            // 最多5个阶段
            groups.TryBuildOrRefresh(this.Layout.proto.gameObject, this.Layout.proto.parent, 4, (vd, index) => { vd.Refresh(this, npc, (uint) index + 2); });

            TextHelper.SetText(this.Layout.title, npc.csvNPCFavorability.RewardPrompt);

            FrameworkTool.ForceRebuildLayout(gameObject);
        }
    }
}