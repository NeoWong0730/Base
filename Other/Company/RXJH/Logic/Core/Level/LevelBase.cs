using Framework;
using System;
using System.Collections.Generic;

namespace Logic.Core
{
    public abstract class LevelBase
    {
        internal static AssetsPreload mLevelPreload = new AssetsPreload();

        internal List<LevelSystemBase> mLevelSystems;
        internal Dictionary<Type, LevelSystemBase> mLevelSystemDic;

        private Type _mType;
        public Type mType
        {
            get
            {
                if(_mType == null)
                {
                    _mType = GetType();
                }
                return _mType;
            }
        }
        /// <summary>
        /// 进入关卡
        /// </summary>
        /// <param name="param">参数</param>
        /// <param name="fromLevelType">关卡是否重新进入</param>
        public virtual void OnEnter(LevelParams param, Type fromLevelType) { }
        /// <summary>
        /// 关卡载入过程控制接口
        /// </summary>
        /// <returns>载入的进度 0 - 1</returns>
        //TODO 进度使用 int 比较安全
        public virtual float OnLoading()
        {
            mLevelPreload.OnLoading();
            return mLevelPreload.fProgress;
        }
        public virtual void OnLoaded() { }
        public virtual void OnUpdate() { }
        public virtual void OnFixedUpdate() { }
        public virtual void LateUpdate() { }
        public virtual void OnLowMemory() { }
        public virtual void OnExit(Type toLevelType)
        {
            //不受堆栈影响的ui 关卡切换的时候不会被销毁，会显示在登录界面上
            UIManager.ClearUI();
            mLevelPreload.UnloadAll();
        }
        public virtual void OnGUI()
        {

        }

        protected T GetOrCreateSystem<T>() where T : LevelSystemBase, new()
        {
            Type type = typeof(T);
            if (mLevelSystemDic.TryGetValue(type, out LevelSystemBase levelSystemBase))
            {
                return levelSystemBase as T;
            }
            else
            {
                T levelSystem = new T();
                levelSystem.mLevelBase = this;

                mLevelSystems.Add(levelSystem);
                mLevelSystemDic.Add(type, levelSystem);
#if DEBUG_MODE
                levelSystem.name = levelSystem.GetType().ToString();
#endif
                levelSystem.OnPreCreate();
                levelSystem.OnCreate();
                return levelSystem;
            }
        }

        public T GetSystem<T>() where T : LevelSystemBase, new()
        {
            Type type = typeof(T);
            if (mLevelSystemDic.TryGetValue(type, out LevelSystemBase levelSystemBase))
            {
                return levelSystemBase as T;
            }
            return null;
        }
    }
}