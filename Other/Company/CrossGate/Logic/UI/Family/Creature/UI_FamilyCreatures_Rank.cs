using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using Table;
using System;
using UnityEngine.UI;
using static Packet.CmdGuildPetQueryRankRes.Types;
using Packet;

namespace Logic
{
    public class UI_FamilyCreatureRankGrid
    {
        private Image rankNumImage;
        private Text rankNumText;
        private Text text01;
        private Text text02;
        private CSVFamilyPetTrainingRankReward.Data data;
        private List<ItemIdCount> items = new List<ItemIdCount>();
        /// <summary> 无限滚动 </summary>
        private InfinityGrid infinityGrid;
        
        public void Init(Transform transform)
        {
            rankNumImage = transform.Find("Rank/Image_Icon").GetComponent<Image>();
            rankNumText = transform.Find("Rank/Text_Rank").GetComponent<Text>();
            text01 = transform.Find("Text_Name").GetComponent<Text>();
            text02 = transform.Find("Text_Point").GetComponent<Text>();
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
            entry.SetData(itemData, EUIID.UI_FamilyCreatures_Reward);
            ImageHelper.SetImageGray(entry.transform, !Sys_Family.Instance.isCanGetTrainStarReward, true);
        }

        public void SetData(RankInfo rankData, CSVFamilyPetTrainingRankReward.Data data, int rank)
        {
            this.data = data;
            SetRankIcon(rank);
            if (null != rankData)
            {
                text01.text = rankData.Name.ToStringUtf8();
                text02.text = rankData.Score.ToString();
            }
            else
            {
                text01.text = Sys_Role.Instance.Role.Name.ToStringUtf8();
                text02.text = "0";
            }
            if (null != data)
            {
                items.Clear();
                items = CSVDrop.Instance.GetDropItem(data.drop_id);
                infinityGrid.CellCount = items.Count;
                infinityGrid.ForceRefreshActiveCell();
            }
            else
            {
                items.Clear();
                infinityGrid.CellCount = items.Count;
                infinityGrid.ForceRefreshActiveCell();
            }

        }

        private void SetRankIcon(int rank)
        {
            uint iconId = Sys_Rank.Instance.GetRankIcon(rank);
            bool isShowIcon = iconId != 0;
            rankNumImage.gameObject.SetActive(isShowIcon);
            rankNumText.gameObject.SetActive(!isShowIcon);
            if (rank > 0)
            {
                if (isShowIcon)
                {
                    ImageHelper.SetIcon(rankNumImage, iconId);
                }
                else
                {
                    rankNumText.text = rank.ToString();
                }
            }
            else
            {
                rankNumText.text = LanguageHelper.GetTextContent(2023807);
            }
        }
    }

    public class UI_FamilyCreatures_Rank_Layout
    {
        private Button closeBtn;
        private Text starText;
        private Text pointNeedText;
        /// <summary> 无限滚动 </summary>
        private InfinityGrid infinityGrid;
        private UI_FamilyCreatureRankGrid mySelf;

        public void Init(Transform transform)
        {
            infinityGrid = transform.Find("Animator/Scroll_Rank").GetComponent<InfinityGrid>();
            mySelf = new UI_FamilyCreatureRankGrid();
            mySelf.Init(transform.Find("Animator/MyRank"));
            closeBtn = transform.Find("Animator/View_TipsBgNew05/Btn_Close").GetComponent<Button>();
            pointNeedText = transform.Find("Animator/Text_Explain").GetComponent<Text>();
            starText = transform.Find("Animator/StarLevel/Text_Value").GetComponent<Text>();
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.CloseBtnClicked);
            infinityGrid.onCreateCell += listener.OnCreateCell;
            infinityGrid.onCellChange += listener.OnCellChange;
        }

        public void SetCurrentPointExpLain(uint value)
        {
            pointNeedText.text = LanguageHelper.GetTextContent(2023802, value.ToString());
        }

        public void SetStarText(GuildPetTraining guildPetTraining)
        {
            starText.text = "x" + GetCurrentStar(guildPetTraining).ToString();
        }

        private List<CSVFamilyPetTrainingStarReward.Data>  datas = new List<CSVFamilyPetTrainingStarReward.Data>(5);
        private int GetCurrentStar(GuildPetTraining guildPetTraining)
        {
            if (null != guildPetTraining)
            {
                datas.Clear();
                for (int i = 1; i < 99; i++)
                {
                    CSVFamilyPetTrainingStarReward.Data cSVFamilyPetTrainingStarReward = CSVFamilyPetTrainingStarReward.Instance.GetConfData((uint)(guildPetTraining.TrainingStage * 10 + i));
                    if (null != cSVFamilyPetTrainingStarReward)
                    {
                        datas.Add(cSVFamilyPetTrainingStarReward);
                    }
                    else
                    {
                        break;
                    }
                }

                for (int i = datas.Count - 1; i >= 0; i--)
                {
                    if (Sys_Family.Instance.totalScore >= datas[i].trainingIntegralCondition)
                    {
                        return (int)datas[i].trainingStar;
                    }
                }
            }
            return 0;
        }

        public void SetInfinityGridCell(int count)
        {
            infinityGrid.CellCount = count;
            infinityGrid.ForceRefreshActiveCell();
        }

        public void SetMySelfInfo(RankInfo rankData, CSVFamilyPetTrainingRankReward.Data data, int rank)
        {
            mySelf.SetData(rankData, data, rank);
        }

        public interface IListener
        {
            void CloseBtnClicked();
            void OnCreateCell(InfinityGridCell cell);
            void OnCellChange(InfinityGridCell cell, int index);
        }
    }

    public class UI_FamilyCreatures_Rank : UIBase, UI_FamilyCreatures_Rank_Layout.IListener
    {
        private UI_FamilyCreatures_Rank_Layout layout = new UI_FamilyCreatures_Rank_Layout();
        private uint currentCreatureId;
        private List<CSVFamilyPetTrainingRankReward.Data>  datas = new List<CSVFamilyPetTrainingRankReward.Data>(5);
        private List<RankInfo> rankDatas = new List<RankInfo>();
        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.OnFamilyRankInfoBack, Refresh, toRegister);
        }

        protected override void OnOpen(object arg = null)
        {
            
        }

        private CSVFamilyPetTrainingRankReward.Data GetRankRewardInfo(int rank)
        {
            for (int i = 0; i < datas.Count; i++)
            {
                CSVFamilyPetTrainingRankReward.Data temp = datas[i];
                if(null != temp.rankingRange && temp.rankingRange.Count >= 2)
                {
                    if(rank >= temp.rankingRange[0] && rank <= temp.rankingRange[1])
                    {
                        return temp;
                    }
                }
                else
                {
                    return null;
                }
            }
            return null;
        }

        protected override void OnShow()
        {
            Sys_Family.Instance.GuildPetQueryRankReq();
            Refresh();
        }

        private void Refresh()
        {
            rankDatas = Sys_Family.Instance.rankInfos;
            GetData();
            layout.SetInfinityGridCell(rankDatas.Count);
            int myIndex = GetMyRankInfoIndex();
            layout.SetMySelfInfo(GetMyRankInfo(), GetRankRewardInfo(myIndex + 1), myIndex + 1);
            
            GuildPetTraining guildPetTraining = Sys_Family.Instance.GetTrainInfo();
            if(null != guildPetTraining)
            {
                var valueD = CSVFamilyPet.Instance.GetConfData(guildPetTraining.TrainingStage);
                if(null!= valueD)
                {
                    layout.SetCurrentPointExpLain(valueD.trainingIntegralCondition);
                }
                else
                {
                    DebugUtil.LogError($"not find id = {guildPetTraining.TrainingStage} in CSVFamilyPet");
                }
                layout.SetStarText(guildPetTraining);
            }
        }

        public RankInfo GetMyRankInfo()
        {
            for (int i = 0; i < rankDatas.Count; i++)
            {
                var rank = rankDatas[i];
                if(rank.RoleId == Sys_Role.Instance.RoleId)
                {
                    return rank;
                }
            }
            return null;
        }

        public int GetMyRankInfoIndex()
        {
            int index = -1;
            for (int i = 0; i < rankDatas.Count; i++)
            {
                var rank = rankDatas[i];
                if (rank.RoleId == Sys_Role.Instance.RoleId)
                {
                    index = i;
                    break;
                }
            }
            return index;
        }

        private void GetData()
        {
            GuildPetTraining guildPetTraining = Sys_Family.Instance.GetTrainInfo();
            if(null != guildPetTraining)
            {
                currentCreatureId = guildPetTraining.TrainingStage;
                datas.Clear();
                for (int i = 0; i < 10; i++)
                {
                    CSVFamilyPetTrainingRankReward.Data cSVFamilyPetTrainingStarReward = CSVFamilyPetTrainingRankReward.Instance.GetConfData((uint)(currentCreatureId * 10 + i));
                    if (null != cSVFamilyPetTrainingStarReward)
                    {
                        datas.Add(cSVFamilyPetTrainingStarReward);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            
        }

        protected override void OnHide()
        {
        }

        protected override void OnDestroy()
        {

        }

        /// <summary>
        /// 滚动列表创建回调
        /// </summary>
        /// <param name="cell"></param>
        public void OnCreateCell(InfinityGridCell cell)
        {
            UI_FamilyCreatureRankGrid entry = new UI_FamilyCreatureRankGrid();
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
            if (index < 0 || index >= rankDatas.Count)
                return;
            UI_FamilyCreatureRankGrid entry = cell.mUserData as UI_FamilyCreatureRankGrid;

            entry.SetData(rankDatas[index], GetRankRewardInfo(index + 1), index + 1);
        }

        public void CloseBtnClicked()
        {
            UIManager.HitButton(EUIID.UI_FamilyCreatures_Rank, "CloseBtnClicked");
            UIManager.CloseUI(EUIID.UI_FamilyCreatures_Rank);
        }
    }
}