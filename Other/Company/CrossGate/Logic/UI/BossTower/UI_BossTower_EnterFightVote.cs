using System;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Table;
using UnityEngine.UI;
using Lib.Core;
using Packet;
using Framework;

namespace Logic
{
    public class BossTowerVoteParmas
    {
        //是否是队长
        public bool isCaptain;
        //是否是Boss挑战
        public bool isBoss;
        //当前特性id
        public uint featureId;
        //当前挑战的关卡id
        public uint stageId;
        //投票状态 1准备投票 2进行投票 3已投票
        public int voteState;
    }
    //进战投票界面
    public class UI_BossTower_EnterFightVote : UIBase
    {
        #region 系统函数
        protected override void OnLoaded()
        {
            OnParseComponent();
        }
        protected override void OnShow()
        {
            InitView();
        }
        protected override void OnUpdate()
        {
            Update();
        }
        protected override void OnOpen(object arg)
        {
            if (arg != null)
                parmas = arg as BossTowerVoteParmas;
        }
        protected override void OnClose()
        {

        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            OnProcessEventsForEnable(toRegister);
        }
        #endregion
        #region 组件
        Button btnClose;
        Transform parent;
        Image imgBg;
        Text text01, text02, text03, text04;
        Transform timeTrans;
        Image imgSlider;
        Text textTime;
        Text textUint;
        GameObject objTips;
        Button btn01, btn02, btn03;
        Text textBtn01, textBtn02, textBtn03;
        GameObject objWait;
        GameObject objPassed;
        #endregion
        #region 数据
        List<TeamMemItem> teamMemItemList = new List<TeamMemItem>();
        BossTowerVoteParmas parmas;
        int voteState;
        bool isCaptain;
        static bool isBoss;
        uint featureId;
        uint curStageId;
        uint curStageNum;
        CSVBOSSTower.Data featureData;
        BossTowerStageData bossTowerStageData;
        BossTowerQualifierData bossTowerQualifierData;
        uint titleLanId;
        bool cdDirty;
        float waitCD;
        int realtime;
        #endregion
        #region 组件查找、事件注册
        private void OnParseComponent()
        {
            btnClose = transform.Find("Animator/TtileAndBg/Btn_Close").GetComponent<Button>();
            parent = transform.Find("Animator/View_Left/Scroll View01/Viewport/Content");
            imgBg = transform.Find("Animator/View_Right/Image_Bg").GetComponent<Image>();
            text01 = transform.Find("Animator/View_Right/Image_Title/Text01").GetComponent<Text>();
            text02 = transform.Find("Animator/View_Right/Image_Title/Text02").GetComponent<Text>();
            text03 = transform.Find("Animator/View_Right/Image_Title/Text03").GetComponent<Text>();
            text04 = transform.Find("Animator/View_Right/Image_Title/Text04").GetComponent<Text>();
            timeTrans = transform.Find("Animator/View_Right/Image_Time");
            imgSlider = timeTrans.Find("Image_Time").GetComponent<Image>();
            textTime = timeTrans.Find("Text").GetComponent<Text>();
            objTips = transform.Find("Animator/View_Right/Image_Tips").gameObject;
            btn01 = transform.Find("Animator/View_Right/Btnlist/Btn_01").GetComponent<Button>();
            btn02 = transform.Find("Animator/View_Right/Btnlist/Btn_02").GetComponent<Button>();
            btn03 = transform.Find("Animator/View_Right/Btnlist/Btn_03").GetComponent<Button>();
            textBtn01 = btn01.transform.Find("Text").GetComponent<Text>();
            textBtn02 = btn02.transform.Find("Text").GetComponent<Text>();
            textBtn03 = btn03.transform.Find("Text").GetComponent<Text>();
            objWait = transform.Find("Animator/View_Right/Text").gameObject;
            objPassed = transform.Find("Animator/View_Right/Image_Tip2").gameObject;

            text01.gameObject.SetActive(false);
            text02.gameObject.SetActive(false);
            text03.gameObject.SetActive(false);
            text04.gameObject.SetActive(true);
            objTips.SetActive(false);
            btn02.gameObject.SetActive(false);

            btnClose.onClick.AddListener(() => {
                Sys_ActivityBossTower.Instance.OnDoVoteCancel();
                CloseSelf(); 
            });
            btn01.onClick.AddListener(()=> { OnClickBtn(1); });
            btn03.onClick.AddListener(()=> { OnClickBtn(3); });
        }
        private void OnProcessEventsForEnable(bool toRegister)
        {
            //Sys_ActivityBossTower.Instance.eventEmitter.Handle(Sys_ActivityBossTower.EEvents.OnDoVote, OnStartVote, toRegister);
            Sys_ActivityBossTower.Instance.eventEmitter.Handle<ulong>(Sys_ActivityBossTower.EEvents.OnDoVote, OnDoVote, toRegister);
            Sys_ActivityBossTower.Instance.eventEmitter.Handle(Sys_ActivityBossTower.EEvents.OnUpdateVote, OnUpdateVote, toRegister);
            Sys_Team.Instance.eventEmitter.Handle<ulong>(Sys_Team.EEvents.NetMsg_MemState, OnMemberStateChange, toRegister);
            Sys_Team.Instance.eventEmitter.Handle<TeamMem>(Sys_Team.EEvents.MemEnterNtf, OnTeamMemberEnter, toRegister);
            if (toRegister)
                Sys_Fight.Instance.OnEnterFight += OnEnterBattle;
            else
                Sys_Fight.Instance.OnEnterFight -= OnEnterBattle;
        }
        //private void OnStartVote()
        //{
        //    if (isCaptain && voteState == 1)
        //        OnWaitOther();
        //}
        private void OnDoVote(ulong roleId)
        {
            if (roleId == Sys_Role.Instance.RoleId)
            {
                voteState = 3;
                RefreshVoteState();
            }
            RefreshMembersState();
        }
        private void OnUpdateVote()
        {
            RefreshMembersData();
        }
        private void OnMemberStateChange(ulong roleId)
        {
            RefreshMembersState(roleId);
        }
        private void OnTeamMemberEnter(TeamMem teamMem)
        {
            RefreshMembersData();
        }
        private void OnEnterBattle(CSVBattleType.Data cSVBattleTypeTb)
        {
            CloseSelf();
        }
        private void OnClickBtn(uint btnType)
        {
            if (btnType == 1)
            {
                if (voteState == 1)//关闭投票
                    CloseSelf();
                if (voteState == 2) //拒绝
                    Sys_ActivityBossTower.Instance.OnDoVoteReq(false);
                if (voteState == 3)//取消投票
                    Sys_ActivityBossTower.Instance.OnDoVoteCancel();
            }
            else if (btnType == 3)
            {
                //if (isCaptain && voteState == 1)
                //    OnWaitOther();

                if (voteState == 2)//投票同意
                    Sys_ActivityBossTower.Instance.OnDoVoteReq(true);
            }
        }
        //private void OnWaitOther()
        //{
        //    voteState = 3;
        //    RefreshVoteState();
        //    StartTime();
        //    RefreshMembersState();
        //    objWait.SetActive(true);
        //    RefreshPassTips();
        //}
        #endregion
        private void InitView()
        {
            if (parmas != null)
            {
                voteState = parmas.voteState;
                isCaptain = parmas.isCaptain;
                isBoss = parmas.isBoss;
                featureId = parmas.featureId;
                curStageId = parmas.stageId;
                featureData = Sys_ActivityBossTower.Instance.GetBossTowerFeature(featureId);

                if (isBoss)
                {
                    bossTowerStageData = Sys_ActivityBossTower.Instance.GetBossTowerStageData(curStageId, featureId);
                    if (bossTowerStageData != null)
                    {
                        titleLanId = bossTowerStageData.csvData.name;
                        curStageNum = bossTowerStageData.csvData.stage_number;
                        realtime = int.Parse(Sys_ActivityBossTower.Instance.GetBossTowerParameter(102));
                    }
                    else
                        DebugUtil.LogError("CSVBOSSTowerStage not find id:" + curStageId);
                }
                else
                {
                    bossTowerQualifierData = Sys_ActivityBossTower.Instance.GetBossTowerQualifierData(curStageId, featureId);
                    if (bossTowerQualifierData != null)
                    {
                        titleLanId = bossTowerQualifierData.csvData.name;
                        curStageNum = bossTowerQualifierData.csvData.floor_number;
                        realtime = int.Parse(Sys_ActivityBossTower.Instance.GetBossTowerParameter(7));
                    }
                    else
                        DebugUtil.LogError("CSVBOSSTowerQualifier not find id:" + curStageId);
                }
                SetBaseData();
                RefreshPassTips();
                RefreshVoteState();
                RefreshTimeState();
                RefreshMembersData();
            }
        }
        private void Update()
        {
            if (cdDirty)
            {
                waitCD -= deltaTime;
                if (waitCD < 0)
                {
                    cdDirty = false;
                }
                float time = (int)Mathf.Max(0, waitCD) + 1;
                float diffValue = 1 - (waitCD / realtime);
                RefreshTimeShow(time, diffValue);
            }
        }
        private void SetBaseData()
        {
            if (featureData != null)
                ImageHelper.SetIcon(imgBg, featureData.bg);
            objWait.SetActive(isCaptain && voteState == 3);
            text04.text = LanguageHelper.GetTextContent(titleLanId);
        }
        private void RefreshPassTips()
        {
            uint stageId = Sys_ActivityBossTower.Instance.GetRolePassStageId();
            uint stageNum = 0;
            if (isBoss)
            {
                BossTowerStageData bossTowerStageData = Sys_ActivityBossTower.Instance.GetBossTowerStageData(stageId);
                if (bossTowerStageData != null)
                    stageNum = bossTowerStageData.csvData.stage_number;
            }
            else
            {
                BossTowerQualifierData bossTowerQualifierData = Sys_ActivityBossTower.Instance.GetBossTowerQualifierData(stageId);
                if (bossTowerQualifierData != null)
                    stageNum = bossTowerQualifierData.csvData.floor_number;
            }
            objPassed.SetActive(stageNum >= curStageNum);
        }
        private void RefreshTimeState()
        {
            if (voteState == 1)
                StopTime();
            else
                StartTime();
        }
        private void StartTime()
        {
            if (cdDirty) return;
            timeTrans.gameObject.SetActive(true);
            cdDirty = true;
            waitCD = Sys_ActivityBossTower.Instance.GetVoteDiffTime(isBoss);
            RefreshTimeShow(waitCD, 1 - (waitCD / realtime));
        }
        private void StopTime()
        {
            timeTrans.gameObject.SetActive(false);
            cdDirty = false;
        }
        private void RefreshTimeShow(float waitCD,float diffTime)
        {
            textTime.text = waitCD.ToString();
            imgSlider.fillAmount = diffTime;
        }
        private void RefreshVoteState()
        {
            bool btn01Active = true;
            bool btn03Active = voteState != 3;
            uint text01LanId = (uint)(voteState == 1 ? 2009626 : (voteState == 2 ? 2009627 : (voteState == 3 ? 2009619 : 0)));
            uint text03LanId = (uint)(voteState == 1 ? 2009636 : (voteState == 2 ? 2009620 : 0));

            btn01.gameObject.SetActive(btn01Active);
            btn03.gameObject.SetActive(btn03Active);
            if (text01LanId != 0)
                TextHelper.SetText(textBtn01, text01LanId);
            if (text03LanId != 0)
                TextHelper.SetText(textBtn03, text03LanId);
        }
        private void RefreshMembersData()
        {
            int count = Sys_Team.Instance.TeamMemsCount;
            for (int i = 0; i < teamMemItemList.Count; i++)
            {
                TeamMemItem cell = teamMemItemList[i];
                PoolManager.Recycle(cell);
            }
            teamMemItemList.Clear();
            FrameworkTool.CreateChildList(parent, count);
            for (int i = 0; i < count; i++)
            {
                TeamMem teamMem = Sys_Team.Instance.getTeamMem(i);
                Transform trans = parent.GetChild(i);
                TeamMemItem cell = PoolManager.Fetch<TeamMemItem>();
                cell.Init(trans);
                cell.SetData(teamMem);
                teamMemItemList.Add(cell);
            }
            FrameworkTool.ForceRebuildLayout(transform.gameObject);
        }
        private void RefreshMembersState(ulong roleId = 0)
        {
            if (roleId != 0)
            {
                for (int i = 0; i < teamMemItemList.Count; i++)
                {
                    if (teamMemItemList[i].teamMem.MemId == roleId)
                    {
                        teamMemItemList[i].RefreshRoleState();
                        break;
                    }
                }
            }
            else
            {
                for (int i = 0; i < teamMemItemList.Count; i++)
                {
                    teamMemItemList[i].RefreshRoleState();
                }
            }
        }
        #region item
        public class TeamMemItem
        {
            Image head;
            Image careerIcon;
            Text textCareer, textLv,textName,textHigh,textHigh_1;
            GameObject[] objStates = new GameObject[5];

            public TeamMem teamMem;
            BossTowerStageData bossTowerStageData;
            BossTowerQualifierData bossTowerQualifierData;
            public void Init(Transform trans)
            {
                head = trans.Find("Head").GetComponent<Image>();
                careerIcon = trans.Find("Image_Profession").GetComponent<Image>();
                textCareer = trans.Find("Image_Profession/Text").GetComponent<Text>();
                textLv = trans.Find("Text_Lv/Text_Num").GetComponent<Text>();
                textName = trans.Find("Text_Name").GetComponent<Text>();
                textHigh = trans.Find("Text2").GetComponent<Text>();
                textHigh.gameObject.SetActive(true);
                textHigh_1 = trans.Find("Text_High/Text_High (1)").GetComponent<Text>();

                objStates[0] = trans.Find("Image_Ok").gameObject;
                objStates[1] = trans.Find("Image_No").gameObject;
                objStates[2] = trans.Find("Image_Wait").gameObject;
                objStates[3] = trans.Find("Image_Leave").gameObject;
                objStates[4] = trans.Find("Image_LeaveMoment").gameObject;
            }
            public void SetData(TeamMem teamMem)
            {
                this.teamMem = teamMem;
                CharacterHelper.SetHeadAndFrameData(head, teamMem.HeroId, teamMem.Photo, teamMem.PhotoFrame);
                CSVCareer.Data csvCareerData = CSVCareer.Instance.GetConfData(teamMem.Career);
                ImageHelper.SetIcon(careerIcon, csvCareerData.icon);
                textCareer.text = LanguageHelper.GetTextContent(csvCareerData.name);
                textName.text = teamMem.Name.ToStringUtf8();
                textLv.text = teamMem.Level.ToString();

                RefreshRoleState();
                uint stageId = Sys_ActivityBossTower.Instance.GetRolePassStageId(teamMem.MemId);
                uint lanId = 0;
                if (isBoss)
                {
                    bossTowerStageData = Sys_ActivityBossTower.Instance.GetBossTowerStageData(stageId);
                    if (bossTowerStageData != null)
                        lanId = bossTowerStageData.csvData.name;
                }
                else
                {
                    bossTowerQualifierData = Sys_ActivityBossTower.Instance.GetBossTowerQualifierData(stageId);
                    if (bossTowerQualifierData != null)
                        lanId = bossTowerQualifierData.csvData.name;
                }
                textHigh.text = LanguageHelper.GetTextContent(2022349);
                Color oldColor = textHigh_1.color;
                Color newColor = new Color(oldColor.r, oldColor.g, oldColor.b, 1);
                textHigh_1.color = newColor;
                textHigh_1.text = LanguageHelper.GetTextContent(lanId != 0 ? lanId : 2022360);
            }
            public void RefreshRoleState()
            {
                int index = 2;
                if (teamMem.IsLeave() || teamMem.IsOffLine())
                {
                    if (teamMem.IsLeave()) index = 4;
                    if (teamMem.IsOffLine()) index = 3;
                }
                else
                {
                    VoterOpType voterOpType = Sys_ActivityBossTower.Instance.GetBossTowerVoteState(teamMem.MemId);
                    switch (voterOpType)
                    {
                        case VoterOpType.None:
                            index = 2;
                            break;
                        case VoterOpType.Agree:
                            index = 0;
                            break;
                        case VoterOpType.Disagree:
                            index = 1;
                            break;
                        default:
                            break;
                    }
                }
                SetRoleState(index);
            }
            public void SetRoleState(int index)
            {
                for (int i = 0; i < objStates.Length; i++)
                {
                    if (index == i)
                        objStates[i].SetActive(true);
                    else
                        objStates[i].SetActive(false);
                }
            }
        }
        #endregion
    }
}