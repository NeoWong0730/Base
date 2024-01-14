using System.Collections;
using System.Collections.Generic;
using Table;
using Net;
using Packet;
using Lib.Core;
using UnityEngine;
using Logic.Core;
using static Packet.CmdCookPrepareConfirmNtf.Types;
using static Packet.CmdCookCookReq.Types;
using Google.Protobuf.Collections;
using System;

namespace Logic
{
    public partial class Sys_Cooking : SystemModuleBase<Sys_Cooking>, ISystemModuleUpdate
    {
        //静态数据,空间换时间,避免页签筛选的时候不断对数据remove add操作
        public List<Cooking> cookings = new List<Cooking>();
        public List<Cooking> type_1 = new List<Cooking>();      //主食
        public List<Cooking> type_2 = new List<Cooking>();      //炒菜
        public List<Cooking> type_3 = new List<Cooking>();      //汤
        public List<Cooking> type_4 = new List<Cooking>();      //点心

        public List<Cooking> cookings_Special = new List<Cooking>();
        public List<Cooking> type_1_Special = new List<Cooking>();
        public List<Cooking> type_2_Special = new List<Cooking>();
        public List<Cooking> type_3_Special = new List<Cooking>();
        public List<Cooking> type_4_Special = new List<Cooking>();

        private Dictionary<uint, Cooking> m_CookingMap = new Dictionary<uint, Cooking>();//用于检索时候用

        //ntf
        private List<uint> m_ConfigRewards = new List<uint>();  //所有烹饪奖励id
        private List<uint> m_RewardsGet = new List<uint>();     //已经获取的烹饪奖励id
        public List<uint> showRewards = new List<uint>();    //需要显示的奖励列表(已排序)
        private Dictionary<uint, bool> m_ShowRewardsMap = new Dictionary<uint, bool>(); //奖励映射
        public uint curScore;                                   //当前积分
        public uint lastCookId;                                 //上一次做出的料理
        public uint lastMultiCookId;                            //上一次做出的多人料理

        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public List<ulong> roles = new List<ulong>();       //多人烹饪 角色列表
        public bool bIsCookingCaptain = true;
        private CmdCookPrepareConfirmNtf m_CmdCookPrepareConfirmNtf = new CmdCookPrepareConfirmNtf();
        public CmdCookPrepareConfirmNtf cookingMember
        {
            get { return m_CmdCookPrepareConfirmNtf; }
            set
            {
                if (m_CmdCookPrepareConfirmNtf != value)
                {
                    m_CmdCookPrepareConfirmNtf = value;
                    //UpdateCookingMember();
                }
            }
        }

        private CmdCookPrepareNtf m_CmdCookPrepareNtf = new CmdCookPrepareNtf();
        public CmdCookPrepareNtf cookPrepareMenber
        {
            get { return m_CmdCookPrepareNtf; }
            set
            {
                if (m_CmdCookPrepareNtf != value)
                {
                    m_CmdCookPrepareNtf = value;
                    UpdateCookingPrepareMember();
                }
            }
        }


        public List<CookStage> cookStages = new List<CookStage>(); //多人烹饪阶段


        public enum EEvents
        {
            OnUpdateBookRightSubmitState,   //更新图册右侧上交道具
            OnUpdateCookPrepareConfirmState,//更新多人烹饪准备阶段各玩家状态
            OnCookSelectRes,                //队长选择完食谱返回
            OnExchangePosition,             //设置成员(换位)
            //OnFireOnNtf,                  //开始烹饪通知
            OnFireOffNtf,                   //起锅通知
            OnUpdateScore,                  //更新积分
            OnGetReward,                    //获取奖励
            OnActiveCook,                   //激活料理
            OnRefreshWatchState,            //更新关注状态
            OnRefreshLeftSubmitRedPoint,    //更新图册上交红点
            OnCookEndPlay,                  //烹饪结束
            OnClearMidSelection,            //清除单人烹饪选中数据
        }


        public override void Init()
        {
            ParseClientData();
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdCook.DataNtf, OnDataNtf, CmdCookDataNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdCook.WatchReq, (ushort)CmdCook.WatchRes, OnWatchRes, CmdCookWatchRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdCook.CollectReq, (ushort)CmdCook.CollectRes, CookCollectRes, CmdCookCollectRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdCook.GetRewardReq, (ushort)CmdCook.GetRewardRes, CookGetRewardRes, CmdCookGetRewardRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdCook.ScoreUpdateNtf, ScoreUpdateNtf, CmdCookScoreUpdateNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdCook.PrepareConfirmNtf, OnPrepareConfirmNtf, CmdCookPrepareConfirmNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdCook.PrepareNtf, OnPrepareNtf, CmdCookPrepareNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdCook.SelectNtf, OnSelectNtf, CmdCookSelectNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdCook.ExchangeConfirmNtf, OnExchangeConfirmNtf, CmdCookExchangeConfirmNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdCook.ExchangeNtf, OnExchangeNtf, CmdCookExchangeNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdCook.ExchangeRefuseNtf, ExchangeRefuseNtf, CmdCookExchangeRefuseNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdCook.CancelNtf, OnCookCancelNtf, CmdCookCancelNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdCook.CookNtf, OnCookCookNtf, CmdCookCookNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdCook.FireOnNtf, OnFireOnNtf, CmdCookFireOnNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdCook.FireOffNtf, OnFireOffNtf, CmdCookFireOffNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdCook.CookEndNtf, OnCookEndNtf, CmdCookCookEndNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdCook.ReturnPrepareNtf, OnCookReturnPrepareNtf, CmdCookReturnPrepareNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdCook.UseingFoodNtf, OnCookUseingFoodNtf, CmdCookUseingFoodNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdCook.UseFoodStateNtf, OnCookUseFoodStateNtf, CmdCookUseFoodStateNtf.Parser);
        }

        public override void OnLogin()
        {
            ClearCookingData();
            //CookBookDataReq();
            usingCookings.Clear();
            m_ScaleUsingCooking = null;
        }

        private void ParseClientData()
        {
            cookings.Clear();
            m_ConfigRewards.Clear();
            m_CookingMap.Clear();

            var cooks = CSVCook.Instance.GetAll();
            for (int i = 0, len = cooks.Count; i < len; i++)
            {
                uint id = cooks[i].id;
                Cooking cooking = new Cooking(cooks[i]);
                cookings.Add(cooking);
                m_CookingMap.Add(id, cooking);

                if (cooking.cSVCookData.is_special)
                {
                    cookings_Special.Add(cooking);
                }

                if (cooking.foodType == 1)
                {
                    type_1.Add(cooking);
                    if (cooking.cSVCookData.is_special)
                    {
                        type_1_Special.Add(cooking);
                    }
                }

                else if (cooking.foodType == 2)
                {
                    type_2.Add(cooking);
                    if (cooking.cSVCookData.is_special)
                    {
                        type_2_Special.Add(cooking);
                    }
                }

                else if (cooking.foodType == 3)
                {
                    type_3.Add(cooking);
                    if (cooking.cSVCookData.is_special)
                    {
                        type_3_Special.Add(cooking);
                    }
                }

                else if (cooking.foodType == 4)
                {
                    type_4.Add(cooking);
                    if (cooking.cSVCookData.is_special)
                    {
                        type_4_Special.Add(cooking);
                    }
                }
            }

            //for (int i = 0; i < CSVCookBookReward.Instance.Count; i++)
            //{
            //    m_ConfigRewards.Add(CSVCookBookReward.Instance[i].id);
            //}
            m_ConfigRewards.AddRange(CSVCookBookReward.Instance.GetKeys());

            maxCookingLevel = CSVCookLv.Instance.Count;
        }

        private void ClearCookingData()
        {
            for (int i = 0; i < cookings.Count; i++)
            {
                cookings[i].ClearData();
            }
        }

        private void OnDataNtf(NetMsg netMsg)
        {
            m_RewardsGet.Clear();
            CmdCookDataNtf cmdCookDataNtf = NetMsgUtil.Deserialize<CmdCookDataNtf>(CmdCookDataNtf.Parser, netMsg);

            curScore = cmdCookDataNtf.Score;
            UpdateCurCookingLevel();

            for (int i = 0; i < cmdCookDataNtf.BookItems.Count; i++)
            {
                Cooking cooking = GetCooking(cmdCookDataNtf.BookItems[i].CookId);
                cooking.SetSubmitItems(cmdCookDataNtf.BookItems[i].SubmitIds);
            }

            for (int i = 0; i < cmdCookDataNtf.ReceiveRewardIds.Count; i++)
            {
                m_RewardsGet.Add(cmdCookDataNtf.ReceiveRewardIds[i]);
            }

            for (int i = 0; i < cmdCookDataNtf.WatchCookIds.Count; i++)
            {
                Cooking cooking = GetCooking(cmdCookDataNtf.WatchCookIds[i]);
                cooking.SetWatch(true);
            }

            lastCookId = cmdCookDataNtf.LastCookId;
            lastMultiCookId = cmdCookDataNtf.LastMultiCookId;

            UpdateAllCookingSubmitState();
            UpdateRewards();
        }

        public void WatchReq(uint cookID, bool watch)
        {
            CmdCookWatchReq req = new CmdCookWatchReq();
            req.CookId = cookID;
            req.Watch = watch;
            NetClient.Instance.SendMessage((ushort)CmdCook.WatchReq, req);
        }

        private void OnWatchRes(NetMsg netMsg)
        {
            CmdCookWatchRes res = NetMsgUtil.Deserialize<CmdCookWatchRes>(CmdCookWatchRes.Parser, netMsg);
            Cooking cooking = GetCooking(res.CookId);
            cooking.SetWatch(res.Watch);
            eventEmitter.Trigger<uint>(EEvents.OnRefreshWatchState, res.CookId);
        }

        /// <summary>
        /// 上交请求
        /// </summary>
        /// <param name="cookId"></param>
        /// <param name="submitId"></param>
        public void CookCollectReq(uint cookId, uint submitId)
        {
            CmdCookCollectReq cmdCookCollectReq = new CmdCookCollectReq();
            cmdCookCollectReq.CookId = cookId;
            cmdCookCollectReq.SubmitId = submitId;
            NetClient.Instance.SendMessage((ushort)CmdCook.CollectReq, cmdCookCollectReq);
        }

        private void CookCollectRes(NetMsg netMsg)
        {
            CmdCookCollectRes cmdCookCollectRes = NetMsgUtil.Deserialize<CmdCookCollectRes>(CmdCookCollectRes.Parser, netMsg);
            Cooking cooking = GetCooking(cmdCookCollectRes.CookId);
            cooking.AddSubmitItem(cmdCookCollectRes.SubmitId);
            if (cmdCookCollectRes.CookActive)
            {
                string content = LanguageHelper.GetTextContent(5939, LanguageHelper.GetTextContent(cooking.cSVCookData.name));
                DebugUtil.Log(ELogType.eCooking, content);
                Sys_Hint.Instance.PushContent_Normal(content);
            }
        }

        private void ScoreUpdateNtf(NetMsg netMsg)
        {
            CmdCookScoreUpdateNtf cmdCookScoreUpdateNtf = NetMsgUtil.Deserialize<CmdCookScoreUpdateNtf>(CmdCookScoreUpdateNtf.Parser, netMsg);
            curScore = cmdCookScoreUpdateNtf.Score;
            UpdateCurCookingLevel();
            eventEmitter.Trigger(EEvents.OnUpdateScore);
        }

        /// <summary>
        /// 获取奖励请求
        /// </summary>
        /// <param name="rewardId"></param>
        public void CookGetRewardReq(uint rewardId)
        {
            CmdCookGetRewardReq cmdCookGetRewardReq = new CmdCookGetRewardReq();
            cmdCookGetRewardReq.RewardId = rewardId;
            NetClient.Instance.SendMessage((ushort)CmdCook.GetRewardReq, cmdCookGetRewardReq);
        }

        private void CookGetRewardRes(NetMsg netMsg)
        {
            CmdCookGetRewardRes cmdCookGetRewardRes = NetMsgUtil.Deserialize<CmdCookGetRewardRes>(CmdCookGetRewardRes.Parser, netMsg);
            m_RewardsGet.Add(cmdCookGetRewardRes.RewardId);
            m_ShowRewardsMap[cmdCookGetRewardRes.RewardId] = true;
            eventEmitter.Trigger<uint>(EEvents.OnGetReward, cmdCookGetRewardRes.RewardId);
            UpdateAllCookingSubmitState();
            eventEmitter.Trigger(EEvents.OnRefreshLeftSubmitRedPoint);
            eventEmitter.Trigger(EEvents.OnUpdateBookRightSubmitState);
        }

        /// <summary>
        /// 队长组队发起请求
        /// </summary>
        /// <param name="multi"></param>
        public void CookPrepareReq(bool multi)
        {
            CmdCookPrepareReq cmdCookPrepareReq = new CmdCookPrepareReq();
            cmdCookPrepareReq.Multi = multi;
            NetClient.Instance.SendMessage((ushort)CmdCook.PrepareReq, cmdCookPrepareReq);
        }

        private void OnPrepareNtf(NetMsg netMsg)
        {
            CmdCookPrepareNtf cmdCookPrepareNtf = NetMsgUtil.Deserialize<CmdCookPrepareNtf>(CmdCookPrepareNtf.Parser, netMsg);
            cookPrepareMenber = cmdCookPrepareNtf;
            UIManager.OpenUI(EUIID.UI_Cooking_Loading);
        }

        public void PrepareConfirmOpReq(uint op)
        {
            CmdCookPrepareConfirmOpReq cmdCookPrepareConfirmOpReq = new CmdCookPrepareConfirmOpReq();
            cmdCookPrepareConfirmOpReq.Op = op;
            NetClient.Instance.SendMessage((ushort)CmdCook.PrepareConfirmOpReq, cmdCookPrepareConfirmOpReq);
        }

        private void OnPrepareConfirmNtf(NetMsg netMsg)
        {
            CmdCookPrepareConfirmNtf res = NetMsgUtil.Deserialize<CmdCookPrepareConfirmNtf>(CmdCookPrepareConfirmNtf.Parser, netMsg);
            cookingMember = res;
            eventEmitter.Trigger(EEvents.OnUpdateCookPrepareConfirmState);
        }

        /// <summary>
        /// 队长食谱选择请求
        /// </summary>
        /// <param name="cookId"></param>
        public void CookSelectReq(uint cookId)
        {
            CmdCookSelectReq cmdCookSelectReq = new CmdCookSelectReq();
            cmdCookSelectReq.CookId = cookId;
            NetClient.Instance.SendMessage((ushort)CmdCook.SelectReq, cmdCookSelectReq);
        }

        private void OnSelectNtf(NetMsg netMsg)
        {
            cookStages.Clear();
            CmdCookSelectNtf res = NetMsgUtil.Deserialize<CmdCookSelectNtf>(CmdCookSelectNtf.Parser, netMsg);
            for (int i = 0; i < res.Mems.Count; i++)
            {
                CookStage cookStage = res.Mems[i];
                cookStages.Add(cookStage);
            }
            eventEmitter.Trigger<uint>(EEvents.OnCookSelectRes, res.CookId);
        }

        /// <summary>
        /// 换位请求
        /// </summary>
        public void CookExchangeReq(uint index)
        {
            CmdCookExchangeReq cmdCookExchangeReq = new CmdCookExchangeReq();
            cmdCookExchangeReq.TargetIndex = index;
            NetClient.Instance.SendMessage((ushort)CmdCook.ExchangeReq, cmdCookExchangeReq);
        }

        /// <summary>
        /// 队员收到换阶段请求
        /// </summary>
        /// <param name="netMsg"></param>
        private void OnExchangeConfirmNtf(NetMsg netMsg)
        {
            CmdCookExchangeConfirmNtf cmdCookExchangeConfirmNtf = NetMsgUtil.Deserialize<CmdCookExchangeConfirmNtf>(CmdCookExchangeConfirmNtf.Parser, netMsg);
            if (Sys_Role.Instance.RoleId == cmdCookExchangeConfirmNtf.FromId)
            {
                UIManager.OpenUI(EUIID.UI_TipsAutoClose, false, new Tuple<uint, uint>(1003071, CSVCookAttr.Instance.GetConfData(9).value));
            }
            else if (Sys_Role.Instance.RoleId == cmdCookExchangeConfirmNtf.TargetId)
            {
                TeamMem teamMem = Sys_Team.Instance.getTeamMem(cmdCookExchangeConfirmNtf.FromId);
                string reqMemName = teamMem.Name.ToStringUtf8();
                string content = LanguageHelper.GetTextContent(1003057, reqMemName);

                PromptBoxParameter.Instance.OpenPromptBox(content, 0,
                () =>
                {
                    CookExchangeOpReq(1);
                },
                () =>
                {
                    CookExchangeOpReq(0);
                }, PromptBoxParameter.ECountdown.Cancel, CSVCookAttr.Instance.GetConfData(9).value);
            }
        }

        /// <summary>
        /// 换位确认(拒绝 同意)
        /// </summary>
        private void CookExchangeOpReq(uint op)
        {
            CmdCookExchangeOpReq cmdCookExchangeOpReq = new CmdCookExchangeOpReq();
            cmdCookExchangeOpReq.Op = op;
            NetClient.Instance.SendMessage((ushort)CmdCook.ExchangeOpReq, cmdCookExchangeOpReq);
        }

        private void ExchangeRefuseNtf(NetMsg netMsg)
        {
            CmdCookExchangeRefuseNtf cmdCookExchangeRefuseNtf = NetMsgUtil.Deserialize<CmdCookExchangeRefuseNtf>(CmdCookExchangeRefuseNtf.Parser, netMsg);
            TeamMem teamMem = Sys_Team.Instance.getTeamMem(cmdCookExchangeRefuseNtf.TargetId);
            if (teamMem != null)
            {
                string reqMemName = teamMem.Name.ToStringUtf8();
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(5915, reqMemName));
            }
            if (UIManager.IsOpen(EUIID.UI_TipsAutoClose))
            {
                UIManager.CloseUI(EUIID.UI_TipsAutoClose);
            }
        }

        /// <summary>
        /// 换位返回
        /// </summary>
        /// <param name="netMsg"></param>
        private void OnExchangeNtf(NetMsg netMsg)
        {
            cookStages.Clear();
            CmdCookExchangeNtf cmdCookExchangeNtf = NetMsgUtil.Deserialize<CmdCookExchangeNtf>(CmdCookExchangeNtf.Parser, netMsg);
            if (Sys_Role.Instance.RoleId == cmdCookExchangeNtf.FromId)
            {
                UIManager.CloseUI(EUIID.UI_TipsAutoClose);
            }
            for (int i = 0; i < cmdCookExchangeNtf.Mems.Count; i++)
            {
                CookStage cookStage = cmdCookExchangeNtf.Mems[i];
                cookStages.Add(cookStage);
            }
            roles.Clear();
            for (int i = 0; i < cookStages.Count; i++)
            {
                roles.Add(cookStages[i].RoleId);
            }
            eventEmitter.Trigger(EEvents.OnExchangePosition);
        }

        /// <summary>
        /// 取消烹饪 cliData   0:multiprepare   1:cookingsatge
        /// </summary>
        public void CookCancelReq(uint cliData)
        {
            CmdCookCancelReq cmdCookCancelReq = new CmdCookCancelReq();
            cmdCookCancelReq.CliData = cliData;
            NetClient.Instance.SendMessage((ushort)CmdCook.CancelReq, cmdCookCancelReq);
        }

        private void OnCookCancelNtf(NetMsg netMsg)
        {
            CmdCookCancelNtf cmdCookCancelNtf = NetMsgUtil.Deserialize<CmdCookCancelNtf>(CmdCookCancelNtf.Parser, netMsg);
            TeamMem teamMem = Sys_Team.Instance.getTeamMem(cmdCookCancelNtf.RoleId);
            if (teamMem != null)
            {
                string reqMemName = teamMem.Name.ToStringUtf8();
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(5917, reqMemName));
            }
            //所有人界面关闭
            CancelCooking();
        }

        /// <summary>
        /// 点击烹饪开始
        /// </summary>
        public void CookCookReq(uint cookType, bool freeCook, uint cookId, List<FreeStage> freeStages,uint multCook=1)
        {
            DebugUtil.Log(ELogType.eCooking, "cookType: " + cookType + "  freeCook:  " + freeCook + "  cookId: " + cookId);
            CmdCookCookReq cmdCookCookReq = new CmdCookCookReq();
            cmdCookCookReq.CookType = cookType;
            cmdCookCookReq.CookId = cookId;
            cmdCookCookReq.FreeCook = freeCook;
            cmdCookCookReq.MultiNum = multCook;
            if (freeStages != null && freeStages.Count > 0)
            {
                for (int i = 0; i < freeStages.Count; i++)
                {
                    cmdCookCookReq.Stages.Add(freeStages[i]);
                }
            }
            NetClient.Instance.SendMessage((ushort)CmdCook.CookReq, cmdCookCookReq);
        }

        private void OnCookCookNtf(NetMsg netMsg)
        {
            eventEmitter.Trigger(EEvents.OnClearMidSelection);
            CmdCookCookNtf cmdCookCookNtf = NetMsgUtil.Deserialize<CmdCookCookNtf>(CmdCookCookNtf.Parser, netMsg);
            cookStages.Clear();
            for (int i = 0; i < cmdCookCookNtf.Stages.Count; i++)
            {
                CookStage cookStage = cmdCookCookNtf.Stages[i];
                cookStages.Add(cookStage);
            }
            if (cmdCookCookNtf.CookType == 1 || cmdCookCookNtf.CookType == 2)
            {
                //Sys_MagicBook.Instance.eventEmitter.Trigger(Sys_MagicBook.EEvents.MagicTaskCheckEvent, EMagicAchievement.Event49);
                //UIManager.CloseUI(EUIID.UI_Cooking_Single);
                bIsCookingCaptain = true;
            }
            else
            {
                //Sys_MagicBook.Instance.eventEmitter.Trigger(Sys_MagicBook.EEvents.MagicTaskCheckEvent, EMagicAchievement.Event50);
                //UIManager.CloseUI(EUIID.UI_Cooking_Multiple);
            }
            Tuple<uint, bool, uint> tuple = new Tuple<uint, bool, uint>(cmdCookCookNtf.CookId, cmdCookCookNtf.FreeCook, cmdCookCookNtf.CookType);
            UIManager.OpenUI(EUIID.UI_Cooking, false, tuple);
        }

        public void CookFireOnReq()
        {
            CmdCookFireOnReq cmdCookFireOnReq = new CmdCookFireOnReq();
            NetClient.Instance.SendMessage((ushort)CmdCook.FireOnReq, cmdCookFireOnReq);
        }

        private void OnFireOnNtf(NetMsg netMsg)
        {
            CmdCookFireOnNtf cmdCookFireOnNtf = NetMsgUtil.Deserialize<CmdCookFireOnNtf>(CmdCookFireOnNtf.Parser, netMsg);
        }

        public void CookFireOffReq(uint fireValue)
        {
            CmdCookFireOffReq cmdCookFireOffReq = new CmdCookFireOffReq();
            cmdCookFireOffReq.FireValue = fireValue;
            NetClient.Instance.SendMessage((ushort)CmdCook.FireOffReq, cmdCookFireOffReq);
        }

        private void OnFireOffNtf(NetMsg netMsg)
        {
            CmdCookFireOffNtf cmdCookFireOffNtf = NetMsgUtil.Deserialize<CmdCookFireOffNtf>(CmdCookFireOffNtf.Parser, netMsg);
            eventEmitter.Trigger<uint, uint>(EEvents.OnFireOffNtf, cmdCookFireOffNtf.CookIndex, cmdCookFireOffNtf.StageScore);
        }

        private void OnCookEndNtf(NetMsg netMsg)
        {
            CmdCookCookEndNtf cmdCookCookEndNtf = NetMsgUtil.Deserialize<CmdCookCookEndNtf>(CmdCookCookEndNtf.Parser, netMsg);

            if (cmdCookCookEndNtf.Multi)
            {
                lastMultiCookId = cmdCookCookEndNtf.LastCookId;
            }
            else
            {
                lastCookId = cmdCookCookEndNtf.LastCookId;
                eventEmitter.Trigger(EEvents.OnCookEndPlay);
            }
            if (cmdCookCookEndNtf.ForceEnd)
            {
                if (UIManager.IsOpen(EUIID.UI_Cooking))
                {
                    UIManager.CloseUI(EUIID.UI_Cooking);
                }
                if (UIManager.IsOpen(EUIID.UI_Cooking_Single))
                {
                    UIManager.CloseUI(EUIID.UI_Cooking_Single);
                }
            }
            eventEmitter.Trigger<uint, uint>(EEvents.OnCookEndPlay, cmdCookCookEndNtf.CookId, cmdCookCookEndNtf.CookScore);
        }

        public void CookReturnPrepareReq()
        {
            CmdCookReturnPrepareReq cmdCookReturnPrepareReq = new CmdCookReturnPrepareReq();
            NetClient.Instance.SendMessage((ushort)CmdCook.ReturnPrepareReq, cmdCookReturnPrepareReq);
#if UNITY_EDITOR
            DebugUtil.LogWarning("CookReturnPrepareReq");
#endif
        }

        private void OnCookReturnPrepareNtf(NetMsg netMsg)
        {
#if UNITY_EDITOR
            DebugUtil.LogWarning("OnCookReturnPrepareNtf");
#endif
            CmdCookReturnPrepareNtf cmdCookCookEndNtf = NetMsgUtil.Deserialize<CmdCookReturnPrepareNtf>(CmdCookReturnPrepareNtf.Parser, netMsg);
            UIManager.CloseUI(EUIID.UI_Cooking);
        }

        private void OnCookUseingFoodNtf(NetMsg netMsg)
        {
            CmdCookUseingFoodNtf cmdCookUseingFoodNtf = NetMsgUtil.Deserialize<CmdCookUseingFoodNtf>(CmdCookUseingFoodNtf.Parser, netMsg);
            for (int i = 0; i < cmdCookUseingFoodNtf.Food.Count; i++)
            {
                uint itemId = cmdCookUseingFoodNtf.Food[i].FoodId;
                CSVItem.Data cSVItemData = CSVItem.Instance.GetConfData(itemId);
                if (cSVItemData.fun_parameter == "changeScale")
                {
                    if (m_ScaleUsingCooking == null)
                    {
                        m_ScaleUsingCooking = new ChangeScaleCooking();
                    }
                    m_ScaleUsingCooking.SetData(itemId, cmdCookUseingFoodNtf.Food[i].EndTime);
                    if (cSVItemData.fun_value[1] > 100)
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(20004));
                    }
                    else if (cSVItemData.fun_value[1] < 100)
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(20005));
                    }
                }
                else if (cSVItemData.fun_parameter == "addPassiveSkill")
                {
                    if (!usingCookings.TryGetValue(cSVItemData.type_id, out AttrCooking usingCooking))
                    {
                        AttrCooking ac = new AttrCooking();
                        ac.SetData(itemId, cmdCookUseingFoodNtf.Food[i].EndTime);
                        usingCookings.Add(cSVItemData.type_id, ac);
                    }
                    else
                    {
                        usingCooking.SetData(itemId, cmdCookUseingFoodNtf.Food[i].EndTime);
                    }
                }
            }
        }

        private void OnCookUseFoodStateNtf(NetMsg netMsg)
        {
            CmdCookUseFoodStateNtf cmdCookUseFoodStateNtf = NetMsgUtil.Deserialize<CmdCookUseFoodStateNtf>(CmdCookUseFoodStateNtf.Parser, netMsg);
            Hero hero = GameCenter.otherActorsDic[cmdCookUseFoodStateNtf.RoleId];
            if (hero == null)
            {
                return;
            }
            if (cmdCookUseFoodStateNtf.FoodId == 0)
            {
                hero.ResetModelScale();
                Sys_HUD.Instance.eventEmitter.Trigger<ulong>(Sys_HUD.EEvents.OnResetScale, hero.UID);
            }
            else
            {
                CSVItem.Data cSVItemData = CSVItem.Instance.GetConfData(cmdCookUseFoodStateNtf.FoodId);
                if (cSVItemData != null)
                {
                    uint rat_X = cSVItemData.fun_value[0];
                    uint rat_Y = cSVItemData.fun_value[1];
                    uint rat_Z = cSVItemData.fun_value[2];
                    hero.ChangeModelScale(rat_X / 100f, rat_Y / 100f, rat_Z / 100f);

                    if (rat_Y >= 100)
                    {
                        Sys_HUD.Instance.eventEmitter.Trigger<ulong, uint>(Sys_HUD.EEvents.OnScaleUp, hero.UID, rat_Y);
                    }
                    else if (rat_Y < 100)
                    {
                        Sys_HUD.Instance.eventEmitter.Trigger<ulong>(Sys_HUD.EEvents.OnResetScale, hero.UID);
                    }
                }
            }
        }

        public void OnUpdate()
        {
            if (m_ScaleUsingCooking != null)
            {
                m_ScaleUsingCooking.Update();
            }

#if UNITY_EDITOR
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.L))
            {
                Sys_Cooking.Instance.StartCooking(1);
                //UIManager.OpenUI(EUIID.UI_Knowledge_Cooking1);
            }
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.K))
            {
                Sys_Cooking.Instance.StartCooking(4);
            }
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.J))
            {
                UIManager.OpenUI(EUIID.UI_ElementalCrystal);
            }
            if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.H))
            {
                UIManager.OpenUI(EUIID.UI_ElementalCrystal_Exchange);
            }
#endif
        }
    }

}


