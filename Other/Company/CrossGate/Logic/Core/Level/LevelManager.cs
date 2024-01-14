using Lib.AssetLoader;
using Lib.Core;
using System;
using System.Collections.Generic;

namespace Logic.Core
{
    /// <summary>
    /// 加载界面类型
    /// </summary>
    public enum ELoadingType
    {
        eNormal = 0,
        eUnuseLoading = 1,
    }

    /// <summary>
    /// 关卡参数
    /// </summary>
    public struct LevelParams
    {
        public ELoadingType eLoadingType;
        public bool bCanSwitchToEqualLevelType;
        public object arg;
    }

    public static class LevelManager
    {
        private static readonly LevelParams gDefaultLevelParams = new LevelParams() { eLoadingType = ELoadingType.eNormal, arg = null };

        public enum ELevelSwitchState
        {
            None,
            StartSwitch,
            Switching,
            EndSwitch,
        }

        public static Action<Type, LevelParams> onLevelSwitchBegin;
        public static Action<Type, LevelParams> onLevelSwitchEnd;

        public static Type mCurrentLevelType { get; private set; }
        public static LevelBase mMainLevel { get { return _mMainLevel; } }
        private static LevelBase _mMainLevel;

        public static Type mSwitchLevelType { get; private set; }
        private static LevelBase _mSwitchLevel;
        private static LevelParams _mSwitchParamsBuffer;

        public static Type mBeforeLevelType { get; private set; }

        private static Type _mAfterLevelType;
        private static LevelParams _mAfterParamsBuffer;

        private static ELevelSwitchState eLevelSwitchState = ELevelSwitchState.None;
        public static float fSwitchProgress { get; private set; }

        public static bool IsSwitching
        {
            get
            {
                return eLevelSwitchState != ELevelSwitchState.None;
            }
        }

        public static void EnterLevel(Type scriptType)
        {
            EnterLevel(scriptType, gDefaultLevelParams);
        }

        public static void EnterLevel(Type scriptType, LevelParams param)
        {
            if (scriptType == null)
                return;

            DebugUtil.LogFormat(ELogType.eNone, "EnterLevel {0}", scriptType.ToString());

            if (!param.bCanSwitchToEqualLevelType)
            {                
                bool isEqual = mSwitchLevelType != null ? scriptType.Equals(mSwitchLevelType) : scriptType.Equals(mCurrentLevelType);
                if (isEqual)
                {
                    return;
                }
            }

            _mAfterLevelType = scriptType;
            _mAfterParamsBuffer = param;
        }

        public static void Update()
        {
            if (eLevelSwitchState == ELevelSwitchState.None)
            {
                if (_mAfterLevelType == null && _mMainLevel != null)
                {
                    _mMainLevel.OnUpdate();
                    
                    List<LevelSystemBase> levelSystems = _mMainLevel.mLevelSystems;
                    if(levelSystems != null)
                    {
                        for (int i = 0, len = levelSystems.Count; i < len; ++i)
                        {
                            LevelSystemBase levelSystem = levelSystems[i];
                            if (levelSystem.isActive != levelSystem.isActivePrev)
                            {
                                if (levelSystem.isActive)
                                {
                                    levelSystem.OnEnable();
                                }
                                else
                                {
                                    levelSystem.OnDisable();
                                }
                                levelSystem.isActivePrev = levelSystem.isActive;
                            }

                            if (levelSystem.isActive && levelSystem.CanUpdate())
                            {
#if DEBUG_MODE
                                float t = UnityEngine.Time.realtimeSinceStartup;
#endif
                                levelSystem.OnUpdate();
#if DEBUG_MODE
                                levelSystem.LastExecuteTime = (int)((UnityEngine.Time.realtimeSinceStartup - t) * 1000000);
                                levelSystem.LastExecuteFrame = UnityEngine.Time.frameCount;
#endif
                            }
                        }
                    }                    
                }

                if (_mAfterLevelType != null)
                {
                    if(!_mAfterLevelType.Equals(mCurrentLevelType))
                    {
                        _mSwitchLevel = Activator.CreateInstance(_mAfterLevelType) as LevelBase;
                    }
                    else
                    {
                        _mSwitchLevel = _mMainLevel;
                    }

                    if (_mSwitchLevel == null)
                    {
                        DebugUtil.LogErrorFormat("关卡切换失败：无法插件关卡实例 {0}", _mAfterLevelType.Name);
                    }
                    else
                    {
                        fSwitchProgress = 0f;
                        eLevelSwitchState = ELevelSwitchState.StartSwitch;

                        mSwitchLevelType = _mAfterLevelType;
                        _mSwitchParamsBuffer = _mAfterParamsBuffer;

                        onLevelSwitchBegin?.Invoke(mSwitchLevelType, _mSwitchParamsBuffer);
                    }
                    _mAfterLevelType = null;
                }
            }
            else
            {
                switch (eLevelSwitchState)
                {
                    case ELevelSwitchState.StartSwitch:
                        State_StartSwitch();
                        break;
                    case ELevelSwitchState.Switching:
                        State_Switching();
                        break;
                    case ELevelSwitchState.EndSwitch:
                        State_EndSwitch();
                        break;
                    default:
                        break;
                }
            }
        }

        public static void LateUpdate()
        {
            if (eLevelSwitchState == ELevelSwitchState.None)
            {
                if (_mMainLevel != null)
                {
                    _mMainLevel.LateUpdate();
                }
            }
        }

        public static void OnLowMemory()
        {
            if (eLevelSwitchState == ELevelSwitchState.None)
            {
                if (_mMainLevel != null)
                {
                    _mMainLevel.OnLowMemory();
                }
            }
        }

        public static void OnFixedUpdate()
        {
            if (eLevelSwitchState == ELevelSwitchState.None)
            {
                if (_mMainLevel != null)
                {
                    _mMainLevel.OnFixedUpdate();
                }
            }
        }
        public static void OnGUI()
        {
            if (eLevelSwitchState == ELevelSwitchState.None)
            {
                if (_mMainLevel != null)
                {
                    _mMainLevel.OnGUI();
                }
            }
        }

        public static void Exit()
        {
            _mMainLevel?.OnExit(null);
            _mMainLevel = null;
        }

        private static void State_StartSwitch()
        {
            _mMainLevel?.OnExit(mSwitchLevelType);
            _mMainLevel = null;

            mBeforeLevelType = mCurrentLevelType;
            mCurrentLevelType = null;

            //Todo--设置资源加载占用
            //AssetMananger.Instance.UseFastLoad = true;

            _mSwitchLevel?.OnEnter(_mSwitchParamsBuffer, mBeforeLevelType);

            //晚点销毁, 减少刚卸载掉又加载
            AssetMananger.Instance.UnloadUnusedAssets(true);

            eLevelSwitchState = ELevelSwitchState.Switching;
        }

        private static void State_Switching()
        {
            fSwitchProgress = _mSwitchLevel.OnLoading();
            if (fSwitchProgress >= 1f)
            {
                _mSwitchLevel.OnLoaded();

                //loading 完成主动GC 一次
                AssetMananger.Instance.UnloadUnusedAssets(true);

                eLevelSwitchState = ELevelSwitchState.EndSwitch;
            }
        }

        private static void State_EndSwitch()
        {      
            //Todo--设置资源占用
            //AssetMananger.Instance.UseFastLoad = false;

            onLevelSwitchEnd?.Invoke(mSwitchLevelType, _mSwitchParamsBuffer);

            _mMainLevel = _mSwitchLevel;
            _mSwitchLevel = null;

            mCurrentLevelType = mSwitchLevelType;
            mSwitchLevelType = null;

            eLevelSwitchState = ELevelSwitchState.None;
        }
    }
}