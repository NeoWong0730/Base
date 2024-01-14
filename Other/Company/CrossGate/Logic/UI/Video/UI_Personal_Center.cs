using Packet;
using UnityEngine;
using Logic.Core;
using System.Collections.Generic;
using UnityEngine.UI;
using Table;
using Lib.Core;
using System;
using Framework;

namespace Logic
{
    public class UI_Local_Item 
    {
        public  Transform transform;    
        private Text type;
        private Text round;
        private Text time;
        public CP_Toggle toggle;
        public ClientVideo clientVideo;
        private  Action<UI_Local_Item> onClick;

        public void BingGameObject(GameObject gameObject)
        {
            transform = gameObject.transform;          
            type = transform.Find("Text_Name").GetComponent<Text>();
            round = transform.Find("Text_Round").GetComponent<Text>();
            time = transform.Find("Text_Date").GetComponent<Text>();
            toggle = transform.GetComponent<CP_Toggle>();
            toggle.onValueChanged.AddListener( OnClicked);
        }

        public void AddClickListener(Action<UI_Local_Item> onclicked = null)
        {
            onClick = onclicked;
        }

        private void OnClicked(bool isOn)
        {
            if(isOn)
            onClick?.Invoke(this);
        }

        public void SetData(ClientVideo _clientVideo)
        {
            clientVideo = _clientVideo;
            if (clientVideo.baseBrif != null)
            {
                CSVVideoType.Instance.TryGetValue(clientVideo.baseBrif.VideoType, out CSVVideoType.Data data);
                type.text = LanguageHelper.GetTextContent(data.Name);
                round.text = LanguageHelper.GetTextContent(2023218, clientVideo.baseBrif.MaxRound.ToString());
                DateTime dt = TimeManager.GetDateTime(clientVideo.time);
                TextHelper.SetText(time, dt.Year.ToString() + "/" + dt.Month.ToString() + "/" + dt.Day.ToString());
            }
        }
    }

    public class UI_LocaI_Detail 
    {
        public Transform transform;   
        private Image image;
        private Text name;
        private Text type;
        private Text likeNum;
        private Text bullectNum;
        private Text playNum;
        private Text btnText;
        private Text btnDeleteText;

        private GameObject player;
        private Transform playerParentTrans;
        private Button uplodeBtn;
        private Button deleteBtn;
        private Button playBtn;

        private Button friendBtn;
        private Button worldBtn;
        private Button familyBtn;
        private Button teamBtn;
        private GameObject shareMenuGo;

        private ClientVideo clientVideo;
        private List<UI_Video_Player> playersList = new List<UI_Video_Player>();

        public void BingGameObject(GameObject gameObject)
        {
            transform = gameObject.transform;
            name = transform.Find("Text_Name").GetComponent<Text>();
            type = transform.Find("Text_Classify/Text").GetComponent<Text>();
            likeNum = transform.Find("LikeNum/Text_Number").GetComponent<Text>();
            bullectNum = transform.Find("BulletChatNum/Text_Number").GetComponent<Text>();
            playNum = transform.Find("PlayNum/Text_Number").GetComponent<Text>();
            btnText = transform.Find("Btn_Upload/Text_01").GetComponent<Text>();
            btnDeleteText = transform.Find("Btn_Delete/Text").GetComponent<Text>();
            player = transform.Find("TeamMember/Item").gameObject;
            playerParentTrans = transform.Find("TeamMember").transform;
            shareMenuGo = transform.Find("View_ButtonGroup").gameObject;
            uplodeBtn = transform.Find("Btn_Upload").GetComponent<Button>();
            deleteBtn = transform.Find("Btn_Delete").GetComponent<Button>();
            playBtn = transform.Find("Btn_Play").GetComponent<Button>();
            friendBtn = transform.Find("View_ButtonGroup/bg/Btn_Friend").GetComponent<Button>();
            worldBtn = transform.Find("View_ButtonGroup/bg/Btn_World").GetComponent<Button>();
            familyBtn = transform.Find("View_ButtonGroup/bg/Btn_Family").GetComponent<Button>();
            teamBtn = transform.Find("View_ButtonGroup/bg/Btn_Team").GetComponent<Button>();

            uplodeBtn.onClick.AddListener(OnUplodeBtnClicked);
            deleteBtn.onClick.AddListener(OnDeleteBtnClicked);
            playBtn.onClick.AddListener(OnPlayBtnClicked);
            worldBtn.onClick.AddListener(OnWorldBtnClicked);
            familyBtn.onClick.AddListener(OnFamilyBtnClicked);
            friendBtn.onClick.AddListener(OnFriendBtnClicked);
            teamBtn.onClick.AddListener(OnTeamBtnClicked);
        }

        public  void SetData(ClientVideo _clientVideo)
        {
            clientVideo = _clientVideo;
            uplodeBtn.gameObject.SetActive(false);
            shareMenuGo.SetActive(false);
            if (clientVideo.where == VideoWhere.Local)
            {
                DateTime dt = TimeManager.GetDateTime(clientVideo.time);
                TextHelper.SetText(name, 2023246, dt.Year.ToString() + "/" + dt.Month.ToString() + "/" + dt.Day.ToString());
                TextHelper.SetText(btnText, 2023221);
                TextHelper.SetText(btnDeleteText, 2023220);
                uplodeBtn.gameObject.SetActive(true);
            }
            else if (clientVideo.where == VideoWhere.Upload && clientVideo.authorBrif!=null)
            {
                name.text = clientVideo.authorBrif.Title.ToStringUtf8();
                TextHelper.SetText(btnText, 2023226);
                TextHelper.SetText(btnDeleteText, 2023220);
                uplodeBtn.gameObject.SetActive(true);
            }
            else if(clientVideo.authorBrif != null)
            {
                name.text = clientVideo.authorBrif.Title.ToStringUtf8();
                TextHelper.SetText(btnDeleteText, 591000025);
            }
            if (clientVideo.muntalBrif != null)
            {
                likeNum.text = clientVideo.muntalBrif.Like.ToString();
                bullectNum.text = clientVideo.muntalBrif.Bullet.ToString();
                playNum.text = clientVideo.muntalBrif.Play.ToString();
            }
            if (clientVideo.baseBrif!= null)
            {
                CSVVideoType.Instance.TryGetValue(clientVideo.baseBrif.VideoType, out CSVVideoType.Data data);
                type.text = LanguageHelper.GetTextContent(data.Name);
            }
            if (clientVideo.players != null && clientVideo.players.Players != null)
            {
                FrameworkTool.DestroyChildren(playerParentTrans.gameObject, playerParentTrans.GetChild(0).name);
                for (int i = 0; i < playersList.Count; ++i)
                {
                    playersList[i].Destroy();
                }
                playersList.Clear();
                FrameworkTool.CreateChildList(playerParentTrans, clientVideo.players.Players.Count);
                for (int i = 0; i < clientVideo.players.Players.Count; ++i)
                {
                    if (i != 0)
                    {
                        playerParentTrans.GetChild(i).name = clientVideo.players.Players[i].RoleId.ToString();
                    }
                    bool isAuthor = false;
                    VideoPlayerInfo info = clientVideo.players.Players[i];
                    if (clientVideo.authorBrif != null)
                    {
                        isAuthor = clientVideo.authorBrif.Author == info.RoleId;
                    }
                    UI_Video_Player item = new UI_Video_Player();
                    item.BingGameObject(playerParentTrans.GetChild(i).gameObject);
                    item.SetData(info, isAuthor);

                    playersList.Add(item);
                }
            }
        }

        private void OnPlayBtnClicked()
        {
            if (GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Fight)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(591000008));
                return;
            }
            if (clientVideo != null)
            {
                if (Sys_Role.Instance.isCrossSrv)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11898));
                    return;
                }
                Net_Combat.Instance.PlayVideoPreview(clientVideo.video, clientVideo);
            }
        }

        private void OnDeleteBtnClicked()
        {
            uint lanId = 0;
            if (clientVideo.where == VideoWhere.Collect)
            {
                lanId = 591000024;           
            }
            else
            {
                lanId = 591000019;
            }
            PromptBoxParameter.Instance.Clear();
            PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(lanId);
            PromptBoxParameter.Instance.SetConfirm(true, () =>
            {
                if (clientVideo.where == VideoWhere.Local)
                {
                    Sys_Video.Instance.DeleteLocalReq(Sys_Role.Instance.RoleId, clientVideo.video);
                }
                else if (clientVideo.where == VideoWhere.Upload)
                {
                    Sys_Video.Instance.DeleteUploadReq(Sys_Role.Instance.RoleId, clientVideo.video);
                }
                else if (clientVideo.where == VideoWhere.Collect)
                {
                    Sys_Video.Instance.CancelCollectReq(Sys_Role.Instance.RoleId, clientVideo.video,clientVideo.authorBrif.Author);
                }
            });
            PromptBoxParameter.Instance.SetCancel(true, null);
            UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
        }

        private void OnUplodeBtnClicked()
        {
            if (clientVideo.where == VideoWhere.Local)
            {
                UIManager.OpenUI(EUIID.UI_Video_Upload, false, clientVideo);
            }
            else if (clientVideo.where == VideoWhere.Upload)
            {
                shareMenuGo.SetActive(!shareMenuGo.activeInHierarchy);
                friendBtn.gameObject.SetActive(Sys_Society.Instance.socialFriendsInfo.GetAllSortedFriendsInfos().Count != 0);
                teamBtn.gameObject.SetActive(Sys_Team.Instance.HaveTeam);
                familyBtn.gameObject.SetActive(Sys_Family.Instance.familyData.isInFamily);
            }
        }

        public void OnFriendBtnClicked()
        {
            UIManager.OpenUI(EUIID.UI_Video_Friend, false, clientVideo);
        }

        public void OnWorldBtnClicked()
        {
            ClientShareVideo clientShareVideo = new ClientShareVideo();
            clientShareVideo.Author = clientVideo.authorBrif.Author;
            clientShareVideo.Channel = ShareChannelType.ShareChannelWorld;
            clientShareVideo.VideoId = clientVideo.video;
            clientShareVideo.VideoType = clientVideo.baseBrif.VideoType;
            clientShareVideo.Title = clientVideo.authorBrif.Title;
            Sys_Video.Instance.ShareChannelReq(Sys_Role.Instance.RoleId, clientShareVideo);
        }

        public void OnFamilyBtnClicked()
        {
            ClientShareVideo clientShareVideo = new ClientShareVideo();
            clientShareVideo.Author = clientVideo.authorBrif.Author;
            clientShareVideo.Channel = ShareChannelType.ShareChannelFamily;
            clientShareVideo.VideoId = clientVideo.video;
            clientShareVideo.VideoType = clientVideo.baseBrif.VideoType;
            clientShareVideo.Title = clientVideo.authorBrif.Title;
            Sys_Video.Instance.ShareChannelReq(Sys_Role.Instance.RoleId, clientShareVideo);
        }

        public void OnTeamBtnClicked()
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

    public class UI_Personal_Center :UIComponent
    {
        private Text level;
        private Text name;
        private Text server;
        private Text likeNum;
        private Text playNum;
        private Text videoNum;
        private Image headImage;
        private GameObject scrollView;
        private GameObject noItemGo;
        private GameObject detailGo;
        private GameObject noDetailGo;

        private CP_Toggle uploadToggle;
        private CP_Toggle unuplodeToggle;
        private CP_Toggle collectToggle;

        private InfinityGrid infinityGrid;
        private int infinityCount;
        private VideoButton curView;
        private VideoPersonCenter videoPersonCenter = new VideoPersonCenter();
        private UI_LocaI_Detail uI_LocaI_Detail = new UI_LocaI_Detail();
        private ClientVideo curClientVideo = new ClientVideo();
        private List<UI_Local_Item> items = new List<UI_Local_Item>();


        #region 系统函数
        protected override void Loaded()
        {
            level = transform.Find("Content/Info/Level/Text").GetComponent<Text>();
            name = transform.Find("Content/Info/Text_Name").GetComponent<Text>();
            server = transform.Find("Content/Info/Text_Server/Text").GetComponent<Text>();
            likeNum = transform.Find("Content/Info/LikeNum/Text").GetComponent<Text>();
            playNum = transform.Find("Content/Info/PlayNum/Text").GetComponent<Text>();
            videoNum = transform.Find("Content/Info/VideoNum/Text").GetComponent<Text>();
            headImage = transform.Find("Content/Info/Head").GetComponent<Image>();
            scrollView= transform.Find("Content/Scroll_View").gameObject;
            noItemGo = transform.Find("Content/Scroll_View/View_None").gameObject;
            detailGo = transform.Find("Content/Details").gameObject;
            noDetailGo = transform.Find("Content/View_None").gameObject;

            uploadToggle = transform.Find("Content/Menu/ListItem").GetComponent<CP_Toggle>();
            unuplodeToggle = transform.Find("Content/Menu/ListItem (1)").GetComponent<CP_Toggle>();
            collectToggle = transform.Find("Content/Menu/ListItem (2)").GetComponent<CP_Toggle>();
            uploadToggle.onValueChanged.AddListener(onUploadToggleValueChanged);
            unuplodeToggle.onValueChanged.AddListener(onUnuplodeToggleValueChanged);
            collectToggle.onValueChanged.AddListener(onCollectToggleValueChanged);

            infinityGrid = transform.Find("Content/Scroll_View/Viewport/Content").GetComponent<InfinityGrid>();
            infinityGrid.onCreateCell += OnCreateCell;
            infinityGrid.onCellChange += OnCellChange;

            uI_LocaI_Detail.BingGameObject(detailGo);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            base.ProcessEventsForAwake(toRegister);
            Sys_Video.Instance.eventEmitter.Handle(Sys_Video.EEvents.OnOpenPersonCenter, OpenPersonCenter, toRegister);
            Sys_Video.Instance.eventEmitter.Handle(Sys_Video.EEvents.OnUpdateCollectVideo, OnUpdateCollectVideo, toRegister);
            Sys_Video.Instance.eventEmitter.Handle(Sys_Video.EEvents.OnUpdateLocalVideo, OnUpdateLocalVideo, toRegister);
            Sys_Video.Instance.eventEmitter.Handle(Sys_Video.EEvents.OnUpdateUploadVideo, OnUpdateUploadVideo, toRegister);
            Sys_Video.Instance.eventEmitter.Handle<ClientVideo>(Sys_Video.EEvents.OnVideoBaseDetail, OnVideoBaseDetail, toRegister);
            Sys_Video.Instance.eventEmitter.Handle<ClientVideo>(Sys_Video.EEvents.OnVideoMutualDetail, OnVideoMutualDetail, toRegister);
            Sys_Video.Instance.eventEmitter.Handle(Sys_Video.EEvents.OnMineMutualInfo, OnMineMutualInfo, toRegister);
        }

        public override void Show()
        {
            base.Show();
            OpenPersonCenter();
            Sys_Video.Instance.MineMutualInfoReq(Sys_Role.Instance.Role.RoleId);
            InitRoleDate();
        }

        public override void Hide()
        {
            base.Hide();
            curClientVideo = null;
        }

        #endregion

        #region function
        private void OpenPersonCenter()
        {
            videoPersonCenter = Sys_Video.Instance.videoPersonCenter;
            InitNumDate();
            if (Sys_Video.Instance.curVideoType == VideoButton.Local)
            {
                unuplodeToggle.SetSelected(true, false);
                InitUnUplodeView();
            }
            else if (Sys_Video.Instance.curVideoType == VideoButton.Collect)
            {
                collectToggle.SetSelected(true, false);
                InitCollectView();
            }
            else if (Sys_Video.Instance.curVideoType == VideoButton.Upload)
            {
                uploadToggle.SetSelected(true, false);
                InitUplodeView();
            }
        }

        private void OnUpdateUploadVideo()
        {
            InitUplodeView();
            videoNum.text =Sys_Video.Instance.uploadList.Count.ToString();
        }

        private void OnVideoBaseDetail(ClientVideo clientVideo)
        {
            for(int i = 0; i < items.Count; ++i)
            {
                if (items[i].clientVideo.video == clientVideo.video&& items[i].clientVideo.authorBrif.Author == clientVideo.authorBrif.Author)
                {
                    items[i].SetData(clientVideo);
                    break;
                }
            }
            if(curClientVideo.video== clientVideo.video)
            uI_LocaI_Detail.SetData(clientVideo);
        }

        private void OnVideoMutualDetail(ClientVideo clientVideo)
        {
            ClientVideo video = new ClientVideo();
            if (Sys_Video.Instance.curVideoType == VideoButton.Local)
            {
                for (int j = 0; j < Sys_Video.Instance.unuploadList.Count; ++j)
                {
                    if (Sys_Video.Instance.unuploadList[j].video == clientVideo.video)
                    {
                        Sys_Video.Instance.unuploadList[j].muntalBrif = clientVideo.muntalBrif;
                        video = Sys_Video.Instance.unuploadList[j];
                        break;
                    }
                }
            }
            else if (Sys_Video.Instance.curVideoType == VideoButton.Upload)
            {
                for (int j = 0; j < Sys_Video.Instance.uploadList.Count; ++j)
                {
                    if (Sys_Video.Instance.uploadList[j].video == clientVideo.video)
                    {
                        Sys_Video.Instance.uploadList[j].muntalBrif = clientVideo.muntalBrif;
                        Sys_Video.Instance.uploadList[j].authorBrif = clientVideo.authorBrif;
                        video = Sys_Video.Instance.uploadList[j];
                        break;
                    }
                }
            }
            else if (Sys_Video.Instance.curVideoType == VideoButton.Collect)
            {
                for (int j = 0; j < Sys_Video.Instance.collectList.Count; ++j)
                {
                    if (Sys_Video.Instance.collectList[j].video == clientVideo.video && Sys_Video.Instance.collectList[j].authorBrif.Author == clientVideo.authorBrif.Author)
                    { 
                        Sys_Video.Instance.collectList[j].muntalBrif = clientVideo.muntalBrif;
                        Sys_Video.Instance.collectList[j].authorBrif = clientVideo.authorBrif;
                        video = Sys_Video.Instance.collectList[j];
                        break;
                    }
                }
            }
            uI_LocaI_Detail.SetData(video);
        }

        private void OnMineMutualInfo()
        {
            InitNumDate();
        }

        private void OnUpdateLocalVideo()
        {
            InitUnUplodeView();
            videoNum.text = Sys_Video.Instance.uploadList.Count.ToString();
        }

        private void OnUpdateCollectVideo()
        {
            RebuildCollect();
        }

        private void InitRoleDate()
        {
            Sys_Head.Instance.SetHeadAndFrameData(headImage);
            level.text = Sys_Role.Instance.Role.Level.ToString();
            name.text = Sys_Role.Instance.sRoleName.ToString();
            server.text= Sys_Login.Instance.mSelectedServer == null ? null : Sys_Login.Instance.mSelectedServer.mServerInfo.ServerName;
        }

        private void InitNumDate()
        {
            if (videoPersonCenter.Recv == null)
            {
                likeNum.text = 0.ToString();
                videoNum.text = 0.ToString();
                playNum.text = 0.ToString();
            }
            else
            {
                likeNum.text = videoPersonCenter.Recv.Like.ToString();
                videoNum.text = videoPersonCenter.Recv.Video.ToString();
                playNum.text = videoPersonCenter.Recv.Play.ToString();
            }  
        }

        private void InitUplodeView()
        {
            if (Sys_Video.Instance.uploadList.Count== 0)
            {
                noItemGo.SetActive(true);
                noDetailGo.SetActive(true);
                detailGo.SetActive(false);
                infinityCount =0;
                curClientVideo = null;
            }
            else
            {
                noItemGo.SetActive(false);
                noDetailGo.SetActive(false);
                detailGo.SetActive(true);
                infinityCount = Sys_Video.Instance.uploadList.Count;
                curClientVideo = Sys_Video.Instance.uploadList[0];
                GetVideoMutualDetail(curClientVideo);
            }
            infinityGrid.CellCount = infinityCount;
            infinityGrid.ForceRefreshActiveCell();
            if (Sys_Video.Instance.uploadList.Count != 0)
            {
                Sys_Video.Instance.VideoBaseDetailReq(VideoButton.Upload);
            }
        }

        private void InitUnUplodeView()
        {
            if (Sys_Video.Instance.unuploadList.Count == 0)
            {
                noItemGo.SetActive(true);
                noDetailGo.SetActive(true);
                detailGo.SetActive(false);
                curClientVideo = null;
                infinityCount = 0;
            }
            else
            {
                noItemGo.SetActive(false);
                noDetailGo.SetActive(false);
                detailGo.SetActive(true);
                infinityCount = Sys_Video.Instance.unuploadList.Count;
                curClientVideo = Sys_Video.Instance.unuploadList[0];
                GetVideoMutualDetail(curClientVideo);
            }
            infinityGrid.CellCount = infinityCount;
            infinityGrid.ForceRefreshActiveCell();
            if (Sys_Video.Instance.unuploadList.Count != 0)
            {
                Sys_Video.Instance.VideoBaseDetailReq(VideoButton.Local);
            }
        }

        private void InitCollectView()
        {
            RebuildCollect();
            if (Sys_Video.Instance.collectList.Count != 0)
            {
                Sys_Video.Instance.VideoBaseDetailReq(VideoButton.Collect);
            }
        }

        private void RebuildCollect()
        {
            if (Sys_Video.Instance.collectList.Count == 0)
            {
                noItemGo.SetActive(true);
                noDetailGo.SetActive(true);
                detailGo.SetActive(false);
                curClientVideo = null;
                infinityCount = 0;
            }
            else
            {
                noItemGo.SetActive(false);
                noDetailGo.SetActive(false);
                detailGo.SetActive(true);
                infinityCount = Sys_Video.Instance.collectList.Count;
                curClientVideo = Sys_Video.Instance.collectList[0];
                GetVideoMutualDetail(curClientVideo);
            }
            infinityGrid.CellCount = infinityCount;
            infinityGrid.ForceRefreshActiveCell();
        }

        private void OnCreateCell(InfinityGridCell cell)
        {
            items.Clear();
            UI_Local_Item entry = new UI_Local_Item();
            GameObject go = cell.mRootTransform.gameObject;
            entry.BingGameObject(go);
            entry.AddClickListener(OnItemSelect);
            cell.BindUserData(entry);
            items.Add(entry);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            ClientVideo clientVideo=new ClientVideo ();
            if (Sys_Video.Instance.curVideoType == VideoButton.Upload && Sys_Video.Instance.uploadList.Count> index)
            {
                clientVideo = Sys_Video.Instance.uploadList[index];
            }
            else if (Sys_Video.Instance.curVideoType == VideoButton.Local && Sys_Video.Instance.unuploadList.Count > index)
            {
                clientVideo = Sys_Video.Instance.unuploadList[index];
            }
            else if (Sys_Video.Instance.curVideoType == VideoButton.Collect && Sys_Video.Instance.collectList.Count > index)
            {
                clientVideo = Sys_Video.Instance.collectList[index];
            }
            UI_Local_Item entry = cell.mUserData as UI_Local_Item;
            entry.SetData(clientVideo);
            entry.toggle.SetSelected(index == 0, false);          
        }

        private void OnItemSelect(UI_Local_Item item)
        {
            GetVideoMutualDetail(item.clientVideo);
        }

        private void GetVideoMutualDetail(ClientVideo clientVideo)
        {
            VideoUniqueInfo info = new VideoUniqueInfo();
            if (Sys_Video.Instance.curVideoType == VideoButton.Upload)
            {
                info.AuthorId = Sys_Role.Instance.Role.RoleId;
                info.VideoId = clientVideo.video;
            }
            else if (Sys_Video.Instance.curVideoType == VideoButton.Local)
            {
                info.AuthorId = 0;
                info.VideoId = clientVideo.video;
            }
            else if (Sys_Video.Instance.curVideoType == VideoButton.Collect)
            {
                info.AuthorId = clientVideo.authorBrif.Author;
                info.VideoId = clientVideo.video;
            }
            Sys_Video.Instance.VideoMutualDetailReq(info);
        }

        #endregion

        #region 按钮Toggle  

        private void onCollectToggleValueChanged(bool isOn)
        {
            if (isOn)
            {
                Sys_Video.Instance.MineCollectListReq(Sys_Role.Instance.Role.RoleId);
                Sys_Video.Instance.curVideoType = VideoButton.Collect;
                InitCollectView();
            }
        }

        private void onUnuplodeToggleValueChanged(bool isOn)
        {
            if (isOn)
            {
                Sys_Video.Instance.MineLocalListReq(Sys_Role.Instance.Role.RoleId);
                Sys_Video.Instance.curVideoType = VideoButton.Local;
                InitUnUplodeView();
            }
        }

        private void onUploadToggleValueChanged(bool isOn)
        {
            if (isOn)
            {
                Sys_Video.Instance.MineUploadListReq(Sys_Role.Instance.Role.RoleId);
                Sys_Video.Instance.curVideoType = VideoButton.Upload;
                InitUplodeView();
            }
        }
        #endregion

    }
}
