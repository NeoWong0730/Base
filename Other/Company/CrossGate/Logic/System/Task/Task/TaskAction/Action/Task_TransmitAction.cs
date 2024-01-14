//using Table;
//using Logic.Core;

//namespace Logic
//{
//    public class Task_TransmitAction : ActionBase
//    {
//        public const string TypeName = "Logic.Task_TransmitAction";
//        public TaskGoal taskGoal;

//        protected override void OnDispose()
//        {
//            taskGoal = null;

//            base.OnDispose();
//        }

//        protected override void OnAutoExecute()
//        {
//            // 请求传送到特定地图的特定地点
//            // Sys_Map.Instance.ReqChangeMap(taskGoal, uiFlag, interactiveChangeMapData.gridX, interactiveChangeMapData.gridY);
//        }

//        public override bool IsCompleted()
//        {
//            return IsInArea();
//        }

//        private bool IsInArea()
//        {
//            return true;
//        }
//    }
//}
