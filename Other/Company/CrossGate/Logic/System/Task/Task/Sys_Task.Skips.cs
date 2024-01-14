using System;
using System.Collections.Generic;
using Logic.Core;
using Table;
using UnityEngine;
using Packet;
using Net;

namespace Logic 
{

    public partial class Sys_Task : SystemModuleBase<Sys_Task> 
    {
        private Dictionary<uint, bool> dictSkips = new Dictionary<uint, bool>();

        private void ParseSkips(CmdTaskDataNtf ntf)
        {
            dictSkips.Clear();
            if (ntf.SkipBtnList != null)
            {
                for (int i = 0; i < ntf.SkipBtnList.Count; ++i)
                    dictSkips.Add(ntf.SkipBtnList[i].Key, ntf.SkipBtnList[i].Skip);
            }
        }

        public void OnChageSkipReq(uint key, bool skip)
        {
            CmdTaskSkipTalkBtnChangeReq req = new CmdTaskSkipTalkBtnChangeReq();
            req.Key = key;
            req.Skip = skip;
            NetClient.Instance.SendMessage((ushort)CmdTask.SkipTalkBtnChangeReq, req);
        }

        private void OnSkipUpdateNtf(NetMsg msg)
        {
            CmdTaskSkipTalkBtnUpdateNtf ntf = NetMsgUtil.Deserialize<CmdTaskSkipTalkBtnUpdateNtf>(CmdTaskSkipTalkBtnUpdateNtf.Parser, msg);

            for (int i = 0; i < ntf.BtnUpdate.Count; ++i)
            {
                if (dictSkips.TryGetValue(ntf.BtnUpdate[i].Key, out bool skip))
                    dictSkips[ntf.BtnUpdate[i].Key] = ntf.BtnUpdate[i].Skip;
                else
                    dictSkips.Add(ntf.BtnUpdate[i].Key, ntf.BtnUpdate[i].Skip);
            }
        }

        /// <summary>
        /// 获取开关状态, 如果没设置过，也返回false
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool GetSkipState(uint key)
        {
            if (dictSkips.TryGetValue(key, out bool state))
                return state;
            else
                return false;
        }

        /// <summary>
        /// 判断开关是否有设置过
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public bool IsSettedSkip(uint key)
        {
            return dictSkips.TryGetValue(key, out bool state);
        }
    }
}
