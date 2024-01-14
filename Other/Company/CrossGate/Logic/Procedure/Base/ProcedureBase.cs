namespace Logic
{
    /// <summary>
    /// 流程基类///
    /// </summary>
    public abstract class ProcedureBase : FsmState
    {
        public abstract ProcedureManager.EProcedureType ProcedureType
        {
            get;
        }

        /// <summary>
        /// 状态初始化时调用///
        /// </summary>
        /// <param name="procedureOwner">流程持有者</param>
        protected internal override void OnInit(IFsm procedureOwner)
        {
            base.OnInit(procedureOwner);
        }

        /// <summary>
        /// 进入状态时调用///
        /// </summary>
        /// <param name="procedureOwner">流程持有者</param>
        protected internal override void OnEnter(IFsm procedureOwner)
        {
            base.OnEnter(procedureOwner);
        }

        /// <summary>
        /// 状态轮询时调用///
        /// </summary>
        /// <param name="procedureOwner">流程持有者</param>
        protected internal override void OnUpdate(IFsm procedureOwner)
        {
            base.OnUpdate(procedureOwner);
        }

        /// <summary>
        /// 离开状态时调用///
        /// </summary>
        /// <param name="procedureOwner">流程持有者</param>
        /// <param name="isShutdown">是否是关闭状态机时触发</param>
        protected internal override void OnExit(IFsm procedureOwner, bool isShutdown)
        {
            base.OnExit(procedureOwner, isShutdown);
        }

        /// <summary>
        /// 状态销毁时调用///
        /// </summary>
        /// <param name="procedureOwner">流程持有者</param>
        protected internal override void OnDestroy(IFsm procedureOwner)
        {
            base.OnDestroy(procedureOwner);
        }
    }
}
