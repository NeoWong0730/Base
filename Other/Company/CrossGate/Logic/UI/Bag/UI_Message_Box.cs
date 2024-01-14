using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using Table;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;
using static Logic.Sys_Equip;

namespace Logic
{
    public class UI_Message_Box : UIBase
    {
        private PropIconLoader.ShowItemData mItemData;
        private CSVItem.Data cSVItem;
        private MessageBoxEvt boxEvt;
        private uint sourceUiID;
        
        private Image imgSkillBook;
        private Image mIcon;
        private Text mItemName;
        private Text mItemContent;
        private Text mItemContent_WorldView;
        private GameObject mLine;
        private Text mItemLevel;
        private Text mItemType;
        private Text mItem_CanDeal;
        private Text mItem_Bind;
        private Image mQuality;
        private RawImage mQualityBG;
        private Image runeLevelImage;
        private GameObject mSourceViewRoot;
        private GameObject mItemViewRoot;
        
        private ItemSource m_ItemSource;
        private bool bSourceActive;
        private Button mItemSourceButton;

        private GameObject mItemChangeCardAttrRoot;
        private GameObject mItemChangeCardAttrItem;
        private Text mItemChangeCardTime;
        private Text mItemChangeCardLv;

        private RectTransform rightBgRect;
        private bool changePos = false;
        protected override void OnOpen(object arg)
        {
            boxEvt = arg as MessageBoxEvt;
            if (boxEvt != null)
            {
                mItemData = boxEvt.itemData;
                cSVItem = CSVItem.Instance.GetConfData(mItemData.id);
                sourceUiID = (uint)boxEvt.sourceUiId;
            }
            else
            {
                DebugUtil.LogErrorFormat("messageboxEvt解析错误");
            }
        }

        protected override void OnLoaded()
        {
            mItemName = transform.Find("Animator/View_Message/Text_Name").GetComponent<Text>();
            mItemLevel = transform.Find("Animator/View_Message/Text_Level").GetComponent<Text>();
            mItemType = transform.Find("Animator/View_Message/Text_Type").GetComponent<Text>();
            mItem_CanDeal = transform.Find("Animator/View_Message/Text_Can_Deal").GetComponent<Text>();
            mItem_Bind = transform.Find("Animator/View_Message/Text_Bound").GetComponent<Text>();
            mItemContent = transform.Find("Animator/View_Message/Image_BG/Text_Ccontent2").GetComponent<Text>();
            mItemContent_WorldView = transform.Find("Animator/View_Message/Image_BG/Text_Ccontent").GetComponent<Text>();
            mLine = transform.Find("Animator/View_Message/Image_BG/Image_Line").gameObject;
            mQualityBG = transform.Find("Animator/View_Message/Image_QualityBG").GetComponent<RawImage>();
            mQuality = transform.Find("Animator/View_Message/ListItem/Btn_Item/Image_Quality").GetComponent<Image>();
            mIcon = transform.Find("Animator/View_Message/ListItem/Btn_Item/Image_Icon").GetComponent<Image>();
            imgSkillBook = transform.Find("Animator/View_Message/ListItem/Btn_Item/Image_Skill").GetComponent<Image>();
            runeLevelImage = transform.Find("Animator/View_Message/ListItem/Image_RuneRank")?.GetComponent<Image>();

            mItemSourceButton = transform.Find("Animator/View_Message/Image_BG/Button").GetComponent<Button>();
            mSourceViewRoot = transform.Find("Animator/View_Right").gameObject;
            rightBgRect = transform.Find("Animator/View_Right/Image_BG").GetComponent<RectTransform>();

            m_ItemSource = new ItemSource();
            m_ItemSource.BindGameObject(mSourceViewRoot);

            mItemViewRoot = transform.Find("Animator/View_Message").gameObject;

            mItemChangeCardAttrRoot= transform.Find("Animator/View_Message/Image_BG/View_Transfiguration").gameObject;
            mItemChangeCardAttrItem = transform.Find("Animator/View_Message/Image_BG/View_Transfiguration/View_Property/Item").gameObject;
            mItemChangeCardTime = transform.Find("Animator/View_Message/Image_BG/View_Transfiguration/Text_Time/Text").GetComponent<Text>();
            mItemChangeCardLv = transform.Find("Animator/View_Message/Image_BG/View_Transfiguration/Text_Lv/Text").GetComponent<Text>();

            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(transform.Find("ClickClose").gameObject);
            eventListener.AddEventListener(EventTriggerType.PointerClick, (_) => { UIManager.CloseUI(EUIID.UI_Message_Box); });

            mItemSourceButton.onClick.AddListener(ShowSourceItems);
        }

        protected override void OnShow()
        {
            mItemData.bShowBtnNo = false;
            if(null != boxEvt && boxEvt.b_changeSourcePos && !changePos)
            {
                changePos = true;
                var point = CameraManager.mUICamera.WorldToScreenPoint(boxEvt.pos);
                var rightRect = mSourceViewRoot.GetComponent<RectTransform>();
                RectTransformUtility.ScreenPointToLocalPointInRectangle(rightRect, point, CameraManager.mUICamera, out Vector2 uiPoint);
                mSourceViewRoot.transform.localPosition = new Vector3(uiPoint.x - rightBgRect.localPosition.x + rightRect.rect.width, mSourceViewRoot.transform.localPosition.y, 0);
            }
            UpdateInfoUi();
        }

        private void UpdateInfoUi()
        {
            mSourceViewRoot.SetActive(false);
            bSourceActive = m_ItemSource.SetData(mItemData.id, sourceUiID, EUIID.UI_Message_Box);

            if (boxEvt.b_ShowItemInfo)
            {
                ImageHelper.SetIcon(mIcon, cSVItem.icon_id);
                mQuality.gameObject.SetActive(true);
                uint tempQuality = 0u;
                if (mItemData.Quality == 0u)
                    tempQuality = (uint)cSVItem.quality;
                else
                    tempQuality = mItemData.Quality;
                if (cSVItem.quality == 0u)
                {
                    TextHelper.SetQuailtyText(mItemName, 1, CSVLanguage.Instance.GetConfData(cSVItem.name_id).words);
                }
                else
                {
                    TextHelper.SetQuailtyText(mItemName, tempQuality, CSVLanguage.Instance.GetConfData(cSVItem.name_id).words);
                }
                if (tempQuality == 0u)
                {
                    mQuality.gameObject.SetActive(false);
                    ImageHelper.SetBgQuality(mQualityBG, 1);
                }
                else
                {
                    ImageHelper.GetQualityColor_Frame(mQuality, (int)tempQuality);
                    ImageHelper.SetBgQuality(mQualityBG, tempQuality);
                }

                mLine.SetActive(cSVItem.world_view != 0 && cSVItem.describe_id != 0);
                if (cSVItem.world_view != 0)
                {
                    TextHelper.SetText(mItemContent_WorldView, cSVItem.world_view);
                    mItemContent_WorldView.gameObject.SetActive(true);
                }
                else
                {
                    TextHelper.SetText(mItemContent_WorldView, string.Empty);
                    mItemContent_WorldView.gameObject.SetActive(false);
                }
                if (cSVItem.describe_id != 0)
                {
                    TextHelper.SetText(mItemContent, cSVItem.describe_id);
                    mItemContent.gameObject.SetActive(true);
                }
                else
                {
                    TextHelper.SetText(mItemContent, string.Empty);
                    mItemContent.gameObject.SetActive(false);
                }

                if (cSVItem.lv == 0)
                {
                    mItemLevel.gameObject.SetActive(false);
                }
                else
                {
                    mItemLevel.gameObject.SetActive(true);
                    TextHelper.SetText(mItemLevel, string.Format(CSVLanguage.Instance.GetConfData(2007412).words, cSVItem.lv));
                }
                TextHelper.SetText(mItemType, string.Format(CSVLanguage.Instance.GetConfData(2007413).words, CSVLanguage.Instance.GetConfData(cSVItem.type_name).words));
                mItemSourceButton.gameObject.SetActive(bSourceActive);
                //技能书等级特殊显示
                Sys_Skill.Instance.ShowPetSkillBook(imgSkillBook, cSVItem);
                RefreshPartner();
            }

            mItemViewRoot.SetActive(boxEvt.b_ShowItemInfo);
            if (boxEvt.b_ForceShowScource)
            {
                mItemSourceButton.gameObject.SetActive(true);
                m_ItemSource.Show();
            }

            if (cSVItem.type_id == (uint)EItemType.ChangeCard)
            {
                mItemChangeCardAttrRoot.SetActive(true);
                mItemType.gameObject.SetActive(true);
                mItemContent.gameObject.SetActive(false);
                if (CSVRaceChangeCard.Instance.TryGetValue(cSVItem.id, out CSVRaceChangeCard.Data curCSVRaceChangeCardData))
                {
                    CSVRaceDepartmentResearch.Data csvRaceDepartmentResearchData = CSVRaceDepartmentResearch.Instance.GetConfData(curCSVRaceChangeCardData.need_race_lv);
                    CSVGenus.Data csvGenusData = CSVGenus.Instance.GetConfData(curCSVRaceChangeCardData.type);
                    TextHelper.SetText(mItemChangeCardTime, 10156, (curCSVRaceChangeCardData.last_time / 60).ToString());                           
                    TextHelper.SetText(mItemChangeCardLv, 2013017, LanguageHelper.GetTextContent(csvGenusData.rale_name), csvRaceDepartmentResearchData.rank.ToString(), csvRaceDepartmentResearchData.level.ToString());                    
                    SetChangeCardAttrView(curCSVRaceChangeCardData);
                }
            }
            else
            {
                mItemChangeCardAttrRoot.SetActive(false);
                mItemContent.gameObject.SetActive(true);
            }
        }

        public void RefreshPartner()
        {
            CSVRuneInfo.Data runeInfo = CSVRuneInfo.Instance.GetConfData(cSVItem.id);
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

        private void ShowSourceItems()
        {
            m_ItemSource.Show();
        }

        private void SetChangeCardAttrView(CSVRaceChangeCard.Data curCSVRaceChangeCardData)
        {
            FrameworkTool.DestroyChildren(mItemChangeCardAttrItem.transform.parent.gameObject, mItemChangeCardAttrItem.name);
            FrameworkTool.CreateChildList(mItemChangeCardAttrItem.transform.parent, curCSVRaceChangeCardData.base_attr.Count);
            for (int i = 0; i < mItemChangeCardAttrItem.transform.parent.childCount; ++i)
            {
                int attrId = curCSVRaceChangeCardData.base_attr[i][0];
                Transform trans = mItemChangeCardAttrItem.transform.parent.GetChild(i).transform;
                if (i != 0)
                {
                    trans.name = attrId.ToString();
                }              
                Text attrName = trans.Find("Text_Name").GetComponent<Text>();
                Text attrValue = trans.Find("Text_Value").GetComponent<Text>();
 
                CSVAttr.Data csvAttrData = CSVAttr.Instance.GetConfData((uint)attrId);
                if (csvAttrData != null)
                {
                    TextHelper.SetText(attrName, csvAttrData.name);
                    float number = curCSVRaceChangeCardData.base_attr[i][1];
                    if (number > 0)
                    {
                        if (csvAttrData.show_type == 1)
                        {
                            attrValue.text = "+" + number.ToString();
                        }
                        else
                        {
                            number = (float)number / 100;
                            attrValue.text = "+" + number.ToString() + "%";
                        }
                    }
                    else
                    {
                        if (csvAttrData.show_type == 1)
                        {
                            attrValue.text = number.ToString();
                        }
                        else
                        {
                            number = number / 100;
                            attrValue.text = number.ToString() + "%";
                        }
                    }
                }
            }
        }
       
    }
}


