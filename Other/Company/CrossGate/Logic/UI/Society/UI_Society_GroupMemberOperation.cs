using Lib.Core;
using Logic.Core;
using Table;
using UnityEngine;
using UnityEngine.UI;

namespace Logic
{
    public class UI_Society_GroupMemberOperation_Layout
    {
        public GameObject root;

        public Button closeButton;
        public Button inviteButton;
        public Button exitButton;

        public GameObject icon1Root;
        public GameObject icon2Root;
        public GameObject icon3Root;
        public GameObject icon4Root;

        public Image icon1_1;
        public Image icon2_1;
        public Image icon2_2;
        public Image icon3_1;
        public Image icon3_2;
        public Image icon3_3;
        public Image icon4_1;
        public Image icon4_2;
        public Image icon4_3;
        public Image icon4_4;

        public Text groupName;
        public Text groupNum;

        public void Init(GameObject obj)
        {
            root = obj;

            closeButton = root.FindChildByName("close").GetComponent<Button>();
            inviteButton = root.FindChildByName("InviteButton").GetComponent<Button>();
            exitButton = root.FindChildByName("ExitButton").GetComponent<Button>();

            icon1Root = root.FindChildByName("View_Head01");
            icon1_1 = icon1Root.FindChildByName("Image_Icon1").GetComponent<Image>();

            icon2Root = root.FindChildByName("View_Head02");
            icon2_1 = icon2Root.FindChildByName("Image_Icon1").GetComponent<Image>();
            icon2_2 = icon2Root.FindChildByName("Image_Icon2").GetComponent<Image>();

            icon3Root = root.FindChildByName("View_Head03");
            icon3_1 = icon3Root.FindChildByName("Image_Icon1").GetComponent<Image>();
            icon3_2 = icon3Root.FindChildByName("Image_Icon2").GetComponent<Image>();
            icon3_3 = icon3Root.FindChildByName("Image_Icon3").GetComponent<Image>();

            icon4Root = root.FindChildByName("View_Head04");
            icon4_1 = icon4Root.FindChildByName("Image_Icon1").GetComponent<Image>();
            icon4_2 = icon4Root.FindChildByName("Image_Icon2").GetComponent<Image>();
            icon4_3 = icon4Root.FindChildByName("Image_Icon3").GetComponent<Image>();
            icon4_4 = icon4Root.FindChildByName("Image_Icon4").GetComponent<Image>();

            groupName = root.FindChildByName("Text_Name").GetComponent<Text>();
            groupNum = root.FindChildByName("Text_Num").GetComponent<Text>();
        }

        public void Update(Sys_Society.GroupInfo groupInfo)
        {
            if (groupInfo.heroIDs.Count == 1)
            {
                icon1Root.SetActive(true);
                icon2Root.SetActive(false);
                icon3Root.SetActive(false);
                icon4Root.SetActive(false);

                ImageHelper.SetIcon(icon1_1, CSVCharacter.Instance.GetConfData((uint)groupInfo.heroIDs[0].infoID).headid);
            }
            else if (groupInfo.heroIDs.Count == 2)
            {
                icon1Root.SetActive(false);
                icon2Root.SetActive(true);
                icon3Root.SetActive(false);
                icon4Root.SetActive(false);

                ImageHelper.SetIcon(icon2_1, CSVCharacter.Instance.GetConfData((uint)groupInfo.heroIDs[0].infoID).headid);
                ImageHelper.SetIcon(icon2_2, CSVCharacter.Instance.GetConfData((uint)groupInfo.heroIDs[1].infoID).headid);
            }
            else if (groupInfo.heroIDs.Count == 3)
            {
                icon1Root.SetActive(false);
                icon2Root.SetActive(false);
                icon3Root.SetActive(true);
                icon4Root.SetActive(false);

                ImageHelper.SetIcon(icon3_1, CSVCharacter.Instance.GetConfData((uint)groupInfo.heroIDs[0].infoID).headid);
                ImageHelper.SetIcon(icon3_2, CSVCharacter.Instance.GetConfData((uint)groupInfo.heroIDs[1].infoID).headid);
                ImageHelper.SetIcon(icon3_3, CSVCharacter.Instance.GetConfData((uint)groupInfo.heroIDs[2].infoID).headid);
            }
            else if (groupInfo.heroIDs.Count >= 4)
            {
                icon1Root.SetActive(false);
                icon2Root.SetActive(false);
                icon3Root.SetActive(false);
                icon4Root.SetActive(true);

                ImageHelper.SetIcon(icon4_1, CSVCharacter.Instance.GetConfData((uint)groupInfo.heroIDs[0].infoID).headid);
                ImageHelper.SetIcon(icon4_2, CSVCharacter.Instance.GetConfData((uint)groupInfo.heroIDs[1].infoID).headid);
                ImageHelper.SetIcon(icon4_3, CSVCharacter.Instance.GetConfData((uint)groupInfo.heroIDs[2].infoID).headid);
                ImageHelper.SetIcon(icon4_4, CSVCharacter.Instance.GetConfData((uint)groupInfo.heroIDs[3].infoID).headid);
            }

            TextHelper.SetText(groupName, groupInfo.name);
            TextHelper.SetText(groupNum, $"({groupInfo.count}/99)");
        }

        public void RegisterEvents(IListener listener)
        {
            closeButton.onClick.AddListener(listener.OnClickCloseButton);
            inviteButton.onClick.AddListener(listener.OnClickInviteButton);
            exitButton.onClick.AddListener(listener.OnClickExitButton);
        }

        public interface IListener
        {
            void OnClickCloseButton();

            void OnClickInviteButton();

            void OnClickExitButton();
        }
    }

    public class UI_Society_GroupMemberOperation : UIBase, UI_Society_GroupMemberOperation_Layout.IListener
    {
        private UI_Society_GroupMemberOperation_Layout layout = new UI_Society_GroupMemberOperation_Layout();

        public Sys_Society.GroupInfo groupInfo;

        protected override void OnLoaded()
        {
            layout.Init(gameObject);
            layout.RegisterEvents(this);
        }

        protected override void OnOpen(object arg)
        {
            groupInfo = arg as Sys_Society.GroupInfo;
        }

        protected override void OnShow()
        {
            layout.Update(groupInfo);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Society.Instance.eventEmitter.Handle<uint>(Sys_Society.EEvents.OnDismissGroup, OnDismissGroup, toRegister);
            Sys_Society.Instance.eventEmitter.Handle<uint>(Sys_Society.EEvents.OnSelfQuitGroupSuccess, OnSelfQuitGroupSuccess, toRegister);
        }

        void OnDismissGroup(uint groupID)
        {
            if (groupInfo.groupID == groupID)
            {
                CloseSelf();
            }
        }

        void OnSelfQuitGroupSuccess(uint groupID)
        {
            if (groupInfo.groupID == groupID)
            {
                CloseSelf();
            }
        }

        public void OnClickCloseButton()
        {
            CloseSelf();
        }

        public void OnClickInviteButton()
        {
            UIManager.OpenUI(EUIID.UI_Society_InviteFriends, false, groupInfo);
            CloseSelf();
        }

        public void OnClickExitButton()
        {
            if (groupInfo == null)
            {
                DebugUtil.LogError("groupInfo is null");
                return;
            }

            PromptBoxParameter.Instance.Clear();
            //TODO
            PromptBoxParameter.Instance.content = "确定要退出群组么";
            PromptBoxParameter.Instance.SetConfirm(true, () =>
            {
                Sys_Society.Instance.ReqQuitGroup(groupInfo.groupID);
            });
            PromptBoxParameter.Instance.SetCancel(true, null);
            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
        }
    }
}
