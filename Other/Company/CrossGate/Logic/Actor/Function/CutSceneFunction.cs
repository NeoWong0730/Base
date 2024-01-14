namespace Logic {
    /// <summary>
    /// 过场动画功能///
    /// </summary>
    public class CutSceneFunction : FunctionBase
    {
        protected override void OnExecute() {
            base.OnExecute();

            Sys_CutScene.Instance.TryDoCutScene(this.ID, this.OnRealEndAction);
        }

        private void OnRealEndAction(uint seriesCutSceneId, uint cutsceneId1) {
            if (EFunctionSourceType.Task == this.FunctionSourceType) {
                Sys_Task.Instance.ReqStepGoalFinish(this.HandlerID, this.HandlerIndex, 0, 0);
            }
        }
    }
}