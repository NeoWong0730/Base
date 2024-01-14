using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lib.Core;
using Table;
using System;

namespace Logic
{
    public abstract class PathFindBase
    {

        protected Action<Vector3> _ActionCompleted;

        public virtual bool DoFind() { return false; }

        /// <summary>
        /// 相连地图的跨地图寻路
        ///（1）判断目标地图是否有激活传送点，如果有传送到距离目标点最近的传送点再寻路  到目标点
        ///（2）目标地图不存在传送点，判断当前地图已激活传送点到地图跳转点的最近距离
        ///   <1>角色到地图跳转点的距离更近，先寻路到地图跳转点，跳转到目标地图，再寻路到的目标点
        ///   <2>传送点到地图跳转点的距离更近，先传送到传送点，再寻路到地图跳转点，跳转到目标地图，再寻路到目标点
        /// </summary>
        /// <param></param>
        protected bool NeighbourMapFind(Vector2 _desPos, Sys_Map.MapClientData _desMapData, uint taskId = 0)
        {
            //----目标地图
            if (GameCenter.mainHero == null || GameCenter.mainHero.transform == null)
                return false;

            //目标传送点，距目标点的距离 小于 当前点距跳转点距离 + 目标掉距跳转点距离，才开始传送
            uint transNpcId = 0;
            float distanceNpcToTrans = float.MaxValue;
            float totalDistance = 0f;
            Sys_Map.MapClientData curMapData = Sys_Map.Instance.GetMapClientData(Sys_Map.Instance.CurMapId);

            //跳转点位置
            Vector2 telPos = Vector2.zero;
            if (curMapData.telList != null)
            {
                for (int i = 0; i < curMapData.telList.Count; ++i)
                {
                    if (curMapData.telList[i].mapId == _desMapData.mapId)
                    {
                        telPos = new Vector2(curMapData.telList[i].pos.X, curMapData.telList[i].pos.Y);
                        break;
                    }
                }
            }
           
            //计算角色距跳转点距离
            Vector2 heroPos = Vector2.zero;
            heroPos.x = GameCenter.mainHero.transform.position.x;
            heroPos.y = GameCenter.mainHero.transform.position.z;
            float distanceRoleToTel = Vector2.Distance(heroPos, telPos);

            totalDistance += distanceRoleToTel;

            //计算目标点距目标地图跳转点距离
            bool hadTelPoint = false;
            if (_desMapData.telList != null)
            {
                for (int i = 0; i < _desMapData.telList.Count; ++i)
                {
                    if (_desMapData.telList[i].mapId == Sys_Map.Instance.CurMapId)
                    {
                        Vector2 tempTel = new Vector2(_desMapData.telList[i].pos.X, _desMapData.telList[i].pos.Y);
                        hadTelPoint = true;
                        totalDistance += Vector2.Distance(tempTel, _desPos);
                        break;
                    }
                }
            }
            //计算目标地图传送点
            if (CalTransPoint(_desPos, _desMapData, ref transNpcId, ref distanceNpcToTrans))
            {
                //如果目标地图没有到当前地图跳转点，则直接传送
                if (!hadTelPoint)
                {
                    Sys_Map.Instance.ReqTelNpc(transNpcId, _desMapData.mapId, taskId);
                    return true;
                }

                //如果行走的距离大于 传送的距离, 则直接传送
                if (totalDistance  > distanceNpcToTrans)
                {
                    Sys_Map.Instance.ReqTelNpc(transNpcId, _desMapData.mapId, taskId);
                    return true;
                }
            }

            //---当前地图
            //计算距离跳转点最近的传送点
            transNpcId = 0;
            float distanceTransToTel = float.MaxValue;
            if (CalTransPoint(telPos, curMapData, ref transNpcId, ref distanceTransToTel, true))
            {
                if (distanceRoleToTel > distanceTransToTel)
                {
                    //判断角色到传送点距离
                    float disHeroToTrans = 0f;
                    foreach (var data in curMapData.rigNpcDict)
                    {
                        if (data.Key == transNpcId)
                        {
                            disHeroToTrans = Vector2.Distance(heroPos, data.Value.posList[0].ToV2);
                            break;
                        }
                    }

                    if (disHeroToTrans > Sys_Map.Instance.TransProtectedDistance)
                    {
                        //TODO: 传送
                        Sys_Map.Instance.ReqTelNpc(transNpcId, curMapData.mapId, taskId);
                        return true;
                    }
                }
            }

            //寻路到跳转点
            Vector3 targetPos = Vector3.zero;
            targetPos.x = telPos.x;
            targetPos.y = 0f;
            targetPos.z = telPos.y;

            return ExcuteTransmitAction(targetPos);
        }

        /// <summary>
        /// 支持非相连地图的跨地图寻路
        ///（1）先判断目标地图是否存在已激活的传送点
        ///   <1>存在传送点，传送到离目标点距离最近的传送点，从传送点向寻路到目标点
        ///   <2>不存在传送点，判断是否存在相邻地图
        ///（2）如果存在相邻地图，判断相邻地图是否存在已激活的传送点
        ///   <1>存在激活的传送点，则传送到离跳转点距离最近的传送点
        ///   <2>不存在激活的传送点，则无法传送，返回错误提示“目标太远，无法自动前往”
        ///（3）不存在与目标地图相邻的地图，返回错误提示“目标太远，无法自动前往”
        /// </summary>
        /// <param></param>
        protected bool NoNeighbourMapFind(Vector2 _desNpcPos, Sys_Map.MapClientData _desMapData, uint taskId = 0)
        {
            //----目标地图有传送点直接传送
            uint transNpcId = 0;
            float distanceNpcToTrans = float.MaxValue;
            if (CalTransPoint(_desNpcPos, _desMapData, ref transNpcId, ref distanceNpcToTrans))
            {
                // 传送
                Sys_Map.Instance.ReqTelNpc(transNpcId, _desMapData.mapId, taskId);
                return true;
            }

            //根据寻路表寻路
            CSVNaviMap.Data navMap = CSVNaviMap.Instance.GetConfData(_desMapData.mapId);
            if (navMap != null)
            {
                if (navMap.navigation_map != null)
                {
                    int index = navMap.navigation_map.IndexOf(Sys_Map.Instance.CurMapId);
                    //判断索引大小
                    if (index >= 0)
                    {
                        //判断前面是否有可以传送的
                        for (int i = 0; i < index; ++i)
                        {
                            uint tempMapId = navMap.navigation_map[i];
                            uint lastMapId = i == 0 ? _desMapData.mapId : navMap.navigation_map[i - 1];
                            Sys_Map.MapClientData tempMapData = Sys_Map.Instance.GetMapClientData(tempMapId);
                            if (SpecialCalTransPoint(lastMapId, tempMapData, ref transNpcId, ref distanceNpcToTrans))
                            {
                                //传送
                                Sys_Map.Instance.ReqTelNpc(transNpcId, tempMapData.mapId, taskId);
                                return true;
                            }
                        }

                        //寻路过去
                        uint telMapId = index == 0 ? Sys_Map.Instance.CurMapId : navMap.navigation_map[index - 1];

                        //跳转点位置
                        Vector2 telPos = Vector2.zero;
                        Sys_Map.MapClientData curMap = Sys_Map.Instance.GetMapClientData(Sys_Map.Instance.CurMapId);
                        if (curMap.telList != null)
                        {
                            for (int i = 0; i < curMap.telList.Count; ++i)
                            {
                                if (curMap.telList[i].mapId == telMapId)
                                {
                                    telPos = new Vector2(curMap.telList[i].pos.X, curMap.telList[i].pos.Y);
                                    break;
                                }
                            }
                        }

                        //寻路到跳转点
                        Vector3 targetPos = Vector3.zero;
                        targetPos.x = telPos.x;
                        targetPos.y = 0f;
                        targetPos.z = telPos.y;

                        return ExcuteTransmitAction(targetPos);
                    }
                    else
                    {
                        for (int i = 0; i < navMap.navigation_map.Count; ++i)
                        {
                            uint tempMapId = navMap.navigation_map[i];
                            uint lastMapId = i == 0 ? _desMapData.mapId : navMap.navigation_map[i - 1];
                            Sys_Map.MapClientData tempMapData = Sys_Map.Instance.GetMapClientData(tempMapId);
                            if (SpecialCalTransPoint(lastMapId, tempMapData, ref transNpcId, ref distanceNpcToTrans))
                            {
                                //传送
                                Sys_Map.Instance.ReqTelNpc(transNpcId, tempMapData.mapId, taskId);
                                return true;
                            }
                        }
                    }
                }
            }

            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(50000));
            return false;
        }

        private bool SpecialCalTransPoint(uint lastMapId, Sys_Map.MapClientData _MapData, ref uint _transNpcId, ref float _disNpcToTrans)
        {
            Vector2 telPos = Vector2.zero;
            if (_MapData.telList != null)
            {
                for (int i = 0; i < _MapData.telList.Count; ++i)
                {
                    if (_MapData.telList[i].mapId == lastMapId)
                    {
                        telPos = new Vector2(_MapData.telList[i].pos.X, _MapData.telList[i].pos.Y);
                        break;
                    }
                }
            }

            foreach (var data in _MapData.rigNpcDict)
            {
                CSVNpc.Data tempNpcData = CSVNpc.Instance.GetConfData(data.Key);
                if (tempNpcData == null)
                {
                    DebugUtil.LogErrorFormat("CSVNpc rigNpc not found id={0}", data.Key);
                    continue;
                }

                if (tempNpcData.type == (uint)ENPCType.Note
                    && tempNpcData.subtype == (uint)ENPCNoteSubType.Transmit) //传送类型
                {
                    //判断开启
                    if (Sys_Npc.Instance.IsActivatedNpc(data.Key))
                    {

                        float tempDistance = Vector2.Distance(data.Value.posList[0].ToV2, telPos);
                        if (tempDistance < _disNpcToTrans)
                        {
                            _disNpcToTrans = tempDistance;
                            _transNpcId = data.Key;
                        }
                    }
                }
            }

            if (_transNpcId != 0)
            {
                return true;
            }

            return false;
        }

        protected bool CalTransPoint(Vector2 _desPos, Sys_Map.MapClientData _MapData, ref uint _transNpcId, ref float _disNpcToTrans, bool checkMarkType = false)
        {
            bool havePoint = false;

            //遍历目标地图传送点
            if (_MapData.rigNpcDict != null)
            {
                foreach (var data in _MapData.rigNpcDict)
                {
                    CSVNpc.Data tempNpcData = CSVNpc.Instance.GetConfData(data.Key);
                    if (tempNpcData == null)
                    {
                        DebugUtil.LogErrorFormat("CSVNpc rigNpc not found id={0}", data.Key);
                        continue;
                    }

                    if (tempNpcData.type == (uint)ENPCType.Note
                        && tempNpcData.subtype == (uint)ENPCNoteSubType.Transmit) //传送类型
                    {
                        //判断开启
                        if (Sys_Npc.Instance.IsActivatedNpc(data.Key))
                        {
                            if (checkMarkType)
                            {
                                if (!Sys_Npc.Instance.IsMarkNpc(tempNpcData, ENPCMarkType.Transmit))
                                {
                                    continue;
                                }
                            }

                            float tempDistance = Vector2.Distance(data.Value.posList[0].ToV2, _desPos);
                            if (tempDistance < _disNpcToTrans)
                            {
                                _disNpcToTrans = tempDistance;
                                _transNpcId = data.Key;
                            }
                        }
                    }
                }
            }
            

            if (_transNpcId != 0)
            {
                havePoint = true;
            }

            return havePoint;
        }

        /// <summary>
        /// ExcuteTransmitAction
        /// </summary>
        /// <param name="pos"></param>
        protected bool ExcuteTransmitAction(Vector3 pos)
        {
            List<ActionBase> actionBases = new List<ActionBase>();
            PathFindAction pathFindAction = ActionCtrl.Instance.CreateAction(typeof(PathFindAction)) as PathFindAction;
            if (pathFindAction != null)
            {
                pathFindAction.targetPos = pos;
                pathFindAction.Init();
                actionBases.Add(pathFindAction);
            }

            ActionCtrl.Instance.AddAutoActions(actionBases);

            return true;
        }

        /// <summary>
        /// ExcutePathFindAction
        /// </summary>
        /// <param name="pos"></param>
        protected bool ExcutePathFindAction(Vector3 pos)
        {
            List<ActionBase> actionBases = new List<ActionBase>();
            PathFindAction pathFindAction = ActionCtrl.Instance.CreateAction(typeof(PathFindAction)) as PathFindAction;
            DebugUtil.LogFormat(ELogType.eTask, "ExcutePathFindAction");
            if (pathFindAction != null)
            {
                pathFindAction.targetPos = pos;
                pathFindAction.Init(null, () =>
                {
                    _ActionCompleted?.Invoke(pos);
                    Sys_Map.Instance.eventEmitter.Trigger(Sys_Map.EEvents.OnPathFindComplete);
                });

                actionBases.Add(pathFindAction);
            }

            ActionCtrl.Instance.AddAutoActions(actionBases);

            return true;
        }

        public virtual void OnDispose() { }
    }

}

