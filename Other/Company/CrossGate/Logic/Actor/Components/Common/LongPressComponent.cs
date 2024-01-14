using Logic.Core;

namespace Logic
{
    public interface ILongPress
    {
        void OnLongPress();
    }
    public class LongPressComponent : Logic.Core.Component
    {
        public static bool GlobalEnableFlag = true;

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
            LayMask = ELayerMask.Default;

            base.OnDispose();
        }

        public void OnClick()
        {
            if (!GlobalEnableFlag)
                return;

            if (!enableFlag)
                return;

            Sys_Interactive.Instance.eventEmitter.Trigger<InteractiveEvtData>(EInteractiveType.LongPress, new InteractiveEvtData()
            {
                eInteractiveAimType = InteractiveAimType,
                sceneActor = actor as ISceneActor,
                immediately = false,
            });
        }
    }
}
