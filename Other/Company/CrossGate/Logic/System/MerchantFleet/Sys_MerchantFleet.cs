using System.Collections.Generic;
using Logic.Core;
using Packet;
using Net;
using Lib.Core;
using Table;
using Logic;
using System;
using UnityEngine;
using Framework;
using UnityEngine.UI;
using Google.Protobuf.Collections;

/// <summary>
/// 法兰商队
/// </summary>
public partial class Sys_MerchantFleet : SystemModuleBase<Sys_MerchantFleet>
{
    #region 系统函数
    private void ProcessEvents(bool toRegister)
    {
        if (toRegister)
        {
            EventDispatcher.Instance.AddEventListener((ushort)CmdMerchant.GetInfoReq, (ushort)CmdMerchant.GetInfoRes, OnMerchantGetInfoRes, CmdMerchantGetInfoRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdMerchant.AccpetTaskReq, (ushort)CmdMerchant.AccpetTaskRes, OnMerchantAccpetTaskRes, CmdMerchantAccpetTaskRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdMerchant.SubmitTradeTaskReq, (ushort)CmdMerchant.SubmitTradeTaskRes, OnMerchantSubmitTradeTaskRes, CmdMerchantSubmitTradeTaskRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdMerchant.LevelUpNtf, OnMerchantLevelUpNtf, CmdMerchantLevelUpNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdMerchant.TakeAwardReq, (ushort)CmdMerchant.TakeAwardRes, OnMerchantTakeAwardRes, CmdMerchantTakeAwardRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdMerchant.GetHelpListReq, (ushort)CmdMerchant.GetHelpListRes, OnMerchantHelpListRes, CmdMerchantGetHelpListRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdMerchant.SeekHelpReq, (ushort)CmdMerchant.SeekHelpRes, OnMerchantSeekHelpRes, CmdMerchantSeekHelpRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdMerchant.UpdateHelpNtf, OnMerchantUpdateHelpNtf, CmdMerchantUpdateHelpNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdMerchant.CancelHelpReq, (ushort)CmdMerchant.CancelHelpRes, OnMerchantCancelHelpRes, CmdMerchantCancelHelpRes.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdMerchant.SubmitHelpReq, (ushort)CmdMerchant.SubmitHelpRes, OnMerchantSubmitHelpRes, CmdMerchantSubmitHelpRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdMerchant.HelpStateNtf, OnMerchantHelpStateNtf, CmdMerchantHelpStateNtf.Parser);
            EventDispatcher.Instance.AddEventListener((ushort)CmdMerchant.ReceviceHelpReq, (ushort)CmdMerchant.ReceviceHelpRes, OnMerchantReceviceHelpRes, CmdMerchantReceviceHelpRes.Parser);
        }
        else
        {
            EventDispatcher.Instance.RemoveEventListener((ushort)CmdMerchant.GetInfoRes, OnMerchantGetInfoRes);
            EventDispatcher.Instance.RemoveEventListener((ushort)CmdMerchant.AccpetTaskRes, OnMerchantAccpetTaskRes);
            EventDispatcher.Instance.RemoveEventListener((ushort)CmdMerchant.SubmitTradeTaskRes, OnMerchantSubmitTradeTaskRes);
            EventDispatcher.Instance.RemoveEventListener((ushort)CmdMerchant.LevelUpNtf, OnMerchantLevelUpNtf);
            EventDispatcher.Instance.RemoveEventListener((ushort)CmdMerchant.TakeAwardRes, OnMerchantTakeAwardRes);
            EventDispatcher.Instance.RemoveEventListener((ushort)CmdMerchant.GetHelpListRes, OnMerchantHelpListRes);
            EventDispatcher.Instance.RemoveEventListener((ushort)CmdMerchant.SeekHelpRes, OnMerchantSeekHelpRes);
            EventDispatcher.Instance.RemoveEventListener((ushort)CmdMerchant.UpdateHelpNtf, OnMerchantUpdateHelpNtf);
            EventDispatcher.Instance.RemoveEventListener((ushort)CmdMerchant.CancelHelpRes, OnMerchantCancelHelpRes);
            EventDispatcher.Instance.RemoveEventListener((ushort)CmdMerchant.SubmitHelpRes, OnMerchantSubmitHelpRes);
            EventDispatcher.Instance.RemoveEventListener((ushort)CmdMerchant.HelpStateNtf, OnMerchantHelpStateNtf);
            EventDispatcher.Instance.RemoveEventListener((ushort)CmdMerchant.ReceviceHelpRes, OnMerchantReceviceHelpRes);
        }
        Sys_Task.Instance.eventEmitter.Handle<int, uint, TaskEntry>(Sys_Task.EEvents.OnSubmited, OnTaskSubmit, toRegister);
        Net_Combat.Instance.eventEmitter.Handle<CmdBattleEndNtf>(Net_Combat.EEvents.OnEndBattle, OnEndBattle, toRegister);
        Sys_FunctionOpen.Instance.eventEmitter.Handle<Sys_FunctionOpen.FunctionOpenData>(Sys_FunctionOpen.EEvents.CompletedFunctionOpen, OnCompletedFunctionOpen, toRegister);
        Sys_FunctionOpen.Instance.eventEmitter.Handle(Sys_FunctionOpen.EEvents.InitFinish, OnFuctionInitFinish, toRegister);
        Sys_FunctionOpen.Instance.eventEmitter.Handle(Sys_FunctionOpen.EEvents.UnLockAllFuntion, OnFuctionInitFinish, toRegister);
        Sys_Time.Instance.eventEmitter.Handle<uint, uint>(Sys_Time.EEvents.OnTimeNtf, OnTimeNtf, toRegister);
        Sys_Role.Instance.eventEmitter.Handle(Sys_Role.EEvents.OnUpdateOpenServiceDay, OnUpdateOpenServiceDay, toRegister);
    }
    public override void Init()
    {
        ProcessEvents(true);
    }
    public override void OnLogin()
    {
        base.OnLogin();
    }
    public override void OnLogout()
    {
        base.OnLogout();
        m_Timer?.Cancel();
        t_Timer?.Cancel();
        TradeTaskHelp = false;
        MerchantGradeAwardTake.Clear();
        MerchantHelpInfoList.Clear();
        MerchantHelpSelfList.Clear();
        TempBattleInfo.Clear();
    }

    public override void Dispose()
    {
        ProcessEvents(false);
    }
    #endregion
    #region net

    /// <summary>
    /// 获取商队信息
    /// </summary>
    public void OnMerchantGetInfoReq()
    {
        CmdMerchantGetInfoReq req = new CmdMerchantGetInfoReq();
        NetClient.Instance.SendMessage((ushort)CmdMerchant.GetInfoReq, req);
    }
    private void OnMerchantGetInfoRes(NetMsg msg)
    {
        CmdMerchantGetInfoRes res = NetMsgUtil.Deserialize<CmdMerchantGetInfoRes>(CmdMerchantGetInfoRes.Parser, msg);
        MerchantLevel = res.Level;
        MerchantExp = res.Exp;
        MerchantTotalCount = res.TradeTaskCount;
        MerchantTaskCount = res.TaskCount;
        MerchantIndex = (int)res.TradeTaskIndex;
        QuickFinishCount = res.QuickCount;
        HelpCount = res.HelpCount;
        InitLevelList(res.AwardTake);
        SeekHelpCount = res.SeekHelpCount;
        InitTaskState(res.TaskId);
        InitTableData();
        InitMerchantTradeTaskType();
        ReqTickTimer();
        NextFiveDayRefresh();
        eventEmitter.Trigger(EEvents.UpdateMerchantInfo);
    }
    /// <summary>
    /// 接受任务请求
    /// </summary>
    public void OnMerchantAccpetTaskReq()
    {
        CmdMerchantAccpetTaskReq req = new CmdMerchantAccpetTaskReq();
        NetClient.Instance.SendMessage((ushort)CmdMerchant.AccpetTaskReq, req);

    }

    private void OnMerchantAccpetTaskRes(NetMsg msg)
    {
        CmdMerchantAccpetTaskRes res = NetMsgUtil.Deserialize<CmdMerchantAccpetTaskRes>(CmdMerchantAccpetTaskRes.Parser, msg);
        MerchantTaskCount++;
        InitTaskState(res.TaskId);
        eventEmitter.Trigger(EEvents.UpdateMerchantInfo);
    }
    /// <summary>
    /// 提交贸易任务请求
    /// </summary>
    public void OnMerchantSubmitTradeTaskReq(bool isQuick)
    {
        CmdMerchantSubmitTradeTaskReq req = new CmdMerchantSubmitTradeTaskReq();
        req.Quick = isQuick;
        NetClient.Instance.SendMessage((ushort)CmdMerchant.SubmitTradeTaskReq, req);
    }
    public float sheepAnimationTime = 2.0f;
    private void OnMerchantSubmitTradeTaskRes(NetMsg msg)
    {
        CmdMerchantSubmitTradeTaskRes res = NetMsgUtil.Deserialize<CmdMerchantSubmitTradeTaskRes>(CmdMerchantSubmitTradeTaskRes.Parser, msg);
        OnSubmitRes();
    }
    /// <summary>
    /// 商队等级更新
    /// </summary>
    /// <param name="msg"></param>
    private void OnMerchantLevelUpNtf(NetMsg msg)
    {
        CmdMerchantLevelUpNtf ntf = NetMsgUtil.Deserialize<CmdMerchantLevelUpNtf>(CmdMerchantLevelUpNtf.Parser, msg);
        MerchantLevel = ntf.Level;
        MerchantExp = ntf.Exp;
        eventEmitter.Trigger(EEvents.UpdateMerchantInfo);
    }
    /// <summary>
    /// 进入贸易任务战斗请求
    /// </summary>
    public void OnMerchantFightTradeTaskReq()
    {
        CmdMerchantFightTradeTaskReq req = new CmdMerchantFightTradeTaskReq();
        NetClient.Instance.SendMessage((ushort)CmdMerchant.FightTradeTaskReq, req);
    }
    /// <summary>
    /// 请求领取等级奖励
    /// </summary>
    public void OnMerchantTakeAwardReq(uint infoId)
    {
        CmdMerchantTakeAwardReq req = new CmdMerchantTakeAwardReq();
        req.InfoId = infoId;
        NetClient.Instance.SendMessage((ushort)CmdMerchant.TakeAwardReq, req);
    }
    private void OnMerchantTakeAwardRes(NetMsg msg)
    {
        CmdMerchantTakeAwardRes res = NetMsgUtil.Deserialize<CmdMerchantTakeAwardRes>(CmdMerchantTakeAwardRes.Parser, msg);
        for (int i=0;i<MerchantGradeAwardTake.Count;i++)
        {
            if (MerchantGradeAwardTake[i][0]== res.InfoId)
            {
                MerchantGradeAwardTake[i][1] = 2;
            }
        }
        eventEmitter.Trigger(EEvents.UpdateLevelAward);
    }
    /// <summary>
    /// 请求求助列表
    /// </summary>
    public void OnMerchantHelpListReq()
    {
        CmdMerchantGetHelpListReq req = new CmdMerchantGetHelpListReq();
        NetClient.Instance.SendMessage((ushort)CmdMerchant.GetHelpListReq, req);
    }
    private void OnMerchantHelpListRes(NetMsg msg)
    {
        CmdMerchantGetHelpListRes res = NetMsgUtil.Deserialize<CmdMerchantGetHelpListRes>(CmdMerchantGetHelpListRes.Parser, msg);
        MerchantHelpInfoList.Clear();
        for (int i=0;i< res.InfoList.Count;i++)
        {
            MerchantHelpInfoList.Add(res.InfoList[i]);
        }
        InitMerchantHelpSelfList();
        eventEmitter.Trigger(EEvents.UpdateFamilyMerchantHelp);
    }
    /// <summary>
    /// 发布求助请求
    /// </summary>
    public void OnMerchantSeekHelpReq()
    {
        CmdMerchantSeekHelpReq req = new CmdMerchantSeekHelpReq();
        NetClient.Instance.SendMessage((ushort)CmdMerchant.SeekHelpReq, req);
    }
    private void OnMerchantSeekHelpRes(NetMsg msg)
    {
        CmdMerchantSeekHelpRes res = NetMsgUtil.Deserialize<CmdMerchantSeekHelpRes>(CmdMerchantSeekHelpRes.Parser, msg);
        eventEmitter.Trigger(EEvents.UpdateFamilyMerchantHelp);
    }

    /// <summary>
    /// 求助请求通知
    /// Add-true:增加委托 false:删除委托
    /// </summary>
    /// <param name="msg"></param>
    private void OnMerchantUpdateHelpNtf(NetMsg msg)
    {
        CmdMerchantUpdateHelpNtf ntf = NetMsgUtil.Deserialize<CmdMerchantUpdateHelpNtf>(CmdMerchantUpdateHelpNtf.Parser, msg);
        if (ntf.Add)
        {
            MerchantHelpInfoList.Add(ntf.Info);
            string content = LanguageHelper.GetTextContent(2028658, ntf.Info.Name.ToStringUtf8(), ntf.Info.RoleId.ToString(), "1");
            Sys_Chat.Instance.PushMessage(ChatType.Guild, null, content, Sys_Chat.EMessageProcess.None, Sys_Chat.EExtMsgType.Normal);
        }
        else
        {
            for (int i=MerchantHelpInfoList.Count-1;i>=0 ;--i)
            {
                if (MerchantHelpInfoList[i].RoleId== ntf.Info.RoleId)
                {
                    MerchantHelpInfoList.Remove(MerchantHelpInfoList[i]);
                }
            }
        }
        InitMerchantHelpSelfList();
        eventEmitter.Trigger(EEvents.UpdateFamilyMerchantHelp);
    }

    /// <summary>
    /// 取消求助请求
    /// </summary>
    public void OnMerchantCancelHelpReq()
    {
        CmdMerchantCancelHelpReq req = new CmdMerchantCancelHelpReq();
        NetClient.Instance.SendMessage((ushort)CmdMerchant.CancelHelpReq, req);
    }
    private void OnMerchantCancelHelpRes(NetMsg msg)
    {
        CmdMerchantCancelHelpRes res = NetMsgUtil.Deserialize<CmdMerchantCancelHelpRes>(CmdMerchantCancelHelpRes.Parser, msg);
    }
    /// <summary>
    /// 完成他人求助请求
    /// </summary>
    public void OnMerchantSubmitHelpReq(ulong roleId)
    {
        CmdMerchantSubmitHelpReq req = new CmdMerchantSubmitHelpReq();
        req.RoleId = roleId;
        NetClient.Instance.SendMessage((ushort)CmdMerchant.SubmitHelpReq, req);
    }
    private void OnMerchantSubmitHelpRes(NetMsg msg)
    {
        CmdMerchantSubmitHelpRes res = NetMsgUtil.Deserialize<CmdMerchantSubmitHelpRes>(CmdMerchantSubmitHelpRes.Parser, msg);
        if (TempHelpInfo != null)
        {
            MerchantResultParam openParam = new MerchantResultParam();
            openParam.TypeIndex = 2;
            var data = CSVMerchantFleetTask.Instance.GetConfData(TempHelpInfo.InfoId);
            int _index = (int)TempHelpInfo.TradeTaskIndex;
            var _list = new List<ItemIdCount>();
            _list.Add(new ItemIdCount(data.handItem[_index][0], data.handItem[_index][1]));
            openParam.AwardList = _list;
            UIManager.OpenUI(EUIID.UI_MerchantFleet_Settlement, false, openParam);
        }
        HelpCount++;
        eventEmitter.Trigger(EEvents.UpdateFamilyMerchantHelp);
    }

    /// <summary>
    /// 确认被帮助的贸易任务
    /// </summary>
    public void OnMerchantReceviceHelpReq()
    {
        CmdMerchantReceviceHelpReq req = new CmdMerchantReceviceHelpReq();
        NetClient.Instance.SendMessage((ushort)CmdMerchant.ReceviceHelpReq, req);
    }
    private void OnMerchantReceviceHelpRes(NetMsg msg)
    {
        CmdMerchantReceviceHelpRes res = NetMsgUtil.Deserialize<CmdMerchantReceviceHelpRes>(CmdMerchantReceviceHelpRes.Parser, msg);
        TradeTaskHelp = false;
        SeekHelpCount = res.SeekHelpCount;
        MerchantResultParam openParam = new MerchantResultParam();
        openParam.TypeIndex = 3;
        UIManager.OpenUI(EUIID.UI_MerchantFleet_Settlement, false, openParam);
        OnMerchantGetInfoReq();
    }
    /// <summary>
    /// 求助状态通知,上线主动发，其它人协助完成任务后会发,不重新登录跨周一五点不会发
    /// </summary>
    /// <param name="msg"></param>
    private void OnMerchantHelpStateNtf(NetMsg msg)
    {
        CmdMerchantHelpStateNtf ntf = NetMsgUtil.Deserialize<CmdMerchantHelpStateNtf>(CmdMerchantHelpStateNtf.Parser, msg);
        TradeTaskHelp = ntf.TradeTaskHelp;
        eventEmitter.Trigger(EEvents.UpdateMerchantInfo); 
        eventEmitter.Trigger(EEvents.UpdateFamilyMerchantHelp);
    }

    #endregion
    #region CallBack
    private void OnTaskSubmit(int arg1, uint arg2, TaskEntry arg3)
    {
        if (arg3 == null) return;
        var data = CSVTask.Instance.GetConfData(arg3.id);
        if (data != null && arg3.taskState == ETaskState.Submited&&data.taskCategory==21)
        {
            MerchantResultParam openParam = new MerchantResultParam();
            openParam.TypeIndex = 1;
            openParam.AwardList = TaskDropItem(arg3.id);
            if (openParam.AwardList.Count==0) return;
            UIManager.OpenUI(EUIID.UI_MerchantFleet_Settlement, false, openParam);
        }
    }
    private void OnEndBattle(CmdBattleEndNtf obj)
    {
        if (!Net_Combat.Instance.m_IsWatchBattle&&CheckMerchantBattle(obj.BattleTypeId))
        {
            MerchantResultParam openParam = new MerchantResultParam();
            var battleResult = Net_Combat.Instance.GetBattleOverResult();
            if (battleResult == 1)
            {//战斗胜利
                if (TempBattleInfo == null || TempBattleInfo.Count < 2)
                {
                    return;
                }
                openParam.TypeIndex = 2; 
                openParam.AwardList=CSVDrop.Instance.GetDropItem(TempBattleInfo[1]);
                UIManager.OpenUI(EUIID.UI_MerchantFleet_Settlement,false, openParam);
                OnMerchantGetInfoReq();
            }
            else
            {//战斗失败
                openParam.TypeIndex = 4;
                UIManager.OpenUI(EUIID.UI_MerchantFleet_Settlement, false, openParam);
            }
            TempBattleInfo.Clear();
        }

    }
    private void OnTimeNtf(uint arg1, uint arg2)
    {
        if (CheckMerchantFleetIsOpen())
        {
            ReqTickTimer();
            NextFiveDayRefresh();
            eventEmitter.Trigger(EEvents.UpdateMerchantInfo);
            eventEmitter.Trigger(EEvents.UpdateFamilyMerchantHelp);
        }
            
    }
    private void OnCompletedFunctionOpen(Sys_FunctionOpen.FunctionOpenData functionOpenData)
    {
        if (functionOpenData.id == 52501)
        {
            OnMerchantGetInfoReq();
        }
    }
    private void OnFuctionInitFinish()
    {
        InitMerchantInfo();
    }
    private void OnUpdateOpenServiceDay()
    {
        InitMerchantInfo();
    }
    #endregion
}

public partial class Sys_MerchantFleet : SystemModuleBase<Sys_MerchantFleet>
{
    #region DataInit
    public void InitMerchantInfo()
    {
        if (CheckMerchantFleetIsOpen())
        {
            OnMerchantGetInfoReq();
        }
    }
    private void InitTaskState(uint _id)
    {
        TaskId = _id;
        if (TaskId != 0)
        {
            IsMerchantTaskAccept = TaskHelper.HasReceived(TaskId);
        }
        else
        {
            IsMerchantTaskAccept = false;
        }
    }
    private void InitMerchantTradeTaskType()
    {
        if (MerchantTaskCount >= OneRoundTotalCount() && TaskId == 0&& taskData!=null)
        {
            MerchantTradeTaskType = taskData.businessType;
        }
        else
        {
            MerchantTradeTaskType = 0;
        }

    }
    private void InitTableData()
    {
        if (MerchantLevel == 0)
        {
            levelData = CSVMerchantFleetLevel.Instance.GetConfData(1);
        }
        else
        {
            levelData = CSVMerchantFleetLevel.Instance.GetConfData(MerchantLevel);
        }
        if (MerchantTotalCount < CSVMerchantFleetTask.Instance.Count)
        {
            taskData = CSVMerchantFleetTask.Instance.GetConfData(MerchantTotalCount + 1);
        }
        else
        {
            taskData = CSVMerchantFleetTask.Instance.GetConfData(MerchantTotalCount);
        }

    }
    private void InitLevelList(RepeatedField<uint> AwardTake)
    {
        MerchantGradeAwardTake.Clear();
        var data = CSVMerchantFleetLevel.Instance.GetAll();
        for (int i = 0; i < data.Count; i++)
        {
            var singleData = data[i];
            if (singleData.levelReward != 0)
            {
                int state = (MerchantLevel >= singleData.id) ? 1 : 0;
                if (AwardTake.Contains(singleData.id))
                {
                    state = 2;
                }
                MerchantGradeAwardTake.Add(new List<uint>() { singleData.id, (uint)state });
            }

        }
    }
    public int AllMerchantACount()
    {
        return CSVMerchantFleetTask.Instance.Count;
    }
    public uint OneRoundTotalCount()
    {
        uint count = 0;
        if (taskData ==null) return 0;
        for (int i = 0; i < taskData.taskLimit.Count; i++)
        {
            count += taskData.taskLimit[i];
        }
        return count;
    }
    public void InitBattleTempList()
    {
        TempBattleInfo.Clear();
        if (taskData==null) return;
        TempBattleInfo.Add(taskData.battleMonster);
        TempBattleInfo.Add(taskData.battleReward);
    }
    private void InitMerchantHelpSelfList()
    {
        MerchantHelpSelfList.Clear();
        for (int i = 0; i < MerchantHelpInfoList.Count; i++)
        {
            if (Sys_Role.Instance.Role.RoleId == MerchantHelpInfoList[i].RoleId)
            {
                MerchantHelpSelfList.Add(MerchantHelpInfoList[i]);

            }
        }
    }
    #endregion
    #region Fuction
    public List<ItemIdCount> TaskDropItem(uint taskId)
    {
        List<ItemIdCount> list = new List<ItemIdCount>();
        if (taskId == 0) return list;
        var data = CSVTask.Instance.GetConfData(taskId);
        if (data.DropId == null) return list;
        for (int i = 0; i < data.DropId.Count; i++)
        {
            var pData = CSVDrop.Instance.GetDropItem(data.DropId[i]);
            for (int j = 0; j < pData.Count; j++)
            {
                list.Add(pData[j]);
            }
        }
        return list;
    }
    public void InitPropItem(GameObject propItem, List<ItemIdCount> list_drop, EUIID euid,bool isShowBag)
    {
        DefaultItem(propItem, propItem.name);
        FrameworkTool.CreateChildList(propItem.transform.parent, list_drop.Count);
        for (int i = 0; i < list_drop.Count; i++)
        {
            Transform child = propItem.transform.parent.GetChild(i);
            PropItem _propItem = new PropItem();
            _propItem.BindGameObject(child.gameObject);
            ItemIdCount itemIdCount = list_drop[i];
            _propItem.SetData(new MessageBoxEvt(euid, new PropIconLoader.ShowItemData(itemIdCount.id, itemIdCount.count, true, false, false, false, false, _bShowCount: true, _bUseClick: true, _bShowBagCount: isShowBag)));
        }
    }
    public void DefaultItem(GameObject go, string name)
    {
        FrameworkTool.DestroyChildren(go.transform.parent.gameObject, name);
    }
    public void OpenFamilyHelp()
    {
        if (Sys_Role.Instance.isCrossSrv)
        {
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11898));//跨服战场内无法使用该功能
            return;
        }
        UI_FamilyOpenParam openParam = new UI_FamilyOpenParam()
        {
            familyMenuEnum = (int)UI_Family.EFamilyMenu.Activity,
            activeId = 70
        };
        bool isInFamily = Sys_Family.Instance.familyData.isInFamily;
        if (isInFamily)
        {
            if (!UIManager.IsOpen(EUIID.UI_Family))
                UIManager.OpenUI(EUIID.UI_Family, false, openParam);
        }
        else
        {
            UIManager.OpenUI(EUIID.UI_ApplyFamily);
        }
    }
    public bool IsAllMerchantTaskFinish()
    {
        if (AllMerchantACount() == MerchantTotalCount && TaskId == 0)
        {
            return true;
        }
        return false;
    }
    private Timer n_Timer;
    /// <summary>
    /// 算下周一五点刷新
    /// </summary>
    private void NextFiveDayRefresh()
    {
        if (!CheckMerchantFleetIsOpen()) return;

        DateTime _nowTime = TimeManager.GetDateTime(Sys_Time.Instance.GetServerTime());
        double _duration= m_MondayFive.Subtract(_nowTime).TotalSeconds + 1;
        n_Timer?.Cancel();
        n_Timer = Timer.Register((float)_duration, () =>
        {
            TradeTaskHelp = false;
        }, null, true);
    }
    private Timer m_Timer;
   
    /// <summary>
    /// 每天五点刷新
    /// </summary>
    private void ReqTickTimer()
    {
        if (!CheckMerchantFleetIsOpen()) return;

        DateTime _nowTime = TimeManager.GetDateTime(Sys_Time.Instance.GetServerTime());
        DateTime m_nowDayFive = _nowTime.Date.AddHours(5);
        double _duration = 0;
        if (_nowTime < m_nowDayFive)
        {
            _duration = m_nowDayFive.Subtract(_nowTime).TotalSeconds + 5;
        }
        else
        {
            DateTime m_nextDayFive = _nowTime.AddDays(1).Date.AddHours(5);
            _duration = m_nextDayFive.Subtract(_nowTime).TotalSeconds + 5;
        }
        m_Timer?.Cancel();
        m_Timer = Timer.Register((float)_duration, () =>
        {
            OnMerchantGetInfoReq();
            OnMerchantHelpListReq();
        }, null, true);
    }

    private Timer t_Timer;
    private bool isQuick = false;
    public void TradeResultShow(bool _isQuick)
    {
        t_Timer?.Cancel();
        isQuick = _isQuick;
        t_Timer = Timer.Register(sheepAnimationTime, OnFresh);
    }
    private void OnFresh()
    {
        OnMerchantSubmitTradeTaskReq(isQuick);
    }
    private void OnSubmitRes()
    {
        MerchantResultParam openParam = new MerchantResultParam();
        if (taskData==null) return;
        openParam.TypeIndex = 2;
        openParam.AwardList = CSVDrop.Instance.GetDropItem(taskData.handReward[MerchantIndex]);
        UIManager.OpenUI(EUIID.UI_MerchantFleet_Settlement, false, openParam);
        OnMerchantGetInfoReq();
        eventEmitter.Trigger(EEvents.UpdateMerchantInfo);
    }

    public bool CheckMerchantGrandeAwardRedPoint()
    {
        for (int i = 0; i < MerchantGradeAwardTake.Count; i++)
        {
            if (MerchantGradeAwardTake[i][1] == 1)
            {
                return true;
            }
        }
        return false;
    }
    public bool CheckMerchantFleetRedPoint()
    {
        return CheckMerchantGrandeAwardRedPoint() || TradeTaskHelp;
    }

    public bool CheckMerchantFleetIsOpen()
    {
        return Sys_FunctionOpen.Instance.IsOpen(52501);
    }
    /// <summary>
    /// 
    /// </summary>
    /// <param name="_id">战斗类型id</param>
    /// <returns></returns>
    public bool CheckMerchantBattle(uint _id)
    {
        var bData = CSVBattleType.Instance.GetConfData(_id);
        return (bData.battle_type_param == 15);
    }
    #endregion
}
public partial class Sys_MerchantFleet : SystemModuleBase<Sys_MerchantFleet>
{
    #region 数据定义
    /// <summary>
    /// 商队等级
    /// </summary>
    public uint MerchantLevel
    {
        get;
        private set;
    }
    /// <summary>
    /// 商队经验
    /// </summary>
    public uint MerchantExp
    {
        get;
        private set;
    } = 0;
    /// <summary>
    /// 本周贸易次数,从0开始(表uid需加1用)
    /// </summary>
    public uint MerchantTotalCount
    {
        get;
        private set;
    } = 0;
    /// <summary>
    /// 跑商任务index,从0开始
    /// </summary>
    public uint MerchantTaskCount
    {
        get;
        private set;
    }
    /// <summary>
    /// 贸易任务类型,0：无 1：战斗 2：上交(读表)
    /// </summary>
    public int MerchantTradeTaskType
    {
        get; private set;
    }
    /// <summary>
    /// 跑商任务是否接取
    /// </summary>
    public bool IsMerchantTaskAccept
    {
        get;
        private set;
    }
    /// <summary>
    /// 贸易上交、奖励等对应索引
    /// </summary>
    public int MerchantIndex
    {
        get;
        private set;
    }
    /// <summary>
    /// 已使用快速完成次数
    /// </summary>
    public uint QuickFinishCount
    {
        get;
        private set;
    } = 0;
    /// <summary>
    /// 求助次数
    /// </summary>
    public uint SeekHelpCount
    {
        get;
        private set;
    } = 0;
    /// <summary>
    /// 已使用帮助次数
    /// </summary>
    public uint HelpCount
    {
        get;
        private set;
    } = 0;
    /// <summary>
    /// 任务id 战斗和上交时为0
    /// </summary>
    public uint TaskId
    {
        get;
        private set;
    }
    /// <summary>
    /// 是否有已被帮助的贸易任务可领取
    /// </summary>
    public bool TradeTaskHelp
    {
        get;
        private set;
    } = false;
    /// <summary>
    /// 贸易任务是否发布
    /// </summary>
    public bool IsTradeHelpPublish
    {
        get
        {
            return MerchantHelpSelfList.Count != 0;
        }
    }
    private CSVMerchantFleetLevel.Data _LevelData;
    public CSVMerchantFleetLevel.Data levelData
    {
        get
        {
            return _LevelData;
        }
        private set
        {
            _LevelData = value;
        }
    }
    private CSVMerchantFleetTask.Data _TaskData;
    public CSVMerchantFleetTask.Data taskData
    {
        get
        {
            return _TaskData;
        }
        private set
        {
            _TaskData = value;
        }
    }
    /// <summary>
    /// 计算周一刷新时间
    /// </summary>
    public DateTime m_MondayFive
    {
        get
        {
            DateTime _nowTime = TimeManager.GetDateTime(Sys_Time.Instance.GetServerTime());
            DateTime mondayFive = _nowTime.Date;
            int count = _nowTime.DayOfWeek - DayOfWeek.Monday;
            if (count == -1) count = 6;
            mondayFive = mondayFive.AddDays(-count).AddHours(5);//本周周一五点
            if (_nowTime > mondayFive)
            {
                mondayFive = mondayFive.AddDays(7);
            }
            return mondayFive;

        }
    }
    /// <summary>
    /// 帮助临时存档
    /// </summary>
    public MerchantHelpInfo TempHelpInfo
    {
        get;
        set;
    }
    public List<uint> TempBattleInfo = new List<uint>();//战斗临时存档
    public List<List<uint>> MerchantGradeAwardTake = new List<List<uint>>();//等级奖励——[0]-表id,[1]-不可领取0,可领取1，已领取2
    public RepeatedField<MerchantHelpInfo> MerchantHelpInfoList = new RepeatedField<MerchantHelpInfo>();//家族求助表-全部
    public RepeatedField<MerchantHelpInfo> MerchantHelpSelfList = new RepeatedField<MerchantHelpInfo>();//家族求助表-自己
    public enum EEvents : int
    {
        UpdateMerchantInfo,
        UpdateFamilyMerchantHelp,
        UpdateLevelAward,
    }
    public EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
    #endregion
}
