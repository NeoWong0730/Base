using Logic.Core;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
	public class UI_Underground_Opponent_Layout
    {
        public class InfoCard : ClickItem
        {
            public Image m_ImgHeadFrame;
            public Image m_ImgHead;
            public Text m_TexProfession;
            public Image m_ImgProfession;
            public Text m_TexLevel;

            public Image m_ImgPetHeadFrame;
            public Image m_ImgPetHead;
            public Text m_TexPetLevel;


            public override void Load(Transform root)
            {
                base.Load(root);

                m_ImgHeadFrame = root.Find("Image_Head").GetComponent<Image>();
                m_ImgHead = root.Find("Image_Head/Image_Icon").GetComponent<Image>();
                m_TexProfession = root.Find("Text_Profession").GetComponent<Text>();
                m_ImgProfession = root.Find("Text_Profession/Image_Prop").GetComponent<Image>();
                m_TexLevel = root.Find("Text_Number").GetComponent<Text>();

                m_ImgPetHeadFrame = root.Find("PetHead").GetComponent<Image>();
                m_ImgPetHead = root.Find("PetHead/Image_Icon").GetComponent<Image>();
                m_TexPetLevel = root.Find("PetHead/Image_Lv/Text").GetComponent<Text>();

            }
        }

        public interface IListener
        {
            void OnClickClose();
            void OnClickVideo();
            void OnClickGo();

            void OnInfinityCreate(InfinityGridCell cell);
            void OnInfinityChange(InfinityGridCell cell,int index);
        }

        public Button m_BtnClose;
        public Button m_BtnVideo;
		public Button m_BtnGo;

        public InfinityGrid m_CardInfinity;
		public void Load(Transform root)
		{
            m_BtnClose = root.Find("Animator/View_TipsBgNew05/Btn_Close").GetComponent<Button>();

            m_BtnVideo = root.Find("Animator/View_Content/Button_Watch").GetComponent<Button>();
            m_BtnGo = root.Find("Animator/View_Content/Button_Go").GetComponent<Button>();


            m_CardInfinity = root.Find("Animator/View_Content/Scroll_View_Gem").GetComponent<InfinityGrid>();
        }

        public void SetListener(IListener listener)
        {
            m_BtnClose.onClick.AddListener(listener.OnClickClose);
            m_BtnVideo.onClick.AddListener(listener.OnClickVideo);
            m_BtnGo.onClick.AddListener(listener.OnClickGo);


            m_CardInfinity.onCreateCell = listener.OnInfinityCreate;
            m_CardInfinity.onCellChange = listener.OnInfinityChange;
        }
	}
}