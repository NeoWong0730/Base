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
            if (UIManager.IsOpen(EUIID.UI_PromptBox))
                UIManager.CloseUI(EUIID.UI_PromptBox, needDestroy: true);
            if (UIManager.IsOpen(EUIID.UI_HangupTip))
                UIManager.CloseUI(EUIID.UI_HangupTip, needDestroy: true);
            if (UIManager.IsOpen(EUIID.UI_Pet_RemakeTips))
                UIManager.CloseUI(EUIID.UI_Pet_RemakeTips, needDestroy: true);
            UIManager.ClearUI();
            mLevelPreload.UnloadAll();
        }
        public virtual void OnGUI()
        {

        }

        public T GetOrCreateSystem<T>() where T : LevelSystemBase, new()
        {
            Type type = typeof(T);
            if (mLevelSystemDic.TryGetValue(type, out LevelSystemBase levelSystemBase))
            {
                return levelSystemBase as T;
            }
            else
            {
                T levelSystem = new T();

                mLevelSystems.Add(levelSystem);
                mLevelSystemDic.Add(type, levelSystem);
#if DEBUG_MODE
                levelSystem.name = levelSystem.GetType().ToString();
#endif
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