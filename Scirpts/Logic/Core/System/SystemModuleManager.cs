using Framework;
using System.Collections.Generic;
using Lib;

namespace Logic
{
    public class SystemAttribute : System.Attribute { }

    public class SystemModuleManager
    {
        private static readonly List<ISystemModuleUpdate> _systemModelUpdates = new List<ISystemModuleUpdate>();        
        private static readonly List<ISystemModule> _systemModels = new List<ISystemModule>();
        private static readonly List<ISystemModuleApplicationPause> _systemModelApplicationPauses = new List<ISystemModuleApplicationPause>();

        private static int _initIndex = -1;

        internal static void Register()
        {
            SystemModuleRegister.Register();
        }

        internal static void Unregister()
        {
#if DEBUG_MODE
            float timePoint = UnityEngine.Time.realtimeSinceStartup;
#endif

            SystemModuleRegister.Unregister();

#if DEBUG_MODE
            DebugUtil.LogTimeCost(ELogType.eNone, "SystemModuleRegister.Unregister()", ref timePoint);
#endif
        }

        public static float Progress()
        {
            return (_systemModels.Count - _initIndex) / (float)_systemModels.Count;
        }

        public static void StartInit()
        {
            _initIndex = _systemModels.Count;
        }

        public static bool InitProgress()
        {
            float endTime = UnityEngine.Time.realtimeSinceStartup + 0.015f;
            while (endTime > UnityEngine.Time.realtimeSinceStartup)
            {
                --_initIndex;

                if (_initIndex >= 0)
                {
#if DEBUG_MODE
                    float timePoint = UnityEngine.Time.realtimeSinceStartup;
#endif
                    _systemModels[_initIndex].Init();
#if DEBUG_MODE
                    if (DebugUtil.IsOpenLogType(ELogType.eExecuteTime))
                    {
                        DebugUtil.LogTimeCost(ELogType.eExecuteTime, string.Format("{0}.Init()", _systemModels[_initIndex].GetType().ToString()), ref timePoint, 10);
                    }
#endif
                }
                else
                {
                    return true;
                }
            }

            return false;
        }

        public static void UnInit()
        {            
            for (int i = _systemModels.Count - 1; i >= 0; --i)
            {
                _systemModels[i].Dispose();
            }
            _initIndex = -1;
        }

        internal static void RegisterSystemModel(ISystemModule systemModel)
        {
            if (systemModel == null)
            {
                return;
            }

            _systemModels.Add(systemModel);
            ISystemModuleUpdate modelUpdate = systemModel as ISystemModuleUpdate;
            ISystemModuleApplicationPause modleApplicationPause = systemModel as ISystemModuleApplicationPause;
            if (modelUpdate != null)
            {                
                _systemModelUpdates.Add(modelUpdate);
                modelUpdate.SetOffsetFrame(_systemModelUpdates.Count);                
            }            

            if (modleApplicationPause != null)
                _systemModelApplicationPauses.Add(modleApplicationPause);
        }

        public static void OnLogin()
        {
#if DEBUG_MODE
            float timePoint = UnityEngine.Time.realtimeSinceStartup;
#endif
            for (int i = _systemModels.Count - 1; i >= 0; --i)
            {
                _systemModels[i].OnLogin();
#if DEBUG_MODE
                if (DebugUtil.IsOpenLogType(ELogType.eExecuteTime))
                {
                    DebugUtil.LogTimeCost(ELogType.eExecuteTime, string.Format("{0}.OnLogin()", _systemModels[i].GetType().ToString()), ref timePoint, 2);
                }                    
#endif
            }
        }
        public static void OnLogout()
        {
#if DEBUG_MODE
            float timePoint = UnityEngine.Time.realtimeSinceStartup;
#endif
            for (int i = _systemModels.Count - 1; i >= 0; --i)
            {
                _systemModels[i].OnLogout();
#if DEBUG_MODE
                if (DebugUtil.IsOpenLogType(ELogType.eExecuteTime))
                    DebugUtil.LogTimeCost(ELogType.eExecuteTime, string.Format("{0}.OnLogout()", _systemModels[i].GetType().ToString()), ref timePoint, 2);
#endif
            }
        }
        public static void OnUpdate()
        {
            float time = UnityEngine.Time.time;
            float unscaledTime = UnityEngine.Time.unscaledTime;

            for (int i = _systemModelUpdates.Count - 1; i >= 0; --i)
            {
                ISystemModuleUpdate systemModuleUpdate = _systemModelUpdates[i];
                if (systemModuleUpdate.CanUpdate())
                {
#if DEBUG_MODE
                    float t = UnityEngine.Time.realtimeSinceStartup;
#endif
                    systemModuleUpdate.UpdateTime(time, unscaledTime);
                    systemModuleUpdate.OnUpdate();
#if DEBUG_MODE
                    systemModuleUpdate.SetLastExecuteTime((int)((UnityEngine.Time.realtimeSinceStartup - t) * 1000000));
#endif
                }
            }
        }

        public static void OnSyncFinished()
        {
#if DEBUG_MODE
            float timePoint = UnityEngine.Time.realtimeSinceStartup;
#endif
            for (int i = _systemModels.Count - 1; i >= 0; --i)
            {
                _systemModels[i].OnSyncFinished();
#if DEBUG_MODE
                if (DebugUtil.IsOpenLogType(ELogType.eExecuteTime))
                    DebugUtil.LogTimeCost(ELogType.eExecuteTime, string.Format("{0}.OnSyncFinished()", _systemModels[i].GetType().ToString()), ref timePoint, 2);
#endif
            }
        }

        public static void OnApplicationPause(bool pause)
        {
            for (int i = _systemModelApplicationPauses.Count -1; i >= 0; --i)
            {
                _systemModelApplicationPauses[i].OnApplicationPause(pause);
            }
        }

        public static void OnLowMemory()
        {
            for (int i = _systemModels.Count - 1; i >= 0; --i)
            {
                ISystemModuleLowMemory lowMemory = _systemModels[i] as ISystemModuleLowMemory;
                lowMemory?.OnLowMemory();
            }
        }

        public static IReadOnlyList<ISystemModuleUpdate> GetSystemModelUpdates()
        {
            return _systemModelUpdates;
        }
    }
}