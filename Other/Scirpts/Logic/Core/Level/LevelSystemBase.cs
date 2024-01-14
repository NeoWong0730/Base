using Framework;

namespace Logic
{
    public interface ILevelSystemConstruct
    {
        void OnLevelSystemContruct();
    }

    public abstract class LevelSystemBase
    {
        public bool isActive = true;
        public bool isActivePrev = false;

        public int offsetFrame = 0;
        public int intervalFrame = 24;

        public LevelBase mLevelBase;

#if DEBUG_MODE
        public int LastExecuteFrame = 0;
        public int LastExecuteTime = 0;
        public string name;
#endif

        public virtual bool CanUpdate()
        {
            return TimeManager.CanExecute(offsetFrame, intervalFrame);
        }

        public virtual void OnPreCreate() { }

        public virtual void OnEnable() { }

        public virtual void OnDisable() { }

        public virtual void OnCreate() { }

        public virtual void OnDestroy() { }

        public virtual void OnUpdate() { }

        public virtual void OnFixedUpdate() { }

        public virtual void OnLateUpdate() { }

        public T GetSystem<T>() where T : LevelSystemBase, new()
        {
            return mLevelBase.GetSystem<T>();
        }


    }
}