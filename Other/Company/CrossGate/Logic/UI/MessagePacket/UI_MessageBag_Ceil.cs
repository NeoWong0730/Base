using Lib.Core;
using Logic;
using Logic.Core;
using System;
using Table;
using UnityEngine;
using UnityEngine.UI;

public class UI_MessageBag_Ceil : UIComponent
{
    private Transform mTrans;
    private Button accpetBtn;
    private Button cancelBtn;
    private Button checkButton;
    private Text txtCountDown;

    private GameObject teamItem;
    private GameObject teamName;
    private Text teamTarget;
    private Text teamLimit;
    private GameObject imgMentor;
    private Text txtMentor;

    private GameObject friendItem;
    private GameObject friendName;
    private Text friendLv;
    private Text friendJob;

    private GameObject familyItem;
    private GameObject invitorName;
    private GameObject familyName;

    private GameObject braveItem;
    private GameObject braveInvitor;
    private Text braveName;

    private uint realCountTime;
    private EMessageBagType mType;
    private Sys_MessageBag.MessageContent mContent;
    private Action<UI_MessageBag_Ceil> onOver;

    //刷新倒计时
    protected override void Refresh()
    {

        realCountTime = Sys_MessageBag.Instance.GetRunTime(mContent);
        txtCountDown.text = LanguageHelper.GetTextContent(1002501, realCountTime.ToString());
        if (Sys_MessageBag.Instance.CheckDestory(mContent))
        {
            Sys_MessageBag.Instance.AutoRefuseSever(mType, mContent);
            Sys_MessageBag.Instance.RemoveContentFromList(mType, mContent);
            onOver?.Invoke(this);
        }

    }

    public override void OnDestroy()
    {
        base.OnDestroy();
    }
    public void BindGameObject(GameObject gameObject)
    {
        mTrans = gameObject.transform;
        accpetBtn = mTrans.Find("Buttons/Btn_01").GetComponent<Button>();
        accpetBtn.onClick.AddListener(OnAcceptClick);
        cancelBtn = mTrans.Find("Buttons/Btn_02").GetComponent<Button>();
        cancelBtn.onClick.AddListener(OnCancelClick);
        checkButton = mTrans.Find("Buttons/Btn_03").GetComponent<Button>();
        checkButton.onClick.AddListener(OnCheckClick);
        txtCountDown = mTrans.Find("Buttons/Btn_02/Text").GetComponent<Text>();

        teamItem = mTrans.Find("Team").gameObject;
        teamName = mTrans.Find("Team/Name").gameObject;
        teamTarget = mTrans.Find("Team/Target").GetComponent<Text>();
        teamLimit = mTrans.Find("Team/Limit").GetComponent<Text>();
        imgMentor = mTrans.Find("Team/Mentor").gameObject;
        txtMentor = mTrans.Find("Team/Mentor/Text").GetComponent<Text>();

        friendItem = mTrans.Find("Friend").gameObject;
        friendName = mTrans.Find("Friend/Name").gameObject;
        friendLv = mTrans.Find("Friend/LV").GetComponent<Text>();
        friendJob = mTrans.Find("Friend/Job").GetComponent<Text>();

        familyItem = mTrans.Find("Family").gameObject;
        invitorName = mTrans.Find("Family/Name").gameObject;
        familyName = mTrans.Find("Family/FamilyName").gameObject;

        braveItem = mTrans.Find("Brave").gameObject;
        braveInvitor = mTrans.Find("Brave/Name").gameObject;
        braveName = mTrans.Find("Brave/BraveTeamName").GetComponent<Text>();

        teamName.GetComponent<Button>().onClick.AddListener(OnInvitorNameClicked);
        friendName.GetComponent<Button>().onClick.AddListener(OnInvitorNameClicked);
        invitorName.GetComponent<Button>().onClick.AddListener(OnInvitorNameClicked);
        familyName.GetComponent<Button>().onClick.AddListener(OnFamilyNameClick);
        braveInvitor.GetComponent<Button>().onClick.AddListener(OnInvitorNameClicked);

    }
    public void SetDate(Sys_MessageBag.MessageContent messageContent)
    {
        mType = messageContent.mType;
        mContent = messageContent;
        if (messageContent == null)
        {
            return;
        }
        CeilTypeInit();
        checkButton.gameObject.SetActive(false);
        switch (messageContent.mType)
        {

            case EMessageBagType.Team:
                TeamItemShow(messageContent,0);
                break;
            case EMessageBagType.Friend:
                friendItem.SetActive(true);
                friendName.GetComponent<Text>().text = messageContent.invitiorName;
                friendLv.text = LanguageHelper.GetTextContent(4811, messageContent.friMess.roleLv.ToString());
                friendJob.text = LanguageHelper.GetTextContent(CSVCareer.Instance.GetConfData(messageContent.friMess.career).name);

                break;
            case EMessageBagType.Family:
                familyItem.SetActive(true);
                invitorName.GetComponent<Text>().text = messageContent.invitiorName;
                familyName.GetComponent<Text>().text = messageContent.cMess.guildName.ToString();

                break;
            case EMessageBagType.Tutor:
                TeamItemShow(messageContent,1);
                break;
            case EMessageBagType.BraveTeam:
                braveItem.SetActive(true);
                braveInvitor.GetComponent<Text>().text = messageContent.invitiorName;
                braveName.text = messageContent.cMess.guildName;
                checkButton.gameObject.SetActive(true);
                break;
            default: break;



        }
    }

    private void CeilTypeInit()
    {
        teamItem.SetActive(false);
        friendItem.SetActive(false);
        familyItem.SetActive(false);
        braveItem.SetActive(false);
    }
    private void TeamItemShow(Sys_MessageBag.MessageContent messageContent,int _type)
    {//type 0-普通 1-导师
        teamItem.SetActive(true);
        teamName.GetComponent<Text>().text = messageContent.invitiorName;
        teamTarget.text = LanguageHelper.GetTextContent(CSVTeam.Instance.GetConfData(messageContent.tMess.targetId).play_name);
        bool _state = (_type == 0 &&!messageContent.tMess.isComeBack);
        imgMentor.SetActive(!_state);
        if (messageContent.tMess.isComeBack)
        {
            txtMentor.text = LanguageHelper.GetTextContent(2014912); //回归
        }
        else
        {
            txtMentor.text = LanguageHelper.GetTextContent(2014913); // 导师
        }

        if (messageContent.tMess.highLv != 0)
        {
            teamLimit.text = LanguageHelper.GetTextContent(1003212, messageContent.tMess.lowLv.ToString(), messageContent.tMess.highLv.ToString());

        }
        else
        {
            teamLimit.text = LanguageHelper.GetTextContent(1003213);
        }

    }
    private void OnInvitorNameClicked()
    {
        Sys_Role_Info.Instance.OpenRoleInfo(mContent.invitorId, Sys_Role_Info.EType.MsgBag);
    }
    private void OnFamilyNameClick()
    {
        if (Sys_Family.Instance.familyData.isInFamily == false)
        {
            UIManager.OpenUI(EUIID.UI_ApplyFamily, false, new Tuple<uint, object>(1u, mContent.cMess.guildId));
        }
    }
    private void OnAcceptClick()
    {
        switch (mType)
        {

            case EMessageBagType.Team:

                Sys_Team.Instance.Send_MessageBag_InviteOpReq(0, mContent.tMess.teamId, mContent.invitorId);
                break;
            case EMessageBagType.Friend:
                Sys_Society.Instance.AcceptAddFriendReq(mContent.invitorId,mContent.invitiorName);
                break;
            case EMessageBagType.Family:
                Sys_Family.Instance.SendGuildInviteRpl(mContent.cMess.guildId, mContent.invitorId, mContent.invitiorName, 0);
                break;
            case EMessageBagType.Tutor:
                Sys_Team.Instance.Send_MessageBag_InviteOpReq(0, mContent.tMess.teamId, mContent.invitorId);
                break;
            default: break;


        }
        Sys_MessageBag.Instance.RemoveContentFromList(mType, mContent);
        onOver?.Invoke(this);
    }
    private void OnCancelClick()
    {
        switch (mType)
        {

            case EMessageBagType.Team:
                Sys_Team.Instance.Send_MessageBag_InviteOpReq(1, mContent.tMess.teamId, mContent.invitorId);
                break;
            case EMessageBagType.Friend:
                Sys_Society.Instance.RefuseAddFriendReq(mContent.invitorId, mContent.invitiorName);
                break;
            case EMessageBagType.Family:
                Sys_Family.Instance.SendGuildInviteRpl(mContent.cMess.guildId, mContent.invitorId, mContent.invitiorName, 1);
                break;
            case EMessageBagType.Tutor:
                Sys_Team.Instance.Send_MessageBag_InviteOpReq(1, mContent.tMess.teamId, mContent.invitorId);
                break;
            case EMessageBagType.BraveTeam:
                Sys_WarriorGroup.Instance.ReqRefuseInvite(mContent.invitorId,mContent.cMess.guildId);
                break;
            default: break;


        }
        Sys_MessageBag.Instance.RemoveContentFromList(mType, mContent);
        onOver?.Invoke(this);


    }

    private void OnCheckClick()
    {
        switch (mType)
        {
            case EMessageBagType.BraveTeam:
                UIManager.CloseUI(EUIID.UI_MessageBag);
                if (Sys_WarriorGroup.Instance.invitedInfoDict.ContainsKey(mContent.invitorId))
                {
                    UIManager.OpenUI(EUIID.UI_WarriorGroup_Sign, false, Sys_WarriorGroup.Instance.invitedInfoDict[mContent.invitorId]);
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(13579));
                    Sys_MessageBag.Instance.RemoveContentFromList(mType, mContent);
                    onOver?.Invoke(this);
                }
                break;
            default: break;
        }
    }
    public void AddRefreshListener(Action<UI_MessageBag_Ceil> onOvered = null)
    {
        onOver = onOvered;
    }



}