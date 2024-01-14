using Framework;
using Lib.Core;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;
using System;

namespace Logic
{
    public class UI_TrialGate_StageTip : UIBase
    {
        private Text tip01;
        private Text tip02;
        private Button closeBtn;
        private Timer timer;
        private uint stage;

        protected override void OnLoaded()
        {
            tip01 = transform.Find("Animator/Image/Text1").GetComponent<Text>();
            tip02 = transform.Find("Animator/Image/Text2").GetComponent<Text>();
            closeBtn = transform.Find("Animator/Image/Image_Close").GetComponent<Button>();
            closeBtn.onClick.AddListener(OnCloseBtnOnClicked);
        }

        private void OnCloseBtnOnClicked()
        {
            UIManager.CloseUI(EUIID.UI_TrialGate_StageTip);
        }

        protected override void OnOpen(object arg)
        {
            if (arg != null)
            {
                stage = Convert.ToUInt32(arg);
            }
        }

        protected override void OnShow()
        {
            SetInfo(stage);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Net_Combat.Instance.eventEmitter.Handle<uint>(Net_Combat.EEvents.OnChangeBattleStage, OnChangeBattleStage, toRegister);
        }

        protected override void OnHide()
        {
            timer?.Cancel();
        }

        protected override void OnDestroy()
        {
            timer?.Cancel();
        }

        private void OnChangeBattleStage(uint curStage)
        {
            SetInfo(curStage);
        }

        private void SetInfo(uint stage)
        {
            CSVBattleType.Data csvBattleTypeData = CSVBattleType.Instance.GetConfData(Sys_Fight.Instance.BattleTypeId);
            uint id = csvBattleTypeData.stageSwitch_id[(int)stage - 1];
            CSVSwitchCondition.Data csvSwitchConditionData = CSVSwitchCondition.Instance.GetConfData(id);
            if (csvSwitchConditionData != null)
            {
                TextHelper.SetText(tip01, csvSwitchConditionData.switchWay);
                TextHelper.SetText(tip02, csvSwitchConditionData.stageDescription);
                ForceRebuildLayout(tip01.transform.parent.gameObject);
            }
            timer?.Cancel();
            timer = Timer.Register(30f, () =>
            {
                UIManager.CloseUI(EUIID.UI_TrialGate_StageTip);
                timer.Cancel();
            }, null, false, true);
        }

        private void ForceRebuildLayout(GameObject go)
        {
            ContentSizeFitter[] fitter = go.GetComponentsInChildren<ContentSizeFitter>();
            for (int i = fitter.Length - 1; i >= 0; --i)
            {
                RectTransform trans = fitter[i].gameObject.GetComponent<RectTransform>();
                if (trans != null)
                    LayoutRebuilder.ForceRebuildLayoutImmediate(trans);
            }
        }
    }
}
