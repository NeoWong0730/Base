using System.Collections.Generic;
using Framework;
using Lib.Core;
using Logic;


namespace Table
{
    public static class CSVEditorMapDataExt
    {        

        public static Dictionary<uint, Sys_Map.MapClientData> mMapClientDatas = new Dictionary<uint, Sys_Map.MapClientData>();

        public static Sys_Map.MapClientData GetMapClientData(this CSVEditorMap table, uint id)
        {
            if (!mMapClientDatas.TryGetValue(id, out Sys_Map.MapClientData clientData))
            {
                if(table.TryGetValue(id, out CSVEditorMap.Data data))
                {
                    clientData = ParseData(data);
                    mMapClientDatas.Add(data.id, clientData);
                }                
            }
            return clientData;
        }

        public static Sys_Map.MapClientData TransData(this CSVEditorMap.Data data)
        {
            if (!mMapClientDatas.TryGetValue(data.id, out Sys_Map.MapClientData clientData))
            {
                clientData = ParseData(data);
                mMapClientDatas.Add(data.id, clientData);
            }
            return clientData;
        }        

        private static Sys_Map.MapClientData ParseData(CSVEditorMap.Data data)
        {
            Sys_Map.MapClientData clientData = new Sys_Map.MapClientData();
            clientData.mapId = data.id;

            //convert
            ConvertTel(data, clientData);
            ConvertSafeArea(data, clientData);
            ConvertWall(data, clientData);
            ConvertNpc(data, clientData);
            ConvertMonsterGP(data, clientData);

            return clientData;
        }
        public static void ConvertTel(CSVEditorMap.Data data, Sys_Map.MapClientData clientData)
        {
            if (data.telIds != null)
            {
                clientData.telList = new List<Sys_Map.TelData>(data.telIds.Count);

                for (int i = 0; i < data.telIds.Count; ++i)
                {
                    Sys_Map.TelData tel = new Sys_Map.TelData();
                    tel.mapId = data.telIds[i];
                    tel.condId = data.telCondIds[i];
                    tel.offX = data.telOffXs[i];
                    tel.offY = data.telOffYs[i];
                    tel.rangeX = (int)data.telRangeXs[i];
                    tel.rangeY = (int)data.telRangeYs[i];

                    tel.pos = new Int2((int)data.telPosXs[i], -(int)data.telPosYs[i]);

                    clientData.telList.Add(tel);
                }
            }
        }
        private static void ConvertWall(CSVEditorMap.Data data, Sys_Map.MapClientData clientData)
        {
            if (data.FightBlockCamp != null)
            {
                clientData.wallList = new List<Sys_Map.WallData>(data.FightBlockCamp.Count);

                for (int i = 0; i < data.FightBlockCamp.Count; ++i)
                {
                    var one = data.FightBlockCamp[i];
                    Sys_Map.WallData wall = new Sys_Map.WallData();
                    wall.campId = (int)one;

                    var pos = new UnityEngine.Vector2(data.FightBlockPos[i][0], -data.FightBlockPos[i][1]);
                    var range = new UnityEngine.Vector2(data.FightBlockSize[i][0], data.FightBlockSize[i][1]);
                    var offset = new UnityEngine.Vector2(data.FightBlockOffset[i][0], data.FightBlockOffset[i][1]);

                    wall.scopeDetection = new RectScopeDetection();
                    wall.scopeDetection.rectList = new List<UnityEngine.Rect>(1);

                    UnityEngine.Rect rect = new UnityEngine.Rect();
                    //UnityEngine.Vector4 rg = new UnityEngine.Vector4(range.x, range.y, offset.x, offset.y);
                    rect.position = new UnityEngine.Vector2(pos.x + offset.x - range.x / 2, pos.y + offset.y - range.y / 2);
                    rect.size = new UnityEngine.Vector2(range.x, range.y);
                    wall.scopeDetection.rectList.Add(rect);

                    clientData.wallList.Add(wall);
                }
            }
        }
        private static void ConvertSafeArea(CSVEditorMap.Data data, Sys_Map.MapClientData clientData)
        {
            if (data.FightBlockCamp != null)
            {
                clientData.safeAreaList = new List<Sys_Map.SateAreaData>(data.FightSafeAreaCamp.Count);

                for (int i = 0; i < data.FightSafeAreaCamp.Count; ++i)
                {
                    var one = data.FightSafeAreaCamp[i];
                    Sys_Map.SateAreaData wall = new Sys_Map.SateAreaData();
                    wall.campId = (int)one;

                    var pos = new UnityEngine.Vector2(data.FightSafeAreaPos[i][0], -data.FightSafeAreaPos[i][1]);
                    var range = new UnityEngine.Vector2(data.FightSafeAreaSize[i][0], data.FightSafeAreaSize[i][1]);
                    var offset = new UnityEngine.Vector2(data.FightSafeAreaOffset[i][0], data.FightSafeAreaOffset[i][1]);

                    wall.scopeDetection = new RectScopeDetection();
                    wall.scopeDetection.rectList = new List<UnityEngine.Rect>(1);

                    UnityEngine.Rect rect = new UnityEngine.Rect();
                    //UnityEngine.Vector4 rg = new UnityEngine.Vector4(range.x, range.y, offset.x, offset.y);
                    rect.position = new UnityEngine.Vector2(pos.x + offset.x - range.x / 2, pos.y + offset.y - range.y / 2);
                    rect.size = new UnityEngine.Vector2(range.x, range.y);
                    wall.scopeDetection.rectList.Add(rect);

                    clientData.safeAreaList.Add(wall);
                }
            }
        }
        private static void ConvertNpc(CSVEditorMap.Data data, Sys_Map.MapClientData clientData)
        {
            if (data.npcIds != null)
            {
                clientData.rigNpcDict = new Dictionary<uint, Sys_Map.RigNpcData>(data.npcIds.Count);

                for (int i = 0; i < data.npcIds.Count; ++i)
                {
                    Sys_Map.RigNpcData rigNpc = null;
                    if (!clientData.rigNpcDict.TryGetValue(data.npcIds[i], out rigNpc))
                    {
                        rigNpc = new Sys_Map.RigNpcData();
                        rigNpc.height = data.npcHeights[i];
                        rigNpc.width = data.npcWidths[i];
                        rigNpc.offX = data.npcOffXs[i];
                        rigNpc.offY = data.npcOffYs[i];
                        rigNpc.rotaX = data.npcRotaXs[i];
                        rigNpc.rotaY = data.npcRotaYs[i];
                        rigNpc.rotaZ = data.npcRotaZs[i];

                        rigNpc.posList = new List<Int2>();
                        rigNpc.scopeDetection = new RectScopeDetection();
                        rigNpc.scopeDetection.rectList = new List<UnityEngine.Rect>();

                        rigNpc.scopeReceiveDetection = new RectScopeDetection();
                        rigNpc.scopeReceiveDetection.rectList = new List<UnityEngine.Rect>();

                        clientData.rigNpcDict.Add(data.npcIds[i], rigNpc);
                    }

                    for (int j = 0; j < data.npcPosXs[i].Count; ++j)
                    {
                        Int2 pos = new Int2((int)data.npcPosXs[i][j], -(int)data.npcPosYs[i][j]);

                        rigNpc.posList.Add(pos);

                        //UnityEngine.Vector4 range = new UnityEngine.Vector4(rigNpc.width, rigNpc.height, rigNpc.offX, rigNpc.offY);
                        UnityEngine.Rect rect = new UnityEngine.Rect();
                        rect.position = new UnityEngine.Vector2(pos.X + rigNpc.offX - rigNpc.width / 2, pos.Y + rigNpc.offY - rigNpc.height / 2);
                        rect.size = new UnityEngine.Vector2(rigNpc.width, rigNpc.height);
                        rigNpc.scopeDetection.rectList.Add(rect);

                        if (Sys_Ini.Instance.Get<IniElement_Float>(114, out IniElement_Float distance))
                        {
                            //UnityEngine.Vector4 rangeTemp = new UnityEngine.Vector4(distance.value, distance.value, 0, 0);
                            var newRect = new UnityEngine.Rect();
                            newRect.position = new UnityEngine.Vector2(pos.X - distance.value, pos.Y - distance.value);
                            newRect.size = new UnityEngine.Vector2(distance.value * 2, distance.value * 2);
                            rigNpc.scopeReceiveDetection.rectList.Add(newRect);
                        }
                    }
                }
            }
        }
        private static void ConvertMonsterGP(CSVEditorMap.Data data, Sys_Map.MapClientData clientData)
        {
            if (data.monstergpIds != null)
            {
                clientData.monsterTeamDict = new Dictionary<uint, List<Int2>>();
                for (int i = 0; i < data.monstergpIds.Count; ++i)
                {
                    List<Int2> posList = null;
                    if (!clientData.monsterTeamDict.TryGetValue(data.monstergpIds[i], out posList))
                    {
                        posList = new List<Int2>(data.monstergpPosXs[i].Count);
                        clientData.monsterTeamDict.Add(data.monstergpIds[i], posList);
                    }

                    //DebugUtil.LogError(this.monstergpPosXs[i].Count.ToString());
                    for (int j = 0; j < data.monstergpPosXs[i].Count; ++j)
                    {
                        Int2 pos = new Int2((int)data.monstergpPosXs[i][j], -(int)data.monstergpPosYs[i][j]);
                        posList.Add(pos);
                    }
                }
            }
        }
    }
}