using System.Collections.Generic;
using Logic.Core;
using Packet;
using Net;
using Lib.Core;
using Table;
using Logic;
using System;
using System.Text.RegularExpressions;

public class Sys_SecureLock : SystemModuleBase<Sys_SecureLock>
{
    

    public EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

    public bool hasPwd;//密码
    public bool lockState;//加锁状态
    public int errorCount;//密码错误次数
    public uint enforceUnlockTick;//强制解锁到期时间戳
    public enum EEvents
    {
        OnVlueInit,
        OnStateUpdate,
        OnSecureLockEnforce,
        OnErrorCountUpdate,
        OnUnLockRes,
        OnResetPwdRes,
    }

    #region 系统函数
    public override void Init()
    {
        //请求数据
        EventDispatcher.Instance.AddEventListener((ushort)CmdSecureLock.InitReq,(ushort)CmdSecureLock.InitRes, OnSecureLockInitRes,CmdSecureLockInitRes.Parser);
        //强制解锁
        EventDispatcher.Instance.AddEventListener((ushort)CmdSecureLock.EnforceResetReq,(ushort)CmdSecureLock.EnforceResetRes, OnSecureLockEnforceResetRes,CmdSecureLockEnforceResetRes.Parser);
        //服务器主动发起
        EventDispatcher.Instance.AddEventListener(0,(ushort)CmdSecureLock.DataUpdateNtf, OnSecureLockDataUpdateNtf,CmdSecureLockDataUpdateNtf.Parser);
        //密码错误
        EventDispatcher.Instance.AddEventListener(0, (ushort)CmdSecureLock.ErrorCountNtf, OnSecureLockErrorCountNtf, CmdSecureLockErrorCountNtf.Parser);
        //解锁
        EventDispatcher.Instance.AddEventListener((ushort)CmdSecureLock.UnlockReq, (ushort)CmdSecureLock.UnlockRes, OnSecureLockUnlockRes, CmdSecureLockUnlockRes.Parser);
        //修改密码
        EventDispatcher.Instance.AddEventListener((ushort)CmdSecureLock.ResetPasswordReq, (ushort)CmdSecureLock.ResetPasswordRes, OnSecureLockResetPasswordRes,CmdSecureLockResetPasswordRes.Parser);

    }
    public override void OnLogout()
    {
        base.OnLogout();
        
    }
    #endregion
    #region net

    //设置密码

    public void SecureLockSetPasswordReq(string pwd)
    {
        CmdSecureLockSetPasswordReq req = new CmdSecureLockSetPasswordReq();
        req.Password = pwd;
        NetClient.Instance.SendMessage((ushort)CmdSecureLock.SetPasswordReq,req);

    }
    //修改密码
    public void SecureLockResetPasswordReq(string oldPwd,string newPwd)
    {
        CmdSecureLockResetPasswordReq req = new CmdSecureLockResetPasswordReq();
        req.Password = oldPwd;
        req.Newpassword = newPwd;
        NetClient.Instance.SendMessage((ushort)CmdSecureLock.ResetPasswordReq, req);
    }

    public void OnSecureLockResetPasswordRes(NetMsg msg)
    {
        eventEmitter.Trigger(EEvents.OnResetPwdRes);
    }

    //加锁
    public void SecureLockLockReq()
    {
        CmdSecureLockLockReq req = new CmdSecureLockLockReq();
        NetClient.Instance.SendMessage((ushort)CmdSecureLock.LockReq, req);
    }

    //解锁
    public void SecureLockUnlockReq(string Pwd)
    {
        CmdSecureLockUnlockReq req = new CmdSecureLockUnlockReq();
        req.Password = Pwd;
        NetClient.Instance.SendMessage((ushort)CmdSecureLock.UnlockReq, req);
    }

    public void OnSecureLockUnlockRes(NetMsg msg)
    {
        eventEmitter.Trigger(EEvents.OnUnLockRes);
    }

    //请求数据
    public void SecureLockInitReq()
    {
        CmdSecureLockInitReq req = new CmdSecureLockInitReq();
        NetClient.Instance.SendMessage((ushort)CmdSecureLock.InitReq,req);

    }

    public void OnSecureLockInitRes(NetMsg msg)
    {
        CmdSecureLockInitRes res = NetMsgUtil.Deserialize<CmdSecureLockInitRes>(CmdSecureLockInitRes.Parser,msg);
        hasPwd = res.HasPassword;
        lockState = res.LockState;
        errorCount = res.ErrorCount;
        enforceUnlockTick = res.EnforceUnlockTick;
        eventEmitter.Trigger(EEvents.OnVlueInit);


    }
    //强制解锁/取消强制解锁
    public void SecureLockEnforceResetReq(uint type)
    {
        CmdSecureLockEnforceResetReq req = new CmdSecureLockEnforceResetReq();
        req.OpType = type;
        NetClient.Instance.SendMessage((ushort)CmdSecureLock.EnforceResetReq,req);

    }
    
    public void OnSecureLockEnforceResetRes(NetMsg msg)
    {
        CmdSecureLockEnforceResetRes res = NetMsgUtil.Deserialize<CmdSecureLockEnforceResetRes>(CmdSecureLockEnforceResetRes.Parser,msg);
        enforceUnlockTick = res.EndTick;
        eventEmitter.Trigger(EEvents.OnSecureLockEnforce);
    }

    //密码错误NTF
    public void OnSecureLockErrorCountNtf(NetMsg msg)
    {
        CmdSecureLockErrorCountNtf ntf = NetMsgUtil.Deserialize<CmdSecureLockErrorCountNtf>(CmdSecureLockErrorCountNtf.Parser, msg);
        errorCount = (int)ntf.ErrorCount;

        eventEmitter.Trigger(EEvents.OnErrorCountUpdate);


    }

    //主动NTF
    public void OnSecureLockDataUpdateNtf(NetMsg msg)
    {
        CmdSecureLockDataUpdateNtf ntf = NetMsgUtil.Deserialize<CmdSecureLockDataUpdateNtf>(CmdSecureLockDataUpdateNtf.Parser,msg);
        hasPwd = ntf.HasPassword;
        lockState = ntf.LockState;
        eventEmitter.Trigger(EEvents.OnStateUpdate);


    }

    #endregion
    #region Function
    public bool ComparePassWard(string setPwd, string comPwd)
    {
        if (setPwd.CompareTo(comPwd) != 0)
        {
            return false;
        }
        return true;
    }

    public bool CheckPassWard(string pwd)
    {
        Regex reg = new Regex(@"^[A-Za-z0-9]+$");
        return reg.IsMatch(pwd);
    }

    public void JumpToSecureLock()
    {
        PromptBoxParameter.Instance.Clear();
        PromptBoxParameter.Instance.content = CSVErrorCode.Instance.GetConfData(106208).words;
        PromptBoxParameter.Instance.SetConfirm(true, () =>
        {
            UIManager.OpenUI(EUIID.UI_Setting, false, Tuple.Create<ESettingPage, Logic.ESetting>(ESettingPage.SaftyLock, Logic.ESetting.Audio));
        });
        PromptBoxParameter.Instance.SetCancel(true, null);
        UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
    }
    #endregion


}
