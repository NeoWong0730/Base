using Logic.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Table;
using UnityEngine.UI;
using UnityEngine;


namespace Logic
{
    public class UI_FamilyBoss_Result : UIBase
    {
        public class BossResult
        {
            private Transform transform;

            private Image m_imgHead;
            private Text m_textDmg;
            private Text m_textScore;
            public void Init(Transform trans)
            {
                transform = trans;

                m_imgHead = transform.Find("Head").GetComponent<Image>();
                m_textDmg = transform.Find("Hurt/Text1").GetComponent<Text>();
                m_textScore = transform.Find("Score/Text1").GetComponent<Text>();
            }

            public void OnShow()
            {
                transform.gameObject.SetActive(true);

                UpdateInfo();
            }

            public void OnHide()
            {
                transform.gameObject.SetActive(false);
            }

            private void UpdateInfo()
            {
                CharacterHelper.SetHeadAndFrameData(m_imgHead, Sys_Role.Instance.HeroId, Sys_Head.Instance.clientHead.headId, Sys_Head.Instance.clientHead.headFrameId);
                m_textDmg.text = Sys_FamilyBoss.Instance.AttackDmg.ToString();
                m_textScore.text = Sys_FamilyBoss.Instance.AttackAddScore.ToString();
            }
        }

        public class SeizeResultSuccess
        {
            private Transform transform;

            private Text m_textScore;
            public void Init(Transform trans)
            {
                transform = trans;

                m_textScore = transform.Find("Image/Text").GetComponent<Text>();
            }

            public void OnShow()
            {
                transform.gameObject.SetActive(true);

                UpdateInfo();
            }

            public void OnHide()
            {
                transform.gameObject.SetActive(false);
            }

            private void UpdateInfo()
            {
                m_textScore.text = LanguageHelper.GetTextContent(3910010312, Sys_FamilyBoss.Instance.AttackAddScore.ToString());
            }
        }

        public class SeizeResultFailure
        {
            private Transform transform;

            private Text m_textScore;
            public void Init(Transform trans)
            {
                transform = trans;

                m_textScore = transform.Find("Image/Text").GetComponent<Text>();
                //m_textScore.gameObject.SetActive(false);
            }

            public void OnShow()
            {
                transform.gameObject.SetActive(true);

                UpdateInfo();
            }

            public void OnHide()
            {
                transform.gameObject.SetActive(false);
            }

            private void UpdateInfo()
            {
                m_textScore.text = LanguageHelper.GetTextContent(3910010312, "0");
            }
        }

        private BossResult m_bossResult;
        private SeizeResultSuccess m_seizeSuccess;
        private SeizeResultFailure m_seizeFailure;

        protected override void OnLoaded()
        {
            Button btnClose = transform.Find("Image_Black").GetComponent<Button>();
            btnClose.onClick.AddListener(OnClickClose);

            m_bossResult = new BossResult();
            m_bossResult.Init(transform.Find("Animator/Image_End"));

            m_seizeSuccess = new SeizeResultSuccess();
            m_seizeSuccess.Init(transform.Find("Animator/Image_Successbg"));

            m_seizeFailure = new SeizeResultFailure();
            m_seizeFailure.Init(transform.Find("Animator/Image_Failedbg"));
        }

        protected override void OnDestroy()
        {

        }
        protected override void OnOpen(object arg)
        {
            
        }

        protected override void OnShow()
        {
            UpdateInfo();
        }

        protected override void OnHide()
        {

        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            //Sys_FamilyBoss.Instance.eventEmitter.Handle<uint, uint>(Sys_FamilyBoss.EEvents.OnBossSimpleInfo, this.OnBossSimpleInfo, toRegister);
        }

        private void OnClickClose()
        {
            CloseSelf();

            UIManager.OpenUI(EUIID.UI_FamilyBoss);
        }

        private void UpdateInfo()
        {
            m_bossResult.OnHide();
            m_seizeSuccess.OnHide();
            m_seizeFailure.OnHide();

            uint result = Sys_FamilyBoss.Instance.AttackEndRet;
            switch (result) //0:打boss 打人有成功失败 1:成功 2:失败
            {
                case 0:
                    m_bossResult.OnShow();
                    break;
                case 1:
                    m_seizeSuccess.OnShow();
                    break;
                case 2:
                    m_seizeFailure.OnShow();
                    break;
            }
        }
    }
}