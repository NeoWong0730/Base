using System;
using UnityEngine;
using UnityEngine.UI;
using Lib.Core;
using Framework;

namespace Logic
{
    public class UIButtonCD : IDisposable
    {
        public Transform transform;
        public Action<bool> OnClick;

        private Button m_btn;
        private Text m_text;
        private uint lanKey;

        private uint m_endCd;
        private Timer m_Timer;
        private uint m_leftTime;

        public void Init(Transform trans)
        {
            transform = trans;
            m_btn = transform.GetComponent<Button>();
            m_btn.onClick.AddListener(OnClickBtn);
            m_text = transform.Find("Text").GetComponent<Text>();
            lanKey = m_text.GetComponent<SurfaceLanguage>().key;
        }

        private void OnClickBtn()
        {
            bool isInCd = m_leftTime > 0;
            OnClick?.Invoke(isInCd);
        }

        public void Start(uint endCd)
        {
            m_endCd = endCd;
            m_leftTime = 0;
            uint serverTime = Sys_Time.Instance.GetServerTime();
            if (serverTime < m_endCd)
            {
                m_leftTime = m_endCd - serverTime;
                ImageHelper.SetImageGray(m_btn.image, true);
                m_text.text = LanguageHelper.TimeToString(m_leftTime, LanguageHelper.TimeFormat.Type_1);

                m_Timer?.Cancel();
                m_Timer = null;
                m_Timer = Timer.Register(1f, () =>
                {
                    m_leftTime--;
                    if (m_leftTime <= 0u)
                    {
                        ShowDefualt();
                    }
                    else
                    {
                        ImageHelper.SetImageGray(m_btn.image, true);
                        m_text.text = LanguageHelper.TimeToString(m_leftTime, LanguageHelper.TimeFormat.Type_1);
                    }
                }, null, true);
            }
            else
            {
                ShowDefualt();
            }
        }

        private void ShowDefualt()
        {
            m_Timer?.Cancel();
            m_Timer = null;
            m_leftTime = 0;
            TextHelper.SetText(m_text, lanKey);
            ImageHelper.SetImageGray(m_btn.image, false);
        }

        public void Dispose()
        {
            ShowDefualt();
        }
    }
}


