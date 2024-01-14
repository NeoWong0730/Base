using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Logic.Core;
using Table;
using Packet;

namespace Logic
{
    public class UI_Trade_Sell_Top
    {
        private Transform transform;

        private PropSpecialItem m_PropItem;
        public GameObject goDomestication_0;
        public GameObject goDomestication_1;

        private Text m_Name;
        private Text m_Lv;
        private Text m_Des;

        private ItemData _itemData;
        private CSVItem.Data _csvItemData;
        public void Init(Transform trans)
        {
            transform = trans;

            m_PropItem = new PropSpecialItem();
            m_PropItem.BindGameObject(transform.Find("PropItem").gameObject);
            Lib.Core.EventTrigger eventListener = Lib.Core.EventTrigger.Get(m_PropItem.btnImage.gameObject);
            eventListener.AddEventListener(EventTriggerType.PointerClick, (ret) => { OnClick(); });

            goDomestication_0 = transform.Find("PropItem/Image_Domestication_0").gameObject;
            goDomestication_1 = transform.Find("PropItem/Image_Domestication_1").gameObject;

            m_Name = transform.Find("Text_Name").GetComponent<Text>();
            m_Lv = transform.Find("Text_Lv").GetComponent<Text>();
            m_Lv.text = "";

            m_Des = transform.Find("Text_Dex").GetComponent<Text>();
        }

        private void OnClick()
        {
            if (_csvItemData != null)
            {
                if (_csvItemData.type_id == (int)EItemType.Equipment)
                {
                    EquipTipsData tipData = new EquipTipsData();
                    tipData.equip = _itemData;
                    tipData.isCompare = true;
                    tipData.isShowOpBtn = false;

                    UIManager.OpenUI(EUIID.UI_TipsEquipment, false, tipData);
                }
                else if (_csvItemData.type_id == (int)EItemType.Pet)
                {
                    ClientPet clientPet = new ClientPet(_itemData.Pet);
                    UIManager.OpenUI(EUIID.UI_Pet_Details, false, clientPet);
                }
                else if (_csvItemData.type_id == (int)EItemType.Ornament)
                {
                    OrnamentTipsData tipData = new OrnamentTipsData();
                    tipData.equip = _itemData;
                    tipData.isCompare = false;
                    tipData.sourceUiId = EUIID.UI_Trade_Sell;
                    UIManager.OpenUI(EUIID.UI_Tips_Ornament, false, tipData);
                }
                else
                {
                    PropIconLoader.ShowItemData temp = new PropIconLoader.ShowItemData(_itemData.Id, _itemData.Count, true, false, false, false, false, false, false, false);
                    MessageBoxEvt msgEvt = new MessageBoxEvt(EUIID.UI_Trade_Sell, temp);

                    UIManager.OpenUI(EUIID.UI_Message_Box, false, msgEvt);
                }
            }
        }

        public void Show()
        {
            transform.gameObject.SetActive(true);
        }

        public void Hide()
        {
            transform.gameObject.SetActive(false);
        }

        public void UpdateInfo(ItemData saleItem)
        {
            _itemData = saleItem;
            CSVCommodity.Data goodData = null;
            if (saleItem != null)
            {
                goodData = CSVCommodity.Instance.GetConfData(saleItem.Id);
            }

            _csvItemData = CSVItem.Instance.GetConfData(saleItem.Id);
            bool isPet = _csvItemData.type_id == (int)EItemType.Pet;

            PropSpeicalLoader.ShowItemData showItem = new PropSpeicalLoader.ShowItemData(saleItem, false, false);
            showItem.SetTradeEnd(true);
            if(isPet)
                showItem.SetLevel(saleItem.Pet.SimpleInfo.Level);
            m_PropItem.SetData(new PropSpeicalLoader.MessageBoxEvt(EUIID.UI_Trade_Sell, showItem));

            m_Name.text = LanguageHelper.GetTextContent(_csvItemData.name_id);
            m_Des.text = LanguageHelper.GetTextContent(_csvItemData.describe_id);
            //if (isPet)
            //    m_Lv.text = saleItem.Pet.SimpleInfo.Level.ToString();
            //else
            //    m_Lv.text = _csvItemData.lv.ToString();

            goDomestication_0.SetActive(false);
            goDomestication_1.SetActive(false);
            //宠物驯化判断
            if (isPet)
            {
                CSVPetNew.Data petNew = CSVPetNew.Instance.GetConfData(saleItem.Id);
                if (petNew != null && petNew.mount)
                {
                    goDomestication_0.SetActive(saleItem.Pet.SimpleInfo.MountDomestication == 0);
                    goDomestication_1.SetActive(saleItem.Pet.SimpleInfo.MountDomestication == 1);
                }
            }
        }
    }
}


