using Table;
using Lib.Core;
using System.Collections.Generic;
using System;
using Packet;
using Net;
using Logic.Core;

namespace Logic
{
    /// <summary>
    /// 对话选择功能///
    /// </summary>
    public class DialogueChooseFunction : FunctionBase
    {
        public CSVTalkChoose.Data CSVTalkChooseData
        {
            get;
            private set;
        }

        public Dictionary<int, DialogueChooseAnswer> dialogueChooseAnswers = new Dictionary<int, DialogueChooseAnswer>();

        public override void Init()
        {
            CSVTalkChooseData = CSVTalkChoose.Instance.GetConfData(ID);
            if (CSVTalkChooseData == null)
            {
                DebugUtil.LogError($"CSVTalkChoose.Data is Null, id: {ID}");
                return;
            }

            dialogueChooseAnswers[0] = InitDialogueAnswer(0, CSVTalkChooseData.TalkChoose1, CSVTalkChooseData.ChooseType1, CSVTalkChooseData.ChooseValue1, CSVTalkChooseData.ChooseEndTalk1, CSVTalkChooseData.ChooseRightAndWrong1, CSVTalkChooseData.ChooseWrongResult, CSVTalkChooseData.DetachWrong, HandlerID, HandlerIndex, ID, CSVTalkChooseData.id);
            dialogueChooseAnswers[1] = InitDialogueAnswer(1, CSVTalkChooseData.TalkChoose2, CSVTalkChooseData.ChooseType2, CSVTalkChooseData.ChooseValue2, CSVTalkChooseData.ChooseEndTalk2, CSVTalkChooseData.ChooseRightAndWrong2, CSVTalkChooseData.ChooseWrongResult, CSVTalkChooseData.DetachWrong, HandlerID, HandlerIndex, ID, CSVTalkChooseData.id);
            dialogueChooseAnswers[2] = InitDialogueAnswer(2, CSVTalkChooseData.TalkChoose3, CSVTalkChooseData.ChooseType3, CSVTalkChooseData.ChooseValue3, CSVTalkChooseData.ChooseEndTalk3, CSVTalkChooseData.ChooseRightAndWrong3, CSVTalkChooseData.ChooseWrongResult, CSVTalkChooseData.DetachWrong, HandlerID, HandlerIndex, ID, CSVTalkChooseData.id);
            dialogueChooseAnswers[3] = InitDialogueAnswer(3, CSVTalkChooseData.TalkChoose4, CSVTalkChooseData.ChooseType4, CSVTalkChooseData.ChooseValue4, CSVTalkChooseData.ChooseEndTalk4, CSVTalkChooseData.ChooseRightAndWrong4, CSVTalkChooseData.ChooseWrongResult, CSVTalkChooseData.DetachWrong, HandlerID, HandlerIndex, ID, CSVTalkChooseData.id);
            dialogueChooseAnswers[4] = InitDialogueAnswer(4, CSVTalkChooseData.TalkChoose5, CSVTalkChooseData.ChooseType5, CSVTalkChooseData.ChooseValue5, CSVTalkChooseData.ChooseEndTalk5, CSVTalkChooseData.ChooseRightAndWrong5, CSVTalkChooseData.ChooseWrongResult, CSVTalkChooseData.DetachWrong, HandlerID, HandlerIndex, ID, CSVTalkChooseData.id);

            foreach (var dialogueChooseAnswer in dialogueChooseAnswers.Values)
            {
                if (dialogueChooseAnswer != null)
                {
                    dialogueChooseAnswer.AnswerWrongDetach = () =>
                    {
                        UI_NPC.eventEmitter.Trigger<Dictionary<int, DialogueChooseAnswer>, int, bool>(UI_NPC.EEvents.OnClickDialogueChooseFunction, dialogueChooseAnswers, dialogueChooseAnswer.Index, true);
                    };
                    dialogueChooseAnswer.AnswerWrongUnDetach = () =>
                    {
                        UI_NPC.eventEmitter.Trigger<Dictionary<int, DialogueChooseAnswer>, int, bool>(UI_NPC.EEvents.OnClickDialogueChooseFunction, dialogueChooseAnswers, dialogueChooseAnswer.Index, false);
                    };
                }
            }
        }

        /// <summary>
        /// 初始化对话选项///
        /// </summary>
        /// <param name="index">选项下标</param>
        /// <param name="languageID">选项文字ID</param>
        /// <param name="chooseTypes">选项功能类型</param>
        /// <param name="values">选项参数</param>
        /// <param name="dialogueID">选项对话ID</param>
        /// <param name="rightAngWrong">是否正确选项</param>
        /// <param name="wrongResult"></param>
        /// <param name="wrongDetach"></param>
        /// <param name="handlerID">持有对话选择的功能ID</param>
        /// <param name="dialogueChooseID"></param>
        /// <param name="dialogueParentID"></param>
        /// <returns></returns>
        DialogueChooseAnswer InitDialogueAnswer(int index, uint languageID, List<uint> chooseTypes, List<uint> values, uint dialogueID, uint rightAngWrong, uint wrongResult, uint wrongDetach, uint handlerID, uint handlerIdnex, uint dialogueChooseID, uint dialogueParentID)
        {
            DialogueChooseAnswer dialogueChooseAnswer = null;
            if (chooseTypes != null && chooseTypes.Count > 0)
            {
                dialogueChooseAnswer = new DialogueChooseAnswer(index, languageID, chooseTypes, values, dialogueID, rightAngWrong, wrongResult, wrongDetach, handlerID, handlerIdnex, dialogueChooseID, dialogueParentID);
            }

            return dialogueChooseAnswer;
        }

        protected override void OnExecute()
        {
            // cuibinbin 进入对话选择的时候，不认为正在做自动任务
            Sys_Task.Instance.InterruptCurrentTaskDoing();

            Init();
            UI_NPC.eventEmitter.Trigger(UI_NPC.EEvents.OnClickDialogueChoose);
            ShowAnswers();
        }

        protected override void SetResetDialogueDataEventData(ResetDialogueDataEventData resetDialogueDataEventData)
        {
            resetDialogueDataEventData.holdDialogueOpen = true;
            resetDialogueDataEventData.onlyOnePass = true;
        }

        void ShowAnswers()
        {
            UI_NPC.eventEmitter.Trigger<Dictionary<int, DialogueChooseAnswer>, int, bool>(UI_NPC.EEvents.OnClickDialogueChooseFunction, dialogueChooseAnswers, -1, false);
        }

        protected override void OnDispose()
        {
            CSVTalkChooseData = null;
            dialogueChooseAnswers?.Clear();

            base.OnDispose();
        }
    }

    public class DialogueChooseAnswer
    {
        public class ReceiveTaskAnswerEvt
        {
            public uint dialogueChooseID;
            public int index;
            public uint handlerID;
            public uint handlerIndex;
            public uint value;
            public bool isCloseDialogue;
        }

        public class CompleteTargetAnswerEvt
        {
            public uint dialogueChooseID;
            public int index;
            public uint handlerID;
            public uint handlerIndex;
            public bool isCloseDialogue;
        }

        public class SubmitItemAnswerEvt
        {
            public uint dialogueChooseID;
            public int index;
            public uint handlerID;
            public uint handlerIndex;
            public uint value;
            public bool isCloseDialogue;
            public ulong npcuid;
        }

        public static readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public enum EEvents
        {
            None = 0,
            NextStep = 1,
            ReceiveTask = 2,
            CompleteTarget = 3,
            GoBack = 4,
            Fight = 5,
            FightWin = 6,
            GetItem = 7,
            LittleGameWin = 8,
            SubmitItem = 9,
            Escort = 10,
            OpenTeamUI = 11,
            SecretMessage = 12,
            StartTimeTask = 13,
            NpcFollow = 14,
            NpcTrack = 15, 

            ChooseAnswer = 99999,
        }

        public uint DialogueChooseID
        {
            get;
            private set;
        }

        public uint DialogueParentID
        {
            get;
            private set;
        }

        public int Index
        {
            get;
            private set;
        }

        public uint LanguageID
        {
            get;
            private set;
        }

        public List<uint> Types;

        public List<uint> Values;

        public uint EndDialogueID
        {
            get;
            private set;
        }

        public uint RightAndWrong
        {
            get;
            private set;
        }

        public uint ChooseWrongResult
        {
            get;
            private set;
        }

        public uint DetachWrong
        {
            get;
            private set;
        }

        public uint HandlerID
        {
            get;
            private set;
        }

        public uint HandlerIndex
        {
            get;
            private set;
        }

        public CSVTalkChoose.Data CSVTalkChooseData
        {
            get;
            private set;
        }

        public Action AnswerWrongDetach;
        public Action AnswerWrongUnDetach;

        public DialogueChooseAnswer(int index, uint languageID, List<uint> types, List<uint> values, uint endDialogueID, uint rightAndWrong, uint chooseWrongResult, uint detachWrong, uint handlerID, uint handlerIndex, uint dialogueChooseID, uint dialogueParentID)
        {
            DialogueChooseID = dialogueChooseID;
            Index = index;
            LanguageID = languageID;
            Types = types;
            Values = values;
            EndDialogueID = endDialogueID;
            RightAndWrong = rightAndWrong;
            ChooseWrongResult = chooseWrongResult;
            DetachWrong = detachWrong;
            HandlerID = handlerID;
            HandlerIndex = handlerIndex;
            DialogueParentID = dialogueParentID;
        }

        DialogueChooseAnswer InitDialogueAnswer(int index, uint languageID, List<uint> chooseTypes, List<uint> values, uint dialogueID, uint rightAngWrong, uint wrongResult, uint wrongDetach, uint handlerID, uint handlerIndex, uint dialogueParentID = 0)
        {
            DialogueChooseAnswer dialogueChooseAnswer = null;
            if (chooseTypes != null && chooseTypes.Count > 0 && languageID != 0)
            {
                if (values != null)
                {
                    dialogueChooseAnswer = new DialogueChooseAnswer(index, languageID, chooseTypes, values, dialogueID, rightAngWrong, wrongResult, wrongDetach, handlerID, handlerIndex, values[0], dialogueParentID);
                }
                else
                {
                    dialogueChooseAnswer = new DialogueChooseAnswer(index, languageID, chooseTypes, values, dialogueID, rightAngWrong, wrongResult, wrongDetach, handlerID, handlerIndex, 0, dialogueParentID);
                }
            }
            return dialogueChooseAnswer;
        }

        public void Choose()
        {
            eventEmitter.Trigger(EEvents.ChooseAnswer);
            if (EndDialogueID == 0)
            {
                Choosing();
            }
            else
            {
                CSVDialogue.Data cSVDialogueData = CSVDialogue.Instance.GetConfData(EndDialogueID);
                if (cSVDialogueData != null)
                {
                    List<Sys_Dialogue.DialogueDataWrap> datas = Sys_Dialogue.GetDialogueDataWraps(cSVDialogueData);

                    ResetDialogueDataEventData resetDialogueDataEventData = PoolManager.Fetch(typeof(ResetDialogueDataEventData)) as ResetDialogueDataEventData;
                    resetDialogueDataEventData.Init(datas, () =>
                    {
                        Choosing();
                    }, cSVDialogueData);

                    resetDialogueDataEventData.holdDialogueOpen = true;
                    if (Types.Contains((uint)EEvents.NextStep))
                    {
                        resetDialogueDataEventData.onlyOnePass = true;
                    }
                    else
                    {
                        resetDialogueDataEventData.onlyOnePass = false;
                    }
                    Sys_Dialogue.Instance.OpenDialogue(resetDialogueDataEventData);
                }
                else
                {
                    DebugUtil.LogError($"DialogueID is null id:{EndDialogueID}");
                }
            }
        }

        void Choosing()
        {
            if (Types != null && Types.Contains((uint)EEvents.GoBack))
            {
                Trigger(false, true);
                return;
            }

            if (RightAndWrong == (uint)EDailogueAnswerRightOrWrong.Wrong)
            {
                if (ChooseWrongResult == (uint)EDialogueWrongResult.Close)
                {
                    Trigger(false, true);
                }
                else if (ChooseWrongResult == (uint)EDialogueWrongResult.ReChoose)
                {
                    Trigger(true, false);
                    if (DetachWrong == (uint)EDialogueWrongDetach.Detach)
                    {
                        AnswerWrongDetach();
                    }
                    else if (DetachWrong == (uint)EDialogueWrongDetach.UnDetach)
                    {
                        AnswerWrongUnDetach();
                    }
                }
            }
            else if (RightAndWrong == (uint)EDailogueAnswerRightOrWrong.Right)
            {
                Trigger(false, true);
            }
        }

        void Trigger(bool reChoose, bool closeDialogue)
        {
            int len = Types.Count;
            for (int index = 0; index < len; index++)
            {
                if (Types[index] == (uint)EEvents.None)
                {
                    eventEmitter.Trigger<bool, bool>((EEvents)Types[index], reChoose, closeDialogue);
                }
                else if (Types[index] == (uint)EEvents.NextStep)
                {
                    CSVTalkChooseData = CSVTalkChoose.Instance.GetConfData(Values[index]);
                    if (CSVTalkChooseData != null)
                    {
                        Dictionary<int, DialogueChooseAnswer> dialogueChooseAnswers1 = new Dictionary<int, DialogueChooseAnswer>();
                        dialogueChooseAnswers1.Add(0, InitDialogueAnswer(0, CSVTalkChooseData.TalkChoose1, CSVTalkChooseData.ChooseType1, CSVTalkChooseData.ChooseValue1, CSVTalkChooseData.ChooseEndTalk1, CSVTalkChooseData.ChooseRightAndWrong1, CSVTalkChooseData.ChooseWrongResult, CSVTalkChooseData.DetachWrong, HandlerID, HandlerIndex, CSVTalkChooseData.id));
                        dialogueChooseAnswers1.Add(1, InitDialogueAnswer(1, CSVTalkChooseData.TalkChoose2, CSVTalkChooseData.ChooseType2, CSVTalkChooseData.ChooseValue2, CSVTalkChooseData.ChooseEndTalk2, CSVTalkChooseData.ChooseRightAndWrong2, CSVTalkChooseData.ChooseWrongResult, CSVTalkChooseData.DetachWrong, HandlerID, HandlerIndex, CSVTalkChooseData.id));
                        dialogueChooseAnswers1.Add(2, InitDialogueAnswer(2, CSVTalkChooseData.TalkChoose3, CSVTalkChooseData.ChooseType3, CSVTalkChooseData.ChooseValue3, CSVTalkChooseData.ChooseEndTalk3, CSVTalkChooseData.ChooseRightAndWrong3, CSVTalkChooseData.ChooseWrongResult, CSVTalkChooseData.DetachWrong, HandlerID, HandlerIndex, CSVTalkChooseData.id));
                        dialogueChooseAnswers1.Add(3, InitDialogueAnswer(3, CSVTalkChooseData.TalkChoose4, CSVTalkChooseData.ChooseType4, CSVTalkChooseData.ChooseValue4, CSVTalkChooseData.ChooseEndTalk4, CSVTalkChooseData.ChooseRightAndWrong4, CSVTalkChooseData.ChooseWrongResult, CSVTalkChooseData.DetachWrong, HandlerID, HandlerIndex, CSVTalkChooseData.id));
                        dialogueChooseAnswers1.Add(4, InitDialogueAnswer(4, CSVTalkChooseData.TalkChoose5, CSVTalkChooseData.ChooseType5, CSVTalkChooseData.ChooseValue5, CSVTalkChooseData.ChooseEndTalk5, CSVTalkChooseData.ChooseRightAndWrong5, CSVTalkChooseData.ChooseWrongResult, CSVTalkChooseData.DetachWrong, HandlerID, HandlerIndex, CSVTalkChooseData.id));

                        foreach (var dialogueChooseAnswer in dialogueChooseAnswers1.Values)
                        {
                            if (dialogueChooseAnswer != null)
                            {
                                dialogueChooseAnswer.AnswerWrongDetach = () =>
                                {
                                    UI_NPC.eventEmitter.Trigger<Dictionary<int, DialogueChooseAnswer>, int, bool>(UI_NPC.EEvents.OnClickDialogueChooseFunction, dialogueChooseAnswers1, dialogueChooseAnswer.Index, true);
                                };
                                dialogueChooseAnswer.AnswerWrongUnDetach = () =>
                                {
                                    UI_NPC.eventEmitter.Trigger<Dictionary<int, DialogueChooseAnswer>, int, bool>(UI_NPC.EEvents.OnClickDialogueChooseFunction, dialogueChooseAnswers1, dialogueChooseAnswer.Index, false);
                                };
                            }
                        }

                        eventEmitter.Trigger<Dictionary<int, DialogueChooseAnswer>>((EEvents)Types[index], dialogueChooseAnswers1);
                    }
                }
                else if (Types[index] == (uint)EEvents.GoBack)
                {
                    CSVTalkChooseData = CSVTalkChoose.Instance.GetConfData(Values[index]);
                    if (CSVTalkChooseData != null)
                    {
                        Dictionary<int, DialogueChooseAnswer> dialogueChooseAnswers2 = new Dictionary<int, DialogueChooseAnswer>();
                        dialogueChooseAnswers2.Add(0, InitDialogueAnswer(0, CSVTalkChooseData.TalkChoose1, CSVTalkChooseData.ChooseType1, CSVTalkChooseData.ChooseValue1, CSVTalkChooseData.ChooseEndTalk1, CSVTalkChooseData.ChooseRightAndWrong1, CSVTalkChooseData.ChooseWrongResult, CSVTalkChooseData.DetachWrong, HandlerID, HandlerIndex, CSVTalkChooseData.id));
                        dialogueChooseAnswers2.Add(1, InitDialogueAnswer(1, CSVTalkChooseData.TalkChoose2, CSVTalkChooseData.ChooseType2, CSVTalkChooseData.ChooseValue2, CSVTalkChooseData.ChooseEndTalk2, CSVTalkChooseData.ChooseRightAndWrong2, CSVTalkChooseData.ChooseWrongResult, CSVTalkChooseData.DetachWrong, HandlerID, HandlerIndex, CSVTalkChooseData.id));
                        dialogueChooseAnswers2.Add(2, InitDialogueAnswer(2, CSVTalkChooseData.TalkChoose3, CSVTalkChooseData.ChooseType3, CSVTalkChooseData.ChooseValue3, CSVTalkChooseData.ChooseEndTalk3, CSVTalkChooseData.ChooseRightAndWrong3, CSVTalkChooseData.ChooseWrongResult, CSVTalkChooseData.DetachWrong, HandlerID, HandlerIndex, CSVTalkChooseData.id));
                        dialogueChooseAnswers2.Add(3, InitDialogueAnswer(3, CSVTalkChooseData.TalkChoose4, CSVTalkChooseData.ChooseType4, CSVTalkChooseData.ChooseValue4, CSVTalkChooseData.ChooseEndTalk4, CSVTalkChooseData.ChooseRightAndWrong4, CSVTalkChooseData.ChooseWrongResult, CSVTalkChooseData.DetachWrong, HandlerID, HandlerIndex, CSVTalkChooseData.id));
                        dialogueChooseAnswers2.Add(4, InitDialogueAnswer(4, CSVTalkChooseData.TalkChoose5, CSVTalkChooseData.ChooseType5, CSVTalkChooseData.ChooseValue5, CSVTalkChooseData.ChooseEndTalk5, CSVTalkChooseData.ChooseRightAndWrong5, CSVTalkChooseData.ChooseWrongResult, CSVTalkChooseData.DetachWrong, HandlerID, HandlerIndex, CSVTalkChooseData.id));

                        foreach (var dialogueChooseAnswer in dialogueChooseAnswers2.Values)
                        {
                            if (dialogueChooseAnswer != null)
                            {
                                dialogueChooseAnswer.AnswerWrongDetach = () =>
                                {
                                    UI_NPC.eventEmitter.Trigger<Dictionary<int, DialogueChooseAnswer>, int, bool>(UI_NPC.EEvents.OnClickDialogueChooseFunction, dialogueChooseAnswers2, dialogueChooseAnswer.Index, true);
                                };
                                dialogueChooseAnswer.AnswerWrongUnDetach = () =>
                                {
                                    UI_NPC.eventEmitter.Trigger<Dictionary<int, DialogueChooseAnswer>, int, bool>(UI_NPC.EEvents.OnClickDialogueChooseFunction, dialogueChooseAnswers2, dialogueChooseAnswer.Index, false);
                                };
                            }
                        }

                        eventEmitter.Trigger<Dictionary<int, DialogueChooseAnswer>>((EEvents)Types[index], dialogueChooseAnswers2);
                    }
                }
                else if (Types[index] == (uint)EEvents.ReceiveTask)
                {
                    if (index == 0)
                    {
                        eventEmitter.Trigger<ReceiveTaskAnswerEvt>((EEvents)Types[index], new ReceiveTaskAnswerEvt()
                        {
                            dialogueChooseID = DialogueChooseID,
                            index = Index,
                            handlerID = HandlerID,
                            handlerIndex = HandlerIndex,
                            value = Values[index],
                            isCloseDialogue = closeDialogue,
                        });
                    }
                }
                else if (Types[index] == (uint)EEvents.CompleteTarget)
                {
                    if (index == 0)
                    {
                        eventEmitter.Trigger<CompleteTargetAnswerEvt>((EEvents)Types[index], new CompleteTargetAnswerEvt()
                        {
                            dialogueChooseID = DialogueParentID,
                            index = Index,
                            handlerID = HandlerID,
                            handlerIndex = HandlerIndex,
                            isCloseDialogue = closeDialogue,
                        });
                    }
                }
                else if (Types[index] == (uint)EEvents.Fight || Types[index] == (uint)EEvents.FightWin)
                {
                    eventEmitter.Trigger((EEvents)Types[index]);

                    if (index == 0)
                    {
                        CmdNpcDialogueChooseReq req = new CmdNpcDialogueChooseReq();
                        req.UNpcId = Sys_Interactive.CurInteractiveNPC.uID;
                        req.TaskId = HandlerID;
                        req.TaskIndex = HandlerIndex;
                        req.DialogueOption = (uint)Index;
                        req.DialogueId = DialogueParentID;
                        NetClient.Instance.SendMessage((ushort)CmdNpc.DialogueChooseReq, req);
                    }
                }
                else if (Types[index] == (uint)EEvents.SubmitItem)
                {
                    if (index == 0)
                    {
                        eventEmitter.Trigger<SubmitItemAnswerEvt>((EEvents)Types[index], new SubmitItemAnswerEvt()
                        {
                            dialogueChooseID = DialogueChooseID,
                            index = Index,
                            handlerID = HandlerID,
                            handlerIndex = HandlerIndex,
                            value = Values[index],
                            isCloseDialogue = closeDialogue,
                            npcuid = Sys_Interactive.CurInteractiveNPC.uID,
                        });
                    }
                }
                else if (Types[index] == (uint)EEvents.GetItem)
                {
                    eventEmitter.Trigger((EEvents)Types[index]);
                }
                else if (Types[index] == (uint)EEvents.NpcFollow)
                {
                    eventEmitter.Trigger((EEvents)Types[index]);

                    if (Sys_NpcFollow.Instance.NpcFollowFlag)
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(13040));
                        return;
                    }

                    if (Sys_Team.Instance.HaveTeam)
                    {
                        if (Sys_Team.Instance.isCaptain())
                        {
                            CmdTeamSyncFollowStart cmdTeamSyncFollowStart = new CmdTeamSyncFollowStart();
                            cmdTeamSyncFollowStart.TaskId = HandlerID;
                            cmdTeamSyncFollowStart.Index = HandlerIndex;

                            NetClient.Instance.SendMessage((ushort)CmdTeam.SyncFollowStart, cmdTeamSyncFollowStart);
                        }
                        else
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(13041));
                        }
                    }
                    else
                    {
                        Sys_NpcFollow.Instance.StartNpcFollow(HandlerID, HandlerIndex);
                    }
                }
                else if (Types[index] == (uint)EEvents.NpcTrack)
                {
                    eventEmitter.Trigger((EEvents)Types[index]);

                    if (Sys_Track.Instance.TrackFlag)
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(13042));
                        return;
                    }

                    if (Sys_Team.Instance.HaveTeam)
                    {
                        if (Sys_Team.Instance.isCaptain())
                        {
                            CmdTeamSyncTrackStart cmdTeamSyncTrackStart = new CmdTeamSyncTrackStart();
                            cmdTeamSyncTrackStart.TaskId = HandlerID;
                            cmdTeamSyncTrackStart.Index = HandlerIndex;

                            NetClient.Instance.SendMessage((ushort)CmdTeam.SyncTrackStart, cmdTeamSyncTrackStart);
                        }
                        else
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(13043));
                        }
                    }
                    else
                    {
                        Sys_Track.Instance.StartTrack(HandlerID, HandlerIndex);
                    }
                }
                else if (Types[index] == (uint)EEvents.Escort)
                {
                    eventEmitter.Trigger((EEvents)Types[index]);

                    if (Sys_Escort.Instance.EscortFlag)
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(13044));
                        return;
                    }

                    if (Sys_Team.Instance.HaveTeam)
                    {
                        if (Sys_Team.Instance.isCaptain())
                        {                            
                            CmdTeamSyncConvoyStart cmdTeamSyncConvoyStart = new CmdTeamSyncConvoyStart();
                            cmdTeamSyncConvoyStart.TaskId = HandlerID;
                            cmdTeamSyncConvoyStart.Index = HandlerIndex;

                            NetClient.Instance.SendMessage((ushort)CmdTeam.SyncConvoyStart, cmdTeamSyncConvoyStart);
                        }
                        else
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(13045));
                        }
                    }
                    else
                    {
                        Sys_Escort.Instance.StartEscort(HandlerID, HandlerIndex);
                    }
                }
                else if (Types[index] == (uint)EEvents.OpenTeamUI)
                {
                    GameMain.Procedure.TriggerEvent(this, (int)EProcedureEvent.EnterNormal);

                    if (Sys_Team.Instance.IsFastOpen(true))
                        Sys_Team.Instance.OpenFastUI(Values[index]);
                }
                else if (Types[index] == (uint)EEvents.SecretMessage)
                {
                    CSVCode.Data cSVCodeData = CSVCode.Instance.GetConfData(Values[index]);
                    if (cSVCodeData != null)
                    {
                        Sys_SecretMessage.Instance.OpenSecretMessage(cSVCodeData);
                    }
                }
                else if (Types[index] == (uint)EEvents.StartTimeTask)
                {
                    Sys_Task.Instance.ReqStartTimeLimit(HandlerID, HandlerIndex);
                    eventEmitter.Trigger((EEvents)Types[index]);
                }
                else
                {
                    eventEmitter.Trigger<int, uint, bool>((EEvents)Types[index], Index, Values[index], closeDialogue);
                }
            }
        }
    }
}
