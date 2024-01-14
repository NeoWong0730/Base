using Logic.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{

    public partial class UI_LadderPvp_Match_Layout
    {
        public class TeamMemItem : ClickItem
        {
            public Image ImgIcon;
            public Image ImgIconFrame;
            public Text TexName;
            public Image ImgCareeIcon;
            public Text TexLevel;
            public Text TexDanLevel;

            public Transform TransLeave;
            public Transform TransOffline;

            public Transform TransInfo;
            public Transform TransAdd;
            public Button BtnAdd;

   
            public override void Load(Transform root)
            {
                base.Load(root);

                TransLeave = root.Find("Image_BG/State/Image_State0");
                TransOffline = root.Find("Image_BG/State/Image_State1");
                ImgIcon = root.Find("Image_BG/Head").GetComponent<Image>();
                ImgIconFrame = root.Find("Image_BG/Head/Image_Before_Frame").GetComponent<Image>();
                TexName = root.Find("Image_BG/Text_Name").GetComponent<Text>();
                ImgCareeIcon = root.Find("Image_BG/Image_Job").GetComponent<Image>();
                TexLevel = root.Find("Image_BG/Text_LV").GetComponent<Text>();
                TexDanLevel = root.Find("Image_BG/Text_Rank").GetComponent<Text>();

                TransInfo = root.Find("Image_BG");
                TransAdd = root.Find("Image_Add");
                BtnAdd = TransAdd.GetComponent<Button>();

            }

        }

        private Button m_BtnMatch;
       
        private Text m_TexTime;


        public ClickItemGroup<TeamMemItem> TeamMemGroup = new ClickItemGroup<TeamMemItem>() { AutoClone = false };
        public void Load(Transform root)
        {
            m_BtnMatch = root.Find("Animator/Button_Quit").GetComponent<Button>();
            //m_TexMatch = root.Find("Animator/Button_Quit/Text").GetComponent<Text>();
            m_TexTime = root.Find("Animator/Text_Count").GetComponent<Text>();

            var teamroot = root.Find("Animator/Grid_team");

            for (int i = 0; i < 5; i++)
            {
                TeamMemGroup.AddChild(teamroot.Find("Member" + i.ToString()));
            }
        }


        public void SetListener(IListener listener)
        {
            m_BtnMatch.onClick.AddListener(listener.OnClickMatch);
        }

        public void SetMatchTimeTex(string tex)
        {
            m_TexTime.text = tex;
        }
    }


    public partial class UI_LadderPvp_Match_Layout
    {
        public interface IListener
        {
            void OnClickMatch();
        }
    }


}
