using Logic.Core;
using UnityEngine;

namespace Logic
{
#if false
    public class NPCActionListenerComponent : Logic.Core.Component, IUpdateCmd
    {
        public Npc Npc
        {
            get;
            private set;
        }
        bool actionlockFlag = true;
        //float triggerPerformRange;
        //float distance;

        protected override void OnConstruct()
        {
            base.OnConstruct();

            Npc = actor as Npc;
            //triggerPerformRange = Npc.cSVNpcData.TriggerPerformRange / 10000f;
            //triggerPerformRange *= triggerPerformRange;
            actionlockFlag = true;
        }

        protected override void OnDispose()
        {
            Npc = null;
            actionlockFlag = true;
            //triggerPerformRange = 0f;
            //distance = 0f;

            base.OnDispose();
        }

        public void Update()
        {
            if (Npc == null)
                return;

            if (CanNpcAction())
            {
                if (!actionlockFlag)
                    return;

                actionlockFlag = false;

                if (Npc.cSVNpcData.behaviorid != 0)
                {
                    Npc.wS_NPCManagerEntity = WS_NPCManagerEntity.StartNPC<WS_NPCControllerEntity>(Npc.cSVNpcData.behaviorid, 0, SwitchWorkStreamEnum.Stop_AllWorkStream, null, OnControllerOverAction
                        , true, (int)NPCEnum.B_EnterTrigger, Npc.uID);                    
                }
                else
                {
                    Npc.wS_NPCManagerEntity = WS_NPCManagerEntity.StartNPC<WS_NPCControllerEntity>(1, 0, SwitchWorkStreamEnum.Stop_AllWorkStream, null, OnControllerOverAction
                        , true, (int)NPCEnum.B_EnterTrigger, Npc.uID);
                }
            }
        }

        bool CanNpcAction()
        {
            if (GameCenter.mainHero == null)
                return false;

            float triggerPerformRange = Npc.cSVNpcData.TriggerPerformRange / 10000f;
            float distance = Npc.DistanceTo(GameCenter.mainHero.transform);
            if (distance < triggerPerformRange)
            {
                return true;
            }

            return false;
        }

        void OnControllerOverAction()
        {
            actionlockFlag = true;
        }
    }
#else
    public class NPCActionListenerComponent : Logic.Core.Component
    {
        public bool actionlockFlag = true;
        protected override void OnDispose()
        {
            actionlockFlag = true;
        }

        public void OnControllerOverAction()
        {
            actionlockFlag = true;
        }
    }
#endif
}
