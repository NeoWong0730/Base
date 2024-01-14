using UnityEngine;
using System.Collections.Generic;
using Logic.Core;
using Lib.Core;
using Table;
using Framework;

namespace Logic
{
    public partial class Sys_Map : SystemModuleBase<Sys_Map>
    {
        public class TelData
        {
            public uint mapId;
            public uint condId;
            public float offX;
            public float offY;
            public int rangeX;
            public int rangeY;
            public Int2 pos;
        }

        public class RigNpcData
        {
            public uint height;
            public uint width;
            public int offX;
            public int offY;
            public int rotaX;
            public int rotaY;
            public int rotaZ;
            public List<Int2> posList;
            public RectScopeDetection scopeDetection;
            public RectScopeDetection scopeReceiveDetection;
        }

        public class SateAreaData : WallData {
        }

        public class WallData {
            public int campId;
            public RectScopeDetection scopeDetection;
        }

        public class MapClientData
        {
            public uint mapId = 0;
            public List<TelData> telList;
            public List<WallData> wallList;
            public List<SateAreaData> safeAreaList;
            public Dictionary<uint, RigNpcData> rigNpcDict;
            public Dictionary<uint, List<Int2>> monsterTeamDict;
            public Dictionary<uint, List<Int2>> dictTeamDict;
        }

        private Dictionary<uint, MapClientData> mapDataDict = new Dictionary<uint, MapClientData>();

        public float TransProtectedDistance = 10f;
        public float TransThresholdValue = 6f;

        //private int ScaleSize = 100;

        private MapClientData _curMapData;

        public MapClientData GetMapClientData(uint mapId)
        {
            if (_curMapData != null && mapId == _curMapData.mapId)
                return _curMapData;

            MapClientData mapData = null;
            if (!mapDataDict.TryGetValue(mapId, out mapData))
            {
                float timePoint = Time.realtimeSinceStartup;

                CSVEditorMap.Data editorData = CSVEditorMap.Instance.GetConfData(mapId);
                if (editorData != null)
                {
                    mapData = editorData.TransData();
                    mapDataDict.Add(mapId, mapData);
                }
                else
                {
                    DebugUtil.LogErrorFormat("地图数据出错 {0}", mapId.ToString());
                }

                DebugUtil.LogTimeCost(ELogType.eNone, "parse map ", ref timePoint);
            }

            return mapData;
        }

        public TelData GetTelData(MapClientData fromMapData, uint toMapId) {
            if (fromMapData != null) {
                if (fromMapData.telList != null)
                {
                    for (int i = 0, length = fromMapData.telList.Count; i < length; ++i)
                    {
                        if (fromMapData.telList[i].mapId == toMapId)
                        {
                            return fromMapData.telList[i];
                        }
                    }
                }
               
            }
            return null;
        }

        public TelData GetTelData(uint fromMapId, uint toMapId) {
            MapClientData mapData = GetMapClientData(fromMapId);
            return GetTelData(mapData, toMapId);
        }

        private void GenTelPoint()
        {
            _curMapData = GetMapClientData(CurMapId);

            if (_curMapData.telList != null)
            {
                for (int i = 0; i < _curMapData.telList.Count; ++i)
                {
                    GameCenter.AddTeleporter(_curMapData.telList[i]);
                }
            }
        }
        private void GetWalls() {
            _curMapData = GetMapClientData(CurMapId);

            if (_curMapData.wallList != null) {
                for (int i = 0; i < _curMapData.wallList.Count; ++i) {
                    GameCenter.AddWall(_curMapData.wallList[i]);
                }
            }
        }

        private void GetSafeAreas() {
            _curMapData = GetMapClientData(CurMapId);

            if (_curMapData.safeAreaList != null) {
                for (int i = 0; i < _curMapData.safeAreaList.Count; ++i) {
                    GameCenter.AddSafeArea(_curMapData.safeAreaList[i]);
                }
            }
        }

        public void GetNpcPos(uint mapId, uint npcId, ref Vector3 pos, ref Quaternion eular)
        {
            MapClientData mapData = GetMapClientData(mapId);
            foreach (var data in mapData.rigNpcDict)
            {
                if (data.Key == npcId)
                {
                    if (data.Value.posList.Count > 0)
                    {
                        pos = data.Value.posList[0].ToV3 + new Vector3(data.Value.offX, 0f, data.Value.offY);

                        eular = Quaternion.Euler(data.Value.rotaX, data.Value.rotaY, data.Value.rotaZ);

                        return;
                    }
                    else
                    {
                        Debug.LogErrorFormat("npc pos is error  npcId={0}, mapId={1}", npcId, mapId);
                    }
                }

            }

            Debug.LogErrorFormat("not found npc={0} in mapId={1}", npcId, mapId);
        }

        #region 地图参数
        /// <summary> 参数类型 </summary>
        public enum EType
        {
            TargetMap = 0, //跳转到目标地图
            ResMark = 1,   //标记资源点
            CatchPet = 2,  //抓宠跳转地图且标记范围
            TargetNpc = 3, //显示目标npc内容
        }
        /// <summary> 参数类 </summary>
        public abstract class MapParameter { }
        /// <summary>目标地图参数 </summary>
        public class TargetMapParameter : MapParameter
        {
            public uint targetMapId; //目标地图Id
            public Vector3 fixedPosition; //固定界面位置

            public TargetMapParameter(uint targetMapId)
            {
                this.targetMapId = targetMapId;
                this.fixedPosition = Vector3.zero;
            }
            public TargetMapParameter(uint targetMapId, Vector3 fixedPosition)
            {
                this.targetMapId = targetMapId;
                this.fixedPosition = fixedPosition;
            }
        }
        /// <summary> 资源点参数 </summary>
        public class ResMarkParameter : MapParameter
        {
            public uint resMarkType; //资源标记Id
            public ResMarkParameter(uint resMarkType)
            {
                this.resMarkType = resMarkType;
            }
        }
        /// <summary> 抓宠参数 </summary>
        public class CatchPetParameter : MapParameter
        {
            public uint catchPetMapId;   //抓宠地图Id
            public Vector2 catchPetRange; //抓宠范围
            public Vector2 catchPetCenter; //抓宠中心点

            public CatchPetParameter(uint catchPetMapId, Vector2 catchPetRange, Vector2 catchPetCenter)
            {
                this.catchPetMapId = catchPetMapId;
                this.catchPetRange = catchPetRange;
                this.catchPetCenter = catchPetCenter;
            }
        }
        /// <summary> 目标npc </summary>
        public class TargetNpcParameter : MapParameter
        {
            public uint targetMapId; //目标地图Id
            public uint targetNpcId; //目标npcId

            public TargetNpcParameter(uint targetMapId, uint targetNpcId)
            {
                this.targetMapId = targetMapId;
                this.targetNpcId = targetNpcId;
            }
        }
        // 经典头目战
        public class TargetClassicBossParameter : MapParameter {
            public uint targetMapId; //目标地图Id
            public uint classicBossId; //经典头目战id

            public TargetClassicBossParameter(uint targetMapId, uint classicBossId) {
                this.targetMapId = targetMapId;
                this.classicBossId = classicBossId;
            }
        }
        #endregion
    }
}


