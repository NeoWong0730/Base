using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using Logic.Core;
using Table;
using UnityEngine.EventSystems;

namespace Logic
{
    public class PropItem
    {
        public Transform transform;

        public Item0_Layout Layout;

        public Image imgSkillBook;
        //public Text textSkillBook;

        public GameObject imgLock;
        public GameObject imgLike;
        public GameObject imgHealth;
        public Image imgSelect;
        public Image imgEquiped;
        public Image imgAdd;
        public Button btnNone;
        public Text txtNumber;
        public Text txtNumberZero;
        public GameObject txtNew;
        public Text txtBind;
        public Text txtName;
        public Text idText;
        public GameObject addGo;
        public GameObject gotGo;
        public GameObject clock;//禁售期
        private Image runeLevelImage;
        private GameObject m_FxBreathing;//呼吸灯

        public UI_LongPressButton longPressButton;

        public PropIconLoader.ShowItemData ItemData { get; private set; }
        public MessageBoxEvt boxEvt = new MessageBoxEvt();

        private Image btnImage;

        private bool bLongPress = false;
        private Action onLongPressed;

        public void BindGameObject(GameObject go)
        {
            transform = go.transform;

            Layout = new Item0_Layout();
            Layout.BindGameObject(transform.Find("Btn_Item").gameObject);

            imgSkillBook = transform.Find("Btn_Item/Image_Skill").GetComponent<Image>();
            //textSkillBook = transform.Find("Btn_Item/Image_Skill/Text_Num").GetComponent<Text>();

            imgLock = transform.Find("Image_Lock").gameObject;
            imgLike = transform.Find("Image_Like")?.gameObject;
            imgHealth = transform.Find("Image_Medicine")?.gameObject;
            imgSelect = transform.Find("Image_Select").GetComponent<Image>();
            imgEquiped = transform.Find("Image_Equiped").GetComponent<Image>();
            imgAdd = transform.Find("Image_Add").GetComponent<Image>();
            btnNone = transform.Find("Btn_None").GetComponent<Button>();
            txtNumber = transform.Find("Text_Number").GetComponent<Text>();
            txtNumberZero = transform.Find("Text_Number_Zero").GetComponent<Text>();
            txtNew = transform.Find("Text_New").gameObject;
            txtBind = transform.Find("Text_Bound").GetComponent<Text>();
            txtName = transform.Find("Text_Name").GetComponent<Text>();
            idText = transform.Find("Id")?.GetComponent<Text>();
            runeLevelImage = transform.Find("Image_RuneRank")?.GetComponent<Image>();
            m_FxBreathing = transform.Find("Fx_ui_PropItem01").gameObject;

            gotGo = transform.Find("Btn_Get").gameObject;
            clock = transform.Find("Image_Clock").gameObject;
            btnImage = transform.Find("Btn_Item/Image_BG").GetComponent<Image>();
            addGo = transform.Find("Image_Add").gameObject;
            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(btnImage.gameObject);
            eventListener.ClearEvents();
            eventListener.AddEventListener(EventTriggerType.PointerClick, (ret) => { OnClick(); });

            longPressButton = btnImage.gameObject.AddComponent<UI_LongPressButton>();
            longPressButton.onStartPress.AddListener(OnLongPressed);
        }

        public void SetData(PropIconLoader.ShowItemData itemData, EUIID uiId)
        {
            imgAdd.gameObject.SetActive(false);
            btnNone.gameObject.SetActive(false);

            this.ItemData = itemData;
            this.boxEvt.Reset(uiId, itemData);
            this.Refresh();
        }
        public uint FinalQuality(CSVItem.Data csv)
        {
            uint tempQuality = 0u;
            uint typeId = csv.type_id;
            if (typeId == (uint)EItemType.Equipment && ItemData.EquipPara != 0)
            {
                ItemData.SetQuality(Sys_Equip.Instance.CalPreviewQuality(ItemData.EquipPara));
            }
            else if(typeId == (uint)EItemType.PetEquipment)
            {
                ItemData.SetQuality(ItemData.EquipPara);
            }

            if (ItemData.Quality == 0u)
                tempQuality = csv.quality;
            else
                tempQuality = ItemData.Quality;

            ItemData.SetQuality(tempQuality);
            return tempQuality;
        }
        public void Refresh()
        {
            var csv = CSVItem.Instance.GetConfData(ItemData.id);
            if (csv != null)
            {
                Layout.imgIcon.gameObject.SetActive(true);
                ImageHelper.SetIcon(Layout.imgIcon, csv.icon_id);
                ImageHelper.SetImgAlpha(Layout.imgIcon, 1f);

                if (ItemData.bUseQuailty)
                {
                    Layout.imgQuality.gameObject.SetActive(true);

                    //设置quality
                    uint tempQuality = FinalQuality(csv);
                    if (tempQuality == 0u)
                    {
                        Layout.imgQuality.gameObject.SetActive(false);
                    }
                    else
                    {
                        ImageHelper.GetQualityColor_Frame(Layout.imgQuality, (int)tempQuality);
                    }
                }
                else
                {
                    Layout.imgQuality.gameObject.SetActive(false);
                }

                long right = ItemData.count;
                txtNumber.gameObject.SetActive(ItemData.bShowCount);

                if (ItemData.bShowBagCount)
                {
                    if (ItemData.bCopyBagGridCount)
                    {
                        txtNumber.text = ItemData.bagGridCount.ToString();
                    }
                    else
                    {
                        long left;
                        if (ItemData.bSetOtherCount)
                        {
                            left = ItemData.otherCount;
                        }
                        else
                        {
                            left = Sys_Bag.Instance.GetItemCount(ItemData.id);
                        }
                        //uint contentId = 0;
                        uint worldStyleId = 0;
                        string content = string.Empty;
                        if (left < right)
                        {
                            //contentId = 1601000004;
                            worldStyleId = 84;
                            if (ItemData.bShowBtnNo)
                            {
                                btnNone.gameObject.SetActive(true);
                                ImageHelper.SetImgAlpha(Layout.imgIcon, 0.5f);
                            }
                        }
                        else
                        {
                            //contentId = 1601000005;
                            worldStyleId = 83;
                        }
                        if (ItemData.id <= (uint)ECurrencyType.SilverCoin)
                        {
                            content = string.Format("{0}/{1}", Sys_Bag.Instance.GetValueFormat(left), right.ToString());
                        }
                        else
                        {
                            content = string.Format("{0}/{1}", left.ToString(), right.ToString());
                        }
                        TextHelper.SetText(txtNumber, content, CSVWordStyle.Instance.GetConfData(worldStyleId));
                    }
                }
                else
                {
                    if (ItemData.id <= (uint)ECurrencyType.SilverCoin)
                    {
                        txtNumber.text = Sys_Bag.Instance.GetValueFormat(right);
                    }
                    else
                    {
                        txtNumber.text = right.ToString();
                    }
                }
                imgSelect.gameObject.SetActive(ItemData.bSelected);
                imgLock.SetActive(ItemData.bUnLock);
                txtNew.SetActive(ItemData.bNew);
                txtBind.gameObject.SetActive(ItemData.bBind);
                if (ItemData.bBind)
                {
                    clock.SetActive(false);     //绑定情况下 一定不显示禁售期锁
                }
                else
                {
                    clock.SetActive(!ItemData.bMarketedEnd);
                }

                //技能书等级特殊显示
                Sys_Skill.Instance.ShowPetSkillBook(imgSkillBook, csv);

                RefreshPartner();
            }
            else
            {
                //当作为添加不定道具格时，空格状态强制隐藏
                if (imgSkillBook.gameObject.activeSelf)
                    imgSkillBook.gameObject.SetActive(false);
            }

            if (idText != null)
            {
#if UNITY_EDITOR
                idText.text = ItemData.id.ToString();
#else
            idText.text = " ";
#endif
            }
        }

        public void RefreshPartner()
        {
            CSVRuneInfo.Data runeInfo = CSVRuneInfo.Instance.GetConfData(ItemData.id);
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

        public void BtnBoneShow(bool enable)
        {
            btnNone.gameObject.SetActive(enable);
        }

        public void SetSelected(bool toSelect)
        {
            imgSelect.gameObject.SetActive(toSelect);
        }
        public void SetGot(bool toGot)
        {
            if (gotGo != null)
            {
                gotGo.SetActive(toGot);
            }
        }

        public void SetBreathing(bool breath)
        {
            if (m_FxBreathing != null && breath != m_FxBreathing.activeSelf) 
            {
                m_FxBreathing.SetActive(breath);
            }    
        }
        
        public void RefreshLRCount(long left, long right)
        {
            txtNumber.gameObject.SetActive(true);
            uint contentId = 0;
            if (left < right)
            {
                contentId = 1601000004;
                if (ItemData.bShowBtnNo)
                {
                    btnNone.gameObject.SetActive(true);
                    ImageHelper.SetImgAlpha(Layout.imgIcon, 0.5f);
                }
            }
            else
            {
                contentId = 1601000005;
            }

            txtNumber.text = string.Format(LanguageHelper.GetTextContent(contentId), left.ToString(), right.ToString());
        }

        public void RefreshCount(long count)
        {
            txtNumber.gameObject.SetActive(true);
            txtNumber.text = count.ToString();
        }

        public void SetData(MessageBoxEvt _boxEvt)
        {
            SetData(_boxEvt.itemData, _boxEvt.sourceUiId);
        }

        public void SetEmpty(bool resetData = true)
        {
            if (resetData) {
                ItemData?.Reset();
            }
            btnNone.gameObject.SetActive(false);
            Layout.imgIcon.gameObject.SetActive(false);
            Layout.imgQuality.gameObject.SetActive(false);
            txtNumber.gameObject.SetActive(false);
            imgSelect.gameObject.SetActive(false);
            imgLock.SetActive(false);
            txtNew.SetActive(false);
            txtBind.gameObject.SetActive(false);
            clock.SetActive(false);
            imgLike?.SetActive(false);
            imgHealth?.SetActive(false);
            imgSkillBook.gameObject.SetActive(false);
            idText.text = " ";
        }

        private void OnClick()
        {
            if (ItemData == null)
                return;
            if (ItemData.bUseClick)
            {
                if (ItemData.bUseTips && !bLongPress)
                {
                    UIManager.OpenUI(EUIID.UI_Message_Box, false, boxEvt);
                }

                ItemData.onclick?.Invoke(this);
            }
        }

        public void EnableLongPress(bool toEnable)
        {
            if (longPressButton != null)
            {
                longPressButton.enabled = toEnable;
            }
        }
        private void OnLongPressed()
        {
            if (bLongPress)
            {
                //open tip
                UIManager.OpenUI(EUIID.UI_Message_Box, false, boxEvt);
                onLongPressed?.Invoke();
            }
        }

        public void OnEnableLongPress(bool enable, Action longPressed = null)
        {
            bLongPress = enable;
            onLongPressed = longPressed;
        }

        public void SetActive(bool active)
        {
            transform.gameObject.SetActive(active);
        }

        public static void OnClickPropItem(PropItem item)
        {
            ItemData mItemData = new ItemData(0, 0, item.ItemData.id, (uint)item.ItemData.count, 0, false, false, null, null, 0);
            mItemData.EquipParam = item.ItemData.EquipPara;
            // mItemData.SetQuality(item.ItemData.Quality);
            uint typeId = mItemData.cSVItemData.type_id;
            if (typeId == (uint)EItemType.Equipment)
            {
                UIManager.OpenUI(EUIID.UI_Equipment_Preview, false, mItemData);
            }
            else if (typeId == (uint)EItemType.Crystal)
            {
                CrystalTipsData crystalTipsData = new CrystalTipsData();
                crystalTipsData.itemData = mItemData;
                crystalTipsData.bShowOp = false;
                crystalTipsData.bShowCompare = false;
                crystalTipsData.bShowDrop = false;
                crystalTipsData.bShowSale = false;
                UIManager.OpenUI(EUIID.UI_Tips_ElementalCrystal, false, crystalTipsData);
            }
            else if (typeId == (uint)EItemType.Ornament)
            {
                OrnamentTipsData tipData = new OrnamentTipsData();
                tipData.equip = mItemData;
                tipData.isShowOpBtn = false;
                tipData.isCompare = false;
                tipData.sourceUiId = item.boxEvt.sourceUiId;
                UIManager.OpenUI(EUIID.UI_Tips_Ornament, false, tipData);
            }
            else
            {
                PropMessageParam propParam = new PropMessageParam();
                propParam.itemData = mItemData;
                propParam.showBtnCheck = false;
                propParam.sourceUiId = item.boxEvt.sourceUiId;
                UIManager.OpenUI(EUIID.UI_Prop_Message, false, propParam);
            }
        }
    }
}

