using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Packet;
using Table;

namespace Logic
{
    public class UI_TerroristEnter : UIBase
    {
        private Button m_BtnClose;

        private UI_TerroristEnter_Left m_leftInfo;
        //private UI_TerroristEnter_Right m_rightInfo;

        private Image m_ImgPic;
        private Button m_BtnAgree;
        private Button m_BtnRefuse;
        private Button m_BtnCancel;

        private Text m_TextTime;
        private Image m_ImgTime;

        private Text m_TextWaitTeam;

        private Lib.Core.Timer m_Timer;
        private float TimeCount = 0;

        private Text m_TextCount;
        private Text m_TextComplete;
        private bool m_IsCompleted;

        private TerrorSeriesMemItems mems;

        //protected virtual void OnInit() { }
        protected override void OnOpen(object arg)
        {
            mems = null;
            if (arg != null)
                mems = (TerrorSeriesMemItems)arg;
        }

        protected override void OnLoaded()
        {
            m_BtnClose = transform.Find("Animator/TtileAndBg/Btn_Close").GetComponent<Button>();
            m_leftInfo = AddComponent<UI_TerroristEnter_Left>(transform.Find("Animator/View_Left"));
            //m_rightInfo = AddComponent<UI_TerroristEnter_Right>(transform.Find("Animator/View_Right"));

            m_ImgPic = transform.Find("Animator/View_Right/Image_Bgmask/Image_Bg (1)").GetComponent<Image>();
            m_BtnAgree = transform.Find("Animator/View_Right/Btnlist/Btn_01").GetComponent<Button>();
            m_BtnRefuse = transform.Find("Animator/View_Right/Btnlist/Btn_02").GetComponent<Button>();
            m_BtnCancel = transform.Find("Animator/View_Right/Btnlist/Btn_03").GetComponent<Button>();

            m_ImgTime = transform.Find("Animator/View_Right/Image_Time/Image_Time").GetComponent<Image>();
            m_TextTime = transform.Find("Animator/View_Right/Image_Time/Text").GetComponent<Text>();

            m_TextWaitTeam = transform.Find("Animator/View_Right/Text").GetComponent<Text>();

            m_BtnClose.onClick.AddListener(() => { this.CloseSelf();});
            m_BtnAgree.onClick.AddListener(OnClickAgree);
            m_BtnRefuse.onClick.AddListener(OnClickRefuse);
            m_BtnCancel.onClick.AddListener(OnClickCancel);

            TimeCount = float.Parse(Table.CSVParam.Instance.GetConfData(350).str_value) / 1000f;

            m_TextCount = transform.Find("Animator/View_Right/Text1").GetComponent<Text>();
            m_TextComplete = transform.Find("Animator/View_Right/Text2").GetComponent<Text>();
        }
        //protected virtual void OnUpdate() { }
        //protected virtual void OnLateUpdate() { }

        protected override void OnShow()
        {
            if (mems != null)
            {
                UpdateInfo();

                m_Timer?.Cancel();
                m_Timer = Lib.Core.Timer.Register(TimeCount, () =>{

                    m_Timer.Cancel();
                    m_TextTime.text = "0";
                    m_ImgTime.fillAmount = 0f;

                }, (elapse) =>{

                    float lefTime = TimeCount - elapse;
                    m_TextTime.text = Mathf.RoundToInt(lefTime).ToString();
                    m_ImgTime.fillAmount = lefTime / TimeCount;
                });
            }
            else
            {
                m_TextTime.text = "0";
                Debug.LogError("TerrorSeriesMemItems is null");
            }
        }

        protected override void OnHide()
        {
            m_Timer?.Cancel();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_TerrorSeries.Instance.eventEmitter.Handle<ulong>(Sys_TerrorSeries.EEvents.OnNtfWeekTaskVote, OnVoteNtf, toRegister);
            Sys_TerrorSeries.Instance.eventEmitter.Handle<TerrorSeriesMemItems>(Sys_TerrorSeries.EEvents.OnUpdateVoteNtf, OnUpdateVoteNtf, toRegister);
        }

        private void OnClickAgree()
        {
            Sys_TerrorSeries.Instance.OnDoVoteReq(true);
        }

        private void OnClickRefuse()
        {
            Sys_TerrorSeries.Instance.OnDoVoteReq(false);
        }

        private void OnClickCancel()
        {
            Sys_TerrorSeries.Instance.OnDoVoteCancel();
        }

        private void OnVoteNtf(ulong roleId)
        {
            m_leftInfo.OnVoteNtf(roleId);

            UpdateBtnState();
        }

        private void OnUpdateVoteNtf(TerrorSeriesMemItems mesntf)
        {
            this.mems = mesntf;
            m_leftInfo.UpdateInfo(mems);
        }

        private void UpdateInfo()
        {
            m_leftInfo.UpdateInfo(mems);

            uint totalCount = 5;
            uint count = Sys_TerrorSeries.Instance.GetWeekTime(this.mems.InstanceID);
            m_IsCompleted = count >= totalCount;

            m_TextCount.text = LanguageHelper.GetTextContent(1006072, count.ToString(), totalCount.ToString());
            m_TextComplete.gameObject.SetActive(m_IsCompleted);
            
            CSVInstance.Data instanceData = CSVInstance.Instance.GetConfData(mems.InstanceID);
            ImageHelper.SetIcon(m_ImgPic, instanceData.bg);

            UpdateBtnState();
        }

        private void UpdateBtnState()
        {
            m_BtnAgree.gameObject.SetActive(false);
            m_BtnRefuse.gameObject.SetActive(false);
            m_BtnCancel.gameObject.SetActive(false);
            m_TextWaitTeam.gameObject.SetActive(false);

            if (Sys_TerrorSeries.Instance.GetVoteOp(Sys_Role.Instance.RoleId) == VoterOpType.Agree)
            {
                m_BtnCancel.gameObject.SetActive(true);
                m_TextWaitTeam.gameObject.SetActive(true);
            }
            else
            {
                m_BtnAgree.gameObject.SetActive(true);
                m_BtnRefuse.gameObject.SetActive(true);
            }
        }
    }
}


