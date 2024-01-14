using System.Collections.Generic;
using System.Reflection;
using NWFramework;
using UnityEngine;

namespace HotFix
{
    public partial class GameApp : Singleton<GameApp>
    {
        private static List<Assembly> _hotfixAssembly;

        /// <summary>
        /// 热更域App主入口
        /// </summary>
        /// <param name="objects"></param>
        public static void Entrance(object[] objects)
        {
            _hotfixAssembly = (List<Assembly>)objects[0];
            Log.Warning("======= 看到此条日志代表你成功运行了热更新代码 =======");
            Log.Warning("======= Entrance GameApp =======");
            Instance.Init();
            Instance.Start();
            Utility.Unity.AddUpdateListener(Instance.Update);
            Utility.Unity.AddFixedUpdateListener(Instance.FixedUpdate);
            Utility.Unity.AddLateUpdateListener(Instance.LateUpdate);
            Utility.Unity.AddDestroyListener(Instance.OnDestroy);
            Utility.Unity.AddOnDrawGizmosListener(Instance.OnDrawGizmos);
            Utility.Unity.AddOnApplicationPauseListener(Instance.OnApplicationPause);
            GameModule.Procedure.RestartProcedure(new OnEnterGameAppProcedure());
            Instance.StartGameLogic();
        }

        /// <summary>
        /// 开始游戏业务层逻辑
        /// <remarks>显示UI、加载场景等</remarks>
        /// </summary>
        private void StartGameLogic()
        {
            GameModule.Base.gameObject.transform.FindChildByName("Test").gameObject.AddComponent<TestBehaviour>();
        }

        /// <summary>
        /// 关闭游戏
        /// </summary>
        /// <param name="shutdownType">关闭游戏框架类型</param>
        public static void Shutdown(ShutdownType shutdownType)
        {
            if (shutdownType == ShutdownType.None)
            {
                return;
            }

            NetClient.Instance.Close();

            if (shutdownType == ShutdownType.Restart)
            {
                Utility.Unity.RemoveUpdateListener(Instance.Update);
                Utility.Unity.RemoveFixedUpdateListener(Instance.FixedUpdate);
                Utility.Unity.RemoveLateUpdateListener(Instance.LateUpdate);
                Utility.Unity.RemoveDestroyListener(Instance.OnDestroy);
                Utility.Unity.RemoveOnDrawGizmosListener(Instance.OnDrawGizmos);
                Utility.Unity.RemoveOnApplicationPauseListener(Instance.OnApplicationPause);
                return;
            }
        }

        private void Start()
        {
            for (int i = 0, len = _listLogicMgr.Count; i < len; i++)
            {
                _listLogicMgr[i].OnStart();
            }
        }

        private void Update()
        {
            MessageRouter.Instance.Update();

            for (int i = 0, len = _listLogicMgr.Count; i < len; i++)
            {
                _listLogicMgr[i].OnUpdate();
            }
        }

        private void FixedUpdate()
        {
            for (int i = 0, len = _listLogicMgr.Count; i < len; i++)
            {
                _listLogicMgr[i].OnFixedUpdate();
            }
        }

        private void LateUpdate()
        {
            for (int i = 0, len = _listLogicMgr.Count; i < len; i++)
            {
                _listLogicMgr[i].OnLateUpdate();
            }
        }

        private void OnDestroy()
        {
            for (int i = 0, len = _listLogicMgr.Count; i < len; i++)
            {
                _listLogicMgr[i].OnDestroy();
            }
        }

        private void OnDrawGizmos()
        {
#if UNITY_EDITOR
            for (int i = 0, len = _listLogicMgr.Count; i < len; i++)
            {
                _listLogicMgr[i].OnDrawGizmos();
            }
#endif
        }

        private void OnApplicationPause(bool isPause)
        {
            for (int i = 0, len = _listLogicMgr.Count; i < len; i++)
            {
                _listLogicMgr[i].OnApplicationPause(isPause);
            }
        }
    }
}