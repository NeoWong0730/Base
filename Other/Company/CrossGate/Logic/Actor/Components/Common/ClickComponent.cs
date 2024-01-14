using Logic.Core;
using UnityEngine;

namespace Logic
{
    public interface IClick
    {
        void OnClick();
    }

    public interface IDoubleClick
    {
        void OnDoubleClick();
    }

    /// <summary>
    /// 单击功能组件///
    /// </summary>
    public class ClickComponent : Logic.Core.Component
    {
        public static bool GlobalEnableFlag = true;

        float fLastClickTime = 0;
        float cd = 0.5f;
        public bool enableFlag = true;

        public EInteractiveAimType InteractiveAimType
        {
            get;
            set;
        }

        public ELayerMask LayMask
        {
            get;
            set;
        } = ELayerMask.Default;

        protected override void OnDispose()
        {
            InteractiveAimType = EInteractiveAimType.None;
            enableFlag = true;
            fLastClickTime = 0f;
            LayMask = ELayerMask.Default;

            base.OnDispose();
        }

        public void OnClick()
        {
            if (!GlobalEnableFlag)
                return;

            if (!enableFlag)
                return;

            if (Time.unscaledTime - fLastClickTime < cd)
                return;

            fLastClickTime = Time.unscaledTime;

            Sys_Interactive.Instance.eventEmitter.Trigger<InteractiveEvtData>(EInteractiveType.Click, new InteractiveEvtData()
            {
                eInteractiveAimType = InteractiveAimType,
                sceneActor = actor as ISceneActor,
                immediately = false,
            });
        }
    }

    /// <summary>
    /// 双击功能组件///
    /// </summary>
    public class DoubleClickComponent : Logic.Core.Component
    {
        public static bool GlobalEnableFlag = true;

        float fLastClickTime = 0;
        float cd = 0.5f;
        public bool enableFlag = true;

        public EInteractiveAimType InteractiveAimType
        {
            get;
            set;
        }

        public ELayerMask LayMask
        {
            get;
            set;
        } = ELayerMask.Default;

        protected override void OnDispose()
        {
            InteractiveAimType = EInteractiveAimType.None;
            enableFlag = true;
            fLastClickTime = 0f;
            LayMask = ELayerMask.Default;

            base.OnDispose();
        }

        public void OnDouleClick()
        {
            if (!GlobalEnableFlag)
                return;

            if (!enableFlag)
                return;

            if (Time.unscaledTime - fLastClickTime < cd)
                return;

            fLastClickTime = Time.unscaledTime;

            Sys_Interactive.Instance.eventEmitter.Trigger<InteractiveEvtData>(EInteractiveType.DoubleClick, new InteractiveEvtData()
            {
                eInteractiveAimType = InteractiveAimType,
                sceneActor = actor as ISceneActor,
                immediately = false,
            });
        }
    }
}


