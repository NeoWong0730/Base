using UnityEngine;
using Lib.Core;
using Logic.Core;
using UnityEngine.UI;
using Table;
using System.Collections.Generic;
using System;

namespace Logic
{
    public class UI_WarriorGroup_MeetingInfo_Layout
    {
        public Transform transform;

        public Button closeButton;

        public Text createRoleName;
        public Text leftTime;
        public Text content;

        public GameObject voteRoot;
        public GameObject infoRoot;

        public Button agreeButton;
        public Button refuseButton;

        public Text resultText;
        public Text agreeText;
        public Text refuseText;

        Timer timer;

        public void Init(Transform transform)
        {
            this.transform = transform;

            closeButton = transform.gameObject.FindChildByName("Btn_Close").GetComponent<Button>();

            createRoleName = transform.gameObject.FindChildByName("Text_Name").GetComponent<Text>();
            leftTime = transform.gameObject.FindChildByName("Text_Time").GetComponent<Text>();
            content = transform.gameObject.FindChildByName("ContentText").GetComponent<Text>();

            voteRoot = transform.gameObject.FindChildByName("State1");
            infoRoot = transform.gameObject.FindChildByName("State0");

            agreeButton = voteRoot.FindChildByName("Btn_01").GetComponent<Button>();
            refuseButton = voteRoot.FindChildByName("Btn_02").GetComponent<Button>();

            resultText = infoRoot.FindChildByName("VoteResult").GetComponent<Text>();
            agreeText = infoRoot.FindChildByName("AgreeContent").GetComponent<Text>();
            refuseText = infoRoot.FindChildByName("RefuseContent").GetComponent<Text>();
        }

        public void UpdateInfo(Sys_WarriorGroup.MeetingInfoBase meetingInfo)
        {
            TextHelper.SetText(createRoleName, meetingInfo.VoteNameList[meetingInfo.CreateRoleId]);

            if (meetingInfo.Status == Sys_WarriorGroup.MeetingInfoBase.MeetingStatusType.Doing)
            {
                DateTime dateTime;
                dateTime = Sys_Time.ConvertToDatetime(meetingInfo.LeftTime);
                TextHelper.SetText(leftTime, LanguageHelper.GetTextContent(13556, dateTime.Hour.ToString(LanguageHelper.gTimeFormat_2), dateTime.Minute.ToString(LanguageHelper.gTimeFormat_2)));
                timer?.Cancel();
                timer = Timer.Register(1f, () =>
                {
                    dateTime = Sys_Time.ConvertToDatetime(meetingInfo.LeftTime);
                    TextHelper.SetText(leftTime, LanguageHelper.GetTextContent(13556, dateTime.Hour.ToString(LanguageHelper.gTimeFormat_2), dateTime.Minute.ToString(LanguageHelper.gTimeFormat_2)));
                }, null, true);
            }
            else if(meetingInfo.Status == Sys_WarriorGroup.MeetingInfoBase.MeetingStatusType.History)
            {
                TextHelper.SetText(leftTime, LanguageHelper.GetTextContent(1002894));
            }

            TextHelper.SetText(content, meetingInfo.GetContent());

            if (meetingInfo.Status == Sys_WarriorGroup.MeetingInfoBase.MeetingStatusType.Doing)
            {
                voteRoot.SetActive(!meetingInfo.Voted);
                infoRoot.SetActive(meetingInfo.Voted);
                TextHelper.SetText(resultText, LanguageHelper.GetTextContent(1002879));
            }
            else if (meetingInfo.Status == Sys_WarriorGroup.MeetingInfoBase.MeetingStatusType.History)
            {
                voteRoot.SetActive(false);
                infoRoot.SetActive(true);

                if (meetingInfo.Result == true)
                    TextHelper.SetText(resultText, LanguageHelper.GetTextContent(1002880));
                else
                    TextHelper.SetText(resultText, LanguageHelper.GetTextContent(1002881));
            }

            RefreshVoteList(meetingInfo);
        }

        public void RefreshVoteList(Sys_WarriorGroup.MeetingInfoBase meetingInfo)
        {
            string agreeStr = $"{LanguageHelper.GetTextContent(1002873)}:";
            string refuseStr = $"{LanguageHelper.GetTextContent(1002872)}:";

            foreach (var vote in meetingInfo.VoteList)
            {
                if (vote.Value == true)
                {
                    agreeStr += $" {meetingInfo.VoteNameList[vote.Key]}";
                }
                else
                {
                    refuseStr += $" {meetingInfo.VoteNameList[vote.Key]}";
                }
            }
            TextHelper.SetText(agreeText, agreeStr);
            TextHelper.SetText(refuseText, refuseStr);
        }

        public void RegisterEvents(IListener listener)
        {
            closeButton.onClick.AddListener(listener.OnClickCloseButton);
            agreeButton.onClick.AddListener(listener.OnClickAgreeButton);
            refuseButton.onClick.AddListener(listener.OnClickRefuseButton);
        }

        public interface IListener
        {
            void OnClickCloseButton();

            void OnClickAgreeButton();

            void OnClickRefuseButton();
        }
    }

    public class UI_WarriorGroup_MeetingInfo : UIBase, UI_WarriorGroup_MeetingInfo_Layout.IListener
    {
        UI_WarriorGroup_MeetingInfo_Layout layout = new UI_WarriorGroup_MeetingInfo_Layout();

        Sys_WarriorGroup.MeetingInfoBase meetingInfo;

        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
        }

        protected override void ProcessEvents(bool toRegister)
        {
            Sys_WarriorGroup.Instance.eventEmitter.Handle(Sys_WarriorGroup.EEvents.QuitSuccessed, OnQuitSuccessed, toRegister);
            Sys_WarriorGroup.Instance.eventEmitter.Handle<uint>(Sys_WarriorGroup.EEvents.RefreshVote, OnRefreshVote, toRegister);
        }

        protected override void OnOpen(object arg)
        {
            meetingInfo = arg as Sys_WarriorGroup.MeetingInfoBase;
        }

        protected override void OnShow()
        {
            layout.UpdateInfo(meetingInfo);
        }

        void OnQuitSuccessed()
        {
            CloseSelf();
        }

        void OnRefreshVote(uint meetingID)
        {
            if (meetingInfo.MeetingID == meetingID)
            {
                layout.UpdateInfo(meetingInfo);
            }
        }

        public void OnClickAgreeButton()
        {
            Sys_WarriorGroup.Instance.ReqMeetingVoteAgree(meetingInfo.MeetingID);
        }

        public void OnClickRefuseButton()
        {
            Sys_WarriorGroup.Instance.ReqMeetingVoteRefuse(meetingInfo.MeetingID);
        }

        /// <summary>
        /// 点击了关闭按钮///
        /// </summary>
        public void OnClickCloseButton()
        {
            UIManager.CloseUI(EUIID.UI_WarriorGroup_MeetingInfo);
        }
    }
}