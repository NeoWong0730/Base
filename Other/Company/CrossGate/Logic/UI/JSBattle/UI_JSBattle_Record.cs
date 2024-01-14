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
    public class UI_JSBattle_Record_Layout
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

    public class UI_JSBattle_Record : UIBase, UI_JSBattle_Record_Layout.IListener
    {
        private UI_JSBattle_Record_Layout layout = new UI_JSBattle_Record_Layout();
        List<VictoryArenaFightRecord> victoryArenaFightRecords;
        public class UI_JSBattle_RecordCell
        {
            private GameObject winGo;
            private GameObject loseGo;
            private Text upRankNumText;
            private Text lowRankNumText;
            private GameObject attStateGo;
            private GameObject DefStateGo;
            private Text timeText;
            private Image headImage;
            private Text nameText;
            private Text levelText;
            private Text pointText;
            private Text careerText;
            private Image careerImage;
            private Button challengeBtn;
            private VictoryArenaFightRecord victoryArenaFightRecord;
            public void Init(Transform transform)
            {
                winGo = transform.Find("State/Win").gameObject;
                upRankNumText = transform.Find("State/Win/Num").GetComponent<Text>();
                loseGo = transform.Find("State/Lose").gameObject;
                lowRankNumText = transform.Find("State/Lose/Num").GetComponent<Text>();
                attStateGo = transform.Find("State1/Att").gameObject;
                DefStateGo = transform.Find("State1/Def").gameObject;
                timeText = transform.Find("Time").GetComponent<Text>();
                headImage = transform.Find("Head").GetComponent<Image>();
                nameText = transform.Find("Name").GetComponent<Text>();
                levelText = transform.Find("LV").GetComponent<Text>();
                pointText = transform.Find("Point").GetComponent<Text>();
                careerText = transform.Find("Profession").GetComponent<Text>();
                careerImage = transform.Find("Image_Profession").GetComponent<Image>();
                challengeBtn = transform.Find("Btn_01_Small").GetComponent<Button>();
                challengeBtn.onClick.AddListener(ChallengeBtnClicked);
            }

            public void SetData(VictoryArenaFightRecord data, int index)
            {
                if (null != data)
                {
                    winGo.SetActive(data.Win);
                    loseGo.SetActive(!data.Win);
                    if(data.Win)
                    {
                        var isUp = data.RankAfter < data.RankBefore;
                        if(isUp)
                        {
                            upRankNumText.text = (data.RankBefore - data.RankAfter).ToString();
                        }
                        for (int i = 0; i < winGo.transform.childCount; i++)
                        {
                            winGo.transform.GetChild(i).gameObject.SetActive(isUp);
                        }
                    }
                    else
                    {
                        var islow = data.RankAfter > data.RankBefore;
                        if (islow)
                        {
                            lowRankNumText.text = (data.RankAfter - data.RankBefore).ToString();
                        }
                        for (int i = 0; i < loseGo.transform.childCount; i++)
                        {
                            loseGo.transform.GetChild(i).gameObject.SetActive(islow);
                        }
  
                    }
                    challengeBtn.gameObject.SetActive(!data.IsAttacker && !data.Win && Sys_JSBattle.Instance.CheckCanFightRecord(data.Oppo.RoleId, index));
                    var serverTime = Sys_Time.Instance.GetServerTime();
                    timeText.text = LanguageHelper.TimeToString(serverTime - data.Timestamp, LanguageHelper.TimeFormat.Type_11);
                    attStateGo.SetActive(data.IsAttacker);
                    DefStateGo.SetActive(!data.IsAttacker);
                    victoryArenaFightRecord = data;
                    TextHelper.SetText(nameText, data.Oppo.Name.ToStringUtf8());
                    TextHelper.SetText(levelText, 2024712u, data.Oppo.Level.ToString());
                    TextHelper.SetText(pointText, 2024713u, data.Oppo.Score.ToString());
                    CSVCareer.Data cSVCareerData = CSVCareer.Instance.GetConfData(data.Oppo.Career);
                    CharacterHelper.SetHeadAndFrameData(headImage, (uint)data.Oppo.HeroId, data.Oppo.Head, data.Oppo.HeadFrame);

                    if (null != cSVCareerData)
                    {
                        TextHelper.SetText(careerText, cSVCareerData.name);

                        ImageHelper.SetIcon(careerImage, cSVCareerData.logo_icon);
                    }
                    else
                    {
                        DebugUtil.LogError($"Not Find {data.Oppo.Career} In CSVCareer");
                    }
                }
            }

            private void ChallengeBtnClicked()
            {
                if(null != victoryArenaFightRecord)
                {
                    RoleVictoryArenaDaily roleVictoryArenaDaily = Sys_JSBattle.Instance.GetRoleVictoryArenaDaily();
                    if (null != roleVictoryArenaDaily)
                    {
                        if (roleVictoryArenaDaily.LeftChallengeTimes > 0)
                        {
                            if (Sys_Team.Instance.HaveTeam)
                            {
                                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2024703));
                            }
                            else
                            {
                                Sys_JSBattle.Instance.VictoryArenaFightBackReq(victoryArenaFightRecord.Oppo.RoleId);
                            }
                        }
                        else
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2024707));
                            if (roleVictoryArenaDaily.LeftBuyTimes > 0)
                            {
                                var buyCostData = CSVDecisiveArenaParam.Instance.GetConfData(8);
                                var buyGetTimesData = CSVDecisiveArenaParam.Instance.GetConfData(9);

                                if (null != buyCostData && null != buyGetTimesData)
                                {
                                    List<uint> costList = ReadHelper.ReadArray_ReadUInt(buyCostData.str_value, '|');
                                    if (costList.Count >= 2)
                                    {
                                        ItemIdCount itemIdCount = new ItemIdCount(costList[0], costList[1]);
                                        string str = LanguageHelper.GetTextContent(2024701, itemIdCount.count.ToString(), LanguageHelper.GetTextContent(itemIdCount.CSV.name_id), buyGetTimesData.str_value, roleVictoryArenaDaily.LeftBuyTimes.ToString());
                                        PromptBoxParameter.Instance.OpenPromptBox(str, 0,
                                        () => {
                                            if (itemIdCount.Enough)
                                            {
                                                Sys_JSBattle.Instance.VictoryArenaBuyChallengeTimesReq();
                                            }
                                            else
                                            {
                                                Sys_Bag.Instance.TryOpenExchangeCoinUI(ECurrencyType.GoldCoin, itemIdCount.count);
                                            }
                                        });
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_JSBattle.Instance.eventEmitter.Handle(Sys_JSBattle.EEvents.RecordDataRefresh, RefreshFightRecord, toRegister);
        }

        protected override void OnShow()
        {
            layout.SetInfinityGridCell(0);
            Sys_JSBattle.Instance.VictoryArenaPullFightRecordReq();
        }

        private void RefreshFightRecord()
        {
            victoryArenaFightRecords = Sys_JSBattle.Instance.GetVictoryArenaFightRecords();
            layout.SetInfinityGridCell(victoryArenaFightRecords.Count);
        }

        

        /// <summary>
        /// 滚动列表创建回调
        /// </summary>
        /// <param name="cell"></param>
        public void OnCreateCell(InfinityGridCell cell)
        {
            UI_JSBattle_RecordCell entry = new UI_JSBattle_RecordCell();
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
            if (index < 0 || index >= victoryArenaFightRecords.Count)
                return;
            UI_JSBattle_RecordCell entry = cell.mUserData as UI_JSBattle_RecordCell;

            entry.SetData(victoryArenaFightRecords[index], index);
        }

        public void CloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_JSBattle_Record);
        }
    }
}