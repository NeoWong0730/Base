using UnityEngine;

namespace Framework
{
    public class AppState_HotFix : IAppState
    {
        public EAppState State
        {
            get
            {
                return EAppState.HotFix;
            }
        }

        public void OnEnter()
        {
            //Todo
            //VideoManager.Play();
            //if (VersionHelper.eHotFixMode != EHotFixMode.Close)
            //{
            //    UIHotFix.Create();
            //}

            //HotFixStateManager.Instance.OnEnter();
        }

        public void OnExit()
        {
            //HotFixStateManager.Instance.OnExit();
            //UIHotFix.Destory();
        }

        public void OnUpdate()
        {
            //HotFixStateManager.Instance.OnUpdate();
        }

        public void OnLateUpdate()
        {

        }

        public void OnFixedUpdate()
        {

        }

        public void OnGUI()
        {

        }

        public void OnApplicationPause(bool pause)
        {

        }

        public void OnLowMemory()
        {
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }
    }
}
