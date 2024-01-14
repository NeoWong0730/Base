using Logic.Core;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{

    public partial class UI_Pvp_SingleMatch_Layout 
    {
        private Button m_BtnMatch;
        private Text m_TexMatch;
        private Text m_TexTime;

        private Transform m_TransOwn;
        private Text m_TexOwnName;
        private Text m_TexOwnLevel;
        private Text m_TexOwnServer;
        private Image m_ImOwnOcc;
        private Image m_ImOwnIcon;

        private Transform m_TransOther;
        private Text m_TexOtherName;
        private Text m_TexOtherLevel;
        private Text m_TexOtherServer;
        private Image m_ImOtherOcc;
        private Image m_ImOtherIcon;


        private Transform m_TransSearch;

       
        public void Load(Transform root)
        {
            m_BtnMatch = root.Find("Animator/Button_Quit").GetComponent<Button>();
            m_TexMatch = root.Find("Animator/Button_Quit/Text").GetComponent<Text>();
            m_TexTime = root.Find("Animator/Text_Count").GetComponent<Text>();

            m_TransOwn = root.Find("Animator/own");
            m_TexOwnName = m_TransOwn.Find("Image_Bottom/Text_Name").GetComponent<Text>();
            m_TexOwnLevel = m_TransOwn.Find("Image_Bottom/Text_Lv").GetComponent<Text>();
            m_TexOwnServer = m_TransOwn.Find("Image_Bottom/Text_ServerName").GetComponent<Text>();
            m_ImOwnOcc = m_TransOwn.Find("Image_Jobbg/Image_Job").GetComponent<Image>();
            m_ImOwnIcon = m_TransOwn.Find("Image_Character").GetComponent<Image>();

            m_TransOther = root.Find("Animator/other");
            m_TexOtherName = m_TransOwn.Find("Image_Bottom/Text_Name").GetComponent<Text>();
            m_TexOtherLevel = m_TransOwn.Find("Image_Bottom/Text_Lv").GetComponent<Text>();
            m_TexOtherServer = m_TransOwn.Find("Image_Bottom/Text_ServerName").GetComponent<Text>();
            m_ImOtherOcc = m_TransOwn.Find("Image_Jobbg/Image_Job").GetComponent<Image>();
            m_ImOtherIcon = m_TransOwn.Find("Image_Character").GetComponent<Image>();

            m_TransSearch = root.Find("Animator/Text_Search");
        }


        public void SetListener(IListener listener)
        {
            m_BtnMatch.onClick.AddListener(listener.OnClickMatch);
        }


        public void SetMatchTex(string tex)
        {
            m_TexMatch.text = tex;
        }

        public void SetMatchTimeTex(string tex)
        {
            m_TexTime.text = tex;
        }
    }


    public partial class UI_Pvp_SingleMatch_Layout
    {
        public interface IListener
        {
            void OnClickMatch();
        }
    }

    /// <summary>
    /// 玩家自己的信息
    /// </summary>
    public partial class UI_Pvp_SingleMatch_Layout
    {
        public void SetOwnName(string tex)
        {
            m_TexOwnName.text = tex;
        }

        public void SetOwnLevel(int level)
        {
            m_TexOwnLevel.text = "Lv." + level;
        }

        public void SetOwnServerName(string tex)
        {
            m_TexOwnServer.text = tex;
        }

        public void SetOwnOcc(uint occID)
        {
            ImageHelper.SetIcon(m_ImOwnOcc, OccupationHelper.GetIconID(occID));
        }

        public void SetOwnIcon(Sprite icon)
        {
            m_ImOwnIcon.sprite = icon;
        }

        public void SetOwnRoleIcon(ulong roleID)
        {
            RoleIconHelper.SetRoleIcon(roleID, m_ImOwnIcon);
        }
    }

    /// <summary>
    /// 对方的信息
    /// </summary>
    public partial class UI_Pvp_SingleMatch_Layout
    {
        public void SetOtherName(string tex)
        {
            m_TexOtherName.text = tex;
        }

        public void SetOtherLevel(int level)
        {
            m_TexOtherLevel.text = "Lv." + level;
        }

        public void SetOtherServerName(string tex)
        {
            m_TexOtherServer.text = tex;
        }

        public void SetOtherOcc(uint occID)
        {
            ImageHelper.SetIcon(m_ImOtherOcc, OccupationHelper.GetIconID(occID));
        }

        public void SetOtherIcon(Sprite icon)
        {
            m_ImOtherIcon.sprite = icon;
        }

        public void SetOtherTransActive(bool active)
        {
            if (m_TransOther.gameObject.activeSelf != active)
                m_TransOther.gameObject.SetActive(active);
        }
    }
}
