using Lib.Core;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Table;
using Logic.Core;

namespace Logic
{
    public partial class UI_Society_Layout
    {
        /// <summary>
        /// 系统提示///
        /// </summary>
        public class ChatTipItem
        {
            public GameObject root;

            public ChatTipItem(GameObject gameObject)
            {
                root = gameObject;
            }
        }

        /// <summary>
        /// 时间提示///
        /// </summary>
        public class ChatTimeItem
        {
            public GameObject root;

            public Text time;

            uint sendTime;

            StringBuilder stringBuilder = new StringBuilder();

            public ChatTimeItem(GameObject gameObject)
            {
                root = gameObject;

                time = root.GetComponentInChildren<Text>();
            }

            public void Update(uint _sendTime)
            {
                sendTime = _sendTime;
                var dateTime = Sys_Time.ConvertToLocalTime(sendTime);
                uint subTime = Sys_Time.Instance.GetServerTime() - _sendTime;
                if (subTime < 86400)
                {
                    time.text = stringBuilder.Append(dateTime.Hour.ToString("D2")).Append(":").Append(dateTime.Minute.ToString("D2")).ToString();
                }
                else if (subTime >= 86400 && subTime < 2 * 86400)
                {
                    stringBuilder.Append("昨天 ").Append(dateTime.Hour.ToString("D2")).Append(":").Append(dateTime.Minute.ToString("D2")).ToString();
                }
                else if (subTime >= 2 * 86400 && subTime < 3 * 86400)
                {
                    stringBuilder.Append("前天 ").Append(dateTime.Hour.ToString("D2")).Append(":").Append(dateTime.Minute.ToString("D2")).ToString();              
                }
                else if (subTime >= 3 * 86400 && subTime < 365 * 86400)
                {
                    time.text = stringBuilder.Append(dateTime.Month.ToString()).Append("月").Append(dateTime.Day.ToString()).Append("日").Append(dateTime.Hour.ToString("D2")).Append(":").Append(dateTime.Minute.ToString("D2")).ToString();
                }
                else
                {
                    time.text = stringBuilder.Append(dateTime.Year.ToString()).Append("年").Append(dateTime.Month.ToString()).Append("月").Append(dateTime.Day.ToString()).Append("日").Append(dateTime.Hour.ToString("D2")).Append(":").Append(dateTime.Minute.ToString("D2")).ToString();
                }
            }
        }

        public class MOLIButton
        {
            public GameObject gameObject;

            public Image image;
            public Text text;
            public Button button;

            uint id;
            CSVMoliSpirit.Data spiritData;

            public MOLIButton(GameObject _gameObject, uint _id)
            {
                gameObject = _gameObject;
                gameObject.SetActive(true);
                id = _id;

                image = _gameObject.FindChildByName("Image_Icon").GetComponent<Image>();
                text = _gameObject.FindChildByName("Text").GetComponent<Text>();
                button = _gameObject.FindChildByName("Button").GetComponent<Button>();
                button.onClick.AddListener(OnClickButton);

                spiritData = CSVMoliSpirit.Instance.GetConfData(id);   
                if (spiritData != null)
                {
                    ImageHelper.SetIcon(image, spiritData.image, true);
                    TextHelper.SetText(text, LanguageHelper.GetTextContent(spiritData.Language));
                }
            }

            void OnClickButton()
            {
                if (spiritData.urlType == 1)
                {
                    string url = spiritData.url;
                }
                else if (spiritData.urlType == 2)
                {
                    UIManager.OpenUI((EUIID)(uint.Parse(spiritData.url)));
                }
            }
        }

        public GameObject root;

        public Button closeButton;

        #region LeftSide

        GameObject leftRoot;
        public Button recentToggle;
        public GameObject recentToggle_Light;
        public GameObject recentToggle_Dark;
        public Button groupToggle;
        public GameObject groupToggle_Light;
        public GameObject groupToggle_Dark;
        public Button contactsToggle;
        public GameObject contactsToggle_Light;
        public GameObject contactsToggle_Dark;
        public Button mailToggle;
        public GameObject mailToggle_Light;
        public GameObject mailToggle_Dark;

        public GameObject buttonRoot;
        public Text friendLimitText;
        public Text mailLimitText;
        Button addButton;
        Button zoneButton;

        #endregion

        #region RightSide

        GameObject rightRoot;

        public GameObject chatTimePrefab;
        public GameObject chatSystemPrefab;
        public GameObject chatLeftPrefab;
        public GameObject chatRightPrefab;
        public GameObject chatNewFriendTipPrefab;

        public GameObject rightNoneRoot;
        public Text rightNoneText;
        public GameObject rightMOLIRoot;
        public GameObject rightMOLIButtonRoot;
        public GameObject rightMOLIButtonPrefab;
        public GameObject inputRoot;
        public InputField inputField;
        public Button emojiButton;
        public Button sendButton;

        public Text roleChatText;

        public GameObject viewTipsRoot;
        public Button viewTipsButton;

        #endregion

        public void Init(GameObject obj)
        {
            root = obj;

            closeButton = root.FindChildByName("Btn_Close").GetComponent<Button>();

            #region LeftSide

            leftRoot = root.FindChildByName("LeftRoot");

            recentToggle = leftRoot.FindChildByName("Toggle_Recent").GetComponent<Button>();
            recentToggle_Light = recentToggle.gameObject.FindChildByName("Image_Menu_Light");
            recentToggle_Dark = recentToggle.gameObject.FindChildByName("Btn_Menu_Dark");
            groupToggle = leftRoot.FindChildByName("Toggle_Group").GetComponent<Button>();
            groupToggle_Light = groupToggle.gameObject.FindChildByName("Image_Menu_Light");
            groupToggle_Dark = groupToggle.gameObject.FindChildByName("Btn_Menu_Dark");
            contactsToggle = leftRoot.FindChildByName("Toggle_Friend").GetComponent<Button>();
            contactsToggle_Light = contactsToggle.gameObject.FindChildByName("Image_Menu_Light");
            contactsToggle_Dark = contactsToggle.gameObject.FindChildByName("Btn_Menu_Dark");
            mailToggle = leftRoot.FindChildByName("Toggle_Mail").GetComponent<Button>();
            mailToggle_Light = mailToggle.gameObject.FindChildByName("Image_Menu_Light");
            mailToggle_Dark = mailToggle.gameObject.FindChildByName("Btn_Menu_Dark");

            buttonRoot = leftRoot.transform.Find("ButtonRoot").gameObject;
            addButton = leftRoot.transform.Find("ButtonRoot/Button_Add").GetComponent<Button>();
            zoneButton = leftRoot.transform.Find("ButtonRoot/Button_Zone").GetComponent<Button>();
            friendLimitText = leftRoot.transform.Find("ButtonRoot/Text_Limited").GetComponent<Text>();
            mailLimitText = leftRoot.transform.Find("MailRoot/Text_Limited").GetComponent<Text>();
            #region Mail

            mailRoot = leftRoot.transform.Find("MailRoot").gameObject;
            mailContentRoot = mailRoot.transform.Find("Viewport/Content").gameObject;
            #endregion

            #endregion

            #region RightSide

            rightRoot = root.FindChildByName("RightRoot");

            mailRightRoot = rightRoot.FindChildByName("MailRoot");
            chatTimePrefab = rightRoot.FindChildByName("chatTimePrefab");
            chatSystemPrefab = rightRoot.FindChildByName("chatSystemPrefab");
            chatLeftPrefab = rightRoot.FindChildByName("chatLeftPrefab");
            chatRightPrefab = rightRoot.FindChildByName("chatRightPrefab");
            chatNewFriendTipPrefab = rightRoot.FindChildByName("chatSystemPrefab2");

            rightNoneRoot = rightRoot.FindChildByName("RightNoneRoot");
            rightNoneText = rightNoneRoot.FindChildByName("Text").GetComponent<Text>();
            
            rightMOLIRoot = rightRoot.FindChildByName("MOLIRoot");
            rightMOLIButtonRoot = rightMOLIRoot.FindChildByName("Buttons");
            rightMOLIButtonPrefab = rightMOLIRoot.FindChildByName("Item");
            inputRoot = rightRoot.FindChildByName("InputRoot");
            inputField = inputRoot.FindChildByName("InputField").GetComponent<InputField>();
            emojiButton = inputRoot.FindChildByName("Btn_Emoji").GetComponent<Button>();
            sendButton = inputRoot.FindChildByName("Btn_Send").GetComponent<Button>();

            roleChatText = rightRoot.FindChildByName("_roleChatText").GetComponent<Text>();

            viewTipsRoot = root.FindChildByName("View_Detail_Tips");
            viewTipsButton = viewTipsRoot.GetComponentInChildren<Button>();

            #endregion

            RecentInit();
            GroupInit();
            ContactsInit();
        }

        public void RegisterEvents(IListener listener)
        {
            closeButton.onClick.AddListener(listener.OnClickCloseButton);
            recentToggle.onClick.AddListener(listener.OnClickRecentToggle);
            groupToggle.onClick.AddListener(listener.OnClickGroupToggle);
            contactsToggle.onClick.AddListener(listener.OnClickContactsToggle);
            mailToggle.onClick.AddListener(listener.OnClickMailToggle);

            addButton.onClick.AddListener(listener.OnClickAddButton);
            zoneButton.onClick.AddListener(listener.OnClickZoneButton);

            inputField.onValueChanged.AddListener(listener.OnInputFieldValueChanged);
            emojiButton.onClick.AddListener(listener.OnClickEmojiButton);
            sendButton.onClick.AddListener(listener.OnClickSendButton);

            viewTipsButton.onClick.AddListener(listener.OnClickViewTipsButton);

            RecentRegisterEvents(listener);
            ContactsRegisterEvents(listener);
            GroupRegisterEvents(listener);

            #region Mail

            #endregion
        }

        public interface IListener
        {
            void OnClickCloseButton();

            void OnClickRecentToggle();

            void OnClickGroupToggle();

            void OnClickContactsToggle();

            void OnClickMailToggle();

            void OnClickAddButton();

            void OnClickZoneButton();

            void OnClickEmojiButton();

            void OnClickSendButton();

            void OnClickRecentFriendDeleteButton();

            void OnClickRecentNoFriendDeleteButton();

            void OnClickRecentSendGiftButton();

            void OnClickRecentFriendTipButton();

            void OnClickRecentGetGiftButton();

            void OnClickContactsDeleteButton();

            void OnClickContactsNoFriendDeleteButton();

            void OnClickContactsSendGiftButton();

            void OnClickContactsGetGiftButton();

            void OnInputFieldValueChanged(string arg);

            void OnClickGroupSettingButton();

            void OnClickViewTipsButton();

            void OnClickContactsFriendTipButton();
        }
    }
}
