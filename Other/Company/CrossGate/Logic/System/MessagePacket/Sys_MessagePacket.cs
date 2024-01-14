using System.Collections.Generic;
using Logic.Core;
using Packet;
using Net;
using Lib.Core;
using Table;
using Logic;
using System;
using UnityEngine;
using Framework;
using UnityEngine.UI;

namespace Logic
{
    public enum EMessageBagType
    {
        Team = 0,//toggle0——0
        Friend = 1,//toggle1——1
        Family = 2,//toggle2——2
        Tutor = 3,//toggle0
        BraveTeam =4,//toggle3——4
    }
    public class Sys_MessageBag : SystemModuleBase<Sys_MessageBag>, ISystemModuleUpdate
    {

        public class MessageData
        {
            public List<MessageContent> messageList = new List<MessageContent>(100);

            public int GetCount()
            {
                return messageList.Count;
            }

            public void Clear()
            {
                messageList.Clear();
            }
            public MessageContent GetMessageByIndex(int index)
            {

                if (messageList.Count > index && index >= 0)
                {
                    return messageList[index];
                }
                return null;
            }
            public void Add(MessageContent content)
            {
                if (messageList.Count >= messageList.Capacity)
                {
                    messageList.RemoveAt(0);
                }
                messageList.Add(content);
            }

            public bool IsContain(ulong sendId)
            {
                for (int i = 0; i < messageList.Count; i++)
                {
                    if (messageList[i].invitorId == sendId)
                    {
                        return true;
                    }
                }
                return false;
            }


        }
        public class MessageContent//单条消息
        {
            public EMessageBagType mType;
            public ulong invitorId;//申请者id
            public string invitiorName;//申请者名
            public TeamMessage tMess;//组队、导师
            public GuildMessage cMess;//家族、勇者团
            public FriendMessage friMess;//好友
            public uint sendServerTime;

            public float countDownTime;
            public void SetTime(float _time)
            {
                countDownTime = _time;
            }

        }
        public class TeamMessage
        {
            public uint targetId; //目标id
            public ulong teamId;
            public uint lowLv;
            public uint highLv;
            public bool isTutor;//普通组队false,导师组队true
            public bool isComeBack;//回归用户 是-true
        }
        public class GuildMessage
        {//用于家族和勇者团
            public ulong guildId;//帮派id
            public string guildName;//帮派名
        }
        public class FriendMessage
        {
            public uint roleLv;//申请者等级
            public uint career;//申请者职业
        }

        public EventEmitter<EEvents> eventEmitter = new EventEmitter<EEvents>();
        public EMessageBagType ShowType;
        public EMessageBagType JumpType;
        public EMessageBagType ClickType;
        public int messageCount;
        Dictionary<EMessageBagType, MessageData> mData = new Dictionary<EMessageBagType, MessageData>();
        public bool isClick=false;
        public int MessageTypeCount=0;
        public enum EEvents
        {
            OnButtonShow,
            OnMessageDateUpdate,
        }

        #region 系统函数
        private void ProcessEvents(bool register)
        {
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.JoinFamily, OnJoinFamily, register);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.CreateFamily, OnJoinFamily, register);
            Sys_Team.Instance.eventEmitter.Handle(Sys_Team.EEvents.HaveTeam, OnHaveTeam, register);
            Sys_WarriorGroup.Instance.eventEmitter.Handle(Sys_WarriorGroup.EEvents.JoinedSuccess, OnJoinWarriorGroup, register);
            Sys_WarriorGroup.Instance.eventEmitter.Handle(Sys_WarriorGroup.EEvents.CreatedSuccess, OnJoinWarriorGroup, register);
            OptionManager.Instance.eventEmitter.Handle<int>(OptionManager.EEvents.OptionFinalChange, RefuseTutorOption, register);
        }
        public override void Init()
        {
            ProcessEvents(true);
            //消息包初始化
            MessagePacketInit();
            //每帧都跑
            SetIntervalFrame(1);
        }

        public void OnUpdate()
        {
            if (TimeManager.CanExecute(0, 360))
            {
                for (int i = 0; i <= MessageTypeCount; i++)
                {
                    CheckUpList((EMessageBagType)i);

                }

            }

            for (int i = 0; i <= MessageTypeCount; i++)
            {
                if (SingleTypeCount((EMessageBagType)i) != 0)
                {
                    if (i==0||i==3)
                    {
                        JumpType = EMessageBagType.Team;
                    }
                    else
                    {
                        JumpType = (EMessageBagType)i;
                    }
                    
                    break;
                }
            }

       
        }

        public override void OnLogout()
        {
            base.OnLogout();
            foreach (var data in mData.Values)
            {
                data.Clear();
            }
        }
        public override void Dispose()
        {
            ProcessEvents(false);
        }
        public void OnHaveTeam()
        {
            GetMessageData(0).Clear();
            GetMessageData((EMessageBagType)3).Clear();
            eventEmitter.Trigger(EEvents.OnMessageDateUpdate);
        }

        public void OnJoinFamily()
        {
            GetMessageData((EMessageBagType)2).Clear();
            eventEmitter.Trigger(EEvents.OnMessageDateUpdate);
        }
        public void OnJoinWarriorGroup()
        {
            GetMessageData((EMessageBagType)4).Clear();
            eventEmitter.Trigger(EEvents.OnMessageDateUpdate);
        }
        #endregion

        #region Function
        private void MessagePacketInit()
        {
            MessageTypeCount = (int)EMessageBagType.BraveTeam;//ATTENTION:这里写枚举最后一个
            for (int i = 0; i <= MessageTypeCount; i++)
            {
                mData.Add((EMessageBagType)i, new MessageData());
            }
            JumpType = EMessageBagType.Team;
            ClickType = JumpType;
            ValueInit();
        }

        //获得当前真实消息数总和
        public void RealValueInit()
        {
            messageCount = 0;
            for (int i = 0; i <= MessageTypeCount; i++)
            {
                messageCount += SingleTypeCount((EMessageBagType)i);
            }
        }
        //获得当前显示消息数总和//这里有20的限制，仅供显示使用，不代表实际消息数量
        public void ValueInit()
        {
            messageCount = 0;
            for (int i = 0; i <= MessageTypeCount; i++)
            {
                var singleTypeCount = SingleTypeCount((EMessageBagType)i);
                if (singleTypeCount >= 20)
                {
                    messageCount += 20;
                }
                else
                {
                    messageCount += singleTypeCount;
                }
            }
        }
           

        public void SendMessageInfo(EMessageBagType type, MessageContent messageContent)
        {
            if (messageContent == null)
            {
                return;
            }
            if (type== EMessageBagType.Tutor&&OptionManager.Instance.GetBoolean(OptionManager.EOptionID.TutorMessageBag))
            {
                return;
            }
            if (GetMessageData(type).IsContain(messageContent.invitorId))//去除重复的消息
            {
                return;

            }
            messageContent.sendServerTime = Sys_Time.Instance.GetServerTime();
            switch (type)
            {
                case EMessageBagType.Team:
                    JumpType = EMessageBagType.Team;
                    ShowType = EMessageBagType.Team;
                    break;
                case EMessageBagType.Family:
                    ShowType = EMessageBagType.Family;
                    messageContent.SetTime(float.Parse(CSVParam.Instance.GetConfData(1122).str_value));
                    break;

                case EMessageBagType.Friend:
                    ShowType = EMessageBagType.Friend;
                    messageContent.SetTime(float.Parse(CSVParam.Instance.GetConfData(1123).str_value));
                    break;
                case EMessageBagType.Tutor:
                    ShowType = EMessageBagType.Tutor;
                    JumpType = EMessageBagType.Team;
                    break;
                case EMessageBagType.BraveTeam:
                    ShowType = EMessageBagType.BraveTeam;
                    messageContent.SetTime(float.Parse(CSVParam.Instance.GetConfData(1379).str_value));
                    break;
                default: break;

            }
            messageContent.countDownTime -= 1.0f;//误差
            GetMessageData(type).Add(messageContent);
            eventEmitter.Trigger<int>(EEvents.OnButtonShow, (int)type);

        }

       

        //获得单个类型数量
        public int SingleTypeCount(EMessageBagType type)
        {
            return mData[type].GetCount();
        }

        //获得对应数据
        public MessageData GetMessageData(EMessageBagType type)
        {
            mData.TryGetValue(type, out MessageData msgChannelData);
            return msgChannelData;
        }
        //获得对应列表
        private List<MessageContent> GetMessageList(EMessageBagType type)
        {
            return GetMessageData(type).messageList;

        }
        //直接从列表中移除元素
        public void RemoveContentFromList(EMessageBagType type, MessageContent mContent)
        {
            GetMessageData(type).messageList.Remove(mContent);
        }
        //主界面按钮是否显示
        public bool IsMessageButtonShow()
        {
            return (messageCount != 0);
        }
        //红点
        public bool IsMessageBagRedPoint(int type)
        {
            if ((mData[(EMessageBagType)type].GetCount() != 0) && ClickType != (EMessageBagType)type)
            {
                return true;
            }
            return false;
        }


        public void CheckUpList(EMessageBagType type)
        {
            var _list = GetMessageList(type);
            if (_list.Count == 0)
            {
                return;
            }
            for (int i = _list.Count - 1; i >= 0; --i)
            {
                if (!CheckDestory(_list[i]))
                {
                    break;
                }
                if (CheckDestory(_list[i]))
                {
                    AutoRefuseSever(type, _list[i]);
                    _list.RemoveAt(i);
                }
            }

        }
        //自动拒绝
        public void AutoRefuseSever(EMessageBagType mType, MessageContent mContent)
        {
            switch (mType)
            {

                case EMessageBagType.Team:
                    Sys_Team.Instance.Send_MessageBag_InviteOpReq(2, mContent.tMess.teamId, mContent.invitorId);
                    break;
                case EMessageBagType.Friend:
                    Sys_Society.Instance.RefuseAddFriendReq(mContent.invitorId, mContent.invitiorName);
                    break;
                case EMessageBagType.Family:
                    Sys_Family.Instance.SendGuildInviteRpl(mContent.cMess.guildId, mContent.invitorId, mContent.invitiorName, 1);
                    break;
                case EMessageBagType.Tutor:
                    Sys_Team.Instance.Send_MessageBag_InviteOpReq(2, mContent.tMess.teamId, mContent.invitorId);
                    break;
                case EMessageBagType.BraveTeam:
                    Sys_WarriorGroup.Instance.RemoveInvite(mContent.invitorId);
                    break;
                default: break;
            }

        }
        //检查列表内是否要删除元素

        public uint GetRunTime(MessageContent mContent)
        {
            uint nowTime = Sys_Time.Instance.GetServerTime();
            DateTime startTime = TimeManager.GetDateTime(mContent.sendServerTime);
            DateTime nowdata = TimeManager.GetDateTime(nowTime);
            TimeSpan sp = nowdata.Subtract(startTime);
            float lastTime = mContent.countDownTime-(float)sp.TotalSeconds;
            if (lastTime <= 0)
            {
                return 0;
            }
            return (uint)lastTime;

        }
        public bool CheckDestory(MessageContent mContent)
        {
            
            if (GetRunTime(mContent)<=0)
            {
                return true;
            }
            return false;
        }
        private void RefuseTutorOption(int optionID)
        {
            EMessageBagType _type = EMessageBagType.Tutor;
            if (OptionManager.Instance.GetBoolean(OptionManager.EOptionID.TutorMessageBag))
            {
                var _list = GetMessageList(_type);
                for (int i =_list.Count-1; i >=0;--i)
                {
                    AutoRefuseSever(_type, _list[i]);
                    _list.RemoveAt(i);
                }
                eventEmitter.Trigger(EEvents.OnMessageDateUpdate);
                eventEmitter.Trigger(EEvents.OnButtonShow);
            }
        }
        public void MessageBagButtonShow(Text _txtCount,Text _txtName)
        {
            ValueInit();
            if (messageCount == 0)
            {
                UIManager.CloseUI(EUIID.UI_MessageBag);
                return;
            }
            
            if (messageCount < 99)
            {
                _txtCount.text = messageCount.ToString();
            }
            else
            {
                _txtCount.text = "99+";
            }

            if (SingleTypeCount(ShowType) == 0)
            {
                if (ShowType == EMessageBagType.BraveTeam)//ATTENTION:如果有添加，这里改成枚举的最后一个
                {
                    ShowType = 0;
                }
                else
                {
                    ShowType++;
                }
            }
            MesaageBagTextShow(_txtName, (int)ShowType);
        }
        public void MesaageBagTextShow(Text _txt,int type)
        {//界面icon上右下角的文字
            switch (type)
            {
                case 0:
                    _txt.text = LanguageHelper.GetTextContent(1003200);
                    break;
                case 1:
                    _txt.text = LanguageHelper.GetTextContent(1003203);
                    break;
                case 2:
                    _txt.text = LanguageHelper.GetTextContent(1003201);
                    break;
                case 3:
                    _txt.text = LanguageHelper.GetTextContent(1003202);
                    break;
                case 4:
                    _txt.text = LanguageHelper.GetTextContent(1003216);
                    break;
                default: break;

            }

        }
        #endregion
    }


}

