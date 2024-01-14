using Logic.Core;
using Lib.Core;
using Net;
using Packet;
using UnityEngine;
using Table;
using Lib.AssetLoader;
using System.Collections.Generic;
using Logic;
using System;
using DG.Tweening;

namespace Logic
{
    /// <summary> npc系统 </summary>
    public class Sys_Npc : SystemModuleBase<Sys_Npc>
    {
        #region 数据定义
        public enum EEvents
        {
            OnUpdateResPoint,
            OnActiveNPC,
            OnNearNpcChange,
            //OnLeaveNpc,
            OnNearNpcClose,
            OnCheckShowNpc,
        }
        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        #endregion
        #region 系统函数
        public override void Init()
        {
            base.Init();

            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdNpc.ActivatedNpcNtf, OnActivatedNpcNtf, CmdNpcActivatedNpcNtf.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdNpc.ActivateNpcRes, OnActivateNpcRes, CmdNpcActivateNpcRes.Parser);
            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdNpc.GetActiveRewardAck, OnNpcGetActiveRewardAck, CmdNpcGetActiveRewardAck.Parser);
            
            Sys_Net.Instance.eventEmitter.Handle<bool>(Sys_Net.EEvents.OnReconnectResult, OnReconnectResult, true);
        }
        public override void OnLogin()
        {
            base.OnLogin();
            ClearMapResPointData();
        }
        public override void OnLogout()
        {
            base.OnLogout();
        }
        private void OnReconnectResult(bool result)
        {
            GameCenter.mNearbyNpcSystem.RequestUpdateAll();
        }
        #endregion
        #region 服务器发送消息
        /// <summary>
        /// 请求激活Npc
        /// </summary>
        /// <param name="id"></param>
        public void ReqNpcActivateNpc(ulong id)
        {
            CmdNpcActivateNpcReq req = new CmdNpcActivateNpcReq();
            req.UId = id;
            NetClient.Instance.SendMessage((ushort)CmdNpc.ActivateNpcReq, req);
        }
        /// <summary>
        /// 请求探索奖励
        /// </summary>
        /// <param name="id"></param>
        public void ReqNpcGetActiveReward(uint id)
        {
            CmdNpcGetActiveRewardReq req = new CmdNpcGetActiveRewardReq();
            req.Id = id;
            NetClient.Instance.SendMessage((ushort)CmdNpc.GetActiveRewardReq, req);
        }
        #endregion
        #region 服务器接收消息
        /// <summary>
        /// 激活Npc列表消息
        /// </summary>
        /// <param name="msg"></param>
        private void OnActivatedNpcNtf(NetMsg msg)
        {
            CmdNpcActivatedNpcNtf res = NetMsgUtil.Deserialize<CmdNpcActivatedNpcNtf>(CmdNpcActivatedNpcNtf.Parser, msg);

            npcActivatedData.Clear();

            if (res.ActivatedInfoId != null)
                npcActivatedData.activatedInfoId.AddRange(res.ActivatedInfoId);
            //foreach (var item in res.ActivatedInfoId)
            //{
            //    npcActivatedData.activatedInfoId.Add(item);
            //}
            if (res.GotRewards != null)
                npcActivatedData.getRewards.AddRange(res.GotRewards);
            //foreach (var item in res.GotRewards)
            //{
            //    npcActivatedData.getRewards.Add(item);
            //}
            foreach (var item in res.ProList)
            {
                NpcActivatedData.MapProcess mapProcess = new NpcActivatedData.MapProcess();
                mapProcess.mapID = item.MapId;
                mapProcess.process = new List<NpcActivatedData.MapProcess.PP>();
                foreach (var child in item.Process)
                {
                    NpcActivatedData.MapProcess.PP pp = new NpcActivatedData.MapProcess.PP();
                    pp.markType = child.MarkType;
                    pp.markCount = child.MarkCount;
                    mapProcess.process.Add(pp);
                }
                npcActivatedData.proList.Add(mapProcess);
                Sys_Exploration.Instance.UpdateExplorationData(mapProcess.mapID, mapProcess.process);
            }
            eventEmitter.Trigger(EEvents.OnUpdateResPoint);
        }
        /// <summary>
        /// 激活Npc消息
        /// </summary>
        /// <param name="msg"></param>
        private void OnActivateNpcRes(NetMsg msg)
        {
            CmdNpcActivateNpcRes res = NetMsgUtil.Deserialize<CmdNpcActivateNpcRes>(CmdNpcActivateNpcRes.Parser, msg);
            if (npcActivatedData.activatedInfoId.Contains(res.InfoId))
                return;
            npcActivatedData.activatedInfoId.Add(res.InfoId);

            eventEmitter.Trigger<uint>(EEvents.OnActiveNPC, res.InfoId);

            CSVNpc.Data cSVNpcData = CSVNpc.Instance.GetConfData(res.InfoId);
            if (!Sys_Npc.Instance.IsMarkNpc(cSVNpcData, ENPCMarkType.None)) return;
            CSVMapInfo.Data cSVMapInfoData = CSVMapInfo.Instance.GetConfData(cSVNpcData.mapId);
            if (null == cSVMapInfoData || null == cSVMapInfoData.map_node || cSVMapInfoData.map_node.Count < 3) return;

            uint markType = cSVNpcData.mark_type[0];
            for (int i = 0; i < 3; i++)
            {
                int mapId = cSVMapInfoData.map_node[i];
                NpcActivatedData.MapProcess mapProcess = npcActivatedData.proList.Find(x => x.mapID == mapId);
                if (null == mapProcess)
                {
                    mapProcess = new NpcActivatedData.MapProcess();
                    mapProcess.mapID = (uint)cSVMapInfoData.map_node[i];
                    mapProcess.process = new List<NpcActivatedData.MapProcess.PP>();
                    npcActivatedData.proList.Add(mapProcess);
                }

                NpcActivatedData.MapProcess.PP pp = mapProcess.process.Find(x => x.markType == markType);
                if (null == pp)
                {
                    pp = new NpcActivatedData.MapProcess.PP();
                    pp.markType = markType;
                    pp.markCount = 1;
                    mapProcess.process.Add(pp);
                }
                else
                {
                    pp.markCount += 1;
                }
                Sys_Exploration.Instance.UpdateExplorationData(mapProcess.mapID, new List<NpcActivatedData.MapProcess.PP>() { pp });
            }
            Sys_Exploration.Instance.AddExplorationTip(cSVNpcData.mapId, cSVNpcData.id, markType);
            eventEmitter.Trigger(EEvents.OnUpdateResPoint);
        }
        /// <summary>
        /// 探索奖励结果
        /// </summary>
        /// <param name="msg"></param>
        private void OnNpcGetActiveRewardAck(NetMsg msg)
        {
            CmdNpcGetActiveRewardAck res = NetMsgUtil.Deserialize<CmdNpcGetActiveRewardAck>(CmdNpcGetActiveRewardAck.Parser, msg);
            if (!npcActivatedData.getRewards.Contains(res.Id))
                npcActivatedData.getRewards.Add(res.Id);

            Sys_Exploration.Instance.OnExplorationRewardNotice(res.Id, false);
            UIScheduler.Push(EUIID.UI_ExploreReward, res.Id, null, true, UIScheduler.popTypes[EUIPopType.WhenLastPopedUIClosed]);
        }
        #endregion
        #region 激活资源数据
        /// <summary> Npc激活数据 </summary>
        public class NpcActivatedData
        {
            public class MapProcess
            {
                public class PP
                {
                    public uint markType;
                    public uint markCount;
                }
                public uint mapID;
                public List<PP> process;
            }
            public List<uint> activatedInfoId = new List<uint>(64);
            public List<MapProcess> proList = new List<MapProcess>();
            public List<uint> getRewards = new List<uint>();

            public void Clear()
            {
                activatedInfoId.Clear();
                proList.Clear();
                getRewards.Clear();
            }
        }
        /// <summary> 资源数据(类服务器数据结构拷贝数据) </summary>
        public NpcActivatedData npcActivatedData = new NpcActivatedData();
        /// <summary>
        /// Npc是否激活
        /// </summary>
        /// <param name="npcId"></param>
        /// <returns></returns>
        public bool IsActivatedNpc(uint npcId)
        {
            return npcActivatedData.activatedInfoId.IndexOf(npcId) >= 0;
        }
        /// <summary>
        /// 是否已领取奖励
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool IsGetRewards(uint id)
        {
            return npcActivatedData.getRewards.Contains(id);
        }
        #endregion
        #region 地图资源数据
        /// <summary> 地图资源点数据 </summary>
        public class MapResPointData
        {
            public uint npcId { get; set; }
            public uint npcType { get; set; }
            public uint mainMarkType { get; set; }
            public uint subMarkType { get; set; }
            public Vector2 position { get; set; }

            //public Rect rect { get; set; }

            private bool _markState = false;
            public bool markState
            {
                get
                {
                    if (!_markState)
                        _markState = Sys_Npc.Instance.IsActivatedNpc(npcId);
                    return _markState;
                }
            }
        }
        /// <summary> 地图资源信息 </summary>
        public Dictionary<uint, Dictionary<uint, MapResPointData>> mapResDict = new Dictionary<uint, Dictionary<uint, MapResPointData>>();
        /// <summary>
        /// 清除地图资源数据
        /// </summary>
        public void ClearMapResPointData()
        {
            mapResDict.Clear();
        }
        /// <summary>
        /// 获取地图中资源点Npc列表
        /// </summary>
        /// <param name="mapId"></param>
        /// <returns></returns>
        public Dictionary<uint, MapResPointData> GetNpcListInMap(uint mapId)
        {
            if (mapResDict.ContainsKey(mapId))
            {
                return mapResDict[mapId];
            }
            else
            {
                return LoadNpcInfo(mapId);
            }
        }
        /// <summary>
        /// 读取资源点Npc信息
        /// </summary>
        /// <param name="mapId"></param>
        /// <returns></returns>
        public Dictionary<uint, MapResPointData> LoadNpcInfo(uint mapId)
        {
            Dictionary<uint, MapResPointData> mapRescoureDataDict = new Dictionary<uint, MapResPointData>();
            Sys_Map.MapClientData mapClientData = Sys_Map.Instance.GetMapClientData(mapId);

            if (null == mapClientData) return mapRescoureDataDict;

            if(mapClientData.rigNpcDict != null)
            {
                foreach (var npc in mapClientData.rigNpcDict)
                {
                    CSVNpc.Data cSVNpcData = CSVNpc.Instance.GetConfData(npc.Key);
                    if (!Sys_Npc.Instance.IsMarkNpc(cSVNpcData, ENPCMarkType.None))
                        continue;

                    Vector2 npcPos = npc.Value.posList[0].ToV2;
                    MapResPointData mapRescoureData = new MapResPointData();
                    mapRescoureData.mainMarkType = cSVNpcData.mark_type[0];
                    mapRescoureData.subMarkType = cSVNpcData.mark_type[cSVNpcData.mark_type.Count - 1];
                    mapRescoureData.npcId = cSVNpcData.id;
                    mapRescoureData.npcType = cSVNpcData.type;
                    mapRescoureData.position = npcPos;
                    //mapRescoureData.rect = npc.Value.rectList[0];
                    mapRescoureDataDict.Add(cSVNpcData.id, mapRescoureData);
                }
            }

            mapResDict.Add(mapId, mapRescoureDataDict);
            return mapRescoureDataDict;
        }
        /// <summary>
        /// 获取资源点信息
        /// </summary>
        /// <param name="mapId"></param>
        /// <param name="npcId"></param>
        /// <returns></returns>
        public MapResPointData GetMapResPointData(uint mapId, uint npcId)
        {
            MapResPointData data = null;
            var dict = GetNpcListInMap(mapId);
            if (null == dict) return data;
            dict.TryGetValue(npcId, out data);
            return data;
        }
        /// <summary>
        /// 当前地图未激活的NPC编号列表
        /// </summary>
        /// <param name="mapId"></param>
        /// <returns></returns>
        public List<uint> UnActivatedNpcList(uint mapId)
        {
            List<uint> list = new List<uint>();
            var Datas = GetNpcListInMap(mapId);

            foreach (var data in Datas.Values)
            {
                if (!IsActivatedNpc(data.npcId) && data.npcType == (uint)ENPCType.Note) //只包含记录点，需要激活的npc（排除任务相关）
                    list.Add(data.npcId);
            }
            return list;
        }
        #endregion
        #region Npc数据

        /// <summary>
        /// 是否进入npc区域
        /// </summary>
        /// <param name="mapId"></param>
        /// <param name="npcId"></param>
        /// <param name="pos"></param>
        /// <returns></returns>
        public bool IsInNpcArea(uint mapId, uint npcId, Transform trans)
        {
            Sys_Map.MapClientData mapClientData = Sys_Map.Instance.GetMapClientData(mapId);
            if (mapClientData == null)
                return false;

            Sys_Map.RigNpcData npcData = null;
            if (!mapClientData.rigNpcDict.TryGetValue(npcId, out npcData))
                return false;

            return npcData.scopeDetection.Contains(trans);
        }

        // 是否在npc的接受任务范围之内
        public bool IsInNpcReceiveArea(uint mapId, uint npcId, Transform trans) {
            Sys_Map.MapClientData mapClientData = Sys_Map.Instance.GetMapClientData(mapId);
            if (mapClientData == null)
                return false;

            Sys_Map.RigNpcData npcData = null;
            if (!mapClientData.rigNpcDict.TryGetValue(npcId, out npcData))
                return false;

            return npcData.scopeReceiveDetection.Contains(trans);
        }

        public bool IsInSafeArea(uint mapId, Transform trans) {
            Sys_Map.MapClientData mapClientData = Sys_Map.Instance.GetMapClientData(mapId);
            if (mapClientData == null)
                return false;

            if (trans == null) {
                return false;
            }

            for (int i = 0, length = mapClientData.safeAreaList.Count; i < length; ++i) {
                var area = mapClientData.safeAreaList[i];
                if (area.scopeDetection.Contains(trans)) {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 是否任务npc
        /// </summary>
        /// <param name="cSVNpcData"></param>
        /// <returns></returns>
        public bool IsTaskNpc(CSVNpc.Data cSVNpcData)
        {
            if (null == cSVNpcData)
                return false;

            if (cSVNpcData.type == (uint)ENPCType.Common && (IsMarkNpc(cSVNpcData, ENPCMarkType.LoveTask) || IsMarkNpc(cSVNpcData, ENPCMarkType.ChallengeTask)))
                return true;

            return false;
        }
        /// <summary>
        /// 是否传送npc
        /// </summary>
        /// <param name="cSVNpcData"></param>
        /// <returns></returns>
        public bool IsTransmitNpc(CSVNpc.Data cSVNpcData)
        {
            if (null == cSVNpcData)
                return false;

            if (cSVNpcData.type == (uint)ENPCType.Note && cSVNpcData.subtype == (uint)ENPCMarkType.Transmit)
                return true;

            return false;
        }
        /// <summary>
        /// 是否资源npc
        /// </summary>
        /// <param name="cSVNpcData"></param>
        /// <returns></returns>
        public bool IsResourcesNpc(CSVNpc.Data cSVNpcData)
        {
            if (null == cSVNpcData)
                return false;

            if (cSVNpcData.type == (uint)ENPCType.Note && (cSVNpcData.subtype == (uint)ENPCMarkType.Resources || cSVNpcData.subtype == (uint)ENPCMarkType.Collection
                || cSVNpcData.subtype == (uint)ENPCMarkType.Lumbering || cSVNpcData.subtype == (uint)ENPCMarkType.Fishing || cSVNpcData.subtype == (uint)ENPCMarkType.Mining
                || cSVNpcData.subtype == (uint)ENPCMarkType.Hunting || cSVNpcData.subtype == (uint)ENPCMarkType.CollectionNew 
                || cSVNpcData.subtype == (uint)ENPCMarkType.LumberingNew || cSVNpcData.subtype == (uint)ENPCMarkType.MiningNew))
            {
                return true;
            }

            return false;
        }
        /// <summary>
        /// 是否标记npc
        /// </summary>
        /// <param name="cSVNpcData"></param>
        /// <param name="eNPCMarkType"></param>
        /// <returns></returns>
        public bool IsMarkNpc(CSVNpc.Data cSVNpcData, ENPCMarkType eNPCMarkType)
        {
            if (null == cSVNpcData || null == cSVNpcData.mark_type)
                return false;

            if (eNPCMarkType == ENPCMarkType.None)
            {
                return cSVNpcData.mark_type.Count > 0;
            }
            else
            {
                return cSVNpcData.mark_type.Contains((uint)eNPCMarkType);
            }
        }
        /// <summary>
        /// 是否开启npc
        /// </summary>
        /// <param name="cSVNpcData"></param>
        /// <returns></returns>
        public bool IsOpenNpc(CSVNpc.Data cSVNpcData)
        {
            if (null == cSVNpcData)
                return false;

            bool isOpen = true;
            if (null != cSVNpcData.mark_type)
            {
                for (int j = 0; j < cSVNpcData.mark_type.Count; j++)
                {
                    uint id = cSVNpcData.mark_type[j];
                    if (!Sys_Exploration.Instance.IsOpen_ResPoint(id))
                    {
                        isOpen = false;
                        break;
                    }
                }
            }
            return isOpen;
        }
        /// <summary>
        /// 获取任务ID
        /// </summary>
        /// <param name="cSVNpcData"></param>
        /// <returns></returns>
        public uint GetTaskId(CSVNpc.Data cSVNpcData)
        {
            uint taskId = 0;
            if (cSVNpcData.type == (uint)ENPCType.Common) {
                var csv = CSVNpcFunctionData.Instance.GetConfData(cSVNpcData.id);
                if (csv != null) {
                    var taskDatas = csv.taskFunctions;
                    if (null != taskDatas && taskDatas.Count > 0 && taskDatas[0].Count > 6)
                    {
                        taskId = taskDatas[0][5];
                    }
                }
            }
            return taskId;
        }
        /// <summary>
        /// 获取资源类型ID
        /// </summary>
        /// <param name="cSVNpcData"></param>
        /// <returns></returns>
        public uint GetMarkId(CSVNpc.Data cSVNpcData)
        {
            uint markId = 0;
            if (cSVNpcData.mark_type != null && cSVNpcData.mark_type.Count > 0) {
                int index = cSVNpcData.mark_type.Count - 1;
                markId = cSVNpcData.mark_type[index];
            }
            else {
                if (cSVNpcData.type == (uint)ENPCType.Common)
                {
                    uint taskId = GetTaskId(cSVNpcData);
                    CSVTask.Data cSVTaskData = CSVTask.Instance.GetConfData(taskId);
                    if (null != cSVTaskData)
                    {
                        switch ((ETaskCategory)cSVTaskData.taskCategory)
                        {
                            case ETaskCategory.Love:
                            {
                                markId = (uint)ENPCMarkType.LoveTask;
                            }
                                break;
                            case ETaskCategory.Challenge:
                            {
                                markId = (uint)ENPCMarkType.ChallengeTask;
                            }
                                break;
                        }
                    }
                }
                else if (cSVNpcData.type == (uint)ENPCType.Note && cSVNpcData.subtype == (uint)ENPCMarkType.Transmit)
                {
                    markId = cSVNpcData.subtype;
                }
                else if (cSVNpcData.type == (uint)ENPCType.Note && (cSVNpcData.subtype == (uint)ENPCMarkType.Resources || cSVNpcData.subtype == (uint)ENPCMarkType.ResourcesNew))
                {
                    if (null == cSVNpcData.mark_type || cSVNpcData.mark_type.Count <= 0)
                    {
                        markId = 0;
                    }
                    else
                    {
                        int index = cSVNpcData.mark_type.Count - 1;
                        markId = cSVNpcData.mark_type[index];
                    }
                }
            }
            
            return markId;
        }
        #endregion
    }
}

