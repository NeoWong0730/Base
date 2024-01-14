using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using Table;
using System;
using UnityEngine.UI;
using Packet;

namespace Logic
{
    public class UI_JSBattle_Result : UIBase
    {
        #region 界面组件
        private Button closeBtn;
        private Text curRankText;
        private Text preRankText;
        private Text highRankText;
        private GameObject winGO;
        private GameObject loseGo;
        private Transform rewardTrans;
        private GameObject upGo;
        private GameObject lowGo;
        private GameObject highTagGo;
        #endregion
        #region 数据定义
        /// <summary> 家族训练战数据结果 </summary>
        public CmdBattleEndNtf cmdBattleEndNtf;
        #endregion
        protected override void OnLoaded()
        {
            closeBtn = transform.Find("Animator/off-bg").GetComponent<Button>();
            closeBtn.onClick.AddListener(CloseBtnClicked);
            curRankText = transform.Find("Animator/Now/Num").GetComponent<Text>();
            preRankText = transform.Find("Animator/Now/Num/Text1/Num").GetComponent<Text>();
            highRankText = transform.Find("Animator/Image_History/Text").GetComponent<Text>();
            winGO = transform.Find("Animator/Image_Successbg").gameObject;
            loseGo = transform.Find("Animator/Image_Failedbg").gameObject;
            rewardTrans = transform.Find("Animator/Grid");
            upGo = transform.Find("Animator/Now/Num/Text1/Image_Win").gameObject;
            lowGo = transform.Find("Animator/Now/Num/Text1/Image_Lose").gameObject;
            highTagGo = transform.Find("Animator/Image_Successbg/Image_New").gameObject;

        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
        }

        protected override void OnOpen(object arg = null)
        {
            cmdBattleEndNtf = null == arg ? new CmdBattleEndNtf() : arg as CmdBattleEndNtf;
        }

        protected override void OnShow()
        {
            RefreshView();
        }

        private void RefreshView()
        {
            if(null != cmdBattleEndNtf)
            {
                var isWin = cmdBattleEndNtf.BattleResult == 1;
                winGO.SetActive(isWin);
                loseGo.SetActive(!isWin);
                var currentValue = cmdBattleEndNtf.VictoryArena.RankAfter;
                var preValue = cmdBattleEndNtf.VictoryArena.RankBefore;
                TextHelper.SetText(curRankText, currentValue.ToString());
                bool isNoChange = currentValue == preValue;
                highTagGo.SetActive(Sys_JSBattle.Instance.IsHigh);
                preRankText.transform.parent.gameObject.SetActive(!isNoChange);
                if(!isNoChange)
                {
                    var disfValue = Math.Abs((int)currentValue - (int)preValue);
                    TextHelper.SetText(preRankText, disfValue.ToString());
                    bool isUp = currentValue < preValue;
                    upGo.SetActive(isUp);
                    lowGo.SetActive(!isUp);
                }
                var count = cmdBattleEndNtf.Rewards.Items.Count;
                FrameworkTool.CreateChildList(rewardTrans, cmdBattleEndNtf.Rewards.Items.Count);
                for (int i = 0; i < count; i++)
                {
                    PropItem item = new PropItem();
                    item.BindGameObject(rewardTrans.GetChild(i).gameObject);
                    PropIconLoader.ShowItemData itemData = new PropIconLoader.ShowItemData(cmdBattleEndNtf.Rewards.Items[i].Infoid, cmdBattleEndNtf.Rewards.Items[i].Count, true, false, false, false, false, true, false, true);
                    item.SetData(itemData, EUIID.UI_JSBattle_Result);
                }
            }
            var highRankData = Sys_JSBattle.Instance.GetRoleVictoryArenaReward();
            if (null != highRankData)
            {
                TextHelper.SetText(highRankText, 2024718, highRankData.HighestRank.ToString());
            }
        }

        protected override void OnHide()
        {
        }

        protected override void OnDestroy()
        {
        }

        protected override void OnClose()
        {
            Sys_JSBattle.Instance.IsHigh = false;
        }

        public void CloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_JSBattle_Result);
            if(Sys_Team.Instance.HaveTeam || Sys_Map.Instance.CurMapId != 1400)
            {
                return;
            }
            UIManager.OpenUI(EUIID.UI_JSBattle);
        }

    }
}
