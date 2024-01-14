using Framework;
using Lib;

namespace Logic
{
    public interface ISystemModuleUpdate
    {
        int GetOffsetFrame();
        int GetIntervalFrame();
        int GetLastFrame();

        void SetOffsetFrame(int offsetFrame);
        void SetIntervalFrame(int intervalFrame);
        void SetLastExecuteTime(int microsecond);
        int GetLastExecuteTime();
        void OnUpdate();
        bool CanUpdate();
        void UpdateTime(float time, float unscaledTime);
    }

    public interface ISystemModuleApplicationPause
    {
        void OnApplicationPause(bool pause);
    }

    public interface ISystemModuleLowMemory 
    { 
        void OnLowMemory();
    }

    public interface ISystemModule
    {
        void Init();
        void Dispose();
        void OnLogin();
        void OnLogout();
        void OnSyncFinished();
    }

    public abstract class SystemModuleBase<T> : ISystemModule where T : class, new()
    {
        protected SystemModuleBase() { }

        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    DebugUtil.LogErrorFormat("Please Construct instance first then Use Instance: {0}", typeof(T).ToString());
                }
                return _instance;
            }
        }

        internal static T ConstructInstance()
        {
            if (null != _instance)
            {
                DebugUtil.LogWarningFormat("Please Dispose instance first then Construct : {0}", typeof(T).ToString());
                return null;
            }
            _instance = new T();
            return _instance;
        }

        //有需要重新创建系统类实例的时候使用
        //一般在严格要求数据初始话的情况下，防止手动初始化数据遗漏
        //本项目应该用不到
        internal static void DisposeInstance()
        {
            if (null == _instance)
            {
                DebugUtil.LogWarningFormat("Can not Dispose a null instance : {0}", typeof(T).ToString());
                return;
            }
            _instance = null;
        }

        private int _lastFrame = 0;
        private int LastExecuteTime = 0;

        public int _offsetFrame = 0;
        public int _intervalFrame = 24;  //默认按照5帧运行 //最大帧率120帧 最小帧率5帧

        private float _unscaledLastUpdateTime = 0f;
        private float _lastUpdateTime = 0f;
        private float _unscaledDeltaTime = 0f;
        private float _deltaTime = 0f;

        public void SetLastExecuteTime(int microsecond)
        {
            LastExecuteTime = microsecond;
        }

        public int GetLastExecuteTime()
        {
            return LastExecuteTime;
        }

        public virtual bool CanUpdate()
        {
            return TimeManager.CanExecute(_offsetFrame, _intervalFrame);
        }

        public void UpdateTime(float time, float unscaledTime)
        {
            _deltaTime = time - _lastUpdateTime;
            _unscaledDeltaTime = unscaledTime - _unscaledLastUpdateTime;

            _lastUpdateTime = time;
            _unscaledLastUpdateTime = unscaledTime;

#if DEBUG_MODE
            _lastFrame = UnityEngine.Time.frameCount;
#endif
        }

        public void SetOffsetFrame(int offsetFrame)
        {
            _offsetFrame = offsetFrame;
        }

        public void SetIntervalFrame(int intervalFrame)
        {            
            _intervalFrame = intervalFrame;
        }

        public int GetOffsetFrame() { return _offsetFrame; }
        public int GetIntervalFrame() { return _intervalFrame; }
        public int GetLastFrame() { return _lastFrame; }

        public float GetDeltaTime() { return _deltaTime; }
        public float GetUnscaledDeltaTime() { return _unscaledDeltaTime; }
        public virtual void Init() { }
        public virtual void Dispose() { }
        public virtual void OnLogin() { }
        public virtual void OnLogout() { }
        public virtual void OnSyncFinished() { }
    }
}