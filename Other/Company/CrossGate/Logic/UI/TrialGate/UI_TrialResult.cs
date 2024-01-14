using System;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Table;
using UnityEngine.UI;
using Packet;
using Framework;

namespace Logic
{
    public class TrialResultParam
    {
        public uint type;//1胜利，未完成所有阶段 2胜利，已完成所有阶段 3战败
        public CmdBattleEndNtf cmdBattleEndNtf;
    }
    //试炼结算界面
    public class UI_TrialResult : UIBase
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
        protected override void OnOpen(object arg)
        {
            if (arg != null)
                trialResultParam = arg as TrialResultParam;
        }
        #endregion
        #region 组件
        Transform transSuccess;
        Transform rewardPar;
        Text successTextResidue;
        GameObject objLock;
        Text successTextLock;

        Transform transFailed;
        Text failedTextResidue;
        #endregion
        #region 数据
        TrialResultParam trialResultParam;
        IList<ItemIdCount> showReward = new List<ItemIdCount>();
        UI_RewardList rewardList;
        #endregion
        #region 组件查找、事件注册
        private void OnParseComponent()
        {
            transSuccess = transform.Find("Animator/Image_Successbg");
            rewardPar = transSuccess.Find("View_Victory/RewardNode/Scroll_View/Viewport");
            transSuccess.Find("View_Victory/Lock_bg/Text").GetComponent<SurfaceLanguage>().enabled = false;
            transSuccess.Find("View_Victory/Text_Residue").GetComponent<SurfaceLanguage>().enabled = false;
            objLock = transSuccess.Find("View_Victory/Lock_bg").gameObject;
            successTextLock = transSuccess.Find("View_Victory/Lock_bg/Text").GetComponent<Text>();
            transSuccess.Find("View_Victory/Lock_bg/Text1").gameObject.SetActive(false);
            successTextResidue = transSuccess.Find("View_Victory/Text_Residue").GetComponent<Text>();
            transSuccess.Find("View_Victory/Text_Residue/Value").gameObject.SetActive(false);

            transFailed = transform.Find("Animator/Image_Failedbg");
            transFailed.Find("View_Fail/Text_Residue").GetComponent<SurfaceLanguage>().enabled = false;
            failedTextResidue = transFailed.Find("View_Fail/Text_Residue").GetComponent<Text>();
            transFailed.Find("View_Fail/Text_Residue/Value").gameObject.SetActive(false); 
        }
        #endregion
        private void InitView()
        {
            if (trialResultParam != null)
            {
                if (trialResultParam.type == 1)
                {
                    transSuccess.gameObject.SetActive(true);
                    transFailed.gameObject.SetActive(false);
                    SetRewardList();
                    SetContent(1);
                }
                else if (trialResultParam.type == 2)
                {
                    transSuccess.gameObject.SetActive(true);
                    transFailed.gameObject.SetActive(false);
                    SetContent(2);
                }
                else
                {
                    transSuccess.gameObject.SetActive(false);
                    transFailed.gameObject.SetActive(true);
                    SetContent(3);
                }
            }
        }
        private void SetRewardList()
        {
            showReward.Clear();
            for (int i = 0, length = trialResultParam.cmdBattleEndNtf.Rewards.Items.Count; i < length; ++i)
            {
                showReward.Add(new ItemIdCount(trialResultParam.cmdBattleEndNtf.Rewards.Items[i].Infoid, trialResultParam.cmdBattleEndNtf.Rewards.Items[i].Count));
            }
            if (rewardList == null)
                rewardList = new UI_RewardList(rewardPar, EUIID.UI_TrialResult);
            rewardList.SetRewardList(showReward);
            rewardList?.Build(true, false, false, false, false, true, false, true, PropItem.OnClickPropItem, false, false);
        }
        /// <summary>
        /// 设置战斗结果文本内容
        /// </summary>
        /// <param name="type"></param>
        private void SetContent(uint type)
        {
            //胜利，未完成所有阶段
            if (type == 1)
            {
                objLock.SetActive(false);
                successTextResidue.gameObject.SetActive(true);
                successTextLock.gameObject.SetActive(false);
                TextHelper.SetText(successTextResidue, 3899000015, trialResultParam.cmdBattleEndNtf.TrialGate.Stage.ToString());
            }
            //胜利，已完成所有阶段
            else if (type == 2)
            {
                objLock.SetActive(true);
                successTextResidue.gameObject.SetActive(false);
                successTextLock.gameObject.SetActive(true);
                TextHelper.SetText(successTextLock, 3899000016);
            }
            //战败
            else
            {
                failedTextResidue.gameObject.SetActive(true);
                TextHelper.SetText(failedTextResidue, 3899000024);
            }
        }
    }
}