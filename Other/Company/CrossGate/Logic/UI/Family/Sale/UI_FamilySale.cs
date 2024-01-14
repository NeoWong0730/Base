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

    public class UI_FamilySale_TypeListSub
    {
        private Transform transform;
        private CP_ToggleEx parToggle;
        private Text textLight;
        private Text textDark;
        public CSVFamilyAuctionAct.Data typeSubData;
        private Action<UI_FamilySale_TypeListSub> action;
        public void Bind(GameObject go)
        {
            transform = go.transform;
            parToggle = go.GetComponent<CP_ToggleEx>();
            parToggle?.onValueChanged.AddListener(OnToggleClick);
            textLight = transform.Find("Image_Select/Text_Light").GetComponent<Text>();
            textDark = transform.Find("Text_Dark").GetComponent<Text>();
        }

        private void OnToggleClick(bool isOn)
        {
            if (isOn)
            {
                parToggle.SetSelected(true, false);
                action?.Invoke(this);
            }
        }

        public void AddListener(Action<UI_FamilySale_TypeListSub> _action)
        {
            action = _action;
        }

        public void OpenTrigger()
        {
            parToggle.SetSelected(true, false);
            action?.Invoke(this);
        }

        public void SetSub(CSVFamilyAuctionAct.Data typeSubData, bool first)
        {
            if (null != typeSubData)
            {
                this.typeSubData = typeSubData;
                uint languageId = typeSubData.subordinatelangid;
                textLight.text = LanguageHelper.GetTextContent(languageId);
                textDark.text = LanguageHelper.GetTextContent(languageId);

                if (null != parToggle && first)
                {
                    parToggle.SetSelected(true, true);
                }
            }
            else
            {
                DebugUtil.Log(ELogType.eNone, "数据空");
            }
        }

        public void HideToggle()
        {
            parToggle.SetSelected(false, false);
        }

    }

    public class UI_FamilySale_TypeListParent
    {
        private Transform transform;
        private Toggle parToggle;
        private GameObject subToggleGo;
        private Transform subTogglePar;
        private Text textLight;
        private Text textDark;
        private Image backImage;
        private Action<uint> parentAction;
        private List<UI_FamilySale_TypeListSub> subCeils = new List<UI_FamilySale_TypeListSub>();
        private Image arrowToggleGo;
        private Action<CSVFamilyAuctionAct.Data>  subAction;
        public CSVFamilyAuctionAct.Data parentData;
        public void Bind(GameObject go)
        {
            transform = go.transform;

            subToggleGo = transform.Find("Toggle_Select01").gameObject;
            subTogglePar = transform.Find("Content_Small");
            parToggle = go.GetComponent<Toggle>();
            parToggle.onValueChanged.AddListener(OnToggleClick);
            //textLight = transform.Find("GameObject/Image_Select/Text_Light").GetComponent<Text>();
            textDark = transform.Find("GameObject/Text_Dark").GetComponent<Text>();
            backImage = transform.Find("GameObject").GetComponent<Image>();
            arrowToggleGo = transform.Find("GameObject/Image_Frame").GetComponent<Image>();

        }

        private void OnToggleClick(bool isOn)
        {
            if (isOn)
            {
                parentAction?.Invoke(this.parentData.Superior);
            }
        }

        public void AddListener(Action<uint> _action, Action<CSVFamilyAuctionAct.Data>  _subAction)
        {
            parentAction = _action;
            subAction = _subAction;
        }

        public void SetSub(CSVFamilyAuctionAct.Data parentData, bool first)
        {
            this.parentData = parentData;
            uint languageId = parentData.Superiorlangid;
            //textLight.text = LanguageHelper.GetTextContent(languageId);
            textDark.text = LanguageHelper.GetTextContent(languageId);
            ImageHelper.SetIcon(backImage, parentData.SuperiorIcom);
            //ReInitChildren(first);
        }

        public void ReInitChildren(bool first)
        {
            List<CSVFamilyAuctionAct.Data>  tySubList = new List<CSVFamilyAuctionAct.Data>();
            for (int i = 0; i < Sys_Family.Instance.actDatas.Count; i++)
            {
                CSVFamilyAuctionAct.Data temp = Sys_Family.Instance.actDatas[i];
                if (temp.Superior == parentData.Superior && null != Sys_Family.Instance.familyData.familyAuctionInfo.GetActiveDicData(temp.id) && !(Sys_Family.Instance.familyData.familyAuctionInfo.GetAuctionIsEnd(temp.id)))
                //if (temp.Superior == parantData.Superior)
                {
                    tySubList.Add(temp);
                }
            }
            subCeils.Clear();
            FrameworkTool.DestroyChildren(subTogglePar.gameObject);
            for (int i = 0; i < tySubList.Count; i++)
            {
                UI_FamilySale_TypeListSub subCeil = new UI_FamilySale_TypeListSub();
                GameObject go = GameObject.Instantiate<GameObject>(subToggleGo, subTogglePar);
                subCeil.Bind(go);
                subCeil.AddListener(SubClicked);
                subCeil.SetSub(tySubList[i], first && i == 0);
                subCeils.Add(subCeil);
                go.SetActive(true);
            }
        }

        public void SubClicked(UI_FamilySale_TypeListSub typeSub)
        {
            if (null != typeSub.typeSubData)
            {
                subAction?.Invoke(typeSub.typeSubData);
            }
            else
            {
                DebugUtil.Log(ELogType.eNone, "CSVFamilySaleMain.Data is null");
            }
        }

        bool isShow = false;
        public void SetParState(uint _parId, bool isInit)
        {
            if (parentData.id == _parId && !isShow)
            {
                isShow = true;
                subTogglePar.gameObject.SetActive(true);
                if(subCeils.Count > 0)
                {
                    subCeils[0].OpenTrigger();
                }
                else
                {
                    subAction?.Invoke(null);
                }
            }
            else if (!isInit || parentData.id != _parId)
            {
                isShow = false;
                subTogglePar.gameObject.SetActive(false);
            }
            SetArrow(_parId == parentData.id && subTogglePar.gameObject.activeSelf);
        }

        public void HideAllSub()
        {
            for (int i = 0; i < subCeils.Count; i++)
            {
                subCeils[i].HideToggle();
            }
        }

        private void SetArrow(bool select)
        {
            float rotateZ = select ? 0f : 90f;
            arrowToggleGo.rectTransform.localRotation = Quaternion.Euler(0f, 0f, rotateZ);
        }

        public void InitToggle()
        {
            if (parToggle.isOn)
            {
                parentAction?.Invoke(this.parentData.id);
                parToggle.isOn = true;
            }
            else
            {
                parToggle.isOn = true;
            }
            SetArrow(true);
        }

    }

    public class UI_FamilySale_TypeList
    {
        private Transform transform;
        private bool listInit = false;
        private GameObject toggleSelectGo;
        private Transform parentGo;
        private List<UI_FamilySale_TypeListParent> parentCeils = new List<UI_FamilySale_TypeListParent>();
        List<CSVFamilyAuctionAct.Data>  typeList = new List<CSVFamilyAuctionAct.Data>();
        public uint showId;
        private IListener listener;
        public void Init(Transform transform)
        {
            this.transform = transform;
            toggleSelectGo = transform.Find("Scroll01/Toggle_Select01").gameObject;
            parentGo = transform.Find("Scroll01/Content");
        }

        public void Close()
        {
            for (int i = 0; i < parentCeils.Count; i++)
            {
                parentCeils[i].HideAllSub();
            }
        }

        public void Show()
        {
            if (!listInit)
            {
                parentCeils.Clear();
                typeList = new List<CSVFamilyAuctionAct.Data>();
                List<uint> superiorIds = new List<uint>();
                for (int i = 0; i < Sys_Family.Instance.actDatas.Count; i++)
                {
                    CSVFamilyAuctionAct.Data temp = Sys_Family.Instance.actDatas[i];
                    if (!superiorIds.Contains(temp.Superior))
                    {
                        superiorIds.Add(temp.Superior);
                        typeList.Add(temp);
                    }
                }

                for (int i = 0; i < typeList.Count; i++)
                {
                    UI_FamilySale_TypeListParent parentCeil = new UI_FamilySale_TypeListParent();
                    GameObject go = GameObject.Instantiate<GameObject>(toggleSelectGo, parentGo);
                    parentCeil.Bind(go);
                    parentCeil.AddListener(OnClickPar, OnClickSub);
                    parentCeil.SetSub(typeList[i], i == 0 ? true : false);
                    parentCeils.Add(parentCeil);
                    go.SetActive(true);
                }
                listInit = true;
                showId = superiorIds[0];
                ResetList(true);
            }
            else
            {
                ResetList(true);
            }
            FrameworkTool.ForceRebuildLayout(transform.gameObject);
        }

        private void OnClickPar(uint id)
        {
            showId = id;
            ResetList(false);
        }

        private void OnClickSub(CSVFamilyAuctionAct.Data subTypeData)
        {
            DebugUtil.Log(ELogType.eFamilyAuction, "SubEventTrigger1");
            listener?.OnSubClick(subTypeData);
        }

        private void ResetList(bool setInit)
        {
            for (int i = 0; i < parentCeils.Count; i++)
            {
                UI_FamilySale_TypeListParent item = parentCeils[i];
                item.ReInitChildren(setInit);
                item.SetParState(showId, setInit);
            }
            FrameworkTool.ForceRebuildLayout(transform.gameObject);
        }

        public void RegisterListener(IListener _listener)
        {
            listener = _listener;
        }

        public interface IListener
        {
            void OnSubClick(CSVFamilyAuctionAct.Data subTypeData);
        }
    }

    public class UI_FamilySale_Layout
    {
        private Button closeBtn;
        private Button mySelfSaleBtn;
        private Button saleRecordBtn;
        /// <summary> 无限滚动 </summary>
        private InfinityGrid infinityGrid;
        private Transform bounsInfoTransfom;
        public Transform parTran;
        private Transform noneGo;
        private FamilySaleCoin willGetCoin;
        public void Init(Transform transform)
        {
            infinityGrid = transform.Find("Animator/View_Society/View_Right/Scroll_Rank").GetComponent<InfinityGrid>();
            parTran = transform.Find("Animator/View_Society/View_Left");
            closeBtn = transform.Find("Animator/TtileAndBg/Btn_Close").GetComponent<Button>();
            mySelfSaleBtn = transform.Find("Animator/View_Society/View_Below/Button_Record").GetComponent<Button>();
            saleRecordBtn = transform.Find("Animator/View_Society/View_Below/Button_Sale").GetComponent<Button>();
            willGetCoin = new FamilySaleCoin();
            willGetCoin.Init(transform.Find("Animator/View_Society/View_Below/Image_Coin"));
            bounsInfoTransfom = transform.Find("Animator/View_Society/View_Below/Text_Tips");
            noneGo = transform.Find("Animator/View_Society/View_None");
        }

        public void SetInfinityGridCell(int count)
        {
            infinityGrid.CellCount = count;
            infinityGrid.ForceRefreshActiveCell();
            noneGo.gameObject.SetActive(count == 0);
        }

        public void SetSaleGetCoin(long num)
        {
            willGetCoin.SetCoin(2, num);
        }

        public void RegisterEvents(IListener listener)
        {
            closeBtn.onClick.AddListener(listener.CloseBtnClicked);
            mySelfSaleBtn.onClick.AddListener(listener.MySelfSaleBtnClicked);
            saleRecordBtn.onClick.AddListener(listener.SaleRecordBtnClicked);
            infinityGrid.onCreateCell += listener.OnCreateCell;
            infinityGrid.onCellChange += listener.OnCellChange;
        }

        public void SetBonusState(bool state)
        {
            bounsInfoTransfom.gameObject.SetActive(state);
            willGetCoin.SetState(state);
        }

        public void SetMyBtnState(bool state)
        {
            mySelfSaleBtn.gameObject.SetActive(state);
        }

        public interface IListener
        {
            void CloseBtnClicked();
            void MySelfSaleBtnClicked();
            void SaleRecordBtnClicked();
            void OnCreateCell(InfinityGridCell cell);
            void OnCellChange(InfinityGridCell cell, int index);
        }
    }

    public class FamilySaleCoin
    {
        public Image coinImage;
        public Text coinText;
        private long num;

        public virtual void Init(Transform transform)
        {
            coinImage = transform.GetComponent<Image>();
            coinText = transform.Find("Text_Add").GetComponent<Text>();
        }

        public void SetCoin(uint id, long num, uint styleId = 132)
        {
            CSVItem.Data itemData = CSVItem.Instance.GetConfData(id);
            ImageHelper.SetIcon(coinImage, itemData.small_icon_id);
            RefreshNum(num, styleId);
        }

        public void RefreshNum(long num, uint styleId = 132)
        {
            this.num = num;
            CSVWordStyle.Instance.GetConfData(styleId);
            TextHelper.SetText(coinText, num.ToString(), CSVWordStyle.Instance.GetConfData(styleId));
        }

        public long GetNum()
        {
            return num;
        }

        public void SetState(bool state)
        {
            coinImage.gameObject.SetActive(state);
        }
    }

    public class UI_FamilySaleCeil
    {
        private PropItem item;
        private FamilySaleCoin saleCoin;
        private FamilySaleCoin oneBuyCoin;
        private Button saleBtn;
        private Button buyBtn;
        private Text timeText;
        private Timer limitTime;
        private GuildAuction.Types.AuctionItem itemData;
        private uint auctionId;
        public void Init(Transform transform)
        {
            item = new PropItem();
            item.BindGameObject(transform.Find("ListItem").gameObject);
            saleCoin = new FamilySaleCoin();
            saleCoin.Init(transform.Find("Present/Image_Coin"));
            saleBtn = transform.Find("Present/Btn_Again").GetComponent<Button>();
            saleBtn.onClick.AddListener(OnSaleBtnClick);
            oneBuyCoin = new FamilySaleCoin();
            oneBuyCoin.Init(transform.Find("One/Image_Coin"));
            buyBtn = transform.Find("One/Btn_Again").GetComponent<Button>();
            buyBtn.onClick.AddListener(OnBuyBtnClick);
            timeText = transform.Find("Text_Time").GetComponent<Text>();
        }

        public void SetInfo(uint auctionId, GuildAuction.Types.AuctionItem data)
        {
            this.auctionId = auctionId;
            itemData = data;
            CSVFamilyAuction.Data awardData = CSVFamilyAuction.Instance.GetConfData(data.InfoId);
            if(null != awardData)
            {
                PropIconLoader.ShowItemData itemN = new PropIconLoader.ShowItemData(awardData.ItemId, data.Count, false, false, false, false, false, true, false, true);
                item.SetData(itemN, EUIID.UI_FamilySale);
                CSVItem.Data itemData = CSVItem.Instance.GetConfData(awardData.ItemId);
                bool hasItem = null != itemData;
                if (hasItem)
                {
                    TextHelper.SetText(item.txtName, itemData.name_id);
                }

                uint style = 132;
                if(!data.Unowned)
                {
                    var myInfoData = Sys_Family.Instance.familyData.familyAuctionInfo.GetMyActiveAuctionDicData();
                    for (int i = 0; i < myInfoData.Count; i++)
                    {
                        var info = myInfoData[i];
                        if (info.Id == data.Id)
                        {
                            style = info.Owned ? 74u : 75u;
                        }   
                    }
                }
                
                item.txtName.gameObject.SetActive(hasItem);
                saleCoin.SetCoin(awardData.AuctionCurrency, data.Price, style);
                oneBuyCoin.SetCoin(awardData.FixedCurrencyType, awardData.FixedCurrency * data.Count);
                limitTime?.Cancel();
                limitTime = Timer.Register(1, null, (t) => {
                    long time = (long)Sys_Family.Instance.familyData.familyAuctionInfo.GetAuctionEndTime(auctionId) - (long)Sys_Time.Instance.GetServerTime();
                    if (time >= 0)
                    {
                        timeText.gameObject.SetActive(true);
                        timeText.text = LanguageHelper.TimeToString((uint)time, LanguageHelper.TimeFormat.Type_1);
                    }
                    else
                    {
                        timeText.text = "";
                        limitTime?.Cancel();
                    }
                }, true);
            }
        }

        public void OnHide()
        {
            limitTime?.Cancel();
        }

        private void OnSaleBtnClick()
        {
            UIManager.HitButton(EUIID.UI_FamilySale, "OnSaleBtnClick");
            if (null != itemData)
            {
                UIManager.OpenUI(EUIID.UI_FamilySale_Detail, false, new Tuple<uint, uint>(auctionId, itemData.Id));
            }
        }

        private void OnBuyBtnClick()
        {
            UIManager.HitButton(EUIID.UI_FamilySale, "OnBuyBtnClick");
            if (null != itemData)
            {
                
                CSVFamilyAuction.Data awardData = CSVFamilyAuction.Instance.GetConfData(itemData.InfoId);
                if(null != awardData)
                {
                    CSVItem.Data auctionItem = CSVItem.Instance.GetConfData(awardData.ItemId);
                    if (null != auctionItem)
                    {
                        ItemIdCount item = new ItemIdCount(awardData.FixedCurrencyType, awardData.FixedCurrency * itemData.Count);
                        if (item.Enough)
                        {
                            string langStr = LanguageHelper.GetTextContent(11909, LanguageHelper.GetTextContent(auctionItem.name_id), itemData.Count.ToString(), LanguageHelper.GetTextContent(item.CSV.name_id), (awardData.FixedCurrency * itemData.Count).ToString());
                            PromptBoxParameter.Instance.OpenPromptBox(langStr, 0, () =>
                            {
                                Sys_Family.Instance.GuildAuctionOnePriceReq(auctionId, itemData.Id, itemData.InfoId);

                            }, null);
                        }
                        else
                        {
                            Sys_Hint.Instance.PushContent_Normal(LanguageHelper.GetTextContent(11907, LanguageHelper.GetTextContent(item.CSV.name_id))); //xxx不足
                        }
                    }
                    else
                    {
                        DebugUtil.Log(ELogType.eNone, $"Not Find id = {awardData.ItemId} in CSVItemData");
                    }
                }
                else
                {
                    DebugUtil.Log(ELogType.eNone, $"Not Find id = {itemData.InfoId} in CSVFamilyAuctionData");
                }
            }
        }
    }

    public class UI_FamilySale : UIBase, UI_FamilySale_Layout.IListener, UI_FamilySale_TypeList.IListener
    {
        private UI_FamilySale_Layout layout = new UI_FamilySale_Layout();
        private UI_FamilySale_TypeList typeList;
        private GuildAuction auctionData;
        private List<UI_FamilySaleCeil> ceilList = new List<UI_FamilySaleCeil>();
        protected override void OnLoaded()
        {
            layout.Init(transform);
            layout.RegisterEvents(this);
            typeList = new UI_FamilySale_TypeList();
            typeList.Init(layout.parTran);
            typeList.RegisterListener(this);
        }

        protected override void ProcessEventsForEnable(bool toRegister)
        {
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.OnAuctionAckEnd, RefreshItemView, toRegister);
            Sys_Family.Instance.eventEmitter.Handle(Sys_Family.EEvents.OnAuctionMyInfoAckEnd, RefreshMyButton, toRegister);
            Sys_Family.Instance.eventEmitter.Handle<uint>(Sys_Family.EEvents.OnAuctionItemChange, RefreshItemView, toRegister);
            Sys_Family.Instance.eventEmitter.Handle<uint, uint>(Sys_Family.EEvents.OnAuctionItemRemove, RefreshItemView, toRegister);
        }

        protected override void OnOpen(object arg = null)
        {
            
        }

        protected override void OnShow()
        {
            Sys_Family.Instance.GuildAuctionListReq();
            //Sys_Family.Instance.GuildAuctionMyInfoReq();
            Sys_Family.Instance.needShowAuctionRedPoint = false;
            RedPointElement.eventEmitter.Trigger<object[]>(RedPointElement.EEvents.OnFamilyActiveRedPoint, null);
            layout.SetBonusState(false);
            RefreshMyButton();
            layout.SetInfinityGridCell(0);
            Sys_Family.Instance.GuildAuctionWatchReq(true);
        }

        private void RefreshMyButton()
        {
            layout.SetMyBtnState(Sys_Family.Instance.familyData.familyAuctionInfo.GetMyActiveAuctionDicData().Count > 0);
        }

        private void RefreshItemView()
        {
            typeList.Show();
            if (null != auctionData)
            {
                auctionData = Sys_Family.Instance.familyData.familyAuctionInfo.GetActiveDicData(auctionData.ActiveId);
            }
            SetViewInfo();
        }

        private void RefreshItemView(uint activeId)
        {
            if (null != auctionData)
            {
                auctionData = Sys_Family.Instance.familyData.familyAuctionInfo.GetActiveDicData(auctionData.ActiveId);
            }
            SetViewInfo();
        }

        private void RefreshItemView(uint activeId,uint itemId)
        {
            if (null != auctionData)
            {
                auctionData = Sys_Family.Instance.familyData.familyAuctionInfo.GetActiveDicData(auctionData.ActiveId);
            }
            SetViewInfo();
        }

        protected override void OnHide()
        {
        }

        protected override void OnClose()
        {
            typeList.Close();
            for (int i = 0; i < ceilList.Count; i++)
            {
                ceilList[i].OnHide();
            }
            Sys_Family.Instance.GuildAuctionWatchReq(false);
        }

        protected override void OnDestroy()
        {

        }

        /// <summary>
        /// 滚动列表创建回调
        /// </summary>
        /// <param name="cell"></param>
        public void OnCreateCell(InfinityGridCell cell)
        {
            UI_FamilySaleCeil entry = new UI_FamilySaleCeil();
            GameObject go = cell.mRootTransform.gameObject;
            entry.Init(go.transform);
            cell.BindUserData(entry);
            ceilList.Add(entry);
        }

        /// <summary>
        /// 滚动列表滚动回调
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="index"></param>
        public void OnCellChange(InfinityGridCell cell, int index)
        {
            UI_FamilySaleCeil entry = cell.mUserData as UI_FamilySaleCeil;
            if (index < 0 || index >= auctionData.Items.Count)
            {
                return;
            }
            entry.SetInfo(auctionData.ActiveId, auctionData.Items[index]);
        }

        public void CloseBtnClicked()
        {
            UIManager.HitButton(EUIID.UI_FamilySale, "CloseBtnClicked");
            UIManager.CloseUI(EUIID.UI_FamilySale);
        }

        private void SetViewInfo()
        {
            if (null != auctionData)
            {
                bool hasBouns = auctionData.Bonus != 0;
                if (hasBouns)
                {
                    layout.SetSaleGetCoin(auctionData.Bonus);
                }
                layout.SetBonusState(hasBouns);
                layout.SetInfinityGridCell(auctionData.Items.Count);
            }
            else
            {
                layout.SetBonusState(false);
                layout.SetInfinityGridCell(0);
            }
            RefreshMyButton();
        }

        public void MySelfSaleBtnClicked()
        {
            UIManager.HitButton(EUIID.UI_FamilySale, "MySelfSaleBtnClicked");
            if (null != auctionData)
                UIManager.OpenUI(EUIID.UI_FamilySale_My);
        }

        public void SaleRecordBtnClicked()
        {
            UIManager.HitButton(EUIID.UI_FamilySale, "SaleRecordBtnClicked");
            UIManager.OpenUI(EUIID.UI_FamilySale_Record);
        }

        public void OnSubClick(CSVFamilyAuctionAct.Data subTypeData)
        {
            if (subTypeData == null)
            {
                auctionData = null;
            }
            else
            {
                UIManager.HitButton(EUIID.UI_FamilySale, "OnSubClick:" + subTypeData.id.ToString());
                auctionData = Sys_Family.Instance.familyData.familyAuctionInfo.GetActiveDicData(subTypeData.id);
            }
            SetViewInfo();
        }
    }
}