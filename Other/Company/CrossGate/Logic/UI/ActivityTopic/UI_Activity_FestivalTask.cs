using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;
using Framework;
using System;
using Lib.Core;

namespace Logic
{
    public class UI_Activity_FestivalTask :UIBase
    {
        private Button btn_Close;
        private Button btn_Go;
        private Text txt_Time;
        private uint activityId;
        private CSVFestivalTask.Data tData;

        protected override void OnLoaded()
        {
            btn_Close = transform.Find("Animator/Content/Btn_Close").GetComponent<Button>();
            btn_Go= transform.Find("Animator/Content/Button").GetComponent<Button>();
            txt_Time= transform.Find("Animator/Content/Text_Time").GetComponent<Text>();
            btn_Close.onClick.AddListener(OnCloseButtonClicked);
            btn_Go.onClick.AddListener(OnGoButtonClicked);

        }
        protected override void OnOpen(object arg)
        {
            activityId = 0u;
            if (arg!=null)
            {
                activityId = (uint)arg;
            }
            else
            {
                Debug.LogError("UI_Activity_FestivalTask OnOpen Error Param");
            }
        }
        protected override void OnDestroy()
        {

        }

        protected override void OnShow()
        {
            Sys_ActivityTopic.Instance.isTaskGuide = false;
            var fData = CSVFestivalTask.Instance.GetAll();
            for (int i=0;i<fData.Count;i++)
            {
                if (fData[i].Activity_Id==activityId)
                {
                    tData = fData[i];
                }
            }
            TimeShow();
            Sys_ActivityTopic.Instance.eventEmitter.Trigger(Sys_ActivityTopic.EEvents.OnCommonActivityUpdate);
        }
        private void TimeShow()
        {
            if (activityId!=0)
            {
                var _data = CSVOperationalActivityRuler.Instance.GetConfData(activityId);
                if (_data!=null)
                {
                    DateTime startTime= TimeManager.GetDateTime(TimeManager.ConvertFromZeroTimeZone(_data.Begining_Date));
                    DateTime endTime= TimeManager.GetDateTime(TimeManager.ConvertFromZeroTimeZone(_data.Begining_Date+_data.Duration_Day * 3600 * 24));
                    txt_Time.text = LanguageHelper.GetTextContent(2025301, startTime.Year.ToString(), startTime.Month.ToString(), startTime.Day.ToString(),endTime.Year.ToString(), endTime.Month.ToString(), endTime.Day.ToString());
                }
            }
        }

        private void OnGoButtonClicked()
        {
            if (tData!=null)
            {
                ActionCtrl.Instance.MoveToTargetNPCAndInteractive(tData.Npc_Id,false);
                UIManager.CloseUI(EUIID.UI_Activity_FestivalTask);
                UIManager.CloseUI(EUIID.UI_Activity_Topic);
            }
            else
            {
                Debug.LogError("UI_Activity_FestivalTask Data Is Null");
            }
            
        }
        private void OnCloseButtonClicked()
        {
            UIManager.CloseUI(EUIID.UI_Activity_FestivalTask);
        }
 
    }
}