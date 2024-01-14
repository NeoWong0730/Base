using System.Collections.Generic;
using System;
using UnityEngine;
using Framework;

namespace Logic
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
                if (_mType == null)
                {
                    _mType = GetType();
                }
                return _mType;
            }
        }

        public virtual void OnEnter(LevelParams param, Type fromLevelType) { }

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
            UIManager.ClearUI();
            mLevelPreload.UnLoadAll();
        }

        public virtual void OnGUI() { }

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
