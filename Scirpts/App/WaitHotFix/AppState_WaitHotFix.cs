using UnityEngine;

namespace Framework
{
    public class AppState_WaitHotFix : IAppState
    {
        public EAppState State { get { return EAppState.WaitOtherAppHotfix; } }

        public void OnEnter()
        {
            //Todo
            //WaitAppHotFixManager.Instance.OnEnter();
        }

        public void OnExit()
        {
            //WaitAppHotFixManager.Instance.OnExit();
        }

        public void OnUpdate()
        {
            //WaitAppHotFixManager.Instance.OnUpdate();
        }

        public void OnFixedUpdate()
        {

        }

        public void OnLowMemory()
        {
            Resources.UnloadUnusedAssets();
            System.GC.Collect();
        }

        public void OnGUI()
        {

        }

        public void OnLateUpdate()
        {

        }

        public void OnApplicationPause(bool pause)
        {

        }
    }
}
