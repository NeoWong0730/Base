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
    public partial class UI_Pvp_SingleLoading_Layout 
    {
        private CommonLayout m_OwnLayout = new CommonLayout();
        private CommonLayout m_OtherLayout = new CommonLayout();
        public void Load(Transform root)
        {
            m_OwnLayout.Load(root.Find("Animator/own"));
            m_OtherLayout.Load(root.Find("Animator/other"));
        }

        public void SetListener(IListener listener)
        {

        }
    }

    /// <summary>
    /// 我方
    /// </summary>
    public partial class UI_Pvp_SingleLoading_Layout
    {
        public void SetOwnRoleIcon(ulong roleid)
        {
            RoleIconHelper.SetRoleIcon(roleid, m_OwnLayout.m_ImRole);
        }

        public void SetOwnLoadingPercent(int tex)
        {
            m_OwnLayout.m_ImagePercentFill.fillAmount = tex / 100f;
            m_OwnLayout.m_TexPercent.text = (tex).ToString() + "%";
        }

        public void SetOwnDanIcon(Sprite sprite)
        {
            m_OwnLayout.m_ImDan.sprite = sprite;
        }

        public void SetOwnDanTex(string tex)
        {
            m_OwnLayout.m_TexDan.text = tex;
        }

        public void SetOwnName(string tex)
        {
            m_OwnLayout.m_TexName.text = tex;
        }

        public void SetOwnServerName(string tex)
        {
            m_OwnLayout.m_TexServerName.text = tex;
        }

        public void SetOwnLevel(int level)
        {
            m_OwnLayout.m_TexLevel.text = level.ToString();
        }
        public void SetOwnMember(int index, uint roleIcon, uint occIcon)
        {
            var item = m_OwnLayout.m_MemberGroup.getAt(index);

            if (item == null)
                return;

            ImageHelper.SetIcon(item.m_ImIcon, roleIcon);
            ImageHelper.SetIcon(item.m_ImOcc, occIcon);

        }

        public void SetOwnMemberSize(int size)
        {
            m_OwnLayout.m_MemberGroup.SetChildSize(size);
        }

        public void SetOwnMemberLevelIcon(uint icon)
        {
            ImageHelper.SetIcon(m_OwnLayout.m_ImLevle, icon);
        }
    }

    /// <summary>
    /// 对方
    /// </summary>
    public partial class UI_Pvp_SingleLoading_Layout
    {
        public void SetOtherRoleIcon(uint fashionid,uint heroid)
        {
            RoleIconHelper.SetRoleIcon( m_OtherLayout.m_ImRole, fashionid,heroid);
        }

        public void SetOtherLoadingPercent(int tex)
        {
            m_OtherLayout.m_TexPercent.text = (tex).ToString() + "%";
            m_OtherLayout.m_ImagePercentFill.fillAmount = tex / 100f;
        }

        public void SetOtherDanIcon(Sprite sprite)
        {
            m_OtherLayout.m_ImDan.sprite = sprite;
        }

        public void SetOtherDanTex(string tex)
        {
            m_OtherLayout.m_TexDan.text = tex;
        }

        public void SetOtherName(string tex)
        {
            m_OtherLayout.m_TexName.text = tex;
        }

        public void SetOtherServerName(string tex)
        {
            m_OtherLayout.m_TexServerName.text = tex;
        }

        public void SetOtherLevel(int level)
        {
            m_OtherLayout.m_TexLevel.text = level.ToString();
        }
        public void SetOtherMember(int index, uint roleIcon, uint occIcon)
        {
            var item = m_OtherLayout.m_MemberGroup.getAt(index);

            if (item == null)
                return;


            ImageHelper.SetIcon(item.m_ImIcon, roleIcon);
            ImageHelper.SetIcon(item.m_ImOcc, occIcon);
        }

        public void SetOtherMemberSize(int size)
        {
            m_OtherLayout.m_MemberGroup.SetChildSize(size);
        }

        public void SetOtherMemberLevelIcon(uint icon)
        {
            ImageHelper.SetIcon(m_OtherLayout.m_ImLevle, icon);
        }
    }

    /// <summary>
    /// 共同布局
    /// </summary>
    public partial class UI_Pvp_SingleLoading_Layout
    {
        class CommonLayout
        {
            public Text m_TexPercent;

            public Image m_ImagePercentFill;

            public Image m_ImRole;

            public Image m_ImDan;
            public Text m_TexDan;
            public Text m_TexName;
            public Text m_TexServerName;

            public Text m_TexLevel;

            public Image m_ImLevle;
           public class MemberItem : ClickItem
            {
                public Transform m_transform;
                public Image m_ImOcc;
                public Image m_ImIcon;

                public override void Load(Transform root)
                {
                    base.Load(root);

                    m_transform = root;

                    m_ImIcon = root.Find("Image_Icon").GetComponent<Image>();

                    m_ImOcc = root.Find("Image_Job").GetComponent<Image>();
                }
            }

          //  public List<MemberItem> m_MemberItems = new List<MemberItem>(4);

            public ClickItemGroup<MemberItem> m_MemberGroup = new ClickItemGroup<MemberItem>() { AutoClone = false }; 
            public void Load(Transform root)
            {
                m_TexPercent = root.Find("Text_Percent").GetComponent<Text>();

                m_ImagePercentFill = root.Find("Image_Percent/Fill Area/Fill").GetComponent<Image>();

                m_ImRole = root.Find("Image_Character").GetComponent<Image>();

                m_ImDan = root.Find("Image_Rank").GetComponent<Image>();
                m_TexDan = root.Find("Image_Rank/Text_Rank").GetComponent<Text>();
                m_TexName = root.Find("Image_Rank/Text_Name").GetComponent<Text>();
                m_TexServerName = root.Find("Image_Rank/Text_ServerName").GetComponent<Text>();
                m_ImLevle = root.Find("Image_Rank/Image_badge").GetComponent<Image>();

                m_TexLevel = root.Find("Image_Rank/Text_Lv").GetComponent<Text>();

                Transform memberTrans = root.Find("members");
                int memberCount = memberTrans.childCount;
                for (int i = 0; i < memberCount; i++)
                {
                    Transform item = memberTrans.Find("Member" + i);
                    if (item != null)
                    {
                        m_MemberGroup.AddChild(item);
                    }
                }
            }
        }
    }

    public partial class UI_Pvp_SingleLoading_Layout
    {
        public interface IListener
        {

        }
    }
}
