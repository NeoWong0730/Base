using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using Table;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

namespace Logic
{
    public class PromptItemData
    {
        public Action onConfire;
        public string content;
        public List<uint> items = new List<uint>();               //需要的道具
        public List<uint> needCount = new List<uint>();         //需要的数量
        public string notEnough;
        public uint titleId;
        public uint type;       //0 升段 1 技能升级 2 学习制造
    }

    public class UI_PromptItemBox : UIBase
    {
        private Transform layout;
        private Transform parentRoot;
        private GameObject itemNotCurrency;
        private GameObject currencyObj;
        private Text content;
        private Text number;
        private Button confire;
        private Button closeBtn;
        private Text title;
        private Image currencyIcon;
        private Text currencyNum;
        private PromptItemData _promptItemData;
        private List<Button> buttons = new List<Button>();

        protected override void OnOpen(object arg)
        {
            _promptItemData = arg as PromptItemData;
        }

        protected override void OnLoaded()
        {
            layout = transform.Find("Animator/Layout");
            parentRoot = transform.Find("Animator/Layout/Scroll_View01/Viewport");
            itemNotCurrency = transform.Find("Animator/Layout/Scroll_View01").gameObject;
            content = transform.Find("Animator/Layout/Text").GetComponent<Text>();
            title = transform.Find("Animator/View_TipsBg01_Small/Text_Title").GetComponent<Text>();
            currencyObj = transform.Find("Animator/Layout/Cost01").gameObject;
            currencyIcon = currencyObj.transform.Find("Image_ICON").GetComponent<Image>();
            currencyNum = currencyObj.transform.Find("Text").GetComponent<Text>();
            number = transform.Find("Animator/Layout/Scroll_View01/Viewport/Item/Text_Number").GetComponent<Text>();
            confire = transform.Find("Animator/Btn_01").GetComponent<Button>();
            closeBtn = transform.Find("Animator/View_TipsBg01_Small/Btn_Close").GetComponent<Button>();
            closeBtn.onClick.AddListener(() =>
            {
                UIManager.CloseUI(EUIID.UI_PromptItemBox);
            });
            confire.onClick.AddListener(OnConfireClicked);
        }

        protected override void OnShow()
        {
            Refresh();
        }

        private void Refresh()
        {
            TextHelper.SetText(title, _promptItemData.titleId);
            List<uint> item_0 = new List<uint>();
            List<uint> needCount_0 = new List<uint>();
            List<uint> item_1 = new List<uint>();           //表示货币
            List<uint> needCount_1 = new List<uint>();
            for (int i = 0; i < _promptItemData.items.Count; i++)
            {
                if (_promptItemData.type == 2)
                {
                    item_0.Add(_promptItemData.items[i]);
                    needCount_0.Add(_promptItemData.needCount[i]);
                }
                else
                {
                    if (_promptItemData.items[i] < 500)
                    {
                        item_1.Add(_promptItemData.items[i]);
                        needCount_1.Add(_promptItemData.needCount[i]);
                    }
                    else
                    {
                        item_0.Add(_promptItemData.items[i]);
                        needCount_0.Add(_promptItemData.needCount[i]);
                    }
                }
            }
            content.text = _promptItemData.content;
            if (item_0.Count == 0)
            {
                itemNotCurrency.SetActive(false);
            }
            else
            {
                itemNotCurrency.SetActive(true);
                FrameworkTool.CreateChildList(parentRoot, item_0.Count);
                for (int i = 0, count = item_0.Count; i < count; i++)
                {
                    Transform trans = parentRoot.GetChild(i);
                    PropItem propItem = new PropItem();
                    propItem.BindGameObject(trans.gameObject);

                    uint itemid = item_0[i];
                    uint needCount = needCount_0[i];

                    PropIconLoader.ShowItemData showItem = new PropIconLoader.ShowItemData
                                   (_id: itemid,
                                   _count: needCount,
                                   _bUseQuailty: true,
                                   _bBind: false,
                                   _bNew: false,
                                   _bUnLock: false,
                                   _bSelected: false,
                                   _bShowCount: true,
                                   _bShowBagCount: true,
                                   _bUseClick: true,
                                   _onClick: null,
                                   _bshowBtnNo: false);
                    propItem.SetData(new MessageBoxEvt(EUIID.UI_PromptItemBox, showItem));
                }
            }
            if (item_1.Count == 0)
            {
                currencyObj.SetActive(false);
            }
            else
            {
                currencyObj.SetActive(true);
                ImageHelper.SetIcon(currencyIcon, CSVItem.Instance.GetConfData(item_1[0]).icon_id);
                uint itemCount = (uint)Sys_Bag.Instance.GetItemCount(item_1[0]);
                uint wordStyleId = 0;
                string content = string.Empty;
                if (itemCount < needCount_1[0])
                {
                    wordStyleId = 20;
                }
                else
                {
                    wordStyleId = 19;
                }
                content = string.Format("{0}/{1}", itemCount.ToString(), needCount_1[0].ToString());
                TextHelper.SetText(currencyNum, content, CSVWordStyle.Instance.GetConfData(wordStyleId));
            }

            FrameworkTool.ForceRebuildLayout(layout.gameObject);
        }

        private void OnConfireClicked()
        {
            bool enough = true;
            for (int i = 0, count = _promptItemData.items.Count; i < count; i++)
            {
                uint needcount = _promptItemData.needCount[i];
                uint itemCount = (uint)Sys_Bag.Instance.GetItemCount(_promptItemData.items[i]);
                if (needcount > itemCount)
                {
                    enough = false;
                    break;
                }
            }
            if (!enough)
            {
                Sys_Hint.Instance.PushContent_Normal(_promptItemData.notEnough);
                return;
            }
            _promptItemData.onConfire?.Invoke();
        }
    }
}


