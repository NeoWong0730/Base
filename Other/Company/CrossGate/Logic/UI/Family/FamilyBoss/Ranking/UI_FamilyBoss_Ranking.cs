using Logic.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Table;
using UnityEngine.UI;
using UnityEngine;


namespace Logic
{
    public class UI_FamilyBoss_Ranking : UIBase, UI_FamilyBoss_Ranking_Type.IListener
    {
        private UI_FamilyBoss_Ranking_Type m_RankType;
        private UI_FamilyBoss_Ranking_Personal m_Personal;
        private UI_FamilyBoss_Ranking_Family m_Family;

        private int RankType;
        protected override void OnLoaded()
        {
            Button btnClose = transform.Find("Animator/View_TipsBgNew05/Btn_Close").GetComponent<Button>();
            btnClose.onClick.AddListener(OnClickClose);

            m_RankType = new UI_FamilyBoss_Ranking_Type();
            m_RankType.Init(transform.Find("Animator/ScrollView_Menu"));
            m_RankType.Register(this);

            m_Personal = new UI_FamilyBoss_Ranking_Personal();
            m_Personal.Init(transform.Find("Animator/View_Personal"));

            m_Family = new UI_FamilyBoss_Ranking_Family();
            m_Family.Init(transform.Find("Animator/View_Family"));
        }

        protected override void OnDestroy()
        {
            //ui_CurrencyTitle.Dispose();
        }
        protected override void OnOpen(object arg)
        {
            //eApplyFamilyMenu = null == arg ? EApplyFamilyMenu.Join : (EApplyFamilyMenu)System.Convert.ToInt32(arg);
        }

        protected override void OnShow()
        {
            m_RankType.OnSelect(0);
        }

        protected override void OnHide()
        {

        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_FamilyBoss.Instance.eventEmitter.Handle(Sys_FamilyBoss.EEvents.OnBossRankInfo, this.OnBossRankInfo, toRegister);
        }

        private void OnClickClose()
        {
            CloseSelf();
        }

        public void OnType(int index)
        {
            RankType = index;
            if (RankType == 0)
            {
                m_Family.OnHide();
                m_Personal.OnShow();
            }
            else
            {
                m_Personal.OnHide();
                m_Family.OnShow();
            }
            Sys_FamilyBoss.Instance.OnGuildBossRankInfoReq((uint)(index + 1));
        }

        private void OnBossRankInfo()
        {
            if (RankType == 0)
            {
                //m_Family.OnHide();
                //m_Personal.OnShow();
                m_Personal.UpdateInfo();
            }
            else
            {
                //m_Personal.OnHide();
                //m_Family.OnShow();
                m_Family.UpdateInfo();
            }
        }
    }
}