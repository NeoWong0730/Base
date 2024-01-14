using Logic.Core;
using System;
using System.Collections.Generic;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_Pvp_MatcheNPCTip : UIComponent
    {
        private UI_Pvp_MatcheNPCTip_Layout mLayout = new UI_Pvp_MatcheNPCTip_Layout();

        protected override void Loaded()
        {
            mLayout.Load(gameObject.transform);
        }

        public override void Hide()
        {
            base.Hide();
        }

        public override void Show()
        {
            base.Show();
            mLayout.PlayAnim();
        }
    }


    public class UI_Pvp_MatcheNPCTip_Layout
    {
        private Animator m_Animator;

        private Text m_TexTips;
        public void Load(Transform root)
        {
            m_Animator = root.Find("Image1").GetComponent<Animator>();

            m_TexTips = root.Find("Image1/Text").GetComponent<Text>();
        }

        public void PlayAnim()
        {
            m_Animator.Play("Open");
        }
    }
}
