using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public partial class UI_Goddness_Charac_Layout
    {
        class PropeItem : ClickItem
        {

            private Image m_IIcon;

            private Text m_TextName;
            private Text m_TextInfo;
            public override void Load(Transform root)
            {
                base.Load(root);

                m_IIcon = root.Find("SkillItem02/Image_Icon").GetComponent<Image>();
                
                m_TextName = root.Find("Text").GetComponent<Text>();
                m_TextInfo = root.Find("Text (1)").GetComponent<Text>();

            }

            public override ClickItem Clone()
            {
                return Clone<PropeItem>(this);
            }

            public void SetName(uint langueid)
            {
                TextHelper.SetText(m_TextName, langueid);
            }

            public void SetInfo(uint langueid)
            {
                TextHelper.SetText(m_TextInfo, langueid);
            }

            public void SetIcon(uint IconID)
            {
                ImageHelper.SetIcon(m_IIcon, IconID);
            }
        }

        public void SetPropeItemCount(int count)
        {
            m_PropeItemGroup.SetChildSize(count);
        }

        public void SetPropeName(int index, uint langueid)
        {
            var item = m_PropeItemGroup.getAt(index);

            if (item == null)
                return;

            item.SetName(langueid);
        }

        public void SetPropeInfo(int index, uint langueid)
        {
            var item = m_PropeItemGroup.getAt(index);
            if (item == null)
                return;
            item.SetInfo(langueid);
        }

        public void SetPropeIcon(int index, uint iconid)
        {
            var item = m_PropeItemGroup.getAt(index);
            if (item == null)
                return;
            item.SetIcon(iconid);
        }
    }

    public partial class UI_Goddness_Charac_Layout
    {
        ClickItemGroup<PropeItem> m_PropeItemGroup = new ClickItemGroup<PropeItem>();

        Button m_BtnClose;
    }
    public partial class UI_Goddness_Charac_Layout
    {
        public void Load(Transform root)
        {
            m_BtnClose = root.Find("Animator/View_TipsBgNew02/Btn_Close").GetComponent<Button>();

            m_PropeItemGroup.AddChild(root.Find("Animator/List/Grid/Item"));
        }

        public void SetListener(IListener listener)
        {
            m_BtnClose.onClick.AddListener(listener.OnClickClose);
        }
    }

    public partial class UI_Goddness_Charac_Layout
    {
        public interface IListener
        {
            void OnClickClose();
        }
    }
}
