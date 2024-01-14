using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections;
using Lib.Core;
using Logic.Core;

namespace Logic
{
    //public class PathComponent : Logic.Core.Component
    //{
    //    public SceneActor sceneActor;
    //    Transform transform;
    //    private Transform transPathAgent;
    //    public NavMeshAgent pathAgent;

    //    private PathFindGenWayPoint genWayPoint;
    //    protected override void OnConstruct()
    //    {
    //        sceneActor = actor as SceneActor;
    //        transform = sceneActor.transform;

    //        ProcessEvents(true);
    //    }

    //    protected override void OnDispose()
    //    {

    //        ProcessEvents(false);
    //        sceneActor = null;
    //        transform = null;

    //        base.OnDispose();
    //    }

    //    private void ProcessEvents(bool register)
    //    {
    //        Sys_Map.Instance.eventEmitter.Handle(Sys_Map.EEvents.OnAutoPathFind, OnAutoPathFind, register);
    //        Sys_Map.Instance.eventEmitter.Handle<uint>(Sys_Map.EEvents.OnGenWayPoints, OnGenWayPoints, register);
    //        Sys_Map.Instance.eventEmitter.Handle(Sys_Map.EEvents.OnWayPointsEnd, ClearData, register);
    //    }

    //    private void OnAutoPathFind()
    //    {
    //        if (genWayPoint != null && !genWayPoint.DoFind())
    //        {
    //            ClearData();
    //            Debug.LogErrorFormat("genWayPoint 失败");
    //        }
    //    }

    //    private void ClearData()
    //    {
    //        genWayPoint?.OnDispose();
    //        genWayPoint = null;
    //    }

    //    /// <summary>
    //    /// 生成路点
    //    /// </summary>
    //    /// <param name="npcId"></param>
    //    private void OnGenWayPoints(uint npcId)
    //    {
    //        //Debug.LogError(npcId.ToString());
    //        genWayPoint?.OnDispose();
    //        genWayPoint = null;
    //        genWayPoint = new PathFindGenWayPoint(npcId);
    //        if (!genWayPoint.DoFind())
    //        {
    //            ClearData();
    //            DebugUtil.Log(ELogType.eTask, "GenWayPoint 失败");
    //        }
    //    }

    //    public void OnEnable(bool enable)
    //    {
    //        if (genWayPoint != null)
    //        {
    //            genWayPoint.OnEnable(enable);
    //        }
    //    }
    //}
}
