using Logic.Core;
using Table;

namespace Logic
{
#if false
    /// <summary>
    /// NPC范围检测组件///
    /// </summary>
    public class NPCAreaCheckComponent : SceneActorDistanceCheckComponent
    {
        public Npc Npc
        {
            get;
            private set;
        }

        uint triggerNum;

        protected override void OnConstruct()
        {
            base.OnConstruct();

            Npc = actor as Npc;
        }

        protected override void OnDispose()
        {
            Npc = null;
            triggerNum = 0;

            base.OnDispose();
        }

        protected override bool VaildCheck()
        {
            bool result = base.VaildCheck();

            if (Npc.VisualComponent != null)
            {
                if (!Npc.VisualComponent.Visiable)
                {
                    result = false;
                }
            }

            return result;
        }
       
        protected override bool CheckResult()
        {
            if(Npc.NPCFunctionComponent.HasActiveFunction(EFunctionType.Inquiry))
            {
                if (!Npc.NPCFunctionComponent.HasActiveFunction(EFunctionType.Inquiry))
                    return false;

                float areaDistance = CSVExploringSkill.Instance.GetConfData(100).range / 10000f;
                float distance = (GameCenter.mainHero.transform.position - Npc.transform.position).sqrMagnitude;

                if (distance > (areaDistance * areaDistance))
                    return false;

                return true;
            }
            else
            {
                return Npc.Contains(GameCenter.mainHero.transform);
            }            
        }

        protected override void Trigger()
        {
            if (Npc.NPCFunctionComponent.HasActiveFunction(EFunctionType.Inquiry))
            {
                Sys_Inquiry.Instance.EnterArea(Npc.uID);
            }
            else
            {
                if (Npc.cSVNpcData.NpcTriggerFrequency != 0 && triggerNum >= Npc.cSVNpcData.NpcTriggerFrequency)
                    return;

                Sys_Interactive.Instance.eventEmitter.Trigger<InteractiveEvtData>(EInteractiveType.AreaCheck, new InteractiveEvtData()
                {
                    eInteractiveAimType = EInteractiveAimType.NPCFunction,
                    sceneActor = actor as ISceneActor,
                    immediately = true,
                });

                triggerNum++;
            }                
        }

        protected override void TriggerExit()
        {
            if (Npc.NPCFunctionComponent.HasActiveFunction(EFunctionType.Inquiry))
            {
                Sys_Inquiry.Instance.ExitArea(Npc.uID);
            }
        }
    }
#endif
}
