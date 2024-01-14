using System.Collections.Generic;
using UnityEngine;
using Logic.Core;
using Lib.Core;
using Table;
using System;
using UnityEngine.UI;
using Packet;
namespace Logic
{
    public class UINumberInputBase
    {
        public enum ENumberInputField
        {
            currencyType,
            Number,
        }

        protected ENumberInputField eNumberInputField;
        protected long currentNum;  //当前价
        protected long minInput;    //最低价
        protected long singleStep; //单次最少加
        protected uint typeParam; // 可变currencyType 为货币id Number 为数量
        public virtual void Init(ENumberInputField type, uint typeParam)
        {
            eNumberInputField = type;
            this.typeParam = typeParam;
        }

        public virtual void RefreshNum(long currentNum)
        {
            this.currentNum = minInput;
        }

        public virtual long GetNum()
        {
            return currentNum;
        }

        public virtual void AddNum()
        {
        }

        public virtual void SubNum()
        {
        }
    }


    public class UINumberInputField: UINumberInputBase
    {
        private InputField input;
        private uint addTips = 0;
        private uint subTips = 0;

        public void SetInputData(long singleStep, uint addTips, uint subTips, long minInput = 0)
        {
            this.currentNum = minInput;
            this.minInput = minInput;
            this.singleStep = singleStep;
            this.addTips = addTips;
            this.subTips = subTips;
            input.text = this.currentNum.ToString();
        }

        public void RegisterAction(Button addAction, Button subAction, InputField InputAction)
        {
            input = InputAction;
            UI_LongPressButton LongPressAddButton = addAction.gameObject.AddComponent<UI_LongPressButton>();
            LongPressAddButton.interval = 0.3f;
            LongPressAddButton.bPressAcc = true;
            LongPressAddButton.onRelease.AddListener(AddNum);
            LongPressAddButton.OnPressAcc.AddListener(AddNum);

            UI_LongPressButton LongPressSubButton = subAction.gameObject.AddComponent<UI_LongPressButton>();
            LongPressSubButton.interval = 0.3f;
            LongPressSubButton.bPressAcc = true;
            LongPressSubButton.onRelease.AddListener(SubNum);
            LongPressSubButton.OnPressAcc.AddListener(SubNum);

            InputAction.contentType = InputField.ContentType.IntegerNumber;
            InputAction.keyboardType = TouchScreenKeyboardType.NumberPad;
            InputAction.onEndEdit.AddListener(InputEnd);
        }

        public override void AddNum()
        {
            long maxCount = eNumberInputField == ENumberInputField.currencyType ? Sys_Bag.Instance.GetItemCount((uint)typeParam) : typeParam;
            if (currentNum + singleStep <= maxCount)
            {
                currentNum += singleStep;
            }
            else
            {
                if(addTips != 0)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(addTips));
                }
                return;
            }
            input.text = currentNum.ToString();
        }

        public override void SubNum()
        {
            if (currentNum - singleStep >= minInput)
            {
                currentNum -= singleStep;
            }
            else
            {
                if (subTips != 0)
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(subTips));
                }
            }
            input.text = currentNum.ToString();
        }

        private void InputEnd(string s)
        {
            if (!string.IsNullOrEmpty(s))
            {
                long result = Convert.ToInt64(s);
                if (result <= minInput)
                {
                    currentNum = minInput;
                }
                else
                {
                    long maxCount = eNumberInputField == ENumberInputField.currencyType ? Sys_Bag.Instance.GetItemCount((uint)typeParam) : typeParam;
                    if(maxCount <= minInput)
                    {
                        maxCount = minInput;
                    }
                    currentNum = result > maxCount ? maxCount : result;
                }
            }
            else
            {
                currentNum = minInput;
            }

            input.text = currentNum.ToString();
        }
    }

    public class UI_FamilySale_Detail_Layout
    {
        private Button closeBtn;
        public PropItem item;
        public Text itemNum;
        //public Text itemName;
        private Button cancelBtn;
        private Button comfireBtn;
        public Image coinImage;
        public UINumberInputField numberInput;
        public void Init(Transform transform)
        {
            closeBtn = transform.Find("Animator/View_TipsBgNew07/Btn_Close").GetComponent<Button>();
            item = new PropItem();
            item.BindGameObject(transform.Find("Animator/View_SailDetail/Item/Image_ItemBG/PropItem").gameObject);
            itemNum = transform.Find("Animator/View_SailDetail/Item/Text_Amount").GetComponent<Text>();
            //itemName =transform.Find("Animator/View_SailDetail/Item/Image_Name/Text_Name").GetComponent<Text>();
            cancelBtn =transform.Find("Animator/View_SailDetail/Btn_02").GetComponent<Button>();
            comfireBtn = transform.Find("Animator/View_SailDetail/Btn_01").GetComponent<Button>();
            coinImage = transform.Find("Animator/View_SailDetail/Image1/Label_title/Image").GetComponent<Image>();
            numberInput = new UINumberInputField();
            numberInput.RegisterAction(transform.Find("Animator/View_SailDetail/Image1/Button_Add").GetComponent<Button>(),
                transform.Find("Animator/View_SailDetail/Image1/Button_Sub").GetComponent<Button>(),
                transform.Find("Animator/View_SailDetail/Image1/InputField").GetComponent<InputField>());
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.CloseBtnClicked);
            cancelBtn.onClick.AddListener(listener.OnCancelBtnClicked);
            comfireBtn.onClick.AddListener(listener.OncomfireBtnClicked);
        }

        public interface IListener
        {
            void CloseBtnClicked();
            void OnCancelBtnClicked();
            void OncomfireBtnClicked();
        }
    }

    public class UI_FamilySale_Detail : UIBase, UI_FamilySale_Detail_Layout.IListener
    {
        private UI_FamilySale_Detail_Layout layout = new UI_FamilySale_Detail_Layout();
        private uint activeId;
        private uint uid;
        private uint myPrice;
        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {

        }

        protected override void OnOpen(object arg = null)
        {
            if (arg is Tuple<uint, uint>)
            {
                Tuple<uint, uint> tuple = arg as Tuple<uint, uint>;
                activeId = tuple.Item1;
                uid = tuple.Item2;
            }
        }

        protected override void OnShow()
        {
            RefreshView();
        }

        private void RefreshView()
        {
            GuildAuction.Types.AuctionItem auctionItem = Sys_Family.Instance.familyData.familyAuctionInfo.GetAuctionItem(activeId, uid);
            if (null != auctionItem)
            {
                CSVFamilyAuction.Data familyAwardData = CSVFamilyAuction.Instance.GetConfData(auctionItem.InfoId);
                if (null != familyAwardData)
                {
                    layout.numberInput.Init(UINumberInputBase.ENumberInputField.currencyType, familyAwardData.AuctionCurrency);

                    CSVItem.Data currencyItem = CSVItem.Instance.GetConfData(familyAwardData.AuctionCurrency);
                    if (null != currencyItem)
                    {
                        ImageHelper.SetIcon(layout.coinImage, currencyItem.icon_id);
                    }
                    else
                    {
                        DebugUtil.Log(ELogType.eNone, $"CSVItem.Data is not find id = {familyAwardData.AuctionCurrency}");
                    }

                    CSVItem.Data itemData = CSVItem.Instance.GetConfData(familyAwardData.ItemId);
                    PropIconLoader.ShowItemData itemN = new PropIconLoader.ShowItemData(familyAwardData.ItemId, 0, false, false, false, false, false, false, false, false);
                    layout.item.SetData(itemN, EUIID.UI_FamilySale_Detail);
                    bool hasItem = null != itemData;
                    if (hasItem)
                    {
                        layout.itemNum.text = LanguageHelper.GetTextContent(11913, LanguageHelper.GetTextContent(itemData.name_id), auctionItem.Count.ToString());
                    }
                }
                else
                {
                    DebugUtil.Log(ELogType.eNone, $"CSVFamilyAuction.Data is not find id = {auctionItem.InfoId}");
                }

                layout.numberInput.SetInputData(familyAwardData.InitialSingleStep * auctionItem.Count, 0, 11912, auctionItem.Unowned ? auctionItem.Price : (auctionItem.Price + familyAwardData.InitialSingleStep * auctionItem.Count));
            }
        }

        protected override void OnHide()
        {
        }

        protected override void OnDestroy()
        {
        }

        private void CloseThisUI()
        {
            UIManager.CloseUI(EUIID.UI_FamilySale_Detail);
        }

        public void CloseBtnClicked()
        {
            UIManager.HitButton(EUIID.UI_FamilySale_Detail, "CloseBtnClicked");
            CloseThisUI();
        }

        public void OnCancelBtnClicked()
        {
            UIManager.HitButton(EUIID.UI_FamilySale_Detail, "OnCancelBtnClicked");
            CloseThisUI();
        }

        public void OncomfireBtnClicked()
        {
            UIManager.HitButton(EUIID.UI_FamilySale_Detail, "OncomfireBtnClicked");
            if (Sys_Family.Instance.familyData.familyAuctionInfo.GetAuctionIsEnd(activeId))
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11916)); //当前拍卖已结束，请下次再来
                return;
            }
            GuildAuction.Types.AuctionItem auctionItem = Sys_Family.Instance.familyData.familyAuctionInfo.GetAuctionItem(activeId, uid);
            if (null != auctionItem)
            {
                //if (Sys_Family.Instance.familyData.familyAuctionInfo.IsMyAuctionItem(activeId, uid))
                //{
                //    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11918)); //您无法对已出价的物品重复出价
                //}
                //else
                //{

                long myPrice = layout.numberInput.GetNum();
                CSVFamilyAuction.Data awardData = CSVFamilyAuction.Instance.GetConfData(auctionItem.InfoId);
                ItemIdCount item = new ItemIdCount(awardData.AuctionCurrency, myPrice);
                if (item.Enough)
                {
                    if ((auctionItem.Unowned && myPrice >= auctionItem.Price) || (!auctionItem.Unowned && myPrice > auctionItem.Price))
                    {
                        CSVItem.Data configItem = CSVItem.Instance.GetConfData(awardData.ItemId);
                        if (null != configItem)
                        {
                            string langStr = LanguageHelper.GetTextContent(11919, LanguageHelper.GetTextContent(configItem.name_id), (auctionItem.Count).ToString(), LanguageHelper.GetTextContent(item.CSV.name_id), myPrice.ToString());
                            PromptBoxParameter.Instance.OpenPromptBox(langStr, 0, () =>
                            {
                                Sys_Family.Instance.GuildAuctionBidReq(activeId, uid, auctionItem.InfoId, (uint)myPrice);
                                CloseThisUI();

                            }, null);
                        }
                    }
                    else
                    {
                        Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11915)); //已有其他出价高于当前价格，请重新出价
                        RefreshView();
                    }
                }
                else
                {
                    Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11907, LanguageHelper.GetTextContent(item.CSV.name_id))); //xxx不足
                    RefreshView();
                }
                // }
            }
            else
            {
                Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11917)); //该道具已被其他玩家购买，请重新选择拍卖道具
            }
        }
    }
}