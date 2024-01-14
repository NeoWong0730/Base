using Lib.Core;
using Logic.Core;
using UnityEngine;
using UnityEngine.UI;


namespace Logic
{
    public class UI_Society_RecommendFriend_Layout
    {
        public GameObject root;

        public Button closeButton;
        public Text reasonText;
        public Image roleIcon;
        public Text roleName;
        public Text roleLevel;
        public Button addFriendButton;

        public void Init(GameObject obj)
        {
            root = obj;

            closeButton = root.FindChildByName("Btn_Close").GetComponent<Button>();
            reasonText = root.FindChildByName("reasonText").GetComponent<Text>();
            roleIcon = root.FindChildByName("Image_Icon").GetComponent<Image>();
            roleName = root.FindChildByName("Text_Name").GetComponent<Text>();
            roleLevel = root.FindChildByName("Text_Level").GetComponent<Text>();
            addFriendButton = root.FindChildByName("Button_Add").GetComponent<Button>();
        }

        public void Update(Sys_Society.RoleInfo roleInfo)
        {
            //TODO: RoleIcon RessonText

            TextHelper.SetText(roleName, roleInfo.roleName);
            TextHelper.SetText(roleLevel, roleInfo.level);
        }

        public void RegisterEvents(IListener listener)
        {
            closeButton.onClick.AddListener(listener.OnClickCloseButton);
            addFriendButton.onClick.AddListener(listener.OnClickAddFriendButton);
        }

        public interface IListener
        {
            void OnClickCloseButton();

            void OnClickAddFriendButton();
        }
    }

    public class UI_Society_RecommendFriend : UIBase, UI_Society_RecommendFriend_Layout.IListener
    {
        private UI_Society_RecommendFriend_Layout layout = new UI_Society_RecommendFriend_Layout();

        private Sys_Society.RoleInfo roleInfo;

        protected override void OnLoaded()
        {
            layout.Init(gameObject);
            layout.RegisterEvents(this);
        }

        public void OnClickCloseButton()
        {
            UIManager.CloseUI(EUIID.UI_Society_RecommendFriend);
        }

        public void OnClickAddFriendButton()
        {
            Sys_Society.Instance.ReqAddFriend(roleInfo.roleID);
        }

        protected override void OnOpen(object arg)
        {            
            roleInfo = arg as Sys_Society.RoleInfo;
            if (roleInfo != null)
            {
                layout.Update(roleInfo);
            }
        }
    }
}
