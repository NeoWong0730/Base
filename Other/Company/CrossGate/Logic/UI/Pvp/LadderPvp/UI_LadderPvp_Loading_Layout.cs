using Logic.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    /// <summary>
    ///pvp 加载
    /// </summary>
    public partial class UI_LadderPvp_Loading_Layout
    {
        public class TeamMemberItem:ClickItem
        {
            public Text TexName;
            public Text TexLevel;

            public Image ImgCareerIcon;

            public Image ImgHead;

            public Text TexDanLv;
            public override void Load(Transform root)
            {
                base.Load(root);

                TexName = root.Find("Text_Name").GetComponent<Text>();
                TexLevel = root.Find("Text_Lv").GetComponent<Text>();
                ImgCareerIcon = root.Find("Image_Job").GetComponent<Image>();
                ImgHead = root.Find("Head").GetComponent<Image>();
                TexDanLv = root.Find("Image_Rank/Text").GetComponent<Text>();
            }
        }

        public ClickItemGroup<TeamMemberItem> LeftTeamGroup = new ClickItemGroup<TeamMemberItem>() { AutoClone = false };
        public ClickItemGroup<TeamMemberItem> RightTeamGroup = new ClickItemGroup<TeamMemberItem>() { AutoClone = false };

        public Image ImgLeftProcess;
        public Image ImgRightProcess;

        public Text TexLeftProcess;
        public Text TexRightProcess;
        public void Load(Transform root)
        {
            Transform lefttrans = root.Find("Animator/RedTeam");
            for (int i = 0; i < 5; i++)
            {
                var itemtrnas = lefttrans.Find("Red" + i.ToString());
                LeftTeamGroup.AddChild(itemtrnas);
            }

            Transform righttrans = root.Find("Animator/BuleTeam");
            for (int i = 0; i < 5; i++)
            {
                var itemtrnas = righttrans.Find("Blue" + i.ToString());
                RightTeamGroup.AddChild(itemtrnas);
            }

            ImgLeftProcess = root.Find("Animator/Red_Slider/Image2").GetComponent<Image>();
            TexLeftProcess = root.Find("Animator/Red_Slider/Text_Num").GetComponent<Text>();

            ImgRightProcess = root.Find("Animator/Blue_Slider/Image2").GetComponent<Image>();
            TexRightProcess = root.Find("Animator/Blue_Slider/Text_Num").GetComponent<Text>();
        }

        public void SetListener(IListener listener)
        {

        }
    }

  

    public partial class UI_LadderPvp_Loading_Layout
    {
        public interface IListener
        {

        }
    }
}
