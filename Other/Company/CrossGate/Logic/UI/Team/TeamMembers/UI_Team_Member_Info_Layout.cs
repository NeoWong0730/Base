using UnityEngine;
using System.Collections;
using Logic.Core;
using UnityEngine.UI;
using Logic;
using System.Collections.Generic;
using System;
using Table;
using Framework;
using Packet;


//
public partial class UI_Team_Member_Layout
{

    public Transform m_Team;

    public Button m_BtnFormation;//阵法
    public Text m_TexFormation;

    public Transform m_IFTarget;//目标
    public Text m_TexFTarget;//目标
    public Button m_BtnCustom;
    public Button m_BtnChangeTeamIcon;
    public Image m_ITeamIcon;

    public Button m_Invitation;//邀请


    public Button m_BtnApply; //入队申请列表

    public Button m_BtnShout;//一键喊话
    public Button m_BtnShoutGrid;
    public Button m_BtnShoutTeam;
    public Button m_BtnShoutFamliy;

    public Button m_BtnMyMate;
    public Button m_BtnFightCommand;
    public Button m_BtnFast;

    public Button m_BtnAutoFind;//自动匹配
    public Button m_BtnTeam;
    public Button m_BtnExitTeam;
    public Button m_BtnOffTeam;
    public Button m_BtnComeTeam;

    //菜单
    public Transform m_MenuTransform;
    public Transform m_MenuItem;


    public Transform m_MemberTransform;
   // public Transform m_MemberItem;

    public Button m_BtnClose;

    //List<Member> m_Members = new List<Member>();

    ClickItemGroup<Member> m_MembersGroup = new ClickItemGroup<Member>();

   // private int m_MemberInfoType = 0;

    public Action OnClickInviteAction;

    public ClickItemEvent OnClickInfo = new ClickItemEvent();

    private AssetDependencies mInfoAssetDependencies;
    private Text mMatchTex;

    private Transform m_TransApplyRed;


    private Transform m_TransTeamMask;
    private Button m_BtnTeamMask;
    private Image m_ImgTeamMaskIcon;

    private Transform m_PriTeamMaskFx;

    private GameObject matchingFxGo;
    private void LoadTeam(Transform root)
    {
        m_Team = root.Find("Animator/View_Team");

        mInfoAssetDependencies = m_Team.GetComponent<AssetDependencies>();



        m_BtnFormation = m_Team.Find("Button_Array").GetComponent<Button>();
        m_TexFormation = m_Team.Find("Button_Array/Text_01").GetComponent<Text>();

        m_IFTarget = m_Team.Find("InputField");
        m_TexFTarget = m_Team.Find("InputField/Text").GetComponent<Text>();

        m_BtnCustom = m_Team.Find("InputField/Btn_Custom").GetComponent<Button>();


        m_Invitation = m_Team.Find("Button_Invitation").GetComponent<Button>();

        m_BtnApply = m_Team.Find("rightupbtns/Button_Apply").GetComponent<Button>();
        m_TransApplyRed = m_BtnApply.transform.Find("UI_RedTips_Small");


        
        m_BtnMyMate = m_Team.Find("rightupbtns/Button_MyMate").GetComponent<Button>();
        m_BtnFightCommand = m_Team.Find("rightupbtns/Button_FightCommand").GetComponent<Button>();

        m_BtnAutoFind = m_Team.Find("bottombtns/Button_Match").GetComponent<Button>();
        matchingFxGo = m_Team.Find("bottombtns/Button_Match/Fx_ui_Team_Fast_huanrao").gameObject;
        mMatchTex = m_Team.Find("bottombtns/Button_Match/Text_01").GetComponent<Text>();

        m_BtnTeam = m_Team.Find("bottombtns/Button_CreateTeam").GetComponent<Button>();
        m_BtnExitTeam = m_Team.Find("bottombtns/Button_ExitTeam ").GetComponent<Button>();
        m_BtnOffTeam = m_Team.Find("bottombtns/Button_OffTeam").GetComponent<Button>();
        m_BtnComeTeam = m_Team.Find("bottombtns/Button_ComeTeam").GetComponent<Button>();
        m_BtnFast = m_Team.Find("bottombtns/Button_FastTeam").GetComponent<Button>();

        m_BtnShout = root.Find("Animator/View_Team/Button").GetComponent<Button>();
        m_BtnShoutGrid = m_BtnShout.transform.Find("Button_Grid").GetComponent<Button>();
        m_BtnShoutTeam = m_BtnShoutGrid.transform.Find("Grid_Button/Button02").GetComponent<Button>();
        m_BtnShoutFamliy = m_BtnShoutGrid.transform.Find("Grid_Button/Button01").GetComponent<Button>();

        m_MenuTransform = m_Team.Find("Button_Grid");
        m_MenuItem = m_Team.Find("Button_Grid/Grid_Button/Button01");
        m_MenuTransform.gameObject.SetActive(false);

        m_MemberTransform = m_Team.Find("Scroll_View/Viewport");

       // m_MemberItem = m_Team.Find("Scroll_View/Viewport/Item");
        int memcount = m_MemberTransform.childCount;
        for (int i = 0; i < memcount; i++)
        {
            var item = m_MemberTransform.Find("Item" + i.ToString());
            if (item != null)
                m_MembersGroup.AddChild(item);
        }

     //   m_MemberItem.gameObject.SetActive(false);

        m_BtnClose = root.Find("Animator/View_Title01/Btn_Close").GetComponent<Button>();

        m_TransTeamMask = root.Find("Animator/View_Team/TeamMark");
        m_BtnTeamMask = m_TransTeamMask.Find("Btn_change").GetComponent<Button>();
        m_ImgTeamMaskIcon = m_TransTeamMask.Find("Icon").GetComponent<Image>();

        m_PriTeamMaskFx = m_TransTeamMask.Find("FX_node");

    }

    private void SetTeamInfoListener(IListener listener)
    {
        m_Invitation.onClick.AddListener(listener.OnClickInvite);
        m_BtnCustom.onClick.AddListener(listener.CustomTarget);
        m_BtnAutoFind.onClick.AddListener(listener.AutoFind);

        m_MembersGroup.SetAddChildListenter(OnAddTeamInfoItem);

        m_BtnApply.onClick.AddListener(OnClickApply);
        m_BtnFast.onClick.AddListener(listener.FastBuild);
        m_BtnFightCommand.onClick.AddListener(listener.Command);
        m_BtnMyMate.onClick.AddListener(listener.MyMate);
        m_BtnApply.onClick.AddListener(listener.OnClickApplyTeam);
        m_BtnShout.onClick.AddListener(listener.Talk);
        m_BtnShoutGrid.onClick.AddListener(listener.OnClickTalkGrid);
        m_BtnShoutTeam.onClick.AddListener(listener.OnClickTalkTeam);
        m_BtnShoutFamliy.onClick.AddListener(listener.OnClickTalkFamliy);
        m_BtnComeTeam.onClick.AddListener(listener.OffLineTeam);
        m_BtnOffTeam.onClick.AddListener(listener.OffLineTeam);
        m_BtnExitTeam.onClick.AddListener(listener.ExitTeam);
        m_BtnTeam.onClick.AddListener(listener.CreateTeam);

        m_BtnTeamMask.onClick.AddListener(listener.OnClickTeamMask);
    }

    private void OnAddTeamInfoItem(Member item)
    {
        item.OnClick.AddListener(OnClickInfoItem);
    }



    /// <summary>
    /// 创建队伍
    /// </summary>
    public void CreateTeam()
    {
        //m_MemberInfoType = 1;
        TeamMemOrCapLayout(false);
        CreateTeamLayout(true);
    }

    private void CreateTeamLayout(bool b)
    {
        m_IFTarget.gameObject.SetActive(b);

        //m_IFTarget.text = "test-你尚未创建队伍，点击便捷组队，可快速匹配到目标队伍";

        if (b)
        {
            SetBottomBtn(0);
        }

    }

    public void SetTargetInfotex(string tex)
    {
        m_TexFTarget.text = tex;
    }
    /// <summary>
    /// 队员
    /// </summary>
    public void TeamMember(bool isLeave)
    {
       // m_MemberInfoType = 2;
        // TeamCapLayout(true);
        TeamMemberLayout(true, isLeave);
    }

    private void TeamMemberLayout(bool b, bool isleave = false)
    {
        TeamMemOrCapLayout(b);
        if (b)
        {
            SetBottomBtn(2, isleave);
        }
    }
    /// <summary>
    /// 队长
    /// </summary>
    public void TeamCap()
    {
        //m_MemberInfoType = 3;
        TeamCapLayout(true);
    }

    private void TeamCapLayout(bool b)
    {
        TeamMemOrCapLayout(b);

        if (b)
        {
            SetBottomBtn(1);
        }
    }


    public void SetMatch(bool isMatch)
    {
        mMatchTex.text = LanguageHelper.GetTextContent((uint)(isMatch == false ? 2002509 : 2002510));
       
        if (matchingFxGo.activeInHierarchy != isMatch)
            matchingFxGo.SetActive(isMatch);
    }
    private void TeamMemOrCapLayout(bool b)
    {
        m_BtnAutoFind.gameObject.SetActive(b);
        m_Invitation.gameObject.SetActive(b);

        m_BtnCustom.gameObject.SetActive(b);
        m_IFTarget.gameObject.SetActive(b);

        //m_ITeamIcon.gameObject.SetActive(b);
        //m_BtnChangeTeamIcon.gameObject.SetActive(b);
    }

    /// <summary>
    /// 根据不同的状态，设置不同的按钮显示
    /// </summary>
    /// <param name="mode">0 无队伍状态， 1 队长状态， 2 队员状态</param>
    private void SetBottomBtn(int mode, bool isOff = false)
    {
        m_BtnTeam.gameObject.SetActive(mode == 0);
        m_BtnExitTeam.gameObject.SetActive(mode != 0);

        m_BtnShout.gameObject.SetActive(mode == 1);
        m_BtnApply.gameObject.SetActive(mode == 1);

        m_BtnMyMate.gameObject.SetActive(mode == 0);
        m_BtnFast.gameObject.SetActive(mode == 0);

        m_BtnOffTeam.gameObject.SetActive(mode == 2 && isOff == false);

        m_BtnComeTeam.gameObject.SetActive(mode == 2 && isOff);
    }


    public void SetMemberSize(int size)
    {
        m_MembersGroup.SetChildSize(size);
    }

    public Member getMemberAt(int index)
    {
        return m_MembersGroup.getAt(index);
    }



    public bool isRightMember(Member member)
    {
        if (member == null)
            return false;

        if (member.m_transform.gameObject.activeSelf == false)
            return false;

        return true;
    }

    Vector3 showModelPosition = Vector3.zero;


   
    public void ShowMembersInfo()
    {
        m_Team.gameObject.SetActive(true);

    }

    public void HideMembersInfo()
    {
        m_Team.gameObject.SetActive(false);

    }

    public void SetApplyRedState(bool b)
    {
        m_TransApplyRed.gameObject.SetActive(b);
    }
    public bool isActive()
    {
        if (m_Team == null)
            return false;

        return m_Team.gameObject.activeSelf;
    }


    private void OnClickInfoItem(int index)
    {
        OnClickInfo?.Invoke(index);
    }

    private void OnClickAutoFind()
    {
        m_listener?.AutoFind();
    }

    private void OnClickApply()
    {
        m_listener?.OnClickApplyTeam();
    }

    public void SetTeamMarkActive(bool active)
    {
        m_TransTeamMask.gameObject.SetActive(active);
    }

    public void SetTeamMark(uint id)
    {
       var data = CSVTeamLogo.Instance.GetConfData(id);

        if (data == null || data.TeamIcon == 0)
        {
            m_ImgTeamMaskIcon.gameObject.SetActive(false);
            return;
        }
            
        if(m_ImgTeamMaskIcon.gameObject.activeSelf == false)
            m_ImgTeamMaskIcon.gameObject.SetActive(true);

        ImageHelper.SetIcon(m_ImgTeamMaskIcon, data.TeamIcon);
        
    }

    public void SetTeamMaskFx(bool active)
    {
        if (m_PriTeamMaskFx.gameObject.activeSelf != active)
            m_PriTeamMaskFx.gameObject.SetActive(active);
    }

    public void SetTalkGridActive(bool active)
    {
        if (m_BtnShoutGrid.gameObject.activeSelf != active)
            m_BtnShoutGrid.gameObject.SetActive(active);
    }


    public Vector3 GetMemberItemPosition(int index)
    {
        var item =  getMemberAt(index);

        if (item == null)
            return Vector3.zero;


        return item.GetPosition();

    }
}


public partial class UI_Team_Member_Layout
{
    #region class member
    public class Member :ClickItem
    {
        public Transform m_transform;

        private Transform m_TransReal;
        private Transform m_TransEmpty;

        public Image m_ISelect;
        public Text m_TexName;
        public Image m_IProfession;//职业图标
       // public Text m_TexNumber;
        public Text m_TexProfession;//职业名称
        public Text m_TexLevel;
        public Text m_TexTitle;//称号
                               // public Image m_ISign;//状态

        public Image m_ImTitle;
        public Transform m_CommandTrans;
        public Text m_TexCommandSign;

        public Button m_Btn;

        public ClickItemEvent OnClick = new ClickItemEvent();

        private string m_Name;

        private List<Transform> m_StateTrans = new List<Transform>();


        private Transform mEntrustTrans; // 指挥标记

        public ulong RoleID { get; set; }

        public uint Career { get; set; }

        public uint WeaponID { get; set; }

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
                SetProfession(m_Profession,value);
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

                TextHelper.SetText(m_TexLevel, LanguageHelper.GetTextContent(1000002, m_Level.ToString()));
            }
        }

        private int m_Title;
        public int Title
        {
            get { return m_Title; }
            set
            {
                if (m_Title == value)
                    return;
                m_Title = value;

                TextHelper.SetText(m_TexLevel, m_Level.ToString());
            }
        }

        private bool m_bSelcet;
        public bool bSclect
        {

            get { return m_bSelcet; }
            set
            {

                m_bSelcet = value;

                m_ISelect.gameObject.SetActive(value);
            }
        }

        private bool m_bDelegation = false;
        public bool bDelegation
        {
            get { return m_bDelegation; }
            set
            {
                m_bDelegation = value;
                m_CommandTrans.gameObject.SetActive(value);
                if (m_bDelegation)
                    TextHelper.SetText(m_TexCommandSign,10711);
            }
        }
        private int m_Index;
        public int Index { get { return m_Index; } set { m_Index = value; /*m_TexNumber.text = m_Index.ToString();*/ } }

        private uint m_MemType = 0;


        private bool m_bChangeOrder = false;



        public Transform TransFiguration;

        public TransformCell Figuration = new TransformCell();

        public uint FigurationID { get; set; }

        public uint MemType // 0 无,1 队长，2暂离，3离线，4助阵
        {
            get { return m_MemType; }
            set
            {

                m_MemType = value;

                SetStateIcon(m_MemType);
            }
        }



        private CSVPartner.Data mPartnerData;
        private void SetStateIcon(uint uvalue)
        {
            int count = m_StateTrans.Count;

            if (uvalue*2 > count )
                return;

            for (int i = 0; i < count; i++)
            {
                if (m_StateTrans[i].gameObject.activeSelf)
                    m_StateTrans[i].gameObject.SetActive(false);
            }

            if (uvalue == 0)
                return;

            int index = (int)uvalue - 1;

            m_StateTrans[index * 2].gameObject.SetActive(true);
            m_StateTrans[index * 2 + 1].gameObject.SetActive(true);
        }

        private void SetProfession(uint value,uint rank)
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

            TextHelper.SetText(m_TexProfession, OccupationHelper.GetTextID(value, rank));
            if (icon != 0)
                ImageHelper.SetIcon(m_IProfession, icon);
        }

        /// <summary>
        /// 设置指挥标记
        /// </summary>
        /// <param name="b"></param>
        public void SetmEntrustState(bool b)
        {
            mEntrustTrans.gameObject.SetActive(b);
        }
        private void OnClickItem()
        {
            OnClick?.Invoke(Index);
        }

        public override void Load(Transform transform)
        {
            m_transform = transform;

            m_TransReal = transform.Find("real");
            m_TransEmpty = transform.Find("empty");

            m_ISelect = m_TransReal.Find("Image_Select").GetComponent<Image>();
            m_TexName = m_TransReal.Find("Text_Name").GetComponent<Text>();
            m_IProfession = m_TransReal.Find("Image_Prop").GetComponent<Image>();
           // m_TexNumber = m_TransReal.Find("Text_Number").GetComponent<Text>();
            m_TexProfession = m_TransReal.Find("Image_Prop/Text_Profession").GetComponent<Text>();
            m_TexLevel = m_TransReal.Find("Text_Level").GetComponent<Text>();


            m_TexTitle = m_TransReal.Find("Title/Image1/Text").GetComponent<Text>();

            m_ImTitle = m_TransReal.Find("Title/Image1").GetComponent<Image>();

            // m_ISign = transform.Find("Image_State").GetComponent<Image>();

            m_CommandTrans = m_TransReal.Find("Image_Command");
            m_TexCommandSign = m_TransReal.Find("Image_Command/Text").GetComponent<Text>();

           // rawImage = m_TransReal.Find("RawImage").GetComponent<RawImage>();

            m_Btn = m_TransReal.Find("Button").GetComponent<Button>();

            m_Btn.onClick.AddListener(OnClickItem);


            m_StateTrans.Add(m_TransReal.Find("state/Image_State_Leader"));
            m_StateTrans.Add(m_TransReal.Find("state/Image_Leader"));
            m_StateTrans.Add(m_TransReal.Find("state/Image_State_Leavel"));
            m_StateTrans.Add(m_TransReal.Find("state/Image_Leavel"));
            m_StateTrans.Add(m_TransReal.Find("state/Image_State_offline"));
            m_StateTrans.Add(m_TransReal.Find("state/Image_offline"));
            m_StateTrans.Add(m_TransReal.Find("state/Image_State_Assistant"));
            m_StateTrans.Add(m_TransReal.Find("state/Image_Assistant"));

            mEntrustTrans = m_TransReal.Find("Image_Entrust");


            TransFiguration = m_TransReal.Find("Transfiguration");
            Figuration.Init(TransFiguration);
            Figuration.action = OnClickFiguration;

        }

        public override ClickItem Clone()
        {
            return Clone<Member>(this);
        }
        public void SetTitile(string str, List<Color32> colors)
        {
            m_TexTitle.text = str;

            if (colors == null || colors.Count == 0)
                return;
            TextHelper.SetTextGradient(m_TexTitle, colors[0],colors[1]);
            TextHelper.SetTextOutLine(m_TexTitle, colors[2]);
        }

        public void SetTitile(string str, CSVWordStyle.Data wordstyle )
        {
            TextHelper.SetText(m_TexTitle, str, wordstyle);

        }

        public void SetTitileIcon(uint iconID)
        {
            if (iconID == 0)
            {
                m_ImTitle.gameObject.SetActive(false);

                return;
            }

            if(m_ImTitle.gameObject.activeSelf == false)
                m_ImTitle.gameObject.SetActive(true);

            ImageHelper.SetIcon(m_ImTitle, iconID);
        }


        public void SetShowChangeOrder(bool b)
        {
            m_CommandTrans.gameObject.SetActive(b);

            TextHelper.SetText(m_TexCommandSign,10712);

            m_bChangeOrder = b;
        }

        public void SetEmpty(bool b)
        {
            m_TransReal.gameObject.SetActive(!b);
        }


        public Vector3 GetPosition()
        {
            return m_TransEmpty.position;
        }

        public void OnClickFiguration(ulong id)
        {
            PropIconLoader.ShowItemData iItemData = new PropIconLoader.ShowItemData(0, 1, true, false, false, false, false, false, false);

            iItemData.id = (uint) id;

            var boxEvt = new MessageBoxEvt(EUIID.UI_Team_Member, iItemData);

            boxEvt.b_ForceShowScource = false;

            UIManager.OpenUI(EUIID.UI_Message_Box, false, boxEvt);
        }
    }
    #endregion

}



public partial class UI_Team_Member_Layout
{

}