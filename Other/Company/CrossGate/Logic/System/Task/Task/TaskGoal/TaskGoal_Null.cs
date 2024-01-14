namespace Logic {
    /// 空对象模式
    public class TaskGoal_Null : TaskGoal {
        public TaskGoal_Null() {
            this.MakeProgressFull();
        }

        public override void Refresh(uint crt) {
        }

        protected override void DoExec(bool auto = true) {
        }
        private void MakeProgressFull() {
            this.limit = 1;
            this.current = 1;
        }
    }
}