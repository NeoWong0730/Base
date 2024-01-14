using UnityEngine;
using System.Collections;
using Logic.Core;
using UnityEngine.UI;
using Logic;
using System.Collections.Generic;
using UnityEngine.Events;


//
public partial class UI_Team_Fast_Layout
{
    IListener m_listener;


    private Transform mTips;

    private Text mMatchTex;

    private Button m_BtnMatch;

    private Transform m_TransEmpty;

    private Text m_TextWaitTeamNum;
    private Text m_TextWaitPlayer;

    private GameObject matchingFxGo;

    Transform m_TexFamilyTips;
    public void Loaded(Transform root)
    {
        LoadContent(root);
        LoadLeftContent(root);

        Transform rightTrans = root.Find("Animator/View_Apply/Right_Content");

        Button createBtn = rightTrans.Find("Button_Create").GetComponent<Button>();
        Button refershBtn = rightTrans.Find("Button_Refersh").GetComponent<Button>();
        Button matchBtn = rightTrans.Find("Button_Match").GetComponent<Button>();

        matchingFxGo = rightTrans.Find("Button_Match/Fx_ui_Team_Fast_huanrao").gameObject;

        m_BtnMatch = matchBtn;
        mMatchTex = rightTrans.Find("Button_Match/Text").GetComponent<Text>();

        mTips = rightTrans.Find("Text_Tip");

        Button closeBtn = root.Find("Animator/View_Title01/Btn_Close").GetComponent<Button>();

        m_TransEmpty = root.Find("Animator/View_Apply/Middle_Menu/Empty");


        createBtn.onClick.AddListener(OnClickCreate);
        refershBtn.onClick.AddListener(OnClickRefresh);
        matchBtn.onClick.AddListener(OnClickMatch);

        closeBtn.onClick.AddListener(OnClickClose);

        m_TextWaitTeamNum = root.Find("Animator/View_Apply/Middle_Menu/Waiting/Team/Num").GetComponent<Text>();
        m_TextWaitPlayer = root.Find("Animator/View_Apply/Middle_Menu/Waiting/Player/Num").GetComponent<Text>();

        m_TexFamilyTips = rightTrans.Find("Text_Tip2");
    }

    public void Init()
    {

    }

    public void Hide()
    {
        RestContent();

    }
    public void RegisterEvents(IListener listener)
    {
        m_listener = listener;

    }

    public void SetWaitNum(uint teamnum, uint playernum)
    {
        m_TextWaitTeamNum.text = teamnum.ToString();
        m_TextWaitPlayer.text = playernum.ToString();
    }
    public void SetMatchInteractable(bool b)
    {
        m_BtnMatch.interactable = b;
    }
    public void SetMatch(bool isMatch)
    {
        if (mTips.gameObject.activeSelf != isMatch)
            mTips.gameObject.SetActive(isMatch);

        mMatchTex.text = LanguageHelper.GetTextContent((uint)(isMatch == false ? 2002509 : 2002510));
        if (matchingFxGo.activeInHierarchy != isMatch)
            matchingFxGo.SetActive(isMatch);
    }

    public void SetEmptyActive(bool b)
    {
        if (m_TransEmpty.gameObject.activeSelf != b)
            m_TransEmpty.gameObject.SetActive(b);
    }
    private void OnClickCreate()
    {
        m_listener?.CreateTeam();
    }

    private void OnClickRefresh()
    {
        m_listener?.Refresh();
    }

    private void OnClickMatch()
    {
        m_listener?.Match();
    }

    private void OnClickClose()
    {
        m_listener?.Close();
    }


    public void SetFamilyTipsActive(bool b)
    {
        m_TexFamilyTips.gameObject.SetActive(b);
    }
    public interface IListener
    {
        void CreateTeam();
        void Refresh();
        void Match();

        void Close();

        void OnClickTypeContent(uint id, int index);

        void OnClickChildContent(uint parentID,int parentindex,uint id, int index);

        void Apply(int id);

        void OnFocusChildContent(uint parentID, int parentindex, uint id, int index);
    }



 


 

 

  





}
