//using Logic.Core;
//using Lib.Core;
//using Table;
//using UnityEngine;

//namespace Logic
//{
//    public class NPCBubbleComponent : Logic.Core.Component
//    {
//        public VisualComponent VisualComponent
//        {
//            get;
//            private set;
//        }

//        public CSVNpc.Data CSVNpcData
//        {
//            get;
//            set;
//        }

//        bool CDFlag = true;
//        Timer cdTimer;

//        protected override void OnConstruct()
//        {
//            base.OnConstruct();

//            Npc npc = actor as Npc;
//            if (npc != null)
//            {
//                VisualComponent = World.GetComponent<VisualComponent>(npc);
//            }
//            CDFlag = true;
//        }

//        protected override void OnDispose()
//        {
//            VisualComponent = null;
//            CDFlag = true;
//            cdTimer?.Cancel();
//            cdTimer = null;
//            CSVNpc.Data = null;

//            base.OnDispose();
//        }

//        public void ActiveBubble()
//        {
//            if (GameMain.Procedure.CurrentProcedure.ProcedureType != ProcedureManager.EProcedureType.Normal)
//                return;

//            if (CDFlag)
//            {
//                CDFlag = false;

//                if (VisualComponent != null && VisualComponent.Visiable)
//                {
//                    CreateBubble();
//                }

//                float time = 5f;
//                if (CSVNpcData.NPCBubbleInteral != null && CSVNpcData.NPCBubbleInteral.Count >= 2)
//                {
//                    time = Random.Range(CSVNpcData.NPCBubbleInteral[0], CSVNpcData.NPCBubbleInteral[1]) / 1000f;
//                }

//                cdTimer?.Cancel();
//                cdTimer = Timer.Register(time, () => 
//                {
//                    CDFlag = true;
//                }, null, false, true);
//            }
//        }

//        void CreateBubble()
//        {
//            if (CSVNpcData.NpcBubbleIDs == null || CSVNpcData.NpcBubbleIDs.Count == 0)
//                return;

//            int count = CSVNpcData.NpcBubbleIDs.Count;
//            int index = UnityEngine.Random.Range(0, count);
//            TriggerNpcBubbleEvt triggerNpcBubbleEvt = new TriggerNpcBubbleEvt();
//            triggerNpcBubbleEvt.npcid = actor.uID;
//            triggerNpcBubbleEvt.npcInfoId = CSVNpcData.id;
//            triggerNpcBubbleEvt.bubbleid = CSVNpcData.NpcBubbleIDs[index];
//            triggerNpcBubbleEvt.ownerType = 0;
//            triggerNpcBubbleEvt.npcobj = (actor as SceneActor).gameObject;
//            Sys_HUD.Instance.eventEmitter.Trigger<TriggerNpcBubbleEvt>(Sys_HUD.EEvents.OnTriggerNpcBubble, triggerNpcBubbleEvt);
//        }
//    }
//}
