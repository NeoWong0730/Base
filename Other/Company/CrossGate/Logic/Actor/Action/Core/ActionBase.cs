using System;

namespace Logic
{
    /// <summary>///
    /// 行为基类
    /// </summary>
    public abstract class ActionBase
    {
        public uint taskId;

        /// <summary>
        /// 行为开始///
        /// </summary>
        public Action actionStart;

        /// <summary>
        /// 行为完成///
        /// </summary>
        public Action actionCompleted;

        /// <summary>
        /// 行为中断///
        /// </summary>
        public Action actionInterrupt;

        /// <summary>
        /// 初始化行为///
        /// </summary>
        /// <param name="_actionStart">行为开始回调</param>
        /// <param name="_actionCompleted">行为完成回调</param>
        /// <param name="_actionInterrupt">行为中断回调</param>
        public virtual void Init(Action _actionStart = null, Action _actionCompleted = null, Action _actionInterrupt = null)
        {
            actionStart = _actionStart;
            actionCompleted = _actionCompleted;
            actionInterrupt = _actionInterrupt;
            ProcessEvents(true);
        }

        /// <summary>
        /// 注册会影响行为状态的外部事件///
        /// </summary>
        /// <param name="toRegister"></param>
        protected virtual void ProcessEvents(bool toRegister)
        {

        }

        public void Dispose()
        {
            OnDispose();
            ProcessEvents(false);

            PoolManager.Recycle(this);
            //Logic.Core.ObjectPool<ActionBase>.Recycle(this);
        }

        protected virtual void OnDispose()
        {
            actionStart = null;
            actionCompleted = null;
            actionInterrupt = null;
            taskId = 0;
        }

        /// <summary>
        /// 手动模式执行///
        /// </summary>
        public void Execute()
        {
            actionStart?.Invoke();          
            OnExecute();
        }

        /// <summary>
        /// 自动模式执行///
        /// </summary>
        public void AutoExecute()
        {
            actionStart?.Invoke();            
            OnAutoExecute();
        }

        protected virtual void OnExecute() { }  

        protected virtual void OnAutoExecute()
        {
            OnExecute();
        }

        public abstract bool IsCompleted();

        public void Completed()
        {
            if (ActionCtrl.Instance.actionCtrlStatus == ActionCtrl.EActionCtrlStatus.Auto)
            {
                ActionCtrl.Instance.currentAutoAction = null;
            }
            else
            {
                ActionCtrl.Instance.currentPlayerCtrlAction = null;
            }

            ActionCtrl.Instance.actionStatus = ActionCtrl.EActionStatus.Idle;
            actionCompleted?.Invoke();          
            OnCompleted();

            Dispose();
        }

        protected virtual void OnCompleted()
        {                     
        }

        public void Interrupt()
        {
            //if (ActionCtrl.Instance.actionCtrlStatus == ActionCtrl.EActionCtrlStatus.Auto)
            //{
            //    ActionCtrl.Instance.currentAutoAction = null;
            //}
            //else
            //{
            //    ActionCtrl.Instance.currentPlayerCtrlAction = null;
            //}

            ActionCtrl.Instance.currentAutoAction = null;
            ActionCtrl.Instance.currentPlayerCtrlAction = null;
            ActionCtrl.Instance.actionStatus = ActionCtrl.EActionStatus.Idle;
            actionInterrupt?.Invoke();            
            OnInterrupt();

            Dispose();
        }

        protected virtual void OnInterrupt()
        {            
        }       
    }
}
