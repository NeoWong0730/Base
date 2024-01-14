using System;
using System.Collections.Generic;
using Packet;
using Logic.Core;
using Lib.Core;
using Framework;
using Net;

namespace Logic
{
    public partial class Sys_Match:SystemModuleBase<Sys_Match>
    {
        public enum EEvents
        {
            MatchPanelClose,
            MatchPanelOpen,
            LoadPanelClose,
            LoadPanelOpen,
            MatcherInfo,
            MatcherLoadOk,
            MatchCancle,
        }

        public enum EMatchStateC
        {
            None = 0,
            Start,
            Cancle,
            GetMatcherInfo,
            CloseMatchUI,
            StartMatchLoad,
            MatchFinish,
        }

        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public EMatchType MatchType { get; private set; }

        public EMatchStateC MatchState { get; private set; } = EMatchStateC.None;
        public override void Init()
        {
            EventDispatcher.Instance.AddEventListener(0,(ushort)CmdMatch.NotifyMatchPanelClose, NotifyMatchClose, CmdMatchNotifyMatchPanelClose.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdMatch.NotifyMatchPanelOpen, NotifyMatchOpen, CmdMatchNotifyMatchPanelOpen.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdMatch.NotifyLoadPanelClose, NotifyMatchLoadClose, CmdMatchNotifyLoadPanelClose.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdMatch.NotifyLoadPanelOpen, NotifyMatchLoadOpen, CmdMatchNotifyLoadPanelOpen.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdMatch.NotifyMatcherInfo, NotifyMatcherInfo, CmdMatchNotifyMatcherInfo.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdMatch.NotifyMatcherLoadOk, NotifyMatcherLoadOk, CmdMatchNotifyMatcherLoadOk.Parser);

        }

    }

    public partial class Sys_Match : SystemModuleBase<Sys_Match>
    {
        /// <summary>
        /// 关闭匹配界面
        /// </summary>
        /// <param name="msg"></param>
        private void NotifyMatchClose(NetMsg msg)
        {
            CmdMatchNotifyMatchPanelClose info = NetMsgUtil.Deserialize<CmdMatchNotifyMatchPanelClose>(CmdMatchNotifyMatchPanelClose.Parser, msg);

            MatchState = MatchState > EMatchStateC.Cancle ? EMatchStateC.CloseMatchUI : EMatchStateC.Cancle;

            if (MatchState == EMatchStateC.Cancle)
            {            
                eventEmitter.Trigger<CmdMatchNotifyMatchPanelClose>(EEvents.MatchCancle, info);

                MatchType = EMatchType.MatchTypeNone;
            }
            else
            {
                eventEmitter.Trigger<CmdMatchNotifyMatchPanelClose>(EEvents.MatchPanelClose, info);
            }   
        }

        /// <summary>
        /// 打开匹配界面
        /// </summary>
        /// <param name="msg"></param>
        private void NotifyMatchOpen(NetMsg msg)
        {
            CmdMatchNotifyMatchPanelOpen info = NetMsgUtil.Deserialize<CmdMatchNotifyMatchPanelOpen>(CmdMatchNotifyMatchPanelOpen.Parser, msg);

            MatchType = (EMatchType)info.Matchtype;

            MatchState = EMatchStateC.Start;

            eventEmitter.Trigger<CmdMatchNotifyMatchPanelOpen>(EEvents.MatchPanelOpen,info);
        }

        /// <summary>
        /// 关闭匹配加载场景界面
        /// </summary>
        /// <param name="msg"></param>
        private void NotifyMatchLoadClose(NetMsg msg)
        {
            CmdMatchNotifyLoadPanelClose info = NetMsgUtil.Deserialize<CmdMatchNotifyLoadPanelClose>(CmdMatchNotifyLoadPanelClose.Parser, msg);

            MatchState = EMatchStateC.MatchFinish;

            eventEmitter.Trigger<CmdMatchNotifyLoadPanelClose>(EEvents.LoadPanelClose,info);

            MatchType = EMatchType.MatchTypeNone;
        }

        /// <summary>
        /// 打开匹配加载场景界面
        /// </summary>
        /// <param name="msg"></param>
        private void NotifyMatchLoadOpen(NetMsg msg)
        {
            CmdMatchNotifyLoadPanelOpen info = NetMsgUtil.Deserialize<CmdMatchNotifyLoadPanelOpen>(CmdMatchNotifyLoadPanelOpen.Parser, msg);

            MatchState = EMatchStateC.StartMatchLoad;

            eventEmitter.Trigger<CmdMatchNotifyLoadPanelOpen>(EEvents.LoadPanelOpen,info);
        }

        /// <summary>
        /// 匹配对象信息
        /// </summary>
        /// <param name="msg"></param>
        private void NotifyMatcherInfo(NetMsg msg)
        {
            CmdMatchNotifyMatcherInfo info = NetMsgUtil.Deserialize<CmdMatchNotifyMatcherInfo>(CmdMatchNotifyMatcherInfo.Parser, msg);

            MatchState = EMatchStateC.GetMatcherInfo;

            eventEmitter.Trigger<CmdMatchNotifyMatcherInfo>(EEvents.MatcherInfo,info);
        }
        /// <summary>
        /// 匹配状态
        /// </summary>
        /// <param name="msg"></param>
        private void NotifyMatcherLoadOk(NetMsg msg)
        {
            CmdMatchNotifyMatcherLoadOk info = NetMsgUtil.Deserialize<CmdMatchNotifyMatcherLoadOk>(CmdMatchNotifyMatcherLoadOk.Parser, msg);

            eventEmitter.Trigger<CmdMatchNotifyMatcherLoadOk>(EEvents.MatcherLoadOk,info);
        }
    }

    public partial class Sys_Match : SystemModuleBase<Sys_Match>
    {
        public void SendStartMatch(EMatchType eMatchType)
        {
            CmdMatchStartMatchReq info = new CmdMatchStartMatchReq();
            info.Matchtype = (uint)eMatchType;
            NetClient.Instance.SendMessage((ushort)CmdMatch.StartMatchReq, info);
        }

        public void SendCancleMatch()
        {
            CmdMatchCancelMatchReq info = new CmdMatchCancelMatchReq();

            NetClient.Instance.SendMessage((ushort)CmdMatch.CancelMatchReq, info);
        }
    }
}
