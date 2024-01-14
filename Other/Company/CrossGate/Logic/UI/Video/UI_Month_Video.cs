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
    public class UI_Video_Item 
    {
        public Transform transform;
        private Text name;
        private Text like_num;
        private Text bullet_num;
        private Text play_num;
        private Text rankNum;
        private Image rankImage;
        private Button button;
        private Transform playerParentTrans;
        private GameObject rankGo;
        private Action<UI_Video_Item> onClick;
        public ClientVideo clientVideo;
        private List<UI_Video_Player> playersList=new List<UI_Video_Player> ();

        public void BingGameObject(GameObject gameObject)
        {
            transform = gameObject.transform;
            name = transform.Find("Title/Text_Name").GetComponent<Text>();
            like_num = transform.Find("LikeNum/Text_Number").GetComponent<Text>();
            bullet_num = transform.Find("BulletChatNum/Text_Number").GetComponent<Text>();
            play_num = transform.Find("PlayNum/Text_Number").GetComponent<Text>();
            rankNum = transform.Find("Rank/Text_Number").GetComponent<Text>();
            rankImage = transform.Find("Rank/Image_Rank").GetComponent<Image>();
            playerParentTrans = transform.Find("Title/TeamMember").transform;
            rankGo = transform.Find("Rank").gameObject;
            button = transform.Find("Button").GetComponent<Button>();
            button.onClick.AddListener(OnClicked);
        }

        public void SetData(ClientVideo _clientVideo, int index=0)
        {
            clientVideo = _clientVideo;
            name.text = clientVideo.authorBrif.Title.ToStringUtf8();
            like_num.text = clientVideo.muntalBrif.Like.ToString();
            bullet_num.text = clientVideo.muntalBrif.Bullet.ToString();
            play_num.text = clientVideo.muntalBrif.Play.ToString();
            int rankNumber = index + 1;
            rankNum.text = rankNumber.ToString();
            rankImage.gameObject.SetActive(true);
            rankGo.SetActive(clientVideo.where == VideoWhere.Mbest);
            if (rankNumber == 1)
            {
                ImageHelper.SetIcon(rankImage, 993901);
            }
            else if (rankNumber == 2)
            {
                ImageHelper.SetIcon(rankImage, 993902);
            }
            else if (rankNumber == 3)
            {
                ImageHelper.SetIcon(rankImage, 993903);
            }
            else
            {
                rankImage.gameObject.SetActive(false);
            }
            FrameworkTool.DestroyChildren(playerParentTrans.gameObject, playerParentTrans.GetChild(0).name);
            for(int i = 0; i < playersList.Count; ++i)
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
                VideoPlayerInfo info = clientVideo.players.Players[i];
                bool isAuthor = clientVideo.authorBrif.Author == info.RoleId;
                UI_Video_Player item = new UI_Video_Player();
                item.BingGameObject( playerParentTrans.GetChild(i).gameObject);
                item.SetData(info, isAuthor);
                playersList.Add(item);
            }
        }

        public void AddClickListener(Action<UI_Video_Item> onclicked = null)
        {
            onClick = onclicked;
        }

        private void OnClicked( )
        {
            onClick?.Invoke(this);
        }
    }

    public class UI_Video_Type
    {
        private Transform transform;
        private Text typeText;
        private Text selectText;
        private CP_Toggle toggle;
        private uint type;
        private VideoButton buttonType;
       
        public void BingGameObject(GameObject gameObject)
        {
            transform = gameObject.transform;
            typeText = transform.Find("Text").GetComponent<Text>();
            selectText = transform.Find("Select/Text").GetComponent<Text>();
            toggle = transform.GetComponent<CP_Toggle>();
            toggle.onValueChanged.AddListener(OnToggleValueChanged);
        }

        public void SetData(uint _type, VideoButton _buttonType)
        {
            type = _type;
            buttonType = _buttonType;
            typeText.text = LanguageHelper.GetTextContent(CSVVideoType.Instance.GetConfData(type).Name);
            selectText.text = LanguageHelper.GetTextContent(CSVVideoType.Instance.GetConfData(type).Name);
        }

        private void OnToggleValueChanged(bool isOn)
        {
            if (isOn)
            {
                if (buttonType == VideoButton.MonthBest)
                {
                    Sys_Video.Instance.MonthBestListReq(Sys_Role.Instance.RoleId, type);
                }
                else
                {
                    Sys_Video.Instance.LastUploadListReq(Sys_Role.Instance.RoleId, type);
                }
                Sys_Video.Instance.eventEmitter.Trigger<uint>(Sys_Video.EEvents.OnSelectViewType,type);
            }
        }

        public void SetToggleOff()
        {
            toggle.SetSelected(false, true);
        }
    }

    public class UI_Video_Player
    {
        private Transform transform;
        private Text level;
        private Image icon;
        private GameObject authorTag;
        private Button button;
        private VideoPlayerInfo info;
        private bool isAuthor;

        public void BingGameObject(GameObject gameObject)
        {
            transform = gameObject.transform;
            level = transform.Find("Level/Text").GetComponent<Text>();
            icon= transform.Find("Icon").GetComponent<Image>();
            authorTag = transform.Find("Author").gameObject;
            button = transform.GetComponent<Button>();
            button.onClick.AddListener(OnRoleClicked);
        }

        private void OnRoleClicked()
        {
            if (info.RoleId==Sys_Role.Instance.Role.RoleId)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(6105));
                return;
            }
            CmdSocialGetBriefInfoAck ack = new CmdSocialGetBriefInfoAck();
            ack.HeroId = info.HeroId;
            ack.RoleId = info.RoleId;
            ack.Name = info.NickName;
            ack.Level = info.Level;
            ack.Occ = info.Career;
            ack.CareerRank = info.CareerRank;
            ack.RoleHead = info.Photo;
            ack.RoleHeadFrame = info.HeadFrame;
            CSVCharacter.Data characterData = CSVCharacter.Instance.GetConfData(info.HeroId);
            if (null != characterData)
            {
                ack.HeadIcon = characterData.headid;
            }
            Sys_Role_Info.InfoParmas infoParmas = new Sys_Role_Info.InfoParmas();
            infoParmas.Clear();
            infoParmas.eType = Sys_Role_Info.EType.Chat;
            infoParmas.mInfo = ack;
            UIManager.OpenUI(EUIID.UI_Team_Player, false, infoParmas);
        }

        public void SetData(VideoPlayerInfo _info,bool _isAuthor)
        {
            info = _info;
            isAuthor = _isAuthor;
            uint iconId = CharacterHelper.getHeadID((uint)info.HeroId, info.HeadFrame);
            ImageHelper.SetIcon(icon, iconId);
           level.text = info.Level.ToString();
           authorTag.gameObject.SetActive(isAuthor);
        }

        public void Destroy()
        {
            button.onClick.RemoveListener(OnRoleClicked);
            info = null; 
        } 

    }

    public class UI_Month_Video : UIComponent
    {
        private InputField nameInputField;
        private Button typeBtn;
        private Button searchBtn;
        private Text curType;
        private GameObject challengeGo;
        private GameObject pkGo;
        private Transform challengeParent;
        private Transform pkParent;
        private InfinityGrid infinityGrid;
        private InfinityGrid infinityChallengGrid;
        private InfinityGrid infinityPkGrid;
        private int infinityCount;
        private int infinityChallengeCount;
        private int infinityPkCount;
        private List<ClientVideo> videoList = new List<ClientVideo>();
        private List<UI_Video_Type> challengeTypeList = new List<UI_Video_Type>();
        private List<UI_Video_Type> pkTypeList = new List<UI_Video_Type>();


        #region 系统函数
        protected override void Loaded()
        {
            nameInputField = transform.Find("InputField").GetComponent<InputField>();
            challengeGo = transform.Find("List_01").gameObject;
            pkGo = transform.Find("List_02").gameObject;
            typeBtn = transform.Find("Btn_Screen").GetComponent<Button>();
            searchBtn = transform.Find("Btn_Search").GetComponent<Button>();
            typeBtn.onClick.AddListener(OnType_ButtonClicked);
            searchBtn.onClick.AddListener(OnSearch_ButtonClicked);
            curType = transform.Find("Btn_Screen/Text").GetComponent<Text>();
            challengeParent = transform.Find("List_01/Scroll View/Viewport/Content").transform;
            pkParent = transform.Find("List_02/Scroll View/Viewport/Content").transform;
            infinityGrid = transform.Find("Content/Scroll_View").GetComponent<InfinityGrid>();
            infinityGrid.onCreateCell += OnCreateCell;
            infinityGrid.onCellChange += OnCellChange;
            infinityChallengGrid = transform.Find("List_01/Scroll View").GetComponent<InfinityGrid>();
            infinityChallengGrid.onCreateCell += OnCreateChallengeTypeCell;
            infinityChallengGrid.onCellChange += OnCellChallengeTypeChange;
            infinityPkGrid = transform.Find("List_02/Scroll View").GetComponent<InfinityGrid>();
            infinityPkGrid.onCreateCell += OnCreatePkTypeCell;
            infinityPkGrid.onCellChange += OnCellPkTypeChange;
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            base.ProcessEventsForAwake(toRegister);
            Sys_Video.Instance.eventEmitter.Handle(Sys_Video.EEvents.OnOpenMonthBest, OpenMonthBest, toRegister);
            Sys_Video.Instance.eventEmitter.Handle<uint>(Sys_Video.EEvents.OnSelectViewType, SelectViewType, toRegister);
        }

        public override void Show()
        {
            base.Show();
            Sys_Video.Instance.MonthBestListReq(0, 0);
            challengeGo.SetActive(false);
            pkGo.SetActive(false);
            nameInputField.text = string.Empty;
            TextHelper.SetText(curType, 2023205);
        }

        public override void Hide()
        {
            base.Hide();
            infinityGrid.Clear();
            infinityChallengGrid.Clear();
            infinityPkGrid.Clear();
        }

        #endregion

        #region  事件回调
        private void OpenMonthBest()
        {
            challengeGo.SetActive(false);
            pkGo.SetActive(false);
            videoList.Clear();
            videoList.AddRange(Sys_Video.Instance.monthList);
            InitMonthBest();
        }

        private void SelectViewType(uint type)
        {
           if( CSVVideoType.Instance.TryGetValue(type, out Framework.Table.FCSVVideoType.Data data))
            {
                if (data.Type == 1)
                {
                    for (int i= 0; i < pkTypeList.Count; ++i)
                    {
                        pkTypeList[i].SetToggleOff();
                    }
                }
                else
                {
                    for (int i = 0; i < challengeTypeList.Count; ++i)
                    {
                        challengeTypeList[i].SetToggleOff();
                    }
                }
            }
            TextHelper.SetText(curType, data.Name);
        }

        #endregion

        #region Function
        private void InitMonthBest()
        {
            infinityCount = videoList.Count;
            infinityGrid.CellCount = infinityCount;
            infinityGrid.ForceRefreshActiveCell();
        }

        private void SetTypeView(bool isShow)
        {
            challengeTypeList.Clear();
            pkTypeList.Clear();
            if (isShow)
            {
                infinityChallengeCount = Sys_Video.Instance.challengeTypeList.Count;
                infinityPkCount = Sys_Video.Instance.pkTypeList.Count;
                infinityChallengGrid.CellCount = infinityChallengeCount;
                infinityChallengGrid.ForceRefreshActiveCell();
                infinityPkGrid.CellCount = infinityPkCount;
                infinityPkGrid.ForceRefreshActiveCell();
                challengeParent.localPosition = Vector3.zero;
                pkParent.localPosition = Vector3.zero;
            }
            else
            {
                infinityChallengGrid.Clear();
                infinityPkGrid.Clear();
            }
        }  

        private void OnCreateCell(InfinityGridCell cell)
        {
            UI_Video_Item entry = new UI_Video_Item();
            GameObject go = cell.mRootTransform.gameObject;
            entry.BingGameObject(go);
            entry.AddClickListener(OnItemSelect);
            cell.BindUserData(entry);
        }

        private void OnItemSelect(UI_Video_Item item)
        {
            UIManager.OpenUI(EUIID.UI_VideoDetails,false, item.clientVideo);
        }

        private void OnCellChange(InfinityGridCell cell, int index)
        {
            ClientVideo clientVideo = new ClientVideo();
            clientVideo = videoList[index];
            UI_Video_Item entry = cell.mUserData as UI_Video_Item;
            entry.SetData(clientVideo, index);
        }

        private void OnCreateChallengeTypeCell(InfinityGridCell cell)
        {
            UI_Video_Type entry = new UI_Video_Type();
            GameObject go = cell.mRootTransform.gameObject;
            entry.BingGameObject(go);
            cell.BindUserData(entry);
        }

        private void OnCellChallengeTypeChange(InfinityGridCell cell, int index)
        {
            uint Id = Sys_Video.Instance.challengeTypeList[index];
            UI_Video_Type entry = cell.mUserData as UI_Video_Type;
            entry.SetData(Id, VideoButton.MonthBest);
            challengeTypeList.Add(entry);
        }

        private void OnCreatePkTypeCell(InfinityGridCell cell)
        {
            UI_Video_Type entry = new UI_Video_Type();
            GameObject go = cell.mRootTransform.gameObject;
            entry.BingGameObject(go);
            cell.BindUserData(entry);
        }

        private void OnCellPkTypeChange(InfinityGridCell cell, int index)
        {
            uint Id = Sys_Video.Instance.pkTypeList[index];
            UI_Video_Type entry = cell.mUserData as UI_Video_Type;
            entry.SetData(Id, VideoButton.MonthBest);
            pkTypeList.Add(entry);
        }
        #endregion

        #region  按钮响应
        public void OnSearch_ButtonClicked()
        {
            ulong.TryParse(nameInputField.text,out ulong Id);
            videoList.Clear();
            videoList.AddRange(Sys_Video.Instance.SearchVideoListByAuthorId(Id, EVideoViewType.MonthVideoView));
            InitMonthBest();
        }

        private void OnType_ButtonClicked()
        {
            if (challengeGo.activeInHierarchy)
            {
                challengeGo.SetActive(false);
                pkGo.SetActive(false);
                SetTypeView(false);
            }
            else
            {
                challengeGo.SetActive(true);
                pkGo.SetActive(true);
                SetTypeView(true);
            }
        }
        #endregion
    }
}
