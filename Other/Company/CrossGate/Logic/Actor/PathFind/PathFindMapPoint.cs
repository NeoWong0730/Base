using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Lib.Core;


namespace Logic
{
    public class PathFindMapPoint : PathFindBase
    {
        private uint _mapId;
        private uint _taskId;
        private Vector2 _point;

        public PathFindMapPoint(uint mapId, Vector2 point, Action<Vector3> actionComplete, uint taskId = 0)
        {
            _ActionCompleted = actionComplete;
            _mapId = mapId;
            _taskId = taskId;
            _point = point;
        }

        public override bool DoFind()
        {
            Vector2 desPos = Vector2.zero;

            Sys_Map.MapClientData desMapData = Sys_Map.Instance.GetMapClientData(_mapId);
            if (_mapId == Sys_Map.Instance.CurMapId) //同一地图寻路
            {
                return SameMapFindPoint(_point, desMapData);
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
                    return NeighbourMapFind(_point, desMapData, _taskId);
                }
                else
                {
                    return NoNeighbourMapFind(_point, desMapData, _taskId);
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
        private bool SameMapFindPoint(Vector2 _desNpcPos, Sys_Map.MapClientData _desMapData, uint taskId = 0)
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
                if (distanceNpcToHero - distanceNpcToTrans > Sys_Map.Instance.TransThresholdValue)
                {
                    //TODO: 传送
                    Sys_Map.Instance.ReqTelNpc(transNpcId, _desMapData.mapId, taskId);
                    return true;
                }
            }

            //寻路到目标点
            Vector3 targetPos = Vector3.zero;
            targetPos.x = _desNpcPos.x;
            targetPos.y = 0f;
            targetPos.z = _desNpcPos.y;
            return ExcutePathFindAction(targetPos);
        }
    }
}
