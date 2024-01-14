using Table;
using UnityEngine;
using System.Collections.Generic;
using Lib.Core;

namespace Logic
{
    public class VirtualNpcHUDComponent
    {
        public VirtualNpc virtualNpc;

        public void OnConstruct()
        {
            if (virtualNpc != null)
            {
                string name = "没配";
                CSVNpcLanguage.Data cSVLanguageData = CSVNpcLanguage.Instance.GetConfData(virtualNpc.cSVNpcData.name);
                if (cSVLanguageData != null)
                {
                    name = cSVLanguageData.words;
                }

                if (virtualNpc.cSVNpcData.signPositionShifting != null)
                {
                    CreateActorHUDEvt createActorHUDEvt = new CreateActorHUDEvt();
                    createActorHUDEvt.id = virtualNpc.uID;
                    createActorHUDEvt.gameObject = virtualNpc.gameObject;
                    createActorHUDEvt.offest = new Vector3(virtualNpc.cSVNpcData.signPositionShifting[0] / 10000f, virtualNpc.cSVNpcData.signPositionShifting[1] / 10000f, virtualNpc.cSVNpcData.signPositionShifting[2] / 10000f);
                    createActorHUDEvt.appellation = virtualNpc.cSVNpcData.appellation;
                    createActorHUDEvt.name = name;
                    Sys_HUD.Instance.eventEmitter.Trigger<CreateActorHUDEvt>(Sys_HUD.EEvents.OnCreateActorHUD, createActorHUDEvt);
                }
                else
                {
                    CreateActorHUDEvt createActorHUDEvt = new CreateActorHUDEvt();
                    createActorHUDEvt.id = virtualNpc.uID;
                    createActorHUDEvt.gameObject = virtualNpc.gameObject;
                    createActorHUDEvt.offest = new Vector3(0, 2.2f, 0);
                    createActorHUDEvt.appellation = virtualNpc.cSVNpcData.appellation;
                    createActorHUDEvt.name = name;
                    Sys_HUD.Instance.eventEmitter.Trigger<CreateActorHUDEvt>(Sys_HUD.EEvents.OnCreateActorHUD, createActorHUDEvt);
                }
            }
        }

        public void Dispose()
        {
            Sys_HUD.Instance.eventEmitter.Trigger<ulong>(Sys_HUD.EEvents.OnRemoveActorHUD, virtualNpc.uID);

            virtualNpc = null;
        }
    }
#if false
    public class NPCHUDComponent : Logic.Core.Component
    {
        public Npc npc;

        //List<TaskFunction> taskFunctions = new List<TaskFunction>();

        Timer updateTimer;
        protected override void OnConstruct()
        {
            base.OnConstruct();

            npc = actor as Npc;
            if (npc != null)
            {
                string name = "没配";
                CSVLanguage.Data cSVLanguageData = CSVLanguage.Instance.GetConfData(npc.cSVNpcData.name);
                if (cSVLanguageData != null)
                {
                    name = cSVLanguageData.words;
                }
                if (npc.cSVNpcData.signPositionShifting != null)
                {
                    CreateActorHUDEvt createActorHUDEvt = new CreateActorHUDEvt();
                    createActorHUDEvt.id = npc.uID;
                    createActorHUDEvt.gameObject = npc.gameObject;
                    createActorHUDEvt.offest = new Vector3(npc.cSVNpcData.signPositionShifting[0] / 10000f, npc.cSVNpcData.signPositionShifting[1] / 10000f, npc.cSVNpcData.signPositionShifting[2] / 10000f);
                    createActorHUDEvt.appellation = npc.cSVNpcData.appellation;
#if DEBUG_MODE
                    createActorHUDEvt.name = string.Format("{0}/{1}", npc.uID, name);
#else
                    createActorHUDEvt.name = name;
#endif
                    createActorHUDEvt.eFightOutActorType = EFightOutActorType.Npc;
                    Sys_HUD.Instance.eventEmitter.Trigger<CreateActorHUDEvt>(Sys_HUD.EEvents.OnCreateActorHUD, createActorHUDEvt);
                }
                else
                {
                    CreateActorHUDEvt createActorHUDEvt = new CreateActorHUDEvt();
                    createActorHUDEvt.id = npc.uID;
                    createActorHUDEvt.gameObject = npc.gameObject;
                    createActorHUDEvt.offest = new Vector3(0, 2.2f, 0);
                    createActorHUDEvt.appellation = npc.cSVNpcData.appellation;
#if DEBUG_MODE
                    createActorHUDEvt.name = string.Format("{0}/{1}", npc.uID, name);
#else
                    createActorHUDEvt.name = name;
#endif
                    createActorHUDEvt.eFightOutActorType = EFightOutActorType.Npc;
                    Sys_HUD.Instance.eventEmitter.Trigger<CreateActorHUDEvt>(Sys_HUD.EEvents.OnCreateActorHUD, createActorHUDEvt);
                }
                UpdateIcon();
                UpdateStateFlag();
            }

            Sys_Task.Instance.eventEmitter.Handle<TaskEntry, ETaskState, ETaskState>(Sys_Task.EEvents.OnTaskStatusChanged, OnTaskStatusChanged, true);
            Sys_Weather.Instance.eventEmitter.Handle(Sys_Weather.EEvents.OnWeatherChange, OnWeatherChange, true);
            Sys_Weather.Instance.eventEmitter.Handle(Sys_Weather.EEvents.OnDayNightChange, OnDayNightChange, true);
            Sys_Weather.Instance.eventEmitter.Handle(Sys_Weather.EEvents.OnSeasonChange, OnSeasonChange, true);
            Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnUpdateLevel, OnUpdateLevel, true);
            Sys_Bag.Instance.eventEmitter.Handle<int, int>(Sys_Bag.EEvents.OnRefreshChangeData, OnRefreshChangeData, true);
            Sys_Inquiry.eventEmitter.Handle(Sys_Inquiry.EEvents.InquiryCompleted, OnInquiryCompleted, true);
            Sys_FunctionOpen.Instance.eventEmitter.Handle<Sys_FunctionOpen.FunctionOpenData>(Sys_FunctionOpen.EEvents.CompletedFunctionOpen, OnCompletedFunctionOpen, true);
            Sys_SecretMessage.Instance.eventEmitter.Handle(Sys_SecretMessage.EEvents.GetMessageRightCallBack, OnMessageRight, true);
            Sys_Role.Instance.eventEmitter.Handle(Sys_Role.EEvents.OnSyncFinished, OnSyncFinished, true);
        }

        protected override void OnDispose()
        {
            npc = null;
            //taskFunctions.Clear();
            updateTimer?.Cancel();
            updateTimer = null;
            Sys_Task.Instance.eventEmitter.Handle<TaskEntry, ETaskState, ETaskState>(Sys_Task.EEvents.OnTaskStatusChanged, OnTaskStatusChanged, false);
            Sys_Weather.Instance.eventEmitter.Handle(Sys_Weather.EEvents.OnWeatherChange, OnWeatherChange, false);
            Sys_Weather.Instance.eventEmitter.Handle(Sys_Weather.EEvents.OnDayNightChange, OnDayNightChange, false);
            Sys_Weather.Instance.eventEmitter.Handle(Sys_Weather.EEvents.OnSeasonChange, OnSeasonChange, false);
            Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnUpdateLevel, OnUpdateLevel, false);
            Sys_Bag.Instance.eventEmitter.Handle<int, int>(Sys_Bag.EEvents.OnRefreshChangeData, OnRefreshChangeData, false);
            Sys_Inquiry.eventEmitter.Handle(Sys_Inquiry.EEvents.InquiryCompleted, OnInquiryCompleted, false);
            Sys_FunctionOpen.Instance.eventEmitter.Handle<Sys_FunctionOpen.FunctionOpenData>(Sys_FunctionOpen.EEvents.CompletedFunctionOpen, OnCompletedFunctionOpen, false);
            Sys_SecretMessage.Instance.eventEmitter.Handle(Sys_SecretMessage.EEvents.GetMessageRightCallBack, OnMessageRight, false);
            Sys_Role.Instance.eventEmitter.Handle(Sys_Role.EEvents.OnSyncFinished, OnSyncFinished, false);

            base.OnDispose();
        }

        void OnRefreshChangeData(int changeType, int curBoxId)
        {
            UpdateStateFlag();
        }

        void OnMessageRight()
        {
            UpdateStateFlag();
        }

        void OnInquiryCompleted()
        {
            UpdateStateFlag();
        }

        void OnUpdateLevel()
        {
            UpdateStateFlag();
        }

        void OnWeatherChange()
        {
            UpdateStateFlag();
        }

        void OnDayNightChange()
        {
            UpdateStateFlag();
        }

        void OnSeasonChange()
        {
            UpdateStateFlag();
        }

        void OnCompletedFunctionOpen(Sys_FunctionOpen.FunctionOpenData functionOpenData)
        {
            UpdateIcon();
        }

        void OnTaskStatusChanged(TaskEntry taskEntry, ETaskState oldState, ETaskState newState)
        {
            updateTimer?.Cancel();
            updateTimer = Timer.Register(1f, () =>
            {
                UpdateIcon();
            }, null, false, true);

            UpdateStateFlag();
        }

        void OnSyncFinished()
        {
            UpdateIcon();
            UpdateStateFlag();
        }

        public void UpdateStateFlag()
        {
            if (npc == null)
                return;

            if (npc.NPCFunctionComponent == null)
                return;

            if (npc.VisualComponent == null)
                return;

            if (!npc.VisualComponent.Visiable)
                return;

            uint maxLevel = 0;
            uint selectType = 0;
            List<FunctionBase> filteredFunctions;
            NPCFunctionComponent.FilterFunctions(npc.cSVNpcData, out filteredFunctions);
            if (filteredFunctions == null)
                return;

            for (int index = 0, len = filteredFunctions.Count; index < len; index++)
            {
                if (filteredFunctions[index].IsValid())
                {
                    CSVFunction.Data cSVFunctionData = CSVFunction.Instance.GetConfData((uint)filteredFunctions[index].Type);
                    if (cSVFunctionData != null)
                    {
                        if (maxLevel < cSVFunctionData.priority)
                        {
                            maxLevel = cSVFunctionData.priority;
                            selectType = (uint)filteredFunctions[index].Type;
                        }
                    }
                }
            }

            if (selectType != 0 && maxLevel != 0)
            {
                if (selectType == (uint)EFunctionType.Collection)
                {
                    if (CSVCollection.Instance.GetConfData(npc.cSVNpcData.id).CollectionIconTips == 0)
                    {
                        return;
                    }
                }

                CreateOrUpdateActorHUDStateFlagEvt createOrUpdateActorHUDStateFlagEvt = new CreateOrUpdateActorHUDStateFlagEvt();
                createOrUpdateActorHUDStateFlagEvt.id = npc.uID;
                createOrUpdateActorHUDStateFlagEvt.type = (int)CSVFunction.Instance.GetConfData(selectType).topType;
                Sys_HUD.Instance.eventEmitter.Trigger<CreateOrUpdateActorHUDStateFlagEvt>(Sys_HUD.EEvents.OnCreateOrUpdateActorHUDStateFlag, createOrUpdateActorHUDStateFlagEvt);
            }
            else
            {
                Sys_HUD.Instance.eventEmitter.Trigger<ulong>(Sys_HUD.EEvents.OnClearActorHUDStateFlag, npc.uID);
            }
        }

        void UpdateIcon()
        {
            if (npc == null || npc.NPCFunctionComponent == null)
                return;

            List<FunctionBase> filteredFunctions = npc.NPCFunctionComponent.GetAllFunctions();
            //for (int index = 0, len = filteredFunctions.Count; index < len; index++)
            //{
            //    if (filteredFunctions[index].Type == EFunctionType.Task /*&& filteredFunctions[index].IsValid()*/)
            //    {
            //        TaskFunction taskFunction = filteredFunctions[index] as TaskFunction;
            //        taskFunctions?.Add(taskFunction);
            //    }
            //}

            ETaskShowType firstShowType = ETaskShowType.None;
            ETaskShowType curShowType = ETaskShowType.None;
            //for (int index = 0, len = taskFunctions.Count; index < len; index++)
            for (int index = 0, len = filteredFunctions.Count; index < len; index++)
            {
                if (!(filteredFunctions[index].Type == EFunctionType.Task /*&& filteredFunctions[index].IsValid()*/))
                    continue;

                TaskFunction taskFunction = filteredFunctions[index] as TaskFunction;
                ETaskState eTaskState = Sys_Task.Instance.GetTaskState(taskFunction.ID);

                if (taskFunction.CSVTaskData.taskCategory == (int)ETaskCategory.Challenge)
                {
                    if (eTaskState == ETaskState.UnReceivedButCanReceive)
                    {
                        curShowType = ETaskShowType.Challenge_NoReceive;
                    }
                    else if (eTaskState == ETaskState.Finished)
                    {
                        curShowType = ETaskShowType.Challenge_Finish;
                    }
                    else if (eTaskState == ETaskState.UnFinished)
                    {
                        curShowType = ETaskShowType.Challenge_UnFinish;
                    }
                    else if (eTaskState == ETaskState.Submited)
                    {
                        curShowType = ETaskShowType.Challenge_Sumbited;
                    }
                    else
                    {
                        curShowType = ETaskShowType.None;
                    }
                }
                else if (taskFunction.CSVTaskData.taskCategory == (int)ETaskCategory.Love)
                {
                    if (eTaskState == ETaskState.UnReceivedButCanReceive)
                    {
                        curShowType = ETaskShowType.Love_NoReceive;
                    }
                    else if (eTaskState == ETaskState.Finished)
                    {
                        curShowType = ETaskShowType.Love_Finish;
                    }
                    else if (eTaskState == ETaskState.UnFinished)
                    {
                        curShowType = ETaskShowType.Love_UnFinish;
                    }
                    else if (eTaskState == ETaskState.Submited)
                    {
                        curShowType = ETaskShowType.Love_Sumbited;
                    }
                    else
                    {
                        curShowType = ETaskShowType.None;
                    }
                }
                else if (taskFunction.CSVTaskData.taskCategory == (int)ETaskCategory.Clue)
                {
                    if (eTaskState == ETaskState.UnReceivedButCanReceive)
                    {
                        curShowType = ETaskShowType.Clue_NoReceive;
                    }
                    //else if (eTaskState == ETaskState.Finished)
                    //{
                    //    curShowType = ETaskShowType.Clue_Finish;
                    //}
                    //else if (eTaskState == ETaskState.UnFinished)
                    //{
                    //    curShowType = ETaskShowType.Clue_UnFinish;
                    //}
                    //else if (eTaskState == ETaskState.Submited)
                    //{
                    //    curShowType = ETaskShowType.Clue_Sumbited;
                    //}
                    else
                    {
                        curShowType = ETaskShowType.None;
                    }
                }
                else if (taskFunction.CSVTaskData.taskCategory == (int)ETaskCategory.Arrest)
                {
                    if (eTaskState == ETaskState.UnReceivedButCanReceive)
                    {
                        curShowType = ETaskShowType.Arrest_NoReceive;
                    }
                    //else if (eTaskState == ETaskState.Finished)
                    //{
                    //    curShowType = ETaskShowType.Arrest_Finish;
                    //}
                    //else if (eTaskState == ETaskState.UnFinished)
                    //{
                    //    curShowType = ETaskShowType.Arrest_UnFinish;
                    //}
                    //else if (eTaskState == ETaskState.Submited)
                    //{
                    //    curShowType = ETaskShowType.Arrest_Sumbited;
                    //}
                    else
                    {
                        curShowType = ETaskShowType.None;
                    }
                }
                else
                {
                    curShowType = ETaskShowType.None;
                }

                if (curShowType > firstShowType)
                {
                    firstShowType = curShowType;
                }
            }

            if (firstShowType == ETaskShowType.Challenge_NoReceive)
            {
                Sys_HUD.Instance.eventEmitter.Trigger<ActorHUDUpdateEvt>(Sys_HUD.EEvents.OnUpdateActorHUD, new ActorHUDUpdateEvt()
                {
                    id = npc.uID,
                    iconId = uint.Parse(CSVParam.Instance.GetConfData(185).str_value),
                });
            }
            else if (firstShowType == ETaskShowType.Challenge_Finish)
            {
                Sys_HUD.Instance.eventEmitter.Trigger<ActorHUDUpdateEvt>(Sys_HUD.EEvents.OnUpdateActorHUD, new ActorHUDUpdateEvt()
                {
                    id = npc.uID,
                    iconId = uint.Parse(CSVParam.Instance.GetConfData(187).str_value),
                });
            }
            else if (firstShowType == ETaskShowType.Challenge_UnFinish)
            {
                Sys_HUD.Instance.eventEmitter.Trigger<ActorHUDUpdateEvt>(Sys_HUD.EEvents.OnUpdateActorHUD, new ActorHUDUpdateEvt()
                {
                    id = npc.uID,
                    iconId = uint.Parse(CSVParam.Instance.GetConfData(186).str_value),
                });
            }
            else if (firstShowType == ETaskShowType.Challenge_Sumbited)
            {
                Sys_HUD.Instance.eventEmitter.Trigger<ActorHUDUpdateEvt>(Sys_HUD.EEvents.OnUpdateActorHUD, new ActorHUDUpdateEvt()
                {
                    id = npc.uID,
                    iconId = uint.Parse(CSVParam.Instance.GetConfData(188).str_value),
                });
            }
            else if (firstShowType == ETaskShowType.Love_NoReceive)
            {
                Sys_HUD.Instance.eventEmitter.Trigger<ActorHUDUpdateEvt>(Sys_HUD.EEvents.OnUpdateActorHUD, new ActorHUDUpdateEvt()
                {
                    id = npc.uID,
                    iconId = uint.Parse(CSVParam.Instance.GetConfData(189).str_value),
                });
            }
            else if (firstShowType == ETaskShowType.Love_Finish)
            {
                Sys_HUD.Instance.eventEmitter.Trigger<ActorHUDUpdateEvt>(Sys_HUD.EEvents.OnUpdateActorHUD, new ActorHUDUpdateEvt()
                {
                    id = npc.uID,
                    iconId = uint.Parse(CSVParam.Instance.GetConfData(191).str_value),
                });
            }
            else if (firstShowType == ETaskShowType.Love_UnFinish)
            {
                Sys_HUD.Instance.eventEmitter.Trigger<ActorHUDUpdateEvt>(Sys_HUD.EEvents.OnUpdateActorHUD, new ActorHUDUpdateEvt()
                {
                    id = npc.uID,
                    iconId = uint.Parse(CSVParam.Instance.GetConfData(190).str_value),
                });
            }
            else if (firstShowType == ETaskShowType.Love_Sumbited)
            {
                Sys_HUD.Instance.eventEmitter.Trigger<ActorHUDUpdateEvt>(Sys_HUD.EEvents.OnUpdateActorHUD, new ActorHUDUpdateEvt()
                {
                    id = npc.uID,
                    iconId = uint.Parse(CSVParam.Instance.GetConfData(192).str_value),
                });
            }
            else if (firstShowType == ETaskShowType.Clue_NoReceive)
            {
                Sys_HUD.Instance.eventEmitter.Trigger<ActorHUDUpdateEvt>(Sys_HUD.EEvents.OnUpdateActorHUD, new ActorHUDUpdateEvt()
                {
                    id = npc.uID,
                    iconId = uint.Parse(CSVParam.Instance.GetConfData(193).str_value),
                });
            }
            //else if (firstShowType == ETaskShowType.Clue_Finish)
            //{
            //    Sys_HUD.Instance.eventEmitter.Trigger<ActorHUDUpdateEvt>(Sys_HUD.EEvents.OnUpdateActorHUD, new ActorHUDUpdateEvt()
            //    {
            //        id = npc.uID,
            //        iconId = uint.Parse(CSVParam.Instance.GetConfData(195).str_value),
            //    });
            //}
            //else if (firstShowType == ETaskShowType.Clue_UnFinish)
            //{
            //    Sys_HUD.Instance.eventEmitter.Trigger<ActorHUDUpdateEvt>(Sys_HUD.EEvents.OnUpdateActorHUD, new ActorHUDUpdateEvt()
            //    {
            //        id = npc.uID,
            //        iconId = uint.Parse(CSVParam.Instance.GetConfData(194).str_value),
            //    });
            //}
            //else if (firstShowType == ETaskShowType.Clue_Sumbited)
            //{
            //    Sys_HUD.Instance.eventEmitter.Trigger<ActorHUDUpdateEvt>(Sys_HUD.EEvents.OnUpdateActorHUD, new ActorHUDUpdateEvt()
            //    {
            //        id = npc.uID,
            //        iconId = uint.Parse(CSVParam.Instance.GetConfData(196).str_value),
            //    });
            //}
            else if (firstShowType == ETaskShowType.Arrest_NoReceive)
            {
                Sys_HUD.Instance.eventEmitter.Trigger<ActorHUDUpdateEvt>(Sys_HUD.EEvents.OnUpdateActorHUD, new ActorHUDUpdateEvt()
                {
                    id = npc.uID,
                    iconId = uint.Parse(CSVParam.Instance.GetConfData(181).str_value),
                });
            }
            //else if (firstShowType == ETaskShowType.Arrest_Finish)
            //{
            //    Sys_HUD.Instance.eventEmitter.Trigger<ActorHUDUpdateEvt>(Sys_HUD.EEvents.OnUpdateActorHUD, new ActorHUDUpdateEvt()
            //    {
            //        id = npc.uID,
            //        iconId = uint.Parse(CSVParam.Instance.GetConfData(183).str_value),
            //    });
            //}
            //else if (firstShowType == ETaskShowType.Arrest_UnFinish)
            //{
            //    Sys_HUD.Instance.eventEmitter.Trigger<ActorHUDUpdateEvt>(Sys_HUD.EEvents.OnUpdateActorHUD, new ActorHUDUpdateEvt()
            //    {
            //        id = npc.uID,
            //        iconId = uint.Parse(CSVParam.Instance.GetConfData(182).str_value),
            //    });
            //}
            //else if (firstShowType == ETaskShowType.Arrest_Sumbited)
            //{
            //    Sys_HUD.Instance.eventEmitter.Trigger<ActorHUDUpdateEvt>(Sys_HUD.EEvents.OnUpdateActorHUD, new ActorHUDUpdateEvt()
            //    {
            //        id = npc.uID,
            //        iconId = uint.Parse(CSVParam.Instance.GetConfData(184).str_value),
            //    });
            //}
            else if (firstShowType == ETaskShowType.None)
            {
                Sys_HUD.Instance.eventEmitter.Trigger<ActorHUDUpdateEvt>(Sys_HUD.EEvents.OnUpdateActorHUD, new ActorHUDUpdateEvt()
                {
                    id = npc.uID,
                    iconId = 0,
                });

                CheckHadCompleted();
            }
        }

        void CheckHadCompleted()
        {
            if (npc != null)
            {
                foreach (uint taskID in Sys_Task.Instance.finishedTasks)
                {
                    CSVTask.Data cSVTaskData = CSVTask.Instance.GetConfData(taskID);
                    if (cSVTaskData != null)
                    {
                        if (cSVTaskData.receiveNpc == npc.cSVNpcData.id)
                        {
                            if (cSVTaskData.taskCategory == (uint)ETaskCategory.Challenge)
                            {
                                Sys_HUD.Instance.eventEmitter.Trigger<ActorHUDUpdateEvt>(Sys_HUD.EEvents.OnUpdateActorHUD, new ActorHUDUpdateEvt()
                                {
                                    id = npc.uID,
                                    iconId = uint.Parse(CSVParam.Instance.GetConfData(188).str_value),
                                });
                                return;
                            }

                            if (cSVTaskData.taskCategory == (uint)ETaskCategory.Love)
                            {
                                Sys_HUD.Instance.eventEmitter.Trigger<ActorHUDUpdateEvt>(Sys_HUD.EEvents.OnUpdateActorHUD, new ActorHUDUpdateEvt()
                                {
                                    id = npc.uID,
                                    iconId = uint.Parse(CSVParam.Instance.GetConfData(192).str_value),
                                });
                                return;
                            }

                            if (cSVTaskData.taskCategory == (uint)ETaskCategory.Clue)
                            {
                                Sys_HUD.Instance.eventEmitter.Trigger<ActorHUDUpdateEvt>(Sys_HUD.EEvents.OnUpdateActorHUD, new ActorHUDUpdateEvt()
                                {
                                    id = npc.uID,
                                    iconId = uint.Parse(CSVParam.Instance.GetConfData(196).str_value),
                                });
                                return;
                            }

                            if (cSVTaskData.taskCategory == (uint)ETaskCategory.Arrest)
                            {
                                Sys_HUD.Instance.eventEmitter.Trigger<ActorHUDUpdateEvt>(Sys_HUD.EEvents.OnUpdateActorHUD, new ActorHUDUpdateEvt()
                                {
                                    id = npc.uID,
                                    iconId = uint.Parse(CSVParam.Instance.GetConfData(184).str_value),
                                });
                                return;
                            }
                        }
                    }
                    else
                    {
                        //DebugUtil.LogError($"cSVTaskData is null {taskID}");
                    }
                }
            }
        }
    }
#endif
}
