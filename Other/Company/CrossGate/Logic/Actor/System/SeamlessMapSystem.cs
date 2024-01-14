using Lib.Core;
using Logic.Core;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Logic
{
    public class SeamlessMapSystem : LevelSystemBase
    {
        public class SceneRequest
        {
            private string _sceneName;
            private uint _mapID;

            public bool isLoaded
            {
                get
                {
                    return Framework.SceneManager.ContainsState(_sceneName, Framework.ESceneState.eFail | Framework.ESceneState.eSuccess);
                }
            }

            public uint mapID
            {
                set
                {
                    if (_mapID != value)
                    {
                        _mapID = value;
                        if (_mapID == 0u)
                        {
                            _sceneName = null;
                            return;
                        }

                        CSVMapInfo.Data mapInfo = CSVMapInfo.Instance.GetConfData(_mapID);
                        if (mapInfo != null)
                        {
                            _sceneName = mapInfo.path;
                        }
                        else
                        {
                            _sceneName = null;
                            DebugUtil.LogErrorFormat("未找到场景 {0} 的配置", _mapID.ToString());
                        }
                    }
                }
                get
                {
                    return _mapID;
                }
            }

            public void Load(bool active)
            {
                if (string.IsNullOrWhiteSpace(_sceneName))
                    return;
                Framework.SceneManager.LoadSceneAsync(_sceneName, active ? LoadSceneMode.Single : LoadSceneMode.Additive, active);
            }

            public void SetActive(bool active)
            {
                if (string.IsNullOrWhiteSpace(_sceneName))
                    return;
                Framework.SceneManager.SetSceneActive(_sceneName, active);
                if (active)
                {
                    Framework.SceneManager.SetMainScene(_sceneName);
                }
            }

            public void Unload()
            {
                if (string.IsNullOrWhiteSpace(_sceneName))
                    return;
                Framework.SceneManager.UnloadScene(_sceneName);
                _mapID = 0;
                _sceneName = null;

            }
        }

        private SceneRequest mCurScene;
        private SceneRequest mPreLoadScene;

        private uint nCurMapID = 0u;
        private Vector2 vCurPos = Vector2.zero;

        public static float fPreloadRange = 100f; //10^2
        public static float fMiniMoveDis = 4f; //1^2

        private bool enableAutoPreload = false;

        //public void Execute()
        public override void OnUpdate()
        {         
            if (GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Normal)
                return;

            if (GameCenter.mainHero == null)
                return;

            Transform mainHeroTransform = GameCenter.mainHero.transform;
            if (mainHeroTransform)
                return;

            Vector3 positionV3 = mainHeroTransform.position;

            uint mapID = Sys_Map.Instance.CurMapId;
            Vector2 pos = new Vector2(positionV3.x, positionV3.z);

            SetPosition(mapID, pos);
        }

        public void SetPosition(uint mapID, Vector2 pos)
        {            
            //if (mapID == nCurMapID && Vector2.SqrMagnitude(vCurPos - pos) <= fMiniMoveDis)// 因为sqrMagnitude在ilRuntime 实现了属性的重定向，而方法没有
            if (mapID == nCurMapID && (enableAutoPreload == false || mCurScene.isLoaded == false || (vCurPos - pos).sqrMagnitude <= fMiniMoveDis))
                return;

            nCurMapID = mapID;
            vCurPos = pos;

            //当主场景发生改变
            if (mCurScene == null || mCurScene.mapID != mapID)
            {
                //将原先的主场景设置为隐藏
                if (mCurScene != null)
                {
                    mCurScene.SetActive(false);
                    //TODO:TEMP在取消主场景设置时 将世界场景作为主场景 防止后续未加载新主场景导致 world被卸载
                    Framework.SceneManager.SetMainScene(EScene.World.ToString());
                }

                //判断预加载的场景是否为需要的场景，若是则交换
                //否则加载
                if (mPreLoadScene != null && mPreLoadScene.mapID == mapID)
                {
                    SceneRequest tmp = mCurScene;
                    mCurScene = mPreLoadScene;
                    mPreLoadScene = tmp;
                }
                else
                {
                    //判断当前主场景是否存在,没有则创建
                    if (mCurScene == null)
                    {
                        mCurScene = new SceneRequest();
                    }
                    else
                    {
                        mCurScene.Unload();
                    }

                    mCurScene.mapID = mapID;
                    mCurScene.Load(true);
                }
                mCurScene.SetActive(true);
            }

            //当主场景已经加载完成后, 开始计算需要预加载的场景
            //计算出最近的在范围内的跳转点
            if (enableAutoPreload)
            {
                if (mCurScene.isLoaded)
                {
                    uint curPreloadMapID = mPreLoadScene != null ? mPreLoadScene.mapID : 0u;
                    uint preloadMapID = 0u;
                    float minRange = fPreloadRange;

                    //获取可以跳转的场景
                    Sys_Map.MapClientData mapData = Sys_Map.Instance.GetMapClientData(mapID);

                    List<Sys_Map.TelData> telDatas = mapData != null ? mapData.telList : null;
                    int telDataCount = telDatas != null ? telDatas.Count : 0;

                    //if (telDataCount == 1)
                    //{
                    //    Sys_Map.TelData tel = telDatas[0];
                    //    if (tel != null && tel.tel != null)
                    //    {
                    //        preloadMapID = tel.tel.MapId;
                    //    }
                    //}
                    //else if (telDataCount > 1)
                    if (telDataCount > 0)
                    {
                        for (int i = 0; i < telDataCount; ++i)
                        {
                            Sys_Map.TelData tel = telDatas[i];
                            if (tel == null)
                            {
                                continue;
                            }

                            if (tel.mapId == nCurMapID)
                            {
                                continue;
                            }

                            //float sqrDis = Vector2.SqrMagnitude(tel.pos - pos);// 因为sqrMagnitude在ilRuntime 实现了属性的重定向，而方法没有
                            float sqrDis = (tel.pos.ToV2 - pos).sqrMagnitude;
                            if (preloadMapID == 0u && tel.mapId == curPreloadMapID)
                            {
                                minRange = Mathf.Min(sqrDis, minRange);
                                preloadMapID = tel.mapId;
                            }
                            else
                            {
                                if (sqrDis < minRange)
                                {
                                    minRange = sqrDis;
                                    preloadMapID = tel.mapId;
                                }
                            }
                        }
                    }

                    //加载需要预加载的地图
                    if (preloadMapID == 0u)
                    {
                        if (mPreLoadScene != null)
                        {
                            mPreLoadScene.Unload();
                            mPreLoadScene.mapID = 0u;
                        }
                    }
                    else
                    {
                        SetPreloadMap(preloadMapID);
                    }
                }
            }
            else
            {
                if (mPreLoadScene != null)
                {
                    mPreLoadScene.Unload();
                    mPreLoadScene = null;
                }
            }
        }

        public void SetPreloadMap(uint preloadMapID)
        {
            if (preloadMapID == 0u)
                return;

            if (preloadMapID == nCurMapID)
                return;

            if (mPreLoadScene == null || mPreLoadScene.mapID != preloadMapID)
            {
                if (mPreLoadScene == null)
                {
                    mPreLoadScene = new SceneRequest();
                }
                else
                {
                    mPreLoadScene.Unload();
                }

                mPreLoadScene.mapID = preloadMapID;
                mPreLoadScene.Load(false);
                mPreLoadScene.SetActive(false);
            }
        }

        public bool IsMainSceneLoaded()
        {
            return mCurScene == null ? false : mCurScene.isLoaded;
        }

        public override void OnDestroy()
        {         
            if(mPreLoadScene != null)
            {
                mPreLoadScene.Unload();
            }
            if(mCurScene != null)
            {
                mCurScene.Unload();
            }            
        }
    }
}