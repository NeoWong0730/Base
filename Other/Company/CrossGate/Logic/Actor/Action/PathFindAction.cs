using Lib.Core;
using Logic.Core;
using UnityEngine;
using UnityEngine.AI;

namespace Logic {
    /// <summary>
    /// 寻路行为///
    /// </summary>
    public class PathFindAction : ActionBase {
        //public const string TypeName = "Logic.PathFindAction";

        public Vector3 targetPos {
            get;
            set;
        }

        public float _tolerance = 0.5f;
        public float tolerance {
            get { return this._tolerance; }
            set {
                this._tolerance = value;
                // 强制一个阈值
                if (this._tolerance < 0.1f) {
                    this._tolerance = 0.1f;
                }
            }
        }

        private NavMeshHit navMeshHit;
        private Vector2 targetPanelPos;

        protected override void OnDispose() {
            this.targetPos = Vector3.zero;
            this.tolerance = 0.5f;
            //navMeshHit = default;
            this.targetPanelPos = Vector2.zero;

            base.OnDispose();
        }

        private new void ProcessEvents(bool toRegister) {
            Sys_PathFind.Instance.eventEmitter.Handle<bool>(Sys_PathFind.EEvents.OnPathFind, this.OnPathFind, toRegister);

            if (toRegister) {
                Sys_Input.Instance.onClickOrTouch += this.OnClickOrTouch;
                Sys_Input.Instance.onLeftJoystick += this.OnLeftJoystick;
                Sys_Input.Instance.onRightJoystick += this.OnRightJoystick;
            }
            else {
                Sys_Input.Instance.onClickOrTouch -= this.OnClickOrTouch;
                Sys_Input.Instance.onLeftJoystick -= this.OnLeftJoystick;
                Sys_Input.Instance.onRightJoystick -= this.OnRightJoystick;
            }
        }
        // 点击地面
        private void OnClickOrTouch(bool down) {
            if (GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Normal) {
                this.OnInterrupt();
            }
        }
        // 控制摇杆
        private void OnLeftJoystick(UnityEngine.Vector2 v2, float f) {
            if (GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Normal)
                this.OnInterrupt();
        }
        private void OnRightJoystick(UnityEngine.Vector2 v2, float f) {
            if (GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Normal)
                this.OnInterrupt();
        }
        private void OnPathFind(bool isMoving) {
            if (!isMoving) {
                this.OnInterrupt();
            }
            else {
                if (Sys_Team.Instance.canManualOperate) {
                    UIManager.OpenUI(Sys_PathFind.Instance.PathFindId, true, null, EUIID.UI_MainInterface);
                }
            }
        }

        protected override void OnExecute() {
            MovementComponent.GetNavMeshHit(this.targetPos, out this.navMeshHit);
            Vector3 hittedTargetPos = this.navMeshHit.position;
            this.targetPanelPos = new Vector2(hittedTargetPos.x, hittedTargetPos.z);

            if (this.navMeshHit.hit && !this.IsCompleted()) {
                GameCenter.mainHero.movementComponent?.MoveTo(this.targetPos, null, this.actionInterrupt, null, 0.1f);
            }
        }

        protected override void OnAutoExecute() {
            MovementComponent.GetNavMeshHit(this.targetPos, out this.navMeshHit);
            Vector3 hittedTargetPos = this.navMeshHit.position;
            this.targetPanelPos = new Vector2(hittedTargetPos.x, hittedTargetPos.z);

            DebugUtil.LogFormat(ELogType.eTask, "targetPos {0}, targetPanelPos {1}", this.targetPos, this.targetPanelPos);

            // DebugUtil.LogFormat(ELogType.eTask, "navMeshHit.hit: {0} targetPos: {1}", navMeshHit.hit, targetPos);
            if (!this.navMeshHit.hit && this.IsCompleted()) { return; }
            GameCenter.mainHero.movementComponent?.MoveTo(this.targetPos, null, this.actionInterrupt, null, 0.1f);

            DebugUtil.LogFormat(ELogType.eTask, "targetPos {0}, targetPanelPos {1}", this.targetPos, this.targetPanelPos);

            this.ProcessEvents(false);
            this.ProcessEvents(true);

            UIManager.OpenUI(Sys_PathFind.Instance.PathFindId, true, null, EUIID.UI_MainInterface);
            Sys_PathFind.Instance.eventEmitter.Trigger<bool>(Sys_PathFind.EEvents.OnPathFind, true);
        }

        protected override void OnInterrupt() {
            UIManager.CloseUI(Sys_PathFind.Instance.PathFindId);
        }

        protected override void OnCompleted() {
            UIManager.CloseUI(Sys_PathFind.Instance.PathFindId);
        }

        public override bool IsCompleted() {
            if (this.navMeshHit.hit) {
                Vector2 heroPos = new Vector2(GameCenter.mainHero.transform.position.x, GameCenter.mainHero.transform.position.z);
                float distance = Mathf.Abs(Vector2.Distance(heroPos, this.targetPanelPos));
                if (distance < this.tolerance) {
                    return true;
                }
            }
            else {
#if UNITY_EDITOR
                Debug.LogErrorFormat("寻路终点不合理 {0} {1}", this.targetPos.ToString(), targetPanelPos.ToString());
#endif
                return true;
            }

            return false;
        }

        public override string ToString() {
            return base.ToString() + this.GetHashCode().ToString();
        }
    }
}
