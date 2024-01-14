using Lib.Core;
using Logic.Core;
using Table;

namespace Logic
{
#if false
    public class UploadTransformComponent : Logic.Core.Component
        //, IUpdateCmd
    {
        private Timer m_SynTimer;

        //private bool m_Send = false;
        //private StateComponent m_StateCom = null;

        public bool CanSendFlag
        {
            get;
            set;
        } = true;

        private bool m_bNetUpdate = true;
        public bool NetUpdate {
            get { return m_bNetUpdate; }
            set {
                m_bNetUpdate = value;
                //if (!m_bNetUpdate)
                //    CancleNetUpdateMove();
            }
        }

        private bool m_TeamNetUpdate = false;

        public bool TeamNetUpdate { get { return m_TeamNetUpdate; } set { m_TeamNetUpdate = value; } }

        private bool m_EnableMainHero = true;

        protected override void OnConstruct()
        {
            base.OnConstruct();

            //m_StateCom = World.GetComponent<StateComponent>(actor);
            CanSendFlag = true;
            m_bNetUpdate = true;
            m_TeamNetUpdate = false;
            m_SynTimer = Timer.Register(0.5f, OnReqMove, null, true);
        }

        //private void CancleNetUpdateMove()
        //{
        //    if (m_SynTimer != null && m_SynTimer.isCancelled == false)
        //        m_SynTimer.Cancel();
        //    //m_Send = false;
        //}
        protected override void OnDispose()
        {
            m_SynTimer?.Cancel();
            m_SynTimer = null;

            //m_StateCom = null;

            //NetUpdate = true;

            //m_Send = false;

            CanSendFlag = true;
            m_bNetUpdate = true;
            m_TeamNetUpdate = false;

            base.OnDispose();
        }

        //public void Update()
        //{
        //    //DebugUtil.LogWarningFormat("UpLoad===-2");

        //    if (GameMain.Procedure == null 
        //        || GameMain.Procedure.CurrentProcedure == null 
        //        || GameMain.Procedure.CurrentProcedure.ProcedureType != ProcedureManager.EProcedureType.Normal)
        //        return;

        //    //DebugUtil.LogWarningFormat("UpLoad===-1");

        //    //if (!CanSendFlag)
        //    //    return;

        //    //DebugUtil.LogWarningFormat("UpLoad===0");

        //    switch (m_StateCom.CurrentState)
        //    {
        //        case EStateType.Run:
        //            if (!m_Send && NetUpdate)
        //            {
        //                m_SynTimer = Timer.Register(0.5f, OnReqMove, null, true);
        //                OnReqMove();
        //                m_Send = true;
        //            }
        //            break;
        //        case EStateType.Idle:
        //            if (m_Send)
        //            {
        //                m_Send = false;

        //                m_SynTimer?.Cancel();
        //                OnReqMove();
        //            }
        //            break;
        //        case EStateType.Collection:
        //            if (m_Send)
        //            {
        //                m_Send = false;
        //                m_SynTimer?.Cancel();

        //                //Sys_Map.Instance.ReqMove(TeamNetUpdate);
        //            }
        //            break;
        //    }       
        //}

        private void OnReqMove()
        {
            if (GameMain.Procedure == null
             || GameMain.Procedure.CurrentProcedure == null
             || GameMain.Procedure.CurrentProcedure.ProcedureType != ProcedureManager.EProcedureType.Normal)
                return;

            if (!m_bNetUpdate)
                return;

            if (!m_EnableMainHero)
                return;

            if (!CanSendFlag)
                return;

            //DebugUtil.LogWarningFormat("UpLoad===1");
            Sys_Map.Instance.ReqMove(TeamNetUpdate);
        }

        public void OnEnableMainHero(bool enable)
        {
            m_EnableMainHero = enable;
        }
    }
#endif
}


