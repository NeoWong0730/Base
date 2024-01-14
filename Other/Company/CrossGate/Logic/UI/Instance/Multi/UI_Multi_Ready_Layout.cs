using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace Logic
{
    public partial class UI_Multi_Ready_Layout
    {
        #region class member
        public class Member
        {
            public Transform m_transform;

            public Text m_TexName;
            public Image m_IProfession;//职业图标
            public Text m_TexNumber;
            public Text m_TexProfession;//职业名称
            public Text m_TexLevel;
           

            public Image m_HeadIcon;//头像
            public Image m_HeadIconFrame;//头像kuang

            public Text m_TexDesc;

            private string m_Name;

            private List<Transform> m_StateTrans = new List<Transform>();
            public uint ModelID { get; set; }

            public ulong RoleID { get; set; }

            public uint Career { get; set; }
            public string Name
            {
                get { return m_Name; }
                set
                {
                    if (m_Name == value)
                        return;
                    m_Name = value;

                    TextHelper.SetText(m_TexName, m_Name);
                }
            }

            private string m_Num;
            public string Number
            {
                get { return m_Num; }
                set
                {
                    if (m_Num == value)
                        return;
                    m_Num = value;

                    TextHelper.SetText(m_TexNumber, m_Num);
                }
            }

            private uint m_Profession;
            public uint Profession
            {
                get { return m_Profession; }
                set
                {
                    if (m_Profession == value)
                        return;
                    SetProfession(value, m_Rank);

                }
            }

            private uint m_Rank;
            public uint Rank
            {
                get { return m_Rank; }
                set
                {
                    if (m_Rank == value)
                        return;
                    SetProfession(m_Profession, value);
                }
            }



            private int m_Level = -1;
            public int Level
            {
                get { return m_Level; }
                set
                {
                    if (m_Level == value)
                        return;
                    m_Level = value;

                    TextHelper.SetText(m_TexLevel, m_Level.ToString());
                }
            }

            private int m_Index;
            public int Index { get { return m_Index; } set { m_Index = value;/* m_TexNumber.text = m_Index.ToString();*/ } }


           public enum EMemType
            {
                None = 0,
                Ready = 1,
                Refuse = 2,
                Wait = 3,
                OffLine = 4,
                Leave = 5,
            }
            private EMemType m_MemType = 0;

            public EMemType MemType // 0 无,1 准备好，2拒绝，3 等待,4 离线 ，5 暂离
            {
                get { return m_MemType; }
                set{ m_MemType = value;SetStateIcon((uint)m_MemType);}
            }


            private uint m_Head;

            public uint Head { get { return m_Head; } set { m_Head = value; ImageHelper.SetIcon(m_HeadIcon, value); } }

            private uint m_HeadFrame;

            public uint HeadFrame
            {
                get
                {
                    return m_HeadFrame;
                }
                set
                {
                    m_HeadFrame = value;
                    if (value == 0)
                    {
                        m_HeadIconFrame.gameObject.SetActive(false);
                    }
                    else
                    {
                        m_HeadIconFrame.gameObject.SetActive(true);
                        ImageHelper.SetIcon(m_HeadIconFrame, value);
                    }
                }
            }


            private string m_Desc;
            public string Desc { get { return m_Desc; } set { m_Desc = value;  TextHelper.SetText(m_TexDesc, m_Desc); } }

            private void SetStateIcon(uint uvalue)
            {
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

            private void SetProfession(uint value, uint rank)
            {

                uint icon = OccupationHelper.GetTeamIconID(value);

                if (m_Profession != value && m_Profession != 0)
                {
                    m_TexProfession.gameObject.SetActive(true);
                    m_IProfession.gameObject.SetActive(true);
                }

                m_Profession = value;

                if (icon == 0)
                {
                    m_TexProfession.gameObject.SetActive(false);
                    m_IProfession.gameObject.SetActive(false);
                    return;
                }

                TextHelper.SetText(m_TexProfession, OccupationHelper.GetTextID(value,rank));
                if (icon != 0)
                    ImageHelper.SetIcon(m_IProfession, icon);
            }

            private void OnClickItem()
            {
                //OnClick?.Invoke(Index);
            }

            public void Load(Transform transform)
            {
                m_transform = transform;

                m_TexName = transform.Find("Text_Name").GetComponent<Text>();
                m_IProfession = transform.Find("Image_Profession").GetComponent<Image>();
               // m_TexNumber = transform.Find("Text_Number").GetComponent<Text>();
                m_TexProfession = transform.Find("Image_Profession/Text").GetComponent<Text>();
                m_TexLevel = transform.Find("Text_Lv/Text_Num").GetComponent<Text>();
              
              

                m_StateTrans.Add(transform.Find("Image_Ok"));
                m_StateTrans.Add(transform.Find("Image_No"));
                m_StateTrans.Add(transform.Find("Image_Wait"));
                m_StateTrans.Add(transform.Find("Image_Leave"));
                m_StateTrans.Add(transform.Find("Image_LeaveMoment"));

                m_HeadIcon = transform.Find("Head").GetComponent<Image>();
                m_HeadIconFrame = transform.Find("Head/Image_Before_Frame").GetComponent<Image>();
                m_TexDesc = transform.Find("Text_High/Text_High (1)").GetComponent<Text>();

            }

        }
        #endregion
    }
    public partial class UI_Multi_Ready_Layout
    {

        private Button mBtn01;
        private Button mBtn02;
        private Button mBtn03;

        private Text mTex01;
        private Text mTex02;
        private Text mTex03;

        private List<Member> mMemberList = new List<Member>();

        private Transform m_Item;

        private Image m_ImgeBg;

        private Text m_TexTitle0;
        private Text m_TexTitle01;

        private Text m_TexTitle10;
        private Text m_TexTitle11;

        private Transform m_Time;
        private Image m_ImTimePorcess;
        private Text m_TexTime;

        private Button mBtnClose;

        private IListener m_Listener;
        public void Load(Transform root)
        {
            m_ImgeBg = root.Find("Animator/View_Right/Image_Bg").GetComponent<Image>();

            m_TexTitle0 = root.Find("Animator/View_Right/Image_Title/Text01").GetComponent<Text>();
            m_TexTitle01 = root.Find("Animator/View_Right/Image_Title/Text03").GetComponent<Text>();
            m_TexTitle10 = root.Find("Animator/View_Right/Image_Title/Text02").GetComponent<Text>();
            m_TexTitle11 = root.Find("Animator/View_Right/Image_Title/Text04").GetComponent<Text>();


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

            Member member = new Member();

            member.Load(m_Item);

            mMemberList.Add(member);


            mBtnClose = root.Find("Animator/TtileAndBg/Btn_Close").GetComponent<Button>();

            mBtnClose.onClick.AddListener(OnClickClose);

            mBtn01.onClick.AddListener(OnClickBtn01);
            mBtn02.onClick.AddListener(OnClickBtn02);
            mBtn03.onClick.AddListener(OnClickBtn03);
        }

        public void setListener(IListener listener)
        {
            m_Listener = listener;
        }

        public void SetTimeActive(bool b)
        {
            m_Time.gameObject.SetActive(b);
        }
        public void SetTime(string text)
        {
            m_TexTime.text = text;
        }

        public void SetMemberState(int state)
        {

        }

        public void SetTitleInfo(string instanceStr, string chapterStr,string chapterNum)
        {
            m_TexTitle0.text = instanceStr;
            m_TexTitle10.text = chapterStr;
            m_TexTitle11.text = chapterNum;
        }
        private void CloneMember()
        {
            var item = GameObject.Instantiate<GameObject>(m_Item.gameObject);

            item.transform.SetParent(m_Item.parent, false);

            Member member = new Member();

            member.Load(item.transform);

            mMemberList.Add(member);
        }
        public void SetMemberSize(int count)
        {
            int listCount = mMemberList.Count;

            if (listCount < count)
            {
                int discount = count - mMemberList.Count;

                for (int i = 0; i < discount; i++)
                {
                    CloneMember();
                }

                listCount = mMemberList.Count;
            }

           

            for (int i = 0; i < listCount; i++)
            {
                var itme = mMemberList[i];

                itme.m_transform.gameObject.SetActive(i < count);
            }
        }

        public Member getMemberAt(int index)
        {
            if (index >= mMemberList.Count)
                return null;

            return mMemberList[index];
        }

        public void SetBtnState(bool value0, uint strValue0,bool value1,uint starValue1,bool value2, uint strValue2)
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

        private AsyncOperationHandle<Sprite> m_Handle;
        public void SetBg(string path)
        {
            ImageHelper.SetIcon(m_ImgeBg, path);
            AddressablesUtil.LoadAssetAsync<Sprite>(ref m_Handle, path, LoadSpriteFinish);
        }

        private void LoadSpriteFinish(AsyncOperationHandle<Sprite> value)
        {
            if (value.Status == AsyncOperationStatus.Succeeded)
            {
                m_ImgeBg.overrideSprite = value.Result;
            }
              
        }
        private void OnClickClose()
        {
            m_Listener?.OnClickClose();
        }

        private void OnClickBtn01()
        {
            m_Listener?.OnBtn01();
        }

        private void OnClickBtn02()
        {
            m_Listener?.OnBtn02();
        }

        private void OnClickBtn03()
        {
            m_Listener?.OnBtn03();
        }
        public interface IListener
        {
            void OnBtn01();
            void OnBtn02();
            void OnBtn03();

            void OnClickClose();
        }
    }
}
