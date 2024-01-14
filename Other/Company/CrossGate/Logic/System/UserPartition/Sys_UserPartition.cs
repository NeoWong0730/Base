using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Packet;
using Net;
using Table;
using Lib.Core;
using System;

namespace Logic
{
    public class Sys_UserPartition : SystemModuleBase<Sys_UserPartition>
    {
        public EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        public enum EEvents : int
        {

        }

        #region 系统函数
        public override void Init()
        {
            base.Init();
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdRole.GetLayerRewardAck, OnUserPartitionGetGiftAck, CmdRoleGetLayerRewardAck.Parser);
        }
        #endregion

        #region net
        /// <summary>
        /// 领取用户分层协议
        /// </summary>
        public void UserPartitionGetGiftReq()
        {
            Sys_Role.Instance.UserPartitionGiftIsGet = true;
            CmdRoleGetLayerRewardReq req = new CmdRoleGetLayerRewardReq();
            NetClient.Instance.SendMessage((ushort)CmdRole.GetLayerRewardReq, req);
        }

        private void OnUserPartitionGetGiftAck(NetMsg msg)
        {
            CmdRoleGetLayerRewardAck res = NetMsgUtil.Deserialize<CmdRoleGetLayerRewardAck>(CmdRoleGetLayerRewardAck.Parser, msg);
        }
        #endregion

        #region function
        /// <summary>
        /// 检测用户分层奖励是否领取
        /// </summary>
        public bool CheckUserPartitionGiftIsGet()
        {
            return Sys_Role.Instance.UserPartitionGiftIsGet;
        }
        /// <summary>
        /// 获取用户分层奖励item数据
        /// </summary>
        public List<ItemData> GetRewardItems()
        {
            List<ItemData> items = new List<ItemData>();

            string dropId = CSVParam.Instance.GetConfData(950u).str_value;
            List<ItemIdCount> dropItems = CSVDrop.Instance.GetDropItem(Convert.ToUInt32(dropId));
            if (dropItems != null)
            {
                int len = dropItems.Count;
                for (int j = 0; j < len; j++)
                {
                    ItemData item = new ItemData(0, 0, dropItems[j].id, (uint)dropItems[j].count, 0, false, false, null, null, 0);
                    items.Add(item);
                }
            }
            return items;

        }
        #endregion

        #region 埋点

        #endregion
    }
}
