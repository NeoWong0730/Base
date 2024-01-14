using Logic.Core;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using Table;
using UnityEngine.UI;
using UnityEngine;
using Lib.Core;
using System;
using Packet;

namespace Logic
{
    /// <summary> 进入经典Boss战确认界面 </summary>
    public class UI_ClassicBossWarEnterConfirm : UIBase
    {
        #region 界面组件
        /// <summary> 标题 </summary>
        private Text text_Title;
        /// <summary> 倒计时 </summary>
        private Image image_Time;
        /// <summary> 倒计时 </summary>
        private Text text_Time;
        /// <summary> 同意按钮 </summary>
        private Button button_Agree;
        /// <summary> 拒绝按钮 </summary>
        private Button button_Refuse;
        /// <summary> 放弃进入 </summary>
        private Button button_Giveup;
        /// <summary> 成员节点 </summary>
        private GameObject go_memberNode;
        #endregion
        #region 数据定义
        /// <summary> 更新行为 </summary>
        public Action updateAction = null;
        /// <summary> 定时器 </summary>
        private Timer timer = null;
        /// <summary> 投票数据 </summary>
        private ClassicBossCliVoteData classicBossCliVoteData;
        /// <summary> 投票时间 </summary>
        private float totalVotingTime;
        /// <summary> 当前投票时间 </summary>
        private float curVotingTime;
        #endregion
        #region 系统函数
        protected override void OnInit()
        {
        }
        protected override void OnLoaded()
        {
            OnParseComponent();
        }
        protected override void OnDestroy()
        {

        }
        protected override void OnOpen(object arg)
        {
            //classicBossCliVoteData = arg == null ? new ClassicBossCliVoteData() : (ClassicBossCliVoteData)arg;
            totalVotingTime = uint.Parse(CSVParam.Instance.GetConfData(351).str_value) / 1000f;
        }
        protected override void OnOpened()
        {
            curVotingTime = totalVotingTime;
        }
        protected override void OnShow()
        {
            RefreshView();
        }
        protected override void OnHide()
        {
            timer?.Cancel();
        }
        protected override void OnUpdate()
        {

        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
            OnProcessEventsForEnable(toRegister);
        }
        #endregion
        #region 初始化
        /// <summary>
        /// 检测组件 
        /// </summary>
        private void OnParseComponent()
        {
            text_Title = transform.Find("Animator/View_Right/Image_Title/Text02").GetComponent<Text>();
            image_Time = transform.Find("Animator/View_Right/Image_Time/Image_Time").GetComponent<Image>();
            text_Time = transform.Find("Animator/View_Right/Image_Time/Text").GetComponent<Text>();
            button_Agree = transform.Find("Animator/View_Right/Btnlist/Btn_01").GetComponent<Button>();
            button_Refuse = transform.Find("Animator/View_Right/Btnlist/Btn_02").GetComponent<Button>();
            button_Giveup = transform.Find("Animator/View_Right/Btnlist/Btn_03").GetComponent<Button>();
            go_memberNode = transform.Find("Animator/View_Left/Scroll View01/Viewport/Content").gameObject;

            transform.Find("Animator/TtileAndBg/Btn_Close").GetComponent<Button>().onClick.AddListener(OnClick_Close);
            button_Agree.onClick.AddListener(OnClick_Agree);
            button_Refuse.onClick.AddListener(OnClick_Refuse);
            button_Giveup.onClick.AddListener(OnClick_Close);
        }
        /// <summary>
        /// 注册事件
        /// </summary>
        /// <param name="toRegister"></param>
        private void OnProcessEventsForEnable(bool toRegister)
        {
            Sys_ClassicBossWar.Instance.eventEmitter.Handle<ulong>(Sys_ClassicBossWar.EEvents.DoVote, OnVoteNtf, toRegister);
        }
        #endregion
        #region 数据处理
        #endregion
        #region 界面显示
        /// <summary>
        /// 刷新界面
        /// </summary>
        private void RefreshView()
        {
            classicBossCliVoteData = Sys_ClassicBossWar.Instance.curBossCliVoteData;
            SetTimer();
            SetTitle();
            SetMemberList();
        }
        /// <summary>
        /// 设置定时器
        /// </summary>
        private void SetTimer()
        {
            updateAction = () =>
            {
                curVotingTime -= 1f;
                if (curVotingTime < 0)
                {
                    curVotingTime = 0;
                }
                text_Time.text = curVotingTime.ToString();
                image_Time.fillAmount = curVotingTime / totalVotingTime;
                if (curVotingTime <= 0)
                {
                    updateAction = null;
                }
            };
            timer?.Cancel();
            timer = Timer.Register(1f, () =>
            {
                updateAction?.Invoke();
            }, null, true);

            text_Time.text = curVotingTime.ToString();
            image_Time.fillAmount = curVotingTime / totalVotingTime;
        }
        /// <summary>
        /// 设置标题
        /// </summary>
        private void SetTitle()
        {
            uint classicBossId = null == classicBossCliVoteData ? 0 : classicBossCliVoteData.ClassicBossId;
            CSVClassicBoss.Data cSVClassicBossData = CSVClassicBoss.Instance.GetConfData(classicBossId);
            uint npcId = null == cSVClassicBossData ? 0 : cSVClassicBossData.NPCID;
            CSVNpc.Data cSVNpcData = CSVNpc.Instance.GetConfData(npcId);
            text_Title.text = null == cSVNpcData ? string.Empty : LanguageHelper.GetNpcTextContent(cSVNpcData.name);
        }
        /// <summary>
        /// 设置成员模版
        /// </summary>
        private void SetMemberList()
        {
            var Mems = classicBossCliVoteData.Mems;
            for (int i = 0; i < go_memberNode.transform.childCount; i++)
            {
                Transform tr = go_memberNode.transform.GetChild(i);
                if (i < Mems.Count)
                {
                    tr.gameObject.SetActive(true);
                    SetMemberItem(tr, Mems[i]);
                }
                else
                {
                    tr.gameObject.SetActive(false);
                }
            }
        }
        /// <summary>
        /// 设置成员模版
        /// </summary>
        /// <param name="tr"></param>
        /// <param name="mem"></param>
        private void SetMemberItem(Transform tr, ClassicBossCliVoteData.Types.Mem mem)
        {
            TeamMem roleInfo = Sys_Team.Instance.getTeamMem(mem.RoleId);
            /// <summary> 头像 </summary>
            Image image_Head = tr.Find("Head").GetComponent<Image>();
            CharacterHelper.SetHeadAndFrameData(image_Head, roleInfo.HeroId, roleInfo.Photo, roleInfo.PhotoFrame);
            /// <summary> 名称 </summary>
            Text text_Name = tr.Find("Text_Name").GetComponent<Text>();
            text_Name.text = roleInfo.Name.ToStringUtf8();
            /// <summary> 等级 </summary>
            Text text_Lv = tr.Find("Text_Lv/Text_Num").GetComponent<Text>();
            text_Lv.text = roleInfo.Level.ToString();
            /// <summary> 职业 </summary>
            Image image_Carrier = tr.Find("Profession_bg/Image_Profession").GetComponent<Image>();
            /// <summary> 职业 </summary>
            Text text_Carrier = tr.Find("Profession_bg/Text_Profession").GetComponent<Text>();
            CSVCareer.Data cSVCareerData = CSVCareer.Instance.GetConfData(roleInfo.Career);
            text_Carrier.text = null == cSVCareerData ? string.Empty : LanguageHelper.GetTextContent(cSVCareerData.name);
            ImageHelper.SetIcon(image_Carrier, null == cSVCareerData ? 0 : cSVCareerData.icon);
            /// <summary> 次数 </summary>
            Text text_Count = tr.Find("Time_bg/Text_Num").GetComponent<Text>();
            text_Count.text = mem.LeftTimes.ToString();

            VoterOpType voteState = Sys_ClassicBossWar.Instance.GetVoteOp_ClassicBossWar(mem.RoleId);
            //bool isOffLine = TeamMemHelper.IsOffLine(roleInfo);
            //bool isLeave = TeamMemHelper.IsLeave(roleInfo);
            /// <summary> 已同意 </summary>
            Image image_Ok = tr.Find("Image_Ok").GetComponent<Image>();
            image_Ok.gameObject.SetActive(voteState == VoterOpType.Agree);
            /// <summary> 已拒绝 </summary>
            Image image_No = tr.Find("Image_No").GetComponent<Image>();
            image_No.gameObject.SetActive(voteState == VoterOpType.Disagree);
            /// <summary> 等待中 </summary>
            Image image_Wait = tr.Find("Image_Wait").GetComponent<Image>();
            image_Wait.gameObject.SetActive(voteState == VoterOpType.None);
            /// <summary> 离线 </summary> 
            Image image_Leave = tr.Find("Image_Leave").GetComponent<Image>();
            image_Leave.gameObject.SetActive(false);
            /// <summary> 暂离 </summary>
            Image image_LeaveMoment = tr.Find("Image_LeaveMoment").GetComponent<Image>();
            image_LeaveMoment.gameObject.SetActive(false);

            if (Sys_Role.Instance.RoleId == mem.RoleId)
            {
                SetButton(voteState);
            }
        }
        /// <summary>
        /// 设置按钮
        /// </summary>
        /// <param name="voterOpType"></param>
        private void SetButton(VoterOpType voterOpType)
        {
            button_Giveup.gameObject.SetActive(voterOpType != VoterOpType.None);
            button_Agree.gameObject.SetActive(voterOpType == VoterOpType.None);
            button_Refuse.gameObject.SetActive(voterOpType == VoterOpType.None);
        }
        #endregion
        #region 响应事件
        /// <summary>
        /// 关闭按钮
        /// </summary>
        public void OnClick_Close()
        {
            CloseSelf();
            OnClick_Giveup();
        }
        /// <summary>
        /// 同意
        /// </summary>
        private void OnClick_Agree()
        {
            Sys_ClassicBossWar.Instance.OnDoVoteReq(true);
        }
        /// <summary>
        /// 拒绝
        /// </summary>
        private void OnClick_Refuse()
        {
            Sys_ClassicBossWar.Instance.OnDoVoteReq(false);
        }
        /// <summary>
        /// 放弃进入
        /// </summary>
        private void OnClick_Giveup()
        {
            Sys_ClassicBossWar.Instance.OnDoVoteCancel();
        }
        /// <summary>
        /// 投票更新
        /// </summary>
        /// <param name="roleId"></param>
        private void OnVoteNtf(ulong roleId)
        {
            classicBossCliVoteData = Sys_ClassicBossWar.Instance.curBossCliVoteData;
            SetMemberList();
        }
        #endregion
        #region 提供功能
        #endregion
    }
}