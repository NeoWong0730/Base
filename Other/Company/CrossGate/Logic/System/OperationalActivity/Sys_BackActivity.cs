using Framework;
using Lib.Core;
using Logic.Core;
using Net;
using Packet;
using System;
using System.Collections.Generic;
using System.Text;
using Table;
using UnityEngine;

namespace Logic
{
    public class Sys_BackActivity : SystemModuleBase<Sys_BackActivity>
    {
        private GiftsData backGiftData;
        private uint OpenTime = 0;
        /// <summary>
        /// 回归所在分组
        /// </summary>
        public uint ActivityGroup { get; private set; } = 0;

        public enum EEvents : int
        {
            OnBackActivityInfoUpdate,//回归活动时间数据刷新
            OnBackActivityRedPointUpdate,//回归活动总红点刷新事件
        }
        public EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        #region 系统函数
        public override void Init()
        {
            RegisterEvents(true);
        }

        public override void Dispose()
        {
            RegisterEvents(false);
        }

        public override void OnLogin()
        {
        }
        public override void OnLogout()
        {
            backGiftData = null;
            OpenTime = 0;
            ActivityGroup = 0;
        }

        /// <summary>
        /// 注册事件
        /// </summary>
        private void RegisterEvents(bool toRegister)
        {
            Sys_Time.Instance.eventEmitter.Handle<uint, uint>(Sys_Time.EEvents.OnTimeNtf, OnTimeNtf, toRegister);
            if (toRegister)
            {
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdActivityReturn.InfoNtf, OnActivityReturnInfoNtf, CmdActivityReturnInfoNtf.Parser);
                EventDispatcher.Instance.AddEventListener((ushort)CmdActivityReturn.TakeGiftsReq, (ushort)CmdActivityReturn.TakeGiftsRes, OnGetBackGiftRes, CmdActivityReturnTakeGiftsRes.Parser);

            }
            else
            {
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdActivityReturn.InfoNtf, OnActivityReturnInfoNtf);
                EventDispatcher.Instance.AddEventListener((ushort)CmdActivityReturn.TakeGiftsReq, (ushort)CmdActivityReturn.TakeGiftsRes, OnGetBackGiftRes, CmdActivityReturnTakeGiftsRes.Parser);
            }
        }
        #endregion

        #region 服务器消息
        public void OnActivityReturnInfoNtf(NetMsg msg)
        {
            CmdActivityReturnInfoNtf ntf = NetMsgUtil.Deserialize<CmdActivityReturnInfoNtf>(CmdActivityReturnInfoNtf.Parser, msg);
            OpenTime = ntf.OpenTime;
            ActivityGroup = ntf.ActivityGroup;
            backGiftData = ntf.GiftsData;
            //Debug.Log("OnActivityReturnInfoNtf " + OpenTime + " | " + ActivityGroup);
            eventEmitter.Trigger(EEvents.OnBackActivityInfoUpdate);
            Sys_BackSign.Instance.OnGetBackSignInDataReq();
            Sys_BackAwardGet.Instance.BackAwardDataInit();
            Sys_ReturnTask.Instance.UpdateReturnTaskData(ntf, OpenTime);
            Sys_Role.Instance.UpdateRoleBackInfo(OpenTime!=0);
        }
        /// <summary>
        /// 领取回归奖励
        /// </summary>
        public void ReqGetBackGift()
        {
            CmdActivityReturnTakeGiftsReq req = new CmdActivityReturnTakeGiftsReq();
            NetClient.Instance.SendMessage((ushort)CmdActivityReturn.TakeGiftsReq, req);
        }
        public void OnGetBackGiftRes(NetMsg msg)
        {
            CmdActivityReturnTakeGiftsRes res = NetMsgUtil.Deserialize<CmdActivityReturnTakeGiftsRes>(CmdActivityReturnTakeGiftsRes.Parser, msg);
            backGiftData.TakeReward = true;
            eventEmitter.Trigger(EEvents.OnBackActivityInfoUpdate);
        }
        #endregion

        #region func
        /// <summary>
        /// 检测回归活动是否开启
        /// </summary>
        public bool CheckBackActivityIsOpen()
        {
            var activityCD = GetBackActivityBtnCD();
            //Debug.Log("CheckBackActivityIsOpen" + activityCD);
            bool isopen = activityCD > 0 && Sys_OperationalActivity.Instance.CheckActivitySwitchIsOpen(215);
            Sys_Role.Instance.UpdateRoleBackInfo(isopen);
            return isopen;
        }
        /// <summary>
        /// 检测回归活动总红点
        /// </summary>
        public bool CheckBackActivityAllRedPoint()
        {
            return CheckBackGiftCanGet()
                || Sys_BackSign.Instance.BackSignRedPoint()
                || Sys_BackAwardGet.Instance.BackAwardRedPoint()
                || Sys_ReturnTask.Instance.HasAllRed()
                ;
        }
        /// <summary>
        /// 获取回归活动按钮倒计时
        /// </summary>
        public uint GetBackActivityBtnCD()
        {
            var nowTime = Sys_Time.Instance.GetServerTime();
            var endTime = GetBackActivityEndTime();
            if (endTime > nowTime)
            {
                return endTime - nowTime;
            }
            return 0;
        }
        public uint GetBackActivityEndTime()
        {
            uint days = uint.Parse(CSVReturnParam.Instance.GetConfData(4).str_value);
            var endTime = OpenTime + 86400 * days;
            //return (endTime - endTime % 86400);//后端这边已经算好了
            return endTime;
        }
        /// <summary>
        /// 回归赠礼是否开启
        /// </summary>
        public bool CheckBackGiftIsOpen()
        {
            return Sys_OperationalActivity.Instance.CheckActivitySwitchIsOpen(216);
        }
        /// <summary>
        /// 触发回归时的拍脸
        /// </summary>
        public void CheckToPopupFace()
        {
            var roleId = Sys_Role.Instance.Role.RoleId;
            string key = roleId.ToString() + "BackActivity";
            if (!PlayerPrefs.HasKey(key) || PlayerPrefs.GetInt(key) != OpenTime)
            {
                PlayerPrefs.SetInt(key, (int)OpenTime);
                UIManager.OpenUI(EUIID.UI_BackActivity);
            }
        }
        /// <summary>
        /// 获取离开天数
        /// </summary>
        public uint GetLeaveDays()
        {
            if(backGiftData != null)
            {
                var leaveTime = OpenTime - backGiftData.LastOffline;
                return leaveTime / 86400;
            }
            return 0;
        }
        /// <summary>
        /// 检测回归礼包是否可以领取
        /// </summary>
        public bool CheckBackGiftCanGet()
        {
            if(backGiftData!=null && !backGiftData.TakeReward)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region event
        /// <summary> 服务器时间同步 </summary>
        private void OnTimeNtf(uint oldTime, uint newTime)
        {

        }
        #endregion
    }
}
