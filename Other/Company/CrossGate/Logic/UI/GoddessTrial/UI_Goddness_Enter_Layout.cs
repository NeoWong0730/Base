using System;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Table;
using UnityEngine.UI;

namespace Logic
{
    public partial class UI_Goddness_Enter_Layout
    {
        public enum EMemType
        {
            None = 0,
            Ready = 1,
            Refuse = 2,
            Wait = 3,
            OffLine = 4,
            Leave = 5,
        }

        class MemberItem : ClickItem
        {
            public Text m_TexName;
            public Image m_IProfession;//职业图标
            public Text m_TexNumber;
            public Text m_TexProfession;//职业名称
            public Text m_TexLevel;

            public Image m_HeadIcon;//头像

            public Text m_TexDesc;

            private string m_Name;

            private List<Transform> m_StateTrans = new List<Transform>();

            public uint Index { get; set; }
            public override void Load(Transform transform)
            {
                base.Load(transform);
                m_TexName = transform.Find("Text_Name").GetComponent<Text>();
                m_IProfession = transform.Find("Image_Profession").GetComponent<Image>();

                m_TexProfession = transform.Find("Image_Profession/Text").GetComponent<Text>();
                m_TexLevel = transform.Find("Text_Lv/Text_Num").GetComponent<Text>();

                m_StateTrans.Add(transform.Find("Image_Ok"));
                m_StateTrans.Add(transform.Find("Image_No"));
                m_StateTrans.Add(transform.Find("Image_Wait"));
                m_StateTrans.Add(transform.Find("Image_Leave"));
                m_StateTrans.Add(transform.Find("Image_LeaveMoment"));

                m_HeadIcon = transform.Find("Image_Head").GetComponent<Image>();

                m_TexDesc = transform.Find("Text_High/Text_High (1)").GetComponent<Text>();
            }

            public override ClickItem Clone()
            {
                return Clone<MemberItem>(this);
            }

            public void SetName(string name)
            {
                TextHelper.SetText(m_TexName, name);
            }
            public void SetLevel(uint level)
            {
                TextHelper.SetText(m_TexLevel, level.ToString());
            }

            public void SetCareer(uint value,uint rank)
            {
                uint icon = OccupationHelper.GetTeamIconID(value);

                if (value != 0)
                {
                    m_TexProfession.gameObject.SetActive(true);
                    m_IProfession.gameObject.SetActive(true);
                }

                if (icon == 0)
                {
                    m_TexProfession.gameObject.SetActive(false);
                    m_IProfession.gameObject.SetActive(false);
                    return;
                }

                TextHelper.SetText(m_TexProfession, OccupationHelper.GetTextID(value, rank));

                if (icon != 0)
                    ImageHelper.SetIcon(m_IProfession, icon);
            }

            public void SetHeadIcon(uint heroId, uint headId, uint headFrameId)
            {
                CharacterHelper.SetHeadAndFrameData(m_HeadIcon, heroId, headId, headFrameId);
            }

            public void SetStateIcon(EMemType value)
            {
                uint uvalue = (uint)value;

                if (uvalue > m_StateTrans.Count)
                    return;

                for (int i = 0; i < m_StateTrans.Count; i++)
                {
                    if (m_StateTrans[i].gameObject.activeSelf)
                        m_StateTrans[i].gameObject.SetActive(false);
                }

                if (uvalue == 0)
                    return;

                int index = (int)uvalue - 1;

                m_StateTrans[index].gameObject.SetActive(true);
            }

            public void SetRecord(string text)
            {
                m_TexDesc.text = text;
            }
        }
    }

    public partial class UI_Goddness_Enter_Layout
    {
        public void SetMemsSize(int size)
        {
            m_MemberGroup.SetChildSize(size);
        }

        public void SetMemName(int index, string name)
        {
            var item = m_MemberGroup.getAt(index);
            if (item == null)
                return;
            item.SetName(name);
        }

        public void SetMemLevel(int index, uint level)
        {
            var item = m_MemberGroup.getAt(index);
            if (item == null)
                return;
            item.SetLevel(level);
        }

        public void SetMemCareer(int index, uint career, uint rank)
        {
            var item = m_MemberGroup.getAt(index);
            if (item == null)
                return;
            item.SetCareer(career, rank);
        }
        public void SetMemIndex(int index, uint id)
        {
            var item = m_MemberGroup.getAt(index);
            if (item == null)
                return;
            item.Index = id;
        }

        public void SetMemHeadIcon(int index, uint heroId,uint headId, uint headFrameId)
        {
            var item = m_MemberGroup.getAt(index);
            if (item == null)
                return;
            item.SetHeadIcon(heroId, headId, headFrameId);
        }
        public void SetMemSetae(int index, EMemType value)
        {
            var item = m_MemberGroup.getAt(index);
            if (item == null)
                return;
            item.SetStateIcon(value);
        }

        public void SetMemRecord(int index, string tex)
        {
            var item = m_MemberGroup.getAt(index);
            if (item == null)
                return;

            item.SetRecord(tex);
        }

    }
    public partial class UI_Goddness_Enter_Layout
    {
        ClickItemGroup<MemberItem> m_MemberGroup = new ClickItemGroup<MemberItem>();

        private Button mBtn01;
        private Button mBtn02;
        private Button mBtn03;

        private Text mTex01;
        private Text mTex02;
        private Text mTex03;

        private Transform m_Item;

        private Image m_ImgeBg;

        private Text m_TexTitle0;
        private Text m_TexTitle01;

        private Text m_TexTitle10;
       // private Text m_TexTitle11;

        private Transform m_Time;
        private Image m_ImTimePorcess;
        private Text m_TexTime;

        private Button mBtnClose;

        private IListener m_Listener;

        public void SetBtnState(bool value0, uint strValue0, bool value1, uint starValue1, bool value2, uint strValue2)
        {

            mBtn01.gameObject.SetActive(value0);

            mBtn02.gameObject.SetActive(value1);

            mBtn03.gameObject.SetActive(value2);

            if (value0)
                TextHelper.SetText(mTex01, strValue0);

            if (value1)
                TextHelper.SetText(mTex02, starValue1);

            if (value2)
                TextHelper.SetText(mTex03, strValue2);

        }

        public void SetTimeActive(bool b)
        {
            m_Time.gameObject.SetActive(b);
        }

        public void SetTime(string timestr)
        {
            m_TexTime.text = timestr;
        }

        public void SetDiffic(uint langueID)
        {
            TextHelper.SetText(m_TexTitle01, langueID);
        }

        public void SetTopicName(uint langueID)
        {
            TextHelper.SetText(m_TexTitle0, langueID);
        }

        public void SetChapterName(string str)
        {
            m_TexTitle10.text = str;
        }
    }
    public partial class UI_Goddness_Enter_Layout
    {

        public void Load(Transform root)
        {
            m_ImgeBg = root.Find("Animator/View_Right/Image_Bg").GetComponent<Image>();

            m_TexTitle0 = root.Find("Animator/View_Right/Image_Title/Text01").GetComponent<Text>();
            m_TexTitle01 = root.Find("Animator/View_Right/Image_Title/Text03").GetComponent<Text>();
            m_TexTitle10 = root.Find("Animator/View_Right/Image_Title/Text02").GetComponent<Text>();
           // m_TexTitle11 = root.Find("Animator/View_Right/Image_Title/Text04").GetComponent<Text>();


            m_Time = root.Find("Animator/View_Right/Image_Time");
            m_ImTimePorcess = root.Find("Animator/View_Right/Image_Time/Image_Time").GetComponent<Image>();
            m_TexTime = root.Find("Animator/View_Right/Image_Time/Text").GetComponent<Text>();


            mBtn02 = root.Find("Animator/View_Right/Btnlist/Btn_02").GetComponent<Button>();

            mBtn01 = root.Find("Animator/View_Right/Btnlist/Btn_01").GetComponent<Button>();
            mBtn03 = root.Find("Animator/View_Right/Btnlist/Btn_03").GetComponent<Button>();

            mTex01 = mBtn01.transform.Find("Text").GetComponent<Text>();
            mTex02 = mBtn02.transform.Find("Text").GetComponent<Text>();
            mTex03 = mBtn03.transform.Find("Text").GetComponent<Text>();

            m_Item = root.Find("Animator/View_Left/Scroll View01/Viewport/Content/Item");
            m_MemberGroup.AddChild(m_Item);



            mBtnClose = root.Find("Animator/TtileAndBg/Btn_Close").GetComponent<Button>();
        }

        public void SetListener(IListener listener)
        {
            mBtnClose.onClick.AddListener(listener.OnClickClose);

            mBtn01.onClick.AddListener(listener.OnBtn01);
            mBtn02.onClick.AddListener(listener.OnBtn02);
            mBtn03.onClick.AddListener(listener.OnBtn03);
        }
    }

    public partial class UI_Goddness_Enter_Layout
    {
        public interface IListener
        {
            void OnBtn01();
            void OnBtn02();
            void OnBtn03();

            void OnClickClose();
        }

    }
}
