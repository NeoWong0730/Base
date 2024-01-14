using Lib.Core;
using UnityEngine;
using UnityEngine.UI;
using Table;
using Framework;
using Logic.Core;
using Packet;
using System.Text;

namespace Logic
{
    public partial class UI_Society_Layout
    {
        public class ChatItem
        {
            public static int gTop = 26;
            public static int contentMinHeight = 28;

            public static int contentTop = 20;
            public static int contentBottom = 20;
            public static int contentLeft = 20;
            public static int contentRight = 20;
            public static int contentMax = 280;
            public static int voiceMin = 100;

            Sys_Society.ChatData chatData;

            GameObject root;

            GameObject rightChatRoot;
            RectTransform rightChatRect;
            Image rightChatBgImage;
            RectTransform rightChatBgRect;
            EmojiText rightChatText;
            Image rightRoleIcon;
            Image rightRoleIconFrame;
            Text rightRoleName;
            GameObject rightTimeRoot;
            Text rightTimeText;

            GameObject leftChatRoot;
            RectTransform leftChatRect;
            Image leftChatBgImage;
            RectTransform leftChatBgRect;
            EmojiText leftChatText;
            Image leftRoleIcon;
            Image leftRoleIconFrame;
            Text leftRoleName;
            GameObject leftTimeRoot;
            Text leftTimeText;

            GameObject systemChatRoot;
            RectTransform systemChatRect;
            EmojiText systemChatText;

            GameObject systemTipRoot;
            Text systemTipText;

            StringBuilder stringBuilder = new StringBuilder();

            public void BindGameObject(GameObject gameObject)
            {
                root = gameObject;

                rightChatRoot = root.FindChildByName("roleChatRightRoot");
                rightChatRect = rightChatRoot.FindChildByName("Image_Chat").GetComponent<RectTransform>();
                rightChatBgImage = rightChatRoot.gameObject.FindChildByName("bg").GetComponent<Image>();
                rightChatBgRect = rightChatBgImage.GetComponent<RectTransform>();
                rightChatText = rightChatRoot.gameObject.FindChildByName("Text").GetComponent<EmojiText>();
                rightRoleIcon = rightChatRoot.FindChildByName("Image_Icon").GetComponent<Image>();
                rightRoleIconFrame = rightChatRoot.FindChildByName("Image_Frame").GetComponent<Image>();
                rightRoleName = rightChatRoot.FindChildByName("Text_Name").GetComponent<Text>();
                rightTimeRoot = rightChatRoot.FindChildByName("timeRoot");
                rightTimeText = rightTimeRoot.FindChildByName("Text_Time").GetComponent<Text>();

                leftChatRoot = root.FindChildByName("roleChatLeftRoot");
                leftChatRect = leftChatRoot.FindChildByName("Image_Chat").GetComponent<RectTransform>();
                leftChatBgImage = leftChatRect.gameObject.FindChildByName("bg").GetComponent<Image>();
                leftChatBgRect = leftChatBgImage.GetComponent<RectTransform>();
                leftChatText = leftChatRect.gameObject.FindChildByName("Text").GetComponent<EmojiText>();
                leftRoleIcon = leftChatRoot.FindChildByName("Image_Icon").GetComponent<Image>();
                leftRoleIconFrame = leftChatRoot.FindChildByName("Image_Frame").GetComponent<Image>();
                leftRoleName = leftChatRoot.FindChildByName("Text_Name").GetComponent<Text>();
                leftTimeRoot = leftChatRoot.FindChildByName("timeRoot");
                leftTimeText = leftTimeRoot.FindChildByName("Text_Time").GetComponent<Text>();

                systemChatRoot = root.FindChildByName("systemChatRoot");
                systemChatRect = systemChatRoot.FindChildByName("Image_Chat").GetComponent<RectTransform>();
                systemChatText = systemChatRect.gameObject.FindChildByName("Text").GetComponent<EmojiText>();

                systemTipRoot = root.FindChildByName("systemTipRoot");
                systemTipText = systemTipRoot.FindChildByName("Text").GetComponent<Text>();

                rightChatText.onHrefClick += OnHrefClick;
                leftChatText.onHrefClick += OnHrefClick;
                systemChatText.onHrefClick += OnSystemHrefClick;
            }

            public void SetData(Sys_Society.ChatData _chatData)
            {
                if (_chatData == null)
                    return;

                chatData = _chatData;

                if (_chatData.roleID == Sys_Society.socialSystemID)
                {
                    systemChatRoot.SetActive(true);
                    rightChatRoot.SetActive(false);
                    leftChatRoot.SetActive(false);
                    systemTipRoot.SetActive(false);
                    RefreshSystemChatRoot();
                }
                else if (_chatData.roleID == Sys_Society.socialSystemTipID)
                {
                    systemChatRoot.SetActive(false);
                    rightChatRoot.SetActive(false);
                    leftChatRoot.SetActive(false);
                    systemTipRoot.SetActive(true);
                    RefreshSystemTipRoot();
                }
                else if (_chatData.roleID == Sys_Role.Instance.RoleId)
                {
                    systemChatRoot.SetActive(false);
                    rightChatRoot.SetActive(true);
                    leftChatRoot.SetActive(false);
                    systemTipRoot.SetActive(false);
                    rightTimeRoot.SetActive(_chatData.needShowTimeText);
                    RefreshTimeRoot(rightTimeText, _chatData.sendTime);
                    RefreshRightChatRoot();
                }
                else
                {
                    systemChatRoot.SetActive(false);
                    rightChatRoot.SetActive(false);
                    leftChatRoot.SetActive(true);
                    systemTipRoot.SetActive(false);
                    leftTimeRoot.SetActive(_chatData.needShowTimeText);
                    RefreshTimeRoot(leftTimeText, _chatData.sendTime);
                    RefreshLeftChatRoot();
                }
            }

            void RefreshTimeRoot(Text time, uint sendTime)
            {
                time.text = string.Empty;
                var dateTime = Sys_Time.ConvertToLocalTime(sendTime);
                var nowTime = Sys_Time.ConvertToLocalTime(Sys_Time.Instance.GetServerTime());
                //uint subTime = Sys_Time.Instance.GetServerTime() - sendTime;
                stringBuilder.Clear();
                if(nowTime.Year == dateTime.Year && nowTime.Month == dateTime.Month && nowTime.Day == dateTime.Day)
                {
                    time.text = stringBuilder.Append(dateTime.Hour.ToString("D2")).Append(":").Append(dateTime.Minute.ToString("D2")).ToString();
                }
                else
                {
                    time.text = stringBuilder.Append(dateTime.Year.ToString()).Append("年").Append(dateTime.Month.ToString()).Append("月").Append(dateTime.Day.ToString()).Append("日").Append(dateTime.Hour.ToString("D2")).Append(":").Append(dateTime.Minute.ToString("D2")).ToString();
                }
            }

            void RefreshSystemTipRoot()
            {
                systemTipText.text = EmojiTextHelper.ParseChatRichText(FontManager.GetEmoji(GlobalAssets.sEmoji_0), null, chatData.content, chatData.paramID);
            }

            void RefreshSystemChatRoot()
            {
                systemChatText.text = EmojiTextHelper.ParseChatRichText(FontManager.GetEmoji(GlobalAssets.sEmoji_0), null, chatData.content, chatData.paramID);
            }

            void RefreshRightChatRoot()
            {
                ImageHelper.SetIcon(rightRoleIcon, Sys_Head.Instance.clientHead.headIconId);
                ImageHelper.SetIcon(rightRoleIconFrame, Sys_Head.Instance.clientHead.headFrameIconId);
                ImageHelper.SetIcon(rightChatBgImage, CSVChatframe.Instance.GetConfData(chatData.frameID).ChatIcon);           
                TextHelper.SetText(rightRoleName, Sys_Role.Instance.Role.Name.ToStringUtf8());
                TextHelper.SetText(rightChatText, chatData.content, CSVWordStyle.Instance.GetConfData(CSVChatWord.Instance.GetConfData(chatData.chatTextID).WordIcon));

                float contentOffsetH = contentTop;
                float width = 0;
                RectTransform rect = rightChatText.rectTransform;
                rect.offsetMax = new Vector2(rect.offsetMax.x, -contentOffsetH);
                width = Mathf.Max(width, rightChatText.preferredWidth + contentLeft + contentRight);

                width = Mathf.Min(contentMax, width);
                Vector2 v = rightChatRect.sizeDelta;
                rightChatRect.sizeDelta = new Vector2(width, v.y);
                rightChatBgRect.sizeDelta = new Vector2(rightChatBgRect.sizeDelta.x, rightChatText.preferredHeight);
            }

            void RefreshLeftChatRoot()
            {
                ImageHelper.SetIcon(leftRoleIcon, Sys_Head.Instance.GetHeadImageId(Sys_Society.Instance.socialRolesInfo.rolesDic[chatData.roleID].heroID, Sys_Society.Instance.socialRolesInfo.rolesDic[chatData.roleID].iconId));
                ImageHelper.SetIcon(leftRoleIconFrame, CSVHeadframe.Instance.GetConfData(Sys_Society.Instance.socialRolesInfo.rolesDic[chatData.roleID].iconFrameId).HeadframeIcon);
                if (Sys_Society.Instance.socialRecentlysInfo.recentlyIdsDic.ContainsKey(chatData.roleID) && Sys_Society.Instance.socialRecentlysInfo.GetAllRecentlysInfos()[chatData.roleID].isOnLine)
                {
                    ImageHelper.SetImageGray(leftRoleIcon, false);
                }
                else
                {
                    ImageHelper.SetImageGray(leftRoleIcon, true);
                }
                ImageHelper.SetIcon(leftChatBgImage, CSVChatframe.Instance.GetConfData(chatData.frameID).ChatIcon);
                TextHelper.SetText(leftRoleName, Sys_Society.Instance.socialRolesInfo.rolesDic[chatData.roleID].roleName);
                TextHelper.SetText(leftChatText, chatData.content, CSVWordStyle.Instance.GetConfData(CSVChatWord.Instance.GetConfData(chatData.chatTextID).WordIcon));

                float contentOffsetH = contentTop;
                float width = 0;
                RectTransform rect = leftChatText.rectTransform;
                rect.offsetMax = new Vector2(rect.offsetMax.x, -contentOffsetH);
                width = Mathf.Max(width, leftChatText.preferredWidth + contentLeft + contentRight);

                width = Mathf.Min(contentMax, width);
                Vector2 v = leftChatRect.sizeDelta;
                leftChatRect.sizeDelta = new Vector2(width, v.y);
                leftChatBgRect.sizeDelta = new Vector2(leftChatBgRect.sizeDelta.x, leftChatText.preferredHeight);
            }

            void OnSystemHrefClick(string data)
            {
                string[] ss = data.Split('_');
                int type = 0;
                ulong id = 0;
                if (ss.Length >= 2)
                {
                    int.TryParse(ss[0], out type);
                    ulong.TryParse(ss[1], out id);
                }

                if (type == (int)ERichType.AddFriend)
                {
                    Sys_Society.Instance.ReqAddFriend(id);
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

                    if (chatData.GetChatExtMsg() == null || chatData.GetChatExtMsg().Item == null || chatData.GetChatExtMsg().Item.Count <= (int)id)
                        return;

                    Packet.ItemCommonData itemData = chatData.GetChatExtMsg().Item[(int)id];

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
                            petEquipTipsData.petEquip = new ItemData(itemData.BoxId, itemData.Uuid, itemData.Id, itemData.Count, 0u, false, false, null, null, 0, null, null, null, itemData.PetEquip);
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
                    uint.TryParse(param[0], out cross);
                    uint.TryParse(param[1], out showType);

                    Sys_Trade.TelData telData = new Sys_Trade.TelData();
                    telData.telType = 0u;
                    telData.bCross = cross == 1u;
                    telData.tradeShowType = (TradeShowType)showType;
                    telData.itemInfoId = (uint)id;

                    if (Sys_FamilyResBattle.Instance.InFamilyBattle)
                    {
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
                    Sys_Role_Info.Instance.OpenRoleInfo(id, Sys_Role_Info.EType.Chat);
                }
                else if (type == (int)ERichType.Achievement)
                {
                    int.TryParse(v, out int index);
                    if (chatData.GetChatExtMsg() == null || chatData.GetChatExtMsg().AchData == null || chatData.GetChatExtMsg().AchData.Count <= index)
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
                        AchievementData achData = chatData.GetChatExtMsg().AchData[index];
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
                else if (type == (int)ERichType.FightRecode)
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
                        clientVideo.baseBrif.MaxRound = (uint)(videoId & 0xFFF) >> 6;
                        clientVideo.video = videoId;
                        clientVideo.authorBrif.Author = authorId;
                        clientVideo.where = VideoWhere.None;
                    }
                    Net_Combat.Instance.PlayVideoPreview(clientVideo.video, clientVideo);
                }
            }
        }

        /// <summary>
        /// 玩家消息///
        /// </summary>
        public class ChatRoleItem
        {
            public GameObject root;

            RectTransform rootRect;
            public Image roleIcon;
            public Image roleFrameIcon;
            public Text roleName;
            public EmojiText content;
            public RectTransform contentBg;
            public Image bgImage;
            ContentSizeFitter sizeFitter;

            public Sys_Society.ChatData roleChatData;

            public ChatRoleItem(GameObject gameObject)
            {
                root = gameObject;

                rootRect = root.GetComponent<RectTransform>();
                roleIcon = root.FindChildByName("Image_Icon").GetComponent<Image>();
                roleFrameIcon = root.FindChildByName("Image_Frame").GetComponent<Image>();
                roleName = root.FindChildByName("Text_Name").GetComponent<Text>();
                content = root.FindChildByName("Text").GetComponent<EmojiText>();
                contentBg = root.FindChildByName("Image_Chat").GetComponent<RectTransform>();
                bgImage = contentBg.gameObject.FindChildByName("Image1").GetComponent<Image>();
                content.horizontalOverflow = HorizontalWrapMode.Wrap;
                content.verticalOverflow = VerticalWrapMode.Overflow;

                sizeFitter = content.GetComponent<ContentSizeFitter>();

                content.onHrefClick += OnHrefClick;
            }

            public void Update(Sys_Society.ChatData _roleChatData)
            {
                roleChatData = _roleChatData;

                if (roleChatData.roleID == GameCenter.mainHero.uID)
                {
                    ImageHelper.SetIcon(roleIcon, Sys_Head.Instance.clientHead.headIconId);
                    ImageHelper.SetIcon(roleFrameIcon, Sys_Head.Instance.clientHead.headFrameIconId, true);
                }
                else
                {
                    ImageHelper.SetIcon(roleIcon, Sys_Head.Instance.GetHeadImageId(Sys_Society.Instance.socialRolesInfo.rolesDic[_roleChatData.roleID].heroID, Sys_Society.Instance.socialRolesInfo.rolesDic[_roleChatData.roleID].iconId));
                    ImageHelper.SetIcon(roleFrameIcon, CSVHeadframe.Instance.GetConfData(Sys_Society.Instance.socialRolesInfo.rolesDic[_roleChatData.roleID].iconFrameId).HeadframeIcon, true);
                }

                ImageHelper.SetIcon(bgImage, CSVChatframe.Instance.GetConfData(roleChatData.frameID).ChatIcon);

                if (roleChatData.roleID == GameCenter.mainHero.uID)
                {
                    ImageHelper.SetImageGray(roleIcon, false);
                }
                else
                {
                    if (Sys_Society.Instance.socialRecentlysInfo.recentlyIdsDic.ContainsKey(roleChatData.roleID) && Sys_Society.Instance.socialRecentlysInfo.GetAllRecentlysInfos()[roleChatData.roleID].isOnLine)
                    {
                        ImageHelper.SetImageGray(roleIcon, false);
                    }
                    else
                    {
                        ImageHelper.SetImageGray(roleIcon, true);
                    }
                }
                TextHelper.SetText(roleName, Sys_Society.Instance.socialRolesInfo.rolesDic[_roleChatData.roleID].roleName);

                TextHelper.SetText(content, roleChatData.content, CSVWordStyle.Instance.GetConfData(CSVChatWord.Instance.GetConfData(_roleChatData.chatTextID).WordIcon));

                ResetRootSize();
            }

            void ResetRootSize()
            {
                ImageFitterText imageFitterText = contentBg.GetComponent<ImageFitterText>();
                imageFitterText.SetIsSingleLine();
                imageFitterText.Refresh();
                if (contentBg.sizeDelta.y + 30 < 75)
                {
                    rootRect.sizeDelta = new Vector2(rootRect.sizeDelta.x, 70);
                    sizeFitter.horizontalFit = ContentSizeFitter.FitMode.PreferredSize;
                }
                else
                {
                    rootRect.sizeDelta = new Vector2(rootRect.sizeDelta.x, contentBg.sizeDelta.y + 45);
                    sizeFitter.horizontalFit = ContentSizeFitter.FitMode.Unconstrained;
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

                    if (roleChatData.GetChatExtMsg() == null || roleChatData.GetChatExtMsg().Item == null || roleChatData.GetChatExtMsg().Item.Count <= (int)id)
                        return;

                    Packet.ItemCommonData itemData = roleChatData.GetChatExtMsg().Item[(int)id];

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
                            petEquipTipsData.petEquip = new ItemData(itemData.BoxId, itemData.Uuid, itemData.Id, itemData.Count, 0u, false, false, null, null, 0, null, null, null, itemData.PetEquip);
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
                    uint.TryParse(param[0], out cross);
                    uint.TryParse(param[1], out showType);

                    Sys_Trade.TelData telData = new Sys_Trade.TelData();
                    telData.telType = 0u;
                    telData.bCross = cross == 1u;
                    telData.tradeShowType = (TradeShowType)showType;
                    telData.itemInfoId = (uint)id;

                    if (Sys_FamilyResBattle.Instance.InFamilyBattle)
                    {
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
                    Sys_Role_Info.Instance.OpenRoleInfo(id, Sys_Role_Info.EType.Chat);
                }
                else if (type == (int)ERichType.Achievement)
                {
                    int.TryParse(v, out int index);
                    if (roleChatData.GetChatExtMsg() == null || roleChatData.GetChatExtMsg().AchData == null || roleChatData.GetChatExtMsg().AchData.Count <= index)
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
                        AchievementData achData = roleChatData.GetChatExtMsg().AchData[index];
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
        }
    }
}
