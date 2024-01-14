using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lib.Core;
using Table;
using System;

namespace Logic
{
    public class PathFindMonsterTeam : PathFindBase
    {
        private uint _teamId;
        private uint _mapId;
        private uint _taskId;
        public PathFindMonsterTeam (uint teamId, uint mapId, Action<Vector3> actionComplete, uint taskId = 0u)
        {
            _teamId = teamId;
            _mapId = mapId;
            _ActionCompleted = actionComplete;
            _taskId = taskId;
        }

        public override bool DoFind()
        {
            Vector2 desPos = Vector2.zero;

            Sys_Map.MapClientData desMapData = Sys_Map.Instance.GetMapClientData(_mapId);
            if (desMapData.monsterTeamDict.ContainsKey(_teamId))
            {
                desPos = desMapData.monsterTeamDict[_teamId][0].ToV2;
            }
            else
            {
                DebugUtil.LogErrorFormat("Cant find for monsterTeamId:{0} in mapId:{1}", _teamId, _mapId);
                return false;
            }

            if (_mapId == Sys_Map.Instance.CurMapId)
            {
                return SameMapFindMonsterTeam(_teamId, desPos, desMapData);
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
                        for (int i = 0; i < curMapData.telList.Count; ++i)
                        {
                            if (curMapData.telList[i].mapId == _mapId)
                            {
                                isNeighbour = true;
                                break;
                            }
                        }
                    }
                }
                else
                {
                    DebugUtil.LogErrorFormat("Cant find  mapId:{0}", _mapId);
                    return false;
                }

                if (isNeighbour)
                {
                    return NeighbourMapFind(desPos, desMapData, _taskId);
                }
                else
                {
                    return NoNeighbourMapFind(desPos, desMapData, _taskId);
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
        private bool SameMapFindMonsterTeam(uint teamId, Vector2 _desPos, Sys_Map.MapClientData _desMapData, uint taskId = 0)
        {
            if (GameCenter.mainHero == null || GameCenter.mainHero.transform == null)
                return false;

            //角色坐标
            Vector2 heroPos = Vector2.zero;
            heroPos.x = GameCenter.mainHero.transform.position.x;
            heroPos.y = GameCenter.mainHero.transform.position.z;

            //目标点和角色距离
            float distanceNpcToHero = Vector2.Distance(heroPos, _desPos);

            //检测是否有传送点
            uint transNpcId = 0;
            float distanceNpcToTrans = float.MaxValue;
            if (CalTransPoint(_desPos, _desMapData, ref transNpcId, ref distanceNpcToTrans, true))
            {
                if (distanceNpcToHero - distanceNpcToTrans > Sys_Map.Instance.TransThresholdValue)
                {
                    Sys_Map.Instance.ReqTelNpc(transNpcId, _desMapData.mapId, taskId);
                    return true;
                }
            }

            //寻路到目标点,刷怪点需要随机
            Vector3 targetPos = Vector3.zero;
            if (_desMapData.monsterTeamDict[teamId].Count > 0)
            {
                int length = _desMapData.monsterTeamDict[teamId].Count;
                int index = UnityEngine.Random.Range(0, length - 1);

                targetPos = _desMapData.monsterTeamDict[teamId][index].ToV3;
                //Vector2 tempPos = new Vector2(_desMapData.monsterTeamDict[teamId][index].X, _desMapData.monsterTeamDict[teamId][index].Y);
                //targetPos.x = tempPos.x;
                //targetPos.y = 0f;
                //targetPos.z = tempPos.y;
            }

            return ExcutePathFindAction(targetPos);
        }
    }

}

