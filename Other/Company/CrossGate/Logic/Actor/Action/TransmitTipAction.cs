namespace Logic
{
    /// <summary>
    /// 传送行为///
    /// </summary>
    public class TransmitTipAction : ActionBase
    {
        //public const string TypeName = "Logic.TransmitTipAction";

        public uint TransNpcId
        {
            get;
            set;
        }

        protected override void ProcessEvents(bool toRegister)
        {
            base.ProcessEvents(toRegister);

            if (GameCenter.mainHero != null && GameCenter.mainHero.stateComponent != null)
            {
                if (toRegister)
                {
                    GameCenter.mainHero.stateComponent.StateChange += OnStateChange;
                }
                else
                {
                    GameCenter.mainHero.stateComponent.StateChange -= OnStateChange;
                }
            }
        }

        protected override void OnDispose()
        {
            TransNpcId = 0;

            base.OnDispose();
        }

        protected override void OnExecute()
        {
            Sys_Map.Instance.eventEmitter.Trigger(Sys_Map.EEvents.OnTransTipStart, TransNpcId);
        }

        protected override void OnInterrupt()
        {
            Sys_Map.Instance.IsTransTipOver = false;
            Sys_Map.Instance.eventEmitter.Trigger(Sys_Map.EEvents.OnTransTipIntterupt);
            //Sys_Map.Instance.IsTransTipOver = false;
        }

        public override bool IsCompleted()
        {
            return Sys_Map.Instance.IsTransTipOver;
        }

        private void OnStateChange(EStateType oldState, EStateType newState)
        {
            if (newState == EStateType.Run)
            {
                Interrupt();
            }
        }
    }
}
