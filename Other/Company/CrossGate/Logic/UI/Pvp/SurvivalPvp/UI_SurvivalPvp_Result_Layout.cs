using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;


namespace Logic
{
    public partial class UI_SurvivalPvp_Result_Layout
    {
        private Button m_BtnClose;

        private Transform m_TransSuccess;
        private Transform m_TransFail;


        private Text m_TexCurSocre;
        private Text m_TexTotalSocre;

        public Transform m_TransReward;
        public ClickItemGroup<RewardPropItem> m_RewardGroup = new ClickItemGroup<RewardPropItem>();

        public Text m_TexTimes;
        public void Load(Transform root)
        {

            m_TransSuccess = root.Find("Animator/Image_Successbg");
            m_TransFail = root.Find("Animator/Image_Failedbg");

            m_BtnClose = root.Find("Image_Black").GetComponent<Button>();

            m_TexCurSocre = root.Find("Animator/This Time/Value").GetComponent<Text>();
            m_TexTotalSocre = root.Find("Animator/Total/Value").GetComponent<Text>();


            m_TransReward = root.Find("Animator/RewardNode");
            m_RewardGroup.AddChild(m_TransReward.Find("Scroll View/Viewport/Content/PropItem"));
            m_TexTimes = m_TransReward.Find("Text_Tips").GetComponent<Text>();

        }

        public void SetListener(IListener listener)
        {
            m_BtnClose.onClick.AddListener(listener.OnClickClose);
        }


        public void SetScore(int curScore, uint totalScore)
        {
            m_TexCurSocre.text = curScore.ToString();
            m_TexTotalSocre.text = totalScore.ToString();
        }

        public void SetSuccess(uint result)
        {
            bool isSuccess = result == 0;

            if (m_TransSuccess.gameObject.activeSelf != isSuccess)
                m_TransSuccess.gameObject.SetActive(isSuccess);

            bool isFail = result == 1;
            if (m_TransFail.gameObject.activeSelf != isFail)
                m_TransFail.gameObject.SetActive(isFail);


        }

    }

    /// <summary>
    /// 操作接口IListener
    /// </summary>
    public partial class UI_SurvivalPvp_Result_Layout
    {
        public interface IListener
        {
            void OnClickClose();
        }
    }

}
