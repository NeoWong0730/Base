using Lib;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Framework
{
    public static class SceneManager
    {
        public readonly static Dictionary<string, SceneEntry> mScenes = new Dictionary<string, SceneEntry>();

        private static Scene _defaultScene;
        public static Scene DefaultScene { get => _defaultScene; }

        private static SceneEntry _mainScene;
        public static SceneEntry MainScene { get => _mainScene; }

        public static bool bMainSceneLoading
        {
            get
            {
                return _mainScene != null ? !_mainScene.IsDone : false;
            }
        }

        public static void Init(Scene defaultScene)
        {
            _defaultScene = defaultScene;
        }

        public static void RegisterCallback()
        {
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
#if DEBUG_MODE
            UnityEngine.SceneManagement.SceneManager.sceneUnloaded += OnSceneUnloaded;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += OnActiveSceneChanged;
#endif
        }

        private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            string sceneName = scene.name;

            DebugUtil.LogFormat(ELogType.eScene, "OnSceneLoaded({0}, {1})", sceneName, mode.ToString());

            SceneEntry entry = null;
            if (!mScenes.TryGetValue(sceneName, out entry))
            {
                UnityEngine.SceneManagement.SceneManager.UnloadSceneAsync(scene);
                DebugUtil.LogErrorFormat("Unload error-load scene {0} {1}", sceneName, mode.ToString());
                return;
            }

            if (entry == _mainScene)
            {
                UnityEngine.SceneManagement.SceneManager.SetActiveScene(scene);
            }
            
            CameraManager.CheckCamera();
        }

#if DEBUG_MODE
        private static void OnActiveSceneChanged(Scene arg0, Scene arg1)
        {
            DebugUtil.LogFormat(ELogType.eScene, "OnActiveSceneChanged({0}, {1})", arg0.name, arg1.name);
        }

        private static void OnSceneUnloaded(Scene scene)
        {
            string sceneName = scene.name;
            DebugUtil.LogFormat(ELogType.eScene, "OnSceneUnloaded({0})", sceneName);
        }
#endif

        public static void LoadSceneAsync(string sceneName, LoadSceneMode loadSceneMode = LoadSceneMode.Single, bool active = true)
        {
            DebugUtil.LogFormat(ELogType.eScene, "LoadSceneAsync({0})", sceneName);

            if (string.IsNullOrWhiteSpace(sceneName))
                return;

            SceneEntry entry = null;
            if (!mScenes.TryGetValue(sceneName, out entry))
            {
                entry = new SceneEntry(sceneName);
                mScenes.Add(sceneName, entry);
            }

            entry.Load();
            entry.SetActive(active);

            if (loadSceneMode == LoadSceneMode.Single)
            {
                _mainScene = entry;
                if (_mainScene.mScene.IsValid() && _mainScene.mScene.isLoaded)
                {
                    UnityEngine.SceneManagement.SceneManager.SetActiveScene(_mainScene.mScene);
                }
            }
        }

        public static void UnLoadAllScene()
        {
            DebugUtil.LogFormat(ELogType.eScene, "UnLoadAllScene()");
            UnityEngine.SceneManagement.SceneManager.SetActiveScene(_defaultScene);

            foreach (var item in mScenes.Values)
            {
                item.Unload();
            }

            _mainScene = null;
        }

        public static void UnLoadScene(string sceneName)
        {
            DebugUtil.LogFormat(ELogType.eScene, "UnLoadAdditiveScene({0})", sceneName);

            if (mScenes.TryGetValue(sceneName, out SceneEntry entry))
            {
                if (UnityEngine.SceneManagement.SceneManager.GetActiveScene() == entry.mScene)
                {
                    UnityEngine.SceneManagement.SceneManager.SetActiveScene(_defaultScene);
                }
                entry.Unload();
            }
        }

        public static void SetMainScene(string sceneName)
        {
            if (string.Equals(sceneName, _mainScene.sName, StringComparison.Ordinal))
                return;

            if (!mScenes.TryGetValue(sceneName, out SceneEntry entry))
            {
                DebugUtil.LogErrorFormat("scene {0} not load", sceneName);
                return;
            }

            _mainScene = entry;
            if (_mainScene != null && _mainScene.mScene.IsValid())
            {
                UnityEngine.SceneManagement.SceneManager.SetActiveScene(_mainScene.mScene);
            }
        }

        public static void SetSceneActive(string sceneName, bool active)
        {
            if (!mScenes.TryGetValue(sceneName, out SceneEntry entry))
            {
                DebugUtil.LogErrorFormat("scene {0} not load", sceneName);
            }
            entry.SetActive(active);
        }

        public static GameObject GetRoot(string sceneName)
        {
            if (mScenes.TryGetValue(sceneName, out SceneEntry entry))
            {
                return entry.Root;
            }
            return null;
        }

        public static bool ContainsState(string sceneName, ESceneState sceneState)
        {
            if (mScenes.TryGetValue(sceneName, out SceneEntry entry))
            {
                return entry.ContainState(sceneState);
            }
            return false;
        }

        public static ESceneState GetSceneState(string sceneName)
        {
            if (mScenes.TryGetValue(sceneName, out SceneEntry entry))
            {
                return entry.eSceneState;
            }
            return ESceneState.eNone;
        }
    }
}