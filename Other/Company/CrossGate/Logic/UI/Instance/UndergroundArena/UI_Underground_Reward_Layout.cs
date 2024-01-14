using Logic.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
	public class UI_Underground_Reward_Layout
    {


        public interface IListener
        {
            void OnClickClose();

        }

        public Text m_TexTitle;
        public Text m_TexDes;
        public Text m_TexRewardName;
        private Button m_BtnClose;

        public ClickItemGroup<RewardPropItem> m_FristGroup = new ClickItemGroup<RewardPropItem>();
        public ClickItemGroup<RewardPropItem> m_SingleGroup = new ClickItemGroup<RewardPropItem>();
        public void Load(Transform root)
		{
            m_TexTitle = root.Find("Animator/View_Content/Image/Text").GetComponent<Text>();
            m_TexDes = root.Find("Animator/View_Content/Text_Des").GetComponent<Text>();
            m_TexRewardName = root.Find("Animator/View_Content/Image_Award/Text").GetComponent<Text>();

            m_BtnClose = root.Find("Animator/View_TipsBgNew07/Btn_Close").GetComponent<Button>();

            m_FristGroup.AddChild(root.Find("Animator/View_Content/Award/Scroll_View/Viewport/PropItem"));
            m_SingleGroup.AddChild(root.Find("Animator/View_Content/Award (1)/Scroll_View/Viewport/PropItem"));
        }

        public void SetListener(IListener listener)
        {
            m_BtnClose.onClick.AddListener(listener.OnClickClose);
        }
	}
}