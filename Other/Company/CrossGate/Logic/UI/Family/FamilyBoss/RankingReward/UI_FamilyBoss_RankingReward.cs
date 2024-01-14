using Logic.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Table;
using UnityEngine.UI;
using UnityEngine;


namespace Logic
{
    public class UI_FamilyBoss_RankingReward : UIBase, UI_FamilyBoss_RankingReward_Type.IListener
    {
        private UI_FamilyBoss_RankingReward_Type m_RankType;
        private UI_FamilyBoss_RankingReward_Right m_Right;

        protected override void OnLoaded()
        {
            Button btnClose = transform.Find("Animator/View_TipsBg01_Square/Btn_Close").GetComponent<Button>();
            btnClose.onClick.AddListener(OnClickClose);

            m_RankType = new UI_FamilyBoss_RankingReward_Type();
            m_RankType.Init(transform.Find("Animator/ScrollView_Menu"));
            m_RankType.Register(this);

            m_Right = new UI_FamilyBoss_RankingReward_Right();
            m_Right.Init(transform.Find("Animator/Scroll_Rank"));
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
            //OnProcessEventsForEnable(toRegister);
        }

        private void OnClickClose()
        {
            CloseSelf();
        }

        public void OnType(int index)
        {
            m_Right.OnRewardType(index);
        }
    }
}