using Table;
using UnityEngine;
using Lib.Core;
using Logic.Core;
using UnityEngine.UI;

namespace Logic
{
    public partial class UI_WarriorGroup_Layout
    {
        public class HistoryMeetingInfoItem
        {
            public Sys_WarriorGroup.MeetingInfoBase data;

            GameObject root;

            public Text createrName;
            public Text meetingType;
            public Text meetingDesc;
            public Text result;
            public Button infoButton;
            public Button createButton;
            public GameObject redPoint;

            public void BindGameObject(GameObject gameObject)
            {
                root = gameObject;

                createrName = root.FindChildByName("Text_Name").GetComponent<Text>();
                meetingType = root.FindChildByName("Text_Type").GetComponent<Text>();
                meetingDesc = root.FindChildByName("Text_Title").GetComponent<Text>();
                result = root.FindChildByName("Text_Time").GetComponent<Text>();
                infoButton = root.FindChildByName("Btn_02_check").GetComponent<Button>();
                infoButton.onClick.AddListener(OnClickInfoButton);
                createButton = root.FindChildByName("Btn_01_Small").GetComponent<Button>();
                redPoint = infoButton.gameObject.FindChildByName("Image_Dot");
            }

            public void UpdateItem(Sys_WarriorGroup.MeetingInfoBase meetingInfo)
            {
                data = meetingInfo;

                infoButton.gameObject.SetActive(true);
                createButton.gameObject.SetActive(false);

                TextHelper.SetText(meetingType, LanguageHelper.GetTextContent(CSVBraveTeamMeeting.Instance.GetConfData(data.InfoID).TypeLan));
                TextHelper.SetText(meetingDesc, LanguageHelper.GetTextContent(CSVBraveTeamMeeting.Instance.GetConfData(data.InfoID).TypeContentLan));
                TextHelper.SetText(createrName, data.CreateRoleName);
                if (data.Result == true)
                    TextHelper.SetText(result, LanguageHelper.GetTextContent(1002896));
                else
                    TextHelper.SetText(result, LanguageHelper.GetTextContent(1002897));

                if (Sys_WarriorGroup.Instance.historyMeetingRedPointInfo.meetingDic.ContainsKey(data.MeetingID) && Sys_WarriorGroup.Instance.historyMeetingRedPointInfo.meetingDic[data.MeetingID].read == false)
                    redPoint.SetActive(true);
                else
                    redPoint.SetActive(false);
            }

            /// <summary>
            /// 点击了详情按钮///
            /// </summary>
            void OnClickInfoButton()
            {
                if (Sys_WarriorGroup.Instance.historyMeetingRedPointInfo.meetingDic.ContainsKey(data.MeetingID))
                {
                    Sys_WarriorGroup.Instance.historyMeetingRedPointInfo.meetingDic[data.MeetingID].read = true;
                }
                Sys_WarriorGroup.Instance.SerializeHistoryMeetingRedPointInfo();

                if (Sys_WarriorGroup.Instance.historyMeetingRedPointInfo.meetingDic.ContainsKey(data.MeetingID) && Sys_WarriorGroup.Instance.historyMeetingRedPointInfo.meetingDic[data.MeetingID].read == false)
                    redPoint.SetActive(true);
                else
                    redPoint.SetActive(false);

                Sys_WarriorGroup.Instance.eventEmitter.Trigger(Sys_WarriorGroup.EEvents.ReadHistoryMeetingInfo);

                UIManager.OpenUI(EUIID.UI_WarriorGroup_MeetingInfo, false, data);
            }
        }
    }
}
