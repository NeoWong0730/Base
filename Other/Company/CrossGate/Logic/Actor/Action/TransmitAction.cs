using Logic.Core;
using UnityEngine.AI;

namespace Logic
{
    /// <summary>
    /// 传送行为///
    /// </summary>
    public class TransmitAction : ActionBase
    {
        //public const string TypeName = "Logic.TransmitAction";

        public bool uiFlag = false;

        //public InteractiveChangeMapData interactiveChangeMapData
        //{
        //    get;
        //    set;
        //}

        public bool enter;
        public uint portID;
        public uint gridX;
        public uint gridY;

        protected override void OnDispose()
        {            
            uiFlag = false;

            base.OnDispose();
        }

        protected override void OnExecute()
        {
            if (Sys_SurvivalPvp.Instance.isSurvivalPvpMap(Sys_Map.Instance.CurMapId))
            {
                Sys_SurvivalPvp.Instance.OpenTipsDialog(ExecuteChangeMap);
                return;
            }
            ExecuteChangeMap();
        }

       
        private void ExecuteChangeMap()
        {
            Sys_Map.Instance.ReqChangeMap(portID, uiFlag, gridX, gridY);
        }
        public override bool IsCompleted()
        {
            if (uiFlag)
            {
                if (Sys_Map.Instance.CurMapId == 10001 && IsOnNavMesh() && !UIManager.IsOpen(EUIID.UI_Loading))
                {
                    return true;
                }
            }
            else
            {
                if ((Sys_Map.Instance.CurMapId == portID) && IsOnNavMesh() && !UIManager.IsOpen(EUIID.UI_Loading))
                {
                    return true;
                }
            }
            return false;
        }

        bool IsOnNavMesh()
        {
            NavMeshAgent navmesh = GameCenter.mainHero.gameObject.GetComponent<NavMeshAgent>();
            if (navmesh != null)
            {
                return navmesh.isOnNavMesh;
            }
            return false;
        }
    }
}
