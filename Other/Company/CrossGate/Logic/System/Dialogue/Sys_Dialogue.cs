using Lib.Core;
using Logic.Core;
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;

namespace Logic
{
    public class ResetDialogueDataEventData
    {
        public List<Sys_Dialogue.DialogueDataWrap> datas;
        public Action callback;
        public bool hideNextButton;
        public bool hideSkipButton;
        public bool autoFlag;
        public bool holdDialogueOpen;
        public bool onlyOnePass;
        public bool secret;
        public uint cSVCodeDataID;

        public void Dispose()
        {
            if (datas != null)
            {
                for (int index = 0, len = datas.Count; index < len; index++)
                {
                    datas[index].Dispose();
                }
                datas.Clear();
            }

            callback = null;
            hideNextButton = false;
            hideSkipButton = false;
            autoFlag = false;
            holdDialogueOpen = false;
            onlyOnePass = false;
            secret = false;
            cSVCodeDataID = 0;

            PoolManager.Recycle(this);
        }

        public void Init(List<Sys_Dialogue.DialogueDataWrap> _datas, Action _callback, CSVDialogue.Data cSVDialogueData)
        {
            datas = _datas;
            callback = _callback;

            if (cSVDialogueData != null)
            {
                if (cSVDialogueData.whetherShowSymbol == 0)
                    hideNextButton = true;
                else
                    hideNextButton = false;

                if (cSVDialogueData.whetherSkipDialogue == 0)
                    hideSkipButton = true;
                else
                    hideSkipButton = false;

                if (cSVDialogueData.whetherAutomaticDialogue == 0)
                    autoFlag = false;
                else
                    autoFlag = true;
            }
            else
            {
                DebugUtil.LogError($"cSVDialogueData is null");
            }
        }
    }

    /// <summary>
    /// 对话系统///
    /// </summary>
    public class Sys_Dialogue : SystemModuleBase<Sys_Dialogue>
    {
        public bool ShowUIActorFlag
        {
            get;
            set;
        } = true;

        public struct DialogueDataWrap
        {
            public uint ActorType;
            public uint CharID;
            public uint ActorNameID;
            public uint ContentID;
            public uint LeftShowActorType;
            public uint LeftShowCharID;
            public uint LeftShowStatus;
            public uint LeftShowAnimID;
            public uint RightShowActorType;
            public uint RightShowCharID;
            public uint RightShowStatus;
            public uint RightShowAnimID;
            public uint BubbleID;
            public uint BackGroundIndex;

            public void Dispose()
            {
                ActorType = 0;
                CharID = 0;
                ActorNameID = 0;
                ContentID = 0;
                LeftShowActorType = 0;
                LeftShowCharID = 49999999;
                LeftShowStatus = 0;
                LeftShowAnimID = 0;
                RightShowActorType = 0;
                RightShowCharID = 49999999;
                RightShowStatus = 0;
                RightShowAnimID = 0;
                BubbleID = 0;
                BackGroundIndex = 0;
            }
        }

        public ResetDialogueDataEventData currentResetDialogueDataEventData;

        public readonly EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();

        public enum EEvents
        {
            OnResetDialogueData,
            OnShowContent, // 隐藏文字内容，只是展示模型
        }

        public static List<DialogueDataWrap> GetAcquiesceDialogueDataWraps(CSVDialogue.Data cSVDialogueData, CSVNpc.Data cSVNpcData)
        {
            List<DialogueDataWrap> datas = new List<DialogueDataWrap>();
            if (cSVDialogueData.dialogueContent == null)
            {
                Debug.LogError($"对话表字段dialogueContent为空！！！id: {cSVDialogueData.id}");
            }
            for (int index = 0, len = cSVDialogueData.dialogueContent.Count; index < len; index++)
            {
                DialogueDataWrap data = new DialogueDataWrap();
                data.ActorType = cSVDialogueData.dialogueContent[index][0];
                data.CharID = cSVNpcData.id;
                data.ActorNameID = cSVNpcData.name;
                data.ContentID = cSVDialogueData.dialogueContent[index][2];
                data.LeftShowCharID = 49999999;
                data.RightShowCharID = 49999999;
                int contentLen = cSVDialogueData.dialogueContent[index].Count;
                if (contentLen >= 4)
                {
                    data.LeftShowActorType = cSVDialogueData.dialogueContent[index][3];
                    if (contentLen >= 5)
                    {
                        data.LeftShowCharID = cSVDialogueData.dialogueContent[index][4];
                        if (contentLen >= 6)
                        {
                            data.LeftShowStatus = cSVDialogueData.dialogueContent[index][5];
                            if (contentLen >= 7)
                            {
                                data.LeftShowAnimID = cSVDialogueData.dialogueContent[index][6];
                                if (contentLen >= 8)
                                {
                                    data.RightShowActorType = cSVDialogueData.dialogueContent[index][7];
                                    if (contentLen >= 9)
                                    {
                                        data.RightShowCharID = cSVDialogueData.dialogueContent[index][8];
                                        if (contentLen >= 10)
                                        {
                                            data.RightShowStatus = cSVDialogueData.dialogueContent[index][9];
                                            if (contentLen >= 11)
                                            {
                                                data.RightShowAnimID = cSVDialogueData.dialogueContent[index][10];
                                                if (contentLen >= 12)
                                                {
                                                    data.BubbleID = cSVDialogueData.dialogueContent[index][11];
                                                    if (contentLen >= 13)
                                                    {
                                                        data.BackGroundIndex = cSVDialogueData.dialogueContent[index][12];
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

                datas.Add(data);
            }
            return datas;
        }

        public static List<DialogueDataWrap> GetDialogueDataWraps(CSVDialogue.Data cSVDialogueData)
        {
            List<DialogueDataWrap> datas = new List<DialogueDataWrap>();
            if (cSVDialogueData.dialogueContent == null)
            {
                Debug.LogError($"对话表字段dialogueContent为空！！！id: {cSVDialogueData.id}");
            }
            for (int index = 0, len = cSVDialogueData.dialogueContent.Count; index < len; index++)
            {
                DialogueDataWrap data = new DialogueDataWrap();
                data.ActorType = cSVDialogueData.dialogueContent[index][0];
                if (data.ActorType == (uint)EDialogueActorType.NPC)
                {
                    CSVNpc.Data cSVNpcData = CSVNpc.Instance.GetConfData(cSVDialogueData.dialogueContent[index][1]);
                    if (cSVNpcData != null)
                    {
                        data.CharID = cSVNpcData.id;
                        data.ActorNameID = cSVNpcData.name;
                    }
                    else
                    {
                        DebugUtil.LogError($"cSVNpcData is null, id: {cSVDialogueData.dialogueContent[index][1]} cSVDialogueData.id: {cSVDialogueData.id}");
                    }
                }
                else if (data.ActorType == (uint)EDialogueActorType.Parnter)
                {
                    CSVPartner.Data cSVPartnerData = CSVPartner.Instance.GetConfData(cSVDialogueData.dialogueContent[index][1]);
                    if (cSVPartnerData != null)
                    {
                        data.CharID = cSVPartnerData.id;
                        data.ActorNameID = cSVPartnerData.name;
                    }
                    else
                    {
                        DebugUtil.LogError($"CSVPartner.Data is null, id: {cSVDialogueData.dialogueContent[index][1]}");
                    }
                }
                data.ContentID = cSVDialogueData.dialogueContent[index][2];
                data.LeftShowCharID = 49999999;
                data.RightShowCharID = 49999999;

                int contentLen = cSVDialogueData.dialogueContent[index].Count;
                if (contentLen >= 4)
                {
                    data.LeftShowActorType = cSVDialogueData.dialogueContent[index][3];
                    if (contentLen >= 5)
                    {
                        data.LeftShowCharID = cSVDialogueData.dialogueContent[index][4];
                        if (contentLen >= 6)
                        {
                            data.LeftShowStatus = cSVDialogueData.dialogueContent[index][5];
                            if (contentLen >= 7)
                            {
                                data.LeftShowAnimID = cSVDialogueData.dialogueContent[index][6];
                                if (contentLen >= 8)
                                {
                                    data.RightShowActorType = cSVDialogueData.dialogueContent[index][7];
                                    if (contentLen >= 9)
                                    {
                                        data.RightShowCharID = cSVDialogueData.dialogueContent[index][8];
                                        if (contentLen >= 10)
                                        {
                                            data.RightShowStatus = cSVDialogueData.dialogueContent[index][9];
                                            if (contentLen >= 11)
                                            {
                                                data.RightShowAnimID = cSVDialogueData.dialogueContent[index][10];
                                                if (contentLen >= 12)
                                                {
                                                    data.BubbleID = cSVDialogueData.dialogueContent[index][11];
                                                    if (contentLen >= 13)
                                                    {
                                                        data.BackGroundIndex = cSVDialogueData.dialogueContent[index][12];
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

                datas.Add(data);
            }

            return datas;
        }

        public void OpenDialogue(ResetDialogueDataEventData resetDialogueDataEventData, bool triggerEventWhenOpenUI = true)
        {
            if (GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Fight)
            {
                return;
            }

            if (triggerEventWhenOpenUI)
            {
                GameMain.Procedure.TriggerEvent(this, (int)EProcedureEvent.EnterInteractive);
                currentResetDialogueDataEventData = resetDialogueDataEventData;
                if (!UIManager.IsOpen(EUIID.UI_Dialogue))
                {
                    UIManager.CloseUI(EUIID.UI_Attribute, false, false);

                    UIManager.OpenUI(EUIID.UI_Dialogue, true, resetDialogueDataEventData);
                }
                eventEmitter.Trigger<ResetDialogueDataEventData>(EEvents.OnResetDialogueData, resetDialogueDataEventData);
            }
            else
            {
                GameMain.Procedure.TriggerEvent(this, (int)EProcedureEvent.EnterInteractive);
                currentResetDialogueDataEventData = resetDialogueDataEventData;
                if (!UIManager.IsOpen(EUIID.UI_Dialogue))
                {
                    UIManager.CloseUI(EUIID.UI_Attribute, false, false);

                    UIManager.OpenUI(EUIID.UI_Dialogue, true, resetDialogueDataEventData);
                }
                else
                {
                    eventEmitter.Trigger<ResetDialogueDataEventData>(EEvents.OnResetDialogueData, resetDialogueDataEventData);
                }
            }
        }

        public override void Dispose()
        {
            currentResetDialogueDataEventData = null;

            base.Dispose();
        }

        #region 打点

        HitPointNpcDialog dialog = new HitPointNpcDialog();
        public void HitPointDialog(uint npcId)
        {
            dialog.AppendBaseData();
            dialog.scene_id = Sys_Map.Instance.CurMapId;
            dialog.npc_id = npcId;
            dialog.online_duration = (Sys_Role.Instance.OnlineTime + Framework.TimeManager.GetElapseTime());

            HitPointManager.HitPoint(HitPointNpcDialog.Key, dialog);
        }
        #endregion
    }
}
