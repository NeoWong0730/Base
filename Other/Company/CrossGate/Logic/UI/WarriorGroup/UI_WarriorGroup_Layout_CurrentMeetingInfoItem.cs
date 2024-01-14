using Table;
using UnityEngine;
using Lib.Core;
using Logic.Core;
using UnityEngine.UI;
using System;

namespace Logic
{
    public partial class UI_WarriorGroup_Layout
    {
        /// <summary>
        /// 当前会议Item///
        /// </summary>
        public class CurrentMeetingInfoItem
        {
            public Sys_WarriorGroup.MeetingInfoBase data;

            GameObject root;

            public Text createrName;
            public Text meetingType;
            public Text meetingDesc;
            public Text time;
            public Button infoButton;
            public Button createButton;
            public GameObject redPoint;

            Timer timer;

            public void BindGameObject(GameObject gameObject)
            {
                root = gameObject;

                createrName = root.FindChildByName("Text_Name").GetComponent<Text>();
                meetingType = root.FindChildByName("Text_Type").GetComponent<Text>();
                meetingDesc = root.FindChildByName("Text_Title").GetComponent<Text>();
                time = root.FindChildByName("Text_Time").GetComponent<Text>();
                infoButton = root.FindChildByName("Btn_02_check").GetComponent<Button>();
                infoButton.onClick.AddListener(OnClickInfoButton);
                createButton = root.FindChildByName("Btn_01_Small").GetComponent<Button>();
                createButton.onClick.AddListener(OnClickCreateButton);
                redPoint = infoButton.gameObject.FindChildByName("Image_Dot");
            }

            public void UpdateItem(Sys_WarriorGroup.MeetingInfoBase meetingInfo)
            {
                data = meetingInfo;

                if (data.Status == Sys_WarriorGroup.MeetingInfoBase.MeetingStatusType.Normal)
                    TextHelper.SetText(createrName, "-");
                else
                    TextHelper.SetText(createrName, data.CreateRoleName);
                TextHelper.SetText(meetingType, LanguageHelper.GetTextContent(CSVBraveTeamMeeting.Instance.GetConfData(data.InfoID).TypeLan));
                TextHelper.SetText(meetingDesc, LanguageHelper.GetTextContent(CSVBraveTeamMeeting.Instance.GetConfData(data.InfoID).TypeContentLan));

                if (data.Status == Sys_WarriorGroup.MeetingInfoBase.MeetingStatusType.Normal)
                {
                    createButton.gameObject.SetActive(true);
                    infoButton.gameObject.SetActive(false);
                    TextHelper.SetText(time, "-");
                }
                else if (data.Status == Sys_WarriorGroup.MeetingInfoBase.MeetingStatusType.Doing)
                {
                    createButton.gameObject.SetActive(false);
                    infoButton.gameObject.SetActive(true);
                    DateTime dateTime;

                    if (data.LeftTime > 0)
                    {
                        dateTime = Sys_Time.ConvertToDatetime(data.LeftTime);
                        TextHelper.SetText(time, LanguageHelper.GetTextContent(13556, dateTime.Hour.ToString(LanguageHelper.gTimeFormat_2), dateTime.Minute.ToString(LanguageHelper.gTimeFormat_2)));
                        timer?.Cancel();
                        timer = Timer.Register(1f, () =>
                        {
                            if (data.LeftTime > 0)
                            {
                                dateTime = Sys_Time.ConvertToDatetime(data.LeftTime);
                                TextHelper.SetText(time, LanguageHelper.GetTextContent(13556, dateTime.Hour.ToString(LanguageHelper.gTimeFormat_2), dateTime.Minute.ToString(LanguageHelper.gTimeFormat_2)));
                            }
                            else
                            {
                                TextHelper.SetText(time, "-");


                            }
                        }, null, true);
                    }
                    else
                    {
                        TextHelper.SetText(time, "-");
                    }

                    redPoint.SetActive(!data.Voted);
                }
            }

            /// <summary>
            /// 点击了详情按钮///
            /// </summary>
            void OnClickInfoButton()
            {
                UIManager.OpenUI(EUIID.UI_WarriorGroup_MeetingInfo, false, data);
            }

            /// <summary>
            /// 点击了发起会议按钮///
            /// </summary>
            void OnClickCreateButton()
            {
                if (Sys_WarriorGroup.Instance.MyWarriorGroup.NextCreateMeetingTime > 0)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(13561));
                }
                else
                {
                    UIManager.OpenUI(EUIID.UI_WarriorGroup_CreateMeeting, false, data);
                }
            }
        }
    }
}
