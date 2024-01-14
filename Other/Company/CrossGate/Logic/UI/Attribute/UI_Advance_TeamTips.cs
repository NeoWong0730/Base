using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Table;
using Packet;
using Lib.Core;
using System;

namespace Logic
{
    public class UI_Advance_TeamTips_Layout 
    {
        public Transform transform;
        public InputField inputField;
        public Button okBtn;
        public Button closeBtn;
        public GameObject itemGo;
        public Text tip;
        public const int count = 6;
        public Text[] activityTimes = new Text[count];
        public Text activityName01;
        public Text activityName02;

        public void Init(Transform transform)
        {
            this.transform = transform;
            for (int i = 0; i < 6; i++)
            {
                activityTimes[i] = transform.Find(string.Format("Animator/View01/Time/Image{0}/Num", i + 1)).GetComponent<Text>();
            }
            tip = transform.Find("Animator/View01/Text_Tips01").GetComponent<Text>();
            inputField = transform.Find("Animator/View01/Input_Word").GetComponent<InputField>();
            okBtn = transform.Find("Animator/View01/Btn_01").GetComponent<Button>();
            closeBtn = transform.Find("Animator/View_TipsBg02_Small/Btn_Close").GetComponent<Button>();
            itemGo = transform.Find("Animator/Scroll View01/Viewport/Content/Item").gameObject;
            activityName01 = transform.Find("Animator/View01/Time/Image1/Text").GetComponent<Text>();
            activityName02 = transform.Find("Animator/View01/Time/Image2/Text").GetComponent<Text>();
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.OnClose_ButtonClicked);
            okBtn.onClick.AddListener(listener.OnOK_ButtonClicked);
        }

        public interface IListener
        {
            void OnClose_ButtonClicked();
            void OnOK_ButtonClicked();
        }
    }

    public class UI_Advance_Team_Item : UIComponent
    {
        private Image headImage;
        private GameObject ok;
        private GameObject no;
        private GameObject wait;
        private GameObject leave;
        private GameObject leaveMoment;
        private Text name;
        private Text lv;
        private int index;
        public ulong roleId;

        public UI_Advance_Team_Item(int _index) : base()
        {
            index = _index;
        }
        
        protected override void Loaded()
        {
            headImage = transform.Find("Head").GetComponent<Image>();
            ok = transform.Find("Image_Ok").gameObject;
            no = transform.Find("Image_No").gameObject;
            wait = transform.Find("Image_Wait").gameObject;
            leave = transform.Find("Image_Leave").gameObject;
            leaveMoment = transform.Find("Image_LeaveMoment").gameObject;
            name = transform.Find("Text_Name").GetComponent<Text>();
            lv = transform.Find("Text_Lv/Text_Num").GetComponent<Text>();

        }

        public override void Show()
        {
            TeamMem teamMem = Sys_Team.Instance.getTeamMem(index);
            roleId = teamMem.MemId;
            CharacterHelper.SetHeadAndFrameData(headImage, teamMem.HeroId, teamMem.HeadId, teamMem.PhotoFrame);
            name.text = teamMem.Name.ToStringUtf8();
            lv.text = LanguageHelper.GetTextContent(12462, teamMem.Level.ToString());
            bool isOff = teamMem.IsOffLine();
            bool isLeave = teamMem.IsLeave();
            leave.SetActive(isOff);
            leaveMoment.SetActive(isLeave);
            if (!isOff && !isLeave)
            {
                wait.SetActive(true);
            }
            if (!Sys_Team.Instance.isCaptain(Sys_Role.Instance.RoleId)&& Sys_Team.Instance.isCaptain(roleId))
            {
                UpdateState(true);
            }
        }

        public override void Hide()
        {
            base.Hide();
        }

        public void UpdateState(bool isAgree)
        {
            ok.SetActive(isAgree);
            no.SetActive(!isAgree);
            wait.SetActive(false);
        }

    }

    public class UI_Advance_TeamTips : UIBase, UI_Advance_TeamTips_Layout.IListener
    {
        private UI_Advance_TeamTips_Layout layout = new UI_Advance_TeamTips_Layout();
        private uint[] activityId;
        bool isShowAdvanceTip = false;
        private List<UI_Advance_Team_Item> items = new List<UI_Advance_Team_Item>();
        private bool isAgreePromote = false;

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
           if( CSVDailyActivity.Instance.TryGetValue(activityId[0], out CSVDailyActivity.Data csvData01))
            {
                TextHelper.SetText(layout.activityName01, csvData01.ActiveName);
            }
            if (CSVDailyActivity.Instance.TryGetValue(activityId[1], out CSVDailyActivity.Data csvData02))
            {
                TextHelper.SetText(layout.activityName02, csvData02.ActiveName);
            }
            ShowActivityNum();
            ShowTeamInfo();
            layout.tip.text = LanguageHelper.GetTextContent(2005053, LanguageHelper.GetTextContent(2005054));
            layout.okBtn.enabled = !isAgreePromote;
            ImageHelper.SetImageGray(layout.okBtn.GetComponent<Image>(), isAgreePromote, true);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Advance.Instance.eventEmitter.Handle<uint,ulong>(Sys_Advance.EEvents.OnTeamPromoteCareerAgreeOrCancel, OnTeamPromoteCareerAgreeOrCancel, toRegister);
            Sys_Role.Instance.eventEmitter.Handle<uint>(Sys_Role.EEvents.OnUpdateCareerRank, OnUpdateCareerRank, toRegister);
            Sys_Team.Instance.eventEmitter.Handle<ulong>(Sys_Team.EEvents.NetMsg_MemState, OnTeamMemberState, toRegister);
            Sys_Team.Instance.eventEmitter.Handle<ulong>(Sys_Team.EEvents.NetMsg_MemLeaveNtf, OnMemberLeave, toRegister);
        }

        private void OnMemberLeave(ulong roleId)
        {
            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2005010));
            UIManager.CloseUI(EUIID.UI_Advance_TeamTips);
        }

        private void OnTeamMemberState(ulong roleId)
        {
            for (int i = 0; i < Sys_Team.Instance.TeamMemsCount; i++)              
            {
                TeamMem teamMem = Sys_Team.Instance.getTeamMem(i);
                if (teamMem.MemId == roleId && (teamMem.IsOffLine() || teamMem.IsLeave()))
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2005011));
                    UIManager.CloseUI(EUIID.UI_Advance_TeamTips);
                    break;
                }          
            }
        }

        private void OnUpdateCareerRank(uint rank)
        {
            UIManager.CloseUI(EUIID.UI_Advance_TeamTips);
        }

        private void OnTeamPromoteCareerAgreeOrCancel(uint isAgree, ulong roleId)
        {
            if (isAgree == 0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2005009));
                UIManager.CloseUI(EUIID.UI_Advance_TeamTips);
            }
            else
            {
                for (int i = 0; i < items.Count; ++i)
                {
                    if (items[i].roleId == roleId)
                    {
                        items[i].UpdateState(isAgree == 1);
                        break;
                    }
                }
                if (roleId == Sys_Role.Instance.RoleId)
                {
                    isAgreePromote = isAgree == 1;
                    layout.okBtn.enabled =! isAgreePromote;
                    ImageHelper.SetImageGray(layout.okBtn.GetComponent<Image>(), isAgreePromote, true);
                }
            }
        }

        private void ShowTeamInfo()
        {
            DefaultTeamInfo();
            FrameworkTool.CreateChildList(layout.itemGo.transform.parent, Sys_Team.Instance.TeamMemsCount);
            for(int i=0;i< Sys_Team.Instance.TeamMemsCount; ++i)
            {
                Transform trans = layout.itemGo.transform.parent.GetChild(i);
                UI_Advance_Team_Item item = new UI_Advance_Team_Item(i);
                item.Init(trans);
                if (i != 0)
                {
                    item.transform.name = i.ToString();
                }
                item.Show();
                items.Add(item);
            }
        }

        private void DefaultTeamInfo()
        {
            FrameworkTool.DestroyChildren(layout.itemGo.transform.parent.gameObject,layout.itemGo.transform.name);
            for(int i = 0; i < items.Count; ++i)
            {
                items[i].OnDestroy();
            }
            items.Clear();
        }

        private void ShowActivityNum()
        {
            isShowAdvanceTip = false;
            if (activityId == null)
                return;
            for (int i = 0; i < UI_Advance_TeamTips_Layout.count; i++)
            {
                uint curTimes = Sys_Daily.Instance.getDailyCurTimes(activityId[i]);//当前次数
                uint totalTimes = Sys_Daily.Instance.getDailyTotalTimes(activityId[i]);
                if (curTimes > totalTimes)
                    curTimes = totalTimes;

                string numT = totalTimes == 0 ? LanguageHelper.GetTextContent(2010255) : (curTimes + "/" + totalTimes);
                string str = string.Format("<color=#784C66>{0}</color>", numT);
                if (curTimes < totalTimes)
                {
                    str = string.Format("<color=#FF0700>{0}</color>", numT);
                    if (!isShowAdvanceTip)
                        isShowAdvanceTip = true;
                }
                layout.activityTimes[i].text = str;
            }
        }

        private void OnSureAdvance()
        {
            Sys_Role.Instance.PromoteCareerRankReq(1);
        }

        private bool IsVerificationSuccess()
        {
            string msg = LanguageHelper.GetTextContent(2005054);
            if (string.IsNullOrEmpty(layout.inputField.text) || !string.Equals(layout.inputField.text, msg))
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
                PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(2005082);
                PromptBoxParameter.Instance.SetConfirm(true, OnSureAdvance, 2005078);
                PromptBoxParameter.Instance.SetCancel(true, null, 2005077);
                UIManager.OpenUI(EUIID.UI_PromptBox, true, PromptBoxParameter.Instance);
                return false;
            }
            return true;
        }

        private bool isTeamMemIsOnLine()
        {
            int count = Sys_Team.Instance.TeamMemsCount;
            for (int i = 0; i < Sys_Team.Instance.TeamMemsCount; ++i)
            {
                TeamMem teamMem = Sys_Team.Instance.getTeamMem(i);
                if (teamMem.IsRob() || teamMem.IsLeave() || teamMem.IsOffLine())
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3899000003));
                    return false;
                }
            }
            return true;
        }


        public void OnClose_ButtonClicked()
        {
            if (!isAgreePromote)
            {
                Sys_Role.Instance.PromoteCareerRankReq(0);
            }
            UIManager.CloseUI(EUIID.UI_Advance_TeamTips);
        }

        public void OnOK_ButtonClicked()
        {
            if (!isTeamMemIsOnLine())
                return;
            if (!IsVerificationSuccess())
                return;
            if (!IsActivityNumComplete())
                return;
            OnSureAdvance();
        }
    }
}
