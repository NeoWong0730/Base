using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Table;
using System.Collections.Generic;

namespace Logic
{
    public class UI_Advance_Tips_Layout
    {
        public Transform transform;
        public Button advanceBtn;
        public Button closeBtn;
        public InputField input_word;
        public Text tipT;

        public const int count = 6;
        public Text[] activityTimes = new Text[count];

        public void Init(Transform transform)
        {
            this.transform = transform;
            advanceBtn = transform.Find("Animator/View01/Btn_01").GetComponent<Button>();
            closeBtn = transform.Find("Animator/View_TipsBg02_Small/Btn_Close").GetComponent<Button>();
            input_word = transform.Find("Animator/View01/Input_Word").GetComponent<InputField>();
            tipT = transform.Find("Animator/View01/Text_Tips01").GetComponent<Text>();
            for (int i = 0; i < 6; i++)
            {
                activityTimes[i] = transform.Find(string.Format("Animator/View01/Time/Image{0}/Num", i + 1)).GetComponent<Text>();
            }
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.OnCloseBtnClicked);
            advanceBtn.onClick.AddListener(listener.OnAdvanceBtnClicked);
        }

        public interface IListener
        {
            void OnCloseBtnClicked();
            void OnAdvanceBtnClicked();
        }
    }

    public class UI_Advance_Tips : UIBase, UI_Advance_Tips_Layout.IListener
    {
        private UI_Advance_Tips_Layout layout = new UI_Advance_Tips_Layout();
        private uint[] activityId;
        bool isShowAdvanceTip = false;

        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
        }

        protected override void OnShow()
        {
            int id = Sys_Role.Instance.Role.CareerRank >= 2 ? 1415 : 1414;
            var activityIdArray = CSVParam.Instance.GetConfData((uint)id).str_value.Split('|');
            activityId = new uint[activityIdArray.Length];
            for (int i = 0; i < activityIdArray.Length; i++)
            {
                uint.TryParse(activityIdArray[i], out activityId[i]);
            }
            ShowActivityNum();
            layout.tipT.text = LanguageHelper.GetTextContent(2005053, LanguageHelper.GetTextContent(2005054));
        }

        public void OnAdvanceBtnClicked()
        {
            if (!IsVerificationSuccess())
                return;
            if (!IsActivityNumComplete())
                return;
            OnSureAdvance();
        }

        private void OnSureAdvance()
        {
            CSVDialogue.Data cSVDialogueData = CSVDialogue.Instance.GetConfData(CSVPromoteCareer.Instance.GetConfData((uint)Sys_Advance.Instance.NextAdvanceRank()).advanceDia);
            if (null == cSVDialogueData)
                return;
            List<Sys_Dialogue.DialogueDataWrap> datas = Sys_Dialogue.GetDialogueDataWraps(cSVDialogueData);
            ResetDialogueDataEventData resetDialogueDataEventData = PoolManager.Fetch(typeof(ResetDialogueDataEventData)) as ResetDialogueDataEventData;
            resetDialogueDataEventData.Init(datas, () =>
            {
                Sys_Role.Instance.PromoteCareerRankReq(1);
                UIManager.CloseUI(EUIID.UI_Advance_Tips);
            }, cSVDialogueData);
            Sys_Dialogue.Instance.OpenDialogue(resetDialogueDataEventData);
        }

        private bool IsVerificationSuccess()
        {
            string msg = LanguageHelper.GetTextContent(2005054);
            if (string.IsNullOrEmpty(layout.input_word.text)||!string.Equals(layout.input_word.text, msg))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2005052));            
                return false;
            }
            return true;
        }

        private bool IsActivityNumComplete()
        {
            if (isShowAdvanceTip)
            {
                PromptBoxParameter.Instance.Clear();
                PromptBoxParameter.Instance.tipType = PromptBoxParameter.TipType.Text;
                PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(2005076);
                PromptBoxParameter.Instance.SetConfirm(true, OnSureAdvance, 2005078);
                PromptBoxParameter.Instance.SetCancel(true, null,2005077);
                UIManager.OpenUI(EUIID.UI_PromptBox, true, PromptBoxParameter.Instance);
                return false;
            }
            return true;
        }

        private void ShowActivityNum()
        {
            isShowAdvanceTip = false;
            if (activityId == null)
                return;
            for (int i = 0; i < UI_Advance_Tips_Layout.count; i++)
            {
                uint curTimes = Sys_Daily.Instance.getDailyCurTimes(activityId[i]);//当前次数
                uint totalTimes = Sys_Daily.Instance.getDailyTotalTimes(activityId[i]);
                if (curTimes > totalTimes)
                    curTimes = totalTimes;

                string numT = totalTimes == 0 ? LanguageHelper.GetTextContent(2010255) : (curTimes + "/" + totalTimes);
                string str= string.Format("<color=#784C66>{0}</color>", numT);
                if (curTimes < totalTimes)
                {
                    str = string.Format("<color=#FF0700>{0}</color>", numT);
                    if (!isShowAdvanceTip)
                        isShowAdvanceTip = true;
                }
                layout.activityTimes[i].text = str;
            }
        }

        public void OnCloseBtnClicked()
        {
            UIManager.CloseUI(EUIID.UI_Advance_Tips);
        }
    }
}