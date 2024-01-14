using System.Collections.Generic;
using Logic.Core;
using Table;
using Lib.Core;
using System;

namespace Logic
{
    /// <summary>
    /// 功能基类///
    /// </summary>
    public abstract class FunctionBase
    {
        /// <summary>
        /// 功能类型///
        /// </summary>
        public EFunctionType Type
        {
            get;
            set;
        } = EFunctionType.None;

        /// <summary>
        /// 对应功能副表ID///
        /// </summary>
        public uint ID
        {
            get;
            set;
        }

        /// <summary>
        /// UI描述ID///
        /// </summary>
        public uint Desc
        {
            get;
            set;
        }

        /// <summary>
        /// 前置对话功能ID///
        /// </summary>
        public uint DialogueID
        {
            get;
            set;
        }

        public EDialogueType EDialogueType
        {
            get;
            set;
        } = EDialogueType.Dialogue;

        /// <summary>
        /// 前置表演动画ID///
        /// </summary>
        public uint PreAnimID
        {
            get;
            set;
        }

        /// <summary>
        /// 条件列表ID//
        /// 此部分条件控制功能是否激活，即是否会在NPC上显示出来///
        /// </summary>
        public List<uint> ConditionIDs;

        /// <summary>
        /// 条件列表参数///
        /// </summary>
        public Dictionary<uint, List<int>> ConditionValues;

        /// <summary>
        /// 条件组表ID///
        /// 此部分条件控制功能是否激活，即是否会在NPC上显示出来///
        /// </summary>
        public uint ConditionGroupID
        {
            get;
            set;
        }

        /// <summary>
        /// 持有者ID(任务或其它)///
        /// </summary>
        public uint HandlerID
        {
            get;
            set;
        }

        /// <summary>
        /// 任务目标下标///
        /// </summary>
        public uint HandlerIndex
        {
            get;
            set;
        }

        /// <summary>
        /// 持有的任务目标ID///
        /// </summary>
        public uint HandlerTaskTargetID
        {
            get
            {
                return HandlerID * 10 + HandlerIndex + 1;
            }
        }

        /// <summary>
        /// 功能的来源///
        /// </summary>
        public EFunctionSourceType FunctionSourceType
        {
            get;
            set;
        } = EFunctionSourceType.None;

        public enum ESpecialType
        {
            Common,
            TimeLimit,
        }

        public ESpecialType SpecialType
        {
            get;
            set;
        }

        public uint OpenList
        {
            get;
            set;
        }

        /// <summary>
        /// 功能持有者///
        /// </summary>
        public Npc npc
        {
            get;
            set;
        }

        public enum ECtrlType
        {
            PlayCtrl,
            Auto,
            NetSync,
        }

        public ECtrlType CtrlType
        {
            get;
            private set;
        } = ECtrlType.PlayCtrl;

        /// <summary>
        /// 初始化功能数据///
        /// </summary>
        public virtual void Init()
        {
        }

        public virtual void InitCompleted()
        {

        }

        public void Dispose()
        {
            OnDispose();
            PoolManager.Recycle(this);
        }

        protected virtual void OnDispose()
        {
            Type = EFunctionType.None;
            ID = 0;
            Desc = 0;
            DialogueID = 0;
            PreAnimID = 0;
            ConditionIDs?.Clear();
            ConditionIDs = null;

            ConditionValues?.Clear();
            ConditionValues = null;
            ConditionGroupID = 0;
            HandlerID = 0;
            HandlerIndex = 0;
            FunctionSourceType = EFunctionSourceType.None;
            SpecialType = ESpecialType.Common;
            OpenList = 0;
            CtrlType = ECtrlType.PlayCtrl;

            npc = null;

            debugTimer?.Cancel();
            debugTimer = null;
        }

        /// <summary>
        /// 执行功能///
        /// </summary>
        public void Execute(ECtrlType ctrlType)
        {
            CtrlType = ctrlType;

            if (!CanExecute())
            {
                OnCantExecTip();
                return;
            }

            if (FunctionSourceType == EFunctionSourceType.Task && CSVTask.Instance.GetConfData(HandlerID).TaskSkipPlot == 1 && Sys_Task.Instance.GetSkipState(CSVTask.Instance.GetConfData(HandlerID).TaskSkipPlotID))
            {
                OnExecute();
            }
            else
            {
                ///前置动画(可为空) -> 前置对话(可为空) ->(具体功能执行)///
                if (PreAnimID == 0)
                {
                    if (DialogueID == 0)
                    {
                        OnExecute();
                    }
                    else
                    {
                        ExecutePreDialogue();
                    }
                }
                else
                {
                    DebugUtil.Log(ELogType.eNPC, $"EnterInteractive PreAnim, AnimID: {PreAnimID}");
                    Sys_Interactive.Instance.CurPreAnimID = PreAnimID;
                    ///进入了前置动画表现，强制流程进入交互模式///
                    GameMain.Procedure.TriggerEvent(this, (int)EProcedureEvent.EnterInteractive);

                    try
                    {
                        WS_NPCManagerEntity.StartNPC<WS_NPCControllerEntity>(PreAnimID, 0, SwitchWorkStreamEnum.Stop_AllWorkStream, null, WS_NPCControllerEntityOverCallBack, true, (int)NPCEnum.B_StartInteractive);
                        debugTimer?.Cancel();
                        debugTimer = Timer.Register(4f, () => 
                        {
                            VirtualShowManager.Instance.ClearVirtualSceneActors();
                            if (DialogueID == 0)
                            {
                                OnExecute();
                            }
                            else
                            {
                                ExecutePreDialogue();
                            }
                        });
                    }
                    catch (Exception e)
                    {
                        WS_NPCControllerEntityOverCallBack();
                    }
                }
            }
        }

        Timer debugTimer;

        void WS_NPCControllerEntityOverCallBack()
        {
            debugTimer?.Cancel();
            debugTimer = null;
            if (DialogueID == 0)
            {
                OnExecute();
            }
            else
            {
                ExecutePreDialogue();
            }
        }

        /// <summary>
        /// 前置对话///
        /// </summary>
        void ExecutePreDialogue()
        {
            if (EDialogueType == EDialogueType.Dialogue)
            {
                CSVDialogue.Data cSVDialogueData = CSVDialogue.Instance.GetConfData(DialogueID);
                if (cSVDialogueData != null)
                {
                    List<Sys_Dialogue.DialogueDataWrap> datas = Sys_Dialogue.GetDialogueDataWraps(cSVDialogueData);

                    ResetDialogueDataEventData resetDialogueDataEventData = PoolManager.Fetch(typeof(ResetDialogueDataEventData)) as ResetDialogueDataEventData;
                    resetDialogueDataEventData.Init(datas, DialogueCallBack, cSVDialogueData);
                    SetResetDialogueDataEventData(resetDialogueDataEventData);
                    Sys_Dialogue.Instance.OpenDialogue(resetDialogueDataEventData);

                    //NPC前置对话打点
                    if (npc != null && npc.cSVNpcData != null)
                        Sys_Dialogue.Instance.HitPointDialog(npc.cSVNpcData.id);
                }
                else
                {
                    DebugUtil.LogError($"DialogueID is null id:{DialogueID}");
                }
            }
            else if (EDialogueType == EDialogueType.MenuDialogue)
            {
                CSVInterfaceBubble.Data cSVInterfaceBubbleData = CSVInterfaceBubble.Instance.GetConfData(DialogueID);
                if (cSVInterfaceBubbleData != null)
                {
                    List<Sys_MenuDialogue.MenuDialogueDataWrap> datas = Sys_MenuDialogue.GetMenuDialogueDataWrap(cSVInterfaceBubbleData);
                    Sys_MenuDialogue.ResetMenuDialogueDataEventData resetMenuDialogueDataEventData = PoolManager.Fetch(typeof(Sys_MenuDialogue.ResetMenuDialogueDataEventData)) as Sys_MenuDialogue.ResetMenuDialogueDataEventData;
                    resetMenuDialogueDataEventData.Init(datas);
                    Sys_MenuDialogue.Instance.OpenMenuDialogue(resetMenuDialogueDataEventData);

                    DialogueCallBack();
                }
                else
                {
                    DebugUtil.LogError($"CSVInterfaceBubble.Data is null id:{DialogueID}");
                }
            }
        }

        void DialogueCallBack()
        {
            OnExecute();
        }

        /// <summary>
        /// 功能是否可执行///
        /// 相应的条件由各个功能的模块及相应功能副表的条件组ID提供///
        /// </summary>
        /// <returns></returns>
        protected virtual bool CanExecute(bool CheckVisual = true)
        {
            return true;
        }

        /// <summary>
        /// 功能不能执行时发出的提示///
        /// </summary>
        protected virtual void OnCantExecTip()
        {

        }

        protected virtual void OnExecute()
        {
            ///功能开始执行，强制流程进入正常模式///
            GameMain.Procedure.TriggerEvent(this, (int)EProcedureEvent.EnterNormal);
        }

        protected virtual void SetResetDialogueDataEventData(ResetDialogueDataEventData resetDialogueDataEventData)
        {

        }

        /// <summary>
        /// 反序列化拓展///
        /// </summary>
        /// <param name="ext"></param>
        public virtual void DeserializeObjectExt(List<uint> ext)
        {

        }

        /// <summary>
        /// 反序列化参数///
        /// NPC表里常态的功能///
        /// </summary>
        /// <param name="strs"></param>
        public virtual void DeserializeObject(List<uint> strs, bool TaskCreate = false)
        {
            Type = (EFunctionType)strs[0];
            int len = strs.Count;
            if (len > 1)
            {
                ID = strs[1];
                if (len > 2)
                {
                    Desc = strs[2];
                    if (len > 3)
                    {
                        DialogueID =strs[3];

                        if (CSVDialogue.Instance.ContainsKey(DialogueID))
                        {
                            EDialogueType = EDialogueType.Dialogue;
                        }
                        else if (CSVInterfaceBubble.Instance.ContainsKey(DialogueID))
                        {
                            EDialogueType = EDialogueType.MenuDialogue;
                        }

                        if (len > 4)
                        {
                            PreAnimID = strs[4];
                            if (len > 5)
                            {
                                HandlerID = strs[5];
                                if (len > 6)
                                {
                                    if (TaskCreate)
                                    {
                                        HandlerIndex = strs[6] - 1;
                                    }
                                    else
                                    {
                                        HandlerIndex = strs[6];
                                    }
                                    if (len > 7)
                                    {
                                        FunctionSourceType = (EFunctionSourceType)strs[7];
                                        if (len > 8)
                                        {
                                            OpenList = strs[8];
                                            if (len > 9)
                                            {
                                                SpecialType = (ESpecialType)strs[9];
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }                
        }

        /// <summary>
        /// 生成功能是否激活(即是否在NPC身上显示出来的条件)///
        /// 这里都是任务目标产生的功能的条件///
        /// 常态功能的条件和条件数据已从NPC表中反序列化了///
        /// </summary>
        public virtual void CreateConditions()
        {
            ConditionIDs = new List<uint>();
            ConditionIDs.Add((uint)EConditionType.TaskUnCompleted);
            ConditionIDs.Add((uint)EConditionType.TaskTargetUnCompleted);          

            ConditionValues = new Dictionary<uint, List<int>>();
            List<int> values1 = new List<int>();
            values1.Add((int)HandlerID);
            ConditionValues.Add(ConditionIDs[0], values1);

            List<int> values2 = new List<int>();
            values2.Add((int)HandlerID);
            values2.Add((int)HandlerIndex + 1);
            ConditionValues.Add(ConditionIDs[1], values2);

            CSVTask.Data cSVTaskData = CSVTask.Instance.GetConfData(HandlerID);
            if (cSVTaskData != null && !cSVTaskData.taskGoalExecDependency && HandlerIndex != 0)
            {
                ConditionIDs.Add((uint)EConditionType.TaskTargetCompleted);
                List<int> values3 = new List<int>();
                values3.Add((int)HandlerID);
                values3.Add((int)HandlerIndex);
                ConditionValues.Add(ConditionIDs[2], values3);

                if (FunctionSourceType == EFunctionSourceType.Task && CSVTaskGoal.Instance.GetConfData(HandlerTaskTargetID).LimitTime != 0)
                {
                    if (SpecialType == ESpecialType.TimeLimit)
                    {
                        ConditionIDs.Add((uint)EConditionType.TimeLimitTaskGoalOff);
                    }
                    else
                    {
                        ConditionIDs.Add((uint)EConditionType.TimeLimitTaskGoalOn);
                    }

                    List<int> values4 = new List<int>();
                    values4.Add((int)HandlerID);
                    values4.Add((int)HandlerIndex + 1);
                    ConditionValues.Add(ConditionIDs[3], values4);
                }
            }
            else
            {
                if (FunctionSourceType == EFunctionSourceType.Task && CSVTaskGoal.Instance.GetConfData(HandlerTaskTargetID).LimitTime != 0)
                {
                    if (SpecialType == ESpecialType.TimeLimit)
                    {
                        ConditionIDs.Add((uint)EConditionType.TimeLimitTaskGoalOff);
                    }
                    else
                    {
                        ConditionIDs.Add((uint)EConditionType.TimeLimitTaskGoalOn);
                    }

                    List<int> values4 = new List<int>();
                    values4.Add((int)HandlerID);
                    values4.Add((int)HandlerIndex + 1);
                    ConditionValues.Add(ConditionIDs[2], values4);
                }
            }


        }

        /// <summary>
        /// 功能是否可用(即是否在NPC身上显示出来)///
        /// </summary>
        /// <returns></returns>
        public virtual bool IsValid()
        {
            ///先判断条件组///
            if (ConditionGroupID != 0)
            {
                var cSVCheckseq = CSVCheckseq.Instance.GetConfData(ConditionGroupID);
                if (cSVCheckseq != null && !cSVCheckseq.IsValid())
                {
                    return false;
                }
            }

            ///再判断单个的条件///
            if (ConditionIDs == null || ConditionIDs.Count == 0)
            {
                return true;
            }

            for (int index = 0, len = ConditionIDs.Count; index < len; index++)
            {
                ConditionBase conditionBase = ConditionManager.CreateCondition((EConditionType)ConditionIDs[index]);

                if (conditionBase != null)
                {
                    conditionBase.DeserializeObject(ConditionValues[ConditionIDs[index]]);
                    if (!conditionBase.IsValid())
                    {
                        conditionBase.Dispose();
                        return false;
                    }
                    else
                    {
                        conditionBase.Dispose();
                    }
                }
                else
                {
                    DebugUtil.LogError($"ERROR!!! new() Condition failed, ConditionType:{((EConditionType)ConditionIDs[index]).ToString()}");
                }
            }

            return true;
        }
    }
}
