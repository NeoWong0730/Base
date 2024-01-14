using System;
using System.Collections.Generic;
using Framework;
using Logic.Core;
using Packet;
using Table;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace Logic
{
    public class UI_ChatEntry1
    {
        private Transform mTrans;

        private Text txtTime_Text;

        private RectTransform rtLeft_RectTransform;
        private Button btn_imgIconL_Button;
        private Image btn_imgIconL_Image;
        private RectTransform rtContentL_RectTransform;
        private Image imgBgL_Image;
        private SequenceFrame imgVoiceL;
        private Text txtSecondL;
        private GameObject Image_RedL;
        private EmojiText txtContentL_EmojiText;
        private Image HeadFrameL_Image;
        private Text txtNameL_Text;

        private RectTransform rtRight_RectTransform;
        //private Button btn_imgIconR_Button; 
        private Image btn_imgIconR_Image;
        private RectTransform rtContentR_RectTransform;
        private Image imgBgR_Image;
        private SequenceFrame imgVoiceR;
        private Text txtSecondR;
        private GameObject Image_RedR;
        private EmojiText txtContentR_EmojiText;
        private Image HeadFrameR_Image;
        private Text txtNameR_Text;

        private RectTransform rtSystem_RectTransform;
        private Image imgIcon_Image;
        private EmojiText txtContent_EmojiText;

        Sys_Chat.ChatContent mChatContent;
        uint wordStyle = 0;

        public void BindGameObject(GameObject gameObject)
        {
            mTrans = gameObject.transform;

            txtTime_Text = mTrans.Find("_txtTime").GetComponent<Text>();

            rtLeft_RectTransform = mTrans.Find("_rtLeft").GetComponent<RectTransform>();
            btn_imgIconL_Image = rtLeft_RectTransform.Find("rtHead/_btn_imgIconL").GetComponent<Image>();
            HeadFrameL_Image = rtLeft_RectTransform.Find("rtHead/imgIconBgR").GetComponent<Image>();
            btn_imgIconL_Button = btn_imgIconL_Image.GetComponent<Button>();
            rtContentL_RectTransform = rtLeft_RectTransform.Find("rtContent") as RectTransform;
            imgBgL_Image = rtContentL_RectTransform.Find("_bg").GetComponent<Image>();
            imgVoiceL = rtContentL_RectTransform.Find("Image_Voice").GetComponent<SequenceFrame>();
            txtSecondL = rtContentL_RectTransform.Find("_txtSecond").GetComponent<Text>();
            Image_RedL = rtContentL_RectTransform.Find("Image_Red").gameObject;
            txtContentL_EmojiText = rtContentL_RectTransform.transform.Find("_txtContentL").GetComponent<EmojiText>();
            txtNameL_Text = rtLeft_RectTransform.Find("_txtName").GetComponent<Text>();

            rtRight_RectTransform = mTrans.Find("_rtRight").GetComponent<RectTransform>();
            btn_imgIconR_Image = rtRight_RectTransform.Find("rtHead/_btn_imgIconR").GetComponent<Image>();
            HeadFrameR_Image = rtRight_RectTransform.Find("rtHead/imgIconBgR").GetComponent<Image>();
            //btn_imgIconR_Button = btn_imgIconR_Image.GetComponent<Button>();
            rtContentR_RectTransform = rtRight_RectTransform.Find("rtContent") as RectTransform;
            imgBgR_Image = rtContentR_RectTransform.Find("_bg").GetComponent<Image>();
            imgVoiceR = rtContentR_RectTransform.Find("Image_Voice").GetComponent<SequenceFrame>();
            txtSecondR = rtContentR_RectTransform.Find("_txtSecond").GetComponent<Text>();
            Image_RedR = rtContentR_RectTransform.Find("Image_Red").gameObject;
            txtContentR_EmojiText = rtContentR_RectTransform.transform.Find("_txtContentR").GetComponent<EmojiText>();
            txtNameR_Text = rtRight_RectTransform.Find("_txtName").GetComponent<Text>();

            rtSystem_RectTransform = mTrans.Find("_rtSystem").GetComponent<RectTransform>();
            imgIcon_Image = rtSystem_RectTransform.Find("_imgIcon").GetComponent<Image>();
            txtContent_EmojiText = rtSystem_RectTransform.Find("_txtContent").GetComponent<EmojiText>();

            btn_imgIconL_Button.onClick.AddListener(OnimgIconL_ButtonClicked);
            //btn_imgIconR_Button.onClick.AddListener(OnimgIconR_ButtonClicked);

            txtContentL_EmojiText.onHrefClick += OnHrefClick;
            txtContentR_EmojiText.onHrefClick += OnHrefClick;
            txtContent_EmojiText.onHrefClick += OnHrefClick;

            imgBgL_Image.GetComponent<Button>().onClick.AddListener(OnPlayVoice);
            imgBgR_Image.GetComponent<Button>().onClick.AddListener(OnPlayVoice);
        }

        private void OnPlayVoice()
        {
            if (string.IsNullOrWhiteSpace(mChatContent.sFileID))
                return;

            mChatContent.bPlayed = true;
            if (mChatContent.bHasIllegalWord)
            {
                Sys_Chat.Instance.PushErrorTip(Sys_Chat.Chat_HasIllegalWord_Error);
            }
            else
            {
                Sys_Chat.Instance.PlayFile(mChatContent.sFileID);
            }
        }

        private void OnHrefClick(string data)
        {
            if (string.IsNullOrWhiteSpace(data))
                return;

            string[] ss = data.Split('_');
            if (ss.Length < 1) return;

            int.TryParse(ss[0], out int type);            
            string v = ss.Length > 1 ? ss[1] : string.Empty;

            if (type == (int)ERichType.Item)
            {
                ulong.TryParse(v, out ulong id);

                //ulong uuid = (ulong)id;
                //ItemData itemData = Sys_Bag.Instance.GetItemDataByUuid(uuid);
                //Sys_Chat.Instance.recItems.TryGetValue(uuid, out Packet.ItemCommonData itemData);

                if (mChatContent.mChatExtData == null || mChatContent.mChatExtData.Item == null || mChatContent.mChatExtData.Item.Count <= (int)id)
                    return;

                Packet.ItemCommonData itemData = mChatContent.mChatExtData.Item[(int)id];

                CSVItem.Data infoData = CSVItem.Instance.GetConfData(itemData.Id);
                if (infoData != null)
                {
                    if (infoData.type_id == (uint)EItemType.Equipment)
                    {
                        EquipTipsData tipData = new EquipTipsData();
                        tipData.equip = new ItemData(itemData.BoxId, itemData.Uuid, itemData.Id, itemData.Count, 0u, false, false, itemData.Equipment, null, 0);
                        tipData.isCompare = false;
                        tipData.isShowOpBtn = false;
                        tipData.isShowLock = false;

                        UIManager.OpenUI(EUIID.UI_TipsEquipment, false, tipData);
                    }
                    else if (infoData.type_id == (uint)EItemType.Pet)
                    {
                        ClientPet pet = new ClientPet(itemData.PetUnit, false);
                        UIManager.OpenUI(EUIID.UI_Pet_Details, false, pet);
                    }
                    else if (infoData.type_id == (uint)EItemType.Ornament)
                    {
                        OrnamentTipsData tipData = new OrnamentTipsData();
                        tipData.equip = new ItemData(itemData.BoxId, itemData.Uuid, itemData.Id, itemData.Count, 0u, false, false, null, null, 0, null, null, itemData.Ornament);
                        tipData.isCompare = false;
                        tipData.isShowLock = false;
                        tipData.sourceUiId = EUIID.UI_Chat;
                        UIManager.OpenUI(EUIID.UI_Tips_Ornament, false, tipData);
                    }
                    else if (infoData.type_id == (int)EItemType.PetEquipment)
                    {
                        PetEquipTipsData petEquipTipsData = new PetEquipTipsData();
                        petEquipTipsData.openUI = EUIID.UI_Chat;
                        petEquipTipsData.petEquip = new ItemData(itemData.BoxId, itemData.Uuid, itemData.Id, itemData.Count, 0u, false, false, null, null, 0,null,null,null, itemData.PetEquip);
                        petEquipTipsData.isCompare = false;
                        petEquipTipsData.isShowOpBtn = false;
                        petEquipTipsData.isShowLock = false;
                        UIManager.OpenUI(EUIID.UI_Tips_PetMagicCore, false, petEquipTipsData);
                    }
                    else
                    {

                        PropIconLoader.ShowItemData showItem = new PropIconLoader.ShowItemData(itemData.Id, itemData.Count, true, false, false, false, false);
                        showItem.SetQuality(itemData.Quality);

                        UIManager.OpenUI(EUIID.UI_Message_Box, false, new MessageBoxEvt(EUIID.UI_Chat, showItem));
                    }
                }
            }
            else if (type == (int)ERichType.Task)
            {
                uint.TryParse(v, out uint id);
                UIManager.OpenUI(EUIID.UI_ChatTaskInfo, false, id);
            }
            else if (type == (int)ERichType.JoinTeam)
            {
                ulong.TryParse(v, out ulong id);
                Sys_Team.Instance.ApplyJoinTeam(id, Sys_Role.Instance.RoleId);
            }
            else if (type == (int)ERichType.MallItem)
            {
                uint.TryParse(v, out uint id);
                UIManager.OpenUI(EUIID.UI_Message_Box, false, new MessageBoxEvt(EUIID.UI_Chat, new PropIconLoader.ShowItemData(id, 0, true, false, false, false, false)));
            }
            else if (type == (int)ERichType.TradeItem)
            {
                uint.TryParse(v, out uint id);
                string[] param = ss[2].Split('|');
                uint cross = 0u;
                uint showType = 0u;
                ulong goodsId = 0;
                uint.TryParse(param[0], out cross);
                uint.TryParse(param[1], out showType);
                if (param.Length >= 3)
                    ulong.TryParse(param[2], out goodsId);

                Sys_Trade.TelData telData = new Sys_Trade.TelData();
                telData.telType = 0u;
                telData.bCross = cross == 1u;
                telData.tradeShowType = (TradeShowType)showType;
                telData.goodsUId = goodsId;
                telData.itemInfoId = (uint)id;

                if (Sys_FamilyResBattle.Instance.InFamilyBattle) {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(3230000059));
                    return;
                }

                UIManager.OpenUI(EUIID.UI_Trade, false, telData);
            }
            else if (type == (int)ERichType.TitleItem)
            {
                uint.TryParse(v, out uint id);
                Title title = Sys_Title.Instance.GetTitleData(id);
                if (title != null)
                {
                    UIManager.OpenUI(EUIID.UI_TitleTips, false, title);
                }
            }
            else if (type == (int)ERichType.Auction)
            {
                UIManager.OpenUI(EUIID.UI_FamilySale);
            }
            else if (type == (int)ERichType.FamilyConsign)
            {
                uint.TryParse(v, out uint id);
                GuildConsignInfo guildConsignInfo = null;
                for (int i = 0; i < Sys_Family.Instance.consignInfos.Count; i++)
                {
                    if (Sys_Family.Instance.consignInfos[i].UId == id)
                    {
                        guildConsignInfo = Sys_Family.Instance.consignInfos[i];
                        break;
                    }
                }
                if (guildConsignInfo != null)
                {
                    if (Sys_Role.Instance.Role.Name != guildConsignInfo.Name)
                    {
                        OpenFamilyConsignDetailParm openFamilyConsignDetailParm = new OpenFamilyConsignDetailParm();
                        openFamilyConsignDetailParm.type = 1;
                        openFamilyConsignDetailParm.guildConsignInfo = guildConsignInfo;
                        openFamilyConsignDetailParm.name = guildConsignInfo.Name.ToStringUtf8();
                        openFamilyConsignDetailParm.fromType = 1;
                        UIManager.OpenUI(EUIID.UI_FamilyWorkshop_Detail, false, openFamilyConsignDetailParm);
                    }
                }
                else
                {
                    string content = LanguageHelper.GetTextContent(590002037);
                    Sys_Hint.Instance.PushContent_Normal(content);
                }
            }
            else if (type == (int)ERichType.FamilyRedPacket)
            {
                uint.TryParse(v, out uint id);
                Sys_Family.Instance.QueryEnvelopeInfoReq((uint)id);
            }
            else if (type == (int)ERichType.Name)
            {
                ulong.TryParse(v, out ulong id);
                if (id != Sys_Role.Instance.RoleId)
                    Sys_Role_Info.Instance.OpenRoleInfo(id, Sys_Role_Info.EType.Chat);
            }
            else if(type == (int)ERichType.FightRecode)
            {
                if (GameMain.Procedure.CurrentProcedure.ProcedureType == ProcedureManager.EProcedureType.Fight)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(591000012));
                    return;
                }
                if (Sys_Role.Instance.isCrossSrv)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11898));
                    return;
                }
                string[] param = ss[1].Split('|');
                ulong videoId = 0u;
                ulong authorId = 0u;
                ulong.TryParse(param[0], out videoId);
                ulong.TryParse(param[1], out authorId);
                ClientVideo clientVideo = Sys_Video.Instance.SearchVideoListById(videoId, authorId);
                if (clientVideo == null)
                {
                    clientVideo = new ClientVideo();
                    clientVideo.baseBrif = new VideoBaseBrief();
                    clientVideo.authorBrif = new VideoAuthorBrief();
                    clientVideo.baseBrif.MaxRound = (uint)(videoId & 0xFFF) >> 6;
                    clientVideo.video = videoId;
                    clientVideo.authorBrif.Author = authorId;
                    clientVideo.where = VideoWhere.None;
                }
                Net_Combat.Instance.PlayVideoPreview(clientVideo.video, clientVideo);
            }
            else if (type == (int)ERichType.Achievement)
            {
                int.TryParse(v, out int index);
                if (mChatContent.mChatExtData == null || mChatContent.mChatExtData.AchData == null || mChatContent.mChatExtData.AchData.Count <= index)
                {
                    uint.TryParse(v, out uint tid);
                    if (!Sys_FunctionOpen.Instance.IsOpen(22090, true))
                        return;
                    UIManager.OpenUI(EUIID.UI_Achievement);
                    OpenAchievementMenuParam param = new OpenAchievementMenuParam()
                    {
                        tid = tid,
                    };
                    UIManager.OpenUI(EUIID.UI_Achievement_Menu, false, param);
                }
                else
                {
                    AchievementData achData = mChatContent.mChatExtData.AchData[index];
                    AchievementDataCell dataCell = new AchievementDataCell();
                    dataCell.tid = achData.Id;
                    dataCell.timestamp = achData.Timestamp;
                    if (achData.HistoryDatas != null && achData.HistoryDatas.Count > 0)
                    {
                        for (int i = 0; i < achData.HistoryDatas.Count; i++)
                        {
                            AchievementDataCell.RoleAchievementHistory historyData = new AchievementDataCell.RoleAchievementHistory();
                            historyData.serverName = achData.HistoryDatas[i].Servername.ToStringUtf8();
                            historyData.timestamp = achData.HistoryDatas[i].Timestamp;
                            dataCell.achHistoryList.Add(historyData);
                        }
                    }
                    UIManager.OpenUI(EUIID.UI_Achievement_AccessTip, false, dataCell);
                }
            }
            else if (type == (int)ERichType.MerchantFleet)
            {
                Sys_MerchantFleet.Instance.OpenFamilyHelp();
            }
        }

        public void SetData(Sys_Chat.ChatContent content)
        {            
            mChatContent = content;
            if (mChatContent == null)
                return;

            Sys_Chat.ChatBaseInfo baseInfo = mChatContent.mBaseInfo;
            //0 = 单条显示 1 = 自己 2 = 其他玩家
            int displayType = content.DisplayType();
            uint headIcon = 0;
            uint textFrame = CSVChatframe.Instance.GetConfData(300).ChatIcon;    //默认值
            uint headFrame = 0;
            //uint wordStyle = 0;

            wordStyle = 0;

            if (displayType != 0)
            {
                if (baseInfo.SenderHead == 100)
                {
                    CSVCharacter.Data heroData = CSVCharacter.Instance.GetConfData(baseInfo.nHeroID);
                    if (heroData != null)
                    {
                        headIcon = heroData.headid;
                    }
                }
                else
                {
                    CSVHead.Data headData = CSVHead.Instance.GetConfData(baseInfo.SenderHead);
                    if (headData != null)
                    {
                        uint headIconId = Sys_Head.Instance.GetHeadIconIdByRoleType(headData.HeadIcon, baseInfo.nHeroID);
                        headIcon = headIconId;
                    }
                }

                //文本框和文字风格
                CSVChatframe.Data chatframeData = CSVChatframe.Instance.GetConfData(baseInfo.SenderChatFrame);
                if (chatframeData != null)
                {
                    textFrame = chatframeData.ChatIcon;
                    wordStyle = chatframeData.Word;
                }

                //默认字体不覆盖聊天框字体
                if (baseInfo.SenderChatText != 500u)
                {
                    CSVChatWord.Data chatWordData = CSVChatWord.Instance.GetConfData(baseInfo.SenderChatText);
                    if (chatWordData != null)
                    {
                        wordStyle = chatWordData.WordIcon;
                    }
                }

                CSVHeadframe.Data headframeData = CSVHeadframe.Instance.GetConfData(baseInfo.SenderHeadFrame);
                if (headframeData != null)
                {
                    headFrame = headframeData.HeadframeIcon;
                }
            }

            bool hasTime = mChatContent.nTimePoint > 0;
            int topOffset = 0;
            if (hasTime)
            {
                txtTime_Text.gameObject.SetActive(true);
                txtTime_Text.text = Sys_Time.ConvertToLocalTime(mChatContent.nTimePoint).ToString("yyyy/MM/dd HH:mm:ss");
                topOffset = -26;
            }
            else
            {
                txtTime_Text.gameObject.SetActive(false);
                txtTime_Text.text = string.Empty;
            }

            if (displayType == 0)
            {
                rtSystem_RectTransform.gameObject.SetActive(true);
                rtRight_RectTransform.gameObject.SetActive(false);
                rtLeft_RectTransform.gameObject.SetActive(false);

                rtSystem_RectTransform.offsetMax = new Vector2(rtSystem_RectTransform.offsetMax.x, topOffset);
            }
            else if (displayType == 1)
            {
                rtSystem_RectTransform.gameObject.SetActive(false);
                rtRight_RectTransform.gameObject.SetActive(true);
                rtLeft_RectTransform.gameObject.SetActive(false);

                rtRight_RectTransform.offsetMax = new Vector2(rtRight_RectTransform.offsetMax.x, topOffset);
            }
            else if (displayType == 2)
            {
                rtSystem_RectTransform.gameObject.SetActive(false);
                rtRight_RectTransform.gameObject.SetActive(false);
                rtLeft_RectTransform.gameObject.SetActive(true);

                rtLeft_RectTransform.offsetMax = new Vector2(rtLeft_RectTransform.offsetMax.x, topOffset);
            }

            switch (displayType)
            {
                case 0:
                    {
                        ImageHelper.SetChatChannelIcon(imgIcon_Image, mChatContent.eChatType, baseInfo == null ? EFightActorType.None : baseInfo.eActorType);
                        txtContent_EmojiText.text = mChatContent.sUIContent;
                        //txtName_Text.text = mChatContent.mBaseInfo?.sSenderName;                        
                    }
                    break;
                case 1:
                    {
                        //设置名字（回归玩家）
                        if (mChatContent.mBaseInfo != null)
                        {
                            if (mChatContent.mBaseInfo.BackActivity)
                            {
                                txtNameR_Text.text = LanguageHelper.GetTextContent(2014918) + mChatContent.mBaseInfo.sSenderName;
                            }
                            else
                            {
                                txtNameR_Text.text = mChatContent.mBaseInfo.sSenderName;
                            }
                        }
                        else
                        {
                            txtNameR_Text.text = string.Empty;
                        }

                        //设置头像
                        ImageHelper.SetIcon(btn_imgIconR_Image, headIcon);

                        //设置头像框
                        if (headFrame != 0)
                        {
                            ImageHelper.SetIcon(HeadFrameR_Image, headFrame);
                            HeadFrameR_Image.gameObject.SetActive(true);
                        }
                        else
                        {
                            HeadFrameR_Image.gameObject.SetActive(false);
                        }

                        //设置文字背景框
                        ImageHelper.SetIcon(imgBgR_Image, textFrame);

                        //计算文本内容顶部偏移高度
                        float contentOffsetH = UI_Chat.gContentTop;
                        float width = 0;

                        //设置语音信息
                        if (!string.IsNullOrWhiteSpace(mChatContent.sFileID))
                        {
                            contentOffsetH += UI_Chat.gContentMinHeight;
                            contentOffsetH += UI_Chat.gSpace;

                            txtSecondR.gameObject.SetActive(true);
                            imgVoiceR.gameObject.SetActive(true);

                            txtSecondR.text = LanguageHelper.GetTextContent(10941, mChatContent.fDuration.ToString());

                            width = (UI_Chat.gContentMax - UI_Chat.gVoiceMin) * ((float)mChatContent.fDuration / Sys_Chat.Instance.fMaxRecordTime) + UI_Chat.gVoiceMin;

                            //刷新喇叭状态
                            RefreshState();
                        }
                        else
                        {
                            txtSecondR.gameObject.SetActive(false);
                            imgVoiceR.gameObject.SetActive(false);
                            Image_RedR.gameObject.SetActive(false);
                        }

                        //设置文本
                        if (!string.IsNullOrWhiteSpace(mChatContent.sUIContent))
                        {
                            TextHelper.SetText(txtContentR_EmojiText, mChatContent.sUIContent, LanguageHelper.GetTextStyle(wordStyle));//                            

                            RectTransform rect = txtContentR_EmojiText.rectTransform;
                            rect.offsetMax = new Vector2(rect.offsetMax.x, -contentOffsetH);

                            txtContentR_EmojiText.gameObject.SetActive(true);

                            //获取文本宽度
                            width = Mathf.Max(width, txtContentR_EmojiText.preferredWidth + UI_Chat.gContentLeft + UI_Chat.gContentBottom);
                        }
                        else
                        {
                            txtContentR_EmojiText.gameObject.SetActive(false);
                        }

                        width = Mathf.Min(UI_Chat.gContentMax, width);
                        Vector2 v = rtContentR_RectTransform.sizeDelta;
                        rtContentR_RectTransform.sizeDelta = new Vector2(width, v.y);
                    }
                    break;
                case 2:
                    {
                        //设置名字（回归玩家）
                        if (mChatContent.mBaseInfo != null)
                        {
                            if (mChatContent.mBaseInfo.BackActivity)
                            {
                                txtNameL_Text.text = LanguageHelper.GetTextContent(2014918) + mChatContent.mBaseInfo.sSenderName;
                            }
                            else
                            {
                                txtNameL_Text.text = mChatContent.mBaseInfo.sSenderName;
                            }
                        }
                        else
                        {
                            txtNameL_Text.text = string.Empty;
                        }

                        //设置头像
                        ImageHelper.SetIcon(btn_imgIconL_Image, headIcon);

                        //设置头像框
                        if (headFrame != 0)
                        {
                            ImageHelper.SetIcon(HeadFrameL_Image, headFrame);
                            HeadFrameL_Image.gameObject.SetActive(true);
                        }
                        else
                        {
                            HeadFrameL_Image.gameObject.SetActive(false);
                        }

                        //设置文字背景框
                        ImageHelper.SetIcon(imgBgL_Image, textFrame);

                        //计算文本内容顶部偏移高度
                        float contentOffsetH = UI_Chat.gContentTop;
                        float width = 0;

                        //设置语音信息
                        if (!string.IsNullOrWhiteSpace(mChatContent.sFileID))
                        {
                            contentOffsetH += UI_Chat.gContentMinHeight;
                            contentOffsetH += UI_Chat.gSpace;

                            txtSecondL.text = LanguageHelper.GetTextContent(10941, mChatContent.fDuration.ToString());

                            txtSecondL.gameObject.SetActive(true);
                            imgVoiceL.gameObject.SetActive(true);

                            width = (UI_Chat.gContentMax - UI_Chat.gVoiceMin) * ((float)mChatContent.fDuration / Sys_Chat.Instance.fMaxRecordTime) + UI_Chat.gVoiceMin;

                            //刷新喇叭状态
                            RefreshState();
                        }
                        else
                        {
                            txtSecondL.gameObject.SetActive(false);
                            imgVoiceL.gameObject.SetActive(false);
                            Image_RedL.gameObject.SetActive(false);
                        }

                        //设置文本
                        if (!string.IsNullOrWhiteSpace(mChatContent.sUIContent))
                        {
                            TextHelper.SetText(txtContentL_EmojiText, mChatContent.sUIContent, LanguageHelper.GetTextStyle(wordStyle));//                            

                            RectTransform rect = txtContentL_EmojiText.rectTransform;
                            rect.offsetMax = new Vector2(rect.offsetMax.x, -contentOffsetH);

                            txtContentL_EmojiText.gameObject.SetActive(true);

                            //获取文本宽度
                            width = Mathf.Max(width, txtContentL_EmojiText.preferredWidth + UI_Chat.gContentLeft + UI_Chat.gContentBottom);
                        }
                        else
                        {
                            txtContentL_EmojiText.gameObject.SetActive(false);
                        }

                        width = Mathf.Min(UI_Chat.gContentMax, width);
                        Vector2 v = rtContentL_RectTransform.sizeDelta;
                        rtContentL_RectTransform.sizeDelta = new Vector2(width, v.y);
                    }
                    break;
                default:
                    break;
            }
        }

        public void RefreshState()
        {
            if (string.IsNullOrWhiteSpace(mChatContent.sFileID))
            {
                return;
            }
            //imgVoiceL.SetAllDirty();
            if (string.Equals(mChatContent.sFileID, Sys_Chat.Instance.sCurrentFileID, StringComparison.Ordinal))
            {
                imgVoiceL.Play(0);
                imgVoiceR.Play(0);
            }
            else
            {
                imgVoiceL.StopIn(0);
                imgVoiceR.StopIn(0);
            }

            Image_RedL.SetActive(!mChatContent.bPlayed);
            Image_RedR.SetActive(!mChatContent.bPlayed);
        }

        public void OnimgIconL_ButtonClicked()
        {
            if (mChatContent.mBaseInfo.nHeroID == 0)
                return;
            Sys_Role_Info.Instance.OpenRoleInfo(mChatContent.mBaseInfo.nRoleID, Sys_Role_Info.EType.Chat);
        }

        public void RefreshContent(ulong uid)
        {
            if (mChatContent.uid != uid)
                return;

            int displayType = mChatContent.DisplayType();
            switch (displayType)
            {
                case 0:
                    {
                        txtContent_EmojiText.text = mChatContent.sUIContent;
                    }
                    break;
                case 1:
                    {
                        //设置文本
                        if (!string.IsNullOrWhiteSpace(mChatContent.sUIContent))
                        {
                            TextHelper.SetText(txtContentR_EmojiText, mChatContent.sUIContent, LanguageHelper.GetTextStyle(wordStyle));
                        }
                    }
                    break;
                case 2:
                    {
                        //设置文本
                        if (!string.IsNullOrWhiteSpace(mChatContent.sUIContent))
                        {
                            TextHelper.SetText(txtContentL_EmojiText, mChatContent.sUIContent, LanguageHelper.GetTextStyle(wordStyle));//                            
                        }
                    }
                    break;
            }
        }
    }

    public class UI_ChatEntry2
    {
        private Image _img_channel_Image;
        private SequenceFrame _img_voice_SequenceFrame;
        private EmojiText _txt_content_Text;
        private Text _txt_name_Text;
        private Sys_Chat.ChatContent chatEntry;
        private RectTransform _txt_content_RT;

        public void BindGameObject(GameObject gameObject)
        {
            RectTransform root = gameObject.GetComponent<RectTransform>();
            _img_channel_Image = root.Find("_img_channel").GetComponent<Image>();
            _img_voice_SequenceFrame = root.Find("_txt_name/_img_voice").GetComponent<SequenceFrame>();
            _txt_content_Text = root.Find("_txt_content").GetComponent<EmojiText>();
            _txt_name_Text = root.Find("_txt_name").GetComponent<Text>();
            _txt_content_RT = _txt_content_Text.GetComponent<RectTransform>();

            root.GetComponent<Button>().onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            ChatType chatType = chatEntry.eChatType;
            Sys_Chat.Instance.eChatType = chatType;
            if (chatType == ChatType.Person)
            {
                Sys_Chat.Instance.eChatType = ChatType.System;
                Sys_Chat.Instance.SetSystemChannelShow(ChatType.System);
            }
            else if (chatType == ChatType.Horn)
            {
                Sys_Chat.Instance.eChatType = ChatType.System;
                Sys_Chat.Instance.SetSystemChannelShow(ChatType.System);
            }
            else if (chatType == ChatType.System || chatType == ChatType.Notice)
            {
                Sys_Chat.Instance.eChatType = ChatType.System;
                Sys_Chat.Instance.SetSystemChannelShow(ChatType.System);
            }

#if UNITY_STANDALONE_WIN && !UNITY_EDITOR
            if (AspectRotioController.IsExpandState)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1012022));
                return;
            }
#endif
            UIManager.CloseUI(EUIID.UI_ChatSimplify, true, false);
            UIManager.OpenUI(EUIID.UI_Chat, false, null, EUIID.UI_MainInterface);
        }

        public void SetData(Sys_Chat.ChatContent content)
        {
            chatEntry = content;
            if (chatEntry == null)
                return;

            Sys_Chat.ChatBaseInfo baseInfo = chatEntry.mBaseInfo;
            EFightActorType actorType = EFightActorType.None;

            if (baseInfo == null)
            {
                _txt_name_Text.gameObject.SetActive(false);
            }
            else
            {
                _txt_name_Text.gameObject.SetActive(true);

                //设置名字（回归玩家）
                if (baseInfo.BackActivity)
                {
                    _txt_name_Text.text = LanguageHelper.GetTextContent(2014918) + baseInfo.sSenderName;
                }
                else
                {
                    _txt_name_Text.text = baseInfo.sSenderName;
                }

                actorType = baseInfo.eActorType;
            }

            _txt_content_Text.text = chatEntry.sSimplifyUIContent;

            ImageHelper.SetChatChannelIcon(_img_channel_Image, chatEntry.eChatType, actorType);

            _img_voice_SequenceFrame.gameObject.SetActive(!string.IsNullOrWhiteSpace(chatEntry.sFileID));

            RefreshState();
            _txt_content_RT.sizeDelta = new Vector2(_txt_content_RT.sizeDelta.x, _txt_content_Text.preferredHeight);
        }

        public void RefreshState()
        {
            if (string.Equals(chatEntry.sFileID, Sys_Chat.Instance.sCurrentFileID, StringComparison.Ordinal))
            {
                _img_voice_SequenceFrame.Play(0);
            }
            else
            {
                _img_voice_SequenceFrame.StopIn(0);
            }            
        }

        public void RefreshContent()
        {
            _txt_content_Text.text = chatEntry.sSimplifyUIContent;
        }

        public int GetHeight()
        {
            return (int)_txt_content_Text.preferredHeight + 8;
        }
    }

    public class UI_ChatBQ
    {
        private EmojiText txt_BQ = null;
        private string _sContent;

        public Action<string> onClick;

        public void BindGameObject(GameObject go)
        {
            txt_BQ = go.transform.Find("_txt_BQ").GetComponent<EmojiText>();
            go.GetComponent<Button>().onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            onClick?.Invoke(_sContent);
        }

        public void Refresh(string content)
        {
            _sContent = content;
            string s = EmojiTextHelper.ParseChatRichText(txt_BQ.emojiAsset, null, content, 0, 64);
            //txt_BQ.text = _sContent;
            txt_BQ.text = s;
        }
    }

    public class UI_ChatLS
    {
        private Text txt_BQ = null;
        private InputCache _cache;

        public Action<InputCache> onClick;

        public void BindGameObject(GameObject go)
        {
            txt_BQ = go.transform.Find("_txt_BQ").GetComponent<Text>();
            go.GetComponent<Button>().onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            onClick?.Invoke(_cache);
        }

        public void SetData(InputCache cache)
        {
            _cache = cache;
            txt_BQ.text = _cache.GetContent();
        }
    }

    public class UI_ChatRW
    {
        private Text txt_BQ = null;
        public uint id = 0;

        public Action<UI_ChatRW> onClick;

        public void BindGameObject(GameObject go)
        {
            txt_BQ = go.transform.Find("_txt_BQ").GetComponent<Text>();
            go.GetComponent<Button>().onClick.AddListener(OnClick);
        }

        private void OnClick()
        {
            onClick?.Invoke(this);
        }

        public void SetData(TaskEntry entry)
        {
            id = entry.id;
            if (entry.csvTask.taskCategory == (int)ETaskCategory.Trunk)
            {
                TextHelper.SetText(txt_BQ, 2003001u, LanguageHelper.GetTaskTextContent(entry.csvTask.taskName));
            }
            else
            {
                TextHelper.SetText(txt_BQ, 2003002u, LanguageHelper.GetTaskTextContent(entry.csvTask.taskName));
            }
        }

        public string ToSendString()
        {
            return string.Format("[!{0}]", id.ToString());
        }
    }

    public class UI_ChatRoomPlayer
    {
        private ulong _id = 0;
        private string openID = string.Empty;

        private Transform transform;
        private Button btn_Invite;
        private Image img_Icon;
        private Image img_audioIcon;

        public void BindGameObject(GameObject go)
        {
            transform = go.transform;
            btn_Invite = transform.Find("Image_Invite").GetComponent<Button>();
            img_Icon = transform.Find("Image_Icon").GetComponent<Image>();
            img_audioIcon = transform.Find("Image_Mask").GetComponent<Image>();

            btn_Invite.onClick.AddListener(OnInviteClick);
        }

        private void OnInviteClick()
        {
            Sys_Chat.Instance.Notice();
        }

        public void SetData(ulong id, bool isEnabled)
        {
            _id = id;
            openID = id.ToString();

            if (isEnabled == false)
            {
                transform.gameObject.SetActive(false);
                return;
            }

            transform.gameObject.SetActive(true);

            TeamMem teamMem = null;
            if (id > 0)
            {
                teamMem = Sys_Team.Instance.getTeamMem(id);
            }

            if (teamMem != null)
            {
                btn_Invite.gameObject.SetActive(false);
                img_Icon.gameObject.SetActive(true);
                if (Sys_Chat.Instance.mInRoomRoleInfos.TryGetValue(id, out bool v) && v)
                {
                    ImageHelper.SetIcon(img_audioIcon, GlobalAssets.sAtlas_Chat, "chat_trumpets03");
                    img_audioIcon.gameObject.SetActive(false);
                    img_Icon.color = Color.white;
                }
                else
                {
                    ImageHelper.SetIcon(img_audioIcon, GlobalAssets.sAtlas_Chat, "chat_trumpets_ban");
                    img_audioIcon.gameObject.SetActive(true);
                    img_Icon.color = Color.gray;
                }

                ImageHelper.SetIcon(img_Icon, CharacterHelper.getHeadID(teamMem.HeroId, teamMem.HeadId), false);
            }
            else
            {
                btn_Invite.gameObject.SetActive(true);
                img_Icon.gameObject.SetActive(false);
                img_audioIcon.gameObject.SetActive(false);
            }
        }

        public void OnLateUpdate()
        {
            if (string.IsNullOrEmpty(openID))
                return;

            if (Sys_Chat.Instance.mInRoomRoleInfos.TryGetValue(_id, out bool v) && v)
            {
                if (Sys_Chat.Instance.GetRecvStreamLevel(openID) > 0)
                {
                    img_audioIcon.gameObject.SetActive(true);
                }
                else
                {
                    img_audioIcon.gameObject.SetActive(false);
                }
            }
        }
    }

    public class UI_ChatVoiceButton
    {
        private RectTransform _target;
        UI_LongPressButton _trigger;
        DragEventTrigger _dragEventTrigger;

        public int _chatType = -1; //对应ChatType 枚举 -1为通用按钮

        public Action onClick;

        public void BindGameObject(RectTransform target, float longPressTime = 0f)
        {
            _target = target;
            _trigger = _target.GetComponent<UI_LongPressButton>();
            _dragEventTrigger = _target.GetComponent<DragEventTrigger>();
            _trigger.interval = longPressTime;

            _trigger.onStartPress.AddListener(OnPointerDown);
            _dragEventTrigger.trigger.AddListener(OnDrag);
            _trigger.onRelease.AddListener(OnPointerUp);
        }

        public void SetData(int chatType)
        {
            _chatType = chatType;
        }

        public void Clear()
        {
            //_trigger.ClearEvents();
        }

        private void OnDrag(PointerEventData eventData)
        {
            RectTransform rt = _target.transform as RectTransform;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(rt, eventData.position, eventData.pressEventCamera, out Vector2 lastPoint);
            Sys_Chat.Instance.IsVaildRecord = Vector2.SqrMagnitude(lastPoint) <= Vector2.SqrMagnitude(rt.sizeDelta * 0.71f);
        }

        private void OnPointerUp()
        {
            if (_trigger.bLongPressed)
            {
                Sys_Chat.Instance.StopRecode();
            }
            else
            {
                onClick?.Invoke();
            }
        }

        private void OnPointerDown()
        {
            if (!Sys_Chat.Instance.IsSDKInited)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1000982));
                return;
            }

            ChatType chatType = _chatType == -1 ? Sys_Chat.Instance.eChatType : (ChatType)_chatType;
            int rlt = Sys_Chat.Instance.CheckCanSend(chatType);
            if (rlt == Sys_Chat.Chat_Success)
            {
                if (Sys_Chat.Instance.StartRecode(chatType) == 0)
                {
                    UIManager.OpenUI(EUIID.UI_Chat_VoiceInput, true);
                }
            }
            else
            {
                Sys_Chat.Instance.PushErrorTip(rlt);
            }
        }

        private void OnCancel(BaseEventData arg0)
        {
            Sys_Chat.Instance.IsVaildRecord = false;
            Sys_Chat.Instance.StopRecode();
        }
    }

    public class UI_ChatCH
    {
        public Title title;
        public int dataIndex;

        private Transform transform;

        private UI_LongPressButton mBG_Button;
        private Text mTitle_text1;

        private Text mTitle_text2;
        private Image mTitle_img2;

        private Image mTitle_img3;
        private Transform mTitle_Fx3parent;

        AsyncOperationHandle<GameObject> requestRef;
        private GameObject titleEffect;


        public Action<UI_ChatCH> onClick;

        public void BindGameObject(GameObject go)
        {
            transform = go.transform;
            mTitle_text1 = transform.Find("_txt_CH/Text").GetComponent<Text>();
            mTitle_text2 = transform.Find("_txt_CH/Image/Text").GetComponent<Text>();
            mTitle_img2 = transform.Find("_txt_CH/Image").GetComponent<Image>();
            mTitle_img3 = transform.Find("_txt_CH/Image1").GetComponent<Image>();
            mTitle_Fx3parent = transform.Find("_txt_CH/Image1/Fx");

            mBG_Button = transform.GetComponent<UI_LongPressButton>();
            mBG_Button.onStartPress.AddListener(OnStartPress);
            mBG_Button.onClick.AddListener(OnClick);
        }

        public void SetData(Title _title, int _dataIndex)
        {
            this.title = _title;
            this.dataIndex = _dataIndex;
            UpdateTitle();
        }

        public void UpdateTitle()
        {
            CSVTitle.Data cSVTitleData = title.cSVTitleData;
            if (cSVTitleData != null)
            {
                if (cSVTitleData.id == Sys_Title.Instance.familyTitle)
                {
                    if (cSVTitleData.titleShowIcon == 0)
                    {
                        SetTitleShowType(1);
                        TextHelper.SetText(mTitle_text1, Sys_Title.Instance.GetTitleFamiltyName());
                        TextHelper.SetTextGradient(mTitle_text1, (Color)cSVTitleData.titleShow[0], (Color)cSVTitleData.titleShow[1]);
                        TextHelper.SetTextOutLine(mTitle_text1, cSVTitleData.titleShow[2]);
                    }
                    else
                    {
                        SetTitleShowType(2);
                        TextHelper.SetText(mTitle_text2, Sys_Title.Instance.GetTitleFamiltyName());
                        TextHelper.SetTextGradient(mTitle_text2, (Color)cSVTitleData.titleShow[0], (Color)cSVTitleData.titleShow[1]);
                        TextHelper.SetTextOutLine(mTitle_text2, cSVTitleData.titleShow[2]);
                        ImageHelper.SetIcon(mTitle_img2, cSVTitleData.titleShowIcon, true);
                    }
                }
                else
                {
                    if (cSVTitleData.titleShowLan != 0)
                    {
                        if (cSVTitleData.titleShowIcon == 0)
                        {
                            SetTitleShowType(1);
                            TextHelper.SetText(mTitle_text1, cSVTitleData.titleShowLan);
                            //TextHelper.SetTextGradient(mTitle_text1, (Color)cSVTitleData.titleShow[0], (Color)cSVTitleData.titleShow[1]);
                            //TextHelper.SetTextOutLine(mTitle_text1, cSVTitleData.titleShow[2]);
                        }
                        else
                        {
                            SetTitleShowType(2);
                            TextHelper.SetText(mTitle_text2, cSVTitleData.titleShowLan);
                            //TextHelper.SetTextGradient(mTitle_text2, (Color)cSVTitleData.titleShow[0], (Color)cSVTitleData.titleShow[1]);
                            //TextHelper.SetTextOutLine(mTitle_text2, cSVTitleData.titleShow[2]);
                            ImageHelper.SetIcon(mTitle_img2, cSVTitleData.titleShowIcon);
                        }
                    }
                    else
                    {
                        SetTitleShowType(3);
                        ImageHelper.SetIcon(mTitle_img3, cSVTitleData.titleShowIcon);
                        uint FxId = cSVTitleData.titleShowEffect;
                        CSVSystemEffect.Data cSVSystemEffectData = CSVSystemEffect.Instance.GetConfData(FxId);
                        if (cSVSystemEffectData != null)
                        {
                            LoadTitleEffectAssetAsyn(cSVSystemEffectData.FxPath);
                        }
                    }
                }
            }
        }

        private void LoadTitleEffectAssetAsyn(string path)
        {
            AddressablesUtil.InstantiateAsync(ref requestRef, path, OnAssetsLoaded);
        }

        private void OnAssetsLoaded(AsyncOperationHandle<GameObject> handle)
        {
            titleEffect = handle.Result;
            if (null != titleEffect)
            {
                titleEffect.transform.SetParent(mTitle_Fx3parent);
                RectTransform rectTransform = titleEffect.transform as RectTransform;
                rectTransform.localPosition = Vector3.zero;
                rectTransform.localEulerAngles = Vector3.zero;
                rectTransform.localScale = Vector3.one;
            }
        }

        private void SetTitleShowType(int type)
        {
            if (type == 1)
            {
                mTitle_text1.gameObject.SetActive(true);
                mTitle_text2.gameObject.SetActive(false);
                mTitle_img2.gameObject.SetActive(false);
                mTitle_img3.gameObject.SetActive(false);
                mTitle_Fx3parent.gameObject.SetActive(false);
            }
            else if (type == 2)
            {
                mTitle_text1.gameObject.SetActive(false);
                mTitle_text2.gameObject.SetActive(true);
                mTitle_img2.gameObject.SetActive(true);
                mTitle_img3.gameObject.SetActive(false);
                mTitle_Fx3parent.gameObject.SetActive(false);
            }
            else
            {
                mTitle_text1.gameObject.SetActive(false);
                mTitle_text2.gameObject.SetActive(false);
                mTitle_img2.gameObject.SetActive(false);
                mTitle_img3.gameObject.SetActive(true);
                mTitle_Fx3parent.gameObject.SetActive(true);
            }
        }

        public void OnDispose()
        {
            AddressablesUtil.ReleaseInstance(ref requestRef, OnAssetsLoaded);
        }

        private void OnClick()
        {
            if (!mBG_Button.bLongPressed)
            {
                onClick?.Invoke(this);
            }
        }

        private void OnStartPress()
        {
            if (title != null)
            {
                UIManager.OpenUI(EUIID.UI_TitleTips, false, title);
            }
        }

        public string GetTitleName()
        {
            return LanguageHelper.GetTextContent(title.cSVTitleData.titleLan);
        }
    }

    public class UI_ChatCW
    {
        public ClientPet mPetData = null;

        private Transform transform;
        private Image mIcon_Image;
        private Text mName_Text;
        private Text mNum_Text;
        private Text mGrade_Text;
        private UI_LongPressButton button;

        public Action<UI_ChatCW> onClick;

        public void BindGameObject(GameObject go)
        {
            transform = go.transform;
            mIcon_Image = transform.Find("Image_Icon").GetComponent<Image>();
            mName_Text = transform.Find("Text_Name").GetComponent<Text>();
            mNum_Text = transform.Find("Text_Num").GetComponent<Text>();
            mGrade_Text = transform.Find("Text_Grade").GetComponent<Text>();

            button = transform.GetComponent<UI_LongPressButton>();
            button.onStartPress.AddListener(OnStartPress);
            button.onClick.AddListener(OnClick);
        }

        private void OnStartPress()
        {
            UIManager.OpenUI(EUIID.UI_Pet_Details, false, mPetData);
        }

        private void OnClick()
        {
            if (!button.bLongPressed)
            {
                onClick?.Invoke(this);
            }
        }

        public void SetData(ClientPet petData)
        {
            mPetData = petData;
            mPetData.isShowLock = false;
            ImageHelper.SetIcon(mIcon_Image, petData.petData.icon_id);
            TextHelper.SetText(mName_Text, petData.petData.name);
            TextHelper.SetText(mNum_Text, petData.petUnit.SimpleInfo.Score.ToString());
            TextHelper.SetText(mGrade_Text, 11705, petData.petUnit.SimpleInfo.Level.ToString());
        }
    }
    public class UI_ChatCJ
    {
        public AchievementDataCell achData;
        private Transform transform;
        private Text textName;
        private Text textTime;
        private Transform starList;
        private UI_LongPressButton button;
        AchievementIconCell iconCell;
        public Action<UI_ChatCJ> onClick;
        Transform[] starArray = new Transform[4];
        public void BindGameObject(GameObject go)
        {
            transform = go.transform;
            iconCell = new AchievementIconCell();
            iconCell.Init(transform.Find("Achievement_Item"));
            textName = transform.Find("Text_Name").GetComponent<Text>();
            textTime = transform.Find("Text_time").GetComponent<Text>();
            starList = transform.Find("Starlist");
            for (int i = 0; i < 4; i++)
            {
                starArray[i] = starList.GetChild(i);
            }
            button = transform.GetComponent<UI_LongPressButton>();
            button.onStartPress.AddListener(OnStartPress);
            button.onClick.AddListener(OnClick);
        }
        private void OnStartPress()
        {
            UIManager.OpenUI(EUIID.UI_Achievement_AccessTip, false, achData);
        }

        private void OnClick()
        {
            if (!button.bLongPressed)
            {
                onClick?.Invoke(this);
            }
        }
        public void SetData(AchievementDataCell data)
        {
            this.achData = data;
            iconCell.SetData(data);
            textName.text = LanguageHelper.GetAchievementContent(achData.csvAchievementData.Achievement_Title);
            textTime.text = TimeManager.GetDateTime(achData.timestamp).ToString("yyyy-MM-dd HH:mm:ss");
            SetStar();
        }
        private void SetStar()
        {
            for (int i = 0; i < starArray.Length; i++)
            {
                starArray[i].gameObject.SetActive(false);
            }
            for (int i = 0; i < achData.csvAchievementData.Rare; i++)
            {
                starArray[i].gameObject.SetActive(true);
            }
        }
    }
}