using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lib.Core;
using Table;
using System;

namespace Logic
{
    //public class PathFindGenWayPoint
    //{
    //    private uint _npcInfoId;
    //    private uint _taskId;

    //    private float _fTimeDuration;
    //    private float _fDisToTarget;
    //    private CSVNpc.Data _npcData;

    //    private Timer _timer;

    //    private Vector3[] arrCorners = new Vector3[256];

    //    private bool _enable;

    //    public PathFindGenWayPoint(uint npcInfoId, Action<Vector3> actionComplete = null, uint taskId = 0)
    //    {
    //        //_ActionCompleted = actionComplete;
    //        _npcInfoId = npcInfoId;
    //        //_taskId = taskId;

    //        float.TryParse(CSVParam.Instance.GetConfData(119).str_value, out _fTimeDuration);
    //        float.TryParse(CSVParam.Instance.GetConfData(120).str_value, out _fDisToTarget);
    //    }

    //    public bool DoFind()
    //    {
    //        _npcData = CSVNpc.Instance.GetConfData(_npcInfoId);

    //        _enable = true;
    //        _timer?.Cancel();
    //        _timer = Timer.Register(_fTimeDuration, () =>
    //        {
    //            CalWayPoint();
    //        }, null, true);

    //        CalWayPoint();

    //        return true;
    //    }

    //    private void CalWayPoint()
    //    {
    //        Vector2 desPos = Vector2.zero;

    //        Sys_Map.MapClientData desMapData = Sys_Map.Instance.GetMapClientData(_npcData.mapId);
    //        if (desMapData.rigNpcDict.ContainsKey(_npcInfoId))
    //        {
    //            desPos = desMapData.rigNpcDict[_npcInfoId].posList[0].ToV2;
    //        }
    //        else
    //        {
    //            DebugUtil.LogErrorFormat("Cant find for npcInfoId:{0} in mapId:{1}", _npcInfoId, _npcData.mapId);
    //            Sys_Map.Instance.eventEmitter.Trigger(Sys_Map.EEvents.OnWayPointsEnd);
    //            return;
    //        }

    //        //寻路路点,只会在同一个地图或者相邻地图
    //        if (_npcData.mapId == Sys_Map.Instance.CurMapId)
    //        {
    //            if (GameCenter.mainHero == null || GameCenter.mainHero.transform == null)
    //                return;

    //            //判断是否到目标点
    //            float disTance = Vector3.Distance(new Vector3(desPos.x, 0f, desPos.y), GameCenter.mainHero.transform.position);
    //            if (disTance > _fDisToTarget)
    //            {
    //                GenWayPoint(desPos);
    //            }
    //            else
    //            {
    //                Sys_Map.Instance.eventEmitter.Trigger(Sys_Map.EEvents.OnWayPointsEnd);
    //            }
    //        }
    //        else
    //        {
    //            //判断是否是相连地图
    //            bool isNeighbour = false;
    //            Sys_Map.MapClientData curMapData = Sys_Map.Instance.GetMapClientData(Sys_Map.Instance.CurMapId);
    //            if (curMapData != null)
    //            {
    //                if (curMapData.telList != null)
    //                {
    //                    for (int i = 0; i < curMapData.telList.Count; ++i)
    //                    {
    //                        if (curMapData.telList[i].mapId == _npcData.mapId)
    //                        {
    //                            desPos = curMapData.telList[i].pos.ToV2;
    //                            isNeighbour = true;
    //                            break;
    //                        }
    //                    }
    //                }
    //            }
    //            else
    //            {
    //                DebugUtil.LogErrorFormat("Cant find  mapId:{0}", _npcData.mapId);
    //                Sys_Map.Instance.eventEmitter.Trigger(Sys_Map.EEvents.OnWayPointsEnd);
    //                return;
    //            }

    //            if (isNeighbour)
    //            {
    //                GenWayPoint(desPos);
    //            }
    //            else
    //            {
    //                DebugUtil.LogErrorFormat("非相邻地图不能打点");
    //                Sys_Map.Instance.eventEmitter.Trigger(Sys_Map.EEvents.OnWayPointsEnd);
    //                return;
    //            }
    //        }
    //    }

    //    /// <summary>
    //    /// 生成路点
    //    /// </summary>
    //    /// <param name="dePos"></param>
    //    /// <returns></returns>
    //    private void GenWayPoint(Vector2 dePos) {
    //        if (GameCenter.mainHero == null && GameCenter.mainHero.transform == null)
    //            return;

    //        if (!_enable)
    //            return;

    //        UnityEngine.AI.NavMeshPath path = new UnityEngine.AI.NavMeshPath();

    //        MovementComponent movCom = GameCenter.mainHero.movementComponent;

    //        bool isEnable = movCom.mNavMeshAgent.enabled;
    //        movCom.mNavMeshAgent.enabled = true;
    //        if (!movCom.mNavMeshAgent.isOnNavMesh)
    //            return;

    //        Vector3 targetPos = PosConvertUtil.Svr2Client((uint)(dePos.x * 100), (uint)(-dePos.y * 100));
    //        UnityEngine.AI.NavMeshHit navMeshHit;
    //        MovementComponent.GetNavMeshHit(targetPos, out navMeshHit);
    //        targetPos = navMeshHit.position;

    //        movCom.mNavMeshAgent.CalculatePath(targetPos, path);

    //        Array.Clear(arrCorners, 0, arrCorners.Length);
    //        int cornersCount = path.GetCornersNonAlloc(arrCorners);

    //        for (int i = 0; i < cornersCount; ++i)
    //        {
    //            int next = i + 1;
    //            if (next < cornersCount)
    //            {
    //                //方向向量
    //                Vector3 vector = arrCorners[next] - arrCorners[i];
    //                Vector3 normalize = vector.normalized;
    //                int length = Mathf.RoundToInt(vector.magnitude);
    //                for (int j = 0; j < length; ++j)
    //                {
    //                    Vector3 pos = arrCorners[i] + j * normalize;
    //                    GameCenter.AddWayPoint(pos);
    //                }
    //            }
    //        }

    //        movCom.mNavMeshAgent.enabled = isEnable;
    //    }

    //    public void OnDispose()
    //    {
    //        _enable = false;
    //        _timer?.Cancel();
    //        _timer = null;
    //        GameCenter.DestroyWayPoints();
    //    }

    //    public void OnEnable(bool enable)
    //    {
    //        _enable = enable;
    //    }
    //}

}

