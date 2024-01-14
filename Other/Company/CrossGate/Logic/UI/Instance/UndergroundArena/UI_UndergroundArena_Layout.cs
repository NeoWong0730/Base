using Logic.Core;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
	public class UI_UndergroundArena_Layout
	{
        public class VsInfoCard : ClickItem
        {
            public Image m_ImgOwnHeadFrame;
            public Image m_ImgOwnHead;
            public Text m_TexOwnName;
            public Button m_BtnOwnIcon;

            public Image m_ImgOtherHeadFrame;
            public Image m_ImgOtherHead;
            public Text m_TexOtherName;
            public Button m_BtnOtherIcon;

            public Text m_TexCardName;

            public Transform m_TransVS;
            public Transform m_TransKO;

           // public Button m_BtnOther;

            public Action<uint> ClickOtherAc;
            public Action<uint> ClickOwnAc;

            public uint ID = 0;

            public Button m_BtnSelect;
            public Button m_BtnSelectKo;
            public override void Load(Transform root)
            {
                base.Load(root);

                m_ImgOwnHeadFrame = root.Find("Image_Head").GetComponent<Image>();
                m_ImgOwnHead = root.Find("Image_Head/Image_Icon").GetComponent<Image>();
                m_TexOwnName = root.Find("Image_Head/Text_name").GetComponent<Text>();
                m_BtnOwnIcon = root.Find("Image_Head/Image_Icon").GetComponent<Button>();

                m_ImgOtherHeadFrame = root.Find("Image_Head1").GetComponent<Image>();
                m_ImgOtherHead = root.Find("Image_Head1/Image_Icon").GetComponent<Image>();
                m_TexOtherName = root.Find("Image_Head1/Text_name").GetComponent<Text>();
                m_BtnOtherIcon = root.Find("Image_Head1/Image_Icon").GetComponent<Button>();

                m_TexCardName = root.Find("Text").GetComponent<Text>();

                m_TransVS = root.Find("Image_Vs");
                m_TransKO = root.Find("Image_Ko");

                m_BtnSelect = m_TransVS.GetComponent<Button>();
                m_BtnSelectKo = m_TransKO.GetComponent<Button>();

                // m_BtnOtherIcon.onClick.AddListener(OnClickOther);
                // m_BtnOwnIcon.onClick.AddListener(OnClickOwn);

                m_BtnSelect.onClick.AddListener(OnClickOther);
                m_BtnSelectKo.onClick.AddListener(OnClickOwn);
            }

            private void OnClickOther()
            {
                ClickOtherAc?.Invoke(ID);
            }

            private void OnClickOwn()
            {
                ClickOwnAc?.Invoke(ID);
            }
        }

        public interface IListener
        {
            void OnClickClose();
            void OnClickReward();
            void OnClickRank();
            void OnClickFastTeam();
            void OnClickGoTo();

            void OnInfinityCreate(InfinityGridCell cell);
            void OnInfinityChange(InfinityGridCell cell,int index);

            void OnClickLowSideIcon(uint id);
            void OnClickUpSideIcon(uint id);
        }
        Button m_BtnClose;
        Button m_BtnReward;
		Button m_BtnRank;

		public Text m_TexName;
		public Text m_TexTime;

		Button m_BtnFastTeam;
		Button m_BtnGoTo;

        public InfinityGrid m_CardInfinity;

        public Text m_TexMaxStage;

		public Transform m_TransScoreTips;
		public Text m_TexScore;

		public void Load(Transform root)
		{
            m_BtnClose = root.Find("Animator/View_Title01/Btn_Close").GetComponent<Button>();
            m_BtnReward = root.Find("Animator/View_Top/Image_BG/Btn_Awrad").GetComponent<Button>();
            m_BtnRank = root.Find("Animator/View_Top/Image_BG/Btn_Rank").GetComponent<Button>();
            m_TexName = root.Find("Animator/View_Top/Image_BG/Tex_Name").GetComponent<Text>();

            m_TexTime = root.Find("Animator/View_Top/Time/Tex").GetComponent<Text>();

            m_BtnFastTeam = root.Find("Animator/View_Middle/Btn_Team").GetComponent<Button>();
            m_BtnGoTo = root.Find("Animator/View_Middle/Btn_Go").GetComponent<Button>();

            m_CardInfinity = root.Find("Animator/View_Middle/Scroll View").GetComponent<InfinityGrid>();

            m_TexMaxStage = root.Find("Animator/View_Middle/Text").GetComponent<Text>();

            m_TransScoreTips = root.Find("Animator/View_Middle/Text1");
            m_TexScore = m_TransScoreTips.Find("Text").GetComponent<Text>();
        }

        public void SetListener(IListener listener)
        {
            m_BtnClose.onClick.AddListener(listener.OnClickClose);
            m_BtnReward.onClick.AddListener(listener.OnClickReward);
            m_BtnRank.onClick.AddListener(listener.OnClickRank);
            m_BtnFastTeam.onClick.AddListener(listener.OnClickFastTeam);
            m_BtnGoTo.onClick.AddListener(listener.OnClickGoTo);

            m_CardInfinity.onCreateCell = listener.OnInfinityCreate;
            m_CardInfinity.onCellChange = listener.OnInfinityChange;
        }
	}
}