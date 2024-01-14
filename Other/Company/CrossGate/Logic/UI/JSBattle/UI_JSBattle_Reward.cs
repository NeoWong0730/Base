using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using Table;
using System;
using UnityEngine.UI;
using Packet;

namespace Logic
{
    public class UI_JSBattle_Reward_Layout
    {
        private Button closeBtn;
        /// <summary> 无限滚动 </summary>
        private InfinityGrid infinityGrid;
        public void Init(Transform transform)
        {
            infinityGrid = transform.Find("Animator/List/Scroll View").GetComponent<InfinityGrid>();
            closeBtn = transform.Find("Animator/View_TipsBgNew02/Btn_Close").GetComponent<Button>();
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.CloseBtnClicked);
            infinityGrid.onCreateCell += listener.OnCreateCell;
            infinityGrid.onCellChange += listener.OnCellChange;
        }

        public void SetInfinityGridCell(int count)
        {
            infinityGrid.CellCount = count;
            infinityGrid.ForceRefreshActiveCell();
        }

        public interface IListener
        {
            void CloseBtnClicked();
            void OnCreateCell(InfinityGridCell cell);
            void OnCellChange(InfinityGridCell cell, int index);
        }
    }

    public class UI_JSBattle_RewardCeil
    {
        private Image rankImage;
        private Text rankText;
        /// <summary> 无限滚动 </summary>
        private InfinityGrid infinityGrid;
        private List<ItemIdCount> items = new List<ItemIdCount>();
        public void Init(Transform transform)
        {
            rankImage = transform.Find("Rank/Image_Icon").GetComponent<Image>();
            rankText = transform.Find("Rank/Text_Rank").GetComponent<Text>();
            infinityGrid = transform.Find("Scroll View").GetComponent<InfinityGrid>();
            infinityGrid.onCreateCell = OnCreateCell;
            infinityGrid.onCellChange = OnCellChange;
        }

        /// <summary>
        /// 滚动列表创建回调
        /// </summary>
        /// <param name="cell"></param>
        public void OnCreateCell(InfinityGridCell cell)
        {
            PropItem entry = new PropItem();
            GameObject go = cell.mRootTransform.gameObject;
            entry.BindGameObject(go);
            cell.BindUserData(entry);
        }

        /// <summary>
        /// 滚动列表滚动回调
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="index"></param>
        public void OnCellChange(InfinityGridCell cell, int index)
        {
            if (index < 0 || index >= items.Count)
                return;
            PropItem entry = cell.mUserData as PropItem;
            ItemIdCount itemdata = items[index];
            PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(itemdata.id, itemdata.count, true, false, false, false, false, true, false, true);
            entry.SetData(itemData, EUIID.UI_JSBattle_Reward);
        }

        public void SetData(uint up, uint low, uint reward)
        {
            bool isHasMedel = low - up <= 1;
            rankImage.gameObject.SetActive(isHasMedel);
            rankText.gameObject.SetActive(!isHasMedel);
            if (isHasMedel)
            {
                uint iconId = Sys_Rank.Instance.GetRankIcon((int)low);
                ImageHelper.SetIcon(rankImage, iconId);
            }
            else
            {
                TextHelper.SetText(rankText, string.Format("{0}-{1}", (up + 1).ToString(), low.ToString()));
            }
            items.Clear();
            items = CSVDrop.Instance.GetDropItem(reward);
            infinityGrid.CellCount = items.Count;
            infinityGrid.ForceRefreshActiveCell();
        }
    }
    public class UI_JSBattle_Reward : UIBase, UI_JSBattle_Reward_Layout.IListener
    {
        private UI_JSBattle_Reward_Layout layout = new UI_JSBattle_Reward_Layout();

        public class UI_JSBattle_Reward_RightView
        {
            private Image rankImage;
            private Text rankText;
            private Text ruleText;
            private GameObject winGo;
            private GameObject endGo;
            private GameObject titleGo;
            private Button rewardBtn;
            /// <summary> 无限滚动 </summary>
            private InfinityGrid infinityGrid;
            private List<ItemIdCount> items = new List<ItemIdCount>();
            public void Init(Transform transform)
            {
                rankImage = transform.Find("Rank/Image_Icon").GetComponent<Image>();
                rankText = transform.Find("Rank/Text_Rank").GetComponent<Text>();
                ruleText = transform.Find("Rule/Text ").GetComponent<Text>();
                winGo = transform.Find("Win").gameObject;
                titleGo = transform.Find("Reward/Title").gameObject;
                endGo = transform.Find("Reward/Tips").gameObject;
                rewardBtn = transform.Find("Btn_01").GetComponent<Button>();
                rewardBtn.onClick.AddListener(RewardBtnClicked);

                infinityGrid = transform.Find("Reward/Scroll View").GetComponent<InfinityGrid>();
                infinityGrid.onCreateCell = OnCreateCell;
                infinityGrid.onCellChange = OnCellChange;
            }

            public void RefreshView()
            {
                RoleVictoryArenaReward roleVictoryArenaReward = Sys_JSBattle.Instance.GetRoleVictoryArenaReward();
                if (null != roleVictoryArenaReward)
                {
                    var highRank = roleVictoryArenaReward.HighestRank;
                    bool isHasMedel = highRank < 4;
                    rankImage.gameObject.SetActive(isHasMedel);
                    rankText.gameObject.SetActive(!isHasMedel);
                    if (isHasMedel)
                    {
                        uint iconId = Sys_Rank.Instance.GetRankIcon((int)highRank);
                        ImageHelper.SetIcon(rankImage, iconId);
                    }
                    else
                    {
                        TextHelper.SetText(rankText, highRank.ToString());
                    }

                    var rewardId = Sys_JSBattle.Instance.GetCanRewardId();
                    bool isGetEnd = Sys_JSBattle.Instance.CheckReward(rewardId);
                    bool isNo_1 = highRank == 1;
                    endGo.SetActive(isNo_1 && isGetEnd);
                    winGo.SetActive(isNo_1 && isGetEnd);
                    titleGo.SetActive(!isNo_1 || !isGetEnd);
                    ruleText.transform.parent.gameObject.SetActive(!isNo_1 || !isGetEnd);
                    infinityGrid.gameObject.SetActive(!isNo_1 || !isGetEnd);
                    rewardBtn.gameObject.SetActive(!isNo_1 || !isGetEnd);
                    if (!isNo_1 || !isGetEnd)
                    {
                        var configData = CSVDecisiveArenaRankingRedward.Instance.GetConfData(rewardId);
                        if(null != configData)
                        {
                            TextHelper.SetText(ruleText, 2024711u, configData.Ranking.ToString());
                            ImageHelper.SetImageGray(rewardBtn, !(highRank <= configData.Ranking), true);
                            items.Clear();
                            items = CSVDrop.Instance.GetDropItem(configData.Reward);
                            infinityGrid.CellCount = items.Count;
                            infinityGrid.ForceRefreshActiveCell();
                        }
                       
                    }
                }
            }

            private void RewardBtnClicked()
            {
                RoleVictoryArenaReward roleVictoryArenaReward = Sys_JSBattle.Instance.GetRoleVictoryArenaReward();
                if (null != roleVictoryArenaReward)
                {
                    var rewardId = Sys_JSBattle.Instance.GetCanRewardId();
                    var configData = CSVDecisiveArenaRankingRedward.Instance.GetConfData(rewardId);
                    if (null != configData)
                    {
                        if (roleVictoryArenaReward.HighestRank <= configData.Ranking)
                        {
                            Sys_JSBattle.Instance.VictoryArenaReceiveRankRewardReq(Sys_JSBattle.Instance.GetCanRewardId());
                        }
                        else
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2024708));
                        }
                    }
                }
            }

            /// <summary>
            /// 滚动列表创建回调
            /// </summary>
            /// <param name="cell"></param>
            public void OnCreateCell(InfinityGridCell cell)
            {
                PropItem entry = new PropItem();
                GameObject go = cell.mRootTransform.gameObject;
                entry.BindGameObject(go);
                cell.BindUserData(entry);
            }

            /// <summary>
            /// 滚动列表滚动回调
            /// </summary>
            /// <param name="cell"></param>
            /// <param name="index"></param>
            public void OnCellChange(InfinityGridCell cell, int index)
            {
                if (index < 0 || index >= items.Count)
                    return;
                PropItem entry = cell.mUserData as PropItem;
                ItemIdCount itemdata = items[index];
                PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(itemdata.id, itemdata.count, true, false, false, false, false, true, false, true);
                entry.SetData(itemData, EUIID.UI_JSBattle_Reward);
            }
        }


        private UI_JSBattle_Reward_RightView rightView = new UI_JSBattle_Reward_RightView();
        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
            rightView.Init(transform.Find("Animator/Right"));
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_JSBattle.Instance.eventEmitter.Handle(Sys_JSBattle.EEvents.GetRewardEnd, RefreshRewardView, toRegister);
        }

        List<uint> rankRewardNum = new List<uint>();
        List<uint> rewardId = new List<uint>();
        protected override void OnOpen(object arg = null)
        {
            Config();
        }

        private void Config()
        {
            var dataList = CSVDecisiveArenaRankingRedward.Instance.GetAll();
            for (int i = 0, len = dataList.Count; i < len; i++)
            {
                rankRewardNum.Add(dataList[i].Ranking);
                rewardId.Add(dataList[i].Reward);
            }
        }

        protected override void OnShow()
        {
            layout.SetInfinityGridCell(rankRewardNum.Count);
            RefreshRewardView();
        }

        private void RefreshRewardView()
        {
            rightView.RefreshView();
        }
        

        /// <summary>
        /// 滚动列表创建回调
        /// </summary>
        /// <param name="cell"></param>
        public void OnCreateCell(InfinityGridCell cell)
        {
            UI_JSBattle_RewardCeil entry = new UI_JSBattle_RewardCeil();
            GameObject go = cell.mRootTransform.gameObject;
            entry.Init(go.transform);
            cell.BindUserData(entry);
        }

        /// <summary>
        /// 滚动列表滚动回调
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="index"></param>
        public void OnCellChange(InfinityGridCell cell, int index)
        {

            if (index < 0 || index >= rankRewardNum.Count)
                return;
            UI_JSBattle_RewardCeil entry = cell.mUserData as UI_JSBattle_RewardCeil;
            uint up = 0;
            if(index - 1 >= 0)
            {
                up = rankRewardNum[index - 1];
            }
            entry.SetData(up, rankRewardNum[index], rewardId[index]);
        }

        public void CloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_JSBattle_Reward);
        }

    }
}