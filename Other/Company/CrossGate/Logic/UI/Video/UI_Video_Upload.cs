using Packet;
using UnityEngine;
using Logic.Core;
using UnityEngine.UI;
using Table;
using Lib.Core;
using System;
using System.Collections.Generic;

namespace Logic
{
    public class UI_Video_Upload_Layout
    {
        public Transform transform;
        public InputField titleInputField;
        public Text type;
        public Button okBtn;
        public Button cancelBtn;
        public Button closeBtn;
        public GameObject playerGo;
        public Transform playerParentTrans;
        public int maxLimit_Name = 24;

        public void Init(Transform transform)
        {
            this.transform = transform;
            titleInputField = transform.Find("Animator/InputField").GetComponent<InputField>();
            titleInputField.characterLimit = 0;
            titleInputField.onValidateInput = OnValidateInput_Name;
            type = transform.Find("Animator/Title_02/Text_Type").GetComponent<Text>();
            closeBtn = transform.Find("Animator/View_TipsBgNew04/Btn_Close").GetComponent<Button>();
            cancelBtn = transform.Find("Animator/Btn_Cancel").GetComponent<Button>();
            okBtn = transform.Find("Animator/Btn_Confirm").GetComponent<Button>();
            playerGo = transform.Find("Animator/TeamMember/Item").gameObject;
            playerParentTrans= transform.Find("Animator/TeamMember").transform;
        }

        char OnValidateInput_Name(string text, int charIndex, char addedChar)
        {
            if (TextHelper.GetCharNum(text) + TextHelper.GetCharNum(addedChar.ToString()) > maxLimit_Name)
            {
                return '\0';
            }
            return addedChar;
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.OnClose_ButtonClicked);
            cancelBtn.onClick.AddListener(listener.OnCancel_ButtonClicked);
            okBtn.onClick.AddListener(listener.OnOk_ButtonClicked);
        }

        public interface IListener
        {
            void OnClose_ButtonClicked();
            void OnCancel_ButtonClicked();
            void OnOk_ButtonClicked();
        }
    } 

    public class UI_Video_Upload : UIBase, UI_Video_Upload_Layout.IListener
    {

        private UI_Video_Upload_Layout layout = new UI_Video_Upload_Layout();
        private ClientVideo clientVideo=new ClientVideo ();
        private List<UI_Video_Player> playersList = new List<UI_Video_Player>();

        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
        }

        protected override void OnOpen(object arg)
        {
            if (arg == null)
            {
               //defaultType = 1;
            }
            else
            {
                clientVideo = arg as ClientVideo;
            }
        }

        protected override void ProcessEvents(bool toRegister)
        {
            Sys_Role.Instance.eventEmitter.Handle(Sys_Role.EEvents.OnUpdateCrossSrvState, OnUpdateCrossSrvState, toRegister);
        }

        private void OnUpdateCrossSrvState()
        {
            UIManager.CloseUI(EUIID.UI_Video_Upload);
        }

        protected override void OnShow()
        {
            SetData();
        }

        protected override void OnHide()
        {
   
        }

        private void SetData()
        {
            if (clientVideo.baseBrif != null)
            {
                CSVVideoType.Instance.TryGetValue(clientVideo.baseBrif.VideoType, out CSVVideoType.Data data);
                layout.type.text = LanguageHelper.GetTextContent(data.Name);
            }
            if (clientVideo.players != null&& clientVideo.players.Players!=null)
            {
                FrameworkTool.DestroyChildren(layout.playerParentTrans.gameObject, layout.playerParentTrans.GetChild(0).name);
                for (int i = 0; i < playersList.Count; ++i)
                {
                    playersList[i].Destroy();
                }
                playersList.Clear();
                FrameworkTool.CreateChildList(layout.playerParentTrans, clientVideo.players.Players.Count);
                for (int i = 0; i < clientVideo.players.Players.Count; ++i)
                {
                    if (i != 0)
                    {
                        layout.playerParentTrans.GetChild(i).name = clientVideo.players.Players[i].RoleId.ToString();
                    }
                    VideoPlayerInfo info = clientVideo.players.Players[i];
                    bool isAuthor = clientVideo.authorBrif.Author == info.RoleId;
                    UI_Video_Player item = new UI_Video_Player();
                    item.BingGameObject(layout.playerParentTrans.GetChild(i).gameObject);
                    item.SetData(info, isAuthor);
                    playersList.Add(item);
                }
            }
        }

        public void OnCancel_ButtonClicked()
        {
            UIManager.CloseUI(EUIID.UI_Video_Upload);
        }

        public void OnClose_ButtonClicked()
        {
            UIManager.CloseUI(EUIID.UI_Video_Upload);
        }

        public void OnOk_ButtonClicked()
        {
            if (TextHelper.GetCharNum(layout.titleInputField.text) > layout.maxLimit_Name|| TextHelper.GetCharNum(layout.titleInputField.text)==0)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(591000027));
                return;
            }
            if (Sys_Video.Instance.uploadList.Count >= 20)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(591000026));
                return;
            }
            Sys_Video.Instance.UploadVideoReq(Sys_Role.Instance.RoleId, layout.titleInputField.text, clientVideo.video);
            UIManager.CloseUI(EUIID.UI_Video_Upload);
        }
    }
}
