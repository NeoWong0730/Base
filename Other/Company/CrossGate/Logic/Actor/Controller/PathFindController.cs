using UnityEngine;
using System.Collections.Generic;
using Logic.Core;
using System;
using Table;
using Lib.Core;

namespace Logic
{
    //public class PathFindController : Actor
    //{
    //    private PathFindBase pathFindInstance;

    //    protected override void OnConstruct()
    //    {
    //        base.OnConstruct();

    //        ProcessEvents(true);
    //    }

    //    protected override void OnDispose()
    //    {
    //        ProcessEvents(false);
    //        ClearData();

    //        base.OnDispose();
    //    }

    //    private void ProcessEvents(bool register)
    //    {
    //        Sys_Map.Instance.eventEmitter.Handle(Sys_Map.EEvents.OnAutoPathFind, OnAutoPathFind, register);
    //        Sys_Map.Instance.eventEmitter.Handle(Sys_Map.EEvents.OnInterruptPathFind, OnInterrupt, register);
    //        Sys_Map.Instance.eventEmitter.Handle(Sys_Map.EEvents.OnPathFindComplete, OnComplete, register);
    //    }

    //    private void OnAutoPathFind()
    //    {
    //        if (pathFindInstance != null && !pathFindInstance.DoFind())
    //        {
    //            ClearData();
    //            Debug.LogErrorFormat("pathFind 失败");
    //        }
    //    }

    //    private void OnInterrupt()
    //    {
    //        ClearData();
    //    }

    //    /// <summary>
    //    /// 任务寻找npc
    //    /// </summary>
    //    /// <param name="npcId"></param>
    //    /// <param name="callback"></param>
    //    public void FindNpc(uint npcId, Action<Vector3> callback = null, uint taskId = 0)
    //    {
    //        DebugUtil.Log(ELogType.eTask, "开始寻路");
    //        CSVNpc.Data npcData = CSVNpc.Instance.GetConfData(npcId);
    //        if (npcData == null)
    //        {
    //            Debug.LogErrorFormat("CSVNpc not found npc id {0}", npcId);
    //            //callback?.Invoke(Vector3.zero);
    //            return;
    //        }

    //        pathFindInstance = new PathFindNpc(npcId, callback, taskId);
    //        if (!pathFindInstance.DoFind())
    //        {
    //            ClearData();
    //            DebugUtil.Log(ELogType.eTask, "FindNpc 失败");
    //        }

    //        DebugUtil.Log(ELogType.eTask, "DoFind");
    //    }

    //    /// <summary>
    //    /// 寻找怪物组
    //    /// </summary>
    //    /// <param name="npcId"></param>
    //    /// <param name="callback"></param>
    //    public void FindMonsterTeamId(uint teamId, uint mapId, Action<Vector3> callback = null, uint taskId = 0)
    //    {
    //        pathFindInstance = new PathFindMonsterTeam(teamId, mapId, callback, taskId);
    //        if (!pathFindInstance.DoFind())
    //        {
    //            ClearData();
    //            DebugUtil.Log(ELogType.eTask, "FindMonsterTeamId 失败");
    //        }
    //    }
    //    /// <summary>
    //    /// 寻找地图坐标
    //    /// </summary>
    //    /// <param name="mapId"></param>
    //    /// <param name="targetPoint"></param>
    //    /// <param name="callback"></param>
    //    public void FindMapPoint(uint mapId, Vector2 targetPoint, Action<Vector3> callback = null, uint taskId = 0)
    //    {
    //        pathFindInstance = new PathFindMapPoint(mapId, targetPoint, callback, taskId);
    //        if (!pathFindInstance.DoFind())
    //        {
    //            ClearData();
    //            DebugUtil.Log(ELogType.eTask, "FindMapPoint 失败");
    //        }
    //    }

    //    private void OnComplete()
    //    {
    //        //Debug.LogError("PathFind OnComplete");
    //        ClearData();
    //    }

    //    public void Interupt()
    //    {
    //        ClearData();
    //    }

    //    private void ClearData()
    //    {
    //        UIManager.CloseUI(EUIID.UI_PathFind);
    //        pathFindInstance?.OnDispose();
    //        pathFindInstance = null;
    //    }

    //}
}
