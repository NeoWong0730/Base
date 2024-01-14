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
    public class PropSpecialItem
    {
        public Transform transform;

        public Item0_Layout Layout;
        public Image imgSkillBook;

        public GameObject imgLock;
        public Image imgSelect;
        public Image imgEquiped;
        public Image imgAdd;
        public Button btnNone;
        public Text txtNumber;
        public GameObject txtNew;
        public Text txtBind;
        public Text txtName;
        public Text idText;

        public GameObject levelGo;
        public Text levelText;

        public GameObject addGo;
        public GameObject gotGo;
        public GameObject clock;//禁售期

        public UI_LongPressButton longPressButton;

        public PropSpeicalLoader.ShowItemData showItemData { get; private set; }
        private Sys_Trade.TradeItemInfo m_itemInfo;
        public PropSpeicalLoader.MessageBoxEvt boxEvt = new PropSpeicalLoader.MessageBoxEvt();

        public Image btnImage;

        private bool bLongPress = false;
        private Action onLongPressed;

        public Sys_Trade.DetailSourceType detailSrcType = Sys_Trade.DetailSourceType.None;

        public void BindGameObject(GameObject go)
        {
            transform = go.transform;

            Layout = new Item0_Layout();
            Layout.BindGameObject(transform.Find("Btn_Item").gameObject);

            imgSkillBook = transform.Find("Btn_Item/Image_Skill").GetComponent<Image>();

            imgLock = transform.Find("Image_Lock").gameObject;
            imgSelect = transform.Find("Image_Select").GetComponent<Image>();
            imgEquiped = transform.Find("Image_Equiped").GetComponent<Image>();
            imgAdd = transform.Find("Image_Add").GetComponent<Image>();
            btnNone = transform.Find("Btn_None").GetComponent<Button>();
            txtNumber = transform.Find("Text_Number").GetComponent<Text>();
            txtNew = transform.Find("Text_New").gameObject;
            txtBind = transform.Find("Text_Bound").GetComponent<Text>();
            txtName = transform.Find("Text_Name").GetComponent<Text>();
            idText = transform.Find("Id")?.GetComponent<Text>();

            levelGo = transform.Find("Image_Level").gameObject;
            levelText = transform.Find("Image_Level/Text_Level/Text_Num").GetComponent<Text>();

            gotGo = transform.Find("Btn_Get").gameObject;
            clock = transform.Find("Image_Clock").gameObject;
            btnImage = transform.Find("Btn_Item/Image_BG").GetComponent<Image>();
            addGo = transform.Find("Image_Add").gameObject;
            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(btnImage.gameObject);
            eventListener.AddEventListener(EventTriggerType.PointerClick, (ret)=> { OnClick(); });

            longPressButton = btnImage.gameObject.AddComponent<UI_LongPressButton>();
            longPressButton.onStartPress.AddListener(OnLongPressed);
        }

        public void SetData(PropSpeicalLoader.ShowItemData showitemdata, EUIID uiId) {
            imgAdd.gameObject.SetActive(false);
            btnNone.gameObject.SetActive(false);

            this.showItemData = showitemdata;
            this.m_itemInfo = new Sys_Trade.TradeItemInfo() { uID = showitemdata.itemData.Uuid, infoID = showitemdata.itemData.Id };
            this.boxEvt.Reset(uiId, showitemdata);

            this.Refresh();
        }
        public void Refresh() {
            var csv = CSVItem.Instance.GetConfData(showItemData.itemData.Id);
            if (csv != null) {
                Layout.imgIcon.gameObject.SetActive(true);
                ImageHelper.SetIcon(Layout.imgIcon, csv.icon_id);
                ImageHelper.SetImgAlpha(Layout.imgIcon, 1f);

                Layout.imgQuality.gameObject.SetActive(true);

                //宠物的道具框，需要单独显示
                if (showItemData.itemData.cSVItemData.type_id == (int)EItemType.Pet)
                {
                    ImageHelper.SetIcon(Layout.imgQuality, 993701u);

                    if (showItemData.Level != 0u)
                    {
                        levelGo.SetActive(true);
                        levelText.text = showItemData.Level.ToString();
                    }
                }
                else
                {
                    levelGo.SetActive(false);
                    //设置quality
                    uint tempQuality = 0u;
                    if (showItemData.itemData.Quality == 0u)
                        tempQuality = csv.quality;
                    else
                        tempQuality = showItemData.itemData.Quality;

                    ImageHelper.GetQualityColor_Frame(Layout.imgQuality, (int)tempQuality);
                }

                long right = showItemData.itemData.Count;
                txtNumber.gameObject.SetActive(showItemData.bShowCount);

                if (showItemData.bShowBagCount)
                {
                    int left = (int)Sys_Bag.Instance.GetItemCount(showItemData.itemData.Id);
                    uint contentId = left < right ? 1601000004u : 1601000005u;
                    txtNumber.text = string.Format(LanguageHelper.GetTextContent(contentId), left.ToString(), right.ToString());
                }
                else
                {
                    txtNumber.text = right.ToString();
                }

                imgSelect.gameObject.SetActive(false);
                imgLock.SetActive(false);
                txtNew.SetActive(false);
                txtBind.gameObject.SetActive(showItemData.itemData.bBind);
                if (showItemData.itemData.bBind)
                {
                    clock.SetActive(false);     //绑定情况下 一定不显示禁售期锁
                }
                else
                {
                    clock.SetActive(!showItemData.bTradeEnd);
                }


                //技能书等级特殊显示
                Sys_Skill.Instance.ShowPetSkillBook(imgSkillBook, csv);
            }

            if(idText != null) {
#if UNITY_EDITOR
            idText.text = showItemData.itemData.Id.ToString();
#else
            idText.text = " ";
#endif
            }
        }
      
        public void SetData(PropSpeicalLoader.MessageBoxEvt _boxEvt)
        {
            SetData(_boxEvt.itemData, _boxEvt.sourceUiId);
        }

        private void OnClick()
        {
            //showItemData.UpdateItemData(Sys_Trade.Instance.GetItemDataByTradeItemInfo(this.m_itemInfo));
            if (showItemData.bUseClick)
            {
                if (showItemData.operationType == 0u) //交易行
                {
                    if (showItemData.itemData.cSVItemData.type_id == (int)EItemType.Equipment
                        || showItemData.itemData.cSVItemData.type_id == (int)EItemType.Pet
                        || showItemData.itemData.cSVItemData.type_id == (int)EItemType.Ornament
                        || showItemData.itemData.cSVItemData.type_id == (int)EItemType.PetEquipment)
                    {
                        Sys_Trade.Instance.OnDetailInfoReq(showItemData.bCross, showItemData.itemData.Uuid, detailSrcType);
                    }
                    else
                    {
                        PropIconLoader.ShowItemData temp = new PropIconLoader.ShowItemData(showItemData.itemData.Id, showItemData.itemData.Count, true, false, false, false, false, false, false, false);
                        MessageBoxEvt msgEvt = new MessageBoxEvt(EUIID.UI_Trade, temp);

                        UIManager.OpenUI(EUIID.UI_Message_Box, false, msgEvt);
                    }
                }
            }

            showItemData.ClickAction?.Invoke(this);
        }

        public void EnableLongPress(bool toEnable) {
            if (longPressButton != null) {
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
    }
}

