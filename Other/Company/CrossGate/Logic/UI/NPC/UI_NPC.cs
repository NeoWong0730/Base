using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Lib.Core;
using Table;
using System;
using Net;
using Packet;

namespace Logic
{
    public class UI_NPC : UIBase
    {
        GameObject functionPrefabObj;
        GameObject answerPrefabObj;
        CSVNpc.Data cSVNpcData;
        ScrollRect functionListView;
        GameObject functionsRoot;
        ScrollRect dialogueAnswerListView;
        GameObject answersRoot;
        Image bgGameObject;
        Button closeBtn;

        public static EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        Action OnLoadedCallbackToDialogue;

        public enum EEvents
        {
            OnClickGroupFunction,
            OnClickDialogueChoose,
            OnClickDialogueChooseFunction,
        }

        protected override void OnLoaded()
        {            
            functionPrefabObj = gameObject.FindChildByName("FunctionItemPrefab");
            answerPrefabObj = gameObject.FindChildByName("AnswerItemPrefab");
            functionListView = gameObject.FindChildByName("FunctionListRoot").GetComponent<ScrollRect>();
            functionsRoot = functionListView.gameObject.FindChildByName("Content");
            dialogueAnswerListView = gameObject.FindChildByName("DialogueAnswerListRoot").GetComponent<ScrollRect>();
            answersRoot = dialogueAnswerListView.gameObject.FindChildByName("Content");
            bgGameObject = gameObject.FindChildByName("Image_BG").GetComponent<Image>();
            closeBtn = bgGameObject.GetComponent<Button>();
            closeBtn.onClick.AddListener(OnClickCloseBtn);
        }

        protected override void ProcessEvents(bool toRegister)
        {
            eventEmitter.Handle<List<FunctionBase>>(EEvents.OnClickGroupFunction, OnClickGroupFunction, toRegister);
            eventEmitter.Handle(EEvents.OnClickDialogueChoose, OnClickDialogueChoose, toRegister);
            eventEmitter.Handle<Dictionary<int, DialogueChooseAnswer>, int, bool>(EEvents.OnClickDialogueChooseFunction, OnClickDialogueChooseFunction, toRegister);
            DialogueChooseAnswer.eventEmitter.Handle<bool, bool>(DialogueChooseAnswer.EEvents.None, OnChooseAnswerNone, toRegister);
            DialogueChooseAnswer.eventEmitter.Handle<Dictionary<int, DialogueChooseAnswer>>(DialogueChooseAnswer.EEvents.NextStep, OnChooseAnswerNextStep, toRegister);
            DialogueChooseAnswer.eventEmitter.Handle<Dictionary<int, DialogueChooseAnswer>>(DialogueChooseAnswer.EEvents.GoBack, OnChooseAnswerGoBack, toRegister);
            DialogueChooseAnswer.eventEmitter.Handle<DialogueChooseAnswer.ReceiveTaskAnswerEvt>(DialogueChooseAnswer.EEvents.ReceiveTask, OnChooseAnswerReceiveTask, toRegister);
            DialogueChooseAnswer.eventEmitter.Handle<DialogueChooseAnswer.CompleteTargetAnswerEvt>(DialogueChooseAnswer.EEvents.CompleteTarget, OnChooseAnswerCompleteTarget, toRegister);
            DialogueChooseAnswer.eventEmitter.Handle(DialogueChooseAnswer.EEvents.Fight, OnChooseFight, toRegister);
            DialogueChooseAnswer.eventEmitter.Handle(DialogueChooseAnswer.EEvents.FightWin, OnChooseFight, toRegister);
            DialogueChooseAnswer.eventEmitter.Handle(DialogueChooseAnswer.EEvents.GetItem, OnGetItem, toRegister);
            DialogueChooseAnswer.eventEmitter.Handle<DialogueChooseAnswer.SubmitItemAnswerEvt>(DialogueChooseAnswer.EEvents.SubmitItem, OnChooseSubmitItem, toRegister);
            DialogueChooseAnswer.eventEmitter.Handle(DialogueChooseAnswer.EEvents.ChooseAnswer, ChooseAnswer, toRegister);
            DialogueChooseAnswer.eventEmitter.Handle(DialogueChooseAnswer.EEvents.Escort, ChooseEscort, toRegister);
            DialogueChooseAnswer.eventEmitter.Handle(DialogueChooseAnswer.EEvents.NpcFollow, ChooseNpcFollow, toRegister);
            DialogueChooseAnswer.eventEmitter.Handle(DialogueChooseAnswer.EEvents.NpcTrack, ChooseNpcTrack, toRegister);
            DialogueChooseAnswer.eventEmitter.Handle(DialogueChooseAnswer.EEvents.StartTimeTask, ChooseStartTimeTask, toRegister);
            if (toRegister)
            {
                EventDispatcher.Instance.AddEventListener(0, (ushort)CmdTeam.SyncTalkChoice, OnTeamSyncTalkChoice, CmdTeamSyncTalkChoice.Parser);
            }
            else
            {
                EventDispatcher.Instance.RemoveEventListener((ushort)CmdTeam.SyncTalkChoice, OnTeamSyncTalkChoice);
            }
        }

        void OnTeamSyncTalkChoice(NetMsg netMsg)
        {
            CmdTeamSyncTalkChoice cmdTeamSyncTalkChoice = NetMsgUtil.Deserialize<CmdTeamSyncTalkChoice>(CmdTeamSyncTalkChoice.Parser, netMsg);
            if (cmdTeamSyncTalkChoice != null)
            {
                if (uI_Item_Answers != null && uI_Item_Answers.ContainsKey((int)cmdTeamSyncTalkChoice.Choice))
                {
                    if (Sys_Team.Instance.isCaptain() && Sys_Team.Instance.TeamMemsCount > 1)
                    {
                        uI_Item_Answers[(int)cmdTeamSyncTalkChoice.Choice].ExecuteClickButton();
                    }
                    else
                    {
                        uI_Item_Answers[(int)cmdTeamSyncTalkChoice.Choice].button.onClick.Invoke();
                    }
                }
            }
        }

        protected override void OnOpen(object arg)
        {            
            cSVNpcData = ((InteractiveData)arg).CSVNpcData;
            hideFunctionList = false;
            OnLoadedCallbackToDialogue = ((InteractiveData)arg).OnloadCallBack;
        }

        protected override void OnShow()
        {            
            ClearFunctionList();

            if (Sys_Interactive.CurInteractiveNPC != null)
            {
                if (!hideFunctionList)
                {
                    List<FunctionBase> filteredFunctions = Sys_Interactive.CurInteractiveNPC.NPCFunctionComponent.FilterFunctions();
                    Refresh(filteredFunctions);
                }
            }
            else
            {
                if (cSVNpcData != null && !hideFunctionList)
                {
                    Npc npc;
                    if (GameCenter.uniqueNpcs.TryGetValue(cSVNpcData.id, out npc))
                    {
                        List<FunctionBase> filteredFunctions = npc.NPCFunctionComponent.FilterFunctions();
                        Refresh(filteredFunctions);
                    }
                    else
                    {
                        DebugUtil.LogError($"当前场景中不存在这个npc, id:{cSVNpcData.id}");
                    }
                }
            }

            OnLoadedCallbackToDialogue?.Invoke();
            OnLoadedCallbackToDialogue = null;
            hideFunctionList = false;
        }

        protected override void OnHide()
        {
            ClearDialogueAnswerList();            
        }

        void ClearFunctionList()
        {
            functionsRoot.DestoryAllChildren();
        }

        void ClearDialogueAnswerList()
        {
            answersRoot.DestoryAllChildren();
        }

        void OnClickGroupFunction(List<FunctionBase> functions)
        {
            ClearFunctionList();
            Refresh(functions);
        }

        bool hideFunctionList;
        public void OnClickDialogueChooseFunction(Dictionary<int, DialogueChooseAnswer> dialogueChooseAnswers, int index, bool detach)
        {
            hideFunctionList = true;
            Show();
            
            ClearDialogueAnswerList();
            
            if (detach)
            {
                dialogueChooseAnswers.Remove(index);
            }
            RefreshAnawerList(dialogueChooseAnswers);
        }

        void OnChooseAnswerNone(bool reChoose, bool closeDialogue)
        {
            if (!reChoose)
                CloseSelf();

            if (closeDialogue)
            {
                GameMain.Procedure.TriggerEvent(this, (int)EProcedureEvent.EnterNormal);
            }
        }

        void OnChooseFight()
        {
            GameMain.Procedure.TriggerEvent(this, (int)EProcedureEvent.EnterNormal);
        }

        void OnGetItem()
        {
            CloseSelf();
            GameMain.Procedure.TriggerEvent(this, (int)EProcedureEvent.EnterNormal);
        }

        void OnChooseAnswerNextStep(Dictionary<int, DialogueChooseAnswer> dialogueChooseAnswers)
        {
            ClearDialogueAnswerList();
            RefreshAnawerList(dialogueChooseAnswers);
        }

        void OnChooseAnswerGoBack(Dictionary<int, DialogueChooseAnswer> dialogueChooseAnswers)
        {
            ClearDialogueAnswerList();
            RefreshAnawerList(dialogueChooseAnswers);
        }

        void OnChooseAnswerReceiveTask(DialogueChooseAnswer.ReceiveTaskAnswerEvt evt)
        {
            CloseSelf();

            if (evt.isCloseDialogue)
            {
                GameMain.Procedure.TriggerEvent(this, (int)EProcedureEvent.EnterNormal);
            }
        }

        void OnChooseSubmitItem(DialogueChooseAnswer.SubmitItemAnswerEvt evt)
        {
            CloseSelf();

            if (evt.isCloseDialogue)
            {
                GameMain.Procedure.TriggerEvent(this, (int)EProcedureEvent.EnterNormal);
            }
        }

        void ChooseEscort()
        {
            CloseSelf();
            GameMain.Procedure.TriggerEvent(this, (int)EProcedureEvent.EnterNormal);
        }

        void ChooseNpcFollow()
        {
            CloseSelf();
            GameMain.Procedure.TriggerEvent(this, (int)EProcedureEvent.EnterNormal);
        }

        void ChooseNpcTrack()
        {
            CloseSelf();
            GameMain.Procedure.TriggerEvent(this, (int)EProcedureEvent.EnterNormal);
        }

        void ChooseStartTimeTask()
        {
            CloseSelf();
            GameMain.Procedure.TriggerEvent(this, (int)EProcedureEvent.EnterNormal);
        }

        void OnChooseAnswerCompleteTarget(DialogueChooseAnswer.CompleteTargetAnswerEvt evt)
        {
            CloseSelf();

            if (evt.isCloseDialogue)
            {
                GameMain.Procedure.TriggerEvent(this, (int)EProcedureEvent.EnterNormal);
            }
        }

        void ChooseAnswer()
        {
            bgGameObject.raycastTarget = false;
            dialogueAnswerListView.gameObject.SetActive(false);
        }

        void OnClickDialogueChoose()
        {
            functionListView.gameObject.SetActive(false);
        }

        Dictionary<int, UI_Item_Answer> uI_Item_Answers = new Dictionary<int, UI_Item_Answer>();

        void RefreshAnawerList(Dictionary<int, DialogueChooseAnswer> dialogueChooseAnswers)
        {
            uI_Item_Answers.Clear();

            if (dialogueChooseAnswers != null)
            {
                functionListView.gameObject.SetActive(false);
                dialogueAnswerListView.gameObject.SetActive(true);

                foreach (DialogueChooseAnswer answer in dialogueChooseAnswers.Values)
                {
                    if (answer != null)
                    {
                        GameObject answerObj = GameObject.Instantiate(answerPrefabObj);
                        answerObj.name = $"answer_{answer.Index}";
                        answerObj.transform.SetParent(answersRoot.transform, false);
                        answerObj.SetActive(true);

                        UI_Item_Answer uI_Item_Answer = new UI_Item_Answer();
                        uI_Item_Answer.Initialize(answerObj, answer);

                        uI_Item_Answers.Add(answer.Index, uI_Item_Answer);
                    }
                }
            }
        }

        int functionNum;
        UI_Item_Function firstFunction;
        void OnClickCloseBtn()
        {
            if (functionNum == 1)
            {
                firstFunction?.OnClickButton();
            }
        }

        void Refresh(List<FunctionBase> functions)
        {
            functionNum = 0;
            firstFunction = null;
            if (cSVNpcData != null && functions != null)
            {
                functionNum = functions.Count;
                functionListView.gameObject.SetActive(true);
                for (int index = 0, len = functions.Count; index < len; index++)
                {
                    GameObject functionObj = GameObject.Instantiate(functionPrefabObj);
                    functionObj.name = $"function_{functions[index].Type}_{functions[index].ID}";
                    functionObj.transform.SetParent(functionsRoot.transform, false);
                    functionObj.SetActive(true);

                    UI_Item_Function uI_Item_Function = new UI_Item_Function();
                    uI_Item_Function.Initialize(functionObj, functions[index]);

                    //if (functions[index].Type == EFunctionType.OpenTeam && functions[index].npc.cSVNpcData.id == 11004001)
                    //{
                    //    Sys_Guide.Instance.TriggerGuideGroup(810010);
                    //}

                    if (index == 0)
                    {
                        firstFunction = uI_Item_Function;
                    }
                }

                GameObject backObj = GameObject.Instantiate(functionPrefabObj);
                backObj.name = $"backButton";
                backObj.transform.SetParent(functionsRoot.transform, false);
                backObj.SetActive(true);

                UI_Item_BackButton uI_Item_BackButton = new UI_Item_BackButton();
                uI_Item_BackButton.Initialize(backObj, null);
            }
        }
    }

    public class UI_Item_BackButton : UI_Item_Function
    {
        protected override void Refresh()
        {
            ImageHelper.SetIcon(icon, 990223);
            ImageHelper.SetIcon(iconChoice, 990223);

            skillRoot.SetActive(false);
        }

        public override void OnClickButton()
        {
            GameMain.Procedure.TriggerEvent(this, (int)EProcedureEvent.EnterNormal);
            UIManager.CloseUI(EUIID.UI_NPC);
            UIManager.CloseUI(EUIID.UI_Dialogue);
        }
    }

    public class UI_Item_Answer
    {
        GameObject root;

        Text text;
        Text textChoice;
        public ButtonPlus button;
        GameObject normalRoot;
        GameObject choiceRoot;
        DialogueChooseAnswer chooseAnswer;

        public void Initialize(GameObject go, DialogueChooseAnswer _chooseAnswer)
        {
            root = go;
            chooseAnswer = _chooseAnswer;

            normalRoot = root.FindChildByName("NormalRoot");
            choiceRoot = root.FindChildByName("ChoiceRoot");

            text = normalRoot.FindChildByName("Text").GetComponent<Text>();
            textChoice = choiceRoot.FindChildByName("Text_Choice").GetComponent<Text>();

            button = root.FindChildByName("ButtonRoot").GetComponent<ButtonPlus>();
            button.onClick.AddListener(OnClickButton);
            ButtonCtrl buttonCtrl = button.gameObject.GetNeedComponent<ButtonCtrl>();
            buttonCtrl.cd = 1f;
            button.HighlightedAction = () =>
            {
                normalRoot.SetActive(false);
                choiceRoot.SetActive(true);
            };

            button.NormalAction = () =>
            {
                normalRoot.SetActive(true);
                choiceRoot.SetActive(false);
            };

            Refresh();
        }

        void Refresh()
        {
            TextHelper.SetText(text, chooseAnswer.LanguageID);
            TextHelper.SetText(textChoice, chooseAnswer.LanguageID);
        }

        void OnClickButton()
        {
            if (Sys_Team.Instance.isCaptain() && Sys_Team.Instance.TeamMemsCount > 1)
            {
                CmdTeamSyncTalkChoice cmdTeamSyncTalkChoice = new CmdTeamSyncTalkChoice();
                cmdTeamSyncTalkChoice.Choice = (uint)chooseAnswer.Index;

                NetClient.Instance.SendMessage((ushort)CmdTeam.SyncTalkChoice, cmdTeamSyncTalkChoice);
            }
            else
            {
                ExecuteClickButton();
            }
        }

        public void ExecuteClickButton()
        {
            chooseAnswer.Choose();
        }
    }

    public class UI_Item_Function
    {
        GameObject root;
        protected Image icon;
        protected Image iconChoice;
        Text text;
        Text textChoice;
        ButtonPlus button;
        GameObject normalRoot;
        GameObject choiceRoot;
        FunctionBase function;

        GameObject normalBgRoot;
        GameObject choiceBgRoot;

        protected GameObject skillRoot;
        Text skillCostText;

        public void Initialize(GameObject go, FunctionBase _function)
        {
            root = go;
            function = _function;
            normalRoot = root.FindChildByName("NormalRoot");
            choiceRoot = root.FindChildByName("ChoiceRoot");

            icon = normalRoot.FindChildByName("Icon").GetComponent<Image>();
            iconChoice = choiceRoot.FindChildByName("Icon_Choice").GetComponent<Image>();
            text = normalRoot.FindChildByName("Text").GetComponent<Text>();
            textChoice = choiceRoot.FindChildByName("Text_Choice").GetComponent<Text>();

            button = root.FindChildByName("ButtonRoot").GetComponent<ButtonPlus>();
            button.onClick.AddListener(OnClickButton);

            normalBgRoot = normalRoot.FindChildByName("BgRoot");
            choiceBgRoot = choiceRoot.FindChildByName("BgRoot");

            skillRoot = root.FindChildByName("SkillRoot");
            skillCostText = skillRoot.FindChildByName("Text_Num").GetComponent<Text>();

            button.HighlightedAction = () =>
            {
                normalRoot.SetActive(false);
                choiceRoot.SetActive(true);
            };

            button.NormalAction = () =>
            {
                normalRoot.SetActive(true);
                choiceRoot.SetActive(false);
            };

            Refresh();
        }

        protected virtual void Refresh()
        {
            if (function.Type == EFunctionType.LearnActiveSkill)
            {
                skillRoot.SetActive(true);
                LearnActiveSkillFunction learnActiveSkillFunction = function as LearnActiveSkillFunction;
                if (learnActiveSkillFunction != null)
                {
                    if (Sys_Bag.Instance.GetItemCount(learnActiveSkillFunction.CSVActiveSkillInfoData.upgrade_cost[0][0]) >= learnActiveSkillFunction.CSVActiveSkillInfoData.upgrade_cost[0][1])
                    {
                        TextHelper.SetText(skillCostText, learnActiveSkillFunction.CSVActiveSkillInfoData.upgrade_cost[0][1].ToString());
                    }
                    else
                    {
                        TextHelper.SetText(skillCostText, learnActiveSkillFunction.CSVActiveSkillInfoData.upgrade_cost[0][1].ToString(), LanguageHelper.GetTextStyle(20));
                    }
                }
            }
            else if (function.Type == EFunctionType.LearnPassiveSkill)
            {
                skillRoot.SetActive(true);
                LearnPassiveSkillFunction learnPassiveSkillFunction = function as LearnPassiveSkillFunction;
                if (learnPassiveSkillFunction != null)
                {
                    if (Sys_Bag.Instance.GetItemCount(learnPassiveSkillFunction.CSVPassiveSkillInfoData.upgrade_cost[0][0]) >= learnPassiveSkillFunction.CSVPassiveSkillInfoData.upgrade_cost[0][1])
                    {
                        TextHelper.SetText(skillCostText, learnPassiveSkillFunction.CSVPassiveSkillInfoData.upgrade_cost[0][1].ToString());
                    }
                    else
                    {
                        TextHelper.SetText(skillCostText, learnPassiveSkillFunction.CSVPassiveSkillInfoData.upgrade_cost[0][1].ToString(), LanguageHelper.GetTextStyle(20));
                    }
                }
            }
            else
            {
                skillRoot.SetActive(false);
            }

            CSVFunction.Data functionData = CSVFunction.Instance.GetConfData((uint)function.Type);
            if (functionData != null)
            {
                ImageHelper.SetIcon(icon, functionData.functionIcon);
                ImageHelper.SetIcon(iconChoice, functionData.functionIcon);
            }

            if (function.FunctionSourceType == EFunctionSourceType.Task)
            {
                TextHelper.SetText(text, LanguageHelper.GetTaskLanguageColorWords(function.Desc));
                TextHelper.SetText(textChoice, LanguageHelper.GetTaskLanguageColorWords(function.Desc));
            }
            else
            {
                if (function.Type == EFunctionType.Prestige)
                {
                    TextHelper.SetText(text, LanguageHelper.GetTextContent(function.Desc));
                    TextHelper.SetText(textChoice, LanguageHelper.GetTextContent(function.Desc));
                }
                else
                {
                    TextHelper.SetText(text, LanguageHelper.GetNpcTextContent(function.Desc));
                    TextHelper.SetText(textChoice, LanguageHelper.GetNpcTextContent(function.Desc));
                }
            }

            CSVFunction.Data cSVFunctionData = CSVFunction.Instance.GetConfData((uint)function.Type);
            if (cSVFunctionData != null)
            {
                for (int index = 0, len = normalBgRoot.transform.childCount; index < len; index++)
                {
                    if (index == cSVFunctionData.TitleColour)
                    {
                        normalBgRoot.transform.GetChild(index).gameObject.SetActive(true);
                    }
                    else
                    {
                        normalBgRoot.transform.GetChild(index).gameObject.SetActive(false);
                    }
                }

                for (int index = 0, len = choiceBgRoot.transform.childCount; index < len; index++)
                {
                    if (index == cSVFunctionData.TitleColour)
                    {
                        choiceBgRoot.transform.GetChild(index).gameObject.SetActive(true);
                    }
                    else
                    {
                        choiceBgRoot.transform.GetChild(index).gameObject.SetActive(false);
                    }
                }
            }
        }

        public virtual void OnClickButton()
        {
            //Debug.LogErrorFormat("OnClickButton = {0}", Time.time);

            if (function.Type != EFunctionType.DialogueChoose)
                UIManager.CloseUI(EUIID.UI_NPC);
            else
                UIManager.CloseUI(EUIID.UI_NPC, true, false);
            if (Sys_Team.Instance.isCaptain() && Sys_Team.Instance.TeamMemsCount > 1)
            {
                if (function.Type == EFunctionType.Dialogue || function.Type == EFunctionType.Task)
                {
                    CmdTeamSyncNpcFunc cmdTeamSyncNpcFunc = new CmdTeamSyncNpcFunc();
                    cmdTeamSyncNpcFunc.NNpcId = Sys_Interactive.CurInteractiveNPC.uID;
                    cmdTeamSyncNpcFunc.FuncId = (uint)function.Type;
                    cmdTeamSyncNpcFunc.ParamId = function.ID;
                    cmdTeamSyncNpcFunc.TaskId = function.HandlerID;

                    NetClient.Instance.SendMessage((ushort)CmdTeam.SyncNpcFunc, cmdTeamSyncNpcFunc);
                }
                else
                {
                    function?.Execute(FunctionBase.ECtrlType.PlayCtrl);
                }
            }
            else
            {
                function?.Execute(FunctionBase.ECtrlType.PlayCtrl);
            }
        }
    }
}
