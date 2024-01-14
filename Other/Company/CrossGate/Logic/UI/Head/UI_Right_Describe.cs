using UnityEngine;
using UnityEngine.UI;
using Framework;
using Table;
using Packet;
using Logic.Core;
using UnityEngine.EventSystems;
using Lib.Core;

namespace Logic
{
    public class UI_Right_Describe 
    {
        private Transform transform;

        private Image headImage;
        private Image headFrameImage;
        private Image chatFrameImage;
        private Image chatTextImage;
        private Text chatText;
        private Text commonName;
        private Text commonDes;

        private Image chatBg;
        private Text chatBgName;
        private Text chatBgDes;
        private Image teamLogo;
        private Text teamLogoName;
        private Text teamLogoDes;

        private Text limitGetWay;
        private Text limitUseWay;
        private Text limitTime;
        private Text foreverGetway;

        private Button limitGoBtn;
        private Button limitUseBtn;
        private Button foreverGoBtn;
        private Button foreverUseBtn;

        private GameObject limitWay;
        private GameObject foreverGo;
        private GameObject foreverUse;

        private GameObject limitGo;
        private GameObject limitUse;
        private GameObject limitUsingFalg;
        private GameObject foreverUsingFalg;

        private GameObject chatBgGo;
        private GameObject teamGo;
        private GameObject commonHeadGo;

        private RawImage rawImage;
        private Image eventImage;

        public AssetDependencies assetDependencies;
        private ShowSceneControl showSceneControl;
        private HeroLoader heroLoader;

        private ClientHeadData clientHeadData;
        private PictureFrameMap.Types.FraInfo info;
        private uint Id;
        private EHeadViewType type;
        private HeadItemGetWayEvt evt;
        public Timer timer;
        public bool showLimit;

        public void BingGameObject(GameObject go) 
        {
            transform = go.transform;
            headImage = transform.Find("View_Common/Image_Head").GetComponent<Image>();
            headFrameImage = transform.Find("View_Common/Image_Frame").GetComponent<Image>();
            chatFrameImage = transform.Find("View_Common/Image_ChatFrame").GetComponent<Image>();
            chatTextImage = transform.Find("View_Common/Image_ChatText").GetComponent<Image>();
            chatText = transform.Find("View_Common/Image_ChatText/Text_Chat").GetComponent<Text>();
            commonName = transform.Find("View_Common/Image_Bottom/Text_Name").GetComponent<Text>();
            commonDes = transform.Find("View_Common/Image_Bottom/Text_Des").GetComponent<Text>();

            chatBg = transform.Find("View_BG").GetComponent<Image>();
            chatBgName = transform.Find("View_BG/Image_Bottom/Text_Name").GetComponent<Text>();
            chatBgDes = transform.Find("View_BG/Image_Bottom/Text_Des").GetComponent<Text>();
            teamLogo = transform.Find("View_Team/Image_Label").GetComponent<Image>();
            teamLogoName = transform.Find("View_Team/Image_Bottom/Text_Name").GetComponent<Text>();
            teamLogoDes = transform.Find("View_Team/Image_Bottom/Text_Des").GetComponent<Text>();

            limitGetWay = transform.Find("Btn_Limit/Image_Limit_Get/Text_Des").GetComponent<Text>();
            limitUseWay = transform.Find("Btn_Limit/Image_Limit_Use/Text_Des").GetComponent<Text>();
            limitTime = transform.Find("Btn_Limit/Image_Limit_Use/Text_Time").GetComponent<Text>();
            foreverGetway = transform.Find("Btn_Forever_Go/Text_Des").GetComponent<Text>();

            limitGoBtn = transform.Find("Btn_Limit/Image_Limit_Get/Btn_01_Small").GetComponent<Button>();
            limitGoBtn.onClick.AddListener(OnlimitGoBtnClicked);
            limitUseBtn = transform.Find("Btn_Limit/Image_Limit_Use/Btn_02_Small").GetComponent<Button>();
            limitUseBtn.onClick.AddListener(OnlimitUseBtnClicked);
            foreverGoBtn = transform.Find("Btn_Forever_Go/Btn_01_Small").GetComponent<Button>();
            foreverGoBtn.onClick.AddListener(OnforeverGoBtnClicked);
            foreverUseBtn = transform.Find("Btn_Forever_Use/Btn_Use").GetComponent<Button>();
            foreverUseBtn.onClick.AddListener(OnforeverUseBtnClicked);

            limitWay = transform.Find("Btn_Limit").gameObject;
            foreverGo = transform.Find("Btn_Forever_Go").gameObject;
            foreverUse = transform.Find("Btn_Forever_Use").gameObject;
            limitGo = transform.Find("Btn_Limit/Image_Limit_Get").gameObject;
            limitUse  =transform.Find("Btn_Limit/Image_Limit_Use").gameObject;
            limitUsingFalg = transform.Find("Btn_Limit/Image_Limit_Use/Text_Useing (1)").gameObject;
            foreverUsingFalg = transform.Find("Btn_Forever_Use/Text_Useing").gameObject;
            teamGo = transform.Find("View_Team").gameObject;
            commonHeadGo = transform.Find("View_Common").gameObject;
            chatBgGo = transform.Find("View_BG").gameObject;

            rawImage = transform.Find("View_Team/RawImage").GetComponent<RawImage>();
            eventImage = transform.Find("View_Team/EventImage").GetComponent<Image>();
            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(eventImage);
            eventListener.AddEventListener(EventTriggerType.Drag, OnDrag);

            clientHeadData = Sys_Head.Instance.clientHead;
        }

        public void SetData(uint _id, EHeadViewType _type)
        {
            Id = _id;
            type = _type;
            evt = new HeadItemGetWayEvt();
            evt = Sys_Head.Instance.GetItemWay(Id, type);
            info = Sys_Head.Instance.GetActivePictureFrameMap(Id, type);
            chatBgGo.SetActive(false);
            commonHeadGo.SetActive(false);
            teamGo.SetActive(false);
            headImage.gameObject.SetActive(false);
            headFrameImage.gameObject.SetActive(false);
            chatTextImage.gameObject.SetActive(false);
            chatFrameImage.gameObject.SetActive(false);
            switch (type)
            {
                case EHeadViewType.HeadView:
                    commonHeadGo.SetActive(true);
                    headImage.gameObject.SetActive(true);
                    CSVHead.Data csvHeadData = CSVHead.Instance.GetConfData(Id);
                    if (csvHeadData != null)
                    {
                        if (csvHeadData.HeadIcon[0] == 0)
                        {
                            CSVCharacter.Data heroData = CSVCharacter.Instance.GetConfData(Sys_Role.Instance.Role.HeroId);
                            if (heroData != null)
                            {
                                ImageHelper.SetIcon(headImage, heroData.headid);
                            }
                        }
                        else
                        {
                            uint headIconId = Sys_Head.Instance.GetHeadIconIdByRoleType(csvHeadData.HeadIcon);
                            ImageHelper.SetIcon(headImage, headIconId);
                        }
                        commonName.text = LanguageHelper.GetTextContent(csvHeadData.HeadName);
                        commonDes.text = LanguageHelper.GetTextContent(csvHeadData.HeadDescribe);
                        SetBtnShow(csvHeadData.LimitedTime != 0, csvHeadData.LimitedTime != 1, csvHeadData.Lock);
                        showLimit = csvHeadData.LimitedTime != 0;
                    }
                    break;
                case EHeadViewType.HeadFrameView:
                    commonHeadGo.SetActive(true);
                    headFrameImage.gameObject.SetActive(true);
                    CSVHeadframe.Data csvHeadframeData = CSVHeadframe.Instance.GetConfData(Id);
                    if (csvHeadframeData != null)
                    {
                        if (csvHeadframeData.HeadframeIcon == 0)
                        {
                            ImageHelper.SetIcon(headFrameImage, 3002100);
                        }
                        else
                        {
                            ImageHelper.SetIcon(headFrameImage, csvHeadframeData.HeadframeIcon);
                        }
                        commonName.text = LanguageHelper.GetTextContent(csvHeadframeData.HeadframeName);
                        commonDes.text = LanguageHelper.GetTextContent(csvHeadframeData.HeadframeDescribe);
                        SetBtnShow( csvHeadframeData.LimitedTime != 0, csvHeadframeData.LimitedTime != 1, csvHeadframeData.Lock);
                        showLimit = csvHeadframeData.LimitedTime != 0;
                    }
                    break;
                case EHeadViewType.ChatFrameView:
                    commonHeadGo.SetActive(true);
                    chatFrameImage.gameObject.SetActive(true);
                    chatTextImage.gameObject.SetActive(true);
                    chatTextImage.enabled = false;
                    CSVChatframe.Data csvChatframe = CSVChatframe.Instance.GetConfData(Id);
                    if (csvChatframe != null)
                    {
                        CSVWordStyle.Data worldData = CSVWordStyle.Instance.GetConfData(csvChatframe.Word);
                        if (worldData != null)
                        {
                            TextHelper.SetText(chatText, LanguageHelper.GetTextContent(csvChatframe.Text), worldData);
                        }
                        ImageHelper.SetIcon(chatFrameImage, csvChatframe.ChatIcon);   
                        commonName.text = LanguageHelper.GetTextContent(csvChatframe.ChatName);
                        commonDes.text = LanguageHelper.GetTextContent(csvChatframe.ChatDescribe);
                        SetBtnShow(csvChatframe.LimitedTime != 0, csvChatframe.LimitedTime != 1, csvChatframe.Lock);
                        showLimit = csvChatframe.LimitedTime != 0;
                    }
                    break;
                case EHeadViewType.ChatBackgraoudView:  
                    chatBgGo.SetActive(true);
                    CSVChatBack.Data csvChatBackData = CSVChatBack.Instance.GetConfData(Id);
                    if (csvChatBackData != null)
                    {
                        if (csvChatBackData.BackIcon == "")
                        {
                            ImageHelper.SetIcon(chatBg, 3002100);
                        }
                        else
                        {
                            ImageHelper.SetIcon(chatBg, csvChatBackData.BackIcon);
                        }
                        chatBgName.text = LanguageHelper.GetTextContent(csvChatBackData.BackName);
                        chatBgDes.text = LanguageHelper.GetTextContent(csvChatBackData.BackDescribe);
                        SetBtnShow(csvChatBackData.LimitedTime != 0, csvChatBackData.LimitedTime != 1, csvChatBackData.Lock);
                        showLimit = csvChatBackData.LimitedTime != 0;
                    }
                    break;
                case EHeadViewType.ChatTextView:
                    commonHeadGo.SetActive(true);
                    chatTextImage.gameObject.SetActive(true);
                    chatTextImage.enabled = true;
                    CSVChatWord.Data csvChatWordData = CSVChatWord.Instance.GetConfData(Id);
                    if (csvChatWordData != null)
                    { 
                        ImageHelper.SetIcon(chatTextImage, CSVChatframe.Instance.GetConfData(300).ChatIcon);
                        CSVWordStyle.Data worldData = CSVWordStyle.Instance.GetConfData(csvChatWordData.WordIcon);
                        if (worldData != null)
                        {
                            TextHelper.SetText(chatText, LanguageHelper.GetTextContent(csvChatWordData.Word), worldData);
                        }
                        commonName.text = LanguageHelper.GetTextContent(csvChatWordData.WordName);
                        commonDes.text = LanguageHelper.GetTextContent(csvChatWordData.WordDescribe);
                        SetBtnShow(csvChatWordData.LimitedTime != 0, csvChatWordData.LimitedTime != 1, csvChatWordData.Lock);
                        showLimit = csvChatWordData.LimitedTime != 0;
                    }
                    break;
                case EHeadViewType.TeamFalgView:
                    teamGo.SetActive(true);
                    CSVTeamLogo.Data csvTeamLogoData = CSVTeamLogo.Instance.GetConfData(Id);
                    if (csvTeamLogoData != null)
                    {
                        if (csvTeamLogoData.TeamIcon == 0)
                        {
                            teamLogo.gameObject.SetActive(false);
                        }
                        else
                        {
                            teamLogo.gameObject.SetActive(true);
                            ImageHelper.SetIcon(teamLogo, csvTeamLogoData.TeamIcon);
                        }
                        teamLogoName.text = LanguageHelper.GetTextContent(csvTeamLogoData.TeamName);
                        teamLogoDes.text = LanguageHelper.GetTextContent(csvTeamLogoData.TeamDescribe);
                        SetBtnShow(csvTeamLogoData.LimitedTime != 0, csvTeamLogoData.LimitedTime != 1, csvTeamLogoData.Lock);
                        showLimit = csvTeamLogoData.LimitedTime != 0;
                    }
                    break;
            }
        }

        private void SetBtnShow(bool showLimit, bool showForever, uint needlock)   //needlock   0需要解锁  1不需要解锁
        {
            if (info == null && needlock==0)  //未激活
            {
                limitUse.SetActive(false);
                foreverUse.SetActive(false);
                limitGo.SetActive(true);
                limitUsingFalg.SetActive(false);
                limitUseBtn.gameObject.SetActive(false);
                limitGoBtn.gameObject.SetActive(evt.limitGetWay == 1);
                foreverGoBtn.gameObject.SetActive(evt.foreverGetWay == 1);
                limitWay.SetActive(showLimit);
                foreverGo.SetActive(showForever);
                if(evt.limitGetWay == 1)
                {
                    limitGetWay.text = LanguageHelper.GetTextContent(2009569);
                }else if(evt.limitGetWay == 2)
                {
                    limitGetWay.text = LanguageHelper.GetTextContent(evt.limitGetParam[1]); ;
                }
                else if (evt.limitGetWay == 3)
                {
                    limitGetWay.text = LanguageHelper.GetTextContent(evt.limitGetParam[1]); ;
                }
                if (evt.foreverGetWay == 1)
                {
                    foreverGetway.text = LanguageHelper.GetTextContent(2009569);
                }
                else if (evt.foreverGetWay == 2)
                {
                    foreverGetway.text = LanguageHelper.GetTextContent(evt.foreverGetParam[1]); ;
                }
                else if (evt.foreverGetWay == 3)
                {
                    foreverGetway.text = LanguageHelper.GetTextContent(evt.foreverGetParam[1]); ;
                }
            }
            else  //已激活
            {
                limitGo.SetActive(false);
                bool isUsing = clientHeadData.isUsing(Id, (uint)type);
                foreverUsingFalg.SetActive(isUsing);
                foreverUseBtn.gameObject.SetActive(!isUsing);
                limitUsingFalg.SetActive(isUsing);
                limitUseBtn.gameObject.SetActive(!isUsing);

                if (needlock == 1)  //不需要解锁道具
                {
                    foreverGo.SetActive(false);
                    limitUse.SetActive(false);
                    foreverUse.SetActive(true);
                    limitWay.SetActive(false);
                }
                else    
                {
                    uint end = info.EndTick;
                    uint nowtime = Sys_Time.Instance.GetServerTime();
                    if (end == 0)  //永久道具  
                    {
                        limitWay.SetActive(false);
                        foreverGo.SetActive(false);
                        foreverUse.SetActive(true);
                    }
                    else          //限时道具           
                    {
                        limitWay.SetActive(true);
                        limitUse.SetActive(true);
                        foreverUse.SetActive(false);
                        foreverGo.SetActive(true);
                        foreverGoBtn.gameObject.SetActive(false);
                        if (evt.foreverGetWay == 1)
                        {
                            foreverGoBtn.gameObject.SetActive(true);
                            foreverGetway.text = LanguageHelper.GetTextContent(2009569); 
                        }
                        else if (evt.foreverGetWay == 2)
                        {
                            foreverGetway.text = LanguageHelper.GetTextContent(evt.foreverGetParam[1]); ;
                        }
                        else if (evt.foreverGetWay == 3)
                        {
                            foreverGetway.text = LanguageHelper.GetTextContent(evt.foreverGetParam[1]); ;
                        }
                        uint leftTime = end - nowtime;
                        timer?.Cancel();
                        timer = Timer.Register(leftTime, () =>
                        {
                            timer.Cancel();
                        },
                        (time) =>
                        {
                            if (limitTime != null&& leftTime>= time)
                                limitTime.text = LanguageHelper.TimeToString((uint)(leftTime - time), LanguageHelper.TimeFormat.Type_4);
                        },
                        false, false);

                    }

                }
            }
        }

        public void UseHeadFrameUpdate()
        {
            if (clientHeadData.isUsing(Id, (uint)type))
            {
                foreverUseBtn.gameObject.SetActive(false);
                limitUseBtn.gameObject.SetActive(false);
                limitGo.SetActive(false);
                foreverGo.SetActive(showLimit);

                if (info == null|| info.EndTick==0)
                {
                    foreverUse.SetActive(true);
                    foreverUsingFalg.SetActive(true);
                    foreverGo.SetActive(false);
                }  
                else
                {
                    foreverUse.SetActive(false);
                    limitUse.SetActive(true);
                    limitUsingFalg.SetActive(true);
                }
            }
        }

        public void ExpritedUpdate()
        {
            limitGo.SetActive(true);
            limitUse.SetActive(false);
            limitUsingFalg.SetActive(false);
            limitUseBtn.gameObject.SetActive(true);
        }

        public void ActiveUpdate()
        {
            SetData(Id,type);
        }

        #region ModelShow
        public void OnCreateModel()
        {
            _LoadShowScene();
            _LoadShowModel((uint)GameCenter.mainHero.careerComponent.CurCarrerType);
            heroLoader.LoadWeaponPart(Sys_Fashion.Instance.GetCurDressedFashionWeapon(), Sys_Equip.Instance.GetCurWeapon());
        }

        private void _LoadShowScene()
        {
            if (showSceneControl == null)
            {
                showSceneControl = new ShowSceneControl();
            }

            GameObject sceneModel = GameObject.Instantiate<GameObject>(assetDependencies.mCustomDependencies[0] as GameObject);
            sceneModel.transform.SetParent(GameCenter.sceneShowRoot.transform);
            showSceneControl.Parse(sceneModel);

            rawImage.texture = showSceneControl.GetTemporary(0, 0, 6, RenderTextureFormat.ARGB32, 1f);
        }
        private void _LoadShowModel(uint careerid)
        {
            if (heroLoader == null)
            {
                heroLoader = HeroLoader.Create(true);
                heroLoader.heroDisplay.onLoaded += OnShowModelLoaded;
            }

            heroLoader.LoadHero(Sys_Role.Instance.Role.HeroId, GameCenter.mainHero.weaponComponent.CurWeaponID, ELayerMask.ModelShow, Sys_Fashion.Instance.GetDressData(), (go) =>
            {
                heroLoader.heroDisplay.GetPart(EHeroModelParts.Main).SetParent(showSceneControl.mModelPos, null);
            });

        }

        public void _UnloadShowContent()
        {
            rawImage.texture = null;
            heroLoader?.Dispose();
            heroLoader = null;
            showSceneControl?.Dispose();
        }

        private void OnShowModelLoaded(int obj)
        {
            if (obj == 0)
            {
                CSVCareer.Data cSVCareerData = CSVCareer.Instance.GetConfData((uint)GameCenter.mainHero.careerComponent.CurCarrerType);
                uint highId = Hero.GetMainHeroHighModelAnimationID();
                heroLoader.heroDisplay.mAnimation.UpdateHoldingAnimations(highId, Sys_Equip.Instance.GetCurWeapon());
            }
        }

        public void OnDrag(BaseEventData eventData)
        {
            PointerEventData ped = eventData as PointerEventData;
            Vector3 angle = new Vector3(0f, ped.delta.x * -0.36f, 0f);
            AddEulerAngles(angle);
        }

        public void AddEulerAngles(Vector3 angle)
        {
            if (showSceneControl.mModelPos.transform != null)
            {
                Vector3 localAngle = showSceneControl.mModelPos.transform.localEulerAngles;
                localAngle.Set(localAngle.x + angle.x, localAngle.y + angle.y, localAngle.z + angle.z);
                showSceneControl.mModelPos.transform.localEulerAngles = localAngle;
            }
        }

        #endregion

        #region Button
        private void OnlimitGoBtnClicked()
        {
            if (evt.limitGetWay == 1)
            {
                MallPrama mallPrama = new MallPrama();
                mallPrama.itemId = evt.limitGetParam[0];
                mallPrama.mallId = evt.limitGetParam[1];
                mallPrama.shopId = evt.limitGetParam[2];
                UIManager.OpenUI(EUIID.UI_Mall, false, mallPrama);
            }
        }

        private void OnlimitUseBtnClicked()
        {
            Sys_Head.Instance.HeadFrameSetReq((uint)type, Id);
        }

        private void OnforeverGoBtnClicked()
        {
            if (evt.foreverGetWay == 1)
            {
                MallPrama mallPrama = new MallPrama();
                mallPrama.itemId = evt.foreverGetParam[0];
                mallPrama.mallId = evt.foreverGetParam[1];
                mallPrama.shopId = evt.foreverGetParam[2];
                UIManager.OpenUI(EUIID.UI_Mall, false, mallPrama);
            }
        }

        private void OnforeverUseBtnClicked()
        {
            Sys_Head.Instance.HeadFrameSetReq((uint)type, Id);
        }
        #endregion

    }
}
