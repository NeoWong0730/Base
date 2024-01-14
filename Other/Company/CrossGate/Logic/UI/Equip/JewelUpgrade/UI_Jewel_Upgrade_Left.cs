using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Table;

namespace Logic
{
    public class UI_Jewel_Upgrade_Left
    {
        public class NeedBuy
        {
            private Transform transform;
            private Item0_Layout layout;
            private Text _textName;
            private Text _textNum;

            private Text _textCost;
            private Image _imgCost;

            private bool _IsPriceFixed; //价格是否满足
            public bool PriceFixed { get { return _IsPriceFixed; } }

            private long _costPrice;
            public long CostPrice { get { return _costPrice; } }

            private uint _costId;
            public uint CostId { get { return _costId; } }

            public void Init(Transform trans)
            {
                transform = trans;

                layout = new Item0_Layout();
                layout.BindGameObject(transform.Find("Item/PropItem/Btn_Item").gameObject);
                _textName = transform.Find("Item/Text").GetComponent<Text>();
                _textNum = transform.Find("Item/PropItem/Text_Number").GetComponent<Text>();

                _textCost = transform.Find("Text_Cost/Text").GetComponent<Text>();
                _imgCost = transform.Find("Text_Cost/Image_Coin").GetComponent<Image>();
            }

            public void OnShow()
            {
                transform.gameObject.SetActive(true);
            }

            public void OnHide()
            {
                transform.gameObject.SetActive(false);

                _IsPriceFixed = true;
                _costPrice = 0L;
            }

            public void UpdateInfo()
            {
                uint jewelId = Sys_Equip.Instance.OneLevelId;
                CSVItem.Data jewelItem = CSVItem.Instance.GetConfData(jewelId);
                layout.SetData(jewelItem, false);
                _textName.text = LanguageHelper.GetTextContent(jewelItem.name_id);
                _textNum.text = string.Format("x{0}", Sys_Equip.Instance.LeftUpgradeCount.ToString());

                CSVJewel.Data jewelInfo = CSVJewel.Instance.GetConfData(jewelId);
                _costId = jewelInfo.price[0];
                uint costValue = jewelInfo.price[1];
                CSVItem.Data costItem = CSVItem.Instance.GetConfData(_costId);
                ImageHelper.SetIcon(_imgCost, costItem.small_icon_id);

                _costPrice = costValue * Sys_Equip.Instance.LeftUpgradeCount;
                long hadCount = Sys_Bag.Instance.GetItemCount(_costId);
                uint colorId = hadCount >= _costPrice ? 19u : 20u;
                TextHelper.SetText(_textCost, _costPrice.ToString(), LanguageHelper.GetTextStyle(colorId));
                _IsPriceFixed = hadCount >= _costPrice;
            }
        }

        private Transform transform;
        private Item0_Layout layout;
        private Text _textName;
        private Text _textAttrName;
        private Text _textAttrValue;

        private Button _btnLeft;
        private Button _btnRight;

        private NeedBuy _needBuy;

        private Button _btnUpgrade;

        private CSVEquipment.Data _equipInfo;
        private uint _srcJewelId;
        private uint _curJewelId;
        private Stack<uint> _queneIds;

        private IListener _listener;

        public void Init(Transform trans)
        {
            transform = trans;

            layout = new Item0_Layout();
            layout.BindGameObject(transform.Find("Item01/PropItem/Btn_Item").gameObject);
            //layout.btnItem.onClick.AddListener(OnClickJewel);

            _textName = transform.Find("Item01/Text_Name").GetComponent<Text>();
            _textAttrName = transform.Find("Item01/Text_Atrr").GetComponent<Text>();
            _textAttrValue = transform.Find("Item01/Text_Atrr/Text_Num").GetComponent<Text>();

            _btnLeft = transform.Find("Button_Left").GetComponent<Button>();
            _btnLeft.onClick.AddListener(OnClickLeft);
            _btnRight = transform.Find("Button_Right").GetComponent<Button>();
            _btnRight.onClick.AddListener(OnClickRight);

            _needBuy = new NeedBuy();
            _needBuy.Init(transform.Find("View_Buy"));

            _btnUpgrade = transform.Find("Button_Grade").GetComponent<Button>();
            _btnUpgrade.onClick.AddListener(OnClickUpgrade);

            _queneIds = new Stack<uint>();
        }

        private void OnClickLeft()
        {
            if (_queneIds.Count > 0)
            {
                _curJewelId = _queneIds.Pop();
                RefreshJewel();
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4189u));
            }
        }

        private void OnClickRight()
        {
            CSVJewel.Data jewelData = CSVJewel.Instance.GetConfData(_curJewelId);
            if (jewelData.level >= _equipInfo.jewel_level)
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4188u));
            }
            else
            {
                _queneIds.Push(_curJewelId);
                _curJewelId = jewelData.next_id;

                RefreshJewel();
            }
        }

        private void OnClickUpgrade()
        {
            if (_needBuy.PriceFixed)
            {
                long costPrice = _needBuy.CostPrice;
                if (costPrice > 0)
                {
                    CSVJewel.Data jewelData = CSVJewel.Instance.GetConfData(_curJewelId);
                    CSVItem.Data itemData = CSVItem.Instance.GetConfData(_needBuy.CostId);

                    PromptBoxParameter.Instance.Clear();
                    PromptBoxParameter.Instance.content = LanguageHelper.GetTextContent(4214, costPrice.ToString(), LanguageHelper.GetTextContent(itemData.name_id), jewelData.level.ToString());
                    PromptBoxParameter.Instance.SetConfirm(true, () =>
                    {
                        _listener?.OnClickUpgrade();
                    });
                    PromptBoxParameter.Instance.SetCancel(true, null);
                    //PromptBoxParameter.Instance.SetCountdown(3f, PromptBoxParameter.ECountdown.Cancel);
                    Logic.Core.UIManager.OpenUI(EUIID.UI_PromptBox, false, PromptBoxParameter.Instance);
                }
                else
                {
                    _listener?.OnClickUpgrade();
                }
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4187u));
            }
        }

        public void Register(IListener listener)
        {
            _listener = listener;
        }

        public void UpdateInfo(uint srcJewelId, uint equipInfoId)
        {
            _srcJewelId = srcJewelId;
            _equipInfo = CSVEquipment.Instance.GetConfData(equipInfoId);
            CSVJewel.Data jewelData = CSVJewel.Instance.GetConfData(srcJewelId);
            _curJewelId = jewelData.next_id;

            _needBuy.OnHide();

            RefreshJewel();
        }

        private void RefreshJewel()
        {
            CSVItem.Data jewelItem = CSVItem.Instance.GetConfData(_curJewelId);
            layout.SetData(jewelItem, false);
            _textName.text = LanguageHelper.GetTextContent(jewelItem.name_id);

            CSVJewel.Data jewelInfo = CSVJewel.Instance.GetConfData(_curJewelId);
            if (jewelInfo.percent != 0)
            {
                _textAttrName.text = LanguageHelper.GetTextContent(4050u);
                _textAttrValue.text = string.Format("+{0}%", jewelInfo.percent);
            }
            else
            {
                for (int i = 0; i < jewelInfo.attr.Count; ++i)
                {
                    CSVAttr.Data attrInfo = CSVAttr.Instance.GetConfData(jewelInfo.attr[i][0]);
                    _textAttrName.text = LanguageHelper.GetTextContent(attrInfo.name);
                    _textAttrValue.text = Sys_Attr.Instance.GetAttrValue(attrInfo, jewelInfo.attr[i][1]);
                }
            }
            

            _listener?.OnSelectJewel(_curJewelId, _srcJewelId);
        }

        public void UpdateNeedBuy()
        {
            if (Sys_Equip.Instance.LeftUpgradeCount > 0)
            {
                _needBuy.OnShow();
                _needBuy.UpdateInfo();
                bool isFixed = _needBuy.PriceFixed;
                ImageHelper.SetImageGray(_btnUpgrade.image, !isFixed);
            }
            else
            {
                _needBuy.OnHide();
                ImageHelper.SetImageGray(_btnUpgrade.image, false);
            }
        }

        public interface IListener
        {
            void OnSelectJewel(uint jewelId, uint srcJewelId);

            void OnClickUpgrade();
        }

    }
}


