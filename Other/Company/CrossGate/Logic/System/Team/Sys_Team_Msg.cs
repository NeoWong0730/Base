using Packet;
using Logic.Core;
using Lib.Core;
using Net;
using System.Collections.Generic;
using UnityEngine;
using System;
using Google.Protobuf;
using Table;

namespace Logic
{
    public partial class Sys_Team : SystemModuleBase<Sys_Team>
    {
        class MsgItem
        {
            public IMessage Message;
            public Action<int,IMessage> CallAction;
            public ushort MsgType;
            /// <summary>
            /// 默认操作，0 无 ， 1 取消  2 确定
            /// </summary>
            public uint DefaultOp = 0;
        }

        class MsgPool
        {
            private List<MsgItem> msgItems = new List<MsgItem>();

            public List<MsgItem> Items { get { return msgItems; } }
            public int Count { get { return msgItems.Count; } }
            public void Enqueue(MsgItem msg)
            {
                msgItems.Add(msg);
            }

            public void Dequeue()
            {
                if (msgItems.Count == 0)
                    return;

                msgItems.RemoveAt(0);
            }

            public MsgItem Peek()
            {
                if (msgItems.Count == 0)
                    return null;

                return msgItems[0];
            }

            public void Clear(ushort ntype)
            {
                msgItems.RemoveAll(o => o.MsgType == ntype);
            }

            public void ClearAll()
            {
                msgItems.Clear();
            }

            public bool isContain(ushort type)
            {
                int count = msgItems.Count;
                if (count == 0)
                    return false;

                for (int i = 0; i < count; i++)
                {
                    if (msgItems[i] != null && msgItems[i].MsgType == type)
                        return true;
                }

                return false;
            }
        }
        private MsgPool m_MsgQueue = new MsgPool();

        private MsgDialog m_msgDiclog = new MsgDialog();
        private void pushMsg(IMessage message,ushort ntype,Action<int, IMessage> action,float time = 60,uint defaultOp = 1)
        {
            m_MsgQueue.Enqueue(new MsgItem() { Message = message, CallAction = action, MsgType = ntype, DefaultOp = defaultOp });

            if (m_MsgQueue.Count == 1 && m_msgDiclog.Active == false)
            {
                OpenMsgDialog(m_MsgQueue.Peek(),time);
            }
        }

        private void popMsg(IMessage message,int result)
        {
            if (m_MsgQueue.Count == 0)
                return;

            MsgItem item = m_MsgQueue.Peek();

            if (message == item.Message)
            {
                if (result != int.MaxValue)
                    item.CallAction?.Invoke(result, item.Message);

                m_MsgQueue.Dequeue();
            }
        }

        private void clearMsg(ushort ntype)
        {
            MsgItem item = m_MsgQueue.Peek();

            if (item != null && m_msgDiclog.Active && item.Message == m_msgDiclog.m_Message)
            {
                m_msgDiclog.CloseDialog();
            }

            m_MsgQueue.Clear(ntype);
        }
        private void OnMsgDialogReslut(int reslut, IMessage message)
        {
            popMsg(message,reslut);

            
            if (m_MsgQueue.Count > 0 && m_msgDiclog.Active == false)
            {
                OpenMsgDialog(m_MsgQueue.Peek());
            }
        }

        private void activeFristMsg()
        {
            if (m_MsgQueue.Count == 0)
                return;

            if (m_msgDiclog.Active)
                return;

            OpenMsgDialog(m_MsgQueue.Peek());
            
        }
        private void OpenMsgDialog(MsgItem msg,float time = 60)
        {
            m_msgDiclog.OpenDialog(msg.Message, time,msg.MsgType, OnMsgDialogReslut,msg.DefaultOp);
        }


        #region class MsgDialog
        class MsgDialog
        {
           public IMessage m_Message { get; set;}

            public Action<int, IMessage> m_Action;

            public CmdTeam cmd { get; set; }
            public bool Active { get; private set; }
            private void OnClick0()
            {
                UIManager.HitButton(EUIID.UI_PromptBox, cmd.ToString() + ": Cancle");
                Result(0);
            }

            private void OnClick1()
            {
                UIManager.HitButton(EUIID.UI_PromptBox, cmd.ToString() + ":Confirm");
                Result(1);
            }

            private void OnClickNoOperator()
            {
                Result(1);
            }
            private void Result(int result)
            {
                Active = false;

                m_Action?.Invoke(result, m_Message);
               
            }

            public void OpenDialog(IMessage message, float time,ushort type, Action<int, IMessage> action,uint defalueOp)
            {
                m_Message = message;

                m_Action = action;

                cmd = (CmdTeam) type;

                PromptBoxParameter.Instance.Clear();

              

                PromptBoxParameter.ECountdown eCountdown = PromptBoxParameter.ECountdown.None;

                if (defalueOp > 0)
                    eCountdown = defalueOp == 1 ? PromptBoxParameter.ECountdown.Cancel : PromptBoxParameter.ECountdown.Confirm;


                PromptBoxParameter.Instance.SetConfirm(true, OnClick0, defalueOp == 2 ? 8000u : 2002121u);

                PromptBoxParameter.Instance.SetCancel(true, OnClick1, defalueOp == 1 ? 2002122u : 2004096u);

                PromptBoxParameter.Instance.SetCountdown(time, eCountdown);

               
                if (defalueOp == 2)
                    PromptBoxParameter.Instance.onNoOperator = OnClick0;
                else if(defalueOp == 1)
                    PromptBoxParameter.Instance.onNoOperator = OnClick1;
                else
                    PromptBoxParameter.Instance.onNoOperator = OnClickNoOperator;
                        

                PromptBoxParameter.Instance.content = getString(message);

                if (UIManager.IsOpen(EUIID.UI_PromptBox) == false)
                {
                    UIManager.OpenUI(EUIID.UI_PromptBox, true, PromptBoxParameter.Instance);

                    Active = true;
                }
                else
                {
                    action?.Invoke(int.MaxValue, message);
                }

            }

            public void CloseDialog()
            {
                Active = false;
                UIManager.CloseUI(EUIID.UI_PromptBox, false);
            }
            private string getString(IMessage message)
            {
                if (message is CmdTeamInviteNtf)
                    return getString(message as CmdTeamInviteNtf);

                if (message is CmdTeamMemInviteNtf)
                    return getString(message as CmdTeamMemInviteNtf);

                if (message is CmdTeamApplyNtf)
                    return getString(message as CmdTeamApplyNtf);

                if (message is CmdTeamApplyLeadingNtf)
                    return getString(message as CmdTeamApplyLeadingNtf);

                if (message is CmdTeamCallBackNtf)
                    return getString(message as CmdTeamCallBackNtf);

                return string.Empty;
            }
            private string getString(CmdTeamInviteNtf info)
            {
                return LanguageHelper.GetTextContent(2002123, info.InvitorName);
            }

            private string getString(CmdTeamMemInviteNtf info)
            {
                TeamMem teamMem = Sys_Team.Instance.getTeamMem(info.MemId);
                if (teamMem == null)
                    return string.Empty;

                return LanguageHelper.GetTextContent(2002124, teamMem.Name.ToStringUtf8(), info.TargetName);
            }

            private string getString(CmdTeamApplyNtf info)
            {   
                return LanguageHelper.GetTextContent(2002125, info.Applyrole.Info.Name.ToStringUtf8());
            }

            private string getString(CmdTeamApplyLeadingNtf info)
            {
                return LanguageHelper.GetTextContent(2002128, info.RoleName.ToStringUtf8());
            }

            private string getString(CmdTeamCallBackNtf info)
            {
                return LanguageHelper.GetTextContent(2002129,info.InvitorName.ToStringUtf8());
            }
        }
        #endregion
    }


    /// <summary>
    /// 消息提示
    /// </summary>
    public partial class Sys_Team : SystemModuleBase<Sys_Team>
    {
        //public class MessageBase
        //{
        //    public string message;

        //    public Action<uint> ResultCall;

        //    public float time = 0;

        //    protected virtual void OnResult(uint op)
        //    {

        //    }
        //}

        Timer m_MatchTimer;

        private void StartNoTeamMatch()
        {
            m_MatchTimer?.Cancel();

           var teamData =  CSVTeam.Instance.GetConfData(MatchingTarget);

            if (teamData == null)
                return;

            MatchTeamTime = (float)teamData.is_captain;

            m_MatchTimer = Timer.Register(MatchTeamTime, () => {

                if (HaveTeam)
                    return;

                OpenNoTeamTargetMatchingTips();

            });
        }

        private void StopNoTeamMatchTimer()
        {
            m_MatchTimer?.Cancel();

            m_MatchTimer = null;
        }
        private void OpenNoTeamTargetMatchingTips()
        {
            if (HaveTeam || isMatching == false)
                return;

            string message = LanguageHelper.GetTextContent(12108);

            OpenTips(MatchTeamTipsTime,message,()=>{

                ApplyCreateTeam(Sys_Role.Instance.RoleId, MatchingTarget);
            
            });
        }


        private void OpenTeamTargetMatchFullTips()
        {
            var data = CSVTeam.Instance.GetConfData(MatchingTarget);

            if (data == null)
                return;

            string message = LanguageHelper.GetTextContent(12107,LanguageHelper.GetTextContent(data.subclass_name));

            OpenTips(-1, message, () => {

                if (data.is_come == 0)
                    return;

                Sys_Daily.Instance.GotoActivity(data.jump_id);

            },null,null, data.is_come > 0);
        }


        public bool OpenFamilySkipTips(uint teamid)
        {
            var data = CSVTeam.Instance.GetConfData(teamid);

            if (data == null)
                return false;

            if (data.only_guild && Sys_Family.Instance.familyData.isInFamily == false)
            {
                Sys_Team.Instance.OpenTips(0, LanguageHelper.GetTextContent(12296u), () => {

                    Sys_Family.Instance.OpenUI_Family(new UI_FamilyOpenParam() { familyMenuEnum = (uint)UI_Family.EFamilyMenu.Hall });
                   
                });

                return true;
            }

            return false;
        }
        public void OpenTips(float time, string message, Action onClickConfirm = null, Action onClickCancle = null, Action onNoOperator = null,
            bool isShowCancle = true,uint confrimLangueid = 0, uint cancleLangueid = 0)
        {

            PromptBoxParameter.Instance.Clear();

            uint canfirmid = confrimLangueid == 0 ? 2002121u : confrimLangueid;
            PromptBoxParameter.Instance.SetConfirm(true, onClickConfirm, canfirmid);

            uint cancleid = cancleLangueid == 0 ? ((time <= 0 ? 4914u : 2002122u)) : cancleLangueid;

            PromptBoxParameter.Instance.SetCancel(isShowCancle, onClickCancle, cancleid);

            PromptBoxParameter.Instance.SetCountdown(time, time <= 0 ? PromptBoxParameter.ECountdown.None: PromptBoxParameter.ECountdown.Cancel);

            PromptBoxParameter.Instance.onNoOperator = onNoOperator;

            PromptBoxParameter.Instance.content = message;

            UIManager.OpenUI(EUIID.UI_PromptBox, true, PromptBoxParameter.Instance);
        }
    }
}
