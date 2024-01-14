using Table;
using UnityEngine;
using System.Collections.Generic;
using Lib.Core;

namespace Logic
{
    public class NPCHUDSystem : LevelSystemBase
    {
        private string sDefaultName = "没配";
        private bool bUpdateStateFlag = true;
        private bool bUpdateIcon = true;
        private float fWaitTimeUpdateIcon = 0f;

        private uint iconId_181;
        //private uint iconId_182;
        //private uint iconId_183;
        private uint iconId_184;
        private uint iconId_185;
        private uint iconId_186;
        private uint iconId_187;
        private uint iconId_188;
        private uint iconId_189;
        private uint iconId_190;
        private uint iconId_191;
        private uint iconId_192;
        private uint iconId_193;
        private uint iconId_196;
        private ActorHUDUpdateEvt tempActorHUDUpdateEvt = new ActorHUDUpdateEvt();
        private CreateOrUpdateActorHUDStateFlagEvt tempcreateOrUpdateActorHUDStateFlagEvt = new CreateOrUpdateActorHUDStateFlagEvt();

        private uint ParseIconID(uint paramID)
        {            
            if (!CSVParam.Instance.TryGetValue(paramID, out CSVParam.Data data))
            {
                DebugUtil.LogErrorFormat("NPCHUDSystem.ParseIconID({0}) not find param data", paramID.ToString());
                return 0;
            }

            if (!uint.TryParse(data.str_value, out uint iconID))
            {
                DebugUtil.LogErrorFormat("NPCHUDSystem.ParseIconID({0}) {1} Parse to uint failed", paramID.ToString(), data.str_value);
                return 0;
            }

            return iconID;
        }

        public override void OnCreate()
        {
            iconId_181 = ParseIconID(181);
            //iconId_182 = ParseIconID(182);
            //iconId_183 = ParseIconID(183);
            iconId_184 = ParseIconID(184);
            iconId_185 = ParseIconID(185);
            iconId_186 = ParseIconID(186);
            iconId_187 = ParseIconID(187);
            iconId_188 = ParseIconID(188);
            iconId_189 = ParseIconID(189);
            iconId_190 = ParseIconID(190);
            iconId_191 = ParseIconID(191);
            iconId_192 = ParseIconID(192);
            iconId_193 = ParseIconID(193);
            iconId_196 = ParseIconID(196);

            ProcessEvents(true);
        }

        public override void OnDestroy()
        {
            ProcessEvents(false);
        }

        private void ProcessEvents(bool toRegister)
        {
            Sys_Task.Instance.eventEmitter.Handle<TaskEntry, ETaskState, ETaskState>(Sys_Task.EEvents.OnTaskStatusChanged, OnTaskStatusChanged, toRegister);
            Sys_Weather.Instance.eventEmitter.Handle(Sys_Weather.EEvents.OnWeatherChange, OnWeatherChange, toRegister);
            Sys_Weather.Instance.eventEmitter.Handle(Sys_Weather.EEvents.OnDayNightChange, OnDayNightChange, toRegister);
            Sys_Weather.Instance.eventEmitter.Handle(Sys_Weather.EEvents.OnSeasonChange, OnSeasonChange, toRegister);
            Sys_Attr.Instance.eventEmitter.Handle(Sys_Attr.EEvents.OnUpdateLevel, OnUpdateLevel, toRegister);
            Sys_Bag.Instance.eventEmitter.Handle<int, int>(Sys_Bag.EEvents.OnRefreshChangeData, OnRefreshChangeData, toRegister);
            Sys_Inquiry.eventEmitter.Handle(Sys_Inquiry.EEvents.InquiryCompleted, OnInquiryCompleted, toRegister);
            Sys_FunctionOpen.Instance.eventEmitter.Handle<Sys_FunctionOpen.FunctionOpenData>(Sys_FunctionOpen.EEvents.CompletedFunctionOpen, OnCompletedFunctionOpen, toRegister);
            Sys_SecretMessage.Instance.eventEmitter.Handle(Sys_SecretMessage.EEvents.GetMessageRightCallBack, OnMessageRight, toRegister);
            Sys_Role.Instance.eventEmitter.Handle(Sys_Role.EEvents.OnSyncFinished, OnSyncFinished, toRegister);
            Sys_CollectItem.Instance.eventEmitter.Handle<uint>(Sys_CollectItem.EEvents.OnCollectSuccess, OnCollectSuccess, toRegister);
        }

        public override void OnUpdate()
        {
            if (fWaitTimeUpdateIcon > 0f)
            {
                fWaitTimeUpdateIcon -= Time.unscaledDeltaTime;
                if (fWaitTimeUpdateIcon <= 0f)
                {
                    fWaitTimeUpdateIcon = 0f;
                    bUpdateIcon = true;
                }
            }

            if(bUpdateIcon && bUpdateStateFlag)
            {
                bUpdateIcon = false;
                bUpdateStateFlag = false;

                for (int i = 0; i < GameCenter.npcsList.Count; ++i)
                {
                    Npc npc = GameCenter.npcsList[i];
                    if (npc.cSVNpcData.nameShow != 1)
                    {
                        _UpdateIcon(npc);
                        _UpdateStateFlag(npc);
                    }
                }
            }
            else
            {
                if (bUpdateIcon)
                {
                    bUpdateIcon = false;

                    for (int i = 0; i < GameCenter.npcsList.Count; ++i)
                    {
                        Npc npc = GameCenter.npcsList[i];
                        if (npc.cSVNpcData.nameShow != 1)
                        {
                            _UpdateIcon(npc);
                        }
                    }
                }

                if (bUpdateStateFlag)
                {
                    bUpdateStateFlag = false;

                    for (int i = 0; i < GameCenter.npcsList.Count; ++i)
                    {
                        Npc npc = GameCenter.npcsList[i];
                        if (npc.cSVNpcData.nameShow != 1)
                        {
                            _UpdateStateFlag(npc);
                        }
                    }
                }
            }
        }

        void OnRefreshChangeData(int changeType, int curBoxId)
        {
            //UpdateStateFlag();
            bUpdateStateFlag = true;            
        }

        void OnMessageRight()
        {
            //UpdateStateFlag();
            bUpdateStateFlag = true;            
        }

        void OnInquiryCompleted()
        {
            //UpdateStateFlag();
            bUpdateStateFlag = true;            
        }

        void OnCollectSuccess(uint id)
        {
            bUpdateStateFlag = true;
        }

        void OnUpdateLevel()
        {
            //UpdateStateFlag();
            bUpdateStateFlag = true;
        }

        void OnWeatherChange()
        {
            bUpdateStateFlag = true;
            //UpdateStateFlag();
        }

        void OnDayNightChange()
        {
            //UpdateStateFlag();
            bUpdateStateFlag = true;            
        }

        void OnSeasonChange()
        {
            //UpdateStateFlag();
            bUpdateStateFlag = true;            
        }

        void OnCompletedFunctionOpen(Sys_FunctionOpen.FunctionOpenData functionOpenData)
        {
            //UpdateIcon();
            bUpdateIcon = true;            
        }

        void OnTaskStatusChanged(TaskEntry taskEntry, ETaskState oldState, ETaskState newState)
        {
            //updateTimer?.Cancel();
            //updateTimer = Timer.Register(1f, () =>
            //{
            //    UpdateIcon();
            //}, null, false, true);

            //UpdateStateFlag();
            fWaitTimeUpdateIcon = 1f;
            bUpdateStateFlag = true;
        }

        void OnSyncFinished()
        {            
            //UpdateIcon();
            //UpdateStateFlag();

            bUpdateIcon = true;
            bUpdateStateFlag = true;
        }

        public void UpdateStateFlag(Npc npc)
        {
            if (npc.cSVNpcData.nameShow == 1)
                return;

            _UpdateStateFlag(npc);
        }

        private void _UpdateStateFlag(Npc npc)
        {
            if (!npc.VisualComponent.Visiable)
                return;

            uint maxLevel = 0;
            uint selectType = 0;

            //里面仅仅做一次.IsValid()的判断过滤 直接外面判断就行
            //List<FunctionBase> filteredFunctions;
            //NPCFunctionComponent.FilterFunctions(npc.cSVNpcData, out filteredFunctions);

            List<FunctionBase> filteredFunctions = npc.cSVNpcData.CreateFunctionBases();            
            if (filteredFunctions == null)
                return;

            for (int index = 0, len = filteredFunctions.Count; index < len; index++)
            {
                if (filteredFunctions[index].IsValid())
                {
                    //CSVFunction.Data cSVFunctionData = CSVFunction.Instance.GetConfData((uint)filteredFunctions[index].Type);
                    if (CSVFunction.Instance.TryGetValue((uint)filteredFunctions[index].Type, out CSVFunction.Data cSVFunctionData))
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
                    //这里缺少安全判断
                    var cSVCollectionData = CSVCollection.Instance.GetConfData(npc.cSVNpcData.id);
                    if (cSVCollectionData.CollectionIconTips == 0)
                    {
                        return;
                    }
                    else
                    {
                        uint group = cSVCollectionData.CollectionGroup;
                        if (cSVCollectionData.CollectionGroup == 0)
                        {
                            group = cSVCollectionData.id;
                        }
                        if (cSVCollectionData.ICollectionNum != 0)
                        {
                            if (!Sys_CollectItem.Instance.collectItemTimes.ContainsKey(group) || Sys_CollectItem.Instance.collectItemTimes[group] < cSVCollectionData.ICollectionNum)
                            {
                            }
                            else
                            {
                                Sys_HUD.Instance.eventEmitter.Trigger<ulong>(Sys_HUD.EEvents.OnClearActorHUDStateFlag, npc.uID);
                                return;
                            }
                        }
                    }
                }

                //CreateOrUpdateActorHUDStateFlagEvt createOrUpdateActorHUDStateFlagEvt = new CreateOrUpdateActorHUDStateFlagEvt();
                //createOrUpdateActorHUDStateFlagEvt.id = npc.uID;
                //createOrUpdateActorHUDStateFlagEvt.type = (int)CSVFunction.Instance.GetConfData(selectType).topType;

                tempcreateOrUpdateActorHUDStateFlagEvt.id = npc.uID;
                tempcreateOrUpdateActorHUDStateFlagEvt.type = (int)CSVFunction.Instance.GetConfData(selectType).topType;
                Sys_HUD.Instance.eventEmitter.Trigger<CreateOrUpdateActorHUDStateFlagEvt>(Sys_HUD.EEvents.OnCreateOrUpdateActorHUDStateFlag, tempcreateOrUpdateActorHUDStateFlagEvt);
            }
            else
            {
                Sys_HUD.Instance.eventEmitter.Trigger<ulong>(Sys_HUD.EEvents.OnClearActorHUDStateFlag, npc.uID);
            }
        }

        private void _UpdateIcon(Npc npc)
        {
            if (npc == null)
                return;

            List<FunctionBase> filteredFunctions = npc.NPCFunctionComponent.GetAllFunctions();

            ETaskShowType firstShowType = ETaskShowType.None;
            ETaskShowType curShowType = ETaskShowType.None;
            
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
                tempActorHUDUpdateEvt.id = npc.uID;
                tempActorHUDUpdateEvt.iconId = iconId_185;
                Sys_HUD.Instance.eventEmitter.Trigger<ActorHUDUpdateEvt>(Sys_HUD.EEvents.OnUpdateActorHUD, tempActorHUDUpdateEvt);
            }
            else if (firstShowType == ETaskShowType.Challenge_Finish)
            {
                tempActorHUDUpdateEvt.id = npc.uID;
                tempActorHUDUpdateEvt.iconId = iconId_187;
                Sys_HUD.Instance.eventEmitter.Trigger<ActorHUDUpdateEvt>(Sys_HUD.EEvents.OnUpdateActorHUD, tempActorHUDUpdateEvt);
            }
            else if (firstShowType == ETaskShowType.Challenge_UnFinish)
            {
                tempActorHUDUpdateEvt.id = npc.uID;
                tempActorHUDUpdateEvt.iconId = iconId_186;
                Sys_HUD.Instance.eventEmitter.Trigger<ActorHUDUpdateEvt>(Sys_HUD.EEvents.OnUpdateActorHUD, tempActorHUDUpdateEvt);
            }
            else if (firstShowType == ETaskShowType.Challenge_Sumbited)
            {
                tempActorHUDUpdateEvt.id = npc.uID;
                tempActorHUDUpdateEvt.iconId = iconId_188;
                Sys_HUD.Instance.eventEmitter.Trigger<ActorHUDUpdateEvt>(Sys_HUD.EEvents.OnUpdateActorHUD, tempActorHUDUpdateEvt);
            }
            else if (firstShowType == ETaskShowType.Love_NoReceive)
            {
                tempActorHUDUpdateEvt.id = npc.uID;
                tempActorHUDUpdateEvt.iconId = iconId_189;
                Sys_HUD.Instance.eventEmitter.Trigger<ActorHUDUpdateEvt>(Sys_HUD.EEvents.OnUpdateActorHUD, tempActorHUDUpdateEvt);
            }
            else if (firstShowType == ETaskShowType.Love_Finish)
            {
                tempActorHUDUpdateEvt.id = npc.uID;
                tempActorHUDUpdateEvt.iconId = iconId_191;
                Sys_HUD.Instance.eventEmitter.Trigger<ActorHUDUpdateEvt>(Sys_HUD.EEvents.OnUpdateActorHUD, tempActorHUDUpdateEvt);
            }
            else if (firstShowType == ETaskShowType.Love_UnFinish)
            {
                tempActorHUDUpdateEvt.id = npc.uID;
                tempActorHUDUpdateEvt.iconId = iconId_190;
                Sys_HUD.Instance.eventEmitter.Trigger<ActorHUDUpdateEvt>(Sys_HUD.EEvents.OnUpdateActorHUD, tempActorHUDUpdateEvt);
            }
            else if (firstShowType == ETaskShowType.Love_Sumbited)
            {
                tempActorHUDUpdateEvt.id = npc.uID;
                tempActorHUDUpdateEvt.iconId = iconId_192;
                Sys_HUD.Instance.eventEmitter.Trigger<ActorHUDUpdateEvt>(Sys_HUD.EEvents.OnUpdateActorHUD, tempActorHUDUpdateEvt);
            }
            else if (firstShowType == ETaskShowType.Clue_NoReceive)
            {
                tempActorHUDUpdateEvt.id = npc.uID;
                tempActorHUDUpdateEvt.iconId = iconId_193;
                Sys_HUD.Instance.eventEmitter.Trigger<ActorHUDUpdateEvt>(Sys_HUD.EEvents.OnUpdateActorHUD, tempActorHUDUpdateEvt);
            }
            //else if (firstShowType == ETaskShowType.Clue_Finish)
            //{
            //    tempActorHUDUpdateEvt.id = npc.uID;
            //    tempActorHUDUpdateEvt.id = iconId_195;
            //    Sys_HUD.Instance.eventEmitter.Trigger<ActorHUDUpdateEvt>(Sys_HUD.EEvents.OnUpdateActorHUD, tempActorHUDUpdateEvt);            
            //}
            //else if (firstShowType == ETaskShowType.Clue_UnFinish)
            //{
            //    tempActorHUDUpdateEvt.id = npc.uID;
            //    tempActorHUDUpdateEvt.id = iconId_194;
            //    Sys_HUD.Instance.eventEmitter.Trigger<ActorHUDUpdateEvt>(Sys_HUD.EEvents.OnUpdateActorHUD, tempActorHUDUpdateEvt);            
            //}
            //else if (firstShowType == ETaskShowType.Clue_Sumbited)
            //{
            //    tempActorHUDUpdateEvt.id = npc.uID;
            //    tempActorHUDUpdateEvt.id = iconId_196;
            //    Sys_HUD.Instance.eventEmitter.Trigger<ActorHUDUpdateEvt>(Sys_HUD.EEvents.OnUpdateActorHUD, tempActorHUDUpdateEvt);            
            //}
            else if (firstShowType == ETaskShowType.Arrest_NoReceive)
            {
                tempActorHUDUpdateEvt.id = npc.uID;
                tempActorHUDUpdateEvt.iconId = iconId_181;
                Sys_HUD.Instance.eventEmitter.Trigger<ActorHUDUpdateEvt>(Sys_HUD.EEvents.OnUpdateActorHUD, tempActorHUDUpdateEvt);
            }
            //else if (firstShowType == ETaskShowType.Arrest_Finish)
            //{
            //    tempActorHUDUpdateEvt.id = npc.uID;
            //    tempActorHUDUpdateEvt.id = iconId_183;
            //    Sys_HUD.Instance.eventEmitter.Trigger<ActorHUDUpdateEvt>(Sys_HUD.EEvents.OnUpdateActorHUD, tempActorHUDUpdateEvt);            
            //}
            //else if (firstShowType == ETaskShowType.Arrest_UnFinish)
            //{
            //    tempActorHUDUpdateEvt.id = npc.uID;
            //    tempActorHUDUpdateEvt.id = iconId_182;
            //    Sys_HUD.Instance.eventEmitter.Trigger<ActorHUDUpdateEvt>(Sys_HUD.EEvents.OnUpdateActorHUD, tempActorHUDUpdateEvt);            
            //}
            //else if (firstShowType == ETaskShowType.Arrest_Sumbited)
            //{
            //    tempActorHUDUpdateEvt.id = npc.uID;
            //    tempActorHUDUpdateEvt.id = iconId_184;
            //    Sys_HUD.Instance.eventEmitter.Trigger<ActorHUDUpdateEvt>(Sys_HUD.EEvents.OnUpdateActorHUD, tempActorHUDUpdateEvt);            
            //}
            else if (firstShowType == ETaskShowType.None)
            {
                tempActorHUDUpdateEvt.id = npc.uID;
                tempActorHUDUpdateEvt.iconId = _CheckHadCompleted(npc);
                Sys_HUD.Instance.eventEmitter.Trigger<ActorHUDUpdateEvt>(Sys_HUD.EEvents.OnUpdateActorHUD, tempActorHUDUpdateEvt);

                //Sys_HUD.Instance.eventEmitter.Trigger<ActorHUDUpdateEvt>(Sys_HUD.EEvents.OnUpdateActorHUD, new ActorHUDUpdateEvt()
                //{
                //    id = npc.uID,
                //    iconId = 0,
                //});

                //_CheckHadCompleted(npc);
            }
        }

        private uint _CheckHadCompleted(Npc npc)
        {
            if (npc == null)
                return 0;

            for (int i = 0, len = Sys_Task.Instance.finishedTasks.Count; i < len; ++i)
            {
                uint taskID = Sys_Task.Instance.finishedTasks[i];
                if (!CSVTask.Instance.TryGetValue(taskID, out CSVTask.Data cSVTaskData))
                {
                    //DebugUtil.LogError($"cSVTaskData is null {taskID}");
                    continue;
                }

                if (cSVTaskData.receiveNpc == npc.cSVNpcData.id)
                {
                    switch (cSVTaskData.taskCategory)
                    {
                        case (int)ETaskCategory.Challenge:
                            return iconId_188;

                        case (int)ETaskCategory.Love:
                            return iconId_192;

                        case (int)ETaskCategory.Clue:
                            return iconId_196;

                        case (int)ETaskCategory.Arrest:
                            return iconId_184;

                        default:
                            break;
                    }
                }
            }

            return 0;
        }

        public void AddNewNpc(Npc npc)
        {
            if (npc.cSVNpcData.nameShow == 1)
                return;

            string name = sDefaultName;
            CSVNpcLanguage.Data cSVLanguageData = CSVNpcLanguage.Instance.GetConfData(npc.cSVNpcData.name);
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

            _UpdateIcon(npc);
            //UpdateStateFlag(npc);
        }
    }
}