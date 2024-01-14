using Lib.Core;
using Logic.Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;
using Framework;
using Table;
using UnityEngine.EventSystems;
using Packet;
using System;
using System.Text;

namespace Logic
{
    public class PropMessageParam
    {
        public ItemData itemData;
        public bool needShowInfo;
        public bool needShowMarket;
        public bool showBtnCheck;
        public uint targetEUIID;
        public uint checkOpenParam;
        public EUIID sourceUiId;
        public bool showOpButton = true;
    }

    public partial class UI_Prop_Message : UIBase
    {
        private ItemData mItemData;

        private Image mIcon;
        private Image mQuality;
        private RawImage mQualityBG;
        private Text mCount;
        private GameObject mNewObj;
        private GameObject mBoundObj;
        private Image m_SkillBook;
        private Image runeLevelImage;

        private Text mItemName;
        private Text mItemContent;
        private Text mItemContent_WorldView;
        private GameObject mLine;
        private Text mItemLevel;
        private Text mItemType;
        private Text mItem_CanDeal;
        private Text mItem_Bind;

        private GameObject clock;
        private GameObject text_Lockdate;
        private Text Lockdate;

        private GameObject m_OutTimeObj;
        private Text m_OutTimeText;

        private Transform buttonParent;
        private List<GameObject> actionGameobjs = new List<GameObject>();
        private int ActionCount = 0;
        private List<uint> wordIds = new List<uint>();  //使用  批量使用  激活  出售  镶嵌
        private bool m_NeedShowInfo;
        private bool m_NeedShowMacket;

        private bool showBtnCheck = false;//查看按钮
        private uint showBtnTargetEUIID = 0;//查看按钮点击跳转的界面UID
        private uint checkOpenParam = 0;//查看按钮点击跳转的参数

        private PenetrationClick penetrationClick;

        private ItemSource m_ItemSource;
        private GameObject m_SourceViewRoot;
        private bool bSourceActive;
        private uint sourceUiID;
        private bool bShowOpButton;

        private bool b_OutTime;
        private int b_MapUseState;

        protected override void OnOpen(object arg)
        {
            if (arg.GetType() == typeof(PropMessageParam))
            {
                PropMessageParam param = arg as PropMessageParam;
                mItemData = param.itemData;
                m_NeedShowInfo = param.needShowInfo;
                m_NeedShowMacket = param.needShowMarket;
                showBtnCheck = param.showBtnCheck;
                showBtnTargetEUIID = param.targetEUIID;
                checkOpenParam = param.checkOpenParam;
                sourceUiID = (uint)param.sourceUiId;
                bShowOpButton = param.showOpButton;
            }
        }

        protected override void OnLoaded()
        {
            m_SourceViewRoot = transform.Find("Animator/View01/View_Right").gameObject;
            m_ItemSource = new ItemSource();
            m_ItemSource.BindGameObject(m_SourceViewRoot);

            mItemName = transform.Find("Animator/View01/View_Message/Text_Name").GetComponent<Text>();
            mIcon = transform.Find("Animator/View01/View_Message/ListItem/Btn_Item/Image_Icon").GetComponent<Image>();
            mQuality = transform.Find("Animator/View01/View_Message/ListItem/Btn_Item/Image_BG").GetComponent<Image>();
            mQualityBG = transform.Find("Animator/View01/View_Message/Image_QualityBG").GetComponent<RawImage>();
            m_SkillBook = transform.Find("Animator/View01/View_Message/ListItem/Btn_Item/Image_Skill").GetComponent<Image>();

            mCount = transform.Find("Animator/View01/View_Message/ListItem/Text_Number").GetComponent<Text>();
            mNewObj = transform.Find("Animator/View01/View_Message/ListItem/Text_New").gameObject;
            mBoundObj = transform.Find("Animator/View01/View_Message/ListItem/Text_Bound").gameObject;
            mItemLevel = transform.Find("Animator/View01/View_Message/Text_Level").GetComponent<Text>();
            mItemType = transform.Find("Animator/View01/View_Message/Text_Type").GetComponent<Text>();
            mItem_CanDeal = transform.Find("Animator/View01/View_Message/Text_Can_Deal").GetComponent<Text>();
            mItem_Bind = transform.Find("Animator/View01/View_Message/Text_Bound").GetComponent<Text>();
            mItemContent = transform.Find("Animator/View01/View_Message/Image_BG/Text_Ccontent2").GetComponent<Text>();
            mItemContent_WorldView = transform.Find("Animator/View01/View_Message/Image_BG/Text_Ccontent").GetComponent<Text>();
            mLine = transform.Find("Animator/View01/View_Message/Image_BG/Image_Line").gameObject;
            clock = transform.Find("Animator/View01/View_Message/ListItem/Image_Clock").gameObject;
            text_Lockdate = transform.Find("Animator/View01/View_Message/Image_BG/Text_Lockdate").gameObject;
            runeLevelImage = transform.Find("Animator/View01/View_Message/ListItem/Image_RuneRank")?.GetComponent<Image>();
            Lockdate = text_Lockdate.GetComponent<Text>();

            m_OutTimeObj = transform.Find("Animator/View01/View_Message/Image_BG/Lose").gameObject;
            m_OutTimeText = transform.Find("Animator/View01/View_Message/Image_BG/Lose/Text_Lockdate").GetComponent<Text>();

            buttonParent = transform.Find("Animator/View01/View_Button");

            string words = CSVParam.Instance.GetConfData(180).str_value;
            string[] str = words.Split('|');
            for (int i = 0; i < str.Length; i++)
            {
                wordIds.Add(uint.Parse(str[i]));
            }

            if (bShowOpButton)
            {
                BulidButtons();
            }

            //penetrationClick = transform.Find("ClickClose").GetComponent<PenetrationClick>();
            //penetrationClick.onCloseClick = OnPenetrationClick;
            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(transform.Find("ClickClose").gameObject);
            eventListener.AddEventListener(EventTriggerType.PointerClick, Close);

        }

        protected override void ProcessEvents(bool toRegister)
        {
            Sys_Bag.Instance.eventEmitter.Handle<PropMessageParam>(Sys_Bag.EEvents.OnUpdatePropItemMessageParm, OnUpdatePropItemMessageParm, toRegister);
        }

        private void OnUpdatePropItemMessageParm(PropMessageParam param)
        {
            mItemData = param.itemData;
            m_NeedShowInfo = param.needShowInfo;
            m_NeedShowMacket = param.needShowMarket;
            showBtnCheck = param.showBtnCheck;
            showBtnTargetEUIID = param.targetEUIID;
            checkOpenParam = param.checkOpenParam;
            UpdateUI();
        }

        private void Close(BaseEventData baseEventData)
        {
            if (Sys_Bag.Instance.enablePropMessagePenetrationClick)
            {
                return;
            }
            UIManager.CloseUI(EUIID.UI_Prop_Message);
        }

        private void OnPenetrationClick()
        {
            UIManager.CloseUI(EUIID.UI_Prop_Message);
        }

        private void BulidButtons()
        {
            int needCount = wordIds.Count;
            FrameworkTool.CreateChildList(buttonParent, needCount);
            for (int i = 0; i < buttonParent.childCount; i++)
            {
                actionGameobjs.Add(buttonParent.GetChild(i).gameObject);
            }
        }

        protected override void OnShow()
        {
            UpdateUI();
        }

        private void UpdateBanMapData()
        {
            b_MapUseState = Sys_Bag.Instance.GetItemMapUseState(mItemData);
        }

        private void UpdateUI()
        {
            UpdateBanMapData();

            ActionCount = 0;
            ProcessItemSource();
            UpdateInfoUi();
            UpdateMarketEndTime();
            UpdateOutTime();

            if (b_MapUseState == 0)
            {
                bShowOpButton = false;
            }
            if (bShowOpButton)
            {
                buttonParent.gameObject.SetActive(true);
                UpdateFuncBtn();
            }
            else
            {
                buttonParent.gameObject.SetActive(false);
            }
        }

        private void ProcessItemSource()
        {
            bSourceActive = m_ItemSource.SetData(mItemData.Id, sourceUiID, EUIID.UI_Prop_Message);
        }


        private void UpdateInfoUi()
        {
            mIcon.enabled = true;
            ImageHelper.SetIcon(mIcon, mItemData.cSVItemData.icon_id);
            Sys_Skill.Instance.ShowPetSkillBook(m_SkillBook, mItemData.cSVItemData);
            mNewObj.SetActive(mItemData.bNew);
            mBoundObj.SetActive(mItemData.bBind);
            CSVLanguage.Data cSVLanguageData = CSVLanguage.Instance.GetConfData(mItemData.cSVItemData.name_id);
            if (cSVLanguageData != null)
            {
                TextHelper.SetQuailtyText(mItemName, (uint)mItemData.cSVItemData.quality, CSVLanguage.Instance.GetConfData(mItemData.cSVItemData.name_id).words);
            }
            else
            {
                DebugUtil.LogErrorFormat("语言表找不到id={0}", mItemData.cSVItemData.name_id);
            }

            mLine.SetActive(mItemData.cSVItemData.world_view != 0 && mItemData.cSVItemData.describe_id != 0);
            if (mItemData.cSVItemData.world_view != 0)
            {
                TextHelper.SetText(mItemContent_WorldView, mItemData.cSVItemData.world_view);
                mItemContent_WorldView.gameObject.SetActive(true);
            }
            else
            {
                TextHelper.SetText(mItemContent_WorldView, string.Empty);
                mItemContent_WorldView.gameObject.SetActive(false);
            }
            if (mItemData.cSVItemData.describe_id != 0)
            {
                TextHelper.SetText(mItemContent, mItemData.cSVItemData.describe_id);
                mItemContent.gameObject.SetActive(true);
            }
            else
            {
                TextHelper.SetText(mItemContent, string.Empty);
                mItemContent.gameObject.SetActive(false);
            }

            TextHelper.SetText(mItemLevel, LanguageHelper.GetTextContent(2007412, mItemData.cSVItemData.lv.ToString()));
            //TextHelper.SetText(mItemType, string.Format(CSVLanguage.Instance.GetConfData(2007413).words, CSVLanguage.Instance.GetConfData(CSVItem.Instance.GetConfData(mItemData.Id).type_name).words));
            TextHelper.SetText(mItemType, LanguageHelper.GetTextContent(2007413, LanguageHelper.GetTextContent(mItemData.cSVItemData.type_name)));
            ImageHelper.GetQualityColor_Frame(mQuality, (int)mItemData.cSVItemData.quality);
            ImageHelper.SetBgQuality(mQualityBG, (uint)mItemData.cSVItemData.quality);

            clock.SetActive(!mItemData.bMarketEnd);
            text_Lockdate.SetActive(!mItemData.bMarketEnd);

            RefreshPartner();
        }

        public void RefreshPartner()
        {
            CSVRuneInfo.Data runeInfo = CSVRuneInfo.Instance.GetConfData((uint)mItemData.cSVItemData.id);
            if (null != runeInfo)
            {
                if (runeLevelImage != null)
                {
                    ImageHelper.SetIcon(runeLevelImage, Sys_Partner.Instance.GetRuneLevelImageId(runeInfo.rune_lvl));
                    runeLevelImage.gameObject.SetActive(true);
                }
            }
            else
            {
                runeLevelImage.gameObject.SetActive(false);
            }
        }

        private void UpdateFuncBtn()
        {
            foreach (var item in actionGameobjs)
            {
                item.SetActive(false);
            }
            if (m_NeedShowInfo)
            {
                if (mItemData.cSVItemData.scene_use == 1)
                {
                    ActionCount++;
                    actionGameobjs[ActionCount - 1].SetActive(true);
                    SetFunBtnWords(actionGameobjs[ActionCount - 1], wordIds[0]);
                    actionGameobjs[ActionCount - 1].GetComponent<Button>().onClick.AddListener(UseItem);
                }
                if (b_MapUseState == 1)         //只显示使用按钮
                {
                    return;
                }
                if (mItemData.Count > 1 && mItemData.cSVItemData.batch_use > 0)
                {
                    ActionCount++;
                    actionGameobjs[ActionCount - 1].SetActive(true);
                    SetFunBtnWords(actionGameobjs[ActionCount - 1], wordIds[1]);
                    actionGameobjs[ActionCount - 1].GetComponent<Button>().onClick.AddListener(UseAll);
                }
                if (mItemData.BoxId == 4)
                {
                    ActionCount++;
                    actionGameobjs[ActionCount - 1].SetActive(true);
                    SetFunBtnWords(actionGameobjs[ActionCount - 1], wordIds[2]);
                    actionGameobjs[ActionCount - 1].GetComponent<Button>().onClick.AddListener(Active);
                }
                if (mItemData.cSVItemData.sell_price > 0)
                {
                    ActionCount++;
                    actionGameobjs[ActionCount - 1].SetActive(true);
                    SetFunBtnWords(actionGameobjs[ActionCount - 1], wordIds[3]);
                    actionGameobjs[ActionCount - 1].GetComponent<Button>().onClick.AddListener(Sale);
                }
                if (mItemData.cSVItemData.type_id == 1101 || mItemData.cSVItemData.type_id == 1102)
                {
                    ActionCount++;
                    actionGameobjs[ActionCount - 1].SetActive(true);
                    SetFunBtnWords(actionGameobjs[ActionCount - 1], wordIds[4]);
                    actionGameobjs[ActionCount - 1].GetComponent<Button>().onClick.AddListener(Mosaic);
                }
                if (mItemData.cSVItemData.type_id == 11000)
                {
                    if (mItemData.cSVItemData.undo_item != null && b_OutTime)
                    {
                        ActionCount++;
                        actionGameobjs[ActionCount - 1].SetActive(true);
                        SetFunBtnWords(actionGameobjs[ActionCount - 1], wordIds[5]);
                        actionGameobjs[ActionCount - 1].GetComponent<Button>().onClick.AddListener(UnDo);
                    }
                }
                else
                {
                    if (mItemData.cSVItemData.undo_item != null)
                    {
                        ActionCount++;
                        actionGameobjs[ActionCount - 1].SetActive(true);
                        SetFunBtnWords(actionGameobjs[ActionCount - 1], wordIds[5]);
                        actionGameobjs[ActionCount - 1].GetComponent<Button>().onClick.AddListener(UnDo);
                    }
                }
                if (mItemData.cSVItemData.type_id != 11000 && (mItemData.cSVItemData.del_item == 1 || b_OutTime))
                {
                    ActionCount++;
                    actionGameobjs[ActionCount - 1].SetActive(true);
                    SetFunBtnWords(actionGameobjs[ActionCount - 1], wordIds[6]);
                    actionGameobjs[ActionCount - 1].GetComponent<Button>().onClick.AddListener(Del);
                }
                if (mItemData.cSVItemData.on_sale/* && !Sys_FamilyResBattle.Instance.InFamilyBattle*/)
                {
                    ActionCount++;
                    actionGameobjs[ActionCount - 1].SetActive(true);
                    SetFunBtnWords(actionGameobjs[ActionCount - 1], wordIds[7]);
                    actionGameobjs[ActionCount - 1].GetComponent<Button>().onClick.AddListener(OnSale);
                }
                if (mItemData.cSVItemData.compose != 0)
                {
                    ActionCount++;
                    actionGameobjs[ActionCount - 1].SetActive(true);
                    SetFunBtnWords(actionGameobjs[ActionCount - 1], wordIds[8]);
                    actionGameobjs[ActionCount - 1].GetComponent<Button>().onClick.AddListener(OnCompose);
                }
                if (mItemData.cSVItemData.PresentType != 0)
                {
                    ActionCount++;
                    actionGameobjs[ActionCount - 1].SetActive(true);
                    SetFunBtnWords(actionGameobjs[ActionCount - 1], wordIds[10]);
                    actionGameobjs[ActionCount - 1].GetComponent<Button>().onClick.AddListener(OnPresent);
                }
                if (mItemData.cSVItemData.ItemSource != 0 && bSourceActive)
                {
                    ActionCount++;
                    actionGameobjs[ActionCount - 1].SetActive(true);
                    SetFunBtnWords(actionGameobjs[ActionCount - 1], wordIds[11]);
                    actionGameobjs[ActionCount - 1].GetComponent<Button>().onClick.AddListener(OnShowItemSource);
                }
                if (mItemData.cSVItemData.composed != 0)
                {
                    ActionCount++;
                    actionGameobjs[ActionCount - 1].SetActive(true);
                    SetFunBtnWords(actionGameobjs[ActionCount - 1], wordIds[12]);
                    actionGameobjs[ActionCount - 1].GetComponent<Button>().onClick.AddListener(OnComposed);
                }
                if (mItemData.cSVItemData.appraisal != null)
                {
                    ActionCount++;
                    actionGameobjs[ActionCount - 1].SetActive(true);
                    SetFunBtnWords(actionGameobjs[ActionCount - 1], wordIds[13]);
                    actionGameobjs[ActionCount - 1].GetComponent<Button>().onClick.AddListener(OnAppraisal);
                }
            }
            if (showBtnCheck)
            {
                ActionCount++;
                actionGameobjs[ActionCount - 1].SetActive(true);
                SetFunBtnWords(actionGameobjs[ActionCount - 1], wordIds[9]);
                actionGameobjs[ActionCount - 1].GetComponent<Button>().onClick.AddListener(OnCheck);
            }
        }

        protected override void OnClose()
        {
            foreach (var item in actionGameobjs)
            {
                item.GetComponent<Button>()?.onClick.RemoveAllListeners();
            }
        }

        private void SetFunBtnWords(GameObject gameObject, uint wordId)
        {
            TextHelper.SetText(gameObject.transform.Find("Text").GetComponent<Text>(), wordId);
        }


        private void UseItem()
        {
            if (Sys_Bag.Instance.UseItem(mItemData))
            {
                UIManager.CloseUI(EUIID.UI_Prop_Message, true);
            }
        }

        private bool CheckFuncOpenUI(uint funcId)
        {
            CSVCheckseq.Data cSVCheckseqData = CSVCheckseq.Instance.GetConfData(funcId);
            return cSVCheckseqData.IsValid();
        }

        private void UseAll()
        {
            if (Sys_Role.Instance.Role.Level >= mItemData.cSVItemData.use_lv)
            {
                UIManager.OpenUI(EUIID.UI_BatchUse_Box, false, mItemData);
            }
            else
            {
                string content = string.Format(CSVLanguage.Instance.GetConfData(101445).words, mItemData.cSVItemData.use_lv);
                Sys_Hint.Instance.PushContent_Normal(content);
            }
            UIManager.CloseUI(EUIID.UI_Prop_Message);
        }

        private void Active()
        {
            uint petId = mItemData.cSVItemData.fun_value[1];
            bool active = Sys_Pet.Instance.GetPetIsActive(petId);
            if (!active)
            {
                UIManager.OpenUI(EUIID.UI_Eraser, false, new Tuple<uint, object>(0, mItemData.cSVItemData.fun_value[2]));
            }
            else
            {
                if (mItemData.cSVItemData.fun_parameter == "petCard")
                {
                    if (!Sys_FunctionOpen.Instance.IsOpen(10545, true))//宠物功能开启条件
                        return;
                    if (mItemData.cSVItemData.fun_value.Count >= 2)
                    {
                        uint uiid = mItemData.cSVItemData.fun_value[0];
                        PetBookListPar petBookListPar = new PetBookListPar();
                        petBookListPar.petId = petId;
                        if (Sys_Pet.Instance.GetPetIsActive(petId))
                        {
                            petBookListPar.eviewType = EPetReviewViewType.Friend;
                            petBookListPar.ePetReviewPageType = EPetBookPageType.Friend;
                        }
                        else
                        {
                            petBookListPar.eviewType = EPetReviewViewType.Book;
                            petBookListPar.ePetReviewPageType = EPetBookPageType.Seal;
                        }
                        UIManager.OpenUI((EUIID)uiid, false, petBookListPar);
                    }
                }
            }
            UIManager.CloseUI(EUIID.UI_Prop_Message);
        }

        //出售
        private void Sale()
        {
            if (!Sys_FunctionOpen.Instance.IsOpen(51201, true))
            {
                return;
            }
            UIManager.CloseUI(EUIID.UI_Prop_Message);
            UIManager.OpenUI(EUIID.UI_Sale_Prop, false, mItemData);
        }

        private void Mosaic()
        {
            UIManager.CloseUI(EUIID.UI_Prop_Message);

            if (!Sys_FunctionOpen.Instance.IsOpen(10300, true))
                return;

            UIManager.OpenUI(EUIID.UI_Equipment);
        }

        //分解
        private void UnDo()
        {
            UIManager.CloseUI(EUIID.UI_Prop_Message);
            UIManager.OpenUI(EUIID.UI_Decompose, false, mItemData);
        }

        //丢弃
        private void Del()
        {
            if (mItemData.cSVItemData.DoubleCheck == 0)
            {
                Sys_Bag.Instance.OnDisCardItemReq(mItemData.Uuid);
                UIManager.CloseUI(EUIID.UI_Prop_Message);
            }
            else
            {
                PromptBoxParameter.Instance.Clear();
                string content1 = CSVLanguage.Instance.GetConfData(mItemData.cSVItemData.DoubleCheckID).words;
                PromptBoxParameter.Instance.content = content1;
                PromptBoxParameter.Instance.SetConfirm(true, () =>
                {
                    UIManager.CloseUI(EUIID.UI_Prop_Message);
                    Sys_Bag.Instance.OnDisCardItemReq(mItemData.Uuid);
                });
                PromptBoxParameter.Instance.SetCancel(true, null);
                UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
            }
        }

        //交易（上架）
        private void OnSale()
        {
            if (!Sys_FunctionOpen.Instance.IsOpen(40201, true))
                return;

            mItemData.marketendTimer.UpdateReMainMarkendTime();
            if (mItemData.bBind)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011185));
            }
            else if (mItemData.marketendTimer.foreverMarket)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011281));
            }
            else if (mItemData.marketendTimer.remainTime > 0)
            {
                uint left = mItemData.marketendTimer.remainTime % 86400;
                uint day = mItemData.marketendTimer.remainTime / 86400;
                if (left != 0)
                    day = day < 1 ? 1 : day + 1;
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011150, day.ToString()));
                //Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011150, LanguageHelper.TimeToString(mItemData.marketendTimer.remainTime, LanguageHelper.TimeFormat.Type_4)));
                //Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(2011150, day.ToString()));
            }
            else
            {
                Sys_Trade.Instance.SaleItem(mItemData);
                this.CloseSelf();
            }
        }

        //道具合成
        private void OnCompose()
        {
            if (mItemData.cSVItemData.type_id == (uint)EItemType.Jewel)//宝石
            {
                UIManager.CloseUI(EUIID.UI_Prop_Message);
                UIManager.OpenUI(EUIID.UI_JewelCompound, false, mItemData);
            }
            else
            {
                UIManager.CloseUI(EUIID.UI_Prop_Message);
                UIManager.OpenUI(EUIID.UI_Compose, false, mItemData.cSVItemData.compose);
            }
        }

        private void OnComposed()
        {
            UIManager.CloseUI(EUIID.UI_Prop_Message);
            UIManager.OpenUI(EUIID.UI_Compose, false, mItemData.cSVItemData.composed);
        }

        private void OnAppraisal()
        {
            uint appraisalId = mItemData.cSVItemData.appraisal[0];
            uint count = mItemData.cSVItemData.appraisal[1];

            CSVItem.Data cSVItemData = CSVItem.Instance.GetConfData(appraisalId);
            if (cSVItemData != null)
            {
                string name = LanguageHelper.GetTextContent(cSVItemData.name_id);
                string content = LanguageHelper.GetTextContent(1012015, name, count.ToString());
                PromptBoxParameter.Instance.OpenPromptBox(content, 0, () =>
                {
                    if (Sys_Bag.Instance.GetItemCount(appraisalId) < count)
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(1012016));
                        return;
                    }
                    Sys_Bag.Instance.RefreshItemReq(mItemData.Uuid);
                });
            }
            UIManager.CloseUI(EUIID.UI_Prop_Message);
        }

        //点击赠送
        private void OnPresent()
        {
            UIManager.OpenUI(EUIID.UI_SendGift, false, new List<ulong>() { 0, 0, mItemData.Id });
        }

        private void OnShowItemSource()
        {
            m_ItemSource.Show();
        }

        //查看
        private void OnCheck()
        {
            if (showBtnTargetEUIID > 0)
            {
                UIManager.OpenUI((int)showBtnTargetEUIID, false, checkOpenParam);
                UIManager.CloseUI(EUIID.UI_Prop_Message);
            }
        }
        private void UpdateMarketEndTime()
        {
            if (m_NeedShowMacket && !mItemData.bMarketEnd)
            {
                if (mItemData.MarketendTime == -1)
                {
                    TextHelper.SetText(Lockdate, 1009002);
                }
                else
                {
                    Lockdate.text = mItemData.marketendTimer.GetMarkendTimeFormat();
                }
            }
            else
            {
                clock.SetActive(false);
            }
        }

        private void UpdateOutTime()
        {
            if (mItemData.outTime == 0)
            {
                m_OutTimeObj.SetActive(false);
                b_OutTime = false;
            }
            else
            {
                m_OutTimeObj.SetActive(true);
                uint curTime = Sys_Time.Instance.GetServerTime();
                if (mItemData.outTime > curTime)
                {
                    DateTime dateTime = TimeManager.GetDateTime(mItemData.outTime);

                    string minuteStr = GetTimeFormat(dateTime.Minute);

                    string secondStr = GetTimeFormat(dateTime.Second);

                    TextHelper.SetText(m_OutTimeText, LanguageHelper.GetTextContent(591000652, dateTime.Year.ToString(), dateTime.Month.ToString(), dateTime.Day.ToString(),
                        dateTime.Hour.ToString(), minuteStr, secondStr));
                    b_OutTime = false;
                }
                else
                {
                    TextHelper.SetText(m_OutTimeText, 591000653);
                    b_OutTime = true;
                }
            }

        }


        private string GetTimeFormat(int value)
        {
            string zero = "0";
            string tp = string.Empty;
            StringBuilder sb = StringBuilderPool.GetTemporary();
            if (value < 10)
            {
                sb.Append(zero);
                sb.Append(value.ToString());
                tp = StringBuilderPool.ReleaseTemporaryAndToString(sb);
            }
            else
            {
                tp = value.ToString();
            }

            return tp;
        }
    }
}


