using Packet;
using UnityEngine;
using Logic.Core;
using System.Collections.Generic;
using UnityEngine.UI;
using Table;
using Lib.Core;
using System;

namespace Logic
{
    public class UI_Video_Message_Layout
    {
        public Transform transform;
        public Image image;
        public Text name;
        public Text type;
        public Text likeNum;
        public Text bullectNum;
        public Text playNum;
        public Text collectBtnText;
        public GameObject player;
        public GameObject menuGo;
        public Transform playerParentTrans;
        public Button collectBtn;
        public Button shareBtn;
        public Button playBtn;
        public Button closeBtn;

        public Button friendBtn;
        public Button worldBtn;
        public Button familyBtn;
        public Button teamBtn;

        public void Init(Transform transform)
        {
            this.transform = transform;
            name = transform.Find("Animator/Details/Text_Name").GetComponent<Text>();
            type = transform.Find("Animator/Details/Text_Classify/Text").GetComponent<Text>();
            likeNum = transform.Find("Animator/Details/LikeNum/Text_Number").GetComponent<Text>();
            bullectNum = transform.Find("Animator/Details/BulletChatNum/Text_Number").GetComponent<Text>();
            playNum = transform.Find("Animator/Details/PlayNum/Text_Number").GetComponent<Text>();
            collectBtnText = transform.Find("Animator/Details/Btn_Collect/Text_01").GetComponent<Text>();
            player = transform.Find("Animator/Details/TeamMember/Item").gameObject;
            menuGo = transform.Find("Animator/View_ButtonGroup").gameObject;
            playerParentTrans = transform.Find("Animator/Details/TeamMember").transform;
            playBtn = transform.Find("Animator/Details/Btn_Play").GetComponent<Button>();
            collectBtn = transform.Find("Animator/Details/Btn_Collect").GetComponent<Button>();
            shareBtn = transform.Find("Animator/Details/Btn_Share").GetComponent<Button>();
            closeBtn = transform.Find("Animator/View_TipsBgNew04/Btn_Close").GetComponent<Button>();

            friendBtn = transform.Find("Animator/View_ButtonGroup/bg/Btn_Friend").GetComponent<Button>();
            worldBtn = transform.Find("Animator/View_ButtonGroup/bg/Btn_World").GetComponent<Button>();
            familyBtn = transform.Find("Animator/View_ButtonGroup/bg/Btn_Family").GetComponent<Button>();
            teamBtn = transform.Find("Animator/View_ButtonGroup/bg/Btn_Team").GetComponent<Button>();
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.OnClose_ButtonClicked);
            playBtn.onClick.AddListener(listener.OnPlay_ButtonClicked);
            collectBtn.onClick.AddListener(listener.OnCollect_ButtonClicked);
            shareBtn.onClick.AddListener(listener.OnShare_ButtonClicked);
            friendBtn.onClick.AddListener(listener.OnFriend_ButtonClicked);
            worldBtn.onClick.AddListener(listener.OnWorld_ButtonClicked);
            familyBtn.onClick.AddListener(listener.OnFamily_ButtonClicked);
            teamBtn.onClick.AddListener(listener.OnTeam_ButtonClicked);
        }

        public interface IListener
        {
            void OnClose_ButtonClicked(); 
            void OnPlay_ButtonClicked();
            void OnCollect_ButtonClicked();
            void OnShare_ButtonClicked();
            void OnFriend_ButtonClicked();
            void OnWorld_ButtonClicked();
            void OnFamily_ButtonClicked();
            void OnTeam_ButtonClicked();
        }
    }

    public class UI_Video_Message : UIBase, UI_Video_Message_Layout.IListener
    {
        private UI_Video_Message_Layout layout = new UI_Video_Message_Layout();
        private ClientVideo clientVideo;
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
                return;
            }
            clientVideo = arg as ClientVideo;
        }

        protected override void OnShow()
        {
            if (clientVideo == null)
            {
                return;
            }
            SetData();
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Video.Instance.eventEmitter.Handle(Sys_Video.EEvents.OnUpdateCollectVideo, OnUpdateCollectVideo, toRegister);
        }

        protected override void ProcessEvents(bool toRegister)
        {
            Sys_Video.Instance.eventEmitter.Handle(Sys_Video.EEvents.OnPlayVideoSuccess, OnPlayVideoSuccess, toRegister);
            Sys_Role.Instance.eventEmitter.Handle(Sys_Role.EEvents.OnUpdateCrossSrvState, OnUpdateCrossSrvState, toRegister);
        }

        private void OnUpdateCrossSrvState()
        {
            UIManager.CloseUI(EUIID.UI_VideoDetails);
        }

        protected override void OnHide()
        {
        }

        private void OnPlayVideoSuccess()
        {
            UIManager.CloseUI(EUIID.UI_Video);
        }

        private void OnUpdateCollectVideo()
        {
            bool isCollect = Sys_Video.Instance.isCollected(clientVideo.video, clientVideo.authorBrif.Author);
            ImageHelper.SetImageGray(layout.collectBtn.GetComponent<Image>(), isCollect, true);
            layout.collectBtn.enabled = !isCollect;
            if (isCollect)
            {
                TextHelper.SetText(layout.collectBtnText, 2023247);
            }
            else
            {
                TextHelper.SetText(layout.collectBtnText, 2023225);
            }
        }

        public void SetData()
        {
            layout.menuGo.SetActive(false);
            layout.name.text = clientVideo.authorBrif.Title.ToStringUtf8();
            CSVVideoType.Instance.TryGetValue(clientVideo.baseBrif.VideoType, out CSVVideoType.Data data);
            layout.type.text = LanguageHelper.GetTextContent(data.Name);
            layout.likeNum.text = clientVideo.muntalBrif.Like.ToString();
            layout.bullectNum.text = clientVideo.muntalBrif.Bullet.ToString();
            layout.playNum.text = clientVideo.muntalBrif.Play.ToString();
            FrameworkTool.CreateChildList(layout.playerParentTrans, clientVideo.players.Players.Count);
            Sys_Society.Instance.socialFriendsInfo.GetAllFirendsInfos();

            if (clientVideo.players != null && clientVideo.players.Players != null)
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
            layout.friendBtn.gameObject.SetActive(Sys_Society.Instance.socialFriendsInfo.GetAllSortedFriendsInfos().Count != 0);   
            layout.teamBtn.gameObject.SetActive(Sys_Team.Instance.HaveTeam);
            layout.familyBtn.gameObject.SetActive(Sys_Family.Instance.familyData.isInFamily);
            bool isCollect = Sys_Video.Instance.isCollected(clientVideo.video, clientVideo.authorBrif.Author);
            ImageHelper.SetImageGray(layout.collectBtn.GetComponent<Image>(), isCollect, true);
            layout.collectBtn.enabled = !isCollect;
            if (isCollect)
            {
                TextHelper.SetText(layout.collectBtnText, 2023247);
            }
            else
            {
                TextHelper.SetText(layout.collectBtnText, 2023225);
            }
        }

        public void OnClose_ButtonClicked()
        {
            UIManager.CloseUI(EUIID.UI_VideoDetails);
        }

        public void OnCollect_ButtonClicked()
        {
            if (clientVideo == null)
            {
                return;
            }
            Sys_Video.Instance.CollectVideoReq(Sys_Role.Instance.RoleId, clientVideo.video, clientVideo.authorBrif.Author, clientVideo.where);
        }

        public void OnPlay_ButtonClicked()
        {
            if (GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Fight)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(591000008));
                return;
            }
            if (Sys_Role.Instance.isCrossSrv)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11898));
                return;
            }
            if (clientVideo != null)
            {
                Net_Combat.Instance.PlayVideoPreview(clientVideo.video, clientVideo);
            }
        }

        public void OnShare_ButtonClicked()
        {
            layout.menuGo.SetActive(!layout.menuGo.activeInHierarchy);
        }

        public void OnFriend_ButtonClicked()
        {
            UIManager.OpenUI(EUIID.UI_Video_Friend, false, clientVideo);
        }

        public void OnWorld_ButtonClicked()
        {
            ClientShareVideo clientShareVideo = new ClientShareVideo();
            clientShareVideo.Author = clientVideo.authorBrif.Author;
            clientShareVideo.Channel = ShareChannelType.ShareChannelWorld;  
            clientShareVideo.VideoId = clientVideo.video;
            clientShareVideo.VideoType = clientVideo.baseBrif.VideoType;
            clientShareVideo.Title = clientVideo.authorBrif.Title;
            Sys_Video.Instance.ShareChannelReq(Sys_Role.Instance.RoleId, clientShareVideo);
        }

        public void OnFamily_ButtonClicked() 
        {
            ClientShareVideo clientShareVideo = new ClientShareVideo();
            clientShareVideo.Author = clientVideo.authorBrif.Author;
            clientShareVideo.Channel = ShareChannelType.ShareChannelFamily;
            clientShareVideo.VideoId = clientVideo.video;
            clientShareVideo.VideoType = clientVideo.baseBrif.VideoType;
            clientShareVideo.Title = clientVideo.authorBrif.Title;
            Sys_Video.Instance.ShareChannelReq(Sys_Role.Instance.RoleId, clientShareVideo);
        }

        public void OnTeam_ButtonClicked()
        {
            ClientShareVideo clientShareVideo = new ClientShareVideo();
            clientShareVideo.Author = clientVideo.authorBrif.Author;
            clientShareVideo.Channel = ShareChannelType.ShareChannelTeam;
            clientShareVideo.VideoId = clientVideo.video;
            clientShareVideo.VideoType = clientVideo.baseBrif.VideoType;
            clientShareVideo.Title = clientVideo.authorBrif.Title;
            Sys_Video.Instance.ShareChannelReq(Sys_Role.Instance.RoleId, clientShareVideo);
        }
    }
}
