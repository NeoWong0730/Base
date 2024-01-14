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
    public class BossTowerResultParam
    {
        public bool isBoss;//是否是boss战
        public uint stageId;//当前层数||阶段
        public bool isCanShowReward;//是否可以展示奖励(已通过不重复展示)
        public BattleReward battleReward;
    }
    // 挑战结算界面
    public class UI_BossTower_Result : UIBase
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
                param = arg as BossTowerResultParam;
        }
        protected override void ProcessEventsForEnable(bool toRegister)
        {
        }
        #endregion
        #region 组件
        Button btnOffBg;

        Transform qualifierTrans;
        Text qualifierTips;
        Transform curQualifierRewardNode;
        Transform nextQualifierRewardNode;
        Button btnGoOn,btnQuit;

        Transform bossTrans;
        Transform bossRewardNode;
        Text bossTips;

        Transform commonTipsTrans;
        Text commonTips;
        #endregion
        #region 数据
        IList<ItemIdCount> bossStageRewardList= new List<ItemIdCount>();
        BossTowerResultParam param;
        BossTowerQualifierData bossTowerQualifierData;
        BossTowerStageData bossTowerStageData;
        public UI_RewardList curRewardList;
        public UI_RewardList nextRewardList;
        uint qualifierNextTid;
        #endregion
        #region 组件查找、事件注册
        private void OnParseComponent()
        {
            btnOffBg = transform.Find("Animator/off-bg").GetComponent<Button>();

            qualifierTrans = transform.Find("Animator/Qualifier");
            qualifierTips = qualifierTrans.Find("Text").GetComponent<Text>();
            curQualifierRewardNode = qualifierTrans.Find("View/Grid");
            nextQualifierRewardNode = qualifierTrans.Find("NextNode/View/Grid2");
            btnGoOn = qualifierTrans.Find("Btn_GoOn").GetComponent<Button>();
            btnQuit = qualifierTrans.Find("Btn_Quit").GetComponent<Button>();

            bossTrans = transform.Find("Animator/Boss");
            bossRewardNode = bossTrans.Find("Scroll_View/Viewport");
            bossTips = bossTrans.Find("Text").GetComponent<Text>();

            commonTipsTrans = transform.Find("Animator/CommonTips");
            commonTips = commonTipsTrans.Find("Text").GetComponent<Text>();

            btnOffBg.onClick.AddListener(() => {
                if (bossTrans.gameObject.activeSelf || commonTipsTrans.gameObject.activeSelf)
                    CloseSelf();
            });
            btnGoOn.onClick.AddListener(OnClickGoOn);
            btnQuit.onClick.AddListener(OnClickQuit);
        }
        private void OnClickGoOn()
        {
            Sys_ActivityBossTower.Instance.OnBossTowerChallengeReq(false, qualifierNextTid);
            CloseSelf();
        }
        private void OnClickQuit()
        {
            CloseSelf();
        }
        #endregion
        private void InitView()
        {
            qualifierNextTid = 0;
            if (param.isBoss)
            {
                bossTowerStageData = Sys_ActivityBossTower.Instance.GetBossTowerStageData(param.stageId);
                qualifierTrans.gameObject.SetActive(false);
                bossTrans.gameObject.SetActive(param.isCanShowReward);
                commonTipsTrans.gameObject.SetActive(!param.isCanShowReward);
                if (param.isCanShowReward)
                    SetBossData();
                else
                    SetCommonData();
            }
            else
            {
                btnGoOn.gameObject.SetActive(true);
                btnQuit.gameObject.SetActive(true);
                bossTowerQualifierData = Sys_ActivityBossTower.Instance.GetBossTowerQualifierData(param.stageId);
                qualifierNextTid = bossTowerQualifierData.csvData.nextFloor_id;
                bossTrans.gameObject.SetActive(false);
                qualifierTrans.gameObject.SetActive(param.isCanShowReward);
                commonTipsTrans.gameObject.SetActive(!param.isCanShowReward);
                if (param.isCanShowReward)
                {
                    if (qualifierNextTid != 0)
                        SetQualifierData();
                    else
                    {
                        btnGoOn.gameObject.SetActive(false);
                        btnQuit.gameObject.SetActive(false);
                        bossTrans.gameObject.SetActive(true);
                        qualifierTrans.gameObject.SetActive(false);
                        SetBossData();
                    }
                }
                else
                    SetCommonData();
            }
        }
        private void SetQualifierData()
        {
            if (curRewardList == null)
                curRewardList = new UI_RewardList(curQualifierRewardNode, EUIID.UI_BossTower_Result);
            if (bossTowerQualifierData != null)
            {
                IList<ItemIdCount> dropList = CSVDrop.Instance.GetDropItem(bossTowerQualifierData.csvData.floor_drop);
                curRewardList.SetRewardList(dropList);
                curRewardList?.Build(true, false, false, false, false, true, false, true, PropItem.OnClickPropItem, false, false);
            }
            if (nextRewardList == null)
                nextRewardList = new UI_RewardList(nextQualifierRewardNode, EUIID.UI_BossTower_Result);
            BossTowerQualifierData nextQualifierData = Sys_ActivityBossTower.Instance.GetBossTowerQualifierData(qualifierNextTid);
            if (nextQualifierData != null)
            {
                IList<ItemIdCount> dropList = CSVDrop.Instance.GetDropItem(nextQualifierData.csvData.floor_drop);
                nextRewardList.SetRewardList(dropList);
                nextRewardList?.Build(true, false, false, false, false, true, false, true, PropItem.OnClickPropItem, false, false);
            }
            qualifierTips.text = LanguageHelper.GetTextContent(1009305, LanguageHelper.GetTextContent(bossTowerQualifierData.csvData.name));
        }
        private void SetBossData()
        {
            string str;
            if (param.isBoss)
            {
                bossStageRewardList.Clear();
                if (param.battleReward != null && param.battleReward.Items != null && param.battleReward.Items.Count > 0)
                {
                    for (int i = 0; i < param.battleReward.Items.Count; i++)
                    {
                        bossStageRewardList.Add(new ItemIdCount(param.battleReward.Items[i].Infoid, param.battleReward.Items[i].Count));
                    }
                }
                if (curRewardList == null)
                    curRewardList = new UI_RewardList(bossRewardNode, EUIID.UI_BossTower_Result);
                curRewardList.SetRewardList(bossStageRewardList);
                curRewardList?.Build(true, false, false, false, false, true, false, true, PropItem.OnClickPropItem, false, false);
                str = LanguageHelper.GetTextContent(1009308, LanguageHelper.GetTextContent(bossTowerStageData.csvData.name));
            }
            else
            {
                if (curRewardList == null)
                    curRewardList = new UI_RewardList(bossRewardNode, EUIID.UI_BossTower_Result);
                if (bossTowerQualifierData != null)
                {
                    IList<ItemIdCount> dropList = CSVDrop.Instance.GetDropItem(bossTowerQualifierData.csvData.floor_drop);
                    curRewardList.SetRewardList(dropList);
                    curRewardList?.Build(true, false, false, false, false, true, false, true, PropItem.OnClickPropItem, false, false);
                }
                str = LanguageHelper.GetTextContent(1009305, LanguageHelper.GetTextContent(bossTowerQualifierData.csvData.name));
            }
            bossTips.text = str;
        }
        private void SetCommonData()
        {
            commonTips.text = LanguageHelper.GetTextContent(1009312);
        }
    }
}