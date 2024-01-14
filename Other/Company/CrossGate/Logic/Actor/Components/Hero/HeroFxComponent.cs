using UnityEngine;
using System;
using Logic.Core;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Lib.Core;
using Table;

namespace Logic
{
    public class HeroFxComponent : Logic.Core.Component
    {
        private SceneActor sceneActor;
        private AsyncOperationHandle<GameObject> mHandle;
        private Timer fxTimer;

        public uint CurFxID
        {
            get;
            private set;
        }

        public GameObject FxObj
        {
            get;
            private set;
        }

        public Action<GameObject> onLoaded;

        protected override void OnConstruct()
        {
            base.OnConstruct();

            sceneActor = actor as SceneActor;
        }

        protected override void OnDispose()
        {
            sceneActor = null;
            FxObj = null;
            onLoaded = null;

            AddressablesUtil.ReleaseInstance(ref mHandle, MHandle_Completed);
            CurFxID = 0;

            base.OnDispose();
        }


        public void UpdateLevelUpFx(bool NeedLoadFx = true)
        {
            if (NeedLoadFx)
            {
                LoadLevelUpFx();
            }
        }

        public void UpdateAdvanceFx(bool NeedLoadFx = true)
        {
            if (NeedLoadFx)
            {
                LoadAdvanceFx();
            }
        }

        public void UpdateReputationFx(bool NeedLoadFx = true)
        {
            if (NeedLoadFx)
            {
                LoadReputationFx();
            }
        }

        public void ShowFx()
        {
            if (FxObj.activeInHierarchy)
            {
                FxObj.SetActive(false);
            }
            FxObj.SetActive(true);
        }

        private  void UnloadFx()
        {
            FxObj = null;
            AddressablesUtil.ReleaseInstance(ref mHandle, MHandle_Completed);
        }

        private void LoadLevelUpFx()
        {
            string finalModelPath = "Prefab/Fx/Fx_level_up.prefab";
            AddressablesUtil.InstantiateAsync(ref mHandle, finalModelPath, MHandle_Completed);

            uint fxtime=0;
            uint.TryParse(CSVParam.Instance.GetConfData(263).str_value, out fxtime);
            fxTimer?.Cancel();
            fxTimer = Timer.Register(fxtime, () =>
            {
                UnloadFx();
                fxTimer.Cancel();
            }, null, false, false);
        }

        private void LoadAdvanceFx()
        {
            string finalModelPath = "Prefab/Fx/Fx_level_up02.prefab";
            AddressablesUtil.InstantiateAsync(ref mHandle, finalModelPath, MHandle_Completed);

            uint fxtime = 0;
            uint.TryParse(CSVParam.Instance.GetConfData(263).str_value, out fxtime);
            fxTimer?.Cancel();
            fxTimer = Timer.Register(fxtime, () =>
            {
                UnloadFx();
                fxTimer.Cancel();
            }, null, false, false);
        }

        private void LoadReputationFx()
        {
            string finalModelPath = "Prefab/Fx/Fx_level_up01.prefab";
            AddressablesUtil.InstantiateAsync(ref mHandle, finalModelPath, MHandle_Completed);

            uint fxtime = 0;
            uint.TryParse(CSVParam.Instance.GetConfData(263).str_value, out fxtime);
            fxTimer?.Cancel();
            fxTimer = Timer.Register(fxtime, () =>
            {
                UnloadFx();
                fxTimer.Cancel();
            }, null, false, false);
        }

        private void Loaded(GameObject go)
        {
            FxObj = go;
            FxObj.transform.SetParent(sceneActor.modelTransform, false);
            FxObj.transform.position = sceneActor.modelTransform.position;
            sceneActor.SetLayer(FxObj.transform);

            onLoaded?.Invoke(FxObj);
        }

        private void MHandle_Completed(AsyncOperationHandle<GameObject> handle)
        {
            Loaded(handle.Result);
        }
    }
}
