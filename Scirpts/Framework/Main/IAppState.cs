namespace Framework
{
    public enum EAppState
    {
        Invalid = 0,
        Game,
        HotFix,
        CheckVersion,
        WaitSdDK,
        WaitOtherAppHotfix,
        PerformanceCheck,
    }

    public interface IAppState
    {
        EAppState State { get; }
        void OnEnter();
        void OnExit();
        void OnUpdate();
        void OnFixedUpdate();
        void OnLateUpdate();
        void OnLowMemory();
        void OnGUI();
        void OnApplicationPause(bool pause);

    }
}
