using UnityEngine;
using System.Collections.Generic;
using Table;
using Logic.Core;

namespace Logic
{
    [InteractiveWatcher(EInteractiveAimType.NPCFunction)]
    public class InteractiveWatcher_NPCFunction : IInteractiveWatcher
    {
        public void OnUIButtonExecute(InteractiveEvtData data)
        {
            if (GameMain.Procedure.CurrentProcedure.ProcedureType != ProcedureManager.EProcedureType.Normal)
                return;

            Npc npc;
            GameCenter.FindNearestNPCInAreaWithFunction(CSVExploringSkill.Instance.GetConfData(100).range / 10000f, (EFunctionType)data.data, out npc);
            if (npc != null)
            {
                if (!IsValid(npc))
                    return;

                PathFindAction pathFindAction = ActionCtrl.Instance.CreateAction(typeof(PathFindAction)) as PathFindAction;
                if (pathFindAction != null)
                {
                    if (npc.cSVNpcData.dialogueParameter == null)
                    {
                        Debug.LogError("NPC表中dialogueParameter字段没填！！！");
                    }
                    pathFindAction.targetPos = npc.gameObject.transform.position + new Vector3(npc.cSVNpcData.dialogueParameter[0] / 10000f, npc.cSVNpcData.dialogueParameter[1] / 10000f, npc.cSVNpcData.dialogueParameter[2] / 10000f);
                    pathFindAction.tolerance = npc.cSVNpcData.InteractiveRange / 10000f;
                    pathFindAction.Init(null, () =>
                    {
                        CreateInteractiveWithNPCAction(npc);
                    });
                    ActionCtrl.Instance.ExecutePlayerCtrlAction(pathFindAction);
                }
            }
        }

        public void OnAreaCheckExecute(InteractiveEvtData data)
        {
            if (GameMain.Procedure.CurrentProcedure.ProcedureType != ProcedureManager.EProcedureType.Normal)
                return;

            Npc npc = data.sceneActor as Npc;
            if (npc == null)
                return;

            if (!IsValid(npc))
                return;

            CreateInteractiveWithNPCAction(npc);
        }

        public void OnClickExecute(InteractiveEvtData data)
        {
            if (GameMain.Procedure.CurrentProcedure.ProcedureType != ProcedureManager.EProcedureType.Normal)
                return;

            Npc npc = data.sceneActor as Npc;
            if (npc == null)
                return;

            if (!IsValid(npc))
                return;

            if (data.immediately)
            {
                CreateInteractiveWithNPCAction(npc);
            }
            else
            {
                ulong cacheUID = npc.uID;
                ///如果是采集物
                if (npc.cSVNpcData.type == (uint)ENPCType.Collection)
                {
                    float distance = Vector3.Distance(GameCenter.mainHero.transform.position, npc.gameObject.transform.position);
                    if (distance < npc.cSVNpcData.InteractiveRange / 10000f)
                    {
                        CreateInteractiveWithNPCAction(npc);
                    }
                    else
                    {
                        PathFindAction pathFindAction = ActionCtrl.Instance.CreateAction(typeof(PathFindAction)) as PathFindAction;
                        if (pathFindAction != null)
                        {
                            pathFindAction.targetPos = npc.gameObject.transform.position + new Vector3(npc.cSVNpcData.dialogueParameter[0] / 10000f, npc.cSVNpcData.dialogueParameter[1] / 10000f, npc.cSVNpcData.dialogueParameter[2] / 10000f);
                            pathFindAction.tolerance = npc.cSVNpcData.InteractiveRange / 10000f;
                            pathFindAction.Init(null, () =>
                            {
                                if (cacheUID == npc.uID)
                                    CreateInteractiveWithNPCAction(npc);
                            });
                            ActionCtrl.Instance.ExecutePlayerCtrlAction(pathFindAction);
                        }
                    }
                }
                else
                {
                    PathFindAction pathFindAction = ActionCtrl.Instance.CreateAction(typeof(PathFindAction)) as PathFindAction;
                    if (pathFindAction != null)
                    {
                        if (npc.cSVNpcData.dialogueParameter == null)
                        {
                            Debug.LogError("NPC表中dialogueParameter字段没填！！！");
                        }
                        pathFindAction.targetPos = npc.gameObject.transform.position + new Vector3(npc.cSVNpcData.dialogueParameter[0] / 10000f, npc.cSVNpcData.dialogueParameter[1] / 10000f, npc.cSVNpcData.dialogueParameter[2] / 10000f);
                        pathFindAction.tolerance = npc.cSVNpcData.InteractiveRange /10000f;
                        pathFindAction.Init(null, () =>
                        {
                            CreateInteractiveWithNPCAction(npc);
                        });
                        ActionCtrl.Instance.ExecutePlayerCtrlAction(pathFindAction);
                    }
                }         
            }        
        }

        bool IsValid(Npc npc)
        {           
            if (npc.cSVNpcData.type == (uint)ENPCType.Collection)
            {
                CSVCollection.Data cSVCollectionData = CSVCollection.Instance.GetConfData(npc.cSVNpcData.CollectionID);
                if (cSVCollectionData == null)
                    return false;

                if (cSVCollectionData.collectionTask != 0)
                {
                    ETaskState eTaskState = Sys_Task.Instance.GetTaskState(cSVCollectionData.collectionTask);
                    if (eTaskState != ETaskState.UnFinished)
                    {
                        return false;
                    }
                }
            }

            Sys_Interactive.CurInteractiveNPC = npc;
            return true;
        }

        void CreateInteractiveWithNPCAction(Npc npc)
        {
            if (ActionCtrl.Instance.actionCtrlStatus != ActionCtrl.EActionCtrlStatus.Auto)
            {
                InteractiveWithNPCAction interactiveWithNPCAction = ActionCtrl.Instance.CreateAction(typeof(InteractiveWithNPCAction)) as InteractiveWithNPCAction;
                if (interactiveWithNPCAction != null)
                {
                    interactiveWithNPCAction.npc = npc;
                    interactiveWithNPCAction.Init();
                    ActionCtrl.Instance.ExecutePlayerCtrlAction(interactiveWithNPCAction);
                }
            }
            else
            {
                List<ActionBase> actionBases = new List<ActionBase>();
                InteractiveWithNPCAction interactiveWithNPCAction = ActionCtrl.Instance.CreateAction(typeof(InteractiveWithNPCAction)) as InteractiveWithNPCAction;
                if (interactiveWithNPCAction != null)
                {
                    interactiveWithNPCAction.npc = npc;
                    interactiveWithNPCAction.Init();
                    actionBases.Add(interactiveWithNPCAction);
                }
                ActionCtrl.Instance.AddAutoActions(actionBases);
            }
        }

        public void OnDoubleClickExecute(InteractiveEvtData data)
        {

        }

        public void OnLongPressExecute(InteractiveEvtData data)
        {

        }

        public void OnDistanceCheckExecute(InteractiveEvtData data)
        {

        }
    }
}
