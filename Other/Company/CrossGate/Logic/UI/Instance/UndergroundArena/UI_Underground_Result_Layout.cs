using Logic.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
	public class UI_Underground_Result_Layout
    {
  

        public interface IListener
        {
            void OnClickClose();
        }

        Button m_BtnClose;
        public Text m_TexTitle;
        public Text m_TexStage;

        public Text m_TexNoRewardInfo;

        public Image m_ImgOwnIcon;
        public Text m_TexOwnName;
        public Button m_BtnOwn;

        public Image m_ImgOtherIcon;
        public Text m_TexOtherName;
        public Button m_BtnOther;

        public Transform m_TransFristAward;
        public ClickItemGroup<RewardPropItem> m_FristRewareGroup = new ClickItemGroup<RewardPropItem>();
        public ClickItemGroup<RewardPropItem> m_FristNormalRewareGroup = new ClickItemGroup<RewardPropItem>();

        public Transform m_TransNormalAward;
        public ClickItemGroup<RewardPropItem> m_NormalRewareGroup = new ClickItemGroup<RewardPropItem>();


		public void Load(Transform root)
		{
            m_BtnClose = root.Find("Image_Black").GetComponent<Button>();
            m_TexTitle = root.Find("Animator/View_BG/Text_Title").GetComponent<Text>();
            m_TexStage = root.Find("Animator/View_BG/Image_BG/View_Content/Text").GetComponent<Text>();

            m_TexNoRewardInfo = root.Find("Animator/View_BG/Image_BG/View_Content/Text_Tips").GetComponent<Text>();

            m_ImgOwnIcon = root.Find("Animator/View_BG/Image_BG/View_Content/Result/Image_Head/Image_Icon").GetComponent<Image>();
            m_TexOwnName = root.Find("Animator/View_BG/Image_BG/View_Content/Result/Image_Head/Text").GetComponent<Text>();
            m_BtnOwn = root.Find("Animator/View_BG/Image_BG/View_Content/Result/Image_Head/Image_Icon").GetComponent<Button>();

            m_ImgOtherIcon = root.Find("Animator/View_BG/Image_BG/View_Content/Result/Image_Head (1)/Image_Icon").GetComponent<Image>();
            m_TexOtherName = root.Find("Animator/View_BG/Image_BG/View_Content/Result/Image_Head (1)/Text").GetComponent<Text>();
            m_BtnOther = root.Find("Animator/View_BG/Image_BG/View_Content/Result/Image_Head (1)/Image_Icon").GetComponent<Button>();

            m_TransFristAward = root.Find("Animator/View_BG/Image_BG/View_Content/Award1");
            m_FristRewareGroup.AddChild(m_TransFristAward.Find("Left/Scroll_View/Viewport/PropItem"));
            m_FristNormalRewareGroup.AddChild(m_TransFristAward.Find("Right/Scroll_View/Viewport/PropItem"));

            m_TransNormalAward = root.Find("Animator/View_BG/Image_BG/View_Content/Award");
            m_NormalRewareGroup.AddChild(m_TransNormalAward.Find("Scroll_View/Viewport/PropItem"));
        }

        public void SetListener(IListener listener)
        {
            m_BtnClose.onClick.AddListener(listener.OnClickClose);

        }
	}
}