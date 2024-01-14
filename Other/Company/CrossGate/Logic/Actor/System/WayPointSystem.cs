using Lib.Core;
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Logic
{
    /// <summary>
    /// 路点系统
    /// </summary>
    public class WayPointSystem : LevelSystemBase
    {
        private Vector3[] arrCorners = new Vector3[256];
        private List<WayPoint> wayPoints = new List<WayPoint>(256);

        private bool _enable = true;

        private uint _npcInfoId;
        private CSVNpc.Data _npcData;

        private float _fTimeDuration;
        private float _fDisToTarget;
        private float _deathTime;        
        
        public AsyncOperationHandle<GameObject> mHandle;
        public GameObject mFxTemplate = null;
        private bool bLoaded = false;

        private bool bNeedPath = false;
        private float fCalTimePoint = 0f;
        private float fDeathTimePoint = 0f;
        private int nCurrentCount = 0;

        public override void OnCreate()
        {
            float.TryParse(CSVParam.Instance.GetConfData(119).str_value, out _fTimeDuration);
            float.TryParse(CSVParam.Instance.GetConfData(120).str_value, out _fDisToTarget);
            float.TryParse(CSVParam.Instance.GetConfData(121).str_value, out _deathTime);

            AddressablesUtil.LoadAssetAsync<GameObject>(ref mHandle, "Prefab/Fx/scene/Fx_Manage.prefab", MHandle_Completed);

            ProcessEvents(true);            
        }

        private void MHandle_Completed(AsyncOperationHandle<GameObject> handle)
        {
            if (handle.Status != AsyncOperationStatus.Succeeded)
            {
                DebugUtil.LogError("模型加载失败 策划看看 上面的报错信息里面资源名称 检查配置");
                return;
            }
            mFxTemplate = handle.Result;
            bLoaded = true;
        }

        public override void OnDestroy()
        {
            ProcessEvents(false);

            _npcInfoId = 0;
            _npcData = null;
            bNeedPath = false;

            for (int i = 0; i < wayPoints.Count; ++i)
            {
                WayPoint point = wayPoints[i];
                GameObject.DestroyImmediate(point.transform);
            }
            wayPoints.Clear();
            nCurrentCount = 0;

            mFxTemplate = null;
            AddressablesUtil.Release<GameObject>(ref mHandle, MHandle_Completed);
            bLoaded = false;
        }

        public override void OnUpdate()
        {
            if (bNeedPath && bLoaded)
            {
                float currentTime = Time.unscaledTime;
                if (currentTime >= fCalTimePoint)
                {
                    CalWayPoint();
                    fCalTimePoint = currentTime + _fTimeDuration;
                    fDeathTimePoint = currentTime + _deathTime;                    
                }
                else if (nCurrentCount > 0 && currentTime >= fDeathTimePoint)
                {
                    HidePath();
                }
            }
        }

        private void ProcessEvents(bool register)
        {
            Sys_Map.Instance.eventEmitter.Handle(Sys_Map.EEvents.OnEnterMap, OnEnterMap, register);
            Sys_Map.Instance.eventEmitter.Handle(Sys_Map.EEvents.OnAutoPathFind, OnAutoPathFind, register);
            Sys_Map.Instance.eventEmitter.Handle<uint>(Sys_Map.EEvents.OnGenWayPoints, OnGenWayPoints, register);
            Sys_Map.Instance.eventEmitter.Handle(Sys_Map.EEvents.OnWayPointsEnd, OnWayPointsEnd, register);

            if (register)
            {
                Sys_Fight.Instance.OnEnterFight += OnEnterFight;
                Sys_Fight.Instance.OnExitFight += OnExitFight;
            }
            else
            {
                Sys_Fight.Instance.OnEnterFight -= OnEnterFight;
                Sys_Fight.Instance.OnExitFight -= OnExitFight;
            }
        }

        private void HidePath()
        {
            for (int i = wayPoints.Count - 1; i >= 0; --i)
            {
                wayPoints[i].Enable(false);
            }
            nCurrentCount = 0;            
        }

        private void ClosePath()
        {
            for (int i = wayPoints.Count - 1; i >= 0; --i)
            {
                wayPoints[i].Enable(false);
            }
            nCurrentCount = 0;
            bNeedPath = false;
        }

        private void OpenPath()
        {
            if (_npcData != null)
            {
                bNeedPath = true;
                fCalTimePoint = Time.unscaledTime;                
            }
        }

        /// <summary>
        /// 切换地图
        /// </summary>
        private void OnEnterMap()
        {
            ClosePath();            
        }

        /// <summary>
        /// 自动刷新
        /// </summary>
        private void OnAutoPathFind()
        {
            OpenPath();
        }

        /// <summary>
        /// 生成路点
        /// </summary>
        /// <param name="npcId"></param>
        private void OnGenWayPoints(uint npcId)
        {
            _npcInfoId = npcId;
            _npcData = CSVNpc.Instance.GetConfData(npcId);
            
            OpenPath();
        }

        private void OnWayPointsEnd()
        {
            _npcInfoId = 0u;
            _npcData = null;

            ClosePath();
        }

        private void CalWayPoint()
        {
            HidePath();

            if (_npcData == null)
                return;

            if (GameCenter.mainHero == null || GameCenter.mainHero.transform == null)
                return;

            Sys_Map.MapClientData desMapData = Sys_Map.Instance.GetMapClientData(_npcData.mapId);
            if(desMapData == null)
                return;

            if (!desMapData.rigNpcDict.TryGetValue(_npcInfoId, out Sys_Map.RigNpcData rigNpcData))
            {
                DebugUtil.LogErrorFormat("Cant find for npcInfoId:{0} in mapId:{1}", _npcInfoId, _npcData.mapId);
                Sys_Map.Instance.eventEmitter.Trigger(Sys_Map.EEvents.OnWayPointsEnd);
                return;
            }

            Vector2 desPos = rigNpcData.posList[0].ToV2;

            //寻路路点,只会在同一个地图或者相邻地图
            if (_npcData.mapId == Sys_Map.Instance.CurMapId)
            {
                //判断是否到目标点
                float disTance = Vector3.Distance(new Vector3(desPos.x, 0f, desPos.y), GameCenter.mainHero.transform.position);
                if (disTance > _fDisToTarget)
                {                    
                    GenWayPoint(desPos);                 
                }
                else
                {
                    Sys_Map.Instance.eventEmitter.Trigger(Sys_Map.EEvents.OnWayPointsEnd);
                }
            }
            else
            {
                //判断是否是相连地图                
                Sys_Map.MapClientData curMapData = Sys_Map.Instance.GetMapClientData(Sys_Map.Instance.CurMapId);
                if (curMapData == null)
                {
                    DebugUtil.LogErrorFormat("Cant find  mapId:{0}", Sys_Map.Instance.CurMapId);
                    Sys_Map.Instance.eventEmitter.Trigger(Sys_Map.EEvents.OnWayPointsEnd);
                    return;
                }

                bool isNeighbour = false;
                int telCount = curMapData.telList != null ? curMapData.telList.Count : 0;

                for (int i = 0; i < telCount; ++i)
                {
                    if (curMapData.telList[i].mapId == _npcData.mapId)
                    {
                        desPos = curMapData.telList[i].pos.ToV2;
                        isNeighbour = true;
                        break;
                    }
                }

                if (isNeighbour)
                {                    
                    GenWayPoint(desPos);
                }
                else
                {
                    DebugUtil.LogErrorFormat("非相邻地图不能打点");
                    Sys_Map.Instance.eventEmitter.Trigger(Sys_Map.EEvents.OnWayPointsEnd);                    
                }
            }
        }

        /// <summary>
        /// 生成路点
        /// </summary>
        /// <param name="dePos"></param>
        /// <returns></returns>
        private void GenWayPoint(Vector2 dePos)
        {            
            if (!_enable)
                return;

            UnityEngine.AI.NavMeshPath path = new UnityEngine.AI.NavMeshPath();

            MovementComponent movCom = GameCenter.mainHero.movementComponent;

            bool isEnable = movCom.mNavMeshAgent.enabled;
            movCom.mNavMeshAgent.enabled = true;
            if (!movCom.mNavMeshAgent.isOnNavMesh)
                return;

            Vector3 targetPos = PosConvertUtil.Svr2Client((uint)(dePos.x * 100), (uint)(-dePos.y * 100));
            UnityEngine.AI.NavMeshHit navMeshHit;
            MovementComponent.GetNavMeshHit(targetPos, out navMeshHit);
            targetPos = navMeshHit.position;

            movCom.mNavMeshAgent.CalculatePath(targetPos, path);

            Array.Clear(arrCorners, 0, arrCorners.Length);
            int cornersCount = path.GetCornersNonAlloc(arrCorners);

            for (int i = 0; i < cornersCount; ++i)
            {
                int next = i + 1;
                if (next < cornersCount)
                {
                    //方向向量
                    Vector3 vector = arrCorners[next] - arrCorners[i];
                    Vector3 normalize = vector.normalized;
                    int length = Mathf.RoundToInt(vector.magnitude);
                    for (int j = 0; j < length; ++j)
                    {
                        Vector3 pos = arrCorners[i] + j * normalize;
                        GenPoint(pos);
                        if (nCurrentCount >= 10)
                        {
                            break;
                        }
                    }

                    if (nCurrentCount >= 10)
                    {
                        break;
                    }
                }
            }

            movCom.mNavMeshAgent.enabled = isEnable;
        }

        private void GenPoint(Vector3 pos)
        {
            Vector3 targetPos = PosConvertUtil.Svr2Client((uint)(pos.x * 100), (uint)(-pos.z * 100));
            UnityEngine.AI.NavMeshHit navMeshHit;
            MovementComponent.GetNavMeshHit(targetPos, out navMeshHit);

            WayPoint point = null;
            if (nCurrentCount >= wayPoints.Count)
            {
                GameObject go = GameObject.Instantiate<GameObject>(mFxTemplate, GameCenter.wayPointRoot.transform);
                Transform tran = go.transform;
                tran.Setlayer(ELayerMask.Default);

                point = new WayPoint();
                point.transform = tran;
                wayPoints.Add(point);
            }
            else
            {
                point = wayPoints[nCurrentCount];
                point.Enable(true);
            }

            ++nCurrentCount;
            point.transform.localPosition = navMeshHit.position;
            point.DeathTime = _deathTime;
        }

        public void OnEnable(bool enable)
        {
            _enable = enable;
        }

        private void OnEnterFight(CSVBattleType.Data  data)
        {
            ClosePath();
        }

        private void OnExitFight()
        {
            OpenPath();
        }
    }
}