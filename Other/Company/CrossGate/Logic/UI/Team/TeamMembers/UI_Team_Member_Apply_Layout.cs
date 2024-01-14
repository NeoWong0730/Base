using UnityEngine;
using System.Collections;
using Logic.Core;
using UnityEngine.UI;
using Logic;
using System.Collections.Generic;
using System;
using Framework;

public partial class UI_Team_Member_Apply_Layout
{
    class ApplyShowScene
    {
        public ShowSceneAndRoleModleLoad ShowScene { get; private set; } = new ShowSceneAndRoleModleLoad();

        public ulong MemID { get; private set; }
        public void BindImage(RawImage image)
        {
            ShowScene.BindImage = image;
        }

        public void LoadMode(ulong memID, uint heroID, uint occupation, Dictionary<uint, List<dressData>> DressValue)
        {
            MemID = memID;

            ShowScene.LoadPawnModel(heroID, occupation, DressValue);
        }

        public void OnDestory()
        {
            ShowScene.DisposeMemberActor();
        }
    }
    class ApplyShowSceneManager
    {
        ApplyShowScene[] mShowScenes = new ApplyShowScene[5];

        ApplyShowScene[] mMems = new ApplyShowScene[5];

        //int mMemsCount = 0;

        int mCount = 0;

        public void AddMem(ulong memID)
        {
           
        }
        public ApplyShowScene GetShowScene(ulong memID)
        {
            for (int i = 0; i < mCount; i++)
            {
                if (mShowScenes[i] != null && mShowScenes[i].MemID == memID)
                    return mShowScenes[i];
            }

            if (mCount >= 5)
                return null;

            mCount++;

            mShowScenes[mCount - 1] = new ApplyShowScene();

            return mShowScenes[mCount - 1];
        }


        public void OnDesotry()
        {
            for (int i = 0; i < 5; i++)
            {
                if (mShowScenes[i] != null)
                {
                    mShowScenes[i].OnDestory();
                    mShowScenes[i] = null;
                }
                    
            }
        }
    }
}
public partial class UI_Team_Member_Apply_Layout
{
    #region class ApplyMember
    public class ApplyMember : ClickItem
    {
        public Transform m_transform;

        public Text m_TexName;
        public Image m_IProfession;//职业图标

        public Text m_TexProfession;//职业名称
        public Text m_TexLevel;
        public Text m_TexTitle;//称号

        public Button m_BtnSure;
        private string m_Name;

        private int m_Index;
        public int Index { get { return m_Index; } set { m_Index = value; } }

        public ulong MemID { get; set; }

        public Action<int,ulong> OnClick;

        public Image m_IIcon;
        public override void Load(Transform root)
        {
            base.Load(root);

            m_TexName = root.Find("Text_Name").GetComponent<Text>();
            m_IProfession = root.Find("Image_Prop").GetComponent<Image>();

            m_TexProfession = root.Find("Text_Profession").GetComponent<Text>();
            m_TexLevel = root.Find("Text_Number").GetComponent<Text>();
            m_TexTitle = root.Find("Text_Content").GetComponent<Text>();

            m_IIcon = root.Find("Image_BG/Head").GetComponent<Image>();

            m_BtnSure = root.Find("Button").GetComponent<Button>();

            m_BtnSure.onClick.AddListener(OnClickAccept);

        }

 
        public override ClickItem Clone()
        {
            return Clone<ApplyMember>(this);
        }

        float m_ClickAcceptTimePoint = 0;
        private void OnClickAccept()
        {
            if (Time.time - m_ClickAcceptTimePoint < 1f)
                return;

            OnClick?.Invoke(Index, MemID);

            m_ClickAcceptTimePoint = Time.time;
        }

        public override void Show()
        {
            base.Show();
            m_ClickAcceptTimePoint = 0;
        }

        public override void Hide()
        {
            base.Hide();
            MemID = 0;
        }

        public void SetProfession(uint value, uint rank)
        {

            uint icon = OccupationHelper.GetLogonIconID(value);

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

        public void SetTitle(string str)
        {
            TextHelper.SetText(m_TexTitle, str);
        }

        public void SetLevel(uint level)
        {
            TextHelper.SetText(m_TexLevel, LanguageHelper.GetTextContent(1000002, level.ToString()));
        }

        public void SetName(string name)
        {
            TextHelper.SetText(m_TexName, name);
        }

        public void SetIcon(uint id,uint headId,uint headFrameId)
        {
            CharacterHelper.SetHeadAndFrameData(m_IIcon, id, headId, headFrameId);
        }
    }
    #endregion
}
//
public partial class UI_Team_Member_Apply_Layout
{
    
    public Transform m_Apply;

    Transform m_ApplyItemParent;
    Transform m_ApplyItem;
    Button m_BtnApplyClear;
    Button m_BtnApplyExit;


    Button m_BtnClose;
    //Transform m_TransNone;

    ClickItemGroup<ApplyMember> m_ApplyMembers = new ClickItemGroup<ApplyMember>();

    IListener m_listener;

    public void LoadApply(Transform root)
    {
        m_Apply = root.Find("Animator/Detail");

        m_ApplyItemParent = m_Apply.Find("Scroll_View/Viewport");

        m_ApplyItem = m_ApplyItemParent.Find("Item");

        m_ApplyItem.gameObject.SetActive(false);

        m_ApplyMembers.AddChild(m_ApplyItem);
    
        m_BtnApplyClear = m_Apply.Find("Btn_Clear").GetComponent<Button>();
        m_BtnApplyExit = m_Apply.Find("Btn_Quit").GetComponent<Button>();

        m_BtnClose = root.Find("Animator/View_TipsBgNew02/Btn_Close").GetComponent<Button>();

      //  m_TransNone = m_Apply.Find("Scroll_View/View_None");
    }

    public void SetListener(IListener listener)
    {
        m_listener = listener;

        m_ApplyMembers.SetAddChildListenter(OnAddApplyItem);

        m_BtnApplyClear.onClick.AddListener(listener.OnClickClearApply);
        m_BtnApplyExit.onClick.AddListener(listener.OnClickExitTeam);
        m_BtnClose.onClick.AddListener(listener.OnClickClose);
    }
    private void OnAddApplyItem(ApplyMember item)
    {
        item.OnClick = m_listener.OnClickAccpet;

    }

    public void SetApplyMemberSize(int size)
    {
        m_ApplyMembers.SetChildSize(size);

       // m_TransNone.gameObject.SetActive(size == 0);
    }

    public ApplyMember getApplyAt(int index)
    {
        return m_ApplyMembers.getAt(index);
    }

}


public partial class UI_Team_Member_Apply_Layout
{
    public interface IListener
    {
        void OnClickClearApply();
        void OnClickClose();

        void OnClickExitTeam();

        void OnClickAccpet(int index, ulong id);
    }
}