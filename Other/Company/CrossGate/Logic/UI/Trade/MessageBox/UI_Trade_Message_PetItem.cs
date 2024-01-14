using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Logic.Core;
using Lib.Core;
using Table;
using Packet;

namespace Logic
{
    public class UI_Trade_Message_PetItem: UI_Tips_Pet_Type.IListener
    {
        private Transform transform;

        private PropSpecialItem _propItem;
        private Text _textLevel;
        private Text _textName;

        private UI_Tips_Pet_Type _typeView;
        private UI_Tips_Pet_Basic _basicView;
        private UI_Tips_Pet_High _highView;
        private UI_Tips_Pet_Skill _skillView;

        private ItemData _itemPet;

        public void Init(Transform trans)
        {
            transform = trans;

            _propItem = new PropSpecialItem();
            _propItem.BindGameObject(transform.Find("View_Buyer/Background_Root/Image_Titlebg/PropItem").gameObject);

            _textLevel = transform.Find("View_Buyer/Background_Root/Image_Titlebg/Text_Lv").GetComponent<Text>();
            _textName = transform.Find("View_Buyer/Background_Root/Image_Titlebg/Text_Name").GetComponent<Text>();

            _typeView = new UI_Tips_Pet_Type();
            _typeView.Init(transform.Find("View_Buyer/Background_Root/Toggles"));
            _typeView.Register(this);

            _basicView = new UI_Tips_Pet_Basic();
            _basicView.Init(transform.Find("View_Buyer/Background_Root/Page0"));

            _highView = new UI_Tips_Pet_High();
            _highView.Init(transform.Find("View_Buyer/Background_Root/Page1"));

            _skillView = new UI_Tips_Pet_Skill();
            _skillView.Init(transform.Find("View_Buyer/Background_Root/Page2"));
        }

        public void Show()
        {
            transform.gameObject.SetActive(true);
        }

        public void Hide()
        {
            transform.gameObject.SetActive(false);
        }

        public void UpdateInfo(TradeItem tradeItem)
        {
            _itemPet = new ItemData(99, tradeItem.GoodsUid, tradeItem.InfoId, tradeItem.Count, 0, false, false, null, null, 0, tradeItem.Pet);

            PropSpeicalLoader.ShowItemData showItem = new PropSpeicalLoader.ShowItemData(_itemPet, false, false);
            //showItem.SetTradeEnd(true);
            //showItem.SetCross(m_Brief.BCross);
            showItem.SetLevel(_itemPet.Pet.SimpleInfo.Level);
            _propItem.SetData(new PropSpeicalLoader.MessageBoxEvt(EUIID.UI_Tips_Pet, showItem));

            _textName.text = LanguageHelper.GetTextContent(_itemPet.cSVItemData.name_id);
            _textLevel.text = "";

            _typeView.Show();
            _basicView.SetData(_itemPet.Pet);
        }

        public void OnSelectType(int index)
        {
            _basicView.Hide();
            _highView.Hide();
            _skillView.Hide();

            switch (index)
            {
                case 0:
                    _basicView.Show();
                    _basicView.SetData(_itemPet.Pet);
                    break;
                case 1:
                    _highView.Show();
                    _highView.SetData(_itemPet.Pet);
                    break;
                case 2:
                    _skillView.Show();
                    _skillView.SetData(_itemPet.Pet);
                    break;
            }
        }
    }
}


