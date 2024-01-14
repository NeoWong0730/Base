using Logic.Core;
using System.Collections.Generic;
using Lib.Core;
using Framework;
using System;
using Table;
using UnityEngine;
using DG.Tweening;
using Net;
using Packet;

namespace Logic
{
    public partial class Sys_Interactive : SystemModuleBase<Sys_Interactive>
    {
        public uint CurPreAnimID
        {
            get;
            set;
        }

        public Dictionary<ulong, VirtualSceneActor> interactiveVirtualActors = new Dictionary<ulong, VirtualSceneActor>();

        public void ClearInteractiveVirtualActors()
        {
            foreach (var actor in interactiveVirtualActors.Values)
            {
                //GameCenter.mainWorld.DestroyActor(actor);
                World.CollecActor(actor);
            }
            interactiveVirtualActors.Clear();
        }

        public void OverActing(Action callback)
        {
            ActionCtrl.ActionExecuteLockFlag = true;

            if (CurPreAnimID != 0)
            {
                WS_NPCManagerEntity.StartNPC<WS_NPCControllerEntity>(CurPreAnimID, 0, SwitchWorkStreamEnum.Stop_AllWorkStream, null, () =>
                {
                    ActionCtrl.ActionExecuteLockFlag = false;
                    if (GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Interactive)
                    {
                        GameMain.Procedure.TriggerEvent(this, (int)EProcedureEvent.EnterNormal);
                    }
                    callback?.Invoke();
                    UIManager.OpenUI(EUIID.UI_BlockClickTime, true, 1.2f);
                }, true, (int)NPCEnum.B_EndInteractive);

                CurPreAnimID = 0;
            }
            else
            {
                ActionCtrl.ActionExecuteLockFlag = false;
                GameMain.Procedure.TriggerEvent(this, (int)EProcedureEvent.EnterNormal);
                callback?.Invoke();
                UIManager.OpenUI(EUIID.UI_BlockClickTime, true, 1.2f);
            }
        }
    }

    /// <summary>
    /// NPC交互相关///
    /// </summary>
    public partial class Sys_Interactive : SystemModuleBase<Sys_Interactive>
    {
        /// <summary>
        /// 当前交互NPC///
        /// </summary>
        public static Npc CurInteractiveNPC
        {
            get;
            set;
        }

        #region PlayerCtrlInteractive

        /// <summary>
        /// 手动交互///
        /// </summary>
        /// <param name="cSVNpcData"></param>
        /// <param name="functions"></param>
        public void InteractiveProcess(CSVNpc.Data cSVNpcData, List<FunctionBase> functions)
        {
            if (GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Fight)
                return;

            AudioUtil.PlayDubbing(cSVNpcData.InteractiveVoice, AudioUtil.EAudioType.NPCSound, false);

            if (functions.Count == 0)
            {
                InteractiveProcessZeroFunction(cSVNpcData);
            }
            else if (functions.Count == 1)
            {
                InteractiveProcessOneFunction(cSVNpcData, functions[0], FunctionBase.ECtrlType.PlayCtrl);
            }
            else
            {
                InteractiveProcessMultiFunction(cSVNpcData);
            }
        }

        /// <summary>
        /// 手动NPC身上没功能///
        /// </summary>
        /// <param name="cSVNpcData"></param>
        void InteractiveProcessZeroFunction(CSVNpc.Data cSVNpcData)
        {
            if (cSVNpcData.type == (uint)ENPCType.Common)
            {
                if (CurInteractiveNPC != null && CurInteractiveNPC.gameObject != null)
                {
                    Vector3 endRot;
                    Vector3 endPos;

                    if (CurInteractiveNPC.modelGameObject != null)
                    {
                        endRot = CurInteractiveNPC.gameObject.transform.eulerAngles + new Vector3(CurInteractiveNPC.cSVNpcData.dialogueEndParameter[3] / 10000f, CurInteractiveNPC.cSVNpcData.dialogueEndParameter[4] / 10000f, CurInteractiveNPC.cSVNpcData.dialogueEndParameter[5] / 10000f);
                        endPos = CurInteractiveNPC.gameObject.transform.position + new Vector3(CurInteractiveNPC.cSVNpcData.dialogueEndParameter[0] / 10000f, CurInteractiveNPC.cSVNpcData.dialogueEndParameter[1] / 10000f, CurInteractiveNPC.cSVNpcData.dialogueEndParameter[2] / 10000f);
                    }
                    else
                    {
                        Vector3 pos = Vector3.zero;
                        Quaternion eular = Quaternion.identity;
                        Sys_Map.Instance.GetNpcPos(Sys_Map.Instance.CurMapId, cSVNpcData.id, ref pos, ref eular);

                        endRot = eular.eulerAngles + new Vector3(CurInteractiveNPC.cSVNpcData.dialogueEndParameter[3] / 10000f, CurInteractiveNPC.cSVNpcData.dialogueEndParameter[4] / 10000f, CurInteractiveNPC.cSVNpcData.dialogueEndParameter[5] / 10000f);
                        endPos = pos + new Vector3(CurInteractiveNPC.cSVNpcData.dialogueEndParameter[0] / 10000f, CurInteractiveNPC.cSVNpcData.dialogueEndParameter[1] / 10000f, CurInteractiveNPC.cSVNpcData.dialogueEndParameter[2] / 10000f);
                    }

                    float distance = Vector3.Distance(GameCenter.mainHero.transform.position, endPos);
                    if (distance > 0.1f)
                    {
                        if (GameCenter.mainHero.Mount == null)
                        {
                            GameCenter.mainHero.transform.DOLookAt(endPos, 0.01f, AxisConstraint.Y, Vector3.up).onComplete = delegate ()
                            {
                                GameCenter.mainHero.animationComponent.Play((uint)EStateType.Run);
                                GameCenter.mainHero.transform.DOMove(endPos, distance / float.Parse(CSVParam.Instance.GetConfData(129).str_value)).SetEase(Ease.Linear).onComplete = delegate ()
                                {
                                    GameCenter.mainHero.animationComponent.Play((uint)EStateType.Idle);
                                    GameCenter.mainHero.transform.DORotate(endRot, 0.1f, RotateMode.Fast);
                                    ShowAcquiesceDialogue(cSVNpcData);
                                };
                            };
                        }
                        else
                        {
                            GameCenter.mainHero.transform.DOLookAt(endPos, 0.01f, AxisConstraint.Y, Vector3.up).onComplete = delegate ()
                            {
                                GameCenter.mainHero.Mount.animationComponent.Play((uint)EStateType.Sprint);
                                if (GameCenter.mainHero.Mount.csvPetData.action_id_mount == 0)
                                {                                    
                                    GameCenter.mainHero.animationComponent.Play((uint)EStateType.mount_1_run);
                                }
                                else
                                {
                                    GameCenter.mainHero.animationComponent.Play((uint)EStateType.mount_2_run);
                                }

                                GameCenter.mainHero.transform.DOMove(endPos, distance / float.Parse(CSVParam.Instance.GetConfData(129).str_value)).SetEase(Ease.Linear).onComplete = delegate ()
                                {
                                    GameCenter.mainHero.Mount.animationComponent.Play((uint)EStateType.Stand);
                                    if (GameCenter.mainHero.Mount.csvPetData.action_id_mount == 0)
                                    {
                                        GameCenter.mainHero.animationComponent.Play((uint)EStateType.mount_1_idle);
                                    }
                                    else
                                    {
                                        GameCenter.mainHero.animationComponent.Play((uint)EStateType.mount_2_idle);
                                    }
                                    GameCenter.mainHero.transform.DORotate(endRot, 0.1f, RotateMode.Fast);

                                    ShowAcquiesceDialogue(cSVNpcData);
                                };
                            };
                        }
                    }
                    else
                    {
                        if (GameCenter.mainHero.Mount == null)
                        {
                            GameCenter.mainHero.animationComponent.Play((uint)EStateType.Idle);
                        }
                        else
                        {
                            GameCenter.mainHero.Mount.animationComponent.Play((uint)EStateType.Stand);
                            if (GameCenter.mainHero.Mount.csvPetData.action_id_mount == 0)
                            {
                                GameCenter.mainHero.animationComponent.Play((uint)EStateType.mount_1_idle);
                            }
                            else
                            {
                                GameCenter.mainHero.animationComponent.Play((uint)EStateType.mount_2_idle);
                            }
                        }
                        GameCenter.mainHero.transform.DORotate(endRot, 0.1f, RotateMode.Fast);

                        ShowAcquiesceDialogue(cSVNpcData);
                    }
                }
            }
        }

        /// <summary>
        /// 手动NPC身上只有一个功能///
        /// </summary>
        /// <param name="cSVNpcData"></param>
        /// <param name="functionBase"></param>
        void InteractiveProcessOneFunction(CSVNpc.Data cSVNpcData, FunctionBase functionBase, FunctionBase.ECtrlType ctrlType)
        {
            ///激活的功能是对话选择或声望称号领取，不执行
            if (functionBase.Type == EFunctionType.DialogueChoose || functionBase.Type == EFunctionType.Prestige || functionBase.Type == EFunctionType.LearnActiveSkill || functionBase.Type == EFunctionType.LearnPassiveSkill)
            {
                ShowFunctionChoosePanel(cSVNpcData);
            }
            ///直接执行这个功能
            else
            {
                if (functionBase.OpenList == 1)
                {
                    ShowFunctionChoosePanel(cSVNpcData);
                }
                else
                {
                    InteractiveDisPlay(cSVNpcData, functionBase, ctrlType);
                }
            }
        }

        void InteractiveProcessMultiFunction(CSVNpc.Data cSVNpcData)
        {
            ShowFunctionChoosePanel(cSVNpcData);
        }

        #endregion

        #region AutoInteractive

        /// <summary>
        /// 自动交互过程///
        /// </summary>
        /// <param name="functions"></param>
        public void AutoInteractiveProcess(Npc npc, List<FunctionBase> functions, TaskEntry currentTaskEntry)
        {
            if (GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Fight)
                return;

            if (npc == null)
                return;

            AudioUtil.PlayDubbing(npc.cSVNpcData.InteractiveVoice, AudioUtil.EAudioType.NPCSound, false);

            CurInteractiveNPC = npc;
            ExecuteAutoInteractiveProcess(npc.cSVNpcData, currentTaskEntry, functions);
        }

        void ExecuteAutoInteractiveProcess(CSVNpc.Data cSVNpcData, TaskEntry currentTaskEntry, List<FunctionBase> functions)
        {
            if (currentTaskEntry != null && currentTaskEntry.csvTask != null)
            {
                if (currentTaskEntry.taskState == ETaskState.UnFinished)
                {
                    List<FunctionBase> dynamicCreateFunctions = new List<FunctionBase>();
                    foreach (FunctionBase function in functions)
                    {
                        if (function.HandlerID != 0)
                        {
                            dynamicCreateFunctions.Add(function);
                        }
                    }

                    foreach (FunctionBase function in dynamicCreateFunctions)
                    {
                        CSVTaskGoal.Data cSVTaskGoalData = CSVTaskGoal.Instance.GetConfData((uint)currentTaskEntry.csvTask.taskGoals[currentTaskEntry.currentTaskGoalIndex]);
                        if (function.ID == cSVTaskGoalData.FunctionParameter)
                        {
                            if (function.Type == EFunctionType.DialogueChoose)
                            {
                                if (!UIManager.IsOpen(EUIID.UI_NPC))
                                {
                                    InteractiveDisPlay(cSVNpcData, function, FunctionBase.ECtrlType.Auto);
                                    UIManager.OpenUI(EUIID.UI_NPC, false, new InteractiveData()
                                    {
                                        OnloadCallBack = () =>
                                        {
                                            UIManager.CloseUI(EUIID.UI_NPC, true, false);
                                            function.Execute(FunctionBase.ECtrlType.Auto);
                                        }
                                    });
                                }
                                else
                                {
                                    InteractiveDisPlay(cSVNpcData, function, FunctionBase.ECtrlType.Auto);
                                }
                                return;
                            }
                            else
                            {
                                if (function.OpenList == 0)
                                {
                                    if (function.Type == EFunctionType.LearnActiveSkill || function.Type == EFunctionType.LearnPassiveSkill)
                                    {
                                        ShowFunctionChoosePanel(cSVNpcData);
                                    }
                                    else
                                    {
                                        InteractiveDisPlay(cSVNpcData, function, FunctionBase.ECtrlType.Auto);
                                    }
                                }
                                else
                                {
                                    ShowFunctionChoosePanel(cSVNpcData);
                                }
                                return;
                            }
                        }
                    }

                    if (functions != null && functions.Count == 1)
                    {
                        if (functions[0].OpenList == 0)
                        {
                            if (functions[0].Type == EFunctionType.LearnActiveSkill || functions[0].Type == EFunctionType.LearnPassiveSkill)
                            {
                                ShowFunctionChoosePanel(cSVNpcData);
                            }
                            else
                            {
                                InteractiveDisPlay(cSVNpcData, functions[0], FunctionBase.ECtrlType.Auto);
                            }
                        }
                        else
                        {
                            ShowFunctionChoosePanel(cSVNpcData);
                        }

                        return;
                    }

                    if (functions != null && functions.Count > 1)
                    {
                        ShowFunctionChoosePanel(cSVNpcData);
                    }
                }
                else if (currentTaskEntry.taskState == ETaskState.Finished)
                {
                    foreach (FunctionBase function in functions)
                    {
                        if (function.ID == currentTaskEntry.id)
                        {
                            TaskFunction taskFunction = function as TaskFunction;
                            if (taskFunction != null && taskFunction.DialogueID != 0)
                            {
                                InteractiveDisPlay(cSVNpcData, function, FunctionBase.ECtrlType.Auto);
                            }
                        }
                    }
                }
            }
            else
            {
                if (CollectionCtrl.Instance.CanCollection)
                {
                    foreach (FunctionBase function in functions)
                    {
                        if (function.Type == EFunctionType.Collection)
                        {
                            InteractiveDisPlay(cSVNpcData, function, FunctionBase.ECtrlType.Auto);
                        }
                    }
                }
                else
                {
                    InteractiveProcess(cSVNpcData, functions);
                }
            }
        }

        #endregion

        /// <summary>
        /// 展示功能选择面板///
        /// </summary>
        void ShowFunctionChoosePanel(CSVNpc.Data cSVNpcData)
        {
            if (CurInteractiveNPC != null && CurInteractiveNPC.gameObject != null)
            {
                Vector3 endRot;
                Vector3 endPos;

                if (CurInteractiveNPC.modelGameObject != null)
                {
                    endRot = CurInteractiveNPC.gameObject.transform.eulerAngles + new Vector3(CurInteractiveNPC.cSVNpcData.dialogueEndParameter[3] / 10000f, CurInteractiveNPC.cSVNpcData.dialogueEndParameter[4] / 10000f, CurInteractiveNPC.cSVNpcData.dialogueEndParameter[5] / 10000f);
                    endPos = CurInteractiveNPC.gameObject.transform.position + new Vector3(CurInteractiveNPC.cSVNpcData.dialogueEndParameter[0] / 10000f, CurInteractiveNPC.cSVNpcData.dialogueEndParameter[1] / 10000f, CurInteractiveNPC.cSVNpcData.dialogueEndParameter[2] / 10000f);
                }
                else
                {
                    Vector3 pos = Vector3.zero;
                    Quaternion eular = Quaternion.identity;
                    Sys_Map.Instance.GetNpcPos(Sys_Map.Instance.CurMapId, cSVNpcData.id, ref pos, ref eular);

                    endRot = eular.eulerAngles + new Vector3(CurInteractiveNPC.cSVNpcData.dialogueEndParameter[3] / 10000f, CurInteractiveNPC.cSVNpcData.dialogueEndParameter[4] / 10000f, CurInteractiveNPC.cSVNpcData.dialogueEndParameter[5] / 10000f);
                    endPos = pos + new Vector3(CurInteractiveNPC.cSVNpcData.dialogueEndParameter[0] / 10000f, CurInteractiveNPC.cSVNpcData.dialogueEndParameter[1] / 10000f, CurInteractiveNPC.cSVNpcData.dialogueEndParameter[2] / 10000f);
                }               

                float distance = Vector3.Distance(GameCenter.mainHero.transform.position, endPos);
                if (distance > 0.1f)
                {
                    if (GameCenter.mainHero.Mount == null)
                    {
                        GameCenter.mainHero.transform.DOLookAt(endPos, 0.01f, AxisConstraint.Y, Vector3.up).onComplete = delegate ()
                        {
                            GameCenter.mainHero.animationComponent.Play((uint)EStateType.Run);
                            GameCenter.mainHero.transform.DOMove(endPos, distance / float.Parse(CSVParam.Instance.GetConfData(129).str_value)).SetEase(Ease.Linear).onComplete = delegate ()
                            {
                                GameCenter.mainHero.animationComponent.Play((uint)EStateType.Idle);
                                GameCenter.mainHero.transform.DORotate(endRot, 0.1f, RotateMode.Fast);
                                if (!UIManager.IsOpen(EUIID.UI_NPC))
                                {
                                    //展示//
                                    ShowAcquiesceDialogue(cSVNpcData);
                                    UIManager.OpenUI(EUIID.UI_NPC, false, new InteractiveData()
                                    {
                                        CSVNpcData = cSVNpcData,
                                    });
                                }
                            };
                        };
                    }
                    else
                    {
                        GameCenter.mainHero.transform.DOLookAt(endPos, 0.01f, AxisConstraint.Y, Vector3.up).onComplete = delegate ()
                        {
                            GameCenter.mainHero.Mount.animationComponent.Play((uint)EStateType.Sprint);
                            if (GameCenter.mainHero.Mount.csvPetData.action_id_mount == 0)
                            {                               
                                GameCenter.mainHero.animationComponent.Play((uint)EStateType.mount_1_run);
                            }
                            else
                            {
                                GameCenter.mainHero.animationComponent.Play((uint)EStateType.mount_2_run);
                            }

                            GameCenter.mainHero.transform.DOMove(endPos, distance / float.Parse(CSVParam.Instance.GetConfData(129).str_value)).SetEase(Ease.Linear).onComplete = delegate ()
                            {
                                GameCenter.mainHero.Mount.animationComponent.Play((uint)EStateType.Stand);
                                if (GameCenter.mainHero.Mount.csvPetData.action_id_mount == 0)
                                {
                                    GameCenter.mainHero.animationComponent.Play((uint)EStateType.mount_1_idle);
                                }
                                else
                                {
                                    GameCenter.mainHero.animationComponent.Play((uint)EStateType.mount_2_idle);
                                }
                                GameCenter.mainHero.transform.DORotate(endRot, 0.1f, RotateMode.Fast);
                                if (!UIManager.IsOpen(EUIID.UI_NPC))
                                {
                                    //展示//
                                    ShowAcquiesceDialogue(cSVNpcData);
                                    UIManager.OpenUI(EUIID.UI_NPC, false, new InteractiveData()
                                    {
                                        CSVNpcData = cSVNpcData,
                                    });
                                }
                            };
                        };
                    }
                }
                else
                {
                    if (GameCenter.mainHero.Mount == null)
                    {
                        GameCenter.mainHero.animationComponent.Play((uint)EStateType.Idle);                      
                    }
                    else
                    {
                        GameCenter.mainHero.Mount.animationComponent.Play((uint)EStateType.Stand);
                        if (GameCenter.mainHero.Mount.csvPetData.action_id_mount == 0)
                        {
                            GameCenter.mainHero.animationComponent.Play((uint)EStateType.mount_1_idle);
                        }
                        else
                        {
                            GameCenter.mainHero.animationComponent.Play((uint)EStateType.mount_2_idle);
                        }
                    }
                    GameCenter.mainHero.transform.DORotate(endRot, 0.1f, RotateMode.Fast);

                    if (!UIManager.IsOpen(EUIID.UI_NPC))
                    {
                        //展示//
                        ShowAcquiesceDialogue(cSVNpcData);
                        UIManager.OpenUI(EUIID.UI_NPC, false, new InteractiveData()
                        {
                            CSVNpcData = cSVNpcData,
                        });
                    }
                }
            }
        }

        /// <summary>
        /// 交互///
        /// </summary>
        /// <param name="cSVNpcData"></param>
        /// <param name="functionBase"></param>
        /// <param name="over"></param>
        public void InteractiveDisPlay(CSVNpc.Data cSVNpcData, FunctionBase functionBase, FunctionBase.ECtrlType ctrlType)
        {
            if (IsDirectlyExecuteFunction(cSVNpcData, functionBase))
            {
                DirectlyExecuteFunction(cSVNpcData, functionBase, ctrlType);
            }
            else
            {
                AnimExecuteFunction(cSVNpcData, functionBase, ctrlType);
            }
        }

        /// <summary>
        /// 是否直接执行交互功能///
        /// </summary>
        /// <returns></returns>
        bool IsDirectlyExecuteFunction(CSVNpc.Data cSVNpcData, FunctionBase functionBase)
        {
            if (cSVNpcData.type != (uint)ENPCType.Common || (functionBase != null && functionBase.Type == EFunctionType.Collection))
            {
                if (cSVNpcData.type == (uint)ENPCType.Collection && cSVNpcData.subtype == 1)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            if (functionBase.Type == EFunctionType.ResourceSubmit || functionBase.Type == EFunctionType.HpMpUp)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 直接执行交互功能///
        /// </summary>
        public void DirectlyExecuteFunction(CSVNpc.Data cSVNpcData, FunctionBase functionBase, FunctionBase.ECtrlType ctrlType)
        {
            if (CurInteractiveNPC != null)
            {
                if (functionBase.Type == EFunctionType.ResourceSubmit || functionBase.Type == EFunctionType.HpMpUp)
                { }
                else
                {
                    GameCenter.mainHero.transform.LookAt(CurInteractiveNPC.gameObject.transform.position, Vector3.up);
                }
            }
            else
            {
                DebugUtil.LogError($"CurInteractiveNPC is null but you want interactive, infoID: {cSVNpcData.id}, taskID: {functionBase.HandlerID}, taskIndex: {functionBase.HandlerIndex}");
            }

            TeamSync(functionBase, ctrlType);
        }

        /// <summary>
        /// 队伍同步///
        /// </summary>
        /// <param name="functionBase"></param>
        void TeamSync(FunctionBase functionBase, FunctionBase.ECtrlType ctrlType)
        {
            if (Sys_Team.Instance.isCaptain() && Sys_Team.Instance.TeamMemsCount > 1)
            {
                if (functionBase.Type == EFunctionType.Dialogue || functionBase.Type == EFunctionType.Task)
                {
                    CmdTeamSyncNpcFunc cmdTeamSyncNpcFunc = new CmdTeamSyncNpcFunc();
                    cmdTeamSyncNpcFunc.NNpcId = CurInteractiveNPC.uID;
                    cmdTeamSyncNpcFunc.FuncId = (uint)functionBase.Type;
                    cmdTeamSyncNpcFunc.ParamId = functionBase.ID;
                    cmdTeamSyncNpcFunc.TaskId = functionBase.HandlerID;

                    NetClient.Instance.SendMessage((ushort)CmdTeam.SyncNpcFunc, cmdTeamSyncNpcFunc);
                }
                else
                {
                    functionBase.Execute(ctrlType);
                }
            }
            else
            {
                functionBase.Execute(ctrlType);
            }
        }

        /// <summary>
        /// 动画执行交互功能///
        /// </summary>
        public void AnimExecuteFunction(CSVNpc.Data cSVNpcData, FunctionBase functionBase, FunctionBase.ECtrlType ctrlType)
        {
            if (CurInteractiveNPC != null && CurInteractiveNPC.gameObject != null)
            {
                Vector3 endRot;
                Vector3 endPos;

                if (CurInteractiveNPC.modelGameObject != null)
                {
                    endRot = CurInteractiveNPC.gameObject.transform.eulerAngles + new Vector3(CurInteractiveNPC.cSVNpcData.dialogueEndParameter[3] / 10000f, CurInteractiveNPC.cSVNpcData.dialogueEndParameter[4] / 10000f, CurInteractiveNPC.cSVNpcData.dialogueEndParameter[5] / 10000f);
                    endPos = CurInteractiveNPC.gameObject.transform.position + new Vector3(CurInteractiveNPC.cSVNpcData.dialogueEndParameter[0] / 10000f, CurInteractiveNPC.cSVNpcData.dialogueEndParameter[1] / 10000f, CurInteractiveNPC.cSVNpcData.dialogueEndParameter[2] / 10000f);
                }
                else
                {
                    Vector3 pos = Vector3.zero;
                    Quaternion eular = Quaternion.identity;
                    Sys_Map.Instance.GetNpcPos(Sys_Map.Instance.CurMapId, cSVNpcData.id, ref pos, ref eular);

                    endRot = eular.eulerAngles + new Vector3(CurInteractiveNPC.cSVNpcData.dialogueEndParameter[3] / 10000f, CurInteractiveNPC.cSVNpcData.dialogueEndParameter[4] / 10000f, CurInteractiveNPC.cSVNpcData.dialogueEndParameter[5] / 10000f);
                    endPos = pos + new Vector3(CurInteractiveNPC.cSVNpcData.dialogueEndParameter[0] / 10000f, CurInteractiveNPC.cSVNpcData.dialogueEndParameter[1] / 10000f, CurInteractiveNPC.cSVNpcData.dialogueEndParameter[2] / 10000f);
                }

                float distance = Vector3.Distance(GameCenter.mainHero.transform.position, endPos);
                if (distance > 0.1f)
                {
                    if (GameCenter.mainHero.Mount == null)
                    {
                        GameCenter.mainHero.transform.DOLookAt(endPos, 0.01f, AxisConstraint.Y, Vector3.up).onComplete = delegate ()
                        {
                            GameCenter.mainHero.animationComponent.Play((uint)EStateType.Run);
                            GameCenter.mainHero.transform.DOMove(endPos, distance / float.Parse(CSVParam.Instance.GetConfData(129).str_value)).SetEase(Ease.Linear).onComplete = delegate ()
                            {
                                GameCenter.mainHero.animationComponent.Play((uint)EStateType.Idle);
                                GameCenter.mainHero.transform.DORotate(endRot, 0.1f, RotateMode.Fast);
                                AnimationingOver(cSVNpcData, CurInteractiveNPC, functionBase, ctrlType);
                            };
                        };
                    }
                    else
                    {
                        GameCenter.mainHero.transform.DOLookAt(endPos, 0.01f, AxisConstraint.Y, Vector3.up).onComplete = delegate ()
                        {
                            GameCenter.mainHero.Mount.animationComponent.Play((uint)EStateType.Sprint);
                            if (GameCenter.mainHero.Mount.csvPetData.action_id_mount == 0)
                            {
                                GameCenter.mainHero.animationComponent.Play((uint)EStateType.mount_1_run);
                            }
                            else
                            {
                                GameCenter.mainHero.animationComponent.Play((uint)EStateType.mount_2_run);
                            }

                            GameCenter.mainHero.transform.DOMove(endPos, distance / float.Parse(CSVParam.Instance.GetConfData(129).str_value)).SetEase(Ease.Linear).onComplete = delegate ()
                            {
                                GameCenter.mainHero.Mount.animationComponent.Play((uint)EStateType.Stand);
                                if (GameCenter.mainHero.Mount.csvPetData.action_id_mount == 0)
                                {
                                    GameCenter.mainHero.animationComponent.Play((uint)EStateType.mount_1_idle);
                                }
                                else
                                {
                                    GameCenter.mainHero.animationComponent.Play((uint)EStateType.mount_2_idle);
                                }
                                GameCenter.mainHero.transform.DORotate(endRot, 0.1f, RotateMode.Fast);
                                AnimationingOver(cSVNpcData, CurInteractiveNPC, functionBase, ctrlType);
                            };
                        };
                    }
                }
                else
                {
                    if (GameCenter.mainHero.Mount == null)
                    {
                        GameCenter.mainHero.animationComponent.Play((uint)EStateType.Idle);
                    }
                    else
                    {
                        GameCenter.mainHero.Mount.animationComponent.Play((uint)EStateType.Stand);
                        if (GameCenter.mainHero.Mount.csvPetData.action_id_mount == 0)
                        {
                            GameCenter.mainHero.animationComponent.Play((uint)EStateType.mount_1_idle);
                        }
                        else
                        {
                            GameCenter.mainHero.animationComponent.Play((uint)EStateType.mount_2_idle);
                        }
                    }
                    GameCenter.mainHero.transform.DORotate(endRot, 0.1f, RotateMode.Fast);

                    AnimationingOver(cSVNpcData, CurInteractiveNPC, functionBase, ctrlType);
                }
            }
            else
            {
                DebugUtil.LogError($"CurInteractiveNPC is null but you want interactive, taskID: {functionBase.HandlerID}, taskIndex: {functionBase.HandlerIndex}");
            }
        }

        void AnimationingOver(CSVNpc.Data cSVNpcData, Npc npc, FunctionBase functionBase, FunctionBase.ECtrlType ctrlType)
        {
            if (GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Fight)
                return;

            TeamSync(functionBase, ctrlType);
        }

        /// <summary>
        /// 打开默认对话///
        /// </summary>
        /// <param name="ForceHide"></param>
        void ShowAcquiesceDialogue(CSVNpc.Data cSVNpcData, bool ForceHide = false)
        {
            CSVDialogue.Data cSVDialogueData = null;
            if (cSVNpcData.AcquiesceDialogue() != null && cSVNpcData.AcquiesceDialogue().Count > 0)
            {
                int index = UnityEngine.Random.Range(0, cSVNpcData.AcquiesceDialogue().Count);
                cSVDialogueData = CSVDialogue.Instance.GetConfData(cSVNpcData.AcquiesceDialogue()[index]);
            }

            if (cSVDialogueData != null)
            {
                List<Sys_Dialogue.DialogueDataWrap> dialogueDataWraps = Sys_Dialogue.GetAcquiesceDialogueDataWraps(cSVDialogueData, cSVNpcData);

                ResetDialogueDataEventData resetDialogueDataEventData = PoolManager.Fetch(typeof(ResetDialogueDataEventData)) as ResetDialogueDataEventData;
                resetDialogueDataEventData.Init(dialogueDataWraps, null, cSVDialogueData);
                if (ForceHide)
                {
                    resetDialogueDataEventData.hideNextButton = false;
                }
                Sys_Dialogue.Instance.OpenDialogue(resetDialogueDataEventData);
            }
        }
    }



    public partial class Sys_Interactive : SystemModuleBase<Sys_Interactive>
    {
        private Dictionary<EInteractiveAimType, List<IInteractiveWatcher>> allWatchers;

        public readonly EventEmitter<EInteractiveType> eventEmitter = new EventEmitter<EInteractiveType>();

        public override void Init()
        {
            base.Init();

            eventEmitter.Handle<InteractiveEvtData>(EInteractiveType.AreaCheck, OnAreaCheck, true);
            eventEmitter.Handle<InteractiveEvtData>(EInteractiveType.Click, OnClick, true);
            eventEmitter.Handle<InteractiveEvtData>(EInteractiveType.DoubleClick, OnDoubleClick, true);
            eventEmitter.Handle<InteractiveEvtData>(EInteractiveType.LongPress, OnLongPress, true);
            eventEmitter.Handle<InteractiveEvtData>(EInteractiveType.DistanceCheck, OnDistanceCheck, true);
            eventEmitter.Handle<InteractiveEvtData>(EInteractiveType.UIButton, OnUIButton, true);

            allWatchers = new Dictionary<EInteractiveAimType, List<IInteractiveWatcher>>();

            Type[] types = AssemblyManager.Instance.GetTypes();

            foreach (Type type in types)
            {
                object[] attrs = type.GetCustomAttributes(typeof(InteractiveWatcherAttribute), false);
                foreach (object attr in attrs)
                {
                    InteractiveWatcherAttribute interactiveWatcherAttribute = (InteractiveWatcherAttribute)attr;
                    IInteractiveWatcher obj = (IInteractiveWatcher)Activator.CreateInstance(type);
                    if (!allWatchers.ContainsKey(interactiveWatcherAttribute.InteractiveAimType))
                    {
                        allWatchers.Add(interactiveWatcherAttribute.InteractiveAimType, new List<IInteractiveWatcher>());
                    }
                    allWatchers[interactiveWatcherAttribute.InteractiveAimType].Add(obj);
                }
            }

            EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTeam.SyncNpcFunc, OnTeamSyncNpcFunc, CmdTeamSyncNpcFunc.Parser);
        }

        public override void Dispose()
        {
            eventEmitter.Handle<InteractiveEvtData>(EInteractiveType.AreaCheck, OnAreaCheck, false);
            eventEmitter.Handle<InteractiveEvtData>(EInteractiveType.Click, OnClick, false);
            eventEmitter.Handle<InteractiveEvtData>(EInteractiveType.DoubleClick, OnDoubleClick, false);
            eventEmitter.Handle<InteractiveEvtData>(EInteractiveType.LongPress, OnLongPress, false);
            eventEmitter.Handle<InteractiveEvtData>(EInteractiveType.DistanceCheck, OnDistanceCheck, false);
            eventEmitter.Handle<InteractiveEvtData>(EInteractiveType.UIButton, OnUIButton, false);

            allWatchers?.Clear();

            base.Dispose();
        }

        void OnAreaCheck(InteractiveEvtData evtData)
        {
            List<IInteractiveWatcher> list;
            if (!allWatchers.TryGetValue(evtData.eInteractiveAimType, out list))
            {
                return;
            }

            foreach (IInteractiveWatcher interactiveWatcher in list)
            {
                interactiveWatcher.OnAreaCheckExecute(evtData);
            }
        }

        void OnClick(InteractiveEvtData evtData)
        {
            List<IInteractiveWatcher> list;
            if (!allWatchers.TryGetValue(evtData.eInteractiveAimType, out list))
            {
                return;
            }

            foreach (IInteractiveWatcher interactiveWatcher in list)
            {
                interactiveWatcher.OnClickExecute(evtData);
            }
        }

        void OnDoubleClick(InteractiveEvtData evtData)
        {
            List<IInteractiveWatcher> list;
            if (!allWatchers.TryGetValue(evtData.eInteractiveAimType, out list))
            {
                return;
            }

            foreach (IInteractiveWatcher interactiveWatcher in list)
            {
                interactiveWatcher.OnDoubleClickExecute(evtData);
            }
        }

        void OnLongPress(InteractiveEvtData evtData)
        {
            List<IInteractiveWatcher> list;
            if (!allWatchers.TryGetValue(evtData.eInteractiveAimType, out list))
            {
                return;
            }

            foreach (IInteractiveWatcher interactiveWatcher in list)
            {
                interactiveWatcher.OnLongPressExecute(evtData);
            }
        }

        void OnDistanceCheck(InteractiveEvtData evtData)
        {
            List<IInteractiveWatcher> list;
            if (!allWatchers.TryGetValue(evtData.eInteractiveAimType, out list))
            {
                return;
            }

            foreach (IInteractiveWatcher interactiveWatcher in list)
            {
                interactiveWatcher.OnDistanceCheckExecute(evtData);
            }
        }

        void OnUIButton(InteractiveEvtData evtData)
        {
            List<IInteractiveWatcher> list;
            if (!allWatchers.TryGetValue(evtData.eInteractiveAimType, out list))
            {
                return;
            }

            foreach (IInteractiveWatcher interactiveWatcher in list)
            {
                interactiveWatcher.OnUIButtonExecute(evtData);
            }
        }

        void OnTeamSyncNpcFunc(NetMsg msg)
        {
            CmdTeamSyncNpcFunc cmdTeamSyncNpcFunc = NetMsgUtil.Deserialize<CmdTeamSyncNpcFunc>(CmdTeamSyncNpcFunc.Parser, msg);

            if (cmdTeamSyncNpcFunc != null)
            {
                //Npc npc = GameCenter.mainWorld.GetActor(typeof(Npc), cmdTeamSyncNpcFunc.NNpcId) as Npc;
                //if (npc != null)
                if (GameCenter.TryGetSceneNPC(cmdTeamSyncNpcFunc.NNpcId, out Npc npc))
                {
                    List<FunctionBase> filteredFunctions = npc.NPCFunctionComponent.FilterFunctions();
                    for (int index = 0, len = filteredFunctions.Count; index < len; index++)
                    {
                        if (filteredFunctions[index].ID == cmdTeamSyncNpcFunc.ParamId && (uint)filteredFunctions[index].Type == cmdTeamSyncNpcFunc.FuncId)
                        {
                            GameMain.Procedure.TriggerEvent(this, (int)EProcedureEvent.EnterInteractive);
                            filteredFunctions[index].Execute(FunctionBase.ECtrlType.NetSync);
                        }
                    }
                }
            }
        }
    }
}
