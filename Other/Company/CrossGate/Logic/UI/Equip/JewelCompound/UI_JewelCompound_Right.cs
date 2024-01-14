using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Table;

namespace Logic
{
    public class UI_JewelCompound_Right : UIParseCommon
    {
        private class JewelItemRoot : UIParseCommon
        {
            private PropItem propItem;
            private Text textName;

            protected override void Parse()
            {
                propItem = new PropItem();
                propItem.BindGameObject(transform.Find("PropItem").gameObject);

                textName = transform.Find("Text_Name").GetComponent<Text>();
                textName.text = "";
            }

            public override void Show()
            {
                gameObject.SetActive(true);
            }

            public override void Hide()
            {
                gameObject.SetActive(false);
            }

            public override void UpdateInfo(ItemData item)
            {
                if (item != null)
                {
                    propItem.SetData(new MessageBoxEvt(EUIID.UI_JewelCompound, new PropIconLoader.ShowItemData(item.Id, (int)item.Count, true, false, false, false, false, false, false, true)));
                    TextHelper.SetText(textName, item.cSVItemData.name_id);
                }
                else
                {
                    propItem.Layout.imgIcon.gameObject.SetActive(false);
                    textName.text = "";
                }
            }

            public void SetTextNum(string strNum)
            {
                propItem.txtNumber.gameObject.SetActive(true);
                propItem.txtNumber.text = strNum;
            }
        }

        private GameObject limitTip;
        private JewelItemRoot srcJewel;
        private JewelItemRoot desJewel;

        private Toggle toggleAuto;

        private Button btnOnce;
        private Button btnHot;

        private IListener listener;

        private bool isMaxLevel = false;
        public bool isAuto = true;

        private Animator _animator;

        private GameObject imgGoBottom;
        private GameObject imgGoArrow;

        protected override void Parse()
        {
            _animator = transform.GetComponent<Animator>();

            limitTip = transform.Find("Text_Limited").gameObject;

            srcJewel = new JewelItemRoot();
            srcJewel.Init(transform.Find("Item02"));

            desJewel = new JewelItemRoot();
            desJewel.Init(transform.Find("Item01"));

            toggleAuto = transform.Find("Toggle_Remind").GetComponent<Toggle>();
            toggleAuto.onValueChanged.AddListener(OnClickToggle);
            toggleAuto.isOn = true;

            btnOnce = transform.Find("Button_One").GetComponent<Button>();
            btnOnce.onClick.AddListener(OnClickBtnOnce);
            btnHot = transform.Find("Button_All").GetComponent<Button>();
            btnHot.onClick.AddListener(OnClickBtnHot);

            imgGoBottom = transform.Find("Image").gameObject;
            imgGoArrow = transform.Find("Image1").gameObject;
        }

        public override void Hide()
        {
            if (_animator != null)
            {
                _animator.speed = 0f;
                _animator.Rebind();
                //_animator.enabled = false;
            }

            base.Hide();
        }

        public override void Show()
        {
            base.Show();
        }

        private void OnClickToggle(bool isOn)
        {
            isAuto = isOn;
        }

        private void OnClickBtnOnce()
        {
            if (!isMaxLevel)
            {
                listener.OnCompoundOnce();
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4004));
            }
        }

        private void OnClickBtnHot()
        {
            if (!isMaxLevel)
            {
                listener.OnCompoundHot();
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(4004));
            }
        }

        public void ReisterListener(IListener _listener)
        {
            listener = _listener;
        }

        public void UpdateComoundInfo(Sys_Equip.JewelGroupData _itemGroup)
        {
            limitTip.SetActive(false);
            
            if (_itemGroup != null)
            {
                CSVJewel.Data jewelData = CSVJewel.Instance.GetConfData(_itemGroup.itemId);
                if (jewelData != null)
                {
                    ItemData srcItem = new ItemData(0, 0, _itemGroup.itemId, _itemGroup.count, 0, false, false, null, null, 0);
                    srcJewel.UpdateInfo(srcItem);
                    string temp = string.Format("x{0}", jewelData.num);
                    bool isCanCompond = _itemGroup.count >= jewelData.num;
                    uint costColorId = isCanCompond ? (uint)2007427 : 2007202;
                    srcJewel.SetTextNum(LanguageHelper.GetLanguageColorWordsFormat(temp, costColorId));

                    isMaxLevel = jewelData.next_id == 0;
                    ImageHelper.SetImageGray(btnOnce.GetComponent<Image>(), isMaxLevel);
                    ImageHelper.SetImageGray(btnHot.GetComponent<Image>(), isMaxLevel);

                    if (isMaxLevel) //最大等级
                    {
                        limitTip.SetActive(true);
                        desJewel.Hide();

                        imgGoBottom.gameObject.SetActive(false);
                        imgGoArrow.gameObject.SetActive(false);
                    }
                    else
                    {
                        //构建目标宝石
                        ItemData desItem = new ItemData(0, 0, jewelData.next_id, 1, 0, false, false, null, null,0);
                        desJewel.Show();
                        desJewel.UpdateInfo(desItem);

                        imgGoBottom.gameObject.SetActive(true);
                        imgGoArrow.gameObject.SetActive(true);
                    }
                }
                else
                {
                    Debug.LogErrorFormat("not found jewel Id = {0}", _itemGroup.itemId);
                }
            }
            else
            {
                srcJewel.UpdateInfo(null);
                desJewel.UpdateInfo(null);
            }
        }

        public interface IListener
        {
            void OnCompoundOnce();
            void OnCompoundHot();
        }
    }
}


