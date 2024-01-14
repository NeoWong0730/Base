using Packet;
using Lib.Core;
using Net;
using System;
using Logic.Core;
using System.Collections.Generic;
using Table;
using UnityEngine;

namespace Logic
{


    /// 通用投票
    public partial class Sys_Vote : SystemModuleBase<Sys_Vote>
    {
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public delegate void VoteListenerDelegate(EVote eVote, object message);

        private Dictionary<ushort, VoteListenerDelegate> m_DicListener = new Dictionary<ushort, VoteListenerDelegate>();

        private uint mCurVoteType = 0;

        private uint VoteStartTime = 0;
        public enum EEvents
        {
            OnCrossDay,
        }

        public enum EVote
        {
            None = 0,
            Start = 1,
            End = 2,
            DoVote = 3,
            Update = 4,

        }

        public override void Init()
        {

            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdRole.StartVoteNtf, Notify_StartVoteNtf, CmdRoleStartVoteNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdRole.DoVoteNtf, Notify_DoVoteNtf, CmdRoleDoVoteNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdRole.VoteEndNtf, Notify_VoteEndNtf, CmdRoleVoteEndNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdRole.VoteUpdateNtf, Notify_VoteUpdateNtf, CmdRoleVoteUpdateNtf.Parser);
        }

        public override void Dispose()
        {

            m_DicListener.Clear();
        }
        public void AddVoteLisitener(ushort type, VoteListenerDelegate voteListener)
        {
            if (m_DicListener.ContainsKey(type))
            {
                DebugUtil.LogErrorFormat(" this vote type({0}) lisitener had register", type);
                return;
            }

            m_DicListener.Add(type, voteListener);

        }

        public void RemoveVoteLisitener(ushort type, VoteListenerDelegate voteListener)
        {
            if (m_DicListener.ContainsKey(type))
            {
                m_DicListener.Remove(type);
                return;
            }
        }

        protected void InvotedLisitener(ushort type, EVote eVote,object message)
        {
            if (m_DicListener.ContainsKey(type) == false)
            {
                return;
            }

            m_DicListener[type].Invoke(eVote, message);
        }

        public float GetVoteTime(uint id)
        {
            if (VoteStartTime == 0)
                return 0;

            var data = CSVParam.Instance.GetConfData(id);

            var realtime = int.Parse(data.str_value) / 1000f;

            var servertime = Sys_Time.Instance.GetServerTime();

            float time = realtime;

            if (servertime > VoteStartTime)
            {
                uint distime = servertime - VoteStartTime;

                time = realtime - (float)distime;

                time = Mathf.Max(0, time);
            }

            return time;
        }
    }


    public partial class Sys_Vote : SystemModuleBase<Sys_Vote>
    {
        private void Notify_StartVoteNtf(NetMsg msg)
        {
           // DebugUtil.LogError("sys_ vote  get start vote ntf :");

            CmdRoleStartVoteNtf info = NetMsgUtil.Deserialize<CmdRoleStartVoteNtf>(CmdRoleStartVoteNtf.Parser, msg);

            mCurVoteType = info.VoteType;

            VoteStartTime = Sys_Time.Instance.GetServerTime();

            InvotedLisitener((ushort)mCurVoteType, EVote.Start, info);
        }

        private void Notify_DoVoteNtf(NetMsg msg)
        {
            CmdRoleDoVoteNtf info = NetMsgUtil.Deserialize<CmdRoleDoVoteNtf>(CmdRoleDoVoteNtf.Parser, msg);

            InvotedLisitener((ushort)mCurVoteType, EVote.DoVote, info);
        }

        private void Notify_VoteEndNtf(NetMsg msg)
        {
            CmdRoleVoteEndNtf info = NetMsgUtil.Deserialize<CmdRoleVoteEndNtf>(CmdRoleVoteEndNtf.Parser, msg);

            InvotedLisitener((ushort)mCurVoteType, EVote.End, info);

            VoteStartTime = 0;
        }

        private void Notify_VoteUpdateNtf(NetMsg msg)
        {
            CmdRoleVoteUpdateNtf info = NetMsgUtil.Deserialize<CmdRoleVoteUpdateNtf>(CmdRoleVoteUpdateNtf.Parser, msg);

            InvotedLisitener((ushort)mCurVoteType, EVote.Update, info);
        }
    }

    public partial class Sys_Vote : SystemModuleBase<Sys_Vote>
    {

        /// <summary>
        /// 投票
        /// </summary>
        /// <param name="voteID">投票唯一号</param>
        /// <param name="reuslt">投票结果 1 同意 ，2 反对</param>
        public void Send_DoVoteReq(ulong voteID, uint reuslt)
        {
            CmdRoleDoVoteReq info = new CmdRoleDoVoteReq();
            info.Op = reuslt;
            info.VoteId = voteID;
            NetClient.Instance.SendMessage((ushort)CmdRole.DoVoteReq, info);
        }

        /// <summary>
        /// 取消投票
        /// </summary>
        /// <param name="voteID">投票唯一号</param>
        public void Send_CancleVoteReq(ulong voteID)
        {
            CmdRoleCancelVoteReq info = new CmdRoleCancelVoteReq();
            info.VoteId = voteID;
            NetClient.Instance.SendMessage((ushort)CmdRole.CancelVoteReq, info);
        }
    }
}
