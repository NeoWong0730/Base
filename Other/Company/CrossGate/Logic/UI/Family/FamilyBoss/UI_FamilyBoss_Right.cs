
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;
using Logic.Core;
using Table;
using Lib.Core;

namespace Logic
{
    public class UI_FamilyBoss_Right : UI_FamilyBoss_Right_Type.IListener
    {
        private Transform transform;

        private Button m_btnRankReward;
        private Text m_textScorePower; //积分倍率
        private Slider m_sliderCountDownPower; //倒计时进度条
        private Text m_textCountDownPower;
        private Text m_textCountDownActivity;

        private UI_FamilyBoss_Right_Type m_Type;
        private UI_FamilyBoss_Right_Personal m_Personal;
        private UI_FamilyBoss_Right_Family m_Family;

        private Text m_textMyRank;
        private Text m_textUnRank;
        private Text m_textMyScore;

        private bool isFirst = false;
        private int m_RnakType = 0;
        private IListener m_Listener;

        private Timer m_TimerBossEnd;
        private Timer m_TimerPower;
        private Timer m_TimerSlider;

        public void Init(Transform trans)
        {
            transform = trans;

            m_btnRankReward = transform.Find("Image1/Button_Rank").GetComponent<Button>();
            m_btnRankReward.onClick.AddListener(OnClickRankReward);

            m_textScorePower = transform.Find("Image1/Text2").GetComponent<Text>();
            m_sliderCountDownPower = transform.Find("Image1/Slider_Star").GetComponent<Slider>();
            m_textCountDownPower = transform.Find("Image1/Text_Time").GetComponent<Text>();
            m_textCountDownActivity = transform.Find("Text_Time").GetComponent<Text>();

            m_Type = new UI_FamilyBoss_Right_Type();
            m_Type.Init(transform.Find("Menu"));
            m_Type.Register(this);

            m_Personal = new UI_FamilyBoss_Right_Personal();
            m_Personal.Init(transform.Find("Personal"));

            m_Family = new UI_FamilyBoss_Right_Family();
            m_Family.Init(transform.Find("Family"));

            m_textMyRank = transform.Find("My/Rank/Text").GetComponent<Text>();
            m_textUnRank = transform.Find("My/Rank/Text1").GetComponent<Text>();
            m_textMyScore = transform.Find("My/Score/Text").GetComponent<Text>();
        }

        public void OnShow()
        {
            transform.gameObject.SetActive(true);
        }

        public void OnHide()
        {
            isFirst = false;
            transform.gameObject.SetActive(false);
        }

        public void OnDispose()
        {
            StopTimer();
        }

        private void StopTimer()
        {
            m_TimerBossEnd?.Cancel();
            m_TimerBossEnd = null;
            m_TimerPower?.Cancel();
            m_TimerPower = null;
            m_TimerSlider?.Cancel();
            m_TimerSlider = null;
        }

        public void OnType(int index)
        {
            m_RnakType = index;
            if (index == 1u)
            {
                m_Personal.OnHide();
                m_Family.OnShow();
                m_Family.UpdateInfo();
            }
            else
            {
                m_Family.OnHide();
                m_Personal.OnShow();
                m_Personal.UpdateInfo();
            }

            m_Listener?.OnSelectRankType(index);
        }

        private void OnClickRankReward()
        {
            UIManager.OpenUI(EUIID.UI_FamilyBoss_RankingReward);
        }

        public void Register(IListener listner)
        {
            m_Listener = listner;
        }

        public void UpdateBossInfo()
        {
            if (Sys_FamilyBoss.Instance.MyRank != 0)
            {
                m_textUnRank.gameObject.SetActive(false);
                m_textMyRank.text = Sys_FamilyBoss.Instance.MyRank.ToString();
            }
            else
            {
                m_textUnRank.gameObject.SetActive(true);
                m_textMyRank.text = "";
            }
            
            m_textMyScore.text = Sys_FamilyBoss.Instance.MyScore.ToString();

            StopTimer();
            BossEndTimer();
            PowerEndTimer();
            BossEndSlider();

            if (!isFirst)
                m_Type.OnSelect(0);
            isFirst = true;
        }

        private void BossEndTimer()
        {
            uint bossLeftTime = Sys_FamilyBoss.Instance.GetLeftTime();
            if (bossLeftTime > 0)
            {
                m_TimerBossEnd = Timer.Register(1f, () =>
                {
                    bossLeftTime--;
                    if (bossLeftTime == 0)
                    {
                        m_TimerBossEnd.Cancel();
                    }

                    m_textCountDownActivity.text = LanguageHelper.TimeToString(bossLeftTime, LanguageHelper.TimeFormat.Type_1);
                }, null, true);
            }
            m_textCountDownActivity.text = LanguageHelper.TimeToString(bossLeftTime, LanguageHelper.TimeFormat.Type_1);
        }

        private void PowerEndTimer()
        {
            m_textScorePower.text = string.Format("x {0}", Sys_FamilyBoss.Instance.GetScorePower().ToString());
            uint leftTime = 0u;
            uint sliderTime = 0u;
            float percent = leftTime * 1.0f / sliderTime;
            if (Sys_FamilyBoss.Instance.GetPowerLeftTime(ref leftTime, ref sliderTime))
            {
                m_TimerPower = Timer.Register(1f, () =>
                {
                    leftTime--;
                    if (leftTime == 0)
                    {
                        m_TimerPower.Cancel();
                        PowerEndTimer();
                    }

                    m_textCountDownPower.text = LanguageHelper.TimeToString(leftTime, LanguageHelper.TimeFormat.Type_1);
                }, null, true);
            }

            m_textCountDownPower.text = LanguageHelper.TimeToString(leftTime, LanguageHelper.TimeFormat.Type_1);
        }

        private void BossEndSlider()
        {
            uint leftTime = 0u;
            uint sliderTime = 0u;
            float percent = 0f;
            if (Sys_FamilyBoss.Instance.GetPowerLeftTime(ref leftTime, ref sliderTime))
            {
                percent = leftTime * 1.0f / sliderTime;
                m_TimerSlider = Timer.Register(1f, () =>
                {
                    leftTime--;
                    if (leftTime == 0)
                    {
                        m_TimerSlider.Cancel();
                        BossEndSlider();
                    }

                    percent = leftTime * 1.0f / sliderTime;
                    m_sliderCountDownPower.value = percent;
                }, null, true);
            }
            m_sliderCountDownPower.value = percent;
        }

        public void UpdateRankInfo()
        {
            if (m_RnakType == 0u)
                m_Personal.UpdateInfo();
            else
                m_Family.UpdateInfo();
        }

        public void UpdateAllRankInfo()
        {
            m_Personal.UpdateInfo();
            m_Family.UpdateInfo();
        }

        public interface IListener
        {
            void OnSelectRankType(int index);
        }
    }
}