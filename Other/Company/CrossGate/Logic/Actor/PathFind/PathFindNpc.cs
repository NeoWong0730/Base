using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lib.Core;
using Table;
using System;

namespace Logic
{
    public class PathFindNpc : PathFindBase
    {
        private uint _npcInfoId;
        private uint _taskId;

        public PathFindNpc(uint npcInfoId, Action<Vector3> actionComplete, uint taskId = 0)
        {
            _ActionCompleted = actionComplete;
            _npcInfoId = npcInfoId;
            _taskId = taskId;
        }

        public override bool DoFind()
        {
            CSVNpc.Data npcData = CSVNpc.Instance.GetConfData(_npcInfoId);
            Vector2 desNpcPos = Vector2.zero;

            Sys_Map.MapClientData desMapData = Sys_Map.Instance.GetMapClientData(npcData.mapId);
            if (desMapData.rigNpcDict.ContainsKey(_npcInfoId))
            {
                desNpcPos = desMapData.rigNpcDict[_npcInfoId].posList[0].ToV2;
            }
            else
            {
                DebugUtil.LogErrorFormat("Cant find for npcInfoId:{0} in mapId:{1}", _npcInfoId, npcData.mapId);
                return false;
            }

            if (npcData.mapId == Sys_Map.Instance.CurMapId)
            {
                return SameMapFindNpc(npcData, desNpcPos, desMapData, _taskId);
            }
            else
            {
                //判断是否是相连地图
                bool isNeighbour = false;
                Sys_Map.MapClientData curMapData = Sys_Map.Instance.GetMapClientData(Sys_Map.Instance.CurMapId);
                if (curMapData != null)
                {
                    if (curMapData.telList != null)
                    {
                        var telList = curMapData.telList;
                        for (int i = 0; i < telList.Count; ++i)
                        {
                            if (telList[i].mapId == npcData.mapId)
                            {
                                isNeighbour = true;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    DebugUtil.LogErrorFormat("Cant find mapId:{0}", npcData.mapId);
                    return false;
                }

                if (isNeighbour)
                {
                    return NeighbourMapFind(desNpcPos, desMapData, _taskId);
                }
                else
                {
                    return NoNeighbourMapFind(desNpcPos, desMapData, _taskId);
                }
            }
        }

        /// <summary>
        /// 支持同一张地图寻路
        ///（1）判断已激活传送点中，到目标点的最近距离
        ///（2）将角色到目标的最近距离与传送点到目标点的最近距离进行对比
        ///   <1> 角色到目标点的距离更近，直接寻路到目标点
        ///   <2> 传送点到目标点的距离更近，传送点传送点，再寻路到目标点
        /// </summary>
        /// <param name="_npcData"></param>
        private bool SameMapFindNpc(CSVNpc.Data _npcData, Vector2 _desNpcPos, Sys_Map.MapClientData _desMapData, uint taskId = 0)
        {
            if (GameCenter.mainHero == null || GameCenter.mainHero.transform == null)
                return false;

            //角色坐标
            Vector2 heroPos = Vector2.zero;
            heroPos.x = GameCenter.mainHero.transform.position.x;
            heroPos.y = GameCenter.mainHero.transform.position.z;

            //目标点和角色距离
            float distanceNpcToHero = Vector2.Distance(heroPos, _desNpcPos);

            //检测是否有传送点
            uint transNpcId = 0;
            float distanceNpcToTrans = float.MaxValue;
            if (CalTransPoint(_desNpcPos, _desMapData, ref transNpcId, ref distanceNpcToTrans, true))
            {
                float disHeroToTrans = 0f;
                foreach (var data in _desMapData.rigNpcDict)
                {
                    if (data.Key == transNpcId)
                    {
                        disHeroToTrans = Vector2.Distance(heroPos, data.Value.posList[0].ToV2);
                        break;
                    }
                }

                if (disHeroToTrans > Sys_Map.Instance.TransProtectedDistance)
                {
                    CSVNpc.Data transInfoData = CSVNpc.Instance.GetConfData(transNpcId);
                    if (transInfoData.subtype == (int)ENPCNoteSubType.Transmit) //子类型,使用传送
                    {
                        if (distanceNpcToHero - distanceNpcToTrans > Sys_Map.Instance.TransThresholdValue)
                        {
                            //TODO: 传送
                            Sys_Map.Instance.ReqTelNpc(transNpcId, _desMapData.mapId, taskId);
                            return true;
                        }
                    }
                }
            }

            //寻路到目标点
            Vector3 npcPos = Vector3.zero;
            npcPos.x = _desNpcPos.x;
            npcPos.y = 0f;
            npcPos.z = _desNpcPos.y;

            npcPos.x += _npcData.dialogueEndParameter[0] / 10000f;
            npcPos.y += _npcData.dialogueEndParameter[1] / 10000f;
            npcPos.z += _npcData.dialogueEndParameter[2] / 10000f;

            return ExcutePathFindAction(npcPos);
        }
    }

}

