using Logic.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
	public class UI_Underground_Rank_Layout
    {
        public class RankInfoCard : ClickItem
        {
            public Image m_ImgRank;
            public Text m_TexRank;
            public Text m_TexName;
            public Text m_TexType;
            public Text m_TexTime;

            
            public override void Load(Transform root)
            {
                base.Load(root);

                m_ImgRank = root.Find("Rank/Image_Icon").GetComponent<Image>();
                m_TexRank = root.Find("Rank/Text_Rank").GetComponent<Text>();
                m_TexName = root.Find("Text_Name").GetComponent<Text>();
                m_TexType = root.Find("Text_Type").GetComponent<Text>();
                m_TexTime = root.Find("Text_Family").GetComponent<Text>();
            }
        }


        public class OwnRankInfoCard
        {
            public Image m_ImgRank;
            public Text m_TexRank;
            public Text m_TexName;
            public Text m_TexType;
            public Text m_TexTime;

            public Text m_TexNoRank;
            public  void Load(Transform root)
            {
                
                m_ImgRank = root.Find("Image_Icon").GetComponent<Image>();
                m_TexRank = root.Find("Text_Rank").GetComponent<Text>();
                m_TexName = root.Find("Text_Name").GetComponent<Text>();
                m_TexType = root.Find("Text_Type").GetComponent<Text>();
                m_TexTime = root.Find("Text_Time").GetComponent<Text>();
                m_TexNoRank = root.Find("Text_unlist").GetComponent<Text>();
            }
        }

        public interface IListener
        {

            void OnClickClose();
            void OnInfinityCreate(InfinityGridCell cell);
            void OnInfinityChange(InfinityGridCell cell,int index);

            void OnToggleChange(int curid, int lastid);
        }

        public Button m_BtnClose;


        public InfinityGrid m_CardInfinity;

        public List<CP_Toggle> m_TogListChoice = new List<CP_Toggle>();

        CP_ToggleRegistry m_ToggleGroup;

        public OwnRankInfoCard m_PlayerRank = new OwnRankInfoCard();

        public Transform m_TransNoRank;
        public void Load(Transform root)
		{
            m_BtnClose = root.Find("Animator/View_TipsBgNew05/Btn_Close").GetComponent<Button>();

            m_CardInfinity = root.Find("Animator/View_Content/Scroll_Rank").GetComponent<InfinityGrid>();

            var toggoleparent = root.Find("Animator/ScrollView_Menu/List");

            m_ToggleGroup = toggoleparent.GetComponent<CP_ToggleRegistry>();

            int togglecount = toggoleparent.childCount;

            for (int i = 1; i <= togglecount; i++)
            {
                string itemname = i.ToString();
                var toggleitem = toggoleparent.Find(itemname).GetComponent<CP_Toggle>();
                if (toggleitem != null)
                {
                    toggleitem.id = m_TogListChoice.Count;
                    m_TogListChoice.Add(toggleitem);
                }          
            }

            m_PlayerRank.Load(root.Find("Animator/View_Content/MyRank"));

            m_TransNoRank = root.Find("Animator/View_None");
        }

        public void SetListener(IListener listener)
        {

            m_CardInfinity.onCreateCell = listener.OnInfinityCreate;
            m_CardInfinity.onCellChange = listener.OnInfinityChange;

            m_BtnClose.onClick.AddListener(listener.OnClickClose);

            m_ToggleGroup.onToggleChange = listener.OnToggleChange;
        }
	}
}